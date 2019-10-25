using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace BookApplication
{
    /// <summary>
    /// 账号页面
    /// </summary>
    public partial class PageAccount : Page
    {
        private ObservableCollection<Account> mList;
        public PageAccount()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            mList = NnReader.Instance.GetAccounts();
            mDGMain.ItemsSource = mList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (((Control)sender).Tag)
            {
                case "add":// 添加
                    _add();
                    break;
                default:return;
            }
        }
        /// <summary>
        /// 添加账户（注意添加的时候扫描文件目录）
        /// </summary>
        private void _add()
        {
            Account a = new Account();
            a.Name = mTBName.Text.Trim();
            a.BDUSS = mTBBDUSS.Text.Trim();
            a.STOKEN = mTBSTOKEN.Text.Trim();
            int count = NnReader.Instance.InsertAccount(a);
            if (count > 0)
            {
                init();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch (((Control)sender).Tag)
            {
                case "rescan":// 重新扫描
                    _scan();
                    break;
                default:break;
            }
        }

        private void _scan()
        {
            bool b = PromptDialog.ShowPrompt("重新扫描文件目录", "确定重新扫描文件目录吗？点击确定继续！");
            if (!b) return;
            new Thread(_rescan).Start();
        }

        /// <summary>
        /// 重新扫描
        /// </summary>
        private void _rescan()
        {
            Dispatcher.Invoke(() => (Application.Current.MainWindow as MainWindow)?.StatusBarState(true, "正在扫描..."));
            Account a = _getSelectedAccount();
            if (a == null) return;

            NnPanManager npm = new NnPanManager(a);
            string str = npm.ScanFile();
            Dispatcher.Invoke(() => (Application.Current.MainWindow as MainWindow)?.StatusBarState());
            NnMessage.ShowMessage("扫描结束");
        }

        /// <summary>
        /// 获取被选中的账号
        /// </summary>
        /// <returns></returns>
        private Account _getSelectedAccount()
        {
            foreach(var v in mDGMain.SelectedCells)
            {
                Account a = v.Item as Account;
                if (a != null)
                {
                    return a;
                }
            }
            return null;
        }
    }
}
