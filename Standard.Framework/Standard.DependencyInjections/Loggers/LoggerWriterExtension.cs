using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Basic.Enums;
using Basic.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Basic.Loggers
{
	/// <summary>
	/// 依赖注入扩展，添加日志配置信息
	/// </summary>
	public static class LoggerOptionsExtension
	{
		/// <summary>使用默认配置节绑定日志配置参数（Loggers）</summary>
		/// <remarks><code>
		/// Program.cs 启动文件中代码如下：<br/>
		/// services.ConfigLoggerOptions(opts =>
		/// {
		///		opts.Information.Enabled = true;
		///		opts.Information.SaveType = Basic.Enums.LogSaveType.DataBase;
		///		opts.Warning.Enabled = true;
		///		opts.Warning.SaveType = Basic.Enums.LogSaveType.DataBase;
		///		opts.Error.Enabled = true;
		///		opts.Error.SaveType = Basic.Enums.LogSaveType.DataBase;
		///		opts.Debug.Enabled = true;
		///		opts.Debug.SaveType = Basic.Enums.LogSaveType.DataBase;
		/// });</code>
		/// </remarks>
		/// <param name="services">用于添加服务的 <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/></param>
		/// <param name="action">包含要使用的设置的 <see cref="IConfigurationRoot"/></param>
		public static IServiceCollection ConfigureOptions(this IServiceCollection services, Action<LoggerOptions> action)
		{
			if (action != null) { action(LoggerOptions.Default); }
			return services;
		}

		/// <summary>使用默认配置节绑定日志配置参数（Loggers）</summary>
		/// <remarks>
		/// <code>	
		/// json配置文件格式如下所示：<br/>
		/// "Loggers": {
		///		"Mode": "Monthly", //表示日志文件记录级别分(Daily / Weekly / Monthly)
		///		"TableName": "SYS_EVENTLOGGER",
		///		"Information": {
		/// 		"SaveType": "DataBase", //日志保存类型, (None, LocalFile, DataBase, Console)
		/// 		"Enabled": true //该级别日志配置信息是否有效
		///		},
		/// 	"Warning": {
		///			"SaveType": "DataBase", //日志保存类型, (None, LocalFile, DataBase, Console)
		/// 		"Enabled": true //该级别日志配置信息是否有效
		///		},
		/// 	"Error": {
		/// 		"SaveType": "DataBase", //日志保存类型, (None, LocalFile, DataBase, Console)
		/// 		"Enabled": true //该级别日志配置信息是否有效
		///		},
		/// 	"Debug": {
		/// 		"SaveType": "LocalFile", //日志保存类型, (None, LocalFile, DataBase, Console)
		/// 		"Enabled": false //该级别日志配置信息是否有效
		///		}
		/// }
		/// </code>
		/// <code>
		/// Program.cs 启动文件中代码如下：<br/>
		/// services.AddLoggerOptions(root, opts =>
		/// {
		///		opts.BindNonPublicProperties = false;
		///		opts.ErrorOnUnknownConfiguration = true;
		/// });</code>
		/// </remarks>
		/// <param name="services">用于添加服务的 <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/></param>
		/// <param name="root">包含要使用的设置的 <see cref="IConfigurationRoot"/></param>
		public static IServiceCollection AddLoggerOptions(this IServiceCollection services, IConfigurationRoot root)
		{
			IConfigurationSection logger = root.GetSection("Loggers");
			return services.AddLoggerOptions(logger, opts =>
			{
				opts.BindNonPublicProperties = false;
				opts.ErrorOnUnknownConfiguration = true;
			});
		}

		/// <summary>使用自定义配置节名称绑定日志配置参数</summary>
		/// <remarks>
		/// <code>	
		/// json配置文件格式如下所示：<br/>
		/// "Loggers": {
		///		"Mode": "Monthly", //表示日志文件记录级别分(Daily / Weekly / Monthly)
		///		"TableName": "SYS_EVENTLOGGER",
		///		"Information": {
		/// 		"SaveType": "DataBase", //日志保存类型, (None, LocalFile, DataBase, Console)
		/// 		"Enabled": true //该级别日志配置信息是否有效
		///		},
		/// 	"Warning": {
		///			"SaveType": "DataBase", //日志保存类型, (None, LocalFile, DataBase, Console)
		/// 		"Enabled": true //该级别日志配置信息是否有效
		///		},
		/// 	"Error": {
		/// 		"SaveType": "DataBase", //日志保存类型, (None, LocalFile, DataBase, Console)
		/// 		"Enabled": true //该级别日志配置信息是否有效
		///		},
		/// 	"Debug": {
		/// 		"SaveType": "LocalFile", //日志保存类型, (None, LocalFile, DataBase, Console)
		/// 		"Enabled": false //该级别日志配置信息是否有效
		///		}
		/// }
		/// </code>
		/// <code>
		/// Program.cs 启动文件中代码如下：<br/>
		/// IConfigurationSection logger = config.GetSection("Loggers");
		/// services.AddLoggerOptions(logger, opts =>
		/// {
		///		opts.BindNonPublicProperties = false;
		///		opts.ErrorOnUnknownConfiguration = true;
		/// });</code>
		/// </remarks>
		/// <param name="services">用于添加服务的 <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/></param>
		/// <param name="logger">包含要使用的设置的 <see cref="IConfigurationSection"/></param>
		public static IServiceCollection AddLoggerOptions(this IServiceCollection services, IConfigurationSection logger)
		{
			return services.AddLoggerOptions(logger, opts =>
			{
				opts.BindNonPublicProperties = false;
				opts.ErrorOnUnknownConfiguration = true;
			});
		}

		/// <summary>绑定日志配置参数</summary>
		/// <remarks>
		/// <code>	
		/// json配置文件格式如下所示：<br/>
		/// "Loggers": {
		///		"Mode": "Monthly", //表示日志文件记录级别分(Daily / Weekly / Monthly)
		///		"TableName": "SYS_EVENTLOGGER",
		///		"Information": {
		/// 		"SaveType": "DataBase", //日志保存类型, (None,LocalFile,DataBase,Console)
		/// 		"Enabled": true //该级别日志配置信息是否有效
		///		},
		/// 	"Warning": {
		///			"SaveType": "DataBase", //日志保存类型, (None,LocalFile,DataBase,Console)
		/// 		"Enabled": true //该级别日志配置信息是否有效
		///		},
		/// 	"Error": {
		/// 		"SaveType": "DataBase", //日志保存类型, (None,LocalFile,DataBase,Console)
		/// 		"Enabled": true //该级别日志配置信息是否有效
		///		},
		/// 	"Debug": {
		/// 		"SaveType": "LocalFile", //日志保存类型, (None,LocalFile,DataBase,Console)
		/// 		"Enabled": false //该级别日志配置信息是否有效
		///		}
		/// }
		/// </code>
		/// <code>
		/// Program.cs 启动文件中代码如下：<br/>
		/// IConfigurationSection logger = config.GetSection("Loggers");
		/// services.AddLoggerOptions(logger, opts =>
		/// {
		///		opts.BindNonPublicProperties = false;
		///		opts.ErrorOnUnknownConfiguration = true;
		/// });</code>
		/// </remarks>
		/// <param name="services">用于添加服务的 <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/></param>
		/// <param name="logger">包含要使用的设置的 <see cref="IConfigurationSection"/></param>
		/// <param name="configureOptions">Configures the binder options.</param>
		public static IServiceCollection AddLoggerOptions(this IServiceCollection services, IConfigurationSection logger, Action<BinderOptions> configureOptions)
		{
			logger.Bind(LoggerOptions.Default, configureOptions);
			return services;
		}
	}
}
