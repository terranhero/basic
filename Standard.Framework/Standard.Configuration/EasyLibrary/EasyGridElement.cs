using System;
using System.Configuration;

namespace Basic.Configuration
{
	/// <summary>
	/// 配置文件中自定义数据库配置节
	/// </summary>
	public sealed class EasyGridElement : ConfigurationElement
	{
		/// <summary>
		/// 
		/// </summary>
		public const int PageSizeDefault = 20;
		/// <summary>
		/// 初始化 EasyGridElement 类实例
		/// </summary>
		public EasyGridElement() { }

		/// <summary>
		/// 获取或设置EasyGrid分页列表，每页允许的记录数。
		/// </summary>
		[ConfigurationProperty("pageList", DefaultValue = "20,50,80", IsRequired = false)]
		public string PageList
		{
			get { return (string)this["pageList"]; }
			set { this["pageList"] = value; }
		}

		/// <summary>
		/// 获取或设置EasyGrid分页列表，每页允许的记录数。
		/// </summary>
		[ConfigurationProperty("pageSize", DefaultValue = PageSizeDefault, IsRequired = false)]
		public int PageSize
		{
			get { return (int)this["pageSize"]; }
			set { this["pageSize"] = value; }
		}


		/// <summary>
		/// 获取或设置 EasyGrid 是否需要填充满屏。
		/// </summary>
		[ConfigurationProperty("fit", DefaultValue = true, IsRequired = false)]
		public bool Fit { get { return (bool)this["fit"]; } set { this["fit"] = value; } }

		/// <summary>
		/// 获取或设置 EasyGrid 是否需要填充满屏。
		/// </summary>
		[ConfigurationProperty("checkOnSelect", DefaultValue = true, IsRequired = false)]
		public bool CheckOnSelect { get { return (bool)this["checkOnSelect"]; } set { this["checkOnSelect"] = value; } }

		/// <summary>
		/// 获取或设置 EasyGrid 是否需要填充满屏。
		/// </summary>
		[ConfigurationProperty("selectOnCheck", DefaultValue = true, IsRequired = false)]
		public bool SelectOnCheck { get { return (bool)this["selectOnCheck"]; } set { this["selectOnCheck"] = value; } }
	}
}
