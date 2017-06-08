using System;
using System.Collections.Generic;
using System.Diagnostics;
using bitkyFlashresUniversal.connClient.model.bean;

namespace bitkyFlashresUniversal.ElectrodeDetection
{
    public class PoleDetectPresenter
    {
        public int StartId { private set; get; }
        private int _endId = 63;
        public int RoundNum { private set; get; } = 1;
        private int _niceId;
        public int BadId { private set; get; }
        private readonly List<int> _badList = new List<int>();

        public void Init()
        {
            StartId = 0;
            _endId = 63;
            RoundNum = 1;
        }

        public void OnceOperateComplete()
        {
            if (RoundNum == 1)
            {
                StartId = StartId + 2;
            }
            if (RoundNum == 2)
            {
                StartId++;
            }
        }

        public FrameData GetPoleList()
        {
            Debug.WriteLine("电极检测模式：" + "start:" + StartId + " end:" + _endId);
            if (RoundNum == 1)
            {
                if (StartId >= _endId)
                {
                    return new FrameData(FrameType.None);
                }
                var electrodes = new List<Electrode>
                {
                    new Electrode(StartId, PoleMode.A),
                    new Electrode(StartId + 1, PoleMode.B)
                };

                return new FrameData(FrameType.ControlGather, electrodes);
            }
            if (RoundNum == 2)
            {
                if (StartId >= _endId)
                    return new FrameData(FrameType.None);

                BadId = _badList[StartId];
                var electrodes = new List<Electrode>
                {
                    new Electrode(_niceId, PoleMode.A),
                    new Electrode(BadId, PoleMode.B)
                };

                return new FrameData(FrameType.ControlGather, electrodes);
            }
            throw new Exception("第二轮检测数据获取程序有误");
        }

        public void SetSecondRoundData(ElectrodeInspect electrodeInspect)
        {
            RoundNum = 2;
            _niceId = electrodeInspect.NiceId;
            StartId = 0;
            _endId = electrodeInspect.BadList.Count;
            _badList.Clear();
            _badList.AddRange(electrodeInspect.BadList);
        }
    }
}