namespace bitkyFlashresUniversal.connClient.model.bean
{
    /// <summary>
    ///     帧头类型
    /// </summary>
    public enum FrameType
    {
        /// <summary>
        ///     握手帧的帧头
        /// </summary>
        HandshakeSwitchWifi,

        /// <summary>
        ///     下位机重置指令
        /// </summary>
        DeviceReset,

        /// <summary>
        ///     数据帧的帧头，包括继电器控制命令帧和数据启动采样帧
        /// </summary>
        DataHeader,

        /// <summary>
        ///     继电器控制命令帧的帧头
        /// </summary>
        ControlGather,

        /// <summary>
        ///     数据启动采样帧的帧头
        /// </summary>
        ActivateGather,

        /// <summary>
        ///     采集数据的反馈结果帧的帧头
        /// </summary>
        ReturnDataGather,

        /// <summary>
        ///     高压继电器开启子帧头
        /// </summary>
        HvRelayOpen,

        /// <summary>
        ///     高压继电器关闭子帧头
        /// </summary>
        HvRelayClose,

        /// <summary>
        ///     无类型，一般不会出现
        /// </summary>
        None
    }
}