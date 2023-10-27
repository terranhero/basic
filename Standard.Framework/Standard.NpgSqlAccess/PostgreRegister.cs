using Basic.DataAccess;
using Basic.Enums;

namespace Basic.PostgreSql
{
	/// <summary>
	/// 提供 PostgreSQL 数据库访问程序注册
	/// </summary>
	public static class PostgreRegister
	{
		/// <summary>
		/// 向基础数据库访问程序注册 Npg 数据库访问类
		/// </summary>
		public static void RegisterPostgreAccess()
		{
			ConnectionFactoryBuilder.RegisterConnectionFactory(ConnectionType.NpgSqlConnection, new NpgConnectionFactory());
		}
	}
}
