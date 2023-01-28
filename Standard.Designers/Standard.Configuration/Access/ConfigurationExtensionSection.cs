using Basic.Enums;
using System.Configuration;

namespace Basic.Configuration
{
	/// <summary>
	/// 配置文件扩展名称
	/// </summary>
	public class ConfigurationExtensionSection : ConfigurationSection
	{
		/// <summary>
		/// 数据库连接构造器全名
		/// </summary>
		[ConfigurationProperty("DefaultExtension", DefaultValue = "cf", IsRequired = true)]
		public string DefaultExtension
		{
			get { return (string)base["DefaultExtension"]; }
			set { base["DefaultExtension"] = value; }
		}

		/// <summary>
		/// 数据库连接配置
		/// </summary>
		[ConfigurationProperty("", IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(ConfigurationExtensionCollection), AddItemName = "ConfigurationExtension",
			CollectionType = ConfigurationElementCollectionType.BasicMap)]
		public ConfigurationExtensionCollection Extensions
		{
			get
			{
				return (ConfigurationExtensionCollection)base[""];
			}
		}
	}


	/// <summary>
	/// 表示ConnectionElement配置节的集合
	/// </summary>
	public sealed class ConfigurationExtensionCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// 创建一个新的 ConnectionElementCollection类实例。
		/// </summary>
		/// <returns>新的 System.Configuration.ConfigurationElement子类实例。</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ConfigurationExtensionElement();
		}

		/// <summary>
		/// 获取指定配置元素的元素键。
		/// </summary>
		/// <param name="element">要为其返回键的 ConnectionElementCollection。</param>
		/// <returns>一个 System.Object，用作指定 ConnectionElementCollection.Name 的键。</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return (element as ConfigurationExtensionElement).ConnectionType;
		}

		/// <summary>
		/// 用于标识配置文件中此元素集合的名称。
		/// </summary>
		/// <value>集合的名称；否则为空字符串。默认值为空字符串。</value>
		protected override string ElementName { get { return "ConfigurationExtension"; } }

		/// <summary>
		///  获取或设置此配置元素。
		/// </summary>
		/// <param name="name">键名称</param>
		/// <returns>返回指定键的元素</returns>
		public ConfigurationExtensionElement this[object name]
		{
			get { return (ConfigurationExtensionElement)BaseGet(name); }
			set
			{
				if (BaseGet(name) != null)
				{
					BaseRemove(name);
				}
				BaseAdd(value);
			}
		}
	}

	/// <summary>
	/// 配置文件中自定义数据库配置节
	/// </summary>
	public sealed class ConfigurationExtensionElement : ConfigurationElement
	{
		/// <summary>
		/// 初始化ConnectionElement类实例
		/// </summary>
		public ConfigurationExtensionElement() { }
		/// <summary>
		/// 数据库连接类型
		/// </summary>
		[ConfigurationProperty("ConnectionType", DefaultValue = "SqlConnection", IsRequired = true, IsKey = true)]
		public ConnectionType ConnectionType
		{
			get { return (ConnectionType)this["ConnectionType"]; }
			set { this["ConnectionType"] = value; }
		}

		/// <summary>
		/// 配置文件扩展名不带点号
		/// </summary>
		[ConfigurationProperty("Extension", DefaultValue = "cf", IsRequired = true)]
		public string Extension
		{
			get { return (string)this["Extension"]; }
			set { this["Extension"] = value; }
		}
	}
}
