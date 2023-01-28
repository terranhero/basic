namespace Basic.Messages
{
	/// <summary>消息文本读取接口</summary>
	public interface IMessageContext
	{
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
		string GetString(string name, System.Globalization.CultureInfo culture);

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
		string GetString(string converterName, string name, System.Globalization.CultureInfo culture);

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
		string GetString(string converterName, string name, System.Globalization.CultureInfo culture, params object[] args);

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
		string GetString(string name, System.Globalization.CultureInfo culture, params object[] args);

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		string GetString(string name);

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="name">资源名称</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>为指定区域性本地化的资源的值。如果不可能有最佳匹配，则返回 null。</returns>
		string GetString(string name, params object[] args);

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">资源名称</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		string GetString(string converterName, string name);

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">资源名称</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		string GetString(string converterName, string name, params object[] args);

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
		byte[] GetByteArray(string name, System.Globalization.CultureInfo culture);

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
		string GetByteArrayString(string name, System.Globalization.CultureInfo culture);
	}

	/// <summary>消息文本读取接口</summary>
	internal sealed class DefaultMessageContext : IMessageContext
	{
		private IMessageConverter defaultConverter;
		private readonly MessageConverterCollection messagerConverters;

		/// <summary></summary>
		/// <param name="converters"></param>
		/// <param name="converter"></param>
		public DefaultMessageContext(MessageConverterCollection converters, IMessageConverter converter)
		{
			messagerConverters = converters; defaultConverter = converter;
		}

		internal void SetDefault(IMessageConverter converter) { defaultConverter = converter; }

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
		public string GetString(string name, System.Globalization.CultureInfo culture)
		{
			if (defaultConverter != null)
			{
				string text;
				if (culture != null) { text = defaultConverter.GetString(name, culture); }
				else { text = defaultConverter.GetString(name); }
				if (text != null) { return text; }
			}
			return name;
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
		public string GetString(string converterName, string name, System.Globalization.CultureInfo culture)
		{
			if (converterName != null && messagerConverters.ContainsKey(converterName))
			{
				IMessageConverter messageConverter = messagerConverters[converterName];
				string text;
				if (culture != null) { text = messageConverter.GetString(name, culture); }
				else { text = messageConverter.GetString(name); }
				if (string.IsNullOrEmpty(text)) { return name; }
				else { return text; }
			}
			return GetString(name, culture);
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
		public string GetString(string converterName, string name, System.Globalization.CultureInfo culture, params object[] args)
		{
			if (converterName != null && messagerConverters.ContainsKey(converterName))
			{
				IMessageConverter messageConverter = messagerConverters[converterName];
				string text;
				if (culture != null)
					text = messageConverter.GetString(name, culture, args);
				else
					text = messageConverter.GetString(name, args);
				if (string.IsNullOrEmpty(text))
					return name;
				else
					return text;
			}
			return GetString(name, culture, args);
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
		public string GetString(string name, System.Globalization.CultureInfo culture, params object[] args)
		{
			if (defaultConverter != null)
			{
				string text;
				if (culture != null)
					text = defaultConverter.GetString(name, culture, args);
				else
					text = defaultConverter.GetString(name, args);

				if (string.IsNullOrEmpty(text))
					return name;
				else
					return text;
			}
			return name;
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string name)
		{
			return defaultConverter.GetString(name);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="name">资源名称</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>为指定区域性本地化的资源的值。如果不可能有最佳匹配，则返回 null。</returns>
		public string GetString(string name, params object[] args)
		{
			return defaultConverter.GetString(name, args);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">资源名称</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		[System.Security.SecuritySafeCritical]
		public string GetString(string converterName, string name)
		{
			if (converterName != null && messagerConverters.ContainsKey(converterName))
			{
				IMessageConverter messageConverter = messagerConverters[converterName];
				string text = messageConverter.GetString(name);
				if (string.IsNullOrEmpty(text))
					return name;
				else
					return text;
			}
			return GetString(name);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">资源名称</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		[System.Security.SecuritySafeCritical]
		public string GetString(string converterName, string name, params object[] args)
		{
			if (converterName != null && messagerConverters.ContainsKey(converterName))
			{
				IMessageConverter messageConverter = messagerConverters[converterName];
				string text = messageConverter.GetString(name, args);
				if (string.IsNullOrEmpty(text))
					return name;
				else
					return text;
			}
			return GetString(name, args);
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
		public byte[] GetByteArray(string name, System.Globalization.CultureInfo culture)
		{
			System.IO.Stream stream = ResUtil.GetStream(name);
			if (stream == null && defaultConverter != null)
				stream = defaultConverter.GetStream(name, culture);
			if (stream == null)
				return null;
			using (stream)
			{
				byte[] buffer = new byte[stream.Length];
				stream.Read(buffer, 0, buffer.Length);
				return buffer;
			}
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
		public string GetByteArrayString(string name, System.Globalization.CultureInfo culture)
		{
			System.IO.Stream stream = ResUtil.GetStream(name, culture);
			if (stream == null && defaultConverter != null)
				stream = defaultConverter.GetStream(name, culture);
			if (stream == null)
				return null;
			using (stream)
			{
				byte[] buffer = new byte[stream.Length];
				stream.Read(buffer, 0, buffer.Length);
				return System.Text.Encoding.Unicode.GetString(buffer);
			}
		}
	}
}
