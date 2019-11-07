using KLineEdCmdApp;
using KLineEdCmdApp.Utils;
using Xunit;

namespace KLineEdCmdAppTest.UtilsTests
{
    public class MxReturnCodeUtilsTest
    {

        [Fact]
        public void GetErrorTest()
        {
            var mxErrorMsg = "error 1100703-user: Warning: you cannot move beyond the end of the chapter";

            Assert.Equal("you cannot move beyond the end of the chapter", MxReturnCodeUtils.GetErrorText(mxErrorMsg));
            Assert.Equal(1100703, MxReturnCodeUtils.GetErrorCode(mxErrorMsg));
            Assert.Equal(MxReturnCodeUtils.MsgClass.Warning, MxReturnCodeUtils.GetErrorClass(mxErrorMsg));

            var mxErrorMsg2 = "error 1100703-user: you cannot move beyond the end of the chapter";

            Assert.Equal("you cannot move beyond the end of the chapter", MxReturnCodeUtils.GetErrorText(mxErrorMsg2));
            Assert.Equal(1100703, MxReturnCodeUtils.GetErrorCode(mxErrorMsg2));
            Assert.Equal(MxReturnCodeUtils.MsgClass.Error, MxReturnCodeUtils.GetErrorClass(mxErrorMsg2));

        }

        [Fact]
        public void GetErrorNoTypeTest()
        {
            var mxErrorMsg = "error 1100703: Warning: you cannot move beyond the end of the chapter";

            Assert.Equal("you cannot move beyond the end of the chapter", MxReturnCodeUtils.GetErrorText(mxErrorMsg));
            Assert.Equal(1100703, MxReturnCodeUtils.GetErrorCode(mxErrorMsg));
            Assert.Equal(MxReturnCodeUtils.MsgClass.Warning, MxReturnCodeUtils.GetErrorClass(mxErrorMsg));

            var mxErrorMsg2 = "error 1100703: you cannot move beyond the end of the chapter";

            Assert.Equal("you cannot move beyond the end of the chapter", MxReturnCodeUtils.GetErrorText(mxErrorMsg2));
            Assert.Equal(1100703, MxReturnCodeUtils.GetErrorCode(mxErrorMsg2));
            Assert.Equal(MxReturnCodeUtils.MsgClass.Error, MxReturnCodeUtils.GetErrorClass(mxErrorMsg2));

        }

        [Fact]
        public void GetErrorFailTest()
        {
            var mxErrorMsg = "1100703-user: Warning: you cannot move beyond the end of the chapter";

            Assert.Equal("1100703-user: Warning: you cannot move beyond the end of the chapter", MxReturnCodeUtils.GetErrorText(mxErrorMsg));
            Assert.Equal(Program.PosIntegerNotSet, MxReturnCodeUtils.GetErrorCode(mxErrorMsg));
            Assert.Equal(MxReturnCodeUtils.MsgClass.Unknown, MxReturnCodeUtils.GetErrorClass(mxErrorMsg));

            var mxErrorMsg2 = "error 11x0703-user: Warning: you cannot move beyond the end of the chapter";

            Assert.Equal("you cannot move beyond the end of the chapter", MxReturnCodeUtils.GetErrorText(mxErrorMsg2));
            Assert.Equal(Program.PosIntegerNotSet, MxReturnCodeUtils.GetErrorCode(mxErrorMsg2));
            Assert.Equal(MxReturnCodeUtils.MsgClass.Warning, MxReturnCodeUtils.GetErrorClass(mxErrorMsg2));

        }
    }
}
