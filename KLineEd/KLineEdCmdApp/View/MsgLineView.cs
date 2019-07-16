using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using MxReturnCode;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;

namespace KLineEdCmdApp.View
{
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    public class MsgLineView : BaseView
    {
        public MsgLineView(ITerminal terminal) : base(terminal)
        {
        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("MsgLineView.Setup");

            if (param == null)
                rc.SetError(1130101, MxError.Source.Param, $"param is null", "MxErrBadMethodParam");
            else
            {
                var rcBase = base.Setup(param);
                rc += rcBase;
                if (rcBase.IsSuccess(true))
                {
                    if (DisplayMsg(MsgType.Clear, "") == false)
                        rc.SetError(1130102, MxError.Source.Program, $"MsgLineView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                    else
                    {
                        Ready = true;
                        rc.SetResult(true);
                    }
                }
            }
            return rc;
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            ChapterModel.ChangeHint change = (ChapterModel.ChangeHint)notificationItem.Change;
            if ((change == ChapterModel.ChangeHint.All) || (change == ChapterModel.ChangeHint.MsgLine))
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                if (model == null)
                    DisplayErrorMsg(1130201, ErrorType.program, $"Unable to access data needed for display. Please quit and report this problem.");
                else
                {
                    if (model.MsgLine.StartsWith(BaseView.ErrorMsgPrecursor))
                        DisplayMsg(MsgType.Error, model.MsgLine);
                    else if (model.MsgLine.StartsWith(BaseView.WarnMsgPrecursor))
                        DisplayMsg(MsgType.Warning, model.MsgLine);
                    else
                        DisplayMsg(MsgType.Info, model.MsgLine);
                }
            }
        }
    }
}
