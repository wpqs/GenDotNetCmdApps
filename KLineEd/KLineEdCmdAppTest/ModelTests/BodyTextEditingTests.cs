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
        public void InsertTextInvalidCharFailTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.InsertText('>'.ToString()).IsSuccess(true));
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.False(body.InsertText('<'.ToString()).IsSuccess(true));
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.False(body.InsertText($"hello{Environment.NewLine}").IsSuccess(true));  //not in col=0
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.False(body.InsertText($"{Environment.NewLine}").IsSuccess(true));  //in col=0
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);
        }


        [Fact]
        public void InsertParaBreakInEmptyChapterTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertParaBreak().GetResult());

            Assert.Equal(0, body.Cursor.ColIndex); //cursor at start of next line
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.WordCount);
            Assert.Equal(Body.ParaBreakChar, body.GetCharacterInLine(1, 1));
        }

        [Fact]
        public void InsertParaBreakAtStartOfLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("qwerty").GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 0).GetResult());

            Assert.Equal(0, body.Cursor.ColIndex); //cursor at start of line
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.End, body.InsertParaBreak().GetResult());

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

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("a ", true).GetResult());
            Assert.Equal('a', body.GetCharacterInLine(2, 1));
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void InsertParaBreakAtEndOfLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("qwerty").GetResult());
            Assert.Equal(6, body.Cursor.ColIndex); //cursor at end of line
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.End, body.InsertParaBreak().GetResult());

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

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("a").GetResult());
            Assert.Equal('a', body.GetCharacterInLine(2, 1));
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void InsertParaBreakAtMidLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("qwerty").GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 2).GetResult());
            Assert.Equal(2, body.Cursor.ColIndex); //cursor in middle of line
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.WordCount);

            Assert.Equal("qwerty>", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);

            Assert.Equal(ChapterModel.ChangeHint.End, body.InsertParaBreak().GetResult());

            Assert.Equal(0, body.Cursor.ColIndex); //cursor at start of next line
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(2, body.WordCount);

            Assert.Equal("qw>", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("erty>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(2, 6));

        }


        [Fact]
        public void IsCursorAtEndOfParagraphTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            body.SetTestLine(line1);

            Assert.Equal(ChapterModel.ChangeHint.End, body.InsertParaBreak().GetResult());
            Assert.Equal(2, body.GetLineCount());

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 0).GetResult());
            Assert.False(body.IsCursorAtEndOfParagraph());

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, line1.Length).GetResult());
            Assert.True(body.IsCursorAtEndOfParagraph());

        }

        [Fact]
        public void MoveCursorEmptyTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 0).GetResult());

            Assert.Equal(ChapterModel.ChangeHint.Unknown, body.MoveCursorInChapter(Body.CursorMove.PreviousCol).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Unknown, body.MoveCursorInChapter(Body.CursorMove.NextCol).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Unknown, body.MoveCursorInChapter(Body.CursorMove.PreviousRow).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Unknown, body.MoveCursorInChapter(Body.CursorMove.NextRow).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Unknown, body.MoveCursorInChapter(Body.CursorMove.StartChapter).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Unknown, body.MoveCursorInChapter(Body.CursorMove.EndChapter).GetResult());
        }

        [Fact]
        public void MoveCursorEndOfLineTest()
        {
            var maxColIndex = 9;
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.ChapterBody.GetLineCount());

            var line1 = "0123456789";
            Assert.True(manuscriptNew.BodyInsertText(line1).GetResult());

            Assert.Equal("0123456789>", manuscriptNew.ChapterBody.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(1, manuscriptNew.ChapterBody.GetLineCount());
            Assert.Equal(0, manuscriptNew.ChapterBody.Cursor.RowIndex);
            Assert.Equal(10, manuscriptNew.ChapterBody.Cursor.ColIndex);

            Assert.True(manuscriptNew.BodyMoveCursor(Body.CursorMove.PreviousCol).GetResult());

        }

        [Fact]
        public void MoveCursorOneLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line = "qwerty";
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(line).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 0).GetResult());

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.PreviousRow).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.NextRow).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.PreviousCol).GetResult());

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.NextCol).GetResult());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.Cursor.ColIndex);

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.StartChapter).GetResult());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.EndChapter).GetResult());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(line.Length, body.Cursor.ColIndex);
        }

        [Fact]
        public void MoveCursorThreeLineTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            body.SetTestLine(line1);
            var line2 = "0123456789";
            body.SetTestLine(line2);
            var line3 = "abc";
            body.SetTestLine(line3);

            Assert.Equal(3, body.GetLineCount());

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 0).GetResult());

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.PreviousRow).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.PreviousCol).GetResult());

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.NextCol).GetResult());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.Cursor.ColIndex);

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.StartChapter).GetResult());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);

            Assert.Contains("Warning: you cannot move beyond the start of the chapter", body.MoveCursorInChapter(Body.CursorMove.PreviousCol).GetErrorUserMsg());
            Assert.Contains("Warning: you cannot move beyond the start of the chapter", body.MoveCursorInChapter(Body.CursorMove.PreviousRow).GetErrorUserMsg());

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.EndChapter).GetResult());
            Assert.Equal(2, body.Cursor.RowIndex);
            Assert.Equal(line3.Length, body.Cursor.ColIndex);

            Assert.Contains("Warning: you cannot move beyond the end of the chapter", body.MoveCursorInChapter(Body.CursorMove.NextCol).GetErrorUserMsg());
            Assert.Contains("Warning: you cannot move beyond the end of the chapter", body.MoveCursorInChapter(Body.CursorMove.NextRow).GetErrorUserMsg());


            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.PreviousCol).GetResult());
            Assert.Equal(2, body.Cursor.RowIndex);
            Assert.Equal(line3.Length-1, body.Cursor.ColIndex);

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.PreviousRow).GetResult());
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(line3.Length-1, body.Cursor.ColIndex); //don't move column - line2.length> line3.line

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.MoveCursorInChapter(Body.CursorMove.PreviousRow).GetResult());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(line3.Length - 1, body.Cursor.ColIndex); //don't move column - line1.length> line3.line
        }


        [Fact]
        public void DeleteCharacterCursorStartOneLineTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            body.SetTestLine(line1);
            Assert.Equal("qwerty", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0,0).GetResult());

            Assert.Equal(ChapterModel.ChangeHint.Line, body.DeleteCharacter().GetResult());
            Assert.Equal("werty", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(ChapterModel.ChangeHint.Line, body.DeleteCharacter().GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.DeleteCharacter().GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.DeleteCharacter().GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.DeleteCharacter().GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.DeleteCharacter().GetResult());

            Assert.Equal(ChapterModel.ChangeHint.Unknown, body.DeleteCharacter().GetResult());
        }

        [Fact]
        public void DeleteCharacterCursorEndOneLineTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            body.SetTestLine(line1);
            Assert.Equal("qwerty", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 5).GetResult());

            Assert.Equal(ChapterModel.ChangeHint.Line, body.DeleteCharacter().GetResult());
            Assert.Equal("qwert", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 4).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.DeleteCharacter().GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 3).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.DeleteCharacter().GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 2).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.DeleteCharacter().GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 1).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.DeleteCharacter().GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 0).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.DeleteCharacter().GetResult());

            Assert.Contains("Warning: Chapter is empty", body.DeleteCharacter().GetErrorUserMsg());

        }

        [Fact]
        public void DeleteCharacterParaBreakTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            body.SetTestLine(line1);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal("qwerty", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 2).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.End, body.InsertParaBreak().GetResult());

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
            Assert.Equal("qw>", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("erty", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 2).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.End, body.DeleteCharacter().GetResult());
            Assert.Equal("qw erty", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        }

        [Fact]
        public void BackSpaceParaBreakTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.ChapterBody.GetLineCount());

            var line1 = "qwerty";
            Assert.True(manuscriptNew.BodyInsertText(line1).GetResult());

            Assert.Equal(ChapterModel.ChangeHint.Cursor, manuscriptNew.ChapterBody.SetCursorInChapter(0, 2).GetResult());
            Assert.True(manuscriptNew.BodyInsertParaBreak().GetResult());

            Assert.Equal(2, manuscriptNew.ChapterBody.GetLineCount());
            Assert.Equal(1, manuscriptNew.ChapterBody.Cursor.RowIndex);
            Assert.Equal(0, manuscriptNew.ChapterBody.Cursor.ColIndex);

            Assert.Equal("qw>", manuscriptNew.ChapterBody.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("erty>", manuscriptNew.ChapterBody.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            Assert.Equal(ChapterModel.ChangeHint.Cursor, manuscriptNew.ChapterBody.SetCursorInChapter(1, 0).GetResult());
            Assert.True(manuscriptNew.BodyBackSpace().GetResult());
            Assert.Equal("qw erty>", manuscriptNew.ChapterBody.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        }

        [Fact]
        public void BackSpaceEndOfLineTest()
        {
            var maxColIndex = 9;
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(TestConst.TextEditorDisplayRows, maxColIndex+1, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.ChapterBody.GetLineCount());

            var line1 = "0123456789";
            Assert.True(manuscriptNew.BodyInsertText(line1).GetResult());

            Assert.Equal("0123456789>", manuscriptNew.ChapterBody.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(1, manuscriptNew.ChapterBody.GetLineCount());
            Assert.Equal(0, manuscriptNew.ChapterBody.Cursor.RowIndex);
            Assert.Equal(10, manuscriptNew.ChapterBody.Cursor.ColIndex);

            Assert.True(manuscriptNew.BodyBackSpace().GetResult());
            Assert.Equal("012345678>", manuscriptNew.ChapterBody.GetEditAreaLinesForDisplay(1).GetResult()[0]);

        }

        [Fact]
        public void BackspaceOneLineTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.ChapterBody.GetLineCount());

            var line1 = "qwerty";
            Assert.Equal(ChapterModel.ChangeHint.Line, manuscriptNew.ChapterBody.InsertText(line1).GetResult());
            Assert.Equal("qwerty>", manuscriptNew.ChapterBody.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.True(manuscriptNew.BodyMoveCursor(Body.CursorMove.EndChapter).GetResult());
            Assert.Equal(6, manuscriptNew.ChapterBody.Cursor.ColIndex);

            Assert.True(manuscriptNew.BodyBackSpace().GetResult());
            Assert.Equal(5, manuscriptNew.ChapterBody.Cursor.ColIndex);
            Assert.Equal("qwert>", manuscriptNew.ChapterBody.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.True(manuscriptNew.BodyBackSpace().GetResult()); 
            Assert.Equal("qwer>", manuscriptNew.ChapterBody.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.True(manuscriptNew.BodyBackSpace().GetResult()); //qwe>
            Assert.Equal("qwe>", manuscriptNew.ChapterBody.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.True(manuscriptNew.BodyBackSpace().GetResult()); //qw>
            Assert.Equal("qw>", manuscriptNew.ChapterBody.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.True(manuscriptNew.BodyBackSpace().GetResult()); //q>
            Assert.Equal("q>", manuscriptNew.ChapterBody.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.True(manuscriptNew.BodyBackSpace().GetResult()); //>

            Assert.Contains("Warning: Chapter is empty", manuscriptNew.BodyBackSpace().GetErrorUserMsg());
        }


        [Fact]
        public void InsertTextInsertInitialTextTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("aaa").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal("aaa>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextAppendLineTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("one");
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(" aaa").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("one aaa", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextOverwriteStartTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("one two three");
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 0);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("xxx").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("xxx two three", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextOverwriteMiddleTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("one two three");
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 4);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("xxx").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("one xxx three", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextOverwriteToEndTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("one two three");
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 8);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("xxxxx").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("one two xxxxx", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextOverwriteBeyondEndTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("one two three");
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 13);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(" xxx").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("one two three xxx", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextInsertStartTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("one two three");
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 0);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("xxx ", true).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("xxx one two three", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextInsertMiddleTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("one two three");
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 4);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("xxx ", true).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("one xxx two three", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextInsertEndTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("one two three");
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            body.SetCursorInChapter(0, 13);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(" xxx", true).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("one two three xxx", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }


        [Fact]
        public void InsertTextLongLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, 35).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789.123456789.123456789.1234";
            Assert.Equal(35, line.Length);

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(line).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);
            Assert.Equal(line + body.ParaBreakDisplayChar.ToString(), body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            var tooLong = line + "x";
            Assert.False( body.InsertText(tooLong).IsSuccess(true));
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);
            Assert.Equal(line + body.ParaBreakDisplayChar.ToString(), body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }


        [Fact]
        public void InsertTextCharacterTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('b'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('c'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("ab c>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.Equal(" >", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextMultiSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('b'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('c'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("ab    c>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextStartSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('b'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal(" a b>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextStartMultiSpaceTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(' '.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('b'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("   a b>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextTabTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(3, body.TabSpaces.Length);

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('b'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(body.TabSpaces).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('c'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("ab   c>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void InsertTextStartTabTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(3, body.TabSpaces.Length);

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(body.TabSpaces).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(body.TabSpaces).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('b'.ToString()).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal("   a   b>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }


        [Fact]
        public void InsertTextInsertSecondLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, 35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("aaaaa").GetResult());  //col #5
            Assert.Equal("aaaaa>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(5, body.Cursor.ColIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(" bbbb").GetResult());   //col #10
            Assert.Equal("aaaaa bbbb>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(10, body.Cursor.ColIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(" 123456789").GetResult()); //col #20
            Assert.Equal("aaaaa bbbb 123456789>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(20, body.Cursor.ColIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(" 123456789").GetResult()); //col #30
            Assert.Equal("aaaaa bbbb 123456789 123456789>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(30, body.Cursor.ColIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(" 1234").GetResult());      //col #35
            Assert.Equal("aaaaa bbbb 123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(35, body.Cursor.ColIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(5, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.End, body.InsertText("x").GetResult());          //col #36
            Assert.Equal("aaaaa bbbb 123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("1234x>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(5, body.Cursor.ColIndex);
            Assert.Equal(1, body.Cursor.RowIndex);
        }

        [Fact]
        public void InsertTextSplitLineTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, 65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            Assert.Equal(65, line.Length);
            body.SetTestLine(line);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(7, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.End, body.InsertText('a'.ToString()).GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("0123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("1234a", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        }

        [Fact]
        public void InsertTextSpaceSplitLineTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, 65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.WordCount);

            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            Assert.Equal(65, line.Length);

            body.SetTestLine(line);

            Assert.Equal(line, body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(65, body.Cursor.ColIndex); //one char after end
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(7, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.End, body.InsertText(' '.ToString()).GetResult());

            Assert.Equal("0123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("1234 ", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(5, body.Cursor.ColIndex); //one char after end
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(7, body.WordCount);

            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText('x'.ToString()).GetResult());

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
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(line1).GetResult());

            Assert.Equal(Program.PosIntegerNotSet, body.GetNextParaBreakRowIndex(-1));
            Assert.Equal(0, body.GetNextParaBreakRowIndex(0));
            Assert.Equal(Program.PosIntegerNotSet, body.GetNextParaBreakRowIndex(1));
        }

        [Fact]
        public void GetNextParaBreakNotFoundTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            body.SetTestLine(line1);
            body.SetTestLine(line1);
            body.SetTestLine(line1);
            Assert.Equal(3, body.GetLineCount());

            Assert.Equal("qwerty", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);

            Assert.Equal(2, body.GetNextParaBreakRowIndex(0));
            Assert.Equal(2, body.GetNextParaBreakRowIndex(1));
            Assert.Equal(2, body.GetNextParaBreakRowIndex(2));
        }

        [Fact]
        public void GetNextParaBreakTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, TestConst.TextEditorDisplayCols).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "qwerty";
            body.SetTestLine(line1);
            body.SetTestLine(line1);
            body.SetTestLine(line1);
            Assert.Equal("qwerty", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(3, body.GetLineCount());

            //0 qwerty
            //1 qwerty
            //2 qwerty
            
            Assert.Equal(2, body.GetNextParaBreakRowIndex(0));

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(1, 2).GetResult());
            Assert.Equal(ChapterModel.ChangeHint.End, body.InsertParaBreak().GetResult()); //invokes LeftJustifyLinesInParagraph()
            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(2, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);

            Assert.Equal("qwerty", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("qw>", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("erty qwerty", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);

            //0 qwerty
            //1 qw>
            //2 erty qwerty

            Assert.Equal(1, body.GetNextParaBreakRowIndex(0));
            Assert.Equal(2, body.GetNextParaBreakRowIndex(2));
        }

        [Fact]
        public void LeftJustifyLinesInParagraphTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, 15).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());

            var line1 = "0123456789 1234";
            body.SetTestLine(line1);
            body.SetTestLine(line1);
            body.SetTestLine(line1);


            Assert.Equal(3, body.GetLineCount());

           // Assert.True(body.LeftJustifyLinesInParagraph(0, 15, 0));

        }



    }
}
