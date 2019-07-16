using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;

namespace KLineEdCmdApp.View
{
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    public class PropsEditView : EditAreaView
    {
        public static readonly string PropsEditorMode = "Properties Editing:";
        public static readonly int AuthorLineNo = 1;
        public static readonly int ProjectLineNo = 3;
        public static readonly int TitleLineNo = 5;
        public static readonly int FilenameLineLineNo = 8;

        public PropsEditView(ITerminal terminal) : base(terminal) { }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            base.OnUpdate(notificationItem);

            ChapterModel model = notificationItem.Data as ChapterModel;
            if (model == null)
                DisplayErrorMsg(1150101, ErrorType.program, $"Unable to access data needed for display. Please quit and report this problem.");
            else
            {
                if (model.EditorHelpLine?.StartsWith(PropsEditView.PropsEditorMode) ?? false)
                {
                    var authorLine = $"1.{model.GetTabSpaces()}{HeaderChapter.AuthorLabel} {model.Header?.Chapter.Author ?? Program.ValueNotSet}";
                    var projectLine = $"2.{model.GetTabSpaces()}{HeaderChapter.ProjectLabel} {model.Header?.Chapter.Project ?? Program.ValueNotSet}";
                    var titleLine = $"3.{model.GetTabSpaces()}{HeaderChapter.TitleLabel} {model.Header?.Chapter.Title ?? Program.ValueNotSet}";
                    var filenameLine = $"{HeaderChapter.PathFileNameLabel} {model.Header?.Chapter.PathFileName ?? Program.ValueNotSet}";

                    ChapterModel.ChangeHint change = (ChapterModel.ChangeHint) notificationItem.Change;
                    switch (change)
                    {
                        case ChapterModel.ChangeHint.Props:
                        case ChapterModel.ChangeHint.All:
                        {
                            var rcClear = ClearTextArea();
                            if (rcClear.IsError(true))
                                DisplayMxErrorMsg(rcClear.GetErrorUserMsg());
                            else
                            {
                                if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex+ AuthorLineNo, KLineEditor.EditAreaMarginLeft) == false)
                                    DisplayErrorMsg(1150201, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                else
                                {
                                    if ((LastTerminalOutput = Terminal.Write(authorLine)) == null)
                                        DisplayErrorMsg(11450202, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                    else
                                    { 
                                        if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + ProjectLineNo, KLineEditor.EditAreaMarginLeft) == false)
                                            DisplayErrorMsg(1150203, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                        else
                                        {
                                            if ((LastTerminalOutput = Terminal.Write(projectLine)) == null)
                                                DisplayErrorMsg(1150204, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                            else
                                            {
                                                if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + TitleLineNo, KLineEditor.EditAreaMarginLeft) == false)
                                                    DisplayErrorMsg(1150205, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                                else
                                                {
                                                    if ((LastTerminalOutput = Terminal.Write(titleLine)) == null)
                                                        DisplayErrorMsg(1150206, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                                    else
                                                    {
                                                        if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + FilenameLineLineNo, KLineEditor.EditAreaMarginLeft) == false)
                                                            DisplayErrorMsg(1150207, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                                        else
                                                        {
                                                            if ((LastTerminalOutput = Terminal.Write(filenameLine)) == null)
                                                                DisplayErrorMsg(1150208, ErrorType.program, $"Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                                            else { }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
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
