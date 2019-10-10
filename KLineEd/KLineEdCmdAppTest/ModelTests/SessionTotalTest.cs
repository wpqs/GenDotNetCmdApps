using System;
using System.Collections.Generic;
using KLineEdCmdApp.Model;
using Xunit;

namespace KLineEdCmdAppTest.ModelTests
{
    public class SessionTotalTest
    {
        [Fact]
        public void NullTest()
        {
            Assert.False(SessionsTotal.RefreshValues(null));
        }
        [Fact]
        public void NoSessionTest()
        {
            var list = new List<HeaderSession>();
            Assert.False(SessionsTotal.RefreshValues(list));
        }
        [Fact]
        public void OneSessionData1Test()
        {
            const string data1 = "Session: 4 Start: 23-06-19 12:10:00 Duration: 02:13:34 Start line: 210 End line: 350 Words: 3501 Corrections count: 970 Spell check count: 255 Typing pauses: 12:30:45,120;14:30:01,630;[end]";
            var session = new HeaderSession();
            Assert.True(session.InitialiseFromString(data1).GetResult());
            Assert.False(session.IsError());

            var list = new List<HeaderSession> {session};
            Assert.True(SessionsTotal.RefreshValues(list));

            Assert.Equal("02:13:34", SessionsTotal.TotalDuration?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("02:13:34", SessionsTotal.MeanDuration?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("02:01:04", SessionsTotal.TotalTypingTime?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("02:01:04", SessionsTotal.MeanTypingTime?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("02:13:34", SessionsTotal.HoursWorkedPerDay?.ToString(Header.MxStdFrmtTimeSpan));

            Assert.Equal(1, SessionsTotal.TotalDaysWorked);
            Assert.Equal(140, SessionsTotal.TotalLinesTyped);
            Assert.Equal(140.0, SessionsTotal.MeanLinesTyped, 1);
            Assert.Equal(3501, SessionsTotal.TotalWordsTyped);
            Assert.Equal(26.2, SessionsTotal.MeanWpm, 1);
            Assert.Equal(970, SessionsTotal.TotalCorrectionsCount);
            Assert.Equal(3.6, SessionsTotal.MeanWpc, 1);
            Assert.Equal(255, SessionsTotal.TotalSpellCheckCount);
            Assert.Equal(13.7, SessionsTotal.MeanWps, 1);
            Assert.Equal(2, SessionsTotal.TotalTypingPauseCount);
            Assert.Equal(2.0, SessionsTotal.MeanTypingPauseCount, 1);
            Assert.Equal(12, SessionsTotal.MeanTypingPauseMinutes);
        }
        [Fact]
        public void OneSessionData2Test()
        {
            //different day to Session 4
            const string data2 = "Session: 5 Start: 24-06-19 13:10:00 Duration: 04:30:30 Start line: 350 End line: 450 Words: 2000 Corrections count: 600 Spell check count: 150 Typing pauses: 23:30:00,7200;[end]";
            var session2 = new HeaderSession();
            Assert.True(session2.InitialiseFromString(data2).GetResult());
            Assert.False(session2.IsError());

            var list = new List<HeaderSession> {session2};
            Assert.True(SessionsTotal.RefreshValues(list));

            Assert.Equal("04:30:30", SessionsTotal.TotalDuration?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("04:30:30", SessionsTotal.MeanDuration?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("02:30:30", SessionsTotal.TotalTypingTime?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("02:30:30", SessionsTotal.MeanTypingTime?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("04:30:30", SessionsTotal.HoursWorkedPerDay?.ToString(Header.MxStdFrmtTimeSpan));

            Assert.Equal(1, SessionsTotal.TotalDaysWorked);
            Assert.Equal(100, SessionsTotal.TotalLinesTyped);
            Assert.Equal(100.0, SessionsTotal.MeanLinesTyped, 1);
            Assert.Equal(2000, SessionsTotal.TotalWordsTyped);
            Assert.Equal(7.4, SessionsTotal.MeanWpm, 1);
            Assert.Equal(600, SessionsTotal.TotalCorrectionsCount);
            Assert.Equal(3.3, SessionsTotal.MeanWpc, 1);
            Assert.Equal(150, SessionsTotal.TotalSpellCheckCount);
            Assert.Equal(13.3, SessionsTotal.MeanWps, 1);
            Assert.Equal(1, SessionsTotal.TotalTypingPauseCount);
            Assert.Equal(1.0, SessionsTotal.MeanTypingPauseCount);
            Assert.Equal(120, SessionsTotal.MeanTypingPauseMinutes);
        }

        [Fact]
        public void OneSessionData3Test()
        {
            //same day as Session 5
            const string data3 = "Session: 6 Start: 24-06-19 23:10:00 Duration: 06:00:00 Start line: 450 End line: 625 Words: 1500 Corrections count: 570 Spell check count: 377 Typing pauses: 23:30:00,3600;23:30:00,1800;23:30:00,900;[end]";

            var session2 = new HeaderSession();
            Assert.True(session2.InitialiseFromString(data3).GetResult());
            Assert.False(session2.IsError());

            var list = new List<HeaderSession> {session2};
            Assert.True(SessionsTotal.RefreshValues(list));

            Assert.Equal("06:00:00", SessionsTotal.TotalDuration?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("06:00:00", SessionsTotal.MeanDuration?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("04:15:00", SessionsTotal.TotalTypingTime?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("04:15:00", SessionsTotal.MeanTypingTime?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("06:00:00", SessionsTotal.HoursWorkedPerDay?.ToString(Header.MxStdFrmtTimeSpan));

            Assert.Equal(1, SessionsTotal.TotalDaysWorked);
            Assert.Equal(175, SessionsTotal.TotalLinesTyped);
            Assert.Equal(175.0, SessionsTotal.MeanLinesTyped, 1);
            Assert.Equal(1500, SessionsTotal.TotalWordsTyped);
            Assert.Equal(4.2, SessionsTotal.MeanWpm, 1);
            Assert.Equal(570, SessionsTotal.TotalCorrectionsCount);
            Assert.Equal(2.6, SessionsTotal.MeanWpc, 1);
            Assert.Equal(377, SessionsTotal.TotalSpellCheckCount);
            Assert.Equal(4.0, SessionsTotal.MeanWps, 1);
            Assert.Equal(3, SessionsTotal.TotalTypingPauseCount);
            Assert.Equal(3.0, SessionsTotal.MeanTypingPauseCount);
            Assert.Equal(105, SessionsTotal.MeanTypingPauseMinutes);
        }

        [Fact]
        public void TwoSessionTest()
        {
            const string data1 = "Session: 4 Start: 23-06-19 12:10:00 Duration: 02:13:34 Start line: 210 End line: 350 Words: 3501 Corrections count: 970 Spell check count: 255 Typing pauses: 12:30:45,120;14:30:01,630;[end]";
            var session1 = new HeaderSession();
            Assert.True(session1.InitialiseFromString(data1).GetResult());
            Assert.False(session1.IsError());
            //different date to session 4
            const string data2 = "Session: 5 Start: 24-06-19 13:10:00 Duration: 04:30:30 Start line: 350 End line: 450 Words: 2000 Corrections count: 600 Spell check count: 150 Typing pauses: 23:30:00,7200;[end]";
            var session2 = new HeaderSession();
            Assert.True(session2.InitialiseFromString(data2).GetResult());
            Assert.False(session2.IsError());

            var list = new List<HeaderSession> {session1, session2};
            Assert.True(SessionsTotal.RefreshValues(list));

            Assert.Equal("06:44:04", SessionsTotal.TotalDuration?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("03:22:02", SessionsTotal.MeanDuration?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("04:31:34", SessionsTotal.TotalTypingTime?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("02:15:47", SessionsTotal.MeanTypingTime?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("03:22:02", SessionsTotal.HoursWorkedPerDay?.ToString(Header.MxStdFrmtTimeSpan));

            Assert.Equal(2, SessionsTotal.TotalDaysWorked);
            Assert.Equal(240, SessionsTotal.TotalLinesTyped);
            Assert.Equal(120.0, SessionsTotal.MeanLinesTyped, 1);
            Assert.Equal(5501, SessionsTotal.TotalWordsTyped);
            Assert.Equal(13.6, SessionsTotal.MeanWpm, 1);
            Assert.Equal(1570, SessionsTotal.TotalCorrectionsCount);
            Assert.Equal(3.5, SessionsTotal.MeanWpc, 1);
            Assert.Equal(405, SessionsTotal.TotalSpellCheckCount);
            Assert.Equal(13.6, SessionsTotal.MeanWps, 1);
            Assert.Equal(3, SessionsTotal.TotalTypingPauseCount);
            Assert.Equal(1.5, SessionsTotal.MeanTypingPauseCount);
            Assert.Equal(66, SessionsTotal.MeanTypingPauseMinutes);
        }

        [Fact]
        public void ThreeSessionTest()
        {
            const string data1 = "Session: 4 Start: 23-06-19 12:10:00 Duration: 02:13:34 Start line: 210 End line: 350 Words: 3501 Corrections count: 970 Spell check count: 255 Typing pauses: 12:30:45,120;14:30:01,630;[end]";
            var session1 = new HeaderSession();
            Assert.True(session1.InitialiseFromString(data1).GetResult());
            Assert.False(session1.IsError());
            //different date to session 4
            const string data2 = "Session: 5 Start: 24-06-19 13:10:00 Duration: 04:30:30 Start line: 350 End line: 450 Words: 2000 Corrections count: 600 Spell check count: 150 Typing pauses: 14:30:00,7200;[end]";
            var session2 = new HeaderSession();
            Assert.True(session2.InitialiseFromString(data2).GetResult());
            Assert.False(session2.IsError());
            //same day as session 5
            const string data3 = "Session: 6 Start: 24-06-19 23:10:00 Duration: 06:00:00 Start line: 450 End line: 625 Words: 1500 Corrections count: 570 Spell check count: 377 Typing pauses: 23:30:00,3600;23:30:00,1800;23:30:00,900;[end]";
            var session3 = new HeaderSession();
            Assert.True(session3.InitialiseFromString(data3).GetResult());
            Assert.False(session3.IsError());

            var list = new List<HeaderSession> {session1, session2, session3 };
            Assert.True(SessionsTotal.RefreshValues(list));

            Assert.Equal("12:44:04", SessionsTotal.TotalDuration?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("04:14:41", SessionsTotal.MeanDuration?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("08:46:34", SessionsTotal.TotalTypingTime?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("02:55:31", SessionsTotal.MeanTypingTime?.ToString(Header.MxStdFrmtTimeSpan));
            Assert.Equal("06:22:02", SessionsTotal.HoursWorkedPerDay?.ToString(Header.MxStdFrmtTimeSpan));

            Assert.Equal(2, SessionsTotal.TotalDaysWorked);
            Assert.Equal(415, SessionsTotal.TotalLinesTyped);
            Assert.Equal(138.3, SessionsTotal.MeanLinesTyped, 1);
            Assert.Equal(7001, SessionsTotal.TotalWordsTyped);
            Assert.Equal(9.2, SessionsTotal.MeanWpm, 1);
            Assert.Equal(2140, SessionsTotal.TotalCorrectionsCount);
            Assert.Equal(3.3, SessionsTotal.MeanWpc, 1);
            Assert.Equal(782, SessionsTotal.TotalSpellCheckCount);
            Assert.Equal(9.0, SessionsTotal.MeanWps, 1);
            Assert.Equal(6, SessionsTotal.TotalTypingPauseCount);
            Assert.Equal(2.0, SessionsTotal.MeanTypingPauseCount);
            Assert.Equal(79, SessionsTotal.MeanTypingPauseMinutes);

        }

        [Fact]
        public void GetReportTest()
        {
            const string data1 = "Session: 4 Start: 23-06-19 12:10:00 Duration: 02:13:34 Start line: 210 End line: 350 Words: 3501 Corrections count: 970 Spell check count: 255 Typing pauses: 12:30:45,120;14:30:01,630;[end]";
            var session1 = new HeaderSession();
            Assert.True(session1.InitialiseFromString(data1).GetResult());
            Assert.False(session1.IsError());
            //different date to session 4
            const string data2 = "Session: 5 Start: 24-06-19 13:10:00 Duration: 04:30:30 Start line: 350 End line: 450 Words: 2000 Corrections count: 600 Spell check count: 150 Typing pauses: 14:30:00,7200;[end]";
            var session2 = new HeaderSession();
            Assert.True(session2.InitialiseFromString(data2).GetResult());
            Assert.False(session2.IsError());
            //same day as session 5
            const string data3 = "Session: 6 Start: 24-06-19 23:10:00 Duration: 06:00:00 Start line: 450 End line: 625 Words: 1500 Corrections count: 570 Spell check count: 377 Typing pauses: 23:30:00,3600;23:30:00,1800;23:30:00,900;[end]";
            var session3 = new HeaderSession();
            Assert.True(session3.InitialiseFromString(data3).GetResult());
            Assert.False(session3.IsError());

            var list = new List<HeaderSession> {session1, session2, session3};
            Assert.StartsWith($"{Environment.NewLine}Chapter stats:{Environment.NewLine}{Environment.NewLine}Pages 9, lines 350 (lines typed 415", SessionsTotal.GetReport(list, 350, 6503, 36));
        }
    }
}