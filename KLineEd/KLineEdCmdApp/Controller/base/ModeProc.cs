using System;
using KLineEdCmdApp.Model;

namespace KLineEdCmdApp.Controller.Base
{
    public abstract class ModeProc
    {
        public virtual KLineEditor.OpMode ProcessKey(ConsoleKey key, ChapterModel model)
        {
            //do common stuff with key - let override in derived class do the rest 
            return KLineEditor.OpMode.Unknown;  //or KLineEditor.CmdMode.Error
        }
    }
}
