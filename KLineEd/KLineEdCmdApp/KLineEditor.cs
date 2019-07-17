using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Model.Base;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.Utils.Properties;
using KLineEdCmdApp.View;
using MxReturnCode;

// ReSharper disable once CheckNamespace
namespace KLineEdCmdApp
{
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public class KLineEditor
    {
        public static readonly int MaxWindowHeight = Console.LargestWindowHeight;  
        public static readonly int MaxWindowWidth = Console.LargestWindowWidth;
        public static readonly int MinWindowHeight = 4; //ModeHelp, Msg, Text, Status
        public static readonly int MinWindowWidth = Program.ValueOverflow.Length;

    //display vertical layout: CmdHelp, Msg, - note: any change to ModeHelpLineCount, MsgLineCount, Status LineCount needs changes to their view.setup()
        public const int EditorHelpLineRowIndex = 0;
        public const int ModeHelpLineRowCount = 1;   //height = ModeHelpLineRow + ModeHelpLineCount + MsgLineCount + EditAreaMarginTop + param.DisplayLastLinesCnt +  EditAreaMarginBottom + StatusLineCount
        public const int MsgLineRowIndex = EditorHelpLineRowIndex + ModeHelpLineRowCount;
        public const int MsgLineRowCount = 1;
        public const int EditAreaMarginTopRowIndex = MsgLineRowIndex + MsgLineRowCount;
        public const int EditAreaMarginTopRowCount = 2;
        public const int EditAreaTopRowIndex = EditAreaMarginTopRowIndex + EditAreaMarginTopRowCount;
        //param.DisplayLastLinesCnt
        public const int EditAreaMarginBottomRowCount = 10;
        public const int StatusLineRowCount = 1;

    //display horizontal layout:
        //TextEditingMode 
        public const int EditAreaMarginLeft = 5;
        //param.DisplayLineWidth
        public const int EditAreaMarginRight = 20;  //width = EditAreaMarginLeft + param.DisplayLineWidth + EditAreaMarginRight
        //ModeHelpLine, MsgLine, StatusLine
        public const int EditorHelpLineLeftCol = 1;   //width -= ModeHelpLineLeftCol;
        public const int MsgLineLeftCol = 3;        //width -= MsgLineLeftCol;      
        public const int StatusLineLeftCol = 1;     //width -= StatusLineLeftCol;   


        public static readonly string ReportSectionDottedLine = $"{Environment.NewLine}....................................................{Environment.NewLine}";

        private EditingBaseController Controller { set; get; }
        public ITerminal Terminal { set; get; }
        public ChapterModel Chapter { private set; get; }

        public int Width { private set; get; }
        public int Height { private set; get; }
        public int LineWidth { private set; get; }

        public bool Ready { private set; get; }

        public KLineEditor()
        {
            Controller = null;
            Terminal = null;
            Chapter = null;
            Width = Program.PosIntegerNotSet;
            Height = Program.PosIntegerNotSet;
            LineWidth = Program.PosIntegerNotSet;

            Ready = false;
        }

        public MxReturnCode<string> Run(CmdLineParamsApp cmdLineParams, ChapterModel editModel, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("Program.RunEditor");

            if ((cmdLineParams == null) || (editModel == null) || (editModel.Ready == false) || (terminal == null) || (terminal.IsError()))
                rc.SetError(1010301, MxError.Source.Param, $"cmdLineParams is null, editModel is null or not read, or terminal is null or error", "MxErrInvalidParamArg");
            else
            {
                var rcInit = Initialise(cmdLineParams, editModel, terminal);
                rc += rcInit;
                if (rcInit.IsSuccess(true))
                {
                    var editHelpView = new EditorHelpLineView(terminal);
                    var msgLineView = new MsgLineView(terminal);
                    var statusLineView = new StatusLineView(terminal);
                    var textEditView = new TextEditView(terminal);
                    var propsEditView = new PropsEditView(terminal);
                    var spellEditView = new SpellEditView(terminal);

                    var rcSetup = SetupViews(cmdLineParams, editHelpView, msgLineView, statusLineView, textEditView, propsEditView, spellEditView);
                    rc += rcSetup;
                    if (rcSetup.IsSuccess(true))
                    {                                               //subscription order determines order in which views are notified
                        using (editModel.Subscribe(editHelpView))
                        using (editModel.Subscribe(msgLineView))
                        using (editModel.Subscribe(statusLineView))
                        using (editModel.Subscribe(textEditView))   //make sure the EditAreaViews are notified after editHelp, msg, status so the cursor to set back to EditArea
                        using (editModel.Subscribe(propsEditView))
                        using (editModel.Subscribe(spellEditView))
                        {
                            var rcStart = Start(editModel); //start
                            rc += rcStart;
                            if (rcStart.IsSuccess(true))
                            {
                                var rcProcess = Process(); //process
                                rc += rcProcess;
                                if (rcProcess.IsSuccess(true))
                                {
                                    var report = GetReport();

                                    var rcFinish = Finish(); //finish
                                    rc += rcFinish;
                                    if (rcFinish.IsSuccess(true))
                                    {
                                        rc.SetResult(report);
                                    }
                                }
                            }
                        }
                        if (editModel.GetSubscriberCount() > 0)
                            rc.SetError(1010302, MxError.Source.Program, $"Views still subscribing to model; count={editModel.GetSubscriberCount()}", "MxErrInvalidCondition");
                    }
                }
            }
            return rc;
        }

        private static MxReturnCode<bool> SetupViews(CmdLineParamsApp cmdLineParams, EditorHelpLineView editorHelpView, MsgLineView msgLineView, StatusLineView statusLineView, TextEditView textEditView, PropsEditView propsEditView, SpellEditView spellEditView)
        {
            var rc = new MxReturnCode<bool>("Program.SetupKlineEdVViews");

            if ((cmdLineParams == null) || (editorHelpView == null) || (msgLineView == null) || (statusLineView == null) || (textEditView == null) || (propsEditView == null) || (spellEditView == null))
                rc.SetError(101401, MxError.Source.Param, $"cmdLineParams is null, or one of the view objects is null", "MxErrInvalidParamArg");
            else
            {
                var rcCmds = editorHelpView.Setup(cmdLineParams);
                rc += rcCmds;
                if (rcCmds.IsSuccess(true))
                {
                    var rcMsg = msgLineView.Setup(cmdLineParams);
                    rc += rcMsg;
                    if (rcMsg.IsSuccess(true))
                    {
                        var rcStatus = statusLineView.Setup(cmdLineParams);
                        rc += rcStatus;
                        if (rcStatus.IsSuccess(true))
                        {
                            var rcTxt = textEditView.Setup(cmdLineParams);
                            rc += rcTxt;
                            if (rcTxt.IsSuccess(true))
                            {
                                var rcProps = propsEditView.Setup(cmdLineParams);
                                rc += rcProps;
                                if (rcProps.IsSuccess(true))
                                {
                                    var rcSpell = spellEditView.Setup(cmdLineParams);
                                    rc += rcSpell;
                                    if (rcSpell.IsSuccess(true))
                                    {
                                        rc.SetResult(true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return rc;
        }

        private MxReturnCode<bool> Initialise(CmdLineParamsApp param, ChapterModel model, ITerminal terminal)
        {
            var rc = new MxReturnCode<bool>("KLineEditor. Initialise");

            if ((param == null) || ((model?.Ready ?? false ) == false) || (terminal?.IsError() ?? true)) 
                rc.SetError(1030101, MxError.Source.Param, $"param is null or editModel null, not ready", "MxErrBadMethodParam");
            else
            {
                try
                {
                    terminal.Clear();

                    LineWidth = param.DisplayLineWidth;
                    Width = EditAreaMarginLeft + LineWidth + EditAreaMarginRight;  //there is actually an addition column, but writing to it creates a new line
                    Height = ModeHelpLineRowCount + EditAreaMarginTopRowCount + param.DisplayLastLinesCnt + EditAreaMarginBottomRowCount + StatusLineRowCount;

                    var settings = new TerminalProperties
                    {
                        Title = $"{Program.CmdAppName} v{Program.CmdAppVersion} - {param.EditFile ?? "[null]"}: Chapter {model.Header?.Chapter?.Title ?? Program.ValueNotSet}",
                        BufferHeight = Height,
                        BufferWidth = Width,
                        WindowHeight = Height,
                        WindowWidth = Width
                    };
                    if ((settings.Validate() == false) || terminal.Setup(settings) == false)
                        rc.SetError(1030102, MxError.Source.User, $"Terminal.Setup() failed; {settings.GetValidationError()} or Terminal.ErrorMSg={Terminal?.ErrorMsg ?? Program.ValueNotSet}", "MxErrInvalidSettingsFile");
                    else
                    {
                        Terminal = terminal;
                        Chapter = model;
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

        private MxReturnCode<bool> Start(ChapterModel model)
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

        private MxReturnCode<bool> Process()
        {
            var rc = new MxReturnCode<bool>("Edit.Process");

            try
            {
                if (Ready)  //report error
                {
                    Chapter.SetStatusLine(); 
                    Controller = ControllerFactory.Make(Chapter, ControllerFactory.TextEditingController); //create PropsEditingController if mode is new

                    var lastKeyStroke = DateTime.UtcNow;
                    var tim = DateTime.UtcNow;

                    while (true)
                    {
                        //within this loop report all non-critical errors using BaseView.DisplayErrorMsg(no, type, msg)
                        //or BaseView.DisplayMxErrorMsg(msg) - in both cases error message is formatted as "error 1010102-exception: msg"

                        if ((DateTime.UtcNow - tim).TotalMilliseconds > 950)
                        {
                            Chapter.SetStatusLine();
                            tim = DateTime.UtcNow;
                        }

                        if ((Chapter.Header.GetLastSession().PauseState == false) && (DateTime.UtcNow - lastKeyStroke).TotalSeconds >= 60)
                            Chapter.Header.GetLastSession().SetPause();
  
                        if (Terminal.IsKeyAvailable())
                        {
                            if (Chapter.Header.GetLastSession()?.PauseState ?? false)
                                Chapter.Header.GetLastSession()?.AddPause(DateTime.UtcNow, lastKeyStroke);

                            lastKeyStroke = DateTime.UtcNow;
                            var op = Terminal.GetKey(true);

                            Controller = EditingBaseController.ProcessKey(Controller, Chapter, op);
                            if (Controller?.IsCritialError() ?? true)
                            {
                                rc.SetError(1030301, MxError.Source.Program, $"Controller null or critical error {Controller?.DisplayMsg ?? Program.ValueNotSet}", "MxErrInvalidCondition");
                                break;
                            }
                            if (Controller.IsDisplayMsg())
                                Chapter.SetMsgLine(Controller.DisplayMsg);

                            if (op == ConsoleKey.Escape)
                                break;
                        }

                        Thread.Sleep(0);


                        //ConsoleKeyInfo op = Terminal.ReadKey();
                        //if ((op.Key == ConsoleKey.A) && (op.Modifiers == ConsoleModifiers.Control))
                        //    break;
                    }
                    if (rc.IsSuccess())
                        rc.SetResult(true);
                }
            }
            catch (Exception e)
            {
                rc.SetError(1030401, MxError.Source.Exception, e.Message);
            }
            return rc;
        }

        private MxReturnCode<bool> Finish()
        {
            var rc = new MxReturnCode<bool>("Edit.Finish");

            try
            {
                if (Ready)
                {
                    // ReSharper disable once RedundantArgumentDefaultValue
                    Chapter.Close(true);
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
        private string GetReport()
        {
            var rc = String.Format(Resources.WelcomeNotice, Program.CmdAppName, Program.CmdAppVersion, Program.CmdAppCopyright, Environment.NewLine);
            rc += Environment.NewLine;
            rc += $"Report for editing session {Chapter?.Header?.GetLastSession()?.SessionNo ?? Program.PosIntegerNotSet} of chapter {Chapter?.Header?.Chapter?.Title ?? "[null]"}:";
            rc += Environment.NewLine;
            rc += Environment.NewLine;
            rc += Chapter?.GetReport() ??HeaderBase.ValueNotSet;
            return rc;
        }
    }
}
