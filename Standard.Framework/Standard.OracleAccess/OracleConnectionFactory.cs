using System;
using System.Collections.Generic;
using System.Data.Common;
using Basic.Configuration;
using Basic.DataAccess;
using Oracle.ManagedDataAccess.Client;

namespace Basic.OracleAccess
{
	/// <summary>
	/// Oracle 数据库连接工厂。
	/// </summary>
	internal sealed class OracleConnectionFactory : ConnectionFactory
	{
		/// <summary>
		/// 初始化 OracleConnectionFactory 类实例。
		/// </summary>
		public OracleConnectionFactory() { }

		/// <summary>根据数据库连接信息，构建 ConnectionInfo 对象。</summary>
		/// <param name="info">数据库连接配置信息</param>
		/// <returns>返回构建完成的 ConnectionInfo 对象。</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:简化对象初始化", Justification = "<挂起>")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0090:使用 \"new(...)\"", Justification = "<挂起>")]
		public override ConnectionInfo CreateConnectionInfo(Interfaces.IConnectionInfo info)
		{
			OracleConnectionStringBuilder display = new OracleConnectionStringBuilder();
			OracleConnectionStringBuilder connection = new OracleConnectionStringBuilder();
			foreach (KeyValuePair<string, string> item in info)
			{
				if (string.IsNullOrEmpty(item.Key)) { continue; }
				else if (string.IsNullOrEmpty(item.Value)) { continue; }
				string upperName = item.Key.ToUpper();

				if (dataSourceKeys.Contains(upperName)) { connection.DataSource = display.DataSource = item.Value; }
				else if (userKeys.Contains(upperName)) { display.UserID = connection.UserID = item.Value; }
				else if (pwdKeys.Contains(upperName))
				{
					connection.Password = ConfigurationAlgorithm.Decryption(item.Value);
				}
				else { connection[item.Key] = item.Value; }
			}
			return new ConnectionInfo(info.Name, info.ConnectionType,
				connection.ConnectionString, display.ConnectionString);
		}

		/// <summary>根据数据库连接信息，构建 ConnectionInfo 对象。</summary>
		/// <param name="element">数据库连接配置信息</param>
		/// <returns>返回构建完成的 ConnectionInfo 对象。</returns>
		internal override ConnectionInfo CreateConnectionInfo(ConnectionElement element)
		{
			OracleConnectionStringBuilder display = new OracleConnectionStringBuilder();
			OracleConnectionStringBuilder connection = new OracleConnectionStringBuilder();
			foreach (ConnectionItem item in element.Values)
			{
				if (string.IsNullOrEmpty(item.Name)) { continue; }
				else if (string.IsNullOrEmpty(item.Value)) { continue; }

				if (dataSourceKeys.Contains(item.Name)) { connection.DataSource = display.DataSource = item.Value; }
				else if (userKeys.Contains(item.Name)) { display.UserID = connection.UserID = item.Value; }
				else if (pwdKeys.Contains(item.Name))
				{
					connection.Password = ConfigurationAlgorithm.Decryption(item.Value);
				}
				else { connection[item.Name] = item.Value; }
			}
			return new ConnectionInfo(element.Name, element.ConnectionType,
				connection.ConnectionString, display.ConnectionString);
		}

		/// <summary>
		/// 返回实现 DbConnection 类的提供程序的类的一个新实例。
		/// </summary>
		/// <param name="info">数据库连接信息</param>
		/// <returns>DbConnection 的新实例。</returns>
		public override DbConnection CreateConnection(ConnectionInfo info) { return new OracleConnection(info.ConnectionString); }

		/// <summary>
		/// 返回实现 DbConnection 类的提供程序的类的一个新实例。
		/// </summary>
		/// <param name="connectionString">数据库连接字符串</param>
		/// <returns>DbConnection 的新实例。</returns>
		public override DbConnection CreateConnection(string connectionString) { return new OracleConnection(connectionString); }

		/// <summary>
		/// 创建动态命令
		/// </summary>
		/// <returns>命令配置文件缓存</returns>
		public override DynamicCommand CreateDynamicCommand() { return new OracleDynamicCommand(); }

		/// <summary>
		/// 创建静态命令
		/// </summary>
		/// <returns>命令配置文件缓存</returns>
		public override StaticCommand CreateStaticCommand() { return new OracleStaticCommand(); }

		/// <summary>
		/// 创建 BatchCommand 类实例。
		/// </summary>
		/// <param name="connection">将用于执行批量复制操作的已经打开的 DbConnection 实例。</param>
		/// <param name="tableInfo">表示当前表结构信息</param>
		/// <returns>返回 BatchCommand 类对应数据库类型的实例。</returns>
		public override BatchCommand CreateBatchCommand(DbConnection connection, TableConfiguration tableInfo)
		{
			return new OracleBatchCommand(connection as OracleConnection, tableInfo);
		}

		/// <summary>
		/// 创建命令执行参数名称
		/// </summary>
		/// <param name="noSymbolName">不带符号的参数名</param>
		/// <returns>返回创建成功的带符号的参数名称</returns>
		public override string CreateParameterName(string noSymbolName)
		{
			if (string.IsNullOrEmpty(noSymbolName)) { throw new ArgumentNullException("noSymbolName"); }
			if (noSymbolName.StartsWith(":")) { return noSymbolName; }
			return string.Concat(":", noSymbolName);
		}
	}
}
