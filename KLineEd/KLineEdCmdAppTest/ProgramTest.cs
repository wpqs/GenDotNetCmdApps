using System;
using Xunit;
using KLineEdCmdApp;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace KLineEdCmdAppTest
{
    public class ProgramTest
    {
        [Fact]
        public void GetVersion()
        {
            Assert.Equal("v1.0.30.0", KLineEdCmdApp.Program.GetVersion());
        }
    }
}
