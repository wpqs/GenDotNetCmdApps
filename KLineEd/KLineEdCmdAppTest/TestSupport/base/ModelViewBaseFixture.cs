using System;
using System.IO;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.Model;


namespace KLineEdCmdAppTest.TestSupport.Base 
{
    public abstract class ModelViewBaseFixture : IDisposable
    {
        public ChapterModel Model { get; }
        public string Error { protected set; get; }
        public CmdLineParamsApp AppCmdLineParams { get; }

        protected IDisposable Unsubscribe;

        public ModelViewBaseFixture()
        {
            Unsubscribe = null;

            Error = TestConst.UnitTestNotSet;

            if (File.Exists(TestConst.UnitTestSharedTestsPathFileName))
                File.Delete(TestConst.UnitTestSharedTestsPathFileName);

            Model = new ChapterModel();
            var rcModel = Model.Initialise(65, TestConst.UnitTestSharedTestsPathFileName);
            if (rcModel.IsError(true))
                Error = rcModel.GetErrorTechMsg();
            else
            {
                if (Model.RemoveAllLines())
                {
                    AppCmdLineParams = new CmdLineParamsApp();
                    var rcParam = AppCmdLineParams.Initialise(new[] {"--edit", "Test.txt"});
                    if (rcParam.IsError(true))
                        Error = rcParam.GetErrorTechMsg();
                    else
                    {
                        Error = TestConst.UnitTestNone;
                    }
                }
            }
        }
        public void Dispose()
        {
            Unsubscribe?.Dispose();
            Model.Close();
        }
    }
}
