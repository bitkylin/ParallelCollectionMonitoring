using System.Threading;
using bitkyFlashresUniversal.connClient.model.commtUtil.ConnClient;

namespace bitkyFlashresUniversal.connClient.model.commtUtil
{
    /// <summary>
    /// 设定发送信息的响应间隔，未在规定时间内响应将采取进一步行动
    /// </summary>
    internal class SendHolder
    {
        private readonly BitkyClient _client;
        private Thread _thread;
        private SendHolderThread _holderThread;


        public SendHolder(BitkyClient client)
        {
            _client = client;
        }

        public void Send(byte[] bytes)
        {
            _client.Send(bytes);
        }

        public void Send(byte[] bytes, int timeInterval)
        {
            if (_thread == null || !_thread.IsAlive)
            {
                _holderThread = new SendHolderThread(bytes, timeInterval, this);
                _thread = new Thread(_holderThread.Run);
                _thread.Start();
            }
            else
            {
                _holderThread.StopSending = true;
                _holderThread = new SendHolderThread(bytes, timeInterval, this);
                _thread = new Thread(_holderThread.Run);
                _thread.Start();
            }
        }

        public void SendDelayed(byte[] bytes, int timeInterval)
        {
//            new Thread(() =>
//            {
//                Thread.Sleep(timeInterval);
//                _client.Send(bytes);
//            }).Start();
        }


        /// <summary>
        /// 持续发送的指令是否得到回馈
        /// </summary>
        public void GetCallback()
        {
            if (_holderThread != null)
            {
                _holderThread.StopSending = true;
            }
        }
    }
}