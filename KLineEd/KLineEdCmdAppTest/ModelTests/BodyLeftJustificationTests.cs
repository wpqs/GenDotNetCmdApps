using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.ComTypes;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ModelTests
{
    [SuppressMessage("ReSharper", "UnusedVariable")]
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
            var line = "0123456789 123456789 123456789 123456789 123456789 1234567   1234";
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
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            Assert.False(body.SplitLongLine(0, 0, -1, out var updatedCursorIndex).GetResult());
            Assert.False(body.SplitLongLine(0, -1, CmdLineParamsApp.ArgTextEditorDisplayColsMin - 1, out updatedCursorIndex).GetResult());
            Assert.False(body.SplitLongLine(-1, 0, CmdLineParamsApp.ArgTextEditorDisplayColsMin - 1, out updatedCursorIndex).GetResult());
            Assert.False(body.SplitLongLine(1, 0, CmdLineParamsApp.ArgTextEditorDisplayColsMin - 1, out updatedCursorIndex).GetResult());

            Assert.Equal(-1, updatedCursorIndex);
        }

        [Fact]
        public void SplitLongLineBasicTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
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
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
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
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
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
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
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
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
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
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
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
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
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
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
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
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
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
            Assert.Equal(maxColIndex + 1, updatedCursorIndex); //maxCursorColIndex == maxColIndex+1
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
            Assert.Equal(maxColIndex + 2, updatedCursorIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }

        [Fact]
        public void SplitLongLineMultiSpaceLineTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
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
        public void FillShortLineBasicOneLineTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());
 
            var line1 = "0123456789 123456789";
            Assert.Equal(20, line1.Length);
            body.SetTestLine(line1);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            var originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(20, originalCursor.ColIndex);

            var rcFill = body.FillShortLine2(0, originalCursor, out var updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.False(rcFill.GetResult());

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(0, updatedCursor.RowIndex);
            Assert.Equal(20, updatedCursor.ColIndex);
        }

        [Fact]
        public void FillShortLineBasicTwoLineTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123456789 123456789 1";
            Assert.Equal(22, line1.Length);
            body.SetTestLine(line1);
            var line2 = "AB";
            Assert.Equal(2, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789 1", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("AB", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(1, originalCursor.RowIndex);
            Assert.Equal(2, originalCursor.ColIndex);

            var rcFill = body.FillShortLine2(0, originalCursor, out var updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.True(rcFill.GetResult());


            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789 1 AB", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(0, updatedCursor.RowIndex);
            Assert.Equal(25, updatedCursor.ColIndex);
        }

        [Fact]
        public void FillShortLineBasicTwoLineParaBreakTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123456789 123456789";
            Assert.Equal(20, line1.Length);
            body.SetTestLine(line1);
            var line2 = "ABCD" + Body.ParaBreak;
            Assert.Equal(5, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("ABCD>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(line2.Length - 1, body.Cursor.ColIndex);
            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(1, originalCursor.RowIndex);
            Assert.Equal(4, originalCursor.ColIndex);

            var rcFill = body.FillShortLine2(0, originalCursor, out var updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.True(rcFill.GetResult());

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("0123456789 123456789 ABCD>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(0, updatedCursor.RowIndex);
            Assert.Equal(25, updatedCursor.ColIndex);
        }

        [Fact]
        public void FillShortLineThreeWordTwoLineParaBreakTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123456789 123456789 1";
            Assert.Equal(22, line1.Length);
            body.SetTestLine(line1);
            var line2 = "AB" + Body.ParaBreak;
            Assert.Equal(3, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789 1", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("AB>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(line2.Length - 1, body.Cursor.ColIndex);
            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(1, originalCursor.RowIndex);
            Assert.Equal(2, originalCursor.ColIndex);

            var rcFill = body.FillShortLine2(0, originalCursor, out var updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.True(rcFill.GetResult());
            
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789 1 AB>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(0, updatedCursor.RowIndex);
            Assert.Equal(25, updatedCursor.ColIndex);
        }

        [Fact]
        public void FillShortLineLongestTwoLineTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123456789 123456789";
            Assert.Equal(20, line1.Length);
            body.SetTestLine(line1);
            var line2 = "ABCD";
            Assert.Equal(4, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("ABCD", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(1, originalCursor.RowIndex);
            Assert.Equal(4, originalCursor.ColIndex);

            var rcFill = body.FillShortLine2(0, originalCursor, out var updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.True(rcFill.GetResult());

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal("0123456789 123456789 ABCD", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(3, body.WordCount);
            Assert.Equal(0, updatedCursor.RowIndex);
            Assert.Equal(25, updatedCursor.ColIndex);
        }

        [Fact]
        public void FillShortLineLongestTwoLineParaBreakTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123456789 123456789";
            Assert.Equal(20, line1.Length);
            body.SetTestLine(line1);
            var line2 = "ABCD" + Body.ParaBreak;
            Assert.Equal(5, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("ABCD>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(1, originalCursor.RowIndex);
            Assert.Equal(4, originalCursor.ColIndex);

            var rcFill = body.FillShortLine2(0, originalCursor, out var updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.True(rcFill.GetResult());

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal("0123456789 123456789 ABCD>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(3, body.WordCount);
            Assert.Equal(0, updatedCursor.RowIndex);
            Assert.Equal(25, updatedCursor.ColIndex);
        }

        [Fact]
        public void FillShortLineTooLongTwoLineTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123456789 123456789";
            Assert.Equal(20, line1.Length);
            body.SetTestLine(line1);
            var line2 = "ABCDE";
            Assert.Equal(5, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("ABCDE", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(1, originalCursor.RowIndex);
            Assert.Equal(5, originalCursor.ColIndex);

            var rcFill = body.FillShortLine2(0, originalCursor, out var updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.False(rcFill.GetResult());

 
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("ABCDE", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(5, updatedCursor.ColIndex);
        }


        [Fact]
        public void FillShortLineTooLongTwoLineParaBreakTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123456789 123456789";
            Assert.Equal(20, line1.Length);
            body.SetTestLine(line1);
            var line2 = "ABCDE" + Body.ParaBreak;
            Assert.Equal(6, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("ABCDE>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(1, originalCursor.RowIndex);
            Assert.Equal(5, originalCursor.ColIndex);

            var rcFill = body.FillShortLine2(0, originalCursor, out var updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.False(rcFill.GetResult());

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("ABCDE>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(5, updatedCursor.ColIndex);
        }

        [Fact]
        public void FillShortLineSpaceLineTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123456789 123456789";
            Assert.Equal(20, line1.Length);
            body.SetTestLine(line1);
            var line2 = "ABCD E";
            Assert.Equal(6, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("ABCD E", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(1, originalCursor.RowIndex);
            Assert.Equal(6, originalCursor.ColIndex);

            var rcFill = body.FillShortLine2(0, originalCursor, out var updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.True(rcFill.GetResult());


            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789 ABCD", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("E", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(0, updatedCursor.ColIndex);  //should be 1
        }

        //[Fact]
        //public void FillShortParaBreakLineSpaceLineTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "012345";
        //    Assert.Equal(6, line1.Length);
        //    body.SetTestLine(line1);
        //    var line2 = "789 A" + Body.ParaBreak;
        //    Assert.Equal(6, line2.Length);
        //    body.SetTestLine(line2);

        //    Assert.Equal("012345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("789 A>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(3, body.WordCount);
        //    Assert.Equal(5, body.Cursor.ColIndex);

        //    Assert.True(body.FillShortLine(0, 0, maxColIndex, out var updatedCursorColIndex).GetResult());

        //    Assert.Equal(0, updatedCursorColIndex);
        //    Assert.Equal("012345 789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("A>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(2, body.GetLineCount());
        //    Assert.Equal(3, body.WordCount);
        //}

        //[Fact]
        //public void FillShortLineMultiSpaceLineTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "012345";
        //    Assert.Equal(6, line1.Length);
        //    body.SetTestLine(line1);
        //    var line2 = "78  A";
        //    Assert.Equal(5, line2.Length);
        //    body.SetTestLine(line2);

        //    Assert.Equal("012345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("78  A", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(3, body.WordCount);
        //    Assert.Equal(5, body.Cursor.ColIndex);

        //    Assert.True(body.FillShortLine(0, 0, maxColIndex, out var updatedCursorColIndex).GetResult());

        //    Assert.Equal(0, updatedCursorColIndex);
        //    Assert.Equal("012345 78 ", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("A", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(2, body.GetLineCount());
        //    Assert.Equal(3, body.WordCount);
        //}


        //[Fact]
        //public void FillShortParaBreakLineMultiSpaceLineTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "012345";
        //    Assert.Equal(6, line1.Length);
        //    body.SetTestLine(line1);
        //    var line2 = "78  A" + Body.ParaBreak;
        //    Assert.Equal(6, line2.Length);
        //    body.SetTestLine(line2);

        //    Assert.Equal("012345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("78  A>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(3, body.WordCount);
        //    Assert.Equal(5, body.Cursor.ColIndex);

        //    Assert.True(body.FillShortLine(0, 0, maxColIndex, out var updatedCursorColIndex).GetResult());

        //    Assert.Equal(0, updatedCursorColIndex);
        //    Assert.Equal("012345 78 ", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("A>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(2, body.GetLineCount());
        //    Assert.Equal(3, body.WordCount);
        //}


        [Fact]
        public void FillShortLineParamFailTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(0, originalCursor.ColIndex);
            CursorPosition updatedCursor;
            Assert.False(body.FillShortLine2(0, originalCursor, out updatedCursor).GetResult());
            Assert.False(body.FillShortLine2(-1, originalCursor, out updatedCursor).GetResult());
            Assert.False(body.FillShortLine2(1, originalCursor, out updatedCursor).GetResult());

            var line1 = "0123456789 123456789";
            Assert.Equal(20, line1.Length);
            body.SetTestLine(line1);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(20, originalCursor.ColIndex);

            var rcFill = body.FillShortLine2(0, null, out updatedCursor);
            Assert.True(rcFill.IsError(true));
            Assert.Contains("error 1101502-param: rowIndex=0; cursor=[not set]", rcFill.GetErrorTechMsg());

            originalCursor.RowIndex = 1;

            rcFill = body.FillShortLine2(0, originalCursor, out updatedCursor);
            Assert.True(rcFill.IsError(true));
            Assert.Contains("error 1101502-param: rowIndex=0; cursor=rowIndex=1; colIndex=20", rcFill.GetErrorTechMsg());

            originalCursor.RowIndex = 0;
            originalCursor.ColIndex = 26;  //one more than the max col for cursor

            rcFill = body.FillShortLine2(0, originalCursor, out updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.Equal(0, updatedCursor.RowIndex);
            Assert.Equal(25, updatedCursor.ColIndex);
        }

        //[Fact]
        //public void LeftJustifyLinesInParagraphParamFailTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    Assert.Equal(ChapterModel.ChangeHint.Unknown, body.LeftJustifyLinesInParagraph(0, -1).GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Unknown, body.LeftJustifyLinesInParagraph(-1, 0).GetResult());
        //}

        //[Fact]
        //public void LeftJustifyLinesInParagraphParaBreakShortLineTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "0123";
        //    Assert.Equal(4, line1.Length);
        //    body.SetTestLine(line1);
        //    var line2 = "456" + Body.ParaBreak;
        //    Assert.Equal(4, line2.Length);
        //    body.SetTestLine(line2);

        //    Assert.Equal("0123", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("456>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(2, body.WordCount);
        //    Assert.Equal(3, body.Cursor.ColIndex);
        //    Assert.Equal(1, body.Cursor.RowIndex);

        //    Assert.Equal(ChapterModel.ChangeHint.End, body.LeftJustifyLinesInParagraph(0, 0).GetResult());

        //    Assert.Equal("0123 456>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        //    Assert.Equal(2, body.WordCount);
        //    Assert.Equal(0, body.Cursor.ColIndex);
        //    Assert.Equal(0, body.Cursor.RowIndex);
        //}

        //[Fact]
        //public void LeftJustifyLinesInParagraphNoLineTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    Assert.Equal(0, body.WordCount);
        //    Assert.Equal(0, body.Cursor.ColIndex);
        //    Assert.Equal(0, body.Cursor.RowIndex);

        //    var rc = body.LeftJustifyLinesInParagraph(0, 0);

        //    Assert.False(rc.IsError(true));
        //    Assert.Equal(0, body.Cursor.ColIndex);
        //    Assert.Equal(0, body.Cursor.RowIndex);
        //}

        //[Fact]
        //public void LeftJustifyLinesInParagraphOneLineLongTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "01 3456 89A12";
        //    Assert.Equal(13, line1.Length);
        //    body.SetTestLine(line1);

        //    Assert.Equal("01 3456 89A12", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        //    Assert.Equal(3, body.WordCount);
        //    Assert.Equal(13, body.Cursor.ColIndex);
        //    Assert.Equal(0, body.Cursor.RowIndex);

        //    Assert.Equal(ChapterModel.ChangeHint.End, body.LeftJustifyLinesInParagraph(0, 13).GetResult());

        //    Assert.Equal("01 3456", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("89A12", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(3, body.WordCount);
        //    Assert.Equal(5, body.Cursor.ColIndex);
        //    Assert.Equal(1, body.Cursor.RowIndex);
        //}

        //[Fact]
        //public void LeftJustifyLinesInParagraphParaBreakOneLineLongTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "01 3456 89A12" + Body.ParaBreak;
        //    Assert.Equal(14, line1.Length);
        //    body.SetTestLine(line1);

        //    Assert.Equal("01 3456 89A12>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        //    Assert.Equal(3, body.WordCount);
        //    Assert.Equal(13, body.Cursor.ColIndex);
        //    Assert.Equal(0, body.Cursor.RowIndex);

        //    Assert.Equal(ChapterModel.ChangeHint.End, body.LeftJustifyLinesInParagraph(0, 13).GetResult());

        //    Assert.Equal("01 3456", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("89A12>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(3, body.WordCount);
        //    Assert.Equal(5, body.Cursor.ColIndex); //long line split, so cursor goes from end of row 0 to end of row 1
        //    Assert.Equal(1, body.Cursor.RowIndex);
        //}

        //[Fact]
        //public void LeftJustifyLinesInParagraphOneLineShortTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "0123 56789";
        //    Assert.Equal(10, line1.Length);
        //    body.SetTestLine(line1);

        //    Assert.Equal("0123 56789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        //    Assert.Equal(2, body.WordCount);
        //    Assert.Equal(10, body.Cursor.ColIndex);
        //    Assert.Equal(0, body.Cursor.RowIndex);

        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.LeftJustifyLinesInParagraph(0, 10).GetResult());

        //    Assert.Equal("0123 56789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        //    Assert.Equal(2, body.WordCount);
        //    Assert.Equal(10, body.Cursor.ColIndex); //line not split so cursor doesn't move
        //    Assert.Equal(0, body.Cursor.RowIndex);
        //}

        //[Fact]
        //public void LeftJustifyLinesInParagraphParamBreakOneLineShortTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "0123 56789" + Body.ParaBreak;
        //    Assert.Equal(11, line1.Length);
        //    body.SetTestLine(line1);

        //    Assert.Equal("0123 56789>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        //    Assert.Equal(2, body.WordCount);
        //    Assert.Equal(10, body.Cursor.ColIndex);
        //    Assert.Equal(0, body.Cursor.RowIndex);

        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.LeftJustifyLinesInParagraph(0, 10).GetResult());

        //    Assert.Equal("0123 56789>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        //    Assert.Equal(2, body.WordCount);
        //    Assert.Equal(10, body.Cursor.ColIndex); //line not split so cursor doesn't move
        //    Assert.Equal(0, body.Cursor.RowIndex);
        //}

        //[Fact]
        //public void LeftJustifyLinesInParagraphTwoLineLongShortTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "0123 56789 A23";
        //    Assert.Equal(14, line1.Length);
        //    body.SetTestLine(line1);
        //    var line2 = "456";
        //    Assert.Equal(3, line2.Length);
        //    body.SetTestLine(line2);

        //    Assert.Equal("0123 56789 A23", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("456", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(4, body.WordCount);
        //    Assert.Equal(3, body.Cursor.ColIndex);
        //    Assert.Equal(1, body.Cursor.RowIndex);

        //    Assert.Equal(ChapterModel.ChangeHint.End, body.LeftJustifyLinesInParagraph(0, 0).GetResult());

        //    Assert.Equal("0123", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
        //    Assert.Equal("56789 A23", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
        //    Assert.Equal("456", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
        //    Assert.Equal(4, body.WordCount);
        //    Assert.Equal(0, body.Cursor.ColIndex);
        //    Assert.Equal(0, body.Cursor.RowIndex);
        //}

        //[Fact]
        //public void LeftJustifyLinesInParagraphParaBreakTwoLineLongShortTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "0123 56789 123";
        //    Assert.Equal(14, line1.Length);
        //    body.SetTestLine(line1);
        //    var line2 = "456" + Body.ParaBreak;
        //    Assert.Equal(4, line2.Length);
        //    body.SetTestLine(line2);

        //    Assert.Equal("0123 56789 123", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("456>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(4, body.WordCount);
        //    Assert.Equal(3, body.Cursor.ColIndex);
        //    Assert.Equal(1, body.Cursor.RowIndex);

        //    Assert.Equal(ChapterModel.ChangeHint.End, body.LeftJustifyLinesInParagraph(0, 0).GetResult());

        //    Assert.Equal("0123", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
        //    Assert.Equal("56789 123", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
        //    Assert.Equal("456>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
        //    Assert.Equal(4, body.WordCount);
        //    Assert.Equal(0, body.Cursor.ColIndex);
        //    Assert.Equal(0, body.Cursor.RowIndex);
        //}

        //[Fact]
        //public void LeftJustifyLinesInParagraphTwoLineShortLongTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "456";
        //    Assert.Equal(3, line1.Length);
        //    body.SetTestLine(line1);
        //    var line2 = "0123 56789 A23";
        //    Assert.Equal(14, line2.Length);
        //    body.SetTestLine(line2);


        //    Assert.Equal("456", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("0123 56789 A23", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(4, body.WordCount);
        //    Assert.Equal(14, body.Cursor.ColIndex);
        //    Assert.Equal(1, body.Cursor.RowIndex);

        //    Assert.Equal(ChapterModel.ChangeHint.End, body.LeftJustifyLinesInParagraph(0, 8).GetResult()); // body.Cursor.ColIndex).GetResult());

        //    Assert.Equal("456 0123", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("56789 A23", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(4, body.WordCount);
        //    Assert.Equal(8, body.Cursor.ColIndex);
        //    Assert.Equal(0, body.Cursor.RowIndex);
        //}


        //[Fact]
        //public void LeftJustifyLinesInParagraphCursorSetTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "0123 56789 123";
        //    Assert.Equal(14, line1.Length);
        //    body.SetTestLine(line1);
        //    var line2 = "456" + Body.ParaBreak;
        //    Assert.Equal(4, line2.Length);
        //    body.SetTestLine(line2);
        //    Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 9).GetResult());

        //    Assert.Equal("0123 56789 123", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("456>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(4, body.WordCount);
        //    Assert.Equal(9, body.Cursor.ColIndex);
        //    Assert.Equal(0, body.Cursor.RowIndex);

        //    Assert.Equal(ChapterModel.ChangeHint.End, body.LeftJustifyLinesInParagraph(0, 9).GetResult());

        //    Assert.Equal("0123", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
        //    Assert.Equal("56789 123", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
        //    Assert.Equal("456>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
        //    Assert.Equal(4, body.WordCount);
        //    Assert.Equal(9, body.Cursor.ColIndex);
        //    Assert.Equal(1, body.Cursor.RowIndex);
        //}

        //[Fact]
        //public void LeftJustifyLinesInParagraphParaBreakShortDeleteTest()
        //{
        //    var maxColIndex = 9;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "01234" + Body.ParaBreak;
        //    Assert.Equal(6, line1.Length);
        //    body.SetTestLine(line1);
        //    var line2 = "123" + Body.ParaBreak;
        //    Assert.Equal(4, line2.Length);
        //    body.SetTestLine(line2);
        //    Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 5).GetResult());

        //    Assert.Equal("01234>", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
        //    Assert.Equal("123>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        //    Assert.Equal(2, body.WordCount);
        //    Assert.Equal(0, body.Cursor.RowIndex);
        //    Assert.Equal(5, body.Cursor.ColIndex);

        //    Assert.Equal(ChapterModel.ChangeHint.End, body.DeleteCharacter().GetResult()); //invokes LeftJustifyLinesInParagraph

        //    Assert.Equal("01234 123>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        //    Assert.Equal(2, body.WordCount);
        //    Assert.Equal(0, body.Cursor.RowIndex);
        //    Assert.Equal(5, body.Cursor.ColIndex);

        //}


        //[Fact]
        //public void LeftJustifyLinesInParagraphThreeLineDeleteTest()
        //{
        //    var maxColIndex = 67;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "A123456789 123456789 123456789 123456789 123456789 123456789";
        //    Assert.Equal(60, line1.Length);
        //    body.SetTestLine(line1);
        //    var line2 = "B123456789 123456789 123456789 123456789 123456789 123456789";
        //    Assert.Equal(60, line2.Length);
        //    body.SetTestLine(line2);
        //    var line3 = "C123456789 123456789 123456789 123456789 123456789 123456789";
        //    Assert.Equal(60, line3.Length);
        //    body.SetTestLine(line3);

        //    Assert.Equal("A123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
        //    Assert.Equal("B123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
        //    Assert.Equal("C123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);

        //    Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 11).GetResult());

        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.LeftJustifyLinesInParagraph(0, 11).GetResult());
        //    Assert.Equal(11, body.Cursor.ColIndex);
        //    Assert.Equal(0, body.Cursor.RowIndex);
        //    Assert.Equal(3, body.GetLineCount());

        //    Assert.Equal(ChapterModel.ChangeHint.End, body.InsertParaBreak().GetResult());
        //    Assert.Equal(0, body.Cursor.ColIndex);
        //    Assert.Equal(1, body.Cursor.RowIndex);
        //    Assert.Equal(4, body.GetLineCount());

        //    Assert.Equal("A123456789 >", body.GetEditAreaLinesForDisplay(4).GetResult()[0]);
        //    Assert.Equal("123456789 123456789 123456789 123456789 123456789 B123456789", body.GetEditAreaLinesForDisplay(4).GetResult()[1]);
        //    Assert.Equal("123456789 123456789 123456789 123456789 123456789 C123456789", body.GetEditAreaLinesForDisplay(4).GetResult()[2]);
        //    Assert.Equal("123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(4).GetResult()[3]);

        //    Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 11).GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.End, body.DeleteCharacter().GetResult());
        //    Assert.Equal(11, body.Cursor.ColIndex);
        //    Assert.Equal(0, body.Cursor.RowIndex);
        //    Assert.Equal(3, body.GetLineCount());

        //    Assert.Equal("A123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
        //    Assert.Equal("B123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
        //    Assert.Equal("C123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
        //}

        //[Fact]
        //public void LeftJustifyLinesInParagraphThreeLineAppendEndTest()
        //{
        //    var maxColIndex = 67;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "A123456789 123456789 123456789 123456789 123456789 123456789";
        //    Assert.Equal(60, line1.Length);
        //    body.SetTestLine(line1);
        //    var line2 = "B123456789 123456789 123456789 123456789 123456789 123456789";
        //    Assert.Equal(60, line2.Length);
        //    body.SetTestLine(line2);
        //    var line3 = "C123456789 123456789 123456789 123456789 123456789 123456789";
        //    Assert.Equal(60, line3.Length);
        //    body.SetTestLine(line3);

        //    Assert.Equal("A123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
        //    Assert.Equal("B123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
        //    Assert.Equal("C123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);

        //    Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 60).GetResult());

        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText(" ").GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("a").GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("b").GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("c").GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("d").GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("e").GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("f").GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("g").GetResult());

        //    Assert.Equal("A123456789 123456789 123456789 123456789 123456789 123456789 abcdefg", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
        //    Assert.Equal("B123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
        //    Assert.Equal("C123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);

        //    Assert.Equal(0, body.Cursor.RowIndex);
        //    Assert.Equal(68, body.Cursor.ColIndex);
        //    Assert.Equal(3, body.GetLineCount());

        //    Assert.Equal(ChapterModel.ChangeHint.End, body.InsertText("h").GetResult());
        //    Assert.Equal(1, body.Cursor.RowIndex);
        //    Assert.Equal(8, body.Cursor.ColIndex);
        //    Assert.Equal(4, body.GetLineCount());

        //    Assert.Equal("A123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(4).GetResult()[0]);
        //    Assert.Equal("abcdefgh B123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(4).GetResult()[1]);
        //    Assert.Equal("123456789 C123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(4).GetResult()[2]);
        //    Assert.Equal("123456789", body.GetEditAreaLinesForDisplay(4).GetResult()[3]);
        //}


        //[Fact]
        //public void LeftJustifyLinesInParagraphThreeLineInsertMiddleTest()
        //{
        //    var maxColIndex = 67;
        //    var body = new MockModelBody();
        //    Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
        //    Assert.False(body.IsError());

        //    var line1 = "A123456789 123456789 123456789 123456789 123456789 123456789";
        //    Assert.Equal(60, line1.Length);
        //    body.SetTestLine(line1);
        //    var line2 = "B123456789 123456789 123456789 123456789 123456789 123456789";
        //    Assert.Equal(60, line2.Length);
        //    body.SetTestLine(line2);
        //    var line3 = "C123456789 123456789 123456789 123456789 123456789 123456789";
        //    Assert.Equal(60, line3.Length);
        //    body.SetTestLine(line3);

        //    Assert.Equal("A123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
        //    Assert.Equal("B123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
        //    Assert.Equal("C123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);

        //    Assert.Equal(ChapterModel.ChangeHint.Cursor, body.SetCursorInChapter(0, 11).GetResult());

        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("a", true).GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("b", true).GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("c", true).GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("d", true).GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("e", true).GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("f", true).GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("g", true).GetResult());
        //    Assert.Equal(ChapterModel.ChangeHint.Line, body.InsertText("h", true).GetResult());

        //    Assert.Equal("A123456789 abcdefgh123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
        //    Assert.Equal("B123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
        //    Assert.Equal("C123456789 123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);

        //    Assert.Equal(0, body.Cursor.RowIndex);
        //    Assert.Equal(19, body.Cursor.ColIndex);
        //    Assert.Equal(3, body.GetLineCount());

        //    Assert.Equal(ChapterModel.ChangeHint.End, body.InsertText("i", true).GetResult());
        //    Assert.Equal(0, body.Cursor.RowIndex);
        //    Assert.Equal(20, body.Cursor.ColIndex);
        //    Assert.Equal(4, body.GetLineCount());

        //    Assert.Equal("A123456789 abcdefghi123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(4).GetResult()[0]);
        //    Assert.Equal("123456789 B123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(4).GetResult()[1]);
        //    Assert.Equal("123456789 C123456789 123456789 123456789 123456789 123456789", body.GetEditAreaLinesForDisplay(4).GetResult()[2]);
        //    Assert.Equal("123456789", body.GetEditAreaLinesForDisplay(4).GetResult()[3]);
        //}

    }
}
