using System;
using KLineEdCmdApp.Utils;
using MxReturnCode;

namespace KLineEdCmdApp.View.Base
{
    public abstract class EditAreaView : BaseView
    {
        public ConsoleColor EditAreaForeGndColour { private set; get; }
        public ConsoleColor EditAreaBackGndColour { private set; get; }

        public EditAreaView(ITerminal terminal) : base(terminal)
        {
            EditAreaForeGndColour = ConsoleColor.Gray;
            EditAreaBackGndColour = ConsoleColor.Black;
        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("EditAreaView.Setup");

            if (param == null)
                rc.SetError(1140101, MxError.Source.Param, $"param is null", "MxErrBadMethodParam");
            else
            {
                var rcBase = base.Setup(param);
                rc += rcBase;
                if (rcBase.IsSuccess(true))
                {
                    EditAreaForeGndColour = param.ForeGndTextColour; //todo rename param.EditAreaForeGndColour  
                    EditAreaBackGndColour = param.BackGndTextColour; //todo rename param.EditAreaBackGndColour  

                    var rcClear = ClearTextArea();
                    rc += rcClear;
                    if (rcClear.IsSuccess(true))
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
            if (Terminal.SetColour(EditAreaForeGndColour, EditAreaBackGndColour) == false)
                DisplayErrorMsg(1140402, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
        }

        public MxReturnCode<bool> ClearTextArea()
        {
            var rc = new MxReturnCode<bool>("TextEditView.ClearTextArea");

            if (Terminal.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour) == false)
                rc.SetError(1140202, MxError.Source.Program, $"TextEditView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
            else
            {
                for (int lineIndex = 0; lineIndex < DisplayLinesHeight; lineIndex++)
                {
                    var rcClear = ClearLine(lineIndex);
                    rc += rcClear;
                    if (rcClear.IsError(true))
                        break;
                }
                if (rc.IsSuccess())
                    rc.SetResult(true);
            }
            return rc;
        }
        public MxReturnCode<bool> ClearLine(int rowIndex)
        {
            var rc = new MxReturnCode<bool>("TextEditView.ClearLine");

            if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + rowIndex, 0) == false)
                rc.SetError(1140301, MxError.Source.Program, $"ClearLine={rowIndex} {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
            else
            {
                //var blank = $"{rowIndex}";
                // blank = blank.PadRight(WindowWidth-2, '.') + 'x';
                if (Terminal.Write(BlankLine) == null)
                    rc.SetError(1140302, MxError.Source.Program, $"ClearLine={rowIndex} {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                else
                {
                    if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + rowIndex, KLineEditor.EditAreaMarginLeft) == false)
                        rc.SetError(1140303, MxError.Source.Program, $"ClearLine={rowIndex} {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                    else
                    {
                        rc.SetResult(true);
                    }
                }
            }
            return rc;
        }
    }
}
