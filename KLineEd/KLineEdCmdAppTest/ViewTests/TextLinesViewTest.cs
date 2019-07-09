using System;
using KLineEdCmdApp;
using KLineEdCmdApp.View;
using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ViewTests
{
    public class TextLinesViewTest
    {
        [Fact]
        public void SetupTest()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--edit", "Test.txt" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(Environment.NewLine, cmdLineParams.HelpHint);

            var terminal = new TextLinesView(new MockTerminal());
            Assert.True(terminal.Setup(cmdLineParams).GetResult());

            Assert.Equal(34, terminal.Height);
            Assert.Equal(110, terminal.Width);
        }
    }
}
