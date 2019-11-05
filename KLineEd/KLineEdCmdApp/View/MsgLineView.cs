using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.Model;
using MxReturnCode;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;

namespace KLineEdCmdApp.View
{
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    public class MsgLineView : BaseView
    {
        public MsgLineView(IMxConsole console) : base(console)
        {
        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("MsgLineView.ApplySettings");

            if (param == null)
                rc.SetError(1130101, MxError.Source.Param, $"param is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                var rcBase = base.Setup(param);
                rc += rcBase;
                if (rcBase.IsSuccess(true))
                {
                    if (Console.SetColour(MsgLineInfoForeGndColour, MsgLineInfoBackGndColour) == false)
                        rc.SetError(1200102, MxError.Source.Program, $"StatusLineView: Invalid cursor position: Row={KLineEditor.MsgLineRowIndex}, LeftCol={KLineEditor.MsgLineLeftCol}", MxMsgs.MxErrInvalidCondition);
                    else
                    {
                        var rcClear = ClearLine(KLineEditor.MsgLineRowIndex, KLineEditor.MsgLineLeftCol);
                        rc += rcClear;
                        if (rcClear.IsSuccess(true))
                        {
                            Ready = true;
                            rc.SetResult(true);
                        }
                    }
                }
            }
            return rc;
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            var rc = new MxReturnCode<bool>("MsgLineView.OnUpdate");

            base.OnUpdate(notificationItem);
            if (IsErrorState() == false)
            {
                ChapterModel.ChangeHint change = (ChapterModel.ChangeHint) notificationItem.Change;
                if ((change != ChapterModel.ChangeHint.All) && (change != ChapterModel.ChangeHint.MsgLine))
                    rc.SetResult(true);
                else
                {
                    ChapterModel model = notificationItem.Data as ChapterModel;
                    if (model == null)
                        rc.SetError(1130201, MxError.Source.Program, "model is null", MxMsgs.MxErrInvalidCondition);
                    else
                    {
                        if (string.IsNullOrEmpty(model.MsgLine))
                            DisplayMsg(MxReturnCodeUtils.MsgClass.Info, "");
                        else
                            DisplayMsg(EditingBaseController.GetMsgClass(model.MsgLine), model.MsgLine); //Msg is formatted in EditingBaseController.ErrorProcessing()
                        rc.SetResult(true);
                    }
                }
            }
            OnUpdateDone(rc, false);
        }
    }
}
