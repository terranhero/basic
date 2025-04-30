using System;

namespace Basic.Messages
{
	/// <summary>多语言消息转换器</summary>
	public static class MessageContext
	{
		private static IMessageConverter defaultMesage;
		private static readonly MessageConverterCollection messagerConverters;
		private static readonly DefaultMessageContext _converter;
		/// <summary>初始化MessageConverter</summary>
		static MessageContext()
		{
			messagerConverters = new MessageConverterCollection(20);
			if (messagerConverters.ContainsKey("MessageStrings") == false)
			{
				defaultMesage = messagerConverters.Register(new MessageConverter(), true);
			}
			if (messagerConverters.ContainsKey("AccessStrings") == false)
			{
				/*判断当前资源中是否存在日志本地化资源*/
				string fullName = typeof(MessageContext).Assembly.FullName;
				string logType = fullName.Replace("Basic.EntityLayer", "Basic.Loggers.AccessStrings, Basic.DataAccess");
				Type type = Type.GetType(logType, false, false);
				if (type != null) { messagerConverters.Register(Activator.CreateInstance(type) as IMessageConverter); }
			}
			if (messagerConverters.ContainsKey("EasyStrings") == false)
			{
				/*判断当前资源中是否存在按钮本地化资源*/
				string fullName = typeof(MessageContext).Assembly.FullName;
				string easyType = fullName.Replace("Basic.EntityLayer", "Basic.EasyLibrary.EasyStrings, Basic.MvcLibrary");
				Type type = Type.GetType(easyType, false, false);
				if (type != null) { messagerConverters.Register(Activator.CreateInstance(type) as IMessageConverter); }
			}
			if (messagerConverters.ContainsKey("WebStrings") == false)
			{
				/*判断当前资源中是否存在按钮本地化资源*/
				string fullName = typeof(MessageContext).Assembly.FullName;
				string easyType = fullName.Replace("Basic.EntityLayer", "Basic.MvcLibrary.WebStrings, Basic.MvcLibrary");
				Type type = Type.GetType(easyType, false, false);
				if (type != null) { messagerConverters.Register(Activator.CreateInstance(type) as IMessageConverter); }
			}
			_converter = new DefaultMessageContext(messagerConverters, defaultMesage);
		}

		//private static Action<MessageConverterCollection> mCreator;
		/// <summary>注册资源转换器。</summary>
		/// <param name="creator">文本消息转换器，该转换器为实现了接口 IMessageConverter 的类实例。</param>
		[System.Security.SecuritySafeCritical]
		public static void Register(Func<IMessageRegister, IMessageConverter> creator)
		{
			IMessageConverter converter = creator?.Invoke(messagerConverters);
			if (converter != null) { defaultMesage = converter; _converter.SetDefault(converter); }
		}

		/// <summary>获取默认消息读取器</summary>
		public static IMessageContext Converter { get { return _converter; } }

		/// <summary>
		/// 获取为指定区域性本地化的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <param name="culture">
		/// System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。请注意，如果尚未为此区域性本地化该资源，则查找将使用当前线程的
		/// System.Globalization.CultureInfo.Parent 属性回退，并在非特定语言区域性中查找后停止。如果此值为 null，则使用当前线程的
		/// System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。
		/// </param>
		/// <returns>为指定区域性本地化的资源的值。如果不可能有最佳匹配，则返回 null。</returns>
		[System.Security.SecuritySafeCritical]
		public static string GetString(string name, System.Globalization.CultureInfo culture)
		{
			return _converter.GetString(name, culture);
		}

		/// <summary>
		/// 获取为指定区域性本地化的 System.String 资源的值。
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">要获取的资源名。</param>
		/// <param name="culture">
		/// System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。请注意，如果尚未为此区域性本地化该资源，则查找将使用当前线程的
		/// System.Globalization.CultureInfo.Parent 属性回退，并在非特定语言区域性中查找后停止。如果此值为 null，则使用当前线程的
		/// System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。
		/// </param>
		/// <returns>为指定区域性本地化的资源的值。如果不可能有最佳匹配，则返回 null。</returns>
		[System.Security.SecuritySafeCritical]
		public static string GetString(string converterName, string name, System.Globalization.CultureInfo culture)
		{
			return _converter.GetString(converterName, name, culture);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">资源名称</param>
		/// <param name="culture">
		/// System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。请注意，如果尚未为此区域性本地化该资源，则查找将使用当前线程的
		/// System.Globalization.CultureInfo.Parent 属性回退，并在非特定语言区域性中查找后停止。如果此值为 null，则使用当前线程的
		/// System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。
		/// </param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		[System.Security.SecuritySafeCritical]
		public static string GetString(string converterName, string name, System.Globalization.CultureInfo culture, params object[] args)
		{
			return _converter.GetString(converterName, name, culture, args);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="name">资源名称</param>
		/// <param name="culture">
		/// System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。请注意，如果尚未为此区域性本地化该资源，则查找将使用当前线程的
		/// System.Globalization.CultureInfo.Parent 属性回退，并在非特定语言区域性中查找后停止。如果此值为 null，则使用当前线程的
		/// System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。
		/// </param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		[System.Security.SecuritySafeCritical]
		public static string GetString(string name, System.Globalization.CultureInfo culture, params object[] args)
		{
			return _converter.GetString(name, culture, args);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public static string GetString(string name)
		{
			return _converter.GetString(name);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="name">资源名称</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>为指定区域性本地化的资源的值。如果不可能有最佳匹配，则返回 null。</returns>
		public static string GetString(string name, params object[] args)
		{
			return _converter.GetString(name, args);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">资源名称</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		[System.Security.SecuritySafeCritical]
		public static string GetString(string converterName, string name)
		{
			return _converter.GetString(converterName, name);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">资源名称</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		[System.Security.SecuritySafeCritical]
		public static string GetString(string converterName, string name, params object[] args)
		{
			return _converter.GetString(converterName, name, args);
		}

		/// <summary>
		/// 从资源文件中读取字节数组
		/// </summary>
		/// <param name="name">资源名称</param>
		/// <param name="culture">
		/// System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。请注意，如果尚未为此区域性本地化该资源，则查找将使用当前线程的
		/// System.Globalization.CultureInfo.Parent 属性回退，并在非特定语言区域性中查找后停止。如果此值为 null，则使用当前线程的
		/// System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。
		/// </param>
		/// <returns>返回读取成功的字节数组</returns>
		public static byte[] GetByteArray(string name, System.Globalization.CultureInfo culture)
		{
			return _converter.GetByteArray(name, culture);
		}

		/// <summary>
		/// 从资源文件中读取字节数组
		/// </summary>
		/// <param name="name">资源名称</param>
		/// <param name="culture">
		/// System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。请注意，如果尚未为此区域性本地化该资源，则查找将使用当前线程的
		/// System.Globalization.CultureInfo.Parent 属性回退，并在非特定语言区域性中查找后停止。如果此值为 null，则使用当前线程的
		/// System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。
		/// </param>
		/// <returns>返回读取成功的字节数组</returns>
		public static string GetByteArrayString(string name, System.Globalization.CultureInfo culture)
		{
			return _converter.GetByteArrayString(name, culture);
		}
	}
}
