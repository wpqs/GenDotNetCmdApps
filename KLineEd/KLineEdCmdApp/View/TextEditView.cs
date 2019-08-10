using System.Diagnostics.CodeAnalysis;
using MxReturnCode;

using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;


namespace KLineEdCmdApp.View
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]

    public class TextEditView : EditAreaView
    {
        public static readonly string TextEditorMode = "Text Editing:";
        // ReSharper disable once RedundantBaseConstructorCall
        public TextEditView(ITerminal terminal) : base(terminal)
        {

        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            var rc = new MxReturnCode<bool>("TextEditView.OnUpdate");

            base.OnUpdate(notificationItem);
            if (IsError())
                rc.SetError(GetErrorNo(), GetErrorSource(), GetErrorTechMsg(), GetErrorUserMsg());
            else
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                if (model == null)
                    rc.SetError(1140101, MxError.Source.Param, $"model is null", MxMsgs.MxErrBadMethodParam);
                else
                {
                    if ((model.EditorHelpLine?.StartsWith(TextEditView.TextEditorMode) ?? false) == false)
                        rc.SetResult(true);
                    else
                    {
                        var editAreaCursor = model.ChapterBody?.GetCursorInEditArea();
                        ChapterModel.ChangeHint change = (ChapterModel.ChangeHint) notificationItem.Change;
                        switch (change)
                        {
                            case ChapterModel.ChangeHint.All:
                            {
                                Terminal.SetCursorVisible(false);
                                rc += ClearEditAreaText();
                                if (rc.IsSuccess(true))
                                {
                                    var rcRes = model.BodyGetEditAreaLinesForDisplay();
                                    rc += rcRes;
                                    if (rcRes.IsSuccess(true))
                                    {
                                        var row = 0;
                                        var lines = rcRes.GetResult();
                                        foreach (var line in lines)
                                        {
                                            if (line != null)
                                                rc += DisplayEditAreaLine(row, line, false);
                                            if (rc.IsError(true))
                                                break;
                                            row++;
                                        }
                                        if (rc.IsSuccess())
                                        { 
                                            rc += SetEditAreaCursor(editAreaCursor?.RowIndex ?? 0, editAreaCursor?.ColIndex ?? 0);
                                            if (rc.IsSuccess(true))
                                                rc.SetResult(true);
                                        }
                                    }
                                }
                                Terminal.SetCursorVisible(CursorOn);
                                break;
                            }
                            case ChapterModel.ChangeHint.Line:
                            {
                                var rcRes = model.BodyGetEditAreaLinesForDisplay(1);
                                rc += rcRes;
                                if (rcRes.IsSuccess(true))
                                {
                                    var line = rcRes.GetResult()?[0] ?? null;
                                    if (line != null)
                                    {
                                        rc += DisplayEditAreaLine(editAreaCursor?.RowIndex ?? 0, line, true);
                                        if (rc.IsSuccess(true))
                                            rc.SetResult(true);
                                    }
                                }

                                break;
                            }
                            case ChapterModel.ChangeHint.Word:
                            {
                                var lastWord = model.ChapterBody?.GetWordInLine() ?? null;
                                if (lastWord != null)
                                {
                                    rc += DisplayEditAreaWord(editAreaCursor?.RowIndex ?? 0, editAreaCursor?.ColIndex ?? 0, lastWord);
                                    if (rc.IsSuccess(true))
                                        rc.SetResult(true);
                                }
                                break;
                            }
                            case ChapterModel.ChangeHint.Char:
                            {
                                var lastChar = model.ChapterBody?.GetCharacterInLine() ?? Body.NullChar;
                                if (lastChar != Body.NullChar)
                                {
                                    rc += DisplayEditAreaChar(editAreaCursor?.RowIndex ?? 0, editAreaCursor?.ColIndex ?? 0, lastChar);
                                    if (rc.IsSuccess(true))
                                        rc.SetResult(true);
                                }
                                break;
                            }
                            case ChapterModel.ChangeHint.Cursor:
                            case ChapterModel.ChangeHint.StatusLine: //reset the cursor after update to EditHelpView, MsgLineView, StatusLineView
                            case ChapterModel.ChangeHint.MsgLine:
                            case ChapterModel.ChangeHint.HelpLine:
                            {
                                rc += SetEditAreaCursor(editAreaCursor?.RowIndex ?? 0, editAreaCursor?.ColIndex ?? 0);
                                if (rc.IsSuccess(true))
                                    rc.SetResult(true);
                                break;
                            }
                            default:
                            {
                                rc.SetError(1140102, MxError.Source.Program, $"hint={MxDotNetUtilsLib.EnumOps.XlatToString(change)} not handled", MxMsgs.MxErrInvalidCondition);
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