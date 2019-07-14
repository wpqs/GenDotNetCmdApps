using System;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View;
using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ViewTests
{
    public class TextEditViewTest : IClassFixture<ModelTextEditViewFixture>
    {
        private readonly ModelTextEditViewFixture _fixture;

        public TextEditViewTest(ModelTextEditViewFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void SetupTest()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--edit", "Test.txt" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(Environment.NewLine, cmdLineParams.HelpHint);

            var view = new TextEditView(new MockTerminal());
            Assert.True(view.Setup(cmdLineParams).GetResult());

            Assert.Equal(25, view.WindowHeight);
            Assert.Equal(93, view.WindowWidth);
        }
    }
}
