using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Utils;
using MxReturnCode;

namespace KLineEdCmdApp.View.Base
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    [SuppressMessage("ReSharper", "RedundantTernaryExpression")]
    public abstract class EditAreaView : BaseView
    {
        public bool DisplayRulers { private set; get; }
        public char TopRuleUnitChar { protected set; get; }
        public char BottomRuleChar { protected set; get; }

        public string TopRule { protected set; get; }
        public string BottomRule { protected set; get; }
        public MxConsole.Color RuleForeGndColour { private set; get; }
        public MxConsole.Color RuleBackGndColour { private set; get; }
        public MxConsole.Color TextForeGndColour { private set; get; }
        public MxConsole.Color TextBackGndColour { private set; get; }

        public EditAreaView(IMxConsole console) : base(console)
        {
            DisplayRulers = false;
            TopRuleUnitChar = CmdLineParamsApp.ArgTextEditorRulersUnitCharDefault;
            BottomRuleChar = CmdLineParamsApp.ArgTextEditorRulersBotCharDefault;

            TopRule = "";
            BottomRule = "";

            RuleForeGndColour = MxConsole.Color.Gray;
            RuleBackGndColour = MxConsole.Color.Black;

            TextForeGndColour = MxConsole.Color.Gray;
            TextBackGndColour = MxConsole.Color.Black;
        }

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("EditAreaView.ApplySettings");

            if (param == null)
                rc.SetError(1170101, MxError.Source.Param, $"param is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                var rcBase = base.Setup(param);
                rc += rcBase;
                if (rcBase.IsSuccess(true))
                {       //todo apply ResetResult()
                    TextForeGndColour = param.ForeGndColourText; 
                    TextBackGndColour = param.BackGndColourText; 

                    RuleForeGndColour = param.ForeGndColourRule;   
                    RuleBackGndColour = param.BackGndColourRule; 
                    DisplayRulers = (param.TextEditorRulersShow == CmdLineParamsApp.BoolValue.Yes) ? true : false;
                    TopRuleUnitChar = param.TextEditorRulersUnitChar;
                    BottomRuleChar = param.TextEditorRulersBotChar;

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
            if (IsErrorState() == false)
            {
                var rcColour = Console.SetColour(TextForeGndColour, TextBackGndColour);
                if (rcColour.IsError(true))
                    SetErrorState(rcColour);
            }
        }

        public MxReturnCode<bool> InitDisplay()
        {
            var rc = new MxReturnCode<bool>("EditAreaView.InitDisplay");

            Console.SetCursorVisible(false);

            var rcRule = Console.SetColour(RuleForeGndColour, RuleBackGndColour);
            rc += rcRule;
            if(rcRule.IsSuccess(true))
            {
                var rcTop = DisplayLine(KLineEditor.EditAreaMarginTopRuleIndex, KLineEditor.EditAreaMarginLeft, TopRule);
                rc += rcTop;
                if (rcTop.IsSuccess(true))
                {
                    var rcBot = DisplayLine(KLineEditor.EditAreaMarginTopRuleIndex + EditAreaHeight + 1, KLineEditor.EditAreaMarginLeft, BottomRule);
                    rc += rcBot;
                    if (rcBot.IsSuccess(true))
                    {
                        var rcText = Console.SetColour(TextForeGndColour, TextBackGndColour);
                        rc += rcText;
                        if (rcText.IsSuccess(true))
                        {
                            for (int editRowIndex = 0; editRowIndex < EditAreaHeight; editRowIndex++)
                            {
                                //var blank = $"{editAreaRowIndex}";         ////see also BaseView.ApplySettings()
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
            Console.SetCursorVisible(CursorOn);

            return rc;
        }

        protected MxReturnCode<bool> SetEditAreaCursorPosition(int editAreaRowIndex = 0, int editAreaColIndex = 0)
        {
            var rc = new MxReturnCode<bool>("EditAreaView.SetEditAreaCursorPosition");

            if ((editAreaRowIndex < 0) || (editAreaRowIndex >= EditAreaHeight) || (editAreaColIndex < 0) || (editAreaColIndex >= WindowWidth - 1))
                rc.SetError(1170401, MxError.Source.Param, $"SetCursor= row{editAreaRowIndex} (max={EditAreaHeight}), col={editAreaColIndex} (max={WindowWidth - 1})", MxMsgs.MxErrBadMethodParam);
            else
            {
                var rcCursor = Console.SetCursorPosition(KLineEditor.EditAreaTopRowIndex + editAreaRowIndex, KLineEditor.EditAreaMarginLeft + editAreaColIndex);
                rc += rcCursor;
                if (rcCursor.IsSuccess(true))
                    rc.SetResult(true);
            }
            return rc;
        }

        protected MxReturnCode<CursorPosition> GetEditAreaCursorPosition(int chapterRowIndex, int chapterColIndex, int editAreaTopLineChapterIndex)
        {
            var rc = new MxReturnCode<CursorPosition>("EditAreaView.GetEditAreaCursorPosition");

            if ((EditAreaHeight < 0) || (chapterRowIndex < 0) ||  (chapterColIndex < 0) || (editAreaTopLineChapterIndex < 0) || (editAreaTopLineChapterIndex > chapterRowIndex))
                rc.SetError(1170501, MxError.Source.Param, $"row{chapterRowIndex} col={chapterColIndex}, editAreaTopLineChapterIndex={editAreaTopLineChapterIndex} (EditAreaHeight={EditAreaHeight})", MxMsgs.MxErrBadMethodParam);
            else
            {
                var editAreaRowIndex = chapterRowIndex - editAreaTopLineChapterIndex; 
                if ((editAreaRowIndex < 0) || (editAreaRowIndex > (EditAreaHeight-1)))
                    rc.SetError(1170502, MxError.Source.Program, $"editAreaRowIndex={editAreaRowIndex} EditAreaHeight={EditAreaHeight}", MxMsgs.MxErrInvalidCondition);
                else
                    rc.SetResult(new CursorPosition(editAreaRowIndex, chapterColIndex));
            }
            return rc;
        }
        public MxReturnCode<bool> DisplayEditAreaLine(int editRowIndex, string line, bool clear=true)
        {
            var rc = new MxReturnCode<bool>("EditAreaView.DisplayEditAreaLine");

            if ((line == null) || (editRowIndex < 0) || (editRowIndex >= EditAreaHeight))
                rc.SetError(1170601, MxError.Source.Param, $"line is null or row={editRowIndex} (max={EditAreaHeight})", MxMsgs.MxErrBadMethodParam);
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
                rc.SetError(1170701, MxError.Source.Param, $"word is null or row={editRowIndex} (max={EditAreaHeight}) or col={editColIndex} (max={EditAreaWidth})", MxMsgs.MxErrBadMethodParam);
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
