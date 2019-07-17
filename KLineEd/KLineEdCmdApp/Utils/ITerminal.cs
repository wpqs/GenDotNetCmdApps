using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;

namespace KLineEdCmdApp.Utils
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public interface ITerminal
    {
        bool IsError();
        string ErrorMsg { get; }
        bool Clear();

        bool Setup(TerminalProperties props);
        TerminalProperties GetSettings();
        bool SetCursorPosition(int line, int column);
        int GetCursorColumn();
        int GetCursorLine();

        string WriteLine(string line, params object[] args);
        string Write(string line, params object[] args);
        char GetKeyChar(bool hide = false, char defaultVal = Body.SpaceChar);
        ConsoleKey GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape);
        ConsoleKeyInfo ReadKey();
        bool IsKeyAvailable();
        bool SetColour(ConsoleColor msgLineErrorForeGndColour, ConsoleColor msgLineErrorBackGndColour);
    }
}
