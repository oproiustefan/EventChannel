namespace BrokerEvent.Framework.Interfaces
{
    public interface IClientProxyPublisher<TResource>
    {
        void Publish(TResource resource);
    }
}