
namespace KLineEdCmdAppTest.TestSupport
{
    public static class TestConst
    {
        public static readonly string UnitTestInstanceTestsPathFileName = "C:\\UnitTestData\\InstanceTests.ksx";
        public static readonly string UnitTestSharedTestsPathFileName = "C:\\UnitTestData\\SharedTests.ksx";
        public static readonly string UnitTestInvalidPathFileName = "C:\\UnitTestData\\xxx\\SharedTests.ksx";

        public static readonly string UnitTestNotSet = KLineEdCmdApp.Program.ValueNotSet;
        public static readonly string UnitTestNone = "[none]";

        public static readonly int UnitTestEditAreaWidth = 66;
        public static readonly int UnitTestEditAreaLines = 20;

        public const string MxNoError = "[no error]";
    }
}
