using System.Collections.Generic;
using System.Data.Common;
using Basic.Configuration;
using Basic.Enums;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示一组方法，这些方法用于创建提供程序对数据源类的实现的实例。
	/// </summary>
	public abstract class ConnectionFactory
	{
		/// <summary>
		/// 返回实现 DbConnection 类的提供程序的类的一个新实例。
		/// </summary>
		/// <param name="info">数据库连接信息</param>
		/// <returns>DbConnection 的新实例。</returns>
		public abstract DbConnection CreateConnection(ConnectionInfo info);

		/// <summary>
		/// 返回实现 DbConnection 类的提供程序的类的一个新实例。
		/// </summary>
		/// <param name="connectionString">数据库连接字符串</param>
		/// <returns>DbConnection 的新实例。</returns>
		public abstract DbConnection CreateConnection(string connectionString);

		/// <summary>
		/// 创建 DynamicCommand 类实例。
		/// </summary>
		/// <returns>返回 DynamicCommand 类对应数据库类型的实例。</returns>
		public abstract DynamicCommand CreateDynamicCommand();

		/// <summary>
		/// 创建 StaticCommand 类实例。
		/// </summary>
		/// <returns>返回 StaticCommand 类对应数据库类型的实例。</returns>
		public abstract StaticCommand CreateStaticCommand();

		/// <summary>
		/// 创建 BatchCommand 类实例。
		/// </summary>
		/// <param name="connection">将用于执行批量复制操作的已经打开的 DbConnection 实例。</param>
		/// <param name="tableInfo">表示当前表结构信息</param>
		/// <returns>返回 BatchCommand 类对应数据库类型的实例。</returns>
		public abstract BatchCommand CreateBatchCommand(DbConnection connection, TableConfiguration tableInfo);

		/// <summary>
		/// 创建命令执行参数名称
		/// </summary>
		/// <param name="noSymbolName">不带符号的参数名</param>
		/// <returns>返回创建成功的带符号的参数名称</returns>
		public abstract string CreateParameterName(string noSymbolName);

	}
}
