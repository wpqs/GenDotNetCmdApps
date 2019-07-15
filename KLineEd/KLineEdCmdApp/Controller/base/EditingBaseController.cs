using System;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using MxReturnCode;

namespace KLineEdCmdApp.Controller.Base
{
    public abstract class EditingBaseController
    {
        public ITerminal Terminal { set; get; }

        public ChapterModel Chapter { private set; get; }

        public bool Ready { private set; get; }

        public EditingBaseController()
        {
            Terminal = null;
            Chapter = null;
            Ready = false;
        }

        public virtual MxReturnCode<bool> Initialise(ITerminal terminal, ChapterModel model)
        {
            var rc = new MxReturnCode<bool>("EditingBaseController.Initialise");

            if ((terminal?.IsError() ?? true) || ((model?.Ready ?? false) == false))
                rc.SetError(1030101, MxError.Source.Param, $"param is null or editModel null, not ready", "MxErrBadMethodParam");
            else
            {
                Terminal = terminal;
                Chapter = model;
                Ready = true;
                rc.SetResult(true);
            }
            return rc;
        }

        public virtual EditingBaseController ProcessKey(ConsoleKey key, ChapterModel model)
        {
            //do common stuff with key - let override in derived class do the rest 

            return this;  
        }

        public virtual string GetModeHelpLine()
        {
            return $"Unsupported: Esc=Refresh F1=Help Ctrl+Q=Quit";
        }
    }
}
