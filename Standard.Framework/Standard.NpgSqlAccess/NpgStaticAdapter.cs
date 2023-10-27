using System;

using Basic.DataAccess;
using Basic.Enums;
using Npgsql;

namespace Basic.PostgreSql
{
	/// <summary>
	/// 表示用于填充 DataSet 和更新 SQL SERVER 数据库的一组数据命令和一个数据库连接。 
	/// </summary>
	[System.Serializable(), System.ComponentModel.ToolboxItem(false)]
	internal sealed class NpgStaticAdapter : StaticDataAdapter
	{
		/// <summary>
		/// 初始化 NpgStaticAdapter 类实例
		/// </summary>
		public NpgStaticAdapter() : base(new NpgsqlDataAdapter()) { }

		/// <summary>
		/// 初始化 SqlStaticAdapter 类实例
		/// </summary>
		/// <param name="dataAdapter">用于创建新 SqlStaticAdapter 的 SqlDataAdapter 对象。 </param>
		internal NpgStaticAdapter(NpgsqlDataAdapter dataAdapter) : base(dataAdapter) { }

		/// <summary>当前命令的数据库类型</summary>
		public override ConnectionType ConnectionType { get { return ConnectionType.NpgSqlConnection; } }
	}
}
