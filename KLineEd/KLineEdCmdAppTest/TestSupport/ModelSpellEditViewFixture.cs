using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View;
using KLineEdCmdAppTest.TestSupport.Base;

namespace KLineEdCmdAppTest.TestSupport
{
    public class ModelSpellEditViewFixture : ModelViewBaseFixture
    {
        public SpellEditView View { get; }

        public ModelSpellEditViewFixture()
        {
            if (Error == TestConst.UnitTestNone)
            {
                Error = TestConst.UnitTestNotSet;
                var terminal = new MockTerminal();
                terminal.Setup(new TerminalProperties());
                View = new SpellEditView(terminal);
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
