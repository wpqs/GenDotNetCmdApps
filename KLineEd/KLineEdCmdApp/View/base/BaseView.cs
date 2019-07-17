using System;
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
    public abstract class BaseView : ObserverView
    {
        public static readonly string ErrorMsgPrecursor = "error";
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
        protected ITerminal Terminal { set; get; }
        public int WindowHeight { private set; get; }
        public int WindowWidth { private set; get; }
        public int EditAreaWidth { private set; get; }
        public int EditAreaHeight { private set; get; }

        protected string BlankLine { private set; get; }

        public ConsoleColor MsgLineErrorForeGndColour { private set; get; }
        public ConsoleColor MsgLineErrorBackGndColour { private set; get; }
        public ConsoleColor MsgLineWarnForeGndColour { private set; get; }
        public ConsoleColor MsgLineWarnBackGndColour { private set; get; }
        public ConsoleColor MsgLineInfoForeGndColour { private set; get; }
        public ConsoleColor MsgLineInfoBackGndColour { private set; get; }

        public bool Ready { protected set; get; }

        // ReSharper disable once RedundantBaseConstructorCall
        public BaseView(ITerminal terminal) : base()
        {
            Terminal = terminal;
            EditAreaWidth = Program.PosIntegerNotSet;
            EditAreaHeight = Program.PosIntegerNotSet;
            WindowHeight = Program.PosIntegerNotSet;
            WindowWidth = Program.PosIntegerNotSet;

            MsgLineErrorForeGndColour = ConsoleColor.Gray;
            MsgLineErrorBackGndColour = ConsoleColor.Black;
            MsgLineWarnForeGndColour = ConsoleColor.Gray;
            MsgLineWarnBackGndColour = ConsoleColor.Black;
            MsgLineInfoForeGndColour = ConsoleColor.Gray;
            MsgLineInfoBackGndColour = ConsoleColor.Black;

            BlankLine = "";

            Ready = false;
        }

        public virtual MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("BaseView.Setup");

            if (param == null)
                rc.SetError(1110101, MxError.Source.Param, $"param is null", "MxErrBadMethodParam");
            else
            {
                EditAreaWidth = param.DisplayLineWidth;                    //todo rename param.DisplayLineWidth 
                EditAreaHeight = param.DisplayLastLinesCnt;                //todo rename param.DisplayLastLinesCnt  

                MsgLineErrorForeGndColour = param.ForeGndSpellColour;     //todo rename param.MsgLineErrorForeGndColour   
                MsgLineErrorBackGndColour = param.BackGndSpellColour;     //todo rename param.MsgLineErrorForeGndColour 
                MsgLineWarnForeGndColour = ConsoleColor.Yellow;           //todo add param.MsgLineWarnForeGndColour
                MsgLineWarnBackGndColour = ConsoleColor.Black;            //todo add param.MsgLineWarnBackGndColour
                MsgLineInfoForeGndColour = ConsoleColor.Gray;             //todo add param.MsgLineInfoForeGndColour
                MsgLineInfoBackGndColour = ConsoleColor.Black;            //todo add param.MsgLineInfoBackGndColour 

                WindowHeight = KLineEditor.ModeHelpLineRowCount + KLineEditor.MsgLineRowCount + KLineEditor.EditAreaMarginTopRowCount + EditAreaHeight + KLineEditor.EditAreaMarginBottomRowCount + KLineEditor.StatusLineRowCount;
                WindowWidth = KLineEditor.EditAreaMarginLeft + EditAreaWidth + KLineEditor.EditAreaMarginRight;

                if ((WindowWidth < KLineEditor.MinWindowWidth) || (WindowWidth > KLineEditor.MaxWindowWidth) || (WindowHeight > KLineEditor.MaxWindowHeight) || (WindowHeight < KLineEditor.MinWindowHeight))
                    rc.SetError(1110102, MxError.Source.User, $"param.DisplayLineWidth={param.DisplayLineWidth} (min={KLineEditor.MinWindowWidth}, max={KLineEditor.MaxWindowWidth}), param.DisplayLastLinesCnt{param.DisplayLastLinesCnt} (min={KLineEditor.MinWindowHeight}, max={KLineEditor.MinWindowHeight}", "MxErrInvalidSettingsFile");
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

        public MxReturnCode<bool> SetCursorPosition(int rowIndex, int leftColIndex)
        {
            var rc = new MxReturnCode<bool>("BaseView.SetCursorPosition");

            if ((rowIndex < 0) || (rowIndex >= WindowHeight) || ((leftColIndex < 0) || (leftColIndex >= (WindowWidth - Program.ValueOverflow.Length))))
                rc.SetError(1110201, MxError.Source.Param, $"rowIndex={rowIndex}, leftColIndex={leftColIndex}, WinHt={WindowHeight} WinWd={WindowWidth}", "MxErrBadMethodParam");
            else
            {
                if (Terminal.SetCursorPosition(rowIndex, leftColIndex) == false)
                    rc.SetError(1110202, MxError.Source.Program, $"BaseView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                else
                    rc.SetResult(true);
            }
            return rc;
        }
        public MxReturnCode<bool> ClearLine(int rowIndex, int leftColIndex)
        {
            var rc = new MxReturnCode<bool>("BaseView.ClearLine");

            if ((rowIndex < 0) || (rowIndex >= WindowHeight) || ((leftColIndex < 0) || (leftColIndex >= (WindowWidth - Program.ValueOverflow.Length))))
                rc.SetError(1110301, MxError.Source.Param, $"rowIndex={rowIndex}, leftColIndex={leftColIndex}, WinHt={WindowHeight} WinWd={WindowWidth}", "MxErrBadMethodParam");
            else
            {
                if (Terminal.SetCursorPosition(rowIndex, 0) == false)
                    rc.SetError(1110302, MxError.Source.Program, $"BaseView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                else
                {
                    if (Terminal.Write(BlankLine) == null)
                        rc.SetError(1110303, MxError.Source.Program, $"BaseView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                    else
                    {
                        if (Terminal.SetCursorPosition(rowIndex, leftColIndex) == false)
                            rc.SetError(1110304, MxError.Source.Program, $"BaseView: {Terminal.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                        else
                        {
                            rc.SetResult(true);
                        }
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> DisplayLine(int rowIndex, int leftColIndex, string text, bool clear=true)
        {
            var rc = new MxReturnCode<bool>("BaseView.DisplayLine");
             
            if ((text == null) || (rowIndex < 0) || (rowIndex >= WindowHeight) || ((leftColIndex < 0) || (leftColIndex >= (WindowWidth - Program.ValueOverflow.Length))))
                rc.SetError(1110401, MxError.Source.Param, $"rowIndex={rowIndex}, leftColIndex={leftColIndex}, WinHt={WindowHeight} WinWd={WindowWidth}", "MxErrBadMethodParam");
            else
            {
                if (clear)
                    rc += ClearLine(rowIndex, leftColIndex);
                else
                    rc += SetCursorPosition(rowIndex, leftColIndex);

                if (rc.IsSuccess(true))
                {
                    if ((LastTerminalOutput = Terminal.Write(BaseView.TruncateTextForLine(text, WindowWidth - leftColIndex))) == null)
                        rc.SetError(1110402, MxError.Source.Data, $"rowIndex={rowIndex}, leftColIndex={leftColIndex}, text={text}", "MxErrInvalidCondition");
                    else
                    {
                        rc.SetResult(true);
                    }
                }
            }
            return rc;
        }
       
        public static bool IsCriticalError(string displayMsg)
        {
            var rc = true;
            if (displayMsg == null)
                rc = false;
            else
            {                                                                                           //"error 1010102-exception: msg"
                if (displayMsg.StartsWith(BaseView.ErrorMsgPrecursor) == false)
                    rc = false;
                else
                {
                    int startIndex = displayMsg.IndexOf('-');
                    if (startIndex != -1)
                    {
                        int endIndex = displayMsg.IndexOf(':', startIndex+1);
                        var errType = displayMsg.Snip(startIndex+1, endIndex - 1);
                        if (errType != null)
                        {
                            if (errType == MxDotNetUtilsLib.EnumOps.XlatToString(ErrorType.user))
                                rc = false;
                        }
                    }
                }
            }
            return rc;
        }

        public static string FormatMxErrorMsg(int errorNo, ErrorType errType, string errMsg) //"error 1010102-exception: msg"
        {
            return $"{ BaseView.ErrorMsgPrecursor} {errorNo}-{errType}: {errMsg ?? Program.ValueNotSet}";
        }

        public void DisplayErrorMsg(int errorNo, ErrorType errType, string msg)
        {
            DisplayMsg(MsgType.Error, BaseView.FormatMxErrorMsg(errorNo, errType, msg));  //msg is formatted as "error 1010102-exception: msg"
        }
        public void DisplayMxErrorMsg(string msg)                                //msg is formatted as "error 1010102-exception: msg"
        {
            DisplayMsg(MsgType.Error, msg);
        }

        public bool DisplayMsg(MsgType msgType, string msg)
        {
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (msg == null)
                DisplayLine(KLineEditor.MsgLineRowIndex, KLineEditor.MsgLineLeftCol, BaseView.FormatMxErrorMsg(1110201, ErrorType.program, "DisplayMsg is null"), true);
            else
                DisplayLine(KLineEditor.MsgLineRowIndex, KLineEditor.MsgLineLeftCol, MsgSetup(msgType, msg), true);

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

        private string MsgSetup(MsgType msgType, string msg)
        {
            // ReSharper disable once RedundantAssignment
            var rc = Program.ValueNotSet;

            switch (msgType)
            {
                case MsgType.Error:
                {
                    Terminal.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour);
                    if ((msg.StartsWith(BaseView.ErrorMsgPrecursor) == false) && (msg.StartsWith(BaseView.ErrorMsgPrecursor.ToLower()) == false))
                        rc = $"{BaseView.ErrorMsgPrecursor} {msg}";
                    else
                        rc = msg;
                }
                break;
                case MsgType.Warning:
                {
                    Terminal.SetColour(MsgLineWarnForeGndColour, MsgLineWarnBackGndColour);
                    if ((msg.StartsWith(BaseView.WarnMsgPrecursor) == false) && (msg.StartsWith(BaseView.WarnMsgPrecursor.ToLower()) == false))
                        rc = $"{BaseView.WarnMsgPrecursor} {msg}";
                    else
                        rc = msg;
                }
                break;
                case MsgType.Info:
                default:
                {
                    Terminal.SetColour(MsgLineInfoForeGndColour, MsgLineInfoBackGndColour);
                    if ((msg.StartsWith(BaseView.InfoMsgPrecursor) == false) && (msg.StartsWith(BaseView.InfoMsgPrecursor.ToLower()) == false))
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
