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
                            BottomRule += "_";
                        }
                        else
                        {
                            TopRule += ".";
                            BottomRule += "_";
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
                if ((model == null) || (model.ChapterBody == null))
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
                                case ChapterModel.ChangeHint.All:
                                {
                                    Terminal.SetCursorVisible(false);
                                    rc += InitDisplay();
                                    if (rc.IsSuccess(true))
                                    {
                                        var rcRes = model.ChapterBody.GetEditAreaLinesForDisplay(EditAreaHeight);
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
                                                rc += SetEditAreaCursorPosition(editAreaCursor.RowIndex, editAreaCursor.ColIndex);
                                                if (rc.IsSuccess(true))
                                                    rc.SetResult(true);
                                            }
                                        }
                                    }

                                    Terminal.SetCursorVisible(CursorOn);
                                    break;
                                }
                                //case ChapterModel.ChangeHint.Line: //change to line so update line at cursor.rowIndex and all others to bottom
                                //{
                                //    var rcRes = model.BodyGetEditAreaLinesForDisplay(1);
                                //    rc += rcRes;
                                //    if (rcRes.IsSuccess(true))
                                //    {
                                //        var line = rcRes.GetResult()?[0] ?? null;
                                //        if (line != null)
                                //        {
                                //            rc += DisplayEditAreaLine(editAreaCursor?.RowIndex ?? 0, line, true);
                                //            if (rc.IsSuccess(true))
                                //                rc.SetResult(true);
                                //        }
                                //    }

                                //    break;
                                //}
                                //case ChapterModel.ChangeHint.Char:  //overwrite char only so just update char at cursor position
                                //{
                                //    var lastChar = model.ChapterBody?.GetCharacterInLine() ?? Body.NullChar;
                                //    if (lastChar != Body.NullChar)
                                //    {
                                //        rc += DisplayEditAreaChar(editAreaCursor?.RowIndex ?? 0, editAreaCursor?.ColIndex ?? 0, lastChar);
                                //        if (rc.IsSuccess(true))
                                //            rc.SetResult(true);
                                //    }
                                //    break;
                                //}
                                //case ChapterModel.ChangeHint.Cursor: //remove comments in BodyMoveCursor() when re-enabling
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