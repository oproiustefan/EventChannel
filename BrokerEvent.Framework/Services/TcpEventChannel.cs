using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using BrokerEvent.Framework.Models;

namespace BrokerEvent.Framework.Services
{
    public class TcpEventChannel<TResource> : AbstractEventChannel<TResource>
    {
        public TcpEventChannel(Address address) : base()
        {
            var addr = IPAddress.Parse(address.IP);
            TcpListener server = null;
            try
            {
                server = new TcpListener(addr, address.Port);
                server.Start();
                while (true)
                {
                    var client = server.AcceptTcpClient();

                    var stream = client.GetStream();

                    var data = Helpers.ReadFromStream(stream);
                    var message = Helpers.FromByteArray<Message>(data);

                    switch (message)
                    {
                        case Message.Subscribe:
                        {
                            var publisher = new TcpServerProxyPublisher<TResource>(client, this);
                            this.RegisterPublisher(publisher);
                            Console.WriteLine($"[EventChannel] Registered subscriber {publisher.Address}");
                            break;
                        }
                        case Message.Publish:
                        {
                            new TcpServerProxySubscriber<TResource>(client, this);
                            Console.WriteLine($"[EventChannel] Registered publisher");
                            break;
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}