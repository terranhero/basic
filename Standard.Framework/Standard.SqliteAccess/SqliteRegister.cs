using Basic.Enums;
using Basic.SqliteAccess;

namespace Basic.DataAccess
{
	/// <summary>
	/// 提供 Sqlite 数据库访问程序注册
	/// </summary>
	public static class SqliteRegister
	{
		/// <summary>
		/// 向基础数据库访问程序注册 SqlServer 数据库访问类
		/// </summary>
		public static void RegisterSqliteAccess()
		{
			ConnectionFactoryBuilder.RegisterConnectionFactory(ConnectionType.SqliteConnection, new SqliteConnectionFactory());
		}
	}
}
