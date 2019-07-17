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
                {       //todo apply ResetResult()
                    EditAreaForeGndColour = param.ForeGndTextColour; //todo rename param.EditAreaForeGndColour  
                    EditAreaBackGndColour = param.BackGndTextColour; //todo rename param.EditAreaBackGndColour 

                    var rcClear = ClearEditAreaText();
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
                DisplayErrorMsg(1140201, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
        }

        public MxReturnCode<bool> ClearEditAreaText()
        {
            var rc = new MxReturnCode<bool>("TextEditView.ClearTextArea");

            if (Terminal.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour) == false)
                rc.SetError(1140301, MxError.Source.Program, $"TextEditView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
            else
            {
                for (int editRowIndex = 0; editRowIndex < EditAreaHeight; editRowIndex++)
                {
                    //var blank = $"{editRowIndex}";         ////see also BaseView.Setup()
                    //blank = blank.PadRight(EditAreaWidth - 2, ',');
                    //blank += "o";
                    //DisplayLine(KLineEditor.EditAreaTopRowIndex+editRowIndex, KLineEditor.EditAreaMarginLeft, blank, true);

                    var rcClear = ClearLine(KLineEditor.EditAreaTopRowIndex + editRowIndex, 0);
                    rc += rcClear;
                    if (rcClear.IsError(true))
                        break;
                }
                if (rc.IsSuccess())
                    rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool>SetEditAreaCursor(int editRowIndex = 0, int editColIndex = 0)
        {
            var rc = new MxReturnCode<bool>("TextEditView.SetCursor");

            if ((editRowIndex < 0) || (editRowIndex >= EditAreaHeight) || (editColIndex < 0) || (editColIndex >= EditAreaWidth))
                rc.SetError(1140401, MxError.Source.Param, $"SetCursor= row{editRowIndex} (max={EditAreaHeight}), col={editColIndex} (max={EditAreaWidth})", "MxErrBadMethodParam");
            else
            {
                if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + editRowIndex, KLineEditor.EditAreaMarginLeft + editColIndex) == false)
                    rc.SetError(1140402, MxError.Source.Program, $"SetCursor={editRowIndex} {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                else
                    rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> DisplayEditAreaLine(int editRowIndex, string line, bool clear=true)
        {
            var rc = new MxReturnCode<bool>("TextEditView.DisplayEditAreaLine");

            if ((line == null) || (editRowIndex < 0) || (editRowIndex >= EditAreaHeight))
                rc.SetError(1140601, MxError.Source.Param, $"line is null or row={editRowIndex} (max={EditAreaHeight})", "MxErrBadMethodParam");
            else
            {
                rc += DisplayLine(KLineEditor.EditAreaTopRowIndex+editRowIndex, KLineEditor.EditAreaMarginLeft, line, clear);
                if (rc.IsSuccess(true))
                {
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> DisplayEditAreaWord(int editRowIndex, int editColIndex, string word)
        {
            var rc = new MxReturnCode<bool>("TextEditView.DisplayEditAreaWord");

            if ((word == null) || (editRowIndex < 0) || (editRowIndex >= EditAreaHeight) || (editColIndex < 0) || (editColIndex >= EditAreaWidth))
                rc.SetError(1140701, MxError.Source.Param, $"word is null or row={editRowIndex} (max={EditAreaHeight}) or col={editColIndex} (max={EditAreaWidth})", "MxErrBadMethodParam");
            else
            {
                rc += SetEditAreaCursor(editRowIndex, editColIndex);
                if (rc.IsSuccess(true))
                {
                    if ((LastTerminalOutput = Terminal.Write(word)) == null)
                        rc.SetError(1140702, MxError.Source.Program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}.", "MxErrInvalidCondition");
                    else
                    {
                        rc.SetResult(true);
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> DisplayEditAreaChar(int editRowIndex, int editColIndex, char c)
        {
            var rc = new MxReturnCode<bool>("TextEditView.DisplayEditAreaChar");

            rc += DisplayEditAreaWord(editRowIndex, editColIndex, c.ToString());
            if (rc.IsSuccess(true))
                rc.SetResult(true);

            return rc;
        }
    }
}
