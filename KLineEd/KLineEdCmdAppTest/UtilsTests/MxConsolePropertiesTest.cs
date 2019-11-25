using System;
using KLineEdCmdApp.Utils;
using Xunit;

namespace KLineEdCmdAppTest.UtilsTests
{
    public class MxConsolePropertiesTest
    {
        //[Fact]
        //public void NoParamTest()
        //{
        //    var props = new MxConsoleProperties();
        //    Assert.False(props.IsError());
        //}

        //[Fact]
        //public void ValidateDefaultTest()
        //{
        //    var props = new MxConsoleProperties();
        //    Assert.True(props.Validate());
        //}

        //[Fact]
        //public void GetValidationErrorTest()
        //{
        //    var props = new MxConsoleProperties();
        //    Assert.Null(props.GetValidationError());
        //}

        //[Fact]
        //public void GetValidationErrorTitleTest()
        //{
        //    var props = new MxConsoleProperties();
        //    props.Title = null;
        //    Assert.Equal($"Title is null", props.GetValidationError());
        //}

        //[Fact]
        //public void GetValidationErrorBufferHeightTest()
        //{
        //    var props = new MxConsoleProperties();
        //    props.BufferHeight = -1;
        //    Assert.Equal($"BufferHeight={props.BufferHeight} is out of range (WindowTop={MxConsoleProperties.DefaultWindowTop}, WindowHeight={MxConsoleProperties.DefaultWindowHeight})", props.GetValidationError());
        //    props.BufferHeight = Int16.MaxValue;
        //    Assert.Equal($"BufferHeight={props.BufferHeight} is out of range (WindowTop={MxConsoleProperties.DefaultWindowTop}, WindowHeight={MxConsoleProperties.DefaultWindowHeight})", props.GetValidationError());
        //    props.BufferHeight = props.WindowTop + props.WindowHeight - 1;
        //    Assert.Equal($"BufferHeight={props.BufferHeight} is out of range (WindowTop={MxConsoleProperties.DefaultWindowTop}, WindowHeight={MxConsoleProperties.DefaultWindowHeight})", props.GetValidationError());
        //}

        //[Fact]
        //public void GetValidationErrorBufferWidthTest()
        //{
        //    var props = new MxConsoleProperties();
        //    props.BufferWidth = -1;
        //    Assert.Equal($"BufferWidth={ props.BufferWidth} is out of range (WindowLeft={MxConsoleProperties.DefaultWindowLeft}, WindowWidth={MxConsoleProperties.DefaultWindowWidth})", props.GetValidationError());
        //    props.BufferWidth = Int16.MaxValue;
        //    Assert.Equal($"BufferWidth={ props.BufferWidth} is out of range (WindowLeft={MxConsoleProperties.DefaultWindowLeft}, WindowWidth={MxConsoleProperties.DefaultWindowWidth})", props.GetValidationError());
        //    props.BufferWidth = props.WindowLeft + props.WindowWidth - 1;
        //    Assert.Equal($"BufferWidth={ props.BufferWidth} is out of range (WindowLeft={MxConsoleProperties.DefaultWindowLeft}, WindowWidth={MxConsoleProperties.DefaultWindowWidth})", props.GetValidationError());
        //}

        //[Fact]
        //public void GetValidationErrorWindowHeightTest()
        //{
        //    var props = new MxConsoleProperties();
        //    props.WindowHeight = -1;
        //    Assert.Equal($"WindowHeight={props.WindowHeight} is out of range (WindowTop={MxConsoleProperties.DefaultWindowTop})", props.GetValidationError());
        //    //caught by BufferHeight test
        //    //props.WindowHeight = Int16.MaxValue; 
        //    //Assert.Equal($"error WindowHeight={props.WindowHeight} is out of range (WindowTop={MxConsoleProperties.DefaultWindowTop})", props.GetValidationError());
        //    //props.WindowHeight = Console.LargestWindowHeight + 1;
        //    //Assert.Equal($"error WindowHeight={props.WindowHeight} is out of range (WindowTop={MxConsoleProperties.DefaultWindowTop})", props.GetValidationError());
        //}

        //[Fact]
        //public void GetValidationErrorWindowWidthTest()
        //{
        //    var props = new MxConsoleProperties();
        //    props.WindowWidth = -1;
        //    Assert.Equal($"WindowWidth={props.WindowWidth} is out of range (WindowLeft={MxConsoleProperties.DefaultWindowLeft})", props.GetValidationError());
        //    //caught by BufferWidth test 
        //    //props.WindowWidth = Int16.MaxValue;
        //    //Assert.Equal($"error WindowWidth={props.WindowWidth} is out of range (WindowLeft={MxConsoleProperties.DefaultWindowLeft})", props.GetValidationError());
        //    //props.WindowWidth = Console.LargestWindowWidth;
        //    //Assert.Equal($"error WindowWidth={props.WindowWidth} is out of range (WindowLeft={MxConsoleProperties.DefaultWindowLeft})", props.GetValidationError());
        //}

        //[Fact]
        //public void GetValidationErrorWindowTopTest()
        //{
        //    var props = new MxConsoleProperties();
        //    props.WindowTop = -1;
        //    Assert.Equal($"WindowTop={props.WindowTop} is less than zero", props.GetValidationError());
        //    props.WindowHeight = 50;
        //    props.BufferHeight = props.WindowHeight;
        //    props.WindowTop = 0;    //ok to display line 50 of buffer in bottom line of window
        //    Assert.Null(props.GetValidationError());
        //    props.WindowTop = 1;    //attempting to display line 51 of buffer in bottom line of window - it doesn't exist!
        //    Assert.StartsWith($"BufferHeight={props.BufferHeight} is out of range (WindowTop={props.WindowTop}, WindowHeight={props.WindowHeight})", props.GetValidationError());
        //}
        //[Fact]
        //public void GetValidationErrorWindowLeftTest()
        //{
        //    var props = new MxConsoleProperties();
        //    props.WindowLeft = -1;
        //    Assert.Equal($"WindowLeft={props.WindowLeft} is less than zero", props.GetValidationError());

        //    props.WindowWidth = 120;
        //    props.BufferWidth = props.WindowWidth;
        //    props.WindowLeft = 0;  //ok to display column 120 of buffer in RHS of window
        //    Assert.Null(props.GetValidationError());
        //    props.WindowLeft = 1;  //attempting to display column 121 of buffer in right most column of window - it doesn't exist! 
        //    Assert.Equal($"BufferWidth={props.BufferWidth} is out of range (WindowLeft={props.WindowLeft}, WindowWidth={props.WindowWidth})", props.GetValidationError());

        //}
        //[Fact]
        //public void GetValidationErrorCursorSizeTest()
        //{
        //    var props = new MxConsoleProperties();
        //    props.CursorSize = 0;
        //    Assert.Equal($"CursorSize={props.CursorSize} is out of range 1-100", props.GetValidationError());
        //    props.CursorSize = 101;
        //    Assert.Equal($"CursorSize={props.CursorSize} is out of range 1-100", props.GetValidationError());

        //}
        //[Fact]
        //public void GetValidationErrorCursorTopTest()
        //{
        //    var props = new MxConsoleProperties();
        //    props.CursorTop = -1;
        //    Assert.Equal($"CursorTop={props.CursorTop} is out of range (BufferHeight={MxConsoleProperties.DefaultBufferHeight})", props.GetValidationError());
        //    props.CursorTop = props.BufferHeight;
        //    Assert.Equal($"CursorTop={props.CursorTop} is out of range (BufferHeight={MxConsoleProperties.DefaultBufferHeight})", props.GetValidationError());

        //}
        //[Fact]
        //public void GetValidationErrorCursorLeftTest()
        //{
        //    var props = new MxConsoleProperties();
        //    props.CursorLeft = -1;
        //    Assert.Equal($"CursorLeft={props.CursorLeft} is out of range (BufferWidth={MxConsoleProperties.DefaultBufferWidth})", props.GetValidationError());
        //    props.CursorLeft = props.BufferWidth;
        //    Assert.Equal($"CursorLeft={props.CursorLeft} is out of range (BufferWidth={MxConsoleProperties.DefaultBufferWidth})", props.GetValidationError());

        //}
    }
}
