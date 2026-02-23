using System;
using System.Collections.Concurrent;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Basic.Collections;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Basic.Loggers
{
	/// <summary>表示抽象的日志写入类</summary>
	public abstract class LoggerWriter : ILoggerWriter
	{
		#region ILoggerWriter 对应数据库实例缓存
		private static ILoggerWriter UpdateValue(string key, ILoggerWriter value) { return value; }

		private static readonly ConcurrentDictionary<string, ILoggerWriter> writers = new ConcurrentDictionary<string, ILoggerWriter>();

		/// <summary>获取缓存的日志写入器</summary>
		/// <param name="key">数据库连接名称</param>
		/// <returns>日志写入器,如果缓存不存在则创建新的实例</returns>
		public static ILoggerWriter GetWriter(string key)
		{
			if (writers.TryGetValue(key, out ILoggerWriter writer) == false)
			{
				writer = new DefaultLoggerWriter(key);
				writers.AddOrUpdate(key, writer, UpdateValue);
			}
			return writer;
		}

		/// <summary>获取缓存的日志写入器</summary>
		/// <remarks>参数 <paramref name="writer"/>使用返回有效值，永不为null。</remarks>
		/// <param name="key">数据库连接名称</param>
		/// <param name="writer">日志写入器</param>
		/// <returns>如果已经存在则返回true，否则返回false。</returns>
		public static bool TryGetWriter(string key, out ILoggerWriter writer)
		{
			if (writers.TryGetValue(key, out writer) == true) { return true; }
			writer = new DefaultLoggerWriter(key);
			writers.AddOrUpdate(key, writer, UpdateValue);
			return false;
		}

		/// <summary>获取缓存的日志写入器</summary>
		/// <param name="key">数据库连接名称</param>
		/// <param name="writer">日志写入器</param>
		/// <remarks><!--true if the key was found in the <see cref="ConcurrentDictionary{TKey,TValue}"/>; otherwise, false--></remarks>
		public static ILoggerWriter AddOrUpdate(string key, ILoggerWriter writer)
		{
			return writers.AddOrUpdate(key, writer, UpdateValue);
		}
		#endregion

		#region 注入请求集合 - 将请求转换为菜单和功能
		private readonly static ActionCollection _actions = new ActionCollection();

		/// <summary>添加 Action映射。</summary>
		/// <param name="url">表示请求的路径。</param>
		/// <param name="controller">表示当前请求所属控制器、窗体名称</param>
		/// <param name="action">表示当前请求名称</param>
		public static void AddAction(string url, string controller, string action)
		{
			if (url == null) { return; } else { url = url.ToLower(); }
			if (_actions.ContainsKey(url)) { _actions[url] = new ActionInfo(url, controller, action); }
			else { _actions.Add(new ActionInfo(url, controller, action)); }
		}

		/// <summary>系统请求配置数量</summary>
		public static ActionCollection Actions { get { return _actions; } }

		/// <summary>系统请求配置数量</summary>
		public static int ActionCount { get { return _actions.Count; } }

		/// <summary>系统是否已经存在请求配置</summary>
		public static bool HasActions { get { return _actions.Count > 0; } }
		#endregion

		/// <summary>日志配置信息</summary>
		internal readonly static LoggerOptions options = LoggerOptions.Default;

		/// <summary>文件型日志记录器</summary>
		internal readonly static LocalFileStorage _fileStorage = new LocalFileStorage(options.Mode);

		/// <summary>控制台型日志记录器</summary>
		internal readonly static ConsoleStorage _console = new ConsoleStorage();

		/// <summary>本机IP地址</summary>
		internal readonly static string _host = GetComputerAddress();

		/// <summary>数据库型日志记录器</summary>
		internal readonly IDataBaseStorage _storage;
		/// <summary>初始化 LoggerWriter 类实例</summary>
		protected LoggerWriter(string connection) { _storage = new DataBaseStorage(connection, _fileStorage); }

		/// <summary>初始化 LoggerWriter 类实例</summary>
		protected LoggerWriter(IUserContext ctx) { _storage = new DataBaseStorage(ctx, _fileStorage); }

		/// <summary>返回当前日志写入器对应的数据库存储库</summary>
		public IDataBaseStorage Storage { get { return _storage; } }

		/// <summary>获取Web程序客户端IP地址或Windows程序本机IP地址</summary>
		/// <returns></returns>
		internal static string GetComputerAddress()
		{
			string hostName = string.Empty;
			System.Net.IPAddress[] ipArray = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
			foreach (System.Net.IPAddress ip in ipArray)
			{
				if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
					hostName = ip.ToString();
			}
			if (string.IsNullOrEmpty(hostName))
				hostName = System.Net.Dns.GetHostName();
			return hostName;
		}

		/// <summary>读取配置文件信息</summary>
		/// <param name="logLevel">日志级别</param>
		/// <param name="saveType">日志保存类型</param>
		/// <returns>返回当前级别日志是否需要记录</returns>
		private static bool GetSectionInfo(LogLevel logLevel, out LogSaveType saveType)
		{
			LogLevelOption opts = options.Information;
			if (logLevel == LogLevel.Information) { opts = options.Information; }
			else if (logLevel == LogLevel.Warning) { opts = options.Warning; }
			else if (logLevel == LogLevel.Error) { opts = options.Error; }
			else if (logLevel == LogLevel.Debug) { opts = options.Debug; }
			if (opts != null) { saveType = opts.SaveType; return opts.Enabled; }
			else { saveType = LogSaveType.None; return false; }
		}

		#region 通过代码初始化日志配置信息
		/// <summary>使用默认配置节绑定日志配置参数（Loggers）</summary>
		/// <remarks><code>
		/// Program.cs 启动文件中代码如下：<br/>
		/// services.ConfigLoggerOptions(opts =>
		/// {
		///		opts.Information.Enabled = true;
		///		opts.Information.SaveType = LogSaveType.DataBase;
		///		opts.Debug.Enabled = false;
		///		opts.Debug.SaveType = LogSaveType.DataBase;
		/// });</code>
		/// </remarks>
		/// <param name="action">包含要使用的设置的 <see cref="IConfigurationRoot"/></param>
		public static void ConfigureOptions(Action<LoggerOptions> action)
		{
			if (action != null) { action(LoggerOptions.Default); }
		}
		#endregion

#if NET6_0_OR_GREATER
		#region 记录日志信息 - 异步
		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		private ValueTask LoggerAsync(Guid batchNo, string url, string host, string user, Exception ex)
		{
			if (url == null) { return ValueTask.CompletedTask; } else { url = url.ToLower(); }
			if (_actions.TryGetValue(url, out ActionInfo ai))
			{
				return LoggerAsync(batchNo, ai.Controller, ai.Action, host, user, ex);
			}
			else
			{
				return LoggerAsync(batchNo, url, url, host, user, ex);
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		private ValueTask LoggerAsync(Guid batchNo, string controller, string action, string host, string user, Exception ex)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(host) == true) { host = GetComputerAddress(); }
				if (GetSectionInfo(LogLevel.Error, out LogSaveType saveType) && saveType != LogSaveType.None)
				{
					if (saveType == LogSaveType.LocalFile || saveType == LogSaveType.Windows)
					{
						return _fileStorage.ErrorAsync(batchNo, controller, action, host, user, ex);
					}
					else if (saveType == LogSaveType.DataBase && _storage != null)
					{
						return _storage.ErrorAsync(batchNo, controller, action, host, user, ex);
					}
					else if (saveType == LogSaveType.Console && _console != null)
					{
						return _console.ErrorAsync(batchNo, controller, action, host, user, ex);
					}

				}
				return ValueTask.CompletedTask;
			}
			catch (Exception ex1)
			{
				return _fileStorage.ErrorAsync(batchNo, controller, action, host, user, ex1);
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		private ValueTask LoggerAsync(Guid batchNo, string url, string host, string user, string message, LogLevel logLevel, LogResult resultType)
		{
			if (url == null) { return ValueTask.CompletedTask; } else { url = url.ToLower(); }
			if (_actions.TryGetValue(url, out ActionInfo ai))
			{
				return LoggerAsync(batchNo, ai.Controller, ai.Action, host, user, message, logLevel, resultType);
			}
			else
			{
				return LoggerAsync(batchNo, url, url, host, user, message, logLevel, resultType);
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		private ValueTask LoggerAsync(Guid batchNo, string controller, string action, string host, string user, string message, LogLevel logLevel, LogResult resultType)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(host) == true) { host = GetComputerAddress(); }
				if (GetSectionInfo(logLevel, out LogSaveType saveType) && saveType != LogSaveType.None)
				{
					if (saveType == LogSaveType.LocalFile || saveType == LogSaveType.Windows)
					{
						return _fileStorage.WriteAsync(batchNo, controller, action, host, user, message, logLevel, resultType);
					}
					else if (saveType == LogSaveType.DataBase && _storage != null)
					{
						return _storage.WriteAsync(batchNo, controller, action, host, user, message, logLevel, resultType);
					}
					else if (saveType == LogSaveType.Console)
					{
						return _console.WriteAsync(batchNo, controller, action, host, user, message, logLevel, resultType);
					}
				}
				return ValueTask.CompletedTask;
			}
			catch (Exception ex1)
			{
				return _fileStorage.ErrorAsync(batchNo, controller, action, host, user, ex1);
			}
		}
		#endregion

		#region 消息日志事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.InformationAsync(string url, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.InformationAsync(string url, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.InformationAsync(string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, controller, action, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.InformationAsync(Guid batchNo, string url, string user, string message)
		{
			return LoggerAsync(batchNo, url, _host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.InformationAsync(Guid batchNo, string url, string host, string user, string message)
		{
			return LoggerAsync(batchNo, url, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.InformationAsync(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(batchNo, controller, action, host, user, message, LogLevel.Information, LogResult.Successful);
		}
		#endregion

		#region 警告日志事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.WarningAsync(string url, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.WarningAsync(Guid batchNo, string url, string user, string message)
		{
			return LoggerAsync(batchNo, url, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.WarningAsync(string url, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.WarningAsync(Guid batchNo, string url, string host, string user, string message)
		{
			return LoggerAsync(batchNo, url, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.WarningAsync(string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, controller, action, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.WarningAsync(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(batchNo, controller, action, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}
		#endregion

		#region 错误日志记录 - 异步
		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.ErrorAsync(string url, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.ErrorAsync(string url, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.ErrorAsync(string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, controller, action, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.ErrorAsync(Guid batchNo, string url, string user, string message)
		{
			return LoggerAsync(batchNo, url, _host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.ErrorAsync(Guid batchNo, string url, string host, string user, string message)
		{
			return LoggerAsync(batchNo, url, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.ErrorAsync(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(batchNo, controller, action, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		ValueTask ILoggerWriter.ErrorAsync(string url, string user, Exception ex)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		ValueTask ILoggerWriter.ErrorAsync(string url, string host, string user, Exception ex)
		{
			return LoggerAsync(Guid.Empty, url, host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		ValueTask ILoggerWriter.ErrorAsync(string controller, string action, string host, string user, Exception ex)
		{
			return LoggerAsync(Guid.Empty, controller, action, host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		ValueTask ILoggerWriter.ErrorAsync(Guid batchNo, string url, string user, Exception ex)
		{
			return LoggerAsync(batchNo, url, _host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		ValueTask ILoggerWriter.ErrorAsync(Guid batchNo, string url, string host, string user, Exception ex)
		{
			return LoggerAsync(batchNo, url, host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		ValueTask ILoggerWriter.ErrorAsync(Guid batchNo, string controller, string action, string host, string user, Exception ex)
		{
			return LoggerAsync(batchNo, controller, action, host, user, ex);
		}
		#endregion

		#region 调试日志事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.DebugAsync(string url, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.DebugAsync(Guid batchNo, string url, string user, string message)
		{
			return LoggerAsync(batchNo, url, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.DebugAsync(string url, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.DebugAsync(Guid batchNo, string url, string host, string user, string message)
		{
			return LoggerAsync(batchNo, url, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.DebugAsync(string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, controller, action, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		ValueTask ILoggerWriter.DebugAsync(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(batchNo, controller, action, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}
		#endregion
#else
		#region 记录日志信息 - 异步
		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		private Task LoggerAsync(Guid batchNo, string url, string host, string user, Exception ex)
		{
			if (url == null) { return Task.CompletedTask; } else { url = url.ToLower(); }
			if (_actions.TryGetValue(url, out ActionInfo ai))
			{
				return LoggerAsync(batchNo, ai.Controller, ai.Action, host, user, ex);
			}
			else
			{
				return LoggerAsync(batchNo, url, url, host, user, ex);
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		private Task LoggerAsync(Guid batchNo, string controller, string action, string host, string user, Exception ex)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(host) == true) { host = GetComputerAddress(); }
				if (GetSectionInfo(LogLevel.Error, out LogSaveType saveType) && saveType != LogSaveType.None)
				{
					if (saveType == LogSaveType.LocalFile || saveType == LogSaveType.Windows)
					{
						return _fileStorage.ErrorAsync(batchNo, controller, action, host, user, ex);
					}
					else if (saveType == LogSaveType.DataBase && _storage != null)
					{
						return _storage.ErrorAsync(batchNo, controller, action, host, user, ex);
					}
					else if (saveType == LogSaveType.Console && _console != null)
					{
						return _console.ErrorAsync(batchNo, controller, action, host, user, ex);
					}
				}
				return Task.CompletedTask;
			}
			catch (Exception ex1)
			{
				return _fileStorage.ErrorAsync(batchNo, controller, action, host, user, ex1);
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		private Task LoggerAsync(Guid batchNo, string url, string host, string user, string message, LogLevel logLevel, LogResult resultType)
		{
			if (url == null) { return Task.CompletedTask; } else { url = url.ToLower(); }
			if (_actions.TryGetValue(url, out ActionInfo ai))
			{
				return LoggerAsync(batchNo, ai.Controller, ai.Action, host, user, message, logLevel, LogResult.Successful);
			}
			else
			{
				return LoggerAsync(batchNo, url, url, host, user, message, logLevel, LogResult.Successful);
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		private Task LoggerAsync(Guid batchNo, string controller, string action, string host, string user, string message, LogLevel logLevel, LogResult resultType)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(host) == true) { host = GetComputerAddress(); }
				if (GetSectionInfo(logLevel, out LogSaveType saveType) && saveType != LogSaveType.None)
				{
					if (saveType == LogSaveType.LocalFile || saveType == LogSaveType.Windows)
					{
						return _fileStorage.WriteAsync(batchNo, controller, action, host, user, message, logLevel, resultType);
					}
					else if (saveType == LogSaveType.DataBase && _storage != null)
					{
						return _storage.WriteAsync(batchNo, controller, action, host, user, message, logLevel, resultType);
					}
					else if (saveType == LogSaveType.Console)
					{
						return _console.WriteAsync(batchNo, controller, action, host, user, message, logLevel, resultType);
					}
				}
				return Task.CompletedTask;
			}
			catch (Exception ex1)
			{
				return _fileStorage.ErrorAsync(batchNo, controller, action, host, user, ex1);
			}
		}
		#endregion

		#region 消息日志事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.InformationAsync(string url, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.InformationAsync(string url, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.InformationAsync(string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, controller, action, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.InformationAsync(Guid batchNo, string url, string user, string message)
		{
			return LoggerAsync(batchNo, url, _host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.InformationAsync(Guid batchNo, string url, string host, string user, string message)
		{
			return LoggerAsync(batchNo, url, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.InformationAsync(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(batchNo, controller, action, host, user, message, LogLevel.Information, LogResult.Successful);
		}
		#endregion

		#region 警告日志事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.WarningAsync(string url, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.WarningAsync(Guid batchNo, string url, string user, string message)
		{
			return LoggerAsync(batchNo, url, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.WarningAsync(string url, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.WarningAsync(Guid batchNo, string url, string host, string user, string message)
		{
			return LoggerAsync(batchNo, url, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.WarningAsync(string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, controller, action, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.WarningAsync(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(batchNo, controller, action, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}
		#endregion

		#region 错误日志记录 - 异步
		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.ErrorAsync(string url, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.ErrorAsync(string url, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.ErrorAsync(string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, controller, action, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.ErrorAsync(Guid batchNo, string url, string user, string message)
		{
			return LoggerAsync(batchNo, url, _host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.ErrorAsync(Guid batchNo, string url, string host, string user, string message)
		{
			return LoggerAsync(batchNo, url, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.ErrorAsync(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(batchNo, controller, action, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ILoggerWriter.ErrorAsync(string url, string user, Exception ex)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ILoggerWriter.ErrorAsync(string url, string host, string user, Exception ex)
		{
			return LoggerAsync(Guid.Empty, url, host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ILoggerWriter.ErrorAsync(string controller, string action, string host, string user, Exception ex)
		{
			return LoggerAsync(Guid.Empty, controller, action, host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ILoggerWriter.ErrorAsync(Guid batchNo, string url, string user, Exception ex)
		{
			return LoggerAsync(batchNo, url, _host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ILoggerWriter.ErrorAsync(Guid batchNo, string url, string host, string user, Exception ex)
		{
			return LoggerAsync(batchNo, url, host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		Task ILoggerWriter.ErrorAsync(Guid batchNo, string controller, string action, string host, string user, Exception ex)
		{
			return LoggerAsync(batchNo, controller, action, host, user, ex);
		}
		#endregion

		#region 调试日志事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.DebugAsync(string url, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.DebugAsync(Guid batchNo, string url, string user, string message)
		{
			return LoggerAsync(batchNo, url, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.DebugAsync(string url, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, url, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.DebugAsync(Guid batchNo, string url, string host, string user, string message)
		{
			return LoggerAsync(batchNo, url, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.DebugAsync(string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(Guid.Empty, controller, action, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		Task ILoggerWriter.DebugAsync(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			return LoggerAsync(batchNo, controller, action, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}
		#endregion
#endif

		#region 日志信息事件 - 同步
		void ILoggerWriter.Information(string url, string user, string message)
		{
			Logger(Guid.Empty, url, _host, user, message, LogLevel.Information, LogResult.Successful);
		}

		void ILoggerWriter.Information(Guid batchNo, string url, string user, string message)
		{
			Logger(batchNo, url, _host, user, message, LogLevel.Information, LogResult.Successful);
		}

		void ILoggerWriter.Information(string url, string host, string user, string message)
		{
			Logger(Guid.Empty, url, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		void ILoggerWriter.Information(Guid batchNo, string url, string host, string user, string message)
		{
			Logger(batchNo, url, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		void ILoggerWriter.Information(string controller, string action, string host, string user, string message)
		{
			Logger(Guid.Empty, controller, action, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		void ILoggerWriter.Information(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			Logger(batchNo, controller, action, host, user, message, LogLevel.Information, LogResult.Successful);
		}
		#endregion

		#region 日志警告事件 - 同步
		void ILoggerWriter.Warning(string url, string user, string message)
		{
			Logger(Guid.Empty, url, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		void ILoggerWriter.Warning(Guid batchNo, string url, string user, string message)
		{
			Logger(batchNo, url, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		void ILoggerWriter.Warning(string url, string host, string user, string message)
		{
			Logger(Guid.Empty, url, host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		void ILoggerWriter.Warning(Guid batchNo, string url, string host, string user, string message)
		{
			Logger(batchNo, url, host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		void ILoggerWriter.Warning(string controller, string action, string host, string user, string message)
		{
			Logger(Guid.Empty, controller, action, host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		void ILoggerWriter.Warning(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			Logger(batchNo, controller, action, host, user, message, LogLevel.Warning, LogResult.Successful);
		}
		#endregion

		#region 日志调试事件 - 同步
		void ILoggerWriter.Debug(string url, string user, string message)
		{
			Logger(Guid.Empty, url, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		void ILoggerWriter.Debug(Guid batchNo, string url, string user, string message)
		{
			Logger(batchNo, url, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		void ILoggerWriter.Debug(string url, string host, string user, string message)
		{
			Logger(Guid.Empty, url, host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		void ILoggerWriter.Debug(Guid batchNo, string url, string host, string user, string message)
		{
			Logger(batchNo, url, host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		void ILoggerWriter.Debug(string controller, string action, string host, string user, string message)
		{
			Logger(Guid.Empty, controller, action, host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		void ILoggerWriter.Debug(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			Logger(batchNo, controller, action, host, user, message, LogLevel.Debug, LogResult.Successful);
		}
		#endregion

		#region 日志错误事件 - 同步
		void ILoggerWriter.Error(string url, string user, string message)
		{
			Logger(Guid.Empty, url, _host, user, message, LogLevel.Error, LogResult.Failed);
		}

		void ILoggerWriter.Error(Guid batchNo, string url, string user, string message)
		{
			Logger(batchNo, url, _host, user, message, LogLevel.Error, LogResult.Failed);
		}

		void ILoggerWriter.Error(string url, string host, string user, string message)
		{
			Logger(Guid.Empty, url, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		void ILoggerWriter.Error(Guid batchNo, string url, string host, string user, string message)
		{
			Logger(batchNo, url, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		void ILoggerWriter.Error(string controller, string action, string host, string user, string message)
		{
			Logger(Guid.Empty, controller, action, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		void ILoggerWriter.Error(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			Logger(batchNo, controller, action, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		void ILoggerWriter.Error(string url, string user, Exception ex)
		{
			Logger(Guid.Empty, url, _host, user, ex);
		}

		void ILoggerWriter.Error(Guid batchNo, string url, string user, Exception ex)
		{
			Logger(batchNo, url, _host, user, ex);
		}

		void ILoggerWriter.Error(string url, string host, string user, Exception ex)
		{
			Logger(Guid.Empty, url, host, user, ex);
		}

		void ILoggerWriter.Error(Guid batchNo, string url, string host, string user, Exception ex)
		{
			Logger(batchNo, url, host, user, ex);
		}

		void ILoggerWriter.Error(string controller, string action, string host, string user, Exception ex)
		{
			Logger(Guid.Empty, controller, action, host, user, ex);
		}

		void ILoggerWriter.Error(Guid batchNo, string controller, string action, string host, string user, Exception ex)
		{
			Logger(batchNo, controller, action, host, user, ex);
		}
		#endregion

		#region 记录日志信息 - 同步
		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		private void Logger(Guid batchNo, string url, string host, string user, Exception ex)
		{
			if (url == null) { return; } else { url = url.ToLower(); }
			if (_actions.TryGetValue(url, out ActionInfo ai))
			{
				Logger(batchNo, ai.Controller, ai.Action, host, user, ex);
			}
			else
			{
				Logger(batchNo, url, url, host, user, ex);
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		private void Logger(Guid batchNo, string url, string host, string user, string message, LogLevel logLevel, LogResult resultType)
		{
			if (url == null) { return; } else { url = url.ToLower(); }
			if (_actions.TryGetValue(url, out ActionInfo ai))
			{
				Logger(batchNo, ai.Controller, ai.Action, host, user, message, logLevel, resultType);
			}
			else
			{
				Logger(batchNo, url, url, host, user, message, logLevel, resultType);
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		private void Logger(Guid batchNo, string controller, string action, string host, string user, Exception ex)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(host) == true) { host = GetComputerAddress(); }
				if (GetSectionInfo(LogLevel.Error, out LogSaveType saveType) && saveType != LogSaveType.None)
				{
					if (saveType == LogSaveType.LocalFile || saveType == LogSaveType.Windows)
					{
						_fileStorage.ErrorAsync(batchNo, controller, action, host, user, ex);
					}
					else if (saveType == LogSaveType.DataBase && _storage != null)
					{
						_storage.ErrorAsync(batchNo, controller, action, host, user, ex);
					}
					else if (saveType == LogSaveType.Console)
					{
						_console.ErrorAsync(batchNo, controller, action, host, user, ex).ConfigureAwait(false);
					}
				}
			}
			catch (Exception ex1)
			{
				_fileStorage.ErrorAsync(batchNo, controller, action, host, user, ex1);
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		private void Logger(Guid batchNo, string controller, string action, string host, string user, string message, LogLevel logLevel, LogResult resultType)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(host) == true) { host = GetComputerAddress(); }
				if (GetSectionInfo(logLevel, out LogSaveType saveType) && saveType != LogSaveType.None)
				{
					if (saveType == LogSaveType.LocalFile || saveType == LogSaveType.Windows)
					{
						_fileStorage.WriteAsync(batchNo, controller, action, host, user, message, logLevel, resultType).ConfigureAwait(false);
					}
					else if (saveType == LogSaveType.DataBase && _storage != null)
					{
						_storage.WriteAsync(batchNo, controller, action, host, user, message, logLevel, resultType).ConfigureAwait(false);
					}
					else if (saveType == LogSaveType.Console)
					{
						_console.WriteAsync(batchNo, controller, action, host, user, message, logLevel, resultType).ConfigureAwait(false);
					}
				}
			}
			catch (Exception ex1)
			{
				_fileStorage.ErrorAsync(batchNo, controller, action, host, user, ex1).ConfigureAwait(false);
			}
		}

		#endregion
	}

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
