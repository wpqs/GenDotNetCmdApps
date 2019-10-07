﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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

    //1. any change to Param consts value needs change to  enum Param

    //2. when adding a new parameter:
    // a) define static readonly for i) name (SettingsParam) ii) args (SettingsArgUpdate) iii) default value(s)(SettingsArgUpdateValueDefault)
    // b) add properties for args (SettingsPathFileName, UpdateSettingsFile) 
    // c) update SetDefaultValues() SetFactoryDefaults() andUpdateProperties()
    // d) add item enum Param (Settings)
    // e) update ParamProc() and create new private method like SettingsParamProc()
    // f) optionally update ValidateParams()
    // g) add GetHelpInfoxxx() for new parameter
    // g) update GetParamHelp(), GetHelpInfoAll()
    // h) update unit tests
    // i) run with invalid values/names and check error report
    // j) check that the settings file is correctly updated
    // k) implement in application
    // l) test with max and min values in range (if appropriate)



    public class CmdLineParamsApp : CmdLineParams
    {
        public static readonly string EditFileType = ".ksx"; //text file in XML format with custom elements
        public static readonly string ColourName = "COLOUR";
        public static readonly string EditFileNameForm = "'drive:path\\*" + EditFileType + "'";
        public static readonly string TextFileNameForm = "'drive:path\\*.txt'";
        public static readonly string AudioFileNameForm = "'drive:path\\*.mp3'";
        public static readonly string SettingsFileNameForm = $"'drive:path\\*.json'";
        public static readonly string ExeFileNameForm = $"'drive:path\\*.exe'";
        public static readonly string UrlForm = $"https://domain.xxx/args?name1=value&name2=value";
        public static readonly string UsernameForm = $"name";
        public static readonly string PasswordKeyForm = $"key";

        public static readonly string DictionaryFileDefault = Program.ValueNotSet;
        public static readonly string DictionaryUrlDefault = Program.ValueNotSet;
        public static readonly string DictionaryVersionDefault = Program.ValueNotSet;

        public const string ArgOn = "on";
        public const string ArgOff = "off";

        public const string ArgYes = "yes";
        public const string ArgNo = "no";

        public static readonly string ArgBlack = MxConsole.Black;
        public static readonly string ArgBlue = MxConsole.Blue;
        public static readonly string ArgCyan = MxConsole.Cyan;
        public static readonly string ArgDarkBlue = MxConsole.DarkBlue;
        public static readonly string ArgDarkCyan = MxConsole.DarkCyan;
        public static readonly string ArgDarkGray = MxConsole.DarkGray;
        public static readonly string ArgDarkGreen = MxConsole.DarkGreen;
        public static readonly string ArgDarkMagenta = MxConsole.DarkMagenta; 
        public static readonly string ArgDarkRed = MxConsole.DarkRed;
        public static readonly string ArgDarkYellow = MxConsole.DarkYellow;
        public static readonly string ArgGray = MxConsole.Gray;
        public static readonly string ArgGreen = MxConsole.Green;
        public static readonly string ArgMagenta = MxConsole.Magenta;
        public static readonly string ArgRed = MxConsole.Red;
        public static readonly string ArgWhite = MxConsole.White;
        public static readonly string ArgYellow = MxConsole.Yellow;

        //main operational parameters
        public const string ParamHelp = "--help";
        public const string ParamEditFile = "--edit";     // filename.ksx 
        public const string ParamExportFile = "--export"; // editfilename.ksx exportfilename.txt
        public const string ParamImportFile = "--import"; // exportfilename.txt editfilename.ksx

        public static readonly string ArgFileFrom = "from";
        public static readonly string ArgFileTo = "to";

        //edit operational parameters - general

        public const string ParamGeneralSettings = "--settings";

            public static readonly string ArgSettingsDisplay = "display";
            public static readonly bool   ArgSettingsDisplayDefault = false;
            public static readonly string ArgSettingsUpdate = "update";
            public static readonly bool   ArgSettingsUpdateDefault = false;
            public static readonly string ArgSettingsPathFileName = "file";
            public static readonly string ArgSettingsPathFileNameDefault = $"{Program.CmdAppName}.json";

        public const string ParamGeneralBackGndColour = "--backgnd";     //  (text COLOUR) (msg-error COLOUR) (msg-warn COLOUR) (msg-note COLOUR) (cmds COLOUR) (status COLOUR) (rule COLOUR)
        public const string ParamGeneralForeGndColour = "--foregnd";     //  (text COLOUR) (msg-error COLOUR) (msg-warn COLOUR) (msg-note COLOUR) (cmds COLOUR) (status COLOUR) (rule COLOUR)

            public static readonly string ArgColourText = "text";
            public static readonly string ArgBackGndColourTextDefault = ArgBlack;
            public static readonly string ArgForeGndColourTextDefault = ArgGreen;

            public static readonly string ArgColourMsgError = "msg-error";
            public static readonly string ArgBackGndColourMsgErrorDefault = ArgBlack;
            public static readonly string ArgForeGndColourMsgErrorDefault = ArgRed;

            public static readonly string ArgColourMsgWarn = "msg-warn";
            public static readonly string ArgBackGndColourMsgWarnDefault = ArgBlack;
            public static readonly string ArgForeGndColourMsgWarnDefault = ArgYellow;

            public static readonly string ArgColourMsgInfo = "msg-info";
            public static readonly string ArgBackGndColourMsgInfoDefault = ArgBlack;
            public static readonly string ArgForeGndColourMsgInfoDefault = ArgWhite;

            public static readonly string ArgColourCmds = "cmds";
            public static readonly string ArgBackGndColourCmdsDefault = ArgBlack;
            public static readonly string ArgForeGndColourCmdsDefault = ArgDarkBlue;

            public static readonly string ArgColourStatus = "status";
            public static readonly string ArgBackGndColourStatusDefault = ArgBlack;
            public static readonly string ArgForeGndColourStatusDefault = ArgGray;

            public static readonly string ArgColourRule = "rule";
            public static readonly string ArgBackGndColourRuleDefault = ArgBlack;
            public static readonly string ArgForeGndColourRuleDefault = ArgGray;

       public const string ParamToolBrowser = "--toolbrowser";

            public static readonly string ArgToolBrowserExe = "exe";
            public static readonly string ArgToolBrowserExeDefault = "explorer.exe";

            public static readonly string ArgToolBrowserUrl = "url";

        public const string ParamToolHelp = "--toolhelp";             // url 'https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual-v1.0/{HelpVer}'

            public static readonly string ArgToolHelpUrlDefault = "https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual/{HelpVer}";

        public const string ParamToolSearch = "--toolsearch";         // url 'https://www.google.com/search?q=%22{word}%22'

            public static readonly string ArgToolSearchUrlDefault = "https://www.google.com/search?q=%22{word}%22";

        public const string ParamToolThesaurus = "--toolthesaurus";   // url https://www.thesaurus.com/browse/{word}

            public static readonly string ArgToolThesaurusUrlDefault = "https://www.thesaurus.com/browse/{word}";

        public const string ParamToolSpell = "--toolspell";           // url http://www.spellcheck.net/{word}

            public static readonly string ArgToolSpellUrlDefault = "http://www.spellcheck.net/{word}";

        public const string ParamToolSvn = "--toolsvn";                   // username 'wills' password=[secret manager key] url 'https//me.svnrepository.com/books'

            public static readonly string ArgToolSvnUser = "username";
            public static readonly string ArgToolSvnUserDefault = Program.ValueNotSet;
            public static readonly string ArgToolSvnPassword = "password";
            public static readonly string ArgToolSvnPasswordDefault = Program.ValueNotSet;
            public static readonly string ArgToolSvnUrl = "url";
            public static readonly string ArgToolSvnUrlDefault = Program.ValueNotSet;

        public const string ParamGeneralAudio = "--audio";         
        
            public static readonly string ArgAudioVol = "vol";      //  0  <min 1 max 10> //(0 is off)
            public static readonly int ArgAudioVolDefault = 0;
            public static readonly int ArgAudioVolMax = 10;
            public static readonly int ArgAudioVolMin = 0;

                public static readonly string ArgAudioFileKeyPressDefault = "keypress.mp3";
                public static readonly string ArgAudioFileBackSpaceDefault = "bs.mp3";
                public static readonly string ArgAudioFileCrDefault = "cr.mp3";
                public static readonly string ArgAudioFileStartupDefault = "paperinsert.mp3";
                public static readonly string ArgAudioFileEndDefault = "paperremove.mp3";

                //edit operational parameters - text editor

        public const string ParamTextEditorRulers = "--rulers";             //  show [yes | no] unitchar '.'

            public static readonly string ArgTextEditorRulersShow = "show";
            public static readonly string ArgTextEditorRulersShowDefault = ArgYes;
            public static readonly string ArgTextEditorRulersUnitChar = "unitchar";
            public static readonly char ArgTextEditorRulersUnitCharDefault = '.';
            public static readonly string ArgTextEditorRulersBotChar = "botchar";
            public static readonly char ArgTextEditorRulersBotCharDefault = '_';

        public const string ParamTextEditorCursor = "--cursor";     

            public static readonly string ArgTextEditorCursorSize = "size";     // 20 <min 1 max 100>
            public static readonly int ArgTextEditorCursorSizeDefault = 20;
            public static readonly int ArgTextEditorCursorSizeMax = 100;
            public static readonly int ArgTextEditorCursorSizeMin = 1;

        public const string ParamTextEditorDisplay = "--display";   

            public static readonly string ArgTextEditorDisplayRows = "rows";      // 10 <min 1 max 25>	    //was displaylastlines, now TextEditorDisplayRows
            public static readonly int ArgTextEditorDisplayRowsDefault = 10;      //was ArgEditAreaLinesCountDefault
            public static readonly int ArgTextEditorDisplayRowsMax = 50;          //was ArgEditAreaLinesCountMax 
            public static readonly int ArgTextEditorDisplayRowsMin = 5;           //was ArgEditAreaLinesCountMin

            public static readonly string ArgTextEditorDisplayCols = "cols";      //68 <min 5 max 250> 
            public static readonly int ArgTextEditorDisplayColsDefault = 68;      //was ArgEditAreaLineWidthDefault - counted from Jack Kerouac's book 'On the Road'
            public static readonly int ArgTextEditorDisplayColsMax = 250;         //was ArgEditAreaLineWidthMax - see EditFile.Create() default StreamBuffer size is 1024, Console.Stream is 256 - length CRLF = 254
            public static readonly int ArgTextEditorDisplayColsMin = 5;           //was ArgEditAreaLineWidthMin

            public static readonly string ArgTextEditorDisplayParaBreakDisplayChar = "parabreak";
            public const char             ArgTextEditorDisplayParaBreakDisplayCharDefault = '>';

        public const string ParamTextEditorPauseTimeout = "--typingpause"; // 60 <min 5 max 36000>

            public static readonly int ArgTextEditorPauseTimeoutDefault = 60;
            public static readonly int ArgTextEditorPauseTimeoutMin = 0;
            public static readonly int ArgTextEditorPauseTimeoutMax = 86400;     //24 * 60 * 60 - 24 hours

        public const string ParamTextEditorLimits = "--limits";     // 0  <min 1 max 10000>		//(0 is unlimited) - was scrollreview, ParamScrollReviewMode

            public static readonly string ArgTextEditorLimitScroll = "scrollrows";
            public static readonly int ArgTextEditorLimitScrollDefault = 0;
            public static readonly int ArgTextEditorLimitScrollMax = 10000;
            public static readonly int ArgTextEditorLimitScrollMin = 0;            //0 is unlimited

            public static readonly string ArgTextEditorLimitEdit = "editrows";
            public static readonly int ArgTextEditorLimitEditDefault = 0;
            public static readonly int ArgTextEditorLimitEditMax = 10000;
            public static readonly int ArgTextEditorLimitEditMin = 0;              //0 is unlimited

        public const string ParamTextEditorTabSize = "--tabsize";           // 3  <min 1 max 25>	

            public const int           ArgTextEditorTabSizeDefault = 3;
            public static readonly int ArgTextEditorTabSizeMax = 25;
            public static readonly int ArgTextEditorTabSizeMin = 1;

        public const string ParamTextEditorAutoSave = "--autosave";         // [CR | ParaBreak | off]

            public const string ArgTextEditorAutoSaveCR = "CR";
            public const string ArgTextEditorAutoSaveParaBreak = "ParaBreak";
            public const string ArgTextEditorAutoSaveOff = ArgOff;
            public static readonly AutoSaveMode ArgTextEditorAutoSaveDefault = AutoSaveMode.Off;

        public const string ParamTextEditorAutoCorrect = "--autocorrect";           // [on | off]

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
        public string AudioFileBackSpace { set; get; }
        public string AudioFileCr { set; get; }
        public string AudioFileStartup { set; get; }
        public string AudioFileEnd { set; get; }

        public BoolValue ReportMxErrors { set; get; }

        public string DictionaryFile { set; get; }
        public string DictionaryUrl { set; get; }
        public string DictionaryVersion { set; get; }

        //edit operational parameters - general

        public BoolValue SettingsDisplay { set; get; }

        public MxConsole.Color BackGndColourText {  set; get; }
        public MxConsole.Color ForeGndColourText {  set; get; }
        public MxConsole.Color BackGndColourMsgError {  set; get; }
        public MxConsole.Color ForeGndColourMsgError {  set; get; }
        public MxConsole.Color BackGndColourMsgWarn {  set; get; }
        public MxConsole.Color ForeGndColourMsgWarn { set; get; }
        public MxConsole.Color BackGndColourMsgInfo {  set; get; }
        public MxConsole.Color ForeGndColourMsgInfo { set; get; }
        public MxConsole.Color BackGndColourCmds { set; get; }
        public MxConsole.Color ForeGndColourCmds { set; get; }
        public MxConsole.Color BackGndColourStatus { set; get; }
        public MxConsole.Color ForeGndColourStatus { set; get; }
        public MxConsole.Color BackGndColourRule { set; get; }
        public MxConsole.Color ForeGndColourRule { set; get; }

        public int AudioVol { set; get; }

        //Tools options page

        public string ToolBrowserExe { set; get; }

        public string ToolHelpUrl { set; get; }
        public string ToolSearchUrl { set; get; }
        public string ToolThesaurusUrl { set; get; }
        public string ToolSpellUrl { set; get; }
        public string ToolSvnUser { set; get; }
        public string ToolSvnPasswordKey { set; get; }
        public string ToolSvnUrl { set; get; }

        //edit operational parameters - text editor

        public BoolValue TextEditorRulersShow { set; get; }
        public char TextEditorRulersUnitChar { set; get; }
        public char TextEditorRulersBotChar { set; get; }
        public int TextEditorCursorSize { set; get; }
        public int TextEditorDisplayRows {  set; get; }
        public int TextEditorDisplayCols {  set; get; }
        public char TextEditorParaBreakDisplayChar {  set; get; }
        public int TextEditorPauseTimeout {set; get; }
        public int TextEditorScrollLimit {  set; get; }
        public int TextEditorEditLimit { set; get; }
        public int TextEditorTabSize { set; get; }
        public AutoSaveMode TextEditorAutoSave { set; get; }
        public BoolValue TextEditorAutoCorrect { set; get; }

        public bool IsValidForSettingBoolValue(string val){ return ((val == ArgNo) || (val == ArgYes)) ? true : false;  }

        public enum BoolValue
        {
            [EnumMember(Value = Program.ValueNotSet)] Unset = Program.PosIntegerNotSet,
            [EnumMember(Value = ArgNo)] No = 0,
            [EnumMember(Value = ArgYes)] Yes = 1,

        }

        public enum OpMode
        {
            [EnumMember(Value = "Help")] Help = 0,
            [EnumMember(Value = "Import")]Import,
            [EnumMember(Value = "Export")] Export,
            [EnumMember(Value = "Edit")] Edit,
            [EnumMember(Value = "Abort")] Abort,
            [EnumMember(Value = Program.ValueUnknown)] Unknown
        }

        public enum Param
        {
            [EnumMember(Value = ParamHelp)] Help = 0,
            [EnumMember(Value = ParamEditFile)] EditFile,
            [EnumMember(Value = ParamExportFile)] ExportFile,
            [EnumMember(Value = ParamImportFile)] ImportFile,
            [EnumMember(Value = ParamGeneralSettings)] Settings,
            [EnumMember(Value = ParamGeneralBackGndColour)] BackGnd,
            [EnumMember(Value = ParamGeneralForeGndColour)] ForeGnd,
            [EnumMember(Value = ParamToolBrowser)] ToolBrowser,
            [EnumMember(Value = ParamToolHelp)] ToolHelp,
            [EnumMember(Value = ParamToolSearch)] ToolSearch,
            [EnumMember(Value = ParamToolThesaurus)] ToolThesaurus,
            [EnumMember(Value = ParamToolSpell)] ToolSpell,
            [EnumMember(Value = ParamToolSvn)] ToolSvn,
            [EnumMember(Value =ParamGeneralAudio)] Audio,
            [EnumMember(Value = ParamTextEditorRulers)] Rulers,
            [EnumMember(Value = ParamTextEditorCursor)] Cursor,
            [EnumMember(Value = ParamTextEditorDisplay)] Display,
            [EnumMember(Value = ParamTextEditorLimits)] Limits,
            [EnumMember(Value = ParamTextEditorTabSize)] TabSize,
            [EnumMember(Value = ParamTextEditorPauseTimeout)] TypingPause,
            [EnumMember(Value = ParamTextEditorAutoSave)] AutoSave,
            [EnumMember(Value = ParamTextEditorAutoCorrect)] AutoCorrect,
            [EnumMember(Value = Program.ValueUnknown)] Unknown
        }

        public enum AutoSaveMode
        {
            [EnumMember(Value = ArgOff)] Off = 0,
            [EnumMember(Value = ArgTextEditorAutoSaveCR)] CR,
            [EnumMember(Value = ArgTextEditorAutoSaveParaBreak)] ParaBreak,
            [EnumMember(Value = Program.ValueUnknown)] Unknown
        }

        public override string ToString()
        {
            var rc = Environment.NewLine;

            rc += "Program settings:" + Environment.NewLine;
            rc += Environment.NewLine;
            rc += "SettingsDisplay=" + ((SettingsDisplay == BoolValue.Yes) ? "yes" : "no") + Environment.NewLine;
            rc += "SettingsUpdate=" + (SettingsUpdate == BoolValue.Unset ? "[not set]" : ((SettingsUpdate == BoolValue.Yes) ? "yes" : "no")) + Environment.NewLine;
            rc += "SettingsFile=" + (SettingsPathFileName ?? "[null]") + Environment.NewLine;
            rc += "ReportMxErrors=" + EnumOps.XlatToString(ReportMxErrors) + Environment.NewLine;
            rc += Environment.NewLine;
            rc +=  "Op=" + EnumOps.XlatToString(Op) + Environment.NewLine;
            rc += Environment.NewLine;

            if (Op == OpMode.Import)
            {
                rc += "ImportInputFile=" + (ImportInputFile ?? "[null]") + Environment.NewLine;
                rc += "EditFile=" + (EditFile ?? "[null]") + Environment.NewLine;
            }
            else if (Op == OpMode.Export)
            {
                rc += "EditFile=" + (EditFile ?? "[null]") + Environment.NewLine;
                rc += "ExportOutputFile=" + (ExportOutputFile ?? "[null]") + Environment.NewLine;
            }
            else if ((Op == OpMode.Edit) || (Op == OpMode.Help))
            {
                if (Op == OpMode.Edit)
                {
                    rc += "EditFile=" + (EditFile ?? "[null]") + Environment.NewLine;
                    rc += Environment.NewLine;
                }
                rc += "BackGndColourText=" + MxConsole.XlatMxConsoleColorToString(BackGndColourText) + $" ({(int)BackGndColourText})" + Environment.NewLine;
                rc += "ForeGndColourText=" + MxConsole.XlatMxConsoleColorToString(ForeGndColourText) + $" ({(int)ForeGndColourText})" + Environment.NewLine;
                rc += "BackGndColourMsgError=" + MxConsole.XlatMxConsoleColorToString(BackGndColourMsgError) + $" ({(int)BackGndColourMsgError})" + Environment.NewLine;
                rc += "ForeGndColourMsgError=" + MxConsole.XlatMxConsoleColorToString(ForeGndColourMsgError) + $" ({(int)ForeGndColourMsgError})" + Environment.NewLine;
                rc += "BackGndColourMsgWarn=" + MxConsole.XlatMxConsoleColorToString(BackGndColourMsgWarn) + $" ({(int)BackGndColourMsgWarn})" + Environment.NewLine;
                rc += "ForeGndColourMsgWarn=" + MxConsole.XlatMxConsoleColorToString(ForeGndColourMsgWarn) + $" ({(int)ForeGndColourMsgWarn})" + Environment.NewLine;
                rc += "BackGndColourMsgInfo=" + MxConsole.XlatMxConsoleColorToString(BackGndColourMsgInfo) + $" ({(int)BackGndColourMsgInfo})" + Environment.NewLine;
                rc += "ForeGndColourMsgInfo=" + MxConsole.XlatMxConsoleColorToString(ForeGndColourMsgInfo) + $" ({(int)ForeGndColourMsgInfo})" + Environment.NewLine;
                rc += "BackGndColourCmds=" + MxConsole.XlatMxConsoleColorToString(BackGndColourCmds) + $" ({(int)BackGndColourCmds})" + Environment.NewLine;
                rc += "ForeGndColourCmds=" + MxConsole.XlatMxConsoleColorToString(ForeGndColourCmds) + $" ({(int)ForeGndColourCmds})" + Environment.NewLine;
                rc += "BackGndColourStatus=" + MxConsole.XlatMxConsoleColorToString(BackGndColourStatus) + $" ({(int)BackGndColourStatus})" + Environment.NewLine;
                rc += "ForeGndColourStatus=" + MxConsole.XlatMxConsoleColorToString(ForeGndColourStatus) + $" ({(int)ForeGndColourStatus})" + Environment.NewLine;
                rc += "BackGndColourRule=" + MxConsole.XlatMxConsoleColorToString(BackGndColourRule) + $" ({(int)BackGndColourRule})" + Environment.NewLine;
                rc += "ForeGndColourRule=" + MxConsole.XlatMxConsoleColorToString(ForeGndColourRule) + $" ({(int)ForeGndColourRule})" + Environment.NewLine;
                rc += Environment.NewLine;
                rc += "ToolBrowserExe=" + (ToolBrowserExe ?? "[null]") + Environment.NewLine;
                rc += "ToolHelpUrl=" + (ToolHelpUrl ?? "[null]") + Environment.NewLine;
                rc += "ToolSearchUrl=" + (ToolSearchUrl ?? "[null]") + Environment.NewLine;
                rc += "ToolThesaurusUrl=" + (ToolThesaurusUrl ?? "[null]") + Environment.NewLine;
                rc += "ToolSpellUrl=" + (ToolSpellUrl ?? "[null]") + Environment.NewLine;
                rc += "ToolSvnUser=" + (ToolSvnUser ?? "[null]" ) + " ToolSvnPasswordKey=" + (ToolSvnPasswordKey ?? "[null]") + " ToolSvnUrl=" + (ToolSvnUrl ?? "[null]") + Environment.NewLine;
                rc += "AudioVol=" + AudioVol + Environment.NewLine;
                rc += "TextEditorRulersShow=" + EnumOps.XlatToString(ReportMxErrors) + Environment.NewLine;
                rc += "TextEditorRulersUnitChar=" + TextEditorRulersUnitChar.ToString() + Environment.NewLine;
                rc += "TextEditorRulersBotChar=" + TextEditorRulersBotChar.ToString() + Environment.NewLine;
                rc += "TextEditorCursorSize=" + TextEditorCursorSize + Environment.NewLine;
                rc += "TextEditorDisplayRows=" + TextEditorDisplayRows + Environment.NewLine;
                rc += "TextEditorDisplayCols=" + TextEditorDisplayCols + Environment.NewLine;
                rc += "TextEditorParaBreakDisplayChar=" + TextEditorParaBreakDisplayChar.ToString() + Environment.NewLine;
                rc += "TextEditorPauseTimeout=" + TextEditorPauseTimeout + Environment.NewLine;
                rc += "TextEditorScrollLimit=" + TextEditorScrollLimit + Environment.NewLine;
                rc += "TextEditorEditLimit=" + TextEditorEditLimit + Environment.NewLine;
                rc += "TextEditorTabSize=" + TextEditorTabSize + Environment.NewLine;
                rc += "TextEditorAutoSave=" + EnumOps.XlatToString(TextEditorAutoSave) + Environment.NewLine;
                rc += "TextEditorAutoCorrect=" + EnumOps.XlatToString(TextEditorAutoCorrect) + Environment.NewLine;
                rc += Environment.NewLine;
                rc += "AudioFileKeyPress=" + (AudioFileKeyPress ?? "[null]") + Environment.NewLine;
                rc += "AudioFileeBackSpace=" + (AudioFileBackSpace ?? "[null]") + Environment.NewLine;
                rc += "AudioFileCr=" + (AudioFileCr ?? "[null]") + Environment.NewLine;
                rc += "AudioFileStartup=" + (AudioFileStartup ?? "[null]") + Environment.NewLine;
                rc += "AudioFileEnd=" + (AudioFileEnd ?? "[null]") + Environment.NewLine;

            }
            //else if ((Op == OpMode.UpdateDictionary) || (Op == OpMode.GetDictionarySettings))
            //{       //--updatedict 
            //    rc += "DictionaryFile=" + (DictionaryFile ?? "[null]") + Environment.NewLine;
            //    rc += "DictionaryUrl=" + (DictionaryUrl ?? "[null]") + Environment.NewLine;
            //    rc += "DictionaryVersion=" + (DictionaryVersion ?? "[null]") + Environment.NewLine;
            //}
            else
            {
                rc += "invalid operation parameter" + Environment.NewLine;
            }
            return rc;
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
            AudioFileBackSpace = null;
            AudioFileCr = null;
            AudioFileStartup = null;
            AudioFileEnd = null;
            ReportMxErrors = BoolValue.Unset;
            DictionaryFile = null;
            DictionaryUrl = null;
            DictionaryVersion = null;
            SettingsDisplay = BoolValue.Unset;

            BackGndColourText = MxConsole.Color.NotSet;
            ForeGndColourText = MxConsole.Color.NotSet;
            BackGndColourMsgError = MxConsole.Color.NotSet;
            ForeGndColourMsgError = MxConsole.Color.NotSet;
            BackGndColourMsgWarn = MxConsole.Color.NotSet;
            ForeGndColourMsgWarn = MxConsole.Color.NotSet;
            BackGndColourMsgInfo = MxConsole.Color.NotSet;
            ForeGndColourMsgInfo = MxConsole.Color.NotSet;
            BackGndColourCmds = MxConsole.Color.NotSet;
            ForeGndColourCmds = MxConsole.Color.NotSet;
            BackGndColourStatus = MxConsole.Color.NotSet;
            ForeGndColourStatus = MxConsole.Color.NotSet;
            BackGndColourRule = MxConsole.Color.NotSet;
            ForeGndColourRule = MxConsole.Color.NotSet;

            AudioVol = Program.PosIntegerNotSet;

            ToolBrowserExe = null;
            ToolHelpUrl = null;
            ToolSearchUrl = null;
            ToolThesaurusUrl = null;
            ToolSpellUrl = null;
            ToolSvnUser = null;
            ToolSvnPasswordKey = null;
            ToolSvnUrl = null;

            TextEditorRulersShow = BoolValue.Unset;
            TextEditorRulersUnitChar = Program.NullChar;
            TextEditorRulersBotChar = Program.NullChar;
            TextEditorCursorSize = Program.PosIntegerNotSet;
            TextEditorDisplayRows = Program.PosIntegerNotSet;
            TextEditorDisplayCols = Program.PosIntegerNotSet;
            TextEditorParaBreakDisplayChar = Program.NullChar;
            TextEditorPauseTimeout = Program.PosIntegerNotSet;
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
            AudioFileBackSpace = ArgAudioFileBackSpaceDefault;
            AudioFileCr = ArgAudioFileCrDefault;
            AudioFileStartup = ArgAudioFileStartupDefault;
            AudioFileEnd = ArgAudioFileEndDefault;
            ReportMxErrors = BoolValue.Yes;
            DictionaryFile = DictionaryFileDefault;
            DictionaryUrl = DictionaryUrlDefault;
            DictionaryVersion = DictionaryVersionDefault;
            SettingsDisplay = BoolValue.No;

            BackGndColourText = MxConsole.XlatStringToMxConsoleColor(ArgBackGndColourTextDefault);
            ForeGndColourText = MxConsole.XlatStringToMxConsoleColor(ArgForeGndColourTextDefault);
            BackGndColourMsgError = MxConsole.XlatStringToMxConsoleColor(ArgBackGndColourMsgErrorDefault);
            ForeGndColourMsgError = MxConsole.XlatStringToMxConsoleColor(ArgForeGndColourMsgErrorDefault);
            BackGndColourMsgWarn = MxConsole.XlatStringToMxConsoleColor(ArgBackGndColourMsgWarnDefault);
            ForeGndColourMsgWarn = MxConsole.XlatStringToMxConsoleColor(ArgForeGndColourMsgWarnDefault);
            BackGndColourMsgInfo = MxConsole.XlatStringToMxConsoleColor(ArgBackGndColourMsgInfoDefault);
            ForeGndColourMsgInfo = MxConsole.XlatStringToMxConsoleColor(ArgForeGndColourMsgInfoDefault);
            BackGndColourCmds = MxConsole.XlatStringToMxConsoleColor(ArgBackGndColourCmdsDefault);
            ForeGndColourCmds = MxConsole.XlatStringToMxConsoleColor(ArgForeGndColourCmdsDefault);
            BackGndColourStatus = MxConsole.XlatStringToMxConsoleColor(ArgBackGndColourStatusDefault);
            ForeGndColourStatus = MxConsole.XlatStringToMxConsoleColor(ArgForeGndColourStatusDefault);
            BackGndColourRule = MxConsole.XlatStringToMxConsoleColor(ArgBackGndColourRuleDefault);
            ForeGndColourRule = MxConsole.XlatStringToMxConsoleColor(ArgForeGndColourRuleDefault);
            AudioVol = ArgAudioVolDefault;

            ToolBrowserExe = ArgToolBrowserExeDefault;
            ToolHelpUrl = ArgToolHelpUrlDefault;
            ToolSearchUrl = ArgToolSearchUrlDefault;
            ToolThesaurusUrl = ArgToolThesaurusUrlDefault;
            ToolSpellUrl = ArgToolSpellUrlDefault;
            ToolSvnUser = Program.ValueNotSet;
            ToolSvnPasswordKey = Program.ValueNotSet;
            ToolSvnUrl = Program.ValueNotSet;

            TextEditorRulersShow = BoolValue.Yes;
            TextEditorRulersUnitChar = ArgTextEditorRulersUnitCharDefault;
            TextEditorRulersBotChar = ArgTextEditorRulersBotCharDefault;
            TextEditorCursorSize = ArgTextEditorCursorSizeDefault;
            TextEditorDisplayRows = ArgTextEditorDisplayRowsDefault;
            TextEditorDisplayCols = ArgTextEditorDisplayColsDefault;
            TextEditorParaBreakDisplayChar = ArgTextEditorDisplayParaBreakDisplayCharDefault;
            TextEditorPauseTimeout = ArgTextEditorPauseTimeoutDefault;
            TextEditorScrollLimit = ArgTextEditorLimitScrollDefault;
            TextEditorEditLimit = ArgTextEditorLimitEditDefault;
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
                    HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\"{Environment.NewLine}" + GetHelpInfoAll();
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
                            break;
                        }
                        case Param.ExportFile:
                        {
                            var rcReset = ProcessExportParam(paramLine);
                            rc += rcReset;
                            if (rcReset.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.ImportFile:
                        {
                            var rcReset = ProcessImportParam(paramLine);
                            rc += rcReset;
                            if (rcReset.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.EditFile:
                        {
                            var rcReset = ProcessEditParam(paramLine);
                            rc += rcReset;
                            if (rcReset.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.Settings:
                        {
                            var rcSettings = ProcessSettingsParam(paramLine); 
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.BackGnd:
                        {
                            var rcSettings = ProcessGeneralBackGndParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.ForeGnd:
                        {
                            var rcSettings = ProcessGeneralForeGndParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.Audio:
                        {
                            var rcSettings = ProcessGeneralAudioParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.ToolBrowser:
                        {
                            var rcSettings = ProcessToolBrowserParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.ToolHelp:
                        {
                            var rcSettings = ProcessToolHelpParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.ToolSearch:
                        {
                            var rcSettings = ProcessToolSearchParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.ToolThesaurus:
                        {
                            var rcSettings = ProcessToolThesaurusParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.ToolSpell:
                        {
                            var rcSettings = ProcessToolSpellParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.ToolSvn:
                        {
                            var rcSettings = ProcessToolSvnParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.Rulers:
                        {
                            var rcSettings = ProcessTextEditorRulersParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.Cursor:
                        {
                            var rcSettings = ProcessTextEditorCursorParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.Display:
                        {
                            var rcSettings = ProcessTextEditorDisplayParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.Limits:
                        {
                            var rcSettings = ProcessTextEditorLimitsParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.TabSize:
                        {
                            var rcSettings = ProcessTextEditorTabSizeParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.TypingPause:
                        {
                            var rcSettings = ProcessTextEditorPauseTimeoutParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.AutoSave:
                        {
                            var rcSettings = ProcessTextEditorAutoSaveParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
                        case Param.AutoCorrect:
                        {
                            var rcSettings = ProcessTextEditorAutoCorrectParam(paramLine);
                            rc += rcSettings;
                            if (rcSettings.IsSuccess())
                                rc.SetResult(true);
                            break;
                        }
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

            if (Op == OpMode.Export)
            {
                if (string.IsNullOrEmpty(EditFile))
                   rc.SetError(1020301, MxError.Source.User, $"Paramter {ParamExportFile} has missing first argument; {EditFileNameForm}");
                else
                {
                    if (string.IsNullOrEmpty(ExportOutputFile))
                        rc.SetError(1020302, MxError.Source.User, $"Paramter {ParamExportFile} has missing second argument; {TextFileNameForm}");
                    else
                    {
                        rc.SetResult(true);
                    }
                }
                if (rc.IsError())
                    HelpHint = $"{GetParamHelp((int)Param.ExportFile)}";
            }
            else if (Op == OpMode.Import)
            {
                if (string.IsNullOrEmpty(EditFile))
                    rc.SetError(1020303, MxError.Source.User, $"Paramter {ParamImportFile} has missing first argument; {EditFileNameForm}");
                else
                {
                    if (string.IsNullOrEmpty(ImportInputFile))
                        rc.SetError(1020304, MxError.Source.User, $"Paramter {ParamImportFile} has missing second argument; {TextFileNameForm}");
                    else
                    {
                        rc.SetResult(true);
                    }
                }
                if (rc.IsError())
                    HelpHint = $"{GetParamHelp((int)Param.ImportFile)}";
            }
            else if ((Op == OpMode.Edit) || (Op == OpMode.Help))
            {
                if (((Op == OpMode.Edit)) && (string.IsNullOrEmpty(EditFile)))
                {
                    HelpHint = $"{GetParamHelp((int) Param.EditFile)}";
                    rc.SetError(1020305, MxError.Source.User, $"Parameter {ParamEditFile} has missing first argument; {EditFileNameForm}");
                }

                var argBad = Program.ValueNotSet;
                if (MxConsole.XlatMxConsoleColorToString(BackGndColourText) == Program.ValueUnknown)
                    argBad = ArgColourText;
                if (MxConsole.XlatMxConsoleColorToString(BackGndColourMsgError) == Program.ValueUnknown)
                    argBad = ArgColourMsgError;
                if (MxConsole.XlatMxConsoleColorToString(BackGndColourMsgWarn) == Program.ValueUnknown)
                    argBad = ArgColourMsgWarn;
                if (MxConsole.XlatMxConsoleColorToString(BackGndColourMsgInfo) == Program.ValueUnknown)
                    argBad = ArgColourMsgInfo;
                if (MxConsole.XlatMxConsoleColorToString(BackGndColourCmds) == Program.ValueUnknown)
                    argBad = ArgColourCmds;
                if (MxConsole.XlatMxConsoleColorToString(BackGndColourStatus) == Program.ValueUnknown)
                    argBad = ArgColourStatus;
                if (MxConsole.XlatMxConsoleColorToString(BackGndColourRule) == Program.ValueUnknown)
                    argBad = ArgColourStatus;
                if (argBad != Program.ValueNotSet)
                {
                    HelpHint = $"{GetParamHelp((int)Param.BackGnd)}";
                    rc.SetError(1020306, MxError.Source.User, $"Parameter {ParamGeneralBackGndColour} has a bad argument; value of '{argBad}' is not a valid COLOR");
                }
                argBad = Program.ValueNotSet;
                if (MxConsole.XlatMxConsoleColorToString(ForeGndColourText) == Program.ValueUnknown)
                    argBad = ArgColourText;
                if (MxConsole.XlatMxConsoleColorToString(ForeGndColourMsgError) == Program.ValueUnknown)
                    argBad = ArgColourMsgError;
                if (MxConsole.XlatMxConsoleColorToString(ForeGndColourMsgWarn) == Program.ValueUnknown)
                    argBad = ArgColourMsgWarn;
                if (MxConsole.XlatMxConsoleColorToString(ForeGndColourMsgInfo) == Program.ValueUnknown)
                    argBad = ArgColourMsgInfo;
                if (MxConsole.XlatMxConsoleColorToString(ForeGndColourCmds) == Program.ValueUnknown)
                    argBad = ArgColourCmds;
                if (MxConsole.XlatMxConsoleColorToString(ForeGndColourStatus) == Program.ValueUnknown)
                    argBad = ArgColourStatus;
                if (MxConsole.XlatMxConsoleColorToString(ForeGndColourRule) == Program.ValueUnknown)
                    argBad = ArgColourStatus;
                if (argBad != Program.ValueNotSet)
                {
                    HelpHint = $"{GetParamHelp((int)Param.ForeGnd)}";
                    rc.SetError(1020307, MxError.Source.User, $"Parameter {ParamGeneralForeGndColour} has a bad argument; value of '{argBad}' is not a valid COLOR");
                }

                if ((AudioVol < ArgAudioVolMin) || (AudioVol > ArgAudioVolMax))
                {
                    HelpHint = $"{GetParamHelp((int)Param.Audio)}";
                    rc.SetError(1020308, MxError.Source.User, $"{ArgAudioVol}: {AudioVol} is invalid");
                }

                if ((TextEditorRulersShow == BoolValue.Unset) || (TextEditorRulersUnitChar == Program.NullChar) || ((TextEditorRulersBotChar == Program.NullChar)))
                {
                    HelpHint = $"{GetParamHelp((int)Param.Rulers)}";
                    if (TextEditorRulersShow == BoolValue.Unset)
                        rc.SetError(1020309, MxError.Source.User, $"{ArgTextEditorRulersShow}: is not set");
                    else
                        rc.SetError(1020310, MxError.Source.User, $"{((TextEditorRulersUnitChar == Program.NullChar) ? ArgTextEditorRulersUnitChar : ArgTextEditorRulersBotChar)}: is not set");
                }
                if (rc.IsSuccess())  
                    rc.SetResult(true);
            }
            else
            {
                HelpHint = $"{GetParamHelp((int)Param.Help)}";
                rc.SetError(1020307, MxError.Source.User, $"{ParamHelp} is missing. It must be provided in conjunction with the other parameters you have given.");
            }
            return rc;
        }

        protected override MxReturnCode<bool> SettingsFileProc(bool readMode)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.SettingsFileProc");

            try
            {
                if (File.Exists(SettingsPathFileName) == false)
                    rc += CreateSettingsFile(SettingsPathFileName);  

                if (rc.IsError() == false)
                {
                    var errors = new List<string>();
                    var jSettings = new JsonSerializerSettings
                    {
                        Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                        {
                            errors.Add(args.ErrorContext.Error.Message);
                            args.ErrorContext.Handled = true;
                        },
                        DefaultValueHandling = DefaultValueHandling.Include,
                        NullValueHandling = NullValueHandling.Include,
                        MissingMemberHandling = MissingMemberHandling.Error,
                    };

                    var savedValues = File.ReadAllText(SettingsPathFileName);
                    var savedSettings = JsonConvert.DeserializeObject<CmdLineParamsApp>(savedValues, jSettings);
                    if (errors.Count > 0)
                        rc.SetError(1020401, MxError.Source.User, $"errors={errors.Count}; first={errors[0]}", MxMsgs.MxErrInvalidSettingsFile);
                    else
                    {
                        UpdateProperties(savedSettings);
                        if ((SettingsUpdate == BoolValue.No) || (SettingsUpdate == BoolValue.Unset))
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
            }
            catch (Exception e)
            {
                rc.SetError(1020403, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
            }
            if (rc.IsError(true))
                HelpHint = $"{Environment.NewLine}no further information.{Environment.NewLine}";
            return rc;
        }

        private MxReturnCode<bool> CreateSettingsFile(string settingsPathFileName)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.CreateSettingsFile");

            if (string.IsNullOrEmpty(settingsPathFileName))
                rc.SetError(1020501, MxError.Source.Param, $"settingsPathFileName is null or empty", MxMsgs.MxErrBadMethodParam);
            else
            {
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

                    var factoryDefaults = new CmdLineParamsApp();
                    factoryDefaults.SetFactoryDefaults();
                    var newValues = JsonConvert.SerializeObject(factoryDefaults, jSettings);
                    if (errors.Count > 0)
                        rc.SetError(1020502, MxError.Source.User, $"errors={errors.Count}; first={errors[0]}", MxMsgs.MxErrInvalidSettingsFile);
                    else
                    {
                        File.WriteAllText(SettingsPathFileName, newValues);
                        rc.SetResult(true);
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1020503, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                }
            }
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
                    rc.SetError(1021501, MxError.Source.User, $"parameter {ParamHelp} has incorrect number of arguments; found {rcCnt.GetResult()} should be none");
                }
                else
                {
                    Op = OpMode.Help;
                    HelpHint = $"Help request:{Environment.NewLine}{GetHelpInfoAll()}";
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
                    rc.SetError(1021601, MxError.Source.User, $"parameter {ParamExportFile} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgNameValue(ParamExportFile, ArgFileFrom, paramLine, true); 
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        EditFile = rcArg1.GetResult();
                        var rcArg2 = GetArgNameValue(ParamExportFile, ArgFileTo, paramLine, true);
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

        private MxReturnCode<bool> ProcessImportParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessImportParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamImportFile);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 2)
                    rc.SetError(1021701, MxError.Source.User, $"parameter {ParamImportFile} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgNameValue(ParamImportFile, ArgFileFrom, paramLine, true);
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        ImportInputFile = rcArg1.GetResult();
                        var rcArg2 = GetArgNameValue(ParamImportFile, ArgFileTo, paramLine, true);
                        rc += rcArg2;
                        if (rcArg2.IsSuccess(true))
                        {
                            EditFile = rcArg2.GetResult();
                            Op = OpMode.Import;
                            rc.SetResult(true);
                        }
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.ImportFile)}";
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
                    rc.SetError(1021801, MxError.Source.User, $"parameter {ParamEditFile} has incorrect number of arguments; found {argCnt} should be two");
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

            var rcCnt = GetArgCount(paramLine, ParamGeneralSettings);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                int argCnt = rcCnt.GetResult();
                if ((argCnt<  0) || (argCnt > 5 ))
                    rc.SetError(1021901, MxError.Source.User, $"parameter {ParamGeneralSettings} has incorrect number of arguments; found {argCnt}");
                else
                {
                    if (argCnt == 0)
                        rc.SetResult(true); //do nothing - no args found
                    else
                    {
                        var argProc = 0;
                        var rcArg = GetArgNameValue(ParamGeneralSettings, ArgSettingsDisplay, paramLine, false);
                        rc += rcArg;
                        if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                        {
                            if (IsValidForSettingBoolValue(rcArg.GetResult()) == false)
                                rc.SetError(1021902, MxError.Source.User, $"parameter {ParamGeneralSettings} argument {ArgSettingsDisplay} value is not '{ArgYes}' or '{ArgNo}'; {rcArg.GetResult()}");
                            else
                            {
                                SettingsDisplay = (rcArg.GetResult() == "yes") ? BoolValue.Yes : BoolValue.No;
                                argProc++;
                            }
                        }
                        rcArg = GetArgNameValue(ParamGeneralSettings, ArgSettingsPathFileName, paramLine, false);
                        rc += rcArg;
                        if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                        {
                            SettingsPathFileName = ArgSettingsPathFileNameDefault; //todo v1.0.45.0 rcArg.GetResult();
                            argProc++;
                        }
                        rcArg = GetArgNameValue(ParamGeneralSettings, ArgSettingsUpdate, paramLine, false);
                        rc += rcArg;
                        if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                        {
                            if (IsValidForSettingBoolValue(rcArg.GetResult()) == false)
                                rc.SetError(1021903, MxError.Source.User, $"parameter {ParamGeneralSettings} argument {ArgSettingsUpdate} value is not '{ArgYes}' or '{ArgNo}'; {rcArg.GetResult()}");
                            else
                            {
                                SettingsUpdate = (rcArg.GetResult() == "yes") ? BoolValue.Yes : BoolValue.No;
                                argProc++;
                            }
                        }
                        if (rc.IsSuccess())
                        {
                           if (argProc < argCnt)
                               rc.SetError(1021904, MxError.Source.User, $"parameter {ParamGeneralSettings} has invalid argument(s); processed {argProc} but found {argCnt}");
                           else
                               rc.SetResult(true);
                        }
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Settings)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessGeneralBackGndParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessGeneralBackGndParam", false);

           // HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamGeneralBackGndColour);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if ((argCnt < 0) || (argCnt > 7))
                    rc.SetError(1022001, MxError.Source.User, $"parameter {ParamGeneralBackGndColour} has incorrect number of arguments; found {argCnt} should be less than 7");
                else
                {
                    var argProc = 0;
                    var rcArg = GetArgNameValue(ParamGeneralBackGndColour, ArgColourText, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        BackGndColourText = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    rcArg = GetArgNameValue(ParamGeneralBackGndColour, ArgColourMsgError, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        BackGndColourMsgError = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    rcArg = GetArgNameValue(ParamGeneralBackGndColour, ArgColourMsgWarn, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        BackGndColourMsgWarn = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    rcArg = GetArgNameValue(ParamGeneralBackGndColour, ArgColourMsgInfo, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        BackGndColourMsgInfo = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    rcArg = GetArgNameValue(ParamGeneralBackGndColour, ArgColourCmds, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        BackGndColourCmds = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    rcArg = GetArgNameValue(ParamGeneralBackGndColour, ArgColourStatus, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        BackGndColourStatus = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    rcArg = GetArgNameValue(ParamGeneralBackGndColour, ArgColourRule, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        BackGndColourRule = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    if (rc.IsSuccess())
                    {
                        if (argProc < argCnt)
                            rc.SetError(1022002, MxError.Source.User, $"parameter {ParamGeneralBackGndColour} has invalid argument(s); processed {argProc} but found {argCnt}");
                        else
                            rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.BackGnd)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessGeneralForeGndParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessGeneralForeGndParam", false);

           // HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamGeneralForeGndColour);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if ((argCnt < 0) || (argCnt > 7))
                    rc.SetError(1022101, MxError.Source.User, $"parameter {ParamGeneralForeGndColour} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var argProc = 0;
                    var rcArg = GetArgNameValue(ParamGeneralForeGndColour, ArgColourText, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        ForeGndColourText = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    rcArg = GetArgNameValue(ParamGeneralForeGndColour, ArgColourMsgError, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        ForeGndColourMsgError = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    rcArg = GetArgNameValue(ParamGeneralForeGndColour, ArgColourMsgWarn, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        ForeGndColourMsgWarn = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    rcArg = GetArgNameValue(ParamGeneralForeGndColour, ArgColourMsgInfo, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        ForeGndColourMsgInfo = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    rcArg = GetArgNameValue(ParamGeneralForeGndColour, ArgColourCmds, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        ForeGndColourCmds = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    rcArg = GetArgNameValue(ParamGeneralForeGndColour, ArgColourStatus, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        ForeGndColourStatus = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    rcArg = GetArgNameValue(ParamGeneralForeGndColour, ArgColourRule, paramLine, false);
                    rc += rcArg;
                    if (rcArg.IsSuccess() && (rcArg.GetResult() != null))
                    {
                        ForeGndColourRule = MxConsole.XlatStringToMxConsoleColor(rcArg.GetResult());
                        argProc++;
                    }
                    if (rc.IsSuccess())
                    {
                        if (argProc < argCnt)
                            rc.SetError(1022101, MxError.Source.User, $"parameter {ParamGeneralForeGndColour} has invalid argument(s); processed {argProc} but found {argCnt}");
                        else
                            rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.ForeGnd)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessGeneralAudioParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessAudioParam", false);

        //    HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamGeneralAudio);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1022201, MxError.Source.User, $"parameter {ParamGeneralAudio} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgNameValue(ParamGeneralAudio, ArgAudioVol, paramLine, true);
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        if(Int32.TryParse(rcArg1.GetResult(), out var vol) == false)
                            rc.SetError(1022202, MxError.Source.User, $"parameter {ParamGeneralAudio} arg {ArgAudioVol} is invalid. It must be a number between {ArgAudioVolMin} and {ArgAudioVolMax}");
                        else
                        {
                            AudioVol = vol;
                            rc.SetResult(true);
                        }
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.Audio)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessToolBrowserParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessToolBrowserParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamToolBrowser);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1022301, MxError.Source.User, $"parameter {ParamToolBrowser} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamToolBrowser}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        // EditFile = rcArg1.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.ToolBrowser)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessToolHelpParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessToolHelpParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamToolHelp);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1022401, MxError.Source.User, $"parameter {ParamToolHelp} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamToolHelp}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        // EditFile = rcArg1.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.ToolHelp)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessToolSearchParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessToolSearchParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamToolHelp);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1022501, MxError.Source.User, $"parameter {ParamToolSearch} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamToolSearch}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        // EditFile = rcArg1.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.ToolSearch)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessToolThesaurusParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessToolSearchParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamToolThesaurus);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1022601, MxError.Source.User, $"parameter {ParamToolThesaurus} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamToolThesaurus}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        // EditFile = rcArg1.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.ToolThesaurus)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessToolSpellParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessToolSpellParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamToolSpell);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1022701, MxError.Source.User, $"parameter {ParamToolSpell} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamToolSpell}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        // EditFile = rcArg1.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.ToolSpell)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessToolSvnParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessToolSvnParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamToolSvn);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1022801, MxError.Source.User, $"parameter {ParamToolSvn} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamToolSvn}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        // EditFile = rcArg1.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.ToolSvn)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessTextEditorRulersParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessTextEditorRulersParam", false);

            //HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamTextEditorRulers);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if ((argCnt < 0) || (argCnt > 3))
                    rc.SetError(1022901, MxError.Source.User, $"parameter {ParamTextEditorRulers} has incorrect number of arguments; found {argCnt} should be 0-3");
                else
                {
                    var argProc = 0;
                    var show = BoolValue.Unset;
                    var rcArgShow = GetArgNameValue(ParamTextEditorRulers, ArgTextEditorRulersShow, paramLine, false);
                    rc += rcArgShow;
                    if (rcArgShow.IsSuccess(true) && (rcArgShow.GetResult() != null))
                    {
                        if (IsValidForSettingBoolValue(rcArgShow.GetResult()) == false)
                            rc.SetError(1022902, MxError.Source.User, $"parameter {ParamTextEditorRulers} argument {ArgTextEditorRulersShow} value is not '{ArgYes}' or '{ArgNo}'; {rcArgShow.GetResult()}");
                        else
                        {
                            show = (rcArgShow.GetResult() == "yes") ? BoolValue.Yes : BoolValue.No;
                            argProc++;
                        }
                    }
                    string unitChar = null;
                    var rcArgCharUnit = GetArgNameValue(ParamTextEditorRulers, ArgTextEditorRulersUnitChar, paramLine, false);
                    rc += rcArgCharUnit;
                    if (rcArgCharUnit.IsSuccess(true) && (rcArgCharUnit.GetResult() != null))
                    {
                        unitChar = rcArgCharUnit.GetResult();
                        if ((string.IsNullOrEmpty(unitChar) == false) && (unitChar.Length == 1))
                            argProc++;
                        else
                        {
                            rc.SetError(1022903, MxError.Source.User, $"parameter {ParamTextEditorRulers} argument {ArgTextEditorRulersUnitChar} value is not a single displayable character; {unitChar ?? Program.ValueNotSet}");
                            unitChar = null;
                        }
                    }
                    string botChar = null;
                    var rcArgCharBot = GetArgNameValue(ParamTextEditorRulers, ArgTextEditorRulersBotChar, paramLine, false);
                    rc += rcArgCharBot;
                    if (rcArgCharBot.IsSuccess(true) && (rcArgCharBot.GetResult() != null))
                    {
                        botChar = rcArgCharBot.GetResult();
                        if ((string.IsNullOrEmpty(botChar) == false) && (botChar.Length == 1))
                            argProc++;
                        else
                        {
                            rc.SetError(1022904, MxError.Source.User, $"parameter {ParamTextEditorRulers} argument {ArgTextEditorRulersBotChar} value is not a single displayable character; {botChar ?? Program.ValueNotSet}");
                            botChar = null;
                        }
                    }
                    if (rc.IsSuccess())
                    {
                        if (argProc < argCnt)
                            rc.SetError(1022905, MxError.Source.User, $"parameter {ParamTextEditorRulers} has invalid argument(s); processed {argProc} but found {argCnt}");
                        else
                        { 
                            if (show != BoolValue.Unset)
                                TextEditorRulersShow = show;
                            if (unitChar != null)
                                TextEditorRulersUnitChar = unitChar[0];
                            if (botChar != null)
                                TextEditorRulersBotChar = botChar[0];
                            rc.SetResult(true);
                        }
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.Rulers)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessTextEditorCursorParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessTextEditorCursorParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamTextEditorCursor);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1023001, MxError.Source.User, $"parameter {ParamTextEditorCursor} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamTextEditorCursor}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        // EditFile = rcArg1.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.Cursor)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessTextEditorDisplayParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessTextEditorDisplayParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamTextEditorDisplay);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1023101, MxError.Source.User, $"parameter {ParamTextEditorDisplay} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamTextEditorDisplay}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        // EditFile = rcArg1.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.Display)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessTextEditorLimitsParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessTextEditorLimitsParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamTextEditorLimits);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1023201, MxError.Source.User, $"parameter {ParamTextEditorLimits} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamTextEditorLimits}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        // EditFile = rcArg1.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.Limits)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessTextEditorTabSizeParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessTextEditorTabSizeParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamTextEditorTabSize);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1023301, MxError.Source.User, $"parameter {ParamTextEditorTabSize} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamTextEditorTabSize}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        // EditFile = rcArg1.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.TabSize)}";
            return rc;
        }


        private MxReturnCode<bool> ProcessTextEditorPauseTimeoutParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessTextEditorPauseTimeoutParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamTextEditorPauseTimeout);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1023401, MxError.Source.User, $"parameter {ParamTextEditorPauseTimeout} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamTextEditorPauseTimeout}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        // EditFile = rcArg1.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.TypingPause)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessTextEditorAutoSaveParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessTextEditorAutoSaveParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamTextEditorAutoSave);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1023501, MxError.Source.User, $"parameter {ParamTextEditorAutoSave} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamTextEditorAutoSave}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        // EditFile = rcArg1.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.AutoSave)}";
            return rc;
        }

        private MxReturnCode<bool> ProcessTextEditorAutoCorrectParam(string paramLine)
        {
            var rc = new MxReturnCode<bool>("CmdLineParamsApp.ProcessTextEditorAutoCorrectParam", false);

            HelpHint = Environment.NewLine;

            var rcCnt = GetArgCount(paramLine, ParamTextEditorAutoCorrect);
            rc += rcCnt;
            if (rcCnt.IsSuccess())
            {
                var argCnt = rcCnt.GetResult();
                if (argCnt != 1)
                    rc.SetError(1023601, MxError.Source.User, $"parameter {ParamTextEditorAutoCorrect} has incorrect number of arguments; found {argCnt} should be two");
                else
                {
                    var rcArg1 = GetArgValue(paramLine, 1, true, $"parameter {ParamTextEditorAutoCorrect}");
                    rc += rcArg1;
                    if (rcArg1.IsSuccess(true))
                    {
                        // EditFile = rcArg1.GetResult();
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError())
                HelpHint = $"{Environment.NewLine}You entered: \"{paramLine}\" {Environment.NewLine}{GetParamHelp((int)Param.AutoCorrect)}";
            return rc;
        }

        //argName is case sensitive and should be a unique argument name in the paraLine
        public static MxReturnCode<string> GetArgNameValue(string paramName, string argName, string paramLine, bool mandatory=true)
        {
            var rc = new MxReturnCode<string>("CmdLineParamsApp.GetArgNameValue");

            if ((string.IsNullOrEmpty(paramName)) || string.IsNullOrEmpty(argName) || string.IsNullOrEmpty(paramLine))
                rc.SetError(1023901, MxError.Source.Param, $"paramName, argName or paramLine is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                var line = paramLine.Trim();
                if (line.Snip(0, paramName.Length-1) != paramName)
                    rc.SetError(1023902, MxError.Source.Program, $"parameter {paramName} not found or not at start of line", MxMsgs.MxErrInvalidCondition);
                else
                {

                    var args = line.Substring(paramName.Length).Trim();
                    var startArgNameIndex = 0;
                    var argValueSeperatorIndex = 0;
                    while ((argValueSeperatorIndex >= 0) && (startArgNameIndex >= 0) && (startArgNameIndex < args.Length))
                    {
                        while (args[startArgNameIndex] == ' ')
                            startArgNameIndex++;
                        if ((argValueSeperatorIndex = args.IndexOfAny(new[] {'=',' '}, startArgNameIndex)) > 0)
                        {
                            var startValueIndex = GetArgValueIndex(true, args, argValueSeperatorIndex+1);
                            var endValueIndex = GetArgValueIndex(false, args, argValueSeperatorIndex+1);
                            var name = args.Snip(startArgNameIndex, argValueSeperatorIndex - 1)?.Trim();
                            if ((name != null) && (args.IndexOf(name + "=", argValueSeperatorIndex, StringComparison.Ordinal) != -1))
                            {
                                rc.SetError(1023903, MxError.Source.User, $"parameter {paramName}, arg {argName}; duplicate name found", MxMsgs.MxErrParamArgNameDuplicateFound);
                                break;
                            }
                            if (name == argName)
                            {
                                var value = args.Snip(startValueIndex, endValueIndex);
                                if (value == null)
                                    rc.SetError(1023904, MxError.Source.User, $"parameter {paramName}, arg {argName}; value not found", MxMsgs.MxErrParamArgValueNotFound);
                                else
                                    rc.SetResult(value);
                                break;
                            }
                            if (endValueIndex < 0)
                                break;
                            startArgNameIndex = (args.IndexOf(' ', endValueIndex ) >= 0) ? args.IndexOf(' ', endValueIndex) + 1 : -1;
                        }
                    }
                    if (rc.IsSuccess() && (rc.GetResult() == null) && (mandatory == true))
                        rc.SetError(1023904, MxError.Source.User, $"parameter {paramName}, arg {argName}; name not found", MxMsgs.MxErrParamArgNameNotFound);
                }
            }
            return rc;
        }

        private static int GetArgValueIndex(bool start, string args, int startArgValueIndex)
        {
            var rc = Program.PosIntegerNotSet;

            if ((string.IsNullOrEmpty(args) == false) && (startArgValueIndex > 0) && (args.Length > startArgValueIndex) && (args[startArgValueIndex-1] == '='))
            {
                if ((args[startArgValueIndex] != quoteChar))
                    rc = start ? startArgValueIndex : (args.IndexOf(' ', startArgValueIndex) >= 0) ? args.IndexOf(' ', startArgValueIndex)-1 : args.Length - 1;
                else
                {
                    if (start)
                        rc = startArgValueIndex + 1;
                    else
                    {
                        var argValueEndIndex = args.IndexOf(quoteChar, startArgValueIndex + 1);
                        if (argValueEndIndex > 0)
                            rc = argValueEndIndex - 1;
                    }
                }
            }
            return rc;
        }

        private MxReturnCode<Param> GetParamType(string paramLine)
        {
            var rc = new MxReturnCode<Param>("CmdLineParamsApp.GetParamType", Param.Unknown);

            if (paramLine == null)
                rc.SetError(1024001, MxError.Source.Param, $"param is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                var offset = paramLine.IndexOf(spaceChar);
                var name = (offset == -1) ? paramLine.Trim().ToLower() : paramLine.Substring(0, offset).Trim().ToLower();
                var param = Param.Unknown;

                if (EnumOps.XlatFromString(name, out param) == false)
                    rc.SetError(1024001, MxError.Source.User, $"the parameter {name} is invalid");
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
            //accordingly the final properties are those in the found in the cmd line for this run (highest priority) together with any found in setting file (lower priority) 
            if (AudioFileKeyPress == null)
                AudioFileKeyPress = savedSettings.AudioFileKeyPress;
            if (AudioFileBackSpace == null)
                AudioFileBackSpace = savedSettings.AudioFileBackSpace;
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

            if (BackGndColourText == MxConsole.Color.NotSet)
                BackGndColourText = savedSettings.BackGndColourText;
            if (ForeGndColourText == MxConsole.Color.NotSet)
                ForeGndColourText = savedSettings.ForeGndColourText;
            if (BackGndColourMsgError == MxConsole.Color.NotSet)
                BackGndColourMsgError = savedSettings.BackGndColourMsgError;
            if (ForeGndColourMsgError == MxConsole.Color.NotSet)
                ForeGndColourMsgError = savedSettings.ForeGndColourMsgError;
            if (BackGndColourMsgWarn == MxConsole.Color.NotSet)
                BackGndColourMsgWarn = savedSettings.BackGndColourMsgWarn;
            if (ForeGndColourMsgWarn == MxConsole.Color.NotSet)
                ForeGndColourMsgWarn = savedSettings.ForeGndColourMsgWarn;
            if (BackGndColourMsgInfo == MxConsole.Color.NotSet)
                BackGndColourMsgInfo = savedSettings.BackGndColourMsgInfo;
            if (ForeGndColourMsgInfo == MxConsole.Color.NotSet)
                ForeGndColourMsgInfo = savedSettings.ForeGndColourMsgInfo;
            if (BackGndColourCmds == MxConsole.Color.NotSet)
                BackGndColourCmds = savedSettings.BackGndColourCmds;
            if (ForeGndColourCmds == MxConsole.Color.NotSet)
                 ForeGndColourCmds = savedSettings.ForeGndColourCmds;
            if (BackGndColourStatus == MxConsole.Color.NotSet)
                BackGndColourStatus = savedSettings.BackGndColourStatus;
            if (ForeGndColourStatus == MxConsole.Color.NotSet)
                ForeGndColourStatus = savedSettings.ForeGndColourStatus;
            if (BackGndColourRule == MxConsole.Color.NotSet)
                BackGndColourRule = savedSettings.BackGndColourRule;
            if (ForeGndColourRule == MxConsole.Color.NotSet)
                ForeGndColourRule = savedSettings.ForeGndColourRule;
            if (AudioVol == Program.PosIntegerNotSet)
                AudioVol = savedSettings.AudioVol;

            if (ToolBrowserExe == null)
                ToolBrowserExe = savedSettings.ToolBrowserExe;
            if (ToolHelpUrl == null)
                ToolHelpUrl = savedSettings.ToolHelpUrl;
            if (ToolSearchUrl == null)
                ToolSearchUrl = savedSettings.ToolSearchUrl;
            if (ToolThesaurusUrl == null)
                ToolThesaurusUrl = savedSettings.ToolThesaurusUrl;
            if (ToolSpellUrl == null)
                ToolSpellUrl = savedSettings.ToolSpellUrl;
            if (ToolSvnUser == null)
                ToolSvnUser = savedSettings.ToolSvnUser;
            if (ToolSvnPasswordKey == null)
                ToolSvnPasswordKey = savedSettings.ToolSvnPasswordKey;
            if (ToolSvnUrl == null)
                ToolSvnUrl = savedSettings.ToolSvnUrl;

            if (TextEditorRulersShow == BoolValue.Unset)
                TextEditorRulersShow = savedSettings.TextEditorRulersShow;
            if (TextEditorRulersUnitChar == Program.NullChar)
                TextEditorRulersUnitChar = savedSettings.TextEditorRulersUnitChar;
            if (TextEditorRulersBotChar == Program.NullChar)
                TextEditorRulersBotChar = savedSettings.TextEditorRulersBotChar;
            if (TextEditorCursorSize == Program.PosIntegerNotSet)
                TextEditorCursorSize = savedSettings.TextEditorCursorSize;
            if (TextEditorDisplayRows == Program.PosIntegerNotSet)
                TextEditorDisplayRows = savedSettings.TextEditorDisplayRows;
            if (TextEditorDisplayCols == Program.PosIntegerNotSet)
                TextEditorDisplayCols = savedSettings.TextEditorDisplayCols;
            if (TextEditorParaBreakDisplayChar == Program.NullChar)
                TextEditorParaBreakDisplayChar = savedSettings.TextEditorParaBreakDisplayChar;
            if (TextEditorPauseTimeout == Program.PosIntegerNotSet)
                TextEditorPauseTimeout = savedSettings.TextEditorPauseTimeout;
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

        
        protected override string GetParamHelp(int paramId = 0) // paramId = KLineEditor.PosIntegerNotSet 
        {
            var rc = "";

            var msg = $"{Environment.NewLine}Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}";

            Param help = (Param)paramId;
            if (help == Param.Help)
            {
                msg += GetHelpInfoHelp() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.ExportFile)
            {
                msg += GetHelpInfoExport() + Environment.NewLine; 
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.ImportFile)
            {
                msg += GetHelpInfoImport() + Environment.NewLine; 
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.EditFile)
            {
                msg += GetHelpInfoEdit() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.Settings)
            {
                msg += GetHelpInfoGeneralSettings() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.BackGnd)
            {
                msg += GetHelpInfoGeneralBackGndColour() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.ForeGnd)
            {
                msg += GetHelpInfoGeneralForeGndColour() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.Audio)
            {
                msg += GetHelpInfoGeneralAudio() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.ToolBrowser)
            {
                msg += GetHelpInfoToolBrowser() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.ToolHelp)
            {
                msg += GetHelpInfoToolHelp() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.ToolSearch)
            {
                msg += GetHelpInfoToolSearch() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.ToolThesaurus)
            {
                msg += GetHelpInfoToolThesaurus() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.ToolSpell)
            {
                msg += GetHelpInfoToolSpell() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.ToolSvn)
            {
                msg += GetHelpInfoToolSvn() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.Rulers)
            {
                msg += GetHelpInfoTextEditorRulers() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.Cursor)
            {
                msg += GetHelpInfoTextEditorCursor() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.Display)
            {
                msg += GetHelpInfoTextEditorDisplay() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.Limits)
            {
                msg += GetHelpInfoTextEditorLimits() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.TabSize)
            {
                msg += GetHelpInfoTextEditorTabSize() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.TypingPause)
            {
                msg += GetHelpInfoTextEditorPauseTimeout() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.AutoSave)
            {
                msg += GetHelpInfoTextEditorAutoSave() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else if (help == Param.AutoCorrect)
            {
                msg += GetHelpInfoTextEditorAutoCorrect() + Environment.NewLine;
                msg += GetAppHelpNotes();
                rc = msg;
            }
            else
            {
                rc = GetHelpInfoAll();
            }
            return rc;
        }

        private static string GetHelpInfoHelp() { return $"{ParamHelp}"; }
        private static string GetHelpInfoExport() { return $"{ParamExportFile} {ArgFileFrom}={EditFileNameForm} {ArgFileTo}={TextFileNameForm}"; }
        private static string GetHelpInfoImport() { return $"{ParamImportFile} {ArgFileFrom}={TextFileNameForm} {ArgFileTo}={EditFileNameForm}"; }
        private static string GetHelpInfoEdit() { return $"{ParamEditFile} {EditFileNameForm}"; }
        private static string GetHelpInfoGeneralSettings() { return $"{ParamGeneralSettings} ({ArgSettingsDisplay}=[yes|no]) ({ArgSettingsPathFileName}={SettingsFileNameForm}) ({ArgSettingsUpdate}=[yes|no]))"; }
        private static string GetHelpInfoGeneralForeGndColour() { return $"{ParamGeneralForeGndColour} ({ArgColourText}=COLOR) ({ArgColourMsgError}=COLOR) ({ArgColourMsgWarn}=COLOR) ({ArgColourMsgInfo}=COLOUR) ({ArgColourCmds}=COLOR) ({ArgColourStatus}=COLOR) ({ArgColourRule}=COLOR)"; }
        private static string GetHelpInfoGeneralBackGndColour() { return $"{ParamGeneralBackGndColour} ({ArgColourText}=COLOR) ({ArgColourMsgError}=COLOR) ({ArgColourMsgWarn}=COLOR) ({ArgColourMsgInfo}=COLOUR) ({ArgColourCmds}=COLOR) ({ArgColourStatus}=COLOR) ({ArgColourRule}=COLOR)"; }
        private static string GetHelpInfoGeneralAudio() { return $"{ParamGeneralAudio} {ArgAudioVol}={ArgAudioVolDefault} <min {ArgAudioVolMin} max {ArgAudioVolMax}>"; }
        private static string GetHelpInfoTextEditorRulers() { return $"{ParamTextEditorRulers} ({ArgTextEditorRulersShow}=[yes|no]) ({ArgTextEditorRulersUnitChar}=.) ({ArgTextEditorRulersBotChar}=_)"; }
        private static string GetHelpInfoTextEditorCursor() { return $"{ParamTextEditorCursor} {ArgTextEditorCursorSize}={ArgTextEditorCursorSizeDefault} <min {ArgTextEditorCursorSizeMin} max {ArgTextEditorCursorSizeMax}>"; }
        private static string GetHelpInfoTextEditorDisplay() { return $"{ParamTextEditorDisplay} ({ArgTextEditorDisplayRows}={ArgTextEditorDisplayRowsDefault} <min {ArgTextEditorDisplayRowsMin} max {ArgTextEditorDisplayRowsMax}>) ({ArgTextEditorDisplayCols}={ArgTextEditorDisplayColsDefault} <min {ArgTextEditorDisplayColsMin} max {ArgTextEditorDisplayColsMax}>) ({ArgTextEditorDisplayParaBreakDisplayChar}={ArgTextEditorDisplayParaBreakDisplayCharDefault})"; }
        private static string GetHelpInfoTextEditorLimits() { return $"{ParamTextEditorLimits} ({ArgTextEditorLimitEdit}={ArgTextEditorLimitEditDefault} <min {ArgTextEditorLimitEditMin} max {ArgTextEditorLimitEditMax}>) ({ArgTextEditorLimitScroll}={ArgTextEditorLimitScrollDefault} <min {ArgTextEditorLimitScrollMin} max {ArgTextEditorLimitScrollMax}>)"; }
        private static string GetHelpInfoTextEditorTabSize() { return $"{ParamTextEditorTabSize} {ArgTextEditorTabSizeDefault} <min {ArgTextEditorTabSizeMin} max {ArgTextEditorTabSizeMax}>"; }
        private static string GetHelpInfoTextEditorPauseTimeout() { return $"{ParamTextEditorPauseTimeout} {ArgTextEditorPauseTimeoutDefault} <min {ArgTextEditorPauseTimeoutMin} max {ArgTextEditorPauseTimeoutMax}>"; }
        private static string GetHelpInfoTextEditorAutoSave() { return $"{ParamTextEditorAutoSave} [{ArgTextEditorAutoSaveCR}|{ArgTextEditorAutoSaveParaBreak}|{ArgTextEditorAutoSaveOff}]"; }
        private static string GetHelpInfoTextEditorAutoCorrect() { return $"{ParamTextEditorAutoCorrect} [on|off]"; }
        private static string GetHelpInfoToolBrowser() { return $"{ParamToolBrowser} {ArgToolBrowserExe}={ExeFileNameForm}"; }
        private static string GetHelpInfoToolHelp() { return $"{ParamToolHelp} {ArgToolBrowserUrl}={UrlForm}"; }
        private static string GetHelpInfoToolSearch() { return $"{ParamToolSearch} {ArgToolBrowserUrl}={UrlForm}"; }
        private static string GetHelpInfoToolThesaurus() { return $"{ParamToolThesaurus} {ArgToolBrowserUrl}={UrlForm}"; }
        private static string GetHelpInfoToolSpell() { return $"{ParamToolSpell} {ArgToolBrowserUrl}={UrlForm}"; }
        private static string GetHelpInfoToolSvn() { return $"{ParamToolSvn} ({ArgToolSvnUser}={UsernameForm }) ({ArgToolSvnPassword}={PasswordKeyForm}) ({ArgToolSvnUrl}={UrlForm})"; }

        private static string GetHelpInfoAll()
        {
            var msg = $"{Environment.NewLine}Hint: retry using program's expected parameters and their arguments which are:{Environment.NewLine}";

            msg += Environment.NewLine;
            msg += $"[{GetHelpInfoHelp()} |" + Environment.NewLine;
            msg += $"{GetHelpInfoExport()} |" + Environment.NewLine;
            msg += $"{GetHelpInfoImport()} |" + Environment.NewLine;
            msg += $"{GetHelpInfoEdit()}" + Environment.NewLine;
            msg += "(" + Environment.NewLine;
            msg += $"   {GetHelpInfoGeneralSettings()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoGeneralForeGndColour()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoGeneralBackGndColour()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoGeneralAudio()}" + Environment.NewLine;
            msg += Environment.NewLine;
            msg += $"   {GetHelpInfoTextEditorRulers()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoTextEditorCursor()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoTextEditorDisplay()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoTextEditorLimits()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoTextEditorTabSize()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoTextEditorPauseTimeout()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoTextEditorAutoSave()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoTextEditorAutoCorrect()}" + Environment.NewLine;
            msg += Environment.NewLine;
            msg += $"   {GetHelpInfoToolBrowser()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoToolHelp()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoToolSearch()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoToolThesaurus()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoToolSpell()}" + Environment.NewLine;
            msg += $"   {GetHelpInfoToolSvn()}" + Environment.NewLine;
            msg += ")] " + Environment.NewLine;

            msg += GetAppHelpNotes();

            return msg;
        }

        private static string GetAppHelpNotes()
        {
            var rc = GetHelpNotes() + Environment.NewLine;

            //{word} is the word currently selected by the cursor
            //{version} is the current help documentation version(typically the app version major.minor)
            //COLOUR is any of the defined colour words; ArgBlack = "black"; ArgBlue = "blue"; ArgCyan = "cyan"; ArgDarkBlue = "darkblue";
            //ArgDarkCyan = "darkcyan"; ArgDarkGray = "darkgray"; ArgDarkGreen = "darkgreen"; ArgDarkMagenta = "darkmagenta";
            //ArgDarkRed = "darkred"; ArgDarkYellow = "darkyellow"; ArgGray = "gray"; ArgGreen = "green"; ArgMagenta = "magenta";
            //ArgRed = "red"; ArgWhite = "white"; ArgYellow = "yellow";
            //    > is any displayable character
            //    . is any displayable character
            //drive:path\ is any valid full path
            //    [secret manager key] lookup value in secret manager using key value

            rc += $"File: {EditFileNameForm} is any valid drive, path, and filename for ksx file" + Environment.NewLine;
            rc += $"Url: '{UrlForm}' is any valid url and arguments" + Environment.NewLine;
            rc += "Characters: '>' or '.' are any displayable character" + Environment.NewLine;
            //rc += "Application Variables: 'word' is word at cursor, 'HelpVer' is help version" + Environment.NewLine;
            rc += "COLOR: " + MxConsole.GetMxConsoleColorNameList(Environment.NewLine + "       ") + Environment.NewLine;

            return rc;
        }

    }
}
