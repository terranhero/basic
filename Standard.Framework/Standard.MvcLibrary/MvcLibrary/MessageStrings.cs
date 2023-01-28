namespace Basic.MvcLibrary
{
	/// <summary></summary>
	public sealed class MessageStrings
	{
		private readonly IBasicContext mBH;
		internal MessageStrings(IBasicContext bh) { mBH = bh; }

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string name, params object[] args) { return mBH.GetString(name, args); }

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string name) { return mBH.GetString(name); }

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="converterName">消息转换器名称</param>
		/// <param name="name">要获取的资源名。</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string converterName, string name, params object[] args)
		{
			return mBH.GetString(converterName, name, args);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="converterName">消息转换器名称</param>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string converterName, string name)
		{
			return mBH.GetString(converterName, name);
		}
	}
}
