using System.Threading;
using bitkyFlashresUniversal.connClient.model.bean;

namespace bitkyFlashresUniversal.connClient.model.commtUtil
{
    public class TimeoutStopThread
    {
        private readonly FrameCollect _frameCollect;
        public bool ReceiveComplete { set; private get; } = false;

        public TimeoutStopThread(FrameCollect frameCollect)
        {
            _frameCollect = frameCollect;
        }

        public void Run()
        {
            Thread.Sleep(PresetInfo.ReceiveTimeout);
            if (_frameCollect.ReceiveComplete() || ReceiveComplete)
                return;
            _frameCollect.TimeoutConnFailed();
        }
    }
}