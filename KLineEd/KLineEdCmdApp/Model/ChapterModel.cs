using MxReturnCode;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using KLineEdCmdApp.Model.Base;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View;
using KLineEdCmdApp.View.Base;

namespace KLineEdCmdApp.Model
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    [SuppressMessage("ReSharper", "ConstantConditionalAccessQualifier")]
    public class ChapterModel : NotifierModel
    {
        public enum CursorState
        {
            Current,
            Next,
            Previous,
        }

        public enum ChangeHint
        {
            [EnumMember(Value = "Cursor")] Cursor = 0,          //TextEditView: ->, <-, ^, v, with no change to EditAreaBottomLineIndex 
            [EnumMember(Value = "Char")] Char = 1,              //TextEditView: not implemented
            [EnumMember(Value = "Line")] Line = 2,              //TextEditView: redraw Cursor.RowIndex line
            [EnumMember(Value = "End")] End = 3,                //TextEditView: redraw from Cursor.RowIndex line to last displayed line
            [EnumMember(Value = "Props")] Props = 4,            //PropsEditView: SetAuthor(), SetTitle(), SetProject() - change to char
            [EnumMember(Value = "Spell")] Spell = 5,            //SpellEditView:
            [EnumMember(Value = "StatusLine")] StatusLine = 6,  //StatusLineView: SetStatusLine()
            [EnumMember(Value = "MsgLine")] MsgLine = 7,        //MsgLineView: SetMsgLine()
            [EnumMember(Value = "HelpLine")] HelpLine = 8,      //EditorHelpLineView: SetEditorHelpLine()
            [EnumMember(Value = "All")] All = 9,                //(all views): RefreshDisplay()
            [EnumMember(Value = "Unknown")] Unknown = NotificationItem.ChangeUnknown
        }
        public string FileName { private set; get; }
        public string Folder { private set; get; }
        public bool Ready { private set; get; }
        public Header ChapterHeader { get; } 
        public Body ChapterBody { get; protected set; }

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

        public bool RemoveAllLines()
        {
            var rc = false;
            if (Ready && ((ChapterBody?.IsError() ?? true) == false))
            {
                var rcRemove = ChapterBody.RemoveAllLines();
                if (rcRemove.IsSuccess(true))
                    rc = true;
            }
            return rc;
        }

        public MxReturnCode<bool> Initialise(int textEditorDisplayRows, int textEditorDisplayCols, string pathFilename, char paraBreakChar = CmdLineParamsApp.ArgTextEditorDisplayParaBreakDisplayCharDefault, int spacesForTab = CmdLineParamsApp.ArgTextEditorTabSizeDefault, int scrollLimit = CmdLineParamsApp.ArgTextEditorLimitScrollDefault)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.Initialise");

            if ((textEditorDisplayRows == Program.PosIntegerNotSet) || (textEditorDisplayCols == Program.PosIntegerNotSet) || (string.IsNullOrEmpty(pathFilename)) || (spacesForTab < CmdLineParamsApp.ArgTextEditorTabSizeMin) || (spacesForTab > CmdLineParamsApp.ArgTextEditorTabSizeMax) || (scrollLimit < CmdLineParamsApp.ArgTextEditorLimitScrollMin) || (scrollLimit > CmdLineParamsApp.ArgTextEditorLimitScrollMax))
                rc.SetError(1050101, MxError.Source.Param, $"textEditorDisplayRows={textEditorDisplayRows}, textEditorDisplayCols={textEditorDisplayCols} is invalid, pathFilename={pathFilename ?? "[null]"}, spacesForTab={spacesForTab} <{CmdLineParamsApp.ArgTextEditorTabSizeMin},{CmdLineParamsApp.ArgTextEditorTabSizeMax}>, scrollLimit={scrollLimit} <{CmdLineParamsApp.ArgTextEditorLimitScrollMin}, {CmdLineParamsApp.ArgTextEditorLimitScrollMax}>", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    ChapterHeader.Properties.SetMaxPropertyLength(textEditorDisplayCols - PropsEditView.LongestLabelLength);
                    var rcInit = ChapterBody.Initialise(textEditorDisplayRows, textEditorDisplayCols, paraBreakChar, spacesForTab, scrollLimit);
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
                            if (File.Exists(pathFilename))
                            {
                                rcDone = Read(pathFilename);
                                rc += rcDone;
                            }
                            else
                            {
                                rcDone = Write(pathFilename, true);
                                rc += rcDone;
                            }
                            if (rcDone.IsSuccess(true))
                             {
                                ChapterHeader.SetPauseWaitSeconds(CmdLineParamsApp.ArgTextEditorPauseTimeoutDefault); //todo set from value
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
                rc.SetError(1050301, MxError.Source.Param, $"InitDone == false", MxMsgs.MxErrBadMethodParam);
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
                rc.SetError(1050401, MxError.Source.Param, $"InitDone == false", MxMsgs.MxErrInvalidCondition);
            else
            {
                var rcSession = ChapterHeader.CreateNewSession(ChapterBody?.GetLineCount() ?? Program.PosIntegerNotSet);
                rc += rcSession;
                if (rcSession.IsSuccess(true))
                    rc.SetResult(true);
            }
            return rc;
        }


        public void SetStatusLine(bool update=true)     //called from Time Thread so readonly model items
        {
            var elapsed = DateTime.UtcNow - ChapterHeader.GetLastSession()?.Start;
            string line = null;
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (elapsed?.TotalDays > 0.99)
                line = $"{(elapsed?.TotalDays ?? 0.0).ToString(Header.MxStdFrmtDouble0)} days {elapsed?.ToString(Header.MxStdFrmtTimeSpan) ?? "00:00:00"} ";
            else
                line = $"{elapsed?.ToString(Header.MxStdFrmtTimeSpan) ?? "00:00:00"} ";

            line += $"Line: {ChapterBody?.Cursor?.RowIndex+1 ?? Program.PosIntegerNotSet} ";
            line += $"Column: {ChapterBody?.Cursor?.ColIndex+1 ?? Program.PosIntegerNotSet} ";
            line += $"Total words: {ChapterBody?.WordCount ?? Program.PosIntegerNotSet} ";

            line += $"{(ChapterHeader?.GetPauseDetails() ?? Program.ValueNotSet)}";

            if (StatusLine != line)
            {
                StatusLine = line;
                if (update)
                    UpdateAllViews((int)ChangeHint.StatusLine);
            }
        }
        public void SetMsgLine(string msg, bool update = true)
        {
            if (MsgLine != msg)
            {
                MsgLine = msg;
                if (update)
                    UpdateAllViews((int) ChangeHint.MsgLine);
            }
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

        public MxReturnCode<bool> BodyMoveCursor(Body.CursorMove move)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.BodyMoveCursor");

            if (ChapterBody == null)
                rc.SetError(1050501, MxError.Source.Program, "ChapterBody is null", MxMsgs.MxErrInvalidCondition);
            else
            {
                var rcMove = ChapterBody.MoveCursorInChapter(move);
                rc += rcMove;
                if (rcMove.IsSuccess(true))
                {
                    UpdateAllViews((int) rcMove.GetResult());
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> BodyBackSpace()
        {
            var rc = new MxReturnCode<bool>("ChapterModel.BodyBackSpace");

            if (ChapterBody == null)
                rc.SetError(1050601, MxError.Source.Program, "ChapterBody is null", MxMsgs.MxErrInvalidCondition);
            else
            {
                var rcMove = ChapterBody.MoveCursorInChapter(Body.CursorMove.PreviousCol);
                rc += rcMove;
                if (rcMove.IsSuccess(true))
                {
                    var rcDelete = BodyDeleteCharacter(rcMove.GetResult());
                    rc += rcDelete;
                    if (rcDelete.IsSuccess(true))
                    {
                        rc.SetResult(true);
                    }
                }
            }
            return rc;
        }


        public MxReturnCode<bool> BodyDeleteCharacter(ChangeHint hint = ChangeHint.Unknown)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.BodyDeleteCharacter");

            if (ChapterBody == null)
                rc.SetError(1050701, MxError.Source.Program, "ChapterBody is null", MxMsgs.MxErrInvalidCondition);
            else
            {
               var rcDelete = ChapterBody.DeleteCharacter();
               rc += rcDelete;
               if (rcDelete.IsSuccess(true))
               {
                   UpdateAllViews((int)rcDelete.GetResult());
                   rc.SetResult(true);
               }
            }
            return rc;
        }

        public MxReturnCode<bool> BodyInsertParaBreak()
        {
            var rc = new MxReturnCode<bool>("ChapterModel.InsertParaBreak");

            if (Ready == false)
                rc.SetError(1050801, MxError.Source.Program, "InitDone is not done- Initialise not called or not successful", MxMsgs.MxErrInvalidCondition);
            else
            {
                var rcInsert = ChapterBody.InsertParaBreak();
                rc += rcInsert;
                if (rcInsert.IsSuccess(true))
                {
                    UpdateAllViews((int)rcInsert.GetResult()); 
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> BodyInsertText(string text, bool insertMode = false)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.BodyInsertText");

            if (ChapterBody == null)
                rc.SetError(1050901, MxError.Source.Program, "ChapterBody is null", MxMsgs.MxErrInvalidCondition);
            else
            {
                var rcInsertText = ChapterBody.InsertText(text, insertMode);
                rc += rcInsertText;
                if (rcInsertText.IsSuccess(true))
                {
                    UpdateAllViews((int) rcInsertText.GetResult());
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public bool PropsSetCursor(HeaderProps.CursorRow row, int colIndex)
        {
            var rc = ChapterHeader?.Properties?.SetCursor(row, colIndex) ?? false;
            if (rc == true)
                UpdateAllViews((int)ChangeHint.Props);
            return rc;
        }

        public bool PropsDelChar(bool backspace = false)
        {
            var rc = ChapterHeader?.Properties?.SetDelChar(backspace) ?? false;
            if (rc == true)
                UpdateAllViews((int)ChangeHint.Props);
            return rc;
        }

        public bool PropsSetChar(char c, bool insert = false)
        {
            var rc = ChapterHeader?.Properties?.SetChar(c, insert) ?? false;
            if (rc == true)
                UpdateAllViews((int)ChangeHint.Props);
            return rc;
        }

        public bool PropsSetText(string text, bool insert = false)
        {
            var rc = ChapterHeader?.Properties?.SetText(text, insert) ?? false;
            if (rc == true)
                UpdateAllViews((int)ChangeHint.Props);
            return rc;
        }

        public void Refresh()
        {
            ChapterBody?.RefreshWordCountInChapter();
            UpdateAllViews((int)ChangeHint.All);
        }

        public string GetTabSpaces()
        {
            return ChapterBody?.TabSpaces ?? Program.ValueNotSet;
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

        private MxReturnCode<bool> Write(string pathFilename, bool newFile = false)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.Write");

            if (string.IsNullOrEmpty(pathFilename) || ((Ready == false) && (newFile == false)))
                rc.SetError(1051101, MxError.Source.Param, $"pathFilename={pathFilename ?? "[null]"}", MxMsgs.MxErrBadMethodParam);
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
                    rc.SetError(1051102, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        private MxReturnCode<bool> Read(string pathFilename)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.Read");

            if (string.IsNullOrEmpty(pathFilename))
                rc.SetError(1051201, MxError.Source.Param, $"pathFilename={pathFilename ?? "[null]"}", MxMsgs.MxErrBadMethodParam);
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
                    rc.SetError(1051202, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }
    }
}
