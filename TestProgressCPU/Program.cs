using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestProgressCPU
{

    class TestCPU
    {
        /// <summary>
        /// 测试进程CPU占用
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string str = System.Web.HttpUtility.UrlDecode("%2F");
            DateTime dt = new DateTime(1571497395535);
            string st = dt.ToString();
            long l = DateTime.Now.Ticks;
        }
    }
}
