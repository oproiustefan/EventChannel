using System.Net.Sockets;
using BrokerEvent.Framework.Models;

namespace BrokerEvent.Framework.Interfaces
{
    public interface IEventChannel<TResource>
    {
        void RegisterPublisher(IServerProxyPublisher<TResource> publisher);
        IServerProxyPublisher<TResource> UnregisterPublisher(Address address);
        void PublishResource(TResource resource);
    }
}