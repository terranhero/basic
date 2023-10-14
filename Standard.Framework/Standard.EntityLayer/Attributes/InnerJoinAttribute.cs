using System;

namespace Basic.EntityLayer
{
	/// <summary>表示数据库 JOIN 连接操作</summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public sealed class MainTableAttribute : Attribute
	{
		/// <summary>MainTableAttribute</summary>
		/// <param name="talbe">表示 JOIN 操作表名称</param>
		/// <param name="alias">表示 JOIN 操作表别名</param>
		public MainTableAttribute(string talbe, string alias)
		{
			JoinTable = talbe; JoinAlias = alias;
		}

		/// <summary>数据库表名称或表别名</summary>
		internal string JoinTable { get; private set; }

		/// <summary>数据库表名称或表别名</summary>
		internal string JoinAlias { get; private set; }
	}

	/// <summary>表示数据库 JOIN 连接操作</summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class InnerJoinAttribute : Attribute
	{
		/// <summary>初始化JoinAttribute类实例</summary>
		/// <param name="talbe">表示 JOIN 操作表名称</param>
		/// <param name="alias">表示 JOIN 操作表别名</param>
		/// <param name="condition">联接行进行求值的谓词</param>
		public InnerJoinAttribute(string talbe, string alias, string condition)
		{
			JoinTable = talbe; JoinAlias = alias; Condition = condition;
		}

		/// <summary>数据库表名称或表别名</summary>
		internal string JoinTable { get; private set; }

		/// <summary>数据库表名称或表别名</summary>
		internal string JoinAlias { get; private set; }

		/// <summary>数据库表名称或表别名</summary>
		internal string Condition { get; private set; }

		/// <summary>根据参数获取JOIN操作SQL语句</summary>
		/// <returns></returns>
		internal string JoinScript { get { return string.Concat(" JOIN ", JoinTable, " AS ", JoinAlias, " ON ", Condition); } }
	}
}
