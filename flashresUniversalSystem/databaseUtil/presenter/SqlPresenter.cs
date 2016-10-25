using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using bitkyFlashresUniversal.connClient.model.bean;
using bitkyFlashresUniversal.connClient.presenter;
using bitkyFlashresUniversal.dataExport.bean;
using bitkyFlashresUniversal.ElectrodeDetection;

namespace bitkyFlashresUniversal.databaseUtil.presenter
{
    public class SqlPresenter : ISqlPresenter
    {
        private readonly ICommPresenter _commPresenter;
        // private IViewSqlStatus _view;
        private readonly SqliteBitky _sqliteBitky;
        private int _currentA;
        private int _currentB;
        private int _currentM;
        private int _currentNum;
        private int _sumNum;

        public SqlPresenter(ICommPresenter presenter)
        {
            _commPresenter = presenter;
            _sqliteBitky = new SqliteBitky(this);
        }

        /// <summary>
        ///     数据表的轮廓信息显示在view中
        /// </summary>
        /// <param name="sumNun">数据总行数</param>
        /// <param name="preCollectRow">待采数据行</param>
        public void SetTableOutline(int sumNun, int preCollectRow)
        {
            _currentNum = preCollectRow;
            _sumNum = sumNun;
            _commPresenter.DataOutlineShow("已采集次数: " + (preCollectRow - 1) + " 总次数: " + sumNun);
        }

        /// <summary>
        ///     从数据库中获取当前行的数据并解析为数据帧的对象
        /// </summary>
        /// <returns>获取的数据帧的对象</returns>
        public FrameData GetFrameDataFromDb()
        {
            if (_currentNum <= _sumNum)
            {
                var frameData = _sqliteBitky.SelectRowData(_currentNum);
                if (frameData.Type != FrameType.None)
                {
                    frameData.PoleList.ForEach(pole =>
                    {
                        switch (pole.Mode)
                        {
                            case PoleMode.A:
                                _currentA = pole.IdOrigin;
                                break;
                            case PoleMode.B:
                                _currentB = pole.IdOrigin;
                                break;
                            case PoleMode.M:
                                _currentM = pole.IdOrigin;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    });
                    _commPresenter.CommunicateMessageShow("读取数据成功:当前行:" + _currentNum + " 总行:" + _sumNum + " A:" +
                                                          _currentA + " B:" +
                                                          _currentB + " M:" + _currentM);

                    return frameData;
                }
                Debug.WriteLine("数据库中无此行");
                return new FrameData(FrameType.None);
            }
            Debug.WriteLine("数据库检索已完成");
            _currentNum = _sumNum;
            return new FrameData(FrameType.None);
        }

        /// <summary>
        ///     将返回的结果插入数据库
        /// </summary>
        public void InsertResultDataToDb(List<Electrode> electrodes, int id)
        {
            switch (PresetInfo.CurrentOperateType)
            {
                case OperateType.Gather:
                    var sqlTextNameBuilder = new StringBuilder();
                    var sqlTextValueBuilder = new StringBuilder();
                    sqlTextNameBuilder.Append("INSERT INTO " + PresetInfo.ElectrodeResultTable + " (typeA,typeB,typeM");
                    sqlTextValueBuilder.Append(") VALUES ('" + _currentA + "','" + _currentB + "','" + _currentM);

                    electrodes.ForEach(pole =>
                    {
                        sqlTextNameBuilder.Append(",pole" + pole.IdOrigin);
                        sqlTextValueBuilder.Append("','" + pole.Value);
                    });
                    sqlTextValueBuilder.Append("')");

                    var sqlText = sqlTextNameBuilder.ToString() + sqlTextValueBuilder;
                    Debug.WriteLine(sqlText);
                    if (_sqliteBitky.InsertResultDataToDb(sqlText))
                    {
                        _sqliteBitky.UpdateCurrentRowNum(_currentNum);
                        SetTableOutline(_sumNum, _currentNum + 1);

                        Debug.WriteLine("插入数据库成功");
                        _commPresenter.InsertDataIntoDbComplete();
                    }
                    break;
                case OperateType.Detect:
                    var count = 0;
                    double valueSum = 0;
                    electrodes.ForEach(pole =>
                    {
                        if (pole.IdOrigin == 64)
                        {
                            count++;
                            valueSum = pole.Value;
                        }
                    });
                    if (count == 0)
                    {
                        _commPresenter.CommunicateMessageShow("未获取到电流值，数据采集异常");
                        Debug.WriteLine("未获取到电流值，数据采集异常");
                        return;
                    }
                    var id2 = id + 1;
                    var sqlTextDetect1 = "INSERT INTO " + PresetInfo.ElectrodeDetectionTable +
                                         " (poleid,value) VALUES ('" + id +
                                         "','" + valueSum + "')";

                    var sqlTextDetect2 = "INSERT INTO " + PresetInfo.ElectrodeDetectionTable +
                                         " (poleid,value) VALUES ('" + id2 +
                                         "','" + valueSum + "')";
                    if (_sqliteBitky.InsertResultDataToDb(sqlTextDetect1) &&
                        _sqliteBitky.InsertResultDataToDb(sqlTextDetect2))
                    {
                        SetTableOutline(63, id);

                        Debug.WriteLine("插入数据库成功");
                        _commPresenter.InsertDataIntoDbComplete();
                    }
                    else
                    {
                        Debug.WriteLine("第一轮检测插入数据库失败");
                    }
                    break;
                case OperateType.Detect2: //第二轮检测结果更新到数据库
                    var count2 = 0;
                    double valueSum2 = 0;
                    electrodes.ForEach(pole =>
                    {
                        if (pole.IdOrigin == 64)
                        {
                            count2++;
                            valueSum2 = pole.Value;
                        }
                    });
                    if (count2 == 0)
                    {
                        _commPresenter.CommunicateMessageShow("未获取到电流值，数据采集异常");
                        Debug.WriteLine("未获取到电流值，数据采集异常");
                        return;
                    }
                    var sqlTextDetect3 = "UPDATE " + PresetInfo.ElectrodeDetectionTable + " SET value = '" + valueSum2 +
                                         "' WHERE poleid = '" + id + "'";

                    if (_sqliteBitky.InsertResultDataToDb(sqlTextDetect3))
                        _commPresenter.InsertDataIntoDbComplete();
                    else
                        Debug.WriteLine("第二轮检测插入数据库失败");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     电极检测前的初始化
        /// </summary>
        public void ElectrodDetectInit()
        {
            const string sqlElectrodDetectInit = "DELETE FROM " + PresetInfo.ElectrodeDetectionTable;
            _sqliteBitky.InsertResultDataToDb(sqlElectrodDetectInit);
            const string sqlElectrodDetectInit2 = "DELETE FROM " + PresetInfo.SqliteSequenceTable;
            _sqliteBitky.InsertResultDataToDb(sqlElectrodDetectInit2);
        }

        /// <summary>
        ///     第二次检测准备，获取良好的电极及不良的电极
        /// </summary>
        /// <returns></returns>
        public ElectrodeInspect SecondRroundInspect()
        {
            return _sqliteBitky.SecondRroundInspect();
        }

        /// <summary>
        ///     检查数据表的正确性,若数据表正确，则显示数据表轮廓信息
        /// </summary>
        public bool CheckTable()
        {
            return _sqliteBitky.CheckTable();
        }

        /// <summary>
        ///     清空已采集的数据
        /// </summary>
        public void GatherDataClear()
        {
            const string sqlElectrodDetectInit = "DELETE FROM " + PresetInfo.ElectrodeResultTable;
            const string sqlElectrodDetectInit2 =
                "UPDATE " + PresetInfo.DataInfoTable + " SET num = '0' WHERE name = 'current'";
            const string sqlElectrodDetectInit3 = "DELETE FROM " + PresetInfo.SqliteSequenceTable;

            _sqliteBitky.InsertResultDataToDb(sqlElectrodDetectInit);
            _sqliteBitky.InsertResultDataToDb(sqlElectrodDetectInit2);
            _sqliteBitky.InsertResultDataToDb(sqlElectrodDetectInit3);
        }

        /// <summary>
        ///     获取配置信息
        /// </summary>
        public void GetPreferences()
        {
            _sqliteBitky.GetPreferences();
        }
        /// <summary>
        /// 在数据库中更新配置信息
        /// </summary>
        
        public void UpdatePreferences()
        {
            _sqliteBitky.UpdatePreferences();
        }
        /// <summary>
        /// 从数据库中获取用于输出的Json格式数据
        /// </summary>
        /// <returns>用于输出的Json格式数据</returns>
        public SummaryDataJson GetJsonFromDb()
        {
           return _sqliteBitky.GetJsonFromDb();
        }
    }
}
