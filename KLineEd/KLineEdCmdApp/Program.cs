using System;
using System.Reflection;
using System.Threading.Tasks;

namespace KLineEdCmdApp
{
    public class Program
    {
        static async Task<int> Main(string[] args)
        {
            int rc = -1;

            Console.WriteLine($"KLineEdCmdApp {GetVersion()}.{Environment.NewLine}Copyright 2019 Will Stott.{Environment.NewLine}Use subject to standard MIT License - see https://github.com/wpqs/GenDotNetCmdApps {Environment.NewLine}");

            rc = 0;

            Console.WriteLine((rc == 0) ? $"program ends : return code {rc}" : $"program abends: return code {rc}");
            return rc;

        }

        public static string GetVersion()
        {
            string rc = "v";

            rc += typeof(Program)?.GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            return rc;
        }
    }
}
