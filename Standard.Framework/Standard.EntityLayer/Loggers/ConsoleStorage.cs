using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;
using BackgroundWorker = System.ComponentModel.BackgroundWorker;
using DoWorkEventArgs = System.ComponentModel.DoWorkEventArgs;
using DoWorkEventHandler = System.ComponentModel.DoWorkEventHandler;

namespace Basic.Loggers
{
	/// <summary>将日志写入本地文件中</summary>
	internal sealed class ConsoleStorage : ILoggerStorage
	{
		private readonly TextWriter writer = Console.Out;
		/// <summary>
		/// ConsoleStorage
		/// </summary>
		internal ConsoleStorage()
		{
			_worker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
		}

		#region 日志队列
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
				while (_loggers.TryDequeue(out LoggerEntity model) && model != null)
				{
					if (model.LogLevel == LogLevel.Information)
					{
						Console.ForegroundColor = ConsoleColor.DarkGreen;
						await writer.WriteAsync("info: "); Console.ResetColor();
					}
					else if (model.LogLevel == LogLevel.Error)
					{
						Console.ForegroundColor = ConsoleColor.DarkRed;
						await writer.WriteAsync("fail: "); Console.ResetColor();
					}
					else if (model.LogLevel == LogLevel.Warning)
					{
						Console.ForegroundColor = ConsoleColor.Yellow;
						await writer.WriteAsync("warn: "); Console.ResetColor();
					}
					else if (model.LogLevel == LogLevel.Debug)
					{
						await writer.WriteAsync("dbug: ");
					}

					await writer.WriteLineAsync(string.Format("Controller: {0}, Action: {1}, Time: {2:yyyy-MM-dd HH:mm:ss.fff K}", model.Controller, model.Action, model.OperationTime));
					await writer.WriteAsync("      "); await writer.WriteLineAsync(model.Message);
				}
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.DarkRed;
				await writer.WriteAsync("fail: "); Console.ResetColor();
				await writer.WriteLineAsync(string.Format("Controller: BackgroundWorker, Action: DoWork, Time: {0:yyyy-MM-dd HH:mm:ss.fff K}", DateTimeOffset.Now));
				await writer.WriteAsync("      "); await writer.WriteLineAsync(ex.Message);
				await writer.WriteAsync("      "); await writer.WriteLineAsync(ex.Source);
				await writer.WriteAsync("      "); await writer.WriteLineAsync(ex.StackTrace);
			}
		}
		#endregion

#if NET6_0_OR_GREATER
		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="computerName">操作计算机名称或操作计算机地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作消息</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		public ValueTask WriteAsync(System.Guid batchNo, string controllerName, string actionName, string computerName, string userName,
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
			return ValueTask.CompletedTask;
		}

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="computerName">操作计算机名称或操作计算机地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="ex">操作失败后的异常信息</param>
		public ValueTask ErrorAsync(Guid batchNo, string controllerName, string actionName, string computerName, string userName, System.Exception ex)
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
			return ValueTask.CompletedTask;
		}
#else
		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="computerName">操作计算机名称或操作计算机地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作消息</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		public Task WriteAsync(System.Guid batchNo, string controllerName, string actionName, string computerName, string userName,
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
			return Task.CompletedTask;
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
		public Task ErrorAsync(Guid batchNo, string controllerName, string actionName, string computerName, string userName, System.Exception ex)
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
			return Task.CompletedTask;
		}
#endif
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
	}
}
