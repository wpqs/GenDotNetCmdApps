﻿using System;
using KLineEdCmdApp.Utils;
using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.UtilsTests
{
    public class CmdLineParamsAppTest : IClassFixture<UtilsCmdLineParamsAppFixture>
    {
        public static readonly string[] StdParamsHelp = { "--Help" };

        private readonly UtilsCmdLineParamsAppFixture _fixture;

        public CmdLineParamsAppTest(UtilsCmdLineParamsAppFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestNoParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(null);

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 2010101-param: args is null", rcParam.GetErrorUserMsg());
        }

        [Fact]
        public void TestUnknownParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--xhelp" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1024001-user: the parameter --xhelp is invalid", rcParam.GetErrorUserMsg());
            Assert.Contains("Hint: retry using program's expected parameters and their arguments which are:", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestHelpParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(StdParamsHelp);

            Assert.True(rcParam.GetResult());
            Assert.Equal(CmdLineParamsApp.OpMode.Help, cmdLineParams.Op);
            Assert.StartsWith("Help request", cmdLineParams.HelpHint);
            Assert.Contains("Hint: retry using program's expected parameters and their arguments which are:", cmdLineParams.HelpHint);
        }


        [Fact]
        public void TestHelpParamUnexpectedArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "x" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021501-user: parameter --help has incorrect number of arguments; found 1", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--help", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestExportParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--export", "from=Edit.ksx", "to=Export.txt" });

            Assert.True(rcParam.GetResult());
            Assert.Equal("Export.txt", cmdLineParams.ExportOutputFile);
            Assert.Equal("Edit.ksx", cmdLineParams.EditFile);
            Assert.Equal(CmdLineParamsApp.OpMode.Export, cmdLineParams.Op);
            Assert.Equal(Environment.NewLine, cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestExportParamNoArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--export" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021601-user: parameter '--export' has incorrect number of arguments; found 0", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--export", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestExportParamNoSecondArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--export", "test.txt" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021601-user: parameter '--export' has incorrect number of arguments; found 1", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--export", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestExportParamExtraArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--export", "Edit.ksx", "Export.txt", "extra" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021601-user: parameter '--export' has incorrect number of arguments; found 3", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--export", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestImportParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--import", "from=Import.txt", "to=Edit.ksx" });

            Assert.True(rcParam.GetResult());
            Assert.Equal("Import.txt", cmdLineParams.ImportInputFile);
            Assert.Equal("Edit.ksx", cmdLineParams.EditFile);
            Assert.Equal(CmdLineParamsApp.OpMode.Import, cmdLineParams.Op);
            Assert.Equal(Environment.NewLine, cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestImportParamNoArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--import" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021701-user: parameter '--import' has incorrect number of arguments; found 0", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--import", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestImportParamNoSecondArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--import", "test.ksx" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021701-user: parameter '--import' has incorrect number of arguments; found 1", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--import", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestImportParamExtraArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--import", "Edit.txt", "Export.ksx", "extra" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021701-user: parameter '--import' has incorrect number of arguments; found 3", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--import", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestSettingsParamsAll()
        {     
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] {"--help", "--settings", "display=yes", "file='KLineEdCmdApp.json'", "update=yes" });

            Assert.True(rcParam.GetResult());
            Assert.Equal("KLineEdCmdApp.json", cmdLineParams.SettingsPathFileName);
            Assert.Equal(CmdLineParamsApp.BoolValue.Yes, cmdLineParams.SettingsDisplay);
            Assert.Equal(CmdLineParamsApp.BoolValue.Yes, cmdLineParams.SettingsUpdate);
            Assert.Contains("Help request:", cmdLineParams.HelpHint);
        }


        [Fact]
        public void TestSettingsParamsNone()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--settings" });

            Assert.True(rcParam.GetResult());
            Assert.Equal("KLineEdCmdApp.json", cmdLineParams.SettingsPathFileName);
            Assert.Contains("Help request:", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestSettingsParamsNameFail()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--settings", "diXplay=yes" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1021904-user: parameter '--settings' has invalid argument(s); processed 0 but found 1", rcParam.GetErrorTechMsg());
        }
        [Fact]
        public void TestSettingsParamsDisplayValueFail()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--settings", "display=yXs" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1021902-user: parameter '--settings' argument 'display' value is not 'yes' or 'no'; yXs", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestSettingsParamsUpdateValueFail()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--settings", "update=yXs" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1021903-user: parameter '--settings' argument 'update' value is not 'yes' or 'no'; yXs", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestBackGndParamsAll()
        {       
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--backgnd", "text=black", "msg-error=white", "msg-warn=gray", "msg-info=red", "cmds=green", "status=blue", "rule=yellow" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(MxConsole.Color.Black, cmdLineParams.BackGndColourText);
            Assert.Equal(MxConsole.Color.White, cmdLineParams.BackGndColourMsgError);
            Assert.Equal(MxConsole.Color.Gray, cmdLineParams.BackGndColourMsgWarn);
            Assert.Equal(MxConsole.Color.Red, cmdLineParams.BackGndColourMsgInfo);
            Assert.Equal(MxConsole.Color.Green, cmdLineParams.BackGndColourCmds);
            Assert.Equal(MxConsole.Color.Blue, cmdLineParams.BackGndColourStatus);
            Assert.Equal(MxConsole.Color.Yellow, cmdLineParams.BackGndColourRule);
        }

        [Fact]
        public void TestBackGndParamsNone()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--backgnd" });

            Assert.True(rcParam.GetResult());
        }

        [Fact]
        public void TestBackGndParamsMin()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--backgnd", "cmds=green" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(MxConsole.Color.Green, cmdLineParams.BackGndColourCmds);
        }

        [Fact]
        public void TestBackGndParamsNameFail()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--backgnd", "cXds=green" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1022002-user: parameter '--backgnd' has invalid argument(s); processed 0 but found 1", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestBackGndParamsValueFail()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--backgnd", "cmds=grXen" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020306-user: parameter --backgnd has a bad argument; value of 'cmds' is not a valid COLOR", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestForeGndParamsAll()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--foregnd", "text=black", "msg-error=white", "msg-warn=gray", "msg-info=red", "cmds=green", "status=blue", "rule=yellow" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(MxConsole.Color.Black, cmdLineParams.ForeGndColourText);
            Assert.Equal(MxConsole.Color.White, cmdLineParams.ForeGndColourMsgError);
            Assert.Equal(MxConsole.Color.Gray, cmdLineParams.ForeGndColourMsgWarn);
            Assert.Equal(MxConsole.Color.Red, cmdLineParams.ForeGndColourMsgInfo);
            Assert.Equal(MxConsole.Color.Green, cmdLineParams.ForeGndColourCmds);
            Assert.Equal(MxConsole.Color.Blue, cmdLineParams.ForeGndColourStatus);
            Assert.Equal(MxConsole.Color.Yellow, cmdLineParams.ForeGndColourRule);
        }

        [Fact]
        public void TestForeGndParamsNone()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--foregnd" });

            Assert.True(rcParam.GetResult());
        }

        [Fact]
        public void TestForeGndParamsMin()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--foregnd", "cmds=green" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(MxConsole.Color.Green, cmdLineParams.ForeGndColourCmds);
        }

        [Fact]
        public void TestForeGndParamsNameFail()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--foregnd", "cXds=green" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1022102-user: parameter '--foregnd' has invalid argument(s); processed 0 but found 1", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestForeGndParamsValueFail()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--foregnd", "cmds=grXen" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020307-user: parameter '--foregnd' has a bad argument; value of 'cmds' is not a valid COLOR", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestAudiosParams()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--audio", "vol=1" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(1, cmdLineParams.AudioVol);
            Assert.Contains("Help request:", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestAudiosParamsMin()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--audio", "vol=0" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(0, cmdLineParams.AudioVol);
            Assert.Contains("Help request:", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestAudiosParamsMax()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--audio", "vol=10" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(10, cmdLineParams.AudioVol);
            Assert.Contains("Help request:", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestAudiosParamsFailTooBig()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--audio", "vol=11" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020308-user: parameter '--audio' has a bad argument; value 11 is invalid for 'vol'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestAudiosParamsFailTooSmall()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--audio", "vol=-2" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020308-user: parameter '--audio' has a bad argument; value -2 is invalid for 'vol'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestRuleParamsAll()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--rulers", "show=yes", "unitchar=x", "botchar=y" });

            Assert.True(rcParam.GetResult());
            Assert.Equal('x', cmdLineParams.TextEditorRulersUnitChar);
            Assert.Equal('y', cmdLineParams.TextEditorRulersBotChar);
            Assert.Equal(CmdLineParamsApp.BoolValue.Yes, cmdLineParams.TextEditorRulersShow);
        }

        [Fact]
        public void TestRuleParamsNoParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--rulers" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(CmdLineParamsApp.BoolValue.Yes, cmdLineParams.TextEditorRulersShow);
            Assert.Equal('.', cmdLineParams.TextEditorRulersUnitChar);
            Assert.Equal('_', cmdLineParams.TextEditorRulersBotChar);
        }

        [Fact]
        public void TestRuleParamsShowParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--rulers", "show=no" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(CmdLineParamsApp.BoolValue.No, cmdLineParams.TextEditorRulersShow);
        }

        [Fact]
        public void TestRuleParamsUnitCharParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--rulers", "unitchar=z" });

            Assert.True(rcParam.GetResult());
            Assert.Equal('z', cmdLineParams.TextEditorRulersUnitChar);
        }

        [Fact]
        public void TestRuleParamsBotCharParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--rulers", "botchar=z" });

            Assert.True(rcParam.GetResult());
            Assert.Equal('z', cmdLineParams.TextEditorRulersBotChar);
        }

        [Fact]
        public void TestRuleParamsFailBadArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--rulers", "show=yes", "uniXchar=x", "botchar=y" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1022905-user: parameter '--rulers' has invalid argument(s); processed 2 but found 3", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestRuleParamsFailExcessArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--rulers", "show=yes", "unitchar=x", "botchar=y", "extra=x" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1022901-user: parameter '--rulers' has incorrect number of arguments; found 4 should be 0-3", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestCursorParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--cursor", "size=20" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(20, cmdLineParams.TextEditorCursorSize);
        }

        [Fact]
        public void TestCursorParamFailTooBig()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--cursor", "size=101" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020311-user: parameter '--cursor' has a bad argument; value '101' is invalid for 'size'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestCursorParamFailTooSmall()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--cursor", "size=0" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020311-user: parameter '--cursor' has a bad argument; value '0' is invalid for 'size'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestCursorParamFailInvalidName()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--cursor", "sXze=1" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--cursor' is missing argument 'size'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestCursorParamFailInvalidValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--cursor", "size=X" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023001-user: parameter '--cursor' argument 'size' value 'X' is invalid. It must be a number between 1 and 100", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestDisplayParamAll()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "rows=10", "cols=50", "parabreak=." });

            Assert.True(rcParam.GetResult());
            Assert.Equal(10, cmdLineParams.TextEditorDisplayRows);
            Assert.Equal(50, cmdLineParams.TextEditorDisplayCols);
            Assert.Equal('.', cmdLineParams.TextEditorParaBreakDisplayChar);
        }

        [Fact]
        public void TestDisplayParamNone()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display" });

            Assert.True(rcParam.GetResult());
            Assert.Equal( CmdLineParamsApp.ArgTextEditorDisplayRowsDefault, cmdLineParams.TextEditorDisplayRows);
            Assert.Equal(CmdLineParamsApp.ArgTextEditorDisplayColsDefault, cmdLineParams.TextEditorDisplayCols);
            Assert.Equal(CmdLineParamsApp.ArgTextEditorDisplayParaBreakDisplayCharDefault, cmdLineParams.TextEditorParaBreakDisplayChar);
        }

        [Fact]
        public void TestDisplayParamRows()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "rows=15" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(15, cmdLineParams.TextEditorDisplayRows);
            Assert.Equal(CmdLineParamsApp.ArgTextEditorDisplayColsDefault, cmdLineParams.TextEditorDisplayCols);
            Assert.Equal(CmdLineParamsApp.ArgTextEditorDisplayParaBreakDisplayCharDefault, cmdLineParams.TextEditorParaBreakDisplayChar);
        }

        [Fact]
        public void TestDisplayParamCols()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "cols=51" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(51, cmdLineParams.TextEditorDisplayCols);
            Assert.Equal(CmdLineParamsApp.ArgTextEditorDisplayRowsDefault, cmdLineParams.TextEditorDisplayRows);
            Assert.Equal(CmdLineParamsApp.ArgTextEditorDisplayParaBreakDisplayCharDefault, cmdLineParams.TextEditorParaBreakDisplayChar);
        }

        [Fact]
        public void TestDisplayParamParaBreak()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "parabreak=>" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(CmdLineParamsApp.ArgTextEditorDisplayRowsDefault, cmdLineParams.TextEditorDisplayRows);
            Assert.Equal(CmdLineParamsApp.ArgTextEditorDisplayColsDefault, cmdLineParams.TextEditorDisplayCols);
            Assert.Equal('>', cmdLineParams.TextEditorParaBreakDisplayChar);
        }

        [Fact]
        public void TestDisplayParamFailParaBreakValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "parabreak=>>" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023104-user: parameter '--display' argument 'parabreak' value is not a single displayable character; >>", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestDisplayParamFailMinCols()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "cols=1" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020313-user: parameter '--display' has a bad argument; value '1' is invalid for 'cols'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestDisplayParamFailMaxCols()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "cols=500" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020313-user: parameter '--display' has a bad argument; value '500' is invalid for 'cols'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestDisplayParamFailColValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "cols=X" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023103-user: parameter '--display' argument 'cols' value 'X' is invalid. It must be a number between 25 and 250", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestDisplayParamFailRowValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "rows=X" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023102-user: parameter '--display' argument 'rows' value 'X' is invalid. It must be a number between 5 and 50", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestDisplayParamFailMinRows()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "rows=0" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020312-user: parameter '--display' has a bad argument; value '0' is invalid for 'rows'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestDisplayParamFailMaxRows()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "rows=500" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020312-user: parameter '--display' has a bad argument; value '500' is invalid for 'rows'", rcParam.GetErrorTechMsg());
        }


        [Fact]
        public void TestDisplayParamFailName()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "coXs=1" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("1023105-user: parameter '--rulers' has invalid argument(s); processed 0 but found 1", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestDisplayParamFailExtra()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "rows=10", "cols=50", "parabreak=.", "extra=1" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023101-user: parameter '--display' has incorrect number of arguments; found 4 should be 0-3", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestLimitsParamScrollBack()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--limits", "scrollback=10" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(10, cmdLineParams.TextEditorScrollLimit);

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--limits", "scrollback=0" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(0, cmdLineParams.TextEditorScrollLimit);

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--limits", "scrollback=90000" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(90000, cmdLineParams.TextEditorScrollLimit);

        }

        [Fact]
        public void TestLimitsParamScrollBackFailOutOfRange()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--limits", "scrollback=-2" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020316-user: parameter '--limits' has a bad argument; value '-2' is invalid for 'scrollback'", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--limits", "scrollback=90001" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020316-user: parameter '--limits' has a bad argument; value '90001' is invalid for 'scrollback'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestLimitsParamScrollBackFailBadName()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--limits", "scrolXback=2" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--limits' is missing argument 'scrollback'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestLimitsParamScrollBackFailBadValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--limits", "scrollback=X" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023202-user: parameter '--limits' argument 'scrollback' value 'X' is invalid. It must be a number between 0 and 90000", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestLimitsParamScrollBackFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--limits", "scrollback=1", "extra=x" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023201-user: parameter '--limits' has incorrect number of arguments; found 2 should be 1", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--limits" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023201-user: parameter '--limits' has incorrect number of arguments; found 0 should be 1", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestLimitsParamScrollBackMax()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--limits", "scrollback=90000" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(90000, cmdLineParams.TextEditorScrollLimit);
        }


        [Fact]
        public void TestTabSizeParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--tabsize", "1" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(1, cmdLineParams.TextEditorTabSize);

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--tabsize", "25" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(25, cmdLineParams.TextEditorTabSize);
        }

        [Fact]
        public void TestTabSizeParamFailOutOfRange()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--tabsize", "0" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020317-user: parameter '--tabsize' value '0' is invalid", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--tabsize", "26" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020317-user: parameter '--tabsize' value '26' is invalid", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestTabSizeParamFailBadValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--tabsize", "X" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023302-user: parameter '--tabsize' value 'X' is invalid. It must be a number between 1 and 25", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestTabSizeParamFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--tabsize" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023301-user: parameter '--tabsize' has incorrect number of arguments; found 0 should be 1", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--tabsize", "12", "9" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023301-user: parameter '--tabsize' has incorrect number of arguments; found 2 should be 1", rcParam.GetErrorTechMsg());

        }

        [Fact]
        public void TestPauseTimeoutParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--typingpause", "seconds=5" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(5, cmdLineParams.TextEditorPauseTimeout);

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--typingpause", "seconds=86400" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(86400, cmdLineParams.TextEditorPauseTimeout);
        }

        [Fact]
        public void TestPauseTimeoutParamFailOutOfRange()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--typingpause", "seconds=-2" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020318-user: parameter '--typingpause' has a bad argument; value '-2' is invalid for 'seconds'", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--typingpause", "seconds=86401" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020318-user: parameter '--typingpause' has a bad argument; value '86401' is invalid for 'seconds'", rcParam.GetErrorTechMsg());
        }


        [Fact]
        public void TestPauseTimeoutParamFailBadValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--typingpause", "seconds=X" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023402-user: parameter '--typingpause' argument 'seconds' value 'X' is invalid. It must be a number between  5 and 86400", rcParam.GetErrorTechMsg());
        }


        [Fact]
        public void TestPauseTimeoutParamFailBadName()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--typingpause", "secs=5" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--typingpause' is missing argument 'seconds'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestPauseTimeoutParamFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--typingpause" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023401-user: parameter '--typingpause' has incorrect number of arguments; found 0 should be 1", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--typingpause", "seconds=12", "9"});

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023401-user: parameter '--typingpause' has incorrect number of arguments; found 2 should be 1", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestAutoSaveParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--autosave", "minutes=0" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(0, cmdLineParams.TextEditorAutoSavePeriod);

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--autosave", "minutes=1440" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(1440, cmdLineParams.TextEditorAutoSavePeriod);
        }

        [Fact]
        public void TestAutoSaveParamFailOutOfRange()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--autosave", "minutes=-2" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020319-user: parameter '--autosave' has a bad argument; value '-2' is invalid for 'minutes'", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--autosave", "minutes=1441" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020319-user: parameter '--autosave' has a bad argument; value '1441' is invalid for 'minutes'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestAutoSaveParamFailBadValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--autosave", "minutes=X" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023502-user: parameter '--autosave' argument 'minutes' value 'X' is invalid. It must be a number between  0 and 1440", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestAutoSaveParamFailBadName()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--autosave", "miXutes=0" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--autosave' is missing argument 'minutes'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestAutoSaveParamFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--autosave" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023501-user: parameter '--autosave' has incorrect number of arguments; found 0 should be 1", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--autosave", "minutes=0", "seconds" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023501-user: parameter '--autosave' has incorrect number of arguments; found 2 should be 1", rcParam.GetErrorTechMsg());

        }

        [Fact]
        public void TestAutoCorrectParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--autocorrect", "yes" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(CmdLineParamsApp.BoolValue.Yes, cmdLineParams.TextEditorAutoCorrect);

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--autocorrect", "no" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(CmdLineParamsApp.BoolValue.No, cmdLineParams.TextEditorAutoCorrect);
        }

        [Fact]
        public void TestAutoCorrectParamFailBadValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--autocorrect", "yXs" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023602-user: parameter '--autocorrect' argument is not 'yes' or 'no'; yXs", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestAutoCorrectParamFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--autocorrect" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023601-user: parameter '--autocorrect' has incorrect number of arguments; found 0 should be 1", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--autocorrect", "yes", "no" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023601-user: parameter '--autocorrect' has incorrect number of arguments; found 2 should be 1", rcParam.GetErrorTechMsg());

        }

        [Fact]
        public void TestLinesPerPageParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--linesperpage", "1" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(1, cmdLineParams.TextEditorLinesPerPage);

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--linesperpage", "10000" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(10000, cmdLineParams.TextEditorLinesPerPage);
        }

        [Fact]
        public void TestLinesPerPageParamFailOutOfRange()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--linesperpage", "-2" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020320-user: parameter '--linesperpage' has bad argument; value '-2' is invalid", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--linesperpage", "10001" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020320-user: parameter '--linesperpage' has bad argument; value '10001' is invalid", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestPerPageParamFailBadValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--linesperpage", "X" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023502-user: parameter '--linesperpage' value 'X' is invalid. It must be a number between  1 and 10000", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestPerPageParamFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--linesperpage" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023701-user: parameter --linesperpage has incorrect number of arguments; found 0 should be 1", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--linesperpage", "12", "99" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023701-user: parameter --linesperpage has incorrect number of arguments; found 2 should be 1", rcParam.GetErrorTechMsg());

        }

        [Fact]
        public void TestToolBrowserParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolbrowser", "command=cmd" });

            Assert.True(rcParam.GetResult());
            Assert.Equal("cmd", cmdLineParams.ToolBrowserCmd);

            rcParam = cmdLineParams.Initialise(new[] {"--help", "--toolbrowser", "command='c:\\ explorer.exe a b c'"});

            Assert.True(rcParam.GetResult());
            Assert.Equal("c:\\ explorer.exe a b c", cmdLineParams.ToolBrowserCmd);
        }

        [Fact]
        public void TestToolBrowserParamFailArgName()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolbrowser", "cXmmand='cmd'" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--toolbrowser' is missing argument 'command'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestToolBrowserParamFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolbrowser" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1022301-user: parameter '--toolbrowser' has incorrect number of arguments; found 0 should be at least 1", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestToolHelpParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolhelp", "url='https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual/'" });

            Assert.True(rcParam.GetResult());
            Assert.Equal("https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual/", cmdLineParams.ToolHelpUrl);
        }

        [Fact]
        public void TestToolHelpParamFailArgName()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolhelp", "urX='explorer.exe'" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--toolhelp' is missing argument 'url'", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolhelp", "url=" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--toolhelp' is missing argument 'url'", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestToolHelpParamFailArgValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolhelp", "url=abc" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020322-user: parameter '--toolhelp' has bad argument; value 'abc' is invalid", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestToolHelpParamFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolhelp" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1022401-user: parameter '--toolhelp' has incorrect number of arguments; found 0 should be at least 1", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestToolSearchParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolsearch", "url='https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual/'" });

            Assert.True(rcParam.GetResult());
            Assert.Equal("https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual/", cmdLineParams.ToolSearchUrl);
        }

        [Fact]
        public void TestToolSearchParamFailArgName()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolsearch", "urX='explorer.exe'" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--toolsearch' is missing argument 'url'", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolsearch", "url=" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--toolsearch' is missing argument 'url'", rcParam.GetErrorTechMsg());

        }

        [Fact]
        public void TestToolSearchParamFailArgValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolsearch", "url=abc" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020323-user: parameter '--toolsearch' has bad argument; value 'abc' is invalid", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestToolSearchParamFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolsearch" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1022501-user: parameter '--toolsearch' has incorrect number of arguments; found 0 should be at least 1", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestToolThesaurusParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolthesaurus", "url='https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual/'" });

            Assert.True(rcParam.GetResult());
            Assert.Equal("https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual/", cmdLineParams.ToolThesaurusUrl);
        }

        [Fact]
        public void TestToolThesaurusParamFailArgName()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolthesaurus", "urX='explorer.exe'" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--toolthesaurus' is missing argument 'url'", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolthesaurus", "url=" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--toolthesaurus' is missing argument 'url'", rcParam.GetErrorTechMsg());

        }

        [Fact]
        public void TestToolThesaurusParamFailArgValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolthesaurus", "url=abc" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020324-user: parameter '--toolthesaurus' has bad argument; value 'abc' is invalid", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestToolThesaurusParamFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolthesaurus" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1022601-user: parameter '--toolthesaurus' has incorrect number of arguments; found 0 should be at least 1", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestToolSpellParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolspell", "url='https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual/'" });

            Assert.True(rcParam.GetResult());
            Assert.Equal("https://github.com/wpqs/GenDotNetCmdApps/wiki/KLineEd-User-Manual/", cmdLineParams.ToolSpellUrl);
        }

        [Fact]
        public void TestToolSpellFailArgName()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolspell", "urX='explorer.exe'" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--toolspell' is missing argument 'url'", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolspell", "url=" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--toolspell' is missing argument 'url'", rcParam.GetErrorTechMsg());

        }

        [Fact]
        public void TestToolSpellParamFailArgValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolspell", "url=abc" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020325-user: parameter '--toolspell' has bad argument; value 'abc' is invalid", rcParam.GetErrorTechMsg());
        }


        [Fact]
        public void TestToolSpellParamFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolspell" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1022701-user: parameter '--toolspell' has incorrect number of arguments; found 0 should be at least 1", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestToolSvnParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolsvn", $"username={CmdLineParamsApp.ArgToolSvnUserDefault}", $"password={CmdLineParamsApp.ArgToolSvnPasswordDefault}", $"url={CmdLineParamsApp.ArgToolSvnUrlDefault}" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(CmdLineParamsApp.ArgToolSvnUserDefault, cmdLineParams.ToolSvnUser);
            Assert.Equal(CmdLineParamsApp.ArgToolSvnPasswordDefault, cmdLineParams.ToolSvnPasswordKey);
            Assert.Equal(CmdLineParamsApp.ArgToolSvnUrlDefault, cmdLineParams.ToolSvnUrl);
        }

        [Fact]
        public void TestToolSvnParamFailValueBad()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolsvn", $"username=''" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--toolsvn' is missing argument 'username'", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolsvn", $"password=''" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023904-user: parameter '--toolsvn' is missing argument 'password'", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolsvn", $"url=abc" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020328-user: parameter '--toolsvn' has bad argument; value 'abc' is invalid", rcParam.GetErrorTechMsg());

        }

        [Fact]
        public void TestToolSvnParamFailNameBad()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolsvn", $"userXame={CmdLineParamsApp.ArgToolSvnUserDefault}", $"password={CmdLineParamsApp.ArgToolSvnPasswordDefault}", $"url={CmdLineParamsApp.ArgToolSvnUrlDefault}" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1022802-user: parameter '--toolsvn' has invalid argument(s); processed 2 but found 3", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolsvn", $"userXame={CmdLineParamsApp.ArgToolSvnUserDefault}", $"pXssword={CmdLineParamsApp.ArgToolSvnPasswordDefault}", $"url={CmdLineParamsApp.ArgToolSvnUrlDefault}" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1022802-user: parameter '--toolsvn' has invalid argument(s); processed 1 but found 3", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolsvn", $"userXame={CmdLineParamsApp.ArgToolSvnUserDefault}", $"pXssword={CmdLineParamsApp.ArgToolSvnPasswordDefault}", $"uXl={CmdLineParamsApp.ArgToolSvnUrlDefault}" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1022802-user: parameter '--toolsvn' has invalid argument(s); processed 0 but found 3", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestToolSvnParamFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--toolsvn", $"username={CmdLineParamsApp.ArgToolSvnUserDefault}", $"password={CmdLineParamsApp.ArgToolSvnPasswordDefault}", $"url={CmdLineParamsApp.ArgToolSvnUrlDefault}", "test=x" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1022801-user: parameter '--toolsvn' has incorrect number of arguments; found 4 should be in the range 0-3", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestEditParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--edit", "Test.ksx" });

            Assert.True(rcParam.GetResult());
            Assert.Equal(Environment.NewLine, cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestEditParamNoArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--edit" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021801-user: parameter '--edit' has incorrect number of arguments; found 0", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--edit", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestEditParamExtraArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--edit", "Test.ksx", "extra" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021801-user: parameter '--edit' has incorrect number of arguments; found 2", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--edit", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestEditSettingsParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--edit", "Test.ksx", "--settings display=no file='KLineEdCmdApp.json' update=yes" });
            Assert.True(rcParam.GetResult());
            Assert.Equal(CmdLineParamsApp.BoolValue.No, cmdLineParams.SettingsDisplay);
            Assert.Equal("KLineEdCmdApp.json", cmdLineParams.SettingsPathFileName);
            Assert.Equal(CmdLineParamsApp.BoolValue.Yes, cmdLineParams.SettingsUpdate);
        }

        [Fact]
        public void TestGetArgValue()
        {
           Assert.Equal("off", CmdLineParamsApp.GetArgNameValue("--settings", "display", "--settings display=off file='KLineEdCmdApp.json' update=yes").GetResult());
           Assert.Equal("KLineEdCmdApp.json", CmdLineParamsApp.GetArgNameValue("--settings", "file", "--settings display=off file='KLineEdCmdApp.json' update=yes").GetResult());
           Assert.Equal("KLineEdCmdApp.json", CmdLineParamsApp.GetArgNameValue("--settings", "file", "--settings display=off file=KLineEdCmdApp.json update=yes").GetResult());
           Assert.Equal("KLineEdCmdApp.json'", CmdLineParamsApp.GetArgNameValue("--settings", "file", "--settings display=off file=KLineEdCmdApp.json' update=yes").GetResult());
           Assert.Equal("c:\\hello \\KLineEdCmdApp.json", CmdLineParamsApp.GetArgNameValue("--settings", "file", "--settings display=off file='c:\\hello \\KLineEdCmdApp.json' update=yes").GetResult());
           Assert.Equal("yes", CmdLineParamsApp.GetArgNameValue("--settings", "update", "--settings display=off file='KLineEdCmdApp.json' update=yes").GetResult());
        }

        [Fact]
        public void TestGetArgValueReorder()
        {
            Assert.Equal("off", CmdLineParamsApp.GetArgNameValue("--settings", "display", "--settings file='KLineEdCmdApp.json' update=yes display=off").GetResult());
            Assert.Equal("KLineEdCmdApp.json", CmdLineParamsApp.GetArgNameValue("--settings", "file", "--settings file='KLineEdCmdApp.json' update=yes display=off").GetResult());
            Assert.Equal("c:\\hello = \\KLineEdCmdApp.json", CmdLineParamsApp.GetArgNameValue("--settings", "file", "--settings file='c:\\hello = \\KLineEdCmdApp.json' update=yes display=off").GetResult());
            Assert.Equal("yes", CmdLineParamsApp.GetArgNameValue("--settings", "update", "--settings file='KLineEdCmdApp.json' update=yes display=off").GetResult());
        }

        [Fact]
        public void TestGetArgValueMultipleSpaceBeforeName()
        {
            Assert.Equal("KLineEdCmdApp.json", CmdLineParamsApp.GetArgNameValue("--settings", "file", "--settings  DisplaY=off   file='KLineEdCmdApp.json' update=yes").GetResult());
        }

        [Fact]
        public void TestGetArgValueNameCaseDifference()
        {
            var rc = CmdLineParamsApp.GetArgNameValue("--settings", "display", "--settings Display=off file='KLineEdCmdApp.json' update=yes");
            Assert.Equal("error 1023904-user: parameter '--settings' is missing argument 'display'", rc.GetErrorTechMsg());
            Assert.False(rc.IsSuccess());
        }

        [Fact]
        public void TestGetArgValueSmallestNameValue()
        {
            Assert.Equal("x", CmdLineParamsApp.GetArgNameValue("--settings", "a", "--settings display=off file='KLineEdCmdApp.json' a=x").GetResult());
            Assert.Equal("x", CmdLineParamsApp.GetArgNameValue("--settings", "a", "--settings a=x display=off file='KLineEdCmdApp.json'").GetResult());
        }

        [Fact]
        public void TestGetArgValueDuplicateName()
        {
            var rc = CmdLineParamsApp.GetArgNameValue("--settings", "display", "--settings display=off file='KLineEdCmdApp.json' update=yes display=off");
            Assert.False(rc.IsSuccess());
            Assert.Equal("error 1023903-user: parameter '--settings' has duplicate argument 'display'", rc.GetErrorTechMsg());
        }

        [Fact]
        public void TestGetArgValueNameDuplicatedInValue()
        {
            var rc = CmdLineParamsApp.GetArgNameValue("--settings", "display", "--settings display=off file=display update=yes");
            Assert.True(rc.IsSuccess());
            Assert.Equal("display", CmdLineParamsApp.GetArgNameValue("--settings", "file", "--settings file=display update=yes display=off").GetResult());
        }

        [Fact]
        public void TestGetArgNameParameterNotFound()
        {
            var rc = CmdLineParamsApp.GetArgNameValue("--seXtings", "display", "--settings display=off file='KLineEdCmdApp.json' update=yes");
            Assert.False(rc.IsSuccess());
            Assert.Contains("error 1023902-program: parameter '--seXtings' not found", rc.GetErrorTechMsg());
        }


        [Fact]
        public void TestGetArgNameNameNotFoundMandatory()
        {
            var rc = CmdLineParamsApp.GetArgNameValue("--settings", "display", "--settings file='KLineEdCmdApp.json' update=yes", true);
            Assert.False(rc.IsSuccess());
            Assert.Equal("error 1023904-user: parameter '--settings' is missing argument 'display'", rc.GetErrorTechMsg());
        }

        [Fact]
        public void TestGetArgNameArgNotFoundOptional()
        {
            var rc = CmdLineParamsApp.GetArgNameValue("--settings", "display", "--settings file='KLineEdCmdApp.json' update=yes", false);
            Assert.True(rc.IsSuccess());
            Assert.Null(rc.GetResult());
        }

        [Fact]
        public void TestGetArgNameValueNotFound()
        {
            var rc = CmdLineParamsApp.GetArgNameValue("--settings", "display", "--settings display file='KLineEdCmdApp.json' update=yes");
            Assert.False(rc.IsSuccess());
            Assert.Equal("error 1023904-user: parameter '--settings' is missing argument 'display'", rc.GetErrorTechMsg());
        }

        [Fact]
        public void TestGetArgNameEmptyQuotes()
        {
            var rc = CmdLineParamsApp.GetArgNameValue("--settings", "file", "--settings display=off file='' update=yes");
            Assert.False(rc.IsSuccess());
            Assert.Equal("error 1023904-user: parameter '--settings' is missing argument 'file'", rc.GetErrorTechMsg());
        }

        [Fact]
        public void TestGetArgNameNoClosingQuote()
        {
            var rc = CmdLineParamsApp.GetArgNameValue("--settings", "file", "--settings display=off file='hello  update=yes");
            Assert.False(rc.IsSuccess());
            Assert.Equal("error 1023904-user: parameter '--settings' is missing argument 'file'", rc.GetErrorTechMsg());
        }
    }
}
