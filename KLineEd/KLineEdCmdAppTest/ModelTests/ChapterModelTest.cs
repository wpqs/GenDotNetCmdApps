using System;
using System.IO;
using KLineEdCmdApp;
using KLineEdCmdApp.Model;
using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ModelTests
{
    public class ChapterModelTest 
    {
        private readonly string _instancePathFileName;

        public ChapterModelTest()
        {
            _instancePathFileName = TestConst.UnitTestInstanceTestsPathFileName;  //file deleted before each test
            if (File.Exists(_instancePathFileName))
                File.Delete(_instancePathFileName);

        }
        [Fact]
        public void InitialiseTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.ChapterBody.GetLineCount());
            Assert.True(manuscriptNew.Close().GetResult());

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptExisting.Ready);
            Assert.Equal(0, manuscriptExisting.ChapterBody.GetLineCount());  //check that reopening an existing file doesn't add any empty lines to body
            Assert.True(manuscriptExisting.Close().GetResult());
        }

        [Fact]
        public void InitialiseNullTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth, null);

            Assert.False(rc.GetResult());
            Assert.Equal($"error 1050101-param: editAreaLinesCount={TestConst.UnitTestEditAreaLines}, editAreaLineWidth={TestConst.UnitTestEditAreaWidth} is invalid, pathFilename=[null], spacesForTab=3 (min=1)", rc.GetErrorTechMsg());
            Assert.False(manuscript.Ready);
        }


        [Fact]
        public void InitialiseLineWidthNotSetTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(TestConst.UnitTestEditAreaLines, Program.PosIntegerNotSet,  _instancePathFileName);

            Assert.False(rc.GetResult());
            Assert.StartsWith($"error 1050101-param: editAreaLinesCount={TestConst.UnitTestEditAreaLines}, editAreaLineWidth={Program.PosIntegerNotSet} is invalid", rc.GetErrorTechMsg());
            Assert.False(manuscript.Ready);
        }

        [Fact]
        public void InitialiseLineCountNotSetTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(Program.PosIntegerNotSet, TestConst.UnitTestEditAreaWidth, _instancePathFileName);

            Assert.False(rc.GetResult());
            Assert.StartsWith($"error 1050101-param: editAreaLinesCount={Program.PosIntegerNotSet}, editAreaLineWidth={TestConst.UnitTestEditAreaWidth} is invalid", rc.GetErrorTechMsg());
            Assert.False(manuscript.Ready);
        }

        [Fact]
        public void InitialiseInvalidDirectoryTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth, TestConst.UnitTestInvalidPathFileName);

            Assert.False(rc.GetResult());
            Assert.StartsWith("error 1050102-user: folder for edit file", rc.GetErrorUserMsg());
            Assert.False(manuscript.Ready);
        }

        [Fact]
        public void InsertLineTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.ChapterBody.GetLineCount());
            Assert.True(manuscriptNew.ChapterBody.InsertLine("test one").GetResult());
            Assert.True(manuscriptNew.ChapterBody.InsertLine("test two").GetResult());
            Assert.True(manuscriptNew.ChapterBody.InsertLine("test three").GetResult());
            Assert.True(manuscriptNew.Save().GetResult());
            Assert.True(manuscriptNew.Close(false).GetResult());

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptExisting.Ready);
            Assert.Equal(3, manuscriptExisting.ChapterBody.GetLineCount());
            Assert.Equal(6, manuscriptExisting.ChapterBody.WordCount);
            Assert.Equal("test one", manuscriptExisting.BodyGetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("test two", manuscriptExisting.BodyGetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("test three", manuscriptExisting.BodyGetEditAreaLinesForDisplay(3).GetResult()[2]);

            Assert.True(manuscriptExisting.Close(false).GetResult());
        }

        [Fact]
        public void SetBodyInsertTextTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.ChapterBody.GetLineCount());
            Assert.True(manuscriptNew.BodyInsertText("one").GetResult());
            Assert.True(manuscriptNew.BodyInsertText(" two").GetResult());
            Assert.True(manuscriptNew.BodyInsertText(" three").GetResult());
            Assert.True(manuscriptNew.Save().GetResult());
            Assert.True(manuscriptNew.Close(false).GetResult());

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptExisting.Ready);
            Assert.Equal(1, manuscriptExisting.ChapterBody.GetLineCount());
            Assert.Equal(3, manuscriptExisting.ChapterBody.WordCount);
            Assert.Equal("one two three>", manuscriptExisting.BodyGetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.True(manuscriptExisting.Close(false).GetResult());
        }

        [Fact]
        public void AppendCharTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.ChapterBody.GetLineCount());
            Assert.True(manuscriptNew.BodyInsertText('a'.ToString()).GetResult());
            Assert.True(manuscriptNew.BodyInsertText('b'.ToString()).GetResult());
            Assert.True(manuscriptNew.BodyInsertText(' '.ToString()).GetResult());
            Assert.True(manuscriptNew.BodyInsertText('c'.ToString()).GetResult());
            Assert.True(manuscriptNew.Save().GetResult());
            Assert.True(manuscriptNew.Close(false).GetResult());

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptExisting.Ready);
            Assert.Equal(1, manuscriptExisting.ChapterBody.GetLineCount());
            Assert.Equal(2, manuscriptExisting.ChapterBody.WordCount);
            Assert.Equal("ab c>", manuscriptExisting.BodyGetEditAreaLinesForDisplay(1).GetResult()[0]);

            Assert.True(manuscriptExisting.Close(false).GetResult());
        }


        [Fact]
        public void GetReportTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth, _instancePathFileName);

            Assert.True(manuscript.CreateNewSession().GetResult());

            Assert.True(rc.GetResult());
            Assert.True(manuscript.Ready);
            Assert.Equal(0, manuscript.ChapterBody.GetLineCount());
            Assert.True(manuscript.ChapterBody.InsertLine("test one").GetResult());
            Assert.True(manuscript.ChapterBody.InsertLine("test two").GetResult());
            Assert.True(manuscript.ChapterBody.InsertLine("test three").GetResult());

            Assert.StartsWith($"{Environment.NewLine}Author: [author not set]{Environment.NewLine}Project: [project not set]", manuscript.GetReport());
            Assert.Contains("Chapter stats:", manuscript.GetReport());

            Assert.True(manuscript.Close(false).GetResult());
        }

        [Fact]
        public void GetLastLinesTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscript.Ready);
            Assert.Equal(0, manuscript.ChapterBody.GetLineCount());
            Assert.True(manuscript.ChapterBody.InsertText("test one").GetResult());
            Assert.Equal("test one>", manuscript.BodyGetEditAreaLinesForDisplay(1).GetResult()[0]);
            Assert.True(manuscript.ChapterBody.InsertParaBreak().GetResult()); 
            Assert.True(manuscript.ChapterBody.InsertText("test two").GetResult());
            Assert.True(manuscript.ChapterBody.InsertParaBreak().GetResult());
            Assert.True(manuscript.ChapterBody.InsertText("test three").GetResult());

            Assert.Equal("test one>", manuscript.BodyGetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("test two>", manuscript.BodyGetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("test three>", manuscript.BodyGetEditAreaLinesForDisplay(3).GetResult()[2]);

            Assert.True(manuscript.Close().GetResult());
        }

        [Fact]
        public void SaveTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.ChapterBody.GetLineCount());

            Assert.True(manuscriptNew.ChapterBody.InsertLine("test one").GetResult());
            Assert.True(manuscriptNew.ChapterBody.InsertLine("test two").GetResult());
            Assert.True(manuscriptNew.ChapterBody.InsertLine("test three").GetResult());

            Assert.True(manuscriptNew.Save().GetResult());
            Assert.True(manuscriptNew.Close(false).GetResult());

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptExisting.Ready);

            Assert.Equal(3, manuscriptExisting.ChapterBody.GetLineCount());  //check that reopening an existing file doesn't add any empty lines to body
            Assert.Equal("test one", manuscriptExisting.BodyGetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("test two", manuscriptExisting.BodyGetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("test three", manuscriptExisting.BodyGetEditAreaLinesForDisplay(3).GetResult()[2]);

            Assert.True(manuscriptExisting.Close().GetResult());
        }

        [Fact]
        public void CloseTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.ChapterBody.GetLineCount());
            Assert.True(manuscriptNew.ChapterBody.InsertLine("test oneX").GetResult());
            Assert.True(manuscriptNew.ChapterBody.InsertLine("test twoX").GetResult());
            Assert.True(manuscriptNew.ChapterBody.InsertLine("test threeX").GetResult());
            Assert.True(manuscriptNew.Close().GetResult()); //default parameter closes and saves

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptExisting.Ready);
            Assert.Equal(3, manuscriptExisting.ChapterBody.GetLineCount());  //check that reopening an existing file doesn't add any empty lines to body
            Assert.Equal("test oneX", manuscriptExisting.BodyGetEditAreaLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("test twoX", manuscriptExisting.BodyGetEditAreaLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("test threeX", manuscriptExisting.BodyGetEditAreaLinesForDisplay(3).GetResult()[2]);
            Assert.True(manuscriptExisting.Close().GetResult());
        }

        [Fact]
        public void CreateNewSessionTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscript.Ready);

            Assert.True(manuscript.CreateNewSession().GetResult());
            Assert.Equal(1, manuscript.GetLastSession().SessionNo);

            Assert.Equal(0, manuscript.ChapterBody.GetLineCount());
            Assert.True(manuscript.ChapterBody.InsertLine("test oneX").GetResult());
            Assert.True(manuscript.ChapterBody.InsertLine("test twoX").GetResult());
            Assert.True(manuscript.ChapterBody.InsertLine("test threeX").GetResult());

            Assert.True(manuscript.Close().GetResult()); //default parameter closes and saves
        }

        [Fact]
        public void RemoveAllLinesTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(TestConst.UnitTestEditAreaLines, TestConst.UnitTestEditAreaWidth, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscript.Ready);

            Assert.Equal(0, manuscript.ChapterBody.GetLineCount());
            Assert.True(manuscript.ChapterBody.InsertLine("test one").GetResult());
            Assert.True(manuscript.ChapterBody.InsertLine("test two").GetResult());
            Assert.True(manuscript.ChapterBody.InsertLine("test three").GetResult());
            Assert.Equal(3, manuscript.ChapterBody.GetLineCount());
        //    Assert.Equal(6, manuscript.ChapterBody.WordCount);

            Assert.True(manuscript.RemoveAllLines());
            Assert.Equal(0, manuscript.ChapterBody.GetLineCount());
            Assert.Equal(0, manuscript.ChapterBody.WordCount);

            Assert.True(manuscript.Close().GetResult()); //default parameter closes and saves
        }
    }
}
