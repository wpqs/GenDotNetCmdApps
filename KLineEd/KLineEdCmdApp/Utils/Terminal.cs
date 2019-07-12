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
    public class Terminal : ITerminal
    {
        public static readonly ConsoleKey InvalidKey = ConsoleKey.F24;
  
        public string ErrorMsg { private set; get; }

        public bool IsError() { return (ErrorMsg ==null) ? false : true; }

        public Terminal()
        {
           ErrorMsg = null;
        }
        public bool Setup(TerminalProperties props)
        {
            ErrorMsg = Program.ValueNotSet;
            if (props.IsError() == false)
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
                    ErrorMsg = e.Message;
                }
            }
            return (ErrorMsg == null) ? true : false;
        }

        public TerminalProperties GetSettings()
        {
            TerminalProperties rc = null;
            if (IsError() == false)
            {
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
                if (props.Validate())
                    rc = props;
            }
            return rc;
        }

        public bool SetCursorPosition(int line, int column)
        {
            var rc = false;
            if ((line >= 0) && (line < Console.BufferHeight) && (column >= 0) && (column < Console.BufferWidth))
            {
                try
                {
                    Console.CursorLeft = column;
                    Console.CursorTop = line;
                    rc = true;
                }
                catch (Exception e)
                {
                    ErrorMsg = e.Message;
                }
            }
            return rc;
        }
        public int GetCursorColumn()
        {
            var rc = Program.PosIntegerNotSet;
            try
            {
                rc = Console.CursorLeft;
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message;
            }
            return rc;
        }
        public int GetCursorLine()
        {
            var rc = Program.PosIntegerNotSet;
            try
            {
                rc = Console.CursorTop; 
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message;
            }
            return rc;
        }
        public bool Clear()
        {
            var rc = false;
            try
            {
                Console.Clear();
                rc = true;
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message;
            }
            return rc;
        }
        public string WriteLine(string line, params object[] args)
        {
            string rc = null;
            if (line != null)
            {
                try
                {
                    Console.WriteLine(line, args);
                    rc = string.Format(line, args);
                }
                catch (Exception e)
                {
                    ErrorMsg = e.Message;
                }
            }
            return rc;
        }
        public string Write(string msg, params object[] args)
        {
            string rc = null;
            if (msg != null)
            {
                try
                {
                    Console.Write(msg, args);
                    rc = string.Format(msg, args);
                }
                catch (Exception e)
                {
                    ErrorMsg = e.Message;
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
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message;
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
                ErrorMsg = e.Message;
            }
            return rc;
        }

        public ConsoleKeyInfo ReadKey()
        {
            ConsoleKeyInfo? rc = null;
            try
            {
                rc = Console.ReadKey();
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message;
            }
            return (ConsoleKeyInfo) rc;
        }
    }
}
