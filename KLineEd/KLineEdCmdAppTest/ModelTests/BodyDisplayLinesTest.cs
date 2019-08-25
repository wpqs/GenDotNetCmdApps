using System;
using System.Collections.Generic;
using System.Text;
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
            Assert.True(body.Initialise(CmdLineParamsApp.ArgEditAreaLinesCountMax, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            body.SetTestLine("one");
            body.SetTestLine("two");
            Assert.Equal(2, body.GetLineCount());

            Assert.Equal("one", body.GetEditAreaLinesForDisplay(CmdLineParamsApp.ArgEditAreaLinesCountMax).GetResult()[0]);
            Assert.Equal("two", body.GetEditAreaLinesForDisplay(CmdLineParamsApp.ArgEditAreaLinesCountMax).GetResult()[1]);
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
            Assert.Null(body.GetEditAreaLinesForDisplay(CmdLineParamsApp.ArgEditAreaLinesCountMax + 1).GetResult());
        }

        [Fact]
        public void SetEditAreaBottomChapterIndexFiveDisplayLineTest()
        {
            var body = new MockModelBody();
            Assert.True(body.Initialise(5, TestConst.UnitTestEditAreaWidth).GetResult());
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
            var body = new MockModelBody();
            Assert.True(body.Initialise(1, TestConst.UnitTestEditAreaWidth).GetResult());
            Assert.False(body.IsError());
            Assert.Equal(0, body.GetLineCount());
            Assert.Equal(0, body.EditAreaViewCursorLimit.RowIndex);

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
