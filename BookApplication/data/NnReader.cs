using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookApplication
{
    class NnReader
    {
        public delegate void NnAction(string v);

        private static NnReader mReader;

        private OleDbConnection mConnection;

        private NnReader()
        {
            int index = 12;
            string path = Environment.CurrentDirectory + "\\BookApplication.accdb";
            while (index < 21)
            {
                try
                {
                    mConnection = new OleDbConnection($"Provider=Microsoft.ACE.OLEDB.{index.ToString()}.0;Data Source={path}");
                    mConnection.Open();
                    return;
                }
                catch (Exception e) { ++index; Console.WriteLine(e.ToString()); }
            }
        }

        public static NnReader Instance
        {
            get
            {
                if (mReader == null)
                    mReader = new NnReader();
                return mReader;
            }
        }

        public void Close()
        {
            try
            {
                mConnection.Close();
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }

        // ====================以上是初始化===================

        // ---------------------功能区域-----------------------
        /// <summary>
        /// 获取所有账号
        /// </summary>
        /// <returns></returns>
        internal ObservableCollection<Account> GetAccounts()
        {
            ObservableCollection<Account> list = new ObservableCollection<Account>();
            try
            {
                using(OleDbCommand cmd = new OleDbCommand("SELECT * FROM accounts", mConnection))
                {
                    using(OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Account a = new Account();
                            a.InitAccountByDb(reader);
                            list.Add(a);
                        }
                    }
                }
            }catch(Exception e) { Console.WriteLine(e.ToString()); }
            return list;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="m_account"></param>
        internal int DeleteFIles(Account a)
        {

            int count = 0;
            try
            {
                using (OleDbCommand cmd = new OleDbCommand("DELETE FROM fileinfo WHERE [accountid]=@v1", mConnection))
                {
                    cmd.Parameters.AddWithValue("v1", a.Id);
                    count = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            return count;
        }
        /// <summary>
        /// 将文件信息写入数据库
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal int InsertFiles(NnPanInfos info, Account account)
        {
            int count = 0;
            try
            {
                using(OleDbCommand cmd = new OleDbCommand("INSERT INTO fileinfo ([server_mtime],[category],[unlist],[fs_id],[size],[isdir],[iper_id],[server_ctime],[thumbs],[lodocpreview],[local_mtime],[md5],[docpreview],[share],[path],[local_ctime],[server_filename],[empty],[accountid]) VALUES(@v1,@v2,@v3,@v4,@v5,@v6,@v7,@v8,@v9,@v10,@v11,@v12,@v13,@v14,@v15,@v16,@v17,@v18,@v19)", mConnection))
                {
                    foreach(var v in info.list)
                    {
                        cmd.Parameters.Clear();
                        foreach(var v2 in _getFileObjects(v,account))
                        {
                            cmd.Parameters.AddWithValue("", v2);
                        }
                        try
                        {
                            count += cmd.ExecuteNonQuery();
                        }
                        catch (Exception e){ Console.WriteLine(e.ToString()); }
                    }
                }
            }catch(Exception e) { Console.WriteLine(e.ToString()); }
            return count;
        }

        private object[] _getFileObjects(NnPanFile v, Account account)
        {
            return new object[] { v.server_ctime, v.category, v.category, v.fs_id.ToString(), v.size.ToString(), v.isdir, v.iper_id, v.server_ctime, v.thumbs==null?"":v.thumbs.url1+";"+v.thumbs.url2+";"+v.thumbs.url3, v.lodocpreview??"", v.local_mtime, v.md5??"", v.docpreview??"", v.share, v.path??"", v.local_ctime, v.server_filename??"", v.empty, account.Id };
        }

        /// <summary>
        /// 将账号写入数据库中
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        internal int InsertAccount(Account a)
        {
            int count  =0;
            try
            {
                using (OleDbCommand cmd = new OleDbCommand("INSERT INTO accounts ([Name],[BDUSS],[STOKEN],[BDStoken],[LogId]) VALUES(@v1,@v2,@v3,@v4,@v5)", mConnection))
                {
                    foreach(var v in a.GetObjects())
                    {
                        cmd.Parameters.AddWithValue("", v);
                    }
                    count = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            return count;
        }

        /// <summary>
        /// 获取所有文件信息
        /// </summary>
        /// <returns></returns>
        internal ObservableCollection<NnFile> GetFiles()
        {
            ObservableCollection<NnFile> list = new ObservableCollection<NnFile>();
            try
            {
                using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM fileinfo", mConnection))
                {
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            NnFile a = new NnFile();
                            a.InitNnFileByDb(reader);
                            list.Add(a);
                        }
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            return list;
        }



        // -------------------工具----------------
        public static bool GetBoolFromDb(OleDbDataReader reader, string key)
        {
            int o = reader.GetOrdinal(key);
            if (!reader.IsDBNull(o))
                return reader.GetBoolean(o);
            return false;
        }

        public static long GetLongFromDb(OleDbDataReader reader, string key)
        {
            int o = reader.GetOrdinal(key);
            if (!reader.IsDBNull(o))
                return reader.GetInt32(o);
            return 0;
        }

        public static DateTime GetDateTimeFromDb(OleDbDataReader reader, string key)
        {
            int o = reader.GetOrdinal(key);
            if (!reader.IsDBNull(o))
                return (DateTime)reader[key];
            return DateTime.MinValue;
        }

        public static string GetStringFromDb(OleDbDataReader reader, string key)
        {
            int o = reader.GetOrdinal(key);
            if (!reader.IsDBNull(o))
                return reader.GetString(o);
            return "";
        }

        public static int GetIntFromDb(OleDbDataReader reader, string key)
        {
            int o = reader.GetOrdinal(key);
            if (!reader.IsDBNull(o))
                return reader.GetInt32(o);
            return 0;
        }

        public static double GetDoubleFromDb(OleDbDataReader reader, string key)
        {
            int o = reader.GetOrdinal(key);
            if (!reader.IsDBNull(o))
                return reader.GetDouble(o);
            return 0;
        }
    }
}
