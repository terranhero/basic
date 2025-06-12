
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Basic.Collections;
using Basic.Configuration;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;

namespace Basic.Loggers
{
	/// <summary>将日志写入本地文件中</summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
	internal sealed class LocalFileStorage : ILoggerStorage
	{
		private const string _SplitLine = @"==================================================================================================================";
		private const string _SplitChar = "=";
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
			if (cycleMode == CycleMode.Daily) { return string.Format("{0}\\logger{1:yyyyMMdd}.log", _LogDirectory, DateTime.Today); }
			else if (cycleMode == CycleMode.Weekly)
			{
				int weeks = ciInfo.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
				if (weeks <= 9) { return string.Format("{0}\\logger{1}0{2}.log", _LogDirectory, DateTime.Today.Year, weeks); }
				return string.Format("{0}\\logger{1}{2}.log", _LogDirectory, DateTime.Today.Year, weeks);
			}
			else if (cycleMode == CycleMode.Monthly)
				return string.Format("{0}\\logger{1:yyyyMM}.log", _LogDirectory, DateTime.Today);
			return string.Format("{0}\\logger{1:yyyyMMdd}.log", _LogDirectory, DateTime.Today);
		}

		/// <summary>根据条件查询日志记录</summary>
		/// <param name="condition">查询条件</param>
		/// <returns>返回日志查询结果</returns>
		public Task<IPagination<LoggerEntity>> GetLoggingsAsync(LoggerCondition condition)
		{
			return Task.FromResult<IPagination<LoggerEntity>>(new Pagination<LoggerEntity>());
		}

		/// <summary>根据条件删除日志记录</summary>
		/// <param name="keys">需要删除的日志主键</param>
		/// <returns>返回日志查询结果</returns>
		public Task<Result> DeleteAsync(Guid[] keys)
		{
			return Task.FromResult(Result.Success);
		}

		/// <summary>
		/// 异步清除此流的所有缓冲区，并将任何缓冲数据写入底层设备
		/// </summary>
		/// <returns>表示异步刷新操作的任务</returns>
		public Task FlushAsync() { return Task.CompletedTask; }

		/// <summary>
		/// 异步清除此流的所有缓冲区，并将任何缓冲数据写入底层设备
		/// </summary>
		/// <param name="cancellationToken">
		/// 用于监视取消请求的 
		/// <see cref="System.Threading.CancellationToken">System.Threading.CancellationToken</see>
		/// </param>
		/// <returns>表示异步刷新操作的任务</returns>
		public Task FlushAsync(CancellationToken cancellationToken) { return Task.CompletedTask; }

		/// <summary>记录日志信息</summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="computerName">操作计算机名称或操作计算机地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作消息</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		public async Task WriteAsync(System.Guid batchNo, string controllerName, string actionName, string computerName, string userName,
			string message, LogLevel logLevel, LogResult resultType)
		{
			string logFileName = GetFileName();
#if NET6_0_OR_GREATER
			using (StreamWriter writer = new StreamWriter(logFileName, Encoding.Unicode, options) { AutoFlush = true })
#else
			using (StreamWriter writer = new StreamWriter(logFileName, true, Encoding.Unicode) { AutoFlush = true })
#endif
			{
				string info = string.Format("[Time: {0:yyyy-MM-dd HH:mm:ss.fff K}], [Level: {1}], [Controller: {2}], [Action: {3}], [Computer: {4}], [User: {5}]",
					DateTimeOffset.Now, logLevel.ToString("G"), controllerName, actionName, computerName, userName);
				await writer.WriteLineAsync(info);
				await writer.WriteLineAsync(string.Format("Message:{0}", message));
				await writer.WriteLineAsync(_SplitChar.PadRight(info.Length - 1, '='));
			}
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
		public async Task WriteAsync(Guid batchNo, string controllerName, string actionName, string computerName, string userName, System.Exception ex)
		{
			string logFileName = GetFileName();
#if NET6_0_OR_GREATER
			using (StreamWriter writer = new StreamWriter(logFileName, Encoding.Unicode, options) { AutoFlush = true })
#else
			using (StreamWriter writer = new StreamWriter(logFileName, true, Encoding.Unicode) { AutoFlush = true })
#endif
			{
				string info = string.Format("[Time: {0:yyyy-MM-dd HH:mm:ss.fff K}], [Level: {1}], [Controller: {2}], [Action: {3}], [Computer: {4}], [User: {5}]",
					DateTimeOffset.Now, LogLevel.Error.ToString("G"), controllerName, actionName, computerName, userName);
				await writer.WriteLineAsync(info);
				await writer.WriteLineAsync(string.Format("Exception.Message:{0}", ex.Message));
				await writer.WriteLineAsync(string.Format("Exception.Source:{0}", ex.Source));
				await writer.WriteLineAsync(string.Format("Exception.StackTrace:{0}", ex.StackTrace));
				if (ex.InnerException != null)
				{
					await writer.WriteLineAsync(string.Format("InnerException.Message:{0}", ex.InnerException.Message));
					await writer.WriteLineAsync(string.Format("InnerException.Source:{0}", ex.InnerException.Source));
					await writer.WriteLineAsync(string.Format("InnerException.StackTrace:{0}", ex.InnerException.StackTrace));
				}
				await writer.WriteLineAsync(_SplitChar.PadRight(info.Length - 1, '='));
			}
		}

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
		public void WriteLog(System.Guid batchNo, string controllerName, string actionName, string computerName, string userName,
				string message, LogLevel logLevel, LogResult resultType)
		{
			string logFileName = GetFileName();
			using (System.IO.FileStream fileStream = new System.IO.FileStream(logFileName, FileMode.Append, FileAccess.Write))
			{
				using (StreamWriter writer = new StreamWriter(fileStream, Encoding.Unicode) { AutoFlush = true })
				{
					writer.Write("[Time:{0:yyyy-MM-dd HH:mm:ss.fff}]", DateTimeConverter.Now);
					writer.Write(",[{0}],[Controller:{1}],[Action:{2}]", logLevel.ToString("G"), controllerName, actionName);
					writer.WriteLine(",[Computer:{0}],[User:{1}]", computerName, userName);
					writer.WriteLine("Message:{0}", message);
					writer.WriteLine(_SplitLine);
				}
			}
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
		public void WriteLog(System.Guid batchNo, string controllerName, string actionName, string computerName, string userName, System.Exception ex)
		{
			string logFileName = GetFileName();
			using (System.IO.FileStream fileStream = new System.IO.FileStream(logFileName, FileMode.Append, FileAccess.Write))
			{
				using (StreamWriter writer = new StreamWriter(fileStream, Encoding.Unicode) { AutoFlush = true })
				{
					writer.Write("[Time:{0:yyyy-MM-dd HH:mm:ss.fff}]", DateTimeConverter.Now);
					writer.Write(",[{0}],[Controller:{1}],[Action:{2}]", LogLevel.Error.ToString("G"), controllerName, actionName);
					writer.WriteLine(",[Computer:{0}],[User:{1}]", computerName, userName);
					writer.WriteLine("Exception.Message:{0}", ex.Message);
					writer.WriteLine("Exception.Source:{0}", ex.Source);
					writer.WriteLine("Exception.StackTrace:{0}", ex.StackTrace);
					if (ex.InnerException != null)
					{
						writer.WriteLine("InnerException.Message:{0}", ex.InnerException.Message);
						writer.WriteLine("InnerException.Source:{0}", ex.InnerException.Source);
						writer.WriteLine("InnerException.StackTrace:{0}", ex.InnerException.StackTrace);
					}
					writer.WriteLine(_SplitLine);
				}
			}
		}
	}
}
