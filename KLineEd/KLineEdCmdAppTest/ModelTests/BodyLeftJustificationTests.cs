using System;
using System.Collections.Generic;
using System.Text;
using KLineEdCmdApp.Model;
using Xunit;

namespace KLineEdCmdAppTest.ModelTests
{
    public class BodyLeftJustificationTests
    {
   [Fact]
        public void GetSplitIndexFromStartLimitTest()
        {
            var line = "0123456789x123456789x123456789x123456789x123456789x123456789x1234";
            Assert.Equal(65, line.Length);
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 1));

            var line1 = "0123456789x123456789x123456789x123456789x123456789x123456789x123 ";
            Assert.Equal(65, line1.Length);
            Assert.Equal(64, Body.GetSplitIndexFromStart(line1, 1));

            var line2 = " 123456789x123456789x123456789x123456789x123456789x123456789x1234";
            Assert.Equal(65, line2.Length);
            Assert.Equal(0, Body.GetSplitIndexFromStart(line2, 1));
        }

        [Fact]
        public void GetSplitIndexFromStartNoParaBreakLineTest()
        {
            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            Assert.Equal(65, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 0));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 1));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 2));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 3));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 10));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 11));

            Assert.Equal(20, Body.GetSplitIndexFromStart(line, 12));
            Assert.Equal(20, Body.GetSplitIndexFromStart(line, 13));

            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 59));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 60));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 61));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 65));
        }

        [Fact]
        public void GetSplitIndexFromStartParaBreakLineTest()
        {
            var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234" + Body.ParaBreak;
            Assert.Equal(66, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 0));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 1));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 2));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 3));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 10));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 11));

            Assert.Equal(20, Body.GetSplitIndexFromStart(line, 12));
            Assert.Equal(20, Body.GetSplitIndexFromStart(line, 13));

            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 59));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 60));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 61));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 65));
        }

        [Fact]
        public void GetSplitIndexFromStartMultiSpaceTest()
        {               //          1         2         3         4         5         6
            //var line = "0123456789 123456789 123456789 123456789 123456789 123456789 1234";
            var line =   "0123456789 123456789 123456789 123456789 123456789 1234567   1234";
            Assert.Equal(65, line.Length);

            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 0));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 1));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 2));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 3));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 10));
            Assert.Equal(10, Body.GetSplitIndexFromStart(line, 11));

            Assert.Equal(20, Body.GetSplitIndexFromStart(line, 12));

            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 50));
            Assert.Equal(50, Body.GetSplitIndexFromStart(line, 51));
            Assert.Equal(58, Body.GetSplitIndexFromStart(line, 52));
            Assert.Equal(58, Body.GetSplitIndexFromStart(line, 58));
            Assert.Equal(58, Body.GetSplitIndexFromStart(line, 59));

            Assert.Equal(59, Body.GetSplitIndexFromStart(line, 60));
            Assert.Equal(60, Body.GetSplitIndexFromStart(line, 61));
            Assert.Equal(-1, Body.GetSplitIndexFromStart(line, 62));
        }

        [Fact]
        public void GetSplitIndexFromEndTest()
        {             //6         5         4         3         2         1
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
    }
}
