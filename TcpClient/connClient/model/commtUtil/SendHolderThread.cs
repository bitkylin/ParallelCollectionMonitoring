using System.Threading;

namespace bitkyFlashresUniversal.connClient.model.commtUtil
{
    internal class SendHolderThread
    {
        private readonly byte[] _bytes;
        private readonly SendHolder _holder;

        private readonly int _timeInterval;
        public bool StopSending { private get; set; }

        public SendHolderThread(byte[] bytes, int timeInterval, SendHolder holder)
        {
            _bytes = bytes;
            _timeInterval = timeInterval;
            _holder = holder;
        }

        public void Run()
        {
            while (!StopSending)
            {
                _holder.Send(_bytes);
                Thread.Sleep(_timeInterval);
            }
        }
    }
}