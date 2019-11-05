using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp;
using KLineEdCmdApp.Utils;
using MxReturnCode;

namespace KLineEdCmdAppTest.TestSupport
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "SimplifyConditionalTernaryExpression")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public class MockMxConsole : IMxConsole
    {
        private MxReturnCode<bool> _mxErrorState;
        public bool IsErrorState() { return (_mxErrorState?.IsError(true) ?? false) ? true : false; }
        public MxReturnCode<bool> GetErrorState() { return _mxErrorState ?? null; }
        public void ResetErrorState() { _mxErrorState = null; }
        public bool SetErrorState(MxReturnCode<bool> mxErr)
        {
            var rc = false;

            if (_mxErrorState == null)
            {
                _mxErrorState = mxErr;
                rc = true;
            }
            return rc;
        }
        public int CursorRow { get; private set; }
        public int CursorColumn { get; private set; }

        public MxConsole.Color ForeGndColour { get; private set; }
        public MxConsole.Color BackGndColour { get; private set; }

        public MockMxConsole()
        {
            _mxErrorState = null;
            CursorRow = 0;
            CursorColumn = 0;
            ForeGndColour = MxConsole.Color.Gray;
            BackGndColour = MxConsole.Color.Black;
        }

        public bool Clear(bool force = false)
        {
            return true;
        }

        public bool IsWindowSizeChanged(int expectedWidth, int expectedHeight)
        {
            return false;
        }

        public bool ApplySettings(MxConsoleProperties props, bool restore=false)
        {
            return true;
        }

        public MxReturnCode<bool> Close()
        {
            var rc = new MxReturnCode<bool>($"MxConsole.Close");

            return rc;
        }

        public MxConsoleProperties GetSettings()
        {
            return new MxConsoleProperties();
        }

        public bool SetCursorPosition(int row, int column)
        {
            CursorColumn = column;
            CursorRow = row;
            return true;
        }

        public int GetCursorColumn()
        {
            return CursorColumn;
        }

        public int GetCursorRow()
        {
            return CursorRow;
        }

        public void SetCursorVisible(bool hide=false)
        {

        }

        public void SetCursorInsertMode(bool insertMode = false)
        {

        }

        public bool IsKeyAvailable()
        {
            return true;
        }

        public bool SetColour(MxConsole.Color foreGndColour, MxConsole.Color msgLineErrorBackGndColour)
        {
            ForeGndColour = foreGndColour;
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

        public string WriteLines(string[] lines)
        {
            string rc = null;
            foreach (var line in lines)
                rc = WriteLine(line);
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

        public ConsoleKeyInfo ReadKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape)
        {
            return new ConsoleKeyInfo('X', defaultVal, false, false, true);
        }
    }
}
