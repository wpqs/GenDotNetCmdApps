using System;
using System.IO;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.View;

namespace KLineEdCmdAppTest.TestSupport
{
    public class ModelTextEditViewFixture : IDisposable
    {
        public TextEditView View { get; }
        public ChapterModel Model { get; }
        public string Error { get; }

        private IDisposable _unsubscribe;

        public ModelTextEditViewFixture()
        {
            _unsubscribe = null;

            Error = TestConst.UnitTestNotSet;

            if (File.Exists(TestConst.UnitTestSharedTestsPathFileName))
                File.Delete(TestConst.UnitTestSharedTestsPathFileName);

            Model = new ChapterModel();
            var rcModel = Model.Initialise(65, TestConst.UnitTestSharedTestsPathFileName);
            if (rcModel.IsError(true))
                Error = rcModel.GetErrorTechMsg();
            else
            {
                var cmdLineParams = new CmdLineParamsApp();
                var rcParam = cmdLineParams.Initialise(new[] { "--edit", "Test.txt" });
                if (rcParam.IsError(true))
                    Error = rcParam.GetErrorTechMsg();
                else
                {
                    View = new TextEditView(new MockTerminal());
                    var rcTerm = View.Setup(cmdLineParams);
                    if (rcTerm.IsError())
                        Error = rcTerm.GetErrorTechMsg();
                    else
                    {
                        if ((_unsubscribe = Model.Subscribe(View)) != null)
                        {
                            Error = TestConst.UnitTestNone;     //Assert.Equal(TestConst.UnitTestNone, _fixture.Error);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            _unsubscribe?.Dispose();
            Model.Close();
            //cleanup
        }
    }
}
