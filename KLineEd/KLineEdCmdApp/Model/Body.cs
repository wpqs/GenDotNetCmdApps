using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using KLineEdCmdApp.Properties;
using KLineEdCmdApp.Utils;
using MxReturnCode;

namespace KLineEdCmdApp.Model
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    [SuppressMessage("ReSharper", "ConvertIfToOrExpression")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "RedundantEmptySwitchSection")]
    [SuppressMessage("ReSharper", "RedundantNameQualifier")]
    [SuppressMessage("ReSharper", "InvertIf")]
    [SuppressMessage("ReSharper", "RedundantCaseLabel")]
    [SuppressMessage("ReSharper", "RedundantTernaryExpression")]
    [SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
    public class Body
    {
        public static readonly string OpeningElement = "<body>";
        public static readonly string ClosingElement = "</body>";

        public static readonly char DisallowedCharOpeningAngle = '<';
        public static readonly char DisallowedCharClosingAngle = '>';

        public static readonly char ParaBreakChar = ((char)0x17); //ASCII ETB - end of block
        public static readonly string ParaBreak = ParaBreakChar.ToString(); //ASCII ETB - end of block

        public const int LastLine = -1;         //used to provide parameter default values so cannot be readonly
        public const int LastColumn = -1;
        public const char NullChar = (char)0;
        public const char SpaceChar = ' ';

        public const int MaxTextLines = 36 * 2500;  //twice the number of pages in Tolstoy's 'War and Peace'


        public enum Scroll
        {
            Top,
            Bottom,
            LineUp,
            LineDown,
            PageUp,
            PageDown,
            ToCursor
        }

        public enum CursorMove
        {
            NextRow,
            PreviousRow,
            NextCol,
            PreviousCol,
            StartChapter,
            EndChapter,
            PageUp,
            PageDown,
            StartPara,
            EndPara,
            StartLine,
            EndLine
        }

        protected List<string> TextLines { set; get; }
        public CursorPosition Cursor { get; private set; }
        public CursorPosition EditAreaViewCursorLimit { get; private set; }
        public int EditAreaTopLineChapterIndex { private set; get; }        //TextLines[EditAreaTopLineChapterIndex] is displayed as first line in console
        public int WordCount { protected set; get; }
        public int LinesPerPage { protected set; get; }
        public string TabSpaces { private set; get; }
        public char ParaBreakDisplayChar { private set; get; }
        public int ScrollLimitChapter { private set; get; }                 //maximum number of lines you can scroll back from the end of the chapter (0=unlimited)

        private bool Error { set; get; }
        public bool IsError(){ return Error; }

        public Body()
        {
            TextLines = new List<string>();

            Cursor = new CursorPosition(0, 0);
            EditAreaViewCursorLimit = new CursorPosition(CmdLineParamsApp.ArgTextEditorDisplayRowsDefault, CmdLineParamsApp.ArgTextEditorDisplayColsDefault); //updated by Model.Initialise
            EditAreaTopLineChapterIndex = Program.PosIntegerNotSet;

            WordCount = Program.PosIntegerNotSet;
            LinesPerPage = Program.PosIntegerNotSet;
            SetTabSpaces(CmdLineParamsApp.ArgTextEditorTabSizeDefault);
            ParaBreakDisplayChar = CmdLineParamsApp.ArgTextEditorDisplayParaBreakDisplayCharDefault;
            ScrollLimitChapter = CmdLineParamsApp.ArgTextEditorLimitScrollDefault;

            Error = true;
        }

        public MxReturnCode<bool> RemoveAllLines()
        {
            var rc = new MxReturnCode<bool>("Body.RemoveAllLines");

            if ((TextLines == null) || (Cursor == null) || (EditAreaViewCursorLimit == null))
                rc.SetError(1100101, MxError.Source.Program, $"TextLines, Cursor or EditAreaViewCursorLimit is null", MxMsgs.MxErrInvalidCondition);
            else
            {
                TextLines.Clear();
                WordCount = 0;
                if(ResetCursorInChapter())
                    rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> Initialise(int textEditorDisplayRows, int textEditorDisplayCols, char paraBreakDisplayChar = CmdLineParamsApp.ArgTextEditorDisplayParaBreakDisplayCharDefault, int spacesForTab = CmdLineParamsApp.ArgTextEditorTabSizeDefault, int scrollLimit = CmdLineParamsApp.ArgTextEditorLimitScrollDefault, int linesPerPage = CmdLineParamsApp.ArgTextEditorLinesPerPageDefault)
        {
            var rc = new MxReturnCode<bool>("Body.Initialise");

            if ((textEditorDisplayCols == Program.PosIntegerNotSet) || (textEditorDisplayRows == Program.PosIntegerNotSet) || (paraBreakDisplayChar == NullChar) || (spacesForTab < CmdLineParamsApp.ArgTextEditorTabSizeMin) || (spacesForTab > CmdLineParamsApp.ArgTextEditorTabSizeMax) || (scrollLimit < CmdLineParamsApp.ArgTextEditorLimitScrollMin) || (scrollLimit > CmdLineParamsApp.ArgTextEditorLimitScrollMax) || (linesPerPage < CmdLineParamsApp.ArgTextEditorLinesPerPageMin) || (linesPerPage > CmdLineParamsApp.ArgTextEditorLinesPerPageMax))
                rc.SetError(1100201, MxError.Source.Param, $"textEditorDisplayRows={textEditorDisplayRows}, textEditorDisplayCols={textEditorDisplayCols}, paraBreak is 0, spacesForTab={spacesForTab} <{CmdLineParamsApp.ArgTextEditorTabSizeMin}, {CmdLineParamsApp.ArgTextEditorTabSizeMax}, scrollLimit={scrollLimit} <{CmdLineParamsApp.ArgTextEditorLimitScrollMin}, {CmdLineParamsApp.ArgTextEditorLimitScrollMax}>, linesPerPage={linesPerPage} <{CmdLineParamsApp.ArgTextEditorLinesPerPageMin},{CmdLineParamsApp.ArgTextEditorLinesPerPageMax}>", MxMsgs.MxErrBadMethodParam);
            else
            {
                if ((TextLines == null) || (Cursor == null) || (EditAreaViewCursorLimit == null))
                    rc.SetError(1100202, MxError.Source.Program, $"TextLines, Cursor or EditAreaViewCursorLimit is null", MxMsgs.MxErrInvalidCondition);
                else
                {
                    EditAreaViewCursorLimit.RowIndex = textEditorDisplayRows - 1;
                    EditAreaViewCursorLimit.ColIndex = textEditorDisplayCols - 1;

                    LinesPerPage = linesPerPage;
                    SetTabSpaces(spacesForTab);
                    ParaBreakDisplayChar = paraBreakDisplayChar;
                    ScrollLimitChapter = scrollLimit;

                    var rcRemove = RemoveAllLines();
                    rc += rcRemove;
                    if (rcRemove.IsSuccess(true))
                    {
                        Error = false;
                        rc.SetResult(true);
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> Write(StreamWriter file)
        {
            var rc = new MxReturnCode<bool>("Body.WriteString");

            if (file == null)
                rc.SetError(1100301, MxError.Source.Param, "file is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    if (IsError())
                        rc.SetError(1100302, MxError.Source.Program, "IsError() == true, Initialise not called?", MxMsgs.MxErrInvalidCondition);
                    else
                    {
                        file.WriteLine(OpeningElement);

                        foreach (var line in TextLines)
                        {
                            file.WriteLine(line); 
                        }

                        file.WriteLine(ClosingElement);
                        rc.SetResult(true);
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1100303, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }

            return rc;
        }

        public MxReturnCode<bool> Read(StreamReader file)
        {
            var rc = new MxReturnCode<bool>("Body.Read");

            if (file == null)
                rc.SetError(1100401, MxError.Source.Param, "file is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    if (IsError())
                        rc.SetError(1100402, MxError.Source.Program, "IsError() == true; Initialise not called?", MxMsgs.MxErrInvalidCondition);
                    else
                    {
                        var rcRemove = RemoveAllLines();
                        rc += rcRemove;
                        if (rcRemove.IsSuccess(true))
                        {
                            var firstLine = file.ReadLine();
                            if (firstLine != OpeningElement)
                                rc.SetError(1100403, MxError.Source.User, $"first line is {firstLine}", MxMsgs.MxErrInvalidCondition);
                            else
                            {
                                var rowIndex = 0;
                                string lastLine = null;
                                string line = null;
                                while ((line = file.ReadLine()) != null)
                                {
                                    lastLine = line;
                                    if (lastLine == ClosingElement)
                                        break;
                                    var rcLine = InsertLine(rowIndex, lastLine); 
                                    if (rcLine.IsError(true))
                                    {
                                        rc += rcLine;
                                        break;
                                    }
                                    rowIndex++;
                                }
                                if (rc.IsSuccess())
                                {
                                    if (string.IsNullOrEmpty(lastLine) || lastLine != ClosingElement)
                                        rc.SetError(1100404, MxError.Source.User, $"last line is {lastLine ?? "[null]"}", MxMsgs.MxErrInvalidCondition);
                                    else
                                    {
                                        var rcRefresh = Refresh();
                                        rc += rcRefresh;
                                        if (rcRefresh.IsSuccess(true))
                                        {
                                            rc.SetResult(true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1100405, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> Refresh()
        {
            var rc = new MxReturnCode<bool>("Body.Refresh");

            if (TextLines == null)
                rc.SetError(1100501, MxError.Source.Program, $"TextLines is null", MxMsgs.MxErrInvalidCondition);
            else
            {
                var limitTemp = ScrollLimitChapter;
                ScrollLimitChapter = 0;
                var rowCountIndex = 0;
                while (rowCountIndex < TextLines.Count)
                {
                    var rcJustify = LeftJustifyLinesInParagraph(rowCountIndex);
                    if (rcJustify.IsError(true))
                    {
                        rc += rcJustify;
                        break;
                    }
                    if ((rowCountIndex = GetNextParaBreakRowIndex(rowCountIndex)) == Program.PosIntegerNotSet)
                        break;
                    rowCountIndex++;
                }
                ScrollLimitChapter = limitTemp;

                if (rc.IsSuccess())
                {
                    var rcCursor = SetCursorInChapter(TextLines.Count-1, GetMaxColCursorIndexForRow(TextLines.Count - 1));
                    rc += rcCursor;
                    if (rcCursor.IsSuccess(true))
                    {
                        WordCount = RefreshWordCountInChapter();
                        rc.SetResult(true);
                    }
                }
            }
            return rc;
        }


        public MxReturnCode<ChapterModel.ChangeHint> MoveCursorInChapter(CursorMove move)
        {
            var rc = new MxReturnCode<ChapterModel.ChangeHint>("Body.MoveCursorInChapter", ChapterModel.ChangeHint.Unknown);

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((TextLines == null) || (linesCount == Program.PosIntegerNotSet) || (Cursor == null) || (EditAreaViewCursorLimit == null) || (Cursor.RowIndex < 0) || ((linesCount > 0) && (Cursor.RowIndex >= linesCount)) || (Cursor.ColIndex < 0) || (Cursor.ColIndex > (EditAreaViewCursorLimit.ColIndex+1)))
                rc.SetError(1100601, MxError.Source.Program, $"TextLines, Cursor or EditAreaViewCursorLimit is null, or existing cursor is invalid; Cursor.RowIndex={Cursor?.RowIndex ?? -1} (max={linesCount}), Cursor.ColIndex={Cursor?.ColIndex ?? -1} (max={EditAreaViewCursorLimit?.ColIndex ?? -1})", MxMsgs.MxErrInvalidCondition);
            else
            {
                if (linesCount == 0)
                    rc.SetError(1100602, MxError.Source.User, Resources.MxWarnChapterEmpty);
                else
                {
                    try
                    {
                        switch (move)
                        {
                            case CursorMove.NextCol:
                            {
                                MxReturnCode<ChapterModel.ChangeHint> rcCursor = null;
                                if ((Cursor.ColIndex + 1) <= GetMaxColCursorIndexForRow(Cursor.RowIndex))
                                    rcCursor = SetCursorInChapter(Cursor.RowIndex, Cursor.ColIndex + 1);
                                else
                                    rcCursor = SetCursorInChapter(Cursor.RowIndex + 1, 0);
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(rcCursor.GetResult());
                                break;
                            }
                            case CursorMove.PreviousCol:
                            {
                                MxReturnCode<ChapterModel.ChangeHint> rcCursor = null;
                                if ((Cursor.ColIndex - 1) >= 0)
                                    rcCursor = SetCursorInChapter(Cursor.RowIndex, Cursor.ColIndex - 1);
                                else
                                    rcCursor = SetCursorInChapter(Cursor.RowIndex - 1, GetMaxColCursorIndexForRow(Cursor.RowIndex - 1));
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(rcCursor.GetResult());
                                break;
                            }
                            case CursorMove.NextRow:
                            {
                                var lastColIndex = GetMaxColCursorIndexForRow(Cursor.RowIndex + 1);
                                var rcCursor = SetCursorInChapter(Cursor.RowIndex + 1, (Cursor.ColIndex < lastColIndex) ? Cursor.ColIndex : lastColIndex);
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(rcCursor.GetResult());
                                break;
                            }
                            case CursorMove.PreviousRow:
                            {
                                var lastColIndex = GetMaxColCursorIndexForRow(Cursor.RowIndex - 1);
                                var rcCursor = SetCursorInChapter(Cursor.RowIndex - 1, (Cursor.ColIndex < lastColIndex) ? Cursor.ColIndex : lastColIndex);
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(rcCursor.GetResult());
                                break;
                            }
                            case CursorMove.PageUp:
                            {
                                var rowIndex = Program.PosIntegerNotSet;
                                if (Cursor.RowIndex > 0)
                                {
                                    var displayHt = EditAreaViewCursorLimit.RowIndex + 1;
                                    rowIndex = ((Cursor.RowIndex - (displayHt-1)) > 0) ? Cursor.RowIndex - (displayHt-1) : 0;
                                }
                                var rcCursor = SetCursorInChapter(rowIndex, Cursor.ColIndex);
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(rcCursor.GetResult());
                                break;
                            }
                            case CursorMove.PageDown:
                            {
                                var rowIndex = linesCount;
                                if (Cursor.RowIndex < linesCount-1)
                                {
                                    var displayHt = EditAreaViewCursorLimit.RowIndex + 1;
                                    rowIndex = ((Cursor.RowIndex + (displayHt-1)) < (linesCount - 1)) ? Cursor.RowIndex + (displayHt-1) : linesCount - 1;
                                }
                                var rcCursor = SetCursorInChapter(rowIndex, Cursor.ColIndex); 
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(rcCursor.GetResult());
                                break;
                            }
                            case CursorMove.StartPara:
                            {
                                var rcCursor = SetCursorInChapter(GetStartParaRowIndex(Cursor.RowIndex), 0);
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(rcCursor.GetResult());
                                break;
                            }
                            case CursorMove.EndPara:
                            {
                                var rowIndex = GetNextParaBreakRowIndex(Cursor.RowIndex);
                                var rcCursor = SetCursorInChapter(rowIndex, GetLineLength(rowIndex)); 
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(rcCursor.GetResult());
                                break;
                            }
                            case CursorMove.StartLine:
                            {
                                var rcCursor = SetCursorInChapter(Cursor.RowIndex, 0); 
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(rcCursor.GetResult());
                                break;
                            }
                            case CursorMove.EndLine:
                            {
                                var rcCursor = SetCursorInChapter(Cursor.RowIndex, GetLineLength(Cursor.RowIndex)); 
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(rcCursor.GetResult());
                                break;
                            }
                            case CursorMove.StartChapter:
                            {
                                var rcCursor = SetCursorInChapter(0, 0);
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(rcCursor.GetResult());
                                break;
                            }
                            case CursorMove.EndChapter:
                            {
                                var rcCursor = SetCursorInChapter(linesCount - 1, GetMaxColCursorIndexForRow(linesCount - 1));
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(rcCursor.GetResult());
                                break;
                            }
                            default:
                            {
                                rc.SetError(1100603, MxError.Source.Program, $"unsupported move={move}", MxMsgs.MxErrInvalidCondition);
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        rc.SetError(1100604, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<ChapterModel.ChangeHint> SetCursorInChapter(int rowIndex, int colIndex)
        {
            var rc = new MxReturnCode<ChapterModel.ChangeHint>("Body.SetCursorInChapter", ChapterModel.ChangeHint.Unknown);
                                        //rowIndex == -1 or rowIndex == TextLines.Count are errors needing specific reporting
            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((linesCount == Program.PosIntegerNotSet) || (rowIndex < -1) || (rowIndex > linesCount) || (colIndex < -1) || (colIndex > (EditAreaViewCursorLimit?.ColIndex+1 ?? colIndex-1))) //allow cursor to move one character after last permitted col
                rc.SetError(1100701, MxError.Source.Param, $"rowIndex={rowIndex} > max({linesCount}), colIndex={colIndex} > max={EditAreaViewCursorLimit?.ColIndex ?? colIndex-1}", MxMsgs.MxErrBadMethodParam);
            else
            {
                if ((Cursor == null) || (TextLines == null))
                    rc.SetError(1100702, MxError.Source.Program, $"Cursor or TextLines is null", MxMsgs.MxErrInvalidCondition);
                else
                {
                    if (linesCount == 0)
                    {
                        Cursor.RowIndex = 0;
                        Cursor.ColIndex = 0;
                    }
                    else
                    {
                        if (rowIndex == linesCount)
                            rc.SetError(1100703, MxError.Source.User, Resources.MxWarnEndOfChapter);
                        else
                        {
                            if ((rowIndex == -1) || (colIndex == -1))
                                rc.SetError(1100704, MxError.Source.User, Resources.MxWarnStartOfChapter);
                            else
                            {
                                if (IsCursorBeyondScrollLimit(rowIndex))
                                    rc.SetError(1100705, MxError.Source.User, Resources.MxWarnScrollLimit);
                                else
                                {
                                    var lastColIndex = GetMaxColCursorIndexForRow(rowIndex);
                                    if ((lastColIndex == Program.PosIntegerNotSet) || (colIndex > lastColIndex))
                                    { //this will result in split line so move cursor to column on next rowIndex which is at end of word where split happened
                                        var lastSpaceIndex = TextLines[rowIndex].LastIndexOf(' ');
                                        var lastWordLen = (lastSpaceIndex > -1) ? TextLines[rowIndex].Length - (lastSpaceIndex + 1) : 1;
                                        Cursor.ColIndex = (lastWordLen <= 1) ? 0 : lastWordLen - 1;
                                        Cursor.RowIndex = rowIndex;
                                    }
                                    else
                                    {
                                        Cursor.RowIndex = rowIndex;
                                        Cursor.ColIndex = colIndex;
                                    }
                                }
                            }
                        }
                    }
                    if (rc.IsSuccess())
                    {
                        var rcScroll = SetEditAreaTopLineChapterIndex(Scroll.ToCursor);
                        rc += rcScroll;
                        if (rcScroll.IsSuccess(true))
                            rc.SetResult(rcScroll.GetResult());
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<ChapterModel.ChangeHint> SetEditAreaTopLineChapterIndex(Body.Scroll scroll = Body.Scroll.Bottom)
        {
            var rc = new MxReturnCode<ChapterModel.ChangeHint>("Body.SetEditAreaTopLineChapterIndex", ChapterModel.ChangeHint.Unknown);

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((linesCount == Program.PosIntegerNotSet) || (EditAreaViewCursorLimit == null))
                rc.SetError(1100801, MxError.Source.Program, $"EditAreaViewCursorLimit or TextLines is null", MxMsgs.MxErrInvalidCondition);
            else
            {
                var displayHt = EditAreaViewCursorLimit.RowIndex + 1;
                switch (scroll)
                {
                    case Scroll.ToCursor:
                    {
                        var existingIndex = EditAreaTopLineChapterIndex;
                        if (Cursor.RowIndex < EditAreaTopLineChapterIndex)                      //cursor above top line in edit area    
                            EditAreaTopLineChapterIndex = Cursor.RowIndex;
                        else if (Cursor.RowIndex >= (EditAreaTopLineChapterIndex + displayHt))   //cursor below bottom line in edit area
                            EditAreaTopLineChapterIndex = Cursor.RowIndex - (displayHt - 1);
                        else
                        {
                                //(Cursor.RowIndex >= EditAreaTopLineChapterIndex) && (Cursor.RowIndex < EditAreaTopLineChapterIndex + displayHt)
                        }
                        rc.SetResult((existingIndex!= EditAreaTopLineChapterIndex) ? ChapterModel.ChangeHint.All: ChapterModel.ChangeHint.Cursor);
                        break;
                    }
                    case Scroll.Top:    //support for moving edit area (display) without moving Cursor.RowIndex - i.e. read only review of chapter
                    case Scroll.Bottom:
                    case Scroll.LineUp:
                    case Scroll.LineDown:
                    case Scroll.PageUp:
                    case Scroll.PageDown:
                    default:
                    {
                        rc.SetError(1100802, MxError.Source.Program, $"unsupported scroll={scroll}", MxMsgs.MxErrInvalidCondition);
                        break;
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<ChapterModel.ChangeHint> InsertParaBreak()
        {
            var rc = new MxReturnCode<ChapterModel.ChangeHint>("Body.InsertParaBreak", ChapterModel.ChangeHint.Unknown);

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if (IsError() || (linesCount == Program.PosIntegerNotSet) || (Cursor == null) || (Cursor.RowIndex < 0) || (Cursor.RowIndex > ((linesCount == 0) ? 0 : linesCount - 1)))
                rc.SetError(1101001, MxError.Source.Program, $"IsError() == true, or invalid Cursor.RowIndex={Cursor?.RowIndex ?? -1}", MxMsgs.MxErrInvalidCondition);
            else
            {
                try
                {
                    if (linesCount >= Body.MaxTextLines)
                        rc.SetError(1101002, MxError.Source.User, $"too many lines in chapter={linesCount}", MxMsgs.MxWarnTooManyLines);
                    else
                    {
                        if (linesCount == 0)
                        {
                            TextLines.Add(ParaBreak);
                            rc.SetResult(ChapterModel.ChangeHint.Line); //insert first line, so doesn't make any difference end or line
                        }
                        else
                        {
                            var currentLine = TextLines[Cursor.RowIndex]; //lines cannot be null or empty
                            if (string.IsNullOrEmpty(currentLine))
                                rc.SetError(1101003, MxError.Source.Program, $"Line {Cursor.RowIndex + 1} is null or empty", MxMsgs.MxErrInvalidCondition);
                            else
                            {
                                if ((Cursor.ColIndex < 0) || (Cursor.ColIndex > currentLine.Length))
                                    rc.SetError(1101004, MxError.Source.Program, $"Invalid Cursor.ColIndex={Cursor.ColIndex} for Cursor.RowIndex={Cursor.RowIndex} length={currentLine.Length}", MxMsgs.MxErrInvalidCondition);
                                else
                                {
                                    var remainderWordCount = WordCount - GetWordCountInLine(currentLine);
                                    var before = currentLine.Snip(0, Cursor.ColIndex - 1);
                                    var after = currentLine.Snip(Cursor.ColIndex, currentLine.Length - 1);

                                    WordCount = remainderWordCount + GetWordCountInLine(before) + GetWordCountInLine(after);

                                    TextLines[Cursor.RowIndex] = (before == null) ? ParaBreak : before + ParaBreak;
                                    TextLines.Insert(Cursor.RowIndex + 1, after ?? ParaBreak);

                                    //adjust rest of paragraph lines - avoids need for SetCursorInChapter(Cursor.RowIndex + 1, 0);
                                    var rcJustify = LeftJustifyLinesInParagraph(Cursor.RowIndex+1, 0);
                                    rc += rcJustify;
                                    if (rcJustify.IsSuccess(true))
                                    {
                                        rc.SetResult( ChapterModel.ChangeHint.End); 
                                    }
                                }
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1101005, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        public MxReturnCode<ChapterModel.ChangeHint> InsertText(string text, bool insert = false)     //text is not null or empty and length < EditAreaViewCursorLimit.ColIndex
        {
            var rc = new MxReturnCode<ChapterModel.ChangeHint>("Body.InsertText", ChapterModel.ChangeHint.Unknown);

            var maxColIndex = EditAreaViewCursorLimit?.ColIndex ?? Program.PosIntegerNotSet;
            if ((string.IsNullOrEmpty(text) == true) || (text.Length-1 > maxColIndex))
                rc.SetError(1101101, MxError.Source.Param, $"text is null, or length={text?.Length ?? Program.PosIntegerNotSet} > maxColIndex={maxColIndex}", MxMsgs.MxErrBadMethodParam);
            else
            {
                var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
                if (IsError() || (TextLines == null) || (linesCount == Program.PosIntegerNotSet))
                    rc.SetError(1101102, MxError.Source.Program, $"Body not initialized", MxMsgs.MxErrInvalidCondition);
                else
                {
                    try
                    {
                        if (Body.GetErrorsInText(text) != null)
                            rc.SetError(1101103, MxError.Source.User, $"{MxReturnCodeUtils.WarningMsgPrecursor}{Body.GetErrorsInText(text, Cursor.RowIndex+1, Cursor.ColIndex+1)}");
                        else
                        {
                            if (linesCount <= 0)
                                rc += InsertParaBreak(); //force hint.End

                            if (rc.IsError() || (Cursor.RowIndex < 0) || (Cursor.RowIndex >= TextLines.Count) || (Cursor.ColIndex < 0) || (Cursor.ColIndex > maxColIndex+1)) //allow cursor one char beyond last permitted char in line
                                rc.SetError(1101104, MxError.Source.Program, $"Invalid Cursor: Cursor.ColIndex={Cursor.ColIndex} (max={maxColIndex}) Cursor.RowIndex={Cursor.RowIndex} (max={linesCount-1})", MxMsgs.MxErrInvalidCondition);
                            else
                            {
                                var hint = rc.GetResult(); //unknown unless InsertParaBreak invoked in which case it is End, or Line
                                var line = TextLines[Cursor.RowIndex];
                                var existWordCount = GetWordCountInLine(line);

                                if ((Cursor.ColIndex == line.Length) || insert)
                                    line = line.Insert(Cursor.ColIndex, text);
                                else
                                {
                                    var paraBreak = line.EndsWith(ParaBreakChar) ? Body.ParaBreak : "";
                                    var start = line.Snip(0, Cursor.ColIndex - 1); //null if Cursor.ColIndex=0
                                    var end = line.Snip(Cursor.ColIndex + text.Length, line.Length-1); //null if text overwrites all text in line after Cursor.ColIndex
                                    if (end?.EndsWith(ParaBreakChar) ?? false)
                                        paraBreak = "";
                                    line = (start ?? "") + text + (end ?? "") + paraBreak;
                                }
                                TextLines[Cursor.RowIndex] = line; //may be longer than maxColIndex, but fixed by LeftJustifyLinesInParagraph

                                var rcJustify = LeftJustifyLinesInParagraph(Cursor.RowIndex, Cursor.ColIndex + text.Length);
                                rc += rcJustify;
                                if (rcJustify.IsSuccess(true))
                                {
                                    WordCount += GetWordCountInLine(line) - existWordCount; //maybe less words now so += -3
                                    rc.SetResult((hint == ChapterModel.ChangeHint.Unknown) ? rcJustify.GetResult() : hint);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        rc.SetError(1101105, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<ChapterModel.ChangeHint> DeleteCharacter()
        {
            var rc = new MxReturnCode<ChapterModel.ChangeHint>("Body.DeleteCharacter", ChapterModel.ChangeHint.Unknown);

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((TextLines == null) || (linesCount == Program.PosIntegerNotSet) || (Cursor == null) || (Cursor.ColIndex < 0) || (Cursor.RowIndex < 0) || ((linesCount > 0) && (Cursor.RowIndex >= linesCount)))
                rc.SetError(1101201, MxError.Source.Param, $"Cursor.RowIndex={Cursor?.RowIndex ?? -1}, Cursor.ColIndex={Cursor?.ColIndex ?? -1} for TextLines.Count={linesCount}", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (linesCount == 0)
                    rc.SetError(1101202, MxError.Source.User, Resources.MxWarnChapterEmpty);
                else
                {
                    try
                    {
                        var rowIndex = Cursor.RowIndex;
                        var colIndex = Cursor.ColIndex;

                        var line = TextLines[rowIndex];
                        if (Cursor.ColIndex >= line.Length)
                            rc.SetError(1101203, MxError.Source.User, Resources.MxWarnNoCharToDelete);
                        else
                        {
                            if (GetCharacterCountInRow(rowIndex) <= 1)
                            {
                                WordCount -= line.EndsWith(ParaBreakChar) ? line.Length - 1 : line.Length;
                                TextLines.RemoveAt(rowIndex);

                                rowIndex = ((TextLines.Count > 0) && (rowIndex == TextLines.Count)) ? rowIndex - 1 : rowIndex;
                                colIndex = ((TextLines.Count > 0) && (rowIndex < TextLines.Count)) ? GetMaxColCursorIndexForRow(rowIndex) : 0;
                            }
                            else
                            {
                                var existWordCount = GetWordCountInLine(line);

                                var start = line.Snip(0, colIndex - 1);             //maybe null if colIndex=0
                                var end = line.Snip(colIndex + 1, line.Length - 1); //null only if line.Length=1 and colIndex=0
                                line = start + end;
                                if (string.IsNullOrEmpty(line))
                                    rc.SetError(1101204, MxError.Source.Program, $"start={start ?? "[null]"}, end={end ?? "[null]"}", MxMsgs.MxErrInvalidCondition);
                                else
                                {
                                    WordCount -= existWordCount - GetWordCountInLine(line);
                                    TextLines[rowIndex] = line;
                                }
                            }
                        }

                        if (rc.IsSuccess())
                        {
                            var rcJustify = LeftJustifyLinesInParagraph(rowIndex, colIndex);
                            rc += rcJustify;
                            if (rcJustify.IsSuccess(true))
                            {
                                rc.SetResult(rcJustify.GetResult());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        rc.SetError(1101205, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<ChapterModel.ChangeHint> LeftJustifyLinesInParagraph(int cursorRowIndex, int cursorColIndex=0)
        {
            var rc = new MxReturnCode<ChapterModel.ChangeHint>("Body.LeftJustifyLinesInParagraph", ChapterModel.ChangeHint.Unknown);

            var maxColIndex = EditAreaViewCursorLimit?.ColIndex ?? Program.PosIntegerNotSet;
            var originalCursor = new CursorPosition(cursorRowIndex, cursorColIndex);

            if ((TextLines == null) || (maxColIndex <= 0) || (maxColIndex >= CmdLineParamsApp.ArgTextEditorDisplayColsMax) ||  (originalCursor.IsValid(TextLines.Count) == false) ) 
                rc.SetError(1101301, MxError.Source.Param, $"TextLines={(TextLines?.Count ?? -1)}, maxColIndex={maxColIndex} or invalid originalCursor={originalCursor}", MxMsgs.MxErrBadMethodParam);
            else
            {
                var hint = ChapterModel.ChangeHint.Line;
                if (TextLines.Count == 0)
                    rc.SetResult(hint);
                else
                {
                    CursorPosition updatedCursor = new CursorPosition(originalCursor);
                    var previousRowIndex = GetPreviousLineIndex(cursorRowIndex);
                    var rowIndex = (previousRowIndex != Program.PosIntegerNotSet) ? previousRowIndex : cursorRowIndex;
                    do
                    {
                        var rcFill = FillShortLine(rowIndex, originalCursor, out var resultCursorFill);
                        if (rcFill.IsError(true))
                        {
                            rc += rcFill;
                            break;
                        }
                        else
                        {
                            if (rcFill.GetResult() == true)
                                hint = ChapterModel.ChangeHint.End;
                        }

                        var rcSplit = SplitLongLine(rowIndex, originalCursor, out var resultCursorSplit);
                        if (rcSplit.IsError(true))
                        {
                            rc += rcSplit;
                            break;
                        }
                        else
                        {
                            if ((originalCursor.RowIndex == rowIndex+1) && (rcFill.GetResult()))
                            {
                                updatedCursor = resultCursorFill?.Copy() ?? null;
                            }
                            if ((originalCursor.RowIndex == rowIndex) && (rcSplit.GetResult()))
                            {
                                updatedCursor = resultCursorSplit?.Copy() ?? null;
                            }
                            if (rcSplit.GetResult() == true)
                                hint = ChapterModel.ChangeHint.End;
                        }
                    }
                    while ((rowIndex = GetNextLineIndex(rowIndex)) != Program.PosIntegerNotSet);

                    if (rc.IsSuccess()) 
                    {
                        if (updatedCursor == null)
                            rc.SetError(1101302, MxError.Source.Program, $"updatedCursor is null", MxMsgs.MxErrInvalidCondition);
                        else
                        {
                            var rcCursor = SetCursorInChapter(updatedCursor.RowIndex, updatedCursor.ColIndex);
                            rc += rcCursor;
                            if (rcCursor.IsSuccess(true))  //SetCursorInChapter returns ChangeHint.Cursor - fixed in next release of MxReturnCode
                            {
                                rc.SetResult(hint);
                            }
                        }
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> FillShortLine(int rowIndex, CursorPosition originalCursor, out CursorPosition updatedCursor)
        {
            var rc = new MxReturnCode<bool>("Body.FillShortLine");

            var maxColIndex = EditAreaViewCursorLimit?.ColIndex ?? Program.PosIntegerNotSet;
            updatedCursor = ((originalCursor?.ColIndex ?? Program.PosIntegerNotSet) < (maxColIndex + 1)) ? new CursorPosition(originalCursor) : new CursorPosition(originalCursor?.RowIndex ?? Program.PosIntegerNotSet, maxColIndex + 1);

            if ((TextLines == null) ||  (maxColIndex < 0) || (maxColIndex >= CmdLineParamsApp.ArgTextEditorDisplayColsMax))
                rc.SetError(1101501, MxError.Source.Param, $"rowIndex={rowIndex}; maxColIndex={maxColIndex}", MxMsgs.MxErrInvalidCondition);
            else
            {
                if ((rowIndex < 0) || (rowIndex >= TextLines.Count) || (originalCursor == null) || (updatedCursor.IsValid(TextLines.Count, maxColIndex + 1) == false)) 
                    rc.SetError(1101502, MxError.Source.Param, $"rowIndex={rowIndex}; cursor={originalCursor?.ToString() ?? Program.ValueNotSet}", MxMsgs.MxErrBadMethodParam);
                else
                {
                    int nextRowIndex = Program.PosIntegerNotSet;
                    while ((nextRowIndex = GetNextLineIndex(rowIndex)) != Program.PosIntegerNotSet)
                    {
                        var spaceAtEndOfCurrentLine = (maxColIndex + 1) - ((TextLines[rowIndex].EndsWith(' ') == false) ? TextLines[rowIndex].Length : TextLines[rowIndex].Length + 1); //if current row ends with space then there will be two spaces between the last word of current row and first word from next nextLine
                        var nextLine = TextLines[nextRowIndex];

                        var splitIndex = Body.GetSplitIndexFromStart(nextLine, spaceAtEndOfCurrentLine); //splitIndex is index of a space in next line, or if entire line can fit then index of last char (inc paraBreak) 
                        if (splitIndex < 0)
                            break; //next nextLine cannot be split or is too long be put into current nextLine

                        var start = nextLine.Snip(0, splitIndex).TrimEnd();
                        if (string.IsNullOrEmpty(start) == false)
                            TextLines[rowIndex] += (TextLines[rowIndex].EndsWith(' ') == false) ? (" " + start) : start;

                        var end = nextLine.Snip(splitIndex + 1, nextLine.Length - 1);
                        if (string.IsNullOrEmpty(end) == false)
                            TextLines[nextRowIndex] = end;
                        else
                            TextLines.RemoveAt(nextRowIndex);

                        if (originalCursor.RowIndex == nextRowIndex) 
                            updatedCursor = UpdateCursorOnFillShortLine(rowIndex, nextRowIndex, originalCursor, start, end);

                        rc.SetResult(true);
                    }
                    if ((rc.IsSuccess()) && (rc.IsSuccess(true) == false))
                        rc.SetResult(false);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> SplitLongLine(int rowIndex, CursorPosition originalCursor, out CursorPosition updatedCursor)
        {
            var rc = new MxReturnCode<bool>("Body.SplitLongLine");

            var maxColIndex = EditAreaViewCursorLimit?.ColIndex ?? Program.PosIntegerNotSet;
            updatedCursor = ((originalCursor?.ColIndex ?? Program.PosIntegerNotSet) < (maxColIndex + 1)) ? new CursorPosition(originalCursor) : new CursorPosition(originalCursor?.RowIndex ?? Program.PosIntegerNotSet, maxColIndex + 1);
            if ((TextLines == null) || (maxColIndex < 0) || (maxColIndex >= CmdLineParamsApp.ArgTextEditorDisplayColsMax))
                rc.SetError(1101601, MxError.Source.Program, $"rowIndex={rowIndex}; maxColIndex={maxColIndex}", MxMsgs.MxErrInvalidCondition);
            else
            {
                if ((rowIndex < 0) || (rowIndex >= TextLines.Count) || (TextLines[rowIndex] == null) || (originalCursor == null) || (updatedCursor.IsValid(TextLines.Count, maxColIndex + 1) == false))
                    rc.SetError(1101602, MxError.Source.Param, $"rowIndex={rowIndex}; cursor={originalCursor?.ToString() ?? Program.ValueNotSet}", MxMsgs.MxErrBadMethodParam);
                else
                {
                    var currentLine = TextLines[rowIndex];
                    var currentLineLen = currentLine.EndsWith(ParaBreakChar) ? currentLine.Length - 1 : currentLine.Length;
                    if ((currentLineLen - 1) <= maxColIndex)
                        rc.SetResult(false);
                    else
                    {
                        string existLine, insertLine;
                        currentLineLen = currentLine.Length; //include any paraBreak
                        var splitIndex = Body.GetSplitIndexFromEnd(currentLine, maxColIndex);
                        if (splitIndex == Program.PosIntegerNotSet)
                        {                                   //line doesn't contain any spaces or otherwise cannot be split as word boundary
                            existLine = currentLine.Snip(0, maxColIndex); //cursor doesn't change as updatedCursor.colIndex must be <= maxColIndex
                            if (((insertLine = currentLine.Snip(maxColIndex + 1, currentLineLen - 1)) != null))
                               WordCount++;                //increment word count as word in currentLine is split into two
                        }
                        else
                        {
                            existLine = currentLine.Snip(0, splitIndex - 1); //space is removed
                            insertLine = currentLine.Snip(splitIndex + 1, currentLineLen - 1);
                        }
                        if (string.IsNullOrEmpty(existLine) || (string.IsNullOrEmpty(insertLine)))
                            rc.SetError(1101604, MxError.Source.Program, $"rowIndex={rowIndex} existLine.Length={existLine?.Length ?? -1}, insertLine.Length={insertLine?.Length ?? -1}", MxMsgs.MxErrInvalidCondition);
                        else
                        {
                            var cursorOffset = TextLines[rowIndex].Length - originalCursor.ColIndex;
                            TextLines[rowIndex] = existLine;
                            if (((insertLine.EndsWith(ParaBreakChar) != false) || ((rowIndex + 1) >= TextLines.Count)) == false)
                                TextLines[rowIndex + 1] = insertLine + " " + TextLines[rowIndex + 1]; //insertLine.Length may be > maxColIndex, but corrected on next call
                            else
                                rc += InsertLine(rowIndex, insertLine);
                            if (rc.IsSuccess())
                            {
                                if ((originalCursor.RowIndex == rowIndex) && (originalCursor.ColIndex > splitIndex)) //only update cursor when processing the first row in paragraph
                                {
                                    updatedCursor.ColIndex = insertLine.Length - cursorOffset; // (originalCursor.ColIndex <= maxColIndex) ? (originalCursor.ColIndex - (splitIndex + 1)) : (maxColIndex - (splitIndex - 1));
                                    updatedCursor.RowIndex++;
                                }
                                if ((updatedCursor.IsValid( TextLines.Count, maxColIndex + 1) == false))
                                    rc.SetError(1101605, MxError.Source.Program, $"rowIndex={rowIndex} updatedCursor={updatedCursor} invalid", MxMsgs.MxErrInvalidCondition);
                                else
                                    rc.SetResult(true);
                            }
                        }
                    }
                }
            }
            return rc;
        }


        public MxReturnCode<string> GetEditAreaCursorLineForDisplay()
        {
            var rc = new MxReturnCode<string>("Body.GetEditAreaCursorLineForDisplay", null);

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((linesCount == Program.PosIntegerNotSet) || (Cursor == null) || (Cursor.RowIndex >= linesCount))
                rc.SetError(1101701, MxError.Source.Program, $"TextLines.Count={linesCount}, Cursor.RowIndex={Cursor?.RowIndex ?? Program.PosIntegerNotSet}", MxMsgs.MxErrInvalidCondition);
            else
            {
                if (IsError())
                    rc.SetError(1101702, MxError.Source.Program, $"IsError() == true - Initialise not called? ", MxMsgs.MxErrInvalidCondition);
                else
                {
                    rc.SetResult((TextLines[Cursor.RowIndex].EndsWith(ParaBreakChar)) ? TextLines[Cursor.RowIndex].Replace(ParaBreakChar, ParaBreakDisplayChar) : TextLines[Cursor.RowIndex]);
                }
            }
            return rc;
        }

        public MxReturnCode<string[]> GetEditAreaLinesForDisplay(int displayLineCount)
        {
            var rc = new MxReturnCode<string[]>("Body.GetEditAreaLinesForDisplay", null);

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((linesCount == Program.PosIntegerNotSet) || (displayLineCount <= 0) || (displayLineCount > (EditAreaViewCursorLimit.RowIndex + 1)))
                rc.SetError(1101801, MxError.Source.Param, $"TextLines.Count={linesCount}; displayLineCount={displayLineCount} is 0 or is > EditAreaViewCursorLimit.RowIndex={EditAreaViewCursorLimit.RowIndex}+1", MxMsgs.MxErrBadMethodParam);
            else
            {
                if ((IsError() || (EditAreaTopLineChapterIndex < 0)))
                    rc.SetError(1101802, MxError.Source.Program, $"IsError() == true - Initialise not called? ", MxMsgs.MxErrInvalidCondition);
                else
                {
                    var lines = new string[displayLineCount];
                    if (linesCount > 0)
                    {
                        var chapterIndex = EditAreaTopLineChapterIndex;
                        for (var bufferIndex = 0; bufferIndex < displayLineCount; bufferIndex++)
                        {
                            if (chapterIndex >= linesCount) 
                                break;
                            lines[bufferIndex] = (TextLines[chapterIndex].EndsWith(ParaBreakChar)) ? TextLines[chapterIndex].Replace(ParaBreakChar, ParaBreakDisplayChar) : TextLines[chapterIndex];

                           // lines[bufferIndex] = lines[bufferIndex].Replace(' ', '.');

                            chapterIndex++;
                        }
                    }
                    rc.SetResult(lines);
                }
            }
            return rc;
        }

        public CursorPosition UpdateCursorOnFillShortLine(int fillRowIndex, int removeRowIndex, CursorPosition originalCursor, string moveText, string remainText = null)
        {
            CursorPosition rc = null;

            var lineCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            var maxCol = (EditAreaViewCursorLimit?.ColIndex ?? Program.PosIntegerNotSet) + 1;

            if ((TextLines != null) && (lineCount != Program.PosIntegerNotSet) && (fillRowIndex >= 0) && (fillRowIndex < lineCount) && (maxCol != Program.PosIntegerNotSet) && (string.IsNullOrEmpty(moveText) == false) && (originalCursor != null))
            {
                var updatedCursor = new CursorPosition(string.IsNullOrEmpty(remainText) ? fillRowIndex : removeRowIndex, 0);
                if (updatedCursor.RowIndex == fillRowIndex)
                    updatedCursor.ColIndex = TextLines[fillRowIndex].Length - (((moveText.Length - originalCursor.ColIndex) > 0) ? (moveText.Length - originalCursor.ColIndex) : 0);
                else
                    updatedCursor.ColIndex = ((originalCursor.ColIndex - (moveText.Length + 1)) > 0) ? (originalCursor.ColIndex - (moveText.Length + 1)) : 0;

                if (updatedCursor.IsValid(lineCount, remainText?.Length ?? maxCol))
                    rc = updatedCursor;
            }
            return rc;
        }

        public static int GetSplitIndexFromStart(string nextLine, int spaceAvailable)
        {
            var rc = Program.PosIntegerNotSet;

            if ((spaceAvailable > 0) && (string.IsNullOrEmpty(nextLine) == false))
            {
                var nextLineLen = (nextLine.EndsWith(Body.ParaBreak)) ? nextLine.Length : nextLine.Length+1; //nextLine always prepended with a space to ensure last word in current line is separated from first word in next line
                if (nextLineLen <= spaceAvailable)  
                    rc = nextLine.Length -1;
                else
                {
                    var splitIndex = Program.PosIntegerNotSet;
                    for (var x = 0; x < spaceAvailable; x++)      //spaceAvailable-1 allows for space between last word in current line and text from next line (ensure there are two words)
                    {
                        if (nextLine[x] == ' ') 
                            splitIndex = x;
                    }
                    rc = splitIndex;
                }
            }
            return rc;  //1) Program.PosIntegerNotSet, cannot be split; 2) nextLine.Length -1, can move entire line 3) index of last space 
        }

        public static int GetSplitIndexFromEnd(string line, int columnLimitIndex)  //refactor candidate
        {
            var rc = Program.PosIntegerNotSet;

            if ((line != null) && (columnLimitIndex > 0) && (columnLimitIndex < (line.Length)))
            {
                var splitIndex = line.Length - 1;
                var endIndex = line.Length - 1;
                for (var x = endIndex; x >= 0; x--)
                {
                    if (line[x] == ' ')
                        splitIndex = x;
                    if (splitIndex <= columnLimitIndex)
                        break;
                    if (x == 0)
                        splitIndex = Program.PosIntegerNotSet;
                }
                rc = (splitIndex < line.Length - 1) ? splitIndex : Program.PosIntegerNotSet;
            }
            return rc;
        }


        public static string GetErrorsInText(string text, int lineNo=-1, int colStartNo=1) //todo update after next release of MxReturnCode
        {
            string rc = null;

            var lineNoText = (lineNo == -1) ? "" : $"line {lineNo}: ";
            if (text == null)
                rc = $"{lineNoText}unexpected text (null). This is a program error. Please save your work and restart the program.";
            else
            {
                if (text.Length > KLineEditor.MaxSplitLineLength)
                    rc = $"{lineNoText}attempt to enter {text.Length} characters, but only {KLineEditor.MaxSplitLineLength} allowed.";
                else
                {
                    var index = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                    if ((index != -1)) // && (index != 0))
                        rc = $"{lineNoText}attempt to enter a new line at column {index + 1}.";
                    else
                    {
                        string output = new string(text.Where(c => ((IsEnteredCharacterValid(c, index == 0) == true))).ToArray());
                        if (output.Length != text.Length)
                            rc = $"{lineNoText}attempt to enter {text.Length - output.Length} disallowed characters.";
                        else
                        {
                            if ((index = text.IndexOf(DisallowedCharOpeningAngle)) != -1)
                            {
                                var colNoText = $"at column {colStartNo+index}.";
                                rc = $"{lineNoText}attempt to enter the disallowed character '{DisallowedCharOpeningAngle}' {colNoText}";
                            }
                            else
                            {
                                if ((index = text.IndexOf(DisallowedCharClosingAngle)) != -1)
                                {
                                    var colNoText = $"at column {colStartNo+index}.";
                                    rc = $"{lineNoText}attempt to enter the disallowed character '{DisallowedCharClosingAngle}' {colNoText}";
                                }
                            }
                        }
                    }
                }
            }
            return rc;
        }
        public static string GetErrorsInEnteredCharacter(char c)  //todo update after next release of MxReturnCode
        {
            string rc = null;

            if (IsEnteredCharacterValid(c) == false)
                rc = $"invalid character; 0x{(int) c:X}. This character cannot be typed into a chapter. Please delete it and try again.";
            else
            {
                if ((c == DisallowedCharOpeningAngle) || ((c == DisallowedCharClosingAngle)))
                    rc = $"disallowed character '{c}'. This character cannot be typed into a chapter. Please delete it and try again.";
            }
            return rc;
        }

        public static bool IsEnteredCharacterValid(char c, bool allowCr=false)
        {
            // ReSharper disable once ReplaceWithSingleAssignment.False
            var rc = false;

            if (allowCr && (c == 0xA) || (c == 0xD))
                rc = true;
            else
            {   //only supports english
                if ((c == ParaBreakChar) || (c == ' ') || (Char.IsLetterOrDigit(c) || (Char.IsPunctuation(c)) || (Char.IsSymbol(c))))
                {
                    rc = true;
                }
            }

            return rc;
        }

        public static int GetWordCountInLine(string line)
        {
            var rc = 0;
            if (String.IsNullOrEmpty(line) == false)
            {
                var text = line.Trim();
                int count = 0;
                bool wordItem = false;

                foreach (char c in text)
                {
                    if (Char.IsWhiteSpace(c) || (c == ParaBreakChar))
                        wordItem = false;
                    else
                    {
                        if (wordItem == false)
                            count++;
                        wordItem = true;
                    }
                }
                rc = count;
            }
            return rc;
        }

        public int RefreshWordCountInChapter()
        {
            var rc = 0;
            if (TextLines != null)
            {
                foreach (var line in TextLines)
                {
                    rc += GetWordCountInLine(line);
                }
                WordCount = rc;
            }
            return rc;
        }

        public int GetPageCount()
        {
            var rc = Program.PosIntegerNotSet;
            if ((TextLines != null) && (LinesPerPage > 0))
                rc = TextLines.Count / LinesPerPage;
            return rc;
        }

        public int GetLineCount()
        {
            return TextLines?.Count ?? Program.PosIntegerNotSet;
        }

        public int GetLineLength(int rowIndex)
        {
            var rc = Program.PosIntegerNotSet;

            if ((TextLines != null) && (rowIndex >= 0) && (rowIndex < TextLines.Count))
                rc = (TextLines[rowIndex].EndsWith(ParaBreakChar)) ? TextLines[rowIndex].Length - 1 : TextLines[rowIndex].Length;

            return rc;
        }

        public bool IsCursorAtEndOfParagraph()
        {
            var rc = false;
            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((linesCount != Program.PosIntegerNotSet) && (TextLines != null) && (Cursor != null) && (Cursor.RowIndex >= 0) && (Cursor.RowIndex < linesCount))
            {
                var line = TextLines[Cursor.RowIndex];
                if (line.EndsWith(ParaBreakChar) && (Cursor.ColIndex == line.Length-1 ))
                    rc = true;
            }
            return rc;
        }

        public int GetStartParaRowIndex(int rowIndex)
        {
            var rc = Program.PosIntegerNotSet;

            var lineCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((TextLines != null) && (lineCount > 0) && (rowIndex >= 0) && (rowIndex < lineCount))
            {
                rc = 0;
                while (--rowIndex >= 0)
                {
                    if (TextLines[rowIndex].EndsWith(ParaBreakChar))
                    {
                        rc = rowIndex+1;
                        break;
                    }
                }
            }
            return rc;
        }

        public int GetNextParaBreakRowIndex(int rowIndex)
        {
            var rc = Program.PosIntegerNotSet;

            var lineCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((TextLines != null) && (lineCount > 0) && (rowIndex >= 0) &&  (rowIndex < lineCount))
            {
                rc = lineCount - 1;
                while (rowIndex < lineCount)
                {
                    if (TextLines[rowIndex].EndsWith(ParaBreakChar))
                    {
                        rc = rowIndex;
                        break;
                    }
                    rowIndex++;
                }
            }
            return rc;
        }

        public string GetSelectedWord()
        {
            string rc = null;
            if ((TextLines.Count > 0) && (Cursor != null) && (TextLines.Count >= Cursor.RowIndex) && (TextLines[Cursor.RowIndex].Length >= Cursor.ColIndex))
            {
                var line = TextLines[Cursor.RowIndex];
                if ((string.IsNullOrEmpty(line) == false) && (line.Length > Cursor.ColIndex))
                {
                    var start = Cursor.ColIndex;
                    if (line[start] != ' ')
                    {
                        var end = start;
                        while ((start >= 0) && (line[start] != ' '))
                            start--;
                        start = (start < 0) ? 0: start + 1;
                        while ((end < line.Length) && (line[end] != ' ') && (line[end] != ParaBreakChar))
                            end++;
                        end = (end < line.Length) ? end - 1 : line.Length - 1;
                        if ((line[start] != ' ') && (line[end] != ' ') && (line[end] != ParaBreakChar))
                            rc = line.Snip(start, end);
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> InsertLine(int rowIndex, string line)
        {
            var rc = new MxReturnCode<bool>("Body.InsertLine");

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((string.IsNullOrEmpty(line)) || (line.Length - 1 > KLineEditor.MaxSplitLineLength))
                rc.SetError(1103001, MxError.Source.User, $"line {GetLineNumberFromCursor()} has {line?.Length ?? -1} characters; permitted range more than 0 and less than {KLineEditor.MaxSplitLineLength}");
            else
            {
                if (IsError() || (linesCount == Program.PosIntegerNotSet))
                    rc.SetError(1103002, MxError.Source.Program, "IsError() == true, Initialise not called?", MxMsgs.MxErrInvalidCondition);
                else
                {
                    try
                    {
                        if (linesCount >= Body.MaxTextLines)
                            rc.SetError(1103003, MxError.Source.User, $"too many lines in chapter={linesCount}", MxMsgs.MxWarnTooManyLines);
                        else
                        {
                            if (Body.GetErrorsInText(line) != null)
                                rc.SetError(1103004, MxError.Source.User, $"{MxReturnCodeUtils.WarningMsgPrecursor}{Body.GetErrorsInText(line, Cursor.RowIndex + 2)}");
                            else
                            {
                                if ((rowIndex + 1) >= linesCount)
                                    TextLines.Add(line);
                                else
                                    TextLines.Insert(rowIndex + 1, line);

                                rc.SetResult(true);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        rc.SetError(1103005, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                    }
                }
            }
            return rc;
        }

        private int GetNextLineIndex(int rowIndex)
        {
            var rc = Program.PosIntegerNotSet;

            if ((rowIndex >= 0) && (rowIndex < TextLines.Count) && (TextLines[rowIndex].EndsWith(ParaBreakChar) == false))
            {
                if ((rowIndex + 1) < TextLines.Count)
                    rc = rowIndex + 1;
            }
            return (rc != 0) ? rc : Program.PosIntegerNotSet;
        }

        private int GetPreviousLineIndex(int rowIndex)
        {
            var rc = Program.PosIntegerNotSet;

            var row = rowIndex - 1;
            if ((row >= 0) && (row < TextLines.Count) && (TextLines[row].EndsWith(ParaBreakChar) == false))
            {
                if ((row) < TextLines.Count)
                    rc = row;
            }
            return (rc != 0) ? rc : Program.PosIntegerNotSet;
        }

        private int GetMaxColCursorIndexForRow(int rowIndex)
        {
            var rc = Program.PosIntegerNotSet;
            if ((TextLines != null) && (rowIndex >= 0) && (rowIndex < (TextLines?.Count ?? -1)))
            {
                if (TextLines[rowIndex].EndsWith(ParaBreakChar))
                    rc = TextLines[rowIndex].Length - 1; //allow cursor to ParaBreakChar character
                else
                    rc = TextLines[rowIndex].Length;     //allow cursor one space beyond last character
            }
            return rc;
        }

        private bool IsCursorBeyondScrollLimit(int rowIndex)
        {
            var rc = false;
            if ((ScrollLimitChapter > 0) && (TextLines.Count > 0))
            {
                // if (((TextLines.Count - 1) - rowIndex) > (ScrollLimitChapter - 1))
                if (((TextLines.Count - 1) - rowIndex) <= (ScrollLimitChapter - 1))
                    rc = false;
                else
                {
                    rc = true;
                }
            }
            return rc;
        }

        private void SetTabSpaces(int count)
        {
            if (count > 0)
            {
                TabSpaces = "";
                for (int x = 0; x < count; x++)
                    TabSpaces += " ";
            }
        }

        private bool ResetCursorInChapter()
        {
            var rc = false;
            if (Cursor != null)
            {
                Cursor.RowIndex = 0;
                Cursor.ColIndex = 0;
                EditAreaTopLineChapterIndex = 0;
                rc = true;
            }
            return rc;
        }

        private int GetLineNumberFromCursor(bool indexNotYetInc = true)
        {
            var rc = Program.PosIntegerNotSet;
            if ((Cursor != null) && (Cursor.RowIndex >= 0))
                rc = (indexNotYetInc) ? (Cursor.RowIndex == 0) ? 1 : Cursor.RowIndex + 2 : Cursor.RowIndex + 1;
            return rc;
        }

        public int GetCharacterCountInRow(int rowIndex = Body.LastLine) 
        {
            var rc = Program.PosIntegerNotSet;

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((TextLines != null) &&(linesCount != Program.PosIntegerNotSet))
            {
                rowIndex = (rowIndex == Body.LastLine) ? linesCount - 1 : rowIndex;
                if ((rowIndex >= 0) && (rowIndex < linesCount) && (TextLines[rowIndex] != null))
                {
                    rc = (TextLines[rowIndex].EndsWith(ParaBreakChar)) ? TextLines[rowIndex].Length-1 : TextLines[rowIndex].Length; 
                }
            }
            return rc;
        }

        public char GetCharacterInLine(int lineNo = Body.LastLine, int columnNo = Body.LastColumn) 
        {
            var rc = Body.NullChar;

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((TextLines != null) && (linesCount != Program.PosIntegerNotSet))
            {
                var lineIndex = (lineNo != Body.LastLine) ? lineNo - 1 : linesCount - 1;
                if ((lineIndex >= 0) && (lineIndex < linesCount))
                {
                    var colIndex = (columnNo != Body.LastColumn) ? columnNo - 1 : (TextLines[lineIndex]?.Length - 1) ?? -1;
                    if ((colIndex >= 0) && (colIndex < TextLines[lineIndex]?.Length))
                        rc = TextLines[lineIndex][colIndex];
                }
            }

            return rc;
        }

        public static int GetIndexOfWord(string text, int wordNo =1, bool startIndex=true)      
        {
            var rc = Program.PosIntegerNotSet;
            if ((String.IsNullOrEmpty(text) == false) && (wordNo > 0))
            {
                var charIndex = 0;
                int wordCount = 0;
                bool wordItem = false;

                foreach (char c in text)
                {
                    if (Char.IsWhiteSpace(c))
                        wordItem = false;
                    else
                    {
                        if (wordItem == false)
                            wordCount++;
                        wordItem = true;
                    }
                    if (wordCount == wordNo)
                    {
                        if (startIndex)
                        {
                            rc = charIndex;
                            break;
                        }
                        if (wordItem == false)
                        {
                            rc = charIndex-1;
                            break;
                        }
                    }
                    charIndex++;
                }
                if ((rc == Program.PosIntegerNotSet) && (wordCount == wordNo) && (startIndex == false))
                    rc = text.Length - 1;
            }
            return rc;
        }
    }
}
