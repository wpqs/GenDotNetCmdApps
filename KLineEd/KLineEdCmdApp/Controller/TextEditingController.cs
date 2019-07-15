using System;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.Model;

namespace KLineEdCmdApp.Controller
{
    public class TextEditingController : EditingBaseController
    {
        public override EditingBaseController ProcessKey(ConsoleKey key, ChapterModel model)
        {
            if (base.ProcessKey(key, model) != null)
            {
                //do stuff related to TextEditing, updating the model as needed
 
            }
            return this;
        }

        public override string GetModeHelpLine()
        {
            return $"Text Editing: Esc=Refresh F1=Help  Ctrl+Q=Quit";
        }
    }
}
