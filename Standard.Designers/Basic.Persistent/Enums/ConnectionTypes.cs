using Basic.Configuration;

namespace Basic.Enums
{
	/// <summary>
	/// 表示数据库连接
	/// </summary>
	public enum ConnectionTypes
	{
		/// <summary>
		/// 
		/// </summary>
		Default = 0,
		/// <summary>SQL Server 数据库</summary>
		SQLSERVER = 1,
		/// <summary>ORACLE 数据库</summary>
		ORACLE = 2,
		/// <summary>MYSQL 数据库</summary>
		MYSQL = 4,
		/// <summary>IBM DB2 数据库</summary>
		DB2 = 6,
		/// <summary>PostgreSQL 数据库</summary>
		PGSQL = 8,
		/// <summary>
		/// Sqlite 数据库
		/// </summary>
		SQLITE = 10

	}

	//   /// <summary>
	//   /// 表示数据库连接
	//   /// </summary>
	//   public enum ConnectionTypes
	//   {
	//       /// <summary>
	//       /// 
	//       /// </summary>
	//       Default = 0,

	//       /// <summary>
	//	/// SQL Server 数据库
	//	/// </summary>
	//       SQLSERVER = 1,

	//       /// <summary>
	//	/// ORACLE 数据库
	//	/// </summary>
	//       ORACLE = 2,

	//       /// <summary>
	//	/// MYSQL 数据库
	//	/// </summary>
	//       MYSQL = 4,

	//       /// <summary>
	//	/// IBM DB2 数据库
	//	/// </summary>
	//       DB2 = 6,

	//       /// <summary>
	//	/// PostgreSQL 数据库
	//	/// </summary>
	//       PGSQL = 8,

	//	/// <summary>
	//	/// Sqlite 数据库
	//	/// </summary>
	//	SQLITE = 10
	//}

	/// <summary>
	/// 读取配置文件中数据库连接类型对应的配置文件扩展名
	/// </summary>
	public static class ConnectionTypesExtension
	{
		//     /// <summary>
		//     /// 获取或设置与指定的键相关联的值。
		//     /// </summary>
		//     /// <param name="sqlConType">要获取或设置的值的键。</param>
		//     /// <exception cref="System.ArgumentNullException">sqlConType为null</exception>
		//     /// <returns> 与指定的键相关联的值。如果找不到指定的键，返回默认扩展名。</returns>
		//     public static string GetExtension(ConnectionTypeEnum sqlConType)
		//     {
		//         if (sqlConType == ConnectionTypeEnum.SQLSERVER)
		//             return ConfigurationExtension.GetExtension(ConnectionType.SqlConnection);
		//         else if (sqlConType == ConnectionTypeEnum.ORACLE)
		//             return ConfigurationExtension.GetExtension(ConnectionType.OracleConnection);
		//         else if (sqlConType == ConnectionTypeEnum.MYSQL)
		//             return ConfigurationExtension.GetExtension(ConnectionType.MySqlConnection);
		//         else if (sqlConType == ConnectionTypeEnum.NPGSQL)
		//             return ConfigurationExtension.GetExtension(ConnectionType.NpgSqlConnection);
		//         else if (sqlConType == ConnectionTypeEnum.DB2)
		//             return ConfigurationExtension.GetExtension(ConnectionType.Db2Connection);
		//else if (sqlConType == ConnectionTypeEnum.SQLITE)
		//	return ConfigurationExtension.GetExtension(ConnectionType.SqliteConnection);
		//return ConfigurationExtension.GetExtension(ConnectionType.SqlConnection);
		//     }

		/// <summary>
		/// 获取或设置与指定的键相关联的值。
		/// </summary>
		/// <param name="sqlConType">要获取或设置的值的键。</param>
		/// <exception cref="System.ArgumentNullException">sqlConType为null</exception>
		/// <returns> 与指定的键相关联的值。如果找不到指定的键，返回默认扩展名。</returns>
		public static string GetExtension(ConnectionTypes sqlConType)
		{
			if (sqlConType == ConnectionTypes.SQLSERVER)
				return ConfigurationExtension.GetExtension(ConnectionType.SqlConnection);
			else if (sqlConType == ConnectionTypes.ORACLE)
				return ConfigurationExtension.GetExtension(ConnectionType.OracleConnection);
			else if (sqlConType == ConnectionTypes.MYSQL)
				return ConfigurationExtension.GetExtension(ConnectionType.MySqlConnection);
			else if (sqlConType == ConnectionTypes.PGSQL)
				return ConfigurationExtension.GetExtension(ConnectionType.NpgSqlConnection);
			else if (sqlConType == ConnectionTypes.DB2)
				return ConfigurationExtension.GetExtension(ConnectionType.Db2Connection);
			else if (sqlConType == ConnectionTypes.SQLITE)
				return ConfigurationExtension.GetExtension(ConnectionType.SqliteConnection);
			return ConfigurationExtension.GetExtension(ConnectionType.SqlConnection);
		}
	}

}
