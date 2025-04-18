﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Basic.Collections;
using Basic.Configuration;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;
using static System.Collections.Specialized.BitVector32;

namespace Basic.LogInfo
{
	/// <summary>将日志写入本地文件中</summary>
	public interface IFileLoggerWriter : ILoggerWriter { }

	/// <summary>将日志写入数据库中</summary>
	public interface IDbLoggerWriter : ILoggerWriter { }

	/// <summary>允许写入文件日志</summary>
	internal sealed class FileLoggerWriter : LoggerWriter
	{
		/// <summary>初始化 LoggerWriter 类实例</summary>
		internal FileLoggerWriter() : base(_FileStorage) { }
	}

	/// <summary>表示抽象的日志写入类</summary>
	public abstract class LoggerWriter : ILoggerWriter, IFileLoggerWriter
	{
		private static ILoggerWriter UpdateValue(string key, ILoggerWriter value) { return value; }

		private static readonly ConcurrentDictionary<string, ILoggerWriter> writers = new ConcurrentDictionary<string, ILoggerWriter>();

		/// <summary>获取缓存的日志写入器</summary>
		/// <param name="key">数据库连接名称</param>
		/// <param name="writer">日志写入器</param>
		/// <returns><!--true if the key was found in the <see cref="ConcurrentDictionary{TKey,TValue}"/>; otherwise, false--></returns>
		public static bool TryGetWriter(string key, out ILoggerWriter writer)
		{
			return writers.TryGetValue(key, out writer);
		}

		/// <summary>获取缓存的日志写入器</summary>
		/// <param name="key">数据库连接名称</param>
		/// <param name="writer">日志写入器</param>
		/// <returns><!--true if the key was found in the <see cref="ConcurrentDictionary{TKey,TValue}"/>; otherwise, false--></returns>
		public static ILoggerWriter AddOrUpdate(string key, ILoggerWriter writer)
		{
			return writers.AddOrUpdate(key, writer, UpdateValue);
		}

		/// <summary></summary>
		internal protected readonly ILoggerStorage _storage = null;
		internal readonly static ActionCollection _actions = new ActionCollection();
		internal readonly static EventLogsSection _EventLogs = EventLogsSection.DefaultSection;
		internal readonly static LocalFileStorage _FileStorage = new LocalFileStorage(_EventLogs);
		internal readonly static string _host = GetComputerAddress();

		/// <summary>初始化 LoggerWriter 类实例</summary>
		/// <param name="storage">日志存储器</param>
		protected LoggerWriter(ILoggerStorage storage) { _storage = storage; }

		/// <summary>根据条件查询日志记录</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="time1">日志记录时间开始</param>
		/// <param name="time2">日志记录时间结束</param>
		/// <param name="controller">控制器名称</param>
		/// <param name="action">操作名称</param>
		/// <param name="computer">操作电脑</param>
		/// <param name="user">操作用户名</param>
		/// <param name="msg">操作消息</param>
		/// <param name="levels">日志级别</param>
		/// <param name="results">操作结果</param>
		/// <returns></returns>
		public virtual Task<IPagination<LoggerEntity>> GetLoggingsAsync(Guid? batchNo, string controller, string action, string computer,
			string user, string msg, DateTime time1, DateTime time2, LogLevel[] levels = null, LogResult[] results = null)
		{
			return Task.FromResult<IPagination<LoggerEntity>>(new Pagination<LoggerEntity>());
		}

		/// <summary>根据条件查询日志记录</summary>
		/// <param name="condition">日志查询条件</param>
		/// <returns>返回日志查询结果</returns>
		public virtual Task<IPagination<LoggerEntity>> GetLoggingsAsync(LoggerCondition condition)
		{
			return Task.FromResult<IPagination<LoggerEntity>>(new Pagination<LoggerEntity>());
		}

		/// <summary>根据条件查询日志记录</summary>
		/// <param name="batchNo">日志批次号</param>
		/// <returns>返回日志查询结果</returns>
		public virtual Task<IPagination<LoggerEntity>> GetLoggingsAsync(Guid batchNo)
		{
			return Task.FromResult<IPagination<LoggerEntity>>(new Pagination<LoggerEntity>());
		}

		/// <summary>根据条件删除日志记录</summary>
		/// <param name="keys">需要删除的日志主键</param>
		/// <returns>返回日志查询结果</returns>
		public virtual Task<Result> DeleteAsync(Guid[] keys)
		{
			return Task.FromResult(Result.Success);
		}

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

		/// <summary>添加 Action映射。</summary>
		/// <param name="url">表示请求的路径。</param>
		/// <param name="controller">表示当前请求所属控制器、窗体名称</param>
		/// <param name="action">表示当前请求名称</param>
		public void AddAction(string url, string controller, string action)
		{
			if (url == null) { return; } else { url = url.ToLower(); }
			if (_actions.ContainsKey(url)) { _actions[url] = new ActionInfo(url, controller, action); }
			else { _actions.Add(new ActionInfo(url, controller, action)); }
		}

		/// <summary>系统请求配置数量</summary>
		public int ActionCount { get { return _actions.Count; } }

		/// <summary>系统是否已经存在请求配置</summary>
		public bool HasActions { get { return _actions.Count > 0; } }

		private static IFileLoggerWriter _logger;
		/// <summary>获取本地文件写入实例</summary>
		public static IFileLoggerWriter Writer
		{
			get
			{
				if (_logger == null) { _logger = new FileLoggerWriter(); }
				return _logger;
			}
		}

		/// <summary>读取配置文件信息</summary>
		/// <param name="logLevel">日志级别</param>
		/// <param name="saveType">日志保存类型</param>
		/// <param name="mailToList">邮件接收人列表</param>
		/// <param name="sendMail">是否需要发送邮件</param>
		/// <returns></returns>
		private static void GetSectionInfo(LogLevel logLevel, out LogSaveType saveType, out bool sendMail, out EventLogItemCollection mailToList)
		{
			EventLogElement eventLog = _EventLogs.Values.GetEventLog(logLevel);
			if (eventLog != null && eventLog.Enabled)
			{
				saveType = eventLog.SaveType;
				sendMail = eventLog.SendMail;
				mailToList = eventLog.Items;
			}
			else
			{
				saveType = LogSaveType.None;
				sendMail = false;
				mailToList = new EventLogItemCollection();
			}
		}

		#region 记录日志信息 - 异步
		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		private async Task LoggerAsync(Guid batchNo, string url, string host, string user, Exception ex)
		{
			if (url == null) { return; } else { url = url.ToLower(); }
			if (_actions.TryGetValue(url, out ActionInfo ai))
			{
				await LoggerAsync(batchNo, ai.Controller, ai.Action, host, user, ex);
			}
			else
			{
				await LoggerAsync(batchNo, url, url, host, user, ex);
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
		private async Task LoggerAsync(Guid batchNo, string url, string host, string user, string message, LogLevel logLevel, LogResult resultType)
		{
			if (url == null) { return; } else { url = url.ToLower(); }
			if (_actions.TryGetValue(url, out ActionInfo ai))
			{
				await LoggerAsync(batchNo, ai.Controller, ai.Action, host, user, message, logLevel, LogResult.Successful);
			}
			else
			{
				await LoggerAsync(batchNo, url, url, host, user, message, logLevel, LogResult.Successful);
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		private async Task LoggerAsync(Guid batchNo, string controller, string action, string host, string user, Exception ex)
		{
			try
			{
				GetSectionInfo(LogLevel.Error, out LogSaveType savetype, out bool sendMail, out EventLogItemCollection mailToList);
				if (savetype == LogSaveType.LocalFile || savetype == LogSaveType.Windows)
				{
					await _FileStorage.WriteLogAsync(batchNo, controller, action, host, user, ex);
				}
				else if (savetype == LogSaveType.DataBase && _storage != null)
				{
					await _storage.WriteLogAsync(batchNo, controller, action, host, user, ex);
				}
			}
			catch (Exception ex1)
			{
				await _FileStorage.WriteLogAsync(batchNo, controller, action, host, user, ex1);
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
		private async Task LoggerAsync(Guid batchNo, string controller, string action, string host, string user, string message, LogLevel logLevel, LogResult resultType)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(host) == true) { host = GetComputerAddress(); }
				GetSectionInfo(logLevel, out LogSaveType savetype, out bool sendMail, out EventLogItemCollection mailToList);
				if (savetype == LogSaveType.LocalFile || savetype == LogSaveType.Windows)
				{
					await _FileStorage.WriteLogAsync(batchNo, controller, action, host, user, message, logLevel, resultType);
				}
				else if (savetype == LogSaveType.DataBase && _storage != null)
				{
					await _storage.WriteLogAsync(batchNo, controller, action, host, user, message, logLevel, resultType);
				}
			}
			catch (Exception ex1)
			{
				await _FileStorage.WriteLogAsync(batchNo, controller, action, host, user, ex1);
			}
		}

		#endregion

		#region 日志信息事件 - 异步
		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		async Task ILoggerWriter.InformationAsync(string url, string user, string message)
		{
			await LoggerAsync(Guid.NewGuid(), url, _host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		async Task ILoggerWriter.InformationAsync(string url, string host, string user, string message)
		{
			await LoggerAsync(Guid.NewGuid(), url, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		async Task ILoggerWriter.InformationAsync(string controller, string action, string host, string user, string message)
		{
			await LoggerAsync(Guid.NewGuid(), controller, action, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		async Task ILoggerWriter.InformationAsync(Guid batchNo, string url, string user, string message)
		{
			await LoggerAsync(batchNo, url, _host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		async Task ILoggerWriter.InformationAsync(Guid batchNo, string url, string host, string user, string message)
		{
			await LoggerAsync(batchNo, url, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>记录操作成功的日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		async Task ILoggerWriter.InformationAsync(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			await LoggerAsync(batchNo, controller, action, host, user, message, LogLevel.Information, LogResult.Successful);
		}
		#endregion

		#region 错误日志记录 - 异步
		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		async Task ILoggerWriter.ErrorAsync(string url, string user, string message)
		{
			await LoggerAsync(Guid.NewGuid(), url, _host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		async Task ILoggerWriter.ErrorAsync(string url, string host, string user, string message)
		{
			await LoggerAsync(Guid.NewGuid(), url, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		async Task ILoggerWriter.ErrorAsync(string controller, string action, string host, string user, string message)
		{
			await LoggerAsync(Guid.NewGuid(), controller, action, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		async Task ILoggerWriter.ErrorAsync(Guid batchNo, string url, string user, string message)
		{
			await LoggerAsync(batchNo, url, _host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		async Task ILoggerWriter.ErrorAsync(Guid batchNo, string url, string host, string user, string message)
		{
			await LoggerAsync(batchNo, url, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作描述</param>
		async Task ILoggerWriter.ErrorAsync(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			await LoggerAsync(batchNo, controller, action, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		async Task ILoggerWriter.ErrorAsync(string url, string user, Exception ex)
		{
			await LoggerAsync(Guid.NewGuid(), url, _host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		async Task ILoggerWriter.ErrorAsync(string url, string host, string user, Exception ex)
		{
			await LoggerAsync(Guid.NewGuid(), url, host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		async Task ILoggerWriter.ErrorAsync(string controller, string action, string host, string user, Exception ex)
		{
			await LoggerAsync(Guid.NewGuid(), controller, action, host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		async Task ILoggerWriter.ErrorAsync(Guid batchNo, string url, string user, Exception ex)
		{
			await LoggerAsync(batchNo, url, _host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		async Task ILoggerWriter.ErrorAsync(Guid batchNo, string url, string host, string user, Exception ex)
		{
			await LoggerAsync(batchNo, url, host, user, ex);
		}

		/// <summary>记录系统异常的操作日志</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="host">当前操作的计算机名称或地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作异常</param>
		async Task ILoggerWriter.ErrorAsync(Guid batchNo, string controller, string action, string host, string user, Exception ex)
		{
			await LoggerAsync(batchNo, controller, action, host, user, ex);
		}
		#endregion

		#region 日志信息事件 - 同步
		void ILoggerWriter.Information(string url, string user, string message)
		{
			Logger(Guid.NewGuid(), url, _host, user, message, LogLevel.Information, LogResult.Successful);
		}

		void ILoggerWriter.Information(Guid batchNo, string url, string user, string message)
		{
			Logger(batchNo, url, _host, user, message, LogLevel.Information, LogResult.Successful);
		}

		void ILoggerWriter.Information(string url, string host, string user, string message)
		{
			Logger(Guid.NewGuid(), url, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		void ILoggerWriter.Information(Guid batchNo, string url, string host, string user, string message)
		{
			Logger(batchNo, url, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		void ILoggerWriter.Information(string controller, string action, string host, string user, string message)
		{
			Logger(Guid.NewGuid(), controller, action, host, user, message, LogLevel.Information, LogResult.Successful);
		}

		void ILoggerWriter.Information(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			Logger(batchNo, controller, action, host, user, message, LogLevel.Information, LogResult.Successful);
		}
		#endregion

		#region 日志警告事件 - 同步
		void ILoggerWriter.Warning(string url, string user, string message)
		{
			Logger(Guid.NewGuid(), url, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		void ILoggerWriter.Warning(Guid batchNo, string url, string user, string message)
		{
			Logger(batchNo, url, _host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		void ILoggerWriter.Warning(string url, string host, string user, string message)
		{
			Logger(Guid.NewGuid(), url, host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		void ILoggerWriter.Warning(Guid batchNo, string url, string host, string user, string message)
		{
			Logger(batchNo, url, host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		void ILoggerWriter.Warning(string controller, string action, string host, string user, string message)
		{
			Logger(Guid.NewGuid(), controller, action, host, user, message, LogLevel.Warning, LogResult.Successful);
		}

		void ILoggerWriter.Warning(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			Logger(batchNo, controller, action, host, user, message, LogLevel.Warning, LogResult.Successful);
		}
		#endregion

		#region 日志调试事件 - 同步
		void ILoggerWriter.Debug(string url, string user, string message)
		{
			Logger(Guid.NewGuid(), url, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		void ILoggerWriter.Debug(Guid batchNo, string url, string user, string message)
		{
			Logger(batchNo, url, _host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		void ILoggerWriter.Debug(string url, string host, string user, string message)
		{
			Logger(Guid.NewGuid(), url, host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		void ILoggerWriter.Debug(Guid batchNo, string url, string host, string user, string message)
		{
			Logger(batchNo, url, host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		void ILoggerWriter.Debug(string controller, string action, string host, string user, string message)
		{
			Logger(Guid.NewGuid(), controller, action, host, user, message, LogLevel.Debug, LogResult.Successful);
		}

		void ILoggerWriter.Debug(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			Logger(batchNo, controller, action, host, user, message, LogLevel.Debug, LogResult.Successful);
		}
		#endregion

		#region 日志错误事件 - 同步
		void ILoggerWriter.Error(string url, string user, string message)
		{
			Logger(Guid.NewGuid(), url, _host, user, message, LogLevel.Error, LogResult.Failed);
		}

		void ILoggerWriter.Error(Guid batchNo, string url, string user, string message)
		{
			Logger(batchNo, url, _host, user, message, LogLevel.Error, LogResult.Failed);
		}

		void ILoggerWriter.Error(string url, string host, string user, string message)
		{
			Logger(Guid.NewGuid(), url, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		void ILoggerWriter.Error(Guid batchNo, string url, string host, string user, string message)
		{
			Logger(batchNo, url, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		void ILoggerWriter.Error(string controller, string action, string host, string user, string message)
		{
			Logger(Guid.NewGuid(), controller, action, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		void ILoggerWriter.Error(Guid batchNo, string controller, string action, string host, string user, string message)
		{
			Logger(batchNo, controller, action, host, user, message, LogLevel.Error, LogResult.Failed);
		}

		void ILoggerWriter.Error(string url, string user, Exception ex)
		{
			Logger(Guid.NewGuid(), url, _host, user, ex);
		}

		void ILoggerWriter.Error(Guid batchNo, string url, string user, Exception ex)
		{
			Logger(batchNo, url, _host, user, ex);
		}

		void ILoggerWriter.Error(string url, string host, string user, Exception ex)
		{
			Logger(Guid.NewGuid(), url, host, user, ex);
		}

		void ILoggerWriter.Error(Guid batchNo, string url, string host, string user, Exception ex)
		{
			Logger(batchNo, url, host, user, ex);
		}

		void ILoggerWriter.Error(string controller, string action, string host, string user, Exception ex)
		{
			Logger(Guid.NewGuid(), controller, action, host, user, ex);
		}

		void ILoggerWriter.Error(Guid batchNo, string controller, string action, string host, string user, Exception ex)
		{
			Logger(batchNo, controller, action, host, user, ex);
		}
		#endregion

		#region 记录日志信息 - 异步
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
				Logger(batchNo, ai.Controller, ai.Action, host, user, message, logLevel, LogResult.Successful);
			}
			else
			{
				Logger(batchNo, url, url, host, user, message, logLevel, LogResult.Successful);
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
				GetSectionInfo(LogLevel.Error, out LogSaveType savetype, out bool sendMail, out EventLogItemCollection mailToList);
				if (savetype == LogSaveType.LocalFile || savetype == LogSaveType.Windows)
				{
					_FileStorage.WriteLog(batchNo, controller, action, host, user, ex);
				}
				else if (savetype == LogSaveType.DataBase && _storage != null)
				{
					_storage.WriteLog(batchNo, controller, action, host, user, ex);
				}
			}
			catch (Exception ex1)
			{
				_FileStorage.WriteLog(batchNo, controller, action, host, user, ex1);
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
				GetSectionInfo(logLevel, out LogSaveType savetype, out bool sendMail, out EventLogItemCollection mailToList);
				if (savetype == LogSaveType.LocalFile || savetype == LogSaveType.Windows)
				{
					_FileStorage.WriteLog(batchNo, controller, action, host, user, message, logLevel, resultType);
				}
				else if (savetype == LogSaveType.DataBase && _storage != null)
				{
					_storage.WriteLog(batchNo, controller, action, host, user, message, logLevel, resultType);
				}
			}
			catch (Exception ex1)
			{
				_FileStorage.WriteLog(batchNo, controller, action, host, user, ex1);
			}
		}
		#endregion
	}

}
