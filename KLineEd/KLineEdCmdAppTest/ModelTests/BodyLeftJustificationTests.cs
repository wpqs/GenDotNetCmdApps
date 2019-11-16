using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ModelTests
{
    [SuppressMessage("ReSharper", "UnusedVariable")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public class BodyLeftJustificationTests
    {
        [Fact]
        public void GetSplitIndexFromEndFailParamTest()
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
        public void SplitLongLineFailParamTest()
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

            CursorPosition updatedCursor;

            var rcSplit = body.SplitLongLine(-1, originalCursor, out updatedCursor);
            Assert.True(rcSplit.IsError(true));
            Assert.Contains("error 1101602-param: rowIndex=-1; cursor=rowIndex=0; colIndex=20", rcSplit.GetErrorTechMsg());
            Assert.True(originalCursor.IsSame(updatedCursor));

            rcSplit = body.SplitLongLine(1, originalCursor, out updatedCursor);
            Assert.True(rcSplit.IsError(true));
            Assert.Contains("error 1101602-param: rowIndex=1; cursor=rowIndex=0; colIndex=20", rcSplit.GetErrorTechMsg());

            rcSplit = body.SplitLongLine(0, null, out updatedCursor);
            Assert.True(rcSplit.IsError(true));
            Assert.Contains("error 1101602-param: rowIndex=0; cursor=[not set]", rcSplit.GetErrorTechMsg());

            originalCursor.RowIndex = 1;
            rcSplit = body.SplitLongLine(0, originalCursor, out updatedCursor);
            Assert.True(rcSplit.IsError(true));
            Assert.Contains("error 1101602-param: rowIndex=0; cursor=rowIndex=1; colIndex=20", rcSplit.GetErrorTechMsg());

            originalCursor.RowIndex = 0;
            originalCursor.ColIndex = 26;  //one more than the max col for cursor

            rcSplit = body.SplitLongLine(0, originalCursor, out updatedCursor);
            Assert.False(rcSplit.IsError(true));
            Assert.Equal(0, updatedCursor.RowIndex);
            Assert.Equal(25, updatedCursor.ColIndex);
        }

        [Fact]
        public void SplitLongLineBasicTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789 123456789 1234ABCDEF";
            Assert.Equal(31, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("0123456789 123456789 1234ABCDEF", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            var originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(31, originalCursor.ColIndex);

            CursorPosition updatedCursor;

            var rcSplit = body.SplitLongLine(0, originalCursor, out updatedCursor);
            Assert.True(rcSplit.IsSuccess(true));

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("1234ABCDEF", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(10, updatedCursor.ColIndex);
        }

        [Fact]
        public void SplitLongLineBasicParaBreakTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789 123456789 1234ABCDEF" + Body.ParaBreak;
            Assert.Equal(32, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("0123456789 123456789 1234ABCDEF>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            var originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(31, originalCursor.ColIndex);

            CursorPosition updatedCursor;

            var rcSplit = body.SplitLongLine(0, originalCursor, out updatedCursor);
            Assert.True(rcSplit.IsSuccess(true));

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("1234ABCDEF>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(10, updatedCursor.ColIndex);
        }

        [Fact]
        public void SplitLongLineJustTooShortTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789 123456789 1234";
            Assert.Equal(25, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("0123456789 123456789 1234", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            var originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(25, originalCursor.ColIndex);

            CursorPosition updatedCursor;

            var rcSplit = body.SplitLongLine(0, originalCursor, out updatedCursor);
            Assert.True(rcSplit.IsSuccess(true));
            Assert.False(rcSplit.GetResult());
            Assert.Equal(0, updatedCursor.RowIndex);
            Assert.Equal(25, updatedCursor.ColIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("0123456789 123456789 1234", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void SplitLongLineJustTooShortParaBreakTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(26, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("0123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            var originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(25, originalCursor.ColIndex);

            CursorPosition updatedCursor;

            var rcSplit = body.SplitLongLine(0, originalCursor, out updatedCursor);
            Assert.True(rcSplit.IsSuccess(true));
            Assert.False(rcSplit.GetResult());
            Assert.Equal(0, updatedCursor.RowIndex);
            Assert.Equal(25, updatedCursor.ColIndex);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("0123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
        }


        [Fact]
        public void SplitLongLineJustTooLongTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789 123456789 123 5";
            Assert.Equal(26, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("0123456789 123456789 123 5", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            var originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(26, originalCursor.ColIndex);

            CursorPosition updatedCursor;

            var rcSplit = body.SplitLongLine(0, originalCursor, out updatedCursor);
            Assert.True(rcSplit.IsSuccess(true));
            Assert.True(rcSplit.GetResult());
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(1, updatedCursor.ColIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789 123", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("5", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        }

        [Fact]
        public void SplitLongLineJustTooLongParaBreakTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789 123456789 123 5" + Body.ParaBreak;
            Assert.Equal(27, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("0123456789 123456789 123 5>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            var originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(26, originalCursor.ColIndex);

            CursorPosition updatedCursor;

            var rcSplit = body.SplitLongLine(0, originalCursor, out updatedCursor);
            Assert.True(rcSplit.IsSuccess(true));
            Assert.True(rcSplit.GetResult());
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(1, updatedCursor.ColIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789 123", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("5>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        }

        [Fact]
        public void SplitLongLineNoSpaceTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789x123456789x123x567";
            Assert.Equal(28, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("0123456789x123456789x123x567", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);
            var originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(28, originalCursor.ColIndex);

            CursorPosition updatedCursor;

            var rcSplit = body.SplitLongLine(0, originalCursor, out updatedCursor);
            Assert.True(rcSplit.IsSuccess(true));
            Assert.True(rcSplit.GetResult());
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(3, updatedCursor.ColIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("0123456789x123456789x123x", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("567", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        }

        [Fact]
        public void SplitLongLineNoSpaceParaBreakTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789x123456789x123x567" + Body.ParaBreak;
            Assert.Equal(29, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("0123456789x123456789x123x567>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);
            var originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(28, originalCursor.ColIndex);

            CursorPosition updatedCursor;

            var rcSplit = body.SplitLongLine(0, originalCursor, out updatedCursor);
            Assert.True(rcSplit.IsSuccess(true));
            Assert.True(rcSplit.GetResult());
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(3, updatedCursor.ColIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("0123456789x123456789x123x", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("567>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        }

        [Fact]
        public void SplitLongLineMultipleSpaceTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789 123456789 12  5";
            Assert.Equal(26, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("0123456789 123456789 12  5", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            var originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(26, originalCursor.ColIndex);

            CursorPosition updatedCursor;

            var rcSplit = body.SplitLongLine(0, originalCursor, out updatedCursor);
            Assert.True(rcSplit.IsSuccess(true));
            Assert.True(rcSplit.GetResult());
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(1, updatedCursor.ColIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789 12 ", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("5", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        }

        [Fact]
        public void SplitLongLineMultipleSpaceParaBreakTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789 123456789 12  5" + Body.ParaBreak;
            Assert.Equal(27, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("0123456789 123456789 12  5>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            var originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(26, originalCursor.ColIndex);

            CursorPosition updatedCursor;

            var rcSplit = body.SplitLongLine(0, originalCursor, out updatedCursor);
            Assert.True(rcSplit.IsSuccess(true));
            Assert.True(rcSplit.GetResult());
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(1, updatedCursor.ColIndex);
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789 12 ", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("5>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
        }


        [Fact]
        public void GetSplitIndexFromStartFailParamTest()
        {
            Assert.Equal(-1, Body.GetSplitIndexFromStart(null, 1));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(null, 0));
        }

        [Fact]
        public void GetSplitIndexFromStartNoSpaceLineTest()
        {
            var line = "0123456789";
            Assert.Equal(10, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 9));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 10));   //allow for space if line doesn't have ParaBreak
            Assert.Equal(9, Body.GetSplitIndexFromStart(line, 11));
            Assert.Equal(9, Body.GetSplitIndexFromStart(line, 12));
        }

        [Fact]
        public void GetSplitIndexFromStartNoSpaceLineParaBreakTest()
        {
            var line = "0123456789" + Body.ParaBreak;
            Assert.Equal(11, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 9));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 10));   //allow for space if line doesn't have ParaBreak
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
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line2, 1));
            Assert.Equal(1, Body.GetSplitIndexFromStart(line2, 2));

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
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 10));

            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 11));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 19));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 20));

            Assert.Equal(20, Body.GetSplitIndexFromStart(line, 21));
            Assert.Equal(20, Body.GetSplitIndexFromStart(line, 22));

            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 59));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 60));

            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 61));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 62));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 63));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 64));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 65));

            Assert.Equal(64, Body.GetSplitIndexFromStart(line, 66));
        }

        [Fact]
        public void GetSplitIndexFromStartBasicParaBreakTest()
        {
            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(66, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 0));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 1));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 2));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 3));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 9));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 10));

            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 11));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 19));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 20));

            Assert.Equal(20, Body.GetSplitIndexFromStart(line, 21));
            Assert.Equal(20, Body.GetSplitIndexFromStart(line, 22));

            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 59));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 60));

            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 61));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 62));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 63));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 64));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 65));

            Assert.Equal(65, Body.GetSplitIndexFromStart(line, 66));

        }

        [Fact]
        public void GetSplitIndexFromStartMultiSpaceTest()
        {   //                      1         2         3         4         5         6
            //var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            var line = "0123456789 123456789 123456789 123456789 123456789 1234567   1234";
            Assert.Equal(65, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 0));

            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 51));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 52));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 57));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 58));

            Assert.Equal(58, Body.GetSplitIndexFromStart(line, 59));
            Assert.Equal(59, Body.GetSplitIndexFromStart(line, 60));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 61));

            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 62));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 63));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 64));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 65));

            Assert.Equal(64, Body.GetSplitIndexFromStart(line, 66));   //move entire line
        }

        [Fact]
        public void GetSplitIndexFromStartMultiSpaceParaBreakTest()
        {   //                      1         2         3         4         5         6
            //var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            var line = "0123456789 123456789 123456789 123456789 123456789 1234567   1234" + Body.ParaBreak;
            Assert.Equal(66, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 0));

            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 51));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 52));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 57));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 58));

            Assert.Equal(58, Body.GetSplitIndexFromStart(line, 59));
            Assert.Equal(59, Body.GetSplitIndexFromStart(line, 60));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 61));

            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 62));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 63));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 64));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 65));

            Assert.Equal(65, Body.GetSplitIndexFromStart(line, 66));   //move entire line
        }


        [Fact]
        public void UpdateCursorOnFillShortLineBasicTest()
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

            var updateCursor = body.UpdateCursorOnFillShortLine(0, 1, originalCursor, "ABCD", "E");
            Assert.Equal(1, updateCursor.RowIndex);
            Assert.Equal(1, updateCursor.ColIndex);
        }

        [Fact]
        public void UpdateCursorOnFillShortLineFailTest()
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

            var updateCursor = body.UpdateCursorOnFillShortLine(0, 1, originalCursor, "ABCD", "E");
            Assert.Equal(1, updateCursor.RowIndex);
            Assert.Equal(1, updateCursor.ColIndex);

            //tests

            Assert.Null(body.UpdateCursorOnFillShortLine(-1, 1, originalCursor, "ABCD", "E"));
            Assert.Null(body.UpdateCursorOnFillShortLine(2, 1, originalCursor, "ABCD", "E"));
            Assert.Null(body.UpdateCursorOnFillShortLine(0, -1, originalCursor, "ABCD", "E"));
            Assert.Null(body.UpdateCursorOnFillShortLine(0, 2, originalCursor, "ABCD", "E"));
            Assert.Null(body.UpdateCursorOnFillShortLine(0, 2, originalCursor, null, "E"));
            Assert.Null(body.UpdateCursorOnFillShortLine(0, 1, null, "ABCD", "E"));

            originalCursor.ColIndex = 7; //ColIndex <= moveText.Length + remainText.Length + 1
            Assert.Null(body.UpdateCursorOnFillShortLine(0, 1, originalCursor, "ABCD", "E"));

            originalCursor.ColIndex = 5; //user editing lineIndex=1 must type space after text to be moved to previous line, but in case of Body.Refresh() we don't care about cursor so just make sure it doesn't fail
            updateCursor = body.UpdateCursorOnFillShortLine(0, 1, originalCursor, "ABCD", "E");
            Assert.Equal(1, updateCursor.RowIndex);
            Assert.Equal(0, updateCursor.ColIndex);
            originalCursor.ColIndex = 4;
            updateCursor = body.UpdateCursorOnFillShortLine(0, 1, originalCursor, "ABCD", "E");
            Assert.Equal(1, updateCursor.RowIndex);
            Assert.Equal(0, updateCursor.ColIndex);

            originalCursor.ColIndex = -1;  //RowIndex isn't used, so not checked
            updateCursor = body.UpdateCursorOnFillShortLine(0, 1, originalCursor, "ABCD", "E");
            Assert.Equal(1, updateCursor.RowIndex);
            Assert.Equal(0, updateCursor.ColIndex);

        }

        [Fact]
        public void FillShortLineParamFailTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            CursorPosition updatedCursor;

            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(0, originalCursor.ColIndex);

            Assert.False(body.FillShortLine(0, originalCursor, out updatedCursor).GetResult());
            Assert.False(body.FillShortLine(-1, originalCursor, out updatedCursor).GetResult());
            Assert.False(body.FillShortLine(1, originalCursor, out updatedCursor).GetResult());

            var line1 = "0123456789 123456789";
            Assert.Equal(20, line1.Length);
            body.SetTestLine(line1);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(0, originalCursor.RowIndex);
            Assert.Equal(20, originalCursor.ColIndex);

            var rcFill = body.FillShortLine(0, null, out updatedCursor);
            Assert.True(rcFill.IsError(true));
            Assert.Contains("error 1101502-param: rowIndex=0; cursor=[not set]", rcFill.GetErrorTechMsg());

            originalCursor.RowIndex = 1;

            rcFill = body.FillShortLine(0, originalCursor, out updatedCursor);
            Assert.True(rcFill.IsError(true));
            Assert.Contains("error 1101502-param: rowIndex=0; cursor=rowIndex=1; colIndex=20", rcFill.GetErrorTechMsg());

            originalCursor.RowIndex = 0;
            originalCursor.ColIndex = 26;  //one more than the max col for cursor

            rcFill = body.FillShortLine(0, originalCursor, out updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.Equal(0, updatedCursor.RowIndex);
            Assert.Equal(25, updatedCursor.ColIndex);
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

            var rcFill = body.FillShortLine(0, originalCursor, out var updatedCursor);
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

            var rcFill = body.FillShortLine(0, originalCursor, out var updatedCursor);
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

            var rcFill = body.FillShortLine(0, originalCursor, out var updatedCursor);
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

            var rcFill = body.FillShortLine(0, originalCursor, out var updatedCursor);
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

            var rcFill = body.FillShortLine(0, originalCursor, out var updatedCursor);
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

            var rcFill = body.FillShortLine(0, originalCursor, out var updatedCursor);
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

            var rcFill = body.FillShortLine(0, originalCursor, out var updatedCursor);
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

            var rcFill = body.FillShortLine(0, originalCursor, out var updatedCursor);
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
        public void FillShortLineSingleSpaceTest()
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

            var rcFill = body.FillShortLine(0, originalCursor, out var updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.True(rcFill.GetResult());


            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789 ABCD", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("E", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(1, updatedCursor.ColIndex); 
        }

        [Fact]
        public void FillShortLineSingleSpaceParaBreakTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123456789 123456789";
            Assert.Equal(20, line1.Length);
            body.SetTestLine(line1);
            var line2 = "ABCD E" + Body.ParaBreak; 
            Assert.Equal(7, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("ABCD E>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(1, originalCursor.RowIndex);
            Assert.Equal(6, originalCursor.ColIndex);

            var rcFill = body.FillShortLine(0, originalCursor, out var updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.True(rcFill.GetResult());


            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789 ABCD", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("E>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(1, updatedCursor.ColIndex);

        }

        [Fact]
        public void FillShortLineMultiSpaceTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123456789 123456789";
            Assert.Equal(20, line1.Length);
            body.SetTestLine(line1);
            var line2 = "ABCD  E";
            Assert.Equal(7, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("ABCD  E", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(1, originalCursor.RowIndex);
            Assert.Equal(7, originalCursor.ColIndex);

            var rcFill = body.FillShortLine(0, originalCursor, out var updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.True(rcFill.GetResult());


            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789 ABCD", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal(" E", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(2, updatedCursor.ColIndex);
        }


        [Fact]
        public void FillShortLineMultiSpaceParaBreakTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "0123456789 123456789";
            Assert.Equal(20, line1.Length);
            body.SetTestLine(line1);
            var line2 = "ABCD  E" + Body.ParaBreak;
            Assert.Equal(8, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("ABCD  E>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(1, originalCursor.RowIndex);
            Assert.Equal(7, originalCursor.ColIndex);

            var rcFill = body.FillShortLine(0, originalCursor, out var updatedCursor);
            Assert.False(rcFill.IsError(true));
            Assert.True(rcFill.GetResult());


            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("0123456789 123456789 ABCD", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal(" E>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(1, updatedCursor.RowIndex);
            Assert.Equal(2, updatedCursor.ColIndex);
        }


        [Fact]
        public void LeftJustifyLinesInParagraphParamFailTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789 123456789 1234";
            Assert.Equal(25, line1.Length);
            body.SetTestLine(line1);
            var line2 = "B123456789 123456789 1234";
            Assert.Equal(25, line2.Length);
            body.SetTestLine(line2);
            var line3 = "C123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(26, line3.Length);
            body.SetTestLine(line3);

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(9, body.WordCount);
            Assert.Equal("A123456789 123456789 1234", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("B123456789 123456789 1234", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("C123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);

            CursorPosition originalCursor = new CursorPosition(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.Equal(2, originalCursor.RowIndex);
            Assert.Equal(25, originalCursor.ColIndex);

            var rcLeft = body.LeftJustifyLinesInParagraph(0, -1);

            Assert.False(rcLeft.IsSuccess(true));
            Assert.Contains("error 1101301-param: TextLines=3, maxColIndex=24 or invalid originalCursor=rowIndex=0; colIndex=-1", rcLeft.GetErrorTechMsg());
            Assert.Equal(ChapterModel.ChangeHint.Unknown, rcLeft.GetResult());

            rcLeft = body.LeftJustifyLinesInParagraph(-1, 0);

            Assert.False(rcLeft.IsSuccess(true));
            Assert.Contains("error 1101301-param: TextLines=3, maxColIndex=24 or invalid originalCursor=rowIndex=-1; colIndex=0", rcLeft.GetErrorTechMsg());
            Assert.Equal(ChapterModel.ChangeHint.Unknown, rcLeft.GetResult());


            rcLeft = body.LeftJustifyLinesInParagraph(3, 0);

            Assert.False(rcLeft.IsSuccess(true));
            Assert.Contains("error 1101301-param: TextLines=3, maxColIndex=24 or invalid originalCursor=rowIndex=3; colIndex=0", rcLeft.GetErrorTechMsg());
            Assert.Equal(ChapterModel.ChangeHint.Unknown, rcLeft.GetResult());

            rcLeft = body.LeftJustifyLinesInParagraph(0, 26);  

            Assert.False(rcLeft.IsSuccess(true));
            Assert.Contains("error 1100701-param: rowIndex=0 > max(3), colIndex=26 > max=24", rcLeft.GetErrorTechMsg());
            Assert.Equal(ChapterModel.ChangeHint.Cursor, rcLeft.GetResult()); //todo - change to unknown after next release of MxReturnCode
        }


        [Fact]
        public void LeftJustifyLinesInParagraphBasicTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789";
            Assert.Equal(10, line1.Length);
            body.SetTestLine(line1);
            var line2 = "B123456789 123456789 1234";
            Assert.Equal(25, line2.Length);
            body.SetTestLine(line2);
            var line3 = "C123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(26, line3.Length);
            body.SetTestLine(line3);

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("B123456789 123456789 1234", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("C123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);

            var rcLeft = body.LeftJustifyLinesInParagraph(0, 0);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.End, rcLeft.GetResult());

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789 B123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("123456789 1234 C123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphBasicNoParaBreakTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789";
            Assert.Equal(10, line1.Length);
            body.SetTestLine(line1);
            var line2 = "B123456789 123456789 1234";
            Assert.Equal(25, line2.Length);
            body.SetTestLine(line2);
            var line3 = "C123456789 123456789 1234";
            Assert.Equal(25, line3.Length);
            body.SetTestLine(line3);

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("B123456789 123456789 1234", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("C123456789 123456789 1234", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);

            var rcLeft = body.LeftJustifyLinesInParagraph(0, 0);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.End, rcLeft.GetResult());

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789 B123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("123456789 1234 C123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("123456789 1234", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphZeroLineTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.WordCount);
            Assert.Equal(0, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);

            var rcLeft = body.LeftJustifyLinesInParagraph(0, 0);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.Line, rcLeft.GetResult());

            Assert.Equal(0, body.WordCount);
            Assert.Equal(0, body.Cursor.ColIndex);
            Assert.Equal(0, body.Cursor.RowIndex);
        }


        [Fact]
        public void LeftJustifyLinesInParagraphOneLineNoChangeTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(26, line1.Length);
            body.SetTestLine(line1);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("A123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);

            var rcLeft = body.LeftJustifyLinesInParagraph(0, 0);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.Line, rcLeft.GetResult());

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("A123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphOneLineSplitTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789 123456789 12345" + Body.ParaBreak;
            Assert.Equal(27, line1.Length);
            body.SetTestLine(line1);

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("A123456789 123456789 12345>", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);

            var rcLeft = body.LeftJustifyLinesInParagraph(0, 0);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.End, rcLeft.GetResult());

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(3, body.WordCount);
            Assert.Equal("A123456789 123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("12345>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphOneLineFillTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789";
            Assert.Equal(10, line1.Length);
            body.SetTestLine(line1);
            var line2 = "B123456789" + Body.ParaBreak;
            Assert.Equal(11, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("A123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("B123456789>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            var rcLeft = body.LeftJustifyLinesInParagraph(0, 0);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.End, rcLeft.GetResult());

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("A123456789 B123456789>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
        }


        [Fact]
        public void LeftJustifyLinesInParagraphTwoLineNoChangeTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789 123456789 1234";
            Assert.Equal(25, line1.Length);
            body.SetTestLine(line1);
            var line2 = "B123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(26, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(6, body.WordCount);
            Assert.Equal("A123456789 123456789 1234", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("B123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
      
            var rcLeft = body.LeftJustifyLinesInParagraph(0, 0);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.Line, rcLeft.GetResult());

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(6, body.WordCount);
            Assert.Equal("A123456789 123456789 1234", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("B123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphTwoLineSplitTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789 123456789 12345";
            Assert.Equal(26, line1.Length);
            body.SetTestLine(line1);
            var line2 = "B123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(26, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(6, body.WordCount);
            Assert.Equal("A123456789 123456789 12345", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("B123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            var rcLeft = body.LeftJustifyLinesInParagraph(0, 0);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.End, rcLeft.GetResult());

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(6, body.WordCount);
            Assert.Equal("A123456789 123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("12345 B123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphTwoLineFillTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789";
            Assert.Equal(10, line1.Length);
            body.SetTestLine(line1);
            var line2 = "B123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(26, line2.Length);
            body.SetTestLine(line2);

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("A123456789", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("B123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);

            var rcLeft = body.LeftJustifyLinesInParagraph(0, 0);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.End, rcLeft.GetResult());

            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(4, body.WordCount);
            Assert.Equal("A123456789 B123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphThreeLineNoChangeTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789 123456789 1234";
            Assert.Equal(25, line1.Length);
            body.SetTestLine(line1);
            var line2 = "B123456789 123456789 1234";
            Assert.Equal(25, line2.Length);
            body.SetTestLine(line2);
            var line3 = "C123456789 123456789 1234" +  Body.ParaBreak;
            Assert.Equal(26, line3.Length);
            body.SetTestLine(line3);

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(9, body.WordCount);
            Assert.Equal("A123456789 123456789 1234", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("B123456789 123456789 1234", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("C123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);

            var rcLeft = body.LeftJustifyLinesInParagraph(0, 0);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.Line, rcLeft.GetResult());

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(9, body.WordCount);
            Assert.Equal("A123456789 123456789 1234", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("B123456789 123456789 1234", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("C123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
        }


        [Fact]
        public void LeftJustifyLinesInParagraphThreeLineFillTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789";
            Assert.Equal(10, line1.Length);
            body.SetTestLine(line1);
            var line2 = "B123456789 123456789 1234";
            Assert.Equal(25, line2.Length);
            body.SetTestLine(line2);
            var line3 = "C123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(26, line3.Length);
            body.SetTestLine(line3);

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("B123456789 123456789 1234", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("C123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);

            var rcLeft = body.LeftJustifyLinesInParagraph(0, 0);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.End, rcLeft.GetResult());

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789 B123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("123456789 1234 C123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphThreeLineSplitTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789 B23456789 C23456789";
            Assert.Equal(30, line1.Length);
            body.SetTestLine(line1);
            var line2 = "D123456789";
            Assert.Equal(10, line2.Length);
            body.SetTestLine(line2);
            var line3 = "E123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(26, line3.Length);
            body.SetTestLine(line3);

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789 B23456789 C23456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("D123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("E123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);

            var rcLeft = body.LeftJustifyLinesInParagraph(0, 0);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.End, rcLeft.GetResult());

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789 B23456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("C23456789 D123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("E123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphCursorBeforeFillTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789";
            Assert.Equal(10, line1.Length);
            body.SetTestLine(line1);
            var line2 = "B123456789 123456789 1234";
            Assert.Equal(25, line2.Length);
            body.SetTestLine(line2);
            var line3 = "C123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(26, line3.Length);
            body.SetTestLine(line3);

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("B123456789 123456789 1234", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("C123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            body.SetCursorInChapter(0, 9);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(9, body.Cursor.ColIndex);

            var rcLeft = body.LeftJustifyLinesInParagraph(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.End, rcLeft.GetResult());

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789 B123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("123456789 1234 C123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(9, body.Cursor.ColIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphCursorAfterFillTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789";
            Assert.Equal(10, line1.Length);
            body.SetTestLine(line1);
            var line2 = "B123456789 123456789 1234";
            Assert.Equal(25, line2.Length);
            body.SetTestLine(line2);
            var line3 = "C123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(26, line3.Length);
            body.SetTestLine(line3);

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("B123456789 123456789 1234", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("C123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            body.SetCursorInChapter(0, 10);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(10, body.Cursor.ColIndex);

            var rcLeft = body.LeftJustifyLinesInParagraph(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.End, rcLeft.GetResult());

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789 B123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("123456789 1234 C123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(10, body.Cursor.ColIndex);
        }


        [Fact]
        public void LeftJustifyLinesInParagraphCursorBeforeSplitTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789 B23456789 C23456789";
            Assert.Equal(30, line1.Length);
            body.SetTestLine(line1);
            var line2 = "D123456789";
            Assert.Equal(10, line2.Length);
            body.SetTestLine(line2);
            var line3 = "E123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(26, line3.Length);
            body.SetTestLine(line3);

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789 B23456789 C23456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("D123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("E123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            body.SetCursorInChapter(0, 20);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(20, body.Cursor.ColIndex);

            var rcLeft = body.LeftJustifyLinesInParagraph(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.End, rcLeft.GetResult());

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789 B23456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("C23456789 D123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("E123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(20, body.Cursor.ColIndex);
        }

        [Fact]
        public void LeftJustifyLinesInParagraphCursorAfterSplitTest()
        {
            var maxColIndex = 24;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.TextEditorDisplayRows, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line1 = "A123456789 B23456789 C23456789";
            Assert.Equal(30, line1.Length);
            body.SetTestLine(line1);
            var line2 = "D123456789";
            Assert.Equal(10, line2.Length);
            body.SetTestLine(line2);
            var line3 = "E123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(26, line3.Length);
            body.SetTestLine(line3);

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789 B23456789 C23456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("D123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("E123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            body.SetCursorInChapter(0, 21);
            Assert.Equal(0, body.Cursor.RowIndex);
            Assert.Equal(21, body.Cursor.ColIndex);

            var rcLeft = body.LeftJustifyLinesInParagraph(body.Cursor.RowIndex, body.Cursor.ColIndex);
            Assert.True(rcLeft.IsSuccess(true));
            Assert.Equal(ChapterModel.ChangeHint.End, rcLeft.GetResult());

            Assert.Equal(3, body.GetLineCount());
            Assert.Equal(7, body.WordCount);
            Assert.Equal("A123456789 B23456789", body.GetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("C23456789 D123456789", body.GetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("E123456789 123456789 1234>", body.GetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.Equal(1, body.Cursor.RowIndex);
            Assert.Equal(0, body.Cursor.ColIndex);
        }
    }
}
