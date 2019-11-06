using KLineEdCmdApp.Model;
using MxReturnCode;

namespace KLineEdCmdApp.Controller.Base
{
    public static class ControllerFactory
    {
        public static readonly string TextEditingController = "TextEditingController";
        public static readonly string PropsEditingController = "PropsEditingController";
        public static readonly string SpellEditingController = "SpellEditingController";
        public static BaseEditingController Make(ChapterModel model, string className, string browserCmd, string helpUrl, string searchUrl, string thesaurusUrl, string spellUrl)
        {
            var rc = new MxReturnCode<bool>($"ControllerFactory.Maker");

            BaseEditingController controller = null;

            if ((model?.Ready ?? false) && (className != null) && (browserCmd != null) && (helpUrl != null) && (searchUrl != null) && (thesaurusUrl != null) && (spellUrl != null))
            {
                BaseEditingController ctrller = null;
                if (className == TextEditingController)
                    ctrller = new TextEditingController();
                else
                {
                    if (className == PropsEditingController)
                        ctrller = new PropsEditingController();
                    else
                    {
                        if (className == SpellEditingController)
                            ctrller = new SpellEditingController();

                        //create other Editors here

                    }
                }

                if (ctrller == null)
                    rc.SetError(1220101, MxError.Source.Program, $"Program defect. {className} is unsupported. Please report this problem.");
                else
                {
                    var rcInit = ctrller.Initialise(model, browserCmd, helpUrl, searchUrl, thesaurusUrl, spellUrl);
                    if (rcInit.IsError(true))
                        rc += rcInit;
                    else
                    {
                        model.SetEditorHelpLine(ctrller.GetEditorHelpLine(), false);
                        model.Refresh();
                        //model.SetTextAreaName(controller.GetEditorTextAreaName());
                        controller = ctrller;
                        rc.SetResult(true);
                    }
                }
            }

            if (rc.IsError(true))
                model.SetErrorState(rc);

            return controller;
        }
    }
}
