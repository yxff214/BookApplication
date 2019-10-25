using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace BookApplication
{
    class NnPanManager
    {
        private CookieContainer m_cookie;

        private Account m_account;
        public NnPanManager(Account a)
        {
            m_account = a;
            m_cookie = new CookieContainer();
            m_cookie.Add(new Uri("https://pan.baidu.com"), new Cookie("BDUSS", a.BDUSS));
            m_cookie.Add(new Uri("https://pan.baidu.com"), new Cookie("STOKEN", a.STOKEN));

        }
        public string ScanFile(int page = 1, int num = 500, string dir = "%2F")
        {
            List<NnPanFile> list = new List<NnPanFile>();// 文件夹
            while (true)
            {
                string url = $"https://pan.baidu.com/api/list?order=time&desc=1&showempty=0&web=1&page={page}&num={num}&dir={dir}&t=0.9934258251250032&channel=chunlei&web=1&app_id=250528&bdstoken={m_account.BDStoken}&logid={m_account.LogId}&clienttype=0&startLogTime=1571497395535";
                NnPanInfos info = _getPanInfos(url);
                if (info == null) continue;
                if (info.list != null)
                {
                    foreach (var v in info.list)// 处理文件夹
                    {
                        if (v.isdir == 1)
                        {
                            list.Add(v);
                        }
                    }
                }
                // 将数据写入数据库
                int count = NnReader.Instance.InsertFiles(info, m_account);
                if (info.errno != 0)
                {
                    return info.errno.ToString();
                }
                else if (info.list.Count < num)
                {
                    break;
                }
                ++page;
            }
            // 处理文件夹
            foreach (var v in list)
            {
                if (v.empty > 0) continue;
                string s = v.path;
                string ss = System.Web.HttpUtility.UrlEncode(v.path);
                ScanFile(1, 500, ss);
            }
            return "success";
        }
        /// <summary>
        /// 获取网盘文件夹列表
        /// </summary>
        private NnPanInfos _getPanInfos(string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.CookieContainer = m_cookie;
            request.Host = "pan.baidu.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362";
            request.Method = "GET";
            request.KeepAlive = true;
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string str = reader.ReadToEnd();
            response.Close();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            NnPanInfos info = serializer.Deserialize<NnPanInfos>(str);
            return info;
        }

        internal ShareInfo Share(NnFile file, string key)
        {
            HttpWebRequest request = WebRequest.CreateHttp($"https://pan.baidu.com/share/set?channel=chunlei&clienttype=0&web=1&channel=chunlei&web=1&app_id=250528&bdstoken=&logid=&clienttype=0");
            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.CookieContainer = m_cookie;
            request.Host = "pan.baidu.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362";
            request.Accept = "*/*";

            string flist = System.Web.HttpUtility.UrlEncode($"[{file.PanFile.fs_id}]");
            string clist = System.Web.HttpUtility.UrlEncode("[]");
            byte[] bytes = Encoding.UTF8.GetBytes($"channel_list={clist}&fid_list={flist}&period=7&pwd={key}&schannel=4");
            request.ContentLength = bytes.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();

            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string resstring = reader.ReadToEnd();
            response.Close();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ShareInfo info = serializer.Deserialize<ShareInfo>(resstring);

            return info;
        }
    }

    /// <summary>
    /// 网盘信息
    /// </summary>
    public class NnPanInfos
    {
        public int errno { get; set; }
        public string guid_info { get; set; }
        public long request_id { get; set; }
        public int guid { get; set; }

        public List<NnPanFile> list { get; set; }
    }
    /// <summary>
    /// 网盘文件
    /// </summary>
    public class NnPanFile
    {
        public int server_mtime { get; set; }
        public int category { get; set; }
        public int unlist { get; set; }
        public long fs_id { get; set; }
        public long size { get; set; }
        public int isdir { get; set; }
        public int iper_id { get; set; }
        public int server_ctime { get; set; }
        public NnPanThumbs thumbs { get; set; }
        public string lodocpreview { get; set; }
        public int local_mtime { get; set; }
        public string md5 { get; set; }
        public string docpreview { get; set; }
        public int share { get; set; }
        public string path { get; set; }
        public int local_ctime { get; set; }
        public string server_filename { get; set; }
        public int empty { get; set; }
    }
    /// <summary>
    /// 文件中的链接
    /// </summary>
    public class NnPanThumbs
    {
        public string url3 { get; set; }
        public string url2 { get; set; }
        public string url1 { get; set; }
    }

    public class NnFile
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 网盘文件
        /// </summary>
        public NnPanFile PanFile { get; set; }

        public int AccountId { get; set; }

        internal void InitNnFileByDb(OleDbDataReader reader)
        {
            Id = NnReader.GetIntFromDb(reader, "ID");
            PanFile.server_mtime = NnReader.GetIntFromDb(reader, "server_mtime");
            PanFile.category = NnReader.GetIntFromDb(reader, "category");
            PanFile.unlist = NnReader.GetIntFromDb(reader, "unlist");
            string s = NnReader.GetStringFromDb(reader, "fs_id");
            long.TryParse(s, out long l);
            PanFile.fs_id = l;
            s = NnReader.GetStringFromDb(reader, "size");
            long.TryParse(s, out l);
            PanFile.size = l;
            PanFile.isdir = NnReader.GetIntFromDb(reader, "isdir");
            PanFile.iper_id = NnReader.GetIntFromDb(reader, "iper_id");
            PanFile.server_ctime = NnReader.GetIntFromDb(reader, "server_ctime");
            PanFile.lodocpreview = NnReader.GetStringFromDb(reader, "lodocpreview");
            PanFile.local_mtime = NnReader.GetIntFromDb(reader, "local_mtime");
            PanFile.md5 = NnReader.GetStringFromDb(reader, "md5");
            PanFile.docpreview = NnReader.GetStringFromDb(reader, "docpreview");
            PanFile.share = NnReader.GetIntFromDb(reader, "share");
            PanFile.path = NnReader.GetStringFromDb(reader,"path");
            PanFile.local_ctime = NnReader.GetIntFromDb(reader, "local_ctime");
            PanFile.server_filename = NnReader.GetStringFromDb(reader, "server_filename");
            PanFile.empty = NnReader.GetIntFromDb(reader, "empty");
            AccountId = NnReader.GetIntFromDb(reader, "accountid");
        }

        public NnFile()
        {
            PanFile = new NnPanFile();
        }
    }

    public class ShareInfo
    {
        public int error { get; set; }
        public long request_id { get; set; }
        public long shareid { get; set; }
        public string link { get; set; }
        public string shorturl { get; set; }
        public long ctime { get; set; }
        public int expiredType { get; set; }
        public string createsharetips_ldlj { get; set; }
        public bool premis { get; set; }
    }
}
