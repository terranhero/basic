using System;
using Basic.Properties;

namespace Basic.Exceptions
{
	/// <summary>
	/// 常用异常实例列表
	/// </summary>
	internal static class ExceptionList
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal static NotSupportedException GetNotSupported(Type type, string mothed)
		{
			return new NotSupportedException(string.Format(Strings.NonSupport_Method, type.ToString(), mothed));
		}
	}
}
