using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
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
        public const int PosIntegerError = Int32.MinValue;
        public const string ValueNotSet = "[not set]";
        public const string ValueUnknown = "[unknown]";
        public const string ValueOverflow = "...";
        public const string MxNoError = "[no error]";
        public const char NullChar = (char) 0;

        public static readonly string HelpVersion = "-v1.0";
        public static readonly string CmdAppVersion = typeof(Program).GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? Program.ValueNotSet;
        public static readonly string CmdAppName = typeof(Program).GetTypeInfo()?.Assembly?.GetName().Name ?? Program.ValueNotSet;
        public static readonly string CmdAppCopyright = typeof(Program).GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? Program.ValueNotSet;
        public static readonly string CmdAppHelpUrl = $"https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual" + HelpVersion;

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
                    else if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Edit)
                    {
                        rcOp = EditProcessing(cmdLineParams: cmdLineParams, terminal);
                    }
                    else if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Export)
                    {
                        rcOp = ExportProcessing(cmdLineParams: cmdLineParams, terminal);
                    }
                    else if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Import)
                    {
                        rcOp = ImportProcessing(cmdLineParams: cmdLineParams, terminal);
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

            if (cmdLineParams.SettingsDisplay == CmdLineParamsApp.BoolValue.Yes)
                terminal.WriteLines(cmdLineParams.ToString().Split(Environment.NewLine));

            terminal.WriteLine((rc.IsSuccess()) ? $"program ends - bye-bye :-) return code {rc.GetResult()} - success" : $"program abends: return code {rc.GetResult()} - failure");
            return rc.GetResult();
        }

        private static MxReturnCode<string> HelpProcessing(CmdLineParamsApp cmdLineParams, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("Program.HelpProcessing");

            try
            {
                var lines = cmdLineParams.HelpHint.Split(Environment.NewLine);
                foreach (var line in lines)
                {
                    terminal.WriteLine(line);
                }

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
                    terminal.WriteLine($"{Environment.NewLine}Opening file: {cmdLineParams?.EditFile ?? ValueNotSet}");

                    var editModel = new ChapterModel();
                    var rcInitModel = editModel.Initialise(cmdLineParams.TextEditorDisplayRows, cmdLineParams.TextEditorDisplayCols, cmdLineParams.EditFile, cmdLineParams.TextEditorParaBreakDisplayChar, cmdLineParams.TextEditorTabSize, cmdLineParams.TextEditorScrollLimit, cmdLineParams.TextEditorPauseTimeout, cmdLineParams.TextEditorLinesPerPage);
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
                                rc += rcRun; //same as rc.SetResult(rcRun.GetResult());

                                if (terminal.Setup(originalSettings) == false)
                                    rc.SetError(1010303, terminal.GetErrorSource(), $"Terminal settings not restored. {terminal.GetErrorTechMsg()}", terminal.GetErrorUserMsg());
                                else
                                {
                                    var report = Environment.NewLine; //reports always start with newline
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


        private static MxReturnCode<string> ExportProcessing(CmdLineParamsApp cmdLineParams, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("Program.ExportProcessing");

            if ((cmdLineParams.EditFile == null) || (cmdLineParams.ExportOutputFile == null))
                rc.SetError(1010501, MxError.Source.Program, $"EditFile={cmdLineParams.EditFile ?? "[null]"}, ExportOutputFile={cmdLineParams.ExportOutputFile ?? "[null]"}", MxMsgs.MxErrInvalidParamArg);
            else
            {
                try
                {
                    if (File.Exists(cmdLineParams.EditFile) == false)
                        rc.SetError(1010502, MxError.Source.User, $"EditFile={cmdLineParams.EditFile ?? "[null]"} not found");
                    else
                    {
                        var folder = Path.GetDirectoryName(cmdLineParams.ExportOutputFile);
                        if ((String.IsNullOrEmpty(folder) == false) && (Directory.Exists(folder) == false))
                            rc.SetError(1010503, MxError.Source.User, $"folder for output file {cmdLineParams.ExportOutputFile} does not exist. Create folder and try again.");
                        else
                        {
                            cmdLineParams.HelpHint = "";
                            terminal.WriteLine($"exporting {cmdLineParams.EditFile} to {cmdLineParams.ExportOutputFile}...");
                            var rcExport = ExportProc.CreateTxtFile(cmdLineParams.EditFile, cmdLineParams.ExportOutputFile);
                            rc += rcExport;
                            if (rcExport.IsSuccess(true))
                                rc.SetResult(rcExport.GetResult());
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

        private static MxReturnCode<string> ImportProcessing(CmdLineParamsApp cmdLineParams, ITerminal terminal)
        {
            var rc = new MxReturnCode<string>("Program.ImportProcessing");

            if ((cmdLineParams.EditFile == null) || (cmdLineParams.ImportInputFile == null))
                rc.SetError(1010601, MxError.Source.Program, $"EditFile={cmdLineParams.EditFile ?? "[null]"}, ImportInputFile={cmdLineParams.ImportInputFile ?? "[null]"}", MxMsgs.MxErrInvalidParamArg);
            else
            {
                try
                {
                    if (File.Exists(cmdLineParams.ImportInputFile) == false)
                        rc.SetError(1010602, MxError.Source.User, $"ImportInputFile={cmdLineParams.ImportInputFile ?? "[null]"} not found");
                    else
                    {
                        var folder = Path.GetDirectoryName(cmdLineParams.ImportInputFile);
                        if ((String.IsNullOrEmpty(folder) == false) && (Directory.Exists(folder) == false))
                            rc.SetError(1010603, MxError.Source.User, $"folder for input file {cmdLineParams.ImportInputFile} does not exist. Create folder and try again.");
                        else
                        {
                            terminal.WriteLine($"importing {cmdLineParams.ImportInputFile} to {cmdLineParams.EditFile}...");

                            //File.Copy(cmdLineParams.EditFile, cmdLineParams.ExportOutputFile, true);
                            cmdLineParams.HelpHint = "";
                            rc.SetResult("succeeded");
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1010604, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }

            return rc;
        }

    }
}
