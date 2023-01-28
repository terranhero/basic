using System;

namespace Basic.EntityLayer
{
	/// <summary>表示数据库 JOIN 连接操作</summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class JoinAttribute : Attribute
	{
		/// <summary>
		/// 初始化JoinAttribute类实例
		/// </summary>
		/// <param name="table">表示 JOIN 操作表名称</param>
		/// <param name="alias">表示 JOIN 操作表别名</param>
		/// <param name="field">表示 JOIN 操作表字段</param>
		/// <param name="table2">表示 JOIN 操作表名称</param>
		/// <param name="field2">表示 JOIN 操作表字段</param>
		public JoinAttribute(string table, string alias, string field, string table2, string field2)
		{
			JoinTable = table; JoinAlias = alias; JoinField = field;
			RightTable = table2; RightField = field2;
		}

		/// <summary>数据库表名称或表别名</summary>
		internal string JoinTable { get; private set; }

		/// <summary>数据库表名称或表别名</summary>
		internal string JoinAlias { get; private set; }

		/// <summary>数据库表名称或表别名</summary>
		internal string JoinField { get; private set; }

		/// <summary>数据库表名称或表别名</summary>
		internal string RightTable { get; private set; }

		/// <summary>数据库表名称或表别名</summary>
		internal string RightField { get; private set; }

		/// <summary>根据参数获取JOIN操作SQL语句</summary>
		/// <returns></returns>
		internal string JoinScript { get { return string.Concat(" JOIN ", JoinTable, " AS ", JoinAlias, " ON ", JoinAlias, ".", JoinField, "=", RightTable, ".", RightField); } }
	}
}
