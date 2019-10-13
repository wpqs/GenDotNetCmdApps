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
        public static readonly string TextEditorMode = "Editor:";



        // ReSharper disable once RedundantBaseConstructorCall
        public TextEditView(ITerminal terminal) : base(terminal)
        {
        }

        protected override bool SetRulers(int maxColIndex)
        {
            var rc = false;
          
            if ((maxColIndex > 0) && (maxColIndex < 999))
            {
                if (DisplayRulers == false)
                    rc = base.SetRulers(maxColIndex);
                else
                {
                    for (var index = 0; index <= maxColIndex; index++)
                    {
                        if ((index % 10) == 0)
                        {
                            var tick = (index / 10).ToString();
                            TopRule += (tick.Length > 1) ? tick.Substring(1) : tick;
                            BottomRule += BottomRuleChar;
                        }
                        else
                        {
                            TopRule += TopRuleUnitChar;
                            BottomRule += BottomRuleChar;
                        }
                    }
                    rc = true;
                }
            }
            return rc;
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            var rc = new MxReturnCode<bool>("TextEditView.OnUpdate");

            base.OnUpdate(notificationItem);
            if (IsOnUpdateError())
                rc.SetError(GetErrorNo(), GetErrorSource(), GetErrorTechMsg(), GetErrorUserMsg());
            else
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                if (model?.ChapterBody == null)
                    rc.SetError(1140101, MxError.Source.Param, $"model is null", MxMsgs.MxErrBadMethodParam);
                else
                {
                    if ((model.EditorHelpLine?.StartsWith(TextEditView.TextEditorMode) ?? false) == false)
                        rc.SetResult(true);
                    else
                    {
                        var rcCursor = GetEditAreaCursorPosition(model.ChapterBody.Cursor.RowIndex, model.ChapterBody.Cursor.ColIndex, model.ChapterBody.EditAreaTopLineChapterIndex );
                        if (rcCursor.IsError(true))
                            rc += rcCursor;
                        else
                        {
                            var editAreaCursor = rcCursor.GetResult();
                            ChapterModel.ChangeHint change = (ChapterModel.ChangeHint) notificationItem.Change;
                            switch (change)
                            {
                                case ChapterModel.ChangeHint.End:
                                case ChapterModel.ChangeHint.All:
                                {
                                    Terminal.SetCursorVisible(false);

                                    var rcRes = model.ChapterBody.GetEditAreaLinesForDisplay(EditAreaHeight);
                                    rc += rcRes;
                                    if (rcRes.IsSuccess(true))
                                    {
                                        if (change == ChapterModel.ChangeHint.All)
                                            rc += InitDisplay();
                                        if (rc.IsSuccess() && (editAreaCursor != null))
                                        {
                                            var row = 0;
                                            var lines = rcRes.GetResult();
                                            foreach (var line in lines)
                                            {
                                                if (line == null)
                                                    break;
                                                if ((change == ChapterModel.ChangeHint.All) || ((change == ChapterModel.ChangeHint.End) && (row >= editAreaCursor.RowIndex - 1)))
                                                {
                                                    rc += DisplayEditAreaLine(row, line, (change == ChapterModel.ChangeHint.End));
                                                    if (rc.IsError(true))
                                                        break;
                                                }
                                                row++;
                                            }
                                        }
                                        if (rc.IsSuccess() && (editAreaCursor != null))
                                        {
                                            rc += SetEditAreaCursorPosition(editAreaCursor.RowIndex, editAreaCursor?.ColIndex ?? 0);
                                            if (rc.IsSuccess(true))
                                                rc.SetResult(true);
                                        }
                                    }
                                    Terminal.SetCursorVisible(CursorOn);
                                    break;
                                }
                                case ChapterModel.ChangeHint.Line: //only change to line so just update line at cursor.rowIndex 
                                {
                                    Terminal.SetCursorVisible(false);

                                    var rcRes = model.ChapterBody.GetEditAreaCursorLineForDisplay();
                                    rc += rcRes;
                                    if (rcRes.IsSuccess(true))
                                    {
                                        rc += DisplayEditAreaLine(editAreaCursor?.RowIndex ?? 0, rcRes.GetResult(), true);
                                        if (rc.IsSuccess(true))
                                        {
                                            rc += SetEditAreaCursorPosition(editAreaCursor?.RowIndex ?? 0, editAreaCursor?.ColIndex ?? 0);
                                            if (rc.IsSuccess(true))
                                                rc.SetResult(true);
                                        }
                                    }
                                    Terminal.SetCursorVisible(true);
                                    break;
                                }
                                case ChapterModel.ChangeHint.Cursor:
                                case ChapterModel.ChangeHint.StatusLine: //reset the cursor after update to EditHelpView, MsgLineView, StatusLineView
                                case ChapterModel.ChangeHint.MsgLine:
                                case ChapterModel.ChangeHint.HelpLine:
                                {
                                    rc += SetEditAreaCursorPosition(editAreaCursor.RowIndex, editAreaCursor.ColIndex);
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
            }
            OnUpdateDone(rc, true);
        }
    }
}