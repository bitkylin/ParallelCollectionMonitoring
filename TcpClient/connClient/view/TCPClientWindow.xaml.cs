using System.Text.RegularExpressions;
using System.Windows;
using bitkyFlashresUniversal.connClient.model.bean;
using bitkyFlashresUniversal.connClient.presenter;
using bitkyFlashresUniversal.ElectrodeSelecter;

namespace bitkyFlashresUniversal.connClient.view
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TcpClientWindow : IViewCommStatus
    {
        private readonly ICommPresenter _commPresenter;

        public TcpClientWindow()
        {
            InitializeComponent();
            _commPresenter = new CommPresenter(this);
            if (!_commPresenter.CheckTable())
            {
                LabelDataOutlineShow.Content = "请使用电极选择器选择待测电极";
            }
           
        }

        /// <summary>
        /// 数据库数据轮廓信息显示
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
        ///     网络连接正在建立中
        /// </summary>
        public void ConnConnecting()
        {
            Dispatcher.Invoke(() =>
            {
                BtnConnect.Content = "正在连接";
                BtnConnect.IsEnabled = false;
            });
        }

        /// <summary>
        ///     网络连接已建立
        /// </summary>
        public void ConnConnected()
        {
            Dispatcher.Invoke(() =>
            {
                BtnConnect.Content = "断开";
                BtnConnect.IsEnabled = true;
            });
        }

        /// <summary>
        ///     网络连接已断开
        /// </summary>
        public void ConnDisconnected()
        {
            Dispatcher.Invoke(() =>
            {
                BtnConnect.Content = "连接";
                BtnConnect.IsEnabled = true;
            });
        }

        /// <summary>
        /// 电极信息初始化成功
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
            if (BtnConnect.Content.ToString().Equals("断开"))
            {
                _commPresenter.FrontConnClosed();
                BtnConnect.Content = "连接";
                return;
            }
            var ip = TextBoxIp.Text.Trim();
            var portStr = TextBoxPort.Text.Trim();
            var match = Regex.IsMatch(ip,
                @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
            if (Regex.IsMatch(portStr, @"^[1-9]\d*$"))
            {
                var port = int.Parse(portStr);
                if ((port < 65536) && (port >= 1024) && match)
                    _commPresenter.InitTcpClient(ip, port);
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

        private void btnCommunicateClear_Click(object sender, RoutedEventArgs e)
        {
            ListBoxCommunicationText.Items.Clear();
        }

        private void btnControlClear_Click(object sender, RoutedEventArgs e)
        {
            ListBoxControlText.Items.Clear();
        }

        private void btnGatherDataClear_Click(object sender, RoutedEventArgs e)
        {
            _commPresenter.GatherDataClear();
        }
    }
}