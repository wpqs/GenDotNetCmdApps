using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Controller;
using MxReturnCode;

using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;

namespace KLineEdCmdApp.View
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]

    public class TextEditView : KLineEdBaseView   
    {
        // ReSharper disable once RedundantBaseConstructorCall
        public TextEditView(ITerminal terminal) : base(terminal)
        {
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            ChapterModel.ChangeHint change = (ChapterModel.ChangeHint)notificationItem.Change;
            if ((change == ChapterModel.ChangeHint.All) || (change == ChapterModel.ChangeHint.Char) || (change == ChapterModel.ChangeHint.Word))
            {
                ChapterModel model = notificationItem.Data as ChapterModel;

                Terminal.SetCursorPosition(KLineEditor.EditAreaTopRow, KLineEditor.EditAreaMarginLeft);
                LastTerminalOutput = Terminal.Write(model.GetLastLinesForDisplay(1).GetResult()?[0] ?? Program.ValueNotSet);

                //this View is only concerned with changes to Words and Characters so update it from the relevant objects in notification.Data

            }
        }
    }
}