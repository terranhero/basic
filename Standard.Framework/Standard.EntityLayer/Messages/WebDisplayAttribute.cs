
namespace Basic.EntityLayer
{
	/// <summary>
	/// 指定属性的显示名称, 此文件带多语言转换。
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
	public sealed class WebDisplayAttribute : System.ComponentModel.DisplayNameAttribute
	{
		private readonly string _ConverterName = null;
		private readonly string _Prompt = null;
		/// <summary>
		/// 使用显示名称初始化 Basic.EntityLayer.WebDisplayAttribute 类的新实例。
		/// </summary>
		/// <param name="displayName">显示名称。</param>
		public WebDisplayAttribute(string displayName)
			: base(displayName) { }

		/// <summary>
		/// 使用显示名称初始化 Basic.EntityLayer.WebDisplayAttribute 类的新实例。
		/// </summary>
		/// <param name="converterName">转换器名称</param>
		/// <param name="displayName">显示名称。</param>
		public WebDisplayAttribute(string displayName, string converterName)
			: base(displayName) { _ConverterName = converterName; }

		/// <summary>
		/// 使用显示名称初始化 Basic.EntityLayer.WebDisplayAttribute 类的新实例。
		/// </summary>
		/// <param name="converterName">转换器名称</param>
		/// <param name="displayName">显示名称。</param>
		/// <param name="prompt">用户界面中的提示设置水印。</param>
		public WebDisplayAttribute(string displayName, string converterName, string prompt)
			: base(displayName) { _ConverterName = converterName; _Prompt = prompt; }

		/// <summary>
		/// 指定当前属性显示名称需要转换的转换器名称。
		/// </summary>
		/// <value>属性名称的文本转换器名称。</value>
		public string ConverterName { get { return _ConverterName; } }

		/// <summary>
		/// 获取当前属性 DisplayName 是否存在。
		/// </summary>
		public bool HasDisplayName { get { return !string.IsNullOrEmpty(base.DisplayNameValue); } }

		/// <summary>
		/// 获取当前属性 ConverterName 是否存在。
		/// </summary>
		public bool HasConverterName { get { return !string.IsNullOrEmpty(_ConverterName); } }

		/// <summary>
		/// 获取一个值，该值将用于为用户界面中的提示设置水印。
		/// </summary>
		public string Prompt { get { return _Prompt; } }
	}

	/// <summary>
	/// 指定属性的显示名称, 此文件带多语言转换。
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Enum)]
	public sealed class WebDisplayConverterAttribute : System.Attribute
	{
		private readonly string _ConverterName = null;
		/// <summary>
		/// 使用显示名称初始化 Basic.EntityLayer.WebDisplayAttribute 类的新实例。
		/// </summary>
		/// <param name="converterName">转换器名称</param>
		public WebDisplayConverterAttribute(string converterName)
			: base() { _ConverterName = converterName; }

		/// <summary>
		/// 指定当前属性显示名称需要转换的转换器名称。
		/// </summary>
		/// <value>属性名称的文本转换器名称。</value>
		public string ConverterName { get { return _ConverterName; } }

		///// <summary>
		///// 指定属性的本地化显示名称。
		///// </summary>
		///// <value>显示名称。</value>
		//public override string DisplayName
		//{
		//    get
		//    {
		//        string displayName = base.DisplayName;
		//        //if (string.IsNullOrEmpty(_ConverterName))
		//        //    base.DisplayNameValue = MessageManager.GetString(displayName);
		//        //else
		//        //    base.DisplayNameValue = MessageManager.GetString(_ConverterName, displayName);
		//        return base.DisplayName;
		//    }
		//}
	}

}
