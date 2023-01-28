using System.Configuration;
using Basic.Enums;

namespace Basic.Configuration
{
	/// <summary>
	/// 配置文件中自定义数据库连接配置信息
	/// </summary>
	public sealed class ConnectionElement : ConfigurationElement
	{
		/// <summary>
		/// 初始化 Connection 类实例
		/// </summary>
		public ConnectionElement() { }

		/// <summary>
		/// 自定义数据库信息名称，此名称集合中唯一。
		/// </summary>
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		/// <summary>
		/// 数据库连接类型
		/// </summary>
		[ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
		public bool Enabled
		{
			get { return (bool)this["enabled"]; }
			set { this["enabled"] = value; }
		}

		/// <summary>
		/// 数据库连接类型
		/// </summary>
		[ConfigurationProperty("connectionType", DefaultValue = "SqlConnection", IsRequired = true)]
		public ConnectionType ConnectionType
		{
			get { return (ConnectionType)this["connectionType"]; }
			set { this["connectionType"] = value; }
		}

		/// <summary>数据库版本</summary>
		[ConfigurationProperty("version", DefaultValue = "10", IsRequired = false)]
		public int Version
		{
			get { return (int)this["version"]; }
			set { this["version"] = value; }
		}

		///// <summary>
		///// 数据库类型
		///// </summary>
		//[ConfigurationProperty("databaseType", DefaultValue = "SqlServer", IsRequired = false)]
		//public DataBaseType DataBaseType
		//{
		//	get { return (DataBaseType)this["databaseType"]; }
		//	set { this["databaseType"] = value; }
		//}

		/// <summary>
		/// 数据库连接配置信息
		/// </summary>
		[ConfigurationProperty("", IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(ConnectionItemCollection), AddItemName = "add", ClearItemsName = "clear")]
		public ConnectionItemCollection Values
		{
			get
			{
				return (ConnectionItemCollection)base[""];
			}
		}
	}

}
