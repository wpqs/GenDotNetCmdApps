using KLineEdCmdApp;
using Xunit;

namespace KLineEdCmdAppTest
{
    public class ExtensionTest
    {
        [Fact]
        public void SnipTest()
        {
            var data = "012";

            var val = data.Snip(0, 2);
            Assert.Equal("012", val);

            val = data.Snip(1, 2);
            Assert.Equal("12", val);

            val = data.Snip(2, 2);
            Assert.Equal("2", val);

            val = data.Snip(3, 2);
            Assert.Null(val);

            val = data.Snip(1, 0);
            Assert.Null(val);
        }

    }
}
