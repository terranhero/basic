using System.Resources;
using System.Globalization;

namespace Basic.Properties
{
    /// <summary>
    /// 
    /// </summary>
	public static class StringUtils
	{
		private static global::System.Resources.ResourceManager resourceMan;

		private static global::System.Globalization.CultureInfo resourceCulture = CultureInfo.CurrentUICulture;

		/// <summary>
		///   返回此类使用的缓存的 ResourceManager 实例。
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		private static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					resourceMan = new ResourceManager("Basic.Properties.Designer", typeof(StringUtils).Assembly);
				}
				return resourceMan;
			}
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		public static string GetString(string name)
		{
			string returnValue = null;
			if (resourceCulture.LCID == 2052)
				returnValue = ResourceManager.GetString(string.Format("{0}-zh-CN", name));
			else
				returnValue = ResourceManager.GetString(string.Format("{0}-en-US", name));
			if (!string.IsNullOrWhiteSpace(returnValue))
				return returnValue;
			return name;
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		public static string GetString(string name, params object[] args)
		{
			string returnValue = null;
			if (resourceCulture.LCID == 2052)
				returnValue = ResourceManager.GetString(string.Format("{0}-zh-CN", name));
			else
				returnValue = ResourceManager.GetString(string.Format("{0}-en-US", name));
			if (!string.IsNullOrWhiteSpace(returnValue) && args != null && args.Length > 0)
				return string.Format(returnValue, args);
			else if (!string.IsNullOrWhiteSpace(returnValue))
				return returnValue;
			return name;
		}

		/// <summary>
		/// 从指定的资源返回 System.IO.UnmanagedMemoryStream 对象。
		/// </summary>
		/// <param name="name">资源的名称。</param>
		/// <returns> 一个 System.IO.UnmanagedMemoryStream 对象。</returns>
		public static System.IO.Stream GetStream(string name)
		{
			return ResourceManager.GetStream(name);
		}
	}
}
