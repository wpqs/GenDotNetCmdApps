using KLineEdCmdApp.Controller;
using KLineEdCmdApp.Model;
using MxReturnCode;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;


namespace KLineEdCmdApp.View
{
    public class StatusLineView : KLineEdBaseView
    {
        public StatusLineView(ITerminal terminal) : base(terminal)
        {
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            if (((ChapterModel.ChangeHint)notificationItem.Change) == ChapterModel.ChangeHint.Status)
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                Terminal.SetCursorPosition(WindowHeight-KLineEditor.StatusLineCount-1, KLineEditor.StatusLineLeftCol);
                var status = model?.StatusLine ?? Program.ValueNotSet;
                LastTerminalOutput = Terminal.Write((status.Length <= WindowWidth) ? status : (status.Substring(0, WindowWidth - Program.ValueOverflow.Length) + Program.ValueOverflow));
            }

        }
    }
}
