using System.Diagnostics.CodeAnalysis;
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
            base.OnUpdate(notificationItem);

            ChapterModel model = notificationItem.Data as ChapterModel;
            if (model == null)
                DisplayErrorMsg(1140101, ErrorType.program, $"Unable to access data needed for display. Please quit and report this problem.");
            else
            {
                if (model.EditorHelpLine?.StartsWith(TextEditView.TextEditorMode) ?? false )
                {
                    ChapterModel.ChangeHint change = (ChapterModel.ChangeHint) notificationItem.Change;
                    switch (change)
                    {
                        case ChapterModel.ChangeHint.All:
                        {
                            var rcClear = ClearTextArea();
                            if (rcClear.IsError(true))
                                DisplayMxErrorMsg(rcClear.GetErrorUserMsg());
                            else
                            {
                                var rcRes = model.GetLastLinesForDisplay(DisplayLinesHeight);
                                if (rcRes.IsError(true))
                                    DisplayMxErrorMsg(rcRes.GetErrorUserMsg());
                                else
                                {
                                    if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex, KLineEditor.EditAreaMarginLeft) == false)
                                        DisplayErrorMsg(1140201, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                    else
                                    {
                                        var row = 0;
                                        var lines = rcRes.GetResult();
                                        foreach (var line in lines)
                                        {
                                            if (line != null)
                                            {
                                                if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + row, KLineEditor.EditAreaMarginLeft) == false)
                                                    DisplayErrorMsg(1140202, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                                else
                                                {
                                                    if ((LastTerminalOutput = Terminal.Write(line)) == null)
                                                        DisplayErrorMsg(1140203, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                                }
                                            }

                                            if (Terminal.IsError())
                                                break;
                                            row++;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        case ChapterModel.ChangeHint.Line:
                        {
                            var rcRes = model.GetLastLinesForDisplay(1);
                            if (rcRes.IsError(true))
                                DisplayMxErrorMsg(rcRes.GetErrorUserMsg());
                            else
                            {
                                var line = rcRes.GetResult()?[0] ?? null;
                                var lastDisplayRowIndex = model.Body?.GetLineCount() - 1 ?? Program.PosIntegerNotSet;

                                var rcClear = ClearLine((lastDisplayRowIndex < 0) ? 0 : lastDisplayRowIndex);
                                if (rcClear.IsError(true))
                                    DisplayMxErrorMsg(rcClear.GetErrorUserMsg());
                                else
                                {
                                    if (line != null)
                                    {
                                        if ((LastTerminalOutput = Terminal.Write(line)) == null)
                                            DisplayErrorMsg(1140301, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                    }
                                }
                            }
                            break;
                        }
                        case ChapterModel.ChangeHint.Word:
                        {
                            var lastWord = model.Body?.GetWordInLine() ?? null;
                            var lastDisplayRowIndex = model.Body?.GetLineCount() - 1 ?? Program.PosIntegerNotSet;
                            var lastDisplayColIndex = model.Body?.GetCharacterCountInLine() - 1 ?? Program.PosIntegerNotSet;

                            if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + ((lastDisplayRowIndex < 0) ? 0 : lastDisplayRowIndex),
                                    KLineEditor.EditAreaMarginLeft + ((lastDisplayColIndex < 0) ? 0 : lastDisplayColIndex)) == false)
                                DisplayErrorMsg(1140401, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                            else
                            {
                                if (lastWord != null)
                                {
                                    if ((LastTerminalOutput = Terminal.Write(lastWord)) == null)
                                        DisplayErrorMsg(1140402, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                }
                            }
                            break;
                        }
                        case ChapterModel.ChangeHint.Char:
                        {
                            var lastChar = model.Body?.GetCharacterInLine() ?? Body.NullChar;
                            var lastDisplayRowIndex = model.Body?.GetLineCount() - 1 ?? Program.PosIntegerNotSet;
                            var lastDisplayColIndex = model.Body?.GetCharacterCountInLine() - 1 ?? Program.PosIntegerNotSet;

                            if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + ((lastDisplayRowIndex < 0) ? 0 : lastDisplayRowIndex),
                                    KLineEditor.EditAreaMarginLeft + ((lastDisplayColIndex < 0) ? 0 : lastDisplayColIndex)) == false)
                                DisplayErrorMsg(1140501, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                            else
                            {
                                if (lastChar != Body.NullChar)
                                {
                                    if ((LastTerminalOutput = Terminal.Write(lastChar.ToString())) == null)
                                        DisplayErrorMsg(1140502, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                }
                            }
                            break;
                        }
                        case ChapterModel.ChangeHint.StatusLine:   //reset the cursor after update to EditHelpView, MsgLineView, StatusLineView
                        case ChapterModel.ChangeHint.MsgLine:
                        case ChapterModel.ChangeHint.HelpLine:
                        {
                            var lastDisplayRowIndex = model.Body?.GetLineCount() - 1 ?? Program.PosIntegerNotSet;
                            var lastDisplayColIndex = model.Body?.GetCharacterCountInLine() - 1 ?? Program.PosIntegerNotSet;

                            if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + ((lastDisplayRowIndex < 0) ? 0 : lastDisplayRowIndex),
                                    KLineEditor.EditAreaMarginLeft + ((lastDisplayColIndex < 0) ? 0 : lastDisplayColIndex)) == false)
                                DisplayErrorMsg(1140601, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                            break;
                        }
                        // ReSharper disable once RedundantEmptySwitchSection
                        default:
                        {      
                            break;
                        }
                    }
                }
            }
        }
    }
}