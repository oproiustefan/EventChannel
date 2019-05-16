namespace BrokerEvent.Framework.Services
{
    public class AvailablePortProvider
    {
        private int port;
        public AvailablePortProvider(int startingPort)
        {
            this.port = startingPort;
        }

        public int GetPort()
        {
            return this.port++;
        }
    }
}