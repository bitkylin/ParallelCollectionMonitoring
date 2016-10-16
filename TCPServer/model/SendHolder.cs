using System.Threading;
using TCPServer.util;

namespace TCPServer.model
{
    internal class SendHolder
    {
        private readonly BitkyTcpServer _client;

        public SendHolder(BitkyTcpServer client)
        {
            _client = client;
        }

        public void SendDelayed(byte[] bytes, int timeInterval)
        {
            new Thread(() =>
            {
                Thread.Sleep(timeInterval);
                _client.Send(bytes);
            }).Start();
        }
    }
}