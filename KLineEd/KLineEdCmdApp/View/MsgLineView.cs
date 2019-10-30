using System;
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
            if (IsOnUpdateError())
                rc.SetError(GetErrorNo(), GetErrorSource(), GetErrorTechMsg(), GetErrorUserMsg());
            else
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
                            DisplayMsg(MsgType.Info, "");
                        else
                            DisplayMsg(GetMsgType(model.MsgLine), GetMsg(model.MsgLine));
                        rc.SetResult(true);
                    }
                }
            }
            OnUpdateDone(rc, false);
        }

        private string GetMsg(string msgLine)
        {
            var rc = Program.ValueNotSet;

            if (msgLine != null)
            {
                var errorPartStart = "error ";
                var start = msgLine.ToLower().IndexOf(errorPartStart, StringComparison.Ordinal);
                if (start < 0)
                    rc = msgLine;
                else
                {
                    var end = msgLine.IndexOf('-');
                    if (end < 0)
                        rc = msgLine;
                    else
                    { 
                        var errorCode = msgLine.Snip(start + errorPartStart.Length, end-1);
                        var errorPartTerminator = ": ";
                        var startTextIndex = msgLine.IndexOf(errorPartTerminator, StringComparison.Ordinal); //error 1100703-user: Warning: you cannot move beyond the end of the chapter
                        if ((startTextIndex < 0) || (errorCode == null))
                            rc = msgLine;
                        else
                        {
                            var msg = msgLine.Substring(startTextIndex + errorPartTerminator.Length);
                            if (msg.StartsWith(MxMsgs.ErrorMsgPrecursor, StringComparison.CurrentCultureIgnoreCase))
                                rc = $"#{errorCode} - {msg.Substring(MxMsgs.ErrorMsgPrecursor.Length + 1)}";
                            else if (msg.StartsWith(MxMsgs.WarningMsgPrecursor, StringComparison.CurrentCultureIgnoreCase))
                                rc = $"#{errorCode} - {msg.Substring(MxMsgs.WarningMsgPrecursor.Length + 1)}";
                            else if (msg.StartsWith(MxMsgs.InfoMsgPrecursor, StringComparison.CurrentCultureIgnoreCase))
                                rc = $"#{errorCode} - {msg.Substring(MxMsgs.InfoMsgPrecursor.Length + 1)}";
                            else
                                rc = msgLine;
                        }
                    }
                }
            }
            return rc;
        }

        private MsgType GetMsgType(string msgLine)
        {
            var rc = MsgType.Error;
            if (msgLine != null)
            {
                var errorPartTerminator = ": ";
                var startTextIndex = msgLine.IndexOf(errorPartTerminator, StringComparison.Ordinal);  //error 1100703-user: Warning: you cannot move beyond the end of the chapter
                if (startTextIndex >= 0)
                {
                    var msg = msgLine.Substring(startTextIndex+ errorPartTerminator.Length);
                    if (msg.StartsWith(MxMsgs.ErrorMsgPrecursor))
                        rc = MsgType.Error;
                    else if (msg.StartsWith(MxMsgs.WarningMsgPrecursor))
                        rc = MsgType.Warning;
                    else
                        rc = MsgType.Info;
                }
            }
            return rc;
        }
    }
}
