using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;

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

        private void SetError(int errorNo, string errMsg)
        {
            ErrorMsg = $"Error: {errorNo} {errMsg ?? Program.ValueNotSet}";
        }

        public Terminal()
        {
            SetError(1210101, "Terminal not setup");
        }
        public bool Setup(TerminalProperties props)
        {
            if (props.IsError())
                SetError(1210201, props.GetValidationError());
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
                    SetError(1210202, e.Message);
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
                SetError(1210301, props.GetValidationError());
            else
            {
                rc = props;
                ErrorMsg = null;
            }
            return rc;
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
                SetError(1210401, e.Message);
            }
            return (IsError()) ? false : true;
        }
        public bool SetCursorPosition(int line, int column)
        {
            if ((line < 0) || (line >= Console.BufferHeight) || (column < 0) || (column >= Console.BufferWidth))
                SetError(1210501, $"Invalid cursor position: line={line}, column={column}");
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
                    SetError(1210502, e.Message);
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
                SetError(1210601, e.Message);
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
                SetError(1210701, e.Message);
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
                SetError(1210801, e.Message);
            }
            return (IsError()) ? false : true;
        }

        public string WriteLine(string line, params object[] args)
        {
            string rc = null;
            if (line == null)
                SetError(1210901, "line is null");
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
                    SetError(1210902, e.Message);
                }
            }
            return rc;
        }
        public string Write(string msg, params object[] args)
        {
            string rc = null;
            if (msg == null)
                SetError(1211001, "msg is null");
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
                    SetError(1211002, e.Message);
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
                SetError(1211101, e.Message);
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
                SetError(1211201, e.Message);
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
                SetError(1211301, e.Message);
            }
            return rc;
        }
    }
}
