using System.Collections.Generic;
using System.Configuration;
using Basic.DataAccess;
using Basic.Enums;
using Basic.Exceptions;
using Basic.Interfaces;
using Microsoft.Extensions.Configuration;
using SC = System.Configuration;

namespace Basic.Configuration
{
	/// <summary>
	/// 默认 IConnectionInfo 接口的实现，用于绑定数据
	/// </summary>
	internal sealed class JsonConnectionInfo : Dictionary<string, string>, IConnectionInfo
	{
		/// <summary>数据库连接类型</summary>
		public ConnectionType ConnectionType { get; set; }

		/// <summary>数据库版本号</summary>
		public int Version { get; set; }

		/// <summary>数据库连接名称</summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// 依赖注入扩展，添加日志配置信息
	/// </summary>
	public static class ConnectionExtension
	{
		/// <summary>初始化数据库连接参数</summary>
		internal static void InitializeConnection(ConnectionStringsSection section)
		{
			ConnectionContext.Clear();
			foreach (ConnectionStringSettings element in section.ConnectionStrings)
			{
				if (element.ProviderName.Contains("SqlClient"))
				{
					ConnectionContext.Create(element.Name, ConnectionType.SqlConnection,
				   element.ConnectionString, element.ConnectionString);
				}
				else if (element.ProviderName.Contains("Oracle"))
				{
					ConnectionContext.Create(element.Name, ConnectionType.OracleConnection,
					element.ConnectionString, element.ConnectionString);
				}
				else if (element.ProviderName.Contains("MySqlClient"))
				{
					ConnectionContext.Create(element.Name, ConnectionType.MySqlConnection,
				   element.ConnectionString, element.ConnectionString);
				}
				ConnectionContext.ChangeDefault(element.Name);
				break;
			}
		}

		/// <summary>初始化数据库连接参数</summary>
		internal static void InitializeConnection(ConnectionsSection section)
		{
			//ConnectionCollection connections = section.Connections;
			string _DefaultName = section.DefaultName;
			ConnectionContext.Clear();
			foreach (ConnectionElement element in section.Connections)
			{
				ConnectionInfo info = ConnectionFactoryBuilder.CreateConnectionInfo(element);
				ConnectionContext.Create(info);
			}
			ConnectionContext.ChangeDefault(_DefaultName);
		}

		/// <summary>从指定配置文件中初始化数据库连接信息</summary>
		/// <param name="fullName">配置文件路径</param>
		public static void InitializeConfiguration(string fullName)
		{
			string sectionName = ConnectionsSection.ElementName;

			SC.ConfigurationFileMap fileMap = new SC.ConfigurationFileMap(fullName);
			SC.Configuration config = SC.ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
			SC.ConfigurationSection section = config.GetSection(string.Concat(ConfigurationGroup.ElementName, "/", sectionName));
			if (section == null)        //读取配置文件异常，配置文件"{0}"中，不存在自定义配置组"'。
				throw new ConfigurationFileException("Access_Configuration_GroupNotFound", fullName, sectionName);
			if (section is ConnectionsSection configurationSection) { InitializeConnection(configurationSection); }
		}

		/// <summary>初始化数据库连接参数</summary>
		/// <param name="connections">表示数据库连接配置</param>
		public static void InitializeConnections(IConfigurationSection connections)
		{
			ConnectionContext.Clear();
			string _DefaultName = connections.GetValue<string>("DefaultName");
			IConfigurationSection dbConnections = connections.GetRequiredSection("Connections");
			foreach (IConfigurationSection item in dbConnections.GetChildren())
			{
				JsonConnectionInfo info = item.Get<JsonConnectionInfo>(); if (info == null) { continue; }
				info.Name = item.Key; info.Version = item.GetValue<int>("Version");
				info.ConnectionType = item.GetValue<ConnectionType>("ConnectionType");
				info.Remove("Version"); info.Remove("ConnectionType");
				Basic.Configuration.ConnectionInfo connection = ConnectionFactoryBuilder.CreateConnectionInfo(info);
				ConnectionContext.Create(connection);
			}
			ConnectionContext.ChangeDefault(_DefaultName);
		}
	}

}
