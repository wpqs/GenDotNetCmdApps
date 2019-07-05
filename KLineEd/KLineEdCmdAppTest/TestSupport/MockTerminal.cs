using System;
using KLineEdCmdApp.View;

namespace KLineEdCmdAppTest.TestSupport
{
    public class MockTerminal : ITerminal
    {
        public string Title { get; set; }
        public void Clear()
        {
            
        }

        public void SetWindowSize(int width, int height)
        {
           
        }

        public void SetBufferSize(int width, int height)
        {
            
        }

        public void WriteLines(string line, params object[] args)
        {
            
        }

        public void Write(string line, params object[] args)
        {
           
        }
        public char GetKeyChar(bool hide = false, char defaultVal = ' ')
        {
            return defaultVal;
        }

        public ConsoleKey GetKey(bool hide = false, ConsoleKey defaultVal = ConsoleKey.Escape)
        {
            return defaultVal;
        }
    }
}
