using System.Collections.Generic;
using System.Configuration;
using Basic.Enums;

namespace Basic.Configuration
{
	/// <summary>
	/// 读取配置文件中数据库连接类型对应的配置文件扩展名
	/// </summary>
	public static class ConfigurationExtension
	{
		/// <summary>
		/// 配置文件默认扩展名称
		/// </summary>
		public static string DefaultExtension { get; private set; }

		private static SortedDictionary<ConnectionType, string> list = new SortedDictionary<ConnectionType, string>();

		static ConfigurationExtension()
		{
			list[ConnectionType.SqlConnection] = "sqlf";
			list[ConnectionType.OracleConnection] = "oraf";
			//list[ConnectionType.OleDbConnection] = "olef";
			//list[ConnectionType.OdbcConnection] = "odbcf";
			list[ConnectionType.SQLiteConnection] = "litf";
			list[ConnectionType.Db2Connection] = "dbf";
			list[ConnectionType.MySqlConnection] = "myf";
			list[ConnectionType.NpgSqlConnection] = "pgf";
			DefaultExtension = "sqlf";
		}

		/// <summary>
		/// 获取或设置与指定的键相关联的值。
		/// </summary>
		/// <param name="sqlConType">要获取或设置的值的键。</param>
		/// <exception cref="System.ArgumentNullException">sqlConType为null</exception>
		/// <returns> 与指定的键相关联的值。如果找不到指定的键，返回默认扩展名。</returns>
		public static string GetExtension(ConnectionType sqlConType)
		{
			if (list.ContainsKey(sqlConType))
				return list[sqlConType];
			return DefaultExtension;
		}
	}
}
