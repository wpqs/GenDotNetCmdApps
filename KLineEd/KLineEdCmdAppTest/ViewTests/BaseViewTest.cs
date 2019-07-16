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
        public void FormatMxErrorTest()
        {
            Assert.Equal("error 1010102-user: msg", BaseView.FormatMxErrorMsg(1010102, BaseView.ErrorType.user, "msg"));
            Assert.Equal("error 1010102-param: msg", BaseView.FormatMxErrorMsg(1010102, BaseView.ErrorType.param, "msg"));
            Assert.Equal("error 1010102-program: msg", BaseView.FormatMxErrorMsg(1010102, BaseView.ErrorType.program, "msg"));
            Assert.Equal("error 1010102-data: msg", BaseView.FormatMxErrorMsg(1010102, BaseView.ErrorType.data, "msg"));
            Assert.Equal("error 1010102-exception: msg", BaseView.FormatMxErrorMsg(1010102, BaseView.ErrorType.exception, "msg"));
        }

        [Fact]
        public void IsCriticalErrorTest()
        {
            Assert.False(BaseView.IsCriticalError(BaseView.FormatMxErrorMsg(1010102, BaseView.ErrorType.user, "msg")));
            Assert.False(BaseView.IsCriticalError(null));   //null error msg means no error

            Assert.True(BaseView.IsCriticalError(BaseView.FormatMxErrorMsg(1010102, BaseView.ErrorType.param, "msg")));
            Assert.True(BaseView.IsCriticalError(BaseView.FormatMxErrorMsg(1010102, BaseView.ErrorType.program, "msg")));
            Assert.True(BaseView.IsCriticalError(BaseView.FormatMxErrorMsg(1010102, BaseView.ErrorType.data, "msg")));
            Assert.True(BaseView.IsCriticalError(BaseView.FormatMxErrorMsg(1010102, BaseView.ErrorType.exception, "msg")));

            Assert.False(BaseView.IsCriticalError("error 1010102-user: msg"));
            Assert.True(BaseView.IsCriticalError("error 1010102user: msg"));
            Assert.True(BaseView.IsCriticalError("error 1010102-user msg"));
            Assert.False(BaseView.IsCriticalError("warn 1010102-user: msg"));

        }

        [Fact]
        public void DisplayMsgTest()
        {
            Assert.True(_fixture.View.Ready);

            Assert.True( _fixture.View.DisplayMsg(BaseView.MsgType.Info, "test"));
            Assert.Equal($"{BaseView.InfoMsgPrecursor} test", _fixture.View.LastTerminalOutput);

            Assert.True(_fixture.View.DisplayMsg(BaseView.MsgType.Warning, "testx"));
            Assert.Equal($"{BaseView.WarnMsgPrecursor} testx", _fixture.View.LastTerminalOutput);

            Assert.True(_fixture.View.DisplayMsg(BaseView.MsgType.Error, "testy"));
            Assert.Equal($"{BaseView.ErrorMsgPrecursor} testy", _fixture.View.LastTerminalOutput);

            Assert.True(_fixture.View.DisplayMsg(BaseView.MsgType.Error, null));
            Assert.Equal($"error 1110201-program: DisplayMsg is null", _fixture.View.LastTerminalOutput);

            Assert.True(_fixture.View.DisplayMsg(BaseView.MsgType.Warning, null));
            Assert.Equal($"error 1110201-program: DisplayMsg is null", _fixture.View.LastTerminalOutput);

            Assert.True(_fixture.View.DisplayMsg(BaseView.MsgType.Info, null));
            Assert.Equal($"error 1110201-program: DisplayMsg is null", _fixture.View.LastTerminalOutput);
        }


        [Fact]
        public void DisplayMxErrorTest()
        {
            Assert.True(_fixture.View.Ready);

            var errMsg = BaseView.FormatMxErrorMsg(1010102, BaseView.ErrorType.user, "msg");
            _fixture.View.DisplayMxErrorMsg(errMsg);
            Assert.Equal($"error 1010102-user: msg", _fixture.View.LastTerminalOutput);

            _fixture.View.DisplayMxErrorMsg(null);
            Assert.Equal($"error 1110201-program: DisplayMsg is null", _fixture.View.LastTerminalOutput);

            _fixture.View.DisplayMxErrorMsg("xyz"); //typical use is DisplayMxErrorMsg(rc.GetErrorUserMsg()) so MxReturnCode is responsible for formatting the message string; no checks made in this regard 
            Assert.Equal($"{BaseView.ErrorMsgPrecursor} xyz", _fixture.View.LastTerminalOutput);

        }
    }
}
