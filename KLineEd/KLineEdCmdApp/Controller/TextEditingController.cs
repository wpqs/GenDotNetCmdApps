﻿using System;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.View;

namespace KLineEdCmdApp.Controller
{
    public class TextEditingController : EditingBaseController
    {
        public static readonly string EditorHelpText = $"{TextEditView.TextEditorMode} Esc=Refresh F1=Help  Ctrl+Q=Quit";
        protected override EditingBaseController ProcessKey(ConsoleKey key)
        {
            EditingBaseController controller = this;
            if (base.ProcessKey(key) != null)   //should never be null
            {
                //do stuff related to TextEditing, updating the model as needed
                if (key == ConsoleKey.F1)
                    controller = ControllerFactory.Make(Chapter, ControllerFactory.PropsEditingController);
                if (key == ConsoleKey.F2)
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
