using System;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.View;

namespace KLineEdCmdApp.Controller
{
    public class PropsEditingController : EditingBaseController
    {
        public static readonly string EditorHelpText = $"{PropsEditView.PropsEditorMode} Ctrl+Q=Quit Esc=Refresh F1=Help F2=Text editing";

        public override EditingBaseController ProcessKey(ChapterModel model, ConsoleKeyInfo keyInfo)
        {
            EditingBaseController controller = this;
            if ((base.ProcessKey(model, keyInfo) != null) && (IsError() == false))  
            {
                //do stuff related to TextEditing, updating the model as needed
                if (keyInfo.Key == ConsoleKey.F2)
                    controller = ControllerFactory.Make(Chapter, ControllerFactory.TextEditingController); 
            }
            return controller;
        }

        public override string GetEditorHelpLine()
        {
            return EditorHelpText;
        }
    }
}
