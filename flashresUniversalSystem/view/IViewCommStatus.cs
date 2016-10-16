using System.Collections.Generic;
using bitkyFlashresUniversal.connClient.model.bean;

namespace bitkyFlashresUniversal.view
{
    internal interface IViewCommStatus
    {
        /// <summary>
        ///     控制信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        void ControlMessageShow(string message);

        /// <summary>
        ///     通信信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        void CommunicateMessageShow(string message);

        /// <summary>
        ///     发送帧信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        void SendDataShow(string message);

        /// <summary>
        ///     接收帧信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        void ReceiveDataShow(string message);

        /// <summary>
        ///     数据库数据轮廓信息显示
        /// </summary>
        /// <param name="message"></param>
        void DataOutlineShow(string message);

        /// <summary>
        ///     网络连接已建立
        /// </summary>
        void ConnConnected();

        /// <summary>
        ///     网络连接已断开
        /// </summary>
        void ConnDisconnected();

        /// <summary>
        /// 在界面上显示电极完整信息
        /// </summary>
        /// <param name="electrodes"></param>
        void BitkyPoleShow(List<Electrode> electrodes);
    }
}