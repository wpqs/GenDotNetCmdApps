using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.View.Base;

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
  
        public string ErrorMsg { private set; get; }

        public bool IsError() { return (ErrorMsg ==null) ? false : true; }

        private void SetError(int errorNo, BaseView.ErrorType errType, string errMsg)
        {
            ErrorMsg = BaseView.FormatMxErrorMsg(errorNo, errType, errMsg);
        }

        public Terminal()
        {
            SetError(1210101, BaseView.ErrorType.program, "Terminal not setup");
        }
        public bool Setup(TerminalProperties props)
        {
            if ((props == null) || (props.IsError()))
                SetError(1210201, BaseView.ErrorType.data, props?.GetValidationError() ?? Program.ValueNotSet);
            else
            {
                try
                {
                    Console.Title = props.Title;

                    Console.SetWindowSize(props.WindowWidth, props.WindowHeight); //must set window before buffer
                    Console.SetBufferSize(props.BufferWidth, props.BufferHeight);

                    Console.CursorSize = props.CursorSize;
                    Console.WindowTop = props.WindowTop;
                    Console.WindowLeft = props.WindowLeft;
                    Console.CursorTop = props.CursorTop;
                    Console.CursorLeft = props.CursorLeft;
                    Console.ForegroundColor = props.ForegroundColor;
                    Console.BackgroundColor = props.BackgroundColor;

                    ErrorMsg = null;
                }
                catch (Exception e)
                {
                    SetError(1210202, BaseView.ErrorType.exception, e.Message);
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
                CursorSize = Console.CursorSize,
                WindowTop = Console.WindowTop,
                WindowLeft = Console.WindowLeft,
                CursorTop = Console.CursorTop,
                CursorLeft = Console.CursorLeft,
                ForegroundColor = Console.ForegroundColor,
                BackgroundColor = Console.BackgroundColor
            };
            if (props.Validate() == false)
                SetError(1210301, BaseView.ErrorType.program, props.GetValidationError());
            else
            {
                rc = props;
                ErrorMsg = null;
            }
            return rc;
        }

        public bool IsKeyAvailable()
        {
            return Console.KeyAvailable;
        }

        public bool SetColour(ConsoleColor msgLineErrorForeGndColour, ConsoleColor msgLineErrorBackGndColour)
        {
            try
            {
                Console.ForegroundColor = msgLineErrorForeGndColour;
                Console.BackgroundColor = msgLineErrorBackGndColour;
                ErrorMsg = null;
            }
            catch (Exception e)
            {
                SetError(1210401, BaseView.ErrorType.exception, e.Message);
            }
            return (IsError()) ? false : true;
        }
        public bool SetCursorPosition(int line, int column)
        {
            if ((line < 0) || (line >= Console.BufferHeight) || (column < 0) || (column >= Console.BufferWidth))
                SetError(1210501, BaseView.ErrorType.param, $"Invalid cursor position: line={line}, column={column}");
            else
            {
                try
                {
                    Console.CursorLeft = column;
                    Console.CursorTop = line;
                    ErrorMsg = null;
                }
                catch (Exception e)
                {
                    SetError(1210502, BaseView.ErrorType.exception, e.Message);
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
                ErrorMsg = null;
            }
            catch (Exception e)
            {
                SetError(1210601, BaseView.ErrorType.exception, e.Message);
            }
            return rc;
        }
        public int GetCursorLine()
        {
            var rc = Program.PosIntegerNotSet;
            try
            {
                rc = Console.CursorTop;
                ErrorMsg = null;
            }
            catch (Exception e)
            {
                SetError(1210701, BaseView.ErrorType.exception, e.Message);
            }
            return rc;
        }
        public bool Clear()
        {
            try
            {
                Console.Clear();
                ErrorMsg = null;
            }
            catch (Exception e)
            {
                SetError(1210801, BaseView.ErrorType.exception, e.Message);
            }
            return (IsError()) ? false : true;
        }

        public string WriteLine(string line, params object[] args)
        {
            string rc = null;
            if (line == null)
                SetError(1210901, BaseView.ErrorType.exception, "line is null");
            else
            {
                try
                {
                    Console.WriteLine(line, args);
                    rc = string.Format(line, args);
                    ErrorMsg = null;
                }
                catch (Exception e)
                {
                    SetError(1210902, BaseView.ErrorType.exception, e.Message);
                }
            }
            return rc;
        }
        public string Write(string msg, params object[] args)
        {
            string rc = null;
            if (msg == null)
                SetError(1211001, BaseView.ErrorType.param, "msg is null");
            else
            {
                try
                {
                    Console.Write(msg, args);
                    rc = string.Format(msg, args);
                    ErrorMsg = null;
                }
                catch (Exception e)
                {
                    SetError(1211002, BaseView.ErrorType.exception, e.Message);
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
                ErrorMsg = null;
            }
            catch (Exception e)
            {
                SetError(1211101, BaseView.ErrorType.exception, e.Message);
            }
            return rc;
        }

        public ConsoleKey GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape)
        {
            var rc =Terminal.InvalidKey;
            try
            {
                rc = Console.ReadKey(hide).Key; //defaultVal is helpful in testing
            }
            catch (Exception e)
            {
                SetError(1211201, BaseView.ErrorType.exception, e.Message);
            }
            return rc;
        }

        public ConsoleKeyInfo ReadKey()
        {
            var rc = new ConsoleKeyInfo('\0', ConsoleKey.Clear, false, false, false);
            try
            {
                rc = Console.ReadKey();
            }
            catch (Exception e)
            {
                SetError(1211301, BaseView.ErrorType.exception, e.Message);
            }
            return rc;
        }

    }
}
