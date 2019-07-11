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
            Error = true;
        }
        private bool Error { set; get; }

        public bool IsError() { return Error; }

        public void Clear()
        {
            
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
        public void WriteLines(string line, params object[] args)
        {
            
        }

        public void Write(string line, params object[] args)
        {
           
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
