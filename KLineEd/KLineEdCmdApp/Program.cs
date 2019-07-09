using System;
using System.IO;
using System.Reflection;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Properties;
using KLineEdCmdApp.View;
using Microsoft.Extensions.Configuration;
using MxDotNetUtilsLib;
using MxReturnCode;
// ReSharper disable All

namespace KLineEdCmdApp
{
    public class Program
    {
        public static readonly string CmdAppVersion = typeof(Program).GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "[not set]";
        public static readonly string CmdAppName = typeof(Program).GetTypeInfo()?.Assembly?.GetName().Name ?? "[not set]";
        public static readonly string CmdAppCopyright = typeof(Program).GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "[not set]";

        static int Main(string[] args)
        {
            var rc = new MxReturnCode<int>(invokeDetails: $"{Program.CmdAppName} v{Program.CmdAppVersion}", defaultResult: KLineEditor.PosIntegerNotSet);

            var tim = DateTime.UtcNow;
            var terminal = new Terminal();
            var cmdLineParams = new CmdLineParamsApp();

            try
            {
                var config = new ConfigurationBuilder()?.SetBasePath(Directory.GetCurrentDirectory())?.AddJsonFile("appsettings.json")?.AddUserSecrets<Program>().Build();

                rc.Init(asm: Assembly.GetExecutingAssembly(), reportToEmail: "admin@imageqc.com", supportedLanguages: MxMsgs.SupportedCultures, null, config?["ConnectionStrings:AzureWebJobsServiceBus"], sbqName: config?["MxLogMsg:AzureServiceBusQueueName"]);

                terminal.WriteLines(Resources.WelcomeNotice, Program.CmdAppName, Program.CmdAppVersion, Program.CmdAppCopyright, Environment.NewLine);

                var rcParams = cmdLineParams.Initialise(args: args);
                rc += rcParams;
                if (rcParams.IsSuccess(reporting: true))
                {
                    if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Help)
                    {
                        terminal.WriteLines(cmdLineParams.HelpHint);
                        rc.SetResult(val: 0);
                    }
                    else
                    {
                        MxReturnCode<string> rcOp = null;
                        if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Edit)
                        {
                            rcOp = EditProcessing(cmdLineParams: cmdLineParams, terminal);
                        }
                        else if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Reset)
                        {
                            rcOp = ResetProcessing(cmdLineParams: cmdLineParams, terminal);
                        }
                        else if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Export)
                        {
                            rcOp = ExportProcessing(cmdLineParams: cmdLineParams, terminal);
                        }
                        else
                        {
                            rc.SetError(errorCode: 1010101, errType: MxError.Source.Program, errorMsg: $"invalid Op={cmdLineParams.Op} not supported", "MxErrInvalidCondition");
                        }
                        if (rcOp != null)
                        {
                            rc += rcOp;
                            if (rcOp.IsSuccess(reporting: true))
                            {
                                terminal.WriteLines(rcOp.GetResult());
                                rc.SetResult(val: 0);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                rc.SetError(errorCode: 1010102, errType: MxError.Source.Exception, e.Message);
            }

            var elapsed = DateTime.UtcNow - tim;
            if (rc.IsError(reporting: true))
            {
                terminal.WriteLines(rc.GetErrorUserMsg());
                terminal.WriteLines(cmdLineParams.HelpHint);
            }
            else
            {
                var rcReport = GetRunReport(cmdLineParams: cmdLineParams, elapsed: elapsed);
                rc += rcReport;
                terminal.WriteLines(rcReport.IsError(reporting: true) ? rc.GetErrorUserMsg() : rcReport.GetResult());
            }

            // DisplayTextLine(cmdLineParams.ToString());

            terminal.WriteLines((rc.IsSuccess()) ? $"program ends - bye-bye :-) return code {rc.GetResult()} - success" : $"program abends: return code {rc.GetResult()} - failure");
            return rc.GetResult();
        }

        private static MxReturnCode<string> EditProcessing(CmdLineParamsApp cmdLineParams, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("Program.EditProcessing");

            var editModel = new ChapterModel();
            var rcInitModel = editModel.Initialise(cmdLineParams.MaxCol, cmdLineParams.EditFile);
            rc += rcInitModel;
            if (rcInitModel.IsSuccess(true))
            {
                terminal.WriteLines($"{Environment.NewLine}Ready to edit chapter: {editModel?.Header?.Chapter?.Title ?? "[null]"}{ Environment.NewLine}");
                terminal.WriteLines(editModel.GetChapterReport());
                terminal.WriteLines($"{Environment.NewLine}Press 'Esc' to cancel, or any other key to open the KLineEd editor...");

                var op = terminal.GetKey(true);
                if (op == ConsoleKey.Escape)
                {
                    cmdLineParams.Op = CmdLineParamsApp.OpMode.Abort;
                    rc.SetResult($"{Environment.NewLine}Edit cancelled - chapter is unchanged");
                }
                else
                {
                    var textLinesView = new TextLinesView(terminal);
                    var rcScreen = textLinesView.Setup(cmdLineParams);
                    rc += rcScreen;
                    if (rcScreen.IsSuccess(true))
                    {
                        using (editModel.Subscribe(textLinesView))
                        {
                            var editController = new KLineEditor();
                            var rcStart = editController.Start(editModel);
                            rc += rcStart;
                            if (rcStart.IsSuccess(true))
                            {
                                var rcProcess = editController.Process();
                                rc += rcProcess;
                                if (rcProcess.IsSuccess(true))
                                {
                                    var report = editController.GetReport();

                                    var rcFinish = editController.Finish();
                                    rc += rcFinish;
                                    if (rcFinish.IsSuccess(true))
                                    {
                                        rc.SetResult(report);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return rc;
        }

        private static MxReturnCode<string> ResetProcessing(CmdLineParamsApp cmdLineParams, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("Program.ResetProcessing"); ;

            try
            {
                if (cmdLineParams.UpdateSettings != CmdLineParamsApp.BoolValue.Yes)
                    rc.SetError(1010301, MxError.Source.Program, $"{CmdLineParamsApp.ArgSettingsUpdate} not set", "MxErrInvalidParamArg");
                else
                {
                    terminal.WriteLines($"resetting {EnumOps.XlatToString(cmdLineParams.ResetType)} in '{cmdLineParams.SettingsFile}'...");
                    var rcReset = cmdLineParams.ResetProperties(cmdLineParams.ResetType); //writes to settingsfile
                    rc += rcReset;
                    if (rcReset.IsError(true))
                        rc.SetResult("...failed");
                    else
                        rc.SetResult("...succeeded");
                }
            }
            catch (Exception e)
            {
                rc.SetError(1010302, MxError.Source.Exception, $"{e.Message}");
            }
            return rc;
        }

        private static MxReturnCode<string> ExportProcessing(CmdLineParamsApp cmdLineParams, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("Program.ExportProcessing"); ;

            try
            {
                if ((cmdLineParams.EditFile == null) || (cmdLineParams.ExportFile == null))
                    rc.SetError(1010401, MxError.Source.Program, $"EditFile={cmdLineParams.EditFile ?? "[null]"}, ExportFile={cmdLineParams.ExportFile ?? "[null]"}", "MxErrInvalidParamArg");
                else
                {
                    if (File.Exists(cmdLineParams.EditFile) == false)
                        rc.SetError(1010402, MxError.Source.User, $"EditFile={cmdLineParams.EditFile ?? "[null]"} not found");
                    else
                    {
                        var folder = Path.GetDirectoryName(cmdLineParams.ExportFile);
                        if ((string.IsNullOrEmpty(folder) == false) && (Directory.Exists(folder) == false))
                            rc.SetError(1010403, MxError.Source.User, $"folder for output file {cmdLineParams.ExportFile} does not exist. Create folder and try again.");
                        else
                        {
                            terminal.WriteLines($"exporting {cmdLineParams.EditFile} to {cmdLineParams.ExportFile}...");
                            //process editfile to remove all metadata - see EditFileOps class
                            File.Copy(cmdLineParams.EditFile, cmdLineParams.ExportFile, true);
                            rc.SetResult("...succeeded");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                rc.SetError(1010404, MxError.Source.Exception, $"{e.Message}");
            }
            return rc;
        }

        private static MxReturnCode<string> GetRunReport(CmdLineParamsApp cmdLineParams, TimeSpan elapsed)
        {
            var rc = new MxReturnCode<string>("Program.GetJobDetails"); ;
            var msg = "";

            if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Abort)
            {
                rc.SetResult(Environment.NewLine);
            }
            else if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Edit)
            {
                msg += Environment.NewLine + "end of report" + Environment.NewLine;
                rc.SetResult(msg);
            }
            else if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Reset)
            {
                msg += Environment.NewLine + "end of report" + Environment.NewLine;
                rc.SetResult(msg);
            }
            else if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Help)
            {
                msg += Environment.NewLine + "end of report" + Environment.NewLine;
                rc.SetResult(msg);
            }
            else
            {
                rc.SetError(1010501, MxError.Source.Program, $"invalid Op={EnumOps.XlatToString(cmdLineParams.Op)} not supported", "MxErrInvalidCondition");
            }
            return rc;
        }
    }
}
