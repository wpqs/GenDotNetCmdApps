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
        public static readonly string Introduction = "Spellings for ";
        public static readonly string NotYetImplemented = "*** Feature has not been implemented ***";
        public static readonly string SpellEditorMode = "Spelling Correction:";
        public SpellEditView(IMxConsole console) : base(console) { }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            var rc = new MxReturnCode<bool>("SpellEditView.OnUpdate");

            base.OnUpdate(notificationItem);
            if (IsErrorState() == false)
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                if (model?.ChapterBody == null)
                    rc.SetError(1160101, MxError.Source.Param, $"model or model.ChapterBody is null", MxMsgs.MxErrBadMethodParam);
                else
                {
                    if ((model.EditorHelpLine?.StartsWith(SpellEditView.SpellEditorMode) ?? false) == false)
                        rc.SetResult(true);
                    else
                    {
                        var msg = $"{SpellEditView.Introduction} <{model.ChapterBody.GetSelectedWord() ?? Program.ValueNotSet}>";
                        msg = msg.Substring(0, (msg.Length < EditAreaWidth) ? msg.Length : EditAreaWidth);

                        ChapterModel.ChangeHint change = (ChapterModel.ChangeHint) notificationItem.Change;
                        switch (change)
                        {
                            case ChapterModel.ChangeHint.Props:
                            case ChapterModel.ChangeHint.All:
                            {
                                rc += InitDisplay();
                                if (rc.IsSuccess(true))
                                {
                                    rc += DisplayLine(KLineEditor.EditAreaTopRowIndex, KLineEditor.EditAreaMarginLeft, msg, false);
                                    if (rc.IsSuccess(true))
                                    {
                                        rc += DisplayLine(KLineEditor.EditAreaTopRowIndex + 3, KLineEditor.EditAreaMarginLeft, SpellEditView.NotYetImplemented, false);
                                        if (rc.IsSuccess(true))
                                        {
                                            rc += SetEditAreaCursorPosition(0, msg.Length + 1);
                                            if (rc.IsSuccess(true))
                                                rc.SetResult(true);

                                        }
                                    }
                                }
                                break;
                            }
                            case ChapterModel.ChangeHint.StatusLine: //reset the cursor after update to EditHelpView, MsgLineView, StatusLineView
                            case ChapterModel.ChangeHint.MsgLine:
                            case ChapterModel.ChangeHint.HelpLine:
                            {
                                rc += SetEditAreaCursorPosition(0, msg.Length + 1);
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
