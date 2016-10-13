using System;
using System.Collections.Generic;
using System.Diagnostics;
using bitkyFlashresUniversal.connClient.model.bean;

namespace bitkyFlashresUniversal.ElectrodeDetection
{
    public class PoleDetectPresenter
    {
        public int StartId { set; get; } = 0;
        private int _endId = 63;
        public int _roundNum { set; get; } = 1;
        private int _niceId;
        public int badId { set; get; }
        List<int> _badList = new List<int>();

        public void Init()
        {
            StartId = 0;
            _endId = 63;
            _roundNum = 1;
        }

        public void OnceOperateComplete()
        {
            if (_roundNum == 1)
            {
                StartId = StartId + 2;
            }
            if (_roundNum == 2)
            {
                StartId++;
            }
        }

        public FrameData GetPoleList()
        {
            Debug.WriteLine("电极检测模式：" + "start:" + StartId + " end:" + _endId);
            if (_roundNum == 1)
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
            if (_roundNum == 2)
            {
                if (StartId >= _endId)
                    return new FrameData(FrameType.None);

                badId = _badList[StartId];
                var electrodes = new List<Electrode>
                {
                    new Electrode(_niceId, PoleMode.A),
                    new Electrode(badId, PoleMode.B)
                };

                return new FrameData(FrameType.ControlGather, electrodes);
            }
            throw new Exception("第二轮检测数据获取程序有误");
        }

        public void SetSecondRoundData(ElectrodeInspect electrodeInspect)
        {
            _roundNum = 2;
            _niceId = electrodeInspect.NiceId;
            StartId = 0;
            _endId = electrodeInspect.BadList.Count;
            _badList.Clear();
            _badList.AddRange(electrodeInspect.BadList);
        }
    }
}