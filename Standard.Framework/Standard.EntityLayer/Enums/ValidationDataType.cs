using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Enums
{
	/// <summary>
	/// 为比较验证特性提供类型支持
	/// </summary>
	public enum ValidationDataType
	{
		/// <summary>
		/// 表示数据值。
		/// </summary>
		Date,
		/// <summary>
		/// 表示时间值。
		/// </summary>
		TimeSpan,
		/// <summary>
		/// 表示某个具体时间，以日期和当天的时间表示。
		/// </summary>
		DateTime,
		/// <summary>
		/// 表示字符类型数据。
		/// </summary>
		String,
		/// <summary>
		/// 表示 16 位有符号的整数。
		/// </summary>
		Short,
		/// <summary>
		/// 表示 32 位有符号的整数。
		/// </summary>
		Integer,
		/// <summary>
		/// 表示 64 位有符号的整数。
		/// </summary>
		Long,
		/// <summary>
		/// 表示双精度浮点数
		/// </summary>
		Double,
		/// <summary>
		/// 表示十进制数。
		/// </summary>
		Decimal
	}
}
