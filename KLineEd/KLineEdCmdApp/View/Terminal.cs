using System;

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

        public void WriteLine(string line, params object[] args)
        {
            Console.WriteLine(line, args);
        }

        public void Write(string format, params object[] args)
        {
            Console.Write(format, args);
        }
    }
}
