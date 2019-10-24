using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookApplication
{
    /// <summary>
    /// 账号
    /// </summary>
    class Account
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }
        private string name;
        /// <summary>
        /// Name用户名
        /// </summary>
        public string Name { get => name ?? ""; set => name = value; }
        private string bduss;
        /// <summary>
        /// 登录Cookie之一
        /// </summary>
        public string BDUSS { get => bduss ?? ""; set => bduss = value; }
        private string stoken;
        /// <summary>
        /// 登录Cookie之一
        /// </summary>
        public string STOKEN { get => stoken ?? ""; set => stoken = value; }
        private string bdstoken;
        public string BDStoken { get => bdstoken ?? ""; set => bdstoken = value; }
        private string logid;
        public string LogId { get => logid ?? ""; set => logid = value; }

        internal void InitAccountByDb(OleDbDataReader reader)
        {
            Id = NnReader.GetLongFromDb(reader, "ID");
            Name = NnReader.GetStringFromDb(reader, "Name");
            BDUSS = NnReader.GetStringFromDb(reader, "BDUSS");
            STOKEN = NnReader.GetStringFromDb(reader, "STOKEN");
            BDStoken = NnReader.GetStringFromDb(reader, "BDStoken");
            LogId = NnReader.GetStringFromDb(reader, "LogId");
        }

        internal object[] GetObjects()
        {
            return new object[] { Name, BDUSS, STOKEN, BDStoken, LogId };
        }
    }
}
