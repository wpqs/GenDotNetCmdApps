using Xunit;
using KLineEdCmdApp.Utils;

namespace KLineEdCmdAppTest.UtilsTests
{
    public class TerminalTest
    {
        [Fact]
        public void NoParamTest()
        {
            var terminal = new Terminal();

            Assert.False(terminal.IsError());
        }

        [Fact]
        public void SetupTest()
        {
            var terminal = new Terminal();
            Assert.True(terminal.IsError() == false);

            var props = new TerminalProperties();
            Assert.True(terminal.Setup(props));
            Assert.True(terminal.IsError() == false);
        }

        [Fact]
        public void GetSettingsTest()
        {
            var terminal = new Terminal();

            var props = terminal.GetSettings();
            Assert.True(props.IsError() == false);

            Assert.True(terminal.Setup(props));
            Assert.True(terminal.IsError() == false);
        }
    }
}
