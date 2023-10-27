using System;
using System.Data.Common;
using Basic.Configuration;
using Basic.DataAccess;
using Npgsql;

namespace Basic.PostgreSql
{
	/// <summary>
	/// Npg数据库连接工厂。
	/// </summary>
	internal sealed class NpgConnectionFactory : ConnectionFactory
	{
		/// <summary>
		/// 初始化 NpgConnectionFactory 类实例。
		/// </summary>
		public NpgConnectionFactory() { }

		/// <summary>
		/// 获取与此 ConnectionConfig 关联的连接字符串。
		/// </summary>
		/// <param name="element">数据库连接配置信息</param>
		/// <returns>返回与此 ConnectionConfig 关联的连接字符串。</returns>
		public string CreateConnectionString(ConnectionElement element)
		{
			NpgsqlConnectionStringBuilder conStringBuilder = new NpgsqlConnectionStringBuilder();
			conStringBuilder.IntegratedSecurity = false;
			foreach (ConnectionItem item in element.Values)
			{
				if (!string.IsNullOrEmpty(item.Name) && !string.IsNullOrEmpty(item.Value))
				{
					if (item.Name == "DataSource" || item.Name == "Data Source")
						conStringBuilder.Database = item.Value;
					else if (item.Name == "UserID" || item.Name == "User ID")
						conStringBuilder.Username = item.Value;
					else if (item.Name == "Password")
						conStringBuilder.Password = ConfigurationAlgorithm.Decryption(item.Value);
					else
						conStringBuilder[item.Name] = item.Value;
				}

			}
			return conStringBuilder.ConnectionString;
		}

		/// <summary>
		/// 返回实现 DbConnection 类的提供程序的类的一个新实例。
		/// </summary>
		/// <param name="info">数据库连接信息</param>
		/// <returns>DbConnection 的新实例。</returns>
		public override DbConnection CreateConnection(ConnectionInfo info) { return new NpgsqlConnection(info.ConnectionString); }

		/// <summary>
		/// 返回实现 DbConnection 类的提供程序的类的一个新实例。
		/// </summary>
		/// <param name="connectionString">数据库连接字符串</param>
		/// <returns>DbConnection 的新实例。</returns>
		public override DbConnection CreateConnection(string connectionString) { return new NpgsqlConnection(connectionString); }

		/// <summary>
		/// 创建动态命令
		/// </summary>
		/// <returns>命令配置文件缓存</returns>
		public override DynamicCommand CreateDynamicCommand() { return new NpgDynamicCommand(); }

		/// <summary>
		/// 创建静态命令
		/// </summary>
		/// <returns>命令配置文件缓存</returns>
		public override StaticCommand CreateStaticCommand() { return new NpgStaticCommand(); }

		/// <summary>
		/// 创建 BatchCommand 类实例。
		/// </summary>
		/// <param name="connection">将用于执行批量复制操作的已经打开的 DbConnection 实例。</param>
		/// <param name="tableInfo">表示当前表结构信息</param>
		/// <returns>返回 BatchCommand 类对应数据库类型的实例。</returns>
		public override BatchCommand CreateBatchCommand(DbConnection connection, TableConfiguration tableInfo)
		{
			return new NpgBatchCommand(connection as NpgsqlConnection, tableInfo);
		}

		/// <summary>
		/// 创建命令执行参数名称
		/// </summary>
		/// <param name="noSymbolName">不带符号的参数名</param>
		/// <returns>返回创建成功的带符号的参数名称</returns>
		public override string CreateParameterName(string noSymbolName)
		{
			if (string.IsNullOrEmpty(noSymbolName)) { throw new ArgumentNullException("noSymbolName"); }
			if (noSymbolName.StartsWith("@")) { return noSymbolName; }
			return string.Concat("@", noSymbolName);
		}
	}
}
