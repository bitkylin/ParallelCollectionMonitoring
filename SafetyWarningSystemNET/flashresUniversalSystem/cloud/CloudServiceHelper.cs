using System;
using cn.bmob.api;
using cn.bmob.tools;

namespace bitkyFlashresUniversal.cloud
{
    class CloudServiceHelper
    {
        private static BmobWindows _bmobWindows;


        public CloudServiceHelper()
        {
            BmobBuilder();
        }

        /// <summary>
        /// Bmob初始化
        /// </summary>
        public static BmobWindows BmobBuilder()
        {
            if (_bmobWindows == null)
            {
                _bmobWindows = new BmobWindows();
                _bmobWindows.initialize(PresetInfo.BmobApplicationId, PresetInfo.BmobRestApiKey);
                BmobDebug.Register(msg => { Console.WriteLine("BmobDebug:" + msg); });
            }
            return _bmobWindows;
        }
    }
}