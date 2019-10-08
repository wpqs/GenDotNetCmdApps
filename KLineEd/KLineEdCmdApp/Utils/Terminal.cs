using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using MxReturnCode;

namespace KLineEdCmdApp.Utils
{
    [SuppressMessage("ReSharper", "SimplifyConditionalTernaryExpression")]
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    [SuppressMessage("ReSharper", "RedundantTernaryExpression")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public class Terminal : ITerminal
    {
        public static readonly ConsoleKey InvalidKey = ConsoleKey.F24;
        public static readonly int StdCursorSize = TerminalProperties.DefaultCursorSize;
        public static readonly int InsertModeCursorSize = 100;

        private readonly MxReturnCode<bool> _mxErrorCode;
        private int CursorSize { set; get; }

        public Terminal()
        {
            _mxErrorCode = new MxReturnCode<bool>($"Terminal.Ctor", false); //SetResult(true) on error
            _mxErrorCode.SetError(1210101, MxError.Source.Program, "Terminal.Setup not called");
            CursorSize = TerminalProperties.DefaultCursorSize;
        }

        public bool IsError() { return (_mxErrorCode?.GetResult() ?? false) ? false : true; }
        public MxError.Source GetErrorSource() { return _mxErrorCode?.GetErrorType() ?? MxError.Source.Program; }
        public int GetErrorNo() { return _mxErrorCode?.GetErrorCode() ?? Program.PosIntegerNotSet; }
        public string GetErrorTechMsg() { return _mxErrorCode?.GetErrorTechMsg() ?? Program.ValueNotSet; }
        public string GetErrorUserMsg() { return _mxErrorCode?.GetErrorUserMsg() ?? Program.ValueNotSet; }

        public MxReturnCode<MxReturnCode<bool>> GetMxError()
        {
            var rc = new MxReturnCode<MxReturnCode<bool>>($"Terminal.GetMxError");

            rc += _mxErrorCode;

            return rc;
        }
        public MxReturnCode<bool> Close()
        {
            var rc = new MxReturnCode<bool>($"Terminal.Close");

            if (_mxErrorCode.IsError())
                rc += _mxErrorCode;

            return rc;
        }

        public bool Setup(TerminalProperties props)
        {
            if ((props == null) || (props.IsError()))
                _mxErrorCode.SetError(1210201, MxError.Source.Data, props?.GetValidationError() ?? Program.ValueNotSet, MxMsgs.MxErrInvalidSettingsFile);
            else
            {
                try
                {
                    Console.Title = props.Title;

                    Console.SetWindowSize(props.WindowWidth, props.WindowHeight); //must set window before buffer
                    Console.SetBufferSize(props.BufferWidth, props.BufferHeight);

                    CursorSize = props.CursorSize;
                    Console.CursorSize = props.CursorSize;
                    Console.WindowTop = props.WindowTop;
                    Console.WindowLeft = props.WindowLeft;
                    Console.CursorTop = props.CursorTop;
                    Console.CursorLeft = props.CursorLeft;
                    Console.ForegroundColor = MxConsole.GetConsoleColor(props.ForegroundColor);
                    Console.BackgroundColor = MxConsole.GetConsoleColor(props.BackgroundColor);
                    Console.TreatControlCAsInput = props.TreateCtrlCAsInput;

                    if (Clear() == false)
                        _mxErrorCode.SetError(1210202, MxError.Source.Sys, "Terminal.Clear() failed", MxMsgs.MxErrSystemFailure);
                    else
                    {
                        _mxErrorCode.SetResult(true);
                    }
                }
                catch (Exception e)
                {
                    _mxErrorCode.SetError(1210203, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return (IsError()) ? false : true;
        }

        public TerminalProperties GetSettings()
        {
            TerminalProperties rc = null;

            var props = new TerminalProperties
            {
                Title = Console.Title,
                BufferWidth = Console.BufferWidth,
                BufferHeight = Console.BufferHeight,
                WindowWidth = Console.WindowWidth,
                WindowHeight = Console.WindowHeight,
                CursorSize = CursorSize,
                WindowTop = Console.WindowTop,
                WindowLeft = Console.WindowLeft,
                CursorTop = Console.CursorTop,
                CursorLeft = Console.CursorLeft,
                ForegroundColor = MxConsole.XlatConsoleColorToMxConsoleColor(Console.ForegroundColor),
                BackgroundColor = MxConsole.XlatConsoleColorToMxConsoleColor(Console.BackgroundColor),
                TreateCtrlCAsInput = Console.TreatControlCAsInput
        };
            if (props.Validate() == false)
                _mxErrorCode.SetError(1210301, MxError.Source.Sys, $"Console's existing props are invalid: {props.GetValidationError()}", MxMsgs.MxErrSystemFailure);
            else
            {
                rc = props;
                _mxErrorCode.SetResult(true);
            }
            return rc;
        }

        public bool IsKeyAvailable()
        {
            return Console.KeyAvailable;
        }

        public bool SetColour(MxConsole.Color foreGndColour, MxConsole.Color backGndColour)
        {
            try
            {
                Console.ForegroundColor = MxConsole.GetConsoleColor(foreGndColour);
                Console.BackgroundColor = MxConsole.GetConsoleColor(backGndColour);
                _mxErrorCode.SetResult(true);
            }
            catch (Exception e)
            {
                _mxErrorCode.SetError(1210401, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            return (IsError()) ? false : true;
        }
        public bool SetCursorPosition(int row, int column)
        {
            if ((row < 0) || (row >= Console.BufferHeight) || (column < 0) || (column >= Console.BufferWidth))
                _mxErrorCode.SetError(1210402, MxError.Source.Param, $"Invalid cursor position: line={row}, column={column}", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    Console.CursorLeft = column;
                    Console.CursorTop = row;
                    _mxErrorCode.SetResult(true);
                }
                catch (Exception e)
                {
                    _mxErrorCode.SetError(1210403, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return (IsError()) ? false : true;
        }

        public int GetCursorColumn()
        {
            var rc = Program.PosIntegerNotSet;
            try
            {
                rc = Console.CursorLeft;
                _mxErrorCode.SetResult(true);
            }
            catch (Exception e)
            {
                _mxErrorCode.SetError(1210501, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            return rc;
        }
        public int GetCursorRow()
        {
            var rc = Program.PosIntegerNotSet;
            try
            {
                rc = Console.CursorTop;
                _mxErrorCode.SetResult(true);
            }
            catch (Exception e)
            {
                _mxErrorCode.SetError(1210601, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            return rc;
        }

        public void SetCursorVisible(bool show=true)
        {
            try
            {
                Console.CursorVisible = show;
            }
            catch (Exception e)
            {
                _mxErrorCode.SetError(1210701, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
        }

        public void SetCursorSize(int size)
        {
            try
            {
                if ((size > 0) && (size <= 100))
                {
                    CursorSize = size;
                    Console.CursorSize = CursorSize;
                }
            }
            catch (Exception e)
            {
                _mxErrorCode.SetError(1210802, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
        }

        public void SetCursorInsertMode(bool insertMode = false)
        {
            try
            {
                Console.CursorSize = ((insertMode) ? InsertModeCursorSize : CursorSize);
            }
            catch (Exception e)
            {
                _mxErrorCode.SetError(1210801, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
        }

        public bool Clear()
        {
            try
            {
                Console.Clear();
                _mxErrorCode.SetResult(true);
            }
            catch (Exception e)
            {
                _mxErrorCode.SetError(1210901, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            return (IsError()) ? false : true;
        }

        public string WriteLine(string line, params object[] args)
        {
            string rc = null;
            if (line == null)
                _mxErrorCode.SetError(1211001, MxError.Source.Param, $"line is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    Console.WriteLine(line, args);
                    rc = string.Format(line, args);
                    _mxErrorCode.SetResult(true);
                }
                catch (Exception e)
                {
                    _mxErrorCode.SetError(1211102, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        public string WriteLines(string[] lines)
        {
            string rc = null;
            if (lines != null)
            {
                foreach (var line in lines)
                    rc = WriteLine(line);
            }
            return rc;
        }

        public string Write(string msg, params object[] args)
        {
            string rc = null;
            if (msg == null)
                _mxErrorCode.SetError(1211201, MxError.Source.Param, $"msg is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    Console.Write(msg, args);
                    rc = string.Format(msg, args);
                    _mxErrorCode.SetResult(true);
                }
                catch (Exception e)
                {
                    _mxErrorCode.SetError(1211202, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        public char GetKeyChar(bool hide = false, char defaultVal = Body.SpaceChar)
        {
            var rc = (char) 0;
            try
            {
                rc = Console.ReadKey(hide).KeyChar;       //defaultVal is helpful in testing
                _mxErrorCode.SetResult(true);
            }
            catch (Exception e)
            {
                _mxErrorCode.SetError(1211401, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            return rc;
        }

        public ConsoleKey GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape)
        {
            var rc =Terminal.InvalidKey;
            try
            {
                rc = Console.ReadKey(hide).Key; //defaultVal is helpful in testing
                _mxErrorCode.SetResult(true);
            }
            catch (Exception e)
            {
                _mxErrorCode.SetError(1211501, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            return rc;
        }

        public ConsoleKeyInfo ReadKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape)
        {
            var rc = new ConsoleKeyInfo('\0', ConsoleKey.Clear, false, false, false);
            try
            {
                rc = Console.ReadKey(hide);
                _mxErrorCode.SetResult(true);
            }
            catch (Exception e)
            {
                _mxErrorCode.SetError(1211601, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            return rc;
        }
    }
}
