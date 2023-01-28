using System;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 表示数据库表信息
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class TableAttribute : Attribute
	{
		/// <summary>
		/// 初始化TableAttribute类实例
		/// </summary>
		/// <param name="tableName">数据库表名称</param>
		public TableAttribute(string tableName) : this(tableName, tableName) { }

		/// <summary>
		/// 初始化TableAttribute类实例
		/// </summary>
		/// <param name="tableName">数据库表名称</param>
		/// <param name="viewName">数据库表对应的视图名称</param>
		public TableAttribute(string tableName, string viewName)
		{
			TableName = tableName;
			ViewName = viewName;
		}

		/// <summary>
		/// 数据库表名称
		/// </summary>
		public string TableName { get; private set; }

		/// <summary>
		/// 数据库视图名称
		/// </summary>
		public string ViewName { get; private set; }

		/// <summary>
		/// 配置文件名称
		/// </summary>
		public string ConfigFile { get; set; }
	}
}
