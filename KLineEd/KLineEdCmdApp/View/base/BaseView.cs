using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
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
    public abstract class BaseView : ObserverView
    {
        public static readonly string ErrorMsgPrecursor = "error:";
        public static readonly string WarnMsgPrecursor = "warn:";
        public static readonly string InfoMsgPrecursor = "info:";

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum ErrorType
        {
            [EnumMember(Value = "exception")] exception = 0,
            [EnumMember(Value = "program")] program = 1,
            [EnumMember(Value = "data")] data = 2,
            [EnumMember(Value = "param")]param = 3,
            [EnumMember(Value = "user")] user = 4
        }

        public enum MsgType
        {
            [EnumMember(Value = "Error")] Error = 0,        
            [EnumMember(Value = "Warning")] Warning = 1,  
            [EnumMember(Value = "Info")] Info= 2,
            [EnumMember(Value = "Unknown")] Unknown =3
        }
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

        private MxReturnCode<bool> _mxErrorCode;

        public bool IsOnUpdateError() { return (_mxErrorCode?.GetResult() ?? false) ? false : true; }
        public MxError.Source GetErrorSource() { return _mxErrorCode?.GetErrorType() ?? MxError.Source.Program; }
        public int GetErrorNo() { return _mxErrorCode?.GetErrorCode() ?? Program.PosIntegerNotSet; }
        public string GetErrorTechMsg() { return _mxErrorCode?.GetErrorTechMsg() ?? Program.ValueNotSet; }
        public string GetErrorUserMsg() { return _mxErrorCode?.GetErrorUserMsg() ?? Program.ValueNotSet; }

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
            _mxErrorCode = null; 

            Ready = false;
        }

        protected virtual void OnUpdateDone(MxReturnCode<bool> errorCode, bool cursorOn)
        {
            CursorOn = cursorOn;
            Console.SetCursorVisible(CursorOn);
            if (errorCode.IsError(true))
                DisplayErrorMsg(errorCode);
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            _mxErrorCode = new MxReturnCode<bool>($"{GetType().Name}.OnUpdate", false); //SetResult(true) on error

            CursorOn = false;
            Console.SetCursorVisible(CursorOn);

            if (Console.IsError())
                SetMxError(1140201, Console.GetErrorSource(), $"MxConsole: {Console.GetErrorTechMsg()}", Console.GetErrorUserMsg());
            else
                _mxErrorCode?.SetResult(true);
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

            if (_mxErrorCode.IsError())
                rc += _mxErrorCode;

            return rc;
        }

        public MxReturnCode<MxReturnCode<bool>> GetMxError()
        {
            var rc = new MxReturnCode<MxReturnCode<bool>>($"{GetType().Name}.GetMxError");

            rc += _mxErrorCode;

            return rc;
        }

        protected bool SetMxError(int errorCode, MxError.Source source, string errMsgTech, string errMsgUser)
        {       //use only in base classes, concrete derived classes use local MxResult variable like EditorHelpLineView, TextEditView, etc
            var rc = false;

            if ((_mxErrorCode != null) && (errMsgTech != null) && (errMsgUser != null))
            {
                _mxErrorCode.SetError(errorCode, source, errMsgTech, errMsgUser);
                rc = true;
            }
            return rc;
        }

        public MxReturnCode<bool> SetCursorPosition(int rowIndex, int colIndex)
        {
            var rc = new MxReturnCode<bool>("BaseView.SetCursorPosition");

            if ((rowIndex < 0) || (rowIndex >= WindowHeight) || ((colIndex < 0) || (colIndex >= (WindowWidth - Program.ValueOverflow.Length))))
                rc.SetError(1110201, MxError.Source.Param, $"rowIndex={rowIndex}, colIndex={colIndex}, WinHt={WindowHeight} WinWd={WindowWidth}", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (Console.SetCursorPosition(rowIndex, colIndex) == false)
                    rc.SetError(1110202, Console.GetErrorSource(), $"BaseView: {Console.GetErrorTechMsg()}", Console.GetErrorUserMsg());
                else
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
                if (Console.SetCursorPosition(rowIndex, 0) == false)
                    rc.SetError(1110302, Console.GetErrorSource(), Console.GetErrorTechMsg(), Console.GetErrorUserMsg());
                else
                {
                    if (Console.Write(BlankLine) == null)
                        rc.SetError(1110303, Console.GetErrorSource(), Console.GetErrorTechMsg(), Console.GetErrorUserMsg());
                    else
                    {
                        if (Console.SetCursorPosition(rowIndex, colIndex) == false)
                            rc.SetError(1110304, Console.GetErrorSource(), Console.GetErrorTechMsg(), Console.GetErrorUserMsg());
                        else
                        {
                            rc.SetResult(true);
                        }
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
                    if ((LastConsoleOutput = Console.Write(BaseView.TruncateTextForLine(text, WindowWidth - colIndex-1))) == null) //column=WindowWidth-1 is the right most column, writing to next col generates a new line
                        rc.SetError(1110402, MxError.Source.Data, $"window rowIndex={rowIndex}, colIndex={colIndex}, text={text}", MxMsgs.MxErrInvalidCondition);
                    else
                    {
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
                    if ((LastConsoleOutput = Console.Write(BaseView.TruncateTextForLine(word, WindowWidth - colIndex - 1))) == null) //column=WindowWidth-1 is the right most column, writing to next col generates a new line
                        rc.SetError(1110502, MxError.Source.Data, $"rowIndex={rowIndex}, colIndex={colIndex}, text={word}", MxMsgs.MxErrInvalidCondition);
                    else
                    {
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

        public void DisplayErrorMsg(MxReturnCode<bool> err)
        {
            var errNo = err.GetErrorCode(); 
            var errType = err.GetErrorType(); //xlat MxError.Source to BaseView.ErrorType
            var errMsg = err.GetErrorUserMsg(); //trim before ': ' 
           // var msg = FormatMxErrorMsg(errNo, errType, errMsg);

            DisplayMsg(MsgType.Error, err.GetErrorUserMsg());  
        }

        public bool DisplayMsg(MsgType msgType, string msg)
        {
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (msg == null)
                DisplayLine(KLineEditor.MsgLineRowIndex, KLineEditor.MsgLineLeftCol, $"{ BaseView.ErrorMsgPrecursor} {1110201}-{ErrorType.program}: DisplayMsg is null", true);
            else
            {
                if (string.IsNullOrEmpty(msg))
                    ClearLine(KLineEditor.MsgLineRowIndex, KLineEditor.MsgLineLeftCol);
                else
                    DisplayLine(KLineEditor.MsgLineRowIndex, KLineEditor.MsgLineLeftCol, MsgSetup(msgType, msg), true);
            }
            return true;
        }

        public static string FormatMxErrorMsg(int errorNo, ErrorType errType, string errMsg) //"error 1010102-exception: msg"
        {
            return $"{ BaseView.ErrorMsgPrecursor} {errorNo}-{errType}: {errMsg ?? Program.ValueNotSet}";
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

        private string MsgSetup(MsgType msgType, string msg)
        {
            // ReSharper disable once RedundantAssignment
            var rc = Program.ValueNotSet;

            switch (msgType)
            {
                case MsgType.Error:
                {
                    Console.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour);
                    if ((msg.Length > 0) && (msg.StartsWith(BaseView.ErrorMsgPrecursor) == false) && (msg.StartsWith(BaseView.ErrorMsgPrecursor.ToLower()) == false))
                        rc = $"{BaseView.ErrorMsgPrecursor} {msg}";
                    else
                        rc = msg;
                }
                break;
                case MsgType.Warning:
                {
                    Console.SetColour(MsgLineWarnForeGndColour, MsgLineWarnBackGndColour);
                    if ((msg.Length > 0) && (msg.StartsWith(BaseView.WarnMsgPrecursor) == false) && (msg.StartsWith(BaseView.WarnMsgPrecursor.ToLower()) == false))
                        rc = $"{BaseView.WarnMsgPrecursor} {msg}";
                    else
                        rc = msg;
                }
                break;
                case MsgType.Info:
                default:
                {
                    Console.SetColour(MsgLineInfoForeGndColour, MsgLineInfoBackGndColour);
                    if ((msg.Length > 0) && (msg.StartsWith(BaseView.InfoMsgPrecursor) == false) && (msg.StartsWith(BaseView.InfoMsgPrecursor.ToLower()) == false))
                        rc = $"{BaseView.InfoMsgPrecursor} {msg}";
                    else
                        rc = msg;
                }
                break;
            }
            return rc;
        }
    }
}
