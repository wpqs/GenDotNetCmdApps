using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MxDotNetUtilsLib;
using MxReturnCode;

using KLineEdCmdApp.Utils;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Properties;

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
        public const string MxNoError = "[no error]";

        public static readonly string CmdAppVersion = typeof(Program).GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? Program.ValueNotSet;
        public static readonly string CmdAppName = typeof(Program).GetTypeInfo()?.Assembly?.GetName().Name ?? Program.ValueNotSet;
        public static readonly string CmdAppCopyright = typeof(Program).GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? Program.ValueNotSet;

        public static readonly string CmdAppHelpUrl = $"https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual{Program.GetAppUserManualVersion()}";

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
                    MxReturnCode<string> rcOp = null;
                    if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Help)
                    {
                        rcOp = HelpProcessing(cmdLineParams: cmdLineParams, terminal);
                    }
                    else  if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Edit)
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
                        rc.SetError(errorCode: 1010101, errType: MxError.Source.Program, errorMsg: $"invalid Op={cmdLineParams.Op} not supported", MxMsgs.MxErrInvalidCondition);
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
            catch (Exception e)
            {
                rc.SetError(errorCode: 1010102, errType: MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }

            if (rc.IsError(reporting: true))
            {
                terminal.WriteLine(rc.GetErrorUserMsg());
                terminal.WriteLine(cmdLineParams.HelpHint);
            }

           // terminal.WriteLine(cmdLineParams.ToString());

            terminal.WriteLine((rc.IsSuccess()) ? $"program ends - bye-bye :-) return code {rc.GetResult()} - success" : $"program abends: return code {rc.GetResult()} - failure");
            return rc.GetResult();
        }

        private static MxReturnCode<string> HelpProcessing(CmdLineParamsApp cmdLineParams, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("Program.HelpProcessing");

            try
            {
                terminal.WriteLine(cmdLineParams.HelpHint);
                terminal.WriteLine("end of report");
                cmdLineParams.HelpHint = "";
                rc.SetResult("succeeded");
            }
            catch (Exception e)
            {
                rc.SetError(1010201, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            return rc;
        }

        private static MxReturnCode<string> EditProcessing(CmdLineParamsApp cmdLineParams, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("Program.EditProcessing");

            if ((cmdLineParams == null) || (terminal == null) || (terminal.IsError()))
                rc.SetError(1010301, MxError.Source.Param, $"cmdLineParams is null, or terminal is null or error", MxMsgs.MxErrInvalidParamArg);
            else
            {
                try
                {
                    var editModel = new ChapterModel();
                    var rcInitModel = editModel.Initialise(cmdLineParams.EditAreaLinesCount, cmdLineParams.EditAreaLineWidth, cmdLineParams.EditFile); //todo CmdLineParams.SpacesForTab, CmdLineParamsParaBreakChar;
                    rc += rcInitModel;
                    if (rcInitModel.IsSuccess(true))
                    {
                        terminal.WriteLine($"{Environment.NewLine}Ready to edit chapter: {editModel.ChapterHeader?.Properties?.Title ?? "[null]"}{Environment.NewLine}");
                        terminal.WriteLine(editModel.GetChapterReport());
                        terminal.WriteLine($"{Environment.NewLine}Press 'Esc' to cancel, or any other key to open the KLineEd editor...");

                        var op = terminal.GetKey(true);
                        if (op == ConsoleKey.Escape)
                        {
                            terminal.WriteLine($"{Environment.NewLine}Edit cancelled - chapter is unchanged");

                            cmdLineParams.HelpHint = "";
                            rc.SetResult("succeeded");
                        }
                        else
                        {
                            var originalSettings = terminal.GetSettings();
                            if (originalSettings?.IsError() ?? false)
                                rc.SetError(1010302, MxError.Source.Data, $"Terminal original settings not saved", MxMsgs.MxErrInvalidCondition);
                            else
                            {
                                var editor = new KLineEditor();
                                var rcRun = editor.Run(cmdLineParams, editModel, terminal);
                                rc += rcRun;  //same as rc.SetResult(rcRun.GetResult());

                                if (terminal.Setup(originalSettings) == false)
                                    rc.SetError(1010303, terminal.GetErrorSource(), $"Terminal settings not restored. {terminal.GetErrorTechMsg()}", terminal.GetErrorUserMsg());
                                else
                                {
                                    var report = Environment.NewLine;   //reports always start with newline
                                    report += String.Format(Resources.WelcomeNotice, Program.CmdAppName, Program.CmdAppVersion, Program.CmdAppCopyright, Environment.NewLine);
                                    report += $"{editor.Report}{Environment.NewLine}";

                                    var lines = report.Split(Environment.NewLine, StringSplitOptions.None);
                                    foreach (var line in lines)
                                        terminal.Write(line + Environment.NewLine);
                                    terminal.WriteLine($"{Environment.NewLine}end of report{Environment.NewLine}");

                                    cmdLineParams.HelpHint = "";
                                    if (rc.IsSuccess())
                                        rc.SetResult(rcRun.GetResult());
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1010302, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
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
                    rc.SetError(1010401, MxError.Source.Program, $"{CmdLineParamsApp.ArgSettingsUpdate} not set", MxMsgs.MxErrInvalidParamArg);
                else
                {
                    terminal.WriteLine($"resetting {EnumOps.XlatToString(cmdLineParams.ResetType)} in '{cmdLineParams.SettingsFile}'...");
                    var rcReset = cmdLineParams.ResetProperties(cmdLineParams.ResetType); //writes to settingsfile
                    rc += rcReset;
                    if (rcReset.IsError(true))
                        cmdLineParams.HelpHint = "";
                    else
                    {
                        cmdLineParams.HelpHint = "";
                        rc.SetResult("succeeded");
                    }
                }
            }
            catch (Exception e)
            {
                rc.SetError(1010402, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            return rc;
        }

        private static MxReturnCode<string> ExportProcessing(CmdLineParamsApp cmdLineParams, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("Program.ExportProcessing");

            if ((cmdLineParams.EditFile == null) || (cmdLineParams.ExportFile == null))
                rc.SetError(1010501, MxError.Source.Program, $"EditFile={cmdLineParams.EditFile ?? "[null]"}, ExportFile={cmdLineParams.ExportFile ?? "[null]"}", MxMsgs.MxErrInvalidParamArg);
            else
            {
                try
                {
                    if (File.Exists(cmdLineParams.EditFile) == false)
                        rc.SetError(1010502, MxError.Source.User, $"EditFile={cmdLineParams.EditFile ?? "[null]"} not found");
                    else
                    {
                        var folder = Path.GetDirectoryName(cmdLineParams.ExportFile);
                        if ((String.IsNullOrEmpty(folder) == false) && (Directory.Exists(folder) == false))
                            rc.SetError(1010503, MxError.Source.User, $"folder for output file {cmdLineParams.ExportFile} does not exist. Create folder and try again.");
                        else
                        {
                            terminal.WriteLine($"exporting {cmdLineParams.EditFile} to {cmdLineParams.ExportFile}...");
                            //process editfile to remove all metadata - see EditFileOps class
                            File.Copy(cmdLineParams.EditFile, cmdLineParams.ExportFile, true);
                            cmdLineParams.HelpHint = "";
                            rc.SetResult("succeeded");
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1010504, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
            return rc;
        }

        private static string GetAppUserManualVersion()
        {
            var rc = "";

            var parts = Program.CmdAppVersion.Split('.'); //https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual-v1.0
            if (parts.Length > 2)
                rc = $"-v{parts[0]}.{parts[1]}";

            return rc;
        }
    }
}
