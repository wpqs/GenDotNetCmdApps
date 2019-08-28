using KLineEdCmdApp.Model;

namespace KLineEdCmdAppTest.TestSupport
{
    public class MockModelChapterModel : ChapterModel
    {
        public MockModelChapterModel() 
        {
            ChapterBody = new MockModelBody();
        }

        public void SetTestLine(string line)
        {
           ((MockModelBody)ChapterBody).SetTestLine(line);
        }
    }
}
