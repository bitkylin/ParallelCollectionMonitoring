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
        /// 或  使用指定的串口名和波特率构建串口客户端
        /// </summary>
        /// <param name="str"></param>
        /// <param name="num"></param>
        void InitCommClient(string str, int num);

        /// <summary>
        ///     建立连接成功，获取Socket成功的消息
        /// </summary>
        void GetSocketSuccess();

        /// <summary>
        ///     TCP连接失效
        /// </summary>
        void CommClientFailed(string str);

        //------------------------通信---------------------------

        /// <summary>
        ///     从TCP客户端获取已接收到的数据[注:底层使用的方法]
        /// </summary>
        /// <param name="data">获取的远程数据</param>
        void GetReceivedData(byte[] data);

        /// <summary>
        ///     发送指定的帧
        /// </summary>
        /// <param name="frameData">指定的帧格式</param>
        void SendDataFrame(FrameData frameData);
        /// <summary>
        ///     发送帧信息的显示
        /// </summary>
        /// <param name="message">输入所需显示的信息</param>
        void SendDataShow(string message);
    }
}