using System;
using KLineEdCmdApp.Model;
using MxReturnCode;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;


namespace KLineEdCmdApp.View
{
    public class StatusLineView : BaseView
    {
        public ConsoleColor StatusLineForeGndColour { private set; get; }
        public ConsoleColor StatusLineBackGndColour { private set; get; }

        private int StatusLineRow { set; get; }
        public StatusLineView(ITerminal terminal) : base(terminal)
        {
            StatusLineForeGndColour = ConsoleColor.Gray;
            StatusLineBackGndColour = ConsoleColor.Black;
            StatusLineRow = Program.PosIntegerNotSet;
        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("StatusLineView.Setup");

            if (param == null)
                rc.SetError(1200101, MxError.Source.Param, $"param is null", "MxErrBadMethodParam");
            else
            {
                var rcBase = base.Setup(param);
                rc += rcBase;
                if (rcBase.IsSuccess(true))
                {
                    StatusLineForeGndColour = param.ForeGndDetailsColour; //todo rename param.StatusLineForeGndColour    
                    StatusLineBackGndColour = param.BackGndDetailsColour; //todo rename param.StatusLineBackGndColour 
                    StatusLineRow = WindowHeight - KLineEditor.StatusLineRowCount - 1;

                    if (Terminal.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour) == false)
                        rc.SetError(1200102, MxError.Source.Program, $"StatusLineView: Invalid cursor position: Row={KLineEditor.MsgLineRowIndex}, LeftCol={KLineEditor.MsgLineLeftCol}", "MxErrInvalidCondition");
                    else
                    {
                        var rcClear = ClearLine();
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

        public MxReturnCode<bool> ClearLine()
        {
            var rc = new MxReturnCode<bool>("StatusLineView.ClearLine");

            if (Terminal.SetCursorPosition(StatusLineRow, 0) == false)
                rc.SetError(1200201, MxError.Source.Program, $"StatusLineView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
            else
            {
                if (Terminal.Write(BlankLine) == null)
                    rc.SetError(1200202, MxError.Source.Program, $"StatusLineView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                else
                {
                    if (Terminal.SetCursorPosition(StatusLineRow, KLineEditor.StatusLineLeftCol) == false)
                        rc.SetError(1200203, MxError.Source.Program, $"StatusLineView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                    else
                    {
                        rc.SetResult(true);
                    }
                }
            }
            return rc;
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            ChapterModel.ChangeHint change = (ChapterModel.ChangeHint)notificationItem.Change;
            if ((change == ChapterModel.ChangeHint.All) || (change == ChapterModel.ChangeHint.StatusLine))
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                if (model == null)
                    DisplayErrorMsg(1200301, "Program Error. Unable to access data needed for display. Please quit and report this problem.");
                else
                {
                    if (Terminal.SetColour(StatusLineForeGndColour,StatusLineBackGndColour) == false)
                        DisplayErrorMsg(1200302, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                    else
                    {
                        var rcClear = ClearLine();
                        if (rcClear.IsError(true))
                            DisplayMxErrorMsg(rcClear.GetErrorUserMsg());
                        else
                        {
                            var status = model.StatusLine ?? Program.ValueNotSet;
                            if ((LastTerminalOutput = Terminal.Write(GetTextForLine(status, WindowWidth - KLineEditor.StatusLineLeftCol))) == null)
                                DisplayErrorMsg(1200304, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                        }
                    }
                }
            }
        }
    }
}
