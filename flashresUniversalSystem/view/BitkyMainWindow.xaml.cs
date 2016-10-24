using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using bitkyFlashresUniversal.connClient.model.bean;
using bitkyFlashresUniversal.connClient.presenter;
using bitkyFlashresUniversal.ElectrodeSelecter;
using bitkyFlashresUniversal.poleInfoShow;

namespace bitkyFlashresUniversal.view
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BitkyMainWindow : IViewCommStatus
    {
        private readonly ICommPresenter _commPresenter;
        private BitkyPoleControl[] _bitkyPoleControls;

        public BitkyMainWindow()
        {
            InitializeComponent();
            _commPresenter = new CommPresenter(this);
            if (!_commPresenter.CheckTable())
                LabelDataOutlineShow.Content = "请使用电极选择器选择待测电极";

            InitBitkyPoleShow();
            InitSettingFragment();
            InitWifiSerialPortShow();
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
        ///     初始化电极信息显示标签界面
        /// </summary>
        private void InitBitkyPoleShow()
        {
            var controls = new List<BitkyPoleControl>();
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
        }

        /// <summary>
        ///     初始化系统设置标签页面
        /// </summary>
        private void InitSettingFragment()
        {
            TextBoxElectricThreshold.Text = PresetInfo.ElectricThreshold.ToString(CultureInfo.CurrentCulture);
            TextBoxFrameReceiveTimeout.Text = PresetInfo.FrameReceiveTimeout.ToString();
            TextBoxFrameSendDelay.Text = PresetInfo.FrameSendDelay.ToString();
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
            addresses.ForEach(ipAddress => { comboBoxIP.Items.Add(ipAddress); });
            comboBoxIP.SelectedIndex = comboBoxIP.Items.Count > 0 ? 0 : -1;
        }

        /// <summary>
        ///     电极信息初始化成功
        /// </summary>
        public void SetElectrodeSuccessful()
        {
            _commPresenter.CheckTable();
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
            var ip = comboBoxIP.Text.Trim();
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
            _commPresenter.DeviceGatherStart(OperateType.Gather);
        }


        /// <summary>
        ///     开启电极选择器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenElectrodeSelectForm_Click(object sender, RoutedEventArgs e)
        {
            new ElectrodeSelecterForm(this).Show();
        }

        private void btnElectrodeDetect_Click(object sender, RoutedEventArgs e)
        {
            _commPresenter.DeviceGatherStart(OperateType.Detect);
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
            _commPresenter.GatherDataClear();
        }

        private void btnShowClear_Click(object sender, RoutedEventArgs e)
        {
            ListBoxCommunicationText.Items.Clear();
            ListBoxControlText.Items.Clear();
            ListBoxSendData.Items.Clear();
            ListBoxReceiveData.Items.Clear();
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
        }

        private void btnDebugPole_Click(object sender, RoutedEventArgs e)
        {
            var electrodes = new List<Electrode>
            {
                new Electrode(int.Parse(TextBoxDebugPole0.Text.Trim()), PoleMode.A),
                new Electrode(int.Parse(TextBoxDebugPole1.Text.Trim()), PoleMode.B)
            };

            _commPresenter.DebugPole(new FrameData(FrameType.ControlGather, electrodes));
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
            PresetInfo.StartAutoCollect = checkBoxStartAutoCollect.IsChecked != false;
        }
    }
}