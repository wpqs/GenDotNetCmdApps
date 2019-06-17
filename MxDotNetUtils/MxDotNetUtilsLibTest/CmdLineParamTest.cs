using System;
using MxDotNetUtilsCmdApp;
using Xunit;

namespace AwsDotNetS3LargeFileXferTest
{
    public class CmdLineParamTest
    {
        public static readonly string[] StdParamsHelp = { "--Help" };

        private readonly string[] StdParamsEndSpace = { "--operation ", "upload", "c:\\users\\wills\\largefile.bin", "--bucketregion  ", "us-west-1", "--bucketname ", "test" };
        private readonly string[] StdParamsStartSpace = { " --operation", "upload", "c:\\users\\wills\\largefile.bin", " --bucketregion", "us-west-1", " --bucketname ", "test" };
        private readonly string[] StdParamsMixedCase = { "--oPeraTion", "upload", "c:\\users\\wills\\largefile.bin", "--bUcketRegion", "us-west-1", "--BucketNaMe", "test" };

        private readonly string[] StdParamsArgEndSpace = { "--operation", "upload ", "c:\\users\\wills\\largefile.bin  ", "--bucketregion", "us-west-1  ", "--bucketname", "test  " };
        private readonly string[] StdParamsArgStartSpace = { "--operation", " upload", " c:\\users\\wills\\largefile.bin", "--bucketregion", " us-west-1", "--bucketname", " test" };
        private readonly string[] StdParamsArgFirstUpper = { "--operation", "Upload", "C:\\users\\wills\\largefile.bin", "--bucketregion", "Us-west-1", "--bucketname", "Test" };

        private readonly string[] StdParamsFilenameNoSpaceNoQuotesEndSpace = { "--operation", "upload", "c:\\users\\wills\\largefile.bin ", "--bucketregion", "us-west-1", "--bucketname", "test" };
        private readonly string[] StdParamsFilenameNoSpaceQuotes = { "--operation", "upload", "'c:\\users\\wills\\largefile.bin'", "--bucketregion", "us-west-1", "--bucketname", "test" };
        private readonly string[] StdParamsFilenameSpaceNoQuotes = { "--operation", "upload", "c:\\users\\wills\\dot space\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test" };
        private readonly string[] StdParamsFilenameSpaceQuotes = { "--operation", "upload", "'c:\\users\\wills\\dot space\\largefile.bin'", "--bucketregion", "us-west-1", "--bucketname", "test" };
        private readonly string[] StdParamsFilenameSpaceQuotesEndSpace = { "--operation", "upload", "'c:\\users\\wills\\dot space\\largefile.bin' ", "--bucketregion", "us-west-1", "--bucketname", "test" };
        private readonly string[] StdParamsFilenameSpaceQuotesStartSpace = { "--operation", "upload", " 'c:\\users\\wills\\dot space\\largefile.bin'", "--bucketregion", "us-west-1", "--bucketname", "test" };
        private readonly string[] StdParamsFilenameSpaceQuotesStartEndSpace = { "--operation", "upload", " 'c:\\users\\wills\\dot space\\largefile.bin'  ", "--bucketregion", "us-west-1", "--bucketname", "test" };
        private readonly string[] StdParamsFilenameEmptyQuotes = { "--operation", "upload", "''", "--bucketregion", "us-west-1", "--bucketname", "test" };
        private readonly string[] StdParamsFilenameNoClosingQuote = { "--operation", "upload", "'c:\\users\\wills\\dot space\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test" };
        private readonly string[] StdParamsFilenameJustOpeningQuote = { "--operation", "upload", "'", "--bucketregion", "us-west-1", "--bucketname", "test" };

        public static readonly string[] StdParamsUpload = { "--operation", "upload", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test" };


        [Fact]
        public void TestNoParam()
        {
            var cmdLine = new CmdLineParamsApp();

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: Command line has no parameters", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestUnknownParam()
        {
            var cmdLine = new CmdLineParamsApp(new string[] { "--xhelp" });

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: Unknown parameter", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestHelpParam()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsHelp);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Help request", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestHelpParamUnexpectedArg()
        {
            var cmdLine = new CmdLineParamsApp(new string[] { "--help", "x" });

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: parameter --help has incorrect number of arguments; found 1 should be none", cmdLine.GetErrorMsg());
        }


        [Fact]
        public void TestErrorMsg()
        {

            var cmdLine = new CmdLineParamsApp(StdParamsUpload);

            Assert.False(cmdLine.IsError);
            Assert.Null(cmdLine.GetErrorMsg());

            cmdLine.SetErrorMsg("test");
            Assert.True(cmdLine.IsError);
            Assert.Equal("test", cmdLine.GetErrorMsg());

            cmdLine.SetErrorMsg("xxx");
            Assert.True(cmdLine.IsError);
            Assert.Equal("test", cmdLine.GetErrorMsg());  //first msg is retained

            cmdLine.SetErrorMsg("xxx", true);
            Assert.True(cmdLine.IsError);
            Assert.Equal("xxx", cmdLine.GetErrorMsg());  //first msg is NOT retained
        }

        [Fact]
        public void TestStdParamsEndSpace()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsEndSpace);

            Assert.False(cmdLine.IsError);

            Assert.Equal("us-west-1", cmdLine.BucketRegion);
            Assert.Equal("test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Upload, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\largefile.bin", cmdLine.InputFile);

        }

        [Fact]
        public void TestStdParamsStartSpace()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsStartSpace);

            Assert.False(cmdLine.IsError);

            Assert.Equal("us-west-1", cmdLine.BucketRegion);
            Assert.Equal("test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Upload, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\largefile.bin", cmdLine.InputFile);
        }

        [Fact]
        public void TestStdParamsMixedCase()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsMixedCase); //parameter name is always converted to lowercase, but args retain their case as it may be significant

            Assert.False(cmdLine.IsError);

            Assert.Equal("us-west-1", cmdLine.BucketRegion);
            Assert.Equal("test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Upload, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\largefile.bin", cmdLine.InputFile);
        }

        [Fact]
        public void TestStdParamsArgEndSpace()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsArgEndSpace);

            Assert.False(cmdLine.IsError);

            Assert.Equal("us-west-1", cmdLine.BucketRegion);
            Assert.Equal("test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Upload, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\largefile.bin", cmdLine.InputFile);
        }

        [Fact]
        public void TestStdParamsArgStartSpace()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsArgStartSpace);

            Assert.False(cmdLine.IsError);

            Assert.Equal("us-west-1", cmdLine.BucketRegion);
            Assert.Equal("test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Upload, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\largefile.bin", cmdLine.InputFile);
        }

        [Fact]
        public void TestStdParamsArgFirstUpper()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsArgFirstUpper); //args retain their case as it may be significant

            Assert.False(cmdLine.IsError);

            Assert.Equal("Us-west-1", cmdLine.BucketRegion);
            Assert.Equal("Test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Upload, cmdLine.Op);
            Assert.Equal("C:\\users\\wills\\largefile.bin", cmdLine.InputFile);
        }

        [Fact]
        public void TestStdParamsFilenameNoSpaceNoQuotesEndSpace()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsFilenameNoSpaceNoQuotesEndSpace);

            Assert.False(cmdLine.IsError);

            Assert.Equal("us-west-1", cmdLine.BucketRegion);
            Assert.Equal("test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Upload, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\largefile.bin", cmdLine.InputFile);
        }

        [Fact]
        public void TestStdParamsFilenameNoSpaceQuotes()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsFilenameNoSpaceQuotes);

            Assert.False(cmdLine.IsError);

            Assert.Equal("us-west-1", cmdLine.BucketRegion);
            Assert.Equal("test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Upload, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\largefile.bin", cmdLine.InputFile);
        }

        [Fact]
        public void TestStdParamsFilenameSpaceNoQuotes()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsFilenameSpaceNoQuotes);

            Assert.True(cmdLine.IsError);   //as filename is not in quotes it looks like three arguments rather than two
            Assert.StartsWith("Error: parameter --operation upload has incorrect number of arguments; found 3 should be 2", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsFilenameSpaceQuotes()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsFilenameSpaceQuotes);

            Assert.False(cmdLine.IsError);

            Assert.Equal("us-west-1", cmdLine.BucketRegion);
            Assert.Equal("test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Upload, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\dot space\\largefile.bin", cmdLine.InputFile);
        }

        [Fact]
        public void TestStdParamsFilenameSpaceQuotesEndSpace()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsFilenameSpaceQuotesEndSpace);

            Assert.False(cmdLine.IsError);

            Assert.Equal("us-west-1", cmdLine.BucketRegion);
            Assert.Equal("test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Upload, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\dot space\\largefile.bin", cmdLine.InputFile);
        }

        [Fact]
        public void TestStdParamsFilenameSpaceQuotesStartSpace()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsFilenameSpaceQuotesStartSpace);

            Assert.False(cmdLine.IsError);

            Assert.Equal("us-west-1", cmdLine.BucketRegion);
            Assert.Equal("test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Upload, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\dot space\\largefile.bin", cmdLine.InputFile);
        }

        [Fact]
        public void TestStdParamsFilenameSpaceQuotesStartEndSpace()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsFilenameSpaceQuotesStartEndSpace);

            Assert.False(cmdLine.IsError);

            Assert.Equal("us-west-1", cmdLine.BucketRegion);
            Assert.Equal("test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Upload, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\dot space\\largefile.bin", cmdLine.InputFile);
        }

        [Fact]
        public void TestStdParamsFilenameEmptyQuotes()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsFilenameEmptyQuotes);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: Invalid argument 2 in \"--operation upload ''\" - nothing between the quotes", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsFilenameNoClosingQuote()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsFilenameNoClosingQuote);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: Invalid argument 2 in \"--operation upload 'c:\\users\\wills\\dot space\\largefile.bin\" - no closing quote character", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsFilenameJustOpeningQuote()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsFilenameJustOpeningQuote);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: Invalid argument 2 in \"--operation upload '\" - no closing quote character", cmdLine.GetErrorMsg());
        }
    }
}
