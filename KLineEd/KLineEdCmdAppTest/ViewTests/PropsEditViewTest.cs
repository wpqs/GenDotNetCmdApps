using KLineEdCmdApp.Controller;
using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ViewTests
{
    public class PropsEditViewTest : IClassFixture<ModelPropsEditViewFixture>
    {
        private readonly ModelPropsEditViewFixture _fixture;

        public PropsEditViewTest(ModelPropsEditViewFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void SetupTest()
        {
            Assert.True(_fixture.View.Ready);

            Assert.Equal(25, _fixture.View.WindowHeight);
            Assert.Equal(93, _fixture.View.WindowWidth);

        }

        [Fact]
        public void OnUpdateTest()
        {

            Assert.Equal(TestConst.UnitTestNone, _fixture.Error);
            Assert.True(_fixture.Model.Ready);
            Assert.True(_fixture.View.Ready);

            _fixture.Model.SetEditorHelpLine(PropsEditingController.EditorHelpText);
            Assert.Equal(TestConst.MxNoError, _fixture.View.GetErrorTechMsg());
        }
    }
}
