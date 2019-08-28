﻿using System;
using KLineEdCmdApp;
using KLineEdCmdApp.Model;
using KLineEdCmdAppTest.TestSupport;
using Xunit;
// ReSharper disable All


namespace KLineEdCmdAppTest.ModelTests
{
    public class BodyTest
    {
        [Fact]
        public void RemoveAllLinesTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("one");
            body.SetTestLine("two");
            body.SetTestLine("three");
            Assert.Equal(5, body.Cursor.ColIndex); //cursor at end of line
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
        public void GetEnteredTextErrorsTest()
        {
            Assert.Null(Body.GetErrorsInText(""));
            Assert.Null(Body.GetErrorsInText("this text is fine"));
            // Assert.Null(Body.GetErrorsInText(Environment.NewLine));  //new line in col 0 is fine too
            Assert.StartsWith("unexpected text (null). This is a program error. Please save your work and restart the program.", Body.GetErrorsInText(null));
            Assert.StartsWith("attempt to enter a new line at column 7", Body.GetErrorsInText($"hello {Environment.NewLine}"));
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
        public void GetLineCountTest()
        {
            var body = new Body();
            Assert.True(body.IsError()); //not initialised, but not necessary for GetLineCount()
            Assert.Equal(0, body.GetLineCount());
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
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("123456a");
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);
        }

        [Fact]
        public void GetWordCountThreeLineTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("123456a");
            body.SetTestLine("123456b");
            body.SetTestLine("123456c");
            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
        }

        [Fact]
        public void GetWordCountInLineTest()
        {
            Assert.Equal(0, Body.GetWordCountInLine(null));
            Assert.Equal(0, Body.GetWordCountInLine(""));
            Assert.Equal(1, Body.GetWordCountInLine("one"));
            Assert.Equal(2, Body.GetWordCountInLine("one two"));
            Assert.Equal(3, Body.GetWordCountInLine("one two\tthree"));
            Assert.Equal(0, Body.GetWordCountInLine($"{Environment.NewLine}"));
            Assert.Equal(3, Body.GetWordCountInLine($"one two{Environment.NewLine}three"));
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
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("123456a");
            body.SetTestLine("123456b");
            body.SetTestLine("123456c");
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
        public void GetCharacterCountInLineTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(Program.PosIntegerNotSet, body.GetCharacterCountInRow(0));

            var line1 = "qwerty";
            body.SetTestLine(line1);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(line1.Length, body.GetCharacterCountInRow(0));
            Assert.Equal(Program.PosIntegerNotSet, body.GetCharacterCountInRow(1));
        }
    }
}
