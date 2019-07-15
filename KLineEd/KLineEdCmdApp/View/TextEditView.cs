using System;
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

    public class TextEditView : BaseView   
    {
        public ConsoleColor EditAreaForeGndColour { private set; get; }
        public ConsoleColor EditAreaBackGndColour { private set; get; }

        // ReSharper disable once RedundantBaseConstructorCall
        public TextEditView(ITerminal terminal) : base(terminal)
        {
            EditAreaForeGndColour = ConsoleColor.Gray;
            EditAreaBackGndColour = ConsoleColor.Black;
        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("TextEditView.Setup");

            if (param == null)
                rc.SetError(1140101, MxError.Source.Param, $"param is null", "MxErrBadMethodParam");
            else
            {
                var rcBase = base.Setup(param);
                rc += rcBase;
                if (rcBase.IsSuccess(true))
                {
                    EditAreaForeGndColour = param.ForeGndTextColour; //todo rename param.EditAreaForeGndColour  
                    EditAreaBackGndColour = param.BackGndTextColour; //todo rename param.EditAreaBackGndColour  

                    var rcClear = ClearTextArea();
                    rc += rcClear;
                    if (rcClear.IsSuccess(true))
                    {
                        Ready = true;
                        rc.SetResult(true);
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> ClearTextArea()
        {
            var rc = new MxReturnCode<bool>("TextEditView.ClearTextArea");

            if (Terminal.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour) == false)
                rc.SetError(1140202, MxError.Source.Program, $"TextEditView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
            else
            {
                for (int lineIndex = 0; lineIndex < DisplayLinesHeight; lineIndex++)
                {
                    var rcClear = ClearLine(lineIndex);
                    rc += rcClear;
                    if (rcClear.IsError(true))
                        break;
                }
                if (rc.IsSuccess())
                    rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> ClearLine(int rowIndex)
        {
            var rc = new MxReturnCode<bool>("TextEditView.ClearLine");

            if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + rowIndex, 0) == false)
                rc.SetError(1140301, MxError.Source.Program, $"ClearLine={rowIndex} {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
            else
            {
                //var blank = $"{rowIndex}";
               // blank = blank.PadRight(WindowWidth-2, '.') + 'x';
                if (Terminal.Write(BlankLine) == null)
                    rc.SetError(1140302, MxError.Source.Program, $"ClearLine={rowIndex} {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                else
                {
                    if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + rowIndex, KLineEditor.EditAreaMarginLeft) == false)
                        rc.SetError(1140303, MxError.Source.Program, $"ClearLine={rowIndex} {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                    else
                    {
                        rc.SetResult(true);
                    }
                }
            }
            return rc;
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            ChapterModel model = notificationItem.Data as ChapterModel;
            if (model == null)
                DisplayErrorMsg(1140401, "Program Error. Unable to access data needed for display. Please quit and report this problem.");
            else
            {
                if (Terminal.SetColour(EditAreaForeGndColour, EditAreaBackGndColour) == false)
                    DisplayErrorMsg(1140402, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                else
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
                                        DisplayErrorMsg(1140501, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                    else
                                    {
                                        var row = 0;
                                        var lines = rcRes.GetResult();
                                        foreach (var line in lines)
                                        {
                                            if (line != null)
                                            {
                                                if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + row, KLineEditor.EditAreaMarginLeft) == false)
                                                    DisplayErrorMsg(1140502, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                                else
                                                {
                                                    if ((LastTerminalOutput = Terminal.Write(line)) == null)
                                                        DisplayErrorMsg(1140503, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                                }
                                            }
                                            if (Terminal.IsError())
                                                break;
                                            row++;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                        case ChapterModel.ChangeHint.Line:
                        {
                            var rcRes = model.GetLastLinesForDisplay(1);
                            if (rcRes.IsError(true))
                                DisplayMxErrorMsg(rcRes.GetErrorUserMsg());
                            else
                            {
                                var line = rcRes.GetResult()?[0] ?? null;
                                var lastDisplayRowIndex = model.Body?.GetLineCount()-1 ?? Program.PosIntegerNotSet;

                                var rcClear = ClearLine((lastDisplayRowIndex < 0) ? 0 : lastDisplayRowIndex);
                                if (rcClear.IsError(true))
                                    DisplayMxErrorMsg(rcClear.GetErrorUserMsg());
                                else
                                {
                                    if (line != null)
                                    {
                                        if ((LastTerminalOutput = Terminal.Write(line)) == null)
                                            DisplayErrorMsg(1140601, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                    }
                                }
                            }
                        }
                        break;
                        case ChapterModel.ChangeHint.Word:
                        {
                            var lastWord = model.Body?.GetWordInLine() ?? null;
                            var lastDisplayRowIndex = model.Body?.GetLineCount()-1 ?? Program.PosIntegerNotSet;
                            var lastDisplayColIndex = model.Body?.GetCharacterCountInLine()-1 ?? Program.PosIntegerNotSet;

                            if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + ((lastDisplayRowIndex < 0) ? 0 : lastDisplayRowIndex),
                                    KLineEditor.EditAreaMarginLeft + ((lastDisplayColIndex < 0) ? 0 : lastDisplayColIndex)) == false)
                                DisplayErrorMsg(1140701, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                            else
                            {
                                if (lastWord != null)
                                {
                                    if ((LastTerminalOutput = Terminal.Write(lastWord)) == null)
                                        DisplayErrorMsg(1140702, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                }
                            }
                        }
                        break;
                        case ChapterModel.ChangeHint.Char:
                        {
                            var lastChar = model.Body?.GetCharacterInLine() ?? Body.NullChar;
                            var lastDisplayRowIndex = model.Body?.GetLineCount()-1 ?? Program.PosIntegerNotSet;
                            var lastDisplayColIndex = model.Body?.GetCharacterCountInLine()-1 ?? Program.PosIntegerNotSet;

                            if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + ((lastDisplayRowIndex < 0) ? 0 : lastDisplayRowIndex), 
                                                          KLineEditor.EditAreaMarginLeft + ((lastDisplayColIndex < 0) ? 0 : lastDisplayColIndex)) == false)
                                DisplayErrorMsg(1140801, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                            else
                            {
                                if (lastChar != Body.NullChar)
                                {
                                    if ((LastTerminalOutput = Terminal.Write(lastChar.ToString())) == null)
                                        DisplayErrorMsg(1140802, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                }
                            }
                        }
                        break;
                        default:
                        {
                            var lastDisplayRowIndex = model.Body?.GetLineCount() - 1 ?? Program.PosIntegerNotSet;
                            var lastDisplayColIndex = model.Body?.GetCharacterCountInLine()-1 ?? Program.PosIntegerNotSet;

                            if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + ((lastDisplayRowIndex < 0) ? 0 : lastDisplayRowIndex),
                                    KLineEditor.EditAreaMarginLeft + ((lastDisplayColIndex < 0) ? 0 : lastDisplayColIndex)) == false)
                                DisplayErrorMsg(1140901, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                        }
                        break;
                    }
                }
            }
        }
    }
}