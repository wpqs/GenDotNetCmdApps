using System;
using Xunit;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp;

namespace KLineEdCmdAppTest.UtilsTests
{
    public class MxConsoleTest
    {
        [Fact]
        public void NoParamTest()
        {
            var console = new MxConsole();

            Assert.True(console.IsError());
        }

        [Fact]
        public void SetupTest()
        {
            var console = new MxConsole();
            Assert.True(console.IsError());

            var props = new MxConsoleProperties();
            Assert.True(console.ApplySettings(props));
            Assert.True(console.IsError() == false);
        }

        [Fact]
        public void GetSettingsTest()
        {
            var console = new MxConsole();

            var props = console.GetSettings();
            Assert.True(props.IsError() == false);

            Assert.True(console.ApplySettings(props));
            Assert.True(console.IsError() == false);
        }

        [Fact]
        public void GetConsoleColorTest()
        {
            Assert.Equal(ConsoleColor.Black, MxConsole.GetConsoleColor(MxConsole.Color.Black));
            Assert.Equal(ConsoleColor.White, MxConsole.GetConsoleColor(MxConsole.Color.White));

            Assert.Equal(ConsoleColor.DarkBlue, MxConsole.GetConsoleColor(MxConsole.Color.DarkBlue));
            Assert.Equal(ConsoleColor.DarkGreen, MxConsole.GetConsoleColor(MxConsole.Color.DarkGreen));
            Assert.Equal(ConsoleColor.DarkCyan, MxConsole.GetConsoleColor(MxConsole.Color.DarkCyan));
            Assert.Equal(ConsoleColor.DarkRed, MxConsole.GetConsoleColor(MxConsole.Color.DarkRed));
            Assert.Equal(ConsoleColor.DarkMagenta, MxConsole.GetConsoleColor(MxConsole.Color.DarkMagenta));
            Assert.Equal(ConsoleColor.DarkYellow, MxConsole.GetConsoleColor(MxConsole.Color.DarkYellow));
            Assert.Equal(ConsoleColor.Gray, MxConsole.GetConsoleColor(MxConsole.Color.Gray));
            Assert.Equal(ConsoleColor.DarkGray, MxConsole.GetConsoleColor(MxConsole.Color.DarkGray));
            Assert.Equal(ConsoleColor.Green, MxConsole.GetConsoleColor(MxConsole.Color.Green));
            Assert.Equal(ConsoleColor.Cyan, MxConsole.GetConsoleColor(MxConsole.Color.Cyan));
            Assert.Equal(ConsoleColor.Red, MxConsole.GetConsoleColor(MxConsole.Color.Red));
            Assert.Equal(ConsoleColor.Magenta, MxConsole.GetConsoleColor(MxConsole.Color.Magenta));
            Assert.Equal(ConsoleColor.Yellow, MxConsole.GetConsoleColor(MxConsole.Color.Yellow));

            Assert.Equal(ConsoleColor.Black, MxConsole.GetConsoleColor(MxConsole.Color.NotSet));
            Assert.Equal(ConsoleColor.Black, MxConsole.GetConsoleColor(MxConsole.Color.Unknown));
        }

        [Fact]
        public void IsValidMxConsoleColorNameTest()
        {
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.Black));
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.White));

            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.DarkBlue));
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.DarkGreen));
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.DarkCyan));
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.DarkRed));
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.DarkMagenta));
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.DarkYellow));
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.Gray));
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.DarkGray));
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.Green));
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.Cyan));
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.Red));
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.Magenta));
            Assert.True(MxConsole.IsValidMxConsoleColorName(MxConsole.Yellow));

            Assert.False(MxConsole.IsValidMxConsoleColorName("pink"));
        }


        [Fact]
        public void XlatConsoleColorToMxConsoleColorTest()
        {
            Assert.Equal(MxConsole.Color.Black, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.Black));
            Assert.Equal(MxConsole.Color.White, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.White));

            Assert.Equal(MxConsole.Color.DarkBlue, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.DarkBlue));
            Assert.Equal(MxConsole.Color.DarkGreen, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.DarkGreen));
            Assert.Equal(MxConsole.Color.DarkCyan, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.DarkCyan));
            Assert.Equal(MxConsole.Color.DarkRed, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.DarkRed));
            Assert.Equal(MxConsole.Color.DarkMagenta, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.DarkMagenta));
            Assert.Equal(MxConsole.Color.DarkYellow, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.DarkYellow));
            Assert.Equal(MxConsole.Color.Gray, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.Gray));
            Assert.Equal(MxConsole.Color.DarkGray, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.DarkGray));
            Assert.Equal(MxConsole.Color.Green, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.Green));
            Assert.Equal(MxConsole.Color.Cyan, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.Cyan));
            Assert.Equal(MxConsole.Color.Red, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.Red));
            Assert.Equal(MxConsole.Color.Magenta, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.Magenta));
            Assert.Equal(MxConsole.Color.Yellow, MxConsole.XlatConsoleColorToMxConsoleColor(ConsoleColor.Yellow));
        }

        [Fact]
        public void XlatMxConsoleColorToStringTest()
        {
            Assert.Equal(MxConsole.Black, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.Black));
            Assert.Equal(MxConsole.White, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.White));

            Assert.Equal(MxConsole.DarkBlue, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.DarkBlue));
            Assert.Equal(MxConsole.DarkGreen, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.DarkGreen));
            Assert.Equal(MxConsole.DarkCyan, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.DarkCyan));
            Assert.Equal(MxConsole.DarkRed, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.DarkRed));
            Assert.Equal(MxConsole.DarkMagenta, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.DarkMagenta));
            Assert.Equal(MxConsole.DarkYellow, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.DarkYellow));
            Assert.Equal(MxConsole.Gray, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.Gray));
            Assert.Equal(MxConsole.DarkGray, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.DarkGray));
            Assert.Equal(MxConsole.Green, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.Green));
            Assert.Equal(MxConsole.Cyan, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.Cyan));
            Assert.Equal(MxConsole.Red, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.Red));
            Assert.Equal(MxConsole.Magenta, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.Magenta));
            Assert.Equal(MxConsole.Yellow, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.Yellow));

            Assert.Equal(Program.ValueUnknown, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.Unknown));
            Assert.Equal(Program.ValueNotSet, MxConsole.XlatMxConsoleColorToString(MxConsole.Color.NotSet));
        }

        [Fact]
        public void XlatStringToMxConsoleColorTest()
        {
            Assert.Equal(MxConsole.Color.Black, MxConsole.XlatStringToMxConsoleColor(MxConsole.Black));
            Assert.Equal(MxConsole.Color.White, MxConsole.XlatStringToMxConsoleColor(MxConsole.White));

            Assert.Equal(MxConsole.Color.DarkBlue, MxConsole.XlatStringToMxConsoleColor(MxConsole.DarkBlue));
            Assert.Equal(MxConsole.Color.DarkGreen, MxConsole.XlatStringToMxConsoleColor(MxConsole.DarkGreen));
            Assert.Equal(MxConsole.Color.DarkCyan, MxConsole.XlatStringToMxConsoleColor(MxConsole.DarkCyan));
            Assert.Equal(MxConsole.Color.DarkRed, MxConsole.XlatStringToMxConsoleColor(MxConsole.DarkRed));
            Assert.Equal(MxConsole.Color.DarkMagenta, MxConsole.XlatStringToMxConsoleColor(MxConsole.DarkMagenta));
            Assert.Equal(MxConsole.Color.DarkYellow, MxConsole.XlatStringToMxConsoleColor(MxConsole.DarkYellow));
            Assert.Equal(MxConsole.Color.Gray, MxConsole.XlatStringToMxConsoleColor(MxConsole.Gray));
            Assert.Equal(MxConsole.Color.DarkGray, MxConsole.XlatStringToMxConsoleColor(MxConsole.DarkGray));
            Assert.Equal(MxConsole.Color.Green, MxConsole.XlatStringToMxConsoleColor(MxConsole.Green));
            Assert.Equal(MxConsole.Color.Cyan, MxConsole.XlatStringToMxConsoleColor(MxConsole.Cyan));
            Assert.Equal(MxConsole.Color.Red, MxConsole.XlatStringToMxConsoleColor(MxConsole.Red));
            Assert.Equal(MxConsole.Color.Magenta, MxConsole.XlatStringToMxConsoleColor(MxConsole.Magenta));
            Assert.Equal(MxConsole.Color.Yellow, MxConsole.XlatStringToMxConsoleColor(MxConsole.Yellow));

            Assert.Equal(MxConsole.Color.Unknown, MxConsole.XlatStringToMxConsoleColor(Program.ValueUnknown));
            Assert.Equal(MxConsole.Color.NotSet, MxConsole.XlatStringToMxConsoleColor(Program.ValueNotSet));

            Assert.Equal(MxConsole.Color.Unknown, MxConsole.XlatStringToMxConsoleColor("???"));
        }

        [Fact]
        public void GetMxConsoleColorNameListTest()
        {
            var separator = Environment.NewLine + "    ";
            var result = "white, black, blue, cyan, gray, green, magenta, red, yellow" + separator + "darkblue, darkcyan, darkgray, darkgreen, darkmagenta, darkred, darkyellow";
            Assert.Equal(result, MxConsole.GetMxConsoleColorNameList(separator));

        }
    }
}
