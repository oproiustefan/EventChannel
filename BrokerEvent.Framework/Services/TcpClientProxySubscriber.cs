using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BrokerEvent.Framework.Interfaces;
using BrokerEvent.Framework.Models;

namespace BrokerEvent.Framework.Services
{
    public class TcpClientProxySubscriber<TResource> : IClientProxySubscriber<TResource>, IDisposable
    {
        private readonly Address _address;
        private readonly CancellationTokenSource _cts;
        private readonly HashSet<Action<TResource>> _callbacks;
        private readonly TcpClient _client;

        public TcpClientProxySubscriber(Address address)
        {
            _address = address;
            _cts = new CancellationTokenSource();
            _callbacks = new HashSet<Action<TResource>>();
            _client = new TcpClient(_address.IP, _address.Port);

            var stream = _client.GetStream();
            Helpers.WriteToStream(stream, Message.Subscribe);

            new Thread(() =>
            {
                bool cancelled = false;
                while (true)
                {
                    while (_client.Available == 0)
                    {
                        if (_cts.IsCancellationRequested)
                        {
                            cancelled = true;
                            break;
                        }
                    }

                    if (cancelled)
                    {
                        break;
                    }
                    
                    Console.WriteLine("[ClientSubscriber] Received resource from ServerPublisher");
                    
                    var data = Helpers.ReadFromStream(stream);
                    var resource = Helpers.FromByteArray<TResource>(data);
                    foreach (var c in _callbacks)
                    {
                        c(resource);
                    }
                }
            }).Start();
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        public void Subscribe(Action<TResource> callback)
        {
            _callbacks.Add(callback);
        }

        public void Unsubscribe()
        {
            _callbacks.RemoveWhere(r => true);
            _cts.Cancel(false);
            Helpers.WriteToStream(_client.GetStream(), Message.Unsubscribe);
        }
    }
}