using System;
using System.Net.Sockets;
using BrokerEvent.Framework.Interfaces;
using BrokerEvent.Framework.Models;

namespace BrokerEvent.Framework.Services
{
    public class TcpClientProxyPublisher<TResource> : IClientProxyPublisher<TResource>, IDisposable
    {
        private readonly TcpClient _client;

        public TcpClientProxyPublisher(Address address)
        {
            _client = new TcpClient(address.IP, address.Port);
            var stream = _client.GetStream();
            Helpers.WriteToStream(stream, Message.Publish);
        }

        public void Dispose()
        {
            _client.Close();
        }

        public void Publish(TResource resource)
        {
            var stream = _client.GetStream();
            Console.WriteLine("[ClientPublisher] Sending resource to ServerSubscriber");
            Helpers.WriteToStream(stream, resource);
        }
    }
}