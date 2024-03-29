﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
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
        public static readonly int MaxSplitLineLength = 25000;  //500 words per page * 5 chars per word * 10 pages - see https://stackoverflow.com/questions/140468/what-is-the-maximum-possible-length-of-a-net-string

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

        public const int WindowFrameRows = HelpLineRowCount + EditAreaMarginTopRowCount + EditAreaMarginTopRuleRowCount + EditAreaMarginBottomRuleRowCount + EditAreaMarginBottomRowCount + StatusLineRowCount; 

        //display horizontal layout:
        //TextEditingMode 
        public const int EditAreaMarginLeft = 15;
        //param.DisplayLineWidth
        public const int EditAreaMarginRight = 20;  //width = EditAreaMarginLeft + param.DisplayLineWidth + EditAreaMarginRight
        //ModeHelpLine, MsgLine, StatusLine
        public const int EditorHelpLineLeftCol = 1;   //width -= ModeHelpLineLeftCol;
        public const int MsgLineLeftCol = 3;        //width -= MsgLineLeftCol;      
        public const int StatusLineLeftCol = 1;     //width -= StatusLineLeftCol;   

        public const int WindowFrameCols = EditAreaMarginLeft + EditAreaMarginRight;


        public static readonly string ReportSectionDottedLine = $"{Environment.NewLine}....................................................";

        private BaseEditingController Controller { set; get; }
        public IMxConsole Console { private set; get; }
        public ChapterModel Model { private set; get; }

        public int Width { private set; get; }
        public int Height { private set; get; }
        public int EditAreaLineWidth { private set; get; }
        public int AutoSavePeriod { private set; get; }
        public int StatusUpdatePeriod { private set; get; }

        public string Report { private set; get; }
        public string BrowserCmd { private set; get; }
        public string HelpUrl { private set; get; }
        public string SearchUrl { private set; get; }
        public string ThesaurusUrl { private set; get; }
        public string SpellUrl { private set; get; }
        public bool Ready { private set; get; }

        private List<BaseView> ViewList { set; get; }


        public KLineEditor()
        {
            Controller = null;
            Console = null;
            Model = null;
            Width = Program.PosIntegerNotSet;
            Height = Program.PosIntegerNotSet;
            EditAreaLineWidth = Program.PosIntegerNotSet;
            AutoSavePeriod = 0;
            StatusUpdatePeriod = CmdLineParamsApp.ArgStatusUpdatePeriodDefault;
            Report = Program.ValueNotSet;
            BrowserCmd = CmdLineParamsApp.ArgToolBrowserCmdDefault;
            HelpUrl = CmdLineParamsApp.ArgToolHelpUrlDefault;
            SearchUrl = CmdLineParamsApp.ArgToolSearchUrlDefault;
            ThesaurusUrl = CmdLineParamsApp.ArgToolThesaurusUrlDefault;
            SpellUrl = CmdLineParamsApp.ArgToolSpellUrlDefault;
            Ready = false;


            ViewList = new List<BaseView>();

        }

        public MxReturnCode<string> Run(CmdLineParamsApp cmdLineParams, ChapterModel editModel, IMxConsole console)
        {
            var rc = new MxReturnCode<string>("KLineEditor.Run");

            if ((cmdLineParams == null) || (editModel == null) || (editModel.Ready == false) || (console == null) )
                rc.SetError(1030101, MxError.Source.Param, $"cmdLineParams is null, editModel is null or not read, or console is null or error", MxMsgs.MxErrInvalidParamArg);
            else
            {
                var rcInit = Initialise(cmdLineParams, editModel, console);
                rc += rcInit;
                if (rcInit.IsSuccess(true))
                {
                    var rcSetup = CreateViews(cmdLineParams, console, editModel);
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
                        var rcClose = TerminateViews(editModel, console);
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

        public static bool StartBrowser(string command, string url)     //call from within try/catch
        {
            var rc = false;

            if (string.IsNullOrEmpty(url) == false)
            {   
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                { 
                    url = url.Replace("&", "^&");  //see https://github.com/dotnet/corefx/issues/10361
                    System.Diagnostics.Process.Start(new ProcessStartInfo(command, $"/c start {url}") { CreateNoWindow = true });
                    rc = true;
                }
                else 
                {
                    System.Diagnostics.Process.Start(command, url);
                    rc = true;
                }
            }
            return rc;
        }

        public static bool IsValidUri(string uri)
        {
            var rc = false;

            if (string.IsNullOrEmpty(uri) == false)
            {
                if (Uri.TryCreate(uri, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    rc = true;
                //if (Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                //    rc = true;
            }
            return rc;
        }

        public static string GetXlatUrl(string generalUrl, string wordMarker, string replaceWord)
        {
            string rc = null;
            if ((string.IsNullOrEmpty(generalUrl) == false) && (string.IsNullOrEmpty(wordMarker) == false) && (string.IsNullOrEmpty(replaceWord) == false))
                rc = generalUrl.Replace(wordMarker, replaceWord);
            return rc;
        }


        private MxReturnCode<bool> Initialise(CmdLineParamsApp param, ChapterModel model, IMxConsole console)
        {
            var rc = new MxReturnCode<bool>("KLineEditor. Initialise");

            if ((param == null) || ((model?.Ready ?? false ) == false) || (console == null)) 
                rc.SetError(1030201, MxError.Source.Param, $"param is null or model null, not ready, console is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    BrowserCmd = param.ToolBrowserCmd;
                    HelpUrl = param.ToolHelpUrl;
                    SearchUrl = param.ToolSearchUrl;
                    ThesaurusUrl = param.ToolThesaurusUrl;
                    SpellUrl = param.ToolSpellUrl;

                    EditAreaLineWidth = param.TextEditorDisplayCols; //there is actually an additional column used by cursor when at end of line
                    StatusUpdatePeriod = param.StatusUpdatePeriod;
                    AutoSavePeriod = param.TextEditorAutoSavePeriod;
                    Width = WindowFrameCols + param.TextEditorDisplayCols;  
                    Height = WindowFrameRows + param.TextEditorDisplayRows;

                    var settings = new MxConsoleProperties
                    {
                        Title = $"{Program.CmdAppName} v{Program.CmdAppVersion} - {param.EditFile ?? "[null]"}: Chapter {model.ChapterHeader?.Properties?.Title ?? Program.ValueNotSet}",
                        LargestWindowHeight = console.GetLargestWindowHeight(),
                        LargestWindowWidth = console.GetLargestWindowWidth(),
                        BufferHeight = Height,
                        BufferWidth = Width,
                        WindowHeight = Height,
                        WindowWidth = Width,
                        CursorSize = param.TextEditorCursorSize
                    };
                    if (settings.Validate() == false) 
                        rc.SetError(1030202, MxError.Source.Sys, $"settings.Validate() failed; {settings.GetValidationError()}");
                    else
                    {
                        var rcSettings = console.ApplySettings(settings);
                        rc += rcSettings;
                        if (rcSettings.IsSuccess(true))
                        {
                            Console = console;
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
            var rc = new MxReturnCode<bool>("Edit.ApplySettings");

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
                var rcSettings = Console.GetSettings();
                rc += rcSettings;
                if (rcSettings.IsSuccess(true))
                {
                    var consoleSettings = rcSettings.GetResult();
                    if (consoleSettings.Validate() == false)
                        rc.SetError(1030402, MxError.Source.Sys, $"settings.Validate() failed; {consoleSettings.GetValidationError()}", MxMsgs.MxErrInvalidSettingsFile);
                    else
                    {
                        if (((Controller = ControllerFactory.Make(Model, ControllerFactory.PropsEditingController, BrowserCmd, HelpUrl, SearchUrl, ThesaurusUrl, SpellUrl)) == null) || Controller.IsErrorState())
                            rc.SetError(1030403, MxError.Source.Program, $"ControllerFactory.Make failed", MxMsgs.MxErrInvalidCondition);
                        else
                        {
                            try
                            {
                                Model.SetStatusLine();
                                var lastStatusUpdateUtc = DateTime.UtcNow;
                                var lastAutoSaveUtc = DateTime.UtcNow;
                                var lastKeyPress = lastStatusUpdateUtc;

                                //!!!!!!!!!!!!!!!!!!! MAIN LOOP !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                                while (rc.IsError() == false)
                                {
                                    if ((AutoSavePeriod > 0) && ((DateTime.UtcNow - lastAutoSaveUtc).TotalMinutes > AutoSavePeriod))
                                    {
                                        lastAutoSaveUtc = DateTime.UtcNow;
                                        Model.Save();
                                    }

                                    if ((StatusUpdatePeriod != 0) && ((DateTime.UtcNow - lastStatusUpdateUtc).TotalMilliseconds > StatusUpdatePeriod))
                                    {
                                        lastStatusUpdateUtc = DateTime.UtcNow;
                                        if (Console.IsWindowSizeChanged(consoleSettings.WindowWidth, consoleSettings.WindowHeight))
                                            Controller.SetRefresh();
                                        else
                                        {
                                            Model.SetStatusLine();
                                            if (Model.ChapterHeader.PauseProcessing(lastStatusUpdateUtc, lastKeyPress, false) == false)
                                            { //move error handling so it sets the Model.ErrorState
                                                rc.SetError(1030404, MxError.Source.Program, $"PauseProcessing failed", MxMsgs.MxErrInvalidCondition);
                                                break;
                                            }
                                        }
                                    }

                                    if (Console.IsKeyAvailable())
                                    {
                                        lastKeyPress = DateTime.UtcNow;
                                        if (Model.ChapterHeader.PauseProcessing(lastStatusUpdateUtc, lastKeyPress, true) == false)
                                        {
                                            rc.SetError(1030405, MxError.Source.Program, $"PauseProcessing failed", MxMsgs.MxErrInvalidCondition);
                                            break;
                                        }
                                        var rcKey = Console.GetKeyInfo(true);
                                        if (rcKey.IsError(true))
                                            rc += rcKey;
                                        else
                                        {
                                            Controller = Controller.ProcessKey(Model, rcKey.GetResult());
                                            Console.SetCursorInsertMode((Controller.IsInsertMode()));
                                        }
                                    }

                                    var rcErr = Controller.ErrorProcessing(ViewList);
                                    if (rcErr.IsError(true) || (rcErr.GetResult() == false))
                                        rc += rcErr;
                                    else
                                    {
                                        if (Controller.IsRefresh())
                                        {
                                            rc += Console.ApplySettings(consoleSettings);
                                            if (rc.IsSuccess())
                                                Model.Refresh();
                                        }

                                        if (Controller.IsQuit() )
                                            break;
                                    }

                                    Thread.Sleep(0);
                                }

                                //!!!!!!!!!!!!!!!!!!! MAIN LOOP !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                                var rcDoneProc = Model.ChapterHeader.KLineEditorProcessDone(lastStatusUpdateUtc, lastKeyPress);
                                if (rc.IsSuccess())
                                {
                                    rc += rcDoneProc; //keep any existing error
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

        private MxReturnCode<bool> CreateViews(CmdLineParamsApp cmdLineParams, IMxConsole console, ChapterModel model)
        {
            var rc = new MxReturnCode<bool>("KLineEditor.CreateViews");

            if ((cmdLineParams == null) || (ViewList == null) || (console == null) || (model == null) || (model.Ready == false))
                rc.SetError(1030601, MxError.Source.Param, $"cmdLineParams= is null, or ViewList is null, console is null, or model is null or not ready", MxMsgs.MxErrInvalidParamArg);
            else
            {
                try
                {                    //subscription order determines order in which views are notified
                    var editorHelpView = new EditorHelpLineView(console);
                    var rcCmds = editorHelpView.Setup(cmdLineParams);
                    rc += rcCmds;
                    if (rcCmds.IsSuccess(true))
                    {
                        model.Subscribe(editorHelpView);                        //editorHelpLine
                        ViewList.Add(editorHelpView);

                        var msgLineView = new MsgLineView(console);
                        var rcMsg = msgLineView.Setup(cmdLineParams);
                        rc += rcMsg;
                        if (rcMsg.IsSuccess(true))
                        {
                            model.Subscribe(msgLineView);                       //MsgLine
                            ViewList.Add(msgLineView);

                            var statusLineView = new StatusLineView(console);
                            var rcStatus = statusLineView.Setup(cmdLineParams);
                            rc += rcStatus;
                            if (rcStatus.IsSuccess(true))
                            {
                                model.Subscribe(statusLineView);                //StatusLine
                                ViewList.Add(statusLineView);

                                //make sure the EditAreaViews are notified after editHelp, msg, status so the cursor to set back to EditArea

                                var textEditView = new TextEditView(console);
                                var rcTxt = textEditView.Setup(cmdLineParams);
                                rc += rcTxt;
                                if (rcTxt.IsSuccess(true))
                                {
                                    model.Subscribe(textEditView);              //textEditView
                                    ViewList.Add(textEditView);

                                    var propsEditView = new PropsEditView(console);
                                    var rcProps = propsEditView.Setup(cmdLineParams);
                                    rc += rcProps;
                                    if (rcProps.IsSuccess(true))
                                    {
                                        model.Subscribe(propsEditView);         //PropsEditView
                                        ViewList.Add(propsEditView);

                                        var spellEditView = new SpellEditView(console);
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
                    TerminateViews(model, console);
            }
            return rc;
        }

        private MxReturnCode<bool> TerminateViews(ChapterModel model, IMxConsole console)
        {
            var rc = new MxReturnCode<bool>("KLineEditor.TerminateViews");

            if ((model == null) || (ViewList == null) || (console == null))
                rc.SetError(1030701, MxError.Source.Param, $"ViewList is null, or console is null, or model is null", MxMsgs.MxErrInvalidParamArg);
            else
            {
                //model.DisconnectAllViews();      //not needed as Finish() calls Model.Close() which in turn calls DisconnectAllViews() irrespective of save or not

                if (model.GetSubscriberCount() > 0)
                    rc.SetError(1030702, MxError.Source.Program, $"Views still subscribing to model; count={model.GetSubscriberCount()}", MxMsgs.MxErrInvalidCondition);

                foreach (var view in ViewList)
                    rc += view.Close();         //add any error held in View.MxErrorCode

                rc += console.Close();         //get any lingering _mxErrorCode

                if (rc.IsSuccess())
                    rc.SetResult(true);
            }
            return rc;
        }
    }
}
