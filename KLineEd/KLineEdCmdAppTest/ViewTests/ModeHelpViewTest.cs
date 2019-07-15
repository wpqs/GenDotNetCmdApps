using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ViewTests
{
    public class CmdsHelpViewTest : IClassFixture<ModelCmdsHelpViewFixture>
    {
        private readonly ModelCmdsHelpViewFixture _fixture;

        public CmdsHelpViewTest(ModelCmdsHelpViewFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void OnUpdateTest()
        {
            Assert.Equal(TestConst.UnitTestNone, _fixture.Error);
            Assert.True(_fixture.Model.Ready);
            Assert.True(_fixture.View.Ready);

            _fixture.Model.SetModeHelpLine("test cmd");
            Assert.Equal("test cmd", _fixture.View.LastTerminalOutput);
        }
    }
}
