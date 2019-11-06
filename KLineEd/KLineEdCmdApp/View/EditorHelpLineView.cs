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
        public MxConsole.Color EditorHelpLineForeGndColour { private set; get; }
        public MxConsole.Color EditorHelpLineBackGndColour { private set; get; }

        public EditorHelpLineView(IMxConsole console) : base(console)
        {
            EditorHelpLineForeGndColour = MxConsole.Color.Gray;
            EditorHelpLineBackGndColour = MxConsole.Color.Black;
        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("EditorHelpLineView.ApplySettings");

            if (param == null)
                rc.SetError(1120101, MxError.Source.Param, $"param is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                var rcBase = base.Setup(param);
                rc += rcBase;
                if (rcBase.IsSuccess(true))
                {
                    EditorHelpLineForeGndColour = param.ForeGndColourCmds; 
                    EditorHelpLineBackGndColour = param.BackGndColourCmds;

                    var rcHelp = Console.SetColour(EditorHelpLineForeGndColour, EditorHelpLineBackGndColour);
                    rc += rcHelp;
                    if (rcHelp.IsSuccess(true))
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

            base.OnUpdate(notificationItem);
            if (IsErrorState() == false)
            {
                ChapterModel.ChangeHint change = (ChapterModel.ChangeHint) notificationItem.Change;
                if ((change != ChapterModel.ChangeHint.All) && (change != ChapterModel.ChangeHint.HelpLine))
                    rc.SetResult(true);
                else
                {
                    ChapterModel model = notificationItem.Data as ChapterModel;
                    if (model == null)
                        rc.SetError(1120301, MxError.Source.Program, "model is null", MxMsgs.MxErrInvalidCondition);
                    else
                    {
                        var rcColour = Console.SetColour(EditorHelpLineForeGndColour, EditorHelpLineBackGndColour);
                        rc += rcColour;
                        if (rcColour.IsSuccess(true))
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
            }
            OnUpdateDone(rc, false);
        }
    }
}
