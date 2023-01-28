using System.Globalization;
using System;

namespace GoldSoft.Basic.Exceptions
{
	/// <summary>
	/// 自定义异常消息
	/// </summary>
	public sealed class MessageException : GoldSoft.Basic.Exceptions.RuntimeException
	{
		/// <summary>
		/// 初始化MessageException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		public MessageException(string errorCode) : this(errorCode, null, null) { }

		/// <summary>
		/// 初始化MessageException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		///  <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		public MessageException(string errorCode, Exception innerException) : this(errorCode, innerException, null) { }

		/// <summary>
		/// 初始化MessageException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public MessageException(string errorCode, params object[] paramArray) : this(errorCode, null, paramArray) { }

		/// <summary>
		/// 初始化MessageException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public MessageException(string errorCode, Exception innerException, params object[] paramArray) :
			base(errorCode, innerException, paramArray) { }
	}
}
