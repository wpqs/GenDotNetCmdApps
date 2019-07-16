using System;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.View.Base;
using MxReturnCode;

namespace KLineEdCmdApp.Controller.Base
{
    public abstract class EditingBaseController
    {
        public bool Ready { private set; get; }
        public ChapterModel Chapter { private set; get; }
        public string DisplayMsg { private set; get; }

        public EditingBaseController()
        {
            DisplayMsg = null;
            Chapter = null;
            Ready = false;
        }

        public bool IsDisplayMsg() { return (DisplayMsg != null); }
        public bool IsCritialError() { return BaseView.IsCriticalError(DisplayMsg); }
        private void SetError(int errorNo, BaseView.ErrorType errType, string errMsg) { DisplayMsg = BaseView.FormatMxErrorMsg(errorNo, errType, errMsg); }
        private void SetWarning(string warnMsg) { DisplayMsg = $"{BaseView.WarnMsgPrecursor} {warnMsg ?? Program.ValueNotSet}"; }
        private void SetInfo(string infoMsg) { DisplayMsg = $"{BaseView.InfoMsgPrecursor} {infoMsg ?? Program.ValueNotSet}"; }



        public virtual MxReturnCode<bool> Initialise(ChapterModel model)
        {
            var rc = new MxReturnCode<bool>("EditingBaseController.Initialise");

            if ((model?.Ready ?? false) == false)
                rc.SetError(1030101, MxError.Source.Param, $"param is null or editModel null, not ready", "MxErrBadMethodParam");
            else
            {
                Chapter = model;
                Ready = true;
                rc.SetResult(true);
            }
            return rc;
        }

        public static EditingBaseController ProcessKey(EditingBaseController controller, ChapterModel model, ConsoleKey key)
        {
            // ReSharper disable once ConstantNullCoalescingCondition
            return controller?.ProcessKey(key) ?? null; 
        }

        protected virtual EditingBaseController ProcessKey(ConsoleKey key)
        {
            //do common stuff with key - let override in derived class do the rest 
            return this;  
        }

        public virtual string GetEditorHelpLine()
        {
            return $"Unsupported: Esc=Refresh F1=Help Ctrl+Q=Quit";
        }
    }
}
