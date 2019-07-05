using MxReturnCode;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
// ReSharper disable All

namespace KLineEdCmdApp.Model
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public class ChapterModel
    {
        public string FileName { private set; get; }
        public string Folder { private set; get; }
        public bool Ready { private set; get; }
        public Header Header { private set; get; } 
        public Body Body { private set; get; }    
        

        public ChapterModel()
        {
            Header = new Header();
            Body = new Body();
            Ready = false;
        }

        public MxReturnCode<bool>Initialise(int LineWidth, string pathFilename)
        {
            var rc = new MxReturnCode<bool>("EditFileOps.Setup");

            if ((string.IsNullOrEmpty(pathFilename)) || (LineWidth == KLineEditor.PosIntegerNotSet))
                rc.SetError(1050101, MxError.Source.Param, $"LineWidth={LineWidth} is invalid or pathFilename={pathFilename ?? "[null]"}", "MxErrBadMethodParam");
            else
            {
                try
                {
                    var rcInit = Body.Initialise(LineWidth);
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
            var rc = new MxReturnCode<bool>("EditFile.Close");

            if (Ready == false)
                rc.SetError(1050201, MxError.Source.Param, $"InitDone == false", "MxErrBadMethodParam");
            else
            {
                if (save == false)
                {
                    Ready = false;
                    rc.SetResult(true);
                }
                else
                {
                    var rcSave = Save();
                    rc += rcSave;
                    if (rcSave.IsSuccess(true))
                    {
                        Ready = false;
                        rc.SetResult(true);
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> Save()
        {
            var rc = new MxReturnCode<bool>("EditFile.Save");

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
            var rc = new MxReturnCode<bool>("EditFile.CreateNewSession");

            if (Ready == false)
                rc.SetError(1050301, MxError.Source.Param, $"InitDone == false", "MxErrInvalidCondition");
            else
            {
                var rcSession = Header.CreateNewSession(Body?.GetLineCount() ?? KLineEditor.PosIntegerNotSet);
                rc += rcSession;
                if (rcSession.IsSuccess(true))
                    rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> AddLine(string line)
        {
            var rc = new MxReturnCode<bool>("EditFile.AddLine");

            if (Ready == false)
                rc.SetError(1050401, MxError.Source.Program, "InitDone is not done- Initialise not called or not successful", "MxErrInvalidCondition");
            else
            {
                var rcAdd = Body.AddLine(line);
                if (rcAdd.IsError(true))
                    rc += rcAdd;        //may be called lots of times, so only log errors
                else
                    rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> AddWord(string word)
        {
            var rc = new MxReturnCode<bool>("EditFile.AddWord");

            if (Ready == false)
                rc.SetError(1050501, MxError.Source.Program, "InitDone is not done- Initialise not called or not successful", "MxErrInvalidCondition");
            else
            {
                var rcAdd = Body.AddWord(word);
                if (rcAdd.IsError(true))
                    rc += rcAdd;        //may be called lots of times, so only log errors
                else
                    rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<string[]> GetLastLinesForDisplay(int count) //string[] returned is only for display - altering these strings will not change the document
        {
            var rc = new MxReturnCode<string[]>("EditFile.GetLastLinesForDisplay", null);

            if (Ready == false)
                rc.SetError(1050601, MxError.Source.Program, "InitDone is not done- Initialise not called or not successful", "MxErrInvalidCondition");
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
            var rc = new MxReturnCode<bool>("EditFile.Write");

            if (string.IsNullOrEmpty(pathFilename) || ((Ready == false) && (newFile == false)))
                rc.SetError(1050701, MxError.Source.Param, $"pathFilename={pathFilename ?? "[null]"}", "MxErrBadMethodParam");
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
                    rc.SetError(1050701, MxError.Source.Exception, e.Message);
                }
            }
            return rc;
        }

        private MxReturnCode<bool> Read(string pathFilename)
        {
            var rc = new MxReturnCode<bool>("EditFile.Read");

            if (string.IsNullOrEmpty(pathFilename))
                rc.SetError(1050801, MxError.Source.Param, $"pathFilename={pathFilename ?? "[null]"}", "MxErrBadMethodParam");
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
                    rc.SetError(1050802, MxError.Source.Exception, e.Message);
                }
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
            var rc = KLineEditor.PosIntegerNotSet;

            if (Ready)
                rc = Body.GetLineCount();
            return rc;
        }
    }
}
