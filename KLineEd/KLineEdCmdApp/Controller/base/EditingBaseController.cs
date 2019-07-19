using System;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.View.Base;
using MxReturnCode;

namespace KLineEdCmdApp.Controller.Base
{
    public abstract class EditingBaseController
    {
        public ChapterModel Chapter { private set; get; }
 
        private readonly MxReturnCode<bool> _mxErrorCode;

        // ReSharper disable once SimplifyConditionalTernaryExpression
        public bool IsError() { return (_mxErrorCode?.GetResult() ?? false) ? false : true; }
        public MxError.Source GetErrorSource() { return _mxErrorCode?.GetErrorType() ?? MxError.Source.Program; }
        public int GetErrorNo() { return _mxErrorCode?.GetErrorCode() ?? Program.PosIntegerNotSet; }
        public string GetErrorTechMsg() { return _mxErrorCode?.GetErrorTechMsg() ?? Program.ValueNotSet; }
        public string GetErrorUserMsg() { return _mxErrorCode?.GetErrorUserMsg() ?? Program.ValueNotSet; }

        public EditingBaseController()
        {
            _mxErrorCode = new MxReturnCode<bool>($"{this.GetType().Name}.Ctor", false); //SetResult(true) on error
            Chapter = null;
   
        }

        public virtual MxReturnCode<bool> Initialise(ChapterModel model)
        {
            var rc = new MxReturnCode<bool>("EditingBaseController.Initialise");

            if ((model?.Ready ?? false) == false)
                rc.SetError(1030101, MxError.Source.Param, $"param is null or editModel null, not ready", MxMsgs.MxErrBadMethodParam);
            else
            {
                Chapter = model;
                _mxErrorCode?.SetResult(true);
                rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> Close()
        {
            var rc = new MxReturnCode<bool>($"{this.GetType().Name}.Close");

            if (_mxErrorCode.IsError())
                rc += _mxErrorCode;

            return rc;
        }

        public MxReturnCode<MxReturnCode<bool>> GetMxError()
        {
            var rc = new MxReturnCode<MxReturnCode<bool>>($"{this.GetType().Name}.GetMxError");

            rc += _mxErrorCode;

            return rc;
        }

        protected bool SetMxError(int errorCode, MxError.Source source, string errMsgTech, string errMsgUser)
        {       //use only in base classes, concrete derived classes use local MxResult variable like PropsEditingController, etc
            var rc = false;

            if ((_mxErrorCode != null) && (errMsgTech != null) && (errMsgUser != null))
            {
                _mxErrorCode.SetError(errorCode, source, errMsgTech, errMsgUser);
                rc = true;
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
