using System;

using KLineEdCmdApp.Utils;

namespace KLineEdCmdApp.View.Base
{
    // ReSharper disable once UnusedTypeParameter
    public interface IObserverView<T> : IObserver<NotificationItem>
    {
        void OnUpdate(NotificationItem notificationItem);
        IDisposable SetObserverDisposer(IDisposable dispose);
    }
}
