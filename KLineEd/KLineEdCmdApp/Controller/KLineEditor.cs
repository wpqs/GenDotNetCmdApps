using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Model.Base;
using KLineEdCmdApp.Utils.Properties;
using MxReturnCode;

// ReSharper disable once CheckNamespace
namespace KLineEdCmdApp.Utils
{
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    public class KLineEditor
    {
        public static readonly string ReportSectionDottedLine = $"{Environment.NewLine}....................................................{Environment.NewLine}";

        public static readonly int ScreenMarginLeft = 10;
        public static readonly int ScreenMarginRight = 20;
        public static readonly int ScreenMarginTop = 10;
        public static readonly int ScreenMarginBottom = 10;

        public static readonly int CmdLineCnt = 3;
        public static readonly int FooterCnt = 1;

        public enum CmdMode
        {
            [EnumMember(Value = "TextEditing")] TextEditing = 0,      //AppendChar()
            [EnumMember(Value = "DetailsEditing")] DetailsEditing = 1,      //AppendLine()
            [EnumMember(Value = "SpellingCheck")] SpellCheck = 2,      //AppendWord()
            [EnumMember(Value = "Unknown")] Unknown = NotificationItem.ChangeUnknown
        }

        public CmdMode Mode { private set; get; }
        public ITerminal Terminal { set; get; }

        public int Width { private set; get; }
        public int Height { private set; get; }
        public int LineWidth { private set; get; }

        public  ChapterModel Chapter { private set; get; }

        public bool Ready { private set; get; }

        public KLineEditor()
        {
            Terminal = null;
            Chapter = null;
            LineWidth = Program.PosIntegerNotSet;
            Mode = CmdMode.Unknown;
            Ready = false;
        }
        public KLineEditor(ITerminal terminal) : this()
        {
            if (terminal.IsError() == false)
            {
                Terminal = terminal;
            }
        }

        public bool SetMode(CmdMode mode)
        {
            var rc = false;

            if (Chapter != null)
            {
                if (mode == CmdMode.TextEditing)
                {
                    Mode = mode;
                    Chapter.SetCmdLine(GetCmdHelpLine());

                    Terminal.SetCursorPosition(3, 1);

                    rc = true;
                }
            }
            return rc;
        }


        private string GetCmdHelpLine()
        {
            var rc = "";
            if (Mode == CmdMode.TextEditing)
                rc = $"Text Editing: Esc=Refresh F1=Help Ctrl+S=Save Ctrl+Q=Quit";
            else
                rc = $"Unsupported: Esc=Refresh F1=Help Ctrl+Q=Quit";

            return rc;
        }

        public MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("KLineEditor.Setup");

            if (param == null)
                rc.SetError(1030101, MxError.Source.Param, $"param is null", "MxErrBadMethodParam");
            else
            {
                try
                {
                    LineWidth = param.MaxCol;
                    Width = ScreenMarginLeft + LineWidth + ScreenMarginRight;
                    Height = CmdLineCnt + ScreenMarginTop + param.DisplayLastLinesCnt + ScreenMarginBottom + FooterCnt;

                    var settings = new TerminalProperties
                    {
                        Title = $"{Program.CmdAppName} v{Program.CmdAppVersion} - {param.EditFile ?? "[null]"}",
                        BufferHeight = Height,
                        BufferWidth = Width,
                        WindowHeight = Height,
                        WindowWidth = Width
                    };
                    if ((settings.Validate() == false) || Terminal.Setup(settings) == false)
                        rc.SetError(1030102, MxError.Source.User, $"Terminal.Setup() failed; {settings.GetValidationError()}", "MxErrInvalidSettingsFile");
                    else
                    {
                        Terminal.Clear();
                        Ready = true;
                        rc.SetResult(true);
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1030102, MxError.Source.Exception, e.Message);
                }
            }
            return rc;
        }
        public MxReturnCode<bool> Start(ChapterModel model)
        {
            var rc = new MxReturnCode<bool>("Edit.Setup");

            if ((model == null) || (model.Ready == false)  )
                rc.SetError(1030201, MxError.Source.Param, $"model is {((model == null) ? "[null]" : "[not ready]")}", "MxErrBadMethodParam");
            else
            {
                Chapter = model;

                var rcSession = model.CreateNewSession();
                rc += rcSession;
                if (rcSession.IsSuccess(true))
                {
                    Ready = true;
                    rc.SetResult(true);
                }
            }
            return rc;
        }
        public MxReturnCode<bool> Process()
        {
            var rc = new MxReturnCode<bool>("Edit.Process");

            try
            {
                if (Ready)
                {
                    SetMode(CmdMode.TextEditing);
                    //setup VDU commands, footer
                    //display previous lines
                    //while()
                    //  process input from user and add to line buffer or process cmds
                    //  at end of line append to file
                    //  setup VDU commands, footer
                    //  display previous lines
                    Terminal.WriteLines($"{Environment.NewLine}Press 'Esc' to continue...");

                    while (true)
                    {
                        var op = Terminal.GetKey(true);
                        if (op == ConsoleKey.Escape)
                            break;

                        //ConsoleKeyInfo op = Terminal.ReadKey();
                        //if ((op.Key == ConsoleKey.A) && (op.Modifiers == ConsoleModifiers.Control))
                        //    break;
                    } 
                    rc.SetResult(true);
                }
            }
            catch (Exception e)
            {
                rc.SetError(1030401, MxError.Source.Exception, e.Message);
            }
            return rc;
        }

        public MxReturnCode<bool> Finish()
        {
            var rc = new MxReturnCode<bool>("Edit.Finish");

            try
            {
                if (Ready)
                {
                    Chapter.Close();
                    Ready = false;
                    rc.SetResult(true);
                }
            }
            catch (Exception e)
            {
                rc.SetError(1030301, MxError.Source.Exception, e.Message);
            }
            return rc;
        }
        public string GetReport()
        {
            var rc = string.Format(Resources.WelcomeNotice, Program.CmdAppName, Program.CmdAppVersion, Program.CmdAppCopyright, Environment.NewLine);
            rc += Environment.NewLine;
            rc += $"Report for editing session {Chapter?.Header?.GetLastSession()?.SessionNo ?? Program.PosIntegerNotSet} of chapter {Chapter?.Header?.Chapter?.Title ?? "[null]"}:";
            rc += Environment.NewLine;
            rc += Environment.NewLine;
            rc += Chapter?.GetReport() ??HeaderBase.ValueNotSet;
            return rc;
        }
        private string GetCommandHints()
        {
            var rc = "[not initialized]";
            if (Ready)
            {
                rc = "Esc=refresh | Ctrl+X=exit | <- | -> | Del | BS";
            }
            return rc;
        }
        private string GetFooter()
        {
            var rc = "[not initialized]";
            if (Ready)
            {
                //start time, current edit duration, WPM, number of words written, number of pages written, total number of words in file, total number of pages in file
                rc = $" | {Chapter?.Folder ?? "[null]"}";
            }
            return rc;
        }
    }
}
