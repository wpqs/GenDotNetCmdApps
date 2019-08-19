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

        public static readonly int TextLinesPerPage = 36;                   //counted from Jack Kerouac's book 'On the Road'
        public static readonly int MaxTextLines = TextLinesPerPage * 2500;  //twice the number of pages in Tolstoy's 'War and Peace'


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
            Home,
            End
        }

        public List<string> TextLines { private set; get; }
        public CursorPosition Cursor { get; private set; }
        public CursorPosition EditAreaViewCursorLimit { get; private set; }

        public int EditAreaBottomChapterIndex { private set; get; }        //TextLines[TopDisplayLineIndex] is displayed as first line in console
        public int WordCount { private set; get; }
        public string TabSpaces { private set; get; }
        public char ParaBreakDisplayChar { private set; get; }

        private bool Error { set; get; }
        public bool IsError(){ return Error; }

        public Body()
        {
            TextLines = new List<string>();

            Cursor = new CursorPosition(0, 0);
            EditAreaViewCursorLimit = new CursorPosition(CmdLineParamsApp.ArgEditAreaLinesCountDefault, CmdLineParamsApp.ArgEditAreaLineWidthDefault); //updated by Model.Initialise

            EditAreaBottomChapterIndex = Program.PosIntegerNotSet;
            WordCount = Program.PosIntegerNotSet;
            SetTabSpaces(CmdLineParamsApp.ArgSpacesForTabDefault);
            ParaBreakDisplayChar = CmdLineParamsApp.ArgParaBreakDisplayCharDefault;

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

        public MxReturnCode<bool> Initialise(int editAreaLinesCount, int editAreaLineWidth, int spacesForTab = CmdLineParamsApp.ArgSpacesForTabDefault, char paraBreakDisplayChar = CmdLineParamsApp.ArgParaBreakDisplayCharDefault)
        {
            var rc = new MxReturnCode<bool>("Body.Initialise");

            if ((editAreaLineWidth == Program.PosIntegerNotSet) || (editAreaLinesCount == Program.PosIntegerNotSet) || (spacesForTab < CmdLineParamsApp.ArgSpacesForTabMin) || (paraBreakDisplayChar == NullChar))
                rc.SetError(1100201, MxError.Source.Param, $"editAreaLinesCount={editAreaLinesCount} not set, editAreaLineWidth={editAreaLineWidth} not set, or spacesForTab={spacesForTab} < min={CmdLineParamsApp.ArgSpacesForTabMin}, paraBreak is 0", MxMsgs.MxErrBadMethodParam);
            else
            {
                if ((TextLines == null) || (Cursor == null) || (EditAreaViewCursorLimit == null))
                    rc.SetError(1100202, MxError.Source.Program, $"TextLines, Cursor or EditAreaViewCursorLimit is null", MxMsgs.MxErrInvalidCondition);
                else
                {
                    EditAreaViewCursorLimit.RowIndex = editAreaLinesCount - 1;
                    EditAreaViewCursorLimit.ColIndex = editAreaLineWidth - 1;

                    SetTabSpaces(spacesForTab);
                    ParaBreakDisplayChar = paraBreakDisplayChar;

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
            var rc = new MxReturnCode<bool>("Body.Write");

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
                                string lastLine = null;
                                string line = null;
                                while ((line = file.ReadLine()) != null)
                                {
                                    lastLine = line;
                                    if (lastLine == ClosingElement)
                                        break;
                                    var rcLine = InsertLine(lastLine); 
                                    if (rcLine.IsError(true))
                                    {
                                        rc += rcLine;
                                        break;
                                    }
                                }
                                if (rc.IsSuccess())
                                {
                                    if (lastLine != ClosingElement)
                                        rc.SetError(1100404, MxError.Source.User, $"last line is {lastLine}", MxMsgs.MxErrInvalidCondition);
                                    else
                                        rc.SetResult(true);
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

    
        public MxReturnCode<bool> MoveCursorInChapter(CursorMove move)
        {
            var rc = new MxReturnCode<bool>("Body.MoveCursorInChapter");

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((linesCount == Program.PosIntegerNotSet) || (Cursor == null) || (EditAreaViewCursorLimit == null) || (Cursor.RowIndex < 0) || ((linesCount > 0) && (Cursor.RowIndex >= linesCount)) || (Cursor.ColIndex < 0) || (Cursor.ColIndex >= EditAreaViewCursorLimit.ColIndex))
                rc.SetError(1100501, MxError.Source.Program, $"TextLines, Cursor or EditAreaViewCursorLimit is null, or existing cursor is invalid; Cursor.RowIndex={Cursor?.RowIndex ?? -1} (max={linesCount}), Cursor.ColIndex={Cursor?.ColIndex ?? -1} (max={EditAreaViewCursorLimit?.ColIndex ?? -1})", MxMsgs.MxErrInvalidCondition);
            else
            {
                if (linesCount == 0)
                    rc.SetError(1100502, MxError.Source.User, Resources.MxWarnChapterEmpty);
                else
                {
                    try
                    {
                        switch (move)
                        {
                            case CursorMove.NextCol:
                            {
                                MxReturnCode<bool> rcCursor = null;
                                if ((Cursor.ColIndex + 1) <= GetMaxColCursorIndexForRow(Cursor.RowIndex))
                                    rcCursor = SetCursorInChapter(Cursor.RowIndex, Cursor.ColIndex + 1);
                                else
                                    rcCursor = SetCursorInChapter(Cursor.RowIndex + 1, 0);
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(true);
                                break;
                            }
                            case CursorMove.PreviousCol:
                            {
                                MxReturnCode<bool> rcCursor = null;
                                if ((Cursor.ColIndex - 1) >= 0)
                                    rcCursor = SetCursorInChapter(Cursor.RowIndex, Cursor.ColIndex - 1);
                                else
                                    rcCursor = SetCursorInChapter(Cursor.RowIndex - 1, GetMaxColCursorIndexForRow(Cursor.RowIndex - 1));
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(true);
                                break;
                            }
                            case CursorMove.NextRow:
                            {
                                var lastColIndex = GetMaxColCursorIndexForRow(Cursor.RowIndex + 1);
                                var rcCursor = SetCursorInChapter(Cursor.RowIndex + 1, (Cursor.ColIndex < lastColIndex) ? Cursor.ColIndex : lastColIndex);
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(true);
                                break;
                            }
                            case CursorMove.PreviousRow:
                            {
                                var lastColIndex = GetMaxColCursorIndexForRow(Cursor.RowIndex - 1);
                                var rcCursor = SetCursorInChapter(Cursor.RowIndex - 1, (Cursor.ColIndex < lastColIndex) ? Cursor.ColIndex : lastColIndex);
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(true);
                                break;
                            }
                            case CursorMove.Home:
                            {
                                var rcCursor = SetCursorInChapter(0, 0);
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(true);
                                break;
                            }
                            case CursorMove.End:
                            {
                                var rcCursor = SetCursorInChapter(linesCount - 1, GetMaxColCursorIndexForRow(linesCount - 1));
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(true);
                                break;
                            }
                            default:
                            {
                                rc.SetError(1100503, MxError.Source.Program, $"unsupported move={move}", MxMsgs.MxErrInvalidCondition);
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        rc.SetError(1100504, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> SetCursorInChapter(int rowIndex, int colIndex)
        {
            var rc = new MxReturnCode<bool>("Body.SetCursorInChapter");
                                        //rowIndex == -1 or rowIndex == TextLines.Count are errors needing specific reporting
            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((linesCount == Program.PosIntegerNotSet) || (rowIndex < -1) || (rowIndex > linesCount) || (colIndex < -1) || (colIndex > (EditAreaViewCursorLimit?.ColIndex+1 ?? colIndex-1))) //allow cursor to move one character after last permitted col
                rc.SetError(1100601, MxError.Source.Param, $"rowIndex={rowIndex} > max({linesCount}), colIndex={colIndex} > max({EditAreaViewCursorLimit?.ColIndex ?? colIndex-1}", MxMsgs.MxErrBadMethodParam);
            else
            {
                if ((Cursor == null) || (TextLines == null))
                    rc.SetError(1100602, MxError.Source.Program, $"Cursor or TextLines is null", MxMsgs.MxErrInvalidCondition);
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
                            rc.SetError(1100603, MxError.Source.User, Resources.MxWarnEndOfChapter);
                        else
                        {
                            if ((rowIndex == -1) || (colIndex == -1))
                                rc.SetError(1100604, MxError.Source.User, Resources.MxWarnStartOfChapter);
                            else
                            {
                                var lastColIndex = GetMaxColCursorIndexForRow(rowIndex);
                                if ((lastColIndex == Program.PosIntegerNotSet) || (colIndex > lastColIndex))
                                    rc.SetError( 1100605, MxError.Source.Program, $"GetLastColumnIndexForRow({rowIndex}) failed or colIndex={colIndex} > lastColIndex={lastColIndex}", MxMsgs.MxErrInvalidCondition);
                                else
                                {
                                    Cursor.RowIndex = rowIndex;
                                    Cursor.ColIndex = colIndex;
                                }
                            }
                        }
                    }
                    if (rc.IsSuccess())
                    {
                        var rcScroll = SetEditAreaBottomIndex(Scroll.ToCursor);
                        rc += rcScroll;
                        if (rcScroll.IsSuccess(true))
                            rc.SetResult(true);
                    }
                }
            }
            return rc;
        }


        public MxReturnCode<bool> SetEditAreaBottomIndex(Body.Scroll scroll = Body.Scroll.Bottom)
        {
            var rc = new MxReturnCode<bool>("Body.SetEditAreaBottomIndex");

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((linesCount == Program.PosIntegerNotSet) || (EditAreaViewCursorLimit == null) )
                rc.SetError(1100701, MxError.Source.Program, $"EditAreaViewCursorLimit or TextLines is null", MxMsgs.MxErrInvalidCondition);
            else
            {
                var displayHt = EditAreaViewCursorLimit.RowIndex + 1;
                switch (scroll)
                {
                    case Scroll.ToCursor:
                    {
                        EditAreaBottomChapterIndex = Cursor.RowIndex; 
                        rc.SetResult(true);
                        break;
                    }
                    case Scroll.Top:
                    {
                        if (EditAreaBottomChapterIndex != displayHt - 1)
                        {
                            EditAreaBottomChapterIndex = displayHt - 1;
                            rc.SetResult(true);
                        }
                        break;
                    }
                    case Scroll.Bottom:
                    {
                        if (EditAreaBottomChapterIndex != linesCount - 1)
                        {
                            EditAreaBottomChapterIndex = linesCount - 1;
                            rc.SetResult(true);
                        }
                        break;
                    }
                    case Scroll.LineUp:
                    {
                        if (EditAreaBottomChapterIndex + 1 > displayHt)
                        {
                            EditAreaBottomChapterIndex -= 1;
                            rc.SetResult(true);
                        }
                        break;
                    }
                    case Scroll.LineDown:
                    {
                        if (EditAreaBottomChapterIndex + 1 < linesCount)
                        {
                            EditAreaBottomChapterIndex += 1;
                            rc.SetResult(true);
                        }
                        break;
                    }
                    case Scroll.PageUp:
                    {
                        if (EditAreaBottomChapterIndex > displayHt - 1)
                        {
                            if (EditAreaBottomChapterIndex + 1 > ((displayHt - 1) * 2))
                                EditAreaBottomChapterIndex -= (displayHt == 1) ? 1 : displayHt - 1;
                            else
                                EditAreaBottomChapterIndex = displayHt - 1;
                            rc.SetResult(true);
                        }
                        break;
                    }
                    case Scroll.PageDown:
                    {
                        if (EditAreaBottomChapterIndex != linesCount - 1)
                        {
                            if (EditAreaBottomChapterIndex + (displayHt - 1) < linesCount)
                                EditAreaBottomChapterIndex += (displayHt == 1) ? 1 : displayHt - 1;
                            else
                                EditAreaBottomChapterIndex = linesCount - 1;
                            rc.SetResult(true);
                        }
                        break;
                    }
                    default:
                    {
                        rc.SetError(1100702, MxError.Source.Program, $"unsupported scroll={scroll}", MxMsgs.MxErrInvalidCondition);
                        break;
                    }
                }
            }
            return rc;
        }

        public CursorPosition GetCursorInEditArea()
        {
            CursorPosition rc = null;

            var displayHt = EditAreaViewCursorLimit?.RowIndex + 1 ?? Program.PosIntegerNotSet;
            var rowIndex = (EditAreaBottomChapterIndex < displayHt) ? Cursor.RowIndex : EditAreaBottomChapterIndex - Cursor.RowIndex;

            if ((displayHt > 0) && (rowIndex >= 0) && (rowIndex < displayHt))
                rc = new CursorPosition(rowIndex, Cursor.ColIndex);

            return rc;
        }

        public MxReturnCode<bool> InsertParaBreak()
        {
            var rc = new MxReturnCode<bool>("Body.InsertParaBreak");

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if (IsError() || (linesCount == Program.PosIntegerNotSet) || (Cursor == null) || (Cursor.RowIndex < 0) || (Cursor.RowIndex > ((linesCount == 0) ? 0 : linesCount - 1)))
                rc.SetError(1100801, MxError.Source.Program, $"IsError() == true, or invalid Cursor.RowIndex={Cursor?.RowIndex ?? -1}", MxMsgs.MxErrInvalidCondition);
            else
            {
                try
                {
                    if (linesCount >= Body.MaxTextLines)
                        rc.SetError(1100802, MxError.Source.User, $"too many lines in chapter={linesCount}", MxMsgs.MxWarnTooManyLines);
                    else
                    {
                        if (linesCount == 0)
                        {
                            TextLines.Add(ParaBreak);
                            rc.SetResult(true);
                        }
                        else
                        {
                            var currentLine = TextLines[Cursor.RowIndex]; //lines cannot be null or empty
                            if (string.IsNullOrEmpty(currentLine))
                                rc.SetError(1100803, MxError.Source.Program, $"Line {Cursor.RowIndex + 1} is null or empty", MxMsgs.MxErrInvalidCondition);
                            else
                            {
                                if ((Cursor.ColIndex < 0) || (Cursor.ColIndex > currentLine.Length))
                                    rc.SetError(1100804, MxError.Source.Program, $"Invalid Cursor.ColIndex={Cursor.ColIndex} for Cursor.RowIndex={Cursor.RowIndex} length={currentLine.Length}", MxMsgs.MxErrInvalidCondition);
                                else
                                {
                                    var remainderWordCount = WordCount - GetWordCountInLine(currentLine);
                                    var before = currentLine.Snip(0, Cursor.ColIndex - 1);
                                    var after = currentLine.Snip(Cursor.ColIndex, currentLine.Length - 1);

                                    WordCount = remainderWordCount + GetWordCountInLine(before) + GetWordCountInLine(after);

                                    TextLines[Cursor.RowIndex] = (before == null) ? ParaBreak : before + ParaBreak;
                                    TextLines.Insert(Cursor.RowIndex + 1, after ?? ParaBreak);

                                    //adjust rest of paragraph lines

                                    SetCursorInChapter(Cursor.RowIndex + 1, 0);

                                    rc.SetResult(true);
                                }
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1100805, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> InsertLine(string line)
        {
            var rc = new MxReturnCode<bool>("Body.InsertLine");

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((string.IsNullOrEmpty(line) ) || (line.Length-1 > EditAreaViewCursorLimit.ColIndex)) 
                rc.SetError(1100801, MxError.Source.User, $"line {GetLineNumberFromCursor()} has {line?.Length ?? -1} characters; permitted range more than 0 and less than {EditAreaViewCursorLimit.ColIndex + 1}");
            else
            {
                if (IsError() || (linesCount == Program.PosIntegerNotSet))
                    rc.SetError(1100802, MxError.Source.Program, "IsError() == true, Initialise not called?", MxMsgs.MxErrInvalidCondition);
                else
                {
                    try
                    {
                        if (linesCount >= Body.MaxTextLines)
                            rc.SetError(1100803, MxError.Source.User, $"too many lines in chapter={linesCount}", MxMsgs.MxWarnTooManyLines);
                        else
                        {
                            if (Body.GetErrorsInText(line) != null)
                                rc.SetError(1100804, MxError.Source.User, Body.GetErrorsInText(line, Cursor.RowIndex + 2));
                            else
                            {
                                var rowIndex = Cursor.RowIndex + 1;
                                if (rowIndex >= linesCount)
                                {
                                    TextLines.Add(line); 
                                    rowIndex = TextLines.Count - 1;
                                }
                                else
                                {
                                    if (Cursor.ColIndex == 0)
                                        TextLines.Insert(rowIndex, line); 
                                    else
                                    {
                                        var startLine = TextLines[Cursor.RowIndex].Substring(0, Cursor.ColIndex);
                                        var endLine = TextLines[Cursor.RowIndex].Substring(Cursor.ColIndex);
                                        TextLines[Cursor.RowIndex] = startLine;
                                        TextLines.Insert(rowIndex, line + endLine); 
                                    }
                                }

                                //LeftJustifyLinesInParagraph

                                WordCount += GetWordCountInLine(line);
                                var rcCursor = SetCursorInChapter(rowIndex, line.EndsWith(ParaBreakChar)? line.Length-1 : line.Length); // (line == ParaBreak) ? 0 : line.Length);  // line added so assume SetCursor succeeds; worst case user needs to click 'end'
                                rc += rcCursor;
                                if (rcCursor.IsSuccess(true))
                                    rc.SetResult(true);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        rc.SetError(1100805, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> InsertText(string text, bool insert = false)     //text is not null or empty and length < EditAreaViewCursorLimit.ColIndex
        {
            var rc = new MxReturnCode<bool>("Body.InsertText");

            var maxColIndex = EditAreaViewCursorLimit?.ColIndex ?? Program.PosIntegerNotSet;
            if ((string.IsNullOrEmpty(text) == true) || (text.Length-1 > maxColIndex))
                rc.SetError(1100901, MxError.Source.Param, $"text is null, or length={text?.Length ?? Program.PosIntegerNotSet} > maxColIndex={maxColIndex}", MxMsgs.MxErrBadMethodParam);
            else
            {
                var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
                if (IsError() || (TextLines == null) || (linesCount == Program.PosIntegerNotSet))
                    rc.SetError(1100902, MxError.Source.Program, $"Body not initialized", MxMsgs.MxErrInvalidCondition);
                else
                {
                    try
                    {
                        if (Body.GetErrorsInText(text) != null)
                            rc.SetError(1100903, MxError.Source.User, Body.GetErrorsInText(text, Cursor.RowIndex+1, Cursor.ColIndex+1));
                        else
                        {
                            if (linesCount <= 0)
                                rc += InsertParaBreak();

                            if (rc.IsError() || (Cursor.RowIndex < 0) || (Cursor.RowIndex >= TextLines.Count) || (Cursor.ColIndex < 0) || (Cursor.ColIndex > maxColIndex+1)) //allow cursor one char beyond last permitted char in line
                                rc.SetError(1100904, MxError.Source.Program, $"Invalid Cursor: Cursor.ColIndex={Cursor.ColIndex} (max={maxColIndex}) Cursor.RowIndex={Cursor.RowIndex} (max={linesCount-1})", MxMsgs.MxErrInvalidCondition);
                            else
                            {
                                var line = TextLines[Cursor.RowIndex];
                                var existWordCount = GetWordCountInLine(line);

                                if ((Cursor.ColIndex == line.Length) || insert)
                                    TextLines[Cursor.RowIndex] = line.Insert(Cursor.ColIndex, text);
                                else
                                {
                                    var paraBreak = line.EndsWith(ParaBreakChar) ? Body.ParaBreak : "";
                                    var start = line.Snip(0, Cursor.ColIndex - 1); //null if Cursor.ColIndex=0
                                    var end = line.Snip(Cursor.ColIndex + text.Length, line.Length-1); //null if text overwrites all text in line after Cursor.ColIndex
                                    if (end?.EndsWith(ParaBreakChar) ?? false)
                                        paraBreak = "";
                                    TextLines[Cursor.RowIndex] = (start ?? "") + text + (end ?? "") + paraBreak;
                                }
                                WordCount += GetWordCountInLine(TextLines[Cursor.RowIndex]) - existWordCount; //maybe less words now so += -3

                                //adjust lines to next ParaBreak 
                                LeftJustifyLinesInParagraph(Cursor.ColIndex + text.Length, maxColIndex+1, Cursor.RowIndex); 

                                //if (Cursor.ColIndex + text.Length <= maxColIndex)
                                //    Cursor.ColIndex = Cursor.ColIndex + text.Length;
                                //else
                                //{
                                //    Cursor.ColIndex = Cursor.ColIndex + text.Length - maxColIndex;
                                //    Cursor.RowIndex++;
                                //}
                                rc.SetResult(true);
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        rc.SetError(1100905, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                    }
                }
            }
            return rc;
        }


        public MxReturnCode<bool> DeleteLine(int rowIndex, bool moveCursorBack = false)
        {
            var rc = new MxReturnCode<bool>("Body.DeleteLine");

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((linesCount <= 0) || (TextLines == null)  || (Cursor == null) || (Cursor.RowIndex < 0) || (Cursor.RowIndex >= linesCount))
                rc.SetError(1101001, MxError.Source.Param, $"TextLines.Count={linesCount} or Cursor.RowIndex={Cursor?.RowIndex ?? -1} for TextLines.Count={linesCount}", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    if (moveCursorBack && (rowIndex == 0))
                        rc.SetError(1101002, MxError.Source.User, Resources.MxWarnStartOfChapter);
                    else
                    {
                        WordCount -= TextLines[rowIndex].EndsWith(ParaBreakChar) ? TextLines[rowIndex].Length-1 : TextLines[rowIndex].Length;
                        TextLines.RemoveAt(rowIndex);

                        //adjust lines to next ParaBreak

                        rowIndex = (((rowIndex > 0)) && ((moveCursorBack == true) || (rowIndex == TextLines.Count))) ? rowIndex - 1 : rowIndex;
                        var colIndex = ((moveCursorBack == true) && (TextLines.Count > 0) && (rowIndex < TextLines.Count)) ? GetMaxColCursorIndexForRow(rowIndex) : 0;

                        var rcCursor = SetCursorInChapter(rowIndex, colIndex);
                        rc += rcCursor;
                        if (rcCursor.IsSuccess(true))
                            rc.SetResult(true);
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1101003, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> DeleteCharacter()
        {
            var rc = new MxReturnCode<bool>("Body.DeleteCharacter");

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((linesCount == Program.PosIntegerNotSet) || (Cursor == null) || (Cursor.ColIndex < 0) || (Cursor.RowIndex < 0) || ((linesCount > 0) && (Cursor.RowIndex >= linesCount)))
                rc.SetError( 1101101, MxError.Source.Param, $"Cursor.RowIndex={Cursor?.RowIndex ?? -1}, Cursor.ColIndex={Cursor?.ColIndex ?? -1} for TextLines.Count={linesCount}", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (linesCount == 0)
                    rc.SetError(1101102, MxError.Source.User, Resources.MxWarnChapterEmpty);
                else
                {
                    try
                    {
                        if (GetCharacterCountInRow(Cursor.RowIndex) <= 1)
                        {
                            var rcDelLine = DeleteLine(Cursor.RowIndex, false);
                            rc += rcDelLine;
                            if (rcDelLine.IsSuccess(true))
                                rc.SetResult(true);
                        }
                        else
                        {
                            var line = TextLines[Cursor.RowIndex];
                            if (Cursor.ColIndex >= line.Length)
                                rc.SetError(1101103, MxError.Source.User, Resources.MxWarnNoCharToDelete);
                            else
                            {
                                var existWordCount = GetWordCountInLine(line);

                                var start = line.Snip(0, Cursor.ColIndex - 1);             //maybe null if Cursor.ColIndex=0
                                var end = line.Snip(Cursor.ColIndex + 1, line.Length - 1); //null only if line.Length=1 and Cursor.ColIndex=0
                                line = start + end;
                                if (string.IsNullOrEmpty(line))
                                    rc.SetError(1101104, MxError.Source.Program, $"start={start ?? "[null]"}, end={end ?? "[null]"}", MxMsgs.MxErrInvalidCondition);
                                else
                                {
                                    WordCount -= existWordCount - GetWordCountInLine(line);
                                    TextLines[Cursor.RowIndex] = line;

                                    //LeftJustifyLinesInParagraph - keep cursor position and adjust lines to next ParaBreak

                                    rc.SetResult(true);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        rc.SetError(1101104, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                    }
                }
            }
            return rc;
        }

        public bool LeftJustifyLinesInParagraph(int colIndex, int maxColWidth, int startRowIndex, int endRowIndex = Program.PosIntegerNotSet)
        {
            var rc = false;

            if ((colIndex >= 0) && (maxColWidth >= 0) && (maxColWidth <= (EditAreaViewCursorLimit?.ColIndex+1 ?? Program.PosIntegerNotSet)))
            {
                if ((endRowIndex = GetNextParaBreakRow(startRowIndex)) != Program.PosIntegerNotSet)
                {
                    var rowIndex = startRowIndex;
                    while (rowIndex <= endRowIndex) //(rc == false) && (rowIndex <= endRowIndex))
                    {
                        var line = TextLines[rowIndex];
                        if (line == null)
                            break;
                        var lineLen = line.EndsWith(ParaBreakChar) ? line.Length-1 : line.Length;
                        if (lineLen > maxColWidth) 
                        {           //line too long so split at last possible previous word and insert rest into next line, move cursor
                            var splitIndex = Body.GetSplitIndexFromEnd(line, lineLen + 1 - maxColWidth);
                            if (splitIndex == Program.PosIntegerNotSet)
                                break;
                            var existLine = line.Snip(0, splitIndex-1);
                            var insertLine = line.Snip(splitIndex + 1, line.Length - 1);

                            WordCount -= GetWordCountInLine(insertLine);
                            TextLines[rowIndex] = existLine;
                            InsertLine(insertLine); //change cursor
                            endRowIndex++;
                        }
                        else
                        {
                            if (rowIndex < endRowIndex)
                            {       //line too short so split next line and append its first part, no change to cursor
                                var nextLine = TextLines[rowIndex + 1];
                                var splitIndex = Body.GetSplitIndexFromStart(nextLine, maxColWidth - line.Length);
                                if (splitIndex == Program.PosIntegerNotSet)
                                    break;
                                TextLines[rowIndex] += nextLine.Snip(0, splitIndex);
                                TextLines[rowIndex + 1] = nextLine.Snip(splitIndex + 1, nextLine.Length - 1);
                            }
                            if (startRowIndex == rowIndex)
                                SetCursorInChapter(rowIndex, colIndex);

                        }
                        rowIndex++;
                    }
                    if (rowIndex - 1 == endRowIndex)
                    {
                      //  WordCount = GetWordCountInChapter();
                        rc = true;
                    }
                }
            }
            return rc;
        }

        public string SplitLine(int rowIndex, int splitIndex)
        {
            string rc = null;

            var lineCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((splitIndex >= 0) && (TextLines != null) && (lineCount != Program.PosIntegerNotSet) && (rowIndex >= 0) && (rowIndex < lineCount)) 
            {
                var lineEndIndex = TextLines[rowIndex].Length - 1;
                if (splitIndex < lineEndIndex)
                {
                    rc = TextLines[rowIndex].Substring(splitIndex + 1);
                    TextLines[rowIndex] = TextLines[rowIndex].Substring(0, splitIndex);
                }
            }
            return rc;
        }

        public MxReturnCode<string[]> GetEditAreaLinesForDisplay(int countFromBottom) 
        {
            var rc = new MxReturnCode<string[]>("Body.GetEditAreaLinesForDisplay", null);

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((linesCount == Program.PosIntegerNotSet) || (countFromBottom <= 0) || (countFromBottom > (EditAreaViewCursorLimit.RowIndex+1)))
                rc.SetError(1101301, MxError.Source.Param, $"TextLines.Count={linesCount}; countFromBottom={countFromBottom} is 0 or is > EditAreaViewCursorLimit.RowIndex={EditAreaViewCursorLimit.RowIndex}+1", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (IsError() || (EditAreaBottomChapterIndex <= Program.PosIntegerNotSet))
                    rc.SetError(1101301, MxError.Source.Program, $"IsError() == true, or TopDisplayLineIndex={EditAreaBottomChapterIndex} invalid - Initialise not called? ", MxMsgs.MxErrInvalidCondition);
                else
                {
                    var lines = new string[countFromBottom];
                    if (linesCount > 0)
                    {
                        var lineIndex = ((EditAreaBottomChapterIndex - EditAreaViewCursorLimit.RowIndex) > 0) ? EditAreaBottomChapterIndex - EditAreaViewCursorLimit.RowIndex : 0;
                        if (lineIndex + countFromBottom < linesCount)
                            lineIndex += linesCount - countFromBottom;
                        for (var bufferIndex = 0; bufferIndex < countFromBottom; bufferIndex++)
                        {
                            if (lineIndex < linesCount)
                            {
                                lines[bufferIndex] =  (TextLines[lineIndex].EndsWith(ParaBreakChar)) ? TextLines[lineIndex].Replace(ParaBreakChar, ParaBreakDisplayChar) : TextLines[lineIndex]; //(TextLines[lineIndex] == Environment.NewLine) ? ParaBreakDisplayChar.ToString(): TextLines[lineIndex];
                                lineIndex++;
                            }
                            else
                                break;
                        }
                    }
                    rc.SetResult(lines);
                }
            }
            return rc;
        }

        public static int GetSplitIndexFromStart(string line, int spaceNeeded)
        {
            var rc = Program.PosIntegerNotSet;

            if ((spaceNeeded > 0) && (line != null) && (line.Length > spaceNeeded))
            {
                var splitIndex = 0;
                var words = line.Split(Body.SpaceChar);

                for (var x = 0; x < words.Length; x++)
                {
                    if (words[x].Length == 0)
                        splitIndex++;
                    else
                    {
                        splitIndex += words[x].Length; // + 1;
                        if (splitIndex >= spaceNeeded)
                            break;
                    }
                }
                rc = splitIndex;
            }
            return rc;
        }

        public static int GetSplitIndexFromEnd(string line, int spaceNeeded)
        {
            var rc = Program.PosIntegerNotSet;

            if ((spaceNeeded > 0) && (line != null) && (line.Length > spaceNeeded))
            {
                var splitIndex = line.Length;
                var words = line.Split(Body.SpaceChar);

                for (var x = words.Length - 1; x >= 0; x--)
                {
                    if (words[x].Length == 0)
                        splitIndex--;
                    else
                    {
                        splitIndex -= words[x].Length + 1;
                        if ((line.Length - splitIndex) > spaceNeeded)
                            break;
                    }
                }
                rc = splitIndex;
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
                if (text.Length > CmdLineParamsApp.ArgEditAreaLineWidthMax)
                    rc = $"{lineNoText}attempt to enter {text.Length} characters, but only {CmdLineParamsApp.ArgEditAreaLineWidthMax} allowed.";
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


        public int GetLineCount()
        {
            return TextLines?.Count ?? Program.PosIntegerNotSet;
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

        private int GetWordCountInChapter()
        {
            var rc = 0;
            foreach (var line in TextLines)
            {
                rc += GetWordCountInLine(line);
            }
            return rc;
        }

        private int GetNextParaBreakRow(int rowIndex)
        {
            var rc = Program.PosIntegerNotSet;

            var lineCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((TextLines != null) && (rowIndex >= 0) && (rowIndex < lineCount))
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
                var rcBottom = SetEditAreaBottomIndex(Scroll.ToCursor);
                if (rcBottom.IsSuccess(true))
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


        public string GetWordInLine(int lineNo = Body.LastLine, int wordNo = Program.PosIntegerNotSet) //delete candidate
        {
            string rc = null;

            var linesCount = TextLines?.Count ?? Program.PosIntegerNotSet;
            if ((TextLines != null) && (linesCount != Program.PosIntegerNotSet))
            {
                var lineIndex = (lineNo != Body.LastLine) ? lineNo - 1 : linesCount - 1;
                if ((lineIndex >= 0) && (lineIndex < linesCount))
                {
                    var line = TextLines[lineIndex];
                    var lineWordCount = GetWordCountInLine(line);
                    wordNo = (wordNo == Program.PosIntegerNotSet) ? lineWordCount : wordNo;
                    if ((wordNo > 0) && (wordNo <= lineWordCount))
                    {
                        rc = line.Snip(GetIndexOfWord(line, wordNo), GetIndexOfWord(line, wordNo, false)) ?? null;
                    }
                }
            }

            return rc;
        }

        public char GetCharacterInLine(int lineNo = Body.LastLine, int columnNo = Body.LastColumn) //delete candidate
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

        public static int GetIndexOfWord(string text, int wordNo =1, bool startIndex=true)      //delete candidate
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

   
        //public int GetDisplayColIndex(ChapterModel.CursorState state = ChapterModel.CursorState.Current)
        //{
        //    var rc = Program.PosIntegerNotSet;

        //    if ((EditAreaViewCursorLimit?.ColIndex ?? Program.PosIntegerNotSet) != Program.PosIntegerNotSet)
        //    {
        //        switch (state)
        //        {
        //            case ChapterModel.CursorState.Current:
        //                {
        //                    if (Cursor.ColIndex <= EditAreaViewCursorLimit?.ColIndex)
        //                        rc = Cursor.ColIndex;
        //                    break;
        //                }
        //            case ChapterModel.CursorState.Next:
        //                {
        //                    if (Cursor.ColIndex + 1 <= EditAreaViewCursorLimit?.ColIndex)
        //                        rc = Cursor.ColIndex + 1;
        //                    break;
        //                }
        //            case ChapterModel.CursorState.Previous:
        //                {
        //                    if (Cursor.ColIndex > 0)
        //                        rc = Cursor.ColIndex - 1;
        //                    break;
        //                }
        //            default:
        //                {
        //                    break;
        //                }
        //        }
        //    }
        //    return rc;
        //}

        //public int GetEditAreaRowIndex(ChapterModel.CursorState state = ChapterModel.CursorState.Current)
        //{
        //    var rc = Program.PosIntegerNotSet; //return Program.PosIntegerNotSet if Cursor.RowIndex not in EditArea

        //    var lastLineIndex = (TextLines?.Count > 0) ? TextLines?.Count - 1 : 0;
        //    var rowMaxIndex = EditAreaViewCursorLimit?.RowIndex ?? Program.PosIntegerNotSet;
        //    if ((rowMaxIndex != Program.PosIntegerNotSet) && (lastLineIndex <= rowMaxIndex))
        //    {
        //        if ((EditAreaBottomChapterIndex < 0) || (EditAreaBottomChapterIndex > lastLineIndex))
        //            SetEditAreaBottomChapterIndex(Scroll.Bottom);  //make sure BottomDisplayLineIndex is set

        //        var displayRowIndex = EditAreaBottomChapterIndex - Cursor.RowIndex;
        //        switch (state)
        //        {
        //            case ChapterModel.CursorState.Current:
        //                {
        //                    if ((displayRowIndex >= 0) && (displayRowIndex <= lastLineIndex))
        //                        rc = displayRowIndex;
        //                    break;
        //                }
        //            case ChapterModel.CursorState.Next:
        //                {
        //                    if ((displayRowIndex >= 0) && ((displayRowIndex + 1) <= lastLineIndex))
        //                        rc = displayRowIndex + 1;
        //                    break;
        //                }
        //            case ChapterModel.CursorState.Previous:
        //                {
        //                    if ((displayRowIndex > 0) && ((displayRowIndex - 1) <= lastLineIndex))
        //                        rc = displayRowIndex - 1;
        //                    break;
        //                }
        //            default:
        //                {
        //                    break;
        //                }
        //        }
        //    }
        //    return rc;
        //}

    }
}
