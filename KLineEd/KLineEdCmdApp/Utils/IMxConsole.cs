using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using MxReturnCode;

namespace KLineEdCmdApp.Utils
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public interface IMxConsole 
    {
        bool IsKeyAvailable();
        bool IsWindowSizeChanged(int expectedWidth, int expectedHeight);

        MxReturnCode<bool> Close();

        MxReturnCode<bool> Clear(bool force = false);

        MxReturnCode<bool> ApplySettings(MxConsoleProperties props, bool restore=false);

        MxReturnCode<MxConsoleProperties> GetSettings();

        MxReturnCode<bool> SetColour(MxConsole.Color foreGndColour, MxConsole.Color msgLineErrorBackGndColour);

        MxReturnCode<bool> SetCursorPosition(int row, int column);
        MxReturnCode<int> GetCursorColumn();
        MxReturnCode<int> GetCursorRow();
        MxReturnCode<bool> SetCursorVisible(bool hide = false);
        MxReturnCode<bool> SetCursorSize(int size);
        MxReturnCode<bool> SetCursorInsertMode(bool insertMode = false);


        MxReturnCode<string> WriteLine(string line, params object[] args);
        MxReturnCode<bool> WriteLines(string[] lines);
        MxReturnCode<string> WriteString(string line, params object[] args);

        MxReturnCode<char> GetKeyChar(bool hide = false, char defaultVal = Body.SpaceChar);
        MxReturnCode<ConsoleKey> GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape);
        MxReturnCode<ConsoleKeyInfo> GetKeyInfo(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape);


    }
}
