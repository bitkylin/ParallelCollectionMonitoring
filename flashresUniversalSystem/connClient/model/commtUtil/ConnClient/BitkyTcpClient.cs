using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace bitkyFlashresUniversal.connClient.model.commtUtil.ConnClient
{
    internal class BitkyTcpClient
    {
        private readonly ICommucationFacade _commucationFacade;
        private TcpClient _client;
        private IPEndPoint _ipEndPoint; //ip & port
        private Socket _socketTcp;


        public BitkyTcpClient(ICommucationFacade commucationFacade) 
        {
            _commucationFacade = commucationFacade;
        }

        /// <summary>
        ///     建立用于通信的Socket
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口号</param>
        public void Build(string ip, int port)
        {
            if (_client == null)
            {
                _client = new TcpClient();
                _ipEndPoint = new IPEndPoint(IPAddress.Parse(ip.Trim()), port);
                new Thread(GetClientSocket).Start();
            }
        }

        public  void Send(byte[] bytes)
        {
            try
            {
                //仅仅用于显示调试信息
                var stringbuilder = new StringBuilder();
                foreach (var b in bytes)
                    stringbuilder.Append($"{b:X2}" + " ");
                _commucationFacade.SendDataShow("已发送:" + stringbuilder);
                Debug.WriteLine("已发送:" + stringbuilder);
               
                _socketTcp?.Send(bytes);
            }
            catch (SocketException)
            {
                _commucationFacade.CommClientFailed("connDisconnect");
            }
        }

        /// <summary>
        ///     新的子线程：接收当前Socket的数据
        /// </summary>
        private void ReceiveData() //接收数据
        {
            var buffer = new byte[1024];
            //根据收听到的客户端套接字向客户端发送信息
            while (true)
            {
                Thread.Sleep(50);
                //在套接字上接收客户端发送的信息
                int bufLen;
                try
                {
                    bufLen = _socketTcp.Available;
                    if (bufLen == 0)
                        continue;
                    _socketTcp.Receive(buffer, 0, bufLen, SocketFlags.None);
                }
                catch (Exception ex)
                {
                    _commucationFacade.CommClientFailed("Socket接收数据异常");
                    Debug.WriteLine("已与TCP客户端断开连接");
                    Debug.WriteLine("服务端接收链接中断：" + ex.Message);
                    return;
                }

                var replyData = new byte[bufLen];
                Array.Copy(buffer, 0, replyData, 0, bufLen);
                _commucationFacade.GetReceivedData(replyData); //返回接收到的byte[]
            }
        }

        /// <summary>
        ///     新的子线程：从TcpClient中获取用于连接的Socket
        /// </summary>
        private void GetClientSocket()
        {
            try
            {
                _client.Connect(_ipEndPoint);
                _socketTcp = _client.Client;
            }
            catch (SocketException)
            {
                _commucationFacade.CommClientFailed("UnobtainableSocket");
                return;
            }

            _commucationFacade.GetSocketSuccess(); //发送"获取Socket成功"消息
            new Thread(ReceiveData).Start(); //新建接收数据线程
        }

        public  void Close()
        {
            _socketTcp?.Close();
            _client?.Close();
            _client = null;
            _socketTcp = null;
        }
    }
}