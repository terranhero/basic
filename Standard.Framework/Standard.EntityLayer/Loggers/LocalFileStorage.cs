using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Basic.Collections;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;
using BackgroundWorker = System.ComponentModel.BackgroundWorker;
using DoWorkEventArgs = System.ComponentModel.DoWorkEventArgs;
using DoWorkEventHandler = System.ComponentModel.DoWorkEventHandler;

namespace Basic.Loggers
{
	/// <summary>将日志写入本地文件中</summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
	internal sealed class LocalFileStorage : ILoggerStorage
	{
		private const string _SplitLine = @"===========================================================================================================================================";
		private readonly string _RootDirectory;
		private readonly string _LogDirectory;
		private readonly CycleMode cycleMode;
		private readonly CultureInfo ciInfo = CultureInfo.CurrentCulture;
#if NET6_0_OR_GREATER
		private readonly FileStreamOptions options = new FileStreamOptions();
#endif
		/// <summary>
		/// 初始化LocalFileStorage类实例
		/// </summary>
		internal LocalFileStorage(CycleMode mode)
		{
			_worker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
			cycleMode = mode;
#if NET6_0_OR_GREATER
			options.Mode = FileMode.Append;
			options.Access = FileAccess.Write;
			options.Share = FileShare.Write;
			options.BufferSize = 4096;
			options.Options = FileOptions.None;
#endif
			_RootDirectory = AppDomain.CurrentDomain.BaseDirectory;
			_LogDirectory = string.Format("{0}\\Logs", _RootDirectory);
			if (!Directory.Exists(_LogDirectory)) { Directory.CreateDirectory(_LogDirectory); }
		}

		/// <summary>获取日志文件名称</summary>
		/// <returns></returns>
		private string GetFileName()
		{
			if (cycleMode == CycleMode.Daily) { return string.Format("{0}\\logger-{1:yyyyMMdd}.log", _LogDirectory, DateTime.Today); }
			else if (cycleMode == CycleMode.Weekly)
			{
				int weeks = ciInfo.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
				if (weeks <= 9) { return string.Format("{0}\\logger-{1}0{2}.log", _LogDirectory, DateTime.Today.Year, weeks); }
				return string.Format("{0}\\logger-{1}{2}.log", _LogDirectory, DateTime.Today.Year, weeks);
			}
			else if (cycleMode == CycleMode.Monthly)
				return string.Format("{0}\\logger-{1:yyyyMM}.log", _LogDirectory, DateTime.Today);
			return string.Format("{0}\\logger-{1:yyyyMMdd}.log", _LogDirectory, DateTime.Today);
		}

		private StreamWriter CreateStream()
		{
			string logFileName = GetFileName();
#if NET6_0_OR_GREATER
			return new StreamWriter(logFileName, Encoding.Unicode, options) { AutoFlush = true };
#else
			return new StreamWriter(logFileName, true, Encoding.Unicode) { AutoFlush = true };
#endif
		}

		#region 日志记录器 - 异步方法采用 ConcurrentQueue 和 SemaphoreSlim
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
		private bool _isProcessing = false; // 标记是否正在处理队列
		/// <summary>处理日志队列的异步方法</summary>
		private async Task ProcessQueueAsync(CancellationToken cancellationToken = default)
		{
			// 确保同时只有一个处理任务在运行
			if (!await _semaphore.WaitAsync(0)) { return; }

			try
			{
				if (_isProcessing || _loggers.Count == 0) { return; }
				_isProcessing = true;
				using (StreamWriter writer = CreateStream())
				{
					while (_loggers.TryDequeue(out LoggerEntity log) && log != null)
					{
						string logLevel = "info";
						if (log.LogLevel == LogLevel.Information) { logLevel = "info"; }
						else if (log.LogLevel == LogLevel.Error) { logLevel = "fail"; }
						else if (log.LogLevel == LogLevel.Warning) { logLevel = "warn"; }
						else if (log.LogLevel == LogLevel.Debug) { logLevel = "dbug"; }
						string info = string.Format("[Time: {0:yyyy-MM-dd HH:mm:ss.fff K}], [Level: {1}], [Controller: {2}], [Action: {3}], [Computer: {4}], [User: {5}]",
							log.OperationTime, logLevel, log.Controller, log.Action, log.Computer, log.UserName);
						await writer.WriteLineAsync(info);
						await writer.WriteLineAsync(string.Format("Message:{0}", log.Message));
						await writer.WriteLineAsync(_SplitLine);
					}
				}
			}
			catch (Exception ex)
			{
				using (StreamWriter writer = CreateStream())
				{
					string info = string.Format("[Time: {0:yyyy-MM-dd HH:mm:ss.fff K}], [Level: fail], [Controller: BackgroundWorker], [Action: DoWork], [Computer: localhost], [User: Worker]",
						DateTimeOffset.Now);
					await writer.WriteLineAsync(info);
					await writer.WriteLineAsync(string.Format("Message:{0}", ex.Message));
					await writer.WriteLineAsync(_SplitLine);
				}
			}
			finally
			{
				_isProcessing = false;
				_semaphore.Release();
			}
			// 如果还有剩余日志，继续处理
			if (_loggers.Count > 0)
			{
				await Task.Delay(50, cancellationToken);
				await ProcessQueueAsync();
			}
		}

		/// <summary>记录日志信息（支持异步环境）</summary>
		private void PushAsync(Guid batchNo, string controller, string action, string computer, string user, string message, LogLevel level, LogResult resultType)
		{
			// "SYS_EVENTLOGGER"的哈希值 = 6066941974602195857;
			Guid key = GuidGenerator.NewGuid(6066941974602195857L);
			_loggers.Enqueue(new LoggerEntity(key)
			{
				BatchNo = batchNo == Guid.Empty ? key : batchNo,
				Controller = controller,
				Action = action,
				Computer = computer,
				UserName = user,
				Message = message == null ? null : WebUtility.HtmlEncode(message),
				LogLevel = level,
				ResultType = resultType,
				OperationTime = DateTimeConverter.Now
			});

			// 触发处理，但不等待完成（fire and forget）
			_ = ProcessQueueAsync();
		}
		#endregion

#if NET6_0_OR_GREATER
		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="computer">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作消息</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		public ValueTask WriteAsync(System.Guid batchNo, string controller, string action, string computer, string user,
			string message, LogLevel logLevel, LogResult resultType)
		{
			PushAsync(batchNo, controller, action, computer, user, message, logLevel, resultType);
			return ValueTask.CompletedTask;
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="computer">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作失败后的异常信息</param>
		public ValueTask ErrorAsync(Guid batchNo, string controller, string action, string computer, string user, System.Exception ex)
		{
			string message = string.Concat(ex.Message, Environment.NewLine, ex.Source, Environment.NewLine, ex.StackTrace);
			PushAsync(batchNo, controller, action, computer, user, message, LogLevel.Error, LogResult.Failed);
			if (ex.InnerException != null)
			{
				message = string.Concat(ex.InnerException.Message, Environment.NewLine,
					ex.InnerException.Source, Environment.NewLine, ex.InnerException.StackTrace);
				PushAsync(batchNo, controller, action, computer, user, message, LogLevel.Error, LogResult.Failed);
			}
			return ValueTask.CompletedTask;
		}
#else
		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="computer">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="message">操作消息</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		public Task WriteAsync(System.Guid batchNo, string controller, string action, string computer, string user,
			string message, LogLevel logLevel, LogResult resultType)
		{
			PushAsync(batchNo, controller, action, computer, user, message, logLevel, resultType);
			return Task.CompletedTask;
		}
	
		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controller">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="action">当前操作名称</param>
		/// <param name="computer">操作计算机名称或操作计算机地址</param>
		/// <param name="user">当前操作用户</param>
		/// <param name="ex">操作失败后的异常信息</param>
		public Task ErrorAsync(Guid batchNo, string controller, string action, string computer, string user, System.Exception ex)
		{
			string message = string.Concat(ex.Message, Environment.NewLine, ex.Source, Environment.NewLine, ex.StackTrace);
			PushAsync(batchNo, controller, action, computer, user, message, LogLevel.Error, LogResult.Failed);
			if (ex.InnerException != null)
			{
				message = string.Concat(ex.InnerException.Message, Environment.NewLine,
					ex.InnerException.Source, Environment.NewLine, ex.InnerException.StackTrace);
				PushAsync(batchNo, controller, action, computer, user, message, LogLevel.Error, LogResult.Failed);
			}
			return Task.CompletedTask;
		}
#endif

		#region 日志记录器 - 同步方法采用 ConcurrentQueue 和 BackgroundWorker
		private readonly ConcurrentQueue<LoggerEntity> _loggers = new ConcurrentQueue<LoggerEntity>();
		private readonly BackgroundWorker _worker = new BackgroundWorker();
		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				if (_loggers.Count == 0) { return; }
				using (StreamWriter writer = CreateStream())
				{
					while (_loggers.TryDequeue(out LoggerEntity log) && log != null)
					{
						string logLevel = "info";
						if (log.LogLevel == LogLevel.Information) { logLevel = "info"; }
						else if (log.LogLevel == LogLevel.Error) { logLevel = "fail"; }
						else if (log.LogLevel == LogLevel.Warning) { logLevel = "warn"; }
						else if (log.LogLevel == LogLevel.Debug) { logLevel = "dbug"; }
						string info = string.Format("[Time: {0:yyyy-MM-dd HH:mm:ss.fff K}], [Level: {1}], [Controller: {2}], [Action: {3}], [Computer: {4}], [User: {5}]",
							log.OperationTime, logLevel, log.Controller, log.Action, log.Computer, log.UserName);
						await writer.WriteLineAsync(info);
						await writer.WriteLineAsync(string.Format("Message:{0}", log.Message));
						await writer.WriteLineAsync(_SplitLine);
					}
				}
			}
			catch (Exception ex)
			{
				using (StreamWriter writer = CreateStream())
				{
					string info = string.Format("[Time: {0:yyyy-MM-dd HH:mm:ss.fff K}], [Level: fail], [Controller: BackgroundWorker], [Action: DoWork], [Computer: localhost], [User: Worker]",
						DateTimeOffset.Now);
					await writer.WriteLineAsync(info);
					await writer.WriteLineAsync(string.Format("Message:{0}", ex.Message));
					await writer.WriteLineAsync(_SplitLine);
				}
			}
		}
		#endregion

		#region 日志记录器 - 同步方法采用 ConcurrentQueue 和 BackgroundWorker
		/// <summary>
		/// 记录日志信息
		/// </summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="computerName">操作计算机名称或操作计算机地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作消息</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		public void Write(System.Guid batchNo, string controllerName, string actionName, string computerName, string userName,
				string message, LogLevel logLevel, LogResult resultType)
		{
			_loggers.Enqueue(new LoggerEntity(Guid.Empty)
			{
				BatchNo = batchNo,
				Controller = controllerName,
				Action = actionName,
				Computer = computerName,
				UserName = userName,
				Message = message == null ? null : WebUtility.HtmlEncode(message),
				LogLevel = logLevel,
				ResultType = resultType,
				OperationTime = DateTimeConverter.Now
			});
			if (_worker.IsBusy == false) { _worker.RunWorkerAsync(); }
		}

		/// <summary>
		/// 记录日志信息
		/// </summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="computerName">操作计算机名称或操作计算机地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作失败后的异常信息</param>
		public void Error(System.Guid batchNo, string controllerName, string actionName, string computerName, string userName, System.Exception ex)
		{
			string message = string.Concat(ex.Message, Environment.NewLine, ex.Source, Environment.NewLine, ex.StackTrace);
			_loggers.Enqueue(new LoggerEntity(Guid.Empty)
			{
				BatchNo = batchNo,
				Controller = controllerName,
				Action = actionName,
				Computer = computerName,
				UserName = userName,
				Message = message == null ? null : WebUtility.HtmlEncode(message),
				LogLevel = LogLevel.Error,
				ResultType = LogResult.Failed,
				OperationTime = DateTimeConverter.Now
			});
			if (ex.InnerException != null)
			{
				message = string.Concat(ex.InnerException.Message, Environment.NewLine,
					ex.InnerException.Source, Environment.NewLine, ex.InnerException.StackTrace);

				_loggers.Enqueue(new LoggerEntity(Guid.Empty)
				{
					BatchNo = batchNo,
					Controller = controllerName,
					Action = actionName,
					Computer = computerName,
					UserName = userName,
					Message = message == null ? null : WebUtility.HtmlEncode(message),
					LogLevel = LogLevel.Error,
					ResultType = LogResult.Failed,
					OperationTime = DateTimeConverter.Now
				});
			}
			if (_worker.IsBusy == false) { _worker.RunWorkerAsync(); }
		}
		#endregion
	}
}
