using Basic.DataAccess;
using Basic.Enums;

namespace Basic.OracleAccess
{
	/// <summary>
	/// 提供 SqlServer 数据库访问程序注册
	/// </summary>
	public static class OracleRegister
	{
		/// <summary>向基础数据库访问程序注册 SqlServer 数据库访问类</summary>
		public static void RegisterAccess()
		{
			ConnectionFactoryBuilder.RegisterConnectionFactory(ConnectionType.OracleConnection, new OracleConnectionFactory());
		}
	}
}
