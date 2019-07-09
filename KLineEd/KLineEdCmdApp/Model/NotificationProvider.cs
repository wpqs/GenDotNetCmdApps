using System;
using System.Collections.Generic;

namespace KLineEdCmdApp.Model
{
    public class NotificationProvider : IObservable<NotificationItem>
    {
        private readonly List<IObserver<NotificationItem>> _observers;
        public NotificationProvider()
        {
            _observers = new List<IObserver<NotificationItem>>();
        }
        public IDisposable Subscribe(IObserver<NotificationItem> observer)
        {
            IDisposable rc = null;
            if ((_observers != null) && (observer != null))
            {
                if (_observers.Contains(observer) == false)
                {
                    _observers.Add(observer);
                    rc = new Unsubscriber(_observers, observer);
                }
            }
            return rc;
        }
        public void Notify( NotificationItem item)
        {
            if (item != null)
            {
                foreach (var observer in _observers)
                {
                    if (item.Unsubscribe)
                    {
                        observer.OnCompleted();
                        Notify(item);
                        break;
                    }
                    else if (item.ErrorMsg != null)
                        observer.OnError(new Exception(item.ErrorMsg ?? "[null]"));
                    else
                        observer.OnNext(new NotificationItem() {Change = item.Change, Data = item.Data});
                }
            }
        }
        public int GetSubscriberCount()
        {
            return _observers?.Count ?? KLineEditor.PosIntegerNotSet;
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<NotificationItem>> _observers;
            private IObserver<NotificationItem> _observer;
            private bool _disposing;
            public Unsubscriber(List<IObserver<NotificationItem>> observers, IObserver<NotificationItem> observer)
            {
                _observers = observers;
                _observer = observer;
                _disposing = false;
            }
            public void Dispose()
            {
                if ((_observer != null) && (_disposing == false))
                {
                    _disposing = true;
                    _observer.OnCompleted();
                    _observers.Remove(_observer);
                    _observer = null;
                }
            }
        }
    }
}
