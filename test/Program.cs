using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test
{
    class ProductExtension
    {
        public ProductExtension(string info)
        {
            int i = 24;
        }

        /// <summary>
        /// 合成班级
        /// </summary>
        public string Syn { get; set; }
        /// <summary>
        /// 纯化班级
        /// </summary>
        public string Pur { get; set; }

        /// <summary>
        /// 生产主管备注
        /// </summary>
        public string SupervisorComments { get; set; }
        /// <summary>
        /// 合成备注
        /// </summary>
        public string SynComments { get; set; }
        /// <summary>
        /// 纯化备注
        /// </summary>
        public string PurComments { get; set; }

        public string GetExtensionInfo()
        {
            StringBuilder sb = new StringBuilder();

            foreach(var v in this.GetType().GetProperties())
            {
                sb.Append(v.GetValue(this)).Append(',');
            }

            return sb.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IntPtr hWnd = FindWindow(null, "AutoControlPdg2pic  Pause键暂停");
            while (true)
            {
                if ((GetAsyncKeyState(1) & 0x8000) > 0)
                {
                    if (hWnd == null)
                    {
                        hWnd = FindWindow(null, "AutoControlPdg2pic  Pause键暂停");
                        Console.WriteLine("为0");
                    }
                    if (hWnd == null) continue;
                    ShowWindow(hWnd, 6);
                    Console.WriteLine("你好");
                }
                Thread.Sleep(50);
            }
        }

        [DllImport("User32")]
        public static extern short GetAsyncKeyState(int vKey);
        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string IpClassName, string IpWIndowName);
    }
}
