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
            Assert.Contains("error 1020306-user: Parameter --backgnd has a bad argument; value of 'cmds' is not a valid COLOR", rcParam.GetErrorTechMsg());
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
            Assert.Contains("error 1022101-user: parameter --foregnd has invalid argument(s); processed 0 but found 1", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestForeGndParamsValueFail()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--foregnd", "cmds=grXen" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020307-user: Parameter --foregnd has a bad argument; value of 'cmds' is not a valid COLOR", rcParam.GetErrorTechMsg());
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
            Assert.Contains("error 1020308-user: vol: 11 is invalid", rcParam.GetErrorTechMsg());
        }

        [Fact]
        public void TestAudiosParamsFailTooSmall()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--help", "--audio", "vol=-2" });

            Assert.False(rcParam.GetResult());
            Assert.Contains("error 1020308-user: vol: -2 is invalid", rcParam.GetErrorTechMsg());
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
