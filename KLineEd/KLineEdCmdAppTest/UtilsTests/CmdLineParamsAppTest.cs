using System;
using KLineEdCmdApp.Utils;
using Xunit;

namespace KLineEdCmdAppTest.UtilsTests
{
    public class CmdLineParamsAppTest
    {
        public static readonly string[] StdParamsHelp = { "--Help" };

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
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--help", cmdLineParams.HelpHint);
        }

        //[Fact]
        //public void TestResetColoursParam()
        //{
        //    var cmdLineParams = new CmdLineParamsApp();
        //    var rcParam = cmdLineParams.Initialise(new [] { "--reset", "colours"}); 

        //    Assert.True(rcParam.GetResult());
        //    Assert.Equal(Environment.NewLine, cmdLineParams.HelpHint);
        //}

        //[Fact]
        //public void TestResetFactoryParam()
        //{
        //    var cmdLineParams = new CmdLineParamsApp();
        //    var rcParam = cmdLineParams.Initialise(new [] { "--reset", "factory-defaults" }); 

        //    Assert.True(rcParam.GetResult());
        //    Assert.Equal(Environment.NewLine, cmdLineParams.HelpHint);
        //}

        //[Fact]
        //public void TestResetFactoryParamWithSettingsParam()
        //{
        //    var cmdLineParams = new CmdLineParamsApp();
        //    var rcParam = cmdLineParams.Initialise(new [] { "--reset", "factory-defaults", "--settings", "KLineEdCmdApp.json" });

        //    Assert.True(rcParam.GetResult());
        //    Assert.Equal(Environment.NewLine, cmdLineParams.HelpHint);
        //}

        //[Fact]
        //public void TestResetFactoryParamWithSettingsUpdateParams()
        //{
        //    var cmdLineParams = new CmdLineParamsApp();
        //    var rcParam = cmdLineParams.Initialise(new [] { "--reset", "factory-defaults", "--settings", "KLineEdCmdApp.json", "update" });

        //    Assert.True(rcParam.GetResult());
        //    Assert.Equal(Environment.NewLine, cmdLineParams.HelpHint);
        //}

        //[Fact]
        //public void TestResetParamNoArg()
        //{
        //    var cmdLineParams = new CmdLineParamsApp();
        //    var rcParam = cmdLineParams.Initialise(new [] { "--reset" });

        //    Assert.False(rcParam.GetResult());
        //    Assert.StartsWith("error 1020601-user: parameter --reset has incorrect number of arguments; found 0", rcParam.GetErrorUserMsg());
        //    Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--reset", cmdLineParams.HelpHint);
        //}

        //[Fact]
        //public void TestResetParamUnknownArg()
        //{
        //    var cmdLineParams = new CmdLineParamsApp();
        //    var rcParam = cmdLineParams.Initialise(new [] { "--reset", "test" });

        //    Assert.False(rcParam.GetResult());
        //    Assert.StartsWith("error 1020602-user: parameter --reset has invalid argument; found test should be [colours | factory-defaults]", rcParam.GetErrorUserMsg());
        //    Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--reset", cmdLineParams.HelpHint);
        //}

        //[Fact]
        //public void TestResetParamExtraArg()
        //{
        //    var cmdLineParams = new CmdLineParamsApp();
        //    var rcParam = cmdLineParams.Initialise(new [] { "--reset", "factory-defaults", "KLineEdApp.json", "update", "extra" });

        //    Assert.False(rcParam.GetResult());
        //    Assert.StartsWith("error 1020601-user: parameter --reset has incorrect number of arguments; found 4", rcParam.GetErrorUserMsg());
        //    Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--reset", cmdLineParams.HelpHint);
        //}

        [Fact]
        public void TestExportParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--export", "Edit.ksx", "Export.txt" });

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
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--export", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestExportParamNoSecondArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--export", "test.txt" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021601-user: parameter --export has incorrect number of arguments; found 1", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--export", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestExportParamExtraArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--export", "Edit.ksx", "Export.txt", "extra" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021601-user: parameter --export has incorrect number of arguments; found 3", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--export", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestImportParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--import", "Import.txt", "Edit.ksx" });

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
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--import", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestImportParamNoSecondArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--import", "test.ksx" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021701-user: parameter --import has incorrect number of arguments; found 1", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--import", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestImportParamExtraArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--import", "Edit.txt", "Export.ksx", "extra" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021701-user: parameter --import has incorrect number of arguments; found 3", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--import", cmdLineParams.HelpHint);
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
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--edit", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestEditParamExtraArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--edit", "Test.ksx", "extra" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1021801-user: parameter --edit has incorrect number of arguments; found 2", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--edit", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestEditSettingsParam()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--edit", "Test.ksx", "--settings display=off file='KLineEdCmdApp.json' update=yes" });
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
