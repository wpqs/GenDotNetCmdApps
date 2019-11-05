using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Properties;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;
using MxReturnCode;

namespace KLineEdCmdApp.Controller.Base
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    [SuppressMessage("ReSharper", "ConstantConditionalAccessQualifier")]
    public abstract class EditingBaseController
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

        private MxReturnCode<bool> _mxErrorState;
        public bool IsErrorState() { return (_mxErrorState?.IsError(true) ?? false) ? true : false; }
        public MxReturnCode<bool> GetErrorState() { return _mxErrorState ?? null; }
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

        protected EditingBaseController()
        {
            _ctrlQ = false;
            _refresh = false;
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
            var rc = new MxReturnCode<bool>("EditingBaseController.Initialise");

            if ((model?.Ready ?? false) == false)
                rc.SetError(1250101, MxError.Source.Param, $"model null, not ready, or browserCmd null", MxMsgs.MxErrBadMethodParam);
            else
            {

                _ctrlQ = false;
                _refresh = false;
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


        public MxReturnCode<bool> ErrorProcessing(List<BaseView> viewList, IMxConsole console)
        {
            var rc = new MxReturnCode<bool>("EditingBaseController.ErrorProcessing");

            if ((viewList == null) || (console == null) || ((Chapter?.Ready ?? false) == false) )
                rc.SetError(1250201, MxError.Source.Param, $"viewList is null, or console is null, or model null, or not ready  ", MxMsgs.MxErrBadMethodParam);
            else
            {
                var err = _mxErrorState;                    //check controller
                if (err == null)
                {
                    if (console.IsErrorState())             //check console
                        err = console.GetErrorState();
                    else
                    {
                        if (Chapter.IsErrorState())         //check model
                            err = Chapter.GetErrorState();
                        else
                        {
                            foreach (var view in viewList)  //check views
                            {
                                if (view.IsErrorState())
                                {
                                    err = view.GetErrorState();
                                    break;
                                }
                            }
                        }
                    }
                }

                if (err != null)
                {
                    SetErrorState(err);     //copy any errors found in model or views into controller error state
                    var mxErrorMsg = GetErrorState()?.GetErrorUserMsg();
                    var msgClass = MxDotNetUtilsLib.EnumOps.XlatToString(MxReturnCodeUtils.GetErrorClass(mxErrorMsg));
                    var msgText = MxReturnCodeUtils.GetErrorText(mxErrorMsg);
                    var msgErrCode = MxReturnCodeUtils.GetErrorCode(mxErrorMsg);

                    var msg = $"{msgClass} #{msgErrCode}: {msgText ?? Program.ValueNotSet}"; ;

                    Chapter.SetMsgLine(msg);
                    if ((GetErrorState()?.GetErrorType() != MxError.Source.Exception))
                        rc.SetResult(true);
                }
                else
                {       //user pressed a key (not Ctrl+Q) and so cleared controller._mxErrorState, if set - see ProcessKey
                        //clear any errors in  console, model or views that were copied into controller.SetErrorState 
                    if (console.IsErrorState())         //check console
                      console.ResetErrorState();
                    if (Chapter.IsErrorState())         //check model
                        Chapter.ResetErrorState();
                    foreach (var view in viewList)      //check views
                    {
                        if (view.IsErrorState())
                            view.ResetErrorState();
                    }
                    rc.SetResult(true);
                }

            }
            return rc;  //terminate loop if error or GetResult() is false;
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

        public MxReturnCode<bool> Close()
        {
            var rc = new MxReturnCode<bool>($"{GetType().Name}.Close");

            if (IsErrorState())
                rc += GetErrorState();

            return rc;
        }


        public virtual EditingBaseController ProcessKey(ChapterModel model, ConsoleKeyInfo keyInfo)
        {
            EditingBaseController controller = null;

            var rc = new MxReturnCode<bool>($"EditingBaseController.ProcessKey");
            
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if ((model == null) || (keyInfo == null))
                rc.SetError(1250301, MxError.Source.Param, $"model is null or keyInfo null", MxMsgs.MxErrBadMethodParam);
            else
            {
                var ctrlKeyPressed = ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0);

                if ((ctrlKeyPressed) && (keyInfo.Key == ConsoleKey.Q))
                    _ctrlQ = true;
                if ((_ctrlQ == false) && (IsErrorState()))
                {
                    ResetErrorState(); 
                    if (Chapter.MsgLine?.Length > 0)
                        Chapter.SetMsgLine("");
                    rc.SetResult(true);
                }

                if (_ctrlQ == false)
                {
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

        protected virtual EditingBaseController ProcessKey(ConsoleKeyInfo key)
        {
            //do common stuff with key - let override in derived class do the rest 
            return this;  
        }

        protected void LaunchBrowser(Body body, string url, bool replaceSelectedWord = false)
        {
            var rc = new MxReturnCode<bool>($"EditingBaseController.LaunchBrowser");

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
