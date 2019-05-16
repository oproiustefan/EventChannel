using System;
using System.Collections.Generic;
using System.Threading;
using BrokerEvent.Framework.Interfaces;
using BrokerEvent.Framework.Models;
using BrokerEvent.Framework.Services;


namespace BrokerEvent.BookLibrary
{
    [Serializable]
    class Book
    {
        public string Title { get; set; }
    }

    class Reader
    {
        private readonly string _name;
        private readonly List<IClientProxySubscriber<Book>> _subscriptions;

        public Reader(string name)
        {
            _name = name;
            _subscriptions = new List<IClientProxySubscriber<Book>>();
        }

        public void AddSubscription(IClientProxySubscriber<Book> subscription)
        {
            subscription.Subscribe(ReadBook);
            _subscriptions.Add(subscription);
        }

        public void RemoveSubscription(IClientProxySubscriber<Book> subscription)
        {
            subscription.Unsubscribe();
            _subscriptions.Remove(subscription);
        }

        private void ReadBook(Book book) => Console.WriteLine($"[Reader: {_name}] Reads {book.Title}");
    }

    class PublishingHouse
    {
        private readonly IClientProxyPublisher<Book> _proxyPublisher;

        public PublishingHouse(IClientProxyPublisher<Book> proxyPublisher)
        {
            _proxyPublisher = proxyPublisher;
        }

        public void Publish(Book book) => _proxyPublisher.Publish(book);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var thrillerChannelAddress = new Address("127.0.0.1", 13000);
            var actionChannelAddress = new Address("127.0.0.1", 13001);

            new Thread(() => new TcpEventChannel<Book>(thrillerChannelAddress)).Start();
            new Thread(() => new TcpEventChannel<Book>(actionChannelAddress)).Start();

            Thread.Sleep(1000);

            var john = new Reader("John");

            var actionProxySubscriber = new TcpClientProxySubscriber<Book>(actionChannelAddress);
            john.AddSubscription(new TcpClientProxySubscriber<Book>(thrillerChannelAddress));
            john.AddSubscription(actionProxySubscriber);

            var zoe = new Reader("Zoe");
            zoe.AddSubscription(new TcpClientProxySubscriber<Book>(actionChannelAddress));

            var thrillerPublishingHouse =
                new PublishingHouse(new TcpClientProxyPublisher<Book>(thrillerChannelAddress));
            var actionPublishingHouse = new PublishingHouse(new TcpClientProxyPublisher<Book>(actionChannelAddress));

            Thread.Sleep(500);
            Console.WriteLine("----- Publishing Thriller and Action");
            thrillerPublishingHouse.Publish(new Book {Title = "Thriller 1"});
            actionPublishingHouse.Publish(new Book {Title = "Action 1"});

            Thread.Sleep(500);
            john.RemoveSubscription(actionProxySubscriber);
            Thread.Sleep(500);
            Console.WriteLine("----- Publishing Action");
            actionPublishingHouse.Publish(new Book {Title = "Action 2"});

            Console.ReadKey();
        }
    }
}