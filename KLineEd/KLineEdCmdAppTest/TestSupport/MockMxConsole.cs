using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Utils;
using MxReturnCode;

namespace KLineEdCmdAppTest.TestSupport
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "SimplifyConditionalTernaryExpression")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public class MockMxConsole : IMxConsole
    {
        public int CursorRow { get; private set; }
        public int CursorColumn { get; private set; }

        public MxConsole.Color ForeGndColour { get; private set; }
        public MxConsole.Color BackGndColour { get; private set; }

        public MockMxConsole()
        {
            CursorRow = 0;
            CursorColumn = 0;
            ForeGndColour = MxConsole.Color.Gray;
            BackGndColour = MxConsole.Color.Black;
        }

        public bool IsKeyAvailable() { return true; }
        public bool IsWindowSizeChanged(int expectedWidth, int expectedHeight) { return false; }

        public MxReturnCode<bool> Close()
        {
            var rc = new MxReturnCode<bool>($"MxConsole.Close");
            return rc;
        }

        public MxReturnCode<bool> Clear(bool force = false)
        {
            var rc = new MxReturnCode<bool>($"MockMxConsole.Clear");
            rc.SetResult(true);
            return rc;
        }

        public MxReturnCode<bool> ApplySettings(MxConsoleProperties props, bool restore=false)
        {
            var rc = new MxReturnCode<bool>($"MockMxConsole.ApplySettings");
            rc.SetResult(true);
            return rc;
        }

        public MxReturnCode<MxConsoleProperties> GetSettings()
        {
            var rc = new MxReturnCode<MxConsoleProperties>($"MockMxConsole.GetSettings");
            rc.SetResult(new MxConsoleProperties());
            return rc;
        }

        public MxReturnCode<bool> SetColour(MxConsole.Color foreGndColour, MxConsole.Color msgLineErrorBackGndColour)
        {
            var rc = new MxReturnCode<bool>($"MockMxConsole.SetColour");

            ForeGndColour = foreGndColour;
            BackGndColour = msgLineErrorBackGndColour;

            rc.SetResult(true);
            return rc;
        }

        public MxReturnCode<bool> SetCursorPosition(int row, int column)
        {
            var rc = new MxReturnCode<bool>($"MockMxConsole.SetCursorPosition");

            CursorColumn = column;
            CursorRow = row;

            rc.SetResult(true);
            return rc;
        }

        public MxReturnCode<int> GetCursorColumn()
        {
            var rc = new MxReturnCode<int>($"MockMxConsole.GetCursorColumn");
            rc.SetResult(CursorColumn);
            return rc;
        }


        public MxReturnCode<int> GetCursorRow()
        {
            var rc = new MxReturnCode<int>($"MockMxConsole.GetCursorRow");
            rc.SetResult(CursorRow);
            return rc;
        }

        public MxReturnCode<bool> SetCursorVisible(bool hide = false)
        {
            var rc = new MxReturnCode<bool>($"MockMxConsole.SetCursorVisible");
            rc.SetResult(true);
            return rc;
        }

        public MxReturnCode<bool> SetCursorSize(int size)
        {
            var rc = new MxReturnCode<bool>($"MockMxConsole.SetCursorSize");
            rc.SetResult(true);
            return rc;
        }

        public MxReturnCode<bool> SetCursorInsertMode(bool insertMode = false)
        {
            var rc = new MxReturnCode<bool>($"MockMxConsole.SetCursorInsertMode");
            rc.SetResult(true);
            return rc;
        }


        public MxReturnCode<string> WriteLine(string line, params object[] args)
        {
            var rc = new MxReturnCode<string>($"MockMxConsole.WriteLine");
            rc.SetResult(string.Format(line, args));
            return rc;
        }

        public MxReturnCode<bool> WriteLines(string[] lines)
        {
            var rc = new MxReturnCode<bool>($"MockMxConsole.WriteLines");
            foreach (var line in lines)
            {
                var rcLine = WriteLine(line);
                if (rcLine.IsError(true))
                {
                    rc += rcLine;
                    break;
                }
            }
            if (rc.IsError())
                rc.SetResult(true);
            return rc;
        }

        public MxReturnCode<string> WriteString(string msg, params object[] args)
        {
            var rc = new MxReturnCode<string>($"MockMxConsole.WriteString");
            rc.SetResult(string.Format(msg, args));
            return rc;
        }

        public MxReturnCode<char> GetKeyChar(bool hide = false, char defaultVal = ' ')
        {
            var rc = new MxReturnCode<char>($"MockMxConsole.GetKeyChar");
            rc.SetResult(defaultVal);
            return rc;
        }

        public MxReturnCode<ConsoleKey> GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape)
        {
            var rc = new MxReturnCode<ConsoleKey>($"MockMxConsole.GetKey");
            rc.SetResult(defaultVal);
            return rc;
        }

        public MxReturnCode<ConsoleKeyInfo> GetKeyInfo(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape)
        {
            var rc = new MxReturnCode<ConsoleKeyInfo>($"MockMxConsole.GetKeyInfo");
            rc.SetResult(new ConsoleKeyInfo('X', defaultVal, false, false, true));
            return rc;
        }
    }
}
