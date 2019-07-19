using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View;
using KLineEdCmdAppTest.TestSupport.Base;

namespace KLineEdCmdAppTest.TestSupport
{
    public class ModelCmdsHelpViewFixture : ModelViewBaseFixture
    {
        public EditorHelpLineView View { get; }

        // ReSharper disable once RedundantBaseConstructorCall
        public ModelCmdsHelpViewFixture() : base()
        {
            if (Error == TestConst.UnitTestNone)
            {
                Error = TestConst.UnitTestNotSet;
                var terminal = new MockTerminal();
                terminal.Setup(new TerminalProperties());
                View = new EditorHelpLineView(terminal);
                var rcTerm = View.Setup(AppCmdLineParams);
                if (rcTerm.IsError())
                    Error = rcTerm.GetErrorTechMsg();
                else
                {
                    if ((Unsubscribe = Model.Subscribe(View)) != null)
                    {
                        Error = TestConst.UnitTestNone;
                    }
                }
            }
        }
    }
}
