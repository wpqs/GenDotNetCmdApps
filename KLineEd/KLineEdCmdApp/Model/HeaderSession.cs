using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using KLineEdCmdApp.Model.Base;
using KLineEdCmdApp.Utils;
using Microsoft.WindowsAzure.Storage.Blob;
using MxDotNetUtilsLib;
using MxReturnCode;

namespace KLineEdCmdApp.Model
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public class HeaderSession : HeaderBase
    {
        //Session: 8
        //Start: 23-06-19 12:10:00 Duration: 02:13:34 Typing time: 02.01.00
        //Start line: 210 End line: 350 Lines typed: 140 
        //Words: 3500 WPM: 31.9
        //Corrections count: 970 WPC: 3.6 
        //Spell check count: 350 WPS: 10.0 
        //Typing pauses: 12:30:45,120; 14:30:01,630; ...

        //session calculations
        //  - Lines typed
        //  - Words per minute (WPM) 
        //  - Mean number of words per correction (WPC) (BS, Del) successive Del/BS are counted as just one 
        //  - total typing pauses
        //  - duration less pauses
        //  - total spell check corrections
        //  - Mean number of words per spell (WPS) check correction 

        //Project calculations - static obtained from passed collection of SessionData objects
        //  - file start date
        //  - total days
        //  - total time mm
        //  - total time hh:mm
        //  - total pauses hh:mm
        //  - total time less pauses
        //  - total lines typed vs. lines in body
        //  - total words typed vs. words in body
        //  - Actual WPM = total words typed / total time mm
        //  - Mean WPM 
        //  - Mean number of words per correction
        //  - Mean number of words per spell check correction

        public static readonly string OpeningElement = "<session>";
        public static readonly string ClosingElement = "</session>";

        public static readonly int PauseRecordsPerLine = 5;

        public static readonly string MxStdFrmtTimeSpan = "hh\\:mm\\:ss";

        public static readonly string DefaultDuration = "00:00:00";

        public static readonly string SessionNoLabel = "Session:";
        public static readonly string StartLabel = "Start:";
        public static readonly string DurationLabel = $" Duration:";
        public static readonly string TypingTimeLabel = $" Typing time:";
        public static readonly string StartLineLabel = $"Start line:";
        public static readonly string EndLineLabel = $" End line:";
        public static readonly string LinesTypedLabel = $" Lines typed:";
        public static readonly string WordsCountLabel = $"Words:";
        public static readonly string WpmLabel = $" WPM:";
        public static readonly string CorrectionsCountLabel = $"Corrections count:";
        public static readonly string WpcLabel = $" WPC:";
        public static readonly string SpellCheckCountLabel = $"Spell check count:";
        public static readonly string WpsLabel = $" WPS:";
        public static readonly string TypingPausesLabel = $"Typing pauses:";
        public static readonly string ProperiesEndLabel = $"[end]";

        public bool PauseState { get; private set; }
        public int SessionNo { get; private set; }
        public DateTime? Start { get; private set; }       
        public TimeSpan? Duration { get; private set; }     
        public TimeSpan? TypingTime { get; private set; }  
        public int StartLine { get; private set; }   
        public int EndLine { get; private set; }     
        public int LinesTyped { get; private set; }   
        public int WordsCount { get; private set; }  
        public double Wpm { get; private set; }        
        public int CorrectionsCount { get; private set; }   
        public double Wpc { get; private set; }      
        public int SpellCheckCount { get; private set; }   
        public double Wps { get; private set; }
        public int TypingPauseCount { get; private set; }
        private List<HeaderSessionPause> TypingPauses { get; set; }  
        
        public HeaderSession()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Reset();
        }

        public static MxReturnCode<HeaderSession> Create(StreamReader file, string closingElementSessions) 
        {
            var rc = new MxReturnCode<HeaderSession>("HeaderSession.Create");

            var sessionString = "";
            string line = null;
            while ((line = file.ReadLine()) != null)
            {
                if ((line == ClosingElement) || (line == closingElementSessions))
                    break;
                sessionString += line + Environment.NewLine;  //allow for multiline SessionPause records
            }
            if (string.IsNullOrWhiteSpace(sessionString))
                rc.SetResult(null);     //no session found, but </sessions> read
            else
            {
                var session = new HeaderSession();
                var rcInit = session.InitialiseFromString(sessionString);
                rc += rcInit;
                if (rcInit.IsSuccess(true))
                {
                    rc.SetResult(session);
                }
            }
            return rc;
        }

        public bool SetDefaults(int sessionNo = 1, int startLine = 1)   //typically called when starting a new session
        {
            bool rc = false;

            if (SetSessionNo(sessionNo) && SetStartTime(DateTime.UtcNow))
            {
                if (SetDuration(DefaultDuration) && SetStartLineLinesTyped(startLine)
                                                 && SetEndLineLinesTyped(startLine) && SetWordsCountWpm("0")
                                                 && SetCorrectionsCountWpc("0") && SetSpellCheckCountWps("0")
                                                 && SetTypingPausesTypingTime(null).GetResult())
                {
                    rc = true;
                }
            }
            return rc;
        }

        public MxReturnCode<bool> Write(StreamWriter file)
        {
            var rc = new MxReturnCode<bool>("HeaderSession.Write");

            if ((file == null) || (IsError() == true))
                rc.SetError(1060101, MxError.Source.Param, "file is null or not initialized", "MxErrBadMethodParam");
            else
            {
                try
                {
                    file.WriteLine(OpeningElement);

                    file.WriteLine(ToString());

                    file.WriteLine(ClosingElement);

                    rc.SetResult(true);
                }
                catch (Exception e)
                {
                    rc.SetError(1060102, MxError.Source.Exception, e.Message, "MxErrInvalidCondition");
                }
            }

            return rc;
        }
        public MxReturnCode<bool> Read(StreamReader file)
        {
            var rc = new MxReturnCode<bool>("HeaderSession.Read");

            if (file == null)
                rc.SetError(1060201, MxError.Source.Param, "file is null", "MxErrBadMethodParam");
            else
            {
                try
                {
                    var firstLine = file.ReadLine();
                    if (firstLine != OpeningElement)
                        rc.SetError(1060202, MxError.Source.Data, $"first line is {firstLine}", "MxErrInvalidCondition");
                    else
                    {
                        var headerString = file.ReadLine();
                        if ((headerString == null) || (headerString != SessionNoLabel))
                            rc.SetError(1060203, MxError.Source.Data, $"{SessionNoLabel} not at start of {firstLine}", "MxErrInvalidCondition");
                        else
                        {
                            while (headerString.EndsWith(ProperiesEndLabel) == false)
                            {
                                var line = file.ReadLine();
                                if ((line == null) || (line.EndsWith(HeaderSessionPause.Terminator) == false))
                                {
                                    rc.SetError(1060204, MxError.Source.Data, $"{ProperiesEndLabel} not found or {line} does not end with '{HeaderSessionPause.Terminator}'", "MxErrInvalidCondition");
                                    break;
                                }
                                headerString += line + Environment.NewLine;
                            }
                            if (rc.IsSuccess())
                            {
                                InitialiseFromString(headerString); //line can only be 1024 - see EditFile.Create default buffer size

                                var lastLine = file.ReadLine();
                                if (lastLine != ClosingElement)
                                    rc.SetError(1060205, MxError.Source.Data, $"last line is {lastLine}", "MxErrInvalidCondition");
                                else
                                {
                                    rc.SetResult(true);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1060206, MxError.Source.Exception, e.Message, "MxErrInvalidCondition");
                }
            }
            return rc;
        }

        public override bool Validate()
        {
            bool rc = (SessionNo != Program.PosIntegerNotSet) && (Start != null) && (Duration != null) 
                        && (StartLine != Program.PosIntegerNotSet) && (EndLine != Program.PosIntegerNotSet) && (LinesTyped != Program.PosIntegerNotSet)
                        && (WordsCount != Program.PosIntegerNotSet) && (double.IsNaN(Wpm) == false)
                        && (CorrectionsCount != Program.PosIntegerNotSet) && (double.IsNaN(Wpc) == false)
                        && (SpellCheckCount != Program.PosIntegerNotSet) && (double.IsNaN(Wps) == false)
                        && (TypingPauses != null) && (TypingTime != null) && (TypingPauseCount != Program.PosIntegerNotSet);
            Error = !rc;  //set Error = false if all properties are now valid, else Error = true;
            return rc;
        }
        public override bool IsLabelFound(string name)
        {
            // ReSharper disable once ReplaceWithSingleAssignment.False
            bool rc = false;

            if (name != null)
            {
                if ((name.Contains(SessionNoLabel)) || (name.Contains(StartLabel)) || (name.Contains(DurationLabel))
                    || (name.Contains(TypingTimeLabel)) || (name.Contains(StartLineLabel)) || (name.Contains(EndLineLabel)) 
                    || (name.Contains(LinesTypedLabel)) || (name.Contains(WordsCountLabel)) || (name.Contains(WpmLabel))
                    || (name.Contains(CorrectionsCountLabel)) || (name.Contains(WpcLabel)) || (name.Contains(SpellCheckCountLabel))
                    || (name.Contains(WpsLabel)) || (name.Contains(TypingPausesLabel)) || (name.Contains(ProperiesEndLabel)))
                    rc = true;
            }
            return rc;
        }
        public override void Reset()
        {
            SessionNo = Program.PosIntegerNotSet;
            Start = null;
            Duration = null;
            StartLine = Program.PosIntegerNotSet;
            EndLine = Program.PosIntegerNotSet;
            LinesTyped = Program.PosIntegerNotSet;     //calculated value
            WordsCount = Program.PosIntegerNotSet;
            Wpm = double.NaN;
            CorrectionsCount = Program.PosIntegerNotSet;
            Wpc = double.NaN;
            SpellCheckCount = Program.PosIntegerNotSet;
            Wps = double.NaN;

            TypingTime = null;
            TypingPauses = null;
            TypingPauseCount = Program.PosIntegerNotSet;
            PauseState = false;

            Error = true;
        }

        //Session: 4
        //Start: 23-06-19 12:10:00 Duration: 02:13:34 Typing pauses: 2 Typing time: 02.01.00
        //Start line: 210 End line: 350 Lines typed: 140 
        //Words: 3500 WPM: 31.9
        //Corrections count: 970 WPC: 3.6 
        //Spell check count: 350 WPS: 10.0 

        public override string GetReport()
        {
            var rc = HeaderBase.ValueNotSet;
            if ((IsError() == false) && (GetTypingPausesString() != null))
            {                                       //ToString(MxStdFrmt.DateTime) appends a space, but ToString(MxStdFrmtTimeSpan) doesn't
                rc = "Last session stats:";
                rc += Environment.NewLine;
                rc += Environment.NewLine;
                rc += $"{SessionNoLabel} {SessionNo} {StartLineLabel} {StartLine} {EndLineLabel} {EndLine} {LinesTypedLabel} {LinesTyped}";
                rc += Environment.NewLine;
                rc += $"{StartLabel} {Start?.ToString(MxStdFrmt.DateTime) ?? "[null]"}{DurationLabel} {Duration?.ToString(MxStdFrmtTimeSpan) ?? "[null]"} ";
                rc += $"{TypingPausesLabel} {TypingPauseCount} {TypingTimeLabel} {TypingTime?.ToString(MxStdFrmtTimeSpan) ?? "[null]"} ";
                rc += Environment.NewLine;
                rc += $"{WordsCountLabel} {WordsCount} {WpmLabel} {Wpm:F1}";
                //rc += Environment.NewLine;
                //rc += $"{CorrectionsCountLabel} {CorrectionsCount} {WpcLabel} {Wpc:F1}";
                //rc += Environment.NewLine;
                //rc += $"{SpellCheckCountLabel} {SpellCheckCount} {WpsLabel} {Wps:F1}";
                rc += $"{KLineEditor.ReportSectionDottedLine}";
                rc += GetAssessment();
                rc += Environment.NewLine;
            }
            return rc;
        }


        //Session: 4 Start: 23-06-19 12:10:00 Duration: 02:13:34 Start line: 210 End line: 350 Words: 3500 Corrections count: 970 Spell check count: 350 Typing pauses: 12:30:45,120;14:30:01,630;[end]

        public override string ToString()
        {
            var rc = HeaderBase.ValueNotSet;
            if (IsError() == false)
            {            //order must be same as InitialiseFromString()
                         // //ToString(MxStdFrmt.DateTime) appends a space, but ToString(MxStdFrmtTimeSpan) doesn't
                rc =  $"{SessionNoLabel} {SessionNo} ";
                rc += $"{StartLabel} {Start?.ToString(MxStdFrmt.DateTime) ?? "[null]"}";
                rc += $"{DurationLabel} {Duration?.ToString(MxStdFrmtTimeSpan) ?? "[null]"} ";
                rc += $"{StartLineLabel} {StartLine}";  //no space here for some reason
                rc += $"{EndLineLabel} {EndLine} ";
                rc += $"{WordsCountLabel} {WordsCount} ";
                rc += $"{CorrectionsCountLabel} {CorrectionsCount} ";
                rc += $"{SpellCheckCountLabel} {SpellCheckCount} ";
                rc += $"{TypingPausesLabel} {GetTypingPausesString()}";
                rc += $"{ProperiesEndLabel}";
            }
            return rc;
        }
        public override MxReturnCode<bool> InitialiseFromString(string toString)
        {
            var rc = new MxReturnCode<bool>("HeaderSession.InitialiseFromString");

            Reset();

            if (toString != null)
            {                //order must be same as ToString()
                if (SetSessionNo(Extensions.GetPropertyFromString(toString, SessionNoLabel, StartLabel)) == false)
                    rc.SetError(1070101, MxError.Source.Data, $"SetSessionNo failed: toString={GetTruncatedString(toString)}", "MxErrInvalidCondition");
                else
                {
                    if (SetStartTime(Extensions.GetPropertyFromString(toString, StartLabel, DurationLabel)) == false)
                        rc.SetError(1070102, MxError.Source.Data, $"SetStartTime failed: toString={GetTruncatedString(toString)}", "MxErrInvalidCondition");
                    else
                    {
                        if (SetDuration(Extensions.GetPropertyFromString(toString, DurationLabel, StartLineLabel)) == false)
                            rc.SetError(1070103, MxError.Source.Data, $"SetDuration failed: toString={GetTruncatedString(toString)}", "MxErrInvalidCondition");
                        else
                        {
                            if (SetStartLineLinesTyped(Extensions.GetPropertyFromString(toString, StartLineLabel, EndLineLabel)) == false)
                                rc.SetError(1070104, MxError.Source.Data, $"SetStartLineLinesTyped failed: toString={GetTruncatedString(toString)}", "MxErrInvalidCondition");
                            else
                            {
                                if (SetEndLineLinesTyped(Extensions.GetPropertyFromString(toString, EndLineLabel, WordsCountLabel)) == false)
                                    rc.SetError(1070105, MxError.Source.Data, $"SetEndLineLinesTyped failed: toString={GetTruncatedString(toString)}", "MxErrInvalidCondition");
                                else
                                {
                                    if (SetWordsCountWpm(Extensions.GetPropertyFromString(toString, WordsCountLabel, CorrectionsCountLabel)) == false)
                                        rc.SetError(1070106, MxError.Source.Data, $"SetWordsCountWpm failed: toString={GetTruncatedString(toString)}", "MxErrInvalidCondition");
                                    else
                                    {
                                        if (SetCorrectionsCountWpc(Extensions.GetPropertyFromString(toString, CorrectionsCountLabel, SpellCheckCountLabel)) == false)
                                            rc.SetError(1070107, MxError.Source.Data, $"SetCorrectionsCountWpc failed: toString={GetTruncatedString(toString)}", "MxErrInvalidCondition");
                                        else
                                        {
                                            if (SetSpellCheckCountWps(Extensions.GetPropertyFromString(toString, SpellCheckCountLabel, TypingPausesLabel)) == false)
                                                rc.SetError(1070108, MxError.Source.Data, $"SetSpellCheckCountWps failed: toString={GetTruncatedString(toString)}", "MxErrInvalidCondition");
                                            else
                                            {
                                                var rcPauses = SetTypingPausesTypingTime(Extensions.GetPropertyFromString(toString.Replace(Environment.NewLine, ""), TypingPausesLabel, ProperiesEndLabel));
                                                rc += rcPauses;
                                                if (rcPauses.IsSuccess(true))
                                                {
                                                    rc.SetResult(true);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (rc.IsError(true))
                    Reset();
            }
            return rc;
        }

        public void SetPause()
        {
            PauseState = true;
        }

        public void AddPause(in DateTime utcNow, in DateTime lastKeyStroke)
        {
            var pause = new HeaderSessionPause(utcNow, (int)(utcNow-lastKeyStroke).TotalSeconds);
            TypingPauses.Add(pause);
            PauseState = false;
        }

        public string GetAssessment()
        {
            return "Great progress - keep on writing!";
        }

        public bool SetSessionNo(int num)
        {
            bool rc = SetSessionNo(num.ToString());
            return rc;
        }
        public bool SetSessionNo(string name)
        {
            SessionNo = GetPosInteger(name, SessionNo, out var rc);
            Validate();     
            return rc;
        }
        public bool SetStartTime(DateTime tim)
        {
            Start = GetDateTime(tim.ToString(MxStdFrmt.DateTime), Start, out var rc);
            Validate();
            return rc;
        }
        public bool SetStartTime(string name)
        {
            Start = GetDateTime(name, Start, out var rc); 
            Validate();    
            return rc;
        }
        public bool SetDuration(string name)
        {
            Duration = GetTimeSpan(name, Duration, out var rc);
            Validate();
            return rc;
        }
        public bool SetStartLineLinesTyped(int num)
        {
            bool rc = SetStartLineLinesTyped(num.ToString());
            return rc;
        }
        public bool SetStartLineLinesTyped(string name)
        {
            var existingStartLine = StartLine;
            StartLine = GetPosInteger(name, StartLine, out var rc);
            if (rc == true)
            {
                if (EndLine == Program.PosIntegerNotSet)
                    LinesTyped = Program.PosIntegerNotSet;
                else
                {
                    if (EndLine < StartLine)
                    {
                        StartLine = existingStartLine;
                        LinesTyped = Program.PosIntegerNotSet;
                        rc = false;
                    }
                    else
                    {
                        LinesTyped = EndLine - StartLine;
                    }
                }
            }
            Validate();     
            return rc;
        }
        public bool SetEndLineLinesTyped(int num)
        {
            bool rc = SetEndLineLinesTyped(num.ToString());
            return rc;
        }
        public bool SetEndLineLinesTyped(string name)
        {
            var existingEndLine = EndLine;
            EndLine = GetPosInteger(name, EndLine, out var rc);
            if (rc == true)
            {
                if (StartLine == Program.PosIntegerNotSet)
                    LinesTyped = Program.PosIntegerNotSet;
                else
                {
                    if (EndLine < StartLine)
                    {
                        EndLine = existingEndLine;
                        LinesTyped = Program.PosIntegerNotSet;
                        rc = false;
                    }
                    else
                    {
                        LinesTyped = EndLine - StartLine;
                    }
                }
            }
            Validate();
            return rc;
        }
        public bool SetWordsCountWpm(string name)
        {
            WordsCount = GetPosInteger(name, WordsCount, out var rc);
            if (rc == true)
            {
                if ((Duration == null) || (Duration?.TotalMinutes < 0.0))
                    Wpm = double.NaN;
                else
                {
                    if (Math.Abs(Math.Round((double) Duration?.TotalMinutes, 1)) <= 0.0)
                        Wpm = 0.0;
                    else
                        Wpm = WordsCount / (double) Duration?.TotalMinutes;
                }
            }
            Validate();
            return rc;
        }
        public bool SetCorrectionsCountWpc(string name)
        {
            CorrectionsCount = GetPosInteger(name, CorrectionsCount, out var rc);
            if (rc == true)
            {
                if (CorrectionsCount == 0)
                    Wpc = WordsCount;
                else
                    Wpc = WordsCount / (double)CorrectionsCount;
            }
            Validate();
            return rc;
        }
        public bool SetSpellCheckCountWps(string name)
        {
            SpellCheckCount = GetPosInteger(name, SpellCheckCount, out var rc);
            if (rc == true)
            {
                if (SpellCheckCount == 0)
                    Wps = WordsCount;
                else
                    Wps = WordsCount / (double)SpellCheckCount;
            }
            Validate();
            return rc;
        }

        public string GetTypingPausesString()
        {
            string rc = HeaderBase.ValueNotSet;

            if (TypingPauses != null)
            {
                rc = "";
                var recordCnt = 0;
                foreach (var pause in TypingPauses)
                {
                    if (recordCnt++ > HeaderSession.PauseRecordsPerLine)
                    {
                        recordCnt = 0;
                        rc += Environment.NewLine;
                    }
                    rc += pause.ToString();
                }
            }
            return rc;
        }
        public MxReturnCode<bool> SetTypingPausesTypingTime(string pauseList)
        {
            var rc = new MxReturnCode<bool>("HeaderSession.SetTypingPausesTypingTime");

            var existingPauses = TypingPauses;
            var existingTypingTime = TypingTime;
            if ((Duration == null) || (IsLabelFound(pauseList) == true))
                rc.SetError(1070201, MxError.Source.Param, $"Duration=null or pauseList contains a label", "MxErrBadMethodParam");
            else
            {
                if (TypingPauses != null)
                    TypingPauses.Clear();
                else
                    TypingPauses = new List<HeaderSessionPause>();

                if (string.IsNullOrWhiteSpace(pauseList))
                {           //empty pause list
                    TypingTime = new TimeSpan(0, 0, (int) (Duration?.TotalSeconds ?? 0.0));
                    TypingPauseCount = 0;
                    rc.SetResult(true);
                }
                else
                {
                    var pauses = pauseList.Split(HeaderSessionPause.Terminator, StringSplitOptions.RemoveEmptyEntries);
                    var pauseTime = 0.0;
                    foreach (var pause in pauses)
                    {
                        var typingPause = new HeaderSessionPause();
                        var rcInit = typingPause.InitialiseFromString(pause + HeaderSessionPause.Terminator);
                        if (rcInit.IsError(true) || typingPause.IsError())
                        {
                            rc.SetError(1070202, MxError.Source.Data, $"TypingPause.InitialiseFromString({pause + HeaderSessionPause.Terminator}) failed", "MxErrInvalidCondition");
                            break;
                        }
                        else
                        {
                            pauseTime += typingPause.Duration;
                            TypingPauses.Add(typingPause);
                        }
                    }
                    if (rc.IsSuccess())
                    {
                        var sessionSeconds = Duration?.TotalSeconds ?? 0.0;
                        if (sessionSeconds <= pauseTime)
                            rc.SetError(1070203, MxError.Source.Data, $"sessionSeconds={sessionSeconds} is less or equal to= pauseTime={pauseTime}", "MxErrInvalidCondition");
                        else
                        {
                            TypingTime = new TimeSpan(0, 0, (int)(sessionSeconds - pauseTime));
                            TypingPauseCount = TypingPauses.Count;
                            rc.SetResult(true);
                        }
                    }
                }
            }
            if (rc.IsError(true))
            {
                TypingPauses = existingPauses;
                TypingTime = existingTypingTime;
            }
            Validate();     
            return rc;
        }

        private string GetTruncatedString(string value)
        {
            var end = value.IndexOf(TypingPausesLabel, StringComparison.Ordinal);
            if ((end == -1) || (end-1 < 0))
                return (value.Length < 50) ? value : value.Substring(0, 50);
            else
                return value.Snip(0, end - 1);
        }
    }
}
