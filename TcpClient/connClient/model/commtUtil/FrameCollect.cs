using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using bitkyFlashresUniversal.connClient.model.bean;

namespace bitkyFlashresUniversal.connClient.model.commtUtil
{
    /// <summary>
    /// 子帧收集器
    /// </summary>
    public class FrameCollect
    {
        private bool _isGetDataFrameHeader;
        private int _subframeSum;
        private int _subframeNum;
        private readonly ICommucationFacade _facade;
        private FrameType _type;
        private TimeoutStopThread _timeoutThread;
        public List<Electrode> Electrodes { get; } = new List<Electrode>();

        public FrameCollect(ICommucationFacade facade)
        {
            _facade = facade;
        }

        public void Init(FrameType type)
        {
            _type = type;
            _subframeNum = 0;
            switch (type)
            {
                case FrameType.ControlGather:
                    _isGetDataFrameHeader = false;
                    _subframeSum = PresetInfo.ModuleNum;
                    break;
                case FrameType.ActivateGather:
                    _isGetDataFrameHeader = false;
                    _subframeSum = PresetInfo.ModuleNum + 1;
                    break;
                case FrameType.ReturnDataGather:
                    _isGetDataFrameHeader = true;
                    _subframeSum = PresetInfo.ModuleNum + 1;
                    Electrodes.Clear();
                    break;
            }
            _timeoutThread = new TimeoutStopThread(this);
            new Thread(_timeoutThread.Run).Start();
        }

        public void TimeoutConnFailed()
        {
           // _facade.TcpClientFailed("TimeoutConnFailed");
           _facade.FrameReceiveTimeout();
        }

        public void GetDataFrameHeader()
        {
            _isGetDataFrameHeader = true;
        }

        public void SaveSubframe(List<Electrode> list)
        {
            if (_type != FrameType.ReturnDataGather)
            {
                Debug.WriteLine("步骤错误，无法保存子帧");
                return;
            }
            Electrodes.AddRange(list);
        }

        /// <summary>
        /// 接收任务是否成功
        /// </summary>
        /// <returns></returns>
        public bool ReceiveComplete()
        {
            return _isGetDataFrameHeader && _subframeNum == _subframeSum;
        }

        public void GetSubframe(FrameType type)
        {
            if (type != _type)
                return;
            _subframeNum++;
            Debug.WriteLine("已收到子帧个数：" + _subframeNum);
            if (ReceiveComplete())
            {
                _timeoutThread.ReceiveComplete = true;
                _facade.GetsubframeComplete(_type);
            }
        }
    }
}