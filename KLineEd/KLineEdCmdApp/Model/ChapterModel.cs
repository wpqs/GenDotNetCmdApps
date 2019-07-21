using MxReturnCode;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using KLineEdCmdApp.Model.Base;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;
using MxDotNetUtilsLib;

namespace KLineEdCmdApp.Model
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]

    public class ChapterModel : NotifierModel
    {
        public enum ChangeHint
        {
            [EnumMember(Value = "Char")] Char = 0,      //AppendChar()
            [EnumMember(Value = "Line")] Line = 1,      //AppendLine()
            [EnumMember(Value = "Word")] Word = 2,      //AppendWord()
            [EnumMember(Value = "Props")] Props = 3,      //SetAuthor(), SetTitle(), SetProject() - change to char
            [EnumMember(Value = "Spell")] Spell = 4,      //
            [EnumMember(Value = "StatusLine")] StatusLine = 5,  //SetStatusLine()
            [EnumMember(Value = "MsgLine")] MsgLine = 6,        //SetMsgLine()
            [EnumMember(Value = "HelpLine")] HelpLine = 7,      //SetEditorHelpLine()
            [EnumMember(Value = "All")] All = 8,        //RefreshDisplay()
            [EnumMember(Value = "Unknown")] Unknown = NotificationItem.ChangeUnknown
        }
        public string FileName { private set; get; }
        public string Folder { private set; get; }
        public bool Ready { private set; get; }
        public Header ChapterHeader { get; } 
        public Body ChapterBody { get; }


        public string StatusLine { private set; get; }
        public string MsgLine { private set; get; }
        public string EditorHelpLine { private set; get; }

        // ReSharper disable once RedundantBaseConstructorCall
        public ChapterModel() : base()
        {
            ChapterHeader = new Header();
            ChapterBody = new Body();
            StatusLine = "";
            MsgLine = "";
            EditorHelpLine = "";
            Ready = false;
        }

        public void SetStatusLine(bool update=true)     //called from Time Thread so readonly model items
        {
            var elapsed = DateTime.UtcNow - ChapterHeader.GetLastSession()?.Start;

            if (elapsed?.TotalDays > 0.99)
                StatusLine = $"{(elapsed?.TotalDays ?? 0.0).ToString(Header.MxStdFrmtDouble0)} days {elapsed?.ToString(Header.MxStdFrmtTimeSpan) ?? "00:00:00"} ";
            else
                StatusLine = $"{elapsed?.ToString(Header.MxStdFrmtTimeSpan) ?? "00:00:00"} ";

            StatusLine += $"Line: {ChapterBody?.GetLineCount() ?? Program.PosIntegerNotSet} ";
            StatusLine += $"Column: {ChapterBody?.GetCharacterCountInLine() + 1 ?? Program.PosIntegerNotSet} ";
            StatusLine += $"Total words: {ChapterBody?.WordCount ?? Program.PosIntegerNotSet} ";

            StatusLine += $"{(ChapterHeader?.GetPauseDetails() ?? Program.ValueNotSet)}";
            
            if (update)
                UpdateAllViews((int)ChangeHint.StatusLine);
        }
        public void SetMsgLine(string msg, bool update = true)
        {
            MsgLine = msg;
            if (update)
                UpdateAllViews((int)ChangeHint.MsgLine);
        }

        public void SetErrorMsg(int errorNo, string msg, bool update = true)
        {
            MsgLine = $"{BaseView.ErrorMsgPrecursor} {errorNo} {msg}";
            if (update)
                UpdateAllViews((int)ChangeHint.MsgLine);
        }

        public void SetMxErrorMsg(string msg, bool update = true)
        {
            MsgLine = msg;
            if (update)
                UpdateAllViews((int)ChangeHint.MsgLine);
        }

        public void SetEditorHelpLine(string text, bool update = true)
        {
            EditorHelpLine = text; //use to discover the active editor - TextEditView, etc
            if (update)
                UpdateAllViews((int)ChangeHint.HelpLine);
        }

        public void Refresh()
        {
            UpdateAllViews((int)ChangeHint.All);
        }

        public string GetTabSpaces()
        {
            return ChapterBody?.TabSpaces ?? Program.ValueNotSet;
        }

        public MxReturnCode<bool>Initialise(int lineWidth, string pathFilename)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.Setup");

            if ((string.IsNullOrEmpty(pathFilename)) || (lineWidth == Program.PosIntegerNotSet))
                rc.SetError(1050101, MxError.Source.Param, $"LineWidth={lineWidth} is invalid or pathFilename={pathFilename ?? "[null]"}", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    var rcInit = ChapterBody.Initialise(lineWidth);
                    rc += rcInit;
                    if (rcInit.IsSuccess(true))
                    {
                        rc.SetResult(false);
                        Folder = Path.GetDirectoryName(pathFilename);
                        if ((string.IsNullOrEmpty(Folder) == false) && (Directory.Exists(Folder) == false))
                            rc.SetError(1050102, MxError.Source.User, $"folder for edit file {pathFilename} does not exist. Create folder and try again.");
                        else
                        {
                            if (string.IsNullOrEmpty(Folder))
                                Folder = Directory.GetCurrentDirectory();
                            FileName = Path.GetFileName(pathFilename);

                            MxReturnCode<bool> rcDone = null;
                            if (File.Exists(pathFilename) == false)
                            {
                                rcDone = Write(pathFilename, true);
                                rc += rcDone;
                            }
                            else
                            {
                                rcDone = Read(pathFilename);
                                rc += rcDone;
                            }

                            if (rcDone.IsSuccess(true))
                            {
                                ChapterHeader.SetPauseWaitSeconds(CmdLineParamsApp.ArgPauseWaitSecsDefault); //todo set from value
                                Ready = true;
                                rc.SetResult(true);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1050103, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> Close(bool save = true)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.Close");

            if (Ready == false)
                rc.SetError(1050201, MxError.Source.Param, $"InitDone == false", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (save == false)
                {
                    DisconnectAllViews();
                    Ready = false;
                    rc.SetResult(true);
                }
                else
                {
                    var rcSave = Save();
                    rc += rcSave;
                    if (rcSave.IsSuccess(true))
                    {
                        DisconnectAllViews();
                        Ready = false;
                        rc.SetResult(true);
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> Save()
        {
            var rc = new MxReturnCode<bool>("ChapterModel.Save");

            if (Ready == false)
                rc.SetError(1050201, MxError.Source.Param, $"InitDone == false", MxMsgs.MxErrBadMethodParam);
            else
            {
                var rcWrite = Write($"{Folder}\\{FileName}");
                rc += rcWrite;
                if (rcWrite.IsSuccess(true))
                {
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> CreateNewSession()
        {
            var rc = new MxReturnCode<bool>("ChapterModel.CreateNewSession");

            if (Ready == false)
                rc.SetError(1050301, MxError.Source.Param, $"InitDone == false", MxMsgs.MxErrInvalidCondition);
            else
            {
                var rcSession = ChapterHeader.CreateNewSession(ChapterBody?.GetLineCount() ?? Program.PosIntegerNotSet);
                rc += rcSession;
                if (rcSession.IsSuccess(true))
                    rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> AppendLine(string line, bool incWordCount=true)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.AppendLine");

            if (Ready == false)
                rc.SetError(1050401, MxError.Source.Program, "InitDone is not done- Initialise not called or not successful", MxMsgs.MxErrInvalidCondition);
            else
            {
                var rcAdd = ChapterBody.AppendLine(line);
                if (rcAdd.IsError(true))
                    rc += rcAdd;        //may be called lots of times, so only log errors
                else
                {
                    UpdateAllViews((int)ChangeHint.Line);
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> AppendWord(string word)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.AppendWord");

            if (Ready == false)
                rc.SetError(1050501, MxError.Source.Program, "Initialise not called or not successful", MxMsgs.MxErrInvalidCondition);
            else
            {
                var rcAdd = ChapterBody.AppendWord(word);
                if (rcAdd.IsError(true))
                    rc += rcAdd;        //may be called lots of times, so only log errors
                else
                {
                    UpdateAllViews((int)ChangeHint.Word);
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> AppendChar(char c)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.AppendChar");

            if (Ready == false)
                rc.SetError(1050601, MxError.Source.Program, "Initialise not called or not successful", MxMsgs.MxErrInvalidCondition);
            else
            {
                var rcAdd = ChapterBody.AppendChar(c);
                if (rcAdd.IsError(true))
                    rc += rcAdd;        //may be called lots of times, so only log errors
                else
                {
                    UpdateAllViews((int)((Char.IsWhiteSpace(c)) ? ChangeHint.Word : ChangeHint.Char));
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public MxReturnCode<string[]> GetLastLinesForDisplay(int count) //string[] returned is only for display - altering these strings will not change the document
        {
            var rc = new MxReturnCode<string[]>("ChapterModel.GetLastLinesForDisplay", null);

            if (Ready == false)
                rc.SetError(1050701, MxError.Source.Program, "Initialise not called or not successful", MxMsgs.MxErrInvalidCondition);
            else
            {
                var rcLastLines = ChapterBody.GetLastLinesForDisplay(count);
                rc += rcLastLines;
                if (rcLastLines.IsSuccess(true))
                    rc.SetResult(rcLastLines.GetResult());
            }
            return rc;
        }

        private MxReturnCode<bool> Write(string pathFilename, bool newFile=false)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.Write");

            if (string.IsNullOrEmpty(pathFilename) || ((Ready == false) && (newFile == false)))
                rc.SetError(1050801, MxError.Source.Param, $"pathFilename={pathFilename ?? "[null]"}", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    using (var file = new StreamWriter(pathFilename)) //default StreamBuffer size is 1024
                    {
                        if (newFile)
                            ChapterHeader.SetDefaults(pathFilename);
                        var rcHeader = ChapterHeader.Write(file, newFile);
                        rc += rcHeader;
                        if (rcHeader.IsSuccess(true))
                        {
                            var rcBody = ChapterBody.Write(file);
                            rc += rcBody;
                            if (rcBody.IsSuccess(true))
                            {
                                rc.SetResult(true);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1050802, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        private MxReturnCode<bool> Read(string pathFilename)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.Read");

            if (string.IsNullOrEmpty(pathFilename))
                rc.SetError(1050901, MxError.Source.Param, $"pathFilename={pathFilename ?? "[null]"}", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    using (var file = new StreamReader(pathFilename)) //default StreamBuffer size is 1024
                    {
                        var rcHeader = ChapterHeader.Read(file);
                        rc += rcHeader;
                        if (rcHeader.IsSuccess(true))
                        {
                            var rcBody = ChapterBody.Read(file);
                            rc += rcBody;
                            if (rcBody.IsSuccess(true))
                            {
                                rc.SetResult(true);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1050902, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        public bool RemoveAllLines()
        {
            var rc = false;
            if (Ready && ((ChapterBody?.IsError() ?? true) == false))
            {
                ChapterBody.RemoveAllLines();
                rc = true;
            }
            return rc;
        }
        public HeaderSession GetLastSession()
        {
            return (Ready) ? ChapterHeader?.GetLastSession() : null;
        }
        public string GetReport()
        {
            string rc = $"{Environment.NewLine}[not initialized]";   //reports always start with newline, but don't end with one
            if (Ready)
            {
                rc = GetChapterReport();
                rc += GetLastSessionReport();
            }
            return rc;
        }
        public string GetChapterReport()
        {
            string rc = $"{Environment.NewLine}[not initialized]";   //reports always start with newline, but don't end with one
            if (Ready)
                rc = ChapterHeader?.GetChapterReport(ChapterBody.GetLineCount(), ChapterBody.WordCount) ?? "[chapter info not available]";

            return rc;
        }
        public string GetLastSessionReport()
        {
            string rc = $"{Environment.NewLine}[not initialized]";   //reports always start with newline, but don't end with one
            if (Ready)
                rc = ChapterHeader?.GetLastSessionReport() ?? "[chapter info not available]";
            return rc;
        }
        public int GetTextLineCount()
        {
            var rc = Program.PosIntegerNotSet;

            if (Ready)
                rc = ChapterBody.GetLineCount();
            return rc;
        }

        public int GetTextWordCount()
        {
            var rc = Program.PosIntegerNotSet;

            if (Ready)
                rc = ChapterBody.RefreshWordCount();
            return rc;
        }
    }
}
