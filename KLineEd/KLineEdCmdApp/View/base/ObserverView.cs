using System;
using KLineEdCmdApp.Utils;

namespace KLineEdCmdApp.View.Base
{
    public abstract class ObserverView :  IObserverView<NotificationItem>
    {
        public abstract void OnUpdate(NotificationItem notificationItem);

        private IDisposable _unsubscriber;
        private bool _unsubscribing;

        protected ObserverView()
        {
            _unsubscriber = null;
            _unsubscribing = false;
        }

        public IDisposable SetObserverDisposer(IDisposable unsubscriber)
        {
            return _unsubscriber = unsubscriber;
        }
        protected bool IsUnsubscribing()
        {
            return _unsubscribing;
        }
        protected virtual bool Unsubscribe()
        {
            var rc = false;
            if ((_unsubscriber != null) && (_unsubscribing == false))
            {
                _unsubscribing = true;
                _unsubscriber.Dispose();
                rc = true;
            }
            return rc;
        }
        public void OnNext(NotificationItem notificationItem)
        {
            OnUpdate(notificationItem);     //the derived class handles the update with the more appropriately named method OnUpdate()
        }
        public virtual void OnCompleted()
        {
            if (_unsubscribing == false)
                Unsubscribe();
        }
        public virtual void OnError(Exception error)
        {
           
        }
    }
}
