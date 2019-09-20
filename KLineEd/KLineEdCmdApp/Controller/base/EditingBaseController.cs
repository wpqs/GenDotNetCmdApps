using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Properties;
using KLineEdCmdApp.Utils;
using MxReturnCode;

namespace KLineEdCmdApp.Controller.Base
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public abstract class EditingBaseController
    {
        public ChapterModel Chapter { private set; get; }
        public string BrowserExe { private set; get; }

        private bool _insertMode;
        private MxReturnCode<bool> _mxErrorCode;
        private bool _ctrlQ;
        private bool _refresh;
        // ReSharper disable once SimplifyConditionalTernaryExpression

        protected EditingBaseController()
        {
            _ctrlQ = false;
            _refresh = false;
            _mxErrorCode = new MxReturnCode<bool>($"{GetType().Name}.Ctor", false); //SetResult(true) on error
            Chapter = null;
            _insertMode = false;
            BrowserExe = CmdLineParamsApp.ArgEditBrowserNameDefault;
        }

        public bool IsInsertMode() { return _insertMode; }
        // ReSharper disable once SimplifyConditionalTernaryExpression
        public bool IsQuit() { return _ctrlQ; }

        public MxError.Source GetErrorSource() { return _mxErrorCode?.GetErrorType() ?? MxError.Source.Program; }
        public int GetErrorNo() { return _mxErrorCode?.GetErrorCode() ?? Program.PosIntegerNotSet; }
        public string GetErrorTechDetails() { return _mxErrorCode?.GetErrorTechMsg(MxError.UserMsgPart.Details) ?? Program.ValueNotSet; }
       // public string GetErrorUserMsg() { return _mxErrorCode?.GetType() ?? Program.ValueNotSet; }
        public bool IsError(bool criticalOnly=false)
        {
            var rc = false;
            if ((criticalOnly == false) && (_mxErrorCode?.IsError() ?? true))
                rc = true;
            else
            {
                if ((_mxErrorCode?.IsError() ?? true) && ((_mxErrorCode?.GetErrorType() ?? MxError.Source.Program) != MxError.Source.User))
                    rc = true;
            }
            return rc;
        }

        public void ResetError()
        {
            _mxErrorCode = new MxReturnCode<bool>($"{GetType().Name}.ProcessKey", true);
            _mxErrorCode?.SetResult(true);
        }

        public bool IsRefresh()
        {
            var rc = _refresh;
            _refresh = false;
            return rc;
        }

        public virtual MxReturnCode<bool> Initialise(ChapterModel model, string browserExe)
        {
            var rc = new MxReturnCode<bool>("EditingBaseController.Initialise");

            if ((model?.Ready ?? false) == false)
                rc.SetError(1030101, MxError.Source.Param, $"model null, not ready, or browserExe null", MxMsgs.MxErrBadMethodParam);
            else
            {

                _ctrlQ = false;
                _refresh = false;
                _insertMode = false;
                Chapter = model;
                BrowserExe = browserExe;
                _mxErrorCode?.SetResult(true);
                rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> Close()
        {
            var rc = new MxReturnCode<bool>($"{GetType().Name}.Close");

            if (_mxErrorCode.IsError())
                rc += _mxErrorCode;

            return rc;
        }

        public MxReturnCode<MxReturnCode<bool>> GetMxError()
        {
            var rc = new MxReturnCode<MxReturnCode<bool>>($"{GetType().Name}.GetMxError");

            rc += _mxErrorCode;

            return rc;
        }

        protected void SetupMxError(MxReturnCode<bool> rc)  //todo update when next release of MxReturnCode is available
        {
            var userMsg = (rc.GetErrorType() == MxError.Source.User) ? null : MxMsgs.MxWarnInvalidChar;
            var techMsg = (rc.GetErrorType() == MxError.Source.User) ? rc.GetErrorTechMsg() : Resources.MxWarnInvalidChar;

            var startIndex = techMsg.IndexOf(':');
            if (startIndex + 2 > techMsg.Length + 1)
                startIndex = -1;
            SetMxError(rc.GetErrorCode(), rc.GetErrorType(), (startIndex!= -1) ? techMsg.Substring(startIndex+2) : techMsg, userMsg); 
        }

        protected bool SetMxError(int errorCode, MxError.Source source, string errMsgTech, string errMsgUser)
        {       //use only in base classes, concrete derived classes use local MxResult variable like PropsEditingController, etc
            var rc = false;

            if ((_mxErrorCode != null) && (errMsgTech != null)) // && (errMsgUser != null))
            {
                _mxErrorCode.SetError(errorCode, source, errMsgTech, errMsgUser);
                rc = true;
            }
            return rc;
        }

        public virtual EditingBaseController ProcessKey(ChapterModel model, ConsoleKeyInfo keyInfo)
        {
            EditingBaseController rc = null;

            ResetError();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if ((model == null) || (keyInfo == null))
                _mxErrorCode.SetError(1030201, MxError.Source.Param, $"model is null or keyInfo null", MxMsgs.MxErrBadMethodParam);
            else
            {
                if ((keyInfo.Modifiers == ConsoleModifiers.Control) && (keyInfo.Key == ConsoleKey.Q))
                    _ctrlQ = true;
                else if (keyInfo.Key == ConsoleKey.F1)
                {
                    try
                    {
                        Process.Start(BrowserExe, Program.CmdAppHelpUrl);
                    }
                    catch (Exception e)
                    {
                        _mxErrorCode?.SetError(1030202, MxError.Source.User, e.Message); //, MxMsgs.MxErrBrowserFailed);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                   // ResetError();
                    _refresh = true;
                }
                else if (keyInfo.Key == ConsoleKey.Insert)
                {
                    _insertMode = !_insertMode;
                }
                else
                {
                    rc = this;
                }
            }
            return rc; 
        }

        protected virtual EditingBaseController ProcessKey(ConsoleKeyInfo key)
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
