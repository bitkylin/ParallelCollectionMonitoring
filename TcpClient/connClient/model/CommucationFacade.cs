using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
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

        private readonly FrameProvider _frameProvider; //远程数据初步解析器，解析为数据帧
        private readonly BitkyTcpClient _myTcpClient; //TCP客户端
        private readonly ICommPresenter _presenter; //通信接口表现层

        private byte[] _controlMsgCurrent;
        private FrameType _frameTypeCurrent = FrameType.None;

        private Timer _timerFrameCollect;
        private Timer _timerActivate;
        private readonly List<Electrode> _electrodes = new List<Electrode>();

        public CommucationFacade(ICommPresenter presenter)
        {
            _presenter = presenter;
            _myTcpClient = new BitkyTcpClient(this);
            _controlFrameBuilder = new ControlFrameBuilder();
            _frameProvider = new FrameProvider();

            //子帧收集计时器
            _timerFrameCollect = new Timer(5000) {AutoReset = false};
            _timerFrameCollect.Elapsed += FrameCollect;
            _timerActivate = new Timer(2000) {AutoReset = false};
            _timerActivate.Elapsed += FrameCollectActivate;
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
            }
            _presenter.SetFrameData(frameData);
            switch (frameData.Type)
            {
                case FrameType.HandshakeSwitchWifi:
                    _presenter.CommunicateMessageShow("收到握手帧的回复");
                    break;

                case FrameType.ReturnDataGather:
                    _electrodes.AddRange(frameData.PoleList);
                    Debug.WriteLine("已接收到并解析成功数据子帧");
                    break;

                case FrameType.HvRelayOpen:
                    _presenter.CommunicateMessageShow("高压继电器控制成功");
                    break;
            }
        }

        /// <summary>
        ///     发送指定的帧
        /// </summary>
        /// <param name="frameData">指定的帧格式</param>
        public void SendDataFrame(FrameData frameData)
        {
            switch (frameData.Type)
            {
                case FrameType.HvRelayOpen:
                    _myTcpClient.Send(CommMsg.HvRelayOpen);
                    break;
                case FrameType.HvRelayClose:
                    _myTcpClient.Send(CommMsg.HvRelayClose);
                    break;
                case FrameType.HandshakeSwitchWifi:
                    _myTcpClient.Send(CommMsg.HandshakeSwitchWifiFrameHeader);
                    break;
                case FrameType.ControlGather:
                    _controlMsgCurrent = _controlFrameBuilder.DataFrameBuild(frameData);
                    _myTcpClient.Send(_controlMsgCurrent);
                    _timerActivate.Start();

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 子帧采集完成事件回调
        /// </summary>
        void FrameCollect(object source, ElapsedEventArgs e)
        {
            if (_electrodes.Count >= 79)
                _presenter.InsertDataIntoDb(_electrodes);
            else
            {
                _presenter.CommunicateMessageShow("未接收到正确的子帧数据");
            }
        }

        /// <summary>
        /// 启动帧发送激活事件回调
        /// </summary>
        void FrameCollectActivate(object source, ElapsedEventArgs e)
        {
            _myTcpClient.Send(CommMsg.ActivateGatherFrameFill);
            _electrodes.Clear();
            _timerFrameCollect.Start();
        }
    }
}