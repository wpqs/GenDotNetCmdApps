using System;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.View;
using MxReturnCode;

namespace KLineEdCmdApp.Controller
{
    public class SpellEditingController : BaseEditingController
    {
        public static readonly string EditorHelpText = $"{SpellEditView.SpellEditorMode} Ctrl+Q=Quit Ctrl+S=Save Esc=Refresh F1=Help F2=Text editing";

        public override BaseEditingController ProcessKey(ChapterModel model, ConsoleKeyInfo keyInfo)
        {
            var rc = new MxReturnCode<bool>($"SpellEditingController.ProcessKey");

            BaseEditingController controller = this;
            if ((base.ProcessKey(model, keyInfo) == null)) // && (IsErrorState() == false))
                rc.SetResult(true);     //key handled by base class - no further action needed  
            else
            {
                //do stuff related to SpellEditing, updating the model as needed
                if (keyInfo.Key == ConsoleKey.F2)
                {
                    controller = ControllerFactory.Make(Chapter, ControllerFactory.TextEditingController, BrowserCmd, HelpUrl, SearchUrl, ThesaurusUrl, SpellUrl);
                    rc.SetResult(true);
                }
                else
                {
                    rc.SetResult(true);
                }
            }

            if (rc.IsError(true))
                SetErrorState(rc);

            return controller;
        }

        public override string GetEditorHelpLine()
        {
            return EditorHelpText;
        }
    }
}
