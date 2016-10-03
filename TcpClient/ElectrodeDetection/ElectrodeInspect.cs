using System.Collections.Generic;

namespace bitkyFlashresUniversal.ElectrodeDetection
{
    public class ElectrodeInspect
    {
        public int NiceId { get; }
        public List<int> BadList { get; } = new List<int>();

        public ElectrodeInspect(List<int> badList, int niceId)
        {
            BadList = badList;
            this.NiceId = niceId;
        }
    }
}