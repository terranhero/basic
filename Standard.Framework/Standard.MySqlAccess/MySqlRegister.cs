using Basic.Enums;
using Basic.MySqlAccess;

namespace Basic.DataAccess
{
	/// <summary>
	/// 提供 MySql 数据库访问程序注册
	/// </summary>
	public static class MySqlRegister
	{
		/// <summary>
		/// 向基础数据库访问程序注册 MySql 数据库访问类
		/// </summary>
		public static void RegisterMySqlAccess()
		{
			ConnectionFactoryBuilder.RegisterConnectionFactory(ConnectionType.MySqlConnection, new MySqlConnectionFactory());
			//ConnectionFactoryBuilder.RegisterConnectionFactory(ConnectionType.MySqlConnection, 57, new MySqlConnectionFactory());
		}
	}
}
