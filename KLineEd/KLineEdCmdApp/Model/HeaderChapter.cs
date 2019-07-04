using System;
using System.IO;
using MxReturnCode;
// ReSharper disable All

namespace KLineEdCmdApp.Model
{
    public class HeaderChapter : HeaderBase
    {
        public static readonly string OpeningElement = "<chapter>";
        public static readonly string ClosingElement = "</chapter>";

        public static readonly string AuthorLabel = "Author:";
        public static readonly string ProjectLabel = "Project:";
        public static readonly string ChapterLabel = "Chapter:";
        public static readonly string PathFileNameLabel = "File:";

        public string Author { get; private set; }
        public string Project { get; private set; }
        public string Chapter { get; private set; }
        public string PathFileName { get; private set; }

        public HeaderChapter()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Reset();
        }

        public bool SetDefaults(string pathFilename)   //typically called when starting a new session
        {
            // ReSharper disable once ReplaceWithSingleAssignment.False
            bool rc = false;

            if (pathFilename != null)
            {
                if (SetAuthor("[author not set]") && SetProject("[project not set]") && SetChapter("[chapter not set]") && SetPathFileName(pathFilename))
                    rc = true;
            }
            return rc;
        }

        public MxReturnCode<bool> Write(StreamWriter file, bool newFile = false)
        {
            var rc = new MxReturnCode<bool>("HeaderChapter.Write");

            if (file == null)
                rc.SetError(1060101, MxError.Source.Param, "file is null", "MxErrBadMethodParam");
            else
            {
                try
                {
                    file.WriteLine(OpeningElement);

                    if ((IsError() == false) || (newFile == true))
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
            var rc = new MxReturnCode<bool>("HeaderChapter.Read");

            if (file == null)
                rc.SetError(1060201, MxError.Source.Param, "file is null", "MxErrBadMethodParam");
            else
            {
                try
                {
                    var firstLine = file.ReadLine();
                    if (firstLine != OpeningElement)
                        rc.SetError(1060202, MxError.Source.User, $"first line is {firstLine}", "MxErrInvalidCondition");
                    else
                    {
                        var lastLine = file.ReadLine();
                        if (lastLine != ClosingElement)
                        {
                            InitialiseFromString(lastLine); //line can only be 1024 - see EditFile.Create default buffer size  
                            lastLine = file.ReadLine();
                        }
                        if (lastLine != ClosingElement)
                            rc.SetError(1060203, MxError.Source.User, $"last line is {lastLine}", "MxErrInvalidCondition");
                        else
                        {
                            rc.SetResult(true);
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1060204, MxError.Source.Exception, e.Message, "MxErrInvalidCondition");
                }
            }
            return rc;
        }

        public override bool Validate()
        {
            bool rc = false;

            if (string.IsNullOrEmpty(HeaderBase.PropertyNotSet))   //don't test _error as calling IsValid() 
            {
                rc =  (string.IsNullOrEmpty(PathFileName) == false) && (string.IsNullOrEmpty(Chapter) == false)
                        && (string.IsNullOrEmpty(Project) == false) && (string.IsNullOrEmpty(Author) == false);
                Error = !rc; //set Error = false if all properties are now valid, else Error = true;
            }
            return rc;
        }
        public override void Reset()
        {
            Author = HeaderBase.PropertyNotSet;
            Project = HeaderBase.PropertyNotSet; 
            Chapter = HeaderBase.PropertyNotSet;
            PathFileName = HeaderBase.PropertyNotSet;
            Error = true;
        }
        public override bool IsLabelFound(string name)
        {
            // ReSharper disable once ReplaceWithSingleAssignment.False
            bool rc = false;

            if (name != null)
            {
                if ((name.Contains(ProjectLabel)) || (name.Contains(ChapterLabel)) 
                      || (name.Contains(PathFileNameLabel)) || (name.Contains(AuthorLabel)))
                    rc = true;
            }
            return rc;
        }
        public override string GetReport()
        {
            var rc = HeaderBase.ValueNotSet;
            if (IsError() == false)     
                rc = $"{AuthorLabel} {Author}{Environment.NewLine}{ProjectLabel} {Project}{Environment.NewLine}{ChapterLabel} {Chapter}{Environment.NewLine}{PathFileNameLabel} {PathFileName}";
            rc += KLineEditor.ReportSectionDottedLine;
            return rc;
        }
        public override string ToString()
        {
            var rc = HeaderBase.ValueNotSet; 
            if (IsError() == false)     //order must be same as InitialiseFromString()
                rc = $"{AuthorLabel} {Author} {ProjectLabel} {Project} {ChapterLabel} {Chapter} {PathFileNameLabel} {PathFileName}";
            return rc;
        }
        public override MxReturnCode<bool> InitialiseFromString(string toString)
        {
            var rc = new MxReturnCode<bool>("HeaderChapter.InitialiseFromString");

            Reset();

            if (toString != null)
            {       //order must be same as ToString()
                if (SetAuthor(Extensions.GetPropertyFromString(toString, AuthorLabel, ProjectLabel)) == false)
                    rc.SetError(1060301, MxError.Source.Data, $"SetAuthor failed for {toString}", "MxErrInvalidCondition");
                else
                {
                    if (SetProject(Extensions.GetPropertyFromString(toString, ProjectLabel, ChapterLabel)) == false)
                        rc.SetError(1060302, MxError.Source.Data, $"SetProject failed for {toString}", "MxErrInvalidCondition");
                    else
                    {
                        if (SetChapter(Extensions.GetPropertyFromString(toString, ChapterLabel, PathFileNameLabel)) == false)
                            rc.SetError(1060303, MxError.Source.Data, $"SetChapter failed for {toString}", "MxErrInvalidCondition");
                        else
                        {
                            if (SetPathFileName(Extensions.GetPropertyFromString(toString, PathFileNameLabel, null)) == false)
                                rc.SetError(1060304, MxError.Source.Data, $"SetPathFileName failed for {toString}", "MxErrInvalidCondition");
                            else
                            {
                                rc.SetResult(true);
                            }
                        }
                    }
                }
                if (rc.IsError(true))
                    Reset();
            }
            return rc;
        }
        public bool SetAuthor(string name)
        {
            Author = GetString(name, Author, out var rc);
            Validate();    
            return rc;
        }
        public bool SetProject(string name)
        {
            Project = GetString(name, Project, out var rc);
            Validate();     
            return rc;
        }
        public bool SetChapter(string name)
        {
            Chapter = GetString(name, Chapter, out var rc);
            Validate();     
            return rc;
        }
        public bool SetPathFileName(string name)
        {
            PathFileName = GetString(name, PathFileName, out var rc);
            Validate();    
            return rc;
        }
    }
}
