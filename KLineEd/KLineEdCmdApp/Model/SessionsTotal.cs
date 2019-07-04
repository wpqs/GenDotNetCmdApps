using System;
using System.Collections.Generic;

namespace KLineEdCmdApp.Model
{
    public static class SessionsTotal
    {
        public static string Error { get; private set; }
        public static TimeSpan? TotalDuration { get; private set; }
        public static TimeSpan? MeanDuration { get; private set; }
        public static TimeSpan? TotalTypingTime { get; private set; }
        public static TimeSpan? MeanTypingTime { get; private set; }
        public static TimeSpan? HoursWorkedPerDay { get; private set; }

        public static int TotalDaysWorked { get; private set; }
        public static int TotalLinesTyped { get; private set; }
        public static double MeanLinesTyped { get; private set; }
        public static int TotalWordsTyped { get; private set; }
        public static double MeanWpm { get; private set; }
        public static int TotalCorrectionsCount { get; private set; }
        public static double MeanWpc { get; private set; }
        public static int TotalSpellCheckCount { get; private set; }
        public static double MeanWps { get; private set; }
        public static int TotalTypingPauseCount { get; private set; }
        public static double MeanTypingPauseCount { get; private set; }
        public static int MeanTypingPauseMinutes { get; private set; }

        public static bool RefreshValues(List<HeaderSession> sessions)
        {
            var rc = false;

            Error = HeaderBase.ValueNotSet;

            if (sessions == null)
                Error = "sessions is null";
            else
            {
                try
                {
                    var sessionCnt = sessions.Count;
                    if (sessionCnt <= 0)
                        Error = "sessions count is 0";
                    else
                    {
                        TotalDaysWorked = 0;
                        TotalTypingPauseCount = 0;
                        TotalLinesTyped = 0;
                        TotalWordsTyped = 0;
                        TotalCorrectionsCount = 0;
                        TotalSpellCheckCount = 0;

                        TotalDuration = new TimeSpan(0, 0, 0);
                        TotalTypingTime = new TimeSpan(0, 0, 0);

                        DateTime? lastStart = new DateTime(1970,1,1,0,0,0);
                        foreach (var session in sessions)
                        {
                            var diff = session.Start - lastStart;
                            if (diff?.TotalDays >= 1.0)
                            {
                                TotalDaysWorked++;
                                lastStart = session.Start;
                            }
                            TotalDuration += session.Duration;
                            TotalTypingTime += session.TypingTime;
                            TotalTypingPauseCount += session.TypingPauseCount;
                            TotalLinesTyped += session.LinesTyped;
                            TotalCorrectionsCount += session.CorrectionsCount;
                            TotalSpellCheckCount += session.SpellCheckCount;
                            TotalWordsTyped += session.WordsCount;
                        }

                        HoursWorkedPerDay = TotalDaysWorked <= 0 ? new TimeSpan(0,0,0,0) : TotalDuration?.Divide(TotalDaysWorked);
                        MeanDuration = TotalDuration?.Divide(sessionCnt);
                        MeanTypingTime = TotalTypingTime?.Divide(sessionCnt);

                        MeanLinesTyped = TotalLinesTyped / ((double) sessionCnt);
                        MeanTypingPauseCount = TotalTypingPauseCount / (double)sessionCnt;

                        var pause = TotalDuration - TotalTypingTime;
                        MeanTypingPauseMinutes = ((int)(pause?.TotalMinutes ?? 0.0)) / sessionCnt;

                        MeanWpm = (TotalDuration?.TotalMinutes ?? 0.0) <= 0.0 ? TotalWordsTyped : (double) TotalWordsTyped / TotalDuration?.TotalMinutes ?? 0.0;
                        MeanWpc = TotalCorrectionsCount <= 0 ? 0.0 : (double) TotalWordsTyped / TotalCorrectionsCount;
                        MeanWps = TotalSpellCheckCount <= 0 ? 0.0 : (double) TotalWordsTyped / TotalSpellCheckCount;

                        rc = true;
                    }
                }
                catch (Exception e)
                {
                    Error = e.Message;
                }
            }
            return rc;
        }

        public static string GetReport(List<HeaderSession> sessions, int linesInChapter, int wordsInChapter)
        {
            string rc = $"Chapter stats:{Environment.NewLine}";

            if (RefreshValues(sessions) == false)
                rc += Error;
            else
            {
                var pages = linesInChapter / Body.TextLinesPerPage;
                rc += Environment.NewLine;
                rc += $"Pages {pages}, lines {linesInChapter} (lines typed {TotalLinesTyped}, mean {MeanLinesTyped}), words {wordsInChapter} (words typed {TotalWordsTyped}, mean WPM {MeanWpm})"; // , total corrections {TotalCorrectionsCount} mean per session {MeanWpc}, total spelling errors {TotalSpellCheckCount}, mean per session {MeanWps})
                rc += Environment.NewLine;
                rc += $"Days worked {TotalDaysWorked} (mean hours worked {(HoursWorkedPerDay?.TotalHours)})";
                rc += Environment.NewLine;
                rc += $"Editing time {TotalDuration?.TotalDays} days, {TotalDuration?.ToString(HeaderSession.MxStdFrmtTimeSpan)} (mean per session {MeanDuration?.ToString(HeaderSession.MxStdFrmtTimeSpan)})";
                rc += Environment.NewLine;
                rc += $"Typing time {TotalTypingTime?.TotalDays} days, {TotalTypingTime?.ToString(HeaderSession.MxStdFrmtTimeSpan)} (mean per session {MeanTypingTime?.ToString(HeaderSession.MxStdFrmtTimeSpan)})";
                rc += Environment.NewLine;
                rc += $"Number of pauses {TotalTypingPauseCount} (mean count {MeanTypingPauseCount}, duration {MeanTypingPauseMinutes} minutes)";
            }
            rc += $"{KLineEditor.ReportSectionDottedLine}";

            return rc;
        }
    }

}
