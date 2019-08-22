using System;
using System.Collections.Generic;
using System.Text;
using KLineEdCmdApp.Model;
using MxReturnCode;

namespace KLineEdCmdAppTest.TestSupport
{
    public class MockModelBody : Body
    {
        public void SetTestLine(string line)
        {
            TextLines.Add(line);
            WordCount += GetWordCountInLine(line);
            Cursor.RowIndex = TextLines.Count - 1;
            Cursor.ColIndex = line.EndsWith(ParaBreakChar) ? line.Length - 1 : line.Length;
            SetEditAreaBottomIndex(Scroll.ToCursor);
        }
    }
}
