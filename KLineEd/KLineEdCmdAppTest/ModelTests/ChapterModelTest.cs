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
            var rc = manuscriptNew.Initialise(65, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.GetTextLineCount());
            Assert.True(manuscriptNew.Close().GetResult());

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(65,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptExisting.Ready);
            Assert.Equal(0, manuscriptExisting.GetTextLineCount());  //check that reopening an existing file doesn't add any empty lines to body
            Assert.True(manuscriptExisting.Close().GetResult());
        }

        [Fact]
        public void InitialiseNullTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(65, null);

            Assert.False(rc.GetResult());
            Assert.Equal("error 1050101-param: LineWidth=65 is invalid or pathFilename=[null]", rc.GetErrorTechMsg());
            Assert.False(manuscript.Ready);
        }


        [Fact]
        public void InitialiseLineWidthNotSetTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(Program.PosIntegerNotSet,  _instancePathFileName);

            Assert.False(rc.GetResult());
            Assert.StartsWith("error 1050101-param: LineWidth=-1 is invalid", rc.GetErrorTechMsg());
            Assert.False(manuscript.Ready);
        }

        [Fact]
        public void InitialiseInvalidDirectoryTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(65, TestConst.UnitTestInvalidPathFileName);

            Assert.False(rc.GetResult());
            Assert.StartsWith("error 1050102-user: folder for edit file", rc.GetErrorUserMsg());
            Assert.False(manuscript.Ready);
        }

        [Fact]
        public void AppendLineTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(65,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.GetTextLineCount());
            Assert.True(manuscriptNew.AppendLine("test one").GetResult());
            Assert.True(manuscriptNew.AppendLine("test two").GetResult());
            Assert.True(manuscriptNew.AppendLine("test three").GetResult());
            Assert.True(manuscriptNew.Save().GetResult());
            Assert.True(manuscriptNew.Close(false).GetResult());

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(65, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptExisting.Ready);
            Assert.Equal(3, manuscriptExisting.GetTextLineCount());
            Assert.Equal(6, manuscriptExisting.GetTextWordCount());
            Assert.Equal("test one", manuscriptExisting.GetLastLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("test two", manuscriptExisting.GetLastLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("test three", manuscriptExisting.GetLastLinesForDisplay(3).GetResult()[2]);

            Assert.True(manuscriptExisting.Close(false).GetResult());
        }

        [Fact]
        public void AppendWordTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(65, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.GetTextLineCount());
            Assert.True(manuscriptNew.AppendWord("one").GetResult());
            Assert.True(manuscriptNew.AppendWord("two").GetResult());
            Assert.True(manuscriptNew.AppendWord("three").GetResult());
            Assert.True(manuscriptNew.Save().GetResult());
            Assert.True(manuscriptNew.Close(false).GetResult());

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(65, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptExisting.Ready);
            Assert.Equal(1, manuscriptExisting.GetTextLineCount());
            Assert.Equal(3, manuscriptExisting.GetTextWordCount());
            Assert.Equal("one two three", manuscriptExisting.GetLastLinesForDisplay(1).GetResult()[0]);

            Assert.True(manuscriptExisting.Close(false).GetResult());
        }

        [Fact]
        public void AppendCharTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(65, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.GetTextLineCount());
            Assert.True(manuscriptNew.AppendChar('a').GetResult());
            Assert.True(manuscriptNew.AppendChar('b').GetResult());
            Assert.True(manuscriptNew.AppendChar(' ').GetResult());
            Assert.True(manuscriptNew.AppendChar('c').GetResult());
            Assert.True(manuscriptNew.Save().GetResult());
            Assert.True(manuscriptNew.Close(false).GetResult());

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(65, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptExisting.Ready);
            Assert.Equal(1, manuscriptExisting.GetTextLineCount());
            Assert.Equal(2, manuscriptExisting.GetTextWordCount());
            Assert.Equal("ab c", manuscriptExisting.GetLastLinesForDisplay(1).GetResult()[0]);

            Assert.True(manuscriptExisting.Close(false).GetResult());
        }


        [Fact]
        public void GetReportTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(65, _instancePathFileName);

            Assert.True(manuscript.CreateNewSession().GetResult());

            Assert.True(rc.GetResult());
            Assert.True(manuscript.Ready);
            Assert.Equal(0, manuscript.GetTextLineCount());
            Assert.True(manuscript.AppendLine("test one").GetResult());
            Assert.True(manuscript.AppendLine("test two").GetResult());
            Assert.True(manuscript.AppendLine("test three").GetResult());

            Assert.StartsWith($"{Environment.NewLine}Author: [author not set]{Environment.NewLine}Project: [project not set]", manuscript.GetReport());
            Assert.Contains("Chapter stats:", manuscript.GetReport());

            Assert.True(manuscript.Close(false).GetResult());
        }

        [Fact]
        public void GetLastLinesTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(65,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscript.Ready);
            Assert.Equal(0, manuscript.GetTextLineCount());
            Assert.True(manuscript.AppendLine("test one").GetResult());
            Assert.True(manuscript.AppendLine("test two").GetResult());
            Assert.True(manuscript.AppendLine("test three").GetResult());

            Assert.Equal("test one", manuscript.GetLastLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("test two", manuscript.GetLastLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("test three", manuscript.GetLastLinesForDisplay(3).GetResult()[2]);

            Assert.True(manuscript.Close().GetResult());
        }

        [Fact]
        public void SaveTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(65,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.GetTextLineCount());

            Assert.True(manuscriptNew.AppendLine("test one").GetResult());
            Assert.True(manuscriptNew.AppendLine("test two").GetResult());
            Assert.True(manuscriptNew.AppendLine("test three").GetResult());

            Assert.True(manuscriptNew.Save().GetResult());
            Assert.True(manuscriptNew.Close(false).GetResult());

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(65,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptExisting.Ready);

            Assert.Equal(3, manuscriptExisting.GetTextLineCount());  //check that reopening an existing file doesn't add any empty lines to body
            Assert.Equal("test one", manuscriptExisting.GetLastLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("test two", manuscriptExisting.GetLastLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("test three", manuscriptExisting.GetLastLinesForDisplay(3).GetResult()[2]);

            Assert.True(manuscriptExisting.Close().GetResult());
        }

        [Fact]
        public void CloseTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(65,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.GetTextLineCount());
            Assert.True(manuscriptNew.AppendLine("test oneX").GetResult());
            Assert.True(manuscriptNew.AppendLine("test twoX").GetResult());
            Assert.True(manuscriptNew.AppendLine("test threeX").GetResult());
            Assert.True(manuscriptNew.Close().GetResult()); //default parameter closes and saves

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(65,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptExisting.Ready);
            Assert.Equal(3, manuscriptExisting.GetTextLineCount());  //check that reopening an existing file doesn't add any empty lines to body
            Assert.Equal("test oneX", manuscriptExisting.GetLastLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("test twoX", manuscriptExisting.GetLastLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("test threeX", manuscriptExisting.GetLastLinesForDisplay(3).GetResult()[2]);
            Assert.True(manuscriptExisting.Close().GetResult());
        }

        [Fact]
        public void CreateNewSessionTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(65,  _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscript.Ready);

            Assert.True(manuscript.CreateNewSession().GetResult());
            Assert.Equal(1, manuscript.GetLastSession().SessionNo);

            Assert.Equal(0, manuscript.GetTextLineCount());
            Assert.True(manuscript.AppendLine("test oneX").GetResult());
            Assert.True(manuscript.AppendLine("test twoX").GetResult());
            Assert.True(manuscript.AppendLine("test threeX").GetResult());

            Assert.True(manuscript.Close().GetResult()); //default parameter closes and saves
        }

        [Fact]
        public void RemoveAllLinesTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(65, _instancePathFileName);

            Assert.True(rc.GetResult());
            Assert.True(manuscript.Ready);

            Assert.Equal(0, manuscript.GetTextLineCount());
            Assert.True(manuscript.AppendLine("test one").GetResult());
            Assert.True(manuscript.AppendLine("test two").GetResult());
            Assert.True(manuscript.AppendLine("test three").GetResult());
            Assert.Equal(3, manuscript.GetTextLineCount());
            Assert.Equal(6, manuscript.GetTextWordCount());

            Assert.True(manuscript.RemoveAllLines());
            Assert.Equal(0, manuscript.GetTextLineCount());
            Assert.Equal(0, manuscript.GetTextWordCount());

            Assert.True(manuscript.Close().GetResult()); //default parameter closes and saves
        }
    }
}
