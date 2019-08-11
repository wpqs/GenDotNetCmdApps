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
            Current,
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
        public char ParaBreakChar { private set; get; }

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
            ParaBreakChar = CmdLineParamsApp.ArgParaBreakCharDefault;

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

        public MxReturnCode<bool> Initialise(int editAreaLinesCount, int editAreaLineWidth, int spacesForTab = CmdLineParamsApp.ArgSpacesForTabDefault, char paraBreakChar = CmdLineParamsApp.ArgParaBreakCharDefault)
        {
            var rc = new MxReturnCode<bool>("Body.Initialise");

            if ((editAreaLineWidth == Program.PosIntegerNotSet) || (editAreaLinesCount == Program.PosIntegerNotSet) || (spacesForTab < CmdLineParamsApp.ArgSpacesForTabMin) || (paraBreakChar == NullChar))
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
                    ParaBreakChar = paraBreakChar;

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
                            file.WriteLine((line == Environment.NewLine) ? "" : line);
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
                                    var rcLine = InsertLine((lastLine.Length == 0) ? Environment.NewLine : lastLine);
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

            if ((TextLines == null) || (Cursor == null) || (EditAreaViewCursorLimit == null) || (Cursor.RowIndex < 0) || (Cursor.RowIndex >= TextLines.Count) || (Cursor.ColIndex < 0) || (Cursor.ColIndex >= EditAreaViewCursorLimit.ColIndex))
                rc.SetError(1100501, MxError.Source.Program, $"TextLines, Cursor or EditAreaViewCursorLimit is null, or existing cursor is invalid; Cursor.RowIndex={Cursor?.RowIndex ?? -1} (max={TextLines?.Count ?? -1}), Cursor.ColIndex={Cursor?.ColIndex ?? -1} (max={EditAreaViewCursorLimit?.ColIndex ?? -1})", MxMsgs.MxErrInvalidCondition);
            else
            {
                try
                {
                    switch (move)
                    {
                        case CursorMove.NextCol:
                        {
                            MxReturnCode<bool> rcCursor = null;
                            if ((Cursor.ColIndex + 1) <= GetLastColumnIndexForRow(Cursor.RowIndex))
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
                                rcCursor = SetCursorInChapter(Cursor.RowIndex - 1, GetLastColumnIndexForRow(Cursor.RowIndex - 1));
                            rc += rcCursor;
                            if (rcCursor.IsSuccess(true))
                                rc.SetResult(true);
                            break;
                            }
                        case CursorMove.NextRow:
                        {
                            var rcCursor = SetCursorInChapter(Cursor.RowIndex + 1, GetLastColumnIndexForRow(Cursor.RowIndex + 1));
                            rc += rcCursor;
                            if (rcCursor.IsSuccess(true))
                                rc.SetResult(true);
                            break;
                        }
                        case CursorMove.PreviousRow:
                        {
                            var rcCursor = SetCursorInChapter(Cursor.RowIndex - 1, GetLastColumnIndexForRow(Cursor.RowIndex - 1));
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
                            var rcCursor = SetCursorInChapter(TextLines.Count - 1, TextLines[TextLines.Count - 1].Length);
                            rc += rcCursor;
                            if (rcCursor.IsSuccess(true))
                                rc.SetResult(true);
                            break;
                        }
                        default:
                        {
                            rc.SetError(1100502, MxError.Source.Program, $"unsupported move={move}", MxMsgs.MxErrInvalidCondition);
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1100503, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> SetCursorInChapter(int rowIndex, int colIndex)
        {
            var rc = new MxReturnCode<bool>("Body.SetCursorInChapter");
                            //rowIndex == -1 or rowIndex == TextLines.Count are errors needing specific reporting
            if ((rowIndex < -1) || (rowIndex > (TextLines?.Count ?? rowIndex-1)) || (colIndex < 0) || (colIndex > (EditAreaViewCursorLimit?.ColIndex ?? colIndex-1)))
                rc.SetError(1100601, MxError.Source.Param, $"rowIndex={rowIndex} > max({TextLines?.Count ?? rowIndex - 1}), colIndex={colIndex} > max({EditAreaViewCursorLimit?.ColIndex ?? colIndex-1}", MxMsgs.MxErrBadMethodParam);
            else
            {
                if ((Cursor == null) || (TextLines == null))
                    rc.SetError(1100602, MxError.Source.Program, $"Cursor or TextLines is null", MxMsgs.MxErrInvalidCondition);
                else
                {
                    if (rowIndex == -1)
                        rc.SetError(1100603, MxError.Source.User, Resources.MxWarnStartOfChapter);
                    else
                    {
                        if (rowIndex == TextLines.Count)
                            rc.SetError(1100604, MxError.Source.User, Resources.MxWarnEndOfChapter);
                        else
                        {
                            var lastColIndex = GetLastColumnIndexForRow(rowIndex);
                            if ((lastColIndex == Program.PosIntegerNotSet) || (colIndex > lastColIndex))
                                rc.SetError(1100605, MxError.Source.Program, $"GetLastColumnIndexForRow({rowIndex}) failed or colIndex={colIndex} > lastColIndex={lastColIndex}", MxMsgs.MxErrInvalidCondition);
                            else
                            {
                                Cursor.RowIndex = rowIndex;
                                Cursor.ColIndex = colIndex;
                                var rcScroll = SetEditAreaBottomIndex(Scroll.ToCursor);
                                rc += rcScroll;
                                if (rcScroll.IsSuccess(true))
                                    rc.SetResult(true);
                            }
                        }
                    }
                }
            }
            return rc;
        }


        public MxReturnCode<bool> SetEditAreaBottomIndex(Body.Scroll scroll = Body.Scroll.Bottom)
        {
            var rc = new MxReturnCode<bool>("Body.SetEditAreaBottomIndex");

            if ((EditAreaViewCursorLimit == null) || (TextLines == null))
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
                        if (EditAreaBottomChapterIndex != TextLines.Count - 1)
                        {
                            EditAreaBottomChapterIndex = TextLines.Count - 1;
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
                        if (EditAreaBottomChapterIndex + 1 < TextLines.Count)
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
                        if (EditAreaBottomChapterIndex != TextLines.Count - 1)
                        {
                            if (EditAreaBottomChapterIndex + (displayHt - 1) < TextLines.Count)
                                EditAreaBottomChapterIndex += (displayHt == 1) ? 1 : displayHt - 1;
                            else
                                EditAreaBottomChapterIndex = TextLines.Count - 1;
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

        public MxReturnCode<bool> InsertLine(string line, bool atEndOfChapter = true)
        {
            var rc = new MxReturnCode<bool>("Body.InsertLine");

            if ((string.IsNullOrEmpty(line) ) || (line.Length+1 > EditAreaViewCursorLimit.ColIndex + 1)) //allow for cursor after end of line
                rc.SetError(1100801, MxError.Source.Param, $"line is null, empty or too long at {line?.Length ?? -1} characters; limit={EditAreaViewCursorLimit.ColIndex + 1}", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (IsError() || (TextLines == null))
                    rc.SetError(1100802, MxError.Source.Program, "IsError() == true, Initialise not called?", MxMsgs.MxErrInvalidCondition);
                else
                {
                    try
                    {
                        if (TextLines.Count >= Body.MaxTextLines)
                            rc.SetError(1100803, MxError.Source.User, $"too many lines in chapter={TextLines.Count}", MxMsgs.MxWarnTooManyLines);
                        else
                        {
                            if (Body.GetErrorsInText(line) != null)
                                rc.SetError(1100804, MxError.Source.User, Body.GetErrorsInText(line, Cursor.RowIndex + 2));
                            else
                            {
                                var rowIndex = Cursor.RowIndex + 1;
                                if (atEndOfChapter == false)
                                {
                                    var startLine = TextLines[Cursor.RowIndex].Substring(0, Cursor.ColIndex);
                                    var endLine = TextLines[Cursor.RowIndex].Substring(Cursor.ColIndex);
                                    TextLines[Cursor.RowIndex] = startLine;

                                    TextLines.Insert(rowIndex, (line == Environment.NewLine) ? endLine : line + endLine);

                                   // AutoLineBreak(Cursor.RowIndex+1, (line == Environment.NewLine) ? endLine : line+endLine);

                                   // 
                                    //get line @ Cursor.RowIndex
                                    //split line @ Cursor.ColIndex - existing, newline
                                    //AutoLineBreak(Cursor.RowIndex, (line == Environment.NewLine) ? "" ? line);


                                    //append split to line and further split if needed + cascade following lines


                                }
                                else
                                {
                                    TextLines.Add(line);
                                    rowIndex = TextLines.Count - 1;
                                }
                                WordCount += GetWordCountInLine(line);
                                SetCursorInChapter(rowIndex, (line == Environment.NewLine) ? 0 : line.Length); //line added so assume SetCursor succeeds; worst case user needs to click 'end'
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
            if ((string.IsNullOrEmpty(text) == true) || (text.Length > maxColIndex))
                rc.SetError(1100901, MxError.Source.Param, $"text is null, or length={text?.Length ?? Program.PosIntegerNotSet} > maxColIndex={maxColIndex}", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (IsError() || (TextLines == null))
                    rc.SetError(1100902, MxError.Source.Program, $"Body not initialized", MxMsgs.MxErrInvalidCondition);
                else
                {
                    try
                    {
                        if (Body.GetErrorsInText(text) != null)
                            rc.SetError(1100903, MxError.Source.User, Body.GetErrorsInText(text, Cursor.RowIndex+1, Cursor.ColIndex+1));
                        else
                        {
                            if ((TextLines?.Count ?? Program.PosIntegerNotSet) <= 0)
                            {
                                var rcStartChapter = InsertLine(text); //add first word in chapter, might be just a space char
                                rc += rcStartChapter;
                                if (rcStartChapter.IsSuccess(true))
                                    rc.SetResult(true);
                            }
                            else
                            {
                                if ((Cursor.RowIndex < 0) || (Cursor.RowIndex >= TextLines.Count))
                                    rc.SetError(1100904, MxError.Source.Program, $"Invalid Cursor.RowIndex={Cursor.RowIndex}, TextLines.Count={TextLines.Count}", MxMsgs.MxErrInvalidCondition);
                                else
                                {
                                    var line = (IsCursorAtEndOfParagraph()) ? "" : TextLines[Cursor.RowIndex];
                                    var insertText = (Cursor.ColIndex == line.Length) || insert;
                                    var existWordCount = GetWordCountInLine(line);

                                    //check if text can fit into line and if not split line
                                    if (line.Length >= (EditAreaViewCursorLimit.ColIndex - text.Length))
                                    {
                                        var rcAuto = AutoLineBreak(Cursor.RowIndex, text); //todo insert not append
                                        rc += rcAuto;
                                        if (rcAuto.IsSuccess(true))
                                            rc.SetResult(true);
                                    }
                                    else
                                    {                                                   
                                        var result = GetLineUpdateText(line, text, Cursor.ColIndex, maxColIndex, insertText);
                                        if (result != null)
                                        {
                                            WordCount += GetWordCountInLine(result) - existWordCount;
                                            TextLines[Cursor.RowIndex] = result;
                                            Cursor.ColIndex += text.Length;
                                            rc.SetResult(true);
                                        }
                                    }
                                }
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

        public string GetLineUpdateText(string existingText, string updateText, int updateIndex, int maxLength, bool insert)
        {
            string rc = null;   //typically returns null if updatedText cannot fit into the line or updateText contains invalid characters
            if ((updateText != null) && (existingText != null) && (updateIndex >= 0) && (updateIndex < maxLength) && (Body.GetErrorsInText(updateText) == null))
            {
                if (insert)
                {       //move all text at index text.length spaces right and insert at index
                    if ((existingText.Length + updateText.Length) <= maxLength)
                        rc = existingText.Insert(updateIndex, updateText);
                }
                else
                {       //overwrite from startindex = index to endIndex=index+word.Length-1
                    if (((updateIndex + updateText.Length) <= existingText.Length) && (existingText.Length <= maxLength))
                    {
                        var start = existingText.Snip(0, updateIndex - 1);
                        var end = existingText.Substring(updateIndex + updateText.Length);
                        if (((start?.Length ?? 0) + updateText.Length + ((end?.Length ?? 0)) <= maxLength))
                            rc = (start ?? "") + updateText + (end ?? "");
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> DeleteLine(int rowIndex, bool moveCursorBack = false)
        {
            var rc = new MxReturnCode<bool>("Body.DeleteLine");

            if (((TextLines?.Count ?? 0) <= 0) || (Cursor == null) || (Cursor.RowIndex < 0) || (Cursor.RowIndex >= TextLines.Count))
                rc.SetError(1101001, MxError.Source.Param, $"TextLines.Count={TextLines?.Count ?? -1} or Cursor.RowIndex={Cursor?.RowIndex ?? -1} for TextLines.Count={TextLines?.Count ?? -1}", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    if (moveCursorBack && (rowIndex == 0))
                        rc.SetError(1101002, MxError.Source.User, Resources.MxWarnStartOfChapter);
                    else
                    {
                        TextLines.RemoveAt(rowIndex);

                        rowIndex = (((rowIndex > 0)) && ((moveCursorBack == true) || (rowIndex == TextLines.Count))) ? rowIndex - 1 : rowIndex;
                        var colIndex = ((moveCursorBack == true) && (TextLines.Count > 0) && (rowIndex < TextLines.Count)) ? TextLines[rowIndex].Length : 0;

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

            if (((TextLines?.Count ?? 0) <= 0) || (Cursor == null) || (Cursor.RowIndex < 0) || (Cursor.RowIndex >= TextLines.Count))
                rc.SetError(1101101, MxError.Source.Param, $"TextLines.Count={TextLines?.Count ?? -1} or Cursor.RowIndex={Cursor?.RowIndex ?? -1} for TextLines.Count={TextLines?.Count ?? -1}", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    if (IsCursorAtEndOfParagraph())
                    {
                        var rcDelLine = DeleteLine(Cursor.RowIndex, false);
                        rc += rcDelLine;
                        if (rcDelLine.IsSuccess(true))
                            rc.SetResult(true);
                    }
                    else
                    {
                        var line = TextLines[Cursor.RowIndex];
                        var existWordCount = GetWordCountInLine(line);

                        var start = line.Snip(0, Cursor.ColIndex - 1);
                        var end = line.Snip(Cursor.ColIndex + 1, line.Length - 1);
                        line = start + end;

                        WordCount -= existWordCount - GetWordCountInLine(line);
                        TextLines[Cursor.RowIndex] = (line.Length == 0) ? Environment.NewLine : line;
                        rc.SetResult(true);
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1101104, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> AutoLineBreak(int rowIndex, string insertText)
        {
            var rc = new MxReturnCode<bool>("Body.AutoLineBreak");

            if ((rowIndex < 0) || (String.IsNullOrEmpty(insertText) == true) || (insertText.Length > (TabSpaces?.Length ?? Program.PosIntegerNotSet)))
                rc.SetError(1100701, MxError.Source.Param, $"lineIndex={rowIndex} is invalid, appendChar is NullorEmpty, or appendChar.Length={insertText?.Length ?? -1} > {TabSpaces?.Length ?? Program.PosIntegerNotSet}", MxMsgs.MxErrBadMethodParam);
            else
            {
                var breakIndex = GetLineBreakIndex(rowIndex, insertText.Length);
                if (breakIndex == Program.PosIntegerNotSet)
                    rc.SetError(1100702, MxError.Source.User, $"appendChar.Length={insertText.Length} > line length={GetCharacterCountInLine()} when LineWidth={EditAreaViewCursorLimit.ColIndex}", MxMsgs.MxErrLineTooLong);
                else
                {
                    var newLine = SplitLine(rowIndex, breakIndex);
                    if (newLine == null)
                        rc.SetError(1100703, MxError.Source.User, $"SplitLine({rowIndex}, {breakIndex}) is null for TextLines.Count={TextLines.Count}", MxMsgs.MxErrLineTooLong);
                    else
                    {
                        var rcInsert = InsertLine(newLine + insertText);  //todo cascade to end of para - take care as Loop here
                        rc += rcInsert;
                        if (rcInsert.IsSuccess(true))
                            rc.SetResult(true);
                    }
                }
            }
            return rc;
        }

        public int GetLineBreakIndex(int lineIndex, int spaceNeeded)
        {
            var rc = Program.PosIntegerNotSet;

            if ((lineIndex < TextLines.Count) && (spaceNeeded > 0) && (spaceNeeded + 1 < EditAreaViewCursorLimit.ColIndex)) //allow for space
            {
                var lineLen = TextLines[lineIndex].Length;
                var splitIndex = lineLen;
                var words = TextLines[lineIndex].Split(Body.SpaceChar);
                for (int x = words.Length - 1; x >= 0; x--)
                {
                    if (words[x].Length == 0)
                        splitIndex--;
                    else
                    {
                        splitIndex -= words[x].Length + 1;
                        if ((lineLen - splitIndex) > spaceNeeded)
                            break;
                    }
                }
                if ((lineLen - splitIndex) > spaceNeeded)
                    rc = splitIndex;
            }
            return rc;
        }
        public string SplitLine(int lineIndex, int splitIndex)
        {
            string rc = null;

            if (lineIndex < TextLines.Count)
            {
                var lineEndIndex = TextLines[lineIndex].Length - 1;
                if (splitIndex < lineEndIndex)
                {
                    rc = TextLines[lineIndex].Substring(splitIndex + 1);
                    WordCount -= GetWordCountInLine(rc);
                    TextLines[lineIndex] = TextLines[lineIndex].Substring(0, splitIndex);
                }
            }
            return rc;
        }

        public MxReturnCode<string[]> GetEditAreaLinesForDisplay(int countFromBottom) 
        {
            var rc = new MxReturnCode<string[]>("Body.GetEditAreaLinesForDisplay", null);

            if ((TextLines == null) || (countFromBottom <= 0) || (countFromBottom > (EditAreaViewCursorLimit.RowIndex+1)))
                rc.SetError(1100801, MxError.Source.Param, $"TextLines.Count={TextLines?.Count ?? -1}; countFromBottom={countFromBottom} is 0 or is > EditAreaViewCursorLimit.RowIndex={EditAreaViewCursorLimit.RowIndex}+1", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (IsError() || (EditAreaBottomChapterIndex <= Program.PosIntegerNotSet))
                    rc.SetError(1100802, MxError.Source.Program, $"IsError() == true, or TopDisplayLineIndex={EditAreaBottomChapterIndex} invalid - Initialise not called? ", MxMsgs.MxErrInvalidCondition);
                else
                {
                    var lines = new string[countFromBottom];
                    if (TextLines.Count > 0)
                    {
                        var lineIndex = ((EditAreaBottomChapterIndex - EditAreaViewCursorLimit.RowIndex) > 0) ? EditAreaBottomChapterIndex - EditAreaViewCursorLimit.RowIndex : 0;
                        if (lineIndex + countFromBottom < TextLines.Count)
                            lineIndex += TextLines.Count - countFromBottom;
                        for (var bufferIndex = 0; bufferIndex < countFromBottom; bufferIndex++)
                        {
                            if (lineIndex < TextLines.Count)
                            {
                                lines[bufferIndex] = (TextLines[lineIndex] == Environment.NewLine) ? ParaBreakChar.ToString(): TextLines[lineIndex];
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
                    if ((index != -1) && (index != 0))
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
                if ( (c == ' ') || (Char.IsLetterOrDigit(c) || (Char.IsPunctuation(c)) || (Char.IsSymbol(c))))
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
                    if (Char.IsWhiteSpace(c))
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
            if ((TextLines != null) && (Cursor != null) && (Cursor.RowIndex >= 0) && (Cursor.RowIndex < TextLines.Count))
            {
                if (TextLines[Cursor.RowIndex] == Environment.NewLine)
                    rc = true;
            }
            return rc;
        }

        public bool IsCursorAtEndOfLine()
        {
            var rc = false;
            //if cursor one character beyond last character in the line, or at EndOfParagraph mark?
            if ((TextLines != null) && (Cursor != null) && (Cursor.ColIndex >= 0) && (Cursor.RowIndex >= 0) && (Cursor.RowIndex < TextLines.Count))
                rc = (IsCursorAtEndOfParagraph()) ? (Cursor.ColIndex == 0) : (TextLines[Cursor.RowIndex].Length == Cursor.ColIndex);

            return rc;
        }

        private int RefreshWordCount()
        {
            var rc = 0;
            foreach (var line in TextLines)
            {
                rc += GetWordCountInLine(line);
            }
            WordCount = rc;
            return rc;
        }

        private int GetLastColumnIndexForRow(int rowIndex)
        {
            var rc = Program.PosIntegerNotSet;
            if ((TextLines != null) && (rowIndex >= 0) && (rowIndex < (TextLines?.Count ?? -1)))
                rc = (TextLines[rowIndex] == Environment.NewLine) ? 0 : TextLines[rowIndex].Length; //allow for cursor after last char
            return rc;
        }

        private int SetTabSpaces(int count)
        {
            if (count > 0)
            {
                TabSpaces = "";
                for (int x = 0; x < count; x++)
                    TabSpaces += " ";
            }
            return TabSpaces.Length;   
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

        public int GetCharacterCountInLine(int lineNo = Body.LastLine) //delete candidate
        {
            var rc = Program.PosIntegerNotSet;

            if (TextLines != null)
            {
                var lineIndex = (lineNo == Body.LastLine) ? TextLines.Count - 1 : lineNo - 1;
                if ((lineIndex >= 0) && (lineIndex < TextLines.Count) && (TextLines[lineIndex] != null))
                {
                    rc = (TextLines[lineIndex] == Environment.NewLine) ? 0 : TextLines[lineIndex].Length;
                }
            }
            return rc;
        }

        public string GetWordInLine(int lineNo = Body.LastLine, int wordNo = Program.PosIntegerNotSet) //delete candidate
        {
            string rc = null;

            var lineIndex = (lineNo != Body.LastLine) ? lineNo - 1 : TextLines.Count - 1;
            if ((lineIndex >= 0) && (lineIndex < TextLines.Count))
            {
                var line = TextLines[lineIndex];
                var lineWordCount = GetWordCountInLine(line);
                wordNo = (wordNo == Program.PosIntegerNotSet) ? lineWordCount : wordNo;
                if ((wordNo > 0) && (wordNo <= lineWordCount))
                {
                    rc = line.Snip(GetIndexOfWord(line, wordNo), GetIndexOfWord(line, wordNo, false)) ?? null;
                }
            }
            return rc;
        }

        public char GetCharacterInLine(int lineNo = Body.LastLine, int columnNo = Body.LastColumn) //delete candidate
        {
            var rc = Body.NullChar;

            var lineIndex = (lineNo != Body.LastLine) ? lineNo - 1 : TextLines.Count - 1;
            if ((lineIndex >= 0) && (lineIndex < TextLines.Count))
            {
                var colIndex = (columnNo != Body.LastColumn) ? columnNo - 1 : (TextLines[lineIndex]?.Length - 1) ?? -1;
                if ((colIndex >= 0) && (colIndex < TextLines[lineIndex]?.Length))
                    rc = TextLines[lineIndex][colIndex];
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
