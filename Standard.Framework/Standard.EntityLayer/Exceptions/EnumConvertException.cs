using System;

namespace Basic.Exceptions
{
	/// <summary>
	/// 枚举类型转换错误时引发的异常
	/// </summary>
	public sealed class EnumConvertException : CustomCodeException
	{
		/// <summary>
		/// 初始化EnumConvertException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		public EnumConvertException(string errorCode) : this(errorCode, null, null) { }

		/// <summary>
		/// 初始化EnumConvertException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		///  <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		public EnumConvertException(string errorCode, Exception innerException) : this(errorCode, innerException, null) { }

		/// <summary>
		/// 初始化EnumConvertException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public EnumConvertException(string errorCode, params object[] paramArray) : this(errorCode, null, paramArray) { }

		/// <summary>
		/// 初始化EnumConvertException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public EnumConvertException(string errorCode, Exception innerException, params object[] paramArray) :
			base(errorCode, innerException, paramArray) { }
	}
}
