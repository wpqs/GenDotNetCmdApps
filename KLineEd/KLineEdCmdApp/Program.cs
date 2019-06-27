using System;
using System.IO;
using System.Reflection;
using KLineEdCmdApp.Properties;
using Microsoft.Extensions.Configuration;
using MxDotNetUtilsLib;
using MxReturnCode;
// ReSharper disable All

namespace KLineEdCmdApp
{
    public class Program
    {
        private delegate void DisplayTextFn(string format, params object[] arg);

        public static readonly string CmdAppVersion = typeof(Program).GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "[not set]";
        public static readonly string CmdAppName = typeof(Program).GetTypeInfo()?.Assembly?.GetName().Name ?? "[not set]";
        private static readonly string CmdAppCopyright = typeof(Program).GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "[not set]";

        static int Main(string[] args)
        {
            var rc = new MxReturnCode<int>(invokeDetails: $"{Program.CmdAppName} v{Program.CmdAppVersion}", defaultResult: -1);

            var tim = DateTime.UtcNow;
            var cmdLineParams = new CmdLineParamsApp();

            try
            {
                var config = new ConfigurationBuilder()?.SetBasePath(Directory.GetCurrentDirectory())?.AddJsonFile("appsettings.json")?.AddUserSecrets<Program>().Build();

                rc.Init(asm: Assembly.GetExecutingAssembly(), reportToEmail: "admin@imageqc.com", supportedLanguages: MxMsgs.SupportedCultures, null, config?["ConnectionStrings:AzureWebJobsServiceBus"], sbqName: config?["MxLogMsg:AzureServiceBusQueueName"]);

                DisplayTextLine(format: Resources.WelcomeNotice, Program.CmdAppName, Program.CmdAppVersion, CmdAppCopyright, Environment.NewLine);

                var rcParams = cmdLineParams.Initialise(args: args);
                rc += rcParams;
                if (rcParams.IsSuccess(reporting: true))
                {
                    if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Help)
                    {
                        DisplayTextLine(format: cmdLineParams.HelpHint);
                        rc.SetResult(val: 0);
                    }
                    else
                    {
                        MxReturnCode<string> rcOp = null;
                        if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Reset)
                        {
                            rcOp = ResetProcessing(cmdLineParams: cmdLineParams, display: DisplayText);
                        }
                        else if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Export)
                        {
                            rcOp = ExportProcessing(cmdLineParams: cmdLineParams, display: DisplayText);
                        }
                        else if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Edit)
                        {
                            rcOp = EditProcessing(cmdLineParams: cmdLineParams, display: DisplayText);
                        }
                        else
                        {
                            rc.SetError(errorCode: 1010101, errType: MxError.Source.Program, errorMsg: $"invalid Op={cmdLineParams.Op} not supported");
                        }
                        if (rcOp != null)
                        {
                            rc += rcOp;
                            if (rcOp.IsSuccess(reporting: true))
                            {
                                DisplayTextLine(format: rcOp.GetResult());
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
                DisplayTextLine(format: rc.GetErrorUserMsg());
                DisplayTextLine(format: cmdLineParams.HelpHint);
            }
            else
            {
                var rcReport = GetRunReport(cmdLineParams: cmdLineParams, elapsed: elapsed);
                rc += rcReport;
                DisplayTextLine(format: rcReport.IsError(reporting: true) ? rc.GetErrorUserMsg() : rcReport.GetResult());
            }

            DisplayTextLine(cmdLineParams.ToString());

            DisplayTextLine(format: (rc.IsSuccess()) ? $"program ends : return code {rc.GetResult()}" : $"program abends: return code {rc.GetResult()}");
            return rc.GetResult();
        }

        private static MxReturnCode<string> EditProcessing(CmdLineParamsApp cmdLineParams, DisplayTextFn display)
        {
            var rc = new MxReturnCode<string>("Program.EditProcessing");

            try
            {
                var folder = Path.GetDirectoryName(cmdLineParams.EditFile);
                if ((string.IsNullOrEmpty(folder) == false) && (Directory.Exists(folder) == false))
                    rc.SetError(1010201, MxError.Source.User, $"folder for edit file {cmdLineParams.EditFile} does not exist. Create folder and try again.");
                else
                {
                    display($"editing '{cmdLineParams.EditFile}'...");

                    if (File.Exists(cmdLineParams.EditFile) == false)
                    {
                        File.CreateText(cmdLineParams.EditFile);
                    }

                    rc.SetResult("...done");
                }
            }
            catch (Exception e)
            {
                rc.SetError(1010202, MxError.Source.Exception, $"{e.Message}");
            }
            return rc;
        }

        private static MxReturnCode<string> ResetProcessing(CmdLineParamsApp cmdLineParams, DisplayTextFn display)
        {
            var rc = new MxReturnCode<string>("Program.ResetProcessing"); ;

            try
            {
                if (cmdLineParams.UpdateSettings != CmdLineParamsApp.BoolValue.Yes)
                    rc.SetError(1010301, MxError.Source.Program, $"{CmdLineParamsApp.ArgSettingsUpdate} not set", "MxErrInvalidParamArg");
                else
                {
                    display($"resetting {EnumOps.XlatToString(cmdLineParams.ResetType)} in '{cmdLineParams.SettingsFile}'...");
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

        private static MxReturnCode<string> ExportProcessing(CmdLineParamsApp cmdLineParams, DisplayTextFn display)
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
                            display($"exporting {cmdLineParams.EditFile} to {cmdLineParams.ExportFile}...");
                            //process editfile to remove all metadata
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

            if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Edit)
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
                rc.SetError(1010501, MxError.Source.Program, $"invalid Op={EnumOps.XlatToString(cmdLineParams.Op)} not supported");
            }
            return rc;
        }
        private static void DisplayTextLine(string format, params object[] arg)
        {
            Console.WriteLine(format ?? "[null]", arg);
        }
        private static void DisplayText(string format, params object[] arg)
        {
            Console.Write(format ?? "[null]", arg);
        }
    }
}
