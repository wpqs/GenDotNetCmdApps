using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using MxReturnCode;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;


namespace KLineEdCmdApp.View
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class CmdsHelpView : KLineEdBaseView
    {
        public CmdsHelpView(ITerminal terminal) : base(terminal)
        {

        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("CmdsHelpView.Setup");

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
            if (((ChapterModel.ChangeHint)notificationItem.Change) == ChapterModel.ChangeHint.Cmd)
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                Terminal.SetCursorPosition(0, 5);
                Terminal.WriteLines(model?.CmdLine ?? Program.ValueNotSet);
            }
        }
    }
}
