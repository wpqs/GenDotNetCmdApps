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

        bool SetColour(ConsoleColor foreGndColour, ConsoleColor msgLineErrorBackGndColour);

        bool SetCursorPosition(int row, int column);
        int GetCursorColumn();
        int GetCursorRow();
        void SetCursorVisible(bool hide=false);

        void SetCursorInsertMode(bool insertMode = false);

        string WriteLine(string line, params object[] args);
        string Write(string line, params object[] args);

        char GetKeyChar(bool hide = false, char defaultVal = Body.SpaceChar);
        ConsoleKey GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape);
        ConsoleKeyInfo ReadKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape);
        bool IsKeyAvailable();

    }
}
