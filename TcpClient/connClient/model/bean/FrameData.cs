using System.Collections.Generic;

namespace bitkyFlashresUniversal.connClient.model.bean
{
    /// <summary>
    ///     处理后的帧数据的封装
    /// </summary>
    public class FrameData
    {
        public FrameData(FrameType type)
        {
            Type = type;
        }


        public FrameData(FrameType type, List<Electrode> electrode)
        {
            Type = type;
            PoleList = electrode;
        }

        /// <summary>
        ///     被选中的电极的集合
        /// </summary>
        public List<Electrode> PoleList { get; set; }

        /// <summary>
        ///     数据帧的类型
        /// </summary>
        public FrameType Type { get; }

        /// <summary>
        /// 帧的备注
        /// </summary>
        public string Note { set; get; }
    }
}