using KLineEdCmdApp;
using KLineEdCmdApp.Utils;
using Xunit;

namespace KLineEdCmdAppTest.UtilsTests
{
    public class MxDotNetLibUtilsTest
    {
        [Fact]
        public void TestIsValidUri()
        {
            Assert.True(KLineEditor.IsValidUri(CmdLineParamsApp.ArgToolHelpUrlDefault));
            Assert.True(KLineEditor.IsValidUri(CmdLineParamsApp.ArgToolSearchUrlDefault));
            Assert.True(KLineEditor.IsValidUri(CmdLineParamsApp.ArgToolThesaurusUrlDefault));
            Assert.True(KLineEditor.IsValidUri(CmdLineParamsApp.ArgToolSpellUrlDefault));

            Assert.False(KLineEditor.IsValidUri(null));
            Assert.False(KLineEditor.IsValidUri("abc"));

        }

        [Fact]
        public void TestGetXlatUrl()
        {
            Assert.Equal("https://www.google.com/search?q=%22test%22", KLineEditor.GetXlatUrl(CmdLineParamsApp.ArgToolSearchUrlDefault, CmdLineParamsApp.UrlWordMarker, "test"));
            Assert.Equal("https://www.google.com/search?q=%22test%22", KLineEditor.GetXlatUrl("https://www.google.com/search?q=%22test%22", CmdLineParamsApp.UrlWordMarker, "XXX"));

            Assert.Null(KLineEditor.GetXlatUrl(null, CmdLineParamsApp.UrlWordMarker, "XXX"));
            Assert.Null(KLineEditor.GetXlatUrl(CmdLineParamsApp.ArgToolSearchUrlDefault, null, "XXX"));
            Assert.Null(KLineEditor.GetXlatUrl(CmdLineParamsApp.ArgToolSearchUrlDefault, CmdLineParamsApp.UrlWordMarker, null));

        }
    }
}
