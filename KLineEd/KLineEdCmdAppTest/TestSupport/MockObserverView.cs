using System;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.View;
// ReSharper disable All

namespace KLineEdCmdAppTest.TestSupport
{
    public class MockObserverView :  ObserverView
    {
        public string WindowTitle { private set; get; }
        public string WindowFooter { private set; get; }
        public MockObserverView() : base()
        {
            WindowTitle = "[not set]";
            WindowFooter = "[not set]";
        }
        public override void OnUpdate(NotificationItem notificationItem)
        {
            MockNotifierModel.ChangeHint change = (MockNotifierModel.ChangeHint) notificationItem.Change;
            if ((change == MockNotifierModel.ChangeHint.All) || (change == MockNotifierModel.ChangeHint.Char) || (change == MockNotifierModel.ChangeHint.Word))
            {
                if (notificationItem.Data is MockNotifierModel data)
                {
                    WindowTitle = data?.Msg ?? "[not set]";
                    WindowFooter = "updated from model";
                }
            }
        }
        public override void OnError(Exception msg)
        {
            base.OnError(msg);
            WindowFooter = (msg != null) ? msg.Message : "[null]";
        }
        public override void OnCompleted()
        {
            if (IsUnsubscribing() == false)
                WindowFooter = "unsubscribing...";
            base.OnCompleted();
        }
        protected override bool Unsubscribe()
        {
            if (IsUnsubscribing() == false)
                WindowFooter = $"view does not subscribe to model";
            return base.Unsubscribe();
        }
    }
}
