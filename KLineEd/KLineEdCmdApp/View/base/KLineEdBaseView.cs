using KLineEdCmdApp.Controller;
using MxReturnCode;

using KLineEdCmdApp.Utils;

namespace KLineEdCmdApp.View.Base
{
    public abstract class KLineEdBaseView : ObserverView
    {
        protected ITerminal Terminal { set; get; }
        public int WindowHeight { private set; get; }
        public int WindowWidth { private set; get; }
        public int DisplayLineWidth { private set; get; }
        public bool Ready { private set; get; }


        // ReSharper disable once RedundantBaseConstructorCall
        public KLineEdBaseView(ITerminal terminal) : base()
        {
            Terminal = terminal;
            DisplayLineWidth = Program.PosIntegerNotSet;
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
                DisplayLineWidth = param.DisplayLineWidth;
                WindowHeight = KLineEditor.CmdsHelpLineCount + KLineEditor.MsgLineCount + KLineEditor.EditAreaMarginTop + param.DisplayLastLinesCnt + KLineEditor.EditAreaMarginBottom + KLineEditor.StatusLineCount;
                WindowWidth = KLineEditor.EditAreaMarginLeft + DisplayLineWidth + KLineEditor.EditAreaMarginRight;

                if ((WindowWidth < Program.ValueOverflow.Length) || (WindowHeight > KLineEditor.MaxHeight))
                    rc.SetError(1110101, MxError.Source.User, $"param.DisplayLineWidth={param.DisplayLineWidth}, param.DisplayLastLinesCnt{param.DisplayLastLinesCnt}", "MxErrInvalidSettingsFile");
                else
                {
                    Ready = true;
                    rc.SetResult(true);
                }

            }
            return rc;
        }
    }
}
