using System;
using System.IO;
using KLineEdCmdApp;
using KLineEdCmdApp.Model;
using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ModelTests
{
    public class ChapterModelTest : IClassFixture<ChapterModelFixture>
    {
        private readonly ChapterModelFixture _editfile;

        public ChapterModelTest(ChapterModelFixture editfile)
        {
            _editfile = editfile;
            if (File.Exists(_editfile.CreatePathFilename))
                File.Delete(_editfile.CreatePathFilename);

        }
        [Fact]
        public void InitialiseTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(65, _editfile.CreatePathFilename);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.GetTextLineCount());
            Assert.True(manuscriptNew.Close().GetResult());

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(65, _editfile.CreatePathFilename);

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
            var rc = manuscript.Initialise(KLineEditor.PosIntegerNotSet, _editfile.CreatePathFilename);

            Assert.False(rc.GetResult());
            Assert.StartsWith("error 1050101-param: LineWidth=-1 is invalid", rc.GetErrorTechMsg());
            Assert.False(manuscript.Ready);
        }

        [Fact]
        public void InitialiseInvalidDirectoryTest()
        {
            var manuscript = new ChapterModel();
            var rc = manuscript.Initialise(65, ChapterModelFixture.UnitTestInvalidPathFileName);

            Assert.False(rc.GetResult());
            Assert.StartsWith("error 1050102-user: folder for edit file", rc.GetErrorUserMsg());
            Assert.False(manuscript.Ready);
        }

        [Fact]
        public void GetChapterReportTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(65, _editfile.CreatePathFilename);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.GetTextLineCount());
            var data = $"Author: [author not set]{Environment.NewLine}Project: [project not set]{Environment.NewLine}Title: [title not set]{Environment.NewLine}File: {_editfile.CreatePathFilename}";
            Assert.StartsWith(data, manuscriptNew.GetChapterReport());
            Assert.True(manuscriptNew.Close().GetResult());
        }

        [Fact]
        public void AddLineTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(65, _editfile.CreatePathFilename);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.GetTextLineCount());
            Assert.True(manuscriptNew.AppendLine("test one").GetResult());
            Assert.True(manuscriptNew.AppendLine("test two").GetResult());
            Assert.True(manuscriptNew.AppendLine("test three").GetResult());
            Assert.True(manuscriptNew.Save().GetResult());
            Assert.True(manuscriptNew.Close(false).GetResult());
        }

        [Fact]
        public void AddWordNewLineTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(65, _editfile.CreatePathFilename);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.GetTextLineCount());
            Assert.True(manuscriptNew.AppendWord("aaa").GetResult());
            Assert.Equal(1, manuscriptNew.GetTextLineCount());
            Assert.Equal("aaa", manuscriptNew.GetLastLinesForDisplay(1).GetResult()[0]);
            Assert.True(manuscriptNew.Save().GetResult());
            Assert.True(manuscriptNew.Close(false).GetResult());

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(65, _editfile.CreatePathFilename);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptExisting.Ready);
            Assert.Equal(1, manuscriptExisting.GetTextLineCount());  //check that reopening an existing file doesn't add any empty lines to body
            Assert.Equal("aaa", manuscriptExisting.GetLastLinesForDisplay(1).GetResult()[0]);
            Assert.True(manuscriptExisting.Close().GetResult());
        }

        [Fact]
        public void GetLastLinesTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(65, _editfile.CreatePathFilename);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.GetTextLineCount());
            Assert.True(manuscriptNew.AppendLine("test one").GetResult());
            Assert.True(manuscriptNew.AppendLine("test two").GetResult());
            Assert.True(manuscriptNew.AppendLine("test three").GetResult());

            Assert.Equal("test one", manuscriptNew.GetLastLinesForDisplay(3).GetResult()[0]);
            Assert.Equal("test two", manuscriptNew.GetLastLinesForDisplay(3).GetResult()[1]);
            Assert.Equal("test three", manuscriptNew.GetLastLinesForDisplay(3).GetResult()[2]);

            Assert.True(manuscriptNew.Close().GetResult());
        }

        [Fact]
        public void SaveTest()
        {
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(65, _editfile.CreatePathFilename);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.GetTextLineCount());

            Assert.True(manuscriptNew.AppendLine("test one").GetResult());
            Assert.True(manuscriptNew.AppendLine("test two").GetResult());
            Assert.True(manuscriptNew.AppendLine("test three").GetResult());

            Assert.True(manuscriptNew.Save().GetResult());
            Assert.True(manuscriptNew.Close(false).GetResult());

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(65, _editfile.CreatePathFilename);

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
            var rc = manuscriptNew.Initialise(65, _editfile.CreatePathFilename);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);
            Assert.Equal(0, manuscriptNew.GetTextLineCount());
            Assert.True(manuscriptNew.AppendLine("test oneX").GetResult());
            Assert.True(manuscriptNew.AppendLine("test twoX").GetResult());
            Assert.True(manuscriptNew.AppendLine("test threeX").GetResult());
            Assert.True(manuscriptNew.Close().GetResult()); //default parameter closes and saves

            var manuscriptExisting = new ChapterModel();
            rc = manuscriptExisting.Initialise(65, _editfile.CreatePathFilename);

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
            var manuscriptNew = new ChapterModel();
            var rc = manuscriptNew.Initialise(65, _editfile.CreatePathFilename);

            Assert.True(rc.GetResult());
            Assert.True(manuscriptNew.Ready);

            Assert.True(manuscriptNew.CreateNewSession().GetResult());
            Assert.Equal(1, manuscriptNew.GetLastSession().SessionNo);

            Assert.Equal(0, manuscriptNew.GetTextLineCount());
            Assert.True(manuscriptNew.AppendLine("test oneX").GetResult());
            Assert.True(manuscriptNew.AppendLine("test twoX").GetResult());
            Assert.True(manuscriptNew.AppendLine("test threeX").GetResult());

            Assert.True(manuscriptNew.Close().GetResult()); //default parameter closes and saves

        }
    }
}
