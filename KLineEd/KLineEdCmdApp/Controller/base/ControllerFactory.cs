using KLineEdCmdApp.Model;

namespace KLineEdCmdApp.Controller.Base
{
    public static class ControllerFactory
    {
        public static readonly string TextEditingController = "TextEditingController";
        public static readonly string PropsEditingController = "PropsEditingController";
        public static readonly string SpellEditingController = "SpellEditingController";
        public static EditingBaseController Make(ChapterModel model, string className, string browserCmd, string helpUrl, string searchUrl, string thesaurusUrl, string spellUrl)
        {
            EditingBaseController rc = null;

            if ((model?.Ready ?? false) && (className != null) && (browserCmd != null) && (helpUrl != null) && (searchUrl != null) && (thesaurusUrl != null) && (spellUrl != null))
            {
                EditingBaseController controller = null;
                if (className == TextEditingController)
                    controller = new TextEditingController();
                else
                {
                    if (className == PropsEditingController)
                        controller = new PropsEditingController();
                    else
                    {
                        if (className == SpellEditingController)
                            controller = new SpellEditingController();

                        //create other Editors here

                    }
                }

                if (controller == null)
                    model.SetErrorMsg(1220101, $"Program defect. {className} is unsupported. Please report this problem.");
                else
                {
                    var rcInit = controller.Initialise(model, browserCmd, helpUrl, searchUrl, thesaurusUrl, spellUrl);
                    if (rcInit.IsError(true))
                        model.SetMxErrorMsg(rcInit.GetErrorUserMsg());
                    else
                    {
                        model.SetEditorHelpLine(controller.GetEditorHelpLine(), false);
                        model.Refresh();
                        //model.SetTextAreaName(controller.GetEditorTextAreaName());
                        rc = controller;
                    }
                }
            }
            return rc;
        }
    }
}
