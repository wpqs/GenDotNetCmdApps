using System;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.Model;

namespace KLineEdCmdApp.Controller
{
    public class TextEditingModeProc : ModeProc
    {
        public override KLineEditor.OpMode ProcessKey(ConsoleKey key, ChapterModel model)
        {
            var rc = KLineEditor.OpMode.Error;
            if (base.ProcessKey(key, model) != KLineEditor.OpMode.Error)
            {
                //do stuff related to TextEditing, updating the model as needed
                rc = KLineEditor.OpMode.TextEditing;
            }
            return rc;
        }

    }
}
