using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using MxReturnCode;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;


namespace KLineEdCmdApp.View
{
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public class StatusLineView : BaseView
    {
        public MxConsole.Color StatusLineForeGndColour { private set; get; }
        public MxConsole.Color StatusLineBackGndColour { private set; get; }

        private int StatusLineRow { set; get; }
        public StatusLineView(IMxConsole console) : base(console)
        {
            StatusLineForeGndColour = MxConsole.Color.Gray;
            StatusLineBackGndColour = MxConsole.Color.Black;
            StatusLineRow = Program.PosIntegerNotSet;
        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("StatusLineView.ApplySettings");

            if (param == null)
                rc.SetError(1200101, MxError.Source.Param, $"param is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                var rcBase = base.Setup(param);
                rc += rcBase;
                if (rcBase.IsSuccess(true))
                {
                    StatusLineForeGndColour = param.ForeGndColourStatus;  
                    StatusLineBackGndColour = param.BackGndColourStatus; 
                    StatusLineRow = WindowHeight - KLineEditor.StatusLineRowCount - 1;

                    var rcColour = Console.SetColour(StatusLineForeGndColour, StatusLineBackGndColour);
                    rc += rcColour;
                    if (rcColour.IsSuccess(true))
                    {
                        var rcClear = ClearLine(StatusLineRow, KLineEditor.StatusLineLeftCol);
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
            var rc = new MxReturnCode<bool>("EditorHelpView.OnUpdate");

            base.OnUpdate(notificationItem);
            if (IsErrorState() == false)
            {
                ChapterModel.ChangeHint change = (ChapterModel.ChangeHint) notificationItem.Change;
                if ((change != ChapterModel.ChangeHint.All) && (change != ChapterModel.ChangeHint.StatusLine))
                    rc.SetResult(true);
                else
                {
                    ChapterModel model = notificationItem.Data as ChapterModel;
                    if (model == null)
                        rc.SetError(1200101, MxError.Source.Program, "model is null", MxMsgs.MxErrInvalidCondition);
                    else
                    {
                        var rcColour = Console.SetColour(StatusLineForeGndColour, StatusLineBackGndColour);
                        rc += rcColour;
                        if( rcColour.IsSuccess(true))
                        {
                            var statusText = model.StatusLine ?? Program.ValueNotSet;
                            rc += DisplayLine(StatusLineRow, KLineEditor.StatusLineLeftCol, statusText, true);
                            if (rc.IsSuccess(true))
                            {
                                rc.SetResult(true);
                            }
                        }
                    }
                }
            }
            OnUpdateDone(rc, false);
        }
    }
}
