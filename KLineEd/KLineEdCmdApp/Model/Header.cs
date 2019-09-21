using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using KLineEdCmdApp.Model.Base;
using KLineEdCmdApp.Utils;
using MxReturnCode;

namespace KLineEdCmdApp.Model
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]

    public class Header
    {
        public static readonly string MxStdFrmtTimeSpan = "hh\\:mm\\:ss";
        public static readonly string MxStdFrmtDouble3 = "#0.000";
        public static readonly string MxStdFrmtDouble2 = "#0.00";
        public static readonly string MxStdFrmtDouble1 = "#0.0";
        public static readonly string MxStdFrmtDouble0 = "#0";

        public static readonly string OpeningElement = "<header>";
        public static readonly string ClosingElement = "</header>";

        public static readonly string SessionsOpeningElement = "<sessions>";
        public static readonly string SessionsClosingElement = "</sessions>";

        public HeaderProps Properties { private set; get; }
        private List<HeaderSession> Sessions { set; get; }

        public bool PauseState { get; private set; }
        public DateTime LastKeyPress { get; private set; }
        public int PauseWaitSeconds { private set; get; }

        public Header()
        {
            Properties = new HeaderProps();
            Sessions = new List<HeaderSession>();
            PauseState = false;
            LastKeyPress = DateTime.UtcNow;
            PauseWaitSeconds = CmdLineParamsApp.ArgTextEditorPauseWaitSecsDefault;
        }

        public bool SetDefaults(string pathFilename)
        {
            return Properties.SetDefaults(pathFilename);
        }

        public bool SetPauseWaitSeconds(int pause)
        {
            var rc = false;
            if ((pause >= CmdLineParamsApp.ArgTextEditorPauseWaitSecsMin) && (pause <= CmdLineParamsApp.ArgTextEditorPauseWaitSecsMax))
            {
                PauseWaitSeconds = pause;
                rc = true;
            }
            return rc;
        }

        public string GetPauseDetails()
        {
            var rc = "";

            if ((GetLastSession()?.SetTypingPausesTypingTime()?.GetResult() ?? false) == false)
                rc = "Pause info not available";
            else
            {
                rc = $"Pauses: {GetLastSession()?.TypingPauseCount ?? Program.PosIntegerNotSet} ";
                rc += $"({GetLastSession()?.TypingTime?.ToString(Header.MxStdFrmtTimeSpan) ?? "0.0"}) ";
                if (PauseState == true)
                {
                    rc += $"Paused: {((DateTime.UtcNow - LastKeyPress)).ToString(MxStdFrmtTimeSpan)}";
                }
            }
            return rc;
        }

        public MxReturnCode<bool> KLineEditorProcessDone(DateTime nowUtc, DateTime lastKeyPress)
        {
            var rc = new MxReturnCode<bool>("Header.KLineEditorProcessDone");

            if (PauseProcessing(nowUtc, lastKeyPress, true) == false)
                rc.SetError(1090101, MxError.Source.Program, "PauseProcessing failed", MxMsgs.MxErrInvalidCondition);
            else
            {
                var session = GetLastSession();
                if ((session == null) || (session.IsError()))
                    rc.SetError(1090102, MxError.Source.Program, "GetLastSession() returned null", MxMsgs.MxErrInvalidCondition);
                else
                {
                    if (session.SetDuration(DateTime.UtcNow) == false)
                        rc.SetError(1090103, MxError.Source.Program, "SetDuration failed", MxMsgs.MxErrInvalidCondition);
                    else
                    {
                        var rcTyping = session.SetTypingPausesTypingTime();
                        rc += rcTyping;
                        if (rcTyping.IsSuccess())
                            rc.SetResult(true);
                    }
                }
            }
            return rc;
        }

        public bool PauseProcessing(DateTime nowUtc, DateTime lastKeyPress, bool keyAvailable)
        {
            var rc = false;

            if (keyAvailable == true)
            {
                if (PauseState == false)
                    rc = true;
                else
                {
                    PauseState = false;
                    rc = GetLastSession()?.AddSessionPause(LastKeyPress) ?? false;
                }
            }
            else
            {
                LastKeyPress = lastKeyPress;
                if ((PauseState == false) && (nowUtc - LastKeyPress).TotalSeconds >= PauseWaitSeconds)
                    PauseState = true;
                rc = true;
            }
            return rc;
        }

        public MxReturnCode<bool> Validate()
        {
            var rc = new MxReturnCode<bool>("Header.Validate");

            if (Properties.Validate() == false)
                rc.SetError(1090201, MxError.Source.Data, $"Chapter is invalid={Properties}", MxMsgs.MxErrInvalidCondition);
            else
            {
                if ((PauseWaitSeconds < CmdLineParamsApp.ArgTextEditorPauseWaitSecsMin) || (PauseWaitSeconds > CmdLineParamsApp.ArgTextEditorPauseWaitSecsMax))
                    rc.SetError(1090202, MxError.Source.Data, $"PauseWaitSeconds={PauseWaitSeconds} is invalid", MxMsgs.MxErrInvalidCondition);
                else
                {
                    var sessionNo = 1;
                    foreach (var session in Sessions)
                    {
                        if (session.Validate() == false)
                        {
                            rc.SetError(1090203, MxError.Source.Data, $"Session {sessionNo} is invalid", MxMsgs.MxErrInvalidCondition);
                            break;
                        }
                        if (session.SessionNo != sessionNo)
                        {
                            rc.SetError(1090204, MxError.Source.Data, $"Session {session.SessionNo} is not in sequence, expected {sessionNo}", MxMsgs.MxErrInvalidCondition);
                            break;
                        }
                        sessionNo++;
                    }
                    if (rc.IsSuccess() && (sessionNo - 1 == Sessions.Count))
                        rc.SetResult(true);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> Write(StreamWriter file, bool newFile = false)
        {
            var rc = new MxReturnCode<bool>("Header.Write");

            if (file == null)
                rc.SetError(1090301, MxError.Source.Param, "file is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    file.WriteLine(OpeningElement);

                    Properties.Write(file, newFile);

                    file.WriteLine(SessionsOpeningElement);
                    foreach (var session in Sessions)
                    {
                        session.Write(file);
                    }

                    file.WriteLine(SessionsClosingElement);

                    file.WriteLine(ClosingElement);

                    rc.SetResult(true);
                }
                catch (Exception e)
                {
                    rc.SetError(1090302, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }
        public MxReturnCode<bool> Read(StreamReader file)
        {
            var rc = new MxReturnCode<bool>("Header.Read");

            if (file == null)
                rc.SetError(1090401, MxError.Source.Param, "file is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    var firstLine = file.ReadLine();
                    if (firstLine != OpeningElement)
                        rc.SetError(1090402, MxError.Source.User, $"Header opening line is {firstLine} not {OpeningElement}", MxMsgs.MxErrInvalidChapterFile);
                    else
                    {
                        var rcChapter = Properties.Read(file);
                        rc += rcChapter;
                        if (rcChapter.IsSuccess(true))
                        {

                            var line = file.ReadLine();
                            if (line != SessionsOpeningElement)
                                rc.SetError(1090403, MxError.Source.User, $"Sessions opening line is {line} not {SessionsOpeningElement}", MxMsgs.MxErrInvalidChapterFile);
                            else
                            {
                                HeaderSession session = null;
                                do
                                {
                                    var rcCreate = HeaderSession.Create(file, SessionsClosingElement);
                                    if (rcCreate.IsError(true))
                                    {
                                        rc += rcCreate; //only add errors to log as it might be long
                                        break;
                                    }
                                    else
                                    {
                                        session = rcCreate.GetResult();
                                        if (session == null)
                                            rc += rcCreate; //add only the last call to log
                                        else
                                            Sessions.Add(session);
                                    }

                                } while (session != null);

                                if (rc.IsSuccess())
                                {
                                    line = file.ReadLine();
                                    if (line != ClosingElement)
                                        rc.SetError(1090404, MxError.Source.User, $"Header closing line is {line} not {ClosingElement}", MxMsgs.MxErrInvalidChapterFile);
                                    else
                                    {
                                        var rcValid = Validate();
                                        rc += rcValid;
                                        if (rcValid.IsSuccess())
                                            rc.SetResult(true);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1090405, MxError.Source.Exception, e.Message, MxMsgs.MxErrInvalidChapterFile);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> CreateNewSession(int startLineNo)
        {
            var rc = new MxReturnCode<bool>("Header.CreateNewSession");

            if (startLineNo < 0 )
                rc.SetError(1090501, MxError.Source.Param, $"invalid StartLineNo={startLineNo}", MxMsgs.MxErrBadMethodParam);
            else
            {
                var session = new HeaderSession();

                var lastSessionNo = 0;
                if (Sessions.Count > 0)
                    lastSessionNo = Sessions[Sessions.Count - 1]?.SessionNo ?? Program.PosIntegerNotSet;

                if (session.SetDefaults(lastSessionNo + 1, startLineNo) == false)
                    rc.SetError(1090502, MxError.Source.Data, $"SetDefaults failed; lastSessionNo={lastSessionNo}+1, startLineNo={startLineNo}", MxMsgs.MxErrInvalidCondition);
                else
                {
                    Sessions.Add(session);
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public bool SetChapterDefaults(string pathFileName)
        {
            return Properties?.SetDefaults(pathFileName) ?? false;
        }
        public int GetSessionCount()
        {
            return Sessions?.Count ?? Program.PosIntegerNotSet;
        }
        public HeaderSession GetLastSession()
        {
            HeaderSession rc = null;
            if (Sessions.Count > 0)
                rc = Sessions[Sessions.Count - 1];
            return rc;
        }
        public string GetChapterReport(int linesInChapter=0, int wordsInChapter=0)
        {
            var rc = Properties?.GetReport() ?? (Environment.NewLine + HeaderElementBase.ValueNotSet); //reports always start with newline, but don't end with one 
            rc += KLineEditor.ReportSectionDottedLine;
            rc += SessionsTotal.GetReport(Sessions, linesInChapter, wordsInChapter);

            return rc;
        }

        public string GetLastSessionReport()
        {
            var rc = GetLastSession()?.GetReport() ?? (Environment.NewLine + HeaderElementBase.ValueNotSet); //reports always start with newline, but don't end with one 
            rc += KLineEditor.ReportSectionDottedLine;
            return rc;
        }
    }
}
