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
			ConnectionExtension.InitializeConnections(connections);
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
			ConnectionExtension.InitializeConnections(connections);
			return services;
		}
	}

}
