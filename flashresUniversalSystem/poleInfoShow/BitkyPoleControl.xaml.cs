using System.Windows.Media;

namespace bitkyFlashresUniversal.poleInfoShow
{
    /// <summary>
    ///     _bitkyPoleControl.xaml 的交互逻辑
    /// </summary>
    public partial class BitkyPoleControl
    {
        private readonly Color _colorBlue = Color.FromRgb(0, 255, 200);
        //改变颜色
        private readonly Color _colorRed = Color.FromRgb(255, 0, 0);

        public BitkyPoleControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     根据参数初始化该控件
        /// </summary>
        /// <param name="id">输入的参数</param>
        public void SetContent(int id)
        {
            Name = "bitkyPoleControl" + id;
            LabelPoleId.Content = id;
        }

        public void SetValue(double num)
        {
            LabelNum.Content = num;
        }

        /// <summary>
        ///     设置背景颜色，0:绿  1:红
        /// </summary>
        /// <param name="i"></param>
        public void SetColor(int i)
        {
            if (i == 0)
                Background = new SolidColorBrush(_colorBlue);
            if (i == 1)
                Background = new SolidColorBrush(_colorRed);
        }
    }
}