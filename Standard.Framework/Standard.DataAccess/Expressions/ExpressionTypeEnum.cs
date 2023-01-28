using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Expressions
{
	/// <summary>
	/// 表达式参数比较类型
	/// </summary>
	public enum ExpressionTypeEnum
	{
		/// <summary>
		/// 表示相等比较，如 a = b。
		/// </summary>
		Equal = 1,
		/// <summary>
		/// 表示不相等比较，如 a &lt;&gt; b。
		/// </summary>
		NotEqual,
		/// <summary>
		/// 表示“大于”比较，如 a &gt; b。
		/// </summary>
		GreaterThan,
		/// <summary>
		/// 表示“大于或等于”比较，如 a &gt;= b。
		/// </summary>
		GreaterThanEqual,
		/// <summary>
		/// 表示 “小于”比较，如 a &lt; b。
		/// </summary>
		LessThan,
		/// <summary>
		/// 表示“小于或等于”比较，如 a &lt;= b。
		/// </summary>
		LessThanEqual,
		/// <summary>
		/// 表示 “LIKE”匹配，如 a LIKE b。
		/// </summary>
		Like,
		/// <summary>
		/// 表示“NOT LIKE”匹配，如 a NOT LIKE b。
		/// </summary>
		NotLike,
		/// <summary>
		/// 表示 “IN”匹配，如 a IN(b)。
		/// </summary>
		In,
		/// <summary>
		/// 表示“NOT IN”匹配，如 a NOT IN(b)。
		/// </summary>
		NotIn,
		/// <summary>
		/// 表示“BETWEEN”匹配，如 a BETWEEN b AND c。
		/// </summary>
		Between,
		/// <summary>
		/// 表示“IS NULL”匹配，如 a Is Null。
		/// </summary>
		IsNull,
	}
}
