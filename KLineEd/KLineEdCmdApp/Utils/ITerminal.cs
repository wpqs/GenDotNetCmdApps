using System;
using KLineEdCmdApp.Model;

namespace KLineEdCmdApp.Utils
{
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

    }
}
