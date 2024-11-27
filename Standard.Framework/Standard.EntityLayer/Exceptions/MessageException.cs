using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Basic.Messages;

namespace Basic.Exceptions
{
	/// <summary>
	/// 自定义异常消息
	/// </summary>
	public sealed class MessageException : System.Exception
	{
		private readonly System.Globalization.CultureInfo _CultureInfo = new CultureInfo(2052);

		/// <summary>初始化 MessageException 类实例</summary>
		/// <param name="code">异常消息的编码</param>
		public MessageException(string code)
			: base(MessageContext.GetString(code)) { }

		/// <summary>初始化 MessageException 类实例</summary>
		/// <param name="code">异常消息的编码</param>
		///  <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		public MessageException(string code, Exception innerException)
			: base(MessageContext.GetString(code), innerException) { }

		/// <summary>初始化 MessageException 类实例</summary>
		/// <param name="code">异常消息的编码</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public MessageException(string code, params object[] paramArray)
			: base(MessageContext.GetString(code, paramArray)) { }

		/// <summary>初始化 MessageException 类实例</summary>
		/// <param name="code">异常消息的编码</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public MessageException(string code, Exception innerException, params object[] paramArray)
			: base(MessageContext.GetString(code, paramArray), innerException) { }

		/// <summary>初始化 MessageException 类实例</summary>
		/// <param name="code">异常消息的编码</param>
		/// <param name="culture">System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性</param>
		public MessageException(string code, CultureInfo culture) : this(code, culture, null, null) { }

		/// <summary>初始化 MessageException 类实例</summary>
		/// <param name="code">异常消息的编码</param>
		/// <param name="culture">System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性</param>
		///  <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		public MessageException(string code, CultureInfo culture, Exception innerException) : this(code, culture, innerException, null) { }

		/// <summary>初始化 MessageException 类实例</summary>
		/// <param name="code">异常消息的编码</param>
		/// <param name="culture">System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public MessageException(string code, CultureInfo culture, params object[] paramArray) : this(code, culture, null, paramArray) { }

		/// <summary>初始化 MessageException 类实例</summary>
		/// <param name="code">异常消息的编码</param>
		/// <param name="culture">System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个空引用。</param>
		/// <param name="paramArray">自定义显示错误消息数组</param>
		public MessageException(string code, CultureInfo culture, Exception innerException, params object[] paramArray)
			: base(MessageContext.GetString(code, culture, paramArray), innerException) { _CultureInfo = culture; Code = code; }
#if NET6_0 || NETSTANDARD2_0

		/// <summary>用序列化数据初始化 MessageException 类的新实例。 </summary>
		/// <param name="info">它存有有关所引发异常的序列化的对象数据。</param>
		/// <param name="context">它包含有关源或目标的上下文信息。 </param>
		private MessageException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Code = info.GetString("ErrorCode");
			_CultureInfo = new CultureInfo(info.GetInt32("CultureInfo"));
		}

		/// <summary>使用将 MessageException 对象序列化所需的数据填充 SerializationInfo。</summary>
		/// <param name="info">要填充数据的 SerializationInfo。</param>
		/// <param name="context">此序列化的目标（请参见 StreamingContext）。</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ErrorCode", Code);
			info.AddValue("CultureInfo", _CultureInfo.LCID);
		}
#endif
		/// <summary>获取异常信息的错误编码</summary>
		public string Code { get; private set; }
	}
}
