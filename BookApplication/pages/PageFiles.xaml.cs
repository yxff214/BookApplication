using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace BookApplication
{
    /// <summary>
    /// 文件页面
    /// </summary>
    public partial class PageFiles : Page
    {
        // TODO 分享记录
        private ObservableCollection<NnFile> mList;
        private Dictionary<long, NnPanManager> mPanManagers;

        private int mSearchIndex = -1;
        public PageFiles()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            mList = NnReader.Instance.GetFiles();
            mDGMain.ItemsSource = mList;

            ObservableCollection<Account> list = NnReader.Instance.GetAccounts();
            mPanManagers = new Dictionary<long, NnPanManager>();
            foreach(var v in list)
            {
                mPanManagers.Add(v.Id, new NnPanManager(v));
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch (((Control)sender).Tag)
            {
                case "shear":// 分享
                    _shear();
                    break;
            }
        }
        /// <summary>
        /// 分享
        /// </summary>
        private void _shear()
        {
            List<NnFile> list = _getSelectedItems();
            StringBuilder sb = new StringBuilder();
            foreach(var v in list)
            {
                if (v.PanFile.isdir == 1)
                {
                    sb.Append(v.PanFile.server_filename).Append("\t是文件夹，无法分享\n");
                    continue;
                }
                try
                {
                    ShareInfo info = mPanManagers[v.AccountId].Share(v, "ersf");

                    if (info.error != 0)
                    {
                        sb.Append(v.PanFile.server_filename).Append("\t").Append("分享错误\n");
                    }
                    else
                    {
                        sb.Append(v.PanFile.server_filename).Append("\t").Append("连接：").Append(info.link).Append(" 提取码：ersf\n");
                    }
                    Console.WriteLine(info.createsharetips_ldlj);
                }catch(Exception e) { Console.WriteLine(e.ToString()); }
            }
            Clipboard.SetText(sb.ToString());
            WarnWindow.ShowMessage("分享信息已复制到剪贴板！\n" + sb.ToString());
        }

        private List<NnFile> _getSelectedItems()
        {
            List<NnFile> list = new List<NnFile>();
            foreach(var v in mDGMain.SelectedCells)
            {
                NnFile file = v.Item as NnFile;
                if(file != null && !list.Contains(file))
                {
                    list.Add(file);
                } 
            }
            return list;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (((Control)sender).Tag)
            {
                case "search":// 搜搜
                    _search();
                    break;
                default:return;
            }
        }

        private void _search()
        {
            string str = mTBSearch.Text.Trim();
            for (++mSearchIndex; mSearchIndex < mList.Count;++mSearchIndex)
            {
                if (mList[mSearchIndex].PanFile.server_filename.Contains(str))
                {
                    mDGMain.ScrollIntoView(mList[mSearchIndex]);
                    mDGMain.SelectedItem = mList[mSearchIndex];
                    mDGMain.Focus();
                    mSearchIndex = -1;
                    return;
                }
            }
            mSearchIndex = -1;
            NnMessage.ShowMessage("已到达搜索终点，未找到记录！");
        }
    }
}
