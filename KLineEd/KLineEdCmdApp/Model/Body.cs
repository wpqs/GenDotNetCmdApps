using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Runtime.InteropServices.ComTypes;
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
    public class Body
    {
        public static readonly string OpeningElement = "<body>";
        public static readonly string ClosingElement = "</body>";

        public static readonly char DisallowedCharOpeningAngle = '<';
        public static readonly char DisallowedCharClosingAngle = '>';
        public static readonly int  DefaultTabSpaceCount = 3;

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
        }

        public List<string> TextLines { private set; get; }

        public int EditAreaBottomChapterIndex { private set; get; }        //TextLines[TopDisplayLineIndex] is displayed as first line in console
        public int WordCount { private set; get; }
        public string TabSpaces { private set; get; }

        public CursorPosition Cursor { get; private set; }
        public CursorPosition EditAreaViewCursorLimit { get; private set; }

        private bool Error { set; get; }
        public bool IsError(){ return Error; }

        public Body()
        {
            TextLines = new List<string>();
            EditAreaBottomChapterIndex = Program.PosIntegerNotSet;
            WordCount = Program.PosIntegerNotSet;
            SetTabSpaces(DefaultTabSpaceCount);
            Cursor = new CursorPosition(0, 0);
            EditAreaViewCursorLimit = new CursorPosition(CmdLineParamsApp.ArgEditAreaLinesCountDefault, CmdLineParamsApp.ArgEditAreaLineWidthDefault); //updated by Model.Initialise

            Error = true;
        }

        public MxReturnCode<bool> Initialise(int editAreaLinesCount, int editAreaLineWidth) //add TabSpaceCount as param
        {
            var rc = new MxReturnCode<bool>("Body.Initialise");

            if ((editAreaLineWidth == Program.PosIntegerNotSet) || (editAreaLinesCount == Program.PosIntegerNotSet))
                rc.SetError(1100101, MxError.Source.Param, $"editAreaLinesCount={editAreaLinesCount} not set, editAreaLineWidth={editAreaLineWidth} not set", MxMsgs.MxErrBadMethodParam);
            else
            {
                EditAreaViewCursorLimit.RowIndex = editAreaLinesCount-1;
                EditAreaViewCursorLimit.ColIndex = editAreaLineWidth-1;

                if(SetCursorInChapter(0, 0) == false)
                    rc.SetError(1100102, MxError.Source.Program, $"editAreaLinesCount={editAreaLinesCount} < 0, editAreaLineWidth={editAreaLineWidth} < 0", MxMsgs.MxErrInvalidCondition);
                else
                {
                    WordCount = 0;
                    SetTabSpaces(3);    //TabSpaceCount
                    Error = false;
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> Write(StreamWriter file)
        {
            var rc = new MxReturnCode<bool>("Body.Write");

            if (file == null)
                rc.SetError(1100201, MxError.Source.Param, "file is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    if (IsError())
                        rc.SetError(1100202, MxError.Source.Program, "IsError() == true, Initialise not called?", MxMsgs.MxErrInvalidCondition);
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
                    rc.SetError(1100203, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }

            return rc;
        }

        public MxReturnCode<bool> Read(StreamReader file)
        {
            var rc = new MxReturnCode<bool>("Body.Read");

            if (file == null)
                rc.SetError(1100301, MxError.Source.Param, "file is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    if (IsError() || (TextLines == null))
                        rc.SetError(1100302, MxError.Source.Program, "IsError() == true or TextLines == null, Initialise not called?", MxMsgs.MxErrInvalidCondition);
                    else
                    {
                        var firstLine = file.ReadLine();
                        if (firstLine != OpeningElement)
                            rc.SetError(1100303, MxError.Source.User, $"first line is {firstLine}", MxMsgs.MxErrInvalidCondition);
                        else
                        {
                            string lastLine = null;
                            string line = null;
                            while ((line = file.ReadLine()) != null)
                            {
                                lastLine = line;
                                if (lastLine.Length == 0)
                                    lastLine = Environment.NewLine;
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
                                    rc.SetError(1100304, MxError.Source.User, $"last line is {lastLine}", MxMsgs.MxErrInvalidCondition);
                                else
                                {
                                    var colIndex = (TextLines.Count > 0) ? TextLines[TextLines.Count - 1]?.Length ?? Program.PosIntegerNotSet : 0;
                                    var rowIndex = (TextLines.Count > 0) ? TextLines.Count - 1 : 0;
                                    if (SetCursorInChapter(rowIndex, colIndex) == false)
                                        rc.SetError(1100305, MxError.Source.Program, $"SetCursorInChapter(rowIndex={rowIndex}, colIndex={colIndex}) failed", MxMsgs.MxErrInvalidCondition);
                                    else
                                        rc.SetResult(true);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1100306, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
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

        public bool MoveCursorInChapter(CursorMove move)
        {
            var rc = false;

            if ((TextLines != null) && (EditAreaViewCursorLimit != null))
            {
                switch (move)
                {
                    case CursorMove.NextCol:
                    {
                        var rowIndex = Cursor.RowIndex;
                        var colIndex = Cursor.ColIndex + 1;
                        var lastColIndex = TextLines[rowIndex]?.Length - 1 ?? Program.PosIntegerNotSet;
                        if ((colIndex > lastColIndex) && (rowIndex+1 < TextLines.Count))
                        {
                            rowIndex++;
                            colIndex = 0;
                        }
                        rc = SetCursorInChapter(rowIndex, colIndex);
                        break;
                    }
                    case CursorMove.PreviousCol:
                    {
                        var rowIndex = Cursor.RowIndex;
                        var colIndex = Cursor.ColIndex - 1;
                        if ((colIndex < 0) && (rowIndex - 1 >= 0))
                        {
                            rowIndex--;
                            colIndex = TextLines[rowIndex]?.Length - 1 ?? Program.PosIntegerNotSet;
                        }
                        rc = SetCursorInChapter(rowIndex, colIndex);
                        break;
                    }
                    case CursorMove.NextRow:
                    {
                        var rowIndex = Cursor.RowIndex+1;
                        if (rowIndex < TextLines.Count)
                        {
                            var lastColIndex = TextLines[rowIndex]?.Length - 1 ?? Program.PosIntegerNotSet;
                            rc = SetCursorInChapter(rowIndex, (Cursor.ColIndex > lastColIndex) ? lastColIndex : Cursor.ColIndex);
                        }
                        break;
                    }
                    case CursorMove.PreviousRow:
                    {
                        var rowIndex = Cursor.RowIndex - 1;
                        if (rowIndex >= 0)
                        {
                            var lastColIndex = TextLines[rowIndex]?.Length - 1 ?? Program.PosIntegerNotSet;
                            rc = SetCursorInChapter(rowIndex, (Cursor.ColIndex > lastColIndex) ? lastColIndex : Cursor.ColIndex);
                        }
                        break;
                    }
                    default:
                    {
                        rc = true;
                        break;
                    }
                }
            }
            return rc;
        }

        public bool SetCursorInChapter(int rowIndex, int colIndex)
        {
            var rc = false;
            if (   (TextLines != null) && (EditAreaViewCursorLimit != null)
                && (colIndex >= 0) && (colIndex <= EditAreaViewCursorLimit.ColIndex)
                && (rowIndex >= 0) && (rowIndex <= TextLines.Count))
            {
                var lineLen = (TextLines.Count == 0) ? 0 : TextLines[rowIndex]?.Length ?? Program.PosIntegerNotSet;
                if (colIndex <= lineLen)
                {
                    Cursor.RowIndex = rowIndex;
                    Cursor.ColIndex = colIndex;
                    rc = SetEditAreaBottomChapterIndex(Scroll.ToCursor); 
                }
            }
            return rc;
        }

        public int GetDisplayColIndex(ChapterModel.CursorState state = ChapterModel.CursorState.Current)
        {
            var rc = Program.PosIntegerNotSet;

            if ((EditAreaViewCursorLimit?.ColIndex ?? Program.PosIntegerNotSet) != Program.PosIntegerNotSet)
            {
                switch (state)
                {
                    case ChapterModel.CursorState.Current:
                    {
                        if (Cursor.ColIndex <= EditAreaViewCursorLimit?.ColIndex)
                            rc = Cursor.ColIndex;
                        break;
                    }
                    case ChapterModel.CursorState.Next:
                    {
                        if (Cursor.ColIndex + 1 <= EditAreaViewCursorLimit?.ColIndex)
                            rc = Cursor.ColIndex + 1;
                        break;
                    }
                    case ChapterModel.CursorState.Previous:
                    {
                        if (Cursor.ColIndex > 0)
                            rc = Cursor.ColIndex - 1;
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
            return rc;
        }

        public int GetEditAreaRowIndex(ChapterModel.CursorState state = ChapterModel.CursorState.Current)
        {
            var rc = Program.PosIntegerNotSet; //return Program.PosIntegerNotSet if Cursor.RowIndex not in EditArea

            var lastLineIndex = (TextLines?.Count > 0) ? TextLines?.Count - 1 : 0;
            var rowMaxIndex = EditAreaViewCursorLimit?.RowIndex ?? Program.PosIntegerNotSet;
            if ((rowMaxIndex != Program.PosIntegerNotSet) && (lastLineIndex <= rowMaxIndex))
            {
                if ((EditAreaBottomChapterIndex < 0) || (EditAreaBottomChapterIndex > lastLineIndex))
                    SetEditAreaBottomChapterIndex(Scroll.Bottom);  //make sure BottomDisplayLineIndex is set

                var displayRowIndex = EditAreaBottomChapterIndex - Cursor.RowIndex;
                switch (state)
                {
                    case ChapterModel.CursorState.Current:
                    {
                        if ((displayRowIndex >= 0) && (displayRowIndex <= lastLineIndex) )
                            rc = displayRowIndex;
                        break;
                    }
                    case ChapterModel.CursorState.Next:
                    {
                        if ((displayRowIndex >= 0) && ((displayRowIndex + 1) <= lastLineIndex))
                             rc = displayRowIndex + 1;
                        break;
                    }
                    case ChapterModel.CursorState.Previous:
                    {
                        if ((displayRowIndex > 0) && ((displayRowIndex - 1) <= lastLineIndex))
                            rc = displayRowIndex - 1;
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
            return rc;
        }


        public bool SetEditAreaBottomChapterIndex(Body.Scroll scroll = Body.Scroll.Bottom)
        {
            var rc = false;
            if ((TextLines != null) && (EditAreaViewCursorLimit != null))
            {
                var displayHt = EditAreaViewCursorLimit.RowIndex + 1;

                switch (scroll)
                {
                    case Scroll.ToCursor:
                    {
                        EditAreaBottomChapterIndex = Cursor.RowIndex; 
                        rc = true;
                        break;
                    }
                    case Scroll.Top:
                    {
                        if (EditAreaBottomChapterIndex != displayHt - 1)
                        {
                            EditAreaBottomChapterIndex = displayHt - 1;
                            rc = true;
                        }
                        break;
                    }
                    case Scroll.Bottom:
                    {
                        if (EditAreaBottomChapterIndex != TextLines.Count - 1)
                        {
                            EditAreaBottomChapterIndex = TextLines.Count - 1;
                            rc = true;
                        }
                        break;
                    }
                    case Scroll.LineUp:
                    {
                        if (EditAreaBottomChapterIndex + 1 > displayHt)
                        {
                            EditAreaBottomChapterIndex -= 1;
                            rc = true;
                        }
                        break;
                    }
                    case Scroll.LineDown:
                    {
                        if (EditAreaBottomChapterIndex + 1 < TextLines.Count)
                        {
                            EditAreaBottomChapterIndex += 1;
                            rc = true;
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
                            rc = true;
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
                            rc = true;
                        }
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> InsertLine(string line, bool atEndOfChapter = true)
        {
            var rc = new MxReturnCode<bool>("Body.InsertLine");

            if ((string.IsNullOrEmpty(line) ) || (line.Length > EditAreaViewCursorLimit.ColIndex + 1))
                rc.SetError(1100401, MxError.Source.Param, $"line is null, empty or too long at {line?.Length ?? -1} characters; limit={EditAreaViewCursorLimit.ColIndex + 1}", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (IsError())
                    rc.SetError(1100402, MxError.Source.Program, "IsError() == true, Initialise not called?", MxMsgs.MxErrInvalidCondition);
                else
                {
                    if (TextLines.Count >= Body.MaxTextLines)
                        rc.SetError(1100403, MxError.Source.User, $"too many lines in chapter={TextLines.Count}", MxMsgs.MxWarnTooManyLines);
                    else
                    {
                        if (Body.GetErrorsInEnteredText(line) != null)
                            rc.SetError(1100404, MxError.Source.User, Body.GetErrorsInEnteredText(line, "line", Cursor.RowIndex + 1));
                        else
                        {
                            var columnIndex = (line == Environment.NewLine) ? 0 : line.Length - 1;
                            var rowIndex = Cursor.RowIndex + 1;
                            if (atEndOfChapter == false)
                                TextLines.Insert(rowIndex, line);
                            else
                            {
                                TextLines.Add(line);
                                rowIndex = TextLines.Count - 1;
                            }
                            if (SetCursorInChapter(rowIndex, columnIndex))
                            {
                                WordCount += GetWordCountInLine(line);
                                rc.SetResult(true);
                            }
                        }
                    }
                }
            }
            return rc;
        }

        public bool SetDelChar(bool backspace = false)
        {
            var rc = false;

            if ((TextLines != null) && ((backspace == false) || (Cursor.ColIndex > 0)))
            {
                if (backspace)
                    Cursor.ColIndex--;
                var result = Body.GetLineUpdateDeleteChar(TextLines[Cursor.RowIndex], Cursor.ColIndex);
                if (result != null)
                {
                    TextLines[Cursor.RowIndex] = result;
                    rc = true;
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

        public bool SetText(string str, bool insert = false, bool addSpaceBefore = true, bool addSpaceAfter = true)
        {
            var rc = false;

            var maxColIndex = EditAreaViewCursorLimit?.ColIndex ?? Program.PosIntegerNotSet;
            if ((str != null) && (TextLines != null) && (maxColIndex != Program.PosIntegerNotSet))
            {
                if ((Cursor.RowIndex >= 0) && (Cursor.RowIndex < (TextLines?.Count ?? -1)))
                {
                    var text = $"{((addSpaceBefore) ? " " : "")}{str}{((addSpaceAfter) ? " " : "")}";
                    var line = TextLines[Cursor.RowIndex];
                    var insertText = (Cursor.ColIndex == (line?.Length ?? Program.PosIntegerNotSet)) || insert;

                    //check if text can fit into line and if not split line

                    var result = Body.GetLineUpdateText(line, text, Cursor.ColIndex, maxColIndex, insertText);
                    if (result != null)
                    {
                        TextLines[Cursor.RowIndex] = result;
                        Cursor.ColIndex += text.Length;
                        rc = true;
                    }
                }
            }
            return rc;
        }


        public MxReturnCode<bool> AppendWord(string word)  //called only from ChapterModel.AppendWord()
        {
            var rc = new MxReturnCode<bool>("Body.AppendWord");

            if (String.IsNullOrWhiteSpace(word)) 
                rc.SetError(1100501, MxError.Source.Param, "word is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (IsError())
                    rc.SetError(1100502, MxError.Source.Program, "IsError() == true, Initialise not called?", MxMsgs.MxErrInvalidCondition);
                else
                {
                    var wordLength = word.Length;
                    if (wordLength > EditAreaViewCursorLimit.ColIndex+1)
                        rc.SetError(110503, MxError.Source.User, $"word.Length={wordLength} > LineWidth={EditAreaViewCursorLimit.ColIndex}", MxMsgs.MxErrLineTooLong);
                    else
                    {
                        if (Body.GetErrorsInEnteredText(word, "word") != null)
                            rc.SetError(1100504, MxError.Source.User, Body.GetErrorsInEnteredText(word));
                        else
                        {
                            if ((TextLines.Count == 0) || (TextLines[TextLines.Count - 1].Length + word.Length > EditAreaViewCursorLimit.ColIndex))
                            {                                              //word makes line too long, so append it to a new line
                                var rcInsert = InsertLine(word);
                                rc += rcInsert;
                                if (rcInsert.IsSuccess(true))
                                    rc.SetResult(true);
                            }
                            else
                            {
                                if (AddWord(word) == false)
                                    rc.SetError(1100506, MxError.Source.Program, $"word={word ?? "[null]"} is NullorEmpty", MxMsgs.MxErrInvalidCondition);
                                else
                                    rc.SetResult(true);
                            }
                        }
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> AppendChar(char c)  //called only from ChapterModel.AppendChar()
        {
            var rc = new MxReturnCode<bool>("Body.AppendChar");

            if (Body.GetErrorsInEnteredCharacter(c) != null)
                rc.SetError(1100601, MxError.Source.User, Body.GetErrorsInEnteredCharacter(c));
            else
            {
                var lineIndex = TextLines.Count - 1;
                var appendChar = (Char.IsWhiteSpace(c)) ? ((c == '\t') ? TabSpaces : Body.SpaceChar.ToString()) :  c.ToString();
                if (appendChar.Length <= 0)
                    rc.SetError(1100602, MxError.Source.Program, $"appendChar={appendChar} for c={c} length <= 0", MxMsgs.MxErrInvalidCondition);
                else
                {
                    var lastChar = GetCharacterInLine();                     //get last character in last line
                    if ((lastChar == Body.NullChar) || (lineIndex < 0))
                    {                                                   
                        if (AddFirstCharToChapter(appendChar) == false)
                            rc.SetError(1100603, MxError.Source.Program, $"AddFirstCharToChapter({appendChar ?? "[null]"}) failed", MxMsgs.MxErrInvalidCondition);
                        else
                            rc.SetResult(true);
                    }
                    else
                    {
                        if (TextLines[lineIndex].Length >= (EditAreaViewCursorLimit.ColIndex - appendChar.Length))
                        {
                            var rcAuto = AutoLineBreak(lineIndex, appendChar);
                            rc += rcAuto;
                            if (rcAuto.IsSuccess(true))
                                rc.SetResult(true);
                        }
                        else
                        {
                            if (lastChar != Body.SpaceChar)
                            {
                                TextLines[lineIndex] += appendChar;     //append char to existing word
                                rc.SetResult(true);
                            }
                            else
                            {                                           //space, so start new word
                                if (AddWord(appendChar) == false)
                                    rc.SetError(1100604, MxError.Source.Program, $"appendChar={appendChar ?? "[null]"} is NullorEmpty", MxMsgs.MxErrInvalidCondition);
                                else
                                    rc.SetResult(true);
                            }
                        }
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> AutoLineBreak(int lineIndex, string appendChar)
        {
            var rc = new MxReturnCode<bool>("Body.AutoLineBreak");

            if ((lineIndex < 0) || (String.IsNullOrEmpty(appendChar) == true) || (appendChar.Length > (TabSpaces?.Length ?? Program.PosIntegerNotSet)))
                rc.SetError(1100701, MxError.Source.Param, $"lineIndex={lineIndex} is invalid, appendChar is NullorEmpty, or appendChar.Length={appendChar?.Length ?? -1} > {TabSpaces?.Length ?? Program.PosIntegerNotSet}", MxMsgs.MxErrBadMethodParam);
            else
            {
                var breakIndex = GetLineBreakIndex(lineIndex, appendChar.Length);
                if (breakIndex == Program.PosIntegerNotSet)
                    rc.SetError(1100702, MxError.Source.User, $"appendChar.Length={appendChar.Length} > line length={GetCharacterCountInLine()} when LineWidth={EditAreaViewCursorLimit.ColIndex}", MxMsgs.MxErrLineTooLong);
                else
                {
                    var newLine = SplitLine(lineIndex, breakIndex);
                    if (newLine == null)
                        rc.SetError(1100703, MxError.Source.User, $"SplitLine({lineIndex}, {breakIndex}) is null for TextLines.Count={TextLines.Count}", MxMsgs.MxErrLineTooLong);
                    else
                    {
                        var rcInsert = InsertLine(newLine + appendChar);
                        rc += rcInsert;
                        if (rcInsert.IsSuccess(true))
                            rc.SetResult(true);
                    }
                }
            }
            return rc;
        }
        public MxReturnCode<string[]> GetLinesForDisplay(int count) 
        {
            var rc = new MxReturnCode<string[]>("Body.GetLinesForDisplay", null);

            if ((TextLines == null) || (count < 0) || (count > (EditAreaViewCursorLimit.RowIndex+1)))
                rc.SetError(1100801, MxError.Source.Param, $"TextLines.Count={TextLines?.Count ?? -1}; count={count} is 0 or is > EditAreaViewCursorLimit.RowIndex={EditAreaViewCursorLimit.RowIndex}+1", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (IsError() || (EditAreaBottomChapterIndex <= Program.PosIntegerNotSet))
                    rc.SetError(1100802, MxError.Source.Program, $"IsError() == true, or TopDisplayLineIndex={EditAreaBottomChapterIndex} invalid - Initialise not called? ", MxMsgs.MxErrInvalidCondition);
                else
                {
                    var lines = new string[count];
                    if (TextLines.Count > 0)
                    {
                        var lineIndex = ((EditAreaBottomChapterIndex - EditAreaViewCursorLimit.RowIndex) > 0) ? EditAreaBottomChapterIndex - EditAreaViewCursorLimit.RowIndex : 0;
                        if (lineIndex + count < TextLines.Count)
                            lineIndex += TextLines.Count - count;
                        for (var bufferIndex = 0; bufferIndex < count; bufferIndex++)
                        {
                            if (lineIndex < TextLines.Count)
                                lines[bufferIndex] = TextLines[lineIndex++];
                            else
                                break;
                        }
                    }
                    rc.SetResult(lines);
                }
            }
            return rc;
        }

        public static string GetErrorsInEnteredText(string text, string textType="line", int lineNo=0)
        {
            string rc = null;

            if (text == null)
                rc = $"line {lineNo}: unexpected {textType}; it is null. This is a program error. Please save your work and restart the program.";
            else
            {
                if (text.Length > CmdLineParamsApp.ArgEditAreaLineWidthMax)
                    rc = $"line {lineNo}: invalid {textType}. It has {text.Length} characters, but only {CmdLineParamsApp.ArgEditAreaLineWidthMax} allowed. Delete some characters and try again.";
                else
                {
                    var index = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                    if ((index != -1) && (index != 0))
                        rc = $"line {lineNo}: invalid {textType}. It contains a new line at column {index + 1}. Delete these characters and try again.";
                    else
                    {
                        string output = new string(text.Where(c => ((IsEnteredCharacterValid(c, index == 0) == true))).ToArray());
                        if (output.Length != text.Length)
                            rc = $"line {lineNo}: invalid {textType}. It contains {text.Length - output.Length} disallowed characters. Retype the text and try again.";
                        else
                        {
                            if ((index = text.IndexOf(DisallowedCharOpeningAngle)) != -1)
                                rc = $"line {lineNo}: invalid {textType}. It contains the disallowed character '{DisallowedCharOpeningAngle}' at column {index + 1}. Delete this character and try again.";
                            else
                            {
                                if ((index = text.IndexOf(DisallowedCharClosingAngle)) != -1)
                                    rc = $"line {lineNo}: invalid {textType}. It contains the disallowed character '{DisallowedCharClosingAngle}' at column {index + 1}. Delete this character and try again.";
                            }
                        }
                    }
                }
            }
            return rc;
        }
        public static string GetErrorsInEnteredCharacter(char c)
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

        public void RemoveAllLines()
        {
            TextLines.Clear();
            SetCursorInChapter(0, 0);
            RefreshWordCount();
        }

        public static bool IsEnteredCharacterValid(char c, bool allowCR=false)
        {
            // ReSharper disable once ReplaceWithSingleAssignment.False
            var rc = false;

            if (allowCR && (c == 0xA) || (c == 0xD))
                rc = true;
            else
            {
                //only supports english
                if ((c == '\t') || (c == ' ') || (Char.IsLetterOrDigit(c) || (Char.IsPunctuation(c)) || (Char.IsSymbol(c))))
                {
                    rc = true;
                }
            }

            return rc;
        }
        public int SetTabSpaces(int count)
        {
            if (count > 0)
            {
                TabSpaces = "";
                for (int x = 0; x < count; x++)
                    TabSpaces += " ";
            }
            return TabSpaces.Length;   
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

        public int RefreshWordCount()
        {
            var rc = 0;
            foreach (var line in TextLines)
            {
                rc += GetWordCountInLine(line);
            }
            WordCount = rc;
            return rc;
        }

        public string GetWordInLine(int lineNo = Body.LastLine, int wordNo = Program.PosIntegerNotSet)
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

        public char GetCharacterInLine(int lineNo = Body.LastLine, int columnNo = Body.LastColumn)
        {
            var rc = Body.NullChar;

            var lineIndex = (lineNo != Body.LastLine) ? lineNo-1 : TextLines.Count-1;
            if ((lineIndex >= 0) && (lineIndex < TextLines.Count))
            {
                var colIndex = (columnNo != Body.LastColumn) ? columnNo-1: (TextLines[lineIndex]?.Length-1) ?? -1;
                if ((colIndex >= 0) && (colIndex < TextLines[lineIndex]?.Length))
                    rc = TextLines[lineIndex][colIndex];
            }
            return rc;
        }
        public int GetLineCount()
        {
            return TextLines?.Count ?? Program.PosIntegerNotSet;
        }

        public int GetCharacterCountInLine(int lineNo = Program.PosIntegerNotSet) //default to LastLine
        {
            var rc = Program.PosIntegerNotSet;

            if (TextLines != null)
            {
                var lineIndex = (lineNo == Program.PosIntegerNotSet) ? TextLines.Count - 1 : lineNo - 1;
                if ((lineIndex >= 0) && (lineIndex < TextLines.Count) && (TextLines[lineIndex] != null))
                {
                    rc = (TextLines[lineIndex] == Environment.NewLine) ? 0 : TextLines[lineIndex].Length;
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

        public static string GetLineUpdateDeleteChar(string existingText, int charIndex)
        {
            string rc = null;

            if ((existingText != null) && (charIndex >= 0) && (charIndex < existingText.Length))
            {
                var start = existingText.Snip(0, charIndex - 1);
                var end = existingText.Snip(charIndex + 1, existingText.Length - 1);
                rc = (start ?? "") + (end ?? "");
            }
            return rc;
        }

        public static string GetLineUpdateText(string existingText, string updateText, int updateIndex, int maxLength, bool insert)
        {
            string rc = null;   //typically returns null if updatedText cannot fit into the line or updateText contains invalid characters
            if ((updateText != null) && (existingText != null) && (updateIndex >= 0) && (updateIndex < maxLength) && (Body.GetErrorsInEnteredText(updateText) == null))
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

        private bool AddWord(string word)
        {
            var rc = false;

            if (String.IsNullOrEmpty(word) == false) 
            {
                if ((TextLines?.Count ?? Program.PosIntegerNotSet) <= 0)
                    rc = InsertLine(word)?.GetResult() ?? false;              //add first word in chapter, might be just a space char
                else
                {
                    var lastChar = GetCharacterInLine();
                    if (lastChar == 0)
                        TextLines[TextLines.Count - 1] = word; //add first word in last (current) line
                    else
                    {                                          //append word to end of last line
                        if (lastChar == Body.SpaceChar)
                            TextLines[TextLines.Count - 1] += word;                 //previously typed char was a space so no need to add another
                        else
                            TextLines[TextLines.Count - 1] += Body.SpaceChar + word; //previously typed char was NOT a space, so add one before new word
                    }
                    if( string.IsNullOrWhiteSpace(word) == false)
                        WordCount++;
                    rc = true;
                }
            }
            return rc;
        }

        private bool AddFirstCharToChapter(string firstChar)
        {
            return AddWord(firstChar) == true; 
        }
    }
}
