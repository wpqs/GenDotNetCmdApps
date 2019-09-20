using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ModelTests
{
    public class BodyDisplayLinesTest
    {
        [Fact]
        public void GetLastLinesTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.NotNull(body.GetEditAreaLinesForDisplay(CmdLineParamsApp.ArgEditDisplayRowsDefault).GetResult());
        }

        [Fact]
        public void GetLastLinesNotInitializedTest()
        {
            var body = new Body();
            Assert.True(body.IsError());
            Assert.Null(body.GetEditAreaLinesForDisplay(CmdLineParamsApp.ArgEditDisplayRowsDefault).GetResult());
        }

        [Fact]
        public void GetLastLinesOneTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("one");
            body.SetTestLine("two");
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal("two", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        }

        [Fact]
        public void GetLastLinesMaxTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(CmdLineParamsApp.ArgEditDisplayRowsMax, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("one");
            body.SetTestLine("two");
            Assert.Equal(2, body.GetLineCount());

            Assert.Equal("one", body.GetEditAreaLinesForDisplay(CmdLineParamsApp.ArgEditDisplayRowsMax).GetResult()[0]);
            Assert.Equal("two", body.GetEditAreaLinesForDisplay(CmdLineParamsApp.ArgEditDisplayRowsMax).GetResult()[1]);
        }

        [Fact]
        public void GetLastLinesMoreThanMaxTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("one");
            body.SetTestLine("two");
            Assert.Equal(2, body.GetLineCount());
            Assert.Null(body.GetEditAreaLinesForDisplay(CmdLineParamsApp.ArgEditDisplayRowsMax + 1).GetResult());
        }

        [Fact]
        public void SetEditAreaTopLineChapterIndexFiveLineTest()
        {
            var displayHt = 5;
            var body = new MockModelBody();
            Assert.True(body.Initialise(displayHt, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(4, body.EditAreaViewCursorLimit.RowIndex);

            body.SetTestLine("0aaaaaa");
            body.SetTestLine("1bbbbbb");
            body.SetTestLine("2cccccc");
            body.SetTestLine("3dddddd");
            body.SetTestLine("4eeeeee");
            body.SetTestLine("5ffffff");
            body.SetTestLine("6gggggg");
            body.SetTestLine("7hhhhhh");
            body.SetTestLine("8iiiiii");
            body.SetTestLine("9jjjjjj");
            body.SetTestLine("10kkkkk");

            Assert.Equal(11, body.GetLineCount());
            Assert.Equal(10, body.Cursor.RowIndex);

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetEditAreaTopLineChapterIndex(Body.Scroll.ToCursor).GetResult());
            Assert.Equal(10-(displayHt-1), body.EditAreaTopLineChapterIndex);

            Assert.Equal(ChapterModel.ChangeHint.All, body.SetCursorInChapter(0, 0).GetResult()); //invokes SetEditAreaTopLineChapterIndex
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.EditAreaTopLineChapterIndex);

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(1, 0).GetResult());
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(0, body.EditAreaTopLineChapterIndex);

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(4,0).GetResult()); 
            Assert.Equal(4, body.Cursor.RowIndex);
            Assert.Equal(0, body.EditAreaTopLineChapterIndex);

            Assert.Equal(ChapterModel.ChangeHint.All, body.SetCursorInChapter(5, 0).GetResult()); 
            Assert.Equal(5, body.Cursor.RowIndex);
            Assert.Equal(5- (displayHt - 1), body.EditAreaTopLineChapterIndex);      //move down one line
            Assert.Equal(1, body.EditAreaTopLineChapterIndex);

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(4, 0).GetResult()); 
            Assert.Equal(4, body.Cursor.RowIndex);
            Assert.Equal(1, body.EditAreaTopLineChapterIndex);                      //move up one line

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(3, 0).GetResult());
            Assert.Equal(3, body.Cursor.RowIndex);
            Assert.Equal(1, body.EditAreaTopLineChapterIndex);                      //move up one line

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(2, 0).GetResult());
            Assert.Equal(2, body.Cursor.RowIndex);
            Assert.Equal(1, body.EditAreaTopLineChapterIndex);                      //move up one line

            Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(1, 0).GetResult());
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(1, body.EditAreaTopLineChapterIndex);                      //move up one line

            Assert.Equal(ChapterModel.ChangeHint.All, body.SetCursorInChapter(0, 0).GetResult());
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.EditAreaTopLineChapterIndex);                      //move up one line

            Assert.Equal(ChapterModel.ChangeHint.All, body.SetCursorInChapter(9, 0).GetResult()); 
            Assert.Equal(9, body.Cursor.RowIndex);
            Assert.Equal(9- (displayHt - 1), body.EditAreaTopLineChapterIndex);     //move to penultimate line
            Assert.Equal(5, body.EditAreaTopLineChapterIndex);

            Assert.Equal(ChapterModel.ChangeHint.All, body.SetCursorInChapter(0, 0).GetResult()); 
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.EditAreaTopLineChapterIndex);                      //move to top line

            Assert.Equal(ChapterModel.ChangeHint.All, body.SetCursorInChapter(10, 0).GetResult()); 
            Assert.Equal(10, body.Cursor.RowIndex);
            Assert.Equal(10 - (displayHt - 1), body.EditAreaTopLineChapterIndex);   //move to bottom line
        }

    }
}
