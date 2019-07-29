using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using KLineEdCmdApp.Model.Base;
using KLineEdCmdApp.Utils;
using MxReturnCode;

using Extensions = KLineEdCmdApp.Utils.Extensions;


namespace KLineEdCmdApp.Model
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public class HeaderProps : HeaderElementBase
    {
        public static readonly string OpeningElement = "<props>";
        public static readonly string ClosingElement = "</props>";

        public static readonly string AuthorLabel = "Author:";  //labels for file
        public static readonly string ProjectLabel = "Project:";
        public static readonly string TitleLabel = "Title:";
        public static readonly string PathFileNameLabel = "File:";

        public string Author { get; private set; }
        public string Project { get; private set; }
        public string Title { get; private set; }
        public string PathFileName { get; private set; }

        public CursorPosition Cursor { get; private set; }
        public int MaxPropertyLength { get; private set; }
        public void SetMaxPropertyLength(int length) { if (length >= 0) MaxPropertyLength = length; }

        public enum CursorRow //see PropsEditView.XlatHeaderPropsRow(CursorRow row)
        {
            Author = 0,
            Project = 1,
            Title = 2,
            PathFileName = 3
        }

        // ReSharper disable once RedundantBaseConstructorCall
        public HeaderProps() : base()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Reset();
            MaxPropertyLength = Program.PosIntegerNotSet; //updated by Model.Initialise
        }

        public HeaderProps(int maxPropertyLength) : this()
        {
            if (maxPropertyLength >= 0)
                MaxPropertyLength = maxPropertyLength; //updated by Model.Initialise
        }

        public override void Reset()
        {
            Author = HeaderElementBase.PropertyNotSet;
            Project = HeaderElementBase.PropertyNotSet;
            Title = HeaderElementBase.PropertyNotSet;
            PathFileName = HeaderElementBase.PropertyNotSet;

            Cursor = new CursorPosition((int)CursorRow.Author, 0);

            Error = true;
            //don't change MaxEditAreaColumnIndex as it is set during Model.Initialise()
        }

        public bool SetDefaults(string pathFilename)   //typically called when starting a new session
        {
            // ReSharper disable once ReplaceWithSingleAssignment.False
            bool rc = false;

            if (pathFilename != null)
            {
                Error = true;
                SetMaxPropertyLength(CmdLineParamsApp.ArgEditAreaLineWidthDefault);
                if (SetAuthor("[author not set]") && SetProject("[project not set]") && SetTitle("[title not set]") && SetPathFileName(pathFilename))
                    rc = !Error;
            }
            return rc;
        }

        public int GetPropertyLength(CursorRow row)
        {
            var rc = Program.PosIntegerNotSet;

            if (row == CursorRow.Author)
                rc = Author?.Length ?? Program.PosIntegerNotSet;
            else if (row == CursorRow.Project)
                rc = Project?.Length ?? Program.PosIntegerNotSet;
            else if (row == CursorRow.Title)
                rc = Title?.Length ?? Program.PosIntegerNotSet;
            else if (row == CursorRow.PathFileName)
                rc = PathFileName?.Length ?? Program.PosIntegerNotSet;
            else
            {
                rc = Program.PosIntegerNotSet;
            }
            return rc;
        }

        public bool IsCursorBeyondEndOfLine(CursorRow row)
        {
            return Cursor.ColIndex > GetPropertyLength(row) - 1;
        }

        public CursorRow GetRowIndex(ChapterModel.CursorState state = ChapterModel.CursorState.Current)
        {
            var rc = CursorRow.Author;

            switch (state)
            {
                case ChapterModel.CursorState.Current:
                {
                    rc = (CursorRow)Cursor.RowIndex;
                    break;
                }
                case ChapterModel.CursorState.Next:
                {
                    var current = (CursorRow)Cursor.RowIndex;
                    if (current == CursorRow.Author)
                        rc = CursorRow.Project;
                    else if (current == CursorRow.Project)
                        rc = CursorRow.Title;
                    else if (current == CursorRow.Title)
                        rc = CursorRow.Author;
                    else
                        rc = CursorRow.Author;
                    break;
                }
                case ChapterModel.CursorState.Previous:
                {
                    var current = (CursorRow)Cursor.RowIndex;
                    if (current == CursorRow.Author)
                        rc = CursorRow.Title;
                    else if (current == CursorRow.Project)
                        rc = CursorRow.Author;
                    else if (current == CursorRow.Title)
                        rc = CursorRow.Project;
                    else
                        rc = CursorRow.Author;
                    break;
                }
                default:
                {
                    rc = (CursorRow)Cursor.RowIndex;
                    break;
                }
            }
            return rc;
        }

        public bool SetCursor(CursorRow row, int colIndex)
        {
            var rc = false;
            if ((colIndex >= 0) && (colIndex <= MaxPropertyLength - 1) && (colIndex <= GetPropertyLength(row))) //allow colIndex to be set immediately after last char
            {
                Cursor.RowIndex = (int)row;   //0 is top row of EditArea
                Cursor.ColIndex = colIndex;   //0 is left column of EditArea + PropsEditView.LongestLabelLength
                rc = true;
            }
            return rc;
        }

        public bool SetDelChar(bool backspace=false)
        {
            var rc = false;

            if ((backspace == false) || (Cursor.ColIndex > 0))
            {
                if (backspace)
                    Cursor.ColIndex--;
                switch ((CursorRow) Cursor.RowIndex)
                {
                    case CursorRow.Author:
                    {
                        var result = Body.GetLineUpdateDeleteChar(Author, Cursor.ColIndex);
                        if (result != null)
                        {
                            Author = result;
                            rc = true;
                        }
                        break;
                    }
                    case CursorRow.Project:
                    {
                        var result = Body.GetLineUpdateDeleteChar(Project, Cursor.ColIndex);
                        if (result != null)
                        {
                            Project = result;
                            rc = true;
                        }
                        break;
                    }
                    case CursorRow.Title:
                    {
                        var result = Body.GetLineUpdateDeleteChar(Title, Cursor.ColIndex);
                        if (result != null)
                        {
                            Title = result;
                            rc = true;
                        }
                        break;
                    }
                    case CursorRow.PathFileName:
                    {
                        var result = Body.GetLineUpdateDeleteChar(PathFileName, Cursor.ColIndex);
                        if (result != null)
                        {
                            PathFileName = result;
                            rc = true;
                        }
                        break;
                    }
                    default:
                    {
                        rc = false;
                        break;
                    }
                }
            }
            return rc;
        }

        public bool SetChar(char c, bool insert = false)
        {
            var rc = false;
            
            rc = SetText(c.ToString(), insert, false, false);

            return rc;
        }

        public bool SetText(string str, bool insert=false, bool addSpaceBefore=true, bool addSpaceAfter=true)
        {
            var rc = false;
            if (str != null)
            {
                var text = $"{((addSpaceBefore) ? " " : "")}{str}{((addSpaceAfter) ? " " : "")}";
                switch ((CursorRow) Cursor.RowIndex)
                {
                    case CursorRow.Author:
                    {
                        var result = Body.GetLineUpdateText(Author, text, Cursor.ColIndex, MaxPropertyLength, insert);
                        if (result != null)
                        {
                            Author = result;
                            Cursor.ColIndex += text.Length;
                            rc = true;
                        }
                        break;
                    }
                    case CursorRow.Project:
                    {
                        var result = Body.GetLineUpdateText(Project, text, Cursor.ColIndex, MaxPropertyLength, insert);
                        if (result != null)
                        {
                            Project = result;
                            Cursor.ColIndex += text.Length;
                            rc = true;
                        }
                        break;
                    }
                    case CursorRow.Title:
                    {
                        var result = Body.GetLineUpdateText(Title, text, Cursor.ColIndex, MaxPropertyLength, insert);
                        if (result != null)
                        {
                            Title = result;
                            Cursor.ColIndex += text.Length;
                            rc = true;
                        }
                        break;
                    }
                    case CursorRow.PathFileName:
                    {
                        var result = Body.GetLineUpdateText(PathFileName, text, Cursor.ColIndex, MaxPropertyLength, insert);
                        if (result != null)
                        {
                           PathFileName = result;
                           Cursor.ColIndex += text.Length;
                           rc = true;
                        }
                        break;
                    }
                    default:
                    {
                        rc = false;
                        break;
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> Write(StreamWriter file, bool newFile = false)
        {
            var rc = new MxReturnCode<bool>("HeaderProps.Write");

            if (file == null)
                rc.SetError(1060101, MxError.Source.Param, "file is null", MxMsgs.MxErrBadMethodParam);
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
                    rc.SetError(1060102, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }
        public MxReturnCode<bool> Read(StreamReader file)
        {
            var rc = new MxReturnCode<bool>("HeaderProps.Read");

            if (file == null)
                rc.SetError(1060201, MxError.Source.Param, "file is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    var firstLine = file.ReadLine();
                    if (firstLine != OpeningElement)
                        rc.SetError(1060202, MxError.Source.User, $"first line is {firstLine}", MxMsgs.MxErrInvalidCondition);
                    else
                    {
                        var lastLine = file.ReadLine();
                        if (lastLine != ClosingElement)
                        {
                            InitialiseFromString(lastLine); //line can only be 1024 - see EditFile.Create default buffer size  
                            lastLine = file.ReadLine();
                        }
                        if (lastLine != ClosingElement)
                            rc.SetError(1060203, MxError.Source.User, $"last line is {lastLine}", MxMsgs.MxErrInvalidCondition);
                        else
                        {
                            rc.SetResult(true);
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1060204, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        public override bool Validate()
        {
            bool rc = false;

            if (string.IsNullOrEmpty(HeaderElementBase.PropertyNotSet))   //don't test _error as calling IsValid() 
            {
                rc =  (string.IsNullOrEmpty(PathFileName) == false) && (string.IsNullOrEmpty(Title) == false)
                        && (string.IsNullOrEmpty(Project) == false) && (string.IsNullOrEmpty(Author) == false)
                        && (MaxPropertyLength != Program.PosIntegerNotSet)
                        && (Cursor.ColIndex >= 0) && ((Cursor.ColIndex <= MaxPropertyLength-1));
                Error = !rc; //set Error = false if all properties are now valid, else Error = true;
            }
            return rc;
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
            var rc = Environment.NewLine;   //reports always start with newline, but don't end with one
            if (IsError())
                rc += HeaderElementBase.ValueNotSet;
            else
                rc += $"{AuthorLabel} {Author}{Environment.NewLine}{ProjectLabel} {Project}{Environment.NewLine}{TitleLabel} {Title}{Environment.NewLine}{PathFileNameLabel} {PathFileName}";
            return rc;
        }
        public override string ToString()
        {
            var rc = HeaderElementBase.ValueNotSet; 
            if (IsError() == false)     //order must be same as InitialiseFromString()
                rc = $"{AuthorLabel} {Author} {ProjectLabel} {Project} {TitleLabel} {Title} {PathFileNameLabel} {PathFileName}";
            return rc;
        }
        public override MxReturnCode<bool> InitialiseFromString(string toString)
        {
            var rc = new MxReturnCode<bool>("HeaderProps.InitialiseFromString");

            Reset();

            if (toString != null)
            {       //order must be same as ToString()
                if (SetAuthor(Extensions.GetPropertyFromString(toString, AuthorLabel, ProjectLabel)) == false)
                    rc.SetError(1060301, MxError.Source.Data, $"SetAuthor failed for {toString}", MxMsgs.MxErrInvalidCondition);
                else
                {
                    if (SetProject(Extensions.GetPropertyFromString(toString, ProjectLabel, TitleLabel)) == false)
                        rc.SetError(1060302, MxError.Source.Data, $"SetProject failed for {toString}", MxMsgs.MxErrInvalidCondition);
                    else
                    {
                        if (SetTitle(Extensions.GetPropertyFromString(toString, TitleLabel, PathFileNameLabel)) == false)
                            rc.SetError(1060303, MxError.Source.Data, $"SetTitle failed for {toString}", MxMsgs.MxErrInvalidCondition);
                        else
                        {
                            if (SetPathFileName(Extensions.GetPropertyFromString(toString, PathFileNameLabel, null)) == false)
                                rc.SetError(1060304, MxError.Source.Data, $"SetPathFileName failed for {toString}", MxMsgs.MxErrInvalidCondition);
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
