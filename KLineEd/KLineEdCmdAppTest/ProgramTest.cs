using Xunit;

namespace KLineEdCmdAppTest
{
    public class ProgramTest
    {
        [Fact]
        public void GetVersionTest()
        {
            Assert.Equal("1.0.42.0", KLineEdCmdApp.Program.CmdAppVersion);
        }
    }
}
