using System;
using KLineEdCmdApp.Utils;

namespace KLineEdCmdAppTest.TestSupport
{
    public class MockTerminal : ITerminal
    {
        private int _cursorLine;
        private int _cursorColumn;

        public MockTerminal()
        {
            _cursorLine = 0;
            _cursorColumn = 0;
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
            _cursorColumn = column;
            _cursorLine = line;
            return true;
        }

        public int GetCursorColumn()
        {
            return _cursorColumn;
        }

        public int GetCursorLine()
        {
            return _cursorLine;
        }
        public string WriteLine(string line, params object[] args)
        {
            string rc = null;

            rc = string.Format(line, args);

            return rc;
        }

        public string Write(string msg, params object[] args)
        {
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
