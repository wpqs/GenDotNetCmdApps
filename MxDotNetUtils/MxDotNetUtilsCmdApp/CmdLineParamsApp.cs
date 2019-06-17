using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using MxDotNetUtilsLib;

namespace MxDotNetUtilsCmdApp
{
    public class CmdLineParamsApp : CmdLineParams
    {
        public static readonly string HelpParam = "--help";
        public static readonly string OpParam = "--operation";
        public static readonly string OpArgUpload = "upload";
        public static readonly string OpArgDownload = "download";
        public static readonly string OpArgOverwrite = "overwrite";
        public static readonly bool   OpOverwriteValueDefault = false;
        public static readonly string BucketNameParam = "--bucketname";
        public static readonly string BucketRegionParam = "--bucketregion";
        public static readonly string ThreadsParam = "--threads";
        public static readonly int    ThreadsValueDefault = 10;
        public static readonly int    ThreadsValueMin = 1;
        public static readonly int    ThreadsValueMax = 100;
        public static readonly string PartialSizeParam = "--partialsize";
        public static readonly long PartialSizeValueDefault = 16777216; //2**24
        public static readonly long PartialSizeValueMin = 4096;         //2**12
        public static readonly long PartialSizeValueMax = 4294967296;   //2**32

        public OpMode Op { private set; get; }
        public string InputFile { private set; get; }
        public string OutputFile { private set; get; }
        public string BucketName { private set; get; }
        public string BucketRegion { private set; get; }
        public int Threads { private set; get; }
        public long PartialSize { private set; get; }
        public bool Overwrite { private set; get; }


        private enum Param
        {
            Help = 0,
            Op,
            BucketRegion,
            BucketName,
            Threads,
            PartialSize,
            Unknown
        }

        public enum OpMode
        {
            Upload = 0,
            Download,
            Unknown
        }

        public CmdLineParamsApp(string[] cmdLine = null) : base(cmdLine)
        {
        }

        protected override void SetDefaultValues()  //called from base class as values may be overwritten by values passed from cmdLine
        {
            IsError = false;
            Op = OpMode.Unknown;
            InputFile = null;
            OutputFile = null;
            BucketName = null;
            BucketRegion = null;
            Threads = ThreadsValueDefault;
            PartialSize = PartialSizeValueDefault;
            Overwrite = OpOverwriteValueDefault;
        }

        protected override bool ParamProc(string paramLine)
        {
            bool rc = false;

            if (paramLine == null)
                SetErrorMsg($"Error: Program bug - please report this problem with details: AwsDotNetS3LargeFileXferCmd {Program.GetVersion()} - ParamProc() paramLine=[null]");
            else
            {
                switch (GetParamType(paramLine))
                {
                    case Param.Help:
                    {
                        int argCnt = GetArgCount(paramLine);
                        if (argCnt != 0)
                            SetErrorMsg($"Error: parameter {HelpParam} has incorrect number of arguments; found {argCnt} should be none {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Help)}");
                        else
                        {
                            SetErrorMsg($"Help request:{Environment.NewLine}{GetParamHelp((int) Param.Help)}");
                        }
                    }
                        break;
                    case Param.Op:
                    {
                        int argCnt = GetArgCount(paramLine);
                        if (argCnt < 2)
                            SetErrorMsg($"Error: parameter {OpParam} has incorrect number of arguments; found {argCnt} should be 2 (or 3) {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                        else
                        {
                            var value = GetArgValue(paramLine, 1)?.ToLower() ?? "[not found]";
                            if (value == OpArgDownload)
                            {
                                if ((argCnt != 2) && (argCnt != 3))
                                    SetErrorMsg($"Error: parameter {OpParam}  {OpArgDownload} has incorrect number of arguments; found {argCnt} should be 2 (or 3){Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                                else
                                {
                                    OutputFile = GetArgValue(paramLine, 2);
                                    if (string.IsNullOrEmpty(OutputFile))
                                        SetErrorMsg($"Error: parameter {OpParam} {OpArgDownload} second argument is empty or missing {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                                    else
                                    {
                                        var overwrite = GetArgValue(paramLine, 3);
                                        if (string.IsNullOrEmpty(overwrite) == false)
                                        {
                                            if (overwrite != OpArgOverwrite)
                                                SetErrorMsg($"Error: parameter {OpParam} {OpArgDownload} third argument is {overwrite} not {OpArgOverwrite}  {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                                            else
                                                Overwrite = true;
                                        }

                                        Op = OpMode.Download;
                                        rc = true;
                                    }
                                }
                            }
                            else if (value == OpArgUpload)
                            {
                                if (argCnt != 2)
                                    SetErrorMsg($"Error: parameter {OpParam} {OpArgUpload} has incorrect number of arguments; found {argCnt} should be 2 {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                                else
                                {
                                    InputFile = GetArgValue(paramLine, 2);
                                    if (string.IsNullOrEmpty(InputFile))
                                        SetErrorMsg($"Error: parameter {OpParam} {OpArgUpload} second argument is empty or missing {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                                    else
                                    {
                                        Op = OpMode.Upload;
                                        rc = true;
                                    }
                                }
                            }
                            else
                                SetErrorMsg($"Error: parameter {OpParam} first argument is invalid; {value} {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                        }
                    }
                        break;
                    case Param.BucketRegion:
                    {
                        int argCnt = GetArgCount(paramLine);
                        if (argCnt != 1)
                            SetErrorMsg($"Error: parameter {BucketRegionParam} has incorrect number of arguments; found {argCnt} should be 1 {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.BucketRegion)}");
                        else
                        {
                            BucketRegion = GetArgValue(paramLine, 1);
                            if (string.IsNullOrEmpty(BucketRegion))
                                SetErrorMsg($"Error: parameter {BucketRegionParam} argument value is null or empty {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.BucketRegion)}");
                            else
                                rc = true;
                        }
                    }
                        break;
                    case Param.BucketName:
                    {
                        int argCnt = GetArgCount(paramLine);
                        if (argCnt != 1)
                            SetErrorMsg($"Error: parameter {BucketNameParam} has incorrect number of arguments; found {argCnt} should be 1 {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.BucketName)}");
                        else
                        {
                            BucketName = GetArgValue(paramLine, 1);
                            if (string.IsNullOrEmpty(BucketName))
                                SetErrorMsg($"Error: parameter {BucketNameParam} argument value is null or empty {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.BucketName)}");
                            else
                                rc = true;
                        }
                    }
                        break;
                    case Param.Threads:
                    {
                        int argCnt = GetArgCount(paramLine);
                        if (argCnt != 1)
                            SetErrorMsg($"Error: parameter {ThreadsParam} has incorrect number of arguments; found {argCnt} should be 1 {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Threads)}");
                        else
                        {
                            var threadsCount = GetArgValue(paramLine, 1);
                            if (string.IsNullOrEmpty(threadsCount))
                                SetErrorMsg($"Error: parameter {ThreadsParam} argument value is null or empty {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Threads)}");
                            else
                            {
                                int threadCnt = 0;
                                if (Int32.TryParse(threadsCount, out threadCnt) == false)
                                    SetErrorMsg($"Error: parameter {ThreadsParam} argument value {threadsCount} is not a valid number {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.Threads)}");
                                else
                                {
                                    Threads = threadCnt;
                                    rc = true;
                                }
                            }
                        }
                    }
                        break;
                    case Param.PartialSize:
                    {
                        int argCnt = GetArgCount(paramLine);
                        if (argCnt != 1)
                            SetErrorMsg($"Error: parameter {PartialSizeParam} has incorrect number of arguments; found {argCnt} should be 1 {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.PartialSize)}");
                        else
                        {
                            var partialSize = GetArgValue(paramLine, 1);
                            if (string.IsNullOrEmpty(partialSize))
                                SetErrorMsg($"Error: parameter {PartialSizeParam} argument value is null or empty {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.PartialSize)}");
                            else
                            {
                                long size = 0L;
                                if (long.TryParse(partialSize, out size) == false)
                                    SetErrorMsg($"Error: parameter {PartialSizeParam} argument value {partialSize} is not a valid number {Environment.NewLine}\"{paramLine}\" {Environment.NewLine}{GetParamHelp((int) Param.PartialSize)}");
                                else
                                {
                                    PartialSize = size;
                                    rc = true;
                                }
                            }
                        }
                    }
                    break;
                    default: //case Param.Unknown:
                    {
                        SetErrorMsg($"Error: Unknown parameter {paramLine}{Environment.NewLine}{GetParamHelp()}");
                    }
                        break;
                }
            }

            return rc;
        }

        protected override void ValidateParams()
        {
            IsError = false;

            if ((Threads < ThreadsValueMin) || (Threads > ThreadsValueMax))
                SetErrorMsg($"Error: {ThreadsParam} value {Threads.ToString()} is invalid {Environment.NewLine}{GetParamHelp((int)Param.Threads)}");
            else
            {
                if ((PartialSize < PartialSizeValueMin) || (PartialSize > PartialSizeValueMax))
                    SetErrorMsg($"Error: {PartialSizeParam} value {PartialSize.ToString()} is invalid {Environment.NewLine}{GetParamHelp((int)Param.PartialSize)}");
                else
                {
                    if (string.IsNullOrEmpty(BucketRegion))
                        SetErrorMsg($"Error: Missing {BucketRegionParam} parameter {Environment.NewLine}{GetParamHelp((int) Param.BucketRegion)}");
                    else
                    {
                        if (string.IsNullOrEmpty(BucketName))
                            SetErrorMsg($"Error: Missing {BucketNameParam} parameter {Environment.NewLine}{GetParamHelp((int) Param.BucketName)}");
                        else
                        {

                            if (Op == OpMode.Download)
                            {
                                if (string.IsNullOrEmpty(OutputFile))
                                    SetErrorMsg($"Error: Filename argument missing {Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                            }
                            else if (Op == OpMode.Upload)
                            {
                                if (string.IsNullOrEmpty(InputFile))
                                    SetErrorMsg($"Error: Filename argument missing {Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                            }
                            else
                            {
                                SetErrorMsg($"Error: Missing {OpParam} parameter {Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                            }
                        }
                    }
                }
            }
        }
        protected override string GetParamHelp(int paramId = 0)
        {
            string rc = null;

            Param help = (Param)paramId;
            if (help == Param.Help)
            {
                rc = $"{Environment.NewLine}Hint: retry using program's expected parameters and their arguments which are:";
                rc += Environment.NewLine;
                rc += $"{BucketRegionParam} us-east-1 ";
                rc += $"{BucketNameParam} name ";
                rc += $"{OpParam} [{OpArgUpload} 'drive:path\\filename'";
                rc += $" | {OpArgDownload} 'drive:path\\filename' ({OpArgOverwrite})]";
                rc += Environment.NewLine;
                rc += $"({ThreadsParam} {ThreadsValueDefault.ToString()} <min {ThreadsValueMin} max {ThreadsValueMax}>) ";
                rc += $"({PartialSizeParam} {PartialSizeValueDefault.ToString()} <min {PartialSizeValueMin} max {PartialSizeValueMax}> )";
                rc += GetHelpNotes();

            }
            else if (help == Param.Op)
            {
                rc = $"{Environment.NewLine}Hint: retry using expected arguments for the parameter.{Environment.NewLine}";
                rc += $"{OpParam} [{OpArgUpload} 'drive:path\\filename' | {OpArgDownload} 'drive:path\\filename']";
                rc += GetHelpNotes();
            }
            else if (help == Param.BucketRegion)
            {
                rc = $"{Environment.NewLine}Hint: retry using expected argument for the parameter.{Environment.NewLine}";
                rc += $"{BucketRegionParam} us-east-1";
                rc += GetHelpNotes();
            }
            else if (help == Param.BucketName)
            {
                rc = $"{Environment.NewLine}Hint: retry using expected argument for the parameter.{Environment.NewLine}";
                rc += $"{BucketNameParam} name";
                rc += GetHelpNotes();
            }
            else if (help == Param.Threads)
            {
                rc = $"{Environment.NewLine}Hint: retry using expected argument for the parameter.{Environment.NewLine}";
                rc += $"{ThreadsParam} {ThreadsValueDefault} <min {ThreadsValueMin} max {ThreadsValueMax}>";
                rc += GetHelpNotes();
            }
            else if (help == Param.PartialSize)
            {
                rc = $"{Environment.NewLine}Hint: retry using expected argument for the parameter.{Environment.NewLine}";
                rc += $"{PartialSizeParam} {PartialSizeValueDefault} <min {PartialSizeValueMin} max {PartialSizeValueMax}>";
                rc += GetHelpNotes();
            }
            else
            {
                rc = $"{Environment.NewLine}Error: Program bug - please report this problem with details: AwsDotNetS3LargeFileXferCmd {Program.GetVersion()} - GetParamHelp() paramId={paramId}";
            }
            return rc;
        }

        private Param GetParamType(string param)
        {
            Param rc = Param.Unknown;

            if (param == null)
                SetErrorMsg($"Error: Program bug - please report this problem with details: AwsDotNetS3LargeFileXferCmd {Program.GetVersion()} - GetParamType() param=[null]");
            else
            {
                var offset = param.IndexOf(spaceChar);
                var name = (offset == -1) ? param.Trim().ToLower() : param.Substring(0, offset).Trim().ToLower();
   
                if (name == OpParam)
                    rc = Param.Op;
                else if (name == BucketRegionParam)
                    rc = Param.BucketRegion;
                else if (name == BucketNameParam)
                    rc = Param.BucketName;
                else if (name == ThreadsParam)
                    rc = Param.Threads;
                else if (name == PartialSizeParam)
                    rc = Param.PartialSize;
                else if (name == HelpParam)
                    rc = Param.Help;
                else
                    rc = Param.Unknown;
            }
            return rc;
        }
    }
}
