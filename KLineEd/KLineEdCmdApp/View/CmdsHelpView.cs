using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Controller;
using KLineEdCmdApp.Model;
using MxReturnCode;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;


namespace KLineEdCmdApp.View
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class CmdsHelpView : KLineEdBaseView
    {
        public ConsoleColor CmdsHelpForeGndColour { private set; get; }
        public ConsoleColor CmdsHelpBackGndColour { private set; get; }

        public CmdsHelpView(ITerminal terminal) : base(terminal)
        {
            CmdsHelpForeGndColour = ConsoleColor.Gray;
            CmdsHelpBackGndColour = ConsoleColor.Black;
        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("CmdHelpView.Setup");

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

                    if (Terminal.SetCursorPosition(KLineEditor.CmdsHelpLineRow, KLineEditor.CmdsHelpLineLeftCol) == false)
                        rc.SetError(1120101, MxError.Source.Program, $"CmdsHelpView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                    else
                    {
                        if (Terminal.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour) == false)
                            rc.SetError(1120102, MxError.Source.Program, $"CmdsHelpView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                        else
                        {
                            if (Terminal.Write(BlankLine) == null)
                                rc.SetError(1120103, MxError.Source.Program, $"CmdsHelpView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
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
            if ((change == ChapterModel.ChangeHint.All) || (change == ChapterModel.ChangeHint.Cmd))
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                if (model == null)
                    DisplayErrorMsg(1120201, "Program Error. Unable to access data needed for display. Please quit and report this problem.");
                else
                {
                    if (Terminal.SetColour(CmdsHelpForeGndColour, CmdsHelpBackGndColour) == false)
                        DisplayErrorMsg(1120202, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                    else
                    {
                        if (Terminal.SetCursorPosition(KLineEditor.CmdsHelpLineRow, KLineEditor.CmdsHelpLineLeftCol) == false)
                            DisplayErrorMsg(1120203, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                        else
                        {
                            var cmds = model.CmdsHelpLine ?? Program.ValueNotSet;
                            if((LastTerminalOutput = Terminal.Write(GetTextForLine(cmds, WindowWidth - KLineEditor.CmdsHelpLineLeftCol))) == null)
                                DisplayErrorMsg(1120204, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                        }
                    }
                }
            }
        }
    }
}
