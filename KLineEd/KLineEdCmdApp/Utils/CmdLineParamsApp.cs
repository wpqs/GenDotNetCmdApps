using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using MxDotNetUtilsLib;
using MxReturnCode;
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo



namespace KLineEdCmdApp.Utils
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "RedundantTernaryExpression")]

    //when adding a new parameter:
    // a) define static readonly for i) name (SettingsParam) ii) args (SettingsArgUpdate) iii) default value(s)(SettingsArgUpdateValueDefault)
    // b) add properties for args (SettingsPathFileName, UpdateSettingsFile) 
    // c) update SetDefaultValues() SetFactoryDefaults() andUpdateProperties()
    // d) add item enum Param (Settings)
    // e) update ParamProc() and create new private method like SettingsParamProc()
    // f) optionally update ValidateParams()
    // g) update GetParamHelp()
    // h) update unit tests


    public class CmdLineParamsApp : CmdLineParams
    {
        public static readonly string EditFileType = ".ksx"; //text file in XML format with custom elements
        public static readonly ConsoleColor UnsetColour = ConsoleColor.DarkMagenta;
        public static readonly string ColourName = "COLOUR";
        public static readonly string EditFileNameForm = "'drive:path\\edit'" + EditFileType;
        public static readonly string ExportFileNameForm = "'drive:path\\export.txt'";
        public static readonly string AudioFileNameForm = "'drive:path\\audio.wav'";
        public static readonly string SettingsFileNameForm = $"'drive:path\\{Program.CmdAppName}.json'";

        public static readonly string DictionaryFileDefault = Program.ValueNotSet;
        public static readonly string DictionaryUrlDefault = Program.ValueNotSet;
        public static readonly string DictionaryVersionDefault = Program.ValueNotSet;

        public static readonly string ArgOn = "on";
        public static readonly string ArgOff = "off";

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

        //main operational parameters
        public static readonly string ParamHelp = "--help";
        public static readonly string ParamExportFile = "--export"; // editfilename.ksx exportfilename.txt
        public static readonly string ParamImportFile = "--import"; // exportfilename.txt editfilename.ksx
        public static readonly string ParamFix = "--fix";           // editfilename.ksx fixed.txt 
        public static readonly string ParamEditFile = "--edit";     // filename.ksx 

 //edit operational parameters - general

        public static readonly string ParamSettings = "--settings";

            public static readonly string ArgSettingsDisplay = "display";
            public static readonly bool   ArgSettingsDisplayDefault = false;
            public static readonly string ArgSettingsUpdate = "update";
            public static readonly bool   ArgSettingsUpdateDefault = false;
            public static readonly string ArgSettingsPathFileName = "file";
            public static readonly string ArgSettingsPathFileNameDefault = $"{Program.CmdAppName}.json";

        public static readonly string ParamBackGndColour = "--backgnd";     //  (text COLOUR) (msg-error COLOUR) (msg-warn COLOUR) (msg-note COLOUR) (cmds COLOUR) (status COLOUR) (rule COLOUR)
        public static readonly string ParamForeGndColour = "--foregnd";     //  (text COLOUR) (msg-error COLOUR) (msg-warn COLOUR) (msg-note COLOUR) (cmds COLOUR) (status COLOUR) (rule COLOUR)

            public static readonly string ArgColourText = "text";
            public static readonly string ArgBackGndColourTextDefault = ArgBlack;
            public static readonly string ArgForeGndColourTextDefault = ArgGreen;

            public static readonly string ArgColourMsgError = "msg-error";
            public static readonly string ArgBackGndColourMsgErrorDefault = ArgBlack;
            public static readonly string ArgForeGndColourMsgErrorDefault = ArgRed;

            public static readonly string ArgColourMsgWarn = "msg-warn";
            public static readonly string ArgBackGndColourMsgWarnDefault = ArgBlack;
            public static readonly string ArgForeGndColourMsgWarnDefault = ArgYellow;

            public static readonly string ArgColourMsgNote = "msg-note";
            public static readonly string ArgBackGndColourMsgNoteDefault = ArgBlack;
            public static readonly string ArgForeGndColourMsgNoteDefault = ArgWhite;

            public static readonly string ArgColourCmds = "cmds";
            public static readonly string ArgBackGndColourCmdsDefault = ArgBlack;
            public static readonly string ArgForeGndColourCmdsDefault = ArgDarkBlue;

            public static readonly string ArgColourStatus = "status";
            public static readonly string ArgBackGndColourStatusDefault = ArgBlack;
            public static readonly string ArgForeGndColourStatusDefault = ArgGray;

            public static readonly string ArgColourRule = "rule";
            public static readonly string ArgBackGndColourRuleDefault = ArgBlack;
            public static readonly string ArgForeGndColourRuleDefault = ArgGray;

       public static readonly string ParamBrowser = "--browser";

            public static readonly string ArgBrowserExe = "exe";
            public static readonly string ArgBrowserExeDefault = "explorer.exe";

            public static readonly string ArgBrowserLookupUrl = "url";
            public static readonly string ArgBrowserLookupArg = "arg";

        public static readonly string ParamBrowserHelp = "--browserhelp";             // url 'https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual-v1.0'

            public static readonly string ArgBrowserHelpUrlDefault = "https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual";
            public static readonly string ArgBrowserHelpArgDefault = "{version}";

        public static readonly string ParamBrowserSearch = "--browsersearch";         // url 'https://www.google.com/' arg 'search/{word}'

            public static readonly string ArgBrowserSearchUrlDefault = " https://www.google.com/";
            public static readonly string ArgBrowserSearchArgDefault = "search/{word}";

        public static readonly string ParamBrowserThesaurus = "--browserthesaurus";   // url https://www.thesaurus.com/ arg '/search/{word}'

            public static readonly string ArgBrowserThesaurusUrlDefault = "https://www.thesaurus.com/";
            public static readonly string ArgBrowserThesaurusArgDefault = "search/{word}";

        public static readonly string ParamBrowserSpell = "--browserspell";           // url https://www.spell.com/ arg '/search/{word}'

            public static readonly string ArgBrowserSpellUrlDefault = "https://www.spell.com/";
            public static readonly string ArgBrowserSpellArgDefault = "search/{word}";

        public static readonly string ParamAudioVol = "--audiovol";         //  0  <min 1 max 10> //(0 is off)

                public static readonly int ArgAudioVolDefault = 3;
                public static readonly int ArgAudioVolMax = 10;
                public static readonly int ArgAudioVolMin = 0;
                public static readonly string ArgAudioFileKeyPressDefault = "keypress.mp3";
                public static readonly string ArgAudioFileCrDefault = "cr.mp3";
                public static readonly string ArgAudioFileStartupDefault = "paperinsert.mp3";
                public static readonly string ArgAudioFileEndDefault = "paperremove.mp3";

        public static readonly string ParamSvn = "--svn";                   // username 'wills' password=[secret manager key] url 'https//me.svnrepository.com/books'

                public static readonly string ArgSVNUser = "username";
                public static readonly string ArgSVNUserDefault = Program.ValueNotSet;
                public static readonly string ArgSVNPassword = "password";
                public static readonly string ArgSVNPasswordDefault = Program.ValueNotSet;
                public static readonly string ArgSVNUrl = "url";
                public static readonly string ArgSVNUrlDefault = Program.ValueNotSet;

        //edit operational parameters - text editor

        public static readonly string ParamTextEditorRulers = "--rulers";             //  show [yes | no] unitchar '.'

            public static readonly string ArgTextEditorRulersShow = "show";
            public static readonly string ArgTextEditorRulersShowDefault = ArgYes;
            public static readonly string ArgTextEditorRulersUnitChar = "UnitChar";
            public static readonly char ArgTextEditorRulersUnitCharDefault = '.';

        public static readonly string ParamTextEditorCursorSize = "--cursorsize";     // 20 <min 1 max 100>

            public static readonly int ArgTextEditorCursorSizeDefault = 20;
            public static readonly int ArgTextEditorCursorSizeMax = 100;
            public static readonly int ArgTextEditorCursorSizeMin = 1;

        public static readonly string ParamTextEditorDisplaysRows = "--displayrow";   // 10 <min 1 max 25>	    //was displaylastlines, now TextEditorDisplayRows

            public static readonly int ArgTextEditorDisplayRowsDefault = 10;      //was ArgEditAreaLinesCountDefault
            public static readonly int ArgTextEditorDisplayRowsMax = 50;          //was ArgEditAreaLinesCountMax 
            public static readonly int ArgTextEditorDisplayRowsMin = 5;           //was ArgEditAreaLinesCountMin

        public static readonly string ParamTextEditorDisplaysCols = "--displaycols";  // 80 <min 20 max 250>	    //was DisplayLineWidth, now TextEditorDisplayCols

            public static readonly int ArgTextEditorDisplayColsDefault = 68;      //was ArgEditAreaLineWidthDefault - counted from Jack Kerouac's book 'On the Road'
            public static readonly int ArgTextEditorDisplayColsMax = 250;         //was ArgEditAreaLineWidthMax - see EditFile.Create() default StreamBuffer size is 1024, Console.Stream is 256 - length CRLF = 254
            public static readonly int ArgTextEditorDisplayColsMin = 5;           //was ArgEditAreaLineWidthMin

        public static readonly string ParamTextEditorParaBreak = "--parabreak";       // displaychar ' '

            public static readonly string ArgTextEditorParaBreakDisplayChar = "DisplayChar";
            public const char             ArgTextEditorParaBreakDisplayCharDefault = '>';

        public static readonly string ParamTextEditorPauseWaitSecs = "--typingpause"; // 60 <min 5 max 36000>

            public static readonly int ArgTextEditorPauseWaitSecsDefault = 60;
            public static readonly int ArgTextEditorPauseWaitSecsMin = 0;
            public static readonly int ArgTextEditorPauseWaitSecsMax = 86400;     //24 * 60 * 60 - 24 hours

        public static readonly string ParamTextEditorScrollLimit = "--scrolllimit";     // 0  <min 1 max 10000>		//(0 is unlimited) - was scrollreview, ParamScrollReviewMode

            public static readonly int ArgTextEditorScrollLimitDefault = 0;
            public static readonly int ArgTextEditorScrollLimitMax = 10000;
            public static readonly int ArgTextEditorScrollLimitMin = 0;            //0 is unlimited

        public static readonly string ParamTextEditorEditLimit = "--editlimit";         // 0  <min 1 max 10000>		//(0 is unlimited) - was editline ParamEditLineMode

            public static readonly int ArgTextEditorEditLimitDefault = 0;
            public static readonly int ArgTextEditorEditLimitMax = 10000;
            public static readonly int ArgTextEditorEditLimitMin = 0;              //0 is unlimited

        public static readonly string ParamTextEditorTabSize = "--tabsize";           // 3  <min 1 max 25>	

            public const int           ArgTextEditorTabSizeDefault = 3;
            public static readonly int ArgTextEditorTabSizeMax = 25;
            public static readonly int ArgTextEditorTabSizeMin = 1;

        public static readonly string ParamTextEditorAutoSave = "--autosave";         // [CR | ParaBreak | off]

            public static readonly string ArgTextEditorAutoSaveCR = "CR";
            public static readonly string ArTextEditorAutoSaveParaBreak = "ParaBreak";
            public static readonly string ArgTextEditorAutoSaveOff = ArgOff;
            public static readonly AutoSaveMode ArgTextEditorAutoSaveDefault = AutoSaveMode.Off;

        public static readonly string ParamTextEditorAutoCorrect = "--autocorrect";           // [on | off]

            public static readonly bool ArgTextEditorAutoCorrectDefault = false;

        //properties that are not set from command line
        [JsonIgnore]
        public OpMode Op { set; get; }
        [JsonIgnore]
        public string SettingsPathFileName { set; get; }
        [JsonIgnore]
        public string HelpHint { set; get; }
        [JsonIgnore]
        public string EditFile { set; get; }
        [JsonIgnore]
        public string ExportOutputFile { set; get; }
        [JsonIgnore]
        public string ImportInputFile { set; get; }
        [JsonIgnore]
        public string FixInputFile { set; get; }
        [JsonIgnore]
        public BoolValue SettingsUpdate { set; get; }

        public string AudioFileKeyPress { set; get; }
        public string AudioFileCr { set; get; }
        public string AudioFileStartup { set; get; }
        public string AudioFileEnd { set; get; }

        public BoolValue ReportMxErrors { set; get; }

        public string DictionaryFile { set; get; }
        public string DictionaryUrl { set; get; }
        public string DictionaryVersion { set; get; }

        //edit operational parameters - general

        public BoolValue SettingsDisplay { set; get; }

        public ConsoleColor BackGndColourText {  set; get; }
        public ConsoleColor ForeGndColourText {  set; get; }
        public ConsoleColor BackGndColourMsgError {  set; get; }
        public ConsoleColor ForeGndColourMsgError {  set; get; }
        public ConsoleColor BackGndColourMsgWarn {  set; get; }
        public ConsoleColor ForeGndColourMsgWarn { set; get; }
        public ConsoleColor BackGndColourMsgNote {  set; get; }
        public ConsoleColor ForeGndColourMsgNote { set; get; }
        public ConsoleColor BackGndColourCmds { set; get; }
        public ConsoleColor ForeGndColourCmds { set; get; }
        public ConsoleColor BackGndColourStatus { set; get; }
        public ConsoleColor ForeGndColourStatus { set; get; }
        public ConsoleColor BackGndColourRule { set; get; }
        public ConsoleColor ForeGndColourRule { set; get; }

        public string BrowserExe { set; get; }

        public string BrowserHelpUrl { set; get; }
        public string BrowserHelpArg { set; get; }
        public string BrowserSearchUrl { set; get; }
        public string BrowserSearchArg { set; get; }
        public string BrowserThesaurusUrl { set; get; }
        public string BrowserThesaurusArg { set; get; }
        public string BrowserSpellUrl { set; get; }
        public string BrowserSpellArg {  set; get; }

        public int AudioVol { set; get; }

        //edit operational parameters - text editor

        public BoolValue TextEditorRulersShow { set; get; }
        public char TextEditorRulersUnitChar { set; get; }
        public int TextEditorCursorSize { set; get; }
        public int TextEditorDisplayRows {  set; get; }
        public int TextEditorDisplayCols {  set; get; }
        public char TextEditorParaBreakDisplayChar {  set; get; }
        public int TextEditorPauseWaitSecs {set; get; }
        public int TextEditorScrollLimit {  set; get; }
        public int TextEditorEditLimit { set; get; }
        public int TextEditorTabSize { set; get; }
        public AutoSaveMode TextEditorAutoSave { set; get; }
        public BoolValue TextEditorAutoCorrect { set; get; }

        public enum BoolValue
        {
            [EnumMember(Value = "unset")] Unset = Program.PosIntegerNotSet,
            [EnumMember(Value = "no")] No = 0,
            [EnumMember(Value = "yes")] Yes = 1,

        }

        public enum OpMode
        {
            [EnumMember(Value = "Help")] Help = 0,
            [EnumMember(Value = "Import")]Import,
            [EnumMember(Value = "Export")] Export,
            [EnumMember(Value = "Fix")] Fix,
            [EnumMember(Value = "Edit")] Edit,
            [EnumMember(Value = "Abort")] Abort,
            [EnumMember(Value = "Unknown")] Unknown
        }

        public enum AutoSaveMode
        {
            [EnumMember(Value = "Off")] Off = 0,
            [EnumMember(Value = "CR")] CR,
            [EnumMember(Value = "ParaBreak")] ParaBreak,
            [EnumMember(Value = "Unknown")] Unknown
        }

        public enum Param
        {
            [EnumMember(Value = "--help")] Help = 0,
            [EnumMember(Value = "--edit")] EditFile,
            [EnumMember(Value = "--export")] ExportFile,
            [EnumMember(Value = "--displaylastlines")]DisplayLastLines,
            [EnumMember(Value = "--displaylinewidth")] DisplayLineWidth,
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
            rc += "ExportOutputFile=" + (ExportOutputFile ?? "[null]") + Environment.NewLine;

            rc += "DisplayLastLines=" + TextEditorDisplayRows + Environment.NewLine;  //todo rename
            rc += "DisplayLineWidth=" + TextEditorDisplayCols + Environment.NewLine;    //todo rename

            rc += "AudioFileCr=" + (AudioFileCr ?? "[null]") + Environment.NewLine;
            rc += "AudioFileKeyPress=" + (AudioFileKeyPress ?? "[null]") + Environment.NewLine;

            rc += "ForeGndColourText=" + XlatConsoleColourToString(ForeGndColourText) + Environment.NewLine;
            rc += "ForeGndColourMsgError=" + XlatConsoleColourToString(ForeGndColourMsgError) + Environment.NewLine;
            rc += "ForeGndColourCmds=" + XlatConsoleColourToString(ForeGndColourCmds) + Environment.NewLine;
            rc += "ForeGndColourStatus=" + XlatConsoleColourToString(ForeGndColourStatus) + Environment.NewLine;
            rc += "BackGndColourText=" + XlatConsoleColourToString(BackGndColourText) + Environment.NewLine;

            rc += "BackGndColourMsgError=" + XlatConsoleColourToString(BackGndColourMsgError) + Environment.NewLine;
            rc += "BackGndColourCmds=" + XlatConsoleColourToString(BackGndColourCmds) + Environment.NewLine;
            rc += "BackGndColourStatus=" + XlatConsoleColourToString(BackGndColourStatus) + Environment.NewLine;
            rc += "BackGndColourStatus=" + XlatConsoleColourToString(BackGndColourStatus) + Environment.NewLine;

            rc += "TextEditorAutoCorrect=" + EnumOps.XlatToString(TextEditorAutoCorrect) + Environment.NewLine;

            rc += "SettingsPathFileName=" + (SettingsPathFileName ?? "[null]") + Environment.NewLine;
            rc += "SettingsUpdate=" + EnumOps.XlatToString(SettingsUpdate) + Environment.NewLine;

            return rc;
        }

        public string GetFullHelpInfo()
        {
            //[--help
            //--reset [colours | factory-defaults] (--settings colours 'mysettings.json' (update)) |
            //--export 'drive:path\\filename' 'drive:path\\filename' |
            //--edit 'drive:path\\filename'
            //   (--displaylastlines 10 <min 0 max 50>
            //    --DisplayLineWidth 80 <min 0 max 250>
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

            //msg += Environment.NewLine;
            //msg += $" {ParamReset} [{ArgResetColours} | {ArgResetFactory}] ({ParamSettings} {SettingsFileNameForm} ({ArgSettingsUpdate})) |";

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

        protected override void SetDefaultValues() //called from base class as values may be overwritten by values passed from cmdLine
        {
            Op = OpMode.Unknown;
            SettingsPathFileName = ArgSettingsPathFileNameDefault;
            HelpHint = $"{Environment.NewLine}No further inforamtion{Environment.NewLine}";
            EditFile = null;
            ExportOutputFile = null;
            ImportInputFile = null;
            FixInputFile = null;
            SettingsUpdate = BoolValue.Unset;

            AudioFileKeyPress = null;
            AudioFileCr = null;
            AudioFileStartup = null;
            AudioFileEnd = null;
            ReportMxErrors = BoolValue.Unset;
            DictionaryFile = null;
            DictionaryUrl = null;
            DictionaryVersion = null;
            SettingsDisplay = BoolValue.Unset;

            BackGndColourText = UnsetColour;
            ForeGndColourText = UnsetColour;
            BackGndColourMsgError = UnsetColour;
            ForeGndColourMsgError = UnsetColour;
            BackGndColourMsgWarn = UnsetColour;
            ForeGndColourMsgWarn = UnsetColour;
            BackGndColourMsgNote = UnsetColour;
            ForeGndColourMsgNote = UnsetColour;
            BackGndColourCmds = UnsetColour;
            ForeGndColourCmds = UnsetColour;
            BackGndColourStatus = UnsetColour;
            ForeGndColourStatus = UnsetColour;
            BackGndColourRule = UnsetColour;
            ForeGndColourRule = UnsetColour;

            BrowserExe = null;
            BrowserHelpUrl = null;
            BrowserHelpArg = null;
            BrowserSearchUrl = null;
            BrowserSearchArg = null;
            BrowserThesaurusUrl = null;
            BrowserThesaurusArg = null;
            BrowserSpellUrl = null;
            BrowserSpellArg = null;
            AudioVol = Program.PosIntegerNotSet;

            TextEditorRulersShow = BoolValue.Unset;
            TextEditorRulersUnitChar = Program.NullChar;
            TextEditorCursorSize = Program.PosIntegerNotSet;
            TextEditorDisplayRows = Program.PosIntegerNotSet;
            TextEditorDisplayCols = Program.PosIntegerNotSet;
            TextEditorParaBreakDisplayChar = Program.NullChar;
            TextEditorPauseWaitSecs = Program.PosIntegerNotSet;
            TextEditorScrollLimit = Program.PosIntegerNotSet;
            TextEditorEditLimit = Program.PosIntegerNotSet;
            TextEditorTabSize = Program.PosIntegerNotSet;
            TextEditorAutoSave = AutoSaveMode.Unknown;
            TextEditorAutoCorrect = BoolValue.Unset;
        }

        protected  void SetFactoryDefaults() 
        {
            Op = OpMode.Unknown;
            SettingsPathFileName = ArgSettingsPathFileNameDefault;
            HelpHint = $"{Environment.NewLine}No further inforamtion{Environment.NewLine}";
            EditFile = null;
            ExportOutputFile = null;
            ImportInputFile = null;
            FixInputFile = null;
            SettingsUpdate = BoolValue.No;

            AudioFileKeyPress = ArgAudioFileKeyPressDefault;
            AudioFileCr = ArgAudioFileCrDefault;
            AudioFileStartup = ArgAudioFileStartupDefault;
            AudioFileEnd = ArgAudioFileEndDefault;
            ReportMxErrors = BoolValue.Yes;
            DictionaryFile = DictionaryFileDefault;
            DictionaryUrl = DictionaryUrlDefault;
            DictionaryVersion = DictionaryVersionDefault;
            SettingsDisplay = BoolValue.No;


            BackGndColourText = XlatStringToConsoleColour(ArgBackGndColourTextDefault);
            ForeGndColourText = XlatStringToConsoleColour(ArgForeGndColourTextDefault);
            BackGndColourMsgError = XlatStringToConsoleColour(ArgBackGndColourMsgErrorDefault);
            ForeGndColourMsgError = XlatStringToConsoleColour(ArgForeGndColourMsgErrorDefault);
            BackGndColourMsgWarn = XlatStringToConsoleColour(ArgBackGndColourMsgWarnDefault);
            ForeGndColourMsgWarn = XlatStringToConsoleColour(ArgForeGndColourMsgWarnDefault);
            BackGndColourMsgNote = XlatStringToConsoleColour(ArgBackGndColourMsgNoteDefault);
            ForeGndColourMsgNote = XlatStringToConsoleColour(ArgForeGndColourMsgNoteDefault);
            BackGndColourCmds = XlatStringToConsoleColour(ArgBackGndColourCmdsDefault);
            ForeGndColourCmds = XlatStringToConsoleColour(ArgForeGndColourCmdsDefault);
            BackGndColourStatus = XlatStringToConsoleColour(ArgBackGndColourStatusDefault);
            ForeGndColourStatus = XlatStringToConsoleColour(ArgForeGndColourStatusDefault);
            BackGndColourRule = XlatStringToConsoleColour(ArgBackGndColourRuleDefault);
            ForeGndColourRule = XlatStringToConsoleColour(ArgForeGndColourRuleDefault);

            BrowserExe = ArgBrowserExeDefault;
            BrowserHelpUrl = ArgBrowserHelpUrlDefault;
            BrowserHelpArg = ArgBrowserHelpArgDefault;
            BrowserSearchUrl = ArgBrowserSearchUrlDefault;
            BrowserSearchArg = ArgBrowserSearchArgDefault;
            BrowserThesaurusUrl = ArgBrowserThesaurusUrlDefault;
            BrowserThesaurusArg = ArgBrowserThesaurusArgDefault;
            BrowserSpellUrl = ArgBrowserSpellUrlDefault;
            BrowserSpellArg = ArgBrowserSpellArgDefault;
            AudioVol = Program.PosIntegerNotSet;

            TextEditorRulersShow = BoolValue.Yes;
            TextEditorRulersUnitChar = ArgTextEditorRulersUnitCharDefault;
            TextEditorCursorSize = ArgTextEditorCursorSizeDefault;
            TextEditorDisplayRows = ArgTextEditorDisplayRowsDefault;
            TextEditorDisplayCols = ArgTextEditorDisplayColsDefault;
            TextEditorParaBreakDisplayChar = ArgTextEditorParaBreakDisplayCharDefault;
            TextEditorPauseWaitSecs = ArgTextEditorPauseWaitSecsDefault;
            TextEditorScrollLimit = ArgTextEditorScrollLimitDefault;
            TextEditorEditLimit = ArgTextEditorEditLimitDefault;
            TextEditorTabSize = ArgTextEditorTabSizeDefault;
            TextEditorAutoSave = ArgTextEditorAutoSaveDefault;
            TextEditorAutoCorrect = BoolValue.No;
        }

        protected override bool IsSettingsUpdate()
        {
            return (SettingsUpdate == BoolValue.Yes) ? true : false;
        }

        protected override MxReturnCode<bool> ParamProc(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ParamProc", false);

            if (paramLine == null)
                rc.SetError(1020201, MxError.Source.Param, "paramLine is null", MxMsgs.MxErrBadMethodParam);
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
                            var rcSettings = ProcessSettingsParam(paramLine); //change to be part of EditFile
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                        }
                        break;
                        default: //case Param.Unknown:
                        {
                            HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\"{Environment.NewLine}{Environment.NewLine}No further information{Environment.NewLine}";
                            rc.SetError(1020202, MxError.Source.Program, $"Unsupported parameter {paramLine}", MxMsgs.MxErrUnknownParam);
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
            else if (Op == OpMode.Export)
            {
                if (string.IsNullOrEmpty(EditFile))
                   rc.SetError(1020302, MxError.Source.User, $"{EditFileNameForm} argument missing");
                else
                {
                    if (string.IsNullOrEmpty(ExportOutputFile))
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
                    rc.SetResult(true);
                }
            }
            else
            {
                HelpHint = $"{Environment.NewLine}No further information{Environment.NewLine}";
                rc.SetError(1020305, MxError.Source.Program, $"Unsupported parameter={EnumOps.XlatToString(Op)}", MxMsgs.MxErrUnknownParam);
            }
            return rc;
        }

        protected override string GetParamHelp(int paramId = 0) // paramId = KLineEditor.PosIntegerNotSet 
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
            //else if (help == Param.TextEditorAutoCorrect)
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

                //var factoryDefaults = new CmdLineParamsApp();
                //factoryDefaults.SetFactoryDefaults();
                //var newValues = JsonConvert.SerializeObject(factoryDefaults, jSettings);
                //if (errors.Count > 0)
                //    rc.SetError(1020402, MxError.Source.User, $"errors={errors.Count}; first={errors[0]}", MxMsgs.MxErrInvalidSettingsFile);
                //else
                //{
                //    File.WriteAllText(SettingsPathFileName, newValues);
                //    rc.SetResult(true);
                //}

                if (readMode)
                {
                    if (File.Exists(SettingsPathFileName) == false)
                        rc.SetResult(true);     //not caught by CmdLineParams.Initialise() if SetResult not set here! - a problem in MxDotNetUtils 
                    else
                    {
                        var savedValues = File.ReadAllText(SettingsPathFileName);
                        var savedSettings = JsonConvert.DeserializeObject<CmdLineParamsApp>(savedValues, jSettings);
                        if (errors.Count > 0)
                            rc.SetError(1020401, MxError.Source.User, $"errors={errors.Count}; first={errors[0]}", MxMsgs.MxErrInvalidSettingsFile);
                        else
                        {
                            UpdateProperties(savedSettings);
                            rc.SetResult(true);
                        }
                    }
                }
                else
                {
                    if (SettingsUpdate == BoolValue.No) 
                        rc.SetResult(true);
                    else
                    {
                        var newValues = JsonConvert.SerializeObject(this, jSettings);
                        if (errors.Count > 0)
                            rc.SetError(1020402, MxError.Source.User, $"errors={errors.Count}; first={errors[0]}", MxMsgs.MxErrInvalidSettingsFile);
                        else
                        {
                            File.WriteAllText(SettingsPathFileName, newValues);
                            rc.SetResult(true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                rc.SetError(1020403, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
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
                            ExportOutputFile = rcArg2.GetResult();
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
                        SettingsPathFileName = rcArg1.GetResult();
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
                                    SettingsUpdate = BoolValue.Yes;
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
                rc.SetError(1023001, MxError.Source.Param, $"param is null", MxMsgs.MxErrBadMethodParam);
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
            //if any property in this obj is the NOTSET value, then update it with savedSettings value
            //accordingly the final properties are those in the setting file together with any changes found from the cmd line for this run
            if (AudioFileKeyPress == null)
                AudioFileKeyPress = savedSettings.AudioFileKeyPress;
            if (AudioFileCr == null)
                AudioFileCr = savedSettings.AudioFileCr;
            if (AudioFileStartup == null)
                AudioFileStartup = savedSettings.AudioFileStartup;
            if (AudioFileEnd == null)
                AudioFileEnd = savedSettings.AudioFileEnd;

            if (ReportMxErrors == BoolValue.Unset)
                ReportMxErrors = savedSettings.ReportMxErrors;

            if (DictionaryFile == null)
                DictionaryUrl = savedSettings.DictionaryUrl;
            if (DictionaryVersion == null)
                DictionaryVersion = savedSettings.DictionaryVersion;

            if (SettingsDisplay == BoolValue.Unset)
                SettingsDisplay = savedSettings.SettingsDisplay;
            if (SettingsUpdate == BoolValue.Unset)
                SettingsUpdate = savedSettings.SettingsUpdate;

            if (BackGndColourText == UnsetColour)
                BackGndColourText = savedSettings.BackGndColourText;
            if (ForeGndColourText == UnsetColour)
                ForeGndColourText = savedSettings.ForeGndColourText;
            if (BackGndColourMsgError == UnsetColour)
                BackGndColourMsgError = savedSettings.BackGndColourMsgError;
            if (ForeGndColourMsgError == UnsetColour)
                ForeGndColourMsgError = savedSettings.ForeGndColourMsgError;
            if (BackGndColourMsgWarn == UnsetColour)
                BackGndColourMsgWarn = savedSettings.BackGndColourMsgWarn;
            if (ForeGndColourMsgWarn == UnsetColour)
                ForeGndColourMsgWarn = savedSettings.ForeGndColourMsgWarn;
            if (BackGndColourMsgNote == UnsetColour)
                BackGndColourMsgNote = savedSettings.BackGndColourMsgNote;
            if (ForeGndColourMsgNote == UnsetColour)
                ForeGndColourMsgNote = savedSettings.ForeGndColourMsgNote;
            if (BackGndColourCmds == UnsetColour)
                BackGndColourCmds = savedSettings.BackGndColourCmds;
            if (ForeGndColourCmds == UnsetColour)
                 ForeGndColourCmds = savedSettings.ForeGndColourCmds;
            if (BackGndColourStatus == UnsetColour)
                BackGndColourStatus = savedSettings.BackGndColourStatus;
            if (ForeGndColourStatus == UnsetColour)
                ForeGndColourStatus = savedSettings.ForeGndColourStatus;
            if (BackGndColourRule == UnsetColour)
                ForeGndColourRule = savedSettings.ForeGndColourRule;

            if (BrowserExe == null)
                BrowserExe = savedSettings.BrowserExe;
            if (BrowserHelpUrl == null)
                BrowserHelpUrl = savedSettings.BrowserHelpUrl;
            if (BrowserHelpArg == null)
                BrowserHelpArg = savedSettings.BrowserHelpArg;
            if (BrowserSearchUrl == null)
                BrowserSearchUrl = savedSettings.BrowserSearchUrl;
            if (BrowserSearchArg == null)
                BrowserSearchArg = savedSettings.BrowserSearchArg;
            if (BrowserThesaurusUrl == null)
                BrowserThesaurusUrl = savedSettings.BrowserThesaurusUrl;
            if (BrowserThesaurusArg == null)
                BrowserThesaurusArg = savedSettings.BrowserThesaurusArg;
            if (BrowserSpellUrl == null)
                BrowserSpellUrl = savedSettings.BrowserSpellUrl;
            if (BrowserSpellArg == null)
                BrowserSpellArg = savedSettings.BrowserSpellArg;

            if (AudioVol == Program.PosIntegerNotSet)
                AudioVol = savedSettings.AudioVol;

            if (TextEditorRulersShow == BoolValue.Unset)
                TextEditorRulersShow = savedSettings.TextEditorRulersShow;
            if (TextEditorRulersUnitChar == Program.NullChar)
                TextEditorRulersUnitChar = savedSettings.TextEditorRulersUnitChar;
            if (TextEditorCursorSize == Program.PosIntegerNotSet)
                TextEditorCursorSize = savedSettings.TextEditorCursorSize;
            if (TextEditorDisplayRows == Program.PosIntegerNotSet)
                TextEditorDisplayRows = savedSettings.TextEditorDisplayRows;
            if (TextEditorDisplayCols == Program.PosIntegerNotSet)
                TextEditorDisplayCols = savedSettings.TextEditorDisplayCols;
            if (TextEditorParaBreakDisplayChar == Program.NullChar)
                TextEditorParaBreakDisplayChar = savedSettings.TextEditorParaBreakDisplayChar;
            if (TextEditorPauseWaitSecs == Program.PosIntegerNotSet)
                TextEditorPauseWaitSecs = savedSettings.TextEditorPauseWaitSecs;
            if (TextEditorScrollLimit == Program.PosIntegerNotSet)
                TextEditorScrollLimit = savedSettings.TextEditorScrollLimit;
            if (TextEditorEditLimit == Program.PosIntegerNotSet)
                TextEditorEditLimit = savedSettings.TextEditorEditLimit;
            if (TextEditorTabSize == Program.PosIntegerNotSet)
                TextEditorTabSize = savedSettings.TextEditorTabSize;
            if (TextEditorAutoSave == AutoSaveMode.Unknown)
                TextEditorAutoSave = savedSettings.TextEditorAutoSave;
            if (TextEditorAutoCorrect == BoolValue.Unset)
                TextEditorAutoCorrect = savedSettings.TextEditorAutoCorrect;
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

        public ConsoleColor XlatStringToConsoleColour(string colour)
        {
            var rc = UnsetColour;

            if (colour == ArgBlack)
                rc = ConsoleColor.Black;
            else if (colour == ArgBlue)
                rc = ConsoleColor.Blue;
            else if (colour == ArgCyan)
                rc = ConsoleColor.Cyan;
            else if (colour == ArgDarkBlue)
                rc = ConsoleColor.DarkBlue;
            else if (colour == ArgDarkCyan)
                rc = ConsoleColor.DarkCyan;
            else if (colour == ArgDarkGreen)
                rc = ConsoleColor.DarkGreen;
            //   else if (colour == ArgDarkMagenta) //used as unset colour
            //    rc = ConsoleColor.DarkMagenta;
            else if (colour == ArgDarkRed)
                rc = ConsoleColor.DarkRed;
            else if (colour == ArgDarkYellow)
                rc = ConsoleColor.DarkYellow;
            else if (colour == ArgGray)
                rc = ConsoleColor.Gray;
            else if (colour == ArgGreen)
                rc = ConsoleColor.Green;
            else if (colour == ArgMagenta)
                rc = ConsoleColor.Magenta;
            else if (colour == ArgRed)
                rc = ConsoleColor.Red;
            else if (colour == ArgWhite)
                rc = ConsoleColor.White;
            else if (colour == ArgYellow)
                rc = ConsoleColor.Yellow;
            else
                rc = UnsetColour;

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
