using System;
using System.Threading;
using BrokerEvent.Framework.Interfaces;
using BrokerEvent.Framework.Models;
using BrokerEvent.Framework.Services;

namespace BrokerEvent.NetworkManagement
{
    enum EventType
    {
        Status,
        Error,
        CriticalError
    }
    
    // Subscriber
    class ManagementObject
    {
        private readonly IClientProxySubscriber<EventType> _proxySubscriber;

        public ManagementObject(IClientProxySubscriber<EventType> proxySubscriber)
        {
            _proxySubscriber = proxySubscriber;
            proxySubscriber.Subscribe(Handle);
        }

        private void Handle(EventType eventType) => Console.WriteLine($"Handling event of type: {eventType.ToString()}");

        public void Unsubscribe() => _proxySubscriber.Unsubscribe();
    }
    
    // Publisher
    class ManagedObject
    {
        private readonly IClientProxyPublisher<EventType> _proxyPublisher;

        public ManagedObject(IClientProxyPublisher<EventType> proxyPublisher)
        {
            _proxyPublisher = proxyPublisher;
        }

        public void Dispatch(EventType eventType)
        {
            _proxyPublisher.Publish(eventType);
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            var address = new Address("127.0.0.1", 13000);
            new Thread(() => new TcpEventChannel<EventType>(address)).Start();
            
            Thread.Sleep(1000);
            var publisher = new ManagedObject(new TcpClientProxyPublisher<EventType>(address));
            
            var sub1 = new ManagementObject(new TcpClientProxySubscriber<EventType>(address));
            var sub2 = new ManagementObject(new TcpClientProxySubscriber<EventType>(address));
            Thread.Sleep(500);
            
            publisher.Dispatch(EventType.Error);
            publisher.Dispatch(EventType.CriticalError);
            publisher.Dispatch(EventType.Status);
            publisher.Dispatch(EventType.Error);
            
            Thread.Sleep(500);
            
            sub1.Unsubscribe();
            
            Thread.Sleep(500);
            Console.WriteLine("Dispatching event to one subscriber only.");
            
            publisher.Dispatch(EventType.Error);

            Console.ReadKey();
        }
    }
}