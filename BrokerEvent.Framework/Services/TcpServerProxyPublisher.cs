using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BrokerEvent.Framework.Interfaces;
using BrokerEvent.Framework.Models;

namespace BrokerEvent.Framework.Services
{
    public class TcpServerProxyPublisher<TResource> : IServerProxyPublisher<TResource>
    {
        private readonly Address _clientAddress;
        private readonly TcpClient _client;
        private readonly IEventChannel<TResource> _channel;


        public TcpServerProxyPublisher(TcpClient client, IEventChannel<TResource> channel)
        {
            _client = client;
            _channel = channel;
            var endpoint = (IPEndPoint) _client.Client.RemoteEndPoint;
            _clientAddress = new Address(endpoint.Address.ToString(), endpoint.Port);
            
            new Thread(() =>
            {
                while (true)
                {
                    while (_client.Available == 0) ;
                    
                    var stream = _client.GetStream();
                    var data = Helpers.ReadFromStream(stream);
                    var message = Helpers.FromByteArray<Message>(data);
                    if (message == Message.Unsubscribe)
                    {
                        _channel.UnregisterPublisher(_clientAddress);
                    }
                }
            }).Start();
        }

        public bool Equals(IServerProxyPublisher<TResource> other)
        {
            var publisher = other as TcpServerProxyPublisher<TResource>;
            if (publisher == null)
            {
                return false;
            }
            return this._clientAddress.Equals(publisher._clientAddress);
        }

        public void NotifySubscriber(TResource resource)
        {
            var stream = _client.GetStream();
            Console.WriteLine("[ServerPublisher] Sending resource to ClientSubscriber");
            Helpers.WriteToStream(stream, resource);
        }

        public void DetatchFromSubscriber()
        {
            _client.Close();
        }

        public Address Address => _clientAddress;

    }
}