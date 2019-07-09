using KLineEdCmdApp.Model;
using MxReturnCode;
// ReSharper disable All

namespace KLineEdCmdApp.View
{
    public class TextLinesView : ObserverView   
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
        public bool Ready { private set; get; }


        public TextLinesView(ITerminal terminal) : base()
        {
            Terminal = terminal;
            LineWidth = KLineEditor.PosIntegerNotSet;
            Ready = false;
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            ChapterModel.ChangeHint change = (ChapterModel.ChangeHint)notificationItem.Change;
            if ((change == ChapterModel.ChangeHint.All) || (change == ChapterModel.ChangeHint.Char) || (change == ChapterModel.ChangeHint.Word))
            {
                //this View is only concerned with changes to Words and Characters so update it from the relevant objects in notification.Data

            }
        }

        //public override void Subscribe(IObservable<NotificationItem> notify)
        //{
        //    base.Subscribe(notify);
        //    //any actions that need to be done for the derived class during Subscribe - i.e. display message
        //}

        protected override bool Unsubscribe()
        {
            return base.Unsubscribe();
            //any actions that need to be done for the derived class during Unsubscribe - i.e. display message
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
                Ready = true;
                rc.SetResult(true);
            }
            return rc;
        }

        public char GetKey(bool hide = false)
        {
            return Terminal.GetKeyChar(hide);
        }

        public void WriteFooter(string line)  //stats or error
        {
            Terminal.WriteLines(line);
        }

        public void WriteEditLine(string line)
        {
            var padding = "";
            Terminal.Write(padding.PadLeft(ScreenMarginLeft));
            Terminal.WriteLines(line);
        }

        public static void WriteTextLine(ITerminal terminal, string format, params object[] arg)
        {
            terminal.WriteLines(format ?? "[null]", arg);
        }

        public static void WriteText(ITerminal terminal, string format, params object[] arg)
        {
            terminal.WriteLines(format ?? "[null]", arg);
        }


    }
}