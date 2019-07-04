using System;
using KLineEdCmdApp;
using KLineEdCmdApp.View;

namespace KLineEdCmdAppTest.TestSupport
{
    public class ScreenFixture : IDisposable
    {
        public Screen Terminal { get; private set; }

        public ScreenFixture()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--edit", "Test.txt" });
            if (rcParam.IsSuccess())
            {
                Terminal = new Screen(new MockTerminal());
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
