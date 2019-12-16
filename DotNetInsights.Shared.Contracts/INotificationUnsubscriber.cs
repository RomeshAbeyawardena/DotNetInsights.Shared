using System;

namespace DotNetInsights.Shared.Contracts
{
    public interface INotificationUnsubscriber : IDisposable
    {
        void Unsubscribe();
    }
}
