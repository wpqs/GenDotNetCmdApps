using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using KLineEdCmdAppTest.TestSupport;
using Microsoft.Azure.Services.AppAuthentication;
using Xunit;

namespace KLineEdCmdAppTest.ModelTests
{
    public class BodyLeftJustificationTests
    {
        [Fact]
        public void GetSplitIndexFromStartParamFailTest()
        {
            Assert.Equal(-1, Body.GetSplitIndexFromStart(null, 1));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(null, 0));
        }

        [Fact]
        public void GetSplitIndexFromStartSmallLineTest()
        {
            var line = "0123456789";
            Assert.Equal(10, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 9));
            Assert.Equal(9, Body.GetSplitIndexFromStart(line, 10));
            Assert.Equal(9, Body.GetSplitIndexFromStart(line, 11));
        }

        [Fact]
        public void GetSplitIndexFromStartSmallLineParaBreakTest()
        {
            var line = "0123456789" + Body.ParaBreak;
            Assert.Equal(11, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 9));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 10));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 11));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 12));
        }


        [Fact]
        public void GetSplitIndexFromStartMinTest()
        {
            var line1 = " 123456789x123456789x123456789x123456789x123456789x123456789x1234";
            Assert.Equal(65, line1.Length);
            Assert.Equal(0, Body.GetSplitIndexFromStart(line1, 1));

            var line2 = "0 123456789x123456789x123456789x123456789x123456789x123456789x123";
            Assert.Equal(65, line2.Length);
            Assert.Equal(1, Body.GetSplitIndexFromStart(line2, 1));

            var line3 = "01 23456789x123456789x123456789x123456789x123456789x123456789x123";
            Assert.Equal(65, line3.Length);
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line3, 1));

        }

        [Fact]
        public void GetSplitIndexFromStartBasicTest()
        {
            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            Assert.Equal(65, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 0));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 1));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 2));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 3));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 9));

            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 10));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 11));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 19));

            Assert.Equal(20, Body.GetSplitIndexFromStart(line, 20));
            Assert.Equal(20, Body.GetSplitIndexFromStart(line, 21));

            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 59));

            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 60));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 61));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 62));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 63));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 64));

            Assert.Equal(64, Body.GetSplitIndexFromStart(line, 65));
            Assert.Equal(64, Body.GetSplitIndexFromStart(line, 66));
        }

        [Fact]
        public void GetSplitIndexFromStartParaBreakLineTest()
        {
            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(66, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 0));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 1));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 2));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 3));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 9));

            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 10));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 11));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 19));

            Assert.Equal(20, Body.GetSplitIndexFromStart(line, 20));
            Assert.Equal(20, Body.GetSplitIndexFromStart(line, 21));

            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 59));

            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 60));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 61));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 62));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 63));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 64));

            Assert.Equal(65, Body.GetSplitIndexFromStart(line, 65));
            Assert.Equal(65, Body.GetSplitIndexFromStart(line, 66));
        }

        [Fact]
        public void GetSplitIndexFromStartMultiSpaceTest()
        {   //                      1         2         3         4         5         6
            //var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            var line =   "0123456789 123456789 123456789 123456789 123456789 1234567   1234";
            Assert.Equal(65, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 0));

            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 50));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 51));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 52));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 57));

            Assert.Equal(58, Body.GetSplitIndexFromStart(line, 58));
            Assert.Equal(59, Body.GetSplitIndexFromStart(line, 59));

            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 60));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 61));

        }

        [Fact]
        public void GetSplitIndexFromStartMultiSpaceParaBreakTest()
        {   //                      1         2         3         4         5         6
            //var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            var line = "0123456789 123456789 123456789 123456789 123456789 1234567   1234" + Body.ParaBreak;
            Assert.Equal(66, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 0));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 50));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 51));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 52));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 57));

            Assert.Equal(58, Body.GetSplitIndexFromStart(line, 58));
            Assert.Equal(59, Body.GetSplitIndexFromStart(line, 59));

            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 60));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 61));
        }

        [Fact]
        public void GetSplitIndexFromEndParamFailTest()
        {
            var line = "0123456789";
            Assert.Equal(10, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromEnd(null, 1));
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 0));
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 11));

        }

        [Fact]
        public void GetSplitIndexFromEndBasicTest()
        {             //0         1         2         3         4         5 
            var line = "0123456789 123456789 123456789 123456789 123456789 123456789";
                            
            Assert.Equal(60, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 61));
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 60));
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 59));  //no point in splitting a line at the very end 

            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 58));
            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 51));
            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 50));

            Assert.Equal(40, Body.GetSplitIndexFromEnd(line, 49));
            Assert.Equal(40, Body.GetSplitIndexFromEnd(line, 41));
            Assert.Equal(40, Body.GetSplitIndexFromEnd(line, 40));

            Assert.Equal(30, Body.GetSplitIndexFromEnd(line, 39));
            Assert.Equal(30, Body.GetSplitIndexFromEnd(line, 31));
            Assert.Equal(30, Body.GetSplitIndexFromEnd(line, 30));

            Assert.Equal(20, Body.GetSplitIndexFromEnd(line, 29));
            Assert.Equal(20, Body.GetSplitIndexFromEnd(line, 21));
            Assert.Equal(20, Body.GetSplitIndexFromEnd(line, 20));

            Assert.Equal(10, Body.GetSplitIndexFromEnd(line, 19));
            Assert.Equal(10, Body.GetSplitIndexFromEnd(line, 11));
            Assert.Equal(10, Body.GetSplitIndexFromEnd(line, 10));

            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 9));
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 8));

            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 2));
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 1));
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 0));
        }

        [Fact]
        public void GetSplitIndexFromEndLimitTest()
        {
            var line0 = "0123456789x123456789x123456789x123456789x123456789x123456789x1234";
            Assert.Equal(65, line0.Length);
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line0, 64));

            var line1 = " 123456789x123456789x123456789x123456789x123456789x123456789x1234";
            Assert.Equal(65, line1.Length);
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line1, 64));
            Assert.Equal(0, Body.GetSplitIndexFromEnd(line1, 63));

            var line2 = "0123456789x123456789x123456789x123456789x123456789x123456789x123 ";
            Assert.Equal(65, line2.Length);
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line2, 64));
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line2, 63));

            var line3 = "0123456789x123456789x123456789x123456789x123456789x123456789x12 4";
            Assert.Equal(65, line3.Length);
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line3, 64));
            Assert.Equal(63, Body.GetSplitIndexFromEnd(line3, 63));
        }

        [Fact]
        public void GetSplitIndexFromEndMultiSpaceTest()
        {
                     //"0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            var line = "0123456789 123456789 123456789 123456789 123456789 1234567   1234";
            Assert.Equal(65, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 64));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 63));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 62));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 61));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 60));
            Assert.Equal(59, Body.GetSplitIndexFromEnd(line, 59));
            Assert.Equal(58, Body.GetSplitIndexFromEnd(line, 58));

            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 57));
            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 56));

            Assert.Equal(10, Body.GetSplitIndexFromEnd(line, 11));
            Assert.Equal(10, Body.GetSplitIndexFromEnd(line, 10));
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 9));
        }

        [Fact]
        public void SplitLongLineParamFailTest()
        {
            var maxColIndex = 9;
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex+1).GetResult());
            Assert.False(body.IsError());

            Assert.False(body.SplitLongLine(0, 0, -1, out var updatedCursorIndex).GetResult());
            Assert.False(body.SplitLongLine(0, -1, CmdLineParamsApp.ArgEditAreaLineWidthMin-1, out updatedCursorIndex).GetResult());
            Assert.False(body.SplitLongLine(-1, 0, CmdLineParamsApp.ArgEditAreaLineWidthMin - 1, out updatedCursorIndex).GetResult());
            Assert.False(body.SplitLongLine(1, 0, CmdLineParamsApp.ArgEditAreaLineWidthMin - 1, out updatedCursorIndex).GetResult());

            Assert.Equal(-1, updatedCursorIndex);
        }

        [Fact]
        public void SplitLongLineBasicTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123 56789ABC";
            Assert.Equal(13, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("0123 56789ABC", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(line.Length, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.True(body.SplitLongLine(0, 9, maxColIndex, out var updatedCursorIndex).GetResult());

            Assert.Equal("0123", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("56789ABC", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(8, updatedCursorIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void SplitLongLineParaBreakBasicTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123 56789ABC";
            Assert.Equal(13, line.Length);
            body.SetTestLine(line + Body.ParaBreak); //use test method to add line > maxColIndex

            Assert.Equal("0123 56789ABC>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(line.Length, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.True(body.SplitLongLine(0, 9, maxColIndex, out var updatedCursorIndex).GetResult());

            Assert.Equal("0123", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("56789ABC>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(8, updatedCursorIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void SplitLongLineTooShortFailTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123 5678";
            Assert.Equal(9, line.Length);
            body.SetTestLine(line); //use test method to add line == maxColIndex

            Assert.Equal("0123 5678", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(line.Length, body.Cursor.ColIndex);

            Assert.False(body.SplitLongLine(0, 0, maxColIndex, out var updatedCursorIndex).GetResult());
        }

        [Fact]
        public void SplitLongLineParaBreakTooShortFailTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123 5678";
            Assert.Equal(9, line.Length);
            body.SetTestLine(line + Body.ParaBreak); //use test method to add line == maxColIndex

            Assert.Equal("0123 5678>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(line.Length, body.Cursor.ColIndex);

            Assert.False(body.SplitLongLine(0, 0, maxColIndex, out var updatedCursorIndex).GetResult());
        }

        [Fact]
        public void SplitLongLineSmallestLineNoSpaceTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789A";
            Assert.Equal(11, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("0123456789A", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(1, body.WordCount);
            Assert.Equal(line.Length, body.Cursor.ColIndex);

            Assert.True(body.SplitLongLine(0, 0, maxColIndex, out var updatedCursorIndex).GetResult());

            Assert.Equal("0123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]); //"0123456789-"
            Assert.Equal("A", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(1, updatedCursorIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void SplitLongLineParaBreakSmallestLineNoSpaceTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789A";
            Assert.Equal(11, line.Length);
            body.SetTestLine(line + Body.ParaBreak); //use test method to add line > maxColIndex

            Assert.Equal("0123456789A>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(1, body.WordCount);
            Assert.Equal(line.Length, body.Cursor.ColIndex);

            Assert.True(body.SplitLongLine(0, 0, maxColIndex, out var updatedCursorIndex).GetResult());

            Assert.Equal("0123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]); //"0123456789-"
            Assert.Equal("A>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(1, updatedCursorIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void SplitLongLineSmallestLineSpaceTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "01234 6789A";
            Assert.Equal(11, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("01234 6789A", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(line.Length, body.Cursor.ColIndex);

            Assert.True(body.SplitLongLine(0, 9, maxColIndex, out var updatedCursorIndex).GetResult());

            Assert.Equal("01234", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("6789A", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(5, updatedCursorIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void SplitLongLineParaBreakSmallestLineSpaceTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "01234 6789";
            Assert.Equal(10, line.Length);
            body.SetTestLine(line + Body.ParaBreak); //use test method to add line > maxColIndex

            Assert.Equal("01234 6789>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(line.Length, body.Cursor.ColIndex);

            Assert.True(body.SplitLongLine(0, 9, maxColIndex, out var updatedCursorIndex).GetResult());

            Assert.Equal("01234", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("6789>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(4, updatedCursorIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void SplitLongLineMaxColLineTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0 0123456789";
            Assert.Equal(12, line1.Length);
            body.SetTestLine(line1); //use test method to add line == maxColIndex

            Assert.Equal("0 0123456789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(line1.Length, body.Cursor.ColIndex);

            Assert.True(body.SplitLongLine(0, 9, maxColIndex, out var updatedCursorIndex).GetResult());

            Assert.Equal("0", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("0123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(maxColIndex+1, updatedCursorIndex); //maxCursorColIndex == maxColIndex+1
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);

            Assert.True(body.RemoveAllLines().GetResult());

            var line2 = "0 0123456789A";
            Assert.Equal(13, line2.Length);
            body.SetTestLine(line2); //use test method to add line > maxColIndex

            Assert.Equal("0 0123456789A", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(line2.Length, body.Cursor.ColIndex);

            Assert.True(body.SplitLongLine(0, 9, maxColIndex, out updatedCursorIndex).GetResult());

            Assert.Equal("0", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("0123456789A", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(maxColIndex+2, updatedCursorIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void SplitLongLineMultiSpaceLineTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123  0123456789";
            Assert.Equal(16, line1.Length);
            body.SetTestLine(line1); //use test method to add line == maxColIndex

            Assert.Equal("0123  0123456789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(line1.Length, body.Cursor.ColIndex);

            Assert.True(body.SplitLongLine(0, 9, maxColIndex, out var updatedCursorIndex).GetResult());

            Assert.Equal("0123 ", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("0123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(maxColIndex + 1, updatedCursorIndex); //maxCursorColIndex == maxColIndex+1
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void FillShortLineParamFailTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            Assert.False(body.FillShortLine(0, 0, -1, out var updatedCursorColIndex).GetResult());
            Assert.Equal(-1, updatedCursorColIndex);
            Assert.False(body.FillShortLine(0,  -1, CmdLineParamsApp.ArgEditAreaLineWidthMin - 1, out updatedCursorColIndex).GetResult());
            Assert.Equal(-1, updatedCursorColIndex);
            Assert.False(body.FillShortLine(-1, 0, CmdLineParamsApp.ArgEditAreaLineWidthMin - 1, out updatedCursorColIndex).GetResult());
            Assert.Equal(-1, updatedCursorColIndex);
            Assert.False(body.FillShortLine(1, 0, CmdLineParamsApp.ArgEditAreaLineWidthMin - 1, out updatedCursorColIndex).GetResult());
            Assert.Equal(-1, updatedCursorColIndex);

        }

        [Fact]
        public void FillShortLineBasicTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123";
            Assert.Equal(4, line1.Length);
            body.SetTestLine(line1);
            var line2 = "456";
            Assert.Equal(3, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal("0123", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("456", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(line2.Length, body.Cursor.ColIndex);

            Assert.True(body.FillShortLine(0, 0, maxColIndex, out var updatedCursorColIndex).GetResult());

            Assert.Equal(0, updatedCursorColIndex);
            Assert.Equal("0123456", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void FillShortParaBreakLineBasicTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123";
            Assert.Equal(4, line1.Length);
            body.SetTestLine(line1);
            var line2 = "456" + Body.ParaBreak;
            Assert.Equal(4, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal("0123", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("456>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(line2.Length-1, body.Cursor.ColIndex);

            Assert.True(body.FillShortLine(0, 0, maxColIndex, out var updatedCursorColIndex).GetResult());

            Assert.Equal(0, updatedCursorColIndex);
            Assert.Equal("0123456>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void FillShortLineTooLongFailTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "012345";
            Assert.Equal(6, line1.Length);
            body.SetTestLine(line1);
            var line2 = "789A";
            Assert.Equal(4, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal("012345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("789A", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(4, body.Cursor.ColIndex);

            Assert.False(body.FillShortLine(0, 0, maxColIndex, out var updatedCursorColIndex).GetResult());
            Assert.Equal(0, updatedCursorColIndex);

            Assert.Equal("012345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("789A", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(4, body.Cursor.ColIndex);
        }

        [Fact]
        public void FillShortLineParaBreakTooLongFailTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "012345";
            Assert.Equal(6, line1.Length);
            body.SetTestLine(line1);
            var line2 = "789A" + Body.ParaBreak;
            Assert.Equal(5, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal("012345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("789A>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(4, body.Cursor.ColIndex);

            Assert.False(body.FillShortLine(0, 0, maxColIndex, out var updatedCursorColIndex).GetResult());
            Assert.Equal(0, updatedCursorColIndex);

            Assert.Equal("012345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("789A>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(4, body.Cursor.ColIndex);
        }

        [Fact]
        public void FillShortLineSpaceLineTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "012345";
            Assert.Equal(6, line1.Length);
            body.SetTestLine(line1);
            var line2 = "789 A";
            Assert.Equal(5, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal("012345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("789 A", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(3, body.WordCount);
            Assert.Equal(5, body.Cursor.ColIndex);

            Assert.True(body.FillShortLine(0, 0,  maxColIndex, out var updatedCursorColIndex).GetResult());

            Assert.Equal(0, updatedCursorColIndex);
            Assert.Equal("012345789 ", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("A", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
        }

        [Fact]
        public void FillShortParaBreakLineSpaceLineTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "012345";
            Assert.Equal(6, line1.Length);
            body.SetTestLine(line1);
            var line2 = "789 A" + Body.ParaBreak;
            Assert.Equal(6, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal("012345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("789 A>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(3, body.WordCount);
            Assert.Equal(5, body.Cursor.ColIndex);

            Assert.True(body.FillShortLine(0, 0, maxColIndex, out var updatedCursorColIndex).GetResult());

            Assert.Equal(0, updatedCursorColIndex);
            Assert.Equal("012345789 ", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("A>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
        }

        [Fact]
        public void FillShortLineMultiSpaceLineTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "012345";
            Assert.Equal(6, line1.Length);
            body.SetTestLine(line1);
            var line2 = "78  A";
            Assert.Equal(5, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal("012345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("78  A", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(3, body.WordCount);
            Assert.Equal(5, body.Cursor.ColIndex);

            Assert.True(body.FillShortLine(0, 0, maxColIndex, out var updatedCursorColIndex).GetResult());

            Assert.Equal(0, updatedCursorColIndex);
            Assert.Equal("01234578  ", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("A", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
        }


        [Fact]
        public void FillShortParaBreakLineMultiSpaceLineTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "012345";
            Assert.Equal(6, line1.Length);
            body.SetTestLine(line1);
            var line2 = "78  A" + Body.ParaBreak;
            Assert.Equal(6, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal("012345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("78  A>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(3, body.WordCount);
            Assert.Equal(5, body.Cursor.ColIndex);

            Assert.True(body.FillShortLine(0, 0, maxColIndex, out var updatedCursorColIndex).GetResult());

            Assert.Equal(0, updatedCursorColIndex);
            Assert.Equal("01234578  ", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("A>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
        }

        [Fact]
        public void FillShortLineLongestLineNoSpaceTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "012345";
            Assert.Equal(6, line1.Length);
            body.SetTestLine(line1);
            var line2 = "789";
            Assert.Equal(3, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal("012345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("789", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(3, body.Cursor.ColIndex);

            Assert.True(body.FillShortLine(0, 0, maxColIndex, out var updatedCursorColIndex).GetResult());

            Assert.Equal(0,updatedCursorColIndex);
            Assert.Equal("012345789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void FillShortLineParaBreakLongestLineNoSpaceTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "012345";
            Assert.Equal(6, line1.Length);
            body.SetTestLine(line1);
            var line2 = "789" + Body.ParaBreak;
            Assert.Equal(4, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal("012345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("789>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(3, body.Cursor.ColIndex);

            Assert.True(body.FillShortLine(0, 0, maxColIndex, out var updatedCursorColIndex).GetResult());

            Assert.Equal(0, updatedCursorColIndex);
            Assert.Equal("012345789>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphParamFailTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            Assert.False(body.LeftJustifyLinesInParagraph(0, -1).GetResult());
            Assert.False(body.LeftJustifyLinesInParagraph(-1, 0).GetResult());
        }

        [Fact]
        public void LeftJustifyLinesInParagraphParaBreakShortLineTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123";
            Assert.Equal(4, line1.Length);
            body.SetTestLine(line1);
            var line2 = "456" + Body.ParaBreak;
            Assert.Equal(4, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal("0123", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("456>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(3, body.Cursor.ColIndex);
            Assert.Equal(1, body.Cursor.RowIndex);

            Assert.True(body.LeftJustifyLinesInParagraph(0, 0).GetResult());

            Assert.Equal("0123456>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(0, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphNoLineTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.WordCount);
            Assert.Equal(0, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);

            var rc = body.LeftJustifyLinesInParagraph(0, 0);

            Assert.False(rc.IsError(true));
            Assert.Equal(0, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphOneLineLongTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "01 3456 89A12";
            Assert.Equal(13, line1.Length);
            body.SetTestLine(line1);

            Assert.Equal("01 3456 89A12", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(3, body.WordCount);
            Assert.Equal(13, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);

            Assert.True(body.LeftJustifyLinesInParagraph(0, 13).GetResult());

            Assert.Equal("01 3456", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("89A12", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(3, body.WordCount);
            Assert.Equal(5, body.Cursor.ColIndex);
            Assert.Equal(1, body.Cursor.RowIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphParaBreakOneLineLongTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "01 3456 89A12" + Body.ParaBreak;
            Assert.Equal(14, line1.Length);
            body.SetTestLine(line1);

            Assert.Equal("01 3456 89A12>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(3, body.WordCount);
            Assert.Equal(13, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);

            Assert.True(body.LeftJustifyLinesInParagraph(0, 13).GetResult());

            Assert.Equal("01 3456", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("89A12>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(3, body.WordCount);
            Assert.Equal(5, body.Cursor.ColIndex); //long line split, so cursor goes from end of row 0 to end of row 1
            Assert.Equal(1, body.Cursor.RowIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphOneLineShortTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123 56789";
            Assert.Equal(10, line1.Length);
            body.SetTestLine(line1);

            Assert.Equal("0123 56789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(10, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);

            Assert.True(body.LeftJustifyLinesInParagraph(0, 10).GetResult());

            Assert.Equal("0123 56789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(10, body.Cursor.ColIndex); //line not split so cursor doesn't move
            Assert.Equal(0, body.Cursor.RowIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphParamBreakOneLineShortTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123 56789" + Body.ParaBreak;
            Assert.Equal(11, line1.Length);
            body.SetTestLine(line1);

            Assert.Equal("0123 56789>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(10, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);

            Assert.True(body.LeftJustifyLinesInParagraph(0, 10).GetResult());

            Assert.Equal("0123 56789>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(10, body.Cursor.ColIndex); //line not split so cursor doesn't move
            Assert.Equal(0, body.Cursor.RowIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphTwoLineLongShortTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123 56789 A23";
            Assert.Equal(14, line1.Length);
            body.SetTestLine(line1);
            var line2 = "456";
            Assert.Equal(3, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal("0123 56789 A23", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("456", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(4, body.WordCount);
            Assert.Equal(3, body.Cursor.ColIndex);
            Assert.Equal(1, body.Cursor.RowIndex);

            Assert.True(body.LeftJustifyLinesInParagraph(0, 0).GetResult());

            Assert.Equal("0123", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("56789 A23", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("456", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.Equal(4, body.WordCount);
            Assert.Equal(0, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphParaBreakTwoLineLongShortTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123 56789 123";
            Assert.Equal(14, line1.Length);
            body.SetTestLine(line1);
            var line2 = "456" + Body.ParaBreak;
            Assert.Equal(4, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal("0123 56789 123", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("456>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(4, body.WordCount);
            Assert.Equal(3, body.Cursor.ColIndex);
            Assert.Equal(1, body.Cursor.RowIndex);

            Assert.True(body.LeftJustifyLinesInParagraph(0, 0).GetResult());

            Assert.Equal("0123", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("56789 123", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("456>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.Equal(4, body.WordCount);
            Assert.Equal(0, body.Cursor.ColIndex);
            Assert.Equal(0,body.Cursor.RowIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphTwoLineShortLongTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "456";
            Assert.Equal(3, line1.Length);
            body.SetTestLine(line1);
            var line2 = "0123 56789 A23";
            Assert.Equal(14, line2.Length);
            body.SetTestLine(line2);


            Assert.Equal("456", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("0123 56789 A23", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(4, body.WordCount);
            Assert.Equal(14, body.Cursor.ColIndex);
            Assert.Equal(1, body.Cursor.RowIndex);

            Assert.True(body.LeftJustifyLinesInParagraph(0, 8).GetResult()); // body.Cursor.ColIndex).GetResult());

            Assert.Equal("4560123 ", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("56789 A23", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(4, body.WordCount);
            Assert.Equal(8, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);
        }


        [Fact]
        public void LeftJustifyLinesInParagraphCursorSetTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123 56789 123";
            Assert.Equal(14, line1.Length);
            body.SetTestLine(line1);
            var line2 = "456" + Body.ParaBreak;
            Assert.Equal(4, line2.Length);
            body.SetTestLine(line2);
            Assert.True(body.SetCursorInChapter(0, 9).GetResult());

            Assert.Equal("0123 56789 123", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("456>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(4, body.WordCount);
            Assert.Equal(9, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);

            Assert.True(body.LeftJustifyLinesInParagraph(0, 9).GetResult());

            Assert.Equal("0123", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("56789 123", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("456>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.Equal(4, body.WordCount);
            Assert.Equal(9, body.Cursor.ColIndex);
            Assert.Equal(1, body.Cursor.RowIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphParaBreakShortDeleteTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "01234" + Body.ParaBreak;
            Assert.Equal(6, line1.Length);
            body.SetTestLine(line1);
            var line2 = "123" + Body.ParaBreak;
            Assert.Equal(4, line2.Length);
            body.SetTestLine(line2);
            Assert.True(body.SetCursorInChapter(0, 5).GetResult());

            Assert.Equal("01234>", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("123>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(5, body.Cursor.ColIndex);

            Assert.True(body.DeleteCharacter().GetResult()); //invokes LeftJustifyLinesInParagraph

            Assert.Equal("01234123>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(5, body.Cursor.ColIndex);

        }

    }
}
