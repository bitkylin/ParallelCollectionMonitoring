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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var dataJson =  _commPresenter.GetJsonFromDb();
            DataExport.Output(dataJson);
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