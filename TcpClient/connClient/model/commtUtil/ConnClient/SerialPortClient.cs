using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace bitkyFlashresUniversal.connClient.model.commtUtil.ConnClient
{
    public class SerialPortClient
    {
        private readonly ICommucationFacade _commucationFacade;
        private SerialPort _comm;


        public SerialPortClient(ICommucationFacade commucationFacade)
        {
            _commucationFacade = commucationFacade;
            _comm = new SerialPort(); //初始化串口
            _comm.NewLine = "\n";
            _comm.RtsEnable = true; //根据实际情况吧。
            _comm.DataReceived += ReceiveData;
        }

        public void Open(string PortName, int BaudRate)
        {
            _comm.PortName = PortName;
            _comm.BaudRate = BaudRate;
            try
            {
                _comm.Open();
            }
            catch (InvalidOperationException  ex)
            {
                _commucationFacade.CommClientFailed("InvalidOperationException");
                return;
            }
            catch (IOException ex)
            {
                _commucationFacade.CommClientFailed("IOException");
                return;
            }

            _commucationFacade.GetSocketSuccess();
        }

        /// <summary>
        ///     新的子线程：接收当前Socket的数据
        /// </summary>
        private void ReceiveData(object sender, SerialDataReceivedEventArgs e) //接收数据
        {
            var buffer = new byte[1024];
            Thread.Sleep(100);
            var bufLen = 0;
            try
            {
                bufLen = _comm.BytesToRead;
                if (bufLen == 0)
                    return;
                _comm.Read(buffer, 0, bufLen);
            }

            catch (InvalidOperationException ex)
            {
                _commucationFacade.CommClientFailed("串口接收数据异常");
                Debug.WriteLine("客户端串口断开连接");
                Debug.WriteLine("错误代码:" + ex.Message);
                return;
            }

            var replyData = new byte[bufLen];
            Array.Copy(buffer, 0, replyData, 0, bufLen);
            _commucationFacade.GetReceivedData(replyData); //返回接收到的byte[]
        }


        public void Send(byte[] bytes)
        {
            //仅仅用于显示调试信息
            var stringbuilder = new StringBuilder();
            foreach (var b in bytes)
                stringbuilder.Append($"{b:X2} " + " ");
            _commucationFacade.SendDataShow("已发送:" + stringbuilder);
            Debug.WriteLine("已发送:" + stringbuilder);

            if (_comm.IsOpen)
                try
                {
                    _comm.Write(bytes, 0, bytes.Length);
                }
                catch (InvalidOperationException ex)
                {
                    _commucationFacade.CommClientFailed("串口发送数据异常");
                    Debug.WriteLine("客户端串口断开连接");
                    Debug.WriteLine("错误代码:" + ex.Message);
                    return;
                }
            else
                _commucationFacade.CommClientFailed("comm.IsOpen==false 串口断开");
        }


        public void Close()
        {
            if (_comm != null && _comm.IsOpen)
                _comm.Close();
        }
    }
}