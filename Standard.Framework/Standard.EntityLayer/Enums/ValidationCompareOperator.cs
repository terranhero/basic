using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Enums
{
	/// <summary>
	/// 指定 WebCompareAttribute 使用的验证比较运算符。 
	/// </summary>
	public enum ValidationCompareOperator
	{
		/// <summary>
		/// 相等比较。
		/// </summary>
		Equal,
		/// <summary>
		/// 不等于比较。
		/// </summary>
		NotEqual,
		/// <summary>
		/// 大于比较。
		/// </summary>
		GreaterThan,
		/// <summary>
		/// 大于或等于比较。
		/// </summary>
		GreaterThanEqual,
		/// <summary>
		/// 小于比较。
		/// </summary>
		LessThan,
		/// <summary>
		/// 小于或等于比较。
		/// </summary>
		LessThanEqual
	}
}
