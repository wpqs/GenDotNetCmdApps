using System;
using System.IO;
using KLineEdCmdApp.Model;
using KLineEdCmdAppTest.TestSupport;
using Xunit;
using Program = KLineEdCmdApp.Program;

// ReSharper disable All

namespace KLineEdCmdAppTest.ModelTests
{
    public class BodyTextEditingTests
    {
        private readonly string _instancePathFileName;

        public BodyTextEditingTests()
        {
            _instancePathFileName = TestConst.UnitTestInstanceTestsPathFileName;  //file deleted before each test
            if (File.Exists(_instancePathFileName))
                File.Delete(_instancePathFileName);
        }

        [Fact]
        public void InsertParaBreakInEmptyChapterTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.True(body.InsertParaBreak().GetResult());

            Assert.Equal(0, body.Cursor.ColIndex); //cursor at start of next line
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.WordCount);
            Assert.Equal(Body.ParaBreakChar, body.GetCharacterInLine(1, 1));
        }

        [Fact]
        public void InsertParaBreakAtStartOfLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.True(body.InsertText("qwerty").GetResult());
            Assert.True(body.SetCursorInChapter(0, 0).GetResult());

            Assert.Equal(0, body.Cursor.ColIndex); //cursor at start of line
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertParaBreak().GetResult());

            Assert.Equal(0, body.Cursor.ColIndex); //cursor at start of next line
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(1, body.WordCount);
            Assert.Equal(Body.ParaBreakChar, body.GetCharacterInLine(1));
            Assert.Equal('q', body.GetCharacterInLine(2, 1));
            Assert.Equal('w', body.GetCharacterInLine(2, 2));
            Assert.Equal('e', body.GetCharacterInLine(2, 3));
            Assert.Equal('r', body.GetCharacterInLine(2, 4));
            Assert.Equal('t', body.GetCharacterInLine(2, 5));
            Assert.Equal('y', body.GetCharacterInLine(2, 6));
            Assert.Equal(Body.ParaBreakChar, body.GetCharacterInLine(2, 7));
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(2, 8));

            Assert.True(body.InsertText("a ", true).GetResult());
            Assert.Equal('a', body.GetCharacterInLine(2, 1));
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void InsertParaBreakAtEndOfLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.True(body.InsertText("qwerty").GetResult());
            Assert.Equal(6, body.Cursor.ColIndex); //cursor at end of line
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertParaBreak().GetResult());

            Assert.Equal(0, body.Cursor.ColIndex); //cursor at start of next line
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(1, body.WordCount);

            Assert.Equal('q', body.GetCharacterInLine(1, 1));
            Assert.Equal('w', body.GetCharacterInLine(1, 2));
            Assert.Equal('e', body.GetCharacterInLine(1, 3));
            Assert.Equal('r', body.GetCharacterInLine(1, 4));
            Assert.Equal('t', body.GetCharacterInLine(1, 5));
            Assert.Equal('y', body.GetCharacterInLine(1, 6));
            Assert.Equal(Body.ParaBreakChar, body.GetCharacterInLine(1, 7));
            Assert.Equal(Body.ParaBreakChar, body.GetCharacterInLine(2, 1));
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(2, 2));

            Assert.True(body.InsertText("a").GetResult());
            Assert.Equal('a', body.GetCharacterInLine(2, 1));
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void InsertParaBreakAtMidLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.True(body.InsertText("qwerty").GetResult());
            Assert.True(body.SetCursorInChapter(0, 2).GetResult());
            Assert.Equal(2, body.Cursor.ColIndex); //cursor in middle of line
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.WordCount);

            Assert.Equal("qwerty>", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);

            Assert.True(body.InsertParaBreak().GetResult());

            Assert.Equal(0, body.Cursor.ColIndex); //cursor at start of next line
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(2, body.WordCount);

            Assert.Equal("qw>", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("erty>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(2, 6));

        }

        [Fact]
        public void MoveCursorEmptyTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            Assert.True(body.SetCursorInChapter(0, 0).GetResult());

            Assert.False(body.MoveCursorInChapter(Body.CursorMove.PreviousCol).GetResult());
            Assert.False(body.MoveCursorInChapter(Body.CursorMove.NextCol).GetResult());
            Assert.False(body.MoveCursorInChapter(Body.CursorMove.PreviousRow).GetResult());
            Assert.False(body.MoveCursorInChapter(Body.CursorMove.NextRow).GetResult());
            Assert.False(body.MoveCursorInChapter(Body.CursorMove.Home).GetResult());
            Assert.False(body.MoveCursorInChapter(Body.CursorMove.End).GetResult());
        }

        [Fact]
        public void MoveCursorOneLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line = "qwerty";
            Assert.True(body.InsertText(line).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.True(body.SetCursorInChapter(0, 0).GetResult());

            Assert.False(body.MoveCursorInChapter(Body.CursorMove.PreviousRow).GetResult());
            Assert.False(body.MoveCursorInChapter(Body.CursorMove.NextRow).GetResult());
            Assert.False(body.MoveCursorInChapter(Body.CursorMove.PreviousCol).GetResult());

            Assert.True(body.MoveCursorInChapter(Body.CursorMove.NextCol).GetResult());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.Cursor.ColIndex);

            Assert.True(body.MoveCursorInChapter(Body.CursorMove.Home).GetResult());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);

            Assert.True(body.MoveCursorInChapter(Body.CursorMove.End).GetResult());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(line.Length, body.Cursor.ColIndex);
        }

        [Fact]
        public void MoveCursorThreeLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            Assert.True(body.InsertLine(line1).GetResult());
            var line2 = "0123456789";
            Assert.True(body.InsertLine(line2).GetResult());
            var line3 = "abc";
            Assert.True(body.InsertLine(line3).GetResult());

            Assert.Equal(3, body.GetLineCount());

            Assert.True(body.SetCursorInChapter(0, 0).GetResult());

            Assert.False(body.MoveCursorInChapter(Body.CursorMove.PreviousRow).GetResult());
            Assert.False(body.MoveCursorInChapter(Body.CursorMove.PreviousCol).GetResult());
            
            Assert.True(body.MoveCursorInChapter(Body.CursorMove.NextCol).GetResult());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.Cursor.ColIndex);

            Assert.True(body.MoveCursorInChapter(Body.CursorMove.Home).GetResult());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);

            Assert.Contains("Warning: you cannot move beyond the start of the chapter", body.MoveCursorInChapter(Body.CursorMove.PreviousCol).GetErrorUserMsg());
            Assert.Contains("Warning: you cannot move beyond the start of the chapter", body.MoveCursorInChapter(Body.CursorMove.PreviousRow).GetErrorUserMsg());

            Assert.True(body.MoveCursorInChapter(Body.CursorMove.End).GetResult());
            Assert.Equal(2, body.Cursor.RowIndex);
            Assert.Equal(line3.Length, body.Cursor.ColIndex);

            Assert.Contains("Warning: you cannot move beyond the end of the chapter", body.MoveCursorInChapter(Body.CursorMove.NextCol).GetErrorUserMsg());
            Assert.Contains("Warning: you cannot move beyond the end of the chapter", body.MoveCursorInChapter(Body.CursorMove.NextRow).GetErrorUserMsg());


            Assert.True(body.MoveCursorInChapter(Body.CursorMove.PreviousCol).GetResult());
            Assert.Equal(2, body.Cursor.RowIndex);
            Assert.Equal(line3.Length-1, body.Cursor.ColIndex);

            Assert.True(body.MoveCursorInChapter(Body.CursorMove.PreviousRow).GetResult());
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(line3.Length-1, body.Cursor.ColIndex); //don't move column - line2.length> line3.line

            Assert.True(body.MoveCursorInChapter(Body.CursorMove.PreviousRow).GetResult());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(line3.Length - 1, body.Cursor.ColIndex); //don't move column - line1.length> line3.line
        }


        [Fact]
        public void DeleteCharacterCursorStartOneLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            Assert.True(body.InsertLine(line1).GetResult());
            Assert.Equal("qwerty", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.True(body.SetCursorInChapter(0,0).GetResult());

            Assert.True(body.DeleteCharacter().GetResult());
            Assert.Equal("werty", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.True(body.DeleteCharacter().GetResult());
            Assert.True(body.DeleteCharacter().GetResult());
            Assert.True(body.DeleteCharacter().GetResult());
            Assert.True(body.DeleteCharacter().GetResult());
            Assert.True(body.DeleteCharacter().GetResult());

            Assert.False(body.DeleteCharacter().GetResult());
        }

        [Fact]
        public void DeleteCharacterCursorEndOneLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            Assert.True(body.InsertLine(line1).GetResult());
            Assert.Equal("qwerty", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.True(body.SetCursorInChapter(0, 5).GetResult());

            Assert.True(body.DeleteCharacter().GetResult());
            Assert.Equal("qwert", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.True(body.SetCursorInChapter(0, 4).GetResult());
            Assert.True(body.DeleteCharacter().GetResult());
            Assert.True(body.SetCursorInChapter(0, 3).GetResult());
            Assert.True(body.DeleteCharacter().GetResult());
            Assert.True(body.SetCursorInChapter(0, 2).GetResult());
            Assert.True(body.DeleteCharacter().GetResult());
            Assert.True(body.SetCursorInChapter(0, 1).GetResult());
            Assert.True(body.DeleteCharacter().GetResult());
            Assert.True(body.SetCursorInChapter(0, 0).GetResult());
            Assert.True(body.DeleteCharacter().GetResult());

            Assert.Contains("Warning: Chapter is empty", body.DeleteCharacter().GetErrorUserMsg());

        }

        [Fact]
        public void DeleteCharacterParaBreakTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            Assert.True(body.InsertLine(line1).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal("qwerty", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.True(body.SetCursorInChapter(0, 2).GetResult());
            Assert.True(body.InsertParaBreak().GetResult());

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
            Assert.Equal("qw>", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("erty", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            Assert.True(body.SetCursorInChapter(0, 2).GetResult());
            Assert.True(body.DeleteCharacter().GetResult());
            Assert.Equal("qw", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        }

        [Fact]
        public void BackSpaceParaBreakTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.ChapterBody.GetLineCount());

            var line1 = "qwerty";
            Assert.True(manuscriptNew.ChapterBody.InsertLine(line1).GetResult());

            Assert.True(manuscriptNew.ChapterBody.SetCursorInChapter(0, 2).GetResult());
            Assert.True(manuscriptNew.BodyInsertParaBreak().GetResult());

            Assert.Equal(2, manuscriptNew.ChapterBody.GetLineCount());
            Assert.Equal(1, manuscriptNew.ChapterBody.Cursor.RowIndex);
            Assert.Equal(0, manuscriptNew.ChapterBody.Cursor.ColIndex);

            Assert.Equal("qw>", manuscriptNew.BodyGetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("erty", manuscriptNew.BodyGetEditAreaLinesForDisplay(2).GetResult()[1]);

            Assert.True(manuscriptNew.ChapterBody.SetCursorInChapter(1, 0).GetResult());
            Assert.True(manuscriptNew.BodyBackSpace().GetResult());
            Assert.Equal("qw", manuscriptNew.BodyGetEditAreaLinesForDisplay(2).GetResult()[0]);
        }

        [Fact]
        public void BackspaceOneLineTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.ChapterBody.GetLineCount());

            var line1 = "qwerty";
            Assert.True(manuscriptNew.ChapterBody.InsertLine(line1).GetResult());
            Assert.Equal("qwerty", manuscriptNew.BodyGetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.True(manuscriptNew.BodyMoveCursor(Body.CursorMove.End).GetResult());
            Assert.Equal(6, manuscriptNew.ChapterBody.Cursor.ColIndex);

            Assert.True(manuscriptNew.BodyBackSpace().GetResult());
            Assert.Equal(5, manuscriptNew.ChapterBody.Cursor.ColIndex);
            Assert.Equal("qwert", manuscriptNew.BodyGetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.True(manuscriptNew.BodyBackSpace().GetResult());
            Assert.True(manuscriptNew.BodyBackSpace().GetResult());
            Assert.True(manuscriptNew.BodyBackSpace().GetResult());
            Assert.True(manuscriptNew.BodyBackSpace().GetResult());
            Assert.True(manuscriptNew.BodyBackSpace().GetResult());

            Assert.Contains("Warning: Chapter is empty", manuscriptNew.BodyBackSpace().GetErrorUserMsg());
        }


        [Fact]
        public void InsertLineNullFailTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.InsertLine(null).GetResult());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void InsertLineEmptyFailTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.InsertLine("").GetResult()); //lines cannot be empty 
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void InsertTextInvalidCharFailTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.InsertText('>'.ToString()).GetResult());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.False(body.InsertText('<'.ToString()).GetResult());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.False(body.InsertText($"hello{Environment.NewLine}").GetResult());  //not in col=0
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.False(body.InsertText($"{Environment.NewLine}").GetResult());  //in col=0
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }


        [Fact]
        public void InsertTextInsertInitialTextTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertText("aaa").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal("aaa>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextAppendLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText(" aaa").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("one aaa", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextOverwriteStartTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one two three").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 0);

            Assert.True(body.InsertText("xxx").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("xxx two three", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextOverwriteMiddleTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one two three").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 4);

            Assert.True(body.InsertText("xxx").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("one xxx three", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextOverwriteToEndTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one two three").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 8);

            Assert.True(body.InsertText("xxxxx").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("one two xxxxx", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextOverwriteBeyondEndTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one two three").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 13);

            Assert.True(body.InsertText(" xxx").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("one two three xxx", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextInsertStartTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one two three").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 0);

            Assert.True(body.InsertText("xxx ", true).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("xxx one two three", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextInsertMiddleTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one two three").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 4);

            Assert.True(body.InsertText("xxx ", true).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("one xxx two three", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextInsertEndTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine("one two three").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 13);

            Assert.True(body.InsertText(" xxx", true).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("one two three xxx", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }


        [Fact]
        public void InsertTextLongLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, 35).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789.123456789.123456789.1234";
            Assert.Equal(35, line.Length);

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertText(line).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);
            Assert.Equal(line + body.ParaBreakDisplayChar.ToString(), body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            var tooLong = line + "x";
            Assert.False(body.InsertText(tooLong).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);
            Assert.Equal(line + body.ParaBreakDisplayChar.ToString(), body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertLineLongLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, 35).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789 123456789 123456789 1234";
            Assert.Equal(35, line.Length);

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine(line).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal(line, body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            var tooLong = line + "x";
            Assert.False(body.InsertLine(tooLong).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
        }

        [Fact]
        public void InsertLineSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertLine(" ").GetResult()); //lines can start with space
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }

        [Fact]
        public void InsertTextCharacterTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText('b'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText('c'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("ab c>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.Equal(" >", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextMultiSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText('b'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText(' '.ToString()).GetResult());
            Assert.True(body.InsertText(' '.ToString()).GetResult());
            Assert.True(body.InsertText(' '.ToString()).GetResult());
            Assert.True(body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText('c'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("ab    c>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextStartSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.True(body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText('b'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal(" a b>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextStartMultiSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertText(' '.ToString()).GetResult());
            Assert.True(body.InsertText(' '.ToString()).GetResult());
            Assert.True(body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.True(body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText('b'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("   a b>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextTabTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(3, body.TabSpaces.Length);

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText('b'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText(body.TabSpaces).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText('c'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("ab   c>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextStartTabTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(3, body.TabSpaces.Length);

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertText(body.TabSpaces).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.True(body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText(body.TabSpaces).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText('b'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("   a   b>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }


        [Fact]
        public void InsertTextInsertSecondLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, 35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.InsertText("aaaaa").GetResult());  //col #5
            Assert.Equal("aaaaa>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(5, body.Cursor.ColIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.InsertText(" bbbb").GetResult());   //col #10
            Assert.Equal("aaaaa bbbb>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(10, body.Cursor.ColIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.True(body.InsertText(" 123456789").GetResult()); //col #20
            Assert.Equal("aaaaa bbbb 123456789>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(20, body.Cursor.ColIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);

            Assert.True(body.InsertText(" 123456789").GetResult()); //col #30
            Assert.Equal("aaaaa bbbb 123456789 123456789>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(30, body.Cursor.ColIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);

            Assert.True(body.InsertText(" 1234").GetResult());      //col #35
            Assert.Equal("aaaaa bbbb 123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(35, body.Cursor.ColIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(5, body.WordCount);

            Assert.True(body.InsertText("x").GetResult());          //col #36
            Assert.Equal("aaaaa bbbb 123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("1234x>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(5, body.Cursor.ColIndex);
            Assert.Equal(1, body.Cursor.RowIndex);
        }

        [Fact]
        public void InsertTextSplitLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, 65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            Assert.Equal(65, line.Length);
            Assert.True(body.InsertLine(line).GetResult());

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(7, body.WordCount);

            Assert.True(body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("0123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("1234a", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        }

        [Fact]
        public void InsertTextSpaceSplitLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, 65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            Assert.Equal(65, line.Length);

            Assert.True(body.InsertLine(line).GetResult());

            Assert.Equal(line, body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(65, body.Cursor.ColIndex); //one char after end
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(7, body.WordCount);

            Assert.True(body.InsertText(' '.ToString()).GetResult());

            Assert.Equal("0123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("1234 ", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(5, body.Cursor.ColIndex); //one char after end
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(7, body.WordCount);

            Assert.True(body.InsertText('x'.ToString()).GetResult());

            Assert.Equal("0123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("1234 x", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(8, body.WordCount);
        }

        [Fact]
        public void GetNextParaBreakNotInitFailTest()
        {
            var body = new Body();
            Assert.Equal(Program.PosIntegerNotSet, body.GetNextParaBreakRowIndex(0));
        }

        [Fact]
        public void GetNextParaBreakParamFailTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            Assert.True(body.InsertText(line1).GetResult());

            Assert.Equal(Program.PosIntegerNotSet, body.GetNextParaBreakRowIndex(-1));
            Assert.Equal(0, body.GetNextParaBreakRowIndex(0));
            Assert.Equal(Program.PosIntegerNotSet, body.GetNextParaBreakRowIndex(1));
        }

        [Fact]
        public void GetNextParaBreakNotFoundTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            Assert.True(body.InsertLine(line1).GetResult());
            Assert.True(body.InsertLine(line1).GetResult());
            Assert.True(body.InsertLine(line1).GetResult());
            Assert.Equal(3, body.GetLineCount());

            Assert.Equal("qwerty", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);

            Assert.Equal(2, body.GetNextParaBreakRowIndex(0));
            Assert.Equal(2, body.GetNextParaBreakRowIndex(1));
            Assert.Equal(2, body.GetNextParaBreakRowIndex(2));
        }

        [Fact]
        public void GetNextParaBreakTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            Assert.True(body.InsertLine(line1).GetResult());
            Assert.True(body.InsertLine(line1).GetResult());
            Assert.True(body.InsertLine(line1).GetResult());
            Assert.Equal("qwerty", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(3, body.GetLineCount());

            //0 qwerty
            //1 qwerty
            //2 qwerty
            
            Assert.Equal(2, body.GetNextParaBreakRowIndex(0));

            Assert.True(body.SetCursorInChapter(1, 2).GetResult());
            Assert.True(body.InsertParaBreak().GetResult());
            Assert.Equal(4, body.GetLineCount());
            Assert.Equal(2, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);

            Assert.Equal("qw>", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("erty", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);

            //0 qwerty
            //1 qw>
            //2 erty
            //3 qwerty

            Assert.Equal(1, body.GetNextParaBreakRowIndex(0));
            Assert.Equal(3, body.GetNextParaBreakRowIndex(2));
        }

        [Fact]
        public void LeftJustifyLinesInParagraphTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, 15).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "0123456789 1234";
            Assert.True(body.InsertLine(line1).GetResult());
            Assert.True(body.InsertLine(line1).GetResult());
            Assert.True(body.InsertLine(line1).GetResult());

            Assert.Equal(3, body.GetLineCount());

           // Assert.True(body.LeftJustifyLinesInParagraph(0, 15, 0));

        }

        [Fact]
        public void GetCharacterCountInLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(Program.PosIntegerNotSet, body.GetCharacterCountInRow(0));

            var line1 = "qwerty";
            Assert.True(body.InsertLine(line1).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(line1.Length, body.GetCharacterCountInRow(0));
            Assert.Equal(Program.PosIntegerNotSet, body.GetCharacterCountInRow(1));
        }

        [Fact]
        public void IsCursorAtEndOfParagraphTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            Assert.True(body.InsertLine(line1).GetResult());
            Assert.True(body.InsertParaBreak().GetResult());
            Assert.Equal(2, body.GetLineCount());

            Assert.True(body.SetCursorInChapter(0, 0).GetResult());
            Assert.False(body.IsCursorAtEndOfParagraph());

            Assert.True(body.SetCursorInChapter(0, line1.Length).GetResult());
            Assert.True(body.IsCursorAtEndOfParagraph());

        }
    }
}
