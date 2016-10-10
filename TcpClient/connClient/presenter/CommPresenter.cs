using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using bitkyFlashresUniversal.connClient.model;
using bitkyFlashresUniversal.connClient.model.bean;
using bitkyFlashresUniversal.connClient.view;
using bitkyFlashresUniversal.databaseUtil.presenter;
using bitkyFlashresUniversal.ElectrodeDetection;

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

        public CommPresenter(IViewCommStatus view)
        {
            _view = view;
            _commucationFacade = new CommucationFacade(this);
            _sqlPresenter = new SqlPresenter(this);
            _poleDetectPresenter = new PoleDetectPresenter();
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
                case FrameType.HandshakeSwitchWifi:
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
                    _view.ControlMessageShow("收到数据子帧,编号:" + frameData.Note);
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
            _connConnected = false;
            _commucationFacade.SendDataFrame(new FrameData(FrameType.HvRelayClose));
            _commucationFacade.TcpClientFailed("UserCloseConn");
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
        public void StartWork()
        {
            switch (PresetInfo.CurrentOperateType)
            {
                case OperateType.Gather:
                    _currentFrameData = _sqlPresenter.GetFrameDataFromDb();
                    switch (_currentFrameData.Type)
                    {
                        case FrameType.None:
                            _view.ControlMessageShow("数据库检索已完成");
                            break;
                        case FrameType.ControlGather:
                            _commucationFacade.SendDataFrame(_currentFrameData);
                            break;
                        default:
                            _view.ControlMessageShow("未知错误");
                            break;
                    }
                    break;
                case OperateType.Detect:
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
                                _view.ControlMessageShow("电极检测完毕,无错误");
                            }
                            else
                            {
                                var builder = new StringBuilder();
                                electrodeInspect.BadList.ForEach(id => { builder.Append(id + " "); });
                                _view.ControlMessageShow("电极检测完毕,有异常");
                                _view.ControlMessageShow("选取好电极:" + electrodeInspect.NiceId + " 坏电极:" + builder);
                                if (_poleDetectPresenter._roundNum == 2) //第二轮从数据库获取不满足阈值的电极
                                {
                                    _view.CommunicateMessageShow("第二轮检测结束");
                                    return;
                                }
                                //第二轮检测初始化
                                _poleDetectPresenter.SetSecondRoundData(electrodeInspect);
                                StartWork();
                            }
                            break;
                        default:
                            _view.ControlMessageShow("未知错误");
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     通信信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        public void CommunicateMessageShow(string message)
        {
            _view.CommunicateMessageShow(message);
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
                    _sqlPresenter.InsertResultDataToDb(electrodes, _poleDetectPresenter.badId);
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
            _view.BitkyPoleShow(_electrodes);
            if (PresetInfo.CurrentOperateType == OperateType.Gather)
                StartWork();
            else
            {
                _poleDetectPresenter.OnceOperateComplete();
                StartWork();
            }
        }

        /// <summary>
        ///     根据预设的IP及端口号初始化TCP客户端
        /// </summary>
        public void InitTcpClient(string ip, int port)
        {
            _view.ConnConnecting();
            _commucationFacade.InitTcpClient(ip, port);
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

            switch (type)
            {
                case OperateType.Handshake:
                    if (CheckTable())
                    {
                        _commucationFacade.SendDataFrame(new FrameData(FrameType.HvRelayOpen));
                        Thread.Sleep(400);
                        _commucationFacade.SendDataFrame(new FrameData(FrameType.HandshakeSwitchWifi));
                    }
                    else
                        _view.ControlMessageShow("数据库初始化检查出错");
                    break;
                case OperateType.Gather:

                    if (CheckTable())
                    {
                        PresetInfo.CurrentOperateType = type;
                        StartWork();
                    }
                    else
                        _view.ControlMessageShow("数据库初始化检查出错");
                    break;

                case OperateType.Detect:
                    PresetInfo.CurrentOperateType = type;
                    StartWork();
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
    }
}