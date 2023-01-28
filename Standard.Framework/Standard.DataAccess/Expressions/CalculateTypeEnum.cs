using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Expressions
{
	/// <summary>
	/// 表达式参数比较类型
	/// </summary>
	public enum CalculateTypeEnum
	{
		/// <summary>
		/// 加法运算，如 a + b，。
		/// </summary>
		Add = 1,
		/// <summary>
		/// 减法运算，如 (a - b)。
		/// </summary>
		Subtract,
		/// <summary>
		/// 乘法运算，如 (a * b)。
		/// </summary>
		Multiply,
		/// <summary>
		///  除法运算，如 (a / b)，针对数值操作数。
		/// </summary>
		Divide,
		/// <summary>
		/// 算术余数运算，如 C# 中的 (a % b) 或 Visual Basic 中的 (a Mod b)。
		/// </summary>
		Modulo,

		/// <summary>
		/// 按位或逻辑 AND 运算，如 C# 中的 (a &amp; b) 和 Visual Basic 中的 (a And b)。
		/// </summary>
		And,
		/// <summary>
		/// 按位或逻辑 OR 运算，如 C# 中的 (a | b) 或 Visual Basic 中的 (a Or b)。
		/// </summary>
		Or,

		/// <summary>
		/// 按位或逻辑 XOR 运算，如 C# 中的 (a ^ b) 或 Visual Basic 中的 (a Xor b)。
		/// </summary>
		ExclusiveOr,
		/// <summary>
		/// 按位左移运算，此运算符在 Oracle 中无效，如 (a &lt;&lt; b)。
		/// </summary>
		LeftShift,
		/// <summary>
		/// 按位右移运算，此运算符在 Oracle 中无效，如 (a &gt;&gt; b)。
		/// </summary>
		RightShift,
	}
}
