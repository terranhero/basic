using System;

namespace Basic.EntityLayer
{
	/// <summary>表示数据库 JOIN 连接操作所需排序字段</summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public sealed class JoinOrderAttribute : Attribute
	{
		/// <summary>初始化 JoinOrderAttribute 类实例</summary>
		/// <param name="orderClauses">当前字段所属表名称或别名</param>
		public JoinOrderAttribute(params string[] orderClauses) { OrderClauses = orderClauses; }

		/// <summary>
		/// 表示where子句
		/// </summary>
		public string[] OrderClauses { get; private set; }
	}
}
