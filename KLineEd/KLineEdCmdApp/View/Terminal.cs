using System;
using KLineEdCmdApp.Model;

namespace KLineEdCmdApp.View
{
    class Terminal : ITerminal
    {
        public string Title { get; set; }
        public void Clear()
        {
            Console.Clear();
        }

        public void SetWindowSize(int width, int height)
        {
            Console.SetWindowSize(width, height);
        }

        public void SetBufferSize(int width, int height)
        {
            Console.SetBufferSize(width, height);
        }

        public void WriteLines(string line, params object[] args)
        {
            Console.WriteLine(line, args);
        }

        public void Write(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        public char GetKeyChar(bool hide = false, char defaultVal = Body.SpaceChar)
        {
            return Console.ReadKey(hide).KeyChar;       //defaultVal is helpful in testing
        }

        public ConsoleKey GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape)
        {
            return Console.ReadKey(hide).Key; //defaultVal is helpful in testing
        }
    }
}
