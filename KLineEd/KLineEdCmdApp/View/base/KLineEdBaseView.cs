using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using KLineEdCmdApp.Controller;
using MxReturnCode;

using KLineEdCmdApp.Utils;

namespace KLineEdCmdApp.View.Base
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "RedundantCaseLabel")]
    public abstract class KLineEdBaseView : ObserverView
    {
        public static readonly string ErrorModelNull = "\a";
        public static readonly string ErrorMsgPrecursor = "Error:";
        public static readonly string WarnMsgPrecursor = "Warn:";
        public static readonly string InfoMsgPrecursor = "Info:";

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
        public KLineEdBaseView(ITerminal terminal) : base()
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
            var rc = new MxReturnCode<bool>("KLineEdBaseView.Setup");

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

                WindowHeight = KLineEditor.CmdsHelpLineCount + KLineEditor.MsgLineCount + KLineEditor.EditAreaMarginTop + DisplayLinesHeight + KLineEditor.EditAreaMarginBottom + KLineEditor.StatusLineCount;
                WindowWidth = KLineEditor.EditAreaMarginLeft + DisplayLineWidth + KLineEditor.EditAreaMarginRight;

                if ((WindowWidth < KLineEditor.MinWindowWidth) || (WindowWidth > KLineEditor.MaxWindowWidth) || (WindowHeight > KLineEditor.MaxWindowHeight) || (WindowHeight < KLineEditor.MinWindowHeight))
                    rc.SetError(1110102, MxError.Source.User, $"param.DisplayLineWidth={param.DisplayLineWidth} (min={KLineEditor.MinWindowWidth}, max={KLineEditor.MaxWindowWidth}), param.DisplayLastLinesCnt{param.DisplayLastLinesCnt} (min={KLineEditor.MinWindowHeight}, max={KLineEditor.MinWindowHeight}", "MxErrInvalidSettingsFile");
                else
                {
                    BlankLine = BlankLine.PadLeft(WindowWidth);
                   // Ready = true;     //set in derived class
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public void DisplayErrorMsg(int errorNo, string msg)
        {
            DisplayMsg(MsgType.Error, $"Error: {errorNo} {msg}");
        }
        public void DisplayMxErrorMsg(string msg)
        {
            DisplayMsg(MsgType.Error, msg);
        }

        public bool DisplayMsg(MsgType msgType, string msg)
        {
            var rc = false;
            if (Terminal.SetCursorPosition(KLineEditor.MsgLineRow, KLineEditor.MsgLineLeftCol))
            {
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (msg == null)
                    LastTerminalOutput = Terminal.Write(MsgSetup(MsgType.Error, $"Error: 1110201 {Program.ValueNotSet}"));
                else
                    LastTerminalOutput = Terminal.Write(MsgSetup(msgType, GetTextForLine(msg, WindowWidth - KLineEditor.MsgLineLeftCol)));
                rc = true;
            }
            return rc;
        }

        private string MsgSetup(MsgType msgType, string msg)
        {
            var rc = Program.ValueNotSet;

            switch (msgType)
            {
                case MsgType.Clear:
                {
                    Terminal.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour);
                    Terminal.Write(BlankLine);
                }
                break;
                case MsgType.Error:
                {
                    Terminal.SetColour(MsgLineErrorForeGndColour, MsgLineErrorBackGndColour);
                    Terminal.Write(BlankLine);
                    if ((msg.StartsWith(KLineEdBaseView.ErrorMsgPrecursor) == false) && (msg.StartsWith(KLineEdBaseView.ErrorMsgPrecursor.ToLower()) == false))
                        rc = $"{KLineEdBaseView.ErrorMsgPrecursor} {msg}";
                    else
                        rc = msg;
                }
                break;
                case MsgType.Warning:
                {
                    Terminal.SetColour(MsgLineWarnForeGndColour, MsgLineWarnBackGndColour);
                    Terminal.Write(BlankLine);
                    if ((msg.StartsWith(KLineEdBaseView.WarnMsgPrecursor) == false) && (msg.StartsWith(KLineEdBaseView.WarnMsgPrecursor.ToLower()) == false))
                        rc = $"{KLineEdBaseView.WarnMsgPrecursor} {msg}";
                    else
                        rc = msg;
                }
                break;
                case MsgType.Info:
                default:
                {
                    Terminal.SetColour(MsgLineInfoForeGndColour, MsgLineInfoBackGndColour);
                    Terminal.Write(BlankLine);
                    if ((msg.StartsWith(KLineEdBaseView.InfoMsgPrecursor) == false) && (msg.StartsWith(KLineEdBaseView.InfoMsgPrecursor.ToLower()) == false))
                        rc = $"{KLineEdBaseView.InfoMsgPrecursor} {msg}";
                    else
                        rc = msg;
                }
                break;
            }
            return rc;
        }

        public string GetTextForLine(string text, int maxLength)
        {
            var rc = Program.ValueOverflow;
            if ((text != null) && (maxLength > 0) && (maxLength <= KLineEditor.MaxWindowWidth))
                rc = (text.Length <= maxLength) ? text : (text.Substring(0, maxLength - Program.ValueOverflow.Length) + Program.ValueOverflow);
            return rc;
        }
    }
}
