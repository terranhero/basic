using Basic.Exceptions;
using System;
using System.Linq;

namespace Basic.Expressions
{
	/// <summary>Lambda 表达式扩展</summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
	public static class LambdaExtension
	{
		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中日期范围
		/// </summary>
		/// <param name="source">条件字段信息</param>
		/// <param name="fromValue">开始值</param>
		/// <param name="toValue">结束值</param>
		/// <returns></returns>
		public static bool Between(this DateTime source, DateTime fromValue, DateTime toValue)
		{
			return source >= fromValue && source <= toValue;
		}

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中日期范围
		/// </summary>
		/// <param name="source">条件字段信息</param>
		/// <param name="fromValue">开始值</param>
		/// <param name="toValue">结束值</param>
		/// <returns></returns>
		public static bool Between(this int source, int fromValue, int toValue)
		{
			return source >= fromValue && source <= toValue;
		}

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中日期范围
		/// </summary>
		/// <param name="source">条件字段信息</param>
		/// <param name="fromValue">开始值</param>
		/// <param name="toValue">结束值</param>
		/// <returns></returns>
		public static bool Between(this long source, long fromValue, long toValue)
		{
			return source >= fromValue && source <= toValue;
		}

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中日期范围
		/// </summary>
		/// <param name="source">条件字段信息</param>
		/// <param name="fromValue">开始值</param>
		/// <param name="toValue">结束值</param>
		/// <returns></returns>
		public static bool Between(this decimal source, decimal fromValue, decimal toValue)
		{
			return source >= fromValue && source <= toValue;
		}

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中日期范围
		/// </summary>
		/// <param name="source">条件字段信息</param>
		/// <param name="fromValue">开始值</param>
		/// <param name="toValue">结束值</param>
		/// <returns></returns>
		public static bool Between(this string source, string fromValue, string toValue)
		{
			if (fromValue == null) { return false; }
			if (toValue == null) { return false; }
			return source.CompareTo(fromValue) >= 0 && source.CompareTo(toValue) <= 0;
		}
		internal const string BetweenFunction = "Between";

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中判断是否为空
		/// </summary>
		/// <param name="source">条件字段信息</param>
		public static bool IsNull(this DateTime source)
		{
			throw ExceptionList.GetNotSupported(typeof(DateTime), "IsNull");
		}

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中判断是否为空
		/// </summary>
		/// <param name="source">条件字段信息</param>
		public static bool IsNull(this int source)
		{
			throw ExceptionList.GetNotSupported(typeof(int), "IsNull");
		}

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中判断是否为空
		/// </summary>
		/// <param name="source">条件字段信息</param>
		public static bool IsNull(this long source)
		{
			throw ExceptionList.GetNotSupported(typeof(long), "IsNull");
		}

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中判断是否为空
		/// </summary>
		/// <param name="source">条件字段信息</param>
		public static bool IsNull(this decimal source)
		{
			throw ExceptionList.GetNotSupported(typeof(decimal), "IsNull");
		}

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中判断是否为空
		/// </summary>
		/// <param name="source">条件字段信息</param>
		public static bool IsNull(this string source)
		{
			throw ExceptionList.GetNotSupported(typeof(string), "IsNull");
		}

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中判断是否为空
		/// </summary>
		/// <param name="source">条件字段信息</param>
		public static bool IsNull<TE>(this TE? source) where TE : struct
		{
			throw ExceptionList.GetNotSupported(typeof(TE), "IsNull");
		}

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中判断是否为空
		/// </summary>
		/// <param name="source">条件字段信息</param>
		public static bool IsNull<TE>(this TE source) where TE : struct
		{
			throw ExceptionList.GetNotSupported(typeof(TE), "IsNull");
		}
		internal const string IsNullFunction = "IsNull";

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中 IN 匹配
		/// </summary>
		/// <param name="preperty">需要做In操作的属性</param>
		/// <param name="values">In关键字包含的数组值</param>
		/// <returns></returns>
		public static bool In(this string preperty, string[] values)
		{
			return values.Contains(preperty);
		}
		internal const string InFunction = "In";

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中Not IN匹配
		/// </summary>
		/// <param name="preperty">需要做In操作的属性</param>
		/// <param name="values">In关键字包含的数组值</param>
		/// <returns></returns>
		public static bool NotIn(this string preperty, string[] values)
		{
			return values.Contains(preperty) == false;
		}
		internal const string NotInFunction = "NotIn";

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中 IN 匹配
		/// </summary>
		/// <param name="preperty">需要做In操作的属性</param>
		/// <param name="values">In关键字包含的数组值</param>
		/// <returns></returns>
		public static bool In(this int preperty, int[] values) { return values.Contains(preperty); }

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中 IN 匹配
		/// </summary>
		/// <typeparam name="TE">表示枚举类型</typeparam>
		/// <param name="preperty">需要做In操作的属性</param>
		/// <param name="values">In关键字包含的数组值</param>
		/// <returns></returns>
		public static bool In<TE>(this TE preperty, params TE[] values) where TE : struct { return values.Contains(preperty); }

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中 IN 匹配
		/// </summary>
		/// <param name="preperty">需要做In操作的属性</param>
		/// <param name="values">In关键字包含的数组值</param>
		/// <returns></returns>
		public static bool In(this short preperty, short[] values) { return values.Contains(preperty); }

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中Not IN匹配
		/// </summary>
		/// <param name="preperty">需要做In操作的属性</param>
		/// <param name="values">In关键字包含的数组值</param>
		/// <returns></returns>
		public static bool NotIn(this short preperty, short[] values) { return values.Contains(preperty) == false; }

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中Not IN匹配
		/// </summary>
		/// <param name="preperty">需要做In操作的属性</param>
		/// <param name="values">In关键字包含的数组值</param>
		/// <returns></returns>
		public static bool NotIn(this int preperty, int[] values) { return values.Contains(preperty) == false; }

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中 Not IN 匹配
		/// </summary>
		/// <typeparam name="TE">表示枚举类型</typeparam>
		/// <param name="preperty">需要做In操作的属性</param>
		/// <param name="values">In关键字包含的数组值</param>
		/// <returns></returns>
		public static bool NotIn<TE>(this TE preperty, TE[] values) where TE : struct
		{ return values.Contains(preperty) == false; }

		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中模糊匹配
		/// </summary>
		/// <param name="source">需要扩展的字符串实例</param>
		/// <param name="likePattern">模糊匹配模式</param>
		/// <returns>如果表达式成立则为true，否则为false。</returns>
		public static bool Like(this string source, string likePattern) { throw ExceptionList.GetNotSupported(typeof(string), "Like"); }
		internal const string LikeFunction = "Like";
		/// <summary>
		/// lambda表达式中使用，用于解析SQL语句中模糊匹配
		/// </summary>
		/// <param name="source">需要扩展的字符串实例</param>
		/// <param name="likePattern">模糊匹配模式</param>
		/// <returns>如果表达式成立则为true，否则为false。</returns>
		public static bool NotLike(this string source, string likePattern) { throw ExceptionList.GetNotSupported(typeof(string), "NotLike"); }
		internal const string NotLikeFunction = "NotLike";

		/// <summary>
		/// lambda表达式中使用，判断元字符串似否小于目标字符串。
		/// </summary>
		/// <param name="source">需要扩展的字符串实例</param>
		/// <param name="target">需要判断的目标字符串。</param>
		/// <returns>如果表达式成立则为true，否则为false。</returns>
		public static bool LessThan(this string source, string target) { throw ExceptionList.GetNotSupported(typeof(string), "LessThan"); }
		internal const string LessThanFunction = "LessThan";

		/// <summary>
		/// lambda表达式中使用，判断元字符串似否小于或等于目标字符串。
		/// </summary>
		/// <param name="source">需要扩展的字符串实例</param>
		/// <param name="target">需要判断的目标字符串。</param>
		/// <returns>如果表达式成立则为true，否则为false。</returns>
		public static bool LessThanOrEqual(this string source, string target) { throw ExceptionList.GetNotSupported(typeof(string), "LessThanOrEqual"); }
		internal const string LessThanOrEqualFunction = "LessThanOrEqual";

		/// <summary>
		/// lambda表达式中使用，判断元字符串似否大于目标字符串。
		/// </summary>
		/// <param name="source">需要扩展的字符串实例</param>
		/// <param name="target">需要判断的目标字符串。</param>
		/// <returns>如果表达式成立则为true，否则为false。</returns>
		public static bool GreaterThan(this string source, string target) { throw ExceptionList.GetNotSupported(typeof(string), "GreaterThan"); }
		internal const string GreaterThanFunction = "GreaterThan";

		/// <summary>
		/// lambda表达式中使用，判断元字符串似否大于或等于目标字符串。
		/// </summary>
		/// <param name="source">需要扩展的字符串实例</param>
		/// <param name="target">需要判断的目标字符串。</param>
		/// <returns>如果表达式成立则为true，否则为false。</returns>
		public static bool GreaterThanOrEqual(this string source, string target) { throw ExceptionList.GetNotSupported(typeof(string), "GreaterThanOrEqual"); }
		internal const string GreaterThanOrEqualFunction = "GreaterThanOrEqual";
	}
}
