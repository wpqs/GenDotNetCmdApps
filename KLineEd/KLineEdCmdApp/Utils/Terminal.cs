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
    public class Terminal : ITerminal
    {

        private bool Error { set; get; }

        public bool IsError() { return Error; }

        public Terminal()
        {
           Error = false;
        }
        public bool Setup(TerminalProperties props)
        {
            Error = true;
            if (props.IsError() == false)
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

                Error = false;
            }
            return Error ? false : true;
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
                Console.CursorLeft = column;
                Console.CursorTop = line;
                rc = true;
            }
            return rc;
        }
        public int GetCursorColumn()
        {
            return Console.CursorLeft;
        }
        public int GetCursorLine()
        {
            return Console.CursorTop;
        }
        public void Clear()
        {
            Console.Clear();
        }
        public void WriteLines(string line, params object[] args)
        {
            Console.WriteLine(line, args);
        }
        public void Write(string format, params object[] args)
        {
            Console.Write(format, args);
        }
        public char GetKeyChar(bool hide = false, char defaultVal = Body.SpaceChar)
        {
            return Console.ReadKey(hide).KeyChar;       //defaultVal is helpful in testing
        }

        public ConsoleKey GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape)
        {
            return Console.ReadKey(hide).Key; //defaultVal is helpful in testing
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }
    }
}
