using KLineEdCmdApp.Model;

namespace KLineEdCmdApp.Controller.Base
{
    public static class ControllerFactory
    {
        public static readonly string TextEditingController = "TextEditingController";
        public static readonly string PropsEditingController = "PropsEditingController";

        public static EditingBaseController Make(ChapterModel model, string className)
        {
            EditingBaseController rc = null;

            if (model?.Ready ?? false)
            {
                EditingBaseController controller = null;
                if (className == TextEditingController)
                    controller = new TextEditingController();
                else
                {
                    if (className == PropsEditingController)
                        controller = new PropsEditingController();

                    //create other Editors here

                }

                if (controller == null)
                    model.SetErrorMsg(1220101, $"Program defect. {className} is unsupported. Please report this problem.");
                else
                {
                    var rcInit = controller.Initialise(model);
                    if (rcInit.IsError(true))
                        model.SetMxErrorMsg(rcInit.GetErrorUserMsg());
                    else
                    {
                        model.SetEditorHelpLine(controller.GetEditorHelpLine(), false);
                        model.SetMsgLine("hello wills...", false);
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
