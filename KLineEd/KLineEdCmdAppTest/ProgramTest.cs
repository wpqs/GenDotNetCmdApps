using Xunit;

namespace KLineEdCmdAppTest
{
    public class ProgramTest
    {
        [Fact]
        public void GetVersion()
        {
            Assert.Equal("1.0.30.0", KLineEdCmdApp.Program.CmdAppVersion);
        }
    }
}
