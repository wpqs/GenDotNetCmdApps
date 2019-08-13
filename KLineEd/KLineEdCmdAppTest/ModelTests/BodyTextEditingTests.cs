using System;
using System.Collections.Generic;
using System.Text;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using KLineEdCmdAppTest.TestSupport;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;
using Program = KLineEdCmdApp.Program;

// ReSharper disable All

namespace KLineEdCmdAppTest.ModelTests
{
    public class BodyTextEditingTests
    {

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
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(2, 7));

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

            Assert.Equal("qwerty", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);

            Assert.True(body.InsertParaBreak().GetResult());

            Assert.Equal(0, body.Cursor.ColIndex); //cursor at start of next line
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(2, body.WordCount);

            Assert.Equal("qw>", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("erty", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(Body.NullChar, body.GetCharacterInLine(2, 5));

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

            Assert.True(body.MoveCursorInChapter(Body.CursorMove.End).GetResult());
            Assert.Equal(2, body.Cursor.RowIndex);
            Assert.Equal(line3.Length, body.Cursor.ColIndex);

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
        public void DeleteCharacterOneLineTest()
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
        public void BackspaceOneLineTest()
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
            Assert.True(body.DeleteCharacter().GetResult());
            Assert.True(body.DeleteCharacter().GetResult());
            Assert.True(body.DeleteCharacter().GetResult());
            Assert.True(body.DeleteCharacter().GetResult());
            Assert.True(body.DeleteCharacter().GetResult());

        //    Assert.False(body.DeleteCharacter().GetResult());

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
