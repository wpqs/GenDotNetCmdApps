using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;
using MxReturnCode;

namespace KLineEdCmdApp.Controller.Base
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    [SuppressMessage("ReSharper", "ConstantConditionalAccessQualifier")]
    [SuppressMessage("ReSharper", "RedundantTernaryExpression")]
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    public abstract class BaseEditingController : IErrorState
    {
        public ChapterModel Chapter { private set; get; }
        public string BrowserCmd { private set; get; }
        public string HelpUrl { private set; get; }
        public string SearchUrl { private set; get; }
        public string ThesaurusUrl { private set; get; }
        public string SpellUrl { private set; get; }

        private bool _insertMode;
        private bool _ctrlQ;
        private bool _refresh;
        private bool _userErrorResetRequest;

        private MxReturnCode<bool> _mxErrorState;
        public bool IsErrorState() { return (_mxErrorState?.IsError(true) ?? false) ? true : false; }
        public MxReturnCode<bool> GetErrorState() { return _mxErrorState; }
        public void ResetErrorState() { _mxErrorState = null; }
        public bool SetErrorState(MxReturnCode<bool> mxErr)
        {
            var rc = false;

            if (_mxErrorState == null)
            {
                _mxErrorState = mxErr;
                rc = true;
            }
            return rc;
        }

        // ReSharper disable once SimplifyConditionalTernaryExpression

        protected BaseEditingController()
        {
            _ctrlQ = false;
            _refresh = false;
            _userErrorResetRequest = false;
            _mxErrorState = null;
            Chapter = null;
            _insertMode = false;
            BrowserCmd = CmdLineParamsApp.ArgToolBrowserCmdDefault;
            HelpUrl = CmdLineParamsApp.ArgToolHelpUrlDefault;
            SearchUrl = CmdLineParamsApp.ArgToolSearchUrlDefault;
            ThesaurusUrl = CmdLineParamsApp.ArgToolThesaurusUrlDefault;
            SpellUrl = CmdLineParamsApp.ArgToolSpellUrlDefault;
        }

        public bool IsInsertMode() { return _insertMode; }
        // ReSharper disable once SimplifyConditionalTernaryExpression
        public bool IsQuit() { return _ctrlQ; }

        public bool IsRefresh()
        {
            var rc = _refresh;
            _refresh = false;
            return rc;
        }

        public bool SetRefresh(bool refresh=true)
        {
            _refresh = refresh;
            return _refresh;
        }

        public virtual MxReturnCode<bool> Initialise(ChapterModel model, string browserCmd, string helpUrl, string searchUrl, string thesaurusUrl, string spellUrl)
        {
            var rc = new MxReturnCode<bool>("BaseEditingController.Initialise");

            if ((model?.Ready ?? false) == false)
                rc.SetError(1250101, MxError.Source.Param, $"model null, not ready, or browserCmd null", MxMsgs.MxErrBadMethodParam);
            else
            {

                _ctrlQ = false;
                _refresh = false;
                _userErrorResetRequest = false;
                _insertMode = false;
                _mxErrorState = null;
                Chapter = model;
                BrowserCmd = browserCmd;
                HelpUrl = helpUrl;
                SearchUrl = searchUrl;
                ThesaurusUrl = thesaurusUrl;
                SpellUrl = spellUrl;
                
                rc.SetResult(true);
            }
            return rc;
        }

        public MxReturnCode<bool> Close()
        {
            var rc = new MxReturnCode<bool>($"{GetType().Name}.Close");

            if (IsErrorState())
                rc += GetErrorState();

            return rc;
        }

        public static MxReturnCodeUtils.MsgClass GetMsgClass(string msg)
        {
            var rc = MxReturnCodeUtils.MsgClass.Unknown;

            if (msg.StartsWith(MxDotNetUtilsLib.EnumOps.XlatToString(MxReturnCodeUtils.MsgClass.Warning)))
                rc = MxReturnCodeUtils.MsgClass.Warning;
            else if (msg.StartsWith(MxDotNetUtilsLib.EnumOps.XlatToString(MxReturnCodeUtils.MsgClass.Info)))
                rc = MxReturnCodeUtils.MsgClass.Info;
            else
                rc = MxReturnCodeUtils.MsgClass.Error;
            return rc;
        }


        public MxReturnCode<bool> ErrorProcessing(List<BaseView> viewList)
        {
            var rc = new MxReturnCode<bool>("BaseEditingController.ErrorProcessing");

            if ((viewList == null) ||  ((Chapter?.Ready ?? false) == false) || (Chapter?.MsgLine == null ))
                rc.SetError(1250201, MxError.Source.Param, $"viewList is null, or model null, or not ready ", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (_userErrorResetRequest)
                {
                    _userErrorResetRequest = false;
                    ResetAllErrorStates(viewList);
                    if (Chapter.MsgLine?.Length > 0)
                        Chapter.SetMsgLine("");
                    rc.SetResult(true);
                }
                else
                {
                    if (Chapter.MsgLine.Length == 0)
                    {
                        var err = GetAllErrorStates(viewList);
                        if (err == null)
                            rc.SetResult(true);
                        else
                        {
                            SetErrorState(err);
                            var mxErrorMsg = err.GetErrorUserMsg();
                            var msgClass = MxDotNetUtilsLib.EnumOps.XlatToString(MxReturnCodeUtils.GetErrorClass(mxErrorMsg));
                            var msgText = MxReturnCodeUtils.GetErrorText(mxErrorMsg);
                            var msgErrCode = MxReturnCodeUtils.GetErrorCode(mxErrorMsg);
                            Chapter.SetMsgLine($"{msgClass} {msgErrCode}: {msgText ?? Program.ValueNotSet}"); //FORMAT MESSAGE FOR DISPLAY - message only displayed if Chapter.MsgLine != msg  
                            _userErrorResetRequest = false;

                            if ((err.GetErrorType() != MxError.Source.Exception))
                                rc.SetResult(true);
                        }
                    }
                }
            }
            return rc;  //terminate loop if error or GetResult() is false;
        }

        private void ResetAllErrorStates(List<BaseView> viewList)
        {
            if (IsErrorState())             //check controller
                ResetErrorState();
            if (Chapter.IsErrorState())    //check model
                Chapter.ResetErrorState();
            foreach (var view in viewList) //check views
            {
                if (view.IsErrorState())
                    view.ResetErrorState();
            }
        }

        private MxReturnCode<bool> GetAllErrorStates(List<BaseView> viewList)
        {
            var err = _mxErrorState; //check controller
            if (err == null)
            {
                if (Chapter.IsErrorState()) //check model
                    err = Chapter.GetErrorState();
                else
                {
                    foreach (var view in viewList) //check views
                    {
                        if (view.IsErrorState())
                        {
                            err = view.GetErrorState();
                            break;
                        }
                    }
                }
            }
            return err;
        }


        public virtual BaseEditingController ProcessKey(ChapterModel model, ConsoleKeyInfo keyInfo)
        {
            BaseEditingController controller = null;

            var rc = new MxReturnCode<bool>($"BaseEditingController.ProcessKey");
            
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if ((model == null) || (keyInfo == null))
                rc.SetError(1250301, MxError.Source.Param, $"model is null or keyInfo null", MxMsgs.MxErrBadMethodParam);
            else
            {
                var ctrlKeyPressed = ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0);

                if ((ctrlKeyPressed) && (keyInfo.Key == ConsoleKey.Q))
                {
                    _ctrlQ = true;
                    rc.SetResult(true);
                }
                else
                {
                    if (IsErrorState())
                        _userErrorResetRequest = true;
                    if (ctrlKeyPressed && (keyInfo.Key == ConsoleKey.S))
                    {
                        var rcSave = Chapter.Save();
                        if (rcSave.IsError(true))
                            SetErrorState(rcSave);
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.F1)
                    {
                        LaunchBrowser(model?.ChapterBody, HelpUrl, false);
                        rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.Escape)
                    {
                        _refresh = true;
                        rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.Insert)
                    {
                        _insertMode = !_insertMode;
                        rc.SetResult(true);
                    }
                    else
                    {
                        controller = this;
                        rc.SetResult(true);
                    }
                }
            }
            if (rc.IsError(true))
                SetErrorState(rc);

            return controller; 
        }

        protected virtual BaseEditingController ProcessKey(ConsoleKeyInfo key)
        {
            //do common stuff with key - let override in derived class do the rest 
            return this;  
        }

        protected void LaunchBrowser(Body body, string url, bool replaceSelectedWord = false)
        {
            var rc = new MxReturnCode<bool>($"BaseEditingController.LaunchBrowser");

            if ((body == null) || (string.IsNullOrEmpty(url)))
                rc.SetError(1250301, MxError.Source.Param, "body of url is null", MxMsgs.MxErrBadMethodParam);
            else
            {
                try
                {
                    if (replaceSelectedWord == false)
                        KLineEditor.StartBrowser(BrowserCmd, url);
                    else
                    {
                        string selectedWord = body.GetSelectedWord();
                        if (selectedWord == null)
                            rc.SetError(1250302, MxError.Source.User, $"no word found at cursor row={(body?.Cursor?.RowIndex ?? Program.PosIntegerNotSet)}, col=row={(body?.Cursor?.ColIndex ?? Program.PosIntegerNotSet)}", MxMsgs.MxErrNoWordSelected);
                        else
                        {
                            url = KLineEditor.GetXlatUrl(url, CmdLineParamsApp.UrlWordMarker, selectedWord);
                            if (KLineEditor.IsValidUri(url) == false)
                                rc.SetError(1250303, MxError.Source.User, $"word={selectedWord} found at cursor row={(body?.Cursor?.RowIndex ?? Program.PosIntegerNotSet)}, col=row={(body?.Cursor?.ColIndex ?? Program.PosIntegerNotSet)} cannot be contained in a url", MxMsgs.MxErrInvalidWordSelected);
                            else
                            {
                                KLineEditor.StartBrowser(BrowserCmd, url);
                                rc.SetResult(true);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    rc.SetError(1250304, MxError.Source.User, e.Message, null);
                }
            }
            if (rc.IsError(true))
                SetErrorState(rc);
        }

        public virtual string GetEditorHelpLine()
        {
            return $"Unsupported: Esc=Refresh F1=Help Ctrl+Q=Quit Ctrl+S=Save";
        }
    }
}
