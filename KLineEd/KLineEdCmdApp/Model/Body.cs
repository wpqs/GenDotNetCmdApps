using System;
using System.Collections.Generic;
using System.IO;
using MxReturnCode;
// ReSharper disable All

namespace KLineEdCmdApp.Model
{
    public class Body
    {
        public static readonly string OpeningElement = "<body>";
        public static readonly string ClosingElement = "</body>";

        public static readonly char InvalidCharOpeningAngle = '<';
        public static readonly char InvalidCharClosingAngle = '>';

        public static readonly int TextLinesPerPage = 36;                   //counted from Jack Kerouac's book 'On the Road'
        public static readonly int MaxTextLines = TextLinesPerPage * 2500;  //twice the number of pages in Tolstoy's 'War and Peace'

        public Body()
        {
            TextLines = new List<string>();
            LineWidth = KLineEditor.PosIntegerNotSet;
            Error = true;
        }

        public List<string> TextLines { private set; get; }
        public int WordCount { private set; get; }
        public int LineWidth { private set; get; }

        public bool IsError(){ return Error; }
        private bool Error { set; get; }

        public MxReturnCode<bool> Initialise(int lineWidth)
        {
            var rc = new MxReturnCode<bool>("Body.Initialise");

            if (lineWidth == KLineEditor.PosIntegerNotSet)
                rc.SetError(1100101, MxError.Source.Param, $"lineWidth={lineWidth} not set", "MxErrBadMethodParam");
            else
            {
                LineWidth = lineWidth;
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
                            rc.SetError(1100302, MxError.Source.User, $"first line is {firstLine}", "MxErrInvalidCondition");
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
                                rc.SetError(1100303, MxError.Source.User, $"last line is {lastLine}", "MxErrInvalidCondition");
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
                    rc.SetError(1100304, MxError.Source.Exception, e.Message, "MxErrInvalidCondition");
                }
            }
            return rc;
        }

        public MxReturnCode<bool> AddLine(string text, bool incWordCount=false)
        {
            var rc = new MxReturnCode<bool>("Body.AddLine");

            if (text == null)
                rc.SetError(1100401, MxError.Source.Param, "text is null", "MxErrBadMethodParam");
            else
            {
                if (IsError())
                    rc.SetError(1100402, MxError.Source.Program, "IsError() == true, Initialise not called?", "MxErrInvalidCondition");
                else
                {
                    if (Body.GetEnteredTextErrors(text) != null)
                        rc.SetError(1100402, MxError.Source.User, Body.GetEnteredTextErrors(text));
                    else
                    {
                        var lineCount = GetLineCount();
                        if ((lineCount == KLineEditor.PosIntegerNotSet) || (lineCount > Body.MaxTextLines))
                            rc.SetError(1100403, MxError.Source.User, $"Error: line count exceeded. Chapters are allowed a maximum of {lineCount} lines. Start a new Chapter and continue.");
                        else
                        {
                            if (incWordCount)
                                WordCount += GetWordsInLine(text);
                            TextLines.Add(text);
                            rc.SetResult(true);
                        }
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> AddWord(string word)
        {
            var rc = new MxReturnCode<bool>("Body.AddWord");

            if (word == null)
                rc.SetError(1100501, MxError.Source.Param, "word is null", "MxErrBadMethodParam");
            else
            {
                if (IsError())
                    rc.SetError(1100502, MxError.Source.Program, "IsError() == true, Initialise not called?", "MxErrInvalidCondition");
                else
                {
                    if (Body.GetEnteredTextErrors(word) != null)
                        rc.SetError(1100503, MxError.Source.User, Body.GetEnteredTextErrors(word));
                    else
                    {
                        if ((TextLines.Count == 0) || (TextLines[TextLines.Count - 1].Length + word.Length > LineWidth))
                        {
                            var rcAddLine = AddLine(word);
                            rc += rcAddLine;
                            if (rcAddLine.IsSuccess(true))
                            {
                                WordCount++;
                                rc.SetResult(true);
                            }
                        }
                        else
                        {
                            TextLines[TextLines.Count - 1] += " " + word; //typing a space triggers AddWord() so previous word will not end with space
                            WordCount++;
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
                rc.SetError(1100601, MxError.Source.Param, $"count={count} > {CmdLineParamsApp.ArgDisplayLastLinesCntMax}", "MxErrBadMethodParam");
            else
            {
                if (IsError())
                    rc.SetError(1100602, MxError.Source.Program, "IsError() == true, Initialise not called?", "MxErrInvalidCondition");
                else
                {
                    var lastLines = new string[count];
                    var buffcnt = count - 1;
                    var filelinecnt = TextLines.Count;
                    foreach (var line in lastLines)
                    {
                        if (filelinecnt > 0)
                            lastLines[buffcnt--] = TextLines[(filelinecnt--) - 1];
                        else
                            lastLines[buffcnt--] = null;
                    }

                    rc.SetResult(lastLines);
                }
            }
            return rc;
        }

        public static string GetEnteredTextErrors(string text)
        {
            string rc = null;

            if (text == null)
                rc = $"Error: Unexpected text; it is null. This is a program error. Please save your work and restart the program.";
            else
            {
                if (text.Length > CmdLineParamsApp.ArgMaxColMax)
                    rc = $"Error: invalid line. It has {text.Length} characters, but only {CmdLineParamsApp.ArgMaxColMax} allowed. Delete some characters and try again.";
                else
                {
                    var index = -1;
                    if ((index = text.IndexOf(Environment.NewLine)) != -1)
                        rc = $"Error: invalid line. It contains a new line at column {index + 1}. Delete these characters and try again.";
                    else
                    {
                        if ((index = text.IndexOf(InvalidCharOpeningAngle)) != -1)
                            rc = $"Error: invalid line. It contains It contains the disallowed character '{InvalidCharOpeningAngle}' at column {index + 1}. Delete this character and try again.";
                        else
                        {
                            if ((index = text.IndexOf(InvalidCharClosingAngle)) != -1)
                                rc = $"Error: invalid line. It contains It contains the disallowed character '{InvalidCharClosingAngle}' at column {index + 1}. Delete this character and try again.";
                        }
                    }
                }
            }
            return rc;
        }
        public int GetLineCount()
        {
            return TextLines?.Count ?? KLineEditor.PosIntegerNotSet;
        }

        public int RefreshWordCount()
        {
            var rc = 0;
            foreach (var line in TextLines)
            {
                rc += GetWordsInLine(line);
            }

            WordCount = rc;
            return rc;
        }
        public static int GetWordsInLine(string line)
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
    }
}
