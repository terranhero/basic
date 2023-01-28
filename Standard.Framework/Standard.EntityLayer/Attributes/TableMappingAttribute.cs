using System;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 表示数据库表信息
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class TableMappingAttribute : Attribute
	{
		/// <summary>
		/// 初始化TableAttribute类实例
		/// </summary>
		/// <param name="tableName">数据库表名称</param>
		public TableMappingAttribute(string tableName) : this(tableName, null) { }

		/// <summary>
		/// 初始化TableAttribute类实例
		/// </summary>
		/// <param name="tableName">数据库表名称</param>
		/// <param name="aliasName">查询数据表别名</param>
		public TableMappingAttribute(string tableName, string aliasName)
		{
			TableName = tableName;
			AliasName = aliasName;
		}

		/// <summary>
		/// 数据库表名称
		/// </summary>
		public string TableName { get; private set; }

		/// <summary>
		/// 数据库视图名称
		/// </summary>
		public string AliasName { get; private set; }
	}
}
