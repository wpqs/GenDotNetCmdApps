using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Utils;

namespace KLineEdCmdAppTest.TestSupport
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public class MockTerminal : ITerminal
    {
        public int CursorLine { get; private set; }
        public int CursorColumn { get; private set; }

        public ConsoleColor ForeGndColour { get; private set; }
        public ConsoleColor BackGndColour { get; private set; }

        public MockTerminal()
        {
            CursorLine = 0;
            CursorColumn = 0;
            ForeGndColour = ConsoleColor.Gray;
            BackGndColour = ConsoleColor.Black;
            ErrorMsg = null;
        }

        public bool IsError() { return (ErrorMsg != null); }
        public string ErrorMsg { get; }

        public bool Clear()
        {
            return true;
        }

        public bool Setup(TerminalProperties props)
        {
            return true;
        }

        public TerminalProperties GetSettings()
        {
            return new TerminalProperties();
        }

        public bool SetCursorPosition(int line, int column)
        {
            CursorColumn = column;
            CursorLine = line;
            return true;
        }

        public int GetCursorColumn()
        {
            return CursorColumn;
        }

        public int GetCursorLine()
        {
            return CursorLine;
        }

        public bool SetColour(ConsoleColor msgLineErrorForeGndColour, ConsoleColor msgLineErrorBackGndColour)
        {
            ForeGndColour = msgLineErrorForeGndColour;
            BackGndColour = msgLineErrorBackGndColour;
            return true;
        }

        public string WriteLine(string line, params object[] args)
        {
            // ReSharper disable once RedundantAssignment
            string rc = null;

            rc = string.Format(line, args);

            return rc;
        }

        public string Write(string msg, params object[] args)
        {
            // ReSharper disable once RedundantAssignment
            string rc = null;

            rc = string.Format(msg, args);

            return rc;

        }
        public char GetKeyChar(bool hide = false, char defaultVal = ' ')
        {
            return defaultVal;
        }

        public ConsoleKey GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape)
        {
            return defaultVal;
        }

        public ConsoleKeyInfo ReadKey()
        {
            return new ConsoleKeyInfo('X', ConsoleKey.X, false, false, true);
        }
    }
}
