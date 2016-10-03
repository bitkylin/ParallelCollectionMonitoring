using System;
using System.Diagnostics;
using bitkyFlashresUniversal.connClient.model.bean;
using bitkyFlashresUniversal.connClient.model.commtUtil;
using bitkyFlashresUniversal.connClient.model.commtUtil.ConnClient;
using bitkyFlashresUniversal.connClient.presenter;

namespace bitkyFlashresUniversal.connClient.model
{
    /// <summary>
    ///     通信的外观模式实现类
    /// </summary>
    public class CommucationFacade : ICommucationFacade
    {
        private readonly ControlFrameBuilder _controlFrameBuilder; //控制帧构建器
        private readonly FrameCollect _frameCollect; //子帧收集状态显示器

        private readonly FrameProvider _frameProvider; //远程数据初步解析器，解析为数据帧
        private readonly BitkyTcpClient _myTcpClient; //TCP客户端
        private readonly ICommPresenter _presenter; //通信接口表现层

        private byte[] _controlMsgCurrent;
        private int _frameReceiveTimeout;
        private FrameType _frameTypeCurrent = FrameType.None;

        public CommucationFacade(ICommPresenter presenter)
        {
            _presenter = presenter;
            _myTcpClient = new BitkyTcpClient(this);
            _controlFrameBuilder = new ControlFrameBuilder();
            _frameProvider = new FrameProvider();
            _frameCollect = new FrameCollect(this);
        }

        /// <summary>
        ///     帧接收超时状态清零
        /// </summary>
        public void FrameReceiveTimeoutClear()
        {
            _frameReceiveTimeout = 0;
        }

        /// <summary>
        ///     使用指定的IP地址和端口号构建TCP客户端
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void InitTcpClient(string ip, int port)
        {
            _myTcpClient.Build(ip, port);
        }

        /// <summary>
        ///     TCP连接失效
        /// </summary>
        public void TcpClientFailed(string evenType)
        {
            _myTcpClient.Close();
            _presenter.ConnFailed(evenType);
            _frameTypeCurrent = FrameType.None;
        }

        /// <summary>
        ///     建立连接成功，获取Socket成功的消息
        /// </summary>
        public void GetSocketSuccess()
        {
            _presenter.GetSocketSuccess();
        }

        /// <summary>
        ///     从TCP客户端获取已接收到的数据
        /// </summary>
        /// <param name="data">获取的远程数据</param>
        public void GetReceivedData(byte[] data)
        {
            var frameData = _frameProvider.ObtainData(data);
            SetFrameData(frameData);
        }

        /// <summary>
        ///     返回程序处理后的数据帧
        /// </summary>
        /// <param name="frameData">处理后的数据帧</param>
        public void SetFrameData(FrameData frameData)
        {
            //维护帧类型：握手帧、数据帧头、返回数据帧头
            if (frameData.Type != FrameType.None)
            {
                _frameTypeCurrent = FrameType.None;
                _myTcpClient.GetCallback();
            }
            switch (frameData.Type)
            {
                case FrameType.HandshakeSwitchWifi:
                    _presenter.ControlMsg();
                    break;
                case FrameType.DeviceReset:
                    _presenter.DeviceGatherStart(PresetInfo.CurrentOperateType);
                    break;
                case FrameType.DataHeader:
                    _frameCollect.GetDataFrameHeader();
                    break;
                case FrameType.ControlGather:
                    _frameCollect.GetSubframe(FrameType.ControlGather);
                    break;
                case FrameType.ActivateGather:
                    _frameCollect.GetSubframe(FrameType.ActivateGather);
                    break;
                case FrameType.ReturnDataGather:
                    _frameCollect.SaveSubframe(frameData.PoleList);
                    _frameCollect.GetSubframe(FrameType.ReturnDataGather);
                    Debug.WriteLine("已接收到并解析成功数据子帧");
                    break;

                case FrameType.HvRelayOpen:
                    _presenter.CommunicateMessageShow("高压继电器控制成功");
                    break;
            }
            _presenter.SetFrameData(frameData);
        }

        /// <summary>
        ///     发送指定的帧
        /// </summary>
        /// <param name="frameData">指定的帧格式</param>
        public void SendDataFrame(FrameData frameData)
        {
            //维护帧类型：握手帧、数据帧头、返回数据帧头
            if (_frameTypeCurrent != FrameType.None)
            {
                Debug.WriteLine("还未获得回应，本次数据发送无效");
                return;
            }

            switch (frameData.Type)
            {
                case FrameType.HvRelayOpen:
                    _myTcpClient.Send(CommMsg.HvRelayOpen, PresetInfo.SendInterval);
                    break;
                case FrameType.HvRelayClose:
                    _myTcpClient.Send(CommMsg.HvRelayClose, PresetInfo.SendInterval);
                    break;
                case FrameType.HandshakeSwitchWifi:
                    _myTcpClient.Send(CommMsg.HandshakeSwitchWifiFrameHeader, PresetInfo.SendInterval);
                    _frameTypeCurrent = FrameType.HandshakeSwitchWifi;
                    break;
                case FrameType.DeviceReset:
                    _myTcpClient.Send(CommMsg.DeviceResetFrame, PresetInfo.SendInterval);
                    _frameTypeCurrent = FrameType.DeviceReset;
                    break;
                case FrameType.ControlGather:
                    _frameCollect.Init(FrameType.ControlGather);
                    _controlMsgCurrent = _controlFrameBuilder.DataFrameBuild(frameData);
                    _myTcpClient.Send(_controlMsgCurrent, PresetInfo.SendInterval);
                    _frameTypeCurrent = FrameType.ControlGather;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     下位机接收数据超时
        /// </summary>
        public void FrameReceiveTimeout()
        {
            _frameReceiveTimeout++;
//            if (_frameReceiveTimeout >= PresetInfo.FrameReceiveTimeoutMaxCount)
//            {
            _presenter.CommunicateMessageShow("对设备进行重置");
//                _frameReceiveTimeout = 0;
//                _presenter.DeviceGatherStart(OperateType.DeviceReset);
//                return;
//            }
//            _presenter.CommunicateMessageShow("下位机帧接收超时，程序已重置");
//            _presenter.DeviceGatherStart(OperateType.Gather);

            SendDataFrame(new FrameData(FrameType.DeviceReset));
        }

        /// <summary>
        ///     收集子帧完成回调方法
        /// </summary>
        public void GetsubframeComplete(FrameType type)
        {
            switch (type)
            {
                case FrameType.ControlGather:
                    var startFrameMsg = CommMsg.ActivateGatherFrameFill;
                    _frameCollect.Init(FrameType.ActivateGather);
                    _myTcpClient.Send(startFrameMsg, PresetInfo.SendInterval);
                    _frameTypeCurrent = FrameType.ActivateGather;
                    break;
                case FrameType.ActivateGather:
                    _frameCollect.Init(FrameType.ReturnDataGather);
                    _frameTypeCurrent = FrameType.ReturnDataGather;
                    break;
                case FrameType.ReturnDataGather:
                    Debug.WriteLine("采集子帧完毕,电极个数：" + _frameCollect.Electrodes.Count);
                    _presenter.InsertDataIntoDb(_frameCollect.Electrodes);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}