using System;
using KLineEdCmdApp.Controller;
using KLineEdCmdApp.Model;
using MxReturnCode;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;


namespace KLineEdCmdApp.View
{
    public class StatusLineView : KLineEdBaseView
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
                    StatusLineRow = WindowHeight - KLineEditor.StatusLineCount - 1;

                    if (Terminal.SetCursorPosition(StatusLineRow, KLineEditor.StatusLineLeftCol) == false)
                        rc.SetError(1200102, MxError.Source.Program, $"StatusLineView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                    else
                    {
                        if (Terminal.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour) == false)
                            rc.SetError(1200103, MxError.Source.Program, $"StatusLineView: Invalid cursor position: Row={KLineEditor.MsgLineRow}, LeftCol={KLineEditor.MsgLineLeftCol}", "MxErrInvalidCondition");
                        else
                        {
                            if (Terminal.Write(BlankLine) == null)
                                rc.SetError(1200104, MxError.Source.Program, $"StatusLineView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                            else
                            {
                                Ready = true;
                                rc.SetResult(true);
                            }
                        }
                    }
                }
            }
            return rc;
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            ChapterModel.ChangeHint change = (ChapterModel.ChangeHint)notificationItem.Change;
            if ((change == ChapterModel.ChangeHint.All) || (change == ChapterModel.ChangeHint.Status))
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                if (model == null)
                    DisplayErrorMsg(1200201, "Program Error. Unable to access data needed for display. Please quit and report this problem.");
                else
                {
                    if (Terminal.SetColour(StatusLineForeGndColour,StatusLineBackGndColour) == false)
                        DisplayErrorMsg(1200202, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                    else
                    {
                        if (Terminal.SetCursorPosition(StatusLineRow, KLineEditor.StatusLineLeftCol) == false)
                            DisplayErrorMsg(1200203, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                        else
                        {
                            var status = model.StatusLine ?? Program.ValueNotSet;
                            if ((LastTerminalOutput = Terminal.Write(GetTextForLine(status, WindowWidth - KLineEditor.StatusLineLeftCol))) == null)
                                DisplayErrorMsg(1200204, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                        }
                    }
                }
            }
        }
    }
}
