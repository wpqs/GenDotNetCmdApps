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
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public class EditorHelpLineView : BaseView
    {
        public ConsoleColor EditorHelpLineForeGndColour { private set; get; }
        public ConsoleColor EditorHelpLineBackGndColour { private set; get; }

        public EditorHelpLineView(ITerminal terminal) : base(terminal)
        {
            EditorHelpLineForeGndColour = ConsoleColor.Gray;
            EditorHelpLineBackGndColour = ConsoleColor.Black;
        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("EditorHelpLineView.Setup");

            if (param == null)
                rc.SetError(1120101, MxError.Source.Param, $"param is null", "MxErrBadMethodParam");
            else
            {
                var rcBase = base.Setup(param);
                rc += rcBase;
                if (rcBase.IsSuccess(true))
                {
                    EditorHelpLineForeGndColour = ConsoleColor.Blue;// param.ForeGndCmdsColour; //todo rename ForeGndCmdsHelpColour
                    EditorHelpLineBackGndColour = ConsoleColor.Black;// param.BackGndCmdsColour; //todo rename BackGndCmdsHelpColour

                    if (Terminal.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour) == false)
                        rc.SetError(1120102, MxError.Source.Program, $"CmdsHelpView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                   else
                    {
                        var rcClear = ClearLine(KLineEditor.EditorHelpLineRowIndex, KLineEditor.EditorHelpLineLeftCol);
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
            var rc = new MxReturnCode<bool>("EditorHelpLineView.OnUpdate");

            ChapterModel.ChangeHint change = (ChapterModel.ChangeHint)notificationItem.Change;
            if ((change != ChapterModel.ChangeHint.All) && (change != ChapterModel.ChangeHint.HelpLine))
                rc.SetResult(true);
            else
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                if (model == null)
                    rc.SetError(1120301, MxError.Source.Program, "model is null", "MxErrInvalidCondition");
                else
                {
                    if (Terminal.SetColour(EditorHelpLineForeGndColour, EditorHelpLineBackGndColour) == false)
                        rc.SetError(1120302, MxError.Source.Program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                    else
                    {
                        var helpText = model.EditorHelpLine ?? Program.ValueNotSet;
                        rc += DisplayLine(KLineEditor.EditorHelpLineRowIndex, KLineEditor.EditorHelpLineLeftCol, helpText, true);
                        if (rc.IsSuccess(true))
                        {
                            rc.SetResult(true);
                        }
                    }
                }
            }
            if (rc.IsError(true))
                DisplayMxErrorMsg(rc.GetErrorUserMsg());
        }
    }
}
