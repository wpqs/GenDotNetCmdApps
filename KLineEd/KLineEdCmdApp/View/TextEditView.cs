using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Controller;
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

    public class TextEditView : KLineEdBaseView   
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
                int lineCnt = 0;
                for (; lineCnt < DisplayLinesHeight; lineCnt++)
                {
                    if (Terminal.SetCursorPosition(KLineEditor.CmdsHelpLineRow + lineCnt, KLineEditor.CmdsHelpLineLeftCol) == false)
                        rc.SetError(1140201, MxError.Source.Program, $"TextEditView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                    else
                    {
                        Terminal.Write(BlankLine);
                        lineCnt++;
                    }
                    if (Terminal.IsError())
                        break;
                }
                if (Terminal.IsError() || (lineCnt != DisplayLinesHeight))
                    rc.SetError(1140203, MxError.Source.Program, $"TextEditView: Line={lineCnt}, {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                else
                    rc.SetResult(true);
            }
  
            return rc;
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            ChapterModel model = notificationItem.Data as ChapterModel;
            if (model == null)
                DisplayErrorMsg(1140301, "Program Error. Unable to access data needed for display. Please quit and report this problem.");
            else
            {
                if (Terminal.SetColour(EditAreaForeGndColour, EditAreaBackGndColour) == false)
                    DisplayErrorMsg(1140302, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                else
                {
                    ChapterModel.ChangeHint change = (ChapterModel.ChangeHint) notificationItem.Change;
                    switch (change)
                    {
                        case ChapterModel.ChangeHint.All:
                        {
                            var rcRes = model.GetLastLinesForDisplay(DisplayLinesHeight);
                            if (rcRes.IsError(true))
                                DisplayMxErrorMsg(rcRes.GetErrorUserMsg());
                            else
                            {
                                var row = 0;
                                var lastDisplayRow = model.Body?.LastDisplayRowIndex ?? 0;
                                var lines = rcRes.GetResult();
                                foreach (var line in lines)
                                {
                                    if (row <= lastDisplayRow)
                                    {
                                        if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRow + row, KLineEditor.EditAreaMarginLeft) == false)
                                            DisplayErrorMsg(1140303, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                        else
                                        {
                                            if ((LastTerminalOutput = Terminal.Write(line)) == null)
                                                DisplayErrorMsg(1140304, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                        }
                                        if (Terminal.IsError())
                                            break;
                                    }
                                    row++;
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
                                var lastDisplayRow = model.Body?.LastDisplayRowIndex ?? 0;
                                var lines = rcRes.GetResult();
                                foreach (var line in lines)
                                {
                                    if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRow + lastDisplayRow, KLineEditor.EditAreaMarginLeft) == false)
                                        DisplayErrorMsg(1140305, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                    else
                                    {
                                        if((LastTerminalOutput = Terminal.Write(line)) == null)
                                            DisplayErrorMsg(1140306, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                        case ChapterModel.ChangeHint.Word:
                        {
                            var lastDisplayRow = model.Body?.LastDisplayRowIndex ?? 0;
                            var lastDisplayCol = model.Body?.LastDisplayColumnIndex ?? 0;
                            var lastWord = model.Body?.GetWordInLine() ?? Body.WordNotSet;

                            if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRow + lastDisplayRow, KLineEditor.EditAreaMarginLeft + lastDisplayCol) == false)
                                DisplayErrorMsg(1140307, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                            else
                            {
                                if ((LastTerminalOutput = Terminal.Write(lastWord)) == null)
                                    DisplayErrorMsg(1140308, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                            }
                        }
                        break;
                        case ChapterModel.ChangeHint.Char:
                        {
                            var lastDisplayRow = model.Body?.LastDisplayRowIndex ?? 0;
                            var lastDisplayCol = model.Body?.LastDisplayColumnIndex ?? 0;
                            var lastChar = model.Body?.GetCharInLine(lastDisplayRow, lastDisplayCol) ?? Body.NullChar;

                            if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRow + lastDisplayRow, KLineEditor.EditAreaMarginLeft + lastDisplayCol) == false)
                                DisplayErrorMsg(1140309, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                            else
                            {
                                if ((LastTerminalOutput = Terminal.Write(lastChar.ToString())) == null)
                                    DisplayErrorMsg(1140310, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                            }
                        }
                        break;
                        default:
                        {
                            var row = model.Body?.LastDisplayRowIndex ?? 0;
                            var col = model.Body?.GetCharInLine() ?? 0;
                            if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRow + row, KLineEditor.EditAreaMarginLeft + col) == false)
                                DisplayErrorMsg(1140311, $"Program Error. Details: {Terminal.ErrorMsg ?? Program.ValueNotSet}. Please quit and report this problem.");
                        }
                        break;
                    }
                }
            }
        }
    }
}