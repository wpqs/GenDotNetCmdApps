using System;
using KLineEdCmdApp.Model;

namespace KLineEdCmdApp.View
{
    // ReSharper disable once UnusedTypeParameter
    public interface IObserverView<T> : IObserver<NotificationItem>
    {
        void OnUpdate(NotificationItem notificationItem);
        IDisposable SetObserverDisposer(IDisposable dispose);
    }
}
