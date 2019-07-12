using System;
using KLineEdCmdApp.Model;

namespace KLineEdCmdApp.Controller.Base
{
    public abstract class ModeProc
    {
        public virtual KLineEditor.CmdMode ProcessKey(ConsoleKey key, ChapterModel model)
        {
            //do common stuff with key - let override in derived class do the rest 
            return KLineEditor.CmdMode.Unknown;  //or KLineEditor.CmdMode.Error
        }
    }
}
