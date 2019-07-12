using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Controller;
using KLineEdCmdApp.Model;
using MxReturnCode;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;


namespace KLineEdCmdApp.View
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class CmdsHelpView : KLineEdBaseView
    {
        public CmdsHelpView(ITerminal terminal) : base(terminal)
        {

        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            if (((ChapterModel.ChangeHint)notificationItem.Change) == ChapterModel.ChangeHint.Cmd)
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                Terminal.SetCursorPosition(KLineEditor.CmdsHelpLineRow, KLineEditor.CmdsHelpLineLeftCol);

                var cmds = model?.CmdsHelpLine ?? Program.ValueNotSet;
                LastTerminalOutput = Terminal.Write((cmds.Length <= WindowWidth) ? cmds : (cmds.Substring(0, WindowWidth - Program.ValueOverflow.Length) + Program.ValueOverflow));
            }
        }
    }
}
