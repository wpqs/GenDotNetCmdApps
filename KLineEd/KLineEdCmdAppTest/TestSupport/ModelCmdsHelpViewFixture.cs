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
                var console = new MockMxConsole();
                console.ApplySettings(new MxConsoleProperties());
                View = new EditorHelpLineView(console);
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
