using System;
using System.Net.Sockets;
using System.Threading;
using BrokerEvent.Framework.Interfaces;
using BrokerEvent.Framework.Models;

namespace BrokerEvent.Framework.Services
{
    public class TcpServerProxySubscriber<TResource> : IServerProxySubscriber<TResource>
    {
        private readonly TcpClient _client;
        private readonly IEventChannel<TResource> _channel;

        public TcpServerProxySubscriber(TcpClient client, IEventChannel<TResource> channel)
        {
            _client = client;
            _channel = channel;
            var stream = client.GetStream();
            
            new Thread(() =>
            {
                while (true)
                {
                    while (_client.Available == 0) ;

                    Console.WriteLine("[ServerSubscriber] Sending resource to ServerPublisher");
                    
                    var data = Helpers.ReadFromStream(stream);
                    var resource = Helpers.FromByteArray<TResource>(data);
                    _channel.PublishResource(resource);
                }
            }).Start();
        }
    }
}