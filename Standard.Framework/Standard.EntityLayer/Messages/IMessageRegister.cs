namespace Basic.Messages
{
	/// <summary>字符资源注册接口</summary>
	public interface IMessageRegister
	{
		/// <summary>注册资源转换器。</summary>
		/// <param name="messageConverter">文本消息转换器，该转换器为实现了接口 IMessageConverter 的类实例。</param>
		IMessageConverter Register(IMessageConverter messageConverter);

		/// <summary>注册资源转换器。</summary>
		/// <param name="messageConverter">文本消息转换器，该转换器为实现了接口 IMessageConverter 的类实例。</param>
		/// <param name="defaultConverter">当前消息转换器是否为默认消息转换器。</param>
		IMessageConverter Register(IMessageConverter messageConverter, bool defaultConverter);
	}
}
