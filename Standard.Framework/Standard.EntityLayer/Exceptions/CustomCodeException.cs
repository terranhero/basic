using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Globalization;
using Basic.Properties;

namespace Basic.Exceptions
{
	/// <summary>
	/// 软件框架开发异常类基类
	/// </summary>
	[Serializable()]
	public abstract class CustomCodeException : ApplicationException
	{
		private string _Message = string.Empty;
		/// <summary>
		/// 获取异常信息的错误编码
		/// </summary>
		public virtual string ErrorCode { get; private set; }

		/// <summary>
		/// 获取描述当前异常的消息。
		/// </summary>
		public override string Message
		{
			get { return _Message; }
		}

		/// <summary>
		/// 将错误编码转换成错误消息
		/// </summary>
		/// <param name="errorCode">错误编码</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		protected virtual string GetString(string errorCode, params object[] paramArray)
		{
			string msg = Strings.ResourceManager.GetString(errorCode, CultureInfo.CurrentUICulture);
			if (msg == null)
				return errorCode;
			return string.Format(msg, paramArray);
		}

		/// <summary>
		/// 使用指定的异常编码和错误信息,对作为此异常原因的内部异常的引用初始化 GoldSoftException 类的新实例。 
		/// </summary>
		/// <param name="errorCode">特定异常错误信息的多语言编码</param>
		protected CustomCodeException(string errorCode) : this(errorCode, null, null) { }

		/// <summary>
		/// 使用指定的异常编码和错误信息,对作为此异常原因的内部异常的引用初始化 GoldSoftException 类的新实例。 
		/// </summary>
		/// <param name="errorCode">特定异常错误信息的多语言编码</param>
		/// <param name="paramArray">填充错误消息的参数数组</param>
		protected CustomCodeException(string errorCode, params object[] paramArray) : this(errorCode, null, paramArray) { }

		/// <summary>
		/// 使用指定的异常编码和错误信息,对作为此异常原因的内部异常的引用初始化 GoldSoftException 类的新实例。 
		/// </summary>
		/// <param name="errorCode">特定异常错误信息的多语言编码</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		/// <param name="paramArray">填充错误消息的参数数组</param>
		protected CustomCodeException(string errorCode, Exception innerException, params object[] paramArray)
			: base(errorCode, innerException)
		{
			_Message = GetString(errorCode, paramArray);
		}

		/// <summary>
		/// 用序列化数据初始化 GoldSoftException 类的新实例。 
		/// </summary>
		/// <param name="info">它存有有关所引发异常的序列化的对象数据。</param>
		/// <param name="context">它包含有关源或目标的上下文信息。 </param>
		protected CustomCodeException(SerializationInfo info, StreamingContext context)
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
