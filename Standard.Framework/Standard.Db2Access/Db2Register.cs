using Basic.DataAccess;
using Basic.Enums;

namespace Basic.DB2Access
{
	/// <summary>
	/// 提供 DB2 数据库访问程序注册
	/// </summary>
	public static class Db2Register
	{
		/// <summary>向基础数据库访问程序注册 SqlServer 数据库访问类</summary>
		public static void RegisterAccess()
		{
			ConnectionFactoryBuilder.RegisterConnectionFactory(ConnectionType.Db2Connection, new DB2ConnectionFactory());
		}
	}
}
