using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using KLineEdCmdApp.Model.Base;
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
        public static readonly string OpeningElement = "<header>";
        public static readonly string ClosingElement = "</header>";

        public static readonly string SessionsOpeningElement = "<sessions>";
        public static readonly string SessionsClosingElement = "</sessions>";

        public HeaderChapter Chapter { private set; get; }
        private List<HeaderSession> Sessions { set; get; }

        public Header()
        {
            Chapter = new HeaderChapter();
            Sessions = new List<HeaderSession>();
        }

        public bool SetDefaults(string pathFilename)
        {
            return Chapter.SetDefaults(pathFilename);
        }

        public MxReturnCode<bool> Validate()
        {
            var rc = new MxReturnCode<bool>("Header.Validate");

            if (Chapter.Validate() == false)
                rc.SetError(1090101, MxError.Source.Data, $"Chapter is invalid={Chapter}");
            else
            {
                var sessionNo = 1;
                foreach (var session in Sessions)
                {
                    if (session.Validate() == false)
                    {
                        rc.SetError(1090102, MxError.Source.Data, $"Session {sessionNo} is invalid");
                        break;
                    }

                    if (session.SessionNo != sessionNo)
                    {
                        rc.SetError(1090103, MxError.Source.Data, $"Session {session.SessionNo} is not in sequence, expected {sessionNo}");
                        break;
                    }
                    sessionNo++;
                }
                if (rc.IsSuccess() && (sessionNo-1 == Sessions.Count))
                    rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> Write(StreamWriter file, bool newFile = false)
        {
            var rc = new MxReturnCode<bool>("Header.Write");

            if (file == null)
                rc.SetError(1090201, MxError.Source.Param, "file is null", "MxErrBadMethodParam");
            else
            {
                try
                {
                    file.WriteLine(OpeningElement);

                    Chapter.Write(file, newFile);

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
                    rc.SetError(1090202, MxError.Source.Exception, e.Message, "MxErrInvalidCondition");
                }
            }
            return rc;
        }
        public MxReturnCode<bool> Read(StreamReader file)
        {
            var rc = new MxReturnCode<bool>("Header.Read");

            if (file == null)
                rc.SetError(1090301, MxError.Source.Param, "file is null", "MxErrBadMethodParam");
            else
            {
                try
                {
                    var firstLine = file.ReadLine();
                    if (firstLine != OpeningElement)
                        rc.SetError(1090302, MxError.Source.User, $"Header opening line is {firstLine} not {OpeningElement}", "MxErrInvalidCondition");
                    else
                    {
                        var rcChapter = Chapter.Read(file);
                        rc += rcChapter;
                        if (rcChapter.IsSuccess(true))
                        {

                            var line = file.ReadLine();
                            if (line != SessionsOpeningElement)
                                rc.SetError(1090303, MxError.Source.User, $"Sessions opening line is {line} not {SessionsOpeningElement}", "MxErrInvalidCondition");
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
                                        rc.SetError(1090304, MxError.Source.User, $"Header closing line is {line} not {ClosingElement}", "MxErrInvalidCondition");
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
                    rc.SetError(1090305, MxError.Source.Exception, e.Message, "MxErrInvalidCondition");
                }
            }
            return rc;
        }

        public MxReturnCode<bool> CreateNewSession(int startLineNo)
        {
            var rc = new MxReturnCode<bool>("Header.CreateNewSession");

            if (startLineNo < 0 )
                rc.SetError(1090401, MxError.Source.Param, $"invalid StartLineNo={startLineNo}", "MxErrBadMethodParam");
            else
            {
                var session = new HeaderSession();

                var lastSessionNo = 0;
                if (Sessions.Count > 0)
                    lastSessionNo = Sessions[Sessions.Count - 1]?.SessionNo ?? Program.PosIntegerNotSet;

                if (session.SetDefaults(lastSessionNo + 1, startLineNo) == false)
                    rc.SetError(1090402, MxError.Source.Data, $"SetDefaults failed; lastSessionNo={lastSessionNo}+1, startLineNo={startLineNo}", "MxErrInvalidCondition");
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
            return Chapter?.SetDefaults(pathFileName) ?? false;
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
            var rc = Chapter?.GetReport() ?? HeaderBase.ValueNotSet;
            if (rc != HeaderBase.ValueNotSet)
            {
                rc += SessionsTotal.GetReport(Sessions, linesInChapter, wordsInChapter);
            }
            return rc;
        }
        public string GetLastSessionReport()
        {
            return GetLastSession()?.GetReport() ?? HeaderBase.ValueNotSet; 
        }
    }
}
