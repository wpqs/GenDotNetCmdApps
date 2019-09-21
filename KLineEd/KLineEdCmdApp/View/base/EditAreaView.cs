using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Utils;
using MxReturnCode;

namespace KLineEdCmdApp.View.Base
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public abstract class EditAreaView : BaseView
    {
        public bool DisplayRulers { private set; get; }
        public string TopRule { protected set; get; }
        public string BottomRule { protected set; get; }
        public ConsoleColor RuleForeGndColour { private set; get; }
        public ConsoleColor RuleBackGndColour { private set; get; }
        public ConsoleColor TextForeGndColour { private set; get; }
        public ConsoleColor TextBackGndColour { private set; get; }

        public EditAreaView(ITerminal terminal) : base(terminal)
        {
            TopRule = "";
            BottomRule = "";

            RuleForeGndColour = ConsoleColor.Gray;
            RuleBackGndColour = ConsoleColor.Black;

            TextForeGndColour = ConsoleColor.Gray;
            TextBackGndColour = ConsoleColor.Black;
        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("EditAreaView.Setup");

            if (param == null)
                rc.SetError(1140101, MxError.Source.Param, $"param is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                var rcBase = base.Setup(param);
                rc += rcBase;
                if (rcBase.IsSuccess(true))
                {       //todo apply ResetResult()
                    TextForeGndColour = ConsoleColor.Green; // param.ForeGndColourText; //todo rename param.EditAreaForeGndColour  
                    TextBackGndColour = ConsoleColor.Black; // param.BackGndColourText; //todo rename param.EditAreaBackGndColour 

                    RuleForeGndColour = ConsoleColor.DarkGreen; // param.ForeGndColourText; //todo rename param.RuleForeGndColour  
                    RuleBackGndColour = ConsoleColor.Black; // param.BackGndColourText; //todo rename param.RuleBackGndColour 
                    DisplayRulers = true;  //param.EditAreaRulersDisplay;

                    if (SetRulers(EditAreaWidth-1))
                    {
                        Ready = true;
                        rc.SetResult(true);
                    }

                }
            }
            return rc;
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            base.OnUpdate(notificationItem);
            if (IsOnUpdateError() == false)
            {
                if (Terminal.SetColour(TextForeGndColour, TextBackGndColour) == false)
                    SetMxError(1140202, Terminal.GetErrorSource(), $"Terminal: {Terminal.GetErrorTechMsg()}", Terminal.GetErrorUserMsg());
            }
        }

        public MxReturnCode<bool> InitDisplay()
        {
            var rc = new MxReturnCode<bool>("EditAreaView.InitDisplay");

            Terminal.SetCursorVisible(false);

            if (Terminal.SetColour(RuleForeGndColour, RuleBackGndColour) == false)
                rc.SetError(1140301, Terminal.GetErrorSource(), $"TextEditView: {Terminal.GetErrorTechMsg()}", Terminal.GetErrorUserMsg());
            else
            {
                var rcTop = DisplayLine(KLineEditor.EditAreaMarginTopRuleIndex, KLineEditor.EditAreaMarginLeft, TopRule);
                rc += rcTop;
                if (rcTop.IsSuccess(true))
                {
                    var rcBot = DisplayLine(KLineEditor.EditAreaMarginTopRuleIndex + EditAreaHeight + 1, KLineEditor.EditAreaMarginLeft, BottomRule);
                    rc += rcBot;
                    if (rcBot.IsSuccess(true))
                    {
                        if (Terminal.SetColour(TextForeGndColour, TextBackGndColour) == false)
                            rc.SetError(1140302, Terminal.GetErrorSource(), $"EditAreaView: {Terminal.GetErrorTechMsg()}", Terminal.GetErrorUserMsg());
                        else
                        {
                            for (int editRowIndex = 0; editRowIndex < EditAreaHeight; editRowIndex++)
                            {
                                //var blank = $"{editAreaRowIndex}";         ////see also BaseView.Setup()
                                //blank = blank.PadRight(EditAreaWidth - 2, ',');
                                //blank += "o";
                                //DisplayLine(KLineEditor.EditAreaTopRowIndex+editAreaRowIndex, KLineEditor.EditAreaMarginLeft, blank, true);

                                var rcClear = ClearLine(KLineEditor.EditAreaTopRowIndex + editRowIndex, 0);
                                rc += rcClear;
                                if (rcClear.IsError(true))
                                    break;
                            }
                            if (rc.IsSuccess())
                                rc.SetResult(true);
                        }
                    }
                }
            }
            Terminal.SetCursorVisible(CursorOn);

            return rc;
        }

        protected MxReturnCode<bool> SetEditAreaCursorPosition(int editAreaRowIndex = 0, int editAreaColIndex = 0)
        {
            var rc = new MxReturnCode<bool>("EditAreaView.SetEditAreaCursorPosition");

            if ((editAreaRowIndex < 0) || (editAreaRowIndex >= EditAreaHeight) || (editAreaColIndex < 0) || (editAreaColIndex >= WindowWidth - 1))
                rc.SetError(1140401, MxError.Source.Param, $"SetCursor= row{editAreaRowIndex} (max={EditAreaHeight}), col={editAreaColIndex} (max={WindowWidth - 1})", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (Terminal.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + editAreaRowIndex, KLineEditor.EditAreaMarginLeft + editAreaColIndex) == false)
                    rc.SetError(1110402, Terminal.GetErrorSource(), Terminal.GetErrorTechMsg(), Terminal.GetErrorUserMsg());
                else
                    rc.SetResult(true);
            }
            return rc;
        }

        protected MxReturnCode<CursorPosition> GetEditAreaCursorPosition(int chapterRowIndex, int chapterColIndex, int editAreaTopLineChapterIndex)
        {
            var rc = new MxReturnCode<CursorPosition>("EditAreaView.GetEditAreaCursorPosition");

            if ((EditAreaHeight < 0) || (chapterRowIndex < 0) ||  (chapterColIndex < 0) || (editAreaTopLineChapterIndex < 0) || (editAreaTopLineChapterIndex > chapterRowIndex))
                rc.SetError(1140501, MxError.Source.Param, $"row{chapterRowIndex} col={chapterColIndex}, editAreaTopLineChapterIndex={editAreaTopLineChapterIndex} (EditAreaHeight={EditAreaHeight})", MxMsgs.MxErrBadMethodParam);
            else
            {
                var editAreaRowIndex = chapterRowIndex - editAreaTopLineChapterIndex; 
                if ((editAreaRowIndex < 0) || (editAreaRowIndex > (EditAreaHeight-1)))
                    rc.SetError(1140502, MxError.Source.Program, $"editAreaRowIndex={editAreaRowIndex} EditAreaHeight={EditAreaHeight}", MxMsgs.MxErrInvalidCondition);
                else
                    rc.SetResult(new CursorPosition(editAreaRowIndex, chapterColIndex));
            }
            return rc;
        }
        public MxReturnCode<bool> DisplayEditAreaLine(int editRowIndex, string line, bool clear=true)
        {
            var rc = new MxReturnCode<bool>("EditAreaView.DisplayEditAreaLine");

            if ((line == null) || (editRowIndex < 0) || (editRowIndex >= EditAreaHeight))
                rc.SetError(1140601, MxError.Source.Param, $"line is null or row={editRowIndex} (max={EditAreaHeight})", MxMsgs.MxErrBadMethodParam);
            else
            {
                rc += DisplayLine(KLineEditor.EditAreaTopRowIndex + editRowIndex, KLineEditor.EditAreaMarginLeft, line, clear);
                if (rc.IsSuccess(true))
                {
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> DisplayEditAreaWord(int editRowIndex, int editColIndex, string word)
        {
            var rc = new MxReturnCode<bool>("EditAreaView.DisplayEditAreaWord");

            if ((word == null) || (editRowIndex < 0) || (editRowIndex >= EditAreaHeight) || (editColIndex < 0) || (editColIndex >= EditAreaWidth))
                rc.SetError(1140701, MxError.Source.Param, $"word is null or row={editRowIndex} (max={EditAreaHeight}) or col={editColIndex} (max={EditAreaWidth})", MxMsgs.MxErrBadMethodParam);
            else
            {
                rc += DisplayWord(KLineEditor.EditAreaTopRowIndex + editRowIndex, KLineEditor.EditAreaMarginLeft + editColIndex, word, true);
                if (rc.IsSuccess())
                    rc.SetResult(true);
            }
            return rc;
        }
        public MxReturnCode<bool> DisplayEditAreaChar(int editRowIndex, int editColIndex, char c)
        {
            var rc = new MxReturnCode<bool>("EditAreaView.DisplayEditAreaChar");

            rc += DisplayEditAreaWord(editRowIndex, editColIndex, c.ToString());
            if (rc.IsSuccess(true))
                rc.SetResult(true);

            return rc;
        }
        protected virtual bool SetRulers(int maxColIndex)
        {
            var rc = false;

            if ((maxColIndex > 0) && (maxColIndex < BlankLine.Length))
            {
                TopRule = BlankLine.Snip(0, maxColIndex);
                if (TopRule != null)
                {
                    BottomRule = TopRule;
                    rc = true;
                }
            }
            return rc;
        }
    }
}
