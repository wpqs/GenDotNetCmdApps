using System.Diagnostics.CodeAnalysis;
using MxReturnCode;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;

namespace KLineEdCmdApp.View
{
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    public class SpellEditView : EditAreaView
    {
        public static readonly string SpellEditorMode = "Spelling Correction:";
        public SpellEditView(ITerminal terminal) : base(terminal) { }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            var rc = new MxReturnCode<bool>("SpellEditView.OnUpdate");

            base.OnUpdate(notificationItem);
            if (IsOnUpdateError())
                rc.SetError(GetErrorNo(), GetErrorSource(), GetErrorTechMsg(), GetErrorUserMsg());
            else
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                if (model == null)
                    rc.SetError(1160101, MxError.Source.Param, $"model is null", MxMsgs.MxErrBadMethodParam);
                else
                {
                    if ((model.EditorHelpLine?.StartsWith(SpellEditView.SpellEditorMode) ?? false) == false)
                        rc.SetResult(true);
                    else
                    {
                        ChapterModel.ChangeHint change = (ChapterModel.ChangeHint) notificationItem.Change;
                        switch (change)
                        {
                            case ChapterModel.ChangeHint.Props:
                            case ChapterModel.ChangeHint.All:
                            {
                                rc += InitDisplay();
                                if (rc.IsSuccess(true))
                                    rc.SetResult(true);
                                break;
                            }
                            case ChapterModel.ChangeHint.StatusLine: //reset the cursor after update to EditHelpView, MsgLineView, StatusLineView
                            case ChapterModel.ChangeHint.MsgLine:
                            case ChapterModel.ChangeHint.HelpLine:
                            {
                                rc += SetEditAreaCursor();
                                if (rc.IsSuccess(true))
                                    rc.SetResult(true);
                                break;
                            }
                            // ReSharper disable once RedundantEmptySwitchSection
                            default:
                            {
                                rc.SetError(1160101, MxError.Source.Program, $"hint={MxDotNetUtilsLib.EnumOps.XlatToString(change)} not handled", MxMsgs.MxErrInvalidCondition);
                                break;
                            }
                        }
                    }
                }
            }
            OnUpdateDone(rc, true);
        }
    }
}
