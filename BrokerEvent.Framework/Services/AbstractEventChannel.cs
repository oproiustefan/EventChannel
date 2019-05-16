using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using BrokerEvent.Framework.Interfaces;
using BrokerEvent.Framework.Models;

namespace BrokerEvent.Framework.Services
{
    public abstract class AbstractEventChannel<TResource> : IEventChannel<TResource>
    {
        private readonly List<IServerProxyPublisher<TResource>> _publishers;
        public AbstractEventChannel()
        {
            _publishers = new List<IServerProxyPublisher<TResource>>();
        }

        public void RegisterPublisher(IServerProxyPublisher<TResource> publisher)
        {
            _publishers.Add(publisher);
        }

        public IServerProxyPublisher<TResource> UnregisterPublisher(Address address)
        {
            var publisher = _publishers.FirstOrDefault(p => p.Address.Equals(address));
            _publishers.Remove(publisher);
            Console.WriteLine($"[EventChannel] Unregistered subscriber {publisher.Address}");
            return (IServerProxyPublisher<TResource>)publisher;
        }

        public void PublishResource(TResource resource)
        {
            foreach(var publisher in _publishers)
            {
                publisher.NotifySubscriber(resource);
            }
        }
    }
}