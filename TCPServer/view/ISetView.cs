namespace TCPServer.view
{
    public interface ISetView
    {
        void ControlMessageShow(string message);//控制信息
        void CommunicateMessageShow(string message);//通信信息

        /// <summary>
        /// 建立远程连接成功
        /// </summary>
        void GetRemoteConnectSuccess();

        /// <summary>
        /// 远程连接断开
        /// </summary>
        void GetRemoteConnectStop();
    }
}
