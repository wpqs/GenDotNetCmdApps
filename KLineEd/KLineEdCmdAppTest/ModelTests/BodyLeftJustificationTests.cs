﻿using System;
using System.Collections.Generic;
using System.Text;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using KLineEdCmdAppTest.TestSupport;
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
            Assert.Equal(9, Body.GetSplitIndexFromStart(line, 10));
            Assert.Equal(9, Body.GetSplitIndexFromStart(line, 11));
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

            Assert.Equal(64, Body.GetSplitIndexFromStart(line, 65));
            Assert.Equal(64, Body.GetSplitIndexFromStart(line, 66));
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
        { //6         5         4         3         2         1
            var line = "0123456789 123456789 123456789 123456789 123456789 123456789";
            //0         1         2         3         4         5       
            Assert.Equal(60, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 0));
            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 1));
            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 2));
            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 10));

            Assert.Equal(40, Body.GetSplitIndexFromEnd(line, 11));
            Assert.Equal(40, Body.GetSplitIndexFromEnd(line, 12));
            Assert.Equal(40, Body.GetSplitIndexFromEnd(line, 20));

            Assert.Equal(30, Body.GetSplitIndexFromEnd(line, 21));
            Assert.Equal(30, Body.GetSplitIndexFromEnd(line, 22));
            Assert.Equal(30, Body.GetSplitIndexFromEnd(line, 30));

            Assert.Equal(20, Body.GetSplitIndexFromEnd(line, 31));
            Assert.Equal(20, Body.GetSplitIndexFromEnd(line, 32));

            Assert.Equal(10, Body.GetSplitIndexFromEnd(line, 49));
            Assert.Equal(10, Body.GetSplitIndexFromEnd(line, 50));
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 51));
        }

        [Fact]
        public void GetSplitIndexFromEndLimitTest()
        {
            var line = "0123456789x123456789x123456789x123456789x123456789x123456789x1234";
            Assert.Equal(65, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 1));

            var line1 = " 123456789x123456789x123456789x123456789x123456789x123456789x1234";
            Assert.Equal(65, line1.Length);
            Assert.Equal(0, Body.GetSplitIndexFromEnd(line1, 1));

            var line2 = "0123456789x123456789x123456789x123456789x123456789x123456789x123 ";
            Assert.Equal(65, line2.Length);
            Assert.Equal(64, Body.GetSplitIndexFromEnd(line2, 1));
        }

        [Fact]
        public void GetSplitIndexFromEndMultiSpaceTest()
        {
            //"0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            var line = "0123456789 123456789 123456789 123456789 123456789 1234567   1234";
            Assert.Equal(65, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 0));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 1));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 2));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 3));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 4));
            Assert.Equal(60, Body.GetSplitIndexFromEnd(line, 5));

            Assert.Equal(59, Body.GetSplitIndexFromEnd(line, 6));
            Assert.Equal(58, Body.GetSplitIndexFromEnd(line, 7));
            Assert.Equal(50, Body.GetSplitIndexFromEnd(line, 8));

            Assert.Equal(10, Body.GetSplitIndexFromEnd(line, 54));
            Assert.Equal(10, Body.GetSplitIndexFromEnd(line, 55));
            Assert.Equal(-1, Body.GetSplitIndexFromEnd(line, 56));
        }

        [Fact]
        public void SplitLongLineParamFailTest()
        {
            var maxColIndex = 9;
            var body = new Body();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex+1).GetResult());
            Assert.False(body.IsError());

            Assert.False(body.SplitLongLine(0, -1, out var updatedCursorIndex).GetResult());
            Assert.False(body.SplitLongLine(0, CmdLineParamsApp.ArgEditAreaLineWidthMin-1, out updatedCursorIndex).GetResult());
            Assert.False(body.SplitLongLine(1, -1, out updatedCursorIndex).GetResult());
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

            Assert.True(body.SplitLongLine(0, maxColIndex, out var updatedCursorIndex).GetResult());

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

            Assert.True(body.SplitLongLine(0, maxColIndex, out var updatedCursorIndex).GetResult());

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

            Assert.False(body.SplitLongLine(0, maxColIndex, out var updatedCursorIndex).GetResult());
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

            Assert.False(body.SplitLongLine(0, maxColIndex, out var updatedCursorIndex).GetResult());
        }

        [Fact]
        public void SplitLongLineSmallestLineNoSpaceTest()
        {
            var maxColIndex = 9;
            var body = new MockModelBody();
            Assert.True(body.Initialise(TestConst.UnitTestEditAreaLines, maxColIndex + 1).GetResult());
            Assert.False(body.IsError());

            var line = "0123456789";
            Assert.Equal(10, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("0123456789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(1, body.WordCount);
            Assert.Equal(line.Length, body.Cursor.ColIndex);

            Assert.True(body.SplitLongLine(0, maxColIndex, out var updatedCursorIndex).GetResult());

            Assert.Equal("012345678", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("9", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
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

            var line = "0123456789";
            Assert.Equal(10, line.Length);
            body.SetTestLine(line + Body.ParaBreak); //use test method to add line > maxColIndex

            Assert.Equal("0123456789>", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(1, body.WordCount);
            Assert.Equal(line.Length, body.Cursor.ColIndex);

            Assert.True(body.SplitLongLine(0, maxColIndex, out var updatedCursorIndex).GetResult());

            Assert.Equal("012345678", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("9>", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
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

            var line = "01234 6789";
            Assert.Equal(10, line.Length);
            body.SetTestLine(line); //use test method to add line > maxColIndex

            Assert.Equal("01234 6789", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(2, body.WordCount);
            Assert.Equal(line.Length, body.Cursor.ColIndex);

            Assert.True(body.SplitLongLine(0, maxColIndex, out var updatedCursorIndex).GetResult());

            Assert.Equal("01234", body.GetEditAreaLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("6789", body.GetEditAreaLinesForDisplay(2).GetResult()[1]);
            Assert.Equal(4, updatedCursorIndex);
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

            Assert.True(body.SplitLongLine(0, maxColIndex, out var updatedCursorIndex).GetResult());

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

            Assert.True(body.SplitLongLine(0, maxColIndex, out var updatedCursorIndex).GetResult());

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

            Assert.True(body.SplitLongLine(0, maxColIndex, out updatedCursorIndex).GetResult());

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

            Assert.True(body.SplitLongLine(0, maxColIndex, out var updatedCursorIndex).GetResult());

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

            Assert.False(body.FillShortLine(0, -1, out var removeLine).GetResult());
            Assert.False(removeLine);
            Assert.False(body.FillShortLine(0, CmdLineParamsApp.ArgEditAreaLineWidthMin - 1, out removeLine).GetResult());
            Assert.False(removeLine);
            Assert.False(body.FillShortLine(1, -1, out removeLine).GetResult());
            Assert.False(removeLine);
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

            Assert.True(body.FillShortLine(0, maxColIndex, out var removeLine).GetResult());
            Assert.True(removeLine);
            Assert.Equal("0123 456", body.GetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
        }
    }
}
