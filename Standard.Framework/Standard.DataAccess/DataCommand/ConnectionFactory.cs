using System;
using System.Collections.Generic;
using System.Data.Common;
using Basic.Configuration;
using Basic.Interfaces;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示一组方法，这些方法用于创建提供程序对数据源类的实现的实例。
	/// </summary>
	public abstract class ConnectionFactory
	{
		/// <summary>数据库服务器地址字段常用名称</summary>
		internal protected static readonly ICollection<string> dataSourceKeys = new SortedSet<string>(new string[] {
			"DATASOURCE", "DATA SOURCE", "SERVER", "ADDRESS", "ADDR", "NETWORK ADDRESS" }, StringComparer.OrdinalIgnoreCase);
		/// <summary>数据库登录账号字段常用名称</summary>
		internal protected static readonly ICollection<string> userKeys = new SortedSet<string>(new string[] {
			"USERID", "USER ID", "USER", "UID" }, StringComparer.OrdinalIgnoreCase);
		/// <summary>数据库账号密码字段常用名称</summary>
		internal protected static readonly ICollection<string> pwdKeys = new SortedSet<string>(new string[] {
			"PASSWORD", "PWD" }, StringComparer.OrdinalIgnoreCase);

		/// <summary>根据数据库连接信息，构建 ConnectionInfo 对象。</summary>
		/// <param name="info">数据库连接配置信息</param>
		/// <returns>返回构建完成的 ConnectionInfo 对象。</returns>
		public abstract ConnectionInfo CreateConnectionInfo(IConnectionInfo info);

		/// <summary>根据数据库连接信息，构建 ConnectionInfo 对象。</summary>
		/// <param name="element">数据库连接配置信息</param>
		/// <returns>返回构建完成的 ConnectionInfo 对象。</returns>
		internal abstract ConnectionInfo CreateConnectionInfo(ConnectionElement element);

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
		/// 创建 BulkCopyCommand 类实例。
		/// </summary>
		/// <param name="connection">将用于执行批量复制操作的已经打开的 DbConnection 实例。</param>
		/// <param name="tableInfo">表示当前表结构信息</param>
		/// <returns>返回 BulkCopyCommand 类对应数据库类型的实例。</returns>
		public abstract BulkCopyCommand CreateBulkCopyCommand(DbConnection connection, TableConfiguration tableInfo);

		/// <summary>
		/// 创建命令执行参数名称
		/// </summary>
		/// <param name="noSymbolName">不带符号的参数名</param>
		/// <returns>返回创建成功的带符号的参数名称</returns>
		public abstract string CreateParameterName(string noSymbolName);

	}
}
