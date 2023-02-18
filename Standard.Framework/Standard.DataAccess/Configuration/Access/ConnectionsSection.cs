using System.Configuration;

namespace Basic.Configuration
{
	/// <summary>
	/// 配置文件中自定义数据库连接配置节
	/// </summary>
	public sealed class ConnectionsSection : ConfigurationSection
	{
		/// <summary>
		/// 初始化 ConnectionsSection 类实例
		/// </summary>
		public ConnectionsSection() { }

		/// <summary>
		/// 基础开发框架数据库连接配置节名称
		/// </summary>
		public const string ElementName = "basic.connections";

		/// <summary>默认数据库连接配置名称</summary>
		[ConfigurationProperty("defaultName", IsRequired = true)]
		public string DefaultName
		{
			get { return (string)this["defaultName"]; }
			set { this["defaultName"] = value; }
		}

		/// <summary>
		/// 数据库连接配置信息
		/// </summary>
		[ConfigurationProperty("", IsDefaultCollection = true), ConfigurationCollection(typeof(ConnectionCollection))]
		public ConnectionCollection Connections
		{
			get
			{
				return (ConnectionCollection)base[""];
			}
		}
	}
}
