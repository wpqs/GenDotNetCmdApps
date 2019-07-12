using System;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.Model;

namespace KLineEdCmdApp.Controller
{
    public class TextEditingModeProc : ModeProc
    {
        public override KLineEditor.CmdMode ProcessKey(ConsoleKey key, ChapterModel model)
        {
            var rc = KLineEditor.CmdMode.Error;
            if (base.ProcessKey(key, model) != KLineEditor.CmdMode.Error)
            {
                //do stuff related to TextEditing, updating the model as needed
                rc = KLineEditor.CmdMode.TextEditing;
            }
            return rc;
        }

    }
}
