using System;

namespace BrokerEvent.Framework.Models
{
    [Serializable]
    public class Address : IEquatable<Address>
    {
        public string IP { get; set; }
        public int Port { get; set; }

        public Address(string ip, int port)
        {
            this.IP = ip;
            this.Port = port;
        }
        
        public override bool Equals(object obj)
        {
            var other = obj as Address;
            return (other != null && other.IP == this.IP && other.Port == this.Port);
        }

        public bool Equals(Address other)
        {
            return (other != null && other.IP == this.IP && other.Port == this.Port);
        }

        public override string ToString()
        {
            return $"{IP}:{Port}";
        }
    }
}