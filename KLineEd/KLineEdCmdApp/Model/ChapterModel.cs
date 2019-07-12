using MxReturnCode;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using KLineEdCmdApp.Model.Base;
using KLineEdCmdApp.Utils;
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
            [EnumMember(Value = "Status")] Status = 3,  //SetStatusLine()
            [EnumMember(Value = "Msg")] Msg = 4,        //SetMsgLine()
            [EnumMember(Value = "Cmd")] Cmd = 5,        //UpdateAllViews(Cmd)
            [EnumMember(Value = "All")] All = 6,
            [EnumMember(Value = "Unknown")] Unknown = NotificationItem.ChangeUnknown
        }
        public string FileName { private set; get; }
        public string Folder { private set; get; }
        public bool Ready { private set; get; }
        public Header Header { get; } 
        public Body Body { get; }

        public string StatusLine { private set; get; }
        public string MsgLine { private set; get; }
        public string CmdsHelpLine { private set; get; }

        // ReSharper disable once RedundantBaseConstructorCall
        public ChapterModel() : base()
        {
            Header = new Header();
            Body = new Body();
            StatusLine = "";
            MsgLine = "";
            CmdsHelpLine = "";
            Ready = false;
        }

        public void SetStatusLine()     //called from Time Thread so readonly model items
        {
            StatusLine = $"{DateTime.Now.ToString(MxStdFrmt.Time)} Line: {Body?.GetLineCount() ?? Program.PosIntegerNotSet} Column: {Body?.GetCharactersInLine() ?? Program.PosIntegerNotSet} Total words: {Body?.WordCount ?? Program.PosIntegerNotSet}";
            UpdateAllViews((int)ChangeHint.Status);
        }
        public void SetMsgLine(string msg)
        {
            MsgLine = msg;
            UpdateAllViews((int)ChangeHint.Msg);
        }

        public void SetCmdLine(string cmd)
        {
            CmdsHelpLine = cmd;
            UpdateAllViews((int)ChangeHint.Cmd);
        }

        public MxReturnCode<bool>Initialise(int lineWidth, string pathFilename)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.Setup");

            if ((string.IsNullOrEmpty(pathFilename)) || (lineWidth == Program.PosIntegerNotSet))
                rc.SetError(1050101, MxError.Source.Param, $"LineWidth={lineWidth} is invalid or pathFilename={pathFilename ?? "[null]"}", "MxErrBadMethodParam");
            else
            {
                try
                {
                    var rcInit = Body.Initialise(lineWidth);
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
                                Ready = true;
                                rc.SetResult(true);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1050103, MxError.Source.Exception, e.Message);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> Close(bool save = true)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.Close");

            if (Ready == false)
                rc.SetError(1050201, MxError.Source.Param, $"InitDone == false", "MxErrBadMethodParam");
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
                rc.SetError(1050201, MxError.Source.Param, $"InitDone == false", "MxErrBadMethodParam");
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
                rc.SetError(1050301, MxError.Source.Param, $"InitDone == false", "MxErrInvalidCondition");
            else
            {
                var rcSession = Header.CreateNewSession(Body?.GetLineCount() ?? Program.PosIntegerNotSet);
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
                rc.SetError(1050401, MxError.Source.Program, "InitDone is not done- Initialise not called or not successful", "MxErrInvalidCondition");
            else
            {
                var rcAdd = Body.AppendLine(line);
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
                rc.SetError(1050501, MxError.Source.Program, "Initialise not called or not successful", "MxErrInvalidCondition");
            else
            {
                var rcAdd = Body.AppendWord(word);
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
                rc.SetError(1050601, MxError.Source.Program, "Initialise not called or not successful", "MxErrInvalidCondition");
            else
            {
                var rcAdd = Body.AppendChar(c);
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
                rc.SetError(1050701, MxError.Source.Program, "Initialise not called or not successful", "MxErrInvalidCondition");
            else
            {
                var rcLastLines = Body.GetLastLinesForDisplay(count);
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
                rc.SetError(1050801, MxError.Source.Param, $"pathFilename={pathFilename ?? "[null]"}", "MxErrBadMethodParam");
            else
            {
                try
                {
                    using (var file = new StreamWriter(pathFilename)) //default StreamBuffer size is 1024
                    {
                        if (newFile)
                            Header.SetDefaults(pathFilename);
                        var rcHeader = Header.Write(file, newFile);
                        rc += rcHeader;
                        if (rcHeader.IsSuccess(true))
                        {
                            var rcBody = Body.Write(file);
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
                    rc.SetError(1050802, MxError.Source.Exception, e.Message);
                }
            }
            return rc;
        }

        private MxReturnCode<bool> Read(string pathFilename)
        {
            var rc = new MxReturnCode<bool>("ChapterModel.Read");

            if (string.IsNullOrEmpty(pathFilename))
                rc.SetError(1050901, MxError.Source.Param, $"pathFilename={pathFilename ?? "[null]"}", "MxErrBadMethodParam");
            else
            {
                try
                {
                    using (var file = new StreamReader(pathFilename)) //default StreamBuffer size is 1024
                    {
                        var rcHeader = Header.Read(file);
                        rc += rcHeader;
                        if (rcHeader.IsSuccess(true))
                        {
                            var rcBody = Body.Read(file);
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
                    rc.SetError(1050902, MxError.Source.Exception, e.Message);
                }
            }
            return rc;
        }

        public bool RemoveAllLines()
        {
            var rc = false;
            if (Ready && ((Body?.IsError() ?? true) == false))
            {
                Body.RemoveAllLines();
                rc = true;
            }
            return rc;
        }
        public HeaderSession GetLastSession()
        {
            return (Ready) ? Header?.GetLastSession() : null;
        }
        public string GetReport()
        {
            var rc = "";
            if (Ready == false)
                rc += "[not initialized]";
            else
            {
                rc += GetChapterReport();
                rc += GetLastSessionReport();
            }
            return rc;
        }
        public string GetChapterReport()
        {
            var rc = "";
            if (Ready == false)
                rc += "[not initialized]";
            else
            {
                rc += Header?.GetChapterReport(Body.GetLineCount(), Body.WordCount) ?? "[chapter info not available]";
            }
            return rc;
        }
        public string GetLastSessionReport()
        {
            var rc = "";
            if (Ready == false)
                rc += "[not initialized]";
            else
            {
                rc += Header?.GetLastSessionReport() ?? "[chapter info not available]";
            }
            return rc;
        }
        public int GetTextLineCount()
        {
            var rc = Program.PosIntegerNotSet;

            if (Ready)
                rc = Body.GetLineCount();
            return rc;
        }

        public int GetTextWordCount()
        {
            var rc = Program.PosIntegerNotSet;

            if (Ready)
                rc = Body.RefreshWordCount();
            return rc;
        }
    }
}
