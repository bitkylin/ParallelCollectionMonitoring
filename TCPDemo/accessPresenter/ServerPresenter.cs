using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TCPDemo.util;
using TCPDemo.view;

namespace TCPDemo.accessPresenter
{
    internal class ServerPresenter : IServerPresenter
    {
        private readonly BitkyTcpServer _bitkyTcpServer;
        private readonly ISetView _view;

        public ServerPresenter(ISetView view)
        {
            _view = view;
            _bitkyTcpServer = new BitkyTcpServer(this);
        }

        public void StartListening(IPAddress ipAddress, int port)
        {
            _bitkyTcpServer.StartListening(ipAddress, port);
        }

        public void StopListening()
        {

            _bitkyTcpServer.StopListening();
        }

        public void GetReceivedData(byte[] data)
        {
            var stringbuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
                stringbuilder.Append(Convert.ToString(data[i], 16) + " ");
            _view.CommunicateMessageShow(stringbuilder.ToString()); //接收到的信息显示在view中

            var byteslist = new List<byte>();
            byteslist.AddRange(data);
            var bytes = byteslist.GetRange(0, 4).ToArray();
            //握手帧校验
            if (CompareByte(bytes, CommMsg.HandshakeSwitchWifiFrameHeader))
            {
                _view.ControlMessageShow("反馈握手帧");
                _bitkyTcpServer.SendDelayed(CommMsg.HandshakeSwitchWifiFrameHeader, 150);
            }
            //接收到数据帧
            if (CompareByte(bytes, CommMsg.DataFrameHeader))
            {
                var bytes2 = byteslist.GetRange(6, 4).ToArray();
                if (CompareByte(bytes2, CommMsg.ControlGatherFrameHeader))
                {
                    SendControlGatherFrame();
                    return;
                }


                if (CompareByte(bytes2, CommMsg.ActivateGatherFrameHeader))
                    SendActivateGatherFrame();
            }
        }

        public void GetControlMessage(string data)
        {
            _view.ControlMessageShow(data);
        }

        private void SendControlGatherFrame()
        {
            _view.ControlMessageShow("反馈控制帧");

            _bitkyTcpServer.SendDelayed(CommMsg.DataFrameHeader, 50);
          
        }

        private void SendActivateGatherFrame()
        {
            _view.ControlMessageShow("反馈启动帧");
            _bitkyTcpServer.SendDelayed(CommMsg.DataFrameHeader, 50);
         
            _bitkyTcpServer.SendDelayed(CommMsg.Data1Subframe, 250);
            _bitkyTcpServer.SendDelayed(CommMsg.Data2Subframe, 450);
            _bitkyTcpServer.SendDelayed(CommMsg.Data3Subframe, 650);
            _bitkyTcpServer.SendDelayed(CommMsg.Data4Subframe, 850);
            _bitkyTcpServer.SendDelayed(CommMsg.Data5Subframe, 1050);
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