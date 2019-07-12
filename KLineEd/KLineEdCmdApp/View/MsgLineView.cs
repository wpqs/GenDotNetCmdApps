using KLineEdCmdApp.Controller;
using KLineEdCmdApp.Model;
using MxReturnCode;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;

namespace KLineEdCmdApp.View
{
    public class MsgLineView : KLineEdBaseView
    {
        public MsgLineView(ITerminal terminal) : base(terminal)
        {
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            if (((ChapterModel.ChangeHint)notificationItem.Change) == ChapterModel.ChangeHint.Msg)
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                Terminal.SetCursorPosition(KLineEditor.MsgLineRow, KLineEditor.MsgLineLeftCol);
                var msg = model?.MsgLine ?? Program.ValueNotSet;
                LastTerminalOutput = Terminal.Write((msg.Length <= WindowWidth) ? msg : (msg.Substring(0, WindowWidth-Program.ValueOverflow.Length) + Program.ValueOverflow));
            }
        }
    }
}
