using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;
using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ViewTests
{
    public class BaseViewTest : IClassFixture<ModelTextEditViewFixture>
    {
        private readonly ModelTextEditViewFixture _fixture;

        public BaseViewTest(ModelTextEditViewFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TruncateTextForLineTest()
        {
            var text = "1234567890";    //10 chars

            Assert.Equal(text,BaseView.TruncateTextForLine(text, text.Length));
            Assert.Equal("123456...", BaseView.TruncateTextForLine(text, text.Length-1)); //truncate to 9 chars
            Assert.Equal("...", BaseView.TruncateTextForLine(text, 3)); //truncate to 3 chars

            Assert.Equal("...", BaseView.TruncateTextForLine(text, 2)); //truncate to 3 chars
            Assert.Equal("...", BaseView.TruncateTextForLine(text, 1)); //truncate to 3 chars
            Assert.Equal("...", BaseView.TruncateTextForLine(text, 0)); //truncate to 3 chars
            Assert.Equal("...", BaseView.TruncateTextForLine(text, -1)); //truncate to 3 chars
            Assert.Equal("...", BaseView.TruncateTextForLine(null, 10)); //truncate to 3 chars
        }


        [Fact]
        public void DisplayMsgTest()
        {
            Assert.True(_fixture.View.Ready);       //Formatting of the message may differ - see BaseEditingController.ErrorProcessing()

            Assert.True( _fixture.View.DisplayMsg(MxReturnCodeUtils.MsgClass.Info, $"{MxReturnCodeUtils.InfoMsgPrecursor}(1110201) test"));
            Assert.Equal("Info: (1110201) test", _fixture.View.LastConsoleOutput);

            Assert.True(_fixture.View.DisplayMsg(MxReturnCodeUtils.MsgClass.Warning, $"{MxReturnCodeUtils.WarningMsgPrecursor}(1110201) testx"));
            Assert.Equal("Warning: (1110201) testx", _fixture.View.LastConsoleOutput);

            Assert.True(_fixture.View.DisplayMsg(MxReturnCodeUtils.MsgClass.Error, $"{MxReturnCodeUtils.ErrorMsgPrecursor}(1110201) testy"));
            Assert.Equal("Error: (1110201) testy", _fixture.View.LastConsoleOutput);

            Assert.True(_fixture.View.DisplayMsg(MxReturnCodeUtils.MsgClass.Error, null));
            Assert.Equal($"Error: (1110201) DisplayMsg is null", _fixture.View.LastConsoleOutput);

            Assert.True(_fixture.View.DisplayMsg(MxReturnCodeUtils.MsgClass.Warning, null));
            Assert.Equal($"Error: (1110201) DisplayMsg is null", _fixture.View.LastConsoleOutput);

            Assert.True(_fixture.View.DisplayMsg(MxReturnCodeUtils.MsgClass.Info, null));
            Assert.Equal($"Error: (1110201) DisplayMsg is null", _fixture.View.LastConsoleOutput);
        }
    }
}
