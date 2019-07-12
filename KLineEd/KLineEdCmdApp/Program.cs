using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using KLineEdCmdApp.Controller;
using Microsoft.Extensions.Configuration;
using MxDotNetUtilsLib;
using MxReturnCode;

using KLineEdCmdApp.Utils;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils.Properties;

namespace KLineEdCmdApp
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    [SuppressMessage("ReSharper", "InvertIf")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "RedundantNameQualifier")]
    public class Program
    {
        public const int PosIntegerNotSet = -1; //used for default values of params so cannot be readonly
        public const string ValueNotSet = "[not set]";
        public const string ValueOverflow = "...";

        public static readonly string CmdAppVersion = typeof(Program).GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? Program.ValueNotSet;
        public static readonly string CmdAppName = typeof(Program).GetTypeInfo()?.Assembly?.GetName().Name ?? Program.ValueNotSet;
        public static readonly string CmdAppCopyright = typeof(Program).GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? Program.ValueNotSet;

        static int Main(string[] args)
        {
            var rc = new MxReturnCode<int>(invokeDetails: $"{Program.CmdAppName} v{Program.CmdAppVersion}", defaultResult: PosIntegerNotSet);

            var terminal = new Terminal();
            var cmdLineParams = new CmdLineParamsApp();
            IConfigurationRoot config = null;

            try
            {
                var reporting = true;
                // var reporting = (CmdLineParams.GetParamValue(args, "--reporting") == "on") ? true : false;
                if (reporting)
                    config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())?.AddJsonFile("appsettings.json")?.AddUserSecrets<Program>().Build();

                rc.Init(asm: Assembly.GetExecutingAssembly(), reportToEmail: "admin@imageqc.com", supportedLanguages: MxMsgs.SupportedCultures, null,
                    config?["ConnectionStrings:AzureWebJobsServiceBus"], sbqName: config?["MxLogMsg:AzureServiceBusQueueName"]);

                terminal.WriteLine(Resources.WelcomeNotice, Program.CmdAppName, Program.CmdAppVersion, Program.CmdAppCopyright, Environment.NewLine);

                var rcParams = cmdLineParams.Initialise(args: args);
                rc += rcParams;
                if (rcParams.IsSuccess(reporting: true))
                {
                    if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Help)
                    {
                        terminal.WriteLine(cmdLineParams.HelpHint);
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
                                terminal.WriteLine(rcOp.GetResult());
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

            if (rc.IsError(reporting: true))
            {
                terminal.WriteLine(rc.GetErrorUserMsg());
                terminal.WriteLine(cmdLineParams.HelpHint);
            }
            else
            {
                var rcReport = GetRunReport(cmdLineParams: cmdLineParams);
                rc += rcReport;
                terminal.WriteLine(rcReport.IsError(reporting: true) ? rc.GetErrorUserMsg() : rcReport.GetResult());
            }

            // DisplayTextLine(cmdLineParams.ToString());

            terminal.WriteLine((rc.IsSuccess()) ? $"program ends - bye-bye :-) return code {rc.GetResult()} - success" : $"program abends: return code {rc.GetResult()} - failure");
            return rc.GetResult();
        }

        private static MxReturnCode<string> EditProcessing(CmdLineParamsApp cmdLineParams, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("Program.EditProcessing");

            if ((cmdLineParams == null) || (terminal == null) || (terminal.IsError()))
                rc.SetError(1010201, MxError.Source.Param, $"cmdLineParams is null, or terminal is null or error", "MxErrInvalidParamArg");
            else
            {
                var editModel = new ChapterModel();
                var rcInitModel = editModel.Initialise(cmdLineParams.DisplayLineWidth, cmdLineParams.EditFile);
                rc += rcInitModel;
                if (rcInitModel.IsSuccess(true))
                {
                    terminal.WriteLine($"{Environment.NewLine}Ready to edit chapter: {editModel.Header?.Chapter?.Title ?? "[null]"}{Environment.NewLine}");
                    terminal.WriteLine(editModel.GetChapterReport());
                    terminal.WriteLine($"{Environment.NewLine}Press 'Esc' to cancel, or any other key to open the KLineEd editor...");

                    var op = terminal.GetKey(true);
                    if (op == ConsoleKey.Escape)
                    {
                        cmdLineParams.Op = CmdLineParamsApp.OpMode.Abort;
                        rc.SetResult($"{Environment.NewLine}Edit cancelled - chapter is unchanged");
                    }
                    else
                    {
                        var originalSettings = terminal.GetSettings();
                        if (originalSettings?.IsError() ?? false)
                            rc.SetError(1010202, MxError.Source.Data, $"Terminal originalSettings not saved", "MxErrInvalidCondition");
                        else
                        {
                            var rcRun = KLineEditor.RunEditor(cmdLineParams, editModel, terminal);
                            rc += rcRun;

                            if (terminal.Setup(originalSettings) == false)
                                rc.SetError(1010203, MxError.Source.Data, $"Error: Terminal settings not restored. {terminal?.ErrorMsg ?? Program.ValueNotSet }. Please report this error.", "MxErrInvalidCondition");
                        }
                    }
                }
            }
            return rc;
        }

        private static MxReturnCode<string> ResetProcessing(CmdLineParamsApp cmdLineParams, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("Program.ResetProcessing");

            try
            {
                if (cmdLineParams.UpdateSettings != CmdLineParamsApp.BoolValue.Yes)
                    rc.SetError(1010501, MxError.Source.Program, $"{CmdLineParamsApp.ArgSettingsUpdate} not set", "MxErrInvalidParamArg");
                else
                {
                    terminal.WriteLine($"resetting {EnumOps.XlatToString(cmdLineParams.ResetType)} in '{cmdLineParams.SettingsFile}'...");
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
                rc.SetError(1010502, MxError.Source.Exception, $"{e.Message}");
            }
            return rc;
        }

        private static MxReturnCode<string> ExportProcessing(CmdLineParamsApp cmdLineParams, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("Program.ExportProcessing");

            if ((cmdLineParams.EditFile == null) || (cmdLineParams.ExportFile == null))
                rc.SetError(1010601, MxError.Source.Program, $"EditFile={cmdLineParams.EditFile ?? "[null]"}, ExportFile={cmdLineParams.ExportFile ?? "[null]"}", "MxErrInvalidParamArg");
            else
            {
                try
                {
                    if (File.Exists(cmdLineParams.EditFile) == false)
                        rc.SetError(101062, MxError.Source.User, $"EditFile={cmdLineParams.EditFile ?? "[null]"} not found");
                    else
                    {
                        var folder = Path.GetDirectoryName(cmdLineParams.ExportFile);
                        if ((String.IsNullOrEmpty(folder) == false) && (Directory.Exists(folder) == false))
                            rc.SetError(1010603, MxError.Source.User, $"folder for output file {cmdLineParams.ExportFile} does not exist. Create folder and try again.");
                        else
                        {
                            terminal.WriteLine($"exporting {cmdLineParams.EditFile} to {cmdLineParams.ExportFile}...");
                            //process editfile to remove all metadata - see EditFileOps class
                            File.Copy(cmdLineParams.EditFile, cmdLineParams.ExportFile, true);
                            rc.SetResult("...succeeded");
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1010604, MxError.Source.Exception, $"{e.Message}");
                }
            }
            return rc;
        }

        private static MxReturnCode<string> GetRunReport(CmdLineParamsApp cmdLineParams)
        {
            var rc = new MxReturnCode<string>("Program.GetRunReport");

            if (cmdLineParams == null) 
                rc.SetError(10106701, MxError.Source.Program, $"cmdLineParams is null", "MxErrInvalidParamArg");
            else
            {
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
                    rc.SetError(1010702, MxError.Source.Program, $"invalid Op={EnumOps.XlatToString(cmdLineParams.Op)} not supported", "MxErrInvalidCondition");
                }
            }
            return rc;
        }
    }
}
