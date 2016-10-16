using System.Collections.Generic;
using System.Diagnostics;
using bitkyFlashresUniversal.connClient.model.bean;

namespace bitkyFlashresUniversal.connClient.model.commtUtil
{
    /// <summary>
    ///     从TCP客户端获取帧并做第一步解析,解析为数据帧并返回
    /// </summary>
    public class FrameProvider
    {
        private readonly List<byte> _dataList = new List<byte>();


        /// <summary>
        ///     获取从TCP客户端接收到的数据
        /// </summary>
        /// <param name="dataBytes"></param>
        public FrameData ObtainData(byte[] dataBytes)
        {
            //开始程序
            _dataList.AddRange(dataBytes);
            int dataCount;
          
//            //接收到帧头(4个字节)
//            if ((_dataList.Count >= 4) && (_dataList.Count < 68))
//                switch (FrameTypeGather(_dataList.GetRange(0, 4).ToArray())) //获取帧头，并判断帧头类型
//                {
//                    case FrameType.HandshakeSwitchWifi: //获取到握手帧的帧头
//                        Debug.WriteLine("当前帧类型:握手帧的帧头");
//                        _dataList.Clear();
//                        return new FrameData(FrameType.HandshakeSwitchWifi);
//
//                    case FrameType.DeviceReset:
//                        Debug.WriteLine("当前帧类型:下位机重置帧的帧头");
//                        _dataList.Clear();
//                        return new FrameData(FrameType.DeviceReset);
//
//
//                    case FrameType.DataHeader: //获取到数据帧的帧头
//                        Debug.WriteLine("当前帧类型:数据帧的帧头");
//                        _dataList.Clear();
//                        return new FrameData(FrameType.DataHeader);
//
//                    case FrameType.None: //当前数据不是数据帧，直接清空
//                        _dataList.Clear();
//                        break;
//                }

            if (_dataList.Count == 340)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (FrameTypeGather(_dataList.GetRange(68*i, 4).ToArray()) == FrameType.None)
                    {
                        return new FrameData(FrameType.None);
                    }
                }


                var subFrame0 = _dataList.GetRange(0 + 4, 64);
                var subFrame1 = _dataList.GetRange(68 + 4, 64);
                var subFrame2 = _dataList.GetRange(136 + 4, 64);
                var subFrame3 = _dataList.GetRange(204 + 4, 64);
                var subFrame4 = _dataList.GetRange(272 + 4, 64);

                var poleList = new List<Electrode>();
                var list = new List<FrameData>
                {
                    ParseConnData(subFrame0),
                    ParseConnData(subFrame1),
                    ParseConnData(subFrame2),
                    ParseConnData(subFrame3),
                    ParseConnData(subFrame4)
                };

                list.ForEach(subFrame => { poleList.AddRange(subFrame.PoleList); });
                _dataList.Clear();
                return new FrameData(FrameType.ReturnDataGather) {PoleList = poleList, FrameId = -1};
            }
            _dataList.Clear();

//            if (_dataList.Count > 340)
//                _dataList.Clear();

            return new FrameData(FrameType.None);
        }

        /// <summary>
        ///     将获取到的数据解析为数据帧对象
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        private FrameData ParseConnData(List<byte> dataBytes)
        {
            var poleList = new List<Electrode>();
            for (var i = 0; i < dataBytes.Count; i = i + 4)
            {
                int id = dataBytes[i];
                var value = ControlFrameBuilder.BytesToInt3(
                    new[]
                    {
                        dataBytes[i + 1],
                        dataBytes[i + 2],
                        dataBytes[i + 3]
                    }
                );
                poleList.Add(new Electrode(id) {Value = value});
            }

            var note = dataBytes[0]/16; //数据子帧的编号
            return new FrameData(FrameType.ReturnDataGather) {PoleList = poleList, FrameId = note};
        }


        /// <summary>
        ///     从数据帧头内容获取并返回帧头类型
        /// </summary>
        /// <param name="bytes">数据帧头内容</param>
        /// <returns>数据帧头类型</returns>
        private FrameType FrameTypeGather(byte[] bytes)
        {
            if (CompareByte(bytes, CommMsg.HvRelaySubframeHeader))
                return FrameType.HvRelayOpen;
            if (CompareByte(bytes, CommMsg.CurrentReceiveSwitchFrame))
                return FrameType.HandshakeSwitchWifi;
            if (CompareByte(bytes, CommMsg.DeviceResetFrameHeader))
                return FrameType.DeviceReset;
            if (CompareByte(bytes, CommMsg.DataFrameHeader))
                return FrameType.DataHeader;
            if (CompareByte(bytes, CommMsg.ControlGatherFrameHeader))
                return FrameType.ControlGather;
            if (CompareByte(bytes, CommMsg.ActivateGatherFrameHeader))
                return FrameType.ActivateGather;
            if (CompareByte(bytes, CommMsg.ReturnFrameHeader))
                return FrameType.ReturnDataGather;

            return FrameType.None;
        }

        /// <summary>
        ///     byte数组比较方法，比较两数组是否相同
        /// </summary>
        /// <returns>相同吗？</returns>
        private bool CompareByte(byte[] byte1, byte[] byte2)
        {
            if (byte1.Length != byte2.Length)
                return false;
            for (var i = 0; i < byte1.Length; i++)
                if (byte1[i] != byte2[i])
                    return false;
            return true;
        }
    }
}