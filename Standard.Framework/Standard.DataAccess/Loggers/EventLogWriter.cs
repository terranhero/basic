
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using Basic.Collections;
using Basic.Configuration;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;

namespace Basic.LogInfo
{
	/// <summary>
	/// 日志代理类，提供开发用户调用日志服务
	/// </summary>
	public static class EventLogWriter
	{
		private static readonly ConcurrentQueue<EventLogEntity> _EventEntities = new ConcurrentQueue<EventLogEntity>();
		/// <summary>系统每一秒钟确认一次日志队列，如果有数据则进行数据持久化操作</summary>
		private static System.Timers.Timer _EventTimer = new System.Timers.Timer(1000);

		private readonly static EventLogsSection _EventLogs = EventLogsSection.DefaultSection;
		private readonly static DataBaseStorage _DbStorage = new DataBaseStorage(_EventLogs);
		private readonly static LocalFileStorage _FileStorage = new LocalFileStorage(_EventLogs);
		private readonly static ActionCollection _Actions = new ActionCollection();
		private readonly static string _HostName;

		/// <summary>
		/// 初始化日志代理类
		/// </summary>
		static EventLogWriter()
		{
			_EventTimer.AutoReset = true;
			_EventTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnEventTimerElapsed);
			_HostName = GetComputerAddress();
		}
		private static async void OnEventTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (_EventEntities.IsEmpty) { _EventTimer.Stop(); return; }
			while (_EventEntities.TryDequeue(out EventLogEntity log))
			{
				try
				{
					GetSectionInfo(log.LogLevel, out LogSaveType savetype, out bool sendMail, out EventLogItemCollection mailToList);
					if (savetype == LogSaveType.LocalFile || savetype == LogSaveType.Windows)
					{
						await _FileStorage.WriteAsync(log.BatchNo, log.Controller, log.Action, log.Computer,
						 log.UserName, log.Message, log.LogLevel, log.ResultType);
					}
					else if (savetype == LogSaveType.DataBase)
					{
						await _DbStorage.WriteAsync(log.BatchNo, log.Controller, log.Action, log.Computer,
						   log.UserName, log.Message, log.LogLevel, log.ResultType);
					}

				}
				catch (Exception ex)
				{
					try
					{
						GetSectionInfo(LogLevel.Error, out LogSaveType savetype, out bool sendMail, out EventLogItemCollection mailToList);
						if (savetype == LogSaveType.LocalFile || savetype == LogSaveType.Windows)
						{
							await _FileStorage.WriteAsync(log.BatchNo, log.Controller, log.Action, log.Computer, log.UserName, ex);
						}
						else if (savetype == LogSaveType.DataBase)
						{
							await _DbStorage.WriteAsync(log.BatchNo, log.Controller, log.Action, log.Computer, log.UserName, ex);
						}
					}
					catch (Exception ex1)
					{
						await _FileStorage.WriteAsync(log.BatchNo, log.Controller, log.Action, log.Computer, log.UserName, ex1);
					}
				}
			}
		}

		/// <summary>
		/// 添加 Action映射。
		/// </summary>
		/// <param name="url">表示请求的路径。</param>
		/// <param name="controller">表示当前请求所属控制器、窗体名称</param>
		/// <param name="action">表示当前请求名称</param>
		public static void AddAction(string url, string controller, string action)
		{
			_Actions.Add(new ActionInfo(url, controller, action));
		}

		/// <summary>
		/// 添加 Action 映射。
		/// </summary>
		/// <param name="actions">ActionInfo 实例数组。</param>
		public static void AddActions(ActionInfo[] actions)
		{
			_Actions.AddRange(actions);
		}

		#region 记录信息日志的方法(信息/错误)
		private static void Information(string url, string userName, string message)
		{
			url = url != null ? url.ToLower() : url;
			if (_Actions.TryGetValue(url, out ActionInfo ai))
			{
				WriteLogging(Guid.NewGuid(), ai.Controller, ai.Action, _HostName, userName, message, LogLevel.Information, LogResult.Successful);
			}
			else
			{
				WriteLogging(Guid.NewGuid(), url, url, _HostName, userName, message, LogLevel.Information, LogResult.Successful);
			}
		}

		private static void Error(string action, string user, string msg)
		{
			EventLogWriter.WriteLog(Guid.NewGuid(), "Program", action, user, msg, LogLevel.Error, LogResult.Failed);
		}

		private static void Error(string action, string user, Exception ex)
		{
			//EventLogWriter.WriteLog(Guid.NewGuid(), "Program", action, user, ex);
		}
		#endregion

		#region 记录Web日志方法
		/// <summary>
		/// 记录操作成功的日志，LogResult值为Successful
		/// </summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		public static void WriteLogging(Guid batchNo, string url, string userName, string message, LogLevel logLevel)
		{
			url = url != null ? url.ToLower() : url;
			if (_Actions.TryGetValue(url, out ActionInfo ai))
			{
				WriteLogging(batchNo, ai.Controller, ai.Action, _HostName, userName, message, logLevel, LogResult.Successful);
			}
			else
			{
				WriteLogging(batchNo, url, url, _HostName, userName, message, logLevel, LogResult.Successful);
			}
		}

		/// <summary>
		/// 记录操作成功的信息日志，LogResult值为Successful
		/// </summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		public static void WriteLogging(Guid batchNo, string url, string userName, string message)
		{
			url = url != null ? url.ToLower() : url;
			if (_Actions.TryGetValue(url, out ActionInfo ai))
			{
				WriteLogging(batchNo, ai.Controller, ai.Action, _HostName, userName, message, LogLevel.Information, LogResult.Successful);
			}
			else
			{
				WriteLogging(batchNo, url, url, _HostName, userName, message, LogLevel.Information, LogResult.Successful);
			}
		}

		/// <summary>
		/// 记录日志信息
		/// </summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		public static void WriteLogging(Guid batchNo, string url, string userName, string message, LogLevel logLevel, LogResult resultType)
		{
			url = url != null ? url.ToLower() : url;
			if (_Actions.TryGetValue(url, out ActionInfo ai))
			{
				WriteLogging(batchNo, ai.Controller, ai.Action, _HostName, userName, message, logLevel, resultType);
			}
			else
			{
				WriteLogging(batchNo, url, url, _HostName, userName, message, logLevel, resultType);
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前请求全路径</param>
		/// <param name="hostName">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		public static void WriteLogging(Guid batchNo, string url, string hostName, string userName, string message, LogLevel logLevel, LogResult resultType)
		{
			url = url != null ? url.ToLower() : url;
			if (_Actions.TryGetValue(url, out ActionInfo ai))
			{
				WriteLogging(batchNo, ai.Controller, ai.Action, hostName, userName, message, logLevel, resultType);
			}
			else
			{
				WriteLogging(batchNo, url, url, hostName, userName, message, logLevel, resultType);
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前请求全路径</param>
		/// <param name="action">当前请求全路径</param>
		/// <param name="hostName">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		public static void WriteLogging(Guid batchNo, string controller, string action, string hostName, string userName, string message, LogLevel logLevel, LogResult resultType)
		{
			EventLogEntity entity = new EventLogEntity()
			{
				GuidKey = EntityLayer.GuidConverter.NewGuid,
				BatchNo = batchNo,
				Controller = controller,
				Action = action,
				Computer = hostName,
				UserName = userName,
				Message = message,
				LogLevel = logLevel,
				ResultType = resultType,
				OperationTime = EntityLayer.DateTimeConverter.Now
			};
			_EventEntities.Enqueue(entity); if (_EventTimer.Enabled == false) { _EventTimer.Start(); }
		}
		#endregion

		#region  本机日志记录方法
		/// <summary>
		/// 记录操作成功的信息日志，LogResult值为Successful
		/// </summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		public static void WriteLog(Guid batchNo, string controllerName, string actionName, string userName, string message)
		{
			WriteLogging(batchNo, controllerName, actionName, _HostName, userName, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>
		/// 记录操作成功的日志，LogResult值为Successful
		/// </summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		public static void WriteLog(Guid batchNo, string controllerName, string actionName, string userName,
			string message, LogLevel logLevel)
		{
			WriteLogging(batchNo, controllerName, actionName, _HostName, userName, message, logLevel, LogResult.Successful);
		}

		/// <summary>
		/// 记录日志信息
		/// </summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		public static void WriteLog(Guid batchNo, string controllerName, string actionName, string userName,
			string message, LogLevel logLevel, LogResult resultType)
		{
			WriteLogging(batchNo, controllerName, actionName, _HostName, userName,
			   message, logLevel, resultType);
		}

		/// <summary>
		/// 记录操作成功的日志，LogResult值为Successful
		/// </summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="hostName">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		/// <param name="logLevel">日志级别</param>
		public static void WriteLog(Guid batchNo, string controllerName, string actionName, string hostName, string userName,
			string message, LogLevel logLevel)
		{
			WriteLogging(batchNo, controllerName, actionName, hostName, userName, message, logLevel, LogResult.Successful);
		}

		/// <summary>
		/// 记录操作成功的信息日志，LogResult值为Successful
		/// </summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="hostName">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作描述</param>
		public static void WriteLog(Guid batchNo, string controllerName, string actionName, string hostName, string userName, string message)
		{
			WriteLogging(batchNo, controllerName, actionName, hostName, userName, message, LogLevel.Information, LogResult.Successful);
		}

		/// <summary>
		/// 记录日志信息
		/// </summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="hostName">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作消息</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		public static void WriteLog(Guid batchNo, string controllerName, string actionName, string hostName, string userName,
			string message, LogLevel logLevel, LogResult resultType)
		{
			WriteLogging(batchNo, controllerName, actionName, hostName, userName, message, logLevel, resultType);
		}
		#endregion

		/// <summary>
		/// 读取配置文件信息
		/// </summary>
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

		/// <summary>
		/// 获取Web程序客户端IP地址或Windows程序本机IP地址
		/// </summary>
		/// <returns></returns>
		private static string GetComputerAddress()
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

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="url">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="hostName">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作失败后的异常信息</param>
		public static void WriteLogging(Guid batchNo, string url, string hostName, string userName, Exception ex)
		{
			url = url != null ? url.ToLower() : url;
			if (_Actions.TryGetValue(url, out ActionInfo ai))
			{
				WriteLogging(batchNo, ai.Controller, ai.Action, hostName, userName, ex.Message, LogLevel.Error, LogResult.Failed);
				if (ex.InnerException != null)
				{
					WriteLogging(batchNo, ai.Controller, ai.Action, hostName, userName, ex.InnerException.Message, LogLevel.Error, LogResult.Failed);
				}
			}
			else
			{
				WriteLogging(batchNo, url, url, hostName, userName, ex.Message, LogLevel.Error, LogResult.Failed);
				if (ex.InnerException != null)
				{
					WriteLogging(batchNo, url, url, hostName, userName, ex.InnerException.Message, LogLevel.Error, LogResult.Failed);
				}
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作失败后的异常信息</param>
		public static void Error(Guid batchNo, string controllerName, string actionName, string userName, Exception ex)
		{
			WriteLogging(batchNo, controllerName, actionName, _HostName, userName, ex.Message, LogLevel.Error, LogResult.Failed);
			if (ex.InnerException != null)
			{
				WriteLogging(batchNo, controllerName, actionName, _HostName, userName, ex.InnerException.Message, LogLevel.Error, LogResult.Failed);
			}
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="hostName">当前操作的计算机名称或地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作失败后的异常信息</param>
		public static void Error(Guid batchNo, string controllerName, string actionName, string hostName, string userName, Exception ex)
		{
			WriteLogging(batchNo, controllerName, actionName, hostName, userName, ex.Message, LogLevel.Error, LogResult.Failed);
			if (ex.InnerException != null)
			{
				WriteLogging(batchNo, controllerName, actionName, hostName, userName, ex.InnerException.Message, LogLevel.Error, LogResult.Failed);
			}
		}

		/// <summary>
		/// 根据条件查询日志记录
		/// </summary>
		/// <param name="eventLogEntity">需要查询的日志实体</param>
		/// <param name="condition">查询条件</param>
		public static void SearchEventLog(EventLogTable eventLogEntity, EventLogCondition condition)
		{
			_DbStorage.SearchEventLog(eventLogEntity, condition);
		}

		/// <summary>
		/// 根据条件查询日志记录
		/// </summary>
		/// <param name="listEventLog">查询的日志信息列表。</param>
		/// <param name="condition">查询条件</param>
		public static void SearchEventLog(Pagination<EventLogEntity> listEventLog, EventLogCondition condition)
		{
			_DbStorage.SearchEventLog(listEventLog, condition);
		}

		/// <summary>
		/// 根据条件查询日志记录
		/// </summary>
		/// <param name="condition">查询条件</param>
		public static IPagination<EventLogEntity> SearchEventLog(EventLogCondition condition)
		{
			return _DbStorage.SearchEventLog(condition);
		}

		/// <summary>
		/// 根据条件查询日志记录
		/// </summary>
		/// <param name="entityArray">需要删除的日志信息(只需要属性GUIDKEY)</param>
		public static Result DeleteEventLog(EventLogDelEntity[] entityArray)
		{
			return _DbStorage.Delete(entityArray);
		}
	}
}
