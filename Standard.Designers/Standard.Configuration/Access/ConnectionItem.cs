using System.Configuration;

namespace Basic.Configuration
{
	/// <summary>
	/// 配置文件中自定义数据库配置节
	/// </summary>
	public sealed class ConnectionItem : ConfigurationElement
	{
		/// <summary>
		/// 初始化 ConnectionItemElement 类实例
		/// </summary>
		public ConnectionItem() { }

		/// <summary>
		/// 数据库连接配置信息项名称
		/// </summary>
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		/// <summary>
		/// 数据库连接配置信息项值
		/// </summary>
		[ConfigurationProperty("value", IsRequired = true)]
		public string Value
		{
			get { return (string)this["value"]; }
			set { this["value"] = value; }
		}
	}

}
