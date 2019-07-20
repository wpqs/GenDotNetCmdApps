using System;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.Model;
using Xunit;
// ReSharper disable All


namespace KLineEdCmdAppTest.ModelTests
{
    public class BodyTest
    {

        [Fact]
        public void IsEnteredCharacterValidTest()
        {
            Assert.True(Body.IsEnteredCharacterValid('a'));
            Assert.True(Body.IsEnteredCharacterValid('z'));
            Assert.True(Body.IsEnteredCharacterValid('A'));
            Assert.True(Body.IsEnteredCharacterValid('Z'));
            Assert.True(Body.IsEnteredCharacterValid('0'));
            Assert.True(Body.IsEnteredCharacterValid('9'));
            Assert.True(Body.IsEnteredCharacterValid(' '));
            Assert.True(Body.IsEnteredCharacterValid('\t'));
            Assert.True(Body.IsEnteredCharacterValid('*'));

            Assert.False(Body.IsEnteredCharacterValid('\0'));
        }

        [Fact]
        public void SetTabSpacesTest()
        {
            var body = new Body();

            Assert.Equal(3, body.SetTabSpaces(3));
            Assert.Equal("   ", body.TabSpaces);

            Assert.Equal(3, body.SetTabSpaces(0));
            Assert.Equal("   ", body.TabSpaces);

            Assert.Equal(3, body.SetTabSpaces(-1));
            Assert.Equal("   ", body.TabSpaces);

            Assert.Equal(5, body.SetTabSpaces(5));
            Assert.Equal("     ", body.TabSpaces);

        }

        [Fact]
        public void GetLineCountTest()
        {
            var body = new Body();
            Assert.True(body.IsError());        //not initialised, but not necessary for GetLineCount()
            Assert.Equal(0, body.GetLineCount() );
        }
        [Fact]
        public void GetLastLinesTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());
            Assert.NotNull(body.GetLastLinesForDisplay(CmdLineParamsApp.ArgDisplayLastLinesCntDefault).GetResult());
        }
        [Fact]
        public void GetLastLinesNotInitializedTest()
        {
            var body = new Body();
            Assert.True(body.IsError());
            Assert.Null(body.GetLastLinesForDisplay(CmdLineParamsApp.ArgDisplayLastLinesCntDefault).GetResult());
        }
        [Fact]
        public void AppendLineNotInitializedTest()
        {
            var body = new Body();
            Assert.True(body.IsError());
            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.AppendLine("one").GetResult());
            Assert.Equal(0, body.GetLineCount());
        }

        [Fact]
        public void RemoveAllLinesTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("one").GetResult());
            Assert.True(body.AppendLine("two").GetResult());
            Assert.True(body.AppendLine("three").GetResult());
            Assert.Equal(3, body.RefreshWordCount());
            Assert.Equal(3, body.GetLineCount());

            body.RemoveAllLines();

            Assert.Equal(0, body.RefreshWordCount());
            Assert.Equal(0, body.WordCount);
            Assert.Equal(0, body.GetLineCount());
        }

        [Fact]
        public void GetIndexOfWordTest()
        {
            var text = "0123 5678 ABCDE";

            Assert.Equal(0, Body.GetIndexOfWord(text));
            Assert.Equal(3, Body.GetIndexOfWord(text, 1, false));
            Assert.Equal(5, Body.GetIndexOfWord(text, 2));
            Assert.Equal(8, Body.GetIndexOfWord(text, 2, false));
            Assert.Equal(10, Body.GetIndexOfWord(text, 3));
            Assert.Equal(14, Body.GetIndexOfWord(text, 3, false));

            Assert.Equal(-1, Body.GetIndexOfWord(text, 0));
            Assert.Equal(-1, Body.GetIndexOfWord(text, 0, false));

            Assert.Equal(-1, Body.GetIndexOfWord(text, 4));
            Assert.Equal(-1, Body.GetIndexOfWord(text, 4, false));

            Assert.Equal(-1, Body.GetIndexOfWord(text, -1));
            Assert.Equal(-1, Body.GetIndexOfWord(text, -1, false));

        }

        [Fact]
        public void GetWordInLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("one two three four").GetResult());
            Assert.Equal(4, body.RefreshWordCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(18, body.GetCharacterCountInLine());
            Assert.Equal("one two three four", body.GetLastLinesForDisplay(1).GetResult()[0]);

            Assert.Equal("one", body.GetWordInLine(Body.LastLine, 1));
            Assert.Equal("two", body.GetWordInLine(Body.LastLine, 2));
            Assert.Equal("three", body.GetWordInLine(Body.LastLine, 3));
            Assert.Equal("four", body.GetWordInLine(Body.LastLine, 4));
            Assert.Equal("four", body.GetWordInLine(Body.LastLine, -1));
            Assert.Equal("four", body.GetWordInLine(Body.LastLine));


            Assert.Null(body.GetWordInLine(Body.LastLine, 5));
            Assert.Null(body.GetWordInLine(Body.LastLine, 0));
            Assert.Null(body.GetWordInLine(Body.LastLine, -2));
        }

        [Fact]
        public void AppendOneLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("one").GetResult());
            Assert.Equal(1, body.RefreshWordCount());
            Assert.Equal(1, body.WordCount);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.GetCharacterCountInLine());
            Assert.Equal("one", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }
        [Fact]
        public void AppendTwoLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("one").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.RefreshWordCount());
            Assert.Equal("one", body.GetLastLinesForDisplay(10).GetResult()[0]);

            Assert.True(body.AppendLine("two").GetResult());  //don't inc WordCount
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);            //
            Assert.Equal(2, body.RefreshWordCount());       //refreshes WordCount from actual words in Body
            Assert.Equal(2, body.WordCount);            //

            Assert.Equal("one", body.GetLastLinesForDisplay(10).GetResult()[0]);
            Assert.Equal("two", body.GetLastLinesForDisplay(10).GetResult()[1]);

        }
        [Fact]
        public void AppendLineNullTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.AppendLine(null).GetResult());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void AppendLineEmptyTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("").GetResult());           //lines can be empty - i.e. user types CR,CR
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void AppendLineSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine(" ").GetResult());          //lines can start with space
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void AppendLineLongLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789 123456789 123456789 1234";
            Assert.Equal(35, line.Length);

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine(line).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal(line, body.GetLastLinesForDisplay(1).GetResult()[0]);

            var tooLong = line + "x";
            Assert.False(body.AppendLine(tooLong).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
        }


        [Fact]
        public void AppendWordNullTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.AppendWord(null).GetResult());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void AppendWordEmptyTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.AppendWord("").GetResult());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void AppendWordSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.AppendWord(" ").GetResult());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void AppendWordInvalidCharTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.AppendWord("<").GetResult());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.False(body.AppendWord(">").GetResult());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.False(body.AppendWord($"{Environment.NewLine}").GetResult());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void AppendWordLongWordTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            var word = "0123456789 123456789 123456789 1234";
            Assert.Equal(35, word.Length);

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendWord(word).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal(word, body.GetLastLinesForDisplay(1).GetResult()[0]);

            var tooLong = word + "x";
            Assert.False(body.AppendWord(tooLong).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
        }

        [Fact]
        public void AppendWordNewLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendWord("aaa").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal("aaa", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }
        [Fact]
        public void AppendWordExistingLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("one").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendWord("aaa").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("one aaa", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }
        [Fact]
        public void AppendWordAppendLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendWord("aaaa").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendWord("bbbbb").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("aaaa bbbbb", body.GetLastLinesForDisplay(1).GetResult()[0]);

            Assert.True(body.AppendWord("0123456789").GetResult());    //col 21
            Assert.True(body.AppendWord("0123456789").GetResult());    //col 32
            Assert.True(body.AppendWord("01").GetResult());            //col 35

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(5, body.WordCount);

            Assert.True(body.AppendWord("x").GetResult());            //col 36
            Assert.Equal(2, body.GetLineCount());

            Assert.Equal("aaaa bbbbb 0123456789 0123456789 01", body.GetLastLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("x", body.GetLastLinesForDisplay(2).GetResult()[1]);
        }

        [Fact]
        public void AppendCharTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendChar('a').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar('b').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar(' ').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar('c').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("ab c", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }


        [Fact]
        public void AppendCharSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendChar(' ').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.Equal(" ", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void AppendCharNullTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.AppendChar(Body.NullChar).GetResult());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void AppendCharTabTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendChar('a').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar('b').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar('\t').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar('c').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("ab   c", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void AppendCharMultiSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendChar('a').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar('b').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar(' ').GetResult());
            Assert.True(body.AppendChar(' ').GetResult());
            Assert.True(body.AppendChar(' ').GetResult());
            Assert.True(body.AppendChar(' ').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar('c').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("ab    c", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void AppendCharStartSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendChar(' ').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.True(body.AppendChar('a').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar(' ').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar('b').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal(" a b", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void AppendCharStartMultiSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendChar(' ').GetResult());
            Assert.True(body.AppendChar(' ').GetResult());
            Assert.True(body.AppendChar(' ').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.True(body.AppendChar('a').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar(' ').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar('b').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("   a b", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void AppendCharStartTabTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendChar('\t').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.True(body.AppendChar('a').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar('\t').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AppendChar('b').GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("   a   b", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }


        [Fact]
        public void AppendCharInvalidCharTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.AppendChar('>').GetResult());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.False(body.AppendChar('<').GetResult());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void GetLineBreakIndexTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            Assert.Equal(65, line.Length);

            Assert.True(body.AppendLine(line).GetResult());
            Assert.Equal(1, body.GetLineCount());

            Assert.Equal(-1, body.GetLineBreakIndex(0, 0));
            Assert.Equal(60, body.GetLineBreakIndex(0, 1));
            Assert.Equal(60, body.GetLineBreakIndex(0, 2));
            Assert.Equal(60, body.GetLineBreakIndex(0, 3));
            Assert.Equal(60, body.GetLineBreakIndex(0, 4));

            Assert.Equal(50, body.GetLineBreakIndex(0, 5));
            Assert.Equal(50, body.GetLineBreakIndex(0, 6));

            Assert.Equal(10, body.GetLineBreakIndex(0, 54));
            Assert.Equal(-1, body.GetLineBreakIndex(0, 55));
        }

        [Fact]
        public void GetLineBreakIndexMultiSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

                     //"0123456789 123456789 123456789 123456789 123456789 1234567 654321";
            var line = "0123456789 123456789 123456789 123456789 123456789 1234567   1234";
            Assert.Equal(65, line.Length);

            Assert.True(body.AppendLine(line).GetResult());
            Assert.Equal(1, body.GetLineCount());

            Assert.Equal(-1, body.GetLineBreakIndex(0, 0));
            Assert.Equal(60, body.GetLineBreakIndex(0, 1));
            Assert.Equal(60, body.GetLineBreakIndex(0, 2));
            Assert.Equal(60, body.GetLineBreakIndex(0, 3));
            Assert.Equal(60, body.GetLineBreakIndex(0, 4));

            Assert.Equal(50, body.GetLineBreakIndex(0, 5));
            Assert.Equal(50, body.GetLineBreakIndex(0, 6));
            Assert.Equal(50, body.GetLineBreakIndex(0, 7));
            Assert.Equal(50, body.GetLineBreakIndex(0, 8));

            Assert.Equal(10, body.GetLineBreakIndex(0, 54));
            Assert.Equal(-1, body.GetLineBreakIndex(0, 55));
        }

        [Fact]
        public void SplitLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            Assert.Equal(65, line.Length);

            Assert.True(body.AppendLine(line).GetResult());
            Assert.Equal(1, body.GetLineCount());

            var splitIndex = body.GetLineBreakIndex(0, 4);

            Assert.Equal(60, splitIndex);
            Assert.Equal("1234", body.SplitLine(0, splitIndex));
            Assert.Equal("0123456789 123456789 123456789 123456789 123456789 123456789", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }


        [Fact]
        public void AppendCharSplitLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            Assert.Equal(65, line.Length);
            Assert.True(body.AppendLine(line).GetResult());

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(7, body.WordCount);

            Assert.True(body.AppendChar('a').GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("0123456789 123456789 123456789 123456789 123456789 123456789", body.GetLastLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("1234a", body.GetLastLinesForDisplay(2).GetResult()[1]);
        }

        [Fact]
        public void AppendCharSpaceSplitLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            Assert.Equal(65, line.Length);
            Assert.True(body.AppendLine(line).GetResult());

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(7, body.WordCount);

            Assert.True(body.AppendChar(' ').GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("0123456789 123456789 123456789 123456789 123456789 123456789", body.GetLastLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("1234 ", body.GetLastLinesForDisplay(2).GetResult()[1]);

            Assert.True(body.AppendChar('x').GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(8, body.WordCount);
            Assert.Equal("1234 x", body.GetLastLinesForDisplay(2).GetResult()[1]);
        }

        [Fact]
        public void GetLastLinesZeroTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("one").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal("one", body.GetLastLinesForDisplay(10).GetResult()[0]);

            Assert.True(body.AppendLine("two").GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal("one", body.GetLastLinesForDisplay(10).GetResult()[0]);
            Assert.Equal("two", body.GetLastLinesForDisplay(10).GetResult()[1]);
        }

        [Fact]
        public void GetWordCountNoLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void GetWordCountOneLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("123456a").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);
        }

        [Fact]
        public void GetWordCountThreeLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("123456a").GetResult());
            Assert.True(body.AppendLine("123456b").GetResult());
            Assert.True(body.AppendLine("123456c").GetResult());
            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
        }

        [Fact]
        public void GetLastLinesOneTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("one").GetResult());
            Assert.True(body.AppendLine("two").GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal("two", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void GetLastLinesMaxTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("one").GetResult());
            Assert.True(body.AppendLine("two").GetResult());
            Assert.Equal(2, body.GetLineCount());

            Assert.Equal("one", body.GetLastLinesForDisplay(CmdLineParamsApp.ArgDisplayLastLinesCntMax).GetResult()[0]);
            Assert.Equal("two", body.GetLastLinesForDisplay(CmdLineParamsApp.ArgDisplayLastLinesCntMax).GetResult()[1]);
        }

        [Fact]
        public void GetLastLinesMoreThanMaxTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("one").GetResult());
            Assert.True(body.AppendLine("two").GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Null(body.GetLastLinesForDisplay(CmdLineParamsApp.ArgDisplayLastLinesCntMax+1).GetResult());
        }

  [Fact]
        public void GetWordsInLineTest()
        {
            Assert.Equal(0, Body.GetWordCountInLine(null));
            Assert.Equal(0, Body.GetWordCountInLine(""));
            Assert.Equal(1, Body.GetWordCountInLine("one"));
            Assert.Equal(2, Body.GetWordCountInLine("one two"));
            Assert.Equal(3, Body.GetWordCountInLine("one two\tthree"));
            Assert.Equal(3, Body.GetWordCountInLine($"one two{Environment.NewLine}three"));
        }

        [Fact]
        public void GetEnteredTextErrorsTest()
        {
            Assert.Null(Body.GetErrorsInEnteredText(""));
            Assert.Null(Body.GetErrorsInEnteredText("this text is fine"));
            Assert.StartsWith("unexpected line; it is null. This is a program error. Please save your work and restart the program.", Body.GetErrorsInEnteredText(null));
            Assert.StartsWith("invalid line. It contains a new line at column 7", Body.GetErrorsInEnteredText($"hello {Environment.NewLine}"));
            Assert.StartsWith("invalid line. It contains the disallowed character '<' at column 8", Body.GetErrorsInEnteredText($"hello .<hi"));
            Assert.StartsWith("invalid line. It contains the disallowed character '>' at column 9", Body.GetErrorsInEnteredText($"hello hi>"));

            var line = "0123456789112345678921234567893123456789412345678951234567896123456789712345678981234567899123456789";
            line += "0123456789112345678921234567893123456789412345678951234567896123456789712345678981234567899123456789";
            line += "012345678911234567892123456789312345678941234567895";
            Assert.StartsWith("invalid line. It has 251 characters, but only 250 allowed", Body.GetErrorsInEnteredText(line));

        }

        [Fact]
        public void GetEnteredCharErrorsTest()
        {
            Assert.Null(Body.GetErrorsInEnteredCharacter('a'));
            Assert.Null(Body.GetErrorsInEnteredCharacter('9'));
            Assert.Null(Body.GetErrorsInEnteredCharacter('{'));
            Assert.Null(Body.GetErrorsInEnteredCharacter('@'));

            Assert.Null(Body.GetErrorsInEnteredCharacter(' '));
            Assert.Null(Body.GetErrorsInEnteredCharacter('\t'));

            Assert.StartsWith("disallowed character '<'.", Body.GetErrorsInEnteredCharacter('<'));
            Assert.StartsWith("disallowed character '>'.", Body.GetErrorsInEnteredCharacter('>'));

            Assert.StartsWith("invalid character; 0x0.", Body.GetErrorsInEnteredCharacter(Body.NullChar));
            Assert.StartsWith("invalid character; 0xF.", Body.GetErrorsInEnteredCharacter((char)15));
       }

        [Fact]
        public void GetCharInLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
            Assert.True(body.AppendWord("123a").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal('a', body.GetCharacterInLine(Body.LastLine));
            Assert.Equal('a', body.GetCharacterInLine());
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(0));
        }

        [Fact]
        public void GetCharInLineNoneTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.Equal(Body.NullChar, body.GetCharacterInLine());
        }

        [Fact]
        public void GetCharInLineEmptyTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.Equal(Body.NullChar, body.GetCharacterInLine());
        }

        [Fact]
        public void GetCharInLineThreeTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AppendLine("123456a").GetResult());
            Assert.True(body.AppendLine("123456b").GetResult());
            Assert.True(body.AppendLine("123456c").GetResult());
            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(3, body.WordCount);

            Assert.Equal('a', body.GetCharacterInLine(1));
            Assert.Equal('a', body.GetCharacterInLine(1, 7));
            Assert.Equal('3', body.GetCharacterInLine(1,3));
            Assert.Equal('b', body.GetCharacterInLine(2));
            Assert.Equal('b', body.GetCharacterInLine(2, 7));
            Assert.Equal('4', body.GetCharacterInLine(2,4));
            Assert.Equal('c', body.GetCharacterInLine(3));
            Assert.Equal('c', body.GetCharacterInLine(3, 7));
            Assert.Equal('5', body.GetCharacterInLine(3,5));

            Assert.Equal(Body.NullChar, body.GetCharacterInLine(0));
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(3, -2));
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(3, 8));
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(4));

        }

    }
}
