using System;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.View;

namespace KLineEdCmdApp.Controller
{
    public class PropsEditingController : EditingBaseController
    {
        public static readonly int AuthorLineEditKey = 1;
        public static readonly int ProjectLineEditKey = 2;
        public static readonly int TitleLineEditKey = 3;

        public static readonly string EditorHelpText = $"{PropsEditView.PropsEditorMode} Esc=Refresh F1=Help  Ctrl+Q=Quit Edit: Author={AuthorLineEditKey} Project={ProjectLineEditKey} Title={TitleLineEditKey}";

        protected override EditingBaseController ProcessKey(ConsoleKey key)
        {
            EditingBaseController controller = this;
            if (base.ProcessKey(key) != null)   //should never be null
            {
                //do stuff related to TextEditing, updating the model as needed
                if (key == ConsoleKey.F1)
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
