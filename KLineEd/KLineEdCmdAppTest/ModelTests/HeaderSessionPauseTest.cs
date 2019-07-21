using System;
using KLineEdCmdApp;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Model.Base;
using MxDotNetUtilsLib;
using Xunit;

namespace KLineEdCmdAppTest.ModelTests
{
    public class HeaderSessionPauseTest
    {
        [Fact]
        public void ToStringTest()
        {
            var data = $"12:30:45,120;";

            var info = new HeaderSessionPause();
            Assert.True(info.IsError());
            Assert.True(info.InitialiseFromString(data).GetResult());
            Assert.False(info.IsError());
            Assert.Equal(data, info.ToString());
            Assert.Equal("12:30:45", info.PauseTime?.ToString(MxStdFrmt.Time));
            Assert.Equal(120, info.Duration);
        }

        [Fact]
        public void GetReportTest()
        {
            var data = $"{Environment.NewLine}12:30:45,120;";

            var info = new HeaderSessionPause();
            Assert.True(info.InitialiseFromString(data).GetResult());
            Assert.False(info.IsError());
            Assert.Equal(data, info.GetReport());
        }

        [Fact]
        public void ZeroPauseTimeTest()
        {
            var info = new HeaderSessionPause();
            Assert.True(info.IsError());

            var data = $"12:30:45,0;";
            Assert.True(info.InitialiseFromString(data).GetResult());
            Assert.False(info.IsError());
            Assert.Equal(data, info.ToString());
            Assert.Equal("12:30:45", info.PauseTime?.ToString(MxStdFrmt.Time));
            Assert.Equal(0, info.Duration);
        }

        [Fact]
        public void LargePauseTimeTest()
        {
            var info = new HeaderSessionPause();
            Assert.True(info.IsError());

            var data = $"12:30:45,99999999;";
            Assert.True(info.InitialiseFromString(data).GetResult());
            Assert.False(info.IsError());
            Assert.Equal(data, info.ToString());
            Assert.Equal("12:30:45", info.PauseTime?.ToString(MxStdFrmt.Time));
            Assert.Equal(99999999, info.Duration);
        }

        [Fact]
        public void NegPauseTimeTest()
        {
            var info = new HeaderSessionPause();
            Assert.True(info.IsError());

            var data = $"12:30:45,-1;";
            var rc = info.InitialiseFromString(data);
            Assert.False(rc.GetResult());
            Assert.StartsWith("error 1080305-data: GetPosInteger(-1) failed", rc.GetErrorTechMsg());

            Assert.True(info.IsError());
            Assert.Null(info.PauseTime?.ToString(MxStdFrmt.Time));
            Assert.Equal(Program.PosIntegerNotSet, info.Duration);
        }

        [Fact]
        public void NullTest()
        {
            var info = new HeaderSessionPause();
            var rc = info.InitialiseFromString(null);
            Assert.False(rc.GetResult());
            Assert.StartsWith("error 1080101-param: toString is null", rc.GetErrorTechMsg());
            Assert.True(info.IsError());
            Assert.Equal("", info.ToString());
            Assert.Null(info.PauseTime?.ToString(MxStdFrmt.Time));
            Assert.Equal(Program.PosIntegerNotSet, info.Duration);
        }

        [Fact]
        public void MissingPauseTimeTest()
        {
            var data = $",120;";
            var info = new HeaderSessionPause();

            var rc = info.InitialiseFromString(data);
            Assert.False(rc.GetResult());
            Assert.StartsWith("error 1080202-data: value=,120; is invalid", rc.GetErrorTechMsg());

            Assert.True(info.IsError());
            Assert.Equal("", info.ToString());
            Assert.Null(info.PauseTime?.ToString(MxStdFrmt.Time));
            Assert.Equal(Program.PosIntegerNotSet, info.Duration);
        }

        [Fact]
        public void MissingDurationTest()
        {
            var data = $"12:30:45,;";
            var info = new HeaderSessionPause();

            var rc = info.InitialiseFromString(data);
            Assert.False(rc.GetResult());
            Assert.StartsWith("error 1080203-data: value=12:30:45,; is invalid", rc.GetErrorTechMsg());
            Assert.True(info.IsError());
            Assert.Equal("", info.ToString());
            Assert.Null(info.PauseTime?.ToString(MxStdFrmt.Time));
            Assert.Equal(Program.PosIntegerNotSet, info.Duration);
        }

        [Fact]
        public void MissingSeparatorTest()
        {
            var data = $"12:30:45 120;";

            var info = new HeaderSessionPause();
            Assert.False(info.InitialiseFromString(data).GetResult());
            Assert.True(info.IsError());
            Assert.Equal("", info.ToString());
            Assert.Null(info.PauseTime?.ToString(MxStdFrmt.Time));
            Assert.Equal(Program.PosIntegerNotSet, info.Duration);
        }

        [Fact]
        public void MissingTerminatorTest()
        {
            var data = $"12:30:45,120";

            var info = new HeaderSessionPause();
            Assert.False(info.InitialiseFromString(data).GetResult());
            Assert.True(info.IsError());
            Assert.Equal("", info.ToString());
            Assert.Null(info.PauseTime?.ToString(MxStdFrmt.Time));
            Assert.Equal(Program.PosIntegerNotSet, info.Duration);
        }

        [Fact]
        public void InvalidDateTest()
        {
            var data = $"25:30:45,120;";

            var info = new HeaderSessionPause();
            Assert.False(info.InitialiseFromString(data).GetResult());
            Assert.True(info.IsError());
            Assert.Equal("", info.ToString());
            Assert.Null(info.PauseTime?.ToString(MxStdFrmt.Time));
            Assert.Equal(Program.PosIntegerNotSet, info.Duration);
        }
    }
}
