using System;
using System.Collections.Generic;

namespace bitkyFlashresUniversal.dataExport.bean
{
    public class SummaryDataJson
    {
        public SummaryDataJson()
        {
        }

        public SummaryDataJson(List<CollectItem> poleResult, List<Pole> elecDetect, Dictionary<string, int> preference)
        {
            PoleResult = poleResult;
            ElecDetect = elecDetect;
            Preference = preference;
        }

        /// <summary>
        ///     不良电极编号集合,如果只有一个值'-1', 说明该项未使用, 有异常
        /// </summary>
        public List<int> EnabledPoleInts { set; get; } = new List<int>() {-1};

        /// <summary>
        ///     采集结果集合
        /// </summary>
        public List<CollectItem> PoleResult { set; get; }

        /// <summary>
        ///     电极检测结果集合
        /// </summary>
        public List<Pole> ElecDetect { set; get; }

        /// <summary>
        ///     包含总次数，已完成次数，电流阈值等
        /// </summary>
        public Dictionary<string, int> Preference { set; get; }

        /// <summary>
        ///     时间
        /// </summary>
        public DateTime DateTime { get; set; } = DateTime.Now;

        /// <summary>
        ///     操作者
        /// </summary>
        public string UserName { get; set; } = "匿名";

        /// <summary>
        ///     备注
        /// </summary>
        public string Note { get; set; } = "无";
    }
}