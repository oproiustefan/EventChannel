using System;
using BrokerEvent.Framework.Models;

namespace BrokerEvent.Framework.Interfaces
{
    public interface IServerProxyPublisher<TResource> : IEquatable<IServerProxyPublisher<TResource>>
    {
        Address Address { get; }
        void NotifySubscriber(TResource resource);
        void DetatchFromSubscriber();
    }
}