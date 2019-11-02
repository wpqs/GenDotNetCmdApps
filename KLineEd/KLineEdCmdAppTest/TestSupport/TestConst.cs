
namespace KLineEdCmdAppTest.TestSupport
{
    public static class TestConst
    {
        public static readonly string UnitTestDir = ".\\UnitTestData";
        public static readonly string UnitTestInstanceTestsPathFileName =$"{UnitTestDir}\\InstanceTests.ksx";
        public static readonly string UnitTestSharedTestsPathFileName = $"{UnitTestDir}\\SharedTests.ksx";
        public static readonly string UnitTestInvalidPathFileName = $"{UnitTestDir}\\xxx\\SharedTests.ksx";

        public static readonly string UnitTestNotSet = KLineEdCmdApp.Program.ValueNotSet;
        public static readonly string UnitTestNone = "[none]";

        public static readonly int TextEditorDisplayCols = 66;
        public static readonly int TextEditorDisplayRows = 20;

        public const string MxNoError = "[no error]";
    }
}
