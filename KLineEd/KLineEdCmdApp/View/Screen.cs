using MxReturnCode;

namespace KLineEdCmdApp.View
{
    public class Screen
    {
        public static readonly int ScreenMarginLeft = 10;
        public static readonly int ScreenMarginRight = 20;
        public static readonly int ScreenMarginTop = 10;
        public static readonly int ScreenMarginBottom = 10;

        public static readonly int CmdLineCnt = 3;
        public static readonly int FooterCnt = 1;

        public ITerminal Terminal { set; get; }
        public CmdLineParamsApp AppParams { private set; get; }
        public int Width { private set; get; }
        public int Height { private set; get; }
        public int LineWidth { private set; get; }

        public Screen(ITerminal terminal)
        {
            Terminal = terminal;
            LineWidth = KLineEditor.PosIntegerNotSet;
        }

        public MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("Screen.Setup");

            if (param == null)
                rc.SetError(1030101, MxError.Source.Param, $"param is null", "MxErrBadMethodParam");
            else
            {
                AppParams = param;

                LineWidth = param.MaxCol;

                Terminal.Clear();
                Width = ScreenMarginLeft + LineWidth + ScreenMarginRight;
                Height = CmdLineCnt + ScreenMarginTop + param.DisplayLastLinesCnt + ScreenMarginBottom + FooterCnt;

                Terminal.SetWindowSize(Width, Height);
                Terminal.SetBufferSize(Width, Height);

                // ReSharper disable once LocalizableElement
                Terminal.Title = $"{Program.CmdAppName} v{Program.CmdAppVersion} - {param.EditFile ?? "[null]"}";
                rc.SetResult(true);
            }
            return rc;
        }

        public void WriteFooter(string line)  //stats or error
        {
            Terminal.WriteLine(line);
        }

        public void WriteEditLine(string line)
        {
            var padding = "";
            Terminal.Write(padding.PadLeft(ScreenMarginLeft));
            Terminal.WriteLine(line);
        }

        public static void WriteTextLine(ITerminal terminal, string format, params object[] arg)
        {
            terminal.WriteLine(format ?? "[null]", arg);
        }

        public static void WriteText(ITerminal terminal, string format, params object[] arg)
        {
            terminal.WriteLine(format ?? "[null]", arg);
        }
    }
}