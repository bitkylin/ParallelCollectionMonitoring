using System.Threading;
using TCPDemo.util;

namespace TCPDemo.model
{
    internal class SendHolder
    {
        private readonly BitkyTcpServer _client;
        private bool _stopSending;

        public SendHolder(BitkyTcpServer client)
        {
            _client = client;
        }

        public void Send(byte[] bytes, int timeInterval)
        {
            new Thread(() =>
            {
                while (!_stopSending)
                {
                    if (_client.Send(bytes))
                    {
                        Thread.Sleep(timeInterval);
                    }
                    else
                        break;
                }
                _stopSending = false;
            }).Start();
        }

        public void SendDelayed(byte[] bytes, int timeInterval)
        {
            new Thread(() =>
            {
                Thread.Sleep(timeInterval);
                _client.Send(bytes);
            }).Start();
        }



        public void GetCallback(bool value)
        {
            _stopSending = true;
        }
    }
}
