using System.Collections.Generic;
using bitkyFlashresUniversal.connClient.model.bean;
using bitkyFlashresUniversal.ElectrodeDetection;

namespace bitkyFlashresUniversal.databaseUtil.presenter
{
    public interface ISqlPresenter
    {
        /// <summary>
        ///     核查数据表的轮廓信息，并读取总的数据行数及当前所在行
        /// </summary>
        /// <param name="sumNun">数据总行数</param>
        /// <param name="preCollectRow">当前所在行</param>
        void SetTableOutline(int sumNun, int preCollectRow);

        /// <summary>
        ///     从数据库中获取当前行的数据并解析为数据帧的对象
        /// </summary>
        /// <returns>获取的数据帧的对象</returns>
        FrameData GetFrameDataFromDb();

        /// <summary>
        ///     检查数据表的正确性,若数据表正确，则显示数据表轮廓信息
        /// </summary>
        bool CheckTable();

        /// <summary>
        ///     将返回的结果插入数据库
        /// </summary>
        /// <param name="electrodes">解析成功的电极集合</param>
        /// <param name="id">id</param>
        void InsertResultDataToDb(List<Electrode> electrodes, int id);

        /// <summary>
        /// 电极检测前的初始化
        /// </summary>
        void ElectrodDetectInit();
        /// <summary>
        ///     第二次检测准备，获取良好的电极及不良的电极
        /// </summary>
        /// <returns></returns>
        ElectrodeInspect SecondRroundInspect();

        /// <summary>
        ///     清空已采集的数据
        /// </summary>
        void GatherDataClear();
        /// <summary>
        ///     获取配置信息
        /// </summary>
        void GetPreferences();
        /// <summary>
        /// 在数据库中更新配置信息
        /// </summary>
        void UpdatePreferences();
    }
}