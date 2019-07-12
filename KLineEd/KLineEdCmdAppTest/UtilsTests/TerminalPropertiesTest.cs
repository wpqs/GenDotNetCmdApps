using System;
using KLineEdCmdApp.Utils;
using Xunit;

namespace KLineEdCmdAppTest.UtilsTests
{
    public class TerminalPropertiesTest
    {
        [Fact]
        public void NoParamTest()
        {
            var props = new TerminalProperties();
            Assert.False(props.IsError());
        }

        [Fact]
        public void ValidateDefaultTest()
        {
            var props = new TerminalProperties();
            Assert.True(props.Validate());
        }

        [Fact]
        public void GetValidationErrorTest()
        {
            var props = new TerminalProperties();
            Assert.Null(props.GetValidationError());
        }

        [Fact]
        public void GetValidationErrorTitleTest()
        {
            var props = new TerminalProperties();
            props.Title = null;
            Assert.Equal("Error: Title is null", props.GetValidationError());
        }

        [Fact]
        public void GetValidationErrorBufferHeightTest()
        {
            var props = new TerminalProperties();
            props.BufferHeight = -1;
            Assert.Equal($"Error: BufferHeight={props.BufferHeight} is out of range (WindowTop={TerminalProperties.DefaultWindowTop}, WindowHeight={TerminalProperties.DefaultWindowHeight})", props.GetValidationError());
            props.BufferHeight = Int16.MaxValue;
            Assert.Equal($"Error: BufferHeight={props.BufferHeight} is out of range (WindowTop={TerminalProperties.DefaultWindowTop}, WindowHeight={TerminalProperties.DefaultWindowHeight})", props.GetValidationError());
            props.BufferHeight = props.WindowTop + props.WindowHeight - 1;
            Assert.Equal($"Error: BufferHeight={props.BufferHeight} is out of range (WindowTop={TerminalProperties.DefaultWindowTop}, WindowHeight={TerminalProperties.DefaultWindowHeight})", props.GetValidationError());
        }

        [Fact]
        public void GetValidationErrorBufferWidthTest()
        {
            var props = new TerminalProperties();
            props.BufferWidth = -1;
            Assert.Equal($"Error: BufferWidth={ props.BufferWidth} is out of range (WindowLeft={TerminalProperties.DefaultWindowLeft}, WindowWidth={TerminalProperties.DefaultWindowWidth})", props.GetValidationError());
            props.BufferWidth = Int16.MaxValue;
            Assert.Equal($"Error: BufferWidth={ props.BufferWidth} is out of range (WindowLeft={TerminalProperties.DefaultWindowLeft}, WindowWidth={TerminalProperties.DefaultWindowWidth})", props.GetValidationError());
            props.BufferWidth = props.WindowLeft + props.WindowWidth - 1;
            Assert.Equal($"Error: BufferWidth={ props.BufferWidth} is out of range (WindowLeft={TerminalProperties.DefaultWindowLeft}, WindowWidth={TerminalProperties.DefaultWindowWidth})", props.GetValidationError());
        }

        [Fact]
        public void GetValidationErrorWindowHeightTest()
        {
            var props = new TerminalProperties();
            props.WindowHeight = -1;
            Assert.Equal($"Error: WindowHeight={props.WindowHeight} is out of range (WindowTop={TerminalProperties.DefaultWindowTop})", props.GetValidationError());
            //caught by BufferHeight test
            //props.WindowHeight = Int16.MaxValue; 
            //Assert.Equal($"Error: WindowHeight={props.WindowHeight} is out of range (WindowTop={TerminalProperties.DefaultWindowTop})", props.GetValidationError());
            //props.WindowHeight = Console.LargestWindowHeight + 1;
            //Assert.Equal($"Error: WindowHeight={props.WindowHeight} is out of range (WindowTop={TerminalProperties.DefaultWindowTop})", props.GetValidationError());
        }

        [Fact]
        public void GetValidationErrorWindowWidthTest()
        {
            var props = new TerminalProperties();
            props.WindowWidth = -1;
            Assert.Equal($"Error: WindowWidth={props.WindowWidth} is out of range (WindowLeft={TerminalProperties.DefaultWindowLeft})", props.GetValidationError());
            //caught by BufferWidth test 
            //props.WindowWidth = Int16.MaxValue;
            //Assert.Equal($"Error: WindowWidth={props.WindowWidth} is out of range (WindowLeft={TerminalProperties.DefaultWindowLeft})", props.GetValidationError());
            //props.WindowWidth = Console.LargestWindowWidth;
            //Assert.Equal($"Error: WindowWidth={props.WindowWidth} is out of range (WindowLeft={TerminalProperties.DefaultWindowLeft})", props.GetValidationError());
        }

        [Fact]
        public void GetValidationErrorWindowTopTest()
        {
            var props = new TerminalProperties();
            props.WindowTop = -1;
            Assert.Equal($"Error: WindowTop={props.WindowTop} is less than zero", props.GetValidationError());
            props.WindowHeight = 50;
            props.BufferHeight = props.WindowHeight;
            props.WindowTop = 0;    //ok to display line 50 of buffer in bottom line of window
            Assert.Null(props.GetValidationError());
            props.WindowTop = 1;    //attempting to display line 51 of buffer in bottom line of window - it doesn't exist!
            Assert.StartsWith($"Error: BufferHeight={props.BufferHeight} is out of range (WindowTop={props.WindowTop}, WindowHeight={props.WindowHeight})", props.GetValidationError());
        }
        [Fact]
        public void GetValidationErrorWindowLeftTest()
        {
            var props = new TerminalProperties();
            props.WindowLeft = -1;
            Assert.Equal($"Error: WindowLeft={props.WindowLeft} is less than zero", props.GetValidationError());

            props.WindowWidth = 120;
            props.BufferWidth = props.WindowWidth;
            props.WindowLeft = 0;  //ok to display column 120 of buffer in RHS of window
            Assert.Null(props.GetValidationError());
            props.WindowLeft = 1;  //attempting to display column 121 of buffer in right most column of window - it doesn't exist! 
            Assert.Equal($"Error: BufferWidth={props.BufferWidth} is out of range (WindowLeft={props.WindowLeft}, WindowWidth={props.WindowWidth})", props.GetValidationError());

        }
        [Fact]
        public void GetValidationErrorCursorSizeTest()
        {
            var props = new TerminalProperties();
            props.CursorSize = 0;
            Assert.Equal($"Error: CursorSize={props.CursorSize} is out of range 1-100", props.GetValidationError());
            props.CursorSize = 101;
            Assert.Equal($"Error: CursorSize={props.CursorSize} is out of range 1-100", props.GetValidationError());

        }
        [Fact]
        public void GetValidationErrorCursorTopTest()
        {
            var props = new TerminalProperties();
            props.CursorTop = -1;
            Assert.Equal($"Error: CursorTop={props.CursorTop} is out of range (BufferHeight={TerminalProperties.DefaultBufferHeight})", props.GetValidationError());
            props.CursorTop = props.BufferHeight;
            Assert.Equal($"Error: CursorTop={props.CursorTop} is out of range (BufferHeight={TerminalProperties.DefaultBufferHeight})", props.GetValidationError());

        }
        [Fact]
        public void GetValidationErrorCursorLeftTest()
        {
            var props = new TerminalProperties();
            props.CursorLeft = -1;
            Assert.Equal($"Error: CursorLeft={props.CursorLeft} is out of range (BufferWidth={TerminalProperties.DefaultBufferWidth})", props.GetValidationError());
            props.CursorLeft = props.BufferWidth;
            Assert.Equal($"Error: CursorLeft={props.CursorLeft} is out of range (BufferWidth={TerminalProperties.DefaultBufferWidth})", props.GetValidationError());

        }
    }
}
