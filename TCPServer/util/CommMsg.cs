namespace TCPServer.util
{
    public static class CommMsg
    {
        /// <summary>
        ///     握手帧，通讯接口选择帧，仅帧头
        /// </summary>
        public static readonly byte[] HandshakeSwitchWifiFrameHeader = {0x55, 0x7F, 0x02, 0xAA};

        /// <summary>
        ///     数据指令帧的总帧头
        /// </summary>
        public static readonly byte[] DataFrameHeader = {0x55, 0xaa, 0x5a, 0xa5};

        /// <summary>
        ///     继电器控制命令帧的返回数据子帧头
        /// </summary>
        public static readonly byte[] ControlGatherFrameHeaderReturn = {0xF3, 0x00, 0x00, 0x0C};
    }
}