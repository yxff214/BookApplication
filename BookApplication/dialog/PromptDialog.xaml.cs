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

namespace BookApplication
{
    /// <summary>
    /// PromptDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PromptDialog : Window
    {
        public TextBlock Text { get => mTBTitle; }
        /// <summary>
        /// 提示窗口
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="context">副标题</param>
        /// <param name="bt1">确定按钮</param>
        /// <param name="bt2">取消按钮</param>
        private PromptDialog(string title = null, string context = null, string bt1 = "确定", string bt2 = "取消")
        {
            InitializeComponent();

            mTBTitle.Text = title;
            mTBContext.Text = context;

            if (bt1 != null)
            {
                mBTOk.Content = bt1;
            }
            if (bt2 != null)
            {
                mBTCancel.Content = bt2;
            }
        }

        private void click_ok(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void click_cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public static bool ShowPrompt(string title = null, string context = null, string bt1 = null, string bt2 = null)
        {
            PromptDialog pd = new PromptDialog(title, context, bt1, bt2);
            pd.Owner = Application.Current.MainWindow;
            bool? b = pd.ShowDialog();
            return b != null && b == true;
        }
    }
}
