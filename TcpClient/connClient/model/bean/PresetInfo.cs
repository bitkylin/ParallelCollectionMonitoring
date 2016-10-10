namespace bitkyFlashresUniversal.connClient.model.bean
{
    public static class PresetInfo
    {
        /// <summary>
        /// 可供采集的模块数量
        /// </summary>
        public const int ModuleNum = 4;

        //-------------数据库-------------
        public const string ElectrodeControllerTable = "ElectrodeController";
        public const string DataInfoTable = "DataInfo";
        public const string ElectrodeDetectionTable = "ElectrodeDetection";
        public const string SqliteSequenceTable = "sqlite_sequence";
        public const string ElectrodeResultTable = "ElectrodeResult";

        public const string DatabasePath = "./bitkyDataReady.db";


        //------------计时器---------------
        public const int SendInterval = 2000;
        public const int ReceiveTimeout = 5000;

        /// <summary>
        /// 下位机出错重置时，最大连续出错次数
        /// </summary>
        public const int FrameReceiveTimeoutMaxCount = 5;

        //---------系统当前运行模式----------
        public static OperateType CurrentOperateType = OperateType.Null;

        //---------电极检测模式-------------
        public const int ElectricThreshold = 2000;
    }
}