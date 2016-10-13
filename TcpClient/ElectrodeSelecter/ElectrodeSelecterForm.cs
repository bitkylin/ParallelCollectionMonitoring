using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using bitkyFlashresUniversal.connClient.model.bean;
using bitkyFlashresUniversal.connClient.model.commtUtil;
using bitkyFlashresUniversal.connClient.view;
using bitkyFlashresUniversal.view;
using CCWin;
using CCWin.SkinControl;

namespace bitkyFlashresUniversal.ElectrodeSelecter
{
    public partial class ElectrodeSelecterForm : Skin_Color
    {
        private readonly List<SkinCheckBox> _listCheckBox = new List<SkinCheckBox>();
        private ProcessPresenter _processPresenter;
        private BitkyMainWindow _window;
        private ControlFrameBuilder _controlFrameBuilder;

        public ElectrodeSelecterForm(BitkyMainWindow window)
        {
            InitializeComponent();
            _window = window;
            _controlFrameBuilder=new ControlFrameBuilder();

            foreach (Control c in groupBoxPoleSelect.Controls)
                _listCheckBox.Add((SkinCheckBox) c);
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            // 由复选框集合生成已选择的已排序的电极集合
            var list = InitPoleList();

            if (list.Count < 4)
            {
                MessageBox.Show("请选择至少4个电极！", "警告");
                return;
            }

            //新建数据处理类
            _processPresenter = new ProcessPresenter(list);
            var listReturn = _processPresenter.Process();
            var fileStream=new FileStream("lmlPoleWrite", FileMode.Create,FileAccess.Write);
            var binaryWriter=new BinaryWriter(fileStream);

            var conn = new SQLiteConnection("Data Source = " + PresetInfo.DatabasePath + "; Version = 3;");
            conn.Open();

            var trans = conn.BeginTransaction();
            var cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = "DELETE FROM sqlite_sequence";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM DataInfo";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM ElectrodeController";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM ElectrodeResult";
                cmd.ExecuteNonQuery();
                listReturn.ForEach(list2 =>
                {
                    binaryWriter.Write( _controlFrameBuilder.DataFrameBuild(  new FrameData(FrameType.ControlGather, list2)));
                    var poleGroup = new ElectrodeGroup();
                    list2.ForEach(pole =>
                    {
                        if (pole.Mode == PoleMode.A)
                            poleGroup.TypeA = pole.IdOrigin;
                        else if (pole.Mode == PoleMode.B)
                            poleGroup.TypeB = pole.IdOrigin;
                        else if (pole.Mode == PoleMode.M)
                            poleGroup.TypeM = pole.IdOrigin;
                    });
                    if (poleGroup.IsReady())
                    {
                        cmd.CommandText = "INSERT INTO " + PresetInfo.ElectrodeControllerTable + "(typeA,typeB,typeM) VALUES (" +
                                          poleGroup.TypeA + "," + poleGroup.TypeB + "," + poleGroup.TypeM + ")";
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        throw new Exception();
                    }
                });
                cmd.CommandText = "SELECT COUNT(*) FROM " + PresetInfo.ElectrodeControllerTable;
                var countRow = int.Parse(cmd.ExecuteScalar().ToString());
                cmd.CommandText = "INSERT INTO DataInfo (name, num) VALUES ('sum', '" + countRow + "')";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO DataInfo (name, num) VALUES ('current', '1')";
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
            }
            trans.Commit();
            conn.Close();
            this.Close();
            _window.SetElectrodeSuccessful();

            binaryWriter.Close();
            fileStream.Close();
        }



        /// <summary>
        ///     复选框选取范围确认
        /// </summary>
        private void btnconfirm_Click(object sender, EventArgs e)
        {
            var beginStr = textBoxCheckedBoxBegin.Text;
            var endStr = textBoxCheckedBoxEnd.Text;
            if (IsInt(beginStr) && IsInt(endStr))
            {
                var begin = int.Parse(beginStr);
                var end = int.Parse(endStr);
                if ((begin < 0) || (begin >= 64) || (end < 0) || (end >= 64) || (begin >= end))
                    MessageBox.Show("输入的值有误，请重新输入！", "警告");
                else
                    _listCheckBox.ForEach(checkBox =>
                    {
                        var num = int.Parse(checkBox.Text);
                        if ((num >= begin) && (num <= end))
                            checkBox.Checked = true;
                    });
            }
            else
                MessageBox.Show("输入的值必须为正整数，请重新输入！", "警告");
        }

        /// <summary>
        ///     复选框全部重置
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            _listCheckBox.ForEach(checkBox => { checkBox.Checked = false; });
        }

        /// <summary>
        ///     判断是正整数
        /// </summary>
        /// <param name="value">待匹配的文本</param>
        /// <returns>匹配结果</returns>
        private static bool IsInt(string value) //判断是正整数
        {
            value = value.Trim();
            return Regex.IsMatch(value, @"^[1-9]\d*|0$");
        }

        /// <summary>
        ///     电极排序方法
        /// </summary>
        private static int CompareSort(Electrode x, Electrode y)
        {
            if (x.IdOrigin > y.IdOrigin)
                return 1;
            if (x.IdOrigin < y.IdOrigin)
                return -1;
            return 0;
        }

        /// <summary>
        ///     由复选框集合生成已选择的已排序的电极集合
        /// </summary>
        /// <returns>已排序的电极集合</returns>
        private List<Electrode> InitPoleList()
        {
            //将选中的电极添加入集合
            var list = new List<Electrode>();
            _listCheckBox.ForEach(checkBox =>
            {
                if (checkBox.Checked)
                    list.Add(new Electrode(int.Parse(checkBox.Text)));
            });
            //对集合进行排序
            Comparison<Electrode> sort = CompareSort;
            list.Sort(sort);
            var i = 1;
            //对电极重新编号
            list.ForEach(pole =>
            {
                pole.IdCurrent = i;
                i++;
            });
            return list;
        }
    }
}