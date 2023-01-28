using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Configuration;
using Basic.Enums;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示 ConfigurationAttribute 对象初始化
	/// </summary>
	internal sealed class ConfigurationInfo : IComparable<ConfigurationInfo>
	{
		private readonly Type _OwnerType;
		private readonly ConnectionInfo _ConnectionInfo;
		private readonly string _configExtension;
		private readonly string _ConfigFilePath;
		/// <summary>初始化ConfigurationInfo类实例</summary>
		/// <param name="module">应用程序模块名称</param>
		/// <param name="tableName">数据库表名称</param>
		/// <param name="configFileType">配置文件的类型</param>
		/// <param name="connectionName">数据连接配置信息名称</param>
		/// <param name="info">表示数据库连接配置信息</param>
		/// <param name="type">表示当前 Access 类型</param>
		internal ConfigurationInfo(string module, string tableName, ConfigFileType configFileType,
			string connectionName, Type type, ConnectionInfo info)
		{
			Module = module;
			TableName = tableName;
			Key = String.IsNullOrWhiteSpace(module) ? tableName : String.Concat(module, ".", tableName);
			ConfigType = configFileType;
			ConnectionName = connectionName;
			_OwnerType = type; _ConnectionInfo = info;
			_configExtension = ConfigurationExtension.GetExtension(info.ConnectionType);
			_ConfigFilePath = GetConfigurationFilePath("Access", _configExtension);
		}

		/// <summary>应用程序模块名称</summary>
		public string Module { get; private set; }

		/// <summary>配置文件键名称</summary>
		public string Key { get; private set; }

		/// <summary>数据库表名称</summary>
		public string TableName { get; private set; }

		/// <summary>
		/// 配置文件类型
		/// </summary>
		public ConfigFileType ConfigType { get; private set; }

		/// <summary>
		/// 数据连接名称
		/// </summary>
		public string ConnectionName { get; private set; }

		/// <summary>
		/// 比较 ConfigurationInfo 对象和另一 ConfigurationInfo 对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>一个值，指示要比较的对象的相对顺序。
		/// 返回值的含义如下：值含义小于零此对象小于 other 参数。
		/// 零此对象等于 other。大于零此对象大于 other。</returns>
		public int CompareTo(ConfigurationInfo other)
		{
			if (string.IsNullOrEmpty(ConnectionName) && string.IsNullOrEmpty(other.ConnectionName)) { return 0; }
			return string.Compare(ConnectionName, other.ConnectionName);
		}


		/// <summary>
		/// 获取配置文件全路径
		/// </summary>
		/// <param name="configFolder">配置文件本地根目录路径</param>
		/// <param name="configFileExtension">配置文件扩展名</param>
		/// <returns>返回配置文件全路径</returns>
		public string GetConfigurationFilePath(string configFolder, string configFileExtension)
		{
			string configFilePath = null;
			if (ConfigType == ConfigFileType.LocalFile)
			{
				if (string.IsNullOrEmpty(Module) == true)
				{
					configFilePath = string.Format("{0}\\{1}.{2}", configFolder, TableName, configFileExtension);
				}
				else { configFilePath = string.Format("{0}\\{1}\\{2}.{3}", configFolder, Module, TableName, configFileExtension); }
			}
			else if (ConfigType == ConfigFileType.AssemlyResource)
			{
				if (string.IsNullOrEmpty(Module) == true)
				{
					configFilePath = string.Format("{0}.{1}", TableName, configFileExtension);
				}
				else { configFilePath = string.Format("{0}.{1}.{2}", Module, TableName, configFileExtension); }
			}
			else { configFilePath = TableName; }
			return configFilePath;
		}


		/// <summary>
		/// 获取签入资源的资源名称
		/// </summary>
		/// <returns></returns>
		internal string GetResourceFile()
		{
			if (ConfigType != ConfigFileType.Resource) { return null; }
			string configFileExtension = ConfigurationExtension.GetExtension(_ConnectionInfo.ConnectionType);
			if (string.IsNullOrEmpty(Module))
				return string.Concat(TableName, ".", configFileExtension);
			return string.Concat(Module, "/", TableName, ".", configFileExtension);
		}

		/// <summary>
		/// 创建命令生成类
		/// </summary>
		/// <returns>返回创建成功的继承 CommandBuilder 接口的实例</returns>
		internal CommandBuilder CreateCommandBuilder()
		{
			if (ConfigType == ConfigFileType.LocalFile)
				return new LocalFileStructBuilder(_ConfigFilePath);
			else if (ConfigType == ConfigFileType.DataBase)
				return new DataBaseStructBuilder(_ConfigFilePath);
			else if (ConfigType == ConfigFileType.Resource)
				return new ResourceStructBuilder(_OwnerType, this);
			return new AssemblyStructBuilder(_OwnerType, _ConfigFilePath);
		}
	}
}
