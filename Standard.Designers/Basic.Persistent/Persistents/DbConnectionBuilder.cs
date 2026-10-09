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
            if (string.IsNullOrEmpty(connectionString)) { return; }
            string[] itemArray = connectionString.Split(';');
            foreach (string item in itemArray)
            {
                if (string.IsNullOrEmpty(item)) { continue; }
                int splitIndex = item.IndexOf('=');
                if (splitIndex <= 0) { continue; }
                string key = item.Substring(0, splitIndex).Trim();
                if (string.IsNullOrEmpty(key)) { continue; }
                // 使用 Substring 而非 Split，避免连接字符串值中出现的 '='（例如加密后的密码）被截断。
                string value = item.Substring(splitIndex + 1).Trim();
                this[key] = value;      // 使用索引器赋值，兼容重复键（后者覆盖前者）。
            }
        }
    }
}
