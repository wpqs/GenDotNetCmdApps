using System;
using System.IO;

namespace KLineEdCmdAppTest.TestSupport
{
    public class ChapterModelFixture : IDisposable
    {
        public static readonly string UnitTestCreateTestsPathFileName = "C:\\UnitTestData\\CreateTests.txt";
        public static readonly string UnitTestInvalidPathFileName = "C:\\UnitTestData\\xxx\\CreateTests.txt";

        public string  CreatePathFilename { get; private set; }
        public ChapterModelFixture()
        {
            CreatePathFilename = UnitTestCreateTestsPathFileName;
            if (File.Exists(CreatePathFilename))
                File.Delete(CreatePathFilename);
        }

        public void Dispose()
        {
            //cleanup
        }
    }
}
