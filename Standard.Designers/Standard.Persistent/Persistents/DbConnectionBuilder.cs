using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Configuration
{
    /// <summary>
    /// Provides a simple way to create and manage the contents of connection strings used by the DbConnection class.
    /// </summary>
    internal sealed class DbConnectionBuilder : Dictionary<string, string>
    {
        /// <summary>
        /// 初始化 DbConnectionBuilder 类实例。
        /// </summary>
        /// <param name="connectionString">表示需要解析的数据库连接字符串。</param>
        internal DbConnectionBuilder(string connectionString)
        {
            if (connectionString == null || connectionString == "") { return; }
            string[] itemArray = connectionString.Split(';');
            foreach (string item in itemArray)
            {
                if (item.IndexOf('=') >= 0)
                {
                    string[] subArray = item.Split('=');
                    base.Add(subArray[0], subArray[1]);
                }
            }
        }
    }
}
