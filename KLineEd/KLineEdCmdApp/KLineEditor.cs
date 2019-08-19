using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Model.Base;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View;
using KLineEdCmdApp.View.Base;
using MxReturnCode;

// ReSharper disable once CheckNamespace
namespace KLineEdCmdApp
{
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public class KLineEditor
    {
        public static readonly int StatusLineUpdateMilliSecs = 200;

        public static readonly int MaxWindowHeight = Console.LargestWindowHeight;  
        public static readonly int MaxWindowWidth = Console.LargestWindowWidth;
        public static readonly int MinWindowHeight = 4; //ModeHelp, Msg, Text, Status
        public static readonly int MinWindowWidth = Program.ValueOverflow.Length;

    //display vertical layout: CmdHelp, Msg, - note: any change to ModeHelpLineCount, MsgLineCount, Status LineCount needs changes to their view.setup()
        public const int EditorHelpLineRowIndex = 0;
        public const int HelpLineRowCount = 1;   //height = ModeHelpLineRow + ModeHelpLineCount + MsgLineCount + EditAreaMarginTop + param.DisplayLastLinesCnt +  EditAreaMarginBottom + StatusLineCount
        public const int MsgLineRowIndex = EditorHelpLineRowIndex + HelpLineRowCount;
        public const int MsgLineRowCount = 1;
        public const int EditAreaMarginTopRowIndex = MsgLineRowIndex + MsgLineRowCount;
        public const int EditAreaMarginTopRowCount = 2;
        public const int EditAreaMarginTopRuleRowCount = 1;
        public const int EditAreaMarginTopRuleIndex = EditAreaMarginTopRowIndex + EditAreaMarginTopRowCount;

        public const int EditAreaTopRowIndex = EditAreaMarginTopRowIndex + EditAreaMarginTopRowCount + EditAreaMarginTopRuleRowCount;
        //param.DisplayLastLinesCnt
        public const int EditAreaMarginBottomRuleRowCount = 1;
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

        public static readonly string ReportSectionDottedLine = $"{Environment.NewLine}....................................................";

        private EditingBaseController Controller { set; get; }
        public ITerminal Terminal { set; get; }
        public ChapterModel Model { private set; get; }

        public int Width { private set; get; }
        public int Height { private set; get; }
        public int EditAreaLineWidth { private set; get; }

        public string Report { private set; get; }
        public string BrowserExe { private set; get; }
        public bool Ready { private set; get; }

        private List<BaseView> ViewList { set; get; }

        public KLineEditor()
        {
            Controller = null;
            Terminal = null;
            Model = null;
            Width = Program.PosIntegerNotSet;
            Height = Program.PosIntegerNotSet;
            EditAreaLineWidth = Program.PosIntegerNotSet;
            Report = Program.ValueNotSet;
            BrowserExe = CmdLineParamsApp.ArgBrowserExeDefault;
            Ready = false;


            ViewList = new List<BaseView>();

        }

        public MxReturnCode<string> Run(CmdLineParamsApp cmdLineParams, ChapterModel editModel, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("KLineEditor.Run");

            if ((cmdLineParams == null) || (editModel == null) || (editModel.Ready == false) || (terminal == null) || (terminal.IsError()))
                rc.SetError(1030101, MxError.Source.Param, $"cmdLineParams is null, editModel is null or not read, or terminal is null or error", MxMsgs.MxErrInvalidParamArg);
            else
            {
                var rcInit = Initialise(cmdLineParams, editModel, terminal);
                rc += rcInit;
                if (rcInit.IsSuccess(true))
                {
                    var rcSetup = CreateViews(cmdLineParams, terminal, editModel);
                    rc += rcSetup;
                    if (rcSetup.IsSuccess(true))
                    {                                              
                        var rcStart = Start(editModel);     //start
                        rc += rcStart;
                        if (rcStart.IsSuccess(true))
                        {
                            var rcProcess = Process();      //process
                            rc += rcProcess;
                            Report = GetReport();

                            var rcFinish = Finish();        //finish irrespective of any error in Process - i.e. save file at all costs
                            if (rcProcess.IsSuccess())
                                rc += rcFinish;                 //keep any prior error set by rc += rcProcess
                            if (rc.IsSuccess() && rcFinish.IsSuccess(true))
                            {
                                rc.SetResult("succeeded");          //no error completion of Run()
                            }
                        }
                        var rcClose = TerminateViews(editModel, terminal);
                        if (rc.IsSuccess(true))         //keep any prior error, otherwise get any errors held in View.MxErrorCode
                            rc += rcClose;  
                    }
                }
            }
            return rc;
        }

        public string GetReport()
        {
            var rc = Environment.NewLine;   //reports always start with newline, but don't end with one
            rc += $"Report for editing session {Model?.ChapterHeader?.GetLastSession()?.SessionNo ?? Program.PosIntegerNotSet} of chapter {Model?.ChapterHeader?.Properties?.Title ?? "[null]"}:";
            rc += Environment.NewLine;
            rc += Model?.GetReport() ?? HeaderElementBase.ValueNotSet;
            return rc;
        }

        private MxReturnCode<bool> Initialise(CmdLineParamsApp param, ChapterModel model, ITerminal terminal)
        {
            var rc = new MxReturnCode<bool>("KLineEditor. Initialise");

            if ((param == null) || ((model?.Ready ?? false ) == false) || (terminal?.IsError() ?? true)) 
                rc.SetError(1030201, MxError.Source.Param, $"param is null or editModel null, not ready", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    //BrowserExe = param.BrowserExe;
                    EditAreaLineWidth = param.EditAreaLineWidth;
                    Width = EditAreaMarginLeft + EditAreaLineWidth + EditAreaMarginRight;  //there is actually an additional column used by cursor when at end of line
                    Height = HelpLineRowCount +  EditAreaMarginTopRowCount + EditAreaMarginTopRuleRowCount + param.EditAreaLinesCount + EditAreaMarginBottomRuleRowCount + EditAreaMarginBottomRowCount + StatusLineRowCount;

                    var settings = new TerminalProperties
                    {
                        Title = $"{Program.CmdAppName} v{Program.CmdAppVersion} - {param.EditFile ?? "[null]"}: Chapter {model.ChapterHeader?.Properties?.Title ?? Program.ValueNotSet}",
                        BufferHeight = Height,
                        BufferWidth = Width,
                        WindowHeight = Height,
                        WindowWidth = Width
                    };
                    if (settings.Validate() == false) 
                        rc.SetError(1030202, MxError.Source.User, $"settings.Validate() failed; {settings.GetValidationError()}", MxMsgs.MxErrInvalidSettingsFile);
                    else
                    {
                        if (terminal.Setup(settings) == false)
                            rc.SetError(1030203, terminal.GetErrorSource(), terminal.GetErrorTechMsg(), terminal.GetErrorUserMsg());
                        else
                        {
                            Terminal = terminal;
                            Model = model;
                            Ready = true;
                            rc.SetResult(true);
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1030204, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        private MxReturnCode<bool> Start(ChapterModel model)
        {
            var rc = new MxReturnCode<bool>("Edit.Setup");

            if ((model == null) || (model.Ready == false)  )
                rc.SetError(1030301, MxError.Source.Param, $"model is {((model == null) ? "[null]" : "[not ready]")}", MxMsgs.MxErrBadMethodParam);
            else
            {
                Model = model;

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

            if (Ready == false)
                rc.SetError(1030401, MxError.Source.Param, $"Ready false", MxMsgs.MxErrBadMethodParam);
            else
            {
                var terminalSettings = Terminal.GetSettings();
                if (terminalSettings.Validate() == false)
                    rc.SetError(1030402, MxError.Source.User, $"settings.Validate() failed; {terminalSettings.GetValidationError()}", MxMsgs.MxErrInvalidSettingsFile);
                else
                {
                    if (((Controller = ControllerFactory.Make(Model, ControllerFactory.PropsEditingController, BrowserExe)) == null) || Controller.IsError())
                        rc.SetError(1030403, MxError.Source.Program, $"ControllerFactory.Make failed", MxMsgs.MxErrInvalidCondition);
                    else
                    {
                        try
                        {
                            Model.SetStatusLine();
                            var nowUtc = DateTime.UtcNow;
                            var lastKeyPress = nowUtc;

                            while (rc.IsError() == false)
                            {
                                if ((DateTime.UtcNow - nowUtc).TotalMilliseconds > StatusLineUpdateMilliSecs)
                                {
                                    nowUtc = DateTime.UtcNow;
                                    Model.SetStatusLine();
                                    if (Model.ChapterHeader.PauseProcessing(nowUtc, lastKeyPress, false) == false)
                                    {
                                        rc.SetError(1030404, MxError.Source.Program, $"PauseProcessing failed", MxMsgs.MxErrInvalidCondition);
                                        break;
                                    }
                                    if (Controller.IsError() == false)
                                        Model.SetMsgLine("");
                                    Terminal.SetCursorInsertMode((Controller.IsInsertMode()));
                                }
                                if (Terminal.IsKeyAvailable())
                                {
                                    lastKeyPress = DateTime.UtcNow;
                                    if (Model.ChapterHeader.PauseProcessing(nowUtc, lastKeyPress, true) == false)
                                    {
                                        rc.SetError(1030405, MxError.Source.Program, $"PauseProcessing failed", MxMsgs.MxErrInvalidCondition);
                                        break; 
                                    }
                                    Controller = Controller.ProcessKey(Model, Terminal.ReadKey(true));
                                    if (Controller.IsError())
                                    {
                                        Model.SetMsgLine(Controller.GetErrorTechDetails());
                                        //Model.SetMxErrorMsg(Controller.GetErrorTechDetails()); //todo wait for next release of MxReturnCode to get resource string
                                    }
                                }
                                if (Controller?.IsRefresh() ?? true)
                                {
                                    Terminal.Setup(terminalSettings);
                                    Model.Refresh();
                                }
                                if (Controller?.IsQuit() ?? true)
                                    break;

                                var rcErr = GetAnyCriticalError(ViewList, Controller); 
                                if (rcErr.IsError())
                                    rc += rcErr;
                                else
                                    Thread.Sleep(0);
                            }
                            var rcDoneProc = Model.ChapterHeader.KLineEditorProcessDone(nowUtc, lastKeyPress);
                            if (rc.IsSuccess())
                            {
                                rc += rcDoneProc;       //keep any existing error
                                rc.SetResult(true);
                            }
                        }
                        catch (Exception e)
                        {
                            rc.SetError(1030406, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                        }
                    }
                }
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
                    Model.Close(true);      //Model.Close() calls DisconnectAllViews() irrespective of save or not
                    Ready = false;
                    rc.SetResult(true);
                }
            }
            catch (Exception e)
            {
                rc.SetError(1030501, MxError.Source.User, e.Message, MxMsgs.MxErrException);
            }
            return rc;
        }

        private MxReturnCode<bool> CreateViews(CmdLineParamsApp cmdLineParams, ITerminal terminal, ChapterModel model)
        {
            var rc = new MxReturnCode<bool>("KLineEditor.CreateViews");

            if ((cmdLineParams == null) || (ViewList == null) || (terminal == null) || (terminal.IsError()) || (model == null) || (model.Ready == false))
                rc.SetError(1030601, MxError.Source.Param, $"cmdLineParams= is null, or ViewList is null, or terminal is null or error, or model is null or not ready", MxMsgs.MxErrInvalidParamArg);
            else
            {
                try
                {                    //subscription order determines order in which views are notified
                    var editorHelpView = new EditorHelpLineView(terminal);
                    var rcCmds = editorHelpView.Setup(cmdLineParams);
                    rc += rcCmds;
                    if (rcCmds.IsSuccess(true))
                    {
                        model.Subscribe(editorHelpView);                        //editorHelpLine
                        ViewList.Add(editorHelpView);

                        var msgLineView = new MsgLineView(terminal);
                        var rcMsg = msgLineView.Setup(cmdLineParams);
                        rc += rcMsg;
                        if (rcMsg.IsSuccess(true))
                        {
                            model.Subscribe(msgLineView);                       //MsgLine
                            ViewList.Add(msgLineView);

                            var statusLineView = new StatusLineView(terminal);
                            var rcStatus = statusLineView.Setup(cmdLineParams);
                            rc += rcStatus;
                            if (rcStatus.IsSuccess(true))
                            {
                                model.Subscribe(statusLineView);                //StatusLine
                                ViewList.Add(statusLineView);

                                //make sure the EditAreaViews are notified after editHelp, msg, status so the cursor to set back to EditArea

                                var textEditView = new TextEditView(terminal);
                                var rcTxt = textEditView.Setup(cmdLineParams);
                                rc += rcTxt;
                                if (rcTxt.IsSuccess(true))
                                {
                                    model.Subscribe(textEditView);              //textEditView
                                    ViewList.Add(textEditView);

                                    var propsEditView = new PropsEditView(terminal);
                                    var rcProps = propsEditView.Setup(cmdLineParams);
                                    rc += rcProps;
                                    if (rcProps.IsSuccess(true))
                                    {
                                        model.Subscribe(propsEditView);         //PropsEditView
                                        ViewList.Add(propsEditView);

                                        var spellEditView = new SpellEditView(terminal);
                                        var rcSpell = spellEditView.Setup(cmdLineParams);
                                        rc += rcSpell;
                                        if (rcSpell.IsSuccess(true))
                                        {
                                            model.Subscribe(spellEditView);     //SpellEditView
                                            ViewList.Add(spellEditView);

                                            rc.SetResult(true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1030602, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
                if (rc.IsError(true))
                    TerminateViews(model, terminal);
            }
            return rc;
        }

        private MxReturnCode<bool> TerminateViews(ChapterModel model, ITerminal terminal)
        {
            var rc = new MxReturnCode<bool>("KLineEditor.TerminateViews");

            if ((model == null) || (ViewList == null) || (terminal == null))
                rc.SetError(1030701, MxError.Source.Param, $"ViewList is null, or terminal is null, or model is null", MxMsgs.MxErrInvalidParamArg);
            else
            {
                //model.DisconnectAllViews();      //not needed as Finish() calls Model.Close() which in turn calls DisconnectAllViews() irrespective of save or not

                if (model.GetSubscriberCount() > 0)
                    rc.SetError(1030702, MxError.Source.Program, $"Views still subscribing to model; count={model.GetSubscriberCount()}", MxMsgs.MxErrInvalidCondition);

                foreach (var view in ViewList)
                    rc += view.Close();         //add any error held in View.MxErrorCode

                rc += terminal.Close();         //get any lingering _mxErrorCode

                if (rc.IsSuccess())
                    rc.SetResult(true);
            }
            return rc;
        }

        private MxReturnCode<bool> GetAnyCriticalError(List<BaseView> viewList, EditingBaseController controller)
        {
            var rc = new MxReturnCode<bool>("KLineEditor.GetAnyCriticalError");

            if ((viewList == null) || (controller == null))
                rc.SetError(1030801, MxError.Source.Param, $"ViewList or controller is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (controller.IsError(true))
                    rc.SetError(controller.GetErrorNo(), controller.GetErrorSource(), controller.GetErrorTechDetails()); //; controller.GetErrorUserMsg()); wait for next MxReturnCode release
                else
                {
                    foreach (var view in viewList)
                    {
                        var rcViewErr = view.GetMxError();
                        if (rcViewErr.IsError() && (rcViewErr.GetErrorType() != MxError.Source.User)) //or MxError.Source.Data
                            rc += rcViewErr;
                    }
                    if (rc.IsSuccess())
                        rc.SetResult(true);
                }
            }
            return rc;
        }
    }
}
