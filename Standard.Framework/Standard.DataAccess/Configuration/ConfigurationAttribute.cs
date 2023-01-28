using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Basic.Configuration;
using Basic.Enums;
using Basic.Exceptions;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示配置信息
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class ConfigurationAttribute : Attribute
	{
		/// <summary>
		/// 初始化ConfigurationAttribute类实例
		/// </summary>
		/// <param name="tableName">数据库表名称</param>
		public ConfigurationAttribute(string tableName) : this(null, tableName, ConfigFileType.AssemlyResource) { }

		/// <summary>
		/// 初始化ConfigurationAttribute类实例
		/// </summary>
		/// <param name="tableName">数据库表名称</param>
		/// <param name="configFileType">配置文件的类型</param>
		public ConfigurationAttribute(string tableName, ConfigFileType configFileType) : this(null, tableName, configFileType) { }

		/// <summary>
		/// 初始化ConfigurationAttribute类实例
		/// </summary>
		/// <param name="module">应用程序模块名称</param>
		/// <param name="tableName">数据库表名称</param>
		public ConfigurationAttribute(string module, string tableName) : this(module, tableName, ConfigFileType.AssemlyResource) { }

		/// <summary>
		/// 初始化ConfigurationAttribute类实例
		/// </summary>
		/// <param name="module">应用程序模块名称</param>
		/// <param name="tableName">数据库表名称</param>
		/// <param name="configFileType">配置文件的类型</param>
		public ConfigurationAttribute(string module, string tableName, ConfigFileType configFileType)
		{
			Module = module;
			TableName = tableName;
			ConfigType = configFileType;
		}

		/// <summary>
		/// 创建 ConfigurationInfo 类实例
		/// </summary>
		/// <param name="type">当前特性标记的Access类</param>
		/// <param name="connectionName">当前数据库连接名称</param>
		/// <returns>返回 ConfigurationInfo 实例类型。</returns>
		internal ConfigurationInfo CreateInfo(Type type, string connectionName)
		{
			string conName = connectionName ?? ConnectionContext.DefaultName;
			ConnectionInfo config = ConnectionContext.DefaultConnection;
			if (string.IsNullOrEmpty(connectionName) == false)
			{
				if (ConnectionContext.Contains(connectionName) == false)
				{
					throw new ConfigurationException("Access_NotExistsConnection", type, connectionName);
				}
				config = ConnectionContext.GetConnection(connectionName);
			}
			return new ConfigurationInfo(Module, TableName, ConfigType, conName, type, config);
		}

		///// <summary>
		///// 获取配置文件全路径
		///// </summary>
		///// <param name="ns">Access类的命名空间</param>
		///// <param name="configFolder">配置文件本地根目录路径</param>
		///// <param name="configFileExtension">配置文件扩展名</param>
		///// <returns>返回配置文件全路径</returns>
		//public string GetConfigurationFilePath(string ns, string configFolder, string configFileExtension)
		//{
		//	string configFilePath = null;
		//	if (ConfigType == ConfigFileType.LocalFile && string.IsNullOrEmpty(Module))
		//		configFilePath = string.Format("{0}\\{1}.{2}", configFolder, TableName, configFileExtension);
		//	else if (ConfigType == ConfigFileType.LocalFile && !string.IsNullOrEmpty(Module))
		//		configFilePath = string.Format("{0}\\{1}\\{2}.{3}", configFolder, Module, TableName, configFileExtension);
		//	else if (ConfigType == ConfigFileType.AssemlyResource && string.IsNullOrEmpty(Module))
		//		configFilePath = string.Format("{0}.{1}.{2}", ns, TableName, configFileExtension);
		//	else if (ConfigType == ConfigFileType.AssemlyResource && !string.IsNullOrEmpty(Module))
		//		configFilePath = string.Format("{0}.{1}.{2}.{3}", ns, Module, TableName, configFileExtension);
		//	else
		//		configFilePath = TableName;
		//	return configFilePath;
		//}

		/// <summary>
		/// 获取签入资源的资源名称
		/// </summary>
		/// <returns></returns>
		internal string GetResourceFile()
		{
			if (ConfigType != ConfigFileType.Resource) { return null; }
			string configFileExtension = ConfigurationExtension.GetExtension(info.ConnectionType);
			if (string.IsNullOrEmpty(Module))
				return string.Concat(TableName, ".", configFileExtension);
			return string.Concat(Module, "/", TableName, ".", configFileExtension);
		}

		private ConnectionInfo info;
		/// <summary>
		/// 登记连接，不同连接配置使用不同的文件。
		/// </summary>
		/// <param name="config"></param>
		internal void RegisterConnection(ConnectionInfo config) { info = config; }

		/// <summary>
		/// 应用程序模块名称
		/// </summary>
		public string Module { get; private set; }

		/// <summary>
		/// 数据库表名称
		/// </summary>
		public string TableName { get; private set; }

		/// <summary>
		/// 配置文件类型
		/// </summary>
		public ConfigFileType ConfigType { get; private set; }

		///// <summary>
		///// 数据连接名称
		///// </summary>
		//public string ConnectionName { get; internal set; }
	}
}
