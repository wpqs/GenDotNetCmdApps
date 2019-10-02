﻿using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp;
using KLineEdCmdApp.Utils;
using MxReturnCode;

namespace KLineEdCmdAppTest.TestSupport
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "SimplifyConditionalTernaryExpression")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public class MockTerminal : ITerminal
    {
        private readonly MxReturnCode<bool> _mxErrorCode;
        public int CursorRow { get; private set; }
        public int CursorColumn { get; private set; }

        public MxConsole.Color ForeGndColour { get; private set; }
        public MxConsole.Color BackGndColour { get; private set; }

        public MockTerminal()
        {
            _mxErrorCode = new MxReturnCode<bool>($"MockTerminal.Ctor", false); //SetResult(true) on error
            _mxErrorCode.SetError(9210201, MxError.Source.Program, "Terminal.Setup not called");
            CursorRow = 0;
            CursorColumn = 0;
            ForeGndColour = MxConsole.Color.Gray;
            BackGndColour = MxConsole.Color.Black;
        }

        public MxReturnCode<MxReturnCode<bool>> GetMxError()
        {
            var rc = new MxReturnCode<MxReturnCode<bool>>($"MockTerminal.GetMxError");

            rc += _mxErrorCode;

            return rc;
        }
        public bool IsError() { return (_mxErrorCode?.GetResult() ?? false) ? false : true; }
        public MxError.Source GetErrorSource() { return _mxErrorCode?.GetErrorType() ?? MxError.Source.Program; }
        public int GetErrorNo() { return _mxErrorCode?.GetErrorCode() ?? Program.PosIntegerNotSet; }
        public string GetErrorTechMsg() { return _mxErrorCode?.GetErrorTechMsg() ?? Program.ValueNotSet; }
        public string GetErrorUserMsg() { return _mxErrorCode?.GetErrorUserMsg() ?? Program.ValueNotSet; }

        public bool Clear()
        {
            return true;
        }

        public bool Setup(TerminalProperties props)
        {
            _mxErrorCode.SetResult(true);
            return true;
        }

        public MxReturnCode<bool> Close()
        {
            var rc = new MxReturnCode<bool>($"Terminal.Close");

            if (_mxErrorCode.IsError())
                rc += _mxErrorCode;

            return rc;
        }

        public TerminalProperties GetSettings()
        {
            _mxErrorCode.SetResult(true);
            return new TerminalProperties();
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
