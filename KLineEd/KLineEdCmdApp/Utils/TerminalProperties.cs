using System;
using System.Diagnostics.CodeAnalysis;

namespace KLineEdCmdApp.Utils
{
    [SuppressMessage("ReSharper", "SimplifyConditionalTernaryExpression")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class TerminalProperties
    {
        public const string DefaultTitle = Program.ValueNotSet;

        public const int DefaultBufferHeight = 80;
        public const int DefaultBufferWidth = 120;
        public const int DefaultWindowHeight = 80;
        public const int DefaultWindowWidth = 80;
        public const int DefaultCursorSize = 20;
        public const int DefaultWindowTop = 0;
        public const int DefaultWindowLeft = 0;
        public const int DefaultCursorTop = 0;
        public const int DefaultCursorLeft = 0;
        public const ConsoleColor DefaultForegroundColor = ConsoleColor.Gray;
        public const ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;

        public string Title { set; get; }
        public int BufferHeight {set; get; }
        public int BufferWidth  { set; get; }
        public int WindowHeight  {set; get; }
        public int WindowWidth  {set; get; }
        public int CursorSize  { set; get; }
        public int WindowTop  {  set; get; }
        public int WindowLeft  { set; get; }
        public int CursorTop  {  set; get; }
        public int CursorLeft  {  set; get; }
        public ConsoleColor ForegroundColor  { set; get; }
        public ConsoleColor BackgroundColor  { set; get; }

        private bool Error { set; get; }

        public bool IsError(){ return Error;}

        public TerminalProperties()
        {
            Error = SetDefaults() ? false : true;
        }

        public bool SetDefaults()
        {
            Title = DefaultTitle;
            BufferHeight = DefaultBufferHeight;
            BufferWidth = DefaultBufferWidth;
            WindowHeight = DefaultWindowHeight;
            WindowWidth = DefaultWindowWidth;
            CursorSize = DefaultCursorSize;
            WindowTop = DefaultWindowTop;
            WindowLeft = DefaultWindowLeft;
            CursorTop = DefaultCursorTop;
            CursorLeft = DefaultCursorLeft;
            ForegroundColor = DefaultForegroundColor;
            BackgroundColor = DefaultBackgroundColor;

            return Validate();
        }

        public bool Validate()
        {
            Error = true;

            var result = GetValidationError();
            if (result == null)
                Error = false;

            return (Error) ? false : true;
        }

        public string GetValidationError()
        {
            // ReSharper disable once RedundantAssignment
            var rc = "Error: [unknown]";

            if (Title == null)
                rc = "Error: Title is null";
            else
            {
                if ((BufferHeight < 0) || (BufferHeight >= Int16.MaxValue) || (BufferHeight < (WindowTop + WindowHeight)))
                    rc = $"Error: BufferHeight={BufferHeight} is out of range (WindowTop={WindowTop}, WindowHeight={WindowHeight})";
                else
                {
                    if ((BufferWidth < 0) || (BufferWidth >= Int16.MaxValue) || (BufferWidth < (WindowLeft + WindowWidth)))
                        rc = $"Error: BufferWidth={BufferWidth} is out of range (WindowLeft={WindowLeft}, WindowWidth={WindowWidth})";
                    else
                    {
                        if ((WindowHeight < 0) || ((WindowHeight + WindowTop) >= Int16.MaxValue) || (WindowHeight > Console.LargestWindowHeight))
                            rc = $"Error: WindowHeight={WindowHeight} is out of range (WindowTop={WindowTop})";
                        else
                        {
                            if ((WindowWidth < 0) || ((WindowWidth + WindowLeft) >= Int16.MaxValue) || (WindowWidth > Console.LargestWindowWidth))
                                rc = $"Error: WindowWidth={WindowWidth} is out of range (WindowLeft={WindowLeft})";
                            else
                            {
                                if ((WindowTop < 0) || ((WindowTop + WindowHeight) > BufferHeight))
                                    rc = $"Error: WindowTop={WindowTop} is out of range (WindowHeight={WindowHeight}, BufferHeight={BufferHeight})";
                                else
                                {
                                    if ((WindowLeft < 0) || ((WindowLeft + WindowWidth) > BufferWidth))
                                        rc = $"Error: WindowLeft={WindowLeft} is out of range (WindowWidth={WindowWidth}, BufferWidth={BufferWidth})";
                                    else
                                    {
                                        if ((CursorSize <= 0) || (CursorSize > 100))
                                            rc = $"Error: CursorSize={CursorSize} is out of range";
                                        else
                                        {
                                            if ((CursorTop < 0) || (CursorTop >= BufferHeight))
                                                rc = $"Error: CursorTop={CursorTop} is out of range (BufferHeight={BufferHeight})";
                                            else
                                            {
                                                if ((CursorLeft < 0) || (CursorLeft >= BufferWidth))
                                                    rc = $"Error: CursorLeft={CursorLeft} is out of range (BufferWidth={BufferWidth})";
                                                else
                                                {
                                                    rc = null;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return rc;
        }
    }
}
