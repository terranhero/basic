using System;
using System.Xml;
using System.Text;
using System.Runtime.Serialization;
using System.Globalization;

namespace GoldSoft.Basic.Exceptions
{
	/// <summary>
	/// 自定义异常的基类,运行时报错，根据运行时语言提醒错误消息
	/// </summary>
	[Serializable()]
	public class RuntimeException : ApplicationException
	{
		private string _Message = string.Empty;
		/// <summary>
		/// 获取异常信息的错误编码
		/// </summary>
		public virtual string ErrorCode { get; private set; }

		/// <summary>
		/// 获取描述当前异常的消息。
		/// </summary>
		public new string Message
		{
			get { return _Message; }
			set { _Message = value; }
		}

		/// <summary>
		/// 使用指定的异常编码初始化 GoldSoftException 类的新实例。 
		/// </summary>
		/// <param name="errorCode">特定异常错误信息的资源名</param>
		public RuntimeException(string errorCode) : this(errorCode, null, null) { }

		/// <summary>
		/// 使用指定的异常编码初始化 GoldSoftException 类的新实例。 
		/// </summary>
		/// <param name="errorCode">特定异常错误信息的资源名</param>
		/// <param name="paramArray">格式化资源的参数数组</param>
		public RuntimeException(string errorCode, params object[] paramArray) : this(errorCode, null, paramArray) { }


		/// <summary>
		/// 使用指定的异常编码和错误信息,对作为此异常原因的内部异常的引用初始化 RuntimeException 类的新实例。 
		/// </summary>
		/// <param name="errorCode">特定异常错误信息的多语言编码</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		/// <param name="paramArray">填充错误消息的参数数组</param>
		private RuntimeException(string errorCode, Exception innerException, params object[] paramArray)
			: base(null, innerException)
		{
			ErrorCode = errorCode;
		}

		/// <summary>
		/// 用序列化数据初始化 GoldSoftException 类的新实例。 
		/// </summary>
		/// <param name="info">它存有有关所引发异常的序列化的对象数据。</param>
		/// <param name="context">它包含有关源或目标的上下文信息。 </param>
		protected RuntimeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			ErrorCode = info.GetString("ErrorCode");
			_Message = info.GetString("CustomMessage");
		}

		/// <summary>
		/// 使用将 GoldSoftException 对象序列化所需的数据填充 SerializationInfo。
		/// </summary>
		/// <param name="info">要填充数据的 SerializationInfo。</param>
		/// <param name="context">此序列化的目标（请参见 StreamingContext）。</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ErrorCode", ErrorCode);
			info.AddValue("CustomMessage", _Message);
		}
	}
}
