using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using MxReturnCode;

namespace KLineEdCmdApp.Utils
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public interface ITerminal
    {
        MxReturnCode<MxReturnCode<bool>> GetMxError();
        bool IsError();
        MxError.Source GetErrorSource();
        int GetErrorNo();
        string GetErrorTechMsg();
        string GetErrorUserMsg();

        bool Setup(TerminalProperties props);
        MxReturnCode<bool> Close();
        TerminalProperties GetSettings();

        bool Clear();

        bool SetColour(ConsoleColor msgLineErrorForeGndColour, ConsoleColor msgLineErrorBackGndColour);

        bool SetCursorPosition(int line, int column);
        int GetCursorColumn();
        int GetCursorLine();

        string WriteLine(string line, params object[] args);
        string Write(string line, params object[] args);

        char GetKeyChar(bool hide = false, char defaultVal = Body.SpaceChar);
        ConsoleKey GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape);
        ConsoleKeyInfo ReadKey();
        bool IsKeyAvailable();
    }
}
