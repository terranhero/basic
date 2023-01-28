
namespace Basic.Exceptions
{
	/// <summary>
	/// 方法，存储过程，Transact-SQL命令传入参数异常
	/// </summary>
	public sealed class ParameterException : Basic.Exceptions.CustomCodeException
	{
		/// <summary>
		/// 初始化ParameterException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		public ParameterException(string errorCode) : this(errorCode, null, null) { }

		/// <summary>
		/// 初始化ParameterException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		///  <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		public ParameterException(string errorCode, System.Exception innerException) : this(errorCode, innerException, null) { }

		/// <summary>
		/// 初始化ParameterException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public ParameterException(string errorCode, params object[] paramArray) : this(errorCode, null, paramArray) { }

		/// <summary>
		/// 初始化ParameterException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public ParameterException(string errorCode, System.Exception innerException, params object[] paramArray) :
			base(errorCode, innerException, paramArray) { }
	}
}
