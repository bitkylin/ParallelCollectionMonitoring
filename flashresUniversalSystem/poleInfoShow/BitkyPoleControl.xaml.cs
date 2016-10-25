using System.Windows.Media;

namespace bitkyFlashresUniversal.poleInfoShow
{
    /// <summary>
    ///     _bitkyPoleControl.xaml 的交互逻辑
    /// </summary>
    public partial class BitkyPoleControl
    {
        private readonly Color _colorBlue = Color.FromArgb(75, 0, 255, 200);
        //改变颜色
        private readonly Color _colorRed = Color.FromArgb(75, 255, 0, 0);

        private readonly Color _colorWrite = Color.FromArgb(50, 255, 255, 255);

        private readonly Color _colorGray = Color.FromArgb(50, 0, 0, 0);

        private readonly Color _colorBlack = Color.FromArgb(95, 0, 0, 0);

        public bool isEnabled = false;

        public BitkyPoleControl()
        {
            InitializeComponent();
        }

        public void SetInvaild()
        {
            isEnabled = false;
            LabelNum.Content = "失效";
         //   Background = new SolidColorBrush(_colorGray);
            GridMain.Background=new SolidColorBrush(_colorGray);
        }

        public void SetVaild()
        {
            isEnabled = true;
            LabelNum.Content = "待获取";
          //  Background = new SolidColorBrush(_colorBlack);
            GridMain.Background = new SolidColorBrush(_colorBlack);
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
            if (isEnabled)
            {
                LabelNum.Content = num;
            }
        }

        /// <summary>
        ///     设置背景颜色，0:绿  1:红
        /// </summary>
        /// <param name="i"></param>
        public void SetColor(int i)
        {
            if (isEnabled)
            {
                if (i == 0)
                    Background = new SolidColorBrush(_colorBlue);
                if (i == 1)
                    Background = new SolidColorBrush(_colorRed);
            }
        }
    }
}