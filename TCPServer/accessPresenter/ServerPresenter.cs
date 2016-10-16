using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TCPServer.model;
using TCPServer.util;
using TCPServer.view;

namespace TCPServer.accessPresenter
{
    internal class ServerPresenter : IServerPresenter
    {
        private readonly BitkyTcpServer _bitkyTcpServer;
        private readonly FrameBuilder _frameBuilder = new FrameBuilder();
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
            foreach (var b in data)
                stringbuilder.Append(Convert.ToString(b, 16) + " ");
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
                var count = byteslist.Count;
                SendActivateGatherFrame();
                _view.ControlMessageShow("接收到数据帧的长度：" + count);
            }
        }

        public void GetControlMessage(string data)
        {
            _view.ControlMessageShow(data);
        }

        private void SendActivateGatherFrame()
        {
            var list = new List<byte>();
            list.AddRange(_frameBuilder.Build(0));
            list.AddRange(_frameBuilder.Build(1));
            list.AddRange(_frameBuilder.Build(2));
            list.AddRange(_frameBuilder.Build(3));
            list.AddRange(_frameBuilder.Build(4));
            _bitkyTcpServer.SendDelayed(list.ToArray(), 50);
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