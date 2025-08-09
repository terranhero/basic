using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using Basic.DataAccess;
using Basic.Enums;
using Basic.Exceptions;
using Basic.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SC = System.Configuration;

namespace Basic.Configuration
{
	/// <summary>
	/// 数据库连接上下文类
	/// </summary>
	public static class ConnectionContext
	{
		//static ConnectionContext()
		//{
		//	string configName = ConfigurationGroup.ElementName;
		//	string secName = ConnectionsSection.ElementName;
		//	object section = ConfigurationManager.GetSection(string.Concat(configName, "/", secName));
		//	if (section != null && section is ConnectionsSection configurationSection)
		//	{
		//		InitializeConnection(configurationSection);
		//	}
		//}

		private readonly static SortedList<string, ConnectionInfo> _Connections = new SortedList<string, ConnectionInfo>(100);
		private static ConnectionInfo _DefaultConnection;
		private static string _DefaultName;
		/// <summary>
		/// 默认数据库连接配置信息
		/// </summary>
		public static string DefaultName
		{
			get { return _DefaultName; }
			private set { _DefaultName = value; }
		}

		/// <summary>
		/// 默认数据库连接配置信息
		/// </summary>
		public static ConnectionInfo DefaultConnection
		{
			get { return _DefaultConnection; }
			private set { _DefaultConnection = value; }
		}

		/// <summary>
		/// 数据数据库连接集合清空
		/// </summary>
		public static void Clear() { _Connections.Clear(); _DefaultConnection = null; _DefaultName = null; }

		/// <summary>初始化数据库连接参数</summary>
		internal static void InitializeConnection(ConnectionStringsSection section)
		{
			_Connections.Clear();
			foreach (ConnectionStringSettings element in section.ConnectionStrings)
			{
				DefaultName = element.Name;
				if (element.ProviderName.Contains("SqlClient"))
				{
					DefaultConnection = Create(element.Name, ConnectionType.SqlConnection,
					element.ConnectionString, element.ConnectionString);
				}
				else if (element.ProviderName.Contains("Oracle"))
				{
					DefaultConnection = Create(element.Name, ConnectionType.OracleConnection,
					element.ConnectionString, element.ConnectionString);
				}
				else if (element.ProviderName.Contains("MySqlClient"))
				{
					DefaultConnection = Create(element.Name, ConnectionType.MySqlConnection,
					element.ConnectionString, element.ConnectionString);
				}
				break;
			}
		}

		/// <summary>初始化数据库连接参数</summary>
		/// <param name="connections">表示数据库连接配置</param>
		public static void InitializeConnections(IConfigurationSection connections)
		{
			Clear(); _DefaultName = connections.GetValue<string>("DefaultName");
			IConfigurationSection dbConnections = connections.GetRequiredSection("Connections");
			foreach (IConfigurationSection item in dbConnections.GetChildren())
			{
				JsonConnectionInfo info = item.Get<JsonConnectionInfo>(); if (info == null) { continue; }
				info.Name = item.Key; info.Version = item.GetValue<int>("Version");
				info.ConnectionType = item.GetValue<ConnectionType>("ConnectionType");
				info.Remove("Version"); info.Remove("ConnectionType");
				Basic.Configuration.ConnectionInfo connection = ConnectionFactoryBuilder.CreateConnectionInfo(info);
				Create(connection); if (_DefaultName == item.Key) { _DefaultConnection = connection; }
			}
		}

		/// <summary>初始化数据库连接参数</summary>
		internal static void InitializeConnection(ConnectionsSection section)
		{
			//ConnectionCollection connections = section.Connections;
			_DefaultName = section.DefaultName;
			_Connections.Clear();
			foreach (ConnectionElement element in section.Connections)
			{
				ConnectionInfo info = ConnectionFactoryBuilder.CreateConnectionInfo(element);
				//string connectionString = ConnectionStringBuilder.CreateConnectionString(element);
				//string display = ConnectionStringBuilder.CreateDisplayString(element);
				Create(info);
				if (_DefaultName == element.Name) { _DefaultConnection = info; }
			}
		}

		/// <summary>从指定配置文件中初始化数据库连接信息</summary>
		/// <param name="fullName">配置文件路径</param>
		public static void InitializeConfiguration(string fullName)
		{
			string sectionName = ConnectionsSection.ElementName;

			SC.ConfigurationFileMap fileMap = new SC.ConfigurationFileMap(fullName);
			SC.Configuration config = SC.ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
			ConfigurationSection section = config.GetSection(string.Concat(ConfigurationGroup.ElementName, "/", sectionName));
			if (section == null)        //读取配置文件异常，配置文件"{0}"中，不存在自定义配置组"'。
				throw new ConfigurationFileException("Access_Configuration_GroupNotFound", fullName, sectionName);
			if (section is ConnectionsSection configurationSection) { InitializeConnection(configurationSection); }
		}

		/// <summary>更改默认数据库连接</summary>
		/// <param name="name">数据库连接名称</param>
		/// <returns>表示数据库连接信息</returns>
		public static void ChangeDefault(string name)
		{
			if (_Connections.ContainsKey(name))
			{
				_DefaultName = name;
				_DefaultConnection = _Connections[name];
			}
		}

		/// <summary>
		/// 将 ConnectionInfo 类实例添加到系统数据库连接集合中。
		/// 如果集合中已经存在相同的名称的连接则覆盖原有连接。
		/// </summary>
		/// <param name="info">表示数据库连接信息</param>
		/// <returns>返回当前添加成功的 ConnectionInfo 类实例</returns>
		/// <exception cref="System.ArgumentNullException">参数 name 为 空或null。</exception>
		public static ConnectionInfo CreateOrReplace(ConnectionInfo info)
		{
			_Connections[info.Name] = info;
			return info;
		}

		/// <summary>
		/// 将 ConnectionInfo 类实例添加到系统数据库连接集合中。
		/// </summary>
		/// <param name="name">数据库连接名称</param>
		/// <param name="type">数据库连接类型</param>
		/// <param name="conString">数据库连接字符串</param>
		/// <param name="display">当前连接字符串显示值</param>
		/// <returns>返回当前添加成功的 ConnectionInfo 类实例</returns>
		/// <exception cref="System.ArgumentNullException">参数 name 为空或null。</exception>
		/// <exception cref="System.ArgumentException">参数 name 已经存在。</exception>
		public static ConnectionInfo CreateOrReplace(string name, ConnectionType type, string conString, string display)
		{
			ConnectionInfo info = new ConnectionInfo(name, type, conString, display);
			_Connections[info.Name] = info;
			return info;
		}

		/// <summary>
		/// 将 ConnectionInfo 类实例添加到系统数据库连接集合中。
		/// </summary>
		/// <param name="name">数据库连接名称</param>
		/// <param name="type">数据库连接类型</param>
		/// <param name="conString">数据库连接字符串</param>
		/// <param name="display">当前连接字符串显示值</param>
		/// <returns>返回当前添加成功的 ConnectionInfo 类实例</returns>
		/// <exception cref="System.ArgumentNullException">参数 name 为空或null。</exception>
		/// <exception cref="System.ArgumentException">参数 name 已经存在。</exception>
		public static ConnectionInfo Create(string name, ConnectionType type, string conString, string display)
		{
			if (_Connections.ContainsKey(name))
				throw new System.ArgumentException("已经存在相同名称的连接", "name");
			ConnectionInfo info = new ConnectionInfo(name, type, conString, display);
			_Connections.Add(info.Name, info);
			return info;
		}

		/// <summary>
		/// 将 ConnectionInfo 类实例添加到系统数据库连接集合中。
		/// 如果集合中已经存在相同的名称的连接则覆盖原有连接。
		/// </summary>
		/// <param name="info">表示数据库连接信息</param>
		/// <returns>返回当前添加成功的 ConnectionInfo 类实例</returns>
		/// <exception cref="System.ArgumentNullException">参数 name 为 空或null。</exception>
		public static ConnectionInfo Create(ConnectionInfo info)
		{
			if (_Connections.ContainsKey(info.Name))
				throw new System.ArgumentException("已经存在相同名称的连接", "info.Name");
			_Connections.Add(info.Name, info);
			return info;
		}

		/// <summary>
		/// 将 ConnectionInfo 类实例添加到系统数据库连接集合中。
		/// </summary>
		/// <param name="info">数据库连接名称</param>
		/// <returns>返回当前添加成功的 ConnectionInfo 类实例</returns>
		/// <exception cref="System.ArgumentNullException">参数 name 为空或null。</exception>
		/// <exception cref="System.ArgumentException">参数 name 已经存在。</exception>
		public static ConnectionInfo CreateOrReplace(IConnectionInfo info)
		{
			ConnectionInfo connection = ConnectionFactoryBuilder.CreateConnectionInfo(info);
			_Connections[connection.Name] = connection;
			return connection;
		}

		/// <summary>
		/// 将 ConnectionInfo 类实例添加到系统数据库连接集合中。
		/// </summary>
		/// <param name="name">数据库连接名称</param>
		/// <param name="type">数据库连接类型</param>
		/// <param name="version">数据库版本号</param>
		/// <param name="conString">数据库连接字符串</param>
		/// <param name="display">当前连接字符串显示值</param>
		/// <returns>返回当前添加成功的 ConnectionInfo 类实例</returns>
		/// <exception cref="System.ArgumentNullException">参数 name 为空或null。</exception>
		/// <exception cref="System.ArgumentException">参数 name 已经存在。</exception>
		public static ConnectionInfo Create(string name, ConnectionType type, int version, string conString, string display)
		{
			if (_Connections.ContainsKey(name))
				throw new System.ArgumentException("已经存在相同名称的连接", "name");
			ConnectionInfo info = new ConnectionInfo(name, type, version, conString, display);
			_Connections.Add(info.Name, info);
			return info;
		}

		/// <summary>获取系统所有已配置的数据库连接</summary>
		/// <returns>系统所有已配置的数据库连接信息。</returns>
		public static IList<ConnectionInfo> GetConnections()
		{
			return new ReadOnlyCollection<ConnectionInfo>(_Connections.Values);
		}

		/// <summary>
		/// 获取与指定的名称相关联的连接信息
		/// </summary>
		/// <param name="name">要获取或设置其值的键。</param>
		/// <returns>获取与指定的键相关联的连接信息。</returns>
		public static ConnectionInfo GetConnection(string name)
		{
			if (!_Connections.ContainsKey(name))
				throw new System.ArgumentException(string.Concat("名称为\"", name, "\"的连接不存在。"));
			return _Connections[name];
		}

		/// <summary>
		/// 确定当前数据库连接集合中是否包含特定连接。
		/// </summary>
		/// <param name="name">要获取或设置其值的键。</param>
		/// <returns>获取与指定的键相关联的连接信息。</returns>
		public static bool Contains(string name)
		{
			if (string.IsNullOrWhiteSpace(name)) { return false; }
			return _Connections.ContainsKey(name);
		}

		/// <summary>
		/// 获取与指定的名称相关联的连接信息。
		/// </summary>
		/// <param name="name">要获取或设置其值的键。</param>
		/// <param name="info">当此方法返回时，如果找到指定键，则返回与该键相关联的值；
		/// 否则，将返回 info 参数的类型的默认值。 该参数未经初始化即被传递。</param>
		/// <returns>如果包含具有指定名称的连接信息，则为 true；否则为 false。</returns>
		/// <exception cref="System.ArgumentNullException">name 为 null。</exception>
		public static bool TryGetConnection(string name, out ConnectionInfo info)
		{
			return _Connections.TryGetValue(name, out info);
		}
	}

	/// <summary>
	/// 依赖注入扩展，添加日志配置信息
	/// </summary>
	public static class ConnectionExtension
	{
		/// <summary>使用默认配置节加载数据库连接（Connections）</summary>
		/// <remarks>
		/// <code>	
		/// json配置文件格式如下所示：<br/>
		/// "Connections": {
		/// 	"DefaultName": "HRMS",
		/// 	"HRMS": {
		///			"ConnectionType": "SqlConnection",
		///			"Version": 12,
		///			"Application Name": "HRMS",
		/// 		"Initial Catalog": "HRMS-V5",
		/// 		"Data Source": "(local)",
		/// 		"User ID": "sa",
		/// 		"Password": "C/1Shp55C14b0TtEhs87bg==",
		/// 		"TrustServerCertificate": "True"
		/// 	}
		/// }
		/// </code>
		/// <code>
		/// Program.cs 启动文件中代码如下：<br/>
		/// services.AddConnections(root);</code>
		/// </remarks>
		/// <param name="services">用于添加服务的 <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/></param>
		/// <param name="root">包含要使用的设置的 <see cref="IConfigurationRoot"/></param>
		public static IServiceCollection AddConnections(this IServiceCollection services, IConfigurationRoot root)
		{
			IConfigurationSection connections = root.GetSection("Connections");
			ConnectionContext.InitializeConnections(connections);
			return services;
		}

		/// <summary>使用自定义配置节名称绑定日志配置参数</summary>
		/// <remarks>
		/// <code>	
		/// json配置文件格式如下所示：<br/>
		/// "Connections": {
		/// 	"DefaultName": "HRMS",
		/// 	"HRMS": {
		///			"ConnectionType": "SqlConnection",
		///			"Version": 12,
		///			"Application Name": "HRMS",
		/// 		"Initial Catalog": "HRMS-V5",
		/// 		"Data Source": "(local)",
		/// 		"User ID": "sa",
		/// 		"Password": "C/1Shp55C14b0TtEhs87bg==",
		/// 		"TrustServerCertificate": "True"
		/// 	}
		/// }
		/// </code>
		/// <code>
		/// Program.cs 启动文件中代码如下：<br/>
		/// services.AddConnections(root);</code>
		/// </remarks>
		/// <param name="services">用于添加服务的 <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/></param>
		/// <param name="connections">包含要使用的设置的 <see cref="IConfigurationSection"/></param>
		public static IServiceCollection AddConnections(this IServiceCollection services, IConfigurationSection connections)
		{
			ConnectionContext.InitializeConnections(connections);
			return services;
		}
	}

}
