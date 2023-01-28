
namespace Basic.Messages
{
	/// <summary>
	///  管理通用程序集信息资源
	/// </summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public static class ResUtil
	{
		private static System.Resources.ResourceManager resourceMan;

		/// <summary>
		///   Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		private static System.Resources.ResourceManager ResManager
		{
			get
			{
				if (resourceMan == null)
					resourceMan = new System.Resources.ResourceManager("Basic.Messages.Message", typeof(ResUtil).Assembly);
				return resourceMan;
			}
		}

		/// <summary>
		/// 获取资源字符串
		/// </summary>
		/// <param name="keyName">资源名称</param>
		/// <param name="cultrueInfo">特定的区域信息</param>
		/// <returns>返回获取的指定键的资源</returns>
		public static string GetString(string keyName, System.Globalization.CultureInfo cultrueInfo)
		{
			return ResManager.GetString(keyName, cultrueInfo);
		}

		/// <summary>
		/// 获取资源字符串
		/// </summary>
		/// <param name="keyName">资源名称</param>
		/// <returns>返回获取的指定键的资源</returns>
		public static string GetString(string keyName)
		{
			return ResManager.GetString(keyName, System.Globalization.CultureInfo.CurrentUICulture);
		}

		/// <summary>
		/// 获取资源字符串,并用指定的格式化字符串，格式化取出的字符串
		/// </summary>
		/// <param name="keyName">资源名称</param>
		/// <param name="cultrueInfo">特定的区域信息</param>
		/// <param name="paramArray">需要格式化字符串的参数</param>
		/// <returns>返回获取的指定键的资源</returns>
		public static string GetString(string keyName, System.Globalization.CultureInfo cultrueInfo, params object[] paramArray)
		{
			string msg = ResManager.GetString(keyName, cultrueInfo);
			if (paramArray == null)
				return msg;
			if (msg == null)
				return keyName;
			return string.Format(msg, paramArray);
		}

		/// <summary>
		/// 获取资源字符串,并用指定的格式化字符串，格式化取出的字符串
		/// </summary>
		/// <param name="keyName">资源名称</param>
		/// <param name="paramArray">需要格式化字符串的参数</param>
		/// <returns>返回获取的指定键的资源</returns>
		public static string GetString(string keyName, params object[] paramArray)
		{
			string msg = ResManager.GetString(keyName, System.Globalization.CultureInfo.CurrentUICulture);
			if (msg == null)
				return keyName;
			if (paramArray == null)
				return msg;
			return string.Format(msg, paramArray);
		}

		/// <summary>
		/// 使用指定的区域性从指定的资源返回 System.IO.UnmanagedMemoryStream 对象。
		/// </summary>
		/// <param name="name">资源的名称。</param>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是 System.IO.MemoryStream 对象。</exception>
		/// <exception cref="System.ArgumentNullException">name 为 null。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		/// <returns> 一个 System.IO.Stream 对象。</returns>
		public static System.IO.Stream GetStream(string name)
		{
			return ResManager.GetStream(name);
		}

		/// <summary>
		/// 使用指定的区域性从指定的资源返回 System.IO.UnmanagedMemoryStream 对象。
		/// </summary>
		/// <param name="name">资源的名称。</param>
		/// <param name="culture"> System.Globalization.CultureInfo 对象，它指定用于资源查找的区域性。如果 culture 为 null，则使用当前线程的区域性。</param>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是 System.IO.MemoryStream 对象。</exception>
		/// <exception cref="System.ArgumentNullException">name 为 null。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		/// <returns> 一个 System.IO.Stream 对象。</returns>
		public static System.IO.Stream GetStream(string name, System.Globalization.CultureInfo culture)
		{
			return ResManager.GetStream(name, culture);
		}
	}
}
