using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Instance.Data
{
    public class ConnectionString
    {
        /// <summary>
        /// 数据库读写分离后，读库访问
        /// 如果没有设置，在默认取
        /// </summary>
        public static string Writing = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    }
}
