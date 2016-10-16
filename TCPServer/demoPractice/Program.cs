using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;

namespace demoPractice
{
    internal class Program
    {
        static private SQLiteConnection conn;
        private const string ElectrodeTable = "ElectrodeController";
        private const string DataInfoTable = "DataInfo";
        private const string DatabasePath = "./bitkyDataReady.db";


        private static void Main(string[] args)
        {
            Program program = new Program();
            //            conn = new SQLiteConnection("Data Source = ./bitkyDataReady.db; Version = 3; ");
            //            //            insertData();
            //            //            selectData();
            //            //            getCount();
            //            // SelectSpecialData();
            //            getLastRow();
            conn = new SQLiteConnection("Data Source = " + DatabasePath + "; Version = 3;");
            bool value = program.CheckTable();
            Debug.WriteLine(value);
        }

        public bool CheckTable()
        {
            var _command = ConnOpen();
            //获取条目总数
            _command.CommandText = "SELECT COUNT(*) FROM " + ElectrodeTable;
            var countRow = int.Parse(_command.ExecuteScalar().ToString());
            if (countRow == 0)
                return false;
            //获取被记录的条目总数
            _command.Reset();
            _command.CommandText = "SELECT num FROM " + DataInfoTable + " where name = 'sum'";
            var countRowInfo = int.Parse(_command.ExecuteScalar().ToString());
            if (countRowInfo == 0)
                return false;
            //获取数据表最后一行的ID
            _command.Reset();
            _command.CommandText = "SELECT num FROM " + ElectrodeTable + " order by num desc limit 1";
            var lastRowInfo = int.Parse(_command.ExecuteScalar().ToString());
            //获取当前指向的条目编号
            var currentRow = 0;
            _command.Reset();
            _command.CommandText = "SELECT num FROM " + DataInfoTable + " WHERE name = 'current'";
            var reader = _command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    currentRow = reader.GetInt32(0);
                }
            }
            else
            {
                _command.Reset();
                _command.CommandText = "INSERT INTO DataInfo (name, num) VALUES ('current', '0')";
                _command.ExecuteNonQuery();
            }
            //判断所获取数据的合法性
            if (countRow != countRowInfo || countRow != lastRowInfo || currentRow > countRow)
                return false;
            ConnClose();
            //数据合法，获准通过
            return true;
        }

        public SQLiteCommand ConnOpen()
        {
            conn.Open();
            return conn.CreateCommand();
        }

        public void ConnClose()
        {
            conn?.Close();
        }


        /// <summary>
        /// 向表中插入条目
        /// </summary>
        static void insertData()
        {
            conn.Open();
            var cmd = conn.CreateCommand();

            cmd.CommandText = "INSERT INTO ElectrodeController(typeA,typeB,typeM) VALUES (1, 2, 3)";
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        /// <summary>
        /// 查询指定条目的内容
        /// </summary>
        static void selectData()
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM ElectrodeController where num > 45";
            try
            {
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Debug.WriteLine("ID: " + reader.GetInt32(0));
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
            conn.Close();
        }

        /// <summary>
        /// 查询指定条目的内容
        /// </summary>
        private static void SelectSpecialData()
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT num FROM DataInfo where name = 'current'";

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Debug.WriteLine(" : " + reader.GetInt32(0));
                }
            }
            else
            {
                cmd.Reset();
                cmd.CommandText = "INSERT INTO DataInfo (name, num) VALUES ('current', '0')";
                cmd.ExecuteNonQuery();
            }

            conn.Close();
        }

        /// <summary>
        /// 获取数据库中条目的数量
        /// </summary>
        static void getCount()
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM ElectrodeController";
            var scalar = cmd.ExecuteScalar();
            Debug.WriteLine("count: " + Convert.ToInt32(scalar));
            conn.Close();
        }

        /// <summary>
        /// 查询数据表最后一行的信息
        /// </summary>
        static void getLastRow()
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT num FROM ElectrodeController order by num desc limit 1";
            var scalar = cmd.ExecuteScalar();
            Debug.WriteLine("count: " + Convert.ToInt32(scalar));
            conn.Close();
        }
    }
}