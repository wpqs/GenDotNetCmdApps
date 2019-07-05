using System;

namespace KLineEdCmdApp.View
{
    public interface ITerminal
    {
        string Title { get; set; }
        void Clear();
        void SetWindowSize(int width, int height);
        void SetBufferSize(int width, int height);
        void WriteLines(string line, params object[] args);
        void Write(string line, params object[] args);
        char GetKeyChar(bool hide = false, char defaultVal = ' ');
        ConsoleKey GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape);

    }
}
