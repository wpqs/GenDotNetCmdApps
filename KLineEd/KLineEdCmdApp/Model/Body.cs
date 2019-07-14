using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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
    public class Body
    {
        public static readonly string OpeningElement = "<body>";
        public static readonly string ClosingElement = "</body>";

        public static readonly char DisallowedCharOpeningAngle = '<';
        public static readonly char DisallowedCharClosingAngle = '>';
        public static readonly int  DefaultTabSpaceCount = 3;

        public const string WordMissing = "[???]";
        public const string WordNotSet = "[?]";

        public const int LastLine = -1;         //used to provide parameter default values so cannot be readonly
        public const int LastColumn = -1;
        public const char NullChar = (char)0;
        public const char SpaceChar = ' ';

        public static readonly int TextLinesPerPage = 36;                   //counted from Jack Kerouac's book 'On the Road'
        public static readonly int MaxTextLines = TextLinesPerPage * 2500;  //twice the number of pages in Tolstoy's 'War and Peace'

        public Body()
        {
            TextLines = new List<string>();
            WordCount = 0;
            LastDisplayRowIndex = 0;
            LastDisplayColumnIndex = 0;
            LineWidth = Program.PosIntegerNotSet;
            SetTabSpaces(DefaultTabSpaceCount);
            Error = true;
        }

        public List<string> TextLines { private set; get; }
        public int WordCount { private set; get; }
        public int LastDisplayColumnIndex { private set; get; }
        public int LastDisplayRowIndex { private set; get; }
        public int LineWidth { private set; get; }
        private string TabSpaces { set; get; }
        private bool Error { set; get; }

        public bool IsError(){ return Error; }

        public MxReturnCode<bool> Initialise(int lineWidth) //add TabSpaceCount as param
        {
            var rc = new MxReturnCode<bool>("Body.Initialise");

            if (lineWidth == Program.PosIntegerNotSet)
                rc.SetError(1100101, MxError.Source.Param, $"lineWidth={lineWidth} not set", "MxErrBadMethodParam");
            else
            {
                LineWidth = lineWidth;
                LastDisplayRowIndex = 0;
                LastDisplayColumnIndex = 0;
                SetTabSpaces(3);    //TabSpaceCount
                Error = false;
                rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> Write(StreamWriter file)
        {
            var rc = new MxReturnCode<bool>("Body.Write");

            if (file == null)
                rc.SetError(1100201, MxError.Source.Param, "file is null", "MxErrBadMethodParam");
            else
            {
                try
                {
                    if (IsError())
                        rc.SetError(1100202, MxError.Source.Program, "IsError() == true, Initialise not called?", "MxErrInvalidCondition");
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
                    rc.SetError(1100203, MxError.Source.Exception, e.Message, "MxErrInvalidCondition");
                }
            }

            return rc;
        }

        public MxReturnCode<bool> Read(StreamReader file)
        {
            var rc = new MxReturnCode<bool>("Body.Read");

            if (file == null)
                rc.SetError(1100301, MxError.Source.Param, "file is null", "MxErrBadMethodParam");
            else
            {
                try
                {
                    if (IsError())
                        rc.SetError(1100302, MxError.Source.Program, "IsError() == true, Initialise not called?", "MxErrInvalidCondition");
                    else
                    {
                        var firstLine = file.ReadLine();
                        if (firstLine != OpeningElement)
                            rc.SetError(1100303, MxError.Source.User, $"first line is {firstLine}", "MxErrInvalidCondition");
                        else
                        {
                            string lastLine = null;
                            string line = null;
                            while ((line = file.ReadLine()) != null)
                            {
                                lastLine = line;
                                if (lastLine == ClosingElement)
                                    break;
                                TextLines.Add(lastLine);
                            }

                            if (lastLine != ClosingElement)
                                rc.SetError(1100304, MxError.Source.User, $"last line is {lastLine}", "MxErrInvalidCondition");
                            else
                            {
                                WordCount = RefreshWordCount();
                                rc.SetResult(true);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1100305, MxError.Source.Exception, e.Message, "MxErrInvalidCondition");
                }
            }
            return rc;
        }

        public MxReturnCode<bool> AppendLine(string line)
        {
            var rc = new MxReturnCode<bool>("Body.AppendLine");

            if (line == null) // allow lines that are empty - i.e. user hits enter key
                rc.SetError(1100401, MxError.Source.Param, "text is null", "MxErrBadMethodParam");
            else
            {
                if (IsError())
                    rc.SetError(1100402, MxError.Source.Program, "IsError() == true, Initialise not called?", "MxErrInvalidCondition");
                else
                {
                    var lineLen = line.Length;
                    if (lineLen > LineWidth)
                        rc.SetError(1100403, MxError.Source.User, $"text.Length={lineLen} > LineWidth={LineWidth}", "MxErrLineTooLong");
                    else
                    {
                        if (Body.GetErrorsForEnteredString(line) != null)
                            rc.SetError(1100404, MxError.Source.User, Body.GetErrorsForEnteredString(line));
                        else
                        {
                            var wordsInLine = AddLine(line);
                            if (wordsInLine == Program.PosIntegerNotSet)
                                rc.SetError(1100405, MxError.Source.User, $"Error: line count exceeded. Chapters are allowed a maximum of {Body.MaxTextLines} lines. Start a new Chapter and continue.");
                            else
                            {
                                WordCount += wordsInLine;
                                rc.SetResult(true);
                            }
                        }
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> AppendWord(string word)  
        {
            var rc = new MxReturnCode<bool>("Body.AppendWord");

            if (string.IsNullOrWhiteSpace(word)) 
                rc.SetError(1100501, MxError.Source.Param, "word is null", "MxErrBadMethodParam");
            else
            {
                if (IsError())
                    rc.SetError(1100502, MxError.Source.Program, "IsError() == true, Initialise not called?", "MxErrInvalidCondition");
                else
                {
                    var lineLen = word.Length;
                    if (lineLen > LineWidth)
                        rc.SetError(110503, MxError.Source.User, $"word.Length={lineLen} > LineWidth={LineWidth}", "MxErrLineTooLong");
                    else
                    {
                        if (Body.GetErrorsForEnteredString(word, "word") != null)
                            rc.SetError(1100504, MxError.Source.User, Body.GetErrorsForEnteredString(word));
                        else
                        {
                            if ((TextLines.Count == 0) || (TextLines[TextLines.Count - 1].Length + word.Length > LineWidth))
                            {                                              //word makes line too long, so append it to a new line
                                var wordsInLine = AddLine(word);
                                if (wordsInLine == Program.PosIntegerNotSet)
                                    rc.SetError(1100505, MxError.Source.User, $"Error: line count exceeded. Chapters are allowed a maximum of {Body.MaxTextLines} lines. Start a new Chapter and continue.");
                                else
                                {
                                    WordCount += wordsInLine;
                                    rc.SetResult(true);
                                }
                            }
                            else
                            {
                                if (AddWord(word) == false)
                                    rc.SetError(1100506, MxError.Source.Program, $"word={word ?? "[null]"} is NullorEmpty", "MxErrInvalidCondition");
                                else
                                {
                                    WordCount++;
                                    rc.SetResult(true);
                                }
                            }
                        }
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> AppendChar(char c)  
        {
            var rc = new MxReturnCode<bool>("Body.AppendChar");

            if (Body.GetErrorsForEnteredCharacter(c) != null)
                rc.SetError(1100601, MxError.Source.User, Body.GetErrorsForEnteredCharacter(c));
            else
            {
                var lineIndex = TextLines.Count - 1;
                var appendChar = (Char.IsWhiteSpace(c)) ? ((c == '\t') ? TabSpaces : Body.SpaceChar.ToString()) :  c.ToString();
                if (appendChar.Length <= 0)
                    rc.SetError(1100602, MxError.Source.Program, $"appendChar={appendChar} for c={c} length <= 0", "MxErrInvalidCondition");
                else
                {
                    var lastChar = GetCharInLine();                     //get last character in last line
                    if ((lastChar == Body.NullChar) || (lineIndex < 0))
                    {                                                   
                        if (AddFirstCharToChapter(appendChar) == false)
                            rc.SetError(1100603, MxError.Source.Program, $"AddFirstCharToChapter({appendChar ?? "[null]"}) failed", "MxErrInvalidCondition");
                        else
                            rc.SetResult(true);
                    }
                    else
                    {
                        if (TextLines[lineIndex].Length >= (LineWidth - appendChar.Length))
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
                                    rc.SetError(1100604, MxError.Source.Program, $"appendChar={appendChar ?? "[null]"} is NullorEmpty", "MxErrInvalidCondition");
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(appendChar) == false)
                                        WordCount++;
                                    rc.SetResult(true);
                                }
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

            if ((lineIndex < 0) || (string.IsNullOrEmpty(appendChar) == true) || (appendChar.Length > (TabSpaces?.Length ?? Program.PosIntegerNotSet)))
                rc.SetError(1100701, MxError.Source.Param, $"lineIndex={lineIndex} is invalid, appendChar is NullorEmpty, or appendChar.Length={appendChar?.Length ?? -1} > {TabSpaces?.Length ?? Program.PosIntegerNotSet}", "MxErrBadMethodParam");
            else
            {
                var breakIndex = GetLineBreakIndex(lineIndex, appendChar.Length);
                if (breakIndex == Program.PosIntegerNotSet)
                    rc.SetError(1100702, MxError.Source.User, $"appendChar.Length={appendChar.Length} > line length={GetCharactersInLine()} when LineWidth={LineWidth}", "MxErrLineTooLong");
                else
                {
                    var newLine = SplitLine(lineIndex, breakIndex);
                    if (newLine == null)
                        rc.SetError(1100703, MxError.Source.User, $"SplitLine({lineIndex}, {breakIndex}) is null for TextLines.Count={TextLines.Count}", "MxErrLineTooLong");
                    else
                    {
                        var wordsInLine = AddLine(newLine + appendChar);
                        if (wordsInLine == Program.PosIntegerNotSet)
                            rc.SetError(1100704, MxError.Source.User, $"Error: line count exceeded. Chapters are allowed a maximum of {Body.MaxTextLines} lines. Start a new Chapter and continue.");
                        else
                        {
                            rc.SetResult(true);
                        }
                    }
                }
            }
            return rc;
        }
        public MxReturnCode<string[]> GetLastLinesForDisplay(int count) 
        {
            var rc = new MxReturnCode<string[]>("Body.GetLastLinesForDisplay", null);

            if (count > CmdLineParamsApp.ArgDisplayLastLinesCntMax)
                rc.SetError(1100801, MxError.Source.Param, $"count={count} > {CmdLineParamsApp.ArgDisplayLastLinesCntMax}", "MxErrBadMethodParam");
            else
            {
                if (IsError())
                    rc.SetError(1100802, MxError.Source.Program, "IsError() == true, Initialise not called?", "MxErrInvalidCondition");
                else
                {
                    LastDisplayColumnIndex = 0;
                    LastDisplayRowIndex = 0;
                    var lastLines = new string[count];
                    var buffcnt = count - 1;
                    var filelinecnt = TextLines.Count;
                    // ReSharper disable once UnusedVariable
                    foreach (var line in lastLines)
                    {
                        if (filelinecnt > 0)
                        {
                            lastLines[buffcnt--] = TextLines[(filelinecnt--) - 1];
                            LastDisplayColumnIndex = lastLines[buffcnt + 1]?.Length ?? Program.PosIntegerNotSet;
                            LastDisplayRowIndex++;
                        }
                        else
                            lastLines[buffcnt--] = null;
                    }
                    rc.SetResult(lastLines);
                }
            }
            return rc;
        }

        public static string GetErrorsForEnteredString(string text, string textType="line")
        {
            string rc = null;

            if (text == null)
                rc = $"Error: Unexpected {textType}; it is null. This is a program error. Please save your work and restart the program.";
            else
            {
                if (text.Length > CmdLineParamsApp.ArgDisplayLineWidthMax)
                    rc = $"Error: invalid {textType}. It has {text.Length} characters, but only {CmdLineParamsApp.ArgDisplayLineWidthMax} allowed. Delete some characters and try again.";
                else
                {
                    var index = -1;
                    if ((index = text.IndexOf(Environment.NewLine, StringComparison.Ordinal)) != -1)
                        rc = $"Error: invalid {textType}. It contains a new line at column {index + 1}. Delete these characters and try again.";
                    else
                    {
                        string output = new string(text.Where(c => ((IsEnteredCharacterValid(c) == true))).ToArray());
                        if (output.Length != text.Length)
                            rc = $"Error: invalid {textType}. It contains {text.Length - output.Length} disallowed characters. Retype the text and try again.";
                        else
                        {
                            if ((index = text.IndexOf(DisallowedCharOpeningAngle)) != -1)
                                rc = $"Error: invalid {textType}. It contains the disallowed character '{DisallowedCharOpeningAngle}' at column {index + 1}. Delete this character and try again.";
                            else
                            {
                                if ((index = text.IndexOf(DisallowedCharClosingAngle)) != -1)
                                    rc = $"Error: invalid {textType}. It contains the disallowed character '{DisallowedCharClosingAngle}' at column {index + 1}. Delete this character and try again.";
                            }
                        }
                    }
                }
            }
            return rc;
        }
        public static string GetErrorsForEnteredCharacter(char c)
        {
            string rc = null;

            if (IsEnteredCharacterValid(c) == false)
                rc = String.Format("Error: invalid character; 0x{0:X}. This character cannot be typed into a chapter. Please delete it and try again.", (int)c);
            else
            {
                if ((c == DisallowedCharOpeningAngle) || ((c == DisallowedCharClosingAngle)))
                    rc = $"Error: disallowed character '{c}'. This character cannot be typed into a chapter. Please delete it and try again.";
            }
            return rc;
        }

        public void RemoveAllLines()
        {
            TextLines.Clear();
            RefreshWordCount();
        }

        public static bool IsEnteredCharacterValid(char c)
        {
            // ReSharper disable once ReplaceWithSingleAssignment.False
            var rc = false;

            //only supports english
            if ((c == '\t') || (c == ' ') || (char.IsLetterOrDigit(c) || (char.IsPunctuation(c)) || (char.IsSymbol(c))))
            {
                rc = true;
            }
            return rc;
        }
        public int SetTabSpaces(int count)
        {
            TabSpaces = "";
            for (int x = 0; x < count; x++)
                TabSpaces += " ";
            return TabSpaces.Length;   
        }
        public int GetLineBreakIndex(int lineIndex, int spaceNeeded)
        {
            var rc = Program.PosIntegerNotSet;

            if ((lineIndex < TextLines.Count) && (spaceNeeded > 0) && (spaceNeeded + 1 < LineWidth)) //allow for space
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
                    TextLines[lineIndex] = TextLines[lineIndex].Substring(0, splitIndex);
                }
            }
            return rc;
        }
        public string GetWordInLine(int lineNo = Body.LastLine, int wordNo = Program.PosIntegerNotSet)
        {
            var rc = Body.WordNotSet;

            var lineIndex = (lineNo != Body.LastLine) ? lineNo - 1 : TextLines.Count - 1;
            if ((lineIndex >= 0) && (lineIndex < TextLines.Count))
            {
                var line = TextLines[lineIndex];
                var lineWordCount = GetWordCountInLine(line);
                var wordIndex = (wordNo == Program.PosIntegerNotSet) ? lineWordCount : wordNo;
                if ((wordIndex >= 0) && (wordIndex < lineWordCount))
                {
                    rc = line.Snip(GetWordIndexInLine(line, wordIndex), GetWordIndexInLine(line, wordIndex, false)) ?? Body.WordMissing;
                }
            }
            return rc;
        }

        public char GetCharInLine(int lineNo = Body.LastLine, int columnNo = Body.LastColumn)
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
        public int GetCharactersInLine(int lineNo = Program.PosIntegerNotSet)
        {
            var rc = Program.PosIntegerNotSet;

            if ((lineNo != -1) && (lineNo <= TextLines.Count))
            {
                rc = TextLines[lineNo]?.Length ?? Program.PosIntegerNotSet;
            }
            return rc;
        }
        public static int GetWordCountInLine(string line)
        {
            var rc = 0;
            if (string.IsNullOrEmpty(line) == false)
            {
                var text = line.Trim();
                int count = 0;
                bool wordItem = false;

                foreach (char c in text)
                {
                    if (char.IsWhiteSpace(c))
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

        public static int GetWordIndexInLine(string line, int wordNo =1, bool startIndex=true)
        {
            var rc = Program.PosIntegerNotSet;
            if ((string.IsNullOrEmpty(line) == false) && (wordNo > 0))
            {
                var charIndex = 0;
                var wordIndex = wordNo - 1;
                int wordCount = 0;
                bool wordItem = false;

                foreach (char c in line)
                {
                    if (char.IsWhiteSpace(c))
                        wordItem = false;
                    else
                    {
                        if (wordItem == false)
                            wordCount++;
                        wordItem = true;
                    }
                    if (wordCount == wordIndex)
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
            }
            return rc;
        }

        private int AddLine(string line)
        {
            var rc = Program.PosIntegerNotSet;
            if (line != null)
            {
                var lineCount = GetLineCount() + ((line.Length > LineWidth) ? 1 : 0);
                if ((lineCount != Program.PosIntegerNotSet) && (lineCount < Body.MaxTextLines))
                {
                    TextLines.Add(line); //add line to end of list
                    rc = GetWordCountInLine(line);
                }
            }
            return rc;
        }
        private bool AddWord(string word)
        {
            var rc = false;

            if (string.IsNullOrEmpty(word) == false) 
            {
                if (GetLineCount() <= 0)
                {
                    var count = AddLine(word);              //add first word in chapter, might be just a space char
                    if ((count == 1) || (count == 0)) 
                        rc = true;
                }
                else
                {
                    var lastChar = GetCharInLine();
                    if (lastChar == 0)
                        TextLines[TextLines.Count - 1] = word; //add first word in last (current) line
                    else
                    {                                          //append word to end of last line
                        if (lastChar == Body.SpaceChar)
                            TextLines[TextLines.Count - 1] += word;                 //previously typed char was a space so no need to add another
                        else
                            TextLines[TextLines.Count - 1] += Body.SpaceChar + word; //previously typed char was NOT a space, so add one before new word
                    }
                    rc = true;
                }
            }
            return rc;
        }

        private bool AddFirstCharToChapter(string firstChar)
        {
            var rc = false;
            if (AddWord(firstChar) == true)
            {
                if (string.IsNullOrWhiteSpace(firstChar) == false)
                    WordCount++;
                rc = true;
            }
            return rc;
        }
    }
}
