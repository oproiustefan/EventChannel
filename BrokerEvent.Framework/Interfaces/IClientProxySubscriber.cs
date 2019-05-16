using System;

namespace BrokerEvent.Framework.Interfaces
{
    public interface IClientProxySubscriber<TResource>
    {
        void Subscribe(Action<TResource> callback);
        void Unsubscribe();
    }
}