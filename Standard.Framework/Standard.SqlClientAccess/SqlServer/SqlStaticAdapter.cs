﻿using Basic.DataAccess;
using Basic.Enums;
using Microsoft.Data.SqlClient;

namespace Basic.SqlServer
{
	/// <summary>
	/// 表示用于填充 DataSet 和更新 SQL SERVER 数据库的一组数据命令和一个数据库连接。 
	/// </summary>
	[System.Serializable(), System.ComponentModel.ToolboxItem(false)]
	internal sealed class SqlStaticAdapter : StaticDataAdapter
	{
		/// <summary>
		/// 初始化 SqlStaticAdapter 类实例
		/// </summary>
		public SqlStaticAdapter() : base(new SqlDataAdapter()) { }

		/// <summary>
		/// 初始化 SqlStaticAdapter 类实例
		/// </summary>
		/// <param name="dataAdapter">用于创建新 SqlStaticAdapter 的 SqlDataAdapter 对象。 </param>
		internal SqlStaticAdapter(SqlDataAdapter dataAdapter) : base(dataAdapter) { }

		/// <summary>当前命令的数据库类型</summary>
		public override ConnectionType ConnectionType { get { return ConnectionType.SqlConnection; } }
	}
}
