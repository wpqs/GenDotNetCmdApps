using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using MxReturnCode;

namespace KLineEdCmdApp.Utils
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public interface IMxConsole
    {
        MxReturnCode<MxReturnCode<bool>> GetMxError();
        bool IsError();
        MxError.Source GetErrorSource();
        int GetErrorNo();
        string GetErrorTechMsg();
        string GetErrorUserMsg();

        bool ApplySettings(MxConsoleProperties props, bool restore=false);
        MxReturnCode<bool> Close();
        MxConsoleProperties GetSettings();

        bool Clear(bool force = false);

        bool SetColour(MxConsole.Color foreGndColour, MxConsole.Color msgLineErrorBackGndColour);

        bool SetCursorPosition(int row, int column);
        int GetCursorColumn();
        int GetCursorRow();
        void SetCursorVisible(bool hide=false);

        void SetCursorInsertMode(bool insertMode = false);

        string WriteLine(string line, params object[] args);

        string WriteLines(string[] lines);

        string Write(string line, params object[] args);

        char GetKeyChar(bool hide = false, char defaultVal = Body.SpaceChar);
        ConsoleKey GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape);
        ConsoleKeyInfo ReadKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape);
        bool IsKeyAvailable();

    }
}
