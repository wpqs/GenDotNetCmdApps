using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Properties;
using KLineEdCmdApp.Utils;
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
            BrowserCmd = CmdLineParamsApp.ArgToolBrowserCmdDefault;
            HelpUrl = CmdLineParamsApp.ArgToolHelpUrlDefault;
            SearchUrl = CmdLineParamsApp.ArgToolSearchUrlDefault;
            ThesaurusUrl = CmdLineParamsApp.ArgToolThesaurusUrlDefault;
            SpellUrl = CmdLineParamsApp.ArgToolSpellUrlDefault;
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
                Chapter = model;
                BrowserCmd = browserCmd;
                HelpUrl = helpUrl;
                SearchUrl = searchUrl;
                ThesaurusUrl = thesaurusUrl;
                SpellUrl = spellUrl;
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
                _mxErrorCode.SetError(1250201, MxError.Source.Param, $"model is null or keyInfo null", MxMsgs.MxErrBadMethodParam);
            else
            {
                var ctrlKeyPressed = ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0);

                if ((ctrlKeyPressed) && (keyInfo.Key == ConsoleKey.Q))
                    _ctrlQ = true;
                else if (ctrlKeyPressed &&(keyInfo.Key == ConsoleKey.S))
                {
                    var rcSave = Chapter.Save();
                    if (rcSave.IsError(true))
                        SetupMxError(rcSave);
                }
                else if (keyInfo.Key == ConsoleKey.F1)
                    LaunchBrowser(model?.ChapterBody, HelpUrl, false);
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

        protected void LaunchBrowser(Body body, string url, bool replaceSelectedWord = false)
        {
            if ((body == null) || (string.IsNullOrEmpty(url)))
                SetMxError(1240101, MxError.Source.Param, "body of url is null", MxMsgs.MxErrBadMethodParam);
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
                            SetMxError(1240102, MxError.Source.User, $"no word found at cursor row={(body?.Cursor?.RowIndex ?? Program.PosIntegerNotSet)}, col=row={(body?.Cursor?.ColIndex ?? Program.PosIntegerNotSet)}", MxMsgs.MxErrNoWordSelected);
                        else
                        {
                            url = KLineEditor.GetXlatUrl(url, CmdLineParamsApp.UrlWordMarker, selectedWord);
                            if (KLineEditor.IsValidUri(url) == false)
                                SetMxError(1240103, MxError.Source.User, $"word={selectedWord} found at cursor row={(body?.Cursor?.RowIndex ?? Program.PosIntegerNotSet)}, col=row={(body?.Cursor?.ColIndex ?? Program.PosIntegerNotSet)} cannot be contained in a url", MxMsgs.MxErrInvalidWordSelected);
                            else
                            {
                                KLineEditor.StartBrowser(BrowserCmd, url);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    SetMxError(1240104, MxError.Source.User, e.Message, null);
                }
            }
        }

        public virtual string GetEditorHelpLine()
        {
            return $"Unsupported: Esc=Refresh F1=Help Ctrl+Q=Quit Ctrl+S=Save";
        }
    }
}
