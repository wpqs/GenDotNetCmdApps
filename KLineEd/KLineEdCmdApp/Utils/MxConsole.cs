using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;
using MxReturnCode;

namespace KLineEdCmdApp.Utils
{
    [SuppressMessage("ReSharper", "RedundantNameQualifier")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "SimplifyConditionalTernaryExpression")]
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    [SuppressMessage("ReSharper", "RedundantTernaryExpression")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public class MxConsole : IMxConsole
    {
        public static readonly ConsoleKey InvalidKey = ConsoleKey.F24;
        public static readonly int StdCursorSize = MxConsoleProperties.DefaultCursorSize;
        public static readonly int InsertModeCursorSize = 100;

        public const string Black = "black";
        public const string Blue = "blue";
        public const string Cyan = "cyan";
        public const string DarkBlue = "darkblue";
        public const string DarkCyan = "darkcyan";
        public const string DarkGray = "darkgray";
        public const string DarkGreen = "darkgreen";
        public const string DarkMagenta = "darkmagenta";
        public const string DarkRed = "darkred";
        public const string DarkYellow = "darkyellow";
        public const string Gray = "gray";
        public const string Green = "green";
        public const string Magenta = "magenta";
        public const string Red = "red";
        public const string White = "white";
        public const string Yellow = "yellow";

        public enum Color
        {
            [EnumMember(Value = MxConsole.Black)] Black = ConsoleColor.Black,
            [EnumMember(Value = MxConsole.DarkBlue)] DarkBlue = ConsoleColor.DarkBlue,
            [EnumMember(Value = MxConsole.DarkGreen)] DarkGreen = ConsoleColor.DarkGreen,
            [EnumMember(Value = MxConsole.DarkCyan)] DarkCyan =  ConsoleColor.DarkCyan,
            [EnumMember(Value = MxConsole.DarkRed)] DarkRed = ConsoleColor.DarkRed,
            [EnumMember(Value = MxConsole.DarkMagenta)] DarkMagenta = ConsoleColor.DarkMagenta,
            [EnumMember(Value = MxConsole.DarkYellow)] DarkYellow = ConsoleColor.DarkYellow,
            [EnumMember(Value = MxConsole.Gray)] Gray = ConsoleColor.Gray,
            [EnumMember(Value = MxConsole.DarkGray)] DarkGray = ConsoleColor.DarkGray,
            [EnumMember(Value = MxConsole.Blue)] Blue = ConsoleColor.Blue,
            [EnumMember(Value = MxConsole.Green)] Green = ConsoleColor.Green,
            [EnumMember(Value = MxConsole.Cyan)] Cyan = ConsoleColor.Cyan,
            [EnumMember(Value = MxConsole.Red)] Red = ConsoleColor.Red,
            [EnumMember(Value = MxConsole.Magenta)] Magenta = ConsoleColor.Magenta,
            [EnumMember(Value = MxConsole.Yellow)] Yellow = ConsoleColor.Yellow,
            [EnumMember(Value = MxConsole.White)] White = ConsoleColor.White,
            [EnumMember(Value = Program.ValueNotSet)] NotSet = Program.PosIntegerNotSet,
            [EnumMember(Value = Program.ValueUnknown)] Unknown = Program.PosIntegerError
        }

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

        private int CursorSize { set; get; }

        public MxConsole()
        {
            _mxErrorState = null;
            CursorSize = MxConsoleProperties.DefaultCursorSize;
        }

        //public bool IsError() { return (_mxErrorCode?.GetResult() ?? false) ? false : true; }
        //public MxError.Source GetErrorSource() { return _mxErrorCode?.GetErrorType() ?? MxError.Source.Program; }
        //public int GetErrorNo() { return _mxErrorCode?.GetErrorCode() ?? Program.PosIntegerNotSet; }
        //public string GetErrorTechMsg() { return _mxErrorCode?.GetErrorTechMsg() ?? Program.ValueNotSet; }
        //public string GetErrorUserMsg() { return _mxErrorCode?.GetErrorUserMsg() ?? Program.ValueNotSet; }


        public MxReturnCode<bool> Close()
        {
            var rc = new MxReturnCode<bool>($"MxConsole.Close");

            if (IsErrorState())
                rc += GetErrorState();

            return rc;
        }

        public bool ApplySettings(MxConsoleProperties props, bool restore=false)
        {
            var rc = new MxReturnCode<bool>($"MxConsole.ApplySettings");

            if ((props == null) || (IsErrorState()))
                rc.SetError(1210201, MxError.Source.Program, "props is null or (IsErrorState()", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (props.IsError())
                    rc.SetError(1210202, MxError.Source.Data, props?.GetValidationError() ?? Program.ValueNotSet, MxMsgs.MxErrInvalidSettingsFile);
                else
                {
                    try
                    {
                        Console.WindowTop = 0;
                        Console.WindowLeft = 0;

                        if (restore == false)
                        {
                            Console.SetWindowSize(props.WindowWidth, props.WindowHeight); //must set window before buffer
                            Console.SetBufferSize(props.BufferWidth, props.BufferHeight);
                            Console.WindowTop = 0; //props.WindowTop;
                            Console.WindowLeft = 0; //props.WindowLeft;
                        }
                        else
                        {
                            Console.SetBufferSize(Console.BufferWidth, props.BufferHeight);
                        }

                        Console.Title = props.Title;
                        CursorSize = props.CursorSize;
                        Console.CursorSize = props.CursorSize;
                        Console.CursorTop = props.CursorTop;
                        Console.CursorLeft = props.CursorLeft;
                        Console.TreatControlCAsInput = props.TreatCtrlCAsInput;

                        Console.ForegroundColor = MxConsole.GetConsoleColor(props.ForegroundColor);
                        Console.BackgroundColor = MxConsole.GetConsoleColor(props.BackgroundColor);

                        if (Clear(true) == false)
                            rc.SetError(1210202, MxError.Source.Sys, "MxConsole.Clear() failed", MxMsgs.MxErrSystemFailure);
                        else
                        {
                            rc.SetResult(true);
                        }
                    }
                    catch (Exception e)
                    {
                        rc.SetError(1210203, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                    }
                }
            }
            if (rc.IsError(true))
                SetErrorState(rc);

            return (IsErrorState()) ? false : true;
        }

        public MxConsoleProperties GetSettings()
        {
            var rc = new MxReturnCode<bool>($"MxConsole.GetSettings");

            var props = new MxConsoleProperties
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
                TreatCtrlCAsInput = Console.TreatControlCAsInput
            };
            if (props.Validate() == false)
                rc.SetError(1210301, MxError.Source.Sys, $"Console's existing props are invalid: {props.GetValidationError()}", MxMsgs.MxErrSystemFailure);
            else
            {
                rc.SetResult(true);
            }
            if (rc.IsError(true))
                SetErrorState(rc);

            return (IsErrorState() == false) ? props : null;
        }

        public bool IsKeyAvailable()
        {
            return Console.KeyAvailable;
        }

        public bool SetColour(MxConsole.Color foreGndColour, MxConsole.Color backGndColour)
        {
            var rc = new MxReturnCode<bool>($"MxConsole.SetColour");

            try
            {
                Console.ForegroundColor = MxConsole.GetConsoleColor(foreGndColour);
                Console.BackgroundColor = MxConsole.GetConsoleColor(backGndColour);
                rc.SetResult(true);
            }
            catch (Exception e)
            {
                rc.SetError(1210401, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            if (rc.IsError(true))
                SetErrorState(rc);
            return (IsErrorState()) ? false : true;
        }
        public bool SetCursorPosition(int row, int column)
        {
            var rc = new MxReturnCode<bool>($"MxConsole.SetCursorPosition");

            if ((row < 0) || (row >= Console.BufferHeight) || (column < 0) || (column >= Console.BufferWidth))
                rc.SetError(1210402, MxError.Source.Param, $"Invalid cursor position: line={row}, column={column}", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    Console.CursorLeft = column;
                    Console.CursorTop = row;
                    rc.SetResult(true);
                }
                catch (Exception e)
                {
                    rc.SetError(1210403, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            if (rc.IsError(true))
                SetErrorState(rc);
            return (IsErrorState()) ? false : true;
        }

        public int GetCursorColumn()
        {
            var rc = new MxReturnCode<bool>($"MxConsole.GetCursorColumn");

            var col = Program.PosIntegerNotSet;
            try
            {
                col = Console.CursorLeft;
                rc.SetResult(true);
            }
            catch (Exception e)
            {
                rc.SetError(1210501, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            if (rc.IsError(true))
                SetErrorState(rc);
            return (IsErrorState()) ? Program.PosIntegerNotSet : col;
        }
        public int GetCursorRow()
        {
            var rc = new MxReturnCode<bool>($"MxConsole.GetCursorRow");

            var row = Program.PosIntegerNotSet;
            try
            {
                row = Console.CursorTop;
                rc.SetResult(true);
            }
            catch (Exception e)
            {
                rc.SetError(1210601, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            if (rc.IsError(true))
                SetErrorState(rc);
            return (IsErrorState()) ? Program.PosIntegerNotSet : row;
        }

        public void SetCursorVisible(bool show = true)
        {
            var rc = new MxReturnCode<bool>($"MxConsole.SetCursorVisible");
            try
            {
                Console.CursorVisible = show;
                rc.SetResult(true);
            }
            catch (Exception e)
            {
                rc.SetError(1210701, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            if (rc.IsError(true))
                SetErrorState(rc);
        }

        public void SetCursorSize(int size)
        {
            var rc = new MxReturnCode<bool>($"MxConsole.SetCursorSize");

            try
            {
                if ((size > 0) && (size <= 100))
                {
                    CursorSize = size;
                    Console.CursorSize = CursorSize;
                    rc.SetResult(true);
                }
            }
            catch (Exception e)
            {
                rc.SetError(1210802, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            if (rc.IsError(true))
                SetErrorState(rc);
        }

        public void SetCursorInsertMode(bool insertMode = false)
        {
            var rc = new MxReturnCode<bool>($"MxConsole.SetCursorInsertMode");

            try
            {
                Console.CursorSize = ((insertMode) ? InsertModeCursorSize : CursorSize);
                rc.SetResult(true);
            }
            catch (Exception e)
            {
                rc.SetError(1210801, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            if (rc.IsError(true))
                SetErrorState(rc);
        }

        public bool Clear(bool force=false)
        {
            var rc = new MxReturnCode<bool>($"MxConsole.Clear");

            try
            {
                Console.Clear();
                if (force == true)
                {
                    var cursorRow = Console.CursorTop;
                    var cursorCol = Console.CursorLeft;
                    var WindowWidth = Console.WindowWidth;
                    var WindowHeight = Console.WindowHeight;
                    var blankLine = new StringBuilder(WindowWidth).Insert(0, " ", WindowWidth).ToString();

                    for (int y = 0; y < WindowHeight; y++)
                    {
                        Console.CursorLeft = 0;
                        Console.CursorTop = y;
                        Console.WriteLine(blankLine);
                    }
                    Console.CursorLeft = cursorCol;
                    Console.CursorTop = cursorRow;

                }
                rc.SetResult(true);
            }
            catch (Exception e)
            {
                rc.SetError(1210901, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            if (rc.IsError(true))
                SetErrorState(rc);
            return (IsErrorState()) ? false : true;
        }

        public bool IsWindowSizeChanged(int expectedWidth, int expectedHeight)
        {
            return ((Console.WindowWidth != expectedWidth) || (Console.WindowHeight != expectedHeight)) ? true : false;
        }

        public string WriteLine(string line, params object[] args)
        {
            var rc = new MxReturnCode<bool>($"MxConsole.WriteLine");

            string text = null;
            if (line == null)
                rc.SetError(1211001, MxError.Source.Param, $"line is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    Console.WriteLine(line, args);
                    text = string.Format(line, args);
                    rc.SetResult(true);
                }
                catch (Exception e)
                {
                    rc.SetError(1211102, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            if (rc.IsError(true))
                SetErrorState(rc);
            return (IsErrorState() == false) ? text : null;
        }

        public string WriteLines(string[] lines)
        {
            string rc = null;
            if (lines != null)
            {
                foreach (var line in lines)
                {
                    if ((rc = WriteLine(line)) == null)
                        break;
                }
            }
            return rc;
        }

        public string Write(string msg, params object[] args)
        {
            var rc = new MxReturnCode<bool>($"MxConsole.Write");

            string txt = null;
            if (msg == null)
                rc.SetError(1211201, MxError.Source.Param, $"msg is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    //if (args.Length == 0)
                    //{
                    //    Console.Write(msg);
                    //    rc = msg;
                    //}
                    //else
                    //{
                        Console.Write(msg, args);
                        txt = string.Format(msg, args);

                    // }
                    rc.SetResult(true);
                }
                catch (Exception e)
                {
                   rc.SetError(1211202, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            if (rc.IsError(true))
                SetErrorState(rc);
            return (IsErrorState() == false) ? txt : null;
        }

        public char GetKeyChar(bool hide = false, char defaultVal = ' ')
        {
            var rc = new MxReturnCode<bool>($"MxConsole.GetKeyChar");

            var input = (char)0;
            try
            {
                input = Console.ReadKey(hide).KeyChar;       //defaultVal is helpful in testing
                rc.SetResult(true);
            }
            catch (Exception e)
            {
               rc.SetError(1211401, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            if (rc.IsError(true))
                SetErrorState(rc);
            return (IsErrorState() == false) ? input : (char)0;
        }

        public ConsoleKey GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape)
        {
            var rc = new MxReturnCode<bool>($"MxConsole.GetKey");

            var input = MxConsole.InvalidKey;
            try
            {
                input = Console.ReadKey(hide).Key; //defaultVal is helpful in testing
               rc.SetResult(true);
            }
            catch (Exception e)
            {
               rc.SetError(1211501, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            if (rc.IsError(true))
                SetErrorState(rc);
            return (IsErrorState() == false) ? input : MxConsole.InvalidKey;
        }

        public ConsoleKeyInfo ReadKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape)
        {
            var rc = new MxReturnCode<bool>($"MxConsole.ReadKey");

            var input = new ConsoleKeyInfo('\0', ConsoleKey.Clear, false, false, false);
            try
            {
                input = Console.ReadKey(hide);
               rc.SetResult(true);
            }
            catch (Exception e)
            {
                rc.SetError(1211601, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            if (rc.IsError(true))
                SetErrorState(rc);
            return input;
        }


        public static ConsoleColor GetConsoleColor(MxConsole.Color color)
        {
            return ((color == Color.NotSet) || (color == Color.Unknown)) ? ConsoleColor.Black : (ConsoleColor) color;
        }

        public static bool IsValidMxConsoleColorName(string name)
        {           //see https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor?view=netframework-4.8
            bool rc = false;

            if (string.IsNullOrEmpty(name) == false)
            {

                if (name == MxConsole.Black)
                    rc = true;
                else if (name == MxConsole.Blue)
                    rc = true;
                else if (name == MxConsole.Cyan)
                    rc = true;
                else if (name == MxConsole.DarkBlue)
                    rc = true;
                else if (name == MxConsole.DarkCyan)
                    rc = true;
                else if (name == MxConsole.DarkGray)
                    rc = true;
                else if (name == MxConsole.DarkGreen)
                    rc = true;
                else if (name == MxConsole.DarkMagenta)
                    rc = true;
                else if (name == MxConsole.DarkRed)
                    rc = true;
                else if (name == MxConsole.DarkYellow)
                    rc = true;
                else if (name == MxConsole.Gray)
                    rc = true;
                else if (name == MxConsole.Green)
                    rc = true;
                else if (name == MxConsole.Magenta)
                    rc = true;
                else if (name == MxConsole.Red)
                    rc = true;
                else if (name == MxConsole.White)
                    rc = true;
                else if (name == MxConsole.Yellow)
                    rc = true;
                else
                {
                    rc = false;
                }
            }
            return rc;
        }

        public static MxConsole.Color XlatConsoleColorToMxConsoleColor(ConsoleColor color)
        {
            var rc = MxConsole.Color.Unknown;

            switch (color)
            {
                case ConsoleColor.Black:
                    rc = MxConsole.Color.Black;
                    break;
                case ConsoleColor.Blue:
                    rc = MxConsole.Color.Blue;
                    break;
                case ConsoleColor.Cyan:
                    rc = MxConsole.Color.Cyan;
                    break;
                case ConsoleColor.DarkBlue:
                    rc = MxConsole.Color.DarkBlue;
                    break;
                case ConsoleColor.DarkCyan:
                    rc = MxConsole.Color.DarkCyan;
                    break;
                case ConsoleColor.DarkGreen:
                    rc = MxConsole.Color.DarkGreen;
                    break;
                case ConsoleColor.DarkMagenta:
                    rc = MxConsole.Color.DarkMagenta;
                    break;
                case ConsoleColor.DarkRed:
                    rc = MxConsole.Color.DarkRed;
                    break;
                case ConsoleColor.DarkYellow:
                    rc = MxConsole.Color.DarkYellow;
                    break;
                case ConsoleColor.DarkGray:
                    rc = MxConsole.Color.DarkGray;
                    break;
                case ConsoleColor.Gray:
                    rc = MxConsole.Color.Gray;
                    break;
                case ConsoleColor.Green:
                    rc = MxConsole.Color.Green;
                    break;
                case ConsoleColor.Magenta:
                    rc = MxConsole.Color.Magenta;
                    break;
                case ConsoleColor.Red:
                    rc = MxConsole.Color.Red;
                    break;
                case ConsoleColor.White:
                    rc = MxConsole.Color.White;
                    break;
                case ConsoleColor.Yellow:
                    rc = MxConsole.Color.Yellow;
                    break;
                default:
                    rc = MxConsole.Color.Unknown;
                    break;
            }
            return rc;
        }

        public static string XlatMxConsoleColorToString(MxConsole.Color color)
        {
            var rc = Program.ValueUnknown;

            switch (color)
            {
                case MxConsole.Color.Black:
                    rc = MxConsole.Black;
                    break;
                case MxConsole.Color.Blue:
                    rc = MxConsole.Blue;
                    break;
                case MxConsole.Color.Cyan:
                    rc = MxConsole.Cyan;
                    break;
                case MxConsole.Color.DarkBlue:
                    rc = MxConsole.DarkBlue;
                    break;
                case MxConsole.Color.DarkCyan:
                    rc = MxConsole.DarkCyan;
                    break;
                case MxConsole.Color.DarkGreen:
                    rc = MxConsole.DarkGreen;
                    break;
                case MxConsole.Color.DarkMagenta: 
                    rc = MxConsole.DarkMagenta;
                    break;
                case MxConsole.Color.DarkRed:
                    rc = MxConsole.DarkRed;
                    break;
                case MxConsole.Color.DarkYellow:
                    rc = MxConsole.DarkYellow;
                    break;
                case MxConsole.Color.DarkGray:
                    rc = MxConsole.DarkGray;
                    break;
                case MxConsole.Color.Gray:
                    rc = MxConsole.Gray;
                    break;
                case MxConsole.Color.Green:
                    rc = MxConsole.Green;
                    break;
                case MxConsole.Color.Magenta:
                    rc = MxConsole.Magenta;
                    break;
                case MxConsole.Color.Red:
                    rc = MxConsole.Red;
                    break;
                case MxConsole.Color.White:
                    rc = MxConsole.White;
                    break;
                case MxConsole.Color.Yellow:
                    rc = MxConsole.Yellow;
                    break;
                case MxConsole.Color.NotSet:
                    rc = Program.ValueNotSet;
                    break;
                default:
                    rc = Program.ValueUnknown;
                    break;
            }
            return rc;
        }

        public static MxConsole.Color XlatStringToMxConsoleColor(string color)
        {
            var rc = MxConsole.Color.Unknown;

            if (string.IsNullOrEmpty(color) == false)
            {
                if (color == MxConsole.Black)
                    rc = MxConsole.Color.Black;
                else if (color == MxConsole.Blue)
                    rc = MxConsole.Color.Blue;
                else if (color == MxConsole.Cyan)
                    rc = MxConsole.Color.Cyan;
                else if (color == MxConsole.DarkBlue)
                    rc = MxConsole.Color.DarkBlue;
                else if (color == MxConsole.DarkCyan)
                    rc = MxConsole.Color.DarkCyan;
                else if (color == MxConsole.DarkGreen)
                    rc = MxConsole.Color.DarkGreen;
                else if (color == MxConsole.DarkMagenta)
                    rc = MxConsole.Color.DarkMagenta;
                else if (color == MxConsole.DarkRed)
                    rc = MxConsole.Color.DarkRed;
                else if (color == MxConsole.DarkYellow)
                    rc = MxConsole.Color.DarkYellow;
                else if (color == MxConsole.DarkGray)
                    rc = MxConsole.Color.DarkGray;
                else if (color == MxConsole.Gray)
                    rc = MxConsole.Color.Gray;
                else if (color == MxConsole.Green)
                    rc = MxConsole.Color.Green;
                else if (color == MxConsole.Magenta)
                    rc = MxConsole.Color.Magenta;
                else if (color == MxConsole.Red)
                    rc = MxConsole.Color.Red;
                else if (color == MxConsole.White)
                    rc = MxConsole.Color.White;
                else if (color == MxConsole.Yellow)
                    rc = MxConsole.Color.Yellow;
                else if (color == Program.ValueNotSet)
                    rc = MxConsole.Color.NotSet;
                else
                    rc = MxConsole.Color.Unknown;
            }
            return rc;
        }
        public static string GetMxConsoleColorNameList(string separator=null)
        {           //see https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor?view=netframework-4.8
            string rc = "";

            rc += MxConsole.White + ", ";
            rc += MxConsole.Black + ", ";
            rc += MxConsole.Blue + ", ";
            rc += MxConsole.Cyan + ", ";
            rc += MxConsole.Gray + ", ";
            rc += MxConsole.Green + ", ";
            rc += MxConsole.Magenta + ", ";
            rc += MxConsole.Red + ", ";
            rc += MxConsole.Yellow;
            rc += separator ?? ", ";
            rc += MxConsole.DarkBlue + ", ";
            rc += MxConsole.DarkCyan + ", ";
            rc += MxConsole.DarkGray + ", ";
            rc += MxConsole.DarkGreen + ", ";
            rc += MxConsole.DarkMagenta + ", ";
            rc += MxConsole.DarkRed + ", ";
            rc += MxConsole.DarkYellow;

            return rc;
        }
    }
}
