using System;
using KLineEdCmdApp;
using KLineEdCmdApp.Model;
using MxDotNetUtilsLib;
using Xunit;

namespace KLineEdCmdAppTest.ModelTests
{
    public class HeaderSessionTest
    {
        [Fact]
        public void ToStringTypicalTest()
        {
            var data = "Session: 4 Start: 23-06-19 12:10:00 Duration: 02:13:34 Start line: 210 End line: 350 Words: 3501 Corrections count: 970 Spell check count: 255 Typing pauses: 12:30:45,120;14:30:01,630;[end]";
            var session = new HeaderSession();
            Assert.True(session.InitialiseFromString(data).GetResult());
            Assert.False(session.IsError());
            Assert.Equal(4, session.SessionNo);
            Assert.Equal("23-06-19 12:10:00", session.Start?.ToString(MxStdFrmt.DateTime));
            Assert.Equal("02:13:34", session.Duration?.ToString(HeaderSession.MxStdFrmtTimeSpan));
            Assert.Equal(210, session.StartLine);
            Assert.Equal(350, session.EndLine);
            Assert.Equal(350-210, session.LinesTyped);
            Assert.Equal(3501, session.WordsCount);
            Assert.Equal(26.2, session.Wpm,1);
            Assert.Equal(970, session.CorrectionsCount);
            Assert.Equal(3.6, session.Wpc, 1);
            Assert.Equal(255, session.SpellCheckCount);
            Assert.Equal(13.7, session.Wps, 1);
            Assert.Equal("02:01:04", session.TypingTime?.ToString(HeaderSession.MxStdFrmtTimeSpan));

            Assert.Equal(data, session.ToString());
        }

        [Fact]
        public void ToStringMinTest()
        {
            var data = "Session: 0 Start: 01-01-00 00:00:00 Duration: 00:00:00 Start line: 1 End line: 1 Words: 0 Corrections count: 0 Spell check count: 0 Typing pauses: [end]";
            var session = new HeaderSession();
            Assert.True(session.InitialiseFromString(data).GetResult());
            Assert.False(session.IsError());
            Assert.Equal(0, session.SessionNo);
            Assert.Equal("01-01-00 00:00:00", session.Start?.ToString(MxStdFrmt.DateTime));
            Assert.Equal("00:00:00", session.Duration?.ToString(HeaderSession.MxStdFrmtTimeSpan));
            Assert.Equal(1, session.StartLine);
            Assert.Equal(1, session.EndLine);
            Assert.Equal(0, session.LinesTyped);
            Assert.Equal(0, session.WordsCount);
            Assert.Equal(0.0, session.Wpm, 0);
            Assert.Equal(0, session.CorrectionsCount);
            Assert.Equal(0.0, session.Wpc, 0);
            Assert.Equal(0, session.SpellCheckCount);
            Assert.Equal(0.0, session.Wps, 0);
            Assert.Equal("00:00:00", session.TypingTime?.ToString(HeaderSession.MxStdFrmtTimeSpan));

            Assert.Equal(data, session.ToString());
        }

        [Fact]
        public void ToStringLargeTest()
        {
            var data = "Session: 999999 Start: 23-06-19 00:00:00 Duration: 23:59:59 Start line: 1 End line: 99999 Words: 999999 Corrections count: 99999 Spell check count: 66666 Typing pauses: 12:30:45,59;[end]";
            var session = new HeaderSession();
            Assert.True(session.InitialiseFromString(data).GetResult());
            Assert.False(session.IsError());
            Assert.Equal(999999, session.SessionNo);
            Assert.Equal("23-06-19 00:00:00", session.Start?.ToString(MxStdFrmt.DateTime));
            Assert.Equal("23:59:59", session.Duration?.ToString(HeaderSession.MxStdFrmtTimeSpan));
            Assert.Equal(1, session.StartLine);
            Assert.Equal(99999, session.EndLine);
            Assert.Equal(99998, session.LinesTyped);
            Assert.Equal(999999, session.WordsCount);
            Assert.Equal(694.4, session.Wpm, 0);
            Assert.Equal(99999, session.CorrectionsCount);
            Assert.Equal(10, session.Wpc, 0);
            Assert.Equal(66666, session.SpellCheckCount);
            Assert.Equal(15.0, session.Wps, 0);
            Assert.Equal("23:59:00", session.TypingTime?.ToString(HeaderSession.MxStdFrmtTimeSpan));

            Assert.Equal(data, session.ToString());
        }

        [Fact]
        public void ToStringWorkingOverMidnightTest()
        {
            var data = "Session: 999999 Start: 23-06-19 23:59:59 Duration: 01:00:00 Start line: 1 End line: 99999 Words: 999999 Corrections count: 99999 Spell check count: 66666 Typing pauses: 12:30:45,59;[end]";
            var session = new HeaderSession();
            Assert.True(session.InitialiseFromString(data).GetResult());
            Assert.False(session.IsError());
            Assert.Equal(999999, session.SessionNo);
            Assert.Equal("23-06-19 23:59:59", session.Start?.ToString(MxStdFrmt.DateTime));
            Assert.Equal("01:00:00", session.Duration?.ToString(HeaderSession.MxStdFrmtTimeSpan));
 
            Assert.Equal(data, session.ToString());
        }

        [Fact]
        public void ToStringNoPausesTest()
        {
            var data = "Session: 4 Start: 23-06-19 12:10:00 Duration: 02:13:34 Start line: 210 End line: 350 Words: 3501 Corrections count: 970 Spell check count: 255 Typing pauses: [end]";
            var session = new HeaderSession();
            Assert.True(session.InitialiseFromString(data).GetResult());
            Assert.False(session.IsError());
            Assert.Equal("02:13:34", session.Duration?.ToString(HeaderSession.MxStdFrmtTimeSpan));
            Assert.Equal("02:13:34", session.TypingTime?.ToString(HeaderSession.MxStdFrmtTimeSpan));

            Assert.Equal(data, session.ToString());
        }

        [Fact]
        public void ToStringNoPausesNoGapTest()
        {
            var data = "Session: 4 Start: 23-06-19 12:10:00 Duration: 02:13:34 Start line: 210 End line: 350 Words: 3501 Corrections count: 970 Spell check count: 255 ";
            var session = new HeaderSession();
            Assert.True(session.InitialiseFromString(data + "Typing pauses:[end]").GetResult());
            Assert.False(session.IsError());
            Assert.Equal("02:13:34", session.Duration?.ToString(HeaderSession.MxStdFrmtTimeSpan));
            Assert.Equal("02:13:34", session.TypingTime?.ToString(HeaderSession.MxStdFrmtTimeSpan));

            Assert.Equal(data + "Typing pauses: [end]", session.ToString());
        }

        [Fact]
        public void SingleSessionPauseTest()
        {
            var data = "Session: 4 Start: 23-06-19 12:10:00 Duration: 02:13:34 Start line: 210 End line: 350 Words: 3501 Corrections count: 970 Spell check count: 255 Typing pauses: 12:30:45,750;[end]";
            var session = new HeaderSession();
            Assert.True(session.InitialiseFromString(data).GetResult());
            Assert.False(session.IsError());
            Assert.Equal(4, session.SessionNo);
            Assert.Equal("23-06-19 12:10:00", session.Start?.ToString(MxStdFrmt.DateTime));
            Assert.Equal("02:13:34", session.Duration?.ToString(HeaderSession.MxStdFrmtTimeSpan));
            Assert.Equal("02:01:04", session.TypingTime?.ToString(HeaderSession.MxStdFrmtTimeSpan));

            Assert.Equal(data, session.ToString());
        }

        [Fact]
        public void FiveSessionPauseTest()     //HeaderSession.PauseRecordsPerLine is 5
        {
            var data = "Session: 4 Start: 23-06-19 12:10:00 Duration: 02:13:34 Start line: 210 End line: 350 Words: 3501 Corrections count: 970 Spell check count: 255 Typing pauses: 12:30:45,100;12:30:45,100;12:30:45,100;12:30:45,100;12:30:45,350;[end]";
            var session = new HeaderSession();
            Assert.True(session.InitialiseFromString(data).GetResult());
            Assert.False(session.IsError());
            Assert.Equal(4, session.SessionNo);
            Assert.Equal("23-06-19 12:10:00", session.Start?.ToString(MxStdFrmt.DateTime));
            Assert.Equal("02:13:34", session.Duration?.ToString(HeaderSession.MxStdFrmtTimeSpan));
            Assert.Equal("02:01:04", session.TypingTime?.ToString(HeaderSession.MxStdFrmtTimeSpan));

            Assert.Equal(data, session.ToString());
        }

        [Fact]
        public void SixSessionPauseTest()     //HeaderSession.PauseRecordsPerLine is 5
        {
            var data = $"Session: 4 Start: 23-06-19 12:10:00 Duration: 02:13:34 Start line: 210 End line: 350 Words: 3501 Corrections count: 970 Spell check count: 255 Typing pauses: 12:30:45,100;12:30:45,100;12:30:45,100;12:30:45,100;12:30:45,100;";
            data += $"{ Environment.NewLine}12:30:45,250;[end]";

            var session = new HeaderSession();
            Assert.True(session.InitialiseFromString(data).GetResult());
            Assert.False(session.IsError());
            Assert.Equal(4, session.SessionNo);
            Assert.Equal("23-06-19 12:10:00", session.Start?.ToString(MxStdFrmt.DateTime));
            Assert.Equal("02:13:34", session.Duration?.ToString(HeaderSession.MxStdFrmtTimeSpan));
            Assert.Equal("02:01:04", session.TypingTime?.ToString(HeaderSession.MxStdFrmtTimeSpan));

            Assert.Equal(data.Replace(Environment.NewLine, ""), session.ToString());
        }

        [Fact]
        public void SetLineLinesTypedZeroTest()
        {
            var session = new HeaderSession();
            Assert.True(session.SetStartLineLinesTyped(1));
            Assert.True(session.SetEndLineLinesTyped(1));
            Assert.Equal(0, session.LinesTyped);
        }

        [Fact]
        public void SetLineLinesTypedOneTest()
        {
            var session = new HeaderSession();
            Assert.True(session.SetStartLineLinesTyped(1));
            Assert.True(session.SetEndLineLinesTyped(2));
            Assert.Equal(1, session.LinesTyped);
        }

        [Fact]
        public void SetLineLinesTypedInvalidTest()
        {
            var session = new HeaderSession();
            Assert.True(session.SetStartLineLinesTyped(5));
            Assert.False(session.SetEndLineLinesTyped(4));
            Assert.Equal(Program.PosIntegerNotSet, session.LinesTyped);
        }

        [Fact]
        public void SetLineLinesTypedInvalidInvTest()
        {
            var session = new HeaderSession();
            Assert.True(session.SetEndLineLinesTyped(4));
            Assert.False(session.SetStartLineLinesTyped(5));

            Assert.Equal(Program.PosIntegerNotSet, session.LinesTyped);
        }

        [Fact]
        public void GetAssessmentTest()
        {
            var session = new HeaderSession();

            Assert.Equal("Great progress - keep on writing!", session.GetAssessment());
        }
    }
}
