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
                rc.SetError(1140101, MxError.Source.Param, $"param is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                var rcBase = base.Setup(param);
                rc += rcBase;
                if (rcBase.IsSuccess(true))
                {       //todo apply ResetResult()
                    EditAreaForeGndColour = ConsoleColor.Green; // param.ForeGndTextColour; //todo rename param.EditAreaForeGndColour  
                    EditAreaBackGndColour = ConsoleColor.Black; // param.BackGndTextColour; //todo rename param.EditAreaBackGndColour 

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
            base.OnUpdate(notificationItem);
            if (IsError() == false)
            {
                if (Terminal.SetColour(EditAreaForeGndColour, EditAreaBackGndColour) == false)
                    SetMxError(1140202, Terminal.GetErrorSource(), $"Terminal: {Terminal.GetErrorTechMsg()}", Terminal.GetErrorUserMsg());
            }
        }

        public MxReturnCode<bool> ClearEditAreaText()
        {
            var rc = new MxReturnCode<bool>("EditAreaView.ClearTextArea");

            if (Terminal.SetColour(EditAreaForeGndColour, EditAreaBackGndColour) == false)
                rc.SetError(1140301, Terminal.GetErrorSource(), $"EditAreaView: {Terminal.GetErrorTechMsg()}", Terminal.GetErrorUserMsg());
            else
            {
                Terminal.SetCursorVisible(false);
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
                Terminal.SetCursorVisible(CursorOn);
                if (rc.IsSuccess())
                    rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool>SetEditAreaCursor(int editRowIndex = 0, int editColIndex = 0)
        {
            var rc = new MxReturnCode<bool>("EditAreaView.SetCursor");

            if ((editRowIndex < 0) || (editRowIndex >= EditAreaHeight) || (editColIndex < 0) || (editColIndex >= WindowWidth-1))
                rc.SetError(1140401, MxError.Source.Param, $"SetCursor= row{editRowIndex} (max={EditAreaHeight}), col={editColIndex} (max={WindowWidth-1})", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + editRowIndex, KLineEditor.EditAreaMarginLeft + editColIndex) == false)
                    rc.SetError(1110402, Terminal.GetErrorSource(), Terminal.GetErrorTechMsg(), Terminal.GetErrorUserMsg());
                else
                    rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> DisplayEditAreaLine(int editRowIndex, string line, bool clear=true)
        {
            var rc = new MxReturnCode<bool>("EditAreaView.DisplayEditAreaLine");

            if ((line == null) || (editRowIndex < 0) || (editRowIndex >= EditAreaHeight))
                rc.SetError(1140601, MxError.Source.Param, $"line is null or row={editRowIndex} (max={EditAreaHeight})", MxMsgs.MxErrBadMethodParam);
            else
            {
                rc += DisplayLine(KLineEditor.EditAreaTopRowIndex + editRowIndex, KLineEditor.EditAreaMarginLeft, line, clear);
                if (rc.IsSuccess(true))
                {
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> DisplayEditAreaWord(int editRowIndex, int editColIndex, string word)
        {
            var rc = new MxReturnCode<bool>("EditAreaView.DisplayEditAreaWord");

            if ((word == null) || (editRowIndex < 0) || (editRowIndex >= EditAreaHeight) || (editColIndex < 0) || (editColIndex >= EditAreaWidth))
                rc.SetError(1140701, MxError.Source.Param, $"word is null or row={editRowIndex} (max={EditAreaHeight}) or col={editColIndex} (max={EditAreaWidth})", MxMsgs.MxErrBadMethodParam);
            else
            {
                rc += DisplayWord(KLineEditor.EditAreaTopRowIndex + editRowIndex, KLineEditor.EditAreaMarginLeft + editColIndex, word, true);
                if (rc.IsSuccess())
                    rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> DisplayEditAreaChar(int editRowIndex, int editColIndex, char c)
        {
            var rc = new MxReturnCode<bool>("EditAreaView.DisplayEditAreaChar");

            rc += DisplayEditAreaWord(editRowIndex, editColIndex, c.ToString());
            if (rc.IsSuccess(true))
                rc.SetResult(true);

            return rc;
        }
    }
}
