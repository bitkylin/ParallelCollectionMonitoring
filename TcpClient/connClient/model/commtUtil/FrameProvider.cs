using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
            var dataCount = _dataList.Count;
            Debug.WriteLine("数量:" + dataCount);

            //接收到帧头(4个字节)
            if ((_dataList.Count >= 4) && (_dataList.Count < 68))
                switch (FrameTypeGather(_dataList.GetRange(0, 4).ToArray())) //获取帧头，并判断帧头类型
                {
                    case FrameType.HandshakeSwitchWifi: //获取到握手帧的帧头
                        Debug.WriteLine("当前帧类型:握手帧的帧头");
                        _dataList.Clear();
                        return new FrameData(FrameType.HandshakeSwitchWifi);

                    case FrameType.DeviceReset:
                        Debug.WriteLine("当前帧类型:下位机重置帧的帧头");
                        _dataList.Clear();
                        return new FrameData(FrameType.DeviceReset);


                    case FrameType.DataHeader: //获取到数据帧的帧头
                        Debug.WriteLine("当前帧类型:数据帧的帧头");
                        _dataList.Clear();
                        return new FrameData(FrameType.DataHeader);

                    case FrameType.None: //当前数据不是数据帧，直接清空
                        _dataList.Clear();
                        break;
                }

            if (_dataList.Count >= 68)
            {
                var dataList = _dataList.GetRange(0, 68);
                _dataList.Clear();
                switch (FrameTypeGather(dataList.GetRange(0, 4).ToArray())) //获取帧头，并判断帧头类型
                {
                    case FrameType.ControlGather:
                        Debug.WriteLine("当前帧类型:控制帧");
                        return new FrameData(FrameType.ControlGather);

                    case FrameType.ActivateGather:
                        Debug.WriteLine("当前帧类型:启动帧");
                        return new FrameData(FrameType.ActivateGather);

                    case FrameType.ReturnDataGather:
                        Debug.WriteLine("当前帧类型:接收到的数据子帧");
                        return ParseConnData(dataList.GetRange(4, 64));

                    case FrameType.HvRelayOpen:
                        Debug.WriteLine("当前帧类型:高压继电器控制子帧");
                        return new FrameData(FrameType.HvRelayOpen);

                }
            }
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

            var note = dataBytes[0]/16;
            Debug.WriteLine("数据子帧解析成功,编号:" + note);
            return new FrameData(FrameType.ReturnDataGather) {PoleList = poleList, Note = note.ToString()};
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
            if (CompareByte(bytes, CommMsg.HandshakeSwitchWifiFrameHeader))
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