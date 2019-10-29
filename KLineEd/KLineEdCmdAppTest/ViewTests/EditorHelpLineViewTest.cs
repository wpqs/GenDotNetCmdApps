using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ViewTests
{
    public class EditorHelpLineViewTest : IClassFixture<ModelCmdsHelpViewFixture>
    {
        private readonly ModelCmdsHelpViewFixture _fixture;

        public EditorHelpLineViewTest(ModelCmdsHelpViewFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void OnUpdateTest()
        {
            Assert.Equal(TestConst.UnitTestNone, _fixture.Error);
            Assert.True(_fixture.Model.Ready);
            Assert.True(_fixture.View.Ready);

            _fixture.Model.SetEditorHelpLine("test cmd");
            Assert.Equal(TestConst.MxNoError, _fixture.View.GetErrorTechMsg());
            Assert.Equal("test cmd", _fixture.View.LastConsoleOutput);
        }
    }
}
