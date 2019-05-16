using System;

namespace BrokerEvent.Framework.Models
{
    [Serializable]
    public enum Message
    {
        Subscribe,
        Unsubscribe,
        Publish
    }
}