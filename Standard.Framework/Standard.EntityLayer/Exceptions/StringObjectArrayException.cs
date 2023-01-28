
namespace Basic.Exceptions
{
	/// <summary>
	/// 参数StringObjectArray为null 或参数个数为零时引发的异常
	/// </summary>
	public sealed class StringObjectArrayException : Basic.Exceptions.CustomCodeException
	{
		/// <summary>
		/// 初始化StringObjectArrayException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		public StringObjectArrayException(string errorCode) : this(errorCode, null, null) { }

		/// <summary>
		/// 初始化StringObjectArrayException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		///  <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		public StringObjectArrayException(string errorCode, System.Exception innerException) : this(errorCode, innerException, null) { }

		/// <summary>
		/// 初始化StringObjectArrayException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public StringObjectArrayException(string errorCode, params object[] paramArray) : this(errorCode, null, paramArray) { }

		/// <summary>
		/// 初始化StringObjectArrayException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public StringObjectArrayException(string errorCode, System.Exception innerException, params object[] paramArray) :
			base(errorCode, innerException, paramArray) { }
	}
}
