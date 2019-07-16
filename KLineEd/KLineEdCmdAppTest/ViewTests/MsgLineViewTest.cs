using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ViewTests
{
    public class MsgLineViewTest : IClassFixture<ModelMsgLineViewFixture>
    {
        private readonly ModelMsgLineViewFixture _fixture;

        public MsgLineViewTest(ModelMsgLineViewFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void OnUpdateTest()
        {
            Assert.Equal(TestConst.UnitTestNone, _fixture.Error);
            Assert.True(_fixture.Model.Ready);
            Assert.True(_fixture.View.Ready);

            _fixture.Model.SetMsgLine("test msg");
            Assert.Equal("info: test msg", _fixture.View.LastTerminalOutput);
        }
    }
}
