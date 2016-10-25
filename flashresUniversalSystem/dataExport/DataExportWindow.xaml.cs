using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using bitkyFlashresUniversal.connClient.presenter;
using bitkyFlashresUniversal.dataExport.bean;

namespace bitkyFlashresUniversal.dataExport
{
    /// <summary>
    /// DataExportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DataExportWindow : Window
    {
        private readonly ICommPresenter _commPresenter;

        public DataExportWindow(ICommPresenter commPresenter1)
        {
            _commPresenter = commPresenter1;
            InitializeComponent();
        }

        public void temp()
        {
            var poles = new List<Pole> {new Pole(1, 1.1), new Pole(2, 2.1), new Pole(3, 3.1)};
            var collectItem = new CollectItem(1, 2, 3, 4, 4.321, poles);
            var poles2 = new List<Pole> {new Pole(11, 1.1), new Pole(12, 2.1), new Pole(13, 3.1)};
            var collectItem2 = new CollectItem(12, 22, 32, 42, 4.3212, poles2);

//            var dataJson = new SummaryDataJson(new List<int>() {16, 17},
//                new List<CollectItem>() {collectItem, collectItem2});

//            DataExport.Output(dataJson);
        }

        public void DataClear()
        {
            _commPresenter.GatherDataClear();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
          string dataJson =  _commPresenter.GetJsonFromDb();
            temp();
            MessageBox.Show("数据已存入指定文件中", "提示");
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("您确定要清空数据库吗？执行此操作后将无法撤销", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _commPresenter.GatherDataClear();
                MessageBox.Show("数据库清空完毕", "提示");
                Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}