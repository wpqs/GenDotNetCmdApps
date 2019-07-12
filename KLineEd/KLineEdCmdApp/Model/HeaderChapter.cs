using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using KLineEdCmdApp.Controller;
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
    public class HeaderChapter : HeaderBase
    {
        public static readonly string OpeningElement = "<chapter>";
        public static readonly string ClosingElement = "</chapter>";

        public static readonly string AuthorLabel = "Author:";
        public static readonly string ProjectLabel = "Project:";
        public static readonly string TitleLabel = "Title:";
        public static readonly string PathFileNameLabel = "File:";

        public string Author { get; private set; }
        public string Project { get; private set; }
        public string Title { get; private set; }
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
                if (SetAuthor("[author not set]") && SetProject("[project not set]") && SetTitle("[title not set]") && SetPathFileName(pathFilename))
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
                rc =  (string.IsNullOrEmpty(PathFileName) == false) && (string.IsNullOrEmpty(Title) == false)
                        && (string.IsNullOrEmpty(Project) == false) && (string.IsNullOrEmpty(Author) == false);
                Error = !rc; //set Error = false if all properties are now valid, else Error = true;
            }
            return rc;
        }
        public override void Reset()
        {
            Author = HeaderBase.PropertyNotSet;
            Project = HeaderBase.PropertyNotSet; 
            Title = HeaderBase.PropertyNotSet;
            PathFileName = HeaderBase.PropertyNotSet;
            Error = true;
        }
        public override bool IsLabelFound(string name)
        {
            // ReSharper disable once ReplaceWithSingleAssignment.False
            bool rc = false;

            if (name != null)
            {
                if ((name.Contains(ProjectLabel)) || (name.Contains(TitleLabel)) 
                      || (name.Contains(PathFileNameLabel)) || (name.Contains(AuthorLabel)))
                    rc = true;
            }
            return rc;
        }
        public override string GetReport()
        {
            var rc = HeaderBase.ValueNotSet;
            if (IsError() == false)     
                rc = $"{AuthorLabel} {Author}{Environment.NewLine}{ProjectLabel} {Project}{Environment.NewLine}{TitleLabel} {Title}{Environment.NewLine}{PathFileNameLabel} {PathFileName}";
            rc += KLineEditor.ReportSectionDottedLine;
            return rc;
        }
        public override string ToString()
        {
            var rc = HeaderBase.ValueNotSet; 
            if (IsError() == false)     //order must be same as InitialiseFromString()
                rc = $"{AuthorLabel} {Author} {ProjectLabel} {Project} {TitleLabel} {Title} {PathFileNameLabel} {PathFileName}";
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
                    if (SetProject(Extensions.GetPropertyFromString(toString, ProjectLabel, TitleLabel)) == false)
                        rc.SetError(1060302, MxError.Source.Data, $"SetProject failed for {toString}", "MxErrInvalidCondition");
                    else
                    {
                        if (SetTitle(Extensions.GetPropertyFromString(toString, TitleLabel, PathFileNameLabel)) == false)
                            rc.SetError(1060303, MxError.Source.Data, $"SetTitle failed for {toString}", "MxErrInvalidCondition");
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
        public bool SetTitle(string name)
        {
            Title = GetString(name, Title, out var rc);
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
