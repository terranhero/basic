using System.Globalization;
using System.Runtime;
using System.Security;
using System.Xml.Serialization;

namespace Basic.Localizations
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class CustomCultureInfo : CultureInfo
	{
		#region Xml Element Definition
		internal const string XmlElementName = "cultureInfo";
		internal const string FileRefElementName = "fileRef";
		internal const string NameAttribute = "name";
		internal const string LcidAttribute = "lcid";
		#endregion

		/// <summary>
		/// 根据区域性标识符指定的区域性初始化 CustomCultureInfo 类的新实例。
		/// </summary>
		/// <param name="culture">预定义的 System.Globalization.CultureInfo 标识符、现有 System.Globalization.CultureInfo 对象的 System.Globalization.CultureInfo.LCID 属性或仅 Windows 区域性标识符。</param>
		/// <exception cref="System.ArgumentException">culture 不是有效的区域性标识符。- 或 -在 .NET Compact Framework 应用程序中，设备的操作系统不支持 culture。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">culture 小于零。</exception>
		public CustomCultureInfo(int culture) : base(culture) { }

		/// <summary>
		/// 基于区域性标识符指定的区域性并基于布尔值（指定是否使用系统中用户选定的区域性设置）来初始化 System.Globalization.CultureInfo 类的新实例。
		/// </summary>
		/// <param name="culture">预定义的 System.Globalization.CultureInfo 标识符、现有 System.Globalization.CultureInfo 对象的 System.Globalization.CultureInfo.LCID 属性或仅 Windows 区域性标识符。</param>
		/// <param name="useUserOverride">一个布尔值，它指示是使用用户选定的区域性设置 (true)，还是使用默认区域性设置 (false)。</param>
		/// <exception cref="System.ArgumentException">culture 不是有效的区域性标识符。- 或 -在 .NET Compact Framework 应用程序中，设备的操作系统不支持 culture。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">culture 小于零。</exception>
		public CustomCultureInfo(int culture, bool useUserOverride) : base(culture, useUserOverride) { }

		/// <summary>
		/// 根据由名称指定的区域性初始化 CustomCultureInfo 类的新实例。
		/// </summary>
		/// <param name="name">预定义的 System.Globalization.CultureInfo 名称、现有 System.Globalization.CultureInfo 的 
		/// System.Globalization.CultureInfo.Name 或仅 Windows 区域性名称。</param>
		/// <exception cref="System.ArgumentException">name 不是有效的区域性名称。- 或 -在 .NET Compact Framework 应用程序中，设备的操作系统不支持 culture。</exception>
		/// <exception cref="System.ArgumentNullException">name 为 null。</exception>
		[System.Security.SecuritySafeCritical]
		[TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
		public CustomCultureInfo(string name) : base(name) { }

		/// <summary>
		/// 基于名称指定的区域性并基于布尔值（指定是否使用系统中用户选定的区域性设置）来初始化 System.Globalization.CultureInfo 类的新实例。
		/// </summary>
		/// <param name="name">预定义的 System.Globalization.CultureInfo 名称、现有 System.Globalization.CultureInfo 的 
		/// System.Globalization.CultureInfo.Name 或仅 Windows 区域性名称。</param>
		/// <param name="useUserOverride">一个布尔值，它指示是使用用户选定的区域性设置 (true)，还是使用默认区域性设置 (false)。</param>
		/// <exception cref="System.ArgumentException">name 不是有效的区域性名称。- 或 -在 .NET Compact Framework 应用程序中，设备的操作系统不支持 culture。</exception>
		/// <exception cref="System.ArgumentNullException">name 为 null。</exception>
		public CustomCultureInfo(string name, bool useUserOverride) : base(name, useUserOverride) { }

	}
}
