using MxReturnCode;

using KLineEdCmdApp.Utils;

namespace KLineEdCmdApp.View.Base
{
    public abstract class KLineEdBaseView : ObserverView
    {
        protected ITerminal Terminal { set; get; }
        public int Width { private set; get; }
        public int Height { private set; get; }
        public int LineWidth { private set; get; }
        public bool Ready { private set; get; }


        // ReSharper disable once RedundantBaseConstructorCall
        public KLineEdBaseView(ITerminal terminal) : base()
        {
            Terminal = terminal;
            LineWidth = Program.PosIntegerNotSet;
            Ready = false;
        }

        public virtual MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("KLineEdBaseView.Setup");

            rc.SetResult(true);
            if (param == null)
                rc.SetError(1110101, MxError.Source.Param, $"param is null", "MxErrBadMethodParam");
            else
            {
                LineWidth = param.MaxCol;
                Width = KLineEditor.ScreenMarginLeft + LineWidth + KLineEditor.ScreenMarginRight;
                Height = KLineEditor.CmdLineCnt + KLineEditor.ScreenMarginTop + param.DisplayLastLinesCnt + KLineEditor.ScreenMarginBottom + KLineEditor.FooterCnt;

                Ready = true;
                rc.SetResult(true);

            }
            return rc;
        }
    }
}
