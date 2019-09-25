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
            Assert.StartsWith("error 1023101-user: the parameter --xhelp is invalid", rcParam.GetErrorUserMsg());
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
            Assert.StartsWith("error 1020501-user: parameter --help has incorrect number of arguments; found 1", rcParam.GetErrorUserMsg());
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
            Assert.StartsWith("error 1020601-user: parameter --export has incorrect number of arguments; found 0", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--export", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestExportParamNoSecondArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--export", "test.txt" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1020601-user: parameter --export has incorrect number of arguments; found 1", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--export", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestExportParamExtraArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--export", "Edit.ksx", "Export.txt", "extra" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1020601-user: parameter --export has incorrect number of arguments; found 3", rcParam.GetErrorUserMsg());
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
            Assert.StartsWith("error 1020701-user: parameter --import has incorrect number of arguments; found 0", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--import", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestImportParamNoSecondArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--import", "test.ksx" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1020701-user: parameter --import has incorrect number of arguments; found 1", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--import", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestImportParamExtraArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new[] { "--import", "Edit.txt", "Export.ksx", "extra" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1020701-user: parameter --import has incorrect number of arguments; found 3", rcParam.GetErrorUserMsg());
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
            Assert.StartsWith("error 1020801-user: parameter --edit has incorrect number of arguments; found 0", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--edit", cmdLineParams.HelpHint);
        }

        [Fact]
        public void TestEditParamExtraArg()
        {
            var cmdLineParams = new CmdLineParamsApp();
            var rcParam = cmdLineParams.Initialise(new [] { "--edit", "Test.ksx", "extra" });

            Assert.False(rcParam.GetResult());
            Assert.StartsWith("error 1020801-user: parameter --edit has incorrect number of arguments; found 2", rcParam.GetErrorUserMsg());
            Assert.Contains($"Hint: retry using expected arguments for the parameter.{Environment.NewLine}--edit", cmdLineParams.HelpHint);
        }
    }
}
