using System;
using KLineEdCmdApp;
using KLineEdCmdApp.View;

namespace KLineEdCmdAppTest.TestSupport
{
    public class EditViewFixture : IDisposable
    {
        public TextLinesView Terminal { get; private set; }

        public EditViewFixture()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--edit", "Test.txt" });
            if (rcParam.IsSuccess())
            {
                Terminal = new TextLinesView(new MockTerminal());
                var rcTerm = Terminal.Setup(cmdLineParams);
                if (rcTerm.IsError())
                    Terminal = null;
            }
        }

        public void Dispose()
        {
            //cleanup
        }
    }
}
