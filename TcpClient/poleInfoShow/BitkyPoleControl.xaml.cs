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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace bitkyFlashresUniversal.poleInfoShow
{
    /// <summary>
    /// _bitkyPoleControl.xaml 的交互逻辑
    /// </summary>
    public partial class BitkyPoleControl : UserControl
    {
        public int _id { get; private set; } = -1;
        Color colorRed = Color.FromRgb(255, 0, 0);
        Color colorBlue = Color.FromRgb(0, 255, 200);

        public BitkyPoleControl()
        {
            InitializeComponent();
        }

        public void setContent(int id)
        {
            Name = "bitkyPoleControl" + id;
            labelPoleId.Content = id;
            _id = id;
        }

        public void setInfo(int num)
        {
            labelNum.Content = num;
        }

        /// <summary>
        /// 设置背景颜色，0:绿  1:红
        /// </summary>
        /// <param name="i"></param>
        public void setColor(int i)
        {
            if (i == 0)
            {
                Background = new SolidColorBrush(colorBlue);
            }
            if (i == 1)
            {
                Background = new SolidColorBrush(colorRed);
            }
        }
    }
}