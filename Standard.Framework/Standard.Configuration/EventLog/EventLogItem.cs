using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;

namespace Basic.Configuration
{
	/// <summary>
	/// 配置文件中自定义数据库配置节
	/// </summary>
	public sealed class EventLogItem : ConfigurationElement
	{
		/// <summary>
		/// 初始化 EventLogItem 类实例
		/// </summary>
		public EventLogItem() { }

		/// <summary>
		/// 获取或设置格式为“&lt;languagecode2&gt;-&lt;country/regioncode2&gt;”的区域性名称。
		/// </summary>
		[ConfigurationProperty("address", IsRequired = true, IsKey = true)]
		public string Address
		{
			get { return (string)this["address"]; }
			set { this["address"] = value; }
		}

		/// <summary>
		/// 获取或设置 由创建此实例时指定的显示名和地址信息构成的显示名。
		/// </summary>
		[ConfigurationProperty("displayName")]
		public string DisplayName
		{
			get { return (string)this["displayName"]; }
			set { this["displayName"] = value; }
		}
	}

	/// <summary>
	/// 表示 CultureItem 配置节的集合
	/// </summary>
	public sealed class EventLogItemCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// 获取在派生的类中重写时用于标识配置文件中此元素集合的名称。
		/// </summary>
		/// <value>集合的名称；否则为空字符串。默认值为空字符串。</value>
		protected override string ElementName { get { return "mailTo"; } }

		/// <summary>
		/// 获取 ConnectionItemCollection 的类型
		/// </summary>
		/// <value>此集合的 System.Configuration.ConfigurationElementCollectionType</value>
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		/// <summary>
		/// 创建一个新的 ConnectionElementCollection类实例。
		/// </summary>
		/// <returns>新的 System.Configuration.ConfigurationElement子类实例。</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new EventLogItem();
		}

		/// <summary>
		/// 获取指定配置元素的元素键。
		/// </summary>
		/// <param name="element">要为其返回键的 ConnectionElementCollection。</param>
		/// <returns>一个 System.Object，用作指定 ConnectionElementCollection.Name 的键。</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return (element as EventLogItem).Address;
		}

		/// <summary>
		/// 获取电子邮件地址
		/// </summary>
		/// <returns></returns>
		public MailAddress[] GetToMailArray()
		{
			List<MailAddress> list = new List<MailAddress>(base.Count);
			foreach (EventLogItem item in this)
			{
				MailAddress mailAddress = new MailAddress(item.Address, item.DisplayName);
				list.Add(mailAddress);
			}
			return list.ToArray();
		}
	}
}
