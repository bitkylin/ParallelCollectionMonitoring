namespace bitkyFlashresUniversal.connClient.model.bean
{
    public static class PresetInfo
    {
        //-------------数据库-------------
        public const string ElectrodeControllerTable = "ElectrodeController";
        public const string DataInfoTable = "DataInfo";
        public const string ElectrodeDetectionTable = "ElectrodeDetection";
        public const string SqliteSequenceTable = "sqlite_sequence";
        public const string ElectrodeResultTable = "ElectrodeResult";
        public const string PreferencesTable = "Preferences";

        public const string DatabasePath = "./bitkyDataReady.db";


        //---------系统当前运行模式----------
        public static OperateType CurrentOperateType = OperateType.Null;
        public static CommType CurrentCommType = CommType.Null;
        public static bool StartAutoCollect = true;

        //---------帧回复超时等待------------
        public static int FrameReceiveTimeout = 3500;
        public static int FrameSendDelay = 50;

        //---------电极检测模式--------------
        public static double ElectricThreshold = 10;
    }
}