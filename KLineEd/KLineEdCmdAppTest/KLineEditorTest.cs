using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest
{
    public class KLineEditorTest : IClassFixture<ModelTextEditViewFixture>
    {
        private readonly ModelTextEditViewFixture _fixture;

        public KLineEditorTest(ModelTextEditViewFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void StartTest()
        {
            //Assert.True(_fixture?.Model?.Ready ?? false);
            //Assert.Equal(0, _fixture.Model.GetTextLineCount());

            //var editor = new KLineEditor();

            //var rcEdit = editor.Start(_fixture.Model);
            //Assert.True(rcEdit.GetResult());
            //Assert.True(editor.Ready);
        }

        //[Fact]
        //public void StartNullParamOneTest()
        //{
        //    var edit = new Edit();

        //    var rcEdit = edit.Start(null, "xxx");

        //    Assert.StartsWith("error 1040101-param: xxx is invalid", rcEdit.GetErrorTechMsg());
        //    Assert.False(rcEdit.GetResult());
        //    Assert.Null(edit.CurrentSession);
        //    Assert.Null(edit.Sessions);
        //    Assert.Null(edit.LastLines);
        //}

        //[Fact]
        //public void StartNullParamTwoTest()
        //{
        //    var edit = new Edit();

        //    var rcEdit = edit.Start(_screen.MxConsole, null);

        //    Assert.StartsWith("error 1040101-param: [null] is invalid", rcEdit.GetErrorTechMsg());

        //    Assert.False(edit.InitDone);
        //    Assert.Null(edit.CurrentSession);
        //    Assert.Null(edit.Sessions);
        //    Assert.Null(edit.LastLines);
        //}

        //[Fact]
        //public void StartNoMatchingEditFileTest()
        //{
        //    var edit = new Edit();

        //    var rcEdit = edit.Start(_screen.MxConsole, "xxx");

        //    Assert.StartsWith("error 1040102-program: editFile=xxx", rcEdit.GetErrorTechMsg());
        //    Assert.False(rcEdit.GetResult());

        //    Assert.False(edit.InitDone);
        //    Assert.Null(edit.CurrentSession);
        //    Assert.Null(edit.Sessions);
        //    Assert.Null(edit.LastLines);
        //}

        //[Fact]
        //public void FinishTest()
        //{
        //    var edit = new Edit();

        //    var rcEdit = edit.Start(_screen.MxConsole, ScreenFixture.UnitTestEditFileName);
        //    Assert.True(rcEdit.GetResult());

        //    Assert.True(edit.InitDone);
        //    Assert.NotNull(edit.CurrentSession);
        //    Assert.NotNull(edit.Sessions);
        //    Assert.NotNull(edit.LastLines);

        //    var rcDone = edit.Finish();
        //    Assert.True(rcDone.GetResult());
        //    Assert.False(edit.InitDone);
        //}

        //[Fact]
        //public void FinishNotStartTest()
        //{
        //    var edit = new Edit();

        //    var rcDone = edit.Finish();
        //    Assert.False(rcDone.GetResult());
        //    Assert.StartsWith("SetResult() not called", rcDone.GetErrorTechMsg());
        //    Assert.False(edit.InitDone);
        //}

        //[Fact]
        //public void ProcessTest()
        //{
        //    var edit = new Edit();

        //    var rcEdit = edit.Start(_screen.MxConsole, ScreenFixture.UnitTestEditFileName);
        //    Assert.True(rcEdit.GetResult());

        //    Assert.True(edit.InitDone);
        //    Assert.NotNull(edit.CurrentSession);
        //    Assert.NotNull(edit.Sessions);
        //    Assert.NotNull(edit.LastLines);

        //    var rcDone = edit.Process();
        //    Assert.True(rcDone.GetResult());
        //    Assert.True(edit.InitDone);
        //}

        //[Fact]
        //public void ProcessNotStartTest()
        //{
        //    var edit = new Edit();

        //    var rcDone = edit.Process();
        //    Assert.False(rcDone.GetResult());
        //    Assert.StartsWith("SetResult() not called", rcDone.GetErrorTechMsg());
        //    Assert.False(edit.InitDone);
        //}

        //[Fact]
        //public void GetMetaDataReportTest()
        //{
        //    var edit = new Edit();

        //    var rcEdit = edit.Start(_screen.MxConsole, ScreenFixture.UnitTestEditFileName);
        //    Assert.True(rcEdit.GetResult());

        //    Assert.True(edit.InitDone);
        //    Assert.NotNull(edit.CurrentSession);
        //    Assert.NotNull(edit.Sessions);
        //    Assert.NotNull(edit.LastLines);

        //    Assert.StartsWith("Chapter summary", edit.GetMetaDataReport());
        //    Assert.True(edit.InitDone);
        //}
    }
}
