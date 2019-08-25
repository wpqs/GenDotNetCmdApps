using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KLineEdCmdApp.Model;

namespace KLineEdCmdAppTest.TestSupport
{
    public class MockModelChapterModel : ChapterModel
    {
        public MockModelChapterModel() : base()
        {
            ChapterBody = new MockModelBody();
        }

        public void SetTestLine(string line)
        {
           ((MockModelBody)ChapterBody).SetTestLine(line);
        }
    }
}
