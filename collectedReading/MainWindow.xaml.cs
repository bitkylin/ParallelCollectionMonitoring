using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using collectedReading.jsonBean;
using collectedReading.jsonBean.display;
using collectedReading.produceData;
using collectedReading.produceData.bean;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace collectedReading
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable _dataTable;
        private SummaryDataJson _dataJson;

        public MainWindow()
        {
            InitializeComponent();
            _dataTable = InitDataTable();
            dataGridElecmatrixShow.ItemsSource = _dataTable.DefaultView;
        }

        /// <summary>
        /// 初始化数据表
        /// </summary>
        /// <returns></returns>
        private DataTable InitDataTable()
        {
            var dataTable = new DataTable();

            for (var i = -1; i < 64; i = i + 2)
            {
                var column = new DataColumn();
                column.ColumnName = i.ToString();
                dataTable.Columns.Add(column);
            }
            for (var i = 0; i < 64; i = i + 2)
            {
                var row = dataTable.NewRow();
                row[0] = i;
                dataTable.Rows.Add(row);
            }


            return dataTable;
        }


        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().Show();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _dataJson = GetDataJson();


            var collectItems = _dataJson.PoleResult;
            var itemDisplays = new List<CollectedResult>();


            var items = dataGridElecmatrixShow.Items;

            collectItems.ForEach(pole =>
            {
                itemDisplays.Add(new CollectedResult(pole));
                var item = items[pole.A/2] as DataRowView;
                item.Row[pole.B/2 + 1] = Math.Round(pole.Elec, 2);
            });


            dataGridPoleResultShow.ItemsSource = itemDisplays;
            dataGridElecShow.ItemsSource = _dataJson.ElecDetect;
            var dictionary = _dataJson.Preference;
            labelSumNum.Content = dictionary["sumNum"];
            labelCompletedNum.Content = dictionary["completedNum"];
            labelElecThreshold.Content = dictionary["elecThreshold"];
            try
            {
                labelEnabledPoleNum.Content = dictionary["enabledPoleNum"];
            }
            catch (KeyNotFoundException)
            {
                labelEnabledPoleNum.Content = dictionary["EnabledPoleNum"];
            }

            labelDatatime.Content = _dataJson.DateTime.ToString("G");
            labelUser.Content = _dataJson.UserName;
            labelNote.Content = _dataJson.Note;
        }

        private SummaryDataJson GetDataJson()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Title = "已采数据读取";
            fileDialog.InitialDirectory = Environment.CurrentDirectory;
            fileDialog.DefaultExt = ".json";
            fileDialog.Filter = "JSON文件 (*.json)|*.json|所有文件(*.*)|*.*";
            if (fileDialog.ShowDialog() != true) throw new Exception("操作错误");
            var fileUrl = fileDialog.FileName;
            var streamReader = new StreamReader(fileDialog.OpenFile());
            var jsonStr = streamReader.ReadToEnd();
            streamReader.Close();
            label.Content = fileUrl;
            var dataJson = JsonConvert.DeserializeObject<SummaryDataJson>(jsonStr);
            return dataJson;
        }

        private void btnOutputData_Click(object sender, RoutedEventArgs e)
        {
            var poleLocations = GetPoleArray();
            ProducePresenter.Builder()
                .SetDataJson(_dataJson)
                .SetPoleLocationArray(poleLocations)
                .ProduceOutputData();
        }

        private PoleLocation[] GetPoleArray()
        {
            var poleLocations = new PoleLocation[64];

            var leftStart = int.Parse(textBoxLeftStart.Text);
            var leftEnd = int.Parse(textBoxLeftEnd.Text);
            var rightStart = int.Parse(textBoxRightStart.Text);
            var rightEnd = int.Parse(textBoxRightEnd.Text);

            var semiLineInterval = int.Parse(textBoxLineInterval.Text)/2;
            var poleInterval = int.Parse(textBoxPoleInterval.Text);

            if (leftStart > leftEnd)
            {
                var i = 0;
                for (var p = leftStart; p >= leftEnd; p--)
                {
                    poleLocations[p] = new PoleLocation(p, semiLineInterval, i);
                    i = i + poleInterval;
                }
            }
            else
            {
                var i = 0;
                for (var p = leftStart; p <= leftEnd; p++)
                {
                    poleLocations[p] = new PoleLocation(p, semiLineInterval, i);
                    i = i + poleInterval;
                }
            }

            if (rightStart > rightEnd)
            {
                var i = 0;
                for (var p = rightStart; p >= rightEnd; p--)
                {
                    poleLocations[p] = new PoleLocation(p, -1*semiLineInterval, i);
                    i = i + poleInterval;
                }
            }
            else
            {
                var i = 0;
                for (var p = rightStart; p <= rightEnd; p++)
                {
                    poleLocations[p] = new PoleLocation(p, -1*semiLineInterval, i);
                    i = i + poleInterval;
                }
            }
            return poleLocations;
        }
    }
}