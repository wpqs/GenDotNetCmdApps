using System;
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
            Assert.StartsWith("error 1021601-user: parameter --export has incorrect number of arguments; found 0", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--export", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestExportParamNoSecondArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--export", "test.txt" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021601-user: parameter --export has incorrect number of arguments; found 1", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--export", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestExportParamExtraArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--export", "Edit.ksx", "Export.txt", "extra" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021601-user: parameter --export has incorrect number of arguments; found 3", rcParam.GetErrorUserMsg());
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
            Assert.StartsWith("error 1021701-user: parameter --import has incorrect number of arguments; found 0", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--import", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestImportParamNoSecondArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--import", "test.ksx" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021701-user: parameter --import has incorrect number of arguments; found 1", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--import", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestImportParamExtraArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--import", "Edit.txt", "Export.ksx", "extra" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021701-user: parameter --import has incorrect number of arguments; found 3", rcParam.GetErrorUserMsg());
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
            Assert.Contains("error 1021904-user: parameter --settings has invalid argument(s); processed 0 but found 1", rcParam.GetErrorTechMsg());
        }
        [Fact]
        public void TestSettingsParamsDisplayValueFail()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--settings", "display=yXs" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1021902-user: parameter --settings argument display value is not 'yes' or 'no'; yXs", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestSettingsParamsUpdateValueFail()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--settings", "update=yXs" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1021903-user: parameter --settings argument update value is not 'yes' or 'no'; yXs", rcParam.GetErrorTechMsg());
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
            Assert.Contains("error 1022002-user: parameter --backgnd has invalid argument(s); processed 0 but found 1", rcParam.GetErrorTechMsg());
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
            Assert.Contains("error 1023904-user: parameter --cursor, arg size; name not found", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestCursorParamFailInvalidValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--cursor", "size=X" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023001-user: parameter '--cursor' argument 'size' is invalid. It must be a number between 1 and 100", rcParam.GetErrorTechMsg());
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
            Assert.Contains("error 1023103-user: parameter '--display' argument 'cols' value X is invalid. It must be a number between 25 and 250", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestDisplayParamFailRowValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--display", "rows=X" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023102-user: parameter '--display' argument 'rows' value X is invalid. It must be a number between 5 and 50", rcParam.GetErrorTechMsg());
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
            Assert.Contains("error 1023904-user: parameter --limits, arg scrollback; name not found", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestLimitsParamScrollBackFailBadValue()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--limits", "scrollback=X" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023202-user: parameter '--limits' argument 'scrollback' value X is invalid. It must be a number between 0 and 90000", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestLimitsParamScrollBackFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--limits", "scrollback=1", "extra=x" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023201-user: parameter --limits has incorrect number of arguments; found 2 should be 1", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--limits" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023201-user: parameter --limits has incorrect number of arguments; found 0 should be 1", rcParam.GetErrorTechMsg());
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
            Assert.Contains("error 1023302-user: parameter '--tabsize' value X is invalid. It must be a number between 1 and 25", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestTabSizeParamFailArgCount()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--tabsize" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023301-user: parameter --tabsize has incorrect number of arguments; found 0 should be 1", rcParam.GetErrorTechMsg());

            rcParam = cmdLineParams.Initialise(new[] { "--help", "--tabsize", "12", "9" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1023301-user: parameter --tabsize has incorrect number of arguments; found 2 should be 1", rcParam.GetErrorTechMsg());

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
            Assert.StartsWith("error 1021801-user: parameter --edit has incorrect number of arguments; found 0", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter, updating the settings file if necessary.{Environment.NewLine}--edit", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestEditParamExtraArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--edit", "Test.ksx", "extra" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021801-user: parameter --edit has incorrect number of arguments; found 2", rcParam.GetErrorUserMsg());
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
            Assert.Equal("error 1023904-user: parameter --settings, arg display; name not found", rc.GetErrorTechMsg());
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
            Assert.Equal("error 1023903-user: parameter --settings, arg display; duplicate name found", rc.GetErrorTechMsg());
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
            Assert.Contains("error 1023902-program: parameter --seXtings not found", rc.GetErrorTechMsg());
        }


        [Fact]
        public void TestGetArgNameNameNotFoundMandatory()
        {
            var rc = CmdLineParamsApp.GetArgNameValue("--settings", "display", "--settings file='KLineEdCmdApp.json' update=yes", true);
            Assert.False(rc.IsSuccess());
            Assert.Equal("error 1023904-user: parameter --settings, arg display; name not found", rc.GetErrorTechMsg());
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
            Assert.Equal("error 1023904-user: parameter --settings, arg display; value not found", rc.GetErrorTechMsg());
        }

        [Fact]
        public void TestGetArgNameEmptyQuotes()
        {
            var rc = CmdLineParamsApp.GetArgNameValue("--settings", "file", "--settings display=off file='' update=yes");
            Assert.False(rc.IsSuccess());
            Assert.Equal("error 1023904-user: parameter --settings, arg file; value not found", rc.GetErrorTechMsg());
        }

        [Fact]
        public void TestGetArgNameNoClosingQuote()
        {
            var rc = CmdLineParamsApp.GetArgNameValue("--settings", "file", "--settings display=off file='hello  update=yes");
            Assert.False(rc.IsSuccess());
            Assert.Equal("error 1023904-user: parameter --settings, arg file; value not found", rc.GetErrorTechMsg());
        }
    }
}
