using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ViewTests
{
    public class StatusLineViewTest : IClassFixture<ModelStatusLineViewFixture>
    {
        private readonly ModelStatusLineViewFixture _fixture;

        public StatusLineViewTest(ModelStatusLineViewFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void OnUpdateTest()
        {
            Assert.Equal(TestConst.UnitTestNone, _fixture.Error);
            Assert.True(_fixture.Model.Ready);
            Assert.True(_fixture.View.Ready);

            _fixture.Model.SetStatusLine();
            Assert.Contains("Total words:", _fixture.View.LastTerminalOutput);
        }
    }
}
