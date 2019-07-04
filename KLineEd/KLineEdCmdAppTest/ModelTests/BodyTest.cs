using System;
using KLineEdCmdApp;
using KLineEdCmdApp.Model;
using Xunit;
// ReSharper disable All


namespace KLineEdCmdAppTest.ModelTests
{
    public class BodyTest
    {
        [Fact]
        public void GetLineCountTest()
        {
            var body = new Body();
            Assert.True(body.IsError());        //not initialised, but not necessary for GetLineCount()
            Assert.Equal(0, body.GetLineCount() );
        }

        [Fact]
        public void GetLastLinesTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());
            Assert.NotNull(body.GetLastLinesForDisplay(CmdLineParamsApp.ArgDisplayLastLinesCntDefault).GetResult());
        }

        [Fact]
        public void GetLastLinesNotInitializedTest()
        {
            var body = new Body();
            Assert.True(body.IsError());
            Assert.Null(body.GetLastLinesForDisplay(CmdLineParamsApp.ArgDisplayLastLinesCntDefault).GetResult());
        }

        [Fact]
        public void AddLineNotInitializedTest()
        {
            var body = new Body();
            Assert.True(body.IsError());
            Assert.Equal(0, body.GetLineCount());
            Assert.False(body.AddLine("one").GetResult());
            Assert.Equal(0, body.GetLineCount());
        }

        [Fact]
        public void AddOneLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AddLine("one", true).GetResult());
            Assert.Equal(1, body.RefreshWordCount());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal("one", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void AddTwoLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AddLine("one", true).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.RefreshWordCount());
            Assert.Equal("one", body.GetLastLinesForDisplay(10).GetResult()[9]);

            Assert.True(body.AddLine("two", false).GetResult());  //don't inc WordCount
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal(1, body.WordCount);            //
            Assert.Equal(2, body.RefreshWordCount());       //refreshes WordCount from actual words in Body
            Assert.Equal(2, body.WordCount);            //

            Assert.Equal("one", body.GetLastLinesForDisplay(10).GetResult()[8]);
            Assert.Equal("two", body.GetLastLinesForDisplay(10).GetResult()[9]);

        }

        [Fact]
        public void GetLastLinesZeroTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AddLine("one").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal("one", body.GetLastLinesForDisplay(10).GetResult()[9]);

            Assert.True(body.AddLine("two").GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal("one", body.GetLastLinesForDisplay(10).GetResult()[8]);
            Assert.Equal("two", body.GetLastLinesForDisplay(10).GetResult()[9]);

        }

        [Fact]
        public void GetLastLinesOneTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AddLine("one").GetResult());
            Assert.True(body.AddLine("two").GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Equal("two", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void GetLastLinesMaxTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AddLine("one").GetResult());
            Assert.True(body.AddLine("two").GetResult());
            Assert.Equal(2, body.GetLineCount());

            Assert.Equal("one", body.GetLastLinesForDisplay(CmdLineParamsApp.ArgDisplayLastLinesCntMax).GetResult()[CmdLineParamsApp.ArgDisplayLastLinesCntMax-2]);
            Assert.Equal("two", body.GetLastLinesForDisplay(CmdLineParamsApp.ArgDisplayLastLinesCntMax).GetResult()[CmdLineParamsApp.ArgDisplayLastLinesCntMax-1]);
        }

        [Fact]
        public void GetLastLinesMoreThanMaxTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AddLine("one").GetResult());
            Assert.True(body.AddLine("two").GetResult());
            Assert.Equal(2, body.GetLineCount());
            Assert.Null(body.GetLastLinesForDisplay(CmdLineParamsApp.ArgDisplayLastLinesCntMax+1).GetResult());
        }

        [Fact]
        public void AddWordNewLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AddWord("aaa").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal("aaa", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void AddWordExistingLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(65).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AddLine("one", true).GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AddWord("aaa").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("one aaa", body.GetLastLinesForDisplay(1).GetResult()[0]);
        }

        [Fact]
        public void AddWordAddLineTest()
        {
            var body = new Body();
            Assert.True(body.Initialise(35).GetResult());
            Assert.False(body.IsError());

            Assert.Equal(0, body.GetLineCount());
            Assert.True(body.AddWord("aaaa").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(1, body.WordCount);

            Assert.True(body.AddWord("bbbbb").GetResult());
            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(2, body.WordCount);
            Assert.Equal("aaaa bbbbb", body.GetLastLinesForDisplay(1).GetResult()[0]);

            Assert.True(body.AddWord("0123456789").GetResult());    //col 21
            Assert.True(body.AddWord("0123456789").GetResult());    //col 32
            Assert.True(body.AddWord("01").GetResult());            //col 35

            Assert.Equal(1, body.GetLineCount());
            Assert.Equal(5, body.WordCount);

            Assert.True(body.AddWord("x").GetResult());            //col 36
            Assert.Equal(2, body.GetLineCount());

            Assert.Equal("aaaa bbbbb 0123456789 0123456789 01", body.GetLastLinesForDisplay(2).GetResult()[0]);
            Assert.Equal("x", body.GetLastLinesForDisplay(2).GetResult()[1]);

        }
        [Fact]
        public void GetLineErrorsTest()
        {
            Assert.Null(Body.GetEnteredTextErrors(""));
            Assert.Null(Body.GetEnteredTextErrors("this text is fine"));
            Assert.StartsWith("Error: Unexpected text; it is null. This is a program error. Please save your work and restart the program.", Body.GetEnteredTextErrors(null));
            Assert.StartsWith("Error: invalid line. It contains a new line at column 7", Body.GetEnteredTextErrors($"hello {Environment.NewLine}"));
            Assert.StartsWith("Error: invalid line. It contains It contains the disallowed character '<' at column 8", Body.GetEnteredTextErrors($"hello .<hi"));
            Assert.StartsWith("Error: invalid line. It contains It contains the disallowed character '>' at column 9", Body.GetEnteredTextErrors($"hello hi>"));

            var line = "0123456789112345678921234567893123456789412345678951234567896123456789712345678981234567899123456789";
            line += "0123456789112345678921234567893123456789412345678951234567896123456789712345678981234567899123456789";
            line += "012345678911234567892123456789312345678941234567895";
            Assert.StartsWith("Error: invalid line. It has 251 characters, but only 250 allowed", Body.GetEnteredTextErrors(line));

        }

        [Fact]
        public void GetWordsInLineTest()
        {
            Assert.Equal(0, Body.GetWordsInLine(null));
            Assert.Equal(0, Body.GetWordsInLine(""));
            Assert.Equal(1, Body.GetWordsInLine("one"));
            Assert.Equal(2, Body.GetWordsInLine("one two"));
            Assert.Equal(3, Body.GetWordsInLine("one two\tthree"));
            Assert.Equal(3, Body.GetWordsInLine($"one two{Environment.NewLine}three"));
        }
    }
}
