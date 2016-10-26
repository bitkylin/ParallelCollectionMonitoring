using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using bitkyFlashresUniversal.connClient.presenter;

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
//            Timer timer = new Timer(5000);
//            timer.AutoReset = true;
//            timer.Elapsed += FormTop;
//            timer.Start();
        }

        private void FormTop(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("保持在顶端");
            
            Topmost = true;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var dataJson = _commPresenter.GetJsonFromDb();
            DataExport.OutputJson(dataJson);
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