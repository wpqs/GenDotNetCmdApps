﻿using MxReturnCode;
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
            [EnumMember(Value = "Cursor")] Cursor = 0,  //->, <-, ^, v, with no change to EditAreaBottomLineIndex 
            [EnumMember(Value = "Char")] Char = 1,      //AppendChar()
            [EnumMember(Value = "Line")] Line = 2,      //AppendLine()
            [EnumMember(Value = "Word")] Word = 3,      //AppendWord()
            [EnumMember(Value = "Props")] Props = 4,      //SetAuthor(), SetTitle(), SetProject() - change to char
            [EnumMember(Value = "Spell")] Spell = 5,      //
            [EnumMember(Value = "StatusLine")] StatusLine = 6,  //SetStatusLine()
            [EnumMember(Value = "MsgLine")] MsgLine = 7,        //SetMsgLine()
            [EnumMember(Value = "HelpLine")] HelpLine = 8,      //SetEditorHelpLine()
            [EnumMember(Value = "All")] All = 9,        //RefreshDisplay()
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

        public bool MoveBodyCursor(Body.CursorMove move)
        {
            var rc = false;

            var existingIndex = ChapterBody?.EditAreaBottomChapterIndex ?? Program.PosIntegerNotSet;
            if (existingIndex != Program.PosIntegerNotSet)
            {
                if (ChapterBody?.MoveCursorInChapter(move) ?? false)
                {
                    if (existingIndex == ChapterBody?.EditAreaBottomChapterIndex)
                        UpdateAllViews((int)ChangeHint.Cursor);
                    else
                        UpdateAllViews((int)ChangeHint.All);
                    rc = true;
                }
            }
            return rc;
        }

        public bool SetBodyDelChar(bool backspace = false)
        {
            var rc = ChapterBody?.SetDelChar(backspace) ?? false;
            if (rc == true)
                UpdateAllViews((int)ChangeHint.All);
            return rc;
        }

        public bool SetBodyChar(char c, bool insert = false)
        {
            var rc = ChapterBody?.SetChar(c, insert) ?? false;
            if (rc == true)
                UpdateAllViews((int)ChangeHint.All);
            return rc;
        }

        public bool SetBodyText(string text, bool insert = false)
        {
            var rc = ChapterBody?.SetText(text, insert, false, false) ?? false;
            if (rc == true)
                UpdateAllViews((int)ChangeHint.All);
            return rc;
        }

        public bool SetBodyInsertLine(string line, bool atEndOfChapter=true)
        {
            var rc = false;
            if ((ChapterBody?.InsertLine(line, atEndOfChapter)?.GetResult() ?? false)) 
            {
                UpdateAllViews((int) ChangeHint.All);
                rc = true;
            }
            return rc;
        }

        public bool SetPropsCursor(HeaderProps.CursorRow row, int colIndex)
        {
            var rc = ChapterHeader?.Properties?.SetCursor(row, colIndex) ?? false;
            if (rc == true)
                UpdateAllViews((int)ChangeHint.Props);
            return rc;
        }

        public bool SetPropsDelChar(bool backspace = false)
        {
            var rc = ChapterHeader?.Properties?.SetDelChar(backspace) ?? false;
            if (rc == true)
                UpdateAllViews((int)ChangeHint.Props);
            return rc;
        }

        public bool SetPropsChar(char c, bool insert = false)
        {
            var rc = ChapterHeader?.Properties?.SetChar(c, insert) ?? false;
            if (rc == true)
                UpdateAllViews((int)ChangeHint.Props);
            return rc;
        }

        public bool SetPropsText(string text, bool insert = false)
        {
            var rc = ChapterHeader?.Properties?.SetText(text, insert) ?? false;
            if (rc == true)
                UpdateAllViews((int)ChangeHint.Props);
            return rc;
        }

        public string GetTabSpaces()
        {
            return ChapterBody?.TabSpaces ?? Program.ValueNotSet;
        }

        public MxReturnCode<bool>Initialise(int editAreaLinesCount, int editAreaLineWidth, string pathFilename)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.Setup");

            if ((string.IsNullOrEmpty(pathFilename)) || (editAreaLinesCount == Program.PosIntegerNotSet) || (editAreaLineWidth == Program.PosIntegerNotSet))
                rc.SetError(1050101, MxError.Source.Param, $"editAreaLinesCount={editAreaLinesCount}, editAreaLineWidth={editAreaLineWidth} is invalid or pathFilename={pathFilename ?? "[null]"}", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    ChapterHeader.Properties.SetMaxPropertyLength(editAreaLineWidth-PropsEditView.LongestLabelLength);
                    var rcInit = ChapterBody.Initialise(editAreaLinesCount, editAreaLineWidth);
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

        public MxReturnCode<bool> InsertLine(string line, bool atEndOfChapter=true)   //only called by Tests
        {
            var rc = new MxReturnCode<bool>("ChapterModel.AppendLine");

            if (Ready == false)
                rc.SetError(1050401, MxError.Source.Program, "InitDone is not done- Initialise not called or not successful", MxMsgs.MxErrInvalidCondition);
            else
            {
                var rcAdd = ChapterBody.InsertLine(line, atEndOfChapter);
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

        public MxReturnCode<bool> AppendWord(string word)   //called only from Tests
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

        public MxReturnCode<bool> AppendChar(char c)        //called only from Tests
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

        public MxReturnCode<string[]> GetLastLinesForDisplay(int count = Program.PosIntegerNotSet) //string[] returned is only for display - altering these strings will not change the document
        {
            var rc = new MxReturnCode<string[]>("ChapterModel.GetLastLinesForDisplay", null);

            if ((Ready == false) || (count < Program.PosIntegerNotSet))
                rc.SetError(1050701, MxError.Source.Program, $"Initialise not called or not successful, or count < {Program.PosIntegerNotSet}", MxMsgs.MxErrInvalidCondition);
            else
            {
                var lineCount = (count == Program.PosIntegerNotSet) ? ChapterBody.EditAreaViewCursorLimit.RowIndex : count;
                var rcLastLines = ChapterBody.GetLinesForDisplay(lineCount);
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
