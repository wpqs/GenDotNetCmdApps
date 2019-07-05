using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using MxDotNetUtilsLib;
using MxReturnCode;
using Newtonsoft.Json;

// ReSharper disable All

namespace KLineEdCmdApp
{
    //when adding a new parameter:
    // a) define static readonly for i) name (SettingsParam) ii) args (SettingsArgUpdate) iii) default value(s)(SettingsArgUpdateValueDefault)
    // b) add properties for args (SettingsFile, UpdateSettingsFile) 
    // c) update SetDefaultValues() and Update()
    // d) add item enum Param (Settings)
    // e) update ParamProc() and create new private method like SettingsParamProc()
    // f) optionally update ValidateParams()
    // g) update GetParamHelp()
    // h) update unit tests

    //--help
    //--reset colours --settings 'mysettings.json' (update)
    //--export 'drive:path\\filename'
    //--edit 'drive:path\\filename' (--displaylastlines 10 --maxcol 80 --audiocr bell.mpeg 2 --audiokey click.mpeg 2 --backgnd text black details black cmds black spell black --foregnd text white details white cmds white spell white --scrollreview on --editline on  --spellcheck on --settings 'mysettings.json' (update))  

    public class CmdLineParamsApp : CmdLineParams
    {
        public static readonly ConsoleColor UnsetColour = ConsoleColor.DarkMagenta;
        public static readonly string ColourName = "COLOUR";
        public static readonly string EditFileNameForm = "'drive:path\\edit.txt'";
        public static readonly string ExportFileNameForm = "'drive:path\\export.txt'";
        public static readonly string AudioFileNameForm = "'drive:path\\audio.wav'";
        public static readonly string SettingsFileNameForm = $"'drive:path\\{Program.CmdAppName}.json'";

        public static readonly string ParamHelp = "--help";

        public static readonly string ParamReset = "--reset";
        public static readonly string ParamSettings = "--settings";
        public static readonly string ParamExportFile = "--export"; // editfilename.txt exportfilename.txt

        public static readonly string ParamEditFile = "--edit"; // filename.txt
        public static readonly string ParamDisplayLastLines = "--displaylastlines"; // 10
        public static readonly string ParamMaxCol = "--maxcol"; // 80
        public static readonly string ParamAudioCR = "--audiocr"; //bell.mpeg 2
        public static readonly string ParamAudioKeyPress = "--audiokey"; //click.mpeg 4
        public static readonly string ParamBackGndColour = "--backgnd"; // text black(details black) (cmds black) (spell black)
        public static readonly string ParamForeGndColour = "--foregnd"; // text green(details grey) (cmds grey) (spell red)
        public static readonly string ParamScrollReviewMode = "--scrollreview"; // on | off
        public static readonly string ParamEditLineMode = "--editline"; // on | off
        public static readonly string ParamSpellCheckMode = "--spellcheck"; // on | off

        public static readonly int ArgDisplayLastLinesCntDefault = 10;
        public static readonly int ArgDisplayLastLinesCntMin = 0;
        public static readonly int ArgDisplayLastLinesCntMax = 50;

        public static readonly int ArgMaxColDefault = 68;   //counted from Jack Kerouac's book 'On the Road';
        public static readonly int ArgMaxColMax = 250;      //see EditFile.Create() default StreamBuffer size is 1024, Console.Stream is 256 - length CRLF = 254
        public static readonly int ArgMaxColMin = 5;

        public static readonly int ArgVolDefault = 3;
        public static readonly int ArgVolMax = 10;
        public static readonly int ArgVolMin = 0;

        public static readonly string ArgYes = "yes";
        public static readonly string ArgNo = "no";

        public static readonly string ArgBlack = "black";
        public static readonly string ArgBlue = "blue";
        public static readonly string ArgCyan = "cyan";
        public static readonly string ArgDarkBlue = "darkblue";
        public static readonly string ArgDarkCyan = "darkcyan";
        public static readonly string ArgDarkGray = "darkgray";

        public static readonly string ArgDarkGreen = "darkgreen";

        // public static readonly string ArgDarkMagenta = "darkmagenta"; //used as unset colour
        public static readonly string ArgDarkRed = "darkred";
        public static readonly string ArgDarkYellow = "darkyellow";
        public static readonly string ArgGray = "gray";
        public static readonly string ArgGreen = "green";
        public static readonly string ArgMagenta = "magenta";
        public static readonly string ArgRed = "red";
        public static readonly string ArgWhite = "white";
        public static readonly string ArgYellow = "yellow";

        public static readonly string ArgText = "text";
        public static readonly string ArgDetails = "details";   //footer
        public static readonly string ArgCmds = "cmds";
        public static readonly string ArgSpell = "spell";

        public static readonly string ArgResetColours = "colours";
        public static readonly string ArgResetFactory = "factory-defaults";

        public static readonly string ArgSettingsUpdate = "update";
        public static readonly bool ArgSettingsUpdateValueDefault = false;
        public static readonly string ArgSettingsFilenameDefault = $"{Program.CmdAppName}.json";

        public string EditFile { set; get; }
        public string ExportFile { set; get; }
        public int DisplayLastLinesCnt { set; get; }
        public int MaxCol { set; get; }
        public string AudioCRFile { set; get; }
        public string AudioKeyFile { set; get; }

        public ConsoleColor ForeGndTextColour { set; get; }
        public ConsoleColor ForeGndDetailsColour { set; get; }
        public ConsoleColor ForeGndCmdsColour { set; get; }
        public ConsoleColor ForeGndSpellColour { set; get; }

        public ConsoleColor BackGndTextColour { set; get; }
        public ConsoleColor BackGndDetailsColour { set; get; }
        public ConsoleColor BackGndCmdsColour { set; get; }
        public ConsoleColor BackGndSpellColour { set; get; }

        public BoolValue ScrollReview { set; get; }
        public BoolValue EditLine { set; get; }
        public BoolValue SpellCheck { set; get; }

        [JsonIgnore]
        public OpMode Op { set; get; }
        [JsonIgnore]
        public ResetMode ResetType { set; get; }
        [JsonIgnore]
        public BoolValue UpdateSettings { set; get; }
        [JsonIgnore]
        public string SettingsFile { set; get; }
        [JsonIgnore]
        public string HelpHint { set; get; }

        public enum OpMode
        {
            [EnumMember(Value = "Help")] Help = 0,
            [EnumMember(Value = "Reset")] Reset,
            [EnumMember(Value = "Export")] Export,
            [EnumMember(Value = "Edit")] Edit,
            [EnumMember(Value = "Abort")] Abort,
            [EnumMember(Value = "Unknown")] Unknown
        }
        public enum ResetMode
        {
            [EnumMember(Value = "None")] None = 0,
            [EnumMember(Value = "Colours")] Colours,

            [EnumMember(Value = "FactoryDefaults")]
            FactoryDefaults
        }
        public enum BoolValue
        {
            [EnumMember(Value = "unset")] Unset = -1,
            [EnumMember(Value = "yes")] Yes,
            [EnumMember(Value = "no")] No,

        }
        public enum Param
        {
            [EnumMember(Value = "--help")] //public static readonly string ParamHelp = "--help";
            Help = 0,
            [EnumMember(Value = "--reset")] Reset,
            [EnumMember(Value = "--edit")] EditFile,
            [EnumMember(Value = "--export")] ExportFile,

            [EnumMember(Value = "--displaylastlines")]
            DisplayLastLines,
            [EnumMember(Value = "--maxcol")] MaxCol,
            [EnumMember(Value = "--audiocr")] AudioCR,
            [EnumMember(Value = "--audiokey")] AudioKey,
            [EnumMember(Value = "--backgnd")] BackGndColour,
            [EnumMember(Value = "--foregnd")] ForeGndColour,
            [EnumMember(Value = "--scrollreview")] ScrollReview,
            [EnumMember(Value = "--editline")] EditLine,
            [EnumMember(Value = "--spellcheck")] SpellCheck,
            [EnumMember(Value = "--settings")] Settings,
            [EnumMember(Value = "[unknown]")] Unknown
        }

        public override string ToString()
        {
            var rc = "Op=" + EnumOps.XlatToString(Op) + Environment.NewLine;

            rc += "EditFile=" + (EditFile ?? "[null]") + Environment.NewLine;
            rc += "ExportFile=" + (ExportFile ?? "[null]") + Environment.NewLine;

            rc += "DisplayLastLines=" + DisplayLastLinesCnt + Environment.NewLine;
            rc += "MaxCol=" + MaxCol + Environment.NewLine;

            rc += "AudioCRFile=" + (AudioCRFile ?? "[null]") + Environment.NewLine;
            rc += "AudioKeyFile=" + (AudioKeyFile ?? "[null]") + Environment.NewLine;

            rc += "ForeGndTextColour=" + XlatConsoleColourToString(ForeGndTextColour) + Environment.NewLine;
            rc += "ForeGndDetailsColour=" + XlatConsoleColourToString(ForeGndDetailsColour) + Environment.NewLine;
            rc += "ForeGndCmdsColour=" + XlatConsoleColourToString(ForeGndCmdsColour) + Environment.NewLine;
            rc += "ForeGndSpellColour=" + XlatConsoleColourToString(ForeGndSpellColour) + Environment.NewLine;
            rc += "BackGndTextColour=" + XlatConsoleColourToString(BackGndTextColour) + Environment.NewLine;

            rc += "BackGndDetailsColour=" + XlatConsoleColourToString(BackGndDetailsColour) + Environment.NewLine;
            rc += "BackGndCmdsColour=" + XlatConsoleColourToString(BackGndCmdsColour) + Environment.NewLine;
            rc += "BackGndSpellColour=" + XlatConsoleColourToString(BackGndSpellColour) + Environment.NewLine;
            rc += "BackGndSpellColour=" + XlatConsoleColourToString(BackGndSpellColour) + Environment.NewLine;

            rc += "ScrollReview=" + EnumOps.XlatToString(ScrollReview) + Environment.NewLine;
            rc += "EditLine=" + EnumOps.XlatToString(EditLine) + Environment.NewLine;
            rc += "SpellCheck=" + EnumOps.XlatToString(SpellCheck) + Environment.NewLine;

            rc += "SettingsFile=" + (SettingsFile ?? "[null]") + Environment.NewLine;
            rc += "UpdateSettings=" + EnumOps.XlatToString(UpdateSettings) + Environment.NewLine;

            return rc;
        }

        public string GetFullHelpInfo()
        {
            //[--help
            //--reset [colours | factory-defaults] (--settings colours 'mysettings.json' (update)) |
            //--export 'drive:path\\filename' 'drive:path\\filename' |
            //--edit 'drive:path\\filename'
            //   (--displaylastlines 10 <min 0 max 50>
            //    --maxcol 80 <min 0 max 250>
            //    --audiocr 'drive:path\\filename' 3 <min 0 max 10>
            //    --audiokey 'drive:path\\filename' 3 <min 0 max 10>
            //    --backgnd text COLOUR details COLOUR cmds COLOUR spell COLOUR
            //    --foregnd  text COLOUR details COLOUR cmds COLOUR spell COLOUR
            //    --scrollreview [yes | no]
            //    --editline [yes | no]
            //    --spellcheck [yes | no]
            //    --settings 'drive:path\\filename' (update))]  

            var msg = $"{Environment.NewLine}Hint: retry using program's expected parameters and their arguments which are:{Environment.NewLine}";

            msg += Environment.NewLine;
            msg += $"[{ParamHelp} |";

            msg += Environment.NewLine;
            msg += $" {ParamReset} [{ArgResetColours} | {ArgResetFactory}] ({ParamSettings} {SettingsFileNameForm} ({ArgSettingsUpdate})) |";

            msg += Environment.NewLine;
            msg += $" {ParamExportFile} {EditFileNameForm} {ExportFileNameForm} |";

            msg += Environment.NewLine;
            msg += $" {ParamEditFile} {EditFileNameForm}";
            //msg += Environment.NewLine;
            //msg += $"  ({ParamDisplayLastLines} {ArgDisplayLastLinesCntDefault} <min {ArgDisplayLastLinesCntMin} max {ArgDisplayLastLinesCntMax}>";
            //msg += Environment.NewLine;
            //msg += $"   {ParamAudioCR} {FileNameForm} {ArgVolDefault} <min {ArgVolMin} max {ArgVolMax}>";
            //msg += Environment.NewLine;
            //msg += $"   {ParamAudioKeyPress} {FileNameForm} {ArgVolDefault} <min {ArgVolMin} max {ArgVolMax}>";
            //msg += Environment.NewLine;
            //msg += $"   {ParamBackGndColour} {ArgText} {ColourName} {ArgDetails} {ColourName} {ArgCmds} {ColourName} {ArgSpell} {ColourName}";
            //msg += Environment.NewLine;
            //msg += $"   {ParamForeGndColour} {ArgText} {ColourName} {ArgDetails} {ColourName} {ArgCmds} {ColourName} {ArgSpell} {ColourName}";
            //msg += Environment.NewLine;
            //msg += $"   {ParamScrollReviewMode} [{ArgYes} | {ArgNo}]";
            //msg += Environment.NewLine;
            //msg += $"   {ParamEditLineMode} [{ArgYes} | {ArgNo}]";
            //msg += Environment.NewLine;
            //msg += $"   {ParamSpellCheckMode} [{ArgYes} | {ArgNo}]";
            msg += Environment.NewLine;
            msg += $"   {ParamSettings} {SettingsFileNameForm} ({ArgSettingsUpdate}))]";
            msg += Environment.NewLine;

            //msg += Environment.NewLine;
            //msg += $"A permitted {ColourName} is {GetConsoleColourNames()}";

            //msg += Environment.NewLine; 
            return msg;
        }

        public MxReturnCode<bool> ResetProperties(ResetMode mode)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ResetProperties", false);

            if (mode == ResetMode.None)
                rc.SetError(1020101, MxError.Source.Param, "paramLine is null", "MxErrBadMethodParam");
            else
            {
                SetPropertiesDefaults(false, mode);

                var rcSettings = SettingsFileProc(false);
                rc += rcSettings;
                if (rcSettings.IsSuccess(true))
                    rc.SetResult(true);
            }
            return rc;
        }

        protected override void SetDefaultValues() //called from base class as values may be overwritten by values passed from cmdLine
        {
            Op = OpMode.Unknown;

            EditFile = null;
            ExportFile = null;
            DisplayLastLinesCnt = KLineEditor.PosIntegerNotSet;
            MaxCol = KLineEditor.PosIntegerNotSet;
            AudioCRFile = null;
            AudioKeyFile = null;

            ForeGndTextColour = UnsetColour;
            ForeGndDetailsColour = UnsetColour;
            ForeGndSpellColour = UnsetColour;

            BackGndTextColour = UnsetColour;
            BackGndDetailsColour = UnsetColour;
            BackGndCmdsColour = UnsetColour;
            BackGndSpellColour = UnsetColour;

            ScrollReview = BoolValue.Unset;
            EditLine = BoolValue.Unset;
            SpellCheck = BoolValue.Unset;

            UpdateSettings = BoolValue.Unset;
            SettingsFile = ArgSettingsFilenameDefault;

            HelpHint = $"{Environment.NewLine}No further inforamtion{Environment.NewLine}";
        }

        protected override bool IsSettingsUpdate()
        {
            return (UpdateSettings == BoolValue.Yes) ? true : false;
        }

        protected override MxReturnCode<bool> ParamProc(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ParamProc", false);

            if (paramLine == null)
                rc.SetError(1020201, MxError.Source.Param, "paramLine is null", "MxErrBadMethodParam");
            else
            {
                var rcParam = GetParamType(paramLine);
                rc += rcParam;
                if (rcParam.IsError(true))
                    HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\"{Environment.NewLine}" + GetFullHelpInfo();
                else
                {
                    switch (rcParam.GetResult())
                    {
                        case Param.Help:
                        {
                            var rcHelp = ProcessHelpParam(paramLine);
                            rc += rcHelp;
                            if (rcHelp.IsSuccess())
                                rc.SetResult(true);
                        }
                        break;
                        case Param.Reset:
                        {
                            var rcReset = ProcessResetParam(paramLine);
                            rc += rcReset;
                            if (rcReset.IsSuccess())
                                rc.SetResult(true);
                        }
                        break;
                        case Param.ExportFile:
                        {
                            var rcReset = ProcessExportParam(paramLine);
                            rc += rcReset;
                            if (rcReset.IsSuccess())
                                rc.SetResult(true);
                        }
                        break;
                        case Param.EditFile:
                        {
                            var rcReset = ProcessEditParam(paramLine);
                            rc += rcReset;
                            if (rcReset.IsSuccess())
                                rc.SetResult(true);
                        }
                        break;
                        case Param.Settings:
                        {
                            var rcSettings = ProcessSettingsParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                        }
                        break;
                        default: //case Param.Unknown:
                        {
                            HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\"{Environment.NewLine}{Environment.NewLine}No further information{Environment.NewLine}";
                            rc.SetError(1020202, MxError.Source.Program, $"Unsupported parameter {paramLine}", "MxErrUnsupportedParam");
                        }
                        break;
                    }
                }
            }
            return rc;
        }

        protected override MxReturnCode<bool> ValidateParams() //called after SettingsFileProc() and  Update()
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ValidateParams", false);

            if (Op == OpMode.Help)
            {
                rc.SetResult(true);
            }
            else if (Op == OpMode.Reset)
            {
                if (string.IsNullOrEmpty(SettingsFile))
                    rc.SetError(1020301, MxError.Source.User, $"{SettingsFileNameForm} argument missing");
                else
                {
                    if (UpdateSettings != BoolValue.Yes)
                        UpdateSettings = BoolValue.Yes;

                    SetPropertiesDefaults();
                    rc.SetResult(true);
                }
                if (rc.IsError())
                    HelpHint = $"{GetParamHelp((int)Param.Reset)}";
            }
            else if (Op == OpMode.Export)
            {
                if (string.IsNullOrEmpty(EditFile))
                   rc.SetError(1020302, MxError.Source.User, $"{EditFileNameForm} argument missing");
                else
                {
                    if (string.IsNullOrEmpty(ExportFile))
                        rc.SetError(1020303, MxError.Source.User, $"{ExportFileNameForm} argument missing");
                    else
                    {
                        rc.SetResult(true);
                    }
                }
                if (rc.IsError())
                    HelpHint = $"{GetParamHelp((int)Param.ExportFile)}";
            }
            else if (Op == OpMode.Edit)
            {
                if (string.IsNullOrEmpty(EditFile))
                {
                    HelpHint = $"{GetParamHelp((int) Param.EditFile)}";
                    rc.SetError(1020304, MxError.Source.User, $"{EditFileNameForm} argument missing");
                }
                else
                {
                    SetPropertiesDefaults(true);
                    rc.SetResult(true);
                }
            }
            else
            {
                HelpHint = $"{Environment.NewLine}No further information{Environment.NewLine}";
                rc.SetError(1020305, MxError.Source.Program, $"Unsupported parameter={EnumOps.XlatToString(Op)}", "MxErrUnsupportedParam");
            }
            return rc;
        }

        protected override string GetParamHelp(int paramId = -1) // (Param) 
        {
            var rc = "";

            var msg = $"{Environment.NewLine}Hint: retry using expected arguments for the parameter.{Environment.NewLine}";

            Param help = (Param) paramId;
            if (help == Param.Help)
            {
                msg += $"{ParamHelp}";
                msg += GetHelpNotes();
                rc = msg;
            }
            else if (help == Param.Reset)
            {
                msg += $"{ParamReset} [{ArgResetColours} | {ArgResetFactory}]";
                msg += GetHelpNotes();
                rc = msg;
            }
            else if (help == Param.ExportFile)
            {
                msg += $"{ParamExportFile} {EditFileNameForm} {ExportFileNameForm}";
                msg += GetHelpNotes();
                rc = msg;
            }
            else if (help == Param.EditFile)
            {
                msg += $"{ParamEditFile} {EditFileNameForm}";
                msg += GetHelpNotes();
                rc = msg;
            }
            //else if (help == Param.AudioCR)
            //{
            //    msg += $"{ParamAudioCR} {FileNameForm} {FileNameForm} {ArgVolDefault} <min {ArgVolMin} max {ArgVolMax}>";
            //    msg += GetHelpNotes();
            //    rc = msg;
            //}
            //else if (help == Param.AudioKey)
            //{
            //    msg += $"{ParamAudioKeyPress} {FileNameForm} {FileNameForm} {ArgVolDefault} <min {ArgVolMin} max {ArgVolMax}>";
            //    msg += GetHelpNotes();
            //    rc = msg;
            //}
            //else if (help == Param.BackGndColour)
            //{
            //    msg += $"{ParamBackGndColour}  {ArgText} {ColourName} {ArgDetails} {ColourName} {ArgCmds} {ColourName} {ArgSpell} {ColourName}";
            //    msg += Environment.NewLine + $"A permitted '{ColourName}' is:" + Environment.NewLine + " " + GetColourNames() + Environment.NewLine;
            //    msg += GetHelpNotes();
            //    rc = msg;
            //}
            //else if (help == Param.ForeGndColour)
            //{
            //    msg += $"{ParamForeGndColour}  {ArgText} {ColourName} {ArgDetails} {ColourName} {ArgCmds} {ColourName} {ArgSpell} {ColourName}";
            //    msg += Environment.NewLine + $"A permitted '{ColourName}' is:" + Environment.NewLine + " " + GetColourNames() + Environment.NewLine;
            //    msg += GetHelpNotes();
            //    rc = msg;
            //}
            //else if (help == Param.ScrollReview)
            //{
            //    msg += $"{ParamScrollReviewMode}  [{ArgYes} | {ArgNo}]";
            //    msg += GetHelpNotes();
            //    rc = msg;
            //}
            //else if (help == Param.EditLine)
            //{
            //    msg += $"{ParamEditLineMode}  [{ArgYes} | {ArgNo}]";
            //    msg += GetHelpNotes();
            //    rc = msg;
            //}
            //else if (help == Param.SpellCheck)
            //{
            //    msg += $"{ParamSpellCheckMode}  [{ArgYes} | {ArgNo}]";
            //    msg += GetHelpNotes();
            //    rc = msg;
            //}
            else if (help == Param.Settings)
            {
                msg += $"{ParamSettings} 'drive:path\\filename' ({ArgSettingsUpdate})";
                msg += GetHelpNotes();
                rc = msg;
            }
            else
            {
                rc = GetFullHelpInfo();
            }

            return rc;
        }

        protected override MxReturnCode<bool> SettingsFileProc(bool readMode)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.SettingsFileProc");

            try
            {
                var errors = new List<string>();
                var jSettings = new JsonSerializerSettings
                {
                    Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                    {
                        errors.Add(args.ErrorContext.Error.Message);
                        args.ErrorContext.Handled = true;
                    },
                    DefaultValueHandling = DefaultValueHandling.Include,
                    NullValueHandling = NullValueHandling.Include,
                    MissingMemberHandling = MissingMemberHandling.Error,
                };

                if (readMode)
                {
                    if (File.Exists(SettingsFile) == false)
                        rc.SetResult(true);     //not caught by CmdLineParams.Initialise() if SetResult not set here! - a problem in MxDotNetUtils 
                    else
                    {
                        var savedValues = File.ReadAllText(SettingsFile);
                        var savedSettings = JsonConvert.DeserializeObject<CmdLineParamsApp>(savedValues, jSettings);
                        if (errors.Count > 0)
                            rc.SetError(1020401, MxError.Source.User, $"errors={errors.Count}; first={errors[0]}", "MxErrInvalidSettingsFile");
                        else
                        {
                            UpdateProperties(savedSettings);
                            rc.SetResult(true);
                        }
                    }
                }
                else
                {
                    if (UpdateSettings == BoolValue.No) 
                        rc.SetResult(true);
                    else
                    {
                        var newValues = JsonConvert.SerializeObject(this, jSettings);
                        if (errors.Count > 0)
                            rc.SetError(1020402, MxError.Source.User, $"errors={errors.Count}; first={errors[0]}", "MxErrInvalidSettingsFile");
                        else
                        {
                            File.WriteAllText(SettingsFile, newValues);
                            rc.SetResult(true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                rc.SetError(1020403, MxError.Source.Exception, $"{e.Message}");
            }
            if (rc.IsError(true))
                HelpHint = $"{Environment.NewLine}no further information.{Environment.NewLine}";
            return rc;
        }

        private MxReturnCode<bool> ProcessHelpParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessHelpParam", false);

            var rcCnt = GetArgCount(paramLine, ParamHelp);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                if (rcCnt.GetResult() != 0)
                {
                    HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Help)}";
                    rc.SetError(1020501, MxError.Source.User, $"parameter {ParamHelp} has incorrect number of arguments; found {rcCnt.GetResult()} should be none");
                }
                else
                {
                    Op = OpMode.Help;
                    HelpHint = $"Help request:{Environment.NewLine}{GetFullHelpInfo()}";
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        private MxReturnCode<bool> ProcessResetParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessResetParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamReset);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1020601, MxError.Source.User, $"parameter {ParamReset} has incorrect number of arguments; found {argCnt} should be one");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamReset}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        var value = rcArg1.GetResult()?.ToLower() ?? "[not found]";
                        if ((value!= ArgResetColours) && (value != ArgResetFactory))
                            rc.SetError(1020602, MxError.Source.User, $"parameter {ParamReset} has invalid argument; found {value} should be [{ArgResetColours} | {ArgResetFactory}]");
                        else
                        {
                            Op = OpMode.Reset;
                            ResetType = (value == ArgResetColours) ? ResetMode.Colours : ResetMode.FactoryDefaults;
                            rc.SetResult(true);
                        }
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Reset)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessExportParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessExportParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamExportFile);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 2)
                    rc.SetError(1020701, MxError.Source.User, $"parameter {ParamExportFile} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamEditFile}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        EditFile = rcArg1.GetResult();
                        var rcArg2 = GetArgValue(paramLine, 2, true, $"parameter {ParamExportFile}");
                        rc += rcArg2;
                        if (rcArg2.IsSuccess(true))
                        {
                            ExportFile = rcArg2.GetResult();
                            Op = OpMode.Export;
                            rc.SetResult(true);
                        }
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.ExportFile)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessEditParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessEditParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamEditFile);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1020801, MxError.Source.User, $"parameter {ParamEditFile} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamEditFile}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        EditFile = rcArg1.GetResult();
                        Op = OpMode.Edit;
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.EditFile)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessSettingsParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessSettingsParam", false);

            var rcCnt = GetArgCount(paramLine, ParamSettings);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                int argCnt = rcCnt.GetResult();
                if ((argCnt != 1) && (argCnt != 2))
                    rc.SetError(1022001, MxError.Source.User, $"parameter {ParamSettings} has incorrect number of arguments; found {argCnt} should be 1 or 2");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamSettings}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess())
                    {
                        SettingsFile = rcArg1.GetResult();
                        if (argCnt != 2)
                            rc.SetResult(true);
                        else
                        {
                            var rcArg2 = GetArgValue(paramLine, 2, false, $"parameter {ParamSettings}");
                            rc += rcArg2;
                            if (rcArg2.IsSuccess())
                            {
                                var update = rcArg2.GetResult();
                                if ((update != null) && (update != ArgSettingsUpdate))
                                    rc.SetError(1022002, MxError.Source.User, $"parameter {ParamSettings} has incorrect second argument {update}; it should be {ArgSettingsUpdate}");
                                else
                                {
                                    UpdateSettings = BoolValue.Yes;
                                    rc.SetResult(true);
                                }
                            }
                        }
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Settings)}";
            return rc;
        }
        
        private MxReturnCode<Param> GetParamType(string paramLine)
        {
            var rc = new MxReturnCode<Param>("CmdLineParamsApp.GetParamType", Param.Unknown);

            if (paramLine == null)
                rc.SetError(1023001, MxError.Source.Param, $"param is null", "MxErrBadMethodParam");
            else
            {
                var offset = paramLine.IndexOf(spaceChar);
                var name = (offset == -1) ? paramLine.Trim().ToLower() : paramLine.Substring(0, offset).Trim().ToLower();
                var param = Param.Unknown;

                if (EnumOps.XlatFromString(name, out param) == false)
                    rc.SetError(1023002, MxError.Source.User, $"the parameter {name} is invalid");
                else
                {
                    rc.SetResult(param);
                }
            }
            return rc;
        }

        private void UpdateProperties(CmdLineParamsApp savedSettings)
        {
            //if any property in this obj is the default value, then update it with savedSettings value
            //accordingly the final properties are those in the setting file together with any changes found from the cmd line for this run

            if (EditFile == null)
                EditFile = savedSettings.EditFile;
            if (ExportFile == null)
                ExportFile = savedSettings.ExportFile;

            if (DisplayLastLinesCnt == KLineEditor.PosIntegerNotSet)   
                DisplayLastLinesCnt = savedSettings.DisplayLastLinesCnt;
            if (MaxCol == KLineEditor.PosIntegerNotSet)
                MaxCol = savedSettings.MaxCol;

            if (AudioCRFile == null)
                AudioCRFile = savedSettings.AudioCRFile;
            if (AudioKeyFile == null)
                AudioKeyFile = savedSettings.AudioKeyFile;

            if (ForeGndTextColour == UnsetColour)
                ForeGndTextColour = savedSettings.ForeGndTextColour;
            if (ForeGndDetailsColour == UnsetColour)
                ForeGndDetailsColour = savedSettings.ForeGndDetailsColour;
            if (ForeGndCmdsColour == UnsetColour)
                ForeGndCmdsColour = savedSettings.ForeGndCmdsColour;
            if (ForeGndSpellColour == UnsetColour)
                ForeGndSpellColour = savedSettings.ForeGndSpellColour;

            if (BackGndTextColour == UnsetColour)
                BackGndTextColour = savedSettings.BackGndTextColour;
            if (BackGndDetailsColour == UnsetColour)
                BackGndDetailsColour = savedSettings.BackGndDetailsColour;
            if (BackGndCmdsColour == UnsetColour)
                BackGndCmdsColour = savedSettings.BackGndCmdsColour;
            if (BackGndSpellColour == UnsetColour)
                BackGndSpellColour = savedSettings.BackGndSpellColour;

            if (ScrollReview == BoolValue.Unset)
                ScrollReview = savedSettings.ScrollReview;
            if (EditLine == BoolValue.Unset)
                EditLine = savedSettings.EditLine;
            if (SpellCheck == BoolValue.Unset)
                SpellCheck = savedSettings.SpellCheck;
        }

        private void SetPropertiesDefaults(bool unsetOnly = true, ResetMode mode = ResetMode.FactoryDefaults)
        {
            if (mode == ResetMode.FactoryDefaults)
            {
                if ((DisplayLastLinesCnt == KLineEditor.PosIntegerNotSet) || (unsetOnly == false))
                    DisplayLastLinesCnt = ArgDisplayLastLinesCntDefault;
                if ((MaxCol == KLineEditor.PosIntegerNotSet) || (unsetOnly == false))
                    MaxCol = ArgMaxColDefault;

                if ((ScrollReview == BoolValue.Unset) || (unsetOnly == false))
                    ScrollReview = BoolValue.Yes;
                if ((EditLine == BoolValue.Unset) || (unsetOnly == false))
                    EditLine = BoolValue.Yes;
                if ((SpellCheck == BoolValue.Unset) || (unsetOnly == false))
                    SpellCheck = BoolValue.Yes;
            }

            if ((mode == ResetMode.FactoryDefaults) || (mode == ResetMode.Colours))
            {
                if ((ForeGndTextColour == UnsetColour) || (unsetOnly == false))
                    ForeGndTextColour = ConsoleColor.White;
                if ((ForeGndDetailsColour == UnsetColour) || (unsetOnly == false))
                    ForeGndDetailsColour = ConsoleColor.White;
                if ((ForeGndCmdsColour == UnsetColour) || (unsetOnly == false))
                    ForeGndCmdsColour = ConsoleColor.White;
                if ((ForeGndSpellColour == UnsetColour) || (unsetOnly == false))
                    ForeGndSpellColour = ConsoleColor.White;

                if ((BackGndTextColour == UnsetColour) || (unsetOnly == false))
                    BackGndTextColour = ConsoleColor.Black;
                if ((BackGndDetailsColour == UnsetColour) || (unsetOnly == false))
                    BackGndDetailsColour = ConsoleColor.Black;
                if ((BackGndCmdsColour == UnsetColour) || (unsetOnly == false))
                    BackGndCmdsColour = ConsoleColor.Black;
                if ((BackGndSpellColour == UnsetColour) || (unsetOnly == false))
                    BackGndSpellColour = ConsoleColor.Black;
            }
        }

        public bool IsValidColourName(string name)
        {           //see https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor?view=netframework-4.8
            bool rc = false;

            if (name == ArgBlack)
                rc = true;
            else if (name == ArgBlue)
                rc = true;
            else if (name == ArgCyan)
                rc = true;
            else if (name == ArgDarkBlue)
                rc = true;
            else if (name == ArgDarkCyan)
                rc = true;
            else if (name == ArgDarkGray)
                rc = true;
            else if (name == ArgDarkGreen)
                rc = true;
            //else if (name == ArgDarkMagenta)  //used as unset colour
            //    rc = true;
            else if (name == ArgDarkRed)
                rc = true;
            else if (name == ArgDarkYellow)
                rc = true;
            else if (name == ArgGray)
                rc = true;
            else if (name == ArgGreen)
                rc = true;
            else if (name == ArgMagenta)
                rc = true;
            else if (name == ArgRed)
                rc = true;
            else if (name == ArgWhite)
                rc = true;
            else if (name == ArgYellow)
                rc = true;
            else
            {
                rc = false;
            }
            return rc;
        }

        public string XlatConsoleColourToString(ConsoleColor colour)
        {
            var rc = "[unknown]";

            switch (colour)
            {
                case ConsoleColor.Black:
                    rc = ArgBlack;
                    break;
                case ConsoleColor.Blue:
                    rc = ArgBlue;
                    break;
                case ConsoleColor.Cyan:
                    rc = ArgCyan;
                    break;
                case ConsoleColor.DarkBlue:
                    rc = ArgDarkBlue;
                    break;
                case ConsoleColor.DarkCyan:
                    rc = ArgDarkCyan;
                    break;
                case ConsoleColor.DarkGreen:
                    rc = ArgDarkGreen;
                    break;
                //case ConsoleColor.DarkMagenta: //used as unset colour
                //    rc = ArgDarkMagenta;
                //    break;
                case ConsoleColor.DarkRed:
                    rc = ArgDarkRed;
                    break;
                case ConsoleColor.DarkYellow:
                    rc = ArgDarkYellow;
                    break;
                case ConsoleColor.Gray:
                    rc = ArgGray;
                    break;
                case ConsoleColor.Green:
                    rc = ArgGreen;
                    break;
                case ConsoleColor.Magenta:
                    rc = ArgMagenta;
                    break;
                case ConsoleColor.Red:
                    rc = ArgRed;
                    break;
                case ConsoleColor.White:
                    rc = ArgWhite;
                    break;
                case ConsoleColor.Yellow:
                    rc = ArgYellow;
                    break;
                default:
                    rc = "[unknown]";
                    break;
            }

            return rc;
        }
        public string GetConsoleColourNames()
        {           //see https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor?view=netframework-4.8
            string rc = "";

            rc += ArgBlack + " | ";
            rc += ArgBlue + " | ";
            rc += ArgCyan + " | ";
            rc += ArgDarkBlue + " | ";
            rc += ArgDarkCyan + " | ";
            rc += ArgDarkGray + " | ";
            rc += ArgDarkGreen + " | ";
         //   rc += ArgDarkMagenta + " | ";  //used as unset colour
            rc += ArgDarkRed + " | ";
            rc += ArgDarkYellow + " | ";
            rc += ArgGray + " | ";
            rc += ArgGreen + " | ";
            rc += ArgMagenta + " | ";
            rc += ArgRed + " | ";
            rc += ArgWhite + " | ";
            rc += ArgYellow;

            return rc;
        }
    }
}
