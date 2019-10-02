using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace KLineEdCmdApp.Utils
{
    public class MxConsole
    {
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

        public static ConsoleColor GetConsoleColor(MxConsole.Color color)
        {
            return ((color == Color.NotSet) || (color == Color.Unknown)) ? ConsoleColor.Black : (ConsoleColor) color;
        }


        public static bool IsValidMxConsoleColorName(string name)
        {           //see https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor?view=netframework-4.8
            bool rc = false;

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
        public static string GetConsoleColorNames()
        {           //see https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor?view=netframework-4.8
            string rc = "";

            rc += MxConsole.Black + " | ";
            rc += MxConsole.Blue + " | ";
            rc += MxConsole.Cyan + " | ";
            rc += MxConsole.DarkBlue + " | ";
            rc += MxConsole.DarkCyan + " | ";
            rc += MxConsole.DarkGray + " | ";
            rc += MxConsole.DarkGreen + " | ";
            rc += MxConsole.DarkMagenta + " | ";  
            rc += MxConsole.DarkRed + " | ";
            rc += MxConsole.DarkYellow + " | ";
            rc += MxConsole.Gray + " | ";
            rc += MxConsole.Green + " | ";
            rc += MxConsole.Magenta + " | ";
            rc += MxConsole.Red + " | ";
            rc += MxConsole.White + " | ";
            rc += MxConsole.Yellow;

            return rc;
        }
    }
}
