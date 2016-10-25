using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using bitkyFlashresUniversal.connClient.model;
using bitkyFlashresUniversal.connClient.model.bean;
using bitkyFlashresUniversal.databaseUtil.presenter;
using bitkyFlashresUniversal.ElectrodeDetection;
using bitkyFlashresUniversal.view;
using Timer = System.Timers.Timer;

namespace bitkyFlashresUniversal.connClient.presenter
{
    internal class CommPresenter : ICommPresenter
    {
        private readonly ICommucationFacade _commucationFacade; //通信接口外观类

        private readonly PoleDetectPresenter _poleDetectPresenter;
        private readonly ISqlPresenter _sqlPresenter;
        private readonly IViewCommStatus _view;
        private bool _connConnected;
        private FrameData _currentFrameData;
        private List<Electrode> _electrodes;
        private readonly Timer _timerSendDelay;


        public CommPresenter(IViewCommStatus view)
        {
            _view = view;
            _commucationFacade = new CommucationFacade(this);
            _sqlPresenter = new SqlPresenter(this);
            _poleDetectPresenter = new PoleDetectPresenter();

            //子帧收集计时器
            _timerSendDelay = new Timer(PresetInfo.FrameSendDelay) {AutoReset = false};
            _timerSendDelay.Elapsed += FrameSendDelay;
        }

        /// <summary>
        ///     建立连接成功，获取Socket成功的消息
        /// </summary>
        public void GetSocketSuccess()
        {
            _connConnected = true;
            _view.ConnConnected();
            Debug.WriteLine("获取Socket成功！");
        }

        /// <summary>
        ///     返回程序处理后的数据帧
        /// </summary>
        /// <param name="frameData">处理后的数据帧</param>
        public void SetFrameData(FrameData frameData)
        {
            switch (frameData.Type)
            {
                case FrameType.HandshakeSwitchDevice:
                    _view.ControlMessageShow("收到握手帧的帧头");
                    break;
                case FrameType.DeviceReset:
                    _view.ControlMessageShow("收到重置帧的帧头");
                    break;
                case FrameType.DataHeader:
                    _view.ControlMessageShow("收到数据帧的帧头");
                    break;
                case FrameType.ControlGather:
                    _view.ControlMessageShow("收到控制子帧的帧头");
                    break;
                case FrameType.ActivateGather:
                    _view.ControlMessageShow("收到启动子帧的帧头");
                    break;
                case FrameType.ReturnDataGather:
                    _view.ControlMessageShow("收到数据子帧,编号:" + frameData.FrameId);
                    break;
                case FrameType.HvRelayOpen:
                    _view.ControlMessageShow("收到高压继电器控制子帧的帧头");
                    break;

                default:
                    _view.ControlMessageShow("收到错误的信息");
                    break;
            }
        }

        /// <summary>
        ///     从前端关闭连接
        /// </summary>
        public void FrontConnClosed()
        {
            _commucationFacade.SendDataFrame(new FrameData(FrameType.HvRelayClose));
            _commucationFacade.CommClientFailed("UserCloseConn");
        }

        /// <summary>
        ///     连接断开回调函数
        /// </summary>
        public void ConnFailed(string evenType)
        {
            _connConnected = false;
            _view.ConnDisconnected();
            _view.ControlMessageShow("连接失效:" + evenType);
        }

        /// <summary>
        ///     开启工作流程
        /// </summary>
        private void StartWork()
        {
            switch (PresetInfo.CurrentOperateType)
            {
                case OperateType.Gather:
                    _currentFrameData = _sqlPresenter.GetFrameDataFromDb();
                    switch (_currentFrameData.Type)
                    {
                        case FrameType.None:
                            _view.CommunicateMessageShow("数据库检索已完成");
                            MessageBox.Show("数据采集已完成！", "提示");
                            break;
                        case FrameType.ControlGather:
                            _timerSendDelay.Interval = PresetInfo.FrameSendDelay;
                            _timerSendDelay.Start();
                            break;
                        default:
                            _view.CommunicateMessageShow("未知错误");
                            break;
                    }
                    break;
                case OperateType.Detect:
                case OperateType.Detect2:
                    _currentFrameData = _poleDetectPresenter.GetPoleList();
                    switch (_currentFrameData.Type)
                    {
                        case FrameType.ControlGather:
                            _commucationFacade.SendDataFrame(_currentFrameData);
                            break;

                        case FrameType.None:
                            var electrodeInspect = _sqlPresenter.SecondRroundInspect();
                            if (electrodeInspect.NiceId == -1)
                            {
                                _view.CommunicateMessageShow("电极检测完毕,无错误");
                                _view.InitPoleSelection(new List<int>());
                            }
                            else
                            {
                                var builder = new StringBuilder();
                                electrodeInspect.BadList.ForEach(id => { builder.Append(id + " "); });
                                _view.CommunicateMessageShow(string.Format("电极检测完毕,坏电极个数为:{0},结果如下:",
                                    electrodeInspect.BadList.Count));
                                _view.CommunicateMessageShow("好电极:" + electrodeInspect.NiceId + " 坏电极:" + builder);
                                if (electrodeInspect.BadList.Count > 32)
                                {
                                    _view.CommunicateMessageShow("电极检测完毕,坏电极较多，程序终止");
                                    return;
                                }
                                if (_poleDetectPresenter.RoundNum == 2) //第二轮从数据库获取不满足阈值的电极
                                {
                                    _view.CommunicateMessageShow("第二轮检测结束");
                                    _view.InitPoleSelection(new List<int>(electrodeInspect.BadList));
                                    return;
                                }
                                else
                                {
                                    _view.CommunicateMessageShow("第一轮检测结束, 已开始第二轮检测");
                                }
                                //第二轮检测初始化
                                _poleDetectPresenter.SetSecondRoundData(electrodeInspect);
                                PresetInfo.CurrentOperateType = OperateType.Detect2;
                                StartWork();
                            }
                            break;
                        default:
                            _view.CommunicateMessageShow("未知错误");
                            break;
                    }
                    break;

                case OperateType.Debug:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 帧发送延时事件回调函数
        /// </summary>
        private void FrameSendDelay(object source, ElapsedEventArgs e)
        {
            _commucationFacade.SendDataFrame(_currentFrameData);
        }

        /// <summary>
        ///     通信信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        public void CommunicateMessageShow(string message)
        {
            _view.CommunicateMessageShow(message);
        }

        /// <summary>
        ///     发送帧信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        public void SendDataShow(string message)
        {
            _view.SendDataShow(message);
        }

        /// <summary>
        ///     接收帧信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        public void ReceiveDataShow(string message)
        {
            _view.ReceiveDataShow(message);
        }

        public void InsertDataIntoDb(List<Electrode> electrodes)
        {
            _electrodes = electrodes;
            switch (PresetInfo.CurrentOperateType)
            {
                case OperateType.Gather:
                    _sqlPresenter.InsertResultDataToDb(electrodes, 0);
                    break;
                case OperateType.Detect:
                    _sqlPresenter.InsertResultDataToDb(electrodes, _poleDetectPresenter.StartId);
                    break;
                case OperateType.Detect2:
                    _sqlPresenter.InsertResultDataToDb(electrodes, _poleDetectPresenter.BadId);
                    break;
                case OperateType.Debug:
                    Debug.WriteLine("一次电极调试完成");
                    _view.BitkyPoleShow(_electrodes);
                    DeviceGatherStart(OperateType.Debug);
                    break;
            }
        }

        /// <summary>
        ///     数据库数据轮廓信息显示
        /// </summary>
        /// <param name="message"></param>
        public void DataOutlineShow(string message)
        {
            _view.DataOutlineShow(message);
        }

        /// <summary>
        ///     采集的数据插入数据库完成，开始下一次采集
        /// </summary>
        public void InsertDataIntoDbComplete()
        {
            Debug.WriteLine("插入数据库完成");
            _view.BitkyPoleShow(_electrodes);
            if (!PresetInfo.StartAutoCollect)
            {
                return;
            }
            if (PresetInfo.CurrentOperateType == OperateType.Gather)
                StartWork();
            else
            {
                _poleDetectPresenter.OnceOperateComplete();
                StartWork();
            }
        }

        /// <summary>
        ///     使用指定的IP地址和端口号构建TCP客户端
        /// 或  使用指定的串口名和波特率构建串口客户端
        /// </summary>
        /// <param name="str"></param>
        /// <param name="num"></param>
        public void InitCommClient(string str, int num)
        {
            _commucationFacade.InitCommClient(str, num);
        }

        /// <summary>
        ///     检查数据表的正确性，并显示轮廓信息
        /// </summary>
        /// <returns></returns>
        public bool CheckTable()
        {
            return _sqlPresenter.CheckTable();
        }

        public void DeviceGatherStart(OperateType type)
        {
            if (!_connConnected)
                return;

            PresetInfo.CurrentOperateType = type;
            switch (type)
            {
                case OperateType.Handshake:
                    if (CheckTable())
                    {
                        _commucationFacade.SendDataFrame(new FrameData(FrameType.HandshakeSwitchDevice));
                    }
                    else
                        _view.ControlMessageShow("数据库初始化检查出错");
                    break;
                case OperateType.Gather:

                    if (CheckTable())
                    {
                        StartWork();
                    }
                    else
                        _view.ControlMessageShow("数据库初始化检查出错");
                    break;

                case OperateType.Detect:
                    _poleDetectPresenter.Init();
                    _sqlPresenter.ElectrodDetectInit();
                    CheckTable();
                    StartWork();
                    break;
                case OperateType.HvRelayOpen:
                    _commucationFacade.SendDataFrame(new FrameData(FrameType.HvRelayOpen));
                    break;
                case OperateType.DeviceReset:
                    _commucationFacade.SendDataFrame(new FrameData(FrameType.DeviceReset));

                    break;
                case OperateType.Debug:
                    _commucationFacade.SendDataFrame(_currentFrameData);
                    break;
                default:
                    _view.CommunicateMessageShow("设备运行时启动了无法识别的帧");
                    break;
            }
        }

        /// <summary>
        ///     清空已采集的数据
        /// </summary>
        public void GatherDataClear()
        {
            _poleDetectPresenter.Init();
            _sqlPresenter.ElectrodDetectInit();
            _sqlPresenter.GatherDataClear();
            CheckTable();
        }

        /// <summary>
        ///     获取配置信息
        /// </summary>
        public void GetPreferences()
        {
            _sqlPresenter.GetPreferences();
            _view.SetPreferencesData();
        }

        /// <summary>
        /// 调试用，直接指定发送帧
        /// </summary>
        /// <param name="frameData"></param>
        public void DebugPole(FrameData frameData)
        {
            _currentFrameData = frameData;
            DeviceGatherStart(OperateType.Debug);
        }

        /// <summary>
        /// 在数据库中更新配置信息
        /// </summary>
        public void UpdatePreferences()
        {
            _sqlPresenter.UpdatePreferences();
        }
    }
}