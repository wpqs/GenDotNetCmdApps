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
            [EnumMember(Value = "Clear")] Clear = 3,
            [EnumMember(Value = "Unknown")] Unknown =4
        }
        protected ITerminal Terminal { set; get; }
        public int WindowHeight { private set; get; }
        public int WindowWidth { private set; get; }
        public int DisplayLineWidth { private set; get; }
        public int DisplayLinesHeight { private set; get; }

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
            DisplayLineWidth = Program.PosIntegerNotSet;
            DisplayLinesHeight = Program.PosIntegerNotSet;
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
                DisplayLineWidth = param.DisplayLineWidth;
                DisplayLinesHeight = param.DisplayLastLinesCnt;

                MsgLineErrorForeGndColour = param.ForeGndSpellColour;     //todo rename param.MsgLineErrorForeGndColour   
                MsgLineErrorBackGndColour = param.BackGndSpellColour;     //todo rename param.MsgLineErrorForeGndColour 
                MsgLineWarnForeGndColour = ConsoleColor.Yellow;           //todo add param.MsgLineWarnForeGndColour
                MsgLineWarnBackGndColour = ConsoleColor.Black;            //todo add param.MsgLineWarnBackGndColour
                MsgLineInfoForeGndColour = ConsoleColor.Gray;             //todo add param.MsgLineInfoForeGndColour
                MsgLineInfoBackGndColour = ConsoleColor.Black;            //todo add param.MsgLineInfoBackGndColour 

                WindowHeight = KLineEditor.ModeHelpLineRowCount + KLineEditor.MsgLineRowCount + KLineEditor.EditAreaMarginTopRowCount + DisplayLinesHeight + KLineEditor.EditAreaMarginBottomRowCount + KLineEditor.StatusLineRowCount;
                WindowWidth = KLineEditor.EditAreaMarginLeft + DisplayLineWidth + KLineEditor.EditAreaMarginRight;

                if ((WindowWidth < KLineEditor.MinWindowWidth) || (WindowWidth > KLineEditor.MaxWindowWidth) || (WindowHeight > KLineEditor.MaxWindowHeight) || (WindowHeight < KLineEditor.MinWindowHeight))
                    rc.SetError(1110102, MxError.Source.User, $"param.DisplayLineWidth={param.DisplayLineWidth} (min={KLineEditor.MinWindowWidth}, max={KLineEditor.MaxWindowWidth}), param.DisplayLastLinesCnt{param.DisplayLastLinesCnt} (min={KLineEditor.MinWindowHeight}, max={KLineEditor.MinWindowHeight}", "MxErrInvalidSettingsFile");
                else
                {
                   // BlankLine = BlankLine.PadLeft(WindowWidth-2, '.') + 'x';
                    BlankLine = BlankLine.PadLeft(WindowWidth - 1, ' ');
                    rc.SetResult(true); 
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
                LastTerminalOutput = Terminal.Write(MsgSetup(MsgType.Error, BaseView.TruncateTextForLine(BaseView.FormatMxErrorMsg(1110201, ErrorType.program, "DisplayMsg is null"), WindowWidth - KLineEditor.MsgLineLeftCol)));
            else
                LastTerminalOutput = Terminal.Write(MsgSetup(msgType, BaseView.TruncateTextForLine(msg, WindowWidth - KLineEditor.MsgLineLeftCol)));
  
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

            Terminal.SetCursorPosition(KLineEditor.MsgLineRowIndex, 0);

            switch (msgType)
            {
                case MsgType.Clear:
                {
                    Terminal.SetColour(MsgLineInfoForeGndColour, MsgLineInfoBackGndColour);
                    Terminal.Write(BlankLine);
                    rc = "";
                }
                break;
                case MsgType.Error:
                {
                    Terminal.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour);
                    Terminal.Write(BlankLine);
                    if ((msg.StartsWith(BaseView.ErrorMsgPrecursor) == false) && (msg.StartsWith(BaseView.ErrorMsgPrecursor.ToLower()) == false))
                        rc = $"{BaseView.ErrorMsgPrecursor} {msg}";
                    else
                        rc = msg;
                }
                break;
                case MsgType.Warning:
                {
                    Terminal.SetColour(MsgLineWarnForeGndColour, MsgLineWarnBackGndColour);
                    Terminal.Write(BlankLine);
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
                    Terminal.Write(BlankLine);
                    if ((msg.StartsWith(BaseView.InfoMsgPrecursor) == false) && (msg.StartsWith(BaseView.InfoMsgPrecursor.ToLower()) == false))
                        rc = $"{BaseView.InfoMsgPrecursor} {msg}";
                    else
                        rc = msg;
                }
                break;
            }
            Terminal.SetCursorPosition(KLineEditor.MsgLineRowIndex, KLineEditor.MsgLineLeftCol);

            return rc;
        }
    }
}
