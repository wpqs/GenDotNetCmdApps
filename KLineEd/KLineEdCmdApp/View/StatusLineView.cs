using MxReturnCode;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;


namespace KLineEdCmdApp.View
{
    public class StatusLineView : KLineEdBaseView
    {
        public StatusLineView(ITerminal terminal) : base(terminal)
        {
        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("StatusLineView.Setup");

            var rcBase = base.Setup(param);
            rc += rcBase;
            if (rcBase.IsSuccess(true))
            {

                rc.SetResult(true);
            }
            return rc;
        }
        public override void OnUpdate(NotificationItem notificationItem)
        {
 
        }
    }
}
