using System;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.Model;
using KLineEdCmdAppTest.TestSupport;
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
            Assert.True(Body.IsEnteredCharacterValid('*'));

            Assert.False(Body.IsEnteredCharacterValid('\0'));
        }

        [Fact]
        public void GetLineCountTest()
        {
            var body = new Body();
            Assert.True(body.IsError()); //not initialised, but not necessary for GetLineCount()
            Assert.Equal(0, body.GetLineCount());
        }

        [Fact]
        public void GetLastLinesTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.NotNull(body.GetEditAreaLinesForDisplay(CmdLineParamsApp.ArgEditAreaLinesCountDefault).GetResult());
        }

        [Fact]
        public void GetLastLinesNotInitializedTest()
        {
            var body = new Body();
            Assert.True(body.IsError());
            Assert.Null(body.GetEditAreaLinesForDisplay(CmdLineParamsApp.ArgEditAreaLinesCountDefault).GetResult());
        }

        [Fact]
        public void AppendLineNotInitializedTest()
        {
            var body = new Body();
            Assert.True(body.IsError());
            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.InsertLine("one").GetResult());
            Assert.Equal(0, body.GetLineCount());
        }

        [Fact]
        public void RemoveAllLinesTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one").GetResult());
            Assert.True(body.InsertLine("two").GetResult());
            Assert.True(body.InsertLine("three").GetResult());
            Assert.Equal(5, body.Cursor.ColIndex);  //cursor at end of line
            Assert.Equal(2, body.Cursor.RowIndex);
            Assert.Equal(2, body.EditAreaBottomChapterIndex);
            Assert.Equal(3, body.WordCount);
            Assert.Equal(3, body.GetLineCount());

            body.RemoveAllLines();

            Assert.Equal(0, body.WordCount);
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
            Assert.Equal(0, body.EditAreaBottomChapterIndex);

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

        //[Fact]
        //public void GetWordInLineTest()
        //{
        //    var body = new Body();
        //    Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
        //    Assert.False(body.IsError());

        //    Assert.Equal(0, body.GetLineCount());
        //    Assert.True(body.InsertLine("one two three four").GetResult());
        //    Assert.Equal(4, body.WordCount);
        //    Assert.Equal(1, body.GetLineCount());
        //    Assert.Equal(18, body.GetCharacterCountInRow());
        //    Assert.Equal("one two three four", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

        //    Assert.Equal("one", body.GetWordInLine(Body.LastLine, 1));
        //    Assert.Equal("two", body.GetWordInLine(Body.LastLine, 2));
        //    Assert.Equal("three", body.GetWordInLine(Body.LastLine, 3));
        //    Assert.Equal("four", body.GetWordInLine(Body.LastLine, 4));
        //    Assert.Equal("four", body.GetWordInLine(Body.LastLine, -1));
        //    Assert.Equal("four", body.GetWordInLine(Body.LastLine));


        //    Assert.Null(body.GetWordInLine(Body.LastLine, 5));
        //    Assert.Null(body.GetWordInLine(Body.LastLine, 0));
        //    Assert.Null(body.GetWordInLine(Body.LastLine, -2));
        //}

        [Fact]
        public void InsertOneLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one").GetResult());
            Assert.Equal(1, body.WordCount);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.GetCharacterCountInRow());
            Assert.Equal("one", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTwoLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);
            Assert.Equal("one", body.GetEditAreaLinesForDisplay(10).GetResult()[0]);

            Assert.True(body.InsertLine("two").GetResult()); //don't inc WordCount
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount); //

            Assert.Equal("one", body.GetEditAreaLinesForDisplay(10).GetResult()[0]);
            Assert.Equal("two", body.GetEditAreaLinesForDisplay(10).GetResult()[1]);

        }

        [Fact]
        public void GetSplitIndexFromStartTest()
        {
            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            Assert.Equal(65, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 0));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 1));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 2));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 3));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 10));

            Assert.Equal(19, Body.GetSplitIndexFromStart(line, 11));
            Assert.Equal(19, Body.GetSplitIndexFromStart(line, 12));

            Assert.Equal(59, Body.GetSplitIndexFromStart(line, 61));
            Assert.Equal(59, Body.GetSplitIndexFromStart(line, 64));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 65));
        }


        [Fact]
        public void GetSplitIndexFromStartMultiSpaceTest()
        {
            //var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            var line =   "0123456789 123456789 123456789 123456789 123456789 1234567   1234";
            Assert.Equal(65, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 0));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 1));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 2));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 3));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 10));

            Assert.Equal(19, Body.GetSplitIndexFromStart(line, 11));
            Assert.Equal(19, Body.GetSplitIndexFromStart(line, 12));

            Assert.Equal(59, Body.GetSplitIndexFromStart(line, 61));
            Assert.Equal(59, Body.GetSplitIndexFromStart(line, 64));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 65));
        }

        [Fact]
        public void GetSplitIndexFromEndTest()
        {
            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            Assert.Equal(65, line.Length);
           
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 0));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 1));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 2));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 3));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 4));

            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 5));
            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 6));

            Assert.Equal(10, Body.GetSplitIndexFromEnd(line, 54));
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 55));
        }

        [Fact]
        public void GetSplitIndexFromEndMultiSpaceTest()
        {
            //"0123456789 123456789 123456789 123456789 123456789 1234567 654321";
            var line = "0123456789 123456789 123456789 123456789 123456789 1234567   1234";
            Assert.Equal(65, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 0));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 1));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 2));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 3));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 4));

            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 5));
            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 6));
            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 7));
            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 8));

            Assert.Equal(10, Body.GetSplitIndexFromEnd(line, 54));
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 55));
        }

        [Fact]
        public void SplitLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            Assert.Equal(65, line.Length);

            Assert.True(body.InsertLine(line).GetResult());
            Assert.Equal(1, body.GetLineCount());

            var splitIndex = Body.GetSplitIndexFromEnd(line, 4);

            Assert.Equal(60, splitIndex);
            Assert.Equal("1234", body.SplitLine(0, splitIndex));
            Assert.Equal("0123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }


   

        [Fact]
        public void GetLastLinesZeroTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal("one", body.GetEditAreaLinesForDisplay(10).GetResult()[0]);

            Assert.True(body.InsertLine("two").GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal("one", body.GetEditAreaLinesForDisplay(10).GetResult()[0]);
            Assert.Equal("two", body.GetEditAreaLinesForDisplay(10).GetResult()[1]);
        }

        [Fact]
        public void GetWordCountNoLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void GetWordCountOneLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("123456a").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);
        }

        [Fact]
        public void GetWordCountThreeLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("123456a").GetResult());
            Assert.True(body.InsertLine("123456b").GetResult());
            Assert.True(body.InsertLine("123456c").GetResult());
            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
        }

        [Fact]
        public void GetLastLinesOneTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one").GetResult());
            Assert.True(body.InsertLine("two").GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal("two", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        }

        [Fact]
        public void GetLastLinesMaxTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(CmdLineParamsApp.ArgEditAreaLinesCountMax, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one").GetResult());
            Assert.True(body.InsertLine("two").GetResult());
            Assert.Equal(2, body.GetLineCount());

            Assert.Equal("one", body.GetEditAreaLinesForDisplay(CmdLineParamsApp.ArgEditAreaLinesCountMax).GetResult()[0]);
            Assert.Equal("two", body.GetEditAreaLinesForDisplay(CmdLineParamsApp.ArgEditAreaLinesCountMax).GetResult()[1]);
        }

        [Fact]
        public void GetLastLinesMoreThanMaxTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one").GetResult());
            Assert.True(body.InsertLine("two").GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Null(body.GetEditAreaLinesForDisplay(CmdLineParamsApp.ArgEditAreaLinesCountMax + 1).GetResult());
        }

        [Fact]
        public void GetWordsInLineTest()
        {
            Assert.Equal(0, Body.GetWordCountInLine(null));
            Assert.Equal(0, Body.GetWordCountInLine(""));
            Assert.Equal(1, Body.GetWordCountInLine("one"));
            Assert.Equal(2, Body.GetWordCountInLine("one two"));
            Assert.Equal(3, Body.GetWordCountInLine("one two\tthree"));
            //Assert.Equal(3, Body.GetWordCountInLine($"one two{Environment.NewLine}three"));
            //Assert.Equal(0, Body.GetWordCountInLine($"{Environment.NewLine}"));
        }

        [Fact]
        public void GetEnteredTextErrorsTest()
        {
            Assert.Null(Body.GetErrorsInText(""));
            Assert.Null(Body.GetErrorsInText("this text is fine"));
           // Assert.Null(Body.GetErrorsInText(Environment.NewLine));  //new line in col 0 is fine too
            Assert.StartsWith("unexpected text (null). This is a program error. Please save your work and restart the program.", Body.GetErrorsInText(null));
            Assert.StartsWith("attempt to enter a new line at column 7",Body.GetErrorsInText($"hello {Environment.NewLine}"));
            Assert.StartsWith("attempt to enter the disallowed character '<' at column 8", Body.GetErrorsInText($"hello .<hi"));
            Assert.StartsWith("attempt to enter the disallowed character '>' at column 9", Body.GetErrorsInText($"hello hi>"));

            var line = "0123456789112345678921234567893123456789412345678951234567896123456789712345678981234567899123456789";
            line += "0123456789112345678921234567893123456789412345678951234567896123456789712345678981234567899123456789";
            line += "012345678911234567892123456789312345678941234567895";
            Assert.StartsWith("line 7: attempt to enter 251 characters, but only 250 allowed", Body.GetErrorsInText(line, 7));

        }

        [Fact]
        public void GetEnteredCharErrorsTest()
        {
            Assert.Null(Body.GetErrorsInEnteredCharacter('a'));
            Assert.Null(Body.GetErrorsInEnteredCharacter('9'));
            Assert.Null(Body.GetErrorsInEnteredCharacter('{'));
            Assert.Null(Body.GetErrorsInEnteredCharacter('@'));

            Assert.Null(Body.GetErrorsInEnteredCharacter(' '));

            Assert.StartsWith("disallowed character '<'.", Body.GetErrorsInEnteredCharacter('<'));
            Assert.StartsWith("disallowed character '>'.", Body.GetErrorsInEnteredCharacter('>'));

            Assert.StartsWith("invalid character; 0x0.", Body.GetErrorsInEnteredCharacter(Body.NullChar));
            Assert.StartsWith("invalid character; 0xF.", Body.GetErrorsInEnteredCharacter((char) 15));
        }

        [Fact]
        public void GetCharInLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
            Assert.True(body.InsertText("123a").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal('a', body.GetCharacterInLine(Body.LastLine, 4));
            Assert.Equal(Body.ParaBreakChar, body.GetCharacterInLine());
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(0));
        }

        [Fact]
        public void GetCharInLineNoneTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.Equal(Body.NullChar, body.GetCharacterInLine());
        }

        [Fact]
        public void GetCharInLineEmptyTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.Equal(Body.NullChar, body.GetCharacterInLine());
        }

        [Fact]
        public void GetCharInLineThreeTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("123456a").GetResult());
            Assert.True(body.InsertLine("123456b").GetResult());
            Assert.True(body.InsertLine("123456c").GetResult());
            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(3, body.WordCount);

            Assert.Equal('a', body.GetCharacterInLine(1));
            Assert.Equal('a', body.GetCharacterInLine(1, 7));
            Assert.Equal('3', body.GetCharacterInLine(1, 3));
            Assert.Equal('b', body.GetCharacterInLine(2));
            Assert.Equal('b', body.GetCharacterInLine(2, 7));
            Assert.Equal('4', body.GetCharacterInLine(2, 4));
            Assert.Equal('c', body.GetCharacterInLine(3));
            Assert.Equal('c', body.GetCharacterInLine(3, 7));
            Assert.Equal('5', body.GetCharacterInLine(3, 5));

            Assert.Equal(Body.NullChar, body.GetCharacterInLine(0));
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(3, -2));
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(3, 8));
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(4));

        }

        [Fact]
        public void WordOverwriteTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("123456a").GetResult());

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);
        }

        [Fact]
        public void SetEditAreaBottomChapterIndexFiveDisplayLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(5, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(4, body.EditAreaViewCursorLimit.RowIndex);

            Assert.True(body.InsertLine("0aaaaaa").GetResult());
            Assert.True(body.InsertLine("1bbbbbb").GetResult());
            Assert.True(body.InsertLine("2cccccc").GetResult());
            Assert.True(body.InsertLine("3dddddd").GetResult());
            Assert.True(body.InsertLine("4eeeeee").GetResult());
            Assert.True(body.InsertLine("5ffffff").GetResult());
            Assert.True(body.InsertLine("6gggggg").GetResult());
            Assert.True(body.InsertLine("7hhhhhh").GetResult());
            Assert.True(body.InsertLine("8iiiiii").GetResult());
            Assert.True(body.InsertLine("9jjjjjj").GetResult());
            Assert.True(body.InsertLine("10kkkkk").GetResult());

            Assert.Equal(11, body.GetLineCount());
            Assert.Equal(10, body.EditAreaBottomChapterIndex);

            Assert.False(body.SetEditAreaBottomIndex(Body.Scroll.Bottom).GetResult());   //can't scroll further down
            Assert.False(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult());
            Assert.False(body.SetEditAreaBottomIndex(Body.Scroll.PageDown).GetResult());

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.Top).GetResult());       //can scroll to top
            Assert.Equal(4, body.EditAreaBottomChapterIndex);

            Assert.False(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());   //can't scroll further up
            Assert.False(body.SetEditAreaBottomIndex(Body.Scroll.PageUp).GetResult());

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult());  //can scroll down
            Assert.Equal(5, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());    //can now scroll up
            Assert.Equal(4, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.PageDown).GetResult());  //can scroll page down
            Assert.Equal(8, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.PageUp).GetResult());    //can now scroll page up
            Assert.Equal(4, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.Bottom).GetResult());    //can scroll to bottom
            Assert.Equal(10, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());     //can scroll up three lines
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());
            Assert.Equal(7, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.PageDown).GetResult());   //can scroll page down, though not entire page
            Assert.Equal(10, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());     //can scroll up four lines
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());
            Assert.Equal(6, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.PageDown).GetResult());   //can scroll page down (entire page)
            Assert.Equal(10, body.EditAreaBottomChapterIndex);
            Assert.False(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult());  //can't scroll further down

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.Top).GetResult());       //can scroll to top
            Assert.Equal(4, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult());     //can scroll down three lines
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult());
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult());
            Assert.Equal(7, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.PageUp).GetResult());   //can scroll page up, though not entire page
            Assert.Equal(4, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult());     //can scroll down four lines
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult());
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult());
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult());
            Assert.Equal(8, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.PageUp).GetResult());   //can scroll page up (entire page)
            Assert.Equal(4, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult()); //can scroll down one line
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.PageUp).GetResult());   //can scroll page up, though not entire page
            Assert.Equal(4, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.Bottom).GetResult());   //can scroll bottom,
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());   //can scroll line up
            Assert.Equal(9, body.EditAreaBottomChapterIndex);
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.PageDown).GetResult());  //can scroll page down, though not entire page
            Assert.Equal(10, body.EditAreaBottomChapterIndex);

        }

        [Fact]
        public void SetEditAreaBottomChapterIndexOneDisplayLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(1, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.EditAreaViewCursorLimit.RowIndex);

            Assert.True(body.InsertLine("0aaaaaa").GetResult());
            Assert.True(body.InsertLine("1bbbbbb").GetResult());
            Assert.True(body.InsertLine("2cccccc").GetResult());
            Assert.True(body.InsertLine("3dddddd").GetResult());
            Assert.True(body.InsertLine("4eeeeee").GetResult());
            Assert.True(body.InsertLine("5ffffff").GetResult());
            Assert.True(body.InsertLine("6gggggg").GetResult());
            Assert.True(body.InsertLine("7hhhhhh").GetResult());
            Assert.True(body.InsertLine("8iiiiii").GetResult());
            Assert.True(body.InsertLine("9jjjjjj").GetResult());
            Assert.True(body.InsertLine("10kkkkk").GetResult());

            Assert.Equal(11, body.GetLineCount());
            Assert.Equal(10, body.EditAreaBottomChapterIndex);

            Assert.False(body.SetEditAreaBottomIndex(Body.Scroll.Bottom).GetResult());   //can't scroll further down
            Assert.False(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult());
            Assert.False(body.SetEditAreaBottomIndex(Body.Scroll.PageDown).GetResult());

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.Top).GetResult());       //can scroll to top
            Assert.Equal(0, body.EditAreaBottomChapterIndex);

            Assert.False(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());   //can't scroll further up
            Assert.False(body.SetEditAreaBottomIndex(Body.Scroll.PageUp).GetResult());

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult());  //can scroll down
            Assert.Equal(1, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());    //can now scroll up
            Assert.Equal(0, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.Bottom).GetResult());    //can scroll to bottom
            Assert.Equal(10, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());     //can scroll up three lines
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());
            Assert.Equal(7, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.PageDown).GetResult());   //can scroll page down, though just one line
            Assert.Equal(8, body.EditAreaBottomChapterIndex);
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.PageDown).GetResult());   //can scroll page down, though just one line
            Assert.Equal(9, body.EditAreaBottomChapterIndex);
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.PageDown).GetResult());   //can scroll page down, though just one line
            Assert.Equal(10, body.EditAreaBottomChapterIndex);
            Assert.False(body.SetEditAreaBottomIndex(Body.Scroll.PageDown).GetResult());   //can't scroll page down any more
            Assert.Equal(10, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.Top).GetResult());       //can scroll to top
            Assert.Equal(0, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineDown).GetResult());  //can scroll down one line
            Assert.Equal(1, body.EditAreaBottomChapterIndex);

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.PageUp).GetResult());   //can scroll page up, though just one line
            Assert.Equal(0, body.EditAreaBottomChapterIndex);
            Assert.False(body.SetEditAreaBottomIndex(Body.Scroll.PageUp).GetResult());   //can't scroll page up again

            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.Bottom).GetResult());   //can scroll bottom,
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.LineUp).GetResult());   //can scroll line up
            Assert.Equal(9, body.EditAreaBottomChapterIndex);
            Assert.True(body.SetEditAreaBottomIndex(Body.Scroll.PageDown).GetResult());  //can scroll page down, though not entire page
            Assert.Equal(10, body.EditAreaBottomChapterIndex);

        }
    }
    }
