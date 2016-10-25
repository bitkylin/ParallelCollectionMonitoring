using System;
using System.Collections.Generic;
using System.Data.SQLite;
using bitkyFlashresUniversal.connClient.model.bean;
using bitkyFlashresUniversal.databaseUtil.presenter;
using bitkyFlashresUniversal.dataExport;
using bitkyFlashresUniversal.dataExport.bean;
using bitkyFlashresUniversal.ElectrodeDetection;

namespace bitkyFlashresUniversal.databaseUtil
{
    internal class SqliteBitky
    {
        private readonly SQLiteCommand _command;

        private readonly SQLiteConnection _conn =
            new SQLiteConnection("Data Source = " + PresetInfo.DatabasePath + "; Version = 3;");

        private readonly ISqlPresenter _presenter;

        public SqliteBitky(ISqlPresenter presenter)
        {
            _presenter = presenter;
            _conn.Open();
            _command = _conn.CreateCommand();
        }


        /// <summary>
        ///     检查数据表的正确性,若数据表正确，则显示数据表轮廓信息，返回总行数和已采集的行数
        /// </summary>
        public bool CheckTable()
        {
            _command.Reset();
            //获取待采集数据总数
            _command.CommandText = "SELECT COUNT(*) FROM " + PresetInfo.ElectrodeControllerTable;
            var countRow = int.Parse(_command.ExecuteScalar().ToString());
            if (countRow == 0)
                return false;

            //获取被记录的待采集数据总数
            _command.Reset();
            _command.CommandText = "SELECT num FROM " + PresetInfo.DataInfoTable + " where name = 'sum'";
            var countRowInfo = int.Parse(_command.ExecuteScalar().ToString());
            if (countRowInfo == 0)
                return false;

            //获取已采集数据的数据表最后一行的ID
            _command.Reset();
            _command.CommandText = "SELECT num FROM " + PresetInfo.ElectrodeControllerTable +
                                   " order by num desc limit 1";
            int lastRowInfo;

            var reader = _command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                lastRowInfo = reader.GetInt32(0);
            }
            else
            {
                lastRowInfo = 0;
            }


            //获取当前指向的条目编号
            var completeRow = 0;

            _command.Reset();
            _command.CommandText = "SELECT num FROM " + PresetInfo.DataInfoTable + " WHERE name = 'current'";
            reader = _command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                    completeRow = reader.GetInt32(0);
            }
            else
            {
                _command.Reset();
                _command.CommandText = "INSERT INTO DataInfo (name, num) VALUES ('current', '0')";
                _command.ExecuteNonQuery();
            }
            //判断所获取数据的合法性
            if ((countRow != countRowInfo) || (countRow != lastRowInfo) || (completeRow >= countRow))
                return false;

            //数据合法，获准通过
            _presenter.SetTableOutline(countRow, completeRow + 1);
            return true;
        }

        /// <summary>
        ///     查询一行的数据
        /// </summary>
        /// <param name="rowNum">行号</param>
        /// <returns>待发送的数据帧的对象</returns>
        public FrameData SelectRowData(int rowNum)
        {
            _command.Reset();
            _command.CommandText = "SELECT * FROM " + PresetInfo.ElectrodeControllerTable + " WHERE num = '" + rowNum +
                                   "'";

            var reader = _command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                var poleList = new List<Electrode>();
                var typeA = reader.GetInt32(1);
                var typeB = reader.GetInt32(2);
                var typeM = reader.GetInt32(3);
                poleList.Add(new Electrode(typeA, PoleMode.A));
                poleList.Add(new Electrode(typeB, PoleMode.B));
                poleList.Add(new Electrode(typeM, PoleMode.M));
                reader.Close();
                return new FrameData(FrameType.ControlGather, poleList);
            }
            return new FrameData(FrameType.None);
        }

        public bool InsertResultDataToDb(string sqlText)
        {
            _command.Reset();
            _command.CommandText = sqlText;
            var i = _command.ExecuteNonQuery();
            return i != 0;
        }

        public void UpdateCurrentRowNum(int rowNum)
        {
            _command.Reset();
            _command.CommandText = "UPDATE " + PresetInfo.DataInfoTable + " SET num = '" + rowNum +
                                   "' WHERE name = 'current'";
            _command.ExecuteNonQuery();
        }

        /// <summary>
        ///     第二次检测准备，获取良好的电极及不良的电极
        /// </summary>
        /// <returns></returns>
        public ElectrodeInspect SecondRroundInspect()
        {
            var badList = new List<int>();
            _command.Reset();
            _command.CommandText = "SELECT max(value) FROM " + PresetInfo.ElectrodeDetectionTable;
            var maxValue = double.Parse(_command.ExecuteScalar().ToString());
            _command.CommandText = "SELECT poleid FROM " + PresetInfo.ElectrodeDetectionTable + " WHERE value = '" +
                                   maxValue +
                                   "'";
            var maxId = int.Parse(_command.ExecuteScalar().ToString());
            _command.CommandText = "SELECT poleid FROM ElectrodeDetection WHERE value <= '" +
                                   PresetInfo.ElectricThreshold + "'";
            var reader = _command.ExecuteReader();
            if (reader.HasRows)
                while (reader.Read())
                {
                    var badId = reader.GetInt32(0);
                    badList.Add(badId);
                }
            reader.Close();
            if (badList.Count == 0)
                return new ElectrodeInspect(badList, -1);

            return new ElectrodeInspect(badList, maxId);
        }

        /// <summary>
        ///     获取配置信息
        /// </summary>
        public void GetPreferences()
        {
            _command.Reset();
            _command.CommandText = "SELECT value FROM Preferences WHERE parameter = 'FrameReceiveTimeout'";
            PresetInfo.FrameReceiveTimeout = int.Parse(_command.ExecuteScalar().ToString());
            _command.Reset();
            _command.CommandText = "SELECT value FROM Preferences WHERE parameter = 'FrameSendDelay'";
            PresetInfo.FrameSendDelay = int.Parse(_command.ExecuteScalar().ToString());
            _command.Reset();
            _command.CommandText = "SELECT value FROM Preferences WHERE parameter = 'ElectricThreshold'";
            PresetInfo.ElectricThreshold = int.Parse(_command.ExecuteScalar().ToString());
        }

        /// <summary>
        ///     设置配置信息
        /// </summary>
        public void UpdatePreferences()
        {
            _command.Reset();
            _command.CommandText = "UPDATE Preferences SET value = '" + PresetInfo.FrameReceiveTimeout +
                                   "' WHERE parameter = 'FrameReceiveTimeout'";
            _command.ExecuteNonQuery();
            _command.Reset();
            _command.CommandText = "UPDATE Preferences SET value = '" + PresetInfo.FrameSendDelay +
                                   "' WHERE parameter = 'FrameSendDelay'";
            _command.ExecuteNonQuery();
            _command.Reset();
            _command.CommandText = "UPDATE Preferences SET value = '" + PresetInfo.ElectricThreshold +
                                   "' WHERE parameter = 'ElectricThreshold'";
            _command.ExecuteNonQuery();
        }

        /// <summary>
        /// 从数据库中获取用于输出的Json格式数据
        /// </summary>
        /// <returns>用于输出的Json格式数据</returns>
        public string GetJsonFromDb()
        {
            //从'DataInfo'和'Preferences'表中获取已完成次数，总次数，电流阈值
            var preference = new Dictionary<string, int>();
            _command.Reset();
            _command.CommandText = "SELECT num FROM " + PresetInfo.DataInfoTable + " WHERE name = 'sum'";
            var sumNum = Convert.ToInt32(_command.ExecuteScalar());
            _command.Reset();
            _command.CommandText = "SELECT num FROM " + PresetInfo.DataInfoTable + " WHERE name = 'current'";
            var completedNum = Convert.ToInt32(_command.ExecuteScalar());
            _command.Reset();
            _command.CommandText = "SELECT value FROM " + PresetInfo.PreferencesTable +
                                   " WHERE parameter = 'ElectricThreshold'";
            var elecThreshold = Convert.ToInt32(_command.ExecuteScalar());

            preference.Add("sumNum", sumNum);
            preference.Add("completedNum", completedNum);
            preference.Add("elecThreshold", elecThreshold);

            //从'ElectrodeDetection'表中获取电极的电流检测信息
            var elecDetect = new List<Pole>(64);
            _command.Reset();
            _command.CommandText = "SELECT poleid,value FROM " + PresetInfo.ElectrodeDetectionTable;
            var reader = _command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var value = reader.GetDouble(1);
                    elecDetect.Add(new Pole(id, value));
                }
            }
            //从'ElectrodeResult'表中获取采集结果数据
            var collectItems = new List<CollectItem>(sumNum);
            _command.Reset();
            _command.CommandText = "SELECT * FROM " + PresetInfo.ElectrodeResultTable;
            reader = _command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var no = reader.GetInt32(0);
                    var a = reader.GetInt32(1);
                    var b = reader.GetInt32(2);
                    var m = reader.GetInt32(3);
                    var poles = new List<Pole>(64);
                    for (var i = 0; i < 64; i++)
                    {
                        var value = reader.GetDouble(i + 4);
                        poles.Add(new Pole(i, value));
                    }
                    var elec = reader.GetDouble(68);
                    collectItems.Add(new CollectItem(no, a, b, m, elec, poles));
                }
            }
            return null;
        }
    }
}