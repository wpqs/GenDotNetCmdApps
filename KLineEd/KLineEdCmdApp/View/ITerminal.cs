namespace KLineEdCmdApp.View
{
    public interface ITerminal
    {
        string Title { get; set; }
        void Clear();
        void SetWindowSize(int width, int height);
        void SetBufferSize(int width, int height);
        void WriteLine(string line, params object[] args);
        void Write(string line, params object[] args);
    }
}
