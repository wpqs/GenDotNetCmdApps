using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MxDotNetUtilsLib
{
    public abstract class CmdLineParams
    {
        protected static readonly char quoteChar = '\'';
        protected static readonly char spaceChar = ' ';
        protected abstract void SetDefaultValues();
        protected abstract bool ParamProc(string paramLine);
        protected abstract string GetParamHelp(int paramId = 0);
        protected abstract void ValidateParams();

        public bool IsError { get; protected set; }

        private string _errMsg;
        public string GetErrorMsg()  { return _errMsg; }
        public void SetErrorMsg(string msg, bool overwrite=false)
        {
            if ((IsError == false) || (overwrite == true))
                _errMsg = msg;      //only set first error
            IsError=true;
        }

        protected CmdLineParams(string[] args =null)
        {
            SetDefaultValues();

            if ((args == null) || (args.Length < 1))
                SetErrorMsg($"Error: Command line has no parameters {Environment.NewLine}{GetParamHelp()}");
            else
            {
                string cmdLine = "";
                foreach (var arg in args)
                    cmdLine += arg + " ";
                cmdLine = cmdLine.TrimEnd();

                var paramList = cmdLine.Split("--");
                foreach (var param in paramList)
                {
                    if (string.IsNullOrWhiteSpace(param) == false)
                    {
                        if (ParamProc("--" + param.TrimEnd()) == false)
                            break;
                    }
                }
                if (IsError == false)
                    ValidateParams();
            }
        }

        protected string GetArgValue(string paramLine, int argNumber=1)
        {
            string rc = null;
            if (paramLine == null)
                SetErrorMsg($"Error: Program bug - please report this problem with details: AwsDotNetS3LargeFileXferCmd {GetVersion()} - GetArgValue() paramLine=[null]");
            else
            {
                int count = 1;  
                var offsetArgEnd = 0;
                while (offsetArgEnd < paramLine.Length)
                {
                    var offsetArgStart = 0;
                    if ((offsetArgStart = paramLine.IndexOf(spaceChar, offsetArgEnd)) == -1)
                        break;
                    while (paramLine[offsetArgStart] == spaceChar)
                        offsetArgStart++;
                    if ((offsetArgEnd = GetEndArgOffset(paramLine, offsetArgStart)) == -1)
                    {
                        SetErrorMsg($"Error: Invalid argument {argNumber} in \"{paramLine}\" - no closing quote character {Environment.NewLine}{GetParamHelp()}");
                        break;
                    }
                    if (count++ == argNumber)
                    {
                        if ((offsetArgEnd - offsetArgStart + 1) <= 0)
                        {
                            SetErrorMsg($"Error: Program bug - please report this problem with details: AwsDotNetS3LargeFileXferCmd {GetVersion()} - GetArgValue() offsetArgEnd={offsetArgEnd}, offsetArgStart={offsetArgStart}");
                            break;
                        }
                        var argument = paramLine.Substring(offsetArgStart, offsetArgEnd - offsetArgStart + 1);
                        if (argument[0] != quoteChar)
                            rc = argument;
                        else
                        {
                            if (argument.Length < 3)
                                SetErrorMsg($"Error: Invalid argument {argNumber} in \"{paramLine}\" - nothing between the quotes {Environment.NewLine}{GetParamHelp()}");
                            else
                                rc = argument.Substring(1, argument.Length - 2);
                        }
                        break;
                    }
                    offsetArgEnd++;
                }
            }
            return rc;
        }

        private int GetEndArgOffset(string paramLine, int offsetArgStart)
        {
            int rc = -1;

            if ((offsetArgStart <= 0) || (paramLine == null) || (offsetArgStart >= paramLine.Length))
                SetErrorMsg($"Error: Program bug - please report this problem with details: AwsDotNetS3LargeFileXferCmd {GetVersion()} - GetEndArg() paramLine=\"{paramLine ?? "[null]"}\" offsetArgStart={offsetArgStart}");
            else
            {
                var offset = -1;
                if (paramLine[offsetArgStart] != quoteChar)
                {
                    offset = paramLine.IndexOf(spaceChar, offsetArgStart);
                    if (offset == -1)
                        rc = paramLine.Length - 1;
                    else
                        rc = offset - 1;
                }
                else
                {
                    if (offsetArgStart + 1 < paramLine.Length)
                    {
                        offset = paramLine.IndexOf(quoteChar, offsetArgStart + 1);
                        if (offset >= 0)
                            rc = offset;
                    }
                }
            }
            return rc;
        }

        protected int GetArgCount(string paramLine)
        {
            int count = 0;
            while (GetArgValue(paramLine, count+1) != null)
                count++;
            return (IsError == false) ? count : -1;
        }

        protected static string GetHelpNotes()
        {
            string rc = "";

            rc += Environment.NewLine;
            rc += Environment.NewLine;
            rc += "Notes:";
            rc += Environment.NewLine;
            rc += "   --a 1 2 3 means parameter 'a' with arguments 1, 2 and 3.";
            rc += Environment.NewLine;
            rc += "   [--a ... | --b ...] means enter either parameter a or b. --a [1 ... | 2 ...] means enter either argument 1 or 2.";
            rc += Environment.NewLine;
            rc += "   (--c ...) means parameter c is optional. --c 1 (2) means argument 2 is optional.";
            rc += Environment.NewLine;
            rc += "   --d 5 <min 0 max 10> means argument for parameter d is a number in range 0-10 with a default value of 5.";
            rc += Environment.NewLine;
            rc += "   --e 1 '2 x' 3 means there are three arguments for parameter e; 1, '2 x' and 3. The second argument contains a space.";
            rc += Environment.NewLine;
            return rc;
        }

        public static string GetVersion()
        {
            string rc = "v";

            rc += typeof(CmdLineParams)?.GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            return rc;
        }
    }
}
