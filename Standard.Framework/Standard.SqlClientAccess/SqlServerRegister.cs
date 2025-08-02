using Basic.Enums;

namespace Basic.DataAccess
{
	/// <summary>
	/// 提供 SqlServer 数据库访问程序注册
	/// </summary>
	public static class SqlServerRegister
	{
		/// <summary>
		/// 向基础数据库访问程序注册 SqlServer 数据库访问类
		/// </summary>
		public static void RegisterSqlServerAccess()
		{
			ConnectionFactoryBuilder.RegisterConnectionFactory(ConnectionType.SqlConnection, new SqlServer.SqlConnectionFactory());
		}

		/// <summary>
		/// 向基础数据库访问程序注册 SqlServer 数据库访问类
		/// </summary>
		public static void RegisterSqlServer2012Access()
		{
			ConnectionFactoryBuilder.RegisterConnectionFactory(ConnectionType.NewSqlConnection, new SqlServer2012.SqlConnectionFactory());
		}
	}
}
