using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using bitkyFlashresUniversal.connClient.model.bean;
using bitkyFlashresUniversal.databaseUtil.presenter;
using bitkyFlashresUniversal.ElectrodeDetection;

namespace bitkyFlashresUniversal.databaseUtil
{
    internal class SqliteBitky
    {
        private readonly SQLiteConnection _conn;
        private readonly ISqlPresenter _presenter;
        private SQLiteCommand command;

        public SqliteBitky(ISqlPresenter presenter)
        {
            _presenter = presenter;
            _conn = new SQLiteConnection("Data Source = " + PresetInfo.DatabasePath + "; Version = 3;");
            _conn.Open();
            command = _conn.CreateCommand();
        }


        private void ConnClose()
        {
               _conn?.Close();
        }

        /// <summary>
        ///     检查数据表的正确性,若数据表正确，则显示数据表轮廓信息
        /// </summary>
        public bool CheckTable()
        {
            command.Reset();
            //获取条目总数
            command.CommandText = "SELECT COUNT(*) FROM " + PresetInfo.ElectrodeControllerTable;
            var countRow = int.Parse(command.ExecuteScalar().ToString());
            if (countRow == 0)
            {
                return false;
            }

            //获取被记录的条目总数
            command.Reset();
            command.CommandText = "SELECT num FROM " + PresetInfo.DataInfoTable + " where name = 'sum'";
            var countRowInfo = int.Parse(command.ExecuteScalar().ToString());
            if (countRowInfo == 0)
                return false;
            //获取数据表最后一行的ID
            command.Reset();
            command.CommandText = "SELECT num FROM " + PresetInfo.ElectrodeControllerTable +
                                  " order by num desc limit 1";
            var lastRowInfo = int.Parse(command.ExecuteScalar().ToString());
            //获取当前指向的条目编号
            var currentRow = 0;

            command.Reset();
            command.CommandText = "SELECT num FROM " + PresetInfo.DataInfoTable + " WHERE name = 'current'";
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                    currentRow = reader.GetInt32(0);
            }
            else
            {
                command.Reset();
                command.CommandText = "INSERT INTO DataInfo (name, num) VALUES ('current', '1')";
                command.ExecuteNonQuery();
            }
            //判断所获取数据的合法性
            if ((countRow != countRowInfo) || (countRow != lastRowInfo) || (currentRow > countRow))
                return false;

            //数据合法，获准通过
            _presenter.SetTableOutline(countRow, currentRow);
            return true;
        }

        /// <summary>
        ///     查询一行的数据
        /// </summary>
        /// <param name="rowNum">行号</param>
        /// <returns>待发送的数据帧的对象</returns>
        public FrameData SelectRowData(int rowNum)
        {
            command.Reset();
            command.CommandText = "SELECT * FROM " + PresetInfo.ElectrodeControllerTable + " WHERE num = '" + rowNum +
                                  "'";

            var reader = command.ExecuteReader();
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
            command.Reset();
            command.CommandText = sqlText;
            var i = command.ExecuteNonQuery();
            return i != 0;
        }

        public void UpdateCurrentRowNum(int rowNum)
        {
            command.Reset();
            command.CommandText = "UPDATE " + PresetInfo.DataInfoTable + " SET num = '" + rowNum +
                                  "' WHERE name = 'current'";
            command.ExecuteNonQuery();
        }

        /// <summary>
        ///     第二次检测准备，获取良好的电极及不良的电极
        /// </summary>
        /// <returns></returns>
        public ElectrodeInspect SecondRroundInspect()
        {
            var badList = new List<int>();
            command.Reset();
            command.CommandText = "SELECT max(value) FROM " + PresetInfo.ElectrodeDetectionTable;
            var maxValue = double.Parse(command.ExecuteScalar().ToString());
            command.CommandText = "SELECT poleid FROM " + PresetInfo.ElectrodeDetectionTable + " WHERE value = '" +
                                  maxValue +
                                  "'";
            var maxId = int.Parse(command.ExecuteScalar().ToString());
            command.CommandText = "SELECT poleid FROM ElectrodeDetection WHERE value <= '" +
                                  PresetInfo.ElectricThreshold + "'";
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var badId = reader.GetInt32(0);
                    badList.Add(badId);
                }
            }
            reader.Close();
            if (badList.Count == 0)
            {
                return new ElectrodeInspect(badList, -1);
            }

            return new ElectrodeInspect(badList, maxId);
        }
    }
}