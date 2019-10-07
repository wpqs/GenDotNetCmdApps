using System;
using System.IO;
using KLineEdCmdApp.Utils;

namespace KLineEdCmdAppTest.TestSupport
{
    public class UtilsCmdLineParamsAppFixture : IDisposable
    {
        public UtilsCmdLineParamsAppFixture()
        {
            if (File.Exists(CmdLineParamsApp.ArgSettingsPathFileNameDefault))
                File.Delete(CmdLineParamsApp.ArgSettingsPathFileNameDefault);

        }
        public void Dispose()
        {

        }
    }
}
