using System.Diagnostics.CodeAnalysis;
using MxReturnCode;

using KLineEdCmdApp.Utils;

namespace KLineEdCmdApp.View.Base
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "RedundantCaseLabel")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    [SuppressMessage("ReSharper", "SimplifyConditionalTernaryExpression")]
    [SuppressMessage("ReSharper", "RedundantTernaryExpression")]
    public abstract class BaseView : ObserverView, IErrorState
    {

        [SuppressMessage("ReSharper", "InconsistentNaming")]

        protected IMxConsole Console { get; }
        public int WindowHeight { private set; get; }
        public int WindowWidth { private set; get; }
        protected int EditAreaWidth { private set; get; }
        protected int EditAreaHeight { private set; get; }
        protected bool CursorOn { set; get; }
        protected string BlankLine { set; get; }

        protected MxConsole.Color MsgLineErrorForeGndColour { private set; get; }
        protected MxConsole.Color MsgLineErrorBackGndColour { private set; get; }
        // ReSharper disable once MemberCanBePrivate.Global
        protected MxConsole.Color MsgLineWarnForeGndColour { set; get; }
        // ReSharper disable once MemberCanBePrivate.Global
        protected MxConsole.Color MsgLineWarnBackGndColour { private set; get; }
        // ReSharper disable once MemberCanBePrivate.Global
        protected MxConsole.Color MsgLineInfoForeGndColour { private set; get; }
        // ReSharper disable once MemberCanBePrivate.Global
        protected MxConsole.Color MsgLineInfoBackGndColour { private set; get; }


        private MxReturnCode<bool> _mxErrorState;
        public bool IsErrorState() { return (_mxErrorState?.IsError(true) ?? false) ? true : false; }
        public MxReturnCode<bool> GetErrorState() { return _mxErrorState; }
        public void ResetErrorState() { _mxErrorState = null; }
        public bool SetErrorState(MxReturnCode<bool> mxErr)
        {
            var rc = false;

            if (_mxErrorState == null)
            {
                _mxErrorState = mxErr;
                rc = true;
            }
            return rc;
        }


        public bool Ready { protected set; get; }

        // ReSharper disable once RedundantBaseConstructorCall
        protected BaseView(IMxConsole console) : base()
        {
            Console = console;
            EditAreaWidth = Program.PosIntegerNotSet;
            EditAreaHeight = Program.PosIntegerNotSet;
            WindowHeight = Program.PosIntegerNotSet;
            WindowWidth = Program.PosIntegerNotSet;

            MsgLineErrorForeGndColour = MxConsole.Color.Red;
            MsgLineErrorBackGndColour = MxConsole.Color.Black;
            MsgLineWarnForeGndColour = MxConsole.Color.Yellow;
            MsgLineWarnBackGndColour = MxConsole.Color.Black;
            MsgLineInfoForeGndColour = MxConsole.Color.Gray;
            MsgLineInfoBackGndColour = MxConsole.Color.Black;

            BlankLine = "";
            CursorOn = false;
            _mxErrorState = null;

            Ready = false;
        }

        protected virtual void OnUpdateDone(MxReturnCode<bool> err, bool cursorOn)
        {
            CursorOn = cursorOn;
            Console.SetCursorVisible(CursorOn);
            if (err.IsError(true))
                SetErrorState(err);
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            var rc = new MxReturnCode<bool>($"{GetType().Name}.OnUpdate", false); //SetResult(true) on error

            CursorOn = false;
            Console.SetCursorVisible(CursorOn);

            rc.SetResult(true);

            if (rc.IsError(true))
                SetErrorState(rc);
        }

        public virtual MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("BaseView.ApplySettings");

            if (param == null)
                rc.SetError(1110101, MxError.Source.Param, $"param is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                EditAreaWidth = param.TextEditorDisplayCols;                    //todo rename param.DisplayLineWidth 
                EditAreaHeight = param.TextEditorDisplayRows;                //todo rename param.DisplayLastLinesCnt  

                MsgLineErrorForeGndColour = param.ForeGndColourMsgError;      
                MsgLineErrorBackGndColour = param.BackGndColourMsgError;    
                MsgLineWarnForeGndColour = param.ForeGndColourMsgWarn; 
                MsgLineWarnBackGndColour = param.BackGndColourMsgWarn;
                MsgLineInfoForeGndColour = param.ForeGndColourMsgInfo;
                MsgLineInfoBackGndColour = param.BackGndColourMsgInfo;

                WindowHeight = KLineEditor.HelpLineRowCount + KLineEditor.MsgLineRowCount + KLineEditor.EditAreaMarginTopRowCount + KLineEditor.EditAreaMarginTopRuleRowCount + EditAreaHeight + KLineEditor.EditAreaMarginBottomRowCount + KLineEditor.EditAreaMarginTopRuleRowCount + KLineEditor.StatusLineRowCount;
                WindowWidth = KLineEditor.EditAreaMarginLeft + EditAreaWidth + KLineEditor.EditAreaMarginRight;

                if ((WindowWidth < KLineEditor.MinWindowWidth) || (WindowWidth > KLineEditor.MaxWindowWidth) || (WindowHeight > KLineEditor.MaxWindowHeight) || (WindowHeight < KLineEditor.MinWindowHeight))
                    rc.SetError(1110102, MxError.Source.User, $"param.DisplayLineWidth={param.TextEditorDisplayCols} (min={KLineEditor.MinWindowWidth}, max={KLineEditor.MaxWindowWidth}), param.DisplayLastLinesCnt{param.TextEditorDisplayRows} (min={KLineEditor.MinWindowHeight}, max={KLineEditor.MinWindowHeight}", MxMsgs.MxErrInvalidSettingsFile);
                else
                {
                    //BlankLine = "0";  //see also EditAreaView.ClearEditAreaText()
                    //BlankLine = BlankLine.PadRight(WindowWidth-2, '.') + 'x';
                    BlankLine = BlankLine.PadLeft(WindowWidth - 1, ' ');
                    rc.SetResult(true); 
                }
            }
            return rc;
        }

        public MxReturnCode<bool> Close()
        {
            var rc = new MxReturnCode<bool>($"{GetType().Name}.Close");

            if (IsErrorState())
                rc += GetErrorState();

            return rc;
        }

        public MxReturnCode<bool> SetCursorPosition(int rowIndex, int colIndex)
        {
            var rc = new MxReturnCode<bool>("BaseView.SetCursorPosition");

            if ((rowIndex < 0) || (rowIndex >= WindowHeight) || ((colIndex < 0) || (colIndex >= (WindowWidth - Program.ValueOverflow.Length))))
                rc.SetError(1110201, MxError.Source.Param, $"rowIndex={rowIndex}, colIndex={colIndex}, WinHt={WindowHeight} WinWd={WindowWidth}", MxMsgs.MxErrBadMethodParam);
            else
            {
                var rcCursor = Console.SetCursorPosition(rowIndex, colIndex);
                rc += rcCursor;
                if (rcCursor.IsSuccess(true))
                    rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> ClearLine(int rowIndex, int colIndex)
        {
            var rc = new MxReturnCode<bool>("BaseView.ClearLine");

            if ((rowIndex < 0) || (rowIndex >= WindowHeight) || ((colIndex < 0) || (colIndex >= (WindowWidth - Program.ValueOverflow.Length))))
                rc.SetError(1110301, MxError.Source.Param, $"rowIndex={rowIndex}, colIndex={colIndex}, WinHt={WindowHeight} WinWd={WindowWidth}", MxMsgs.MxErrBadMethodParam);
            else
            {
                var rcCursorColStart = Console.SetCursorPosition(rowIndex, 0);
                rc += rcCursorColStart;
                if (rcCursorColStart.IsSuccess(true))
                {
                    var rcWrite = Console.WriteString(BlankLine);
                    rc += rcWrite;
                    if (rcWrite.IsSuccess(true))
                    {
                        var rcCursorIndex = Console.SetCursorPosition(rowIndex, colIndex);
                        rc += rcCursorIndex;
                        if (rcCursorIndex.IsSuccess(true))
                            rc.SetResult(true);
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> DisplayLine(int rowIndex, int colIndex, string text, bool clear=true)
        {
            var rc = new MxReturnCode<bool>("BaseView.DisplayLine");
             
            if ((text == null) || (rowIndex < 0) || (rowIndex >= WindowHeight) || ((colIndex < 0) || (colIndex >= (WindowWidth - Program.ValueOverflow.Length))))
                rc.SetError(1110401, MxError.Source.Param, $"rowIndex={rowIndex}, colIndex={colIndex}, WinHt={WindowHeight} WinWd={WindowWidth}", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (clear)
                    rc += ClearLine(rowIndex, colIndex);
                else
                    rc += SetCursorPosition(rowIndex, colIndex);

                if (rc.IsSuccess(true))
                {
                    var rcWrite = Console.WriteString(BaseView.TruncateTextForLine(text, WindowWidth - colIndex-1)); //column=WindowWidth-1 is the right most column, writing to next col generates a new line
                    if (rcWrite.IsError(true)) 
                        rc.SetError(1110402, MxError.Source.Data, $"window rowIndex={rowIndex}, colIndex={colIndex}, text={text}: error={rcWrite.GetErrorTechMsg()}", MxMsgs.MxErrInvalidCondition);
                    else
                    {
                        LastConsoleOutput = rcWrite.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> DisplayWord(int rowIndex, int colIndex, string word, bool setCursor = true)
        {
            var rc = new MxReturnCode<bool>("BaseView.DisplayWord");

            if ((word == null) || (rowIndex < 0) || (rowIndex >= WindowHeight) || ((colIndex < 0) || (colIndex >= (WindowWidth - Program.ValueOverflow.Length))))
                rc.SetError(1110501, MxError.Source.Param, $"rowIndex={rowIndex}, colIndex={colIndex}, WinHt={WindowHeight} WinWd={WindowWidth}", MxMsgs.MxErrBadMethodParam);
            else
            {
                rc += SetCursorPosition(rowIndex, colIndex);
                if (rc.IsSuccess(true))
                {
                    var rcWrite = Console.WriteString(BaseView.TruncateTextForLine(word, WindowWidth - colIndex - 1)); //column=WindowWidth-1 is the right most column, writing to next col generates a new line
                    if (rcWrite.IsError(true))
                        rc.SetError(1110502, MxError.Source.Data, $"window rowIndex={rowIndex}, colIndex={colIndex}, text={word}: error={rcWrite.GetErrorTechMsg()}", MxMsgs.MxErrInvalidCondition);
                    else
                    {
                        LastConsoleOutput = rcWrite.GetResult();
                        if (setCursor)
                        {
                            var nextColIndex = ((colIndex + word.Length) <= WindowWidth - 1) ? (colIndex + word.Length) : WindowWidth - 1;
                            rc += SetCursorPosition(rowIndex, nextColIndex);
                        }
                        if (rc.IsSuccess())
                            rc.SetResult(true);
                    }
                }
            }
            return rc;
        }

        public bool DisplayMsg(MxReturnCodeUtils.MsgClass msgClass, string msg)
        {
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (msg == null)
                DisplayLine(KLineEditor.MsgLineRowIndex, KLineEditor.MsgLineLeftCol, $"{MxReturnCodeUtils.ErrorMsgPrecursor}({1110201}) DisplayMsg is null", true);
            else
            {
                if (string.IsNullOrEmpty(msg))
                    ClearLine(KLineEditor.MsgLineRowIndex, KLineEditor.MsgLineLeftCol);
                else
                    DisplayLine(KLineEditor.MsgLineRowIndex, KLineEditor.MsgLineLeftCol, MsgSetup(msgClass, msg), true);
            }
            return true;
        }

        public static string TruncateTextForLine(string text, int maxLength)
        {
            var rc = Program.ValueOverflow;
            if ((text != null) && (maxLength > 0) && (maxLength <= KLineEditor.MaxWindowWidth) && (maxLength >= Program.ValueOverflow.Length))
            {
                if (text.Length <= maxLength)
                    rc = text;
                else
                    rc = text.Substring(0, maxLength - Program.ValueOverflow.Length) + Program.ValueOverflow;
            }
            return rc;
        }

        private string MsgSetup(MxReturnCodeUtils.MsgClass msgClass, string msg)
        {
            // ReSharper disable once RedundantAssignment
            var rc = Program.ValueNotSet;

            switch (msgClass)
            {
                case MxReturnCodeUtils.MsgClass.Error:
                {
                    Console.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour);
                    rc = msg;
                }
                break;
                case MxReturnCodeUtils.MsgClass.Warning:
                {
                    Console.SetColour(MsgLineWarnForeGndColour, MsgLineWarnBackGndColour);
                     rc = msg;
                }
                break;
                case MxReturnCodeUtils.MsgClass.Info:
                default:
                {
                    Console.SetColour(MsgLineInfoForeGndColour, MsgLineInfoBackGndColour);
                    rc = msg;
                }
                break;
            }
            return rc;
        }
    }
}
