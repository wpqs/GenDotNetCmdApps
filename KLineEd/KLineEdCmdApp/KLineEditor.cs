using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Controller;
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
        public static readonly int ModeHelpLineRowIndex = 0;
        public static readonly int ModeHelpLineRowCount = 1;   //height = ModeHelpLineRow + ModeHelpLineCount + MsgLineCount + EditAreaMarginTop + param.DisplayLastLinesCnt +  EditAreaMarginBottom + StatusLineCount
        public static readonly int MsgLineRowIndex = ModeHelpLineRowIndex + ModeHelpLineRowCount;
        public static readonly int MsgLineRowCount = 1;
        public static readonly int EditAreaMarginTopRowIndex = MsgLineRowIndex + MsgLineRowCount;
        public static readonly int EditAreaMarginTopRowCount = 2;
        public static readonly int EditAreaTopRowIndex = EditAreaMarginTopRowIndex + EditAreaMarginTopRowCount;
        //param.DisplayLastLinesCnt
        public static readonly int EditAreaMarginBottomRowCount = 10;
        public static readonly int StatusLineRowCount = 1;

    //display horizontal layout:
        //TextEditingMode 
        public static readonly int EditAreaMarginLeft = 5;
        //param.DisplayLineWidth
        public static readonly int EditAreaMarginRight = 20;  //width = EditAreaMarginLeft + param.DisplayLineWidth + EditAreaMarginRight
        //ModeHelpLine, MsgLine, StatusLine
        public static readonly int ModeHelpLineLeftCol = 1;   //width -= ModeHelpLineLeftCol;
        public static readonly int MsgLineLeftCol = 3;        //width -= MsgLineLeftCol;      
        public static readonly int StatusLineLeftCol = 1;     //width -= StatusLineLeftCol;   


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

        //public void SetMode(EditModeController.EditingMode editingMode)
        //{
        //    if (Chapter != null)
        //    {
        //        switch (editingMode)
        //        {
        //            case EditModeController.EditingMode.Text:
        //            {
        //                Mode = TextEditProc;
        //                break;
        //            }
        //            case EditModeController.EditingMode.Properties:
        //            {
        //                break;
        //            }
        //            case EditModeController.EditingMode.Spell:
        //            {
        //                break;
        //            }
        //            case EditModeController.EditingMode.Error:
        //            default:
        //            {
        //                break;
        //            }
        //        }
        //        Chapter.SetModeHelpLine(Mode?.GetModeHelpLine() ?? Program.ValueNotSet, false);
        //        Chapter.SetMode(Mode);
        //    }
        //}


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
                    var cmdsHelpView = new EditorHelpView(terminal);
                    var msgLineView = new MsgLineView(terminal);
                    var statusLineView = new StatusLineView(terminal);
                    var textEditView = new TextEditView(terminal);

                    var rcSetup = SetupViews(cmdLineParams, cmdsHelpView, msgLineView, statusLineView, textEditView);
                    rc += rcSetup;
                    if (rcSetup.IsSuccess(true))
                    {
                        using (editModel.Subscribe(cmdsHelpView))
                        using (editModel.Subscribe(msgLineView))
                        using (editModel.Subscribe(statusLineView))
                        using (editModel.Subscribe(textEditView))           //make the last view to be notified so it sets the cursor back to EditArea
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

        private static MxReturnCode<bool> SetupViews(CmdLineParamsApp cmdLineParams, EditorHelpView editorHelpView, MsgLineView msgLineView, StatusLineView statusLineView, TextEditView textEditView)
        {
            var rc = new MxReturnCode<bool>("Program.SetupKlineEdVViews");

            if ((cmdLineParams == null) || (editorHelpView == null) || (msgLineView == null) || (statusLineView == null) || (textEditView == null))
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
                                rc.SetResult(true);
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
                    Width = EditAreaMarginLeft + LineWidth + EditAreaMarginRight;
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
                        Controller = new TextEditingController();   //if model is new, then PropsEditingController() 
                        var rcInit = Controller.Initialise(terminal, model);
                        rc += rcInit;
                        if (rcInit.IsSuccess(true))
                        {
                            Terminal = terminal;
                            Chapter = model;
                            Ready = true;
                            rc.SetResult(true);
                        }
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
                    while (true)
                    {
                        Chapter.SetEditorHelpLine(Controller.GetModeHelpLine());
                        Chapter.SetMsgLine("hello Will...");
                        Chapter.SetStatusLine();

                        var op = Terminal.GetKey(true);
                        if (op == ConsoleKey.Escape)
                            break;
                        Controller.ProcessKey(op, Chapter);

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

        private MxReturnCode<bool> Finish()
        {
            var rc = new MxReturnCode<bool>("Edit.Finish");

            try
            {
                if (Ready)
                {
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
