using bitkyFlashresUniversal.connClient.model.bean;

namespace bitkyFlashresUniversal.connClient.model
{
    /// <summary>
    ///     通信连接的外观模式
    /// </summary>
    public interface ICommucationFacade
    {
        /// <summary>
        ///     使用指定的IP地址和端口号构建TCP客户端
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        void InitTcpClient(string ip, int port);

        /// <summary>
        ///     建立连接成功，获取Socket成功的消息
        /// </summary>
        void GetSocketSuccess();

        /// <summary>
        ///     TCP连接失效
        /// </summary>
        void TcpClientFailed(string evenType);

        //------------------------通信---------------------------

        /// <summary>
        ///     从TCP客户端获取已接收到的数据[注:底层使用的方法]
        /// </summary>
        /// <param name="data">获取的远程数据</param>
        void GetReceivedData(byte[] data);

        /// <summary>
        ///     返回程序处理后的数据帧[注:底层使用的方法]
        /// </summary>
        /// <param name="frameData">处理后的数据帧</param>
        void SetFrameData(FrameData frameData);

        /// <summary>
        ///     发送指定的帧
        /// </summary>
        /// <param name="frameData">指定的帧格式</param>
        void SendDataFrame(FrameData frameData);

        //--------------------------保持模式----------------------------------

        /// <summary>
        /// 收集子帧完成回调方法
        /// </summary>
        void GetsubframeComplete(FrameType type);

        /// <summary>
        /// 帧接收超时状态清零
        /// </summary>
        void FrameReceiveTimeoutClear();

        /// <summary>
        /// 下位机接收数据超时
        /// </summary>
        void FrameReceiveTimeout();
    }
}