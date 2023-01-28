
namespace Basic.Exceptions
{
	/// <summary>
	/// 数据库参数类型错误引发的错误
	/// </summary>
	public class ParameterDbTypeException : Basic.Exceptions.RuntimeException
	{
		/// <summary>
		/// 初始化ParameterDbTypeException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		public ParameterDbTypeException(string errorCode) : this(errorCode, null, null) { }

		/// <summary>
		/// 初始化ParameterDbTypeException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		///  <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		public ParameterDbTypeException(string errorCode, System.Exception innerException) : this(errorCode, innerException, null) { }

		/// <summary>
		/// 初始化ParameterDbTypeException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public ParameterDbTypeException(string errorCode, params object[] paramArray) : this(errorCode, null, paramArray) { }

		/// <summary>
		/// 初始化ParameterDbTypeException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public ParameterDbTypeException(string errorCode, System.Exception innerException, params object[] paramArray) :
			base(errorCode, innerException, paramArray) { }
	}

	/// <summary>
	/// 数据库参数值错误引发的异常
	/// </summary>
	public class ParameterValueException : Basic.Exceptions.CustomCodeException
	{
		/// <summary>
		/// 初始化ParameterValueException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		public ParameterValueException(string errorCode) : this(errorCode, null, null) { }

		/// <summary>
		/// 初始化ParameterValueException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		///  <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		public ParameterValueException(string errorCode, System.Exception innerException) : this(errorCode, innerException, null) { }

		/// <summary>
		/// 初始化ParameterValueException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public ParameterValueException(string errorCode, params object[] paramArray) : this(errorCode, null, paramArray) { }

		/// <summary>
		/// 初始化ParameterValueException类实例
		/// </summary>
		/// <param name="errorCode">异常消息的编码</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public ParameterValueException(string errorCode, System.Exception innerException, params object[] paramArray) :
			base(errorCode, innerException, paramArray) { }
	}
}
