using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using MxReturnCode;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;


namespace KLineEdCmdApp.View
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    public class EditorHelpView : BaseView
    {
        public ConsoleColor CmdsHelpForeGndColour { private set; get; }
        public ConsoleColor CmdsHelpBackGndColour { private set; get; }

        public EditorHelpView(ITerminal terminal) : base(terminal)
        {
            CmdsHelpForeGndColour = ConsoleColor.Gray;
            CmdsHelpBackGndColour = ConsoleColor.Black;
        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("EditorHelpView.Setup");

            if (param == null)
                rc.SetError(1120101, MxError.Source.Param, $"param is null", "MxErrBadMethodParam");
            else
            {
                var rcBase = base.Setup(param);
                rc += rcBase;
                if (rcBase.IsSuccess(true))
                {
                    CmdsHelpForeGndColour = ConsoleColor.Blue;// param.ForeGndCmdsColour; //todo rename ForeGndCmdsHelpColour
                    CmdsHelpBackGndColour = ConsoleColor.Black;// param.BackGndCmdsColour; //todo rename BackGndCmdsHelpColour

                    if (Terminal.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour) == false)
                        rc.SetError(1120102, MxError.Source.Program, $"CmdsHelpView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
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
            var rc = new MxReturnCode<bool>("EditorHelpView.ClearLine");

            if (Terminal.SetCursorPosition(KLineEditor.ModeHelpLineRowIndex, 0) == false)
                rc.SetError(1120201, MxError.Source.Program, $"EditorHelpView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
            else
            {
                if (Terminal.Write(BlankLine) == null)
                    rc.SetError(1120203, MxError.Source.Program, $"EditorHelpView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                else
                {
                    if (Terminal.SetCursorPosition(KLineEditor.ModeHelpLineRowIndex, KLineEditor.ModeHelpLineLeftCol) == false)
                        rc.SetError(1120204, MxError.Source.Program, $"EditorHelpView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
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
            if ((change == ChapterModel.ChangeHint.All) || (change == ChapterModel.ChangeHint.HelpLine))
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                if (model == null)
                    DisplayErrorMsg(1120301, ErrorType.program, $"Unable to access data needed for display. Please quit and report this problem.");
                else
                {
                    if (Terminal.SetColour(CmdsHelpForeGndColour, CmdsHelpBackGndColour) == false)
                        DisplayErrorMsg(1120302, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                    else
                    {
                        var rcClear = ClearLine();
                        if (rcClear.IsError(true))
                            DisplayMxErrorMsg(rcClear.GetErrorUserMsg());
                        else
                        {
                            var cmds = model.EditorHelpLine ?? Program.ValueNotSet;
                            if((LastTerminalOutput = Terminal.Write(BaseView.TruncateTextForLine(cmds, WindowWidth - KLineEditor.ModeHelpLineLeftCol))) == null)
                                DisplayErrorMsg(1120304, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                        }
                    }
                }
            }
        }
    }
}
