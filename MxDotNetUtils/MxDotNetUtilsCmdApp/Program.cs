using System;
using System.Reflection;

namespace MxDotNetUtilsCmdApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public static string GetVersion()
        {
            string rc = "v";

            rc += typeof(Program)?.GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            return rc;
        }
    }
}
