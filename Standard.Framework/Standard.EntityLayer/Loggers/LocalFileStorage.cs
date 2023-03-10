
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Basic.Configuration;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;

namespace Basic.LogInfo
{
	/// <summary>将日志写入本地文件中</summary>
	internal sealed class LocalFileStorage : ILoggerStorage
	{
		private const string _SplitLine = @"==================================================================================================================";
		private readonly string _RootDirectory;
		private readonly string _LogDirectory;
		private readonly CycleMode cycleMode;
		private readonly CultureInfo ciInfo = CultureInfo.CurrentCulture;

		/// <summary>
		/// 初始化LocalFileStorage类实例
		/// </summary>
		internal LocalFileStorage(EventLogsSection section)
		{
			cycleMode = section.Mode;
			_RootDirectory = AppDomain.CurrentDomain.BaseDirectory;
			_LogDirectory = string.Format("{0}\\Logs", _RootDirectory);
			if (!Directory.Exists(_LogDirectory)) { Directory.CreateDirectory(_LogDirectory); }
		}

		/// <summary>获取日志文件名称</summary>
		/// <returns></returns>
		private string GetFileName()
		{
			if (cycleMode == CycleMode.Daily) { return string.Format("{0}\\log_{1:yyyyMMdd}.log", _LogDirectory, DateTime.Today); }
			else if (cycleMode == CycleMode.Weekly)
			{
				int weeks = ciInfo.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
				if (weeks <= 9) { return string.Format("{0}\\log_{1}0{2}.log", _LogDirectory, DateTime.Today.Year, weeks); }
				return string.Format("{0}\\log_{1}{2}.log", _LogDirectory, DateTime.Today.Year, weeks);
			}
			else if (cycleMode == CycleMode.Monthly)
				return string.Format("{0}\\log_{1:yyyyMM}.log", _LogDirectory, DateTime.Today);
			return string.Format("{0}\\log_{1:yyyyMMdd}.log", _LogDirectory, DateTime.Today);
		}

		/// <summary>/// 记录日志信息/// </summary>
		/// <param name="batchNo">日志批次</param>
		/// <param name="controllerName">当前操作所属控制器、页面、窗体名称</param>
		/// <param name="actionName">当前操作名称</param>
		/// <param name="computerName">操作计算机名称或操作计算机地址</param>
		/// <param name="userName">当前操作用户</param>
		/// <param name="message">操作消息</param>
		/// <param name="logLevel">日志级别</param>
		/// <param name="resultType">操作结果</param>
		public async Task WriteLogAsync(System.Guid batchNo, string controllerName, string actionName, string computerName, string userName,
			string message, LogLevel logLevel, LogResult resultType)
		{
			string logFileName = GetFileName();
			using (System.IO.FileStream fileStream = new System.IO.FileStream(logFileName, FileMode.Append, FileAccess.Write))
			{
				using (StreamWriter writer = new StreamWriter(fileStream, Encoding.Unicode) { AutoFlush = true })
				{
					await writer.WriteAsync(string.Format("[Time:{0:yyyy-MM-dd HH:mm:ss.fff}]", DateTimeConverter.Now));
					await writer.WriteAsync(string.Format(",[{0}],[Controller:{1}],[Action:{2}]", logLevel.ToString("G"), controllerName, actionName));
					await writer.WriteLineAsync(string.Format(",[Computer:{0}],[User:{1}]", computerName, userName));
					await writer.WriteLineAsync(string.Format("Message:{0}", message));
					await writer.WriteLineAsync(_SplitLine);
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
		public async Task WriteLogAsync(Guid batchNo, string controllerName, string actionName, string computerName, string userName, System.Exception ex)
		{
			string logFileName = GetFileName();
			using (System.IO.FileStream fileStream = new System.IO.FileStream(logFileName, FileMode.Append, FileAccess.Write))
			{
				using (StreamWriter writer = new StreamWriter(fileStream, Encoding.Unicode) { AutoFlush = true })
				{
					await writer.WriteAsync(string.Format("[Time:{0:yyyy-MM-dd HH:mm:ss.fff}]", DateTimeConverter.Now));
					await writer.WriteAsync(string.Format(",[{0}],[Controller:{1}],[Action:{2}]", LogLevel.Error.ToString("G"), controllerName, actionName));
					await writer.WriteLineAsync(string.Format(",[Computer:{0}],[User:{1}]", computerName, userName));
					await writer.WriteLineAsync(string.Format("Exception.Message:{0}", ex.Message));
					await writer.WriteLineAsync(string.Format("Exception.Source:{0}", ex.Source));
					await writer.WriteLineAsync(string.Format("Exception.StackTrace:{0}", ex.StackTrace));
					if (ex.InnerException != null)
					{
						await writer.WriteLineAsync(string.Format("InnerException.Message:{0}", ex.InnerException.Message));
						await writer.WriteLineAsync(string.Format("InnerException.Source:{0}", ex.InnerException.Source));
						await writer.WriteLineAsync(string.Format("InnerException.StackTrace:{0}", ex.InnerException.StackTrace));
					}
					await writer.WriteLineAsync(_SplitLine);
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
