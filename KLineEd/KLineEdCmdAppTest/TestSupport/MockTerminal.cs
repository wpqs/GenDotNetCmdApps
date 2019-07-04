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

        public void WriteLine(string line, params object[] args)
        {
            
        }

        public void Write(string line, params object[] args)
        {
           
        }
    }
}
