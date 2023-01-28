using System;
using System.Configuration;

namespace Basic.Configuration
{
	/// <summary>
	/// 自定义多语言配置信息
	/// </summary>
	public sealed class CultureElement : ConfigurationElement
	{
		/// <summary>
		/// 初始化 CultureElement 类实例
		/// </summary>
		public CultureElement() { }

		/// <summary>
		/// 获取或设置格式为“&lt;languagecode2&gt;-&lt;country/regioncode2&gt;”的区域性名称。
		/// </summary>
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		/// <summary>
		/// 获取或设置当前 CultureInfo 的区域性标识符。
		/// </summary>
		[ConfigurationProperty("lcid", IsRequired = true, IsKey = true)]
		public string LCID
		{
			get { return (string)this["lcid"]; }
			set { this["lcid"] = value; }
		}

		/// <summary>
		/// 获取或设置当前 CultureInfo 的区域性标识符描述。
		/// </summary>
		[ConfigurationProperty("description", IsRequired = true, IsKey = true)]
		public string Description
		{
			get { return (string)this["description"]; }
			set { this["description"] = value; }
		}

		/// <summary>
		/// 默认转换的多语言信息
		/// </summary>
		[ConfigurationProperty("", IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(CultureItemCollection), AddItemName = "add", ClearItemsName = "clear")]
		public CultureItemCollection Values
		{
			get
			{
				return (CultureItemCollection)base[""];
			}
		}
	}

	/// <summary>
	/// 表示 CultureElement 配置节的集合
	/// </summary>
	public sealed class CultureCollection : ConfigurationElementCollection
	{
        /// <summary>
        /// 初始化 CultureCollection 类实例
        /// </summary>
        public CultureCollection() : base(StringComparer.CurrentCultureIgnoreCase) { }
		/// <summary>
		/// 获取在派生的类中重写时用于标识配置文件中此元素集合的名称。
		/// </summary>
		/// <value>集合的名称；否则为空字符串。默认值为空字符串。</value>
		protected override string ElementName { get { return "basic.culture"; } }

		/// <summary>
		/// 获取 ConnectionCollection 的类型
		/// </summary>
		/// <value>此集合的 System.Configuration.ConfigurationElementCollectionType</value>
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}

		/// <summary>
		/// 创建一个新的 ConnectionElementCollection类实例。
		/// </summary>
		/// <returns>新的 System.Configuration.ConfigurationElement子类实例。</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new CultureElement();
		}

		/// <summary>
		/// 获取指定配置元素的元素键。
		/// </summary>
		/// <param name="element">要为其返回键的 ConnectionElementCollection。</param>
		/// <returns>一个 System.Object，用作指定 ConnectionElementCollection.Name 的键。</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return (element as CultureElement).Name;
		}

		/// <summary>
		/// 根据配置元素关键字获取配置元素
		/// </summary>
		/// <param name="name">配置信息名称</param>
		/// <returns>返回具有指定键的连接信息。</returns>
		public CultureElement GetCulture(string name)
		{
			return base.BaseGet(name) as CultureElement;
		}
	}

}
