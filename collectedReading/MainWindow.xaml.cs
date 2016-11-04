using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using collectedReading.jsonBean;
using collectedReading.jsonBean.display;
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

            for (var i = -1; i < 64; i=i+2)
            {
                var column = new DataColumn();
                column.ColumnName = i.ToString();
                dataTable.Columns.Add(column);
            }
            for (var i = 0; i < 64; i=i+2)
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
            var dataJson = GetDataJson();


            var collectItems = dataJson.PoleResult;
            var itemDisplays = new List<CollectedResult>();


          var items =  dataGridElecmatrixShow.Items;

            collectItems.ForEach(pole =>
            {
                itemDisplays.Add(new CollectedResult(pole));
                var item = items[pole.A/2] as DataRowView;
                item.Row[pole.B/2 + 1] = Math.Round(pole.Elec,2);
            });


            dataGridPoleResultShow.ItemsSource = itemDisplays;
            dataGridElecShow.ItemsSource = dataJson.ElecDetect;
            var dictionary = dataJson.Preference;
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
            
            labelDatatime.Content = dataJson.DateTime.ToString("G");
            labelUser.Content = dataJson.UserName;
            labelNote.Content = dataJson.Note;
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
    }
}