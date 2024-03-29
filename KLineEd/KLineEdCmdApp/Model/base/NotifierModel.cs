﻿using System;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;

namespace KLineEdCmdApp.Model.Base
{
    public abstract class NotifierModel
    {
        private NotificationProvider Provider { set; get; }

        protected NotifierModel()
        {
            Provider = new NotificationProvider();
        }
        public IDisposable Subscribe(IObserverView<NotificationItem> observer)
        {
            return observer?.SetObserverDisposer(Provider?.Subscribe(observer));
        }
        public int GetSubscriberCount()
        {
            return Provider?.GetSubscriberCount() ?? Program.PosIntegerNotSet;
        }
        public virtual void UpdateAllViews(int change)
        {
            Provider?.Notify(new NotificationItem { Data = this, Change = change });
        }
        protected virtual void NotifyErrorAllViews(string msg)
        {
            Provider?.Notify(new NotificationItem { ErrorMsg = msg });
        }
        public virtual void DisconnectAllViews()
        {
            Provider?.Notify(new NotificationItem { Unsubscribe = true });
        }
    }
}
