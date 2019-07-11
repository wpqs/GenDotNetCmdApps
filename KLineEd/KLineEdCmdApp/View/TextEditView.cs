using System.Diagnostics.CodeAnalysis;
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

        public override MxReturnCode<bool> Setup(CmdLineParamsApp param)
        {
            var rc = new MxReturnCode<bool>("TextEditView.Setup");

            var rcBase = base.Setup(param);
            rc += rcBase;
            if (rcBase.IsSuccess(true))
            {

                rc.SetResult(true);
            }
            return rc;
        }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            ChapterModel.ChangeHint change = (ChapterModel.ChangeHint)notificationItem.Change;
            if ((change == ChapterModel.ChangeHint.All) || (change == ChapterModel.ChangeHint.Char) || (change == ChapterModel.ChangeHint.Word))
            {
                //this View is only concerned with changes to Words and Characters so update it from the relevant objects in notification.Data

            }
        }

        // ReSharper disable once RedundantOverriddenMember
        protected override bool Unsubscribe()
        {
            return base.Unsubscribe();
            //any actions that need to be done for the derived class during Unsubscribe - i.e. display message
        }

        public void WriteEditLine(string line)
        {
            var padding = "";
            Terminal.Write(padding.PadLeft(KLineEditor.ScreenMarginLeft));
            Terminal.WriteLines(line);
        }
    }
}