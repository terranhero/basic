using Basic.Configuration;
using Basic.Enums;

namespace Basic.DataContexts
{
	/// <summary>
	/// 数据上下文静态工厂类
	/// </summary>
	public static class DataContextFactory
	{
		/// <summary>
		/// 根据配置文件信息创建数据上下文信息。
		/// </summary>
		/// <param name="structCon">数据库连接配置信息</param>
		/// <returns>返回 实现IDataContext 接口的实例。</returns>
		public static IDataContext CreateDbAccess(ConnectionInfo structCon)
		{
			if (structCon.ConnectionType == ConnectionType.SqlConnection)
			{
				return new SqlServerDataContext(structCon.ConnectionString);
			}
			else if (structCon.ConnectionType == ConnectionType.OracleConnection)
			{
				return new OracleDataContext(structCon.ConnectionString);
			}
			else if (structCon.ConnectionType == ConnectionType.MySqlConnection)
			{
				return new MySqlDataContext(structCon.ConnectionString);
			}
			return new SqlServerDataContext(structCon.ConnectionString);
		}

		/// <summary>
		/// 根据配置文件信息创建数据上下文信息。
		/// </summary>
		/// <returns>返回 实现IDataContext 接口的实例。</returns>
		public static IDataContext CreateDbAccess()
		{
			return CreateDbAccess(ConnectionContext.DefaultConnection);
		}
	}
}
