using System.Diagnostics;
using System.Net;
using System.Windows;
using TCPServer.accessPresenter;

namespace TCPServer.view
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MyTcpServer : ISetView
    {
        private readonly IServerPresenter _accessPresenter;
        private IPAddress[] _addressList; //本地IP地址集合


        public MyTcpServer()
        {
            InitializeComponent();
            SetComboBoxIpAddress();
            _accessPresenter = new ServerPresenter(this);
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
                ListBoxControlText.ScrollIntoView(ListBoxControlText.Items[ListBoxControlText.Items.Count - 1]);

            });
        }

        /// <summary>
        ///     控制信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        public void CommunicateMessageShow(string message) //通信信息
        {
            Dispatcher.Invoke(() =>
            {
                ListBoxCommunicationText.Items.Add(message);
                ListBoxCommunicationText.SelectedIndex = ListBoxCommunicationText.Items.Count - 1;
                ListBoxCommunicationText.ScrollIntoView(ListBoxCommunicationText.Items[ListBoxCommunicationText.Items.Count - 1]);

            });
        }

        /// <summary>
        ///     建立远程连接成功
        /// </summary>
        public void GetRemoteConnectSuccess()
        {
            Debug.WriteLine("建立远程连接成功");
        }

        /// <summary>
        ///     远程连接断开
        /// </summary>
        public void GetRemoteConnectStop()
        {
            Debug.WriteLine("远程连接断开");
            BtnStart.Content = "开始监听";
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (BtnStart.Content.ToString() == "开始监听")
            {
                if ((TextBoxPort.Text != string.Empty) && (ComboBoxIp.Text != string.Empty)) //检测IP地址框和端口框是否为空
                {
                    BtnStart.Content = "断开监听";
                    _accessPresenter.StartListening(_addressList[ComboBoxIp.SelectedIndex],
                        int.Parse(TextBoxPort.Text));
                    ControlMessageShow("准备开启TCP服务");
                }
                else MessageBox.Show("请输入本地端口号，重新连接", "提示");
            }
            else
            {
                BtnStart.Content = "开始监听";
                _accessPresenter.StopListening();
            }
        }

        /// <summary>
        ///     获取本机IP地址并填入comboBox中
        /// </summary>
        private void SetComboBoxIpAddress()
        {
            _addressList = Dns.GetHostByName(Dns.GetHostName()).AddressList; //会返回所有地址，包括IPv4和IPv6   
            foreach (var address in _addressList)
                ComboBoxIp.Items.Add(address);
            ComboBoxIp.SelectedIndex = ComboBoxIp.Items.Count > 0 ? 0 : -1;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
} 