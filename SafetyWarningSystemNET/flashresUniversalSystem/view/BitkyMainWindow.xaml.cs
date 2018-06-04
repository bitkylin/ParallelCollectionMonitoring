using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using bitkyFlashresUniversal.cloud;
using bitkyFlashresUniversal.cloud.bean;
using bitkyFlashresUniversal.connClient.model;
using bitkyFlashresUniversal.connClient.model.bean;
using bitkyFlashresUniversal.connClient.presenter;
using bitkyFlashresUniversal.dataExport;
using bitkyFlashresUniversal.ElectrodeSelecter;
using bitkyFlashresUniversal.poleInfoShow;
using cn.bmob.api;
using PresetInfo = bitkyFlashresUniversal.connClient.PresetInfo;

namespace bitkyFlashresUniversal.view
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BitkyMainWindow : IViewCommStatus
    {
        private readonly ICommPresenter _commPresenter;
        private BitkyPoleControl[] _bitkyPoleControls;

        /// <summary>
        /// 当前已采集的次数
        /// </summary>
        int currentCollectNum = 0;

        /// <summary>
        /// 当前已采集的总次数
        /// </summary>
        int currentCollectSumNum = 0;

        public BitkyMainWindow()
        {
            InitializeComponent();
            Console.WriteLine("程序开启");
            _commPresenter = new CommPresenter(this);
            if (!_commPresenter.CheckTable())
                LabelDataOutlineShow.Content = "请使用电极选择器选择待测电极";

            InitBitkyPoleShow();
            InitWifiSerialPortShow();
            _commPresenter.GetPreferences();
        }

        /// <summary>
        ///     数据库数据轮廓信息显示
        /// </summary>
        /// <param name="message"></param>
        public void DataOutlineShow(string message)
        {
            Dispatcher.Invoke(() => { LabelDataOutlineShow.Content = message; });
        }

        /// <summary>
        ///     控制信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        public void ControlMessageShow(string message)
        {
            Dispatcher.Invoke(() =>
            {
                ListBoxControlText.Items.Add(message);
                ListBoxControlText.SelectedIndex = ListBoxControlText.Items.Count - 1;
                ListBoxControlText.ScrollIntoView(ListBoxControlText.Items.CurrentItem);
            });
        }

        /// <summary>
        ///     通信信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        public void CommunicateMessageShow(string message) //通信信息
        {
            Dispatcher.Invoke(() =>
            {
                ListBoxCommunicationText.Items.Add(message);
                ListBoxCommunicationText.SelectedIndex = ListBoxCommunicationText.Items.Count - 1;

                ListBoxCommunicationText.ScrollIntoView(
                    ListBoxCommunicationText.Items[ListBoxCommunicationText.Items.Count - 1]);
            });
        }

        /// <summary>
        ///     发送帧信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        public void SendDataShow(string message)
        {
            Dispatcher.Invoke(() =>
            {
                ListBoxSendData.Items.Add(message);
                ListBoxSendData.SelectedIndex = ListBoxSendData.Items.Count - 1;
            });
        }

        /// <summary>
        ///     接收帧信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        public void ReceiveDataShow(string message)
        {
            Dispatcher.Invoke(() =>
            {
                ListBoxReceiveData.Items.Add(message);
                ListBoxReceiveData.SelectedIndex = ListBoxReceiveData.Items.Count - 1;
            });
        }

        /// <summary>
        ///     网络连接已建立
        /// </summary>
        public void ConnConnected()
        {
            Dispatcher.Invoke(() =>
            {
                switch (PresetInfo.CurrentCommType)
                {
                    case CommType.Wifi:
                        BtnConnect.Content = "断开";
                        BtnConnect.IsEnabled = true;
                        BtnPort.IsEnabled = false;

                        break;
                    case CommType.SerialPort:
                        BtnPort.Content = "关闭串口";
                        BtnPort.IsEnabled = true;
                        BtnConnect.IsEnabled = false;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        /// <summary>
        ///     网络连接已断开
        /// </summary>
        public void ConnDisconnected()
        {
            Dispatcher.Invoke(() =>
            {
                BtnConnect.IsEnabled = true;
                BtnPort.IsEnabled = true;
                switch (PresetInfo.CurrentCommType)
                {
                    case CommType.Wifi:
                        BtnConnect.Content = "连接";
                        break;
                    case CommType.SerialPort:
                        BtnPort.Content = "打开串口";
                        break;
                }
            });
        }

        /// <summary>
        ///     在界面上显示电极完整信息
        /// </summary>
        /// <param name="electrodes"></param>
        public void BitkyPoleShow(List<Electrode> electrodes)
        {
            var ints = new List<int>();
            for (var i = 0; i < 64; i++)
                ints.Add(i);
            Dispatcher.Invoke(() =>
            {
                electrodes.ForEach(pole =>
                {
                    var id = pole.IdOrigin;
                    if ((id >= 0) && (id <= 63))
                    {
                        _bitkyPoleControls[id].SetValue(Math.Round(pole.Value, 4));
                        _bitkyPoleControls[id].SetColor(0);
                        ints.Remove(id);
                    }
                    if (id == 64)
                        LabelElectricShow.Content = Math.Round(pole.Value, 4);
                });
                ints.ForEach(i =>
                {
                    _bitkyPoleControls[i].SetValue(-1);
                    _bitkyPoleControls[i].SetColor(1);
                });
            });
        }

        /// <summary>
        /// 在view中显示配置信息, 初始化系统设置标签页面
        /// </summary>
        public void SetPreferencesData()
        {
            TextBoxElectricThreshold.Text = PresetInfo.ElectricThreshold.ToString(CultureInfo.CurrentCulture);
            TextBoxFrameReceiveTimeout.Text = PresetInfo.FrameReceiveTimeout.ToString();
            TextBoxFrameSendDelay.Text = PresetInfo.FrameSendDelay.ToString();
        }

        /// <summary>
        /// 将电极检测结果返回View，返回坏电极的集合
        /// </summary>
        public void InitPoleSelection(List<int> badList)
        {
            Dispatcher.Invoke(() =>
            {
                if (badList.Count == 0)
                {
                    if (MessageBox.Show("电极检测结束, 所有电极均有效, 系统已自动勾选全部电极, 确定后进行电极选择。", "电极检测结束", MessageBoxButton.OK) ==
                        MessageBoxResult.OK)
                        new ElectrodeSelecterWindow(this).Show();
                }
                else
                {
                    if (
                        MessageBox.Show(
                            "电极检测结束, 其中" + badList.Count + "个电极无效, 系统已自动勾选其余" + (64 - badList.Count) +
                            "个电极, 确定后打开电极选择器, 自主进行电极选择。",
                            "电极检测结束",
                            MessageBoxButton.OKCancel) ==
                        MessageBoxResult.OK)
                        new ElectrodeSelecterWindow(this, badList).Show();
                }
            });
        }

        /// <summary>
        ///     初始化电极信息显示标签界面
        /// </summary>
        private void InitBitkyPoleShow()
        {
            var controls = new List<BitkyPoleControl>();
            GridPoleStatusShow.Children.Clear();
            var id = 0;
            for (var i = 0; i < 8; i++)
                for (var j = 0; j < 8; j++)
                {
                    var bitkyPoleControl = new BitkyPoleControl();
                    //在Grid中动态添加控件
                    GridPoleStatusShow.Children.Add(bitkyPoleControl);
                    //设定控件在Grid中的位置
                    Grid.SetRow(bitkyPoleControl, i);
                    Grid.SetColumn(bitkyPoleControl, j);
                    //将控件添加到集合中，方便下一步的使用
                    controls.Add(bitkyPoleControl);
                    //对控件使用自定义方法进行初始化
                    bitkyPoleControl.SetContent(id);
                    id++;
                }
            _bitkyPoleControls = controls.ToArray();

            //初始化已启用的电极，并将信息保存在presenter中
            var enabledPolesList = new List<Electrode>(64);
            for (var i = 0; i < 64; i++)
            {
                enabledPolesList.Add(new Electrode(i));
            }
            _commPresenter.EnabledPoleList = enabledPolesList;
        }


        /// <summary>
        ///     电极信息初始化成功
        /// </summary>
        /// <param name="electrodes">使用的电极的集合</param>
        public void SetElectrodeSuccessful(List<Electrode> electrodes)
        {
            foreach (var control in _bitkyPoleControls)
            {
                control.SetInvaild();
                var id = int.Parse(control.LabelPoleId.Content.ToString());
                foreach (var pole in electrodes)
                {
                    if (pole.IdOrigin == id)
                    {
                        control.SetVaild();
                    }
                }
            }
            _commPresenter.CheckTable();
            //初始化已启用的电极，并将信息保存在presenter中
            _commPresenter.EnabledPoleList = electrodes;
            PresetInfo.currentObjectId = null;
        }

        public void DataOutlineShow(int v, int sumNun)
        {
            currentCollectNum = v;
            currentCollectSumNum = sumNun;
            string s = "已采集次数: " + (v - 1) + " 总次数: " + sumNun;
            Dispatcher.Invoke(() => { LabelDataOutlineShow.Content = s; });
        }

        /// <summary>
        /// 初始化TCP、串口相关环境配置
        /// </summary>
        private void InitWifiSerialPortShow()
        {
            var ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            ComboPortName.Items.Clear();
            //初始化串口名称下拉列表框
            foreach (var port in ports)
                ComboPortName.Items.Add(port);
            //初始化波特率下拉列表框
            ComboPortName.SelectedIndex = ComboPortName.Items.Count - 1;
            ComboBaudrate.SelectedIndex = ComboBaudrate.Items.Count - 1;

            //初始化IP地址下拉列表框
            var addresses = new List<IPAddress>();
            var addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (var ipAddress in addressList)
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    addresses.Add(ipAddress);
            addresses.ForEach(ipAddress => { ComboBoxIp.Items.Add(ipAddress); });
            ComboBoxIp.SelectedIndex = ComboBoxIp.Items.Count > 0 ? 0 : -1;
        }


        /// <summary>
        ///     建立连接按钮点击事件
        /// </summary>
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            PresetInfo.CurrentCommType = CommType.Wifi;
            if (BtnConnect.Content.ToString().Equals("断开"))
            {
                _commPresenter.FrontConnClosed();
                return;
            }
            var ip = ComboBoxIp.Text.Trim();
            var portStr = TextBoxPort.Text.Trim();
            var match = Regex.IsMatch(ip,
                @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
            if (Regex.IsMatch(portStr, @"^[1-9]\d*$"))
            {
                var port = int.Parse(portStr);
                if ((port < 65536) && (port >= 1024) && match)
                {
                    BtnConnect.Content = "正在连接";
                    BtnConnect.IsEnabled = false;
                    _commPresenter.InitCommClient(ip, port);
                }
                else
                    MessageBox.Show("请数入正确的IP地址和端口号!", "警告");
            }
        }

        private void btnHandshake_Click(object sender, RoutedEventArgs e)
        {
            if (PresetInfo.currentObjectId != null)
            {
                _commPresenter.DeviceGatherStart(OperateType.Gather);
                labelCollectStatus.Content = "正在进行数据采集";
                return;
            }

            CloudAreaItem item = new CloudAreaItem()
            {
                time = DateTime.Now,
                status = true,
                processStatus = 1,
                processBar = 0,
                photoUri = "http://img.bitlight.cc/printPhoto1.jpg",
                name = "测点「1」",
                enabled = true,
                detail = "未发现任何异常",
                coordinate = "110.343317,25.28906",
            };

            PresetInfo.bmobWindows.Create("CloudAreaItem", item, (result, ex) =>
            {
                if (ex == null)
                {
                    PresetInfo.currentObjectId = result.objectId;
                    Dispatcher.Invoke(() =>
                    {
                        _commPresenter.DeviceGatherStart(OperateType.Gather);
                        labelCollectStatus.Content = "正在进行数据采集";
                    });
                }
                else
                {
                    CloudDisconnected();
                }
            });
        }


        /// <summary>
        ///     开启电极选择器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenElectrodeSelectForm_Click(object sender, RoutedEventArgs e)
        {
            new ElectrodeSelecterWindow(this, new List<int>()).Show();
        }

        private void btnElectrodeDetect_Click(object sender, RoutedEventArgs e)
        {
            _commPresenter.DeviceGatherStart(OperateType.Detect);
            labelCollectStatus.Content = "正在检测节点状态";
        }

        private void BtnHandshakeStart_Click(object sender, RoutedEventArgs e)
        {
            _commPresenter.DeviceGatherStart(OperateType.Handshake);
        }

        private void BtnDeviceReset_Click(object sender, RoutedEventArgs e)
        {
            _commPresenter.DeviceGatherStart(OperateType.DeviceReset);
        }

        private void BtnHvRelayOpen_Click(object sender, RoutedEventArgs e)
        {
            _commPresenter.DeviceGatherStart(OperateType.HvRelayOpen);
        }

        private void btnCommunicateClear_Click(object sender, RoutedEventArgs e)
        {
            ListBoxCommunicationText.Items.Clear();
        }

        private void btnControlClear_Click(object sender, RoutedEventArgs e)
        {
            ListBoxControlText.Items.Clear();
        }

        private void btnSendDataClear_Click(object sender, RoutedEventArgs e)
        {
            ListBoxSendData.Items.Clear();
        }

        private void btnReceiveDataClear_Click(object sender, RoutedEventArgs e)
        {
            ListBoxReceiveData.Items.Clear();
        }

        private void btnGatherDataClear_Click(object sender, RoutedEventArgs e)
        {
            new DataExportWindow(_commPresenter).Show();
        }

        private void btnShowClear_Click(object sender, RoutedEventArgs e)
        {
            ListBoxCommunicationText.Items.Clear();
            ListBoxControlText.Items.Clear();
            ListBoxSendData.Items.Clear();
            ListBoxReceiveData.Items.Clear();
            LabelElectricShow.Content = "待获取";
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }


        /// <summary>
        ///     确认系统设置界面的设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonConfirmSetting_Click(object sender, RoutedEventArgs e)
        {
            PresetInfo.ElectricThreshold = double.Parse(TextBoxElectricThreshold.Text);
            PresetInfo.FrameReceiveTimeout = int.Parse(TextBoxFrameReceiveTimeout.Text);
            PresetInfo.FrameSendDelay = int.Parse(TextBoxFrameSendDelay.Text);
            _commPresenter.UpdatePreferences();
            MessageBox.Show("系统配置信息已更新成功!", "提示");
        }


        /// <summary>
        ///     刷新串口按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefreshPort_Click(object sender, RoutedEventArgs e)
        {
            InitWifiSerialPortShow();
        }

        /// <summary>
        ///     串口连接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPort_Click(object sender, RoutedEventArgs e)
        {
            PresetInfo.CurrentCommType = CommType.SerialPort;
            if (BtnPort.Content.ToString().Equals("打开串口"))
            {
                BtnPort.Content = "正在连接";
                BtnPort.IsEnabled = false;
                _commPresenter.InitCommClient(ComboPortName.Text, int.Parse(ComboBaudrate.Text));
            }
            else
            {
                _commPresenter.FrontConnClosed();
            }
        }

        private void checkBox_Clicked(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("change");
            PresetInfo.StartAutoCollect = CheckBoxStartAutoCollect.IsChecked.GetValueOrDefault(false);
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnMaximizedWindow_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void BtnMinimizedWindow_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Minimized)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private DispatcherTimer mDataTimer = null;

        /// <summary>
        /// 开启 Bmob 云服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCloudBmob_Click(object sender, RoutedEventArgs e)
        {
            if (BtnCloudBmob.Content.Equals("云服务已开启"))
            {
                CloudDisconnected();
                return;
            }
            PresetInfo.bmobWindows.Update("CloudDevice", "L3fM2226", new CloudDevice() { enabled = true }, (result2, exUpdate) => { });
            CheckBoxStartAutoCollect.IsChecked = true;
            PresetInfo.StartAutoCollect = true;
            mDataTimer = new DispatcherTimer();
            mDataTimer.Tick += new EventHandler(DataTimer_Tick);
            mDataTimer.Interval = TimeSpan.FromSeconds(5);
            mDataTimer.Start();
            labelCloudStatus.Content = "已建立连接";
            BtnCloudBmob.Content = "云服务已开启";
        }

        private void DataTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("定时器");
            if (PresetInfo.currentObjectId == null) { return; }
            PresetInfo.bmobWindows.Get<CloudAreaItem>("CloudAreaItem", PresetInfo.currentObjectId, (result, ex) =>
             {
                 if (ex == null)
                 {
                     CloudAreaItem item = new CloudAreaItem() { };
                     int statusCode = result.processStatus.Get();
                     int value = 0;
                     string statusShow = "";
                     if (statusCode == 1)
                     {
                         if (currentCollectSumNum > 0)
                             value = currentCollectNum * 100 / currentCollectSumNum;
                         item.processBar = value;
                         if (value == 100)
                         {
                             statusCode = 2;
                             value = 0;
                         }
                     }
                     else if (statusCode == 2)
                     {
                         value = result.processBar.Get() + new Random().Next(8, 15);
                         if (value >= 100) { statusCode = 3; value = 0; }
                     }
                     else
                     {
                         PresetInfo.currentObjectId = null;
                         return;
                     }

                     item.processStatus = statusCode;
                     item.processBar = value;
                     switch (statusCode)
                     {
                         case 1: statusShow = "正在数据采集「" + value + "%」"; break;
                         case 2: statusShow = "正在数据处理「" + value + "%」"; break;
                         default: statusShow = "任务执行完毕"; break;
                     }

                     Dispatcher.Invoke(() => { labelCollectStatus.Content = statusShow; });
                     PresetInfo.bmobWindows.Update("CloudDevice", "L3fM2226", new CloudDevice() { status = statusCode * 100 + value }, (result2, exUpdate) => { });
                     PresetInfo.bmobWindows.Update("CloudAreaItem", PresetInfo.currentObjectId, item, (result2, exUpdate) =>
                     {
                         if (exUpdate == null)
                         {
                             if (statusCode == 3)
                             {
                                 PresetInfo.currentObjectId = null;
                             }
                         }
                         else
                         {
                             CloudDisconnected();
                         }
                     });
                 }
                 else
                 {
                     CloudDisconnected();
                     PresetInfo.currentObjectId = null;
                 }
             });
        }

        private void CloudDisconnected()
        {
            PresetInfo.bmobWindows.Update("CloudDevice", "L3fM2226", new CloudDevice() { enabled = false }, (result2, exUpdate) => { });
            if (mDataTimer != null)
            {
                mDataTimer.Stop();
                mDataTimer = null;
            }
            Dispatcher.Invoke(() =>
            {
                CheckBoxStartAutoCollect.IsChecked = false;
                PresetInfo.StartAutoCollect = false;
                labelCloudStatus.Content = "未建立连接";
                labelCollectStatus.Content = "数据采集已停止";
                BtnCloudBmob.Content = "云服务已关闭";
                CommunicateMessageShow("云服务器已断开连接");
            });
        }


        private void BtnCloudReadDevice_Click(object sender, RoutedEventArgs e)
        {
            GridReadShow.Visibility = Visibility.Visible;
        }

        private void BtnMsgShow2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("探测深度更新成功", "更新成功");
        }

        private void BtnMsgShow_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("节点位置更新成功", "更新成功");
        }

        private void BtnlabelGeoShow_Click(object sender, RoutedEventArgs e)
        {
            labelGeoShow.Content = TextBoxlabelGeoShow.Text;
        }
    }
}