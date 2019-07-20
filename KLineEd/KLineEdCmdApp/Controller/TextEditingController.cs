using System;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.View;

namespace KLineEdCmdApp.Controller
{
    public class TextEditingController : EditingBaseController
    {
        public static readonly string EditorHelpText = $"{TextEditView.TextEditorMode} Ctrl+Q=Quit Esc=Refresh F1=Help F2=Props F3=Spelling";
        public override EditingBaseController ProcessKey(ChapterModel model, ConsoleKeyInfo keyInfo)
        {
            EditingBaseController controller = this;
            if ((base.ProcessKey(model, keyInfo) != null) && (IsError() == false))
            {
                //do stuff related to TextEditing, updating the model as needed
                if (keyInfo.Key == ConsoleKey.F2)
                    controller = ControllerFactory.Make(Chapter, ControllerFactory.PropsEditingController);
                if (keyInfo.Key == ConsoleKey.F3)
                    controller = ControllerFactory.Make(Chapter, ControllerFactory.SpellEditingController);
            }
            return controller;
        }

        public override string GetEditorHelpLine()
        {
            return EditorHelpText;
        }
    }
}
