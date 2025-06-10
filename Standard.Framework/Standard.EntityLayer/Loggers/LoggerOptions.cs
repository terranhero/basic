using System.Configuration;
using Basic.Enums;

namespace Basic.Configuration
{
	/// <summary>
	/// 自定义日志配置信息。
	/// </summary>
	public sealed class LoggerOptions
	{
		private static LoggerOptions _default = new LoggerOptions();

		/// <summary>日志配置信息</summary>
		internal static LoggerOptions Default { get { return _default; } }

		/// <summary>
		/// 初始化 LoggerOptions 类实例
		/// </summary>
		private LoggerOptions() { }

		/// <summary>系统日志表</summary>
		public string TableName { get; set; } = "SYS_EVENTLOGGER";

		/// <summary>文件型日志保存模式(按天/按周/按月)。</summary>
		public CycleMode Mode { get; set; } = CycleMode.Weekly;

		/// <summary>日志记录级别(仅记录小于等于此级别的日志)</summary>
		public LogLevel Level { get; set; } = LogLevel.Error;

		/// <summary>消息日志保存方式</summary>
		public LogLeveOption Information { get { return _information; } }
		private LogLeveOption _information = new LogLeveOption(LogLevel.Information, LogSaveType.DataBase, true);

		/// <summary>警告日志保存方式</summary>
		public LogLeveOption Warning { get { return _warning; } }
		private LogLeveOption _warning = new LogLeveOption(LogLevel.Warning, LogSaveType.DataBase, true);

		/// <summary>错误日志保存方式</summary>
		public LogLeveOption Error { get { return _error; } }
		private LogLeveOption _error = new LogLeveOption(LogLevel.Error, LogSaveType.DataBase, true);

		/// <summary>调试日志保存方式</summary>
		public LogLeveOption Debug { get { return _debug; } }
		private LogLeveOption _debug = new LogLeveOption(LogLevel.Debug, LogSaveType.LocalFile, false);
	}

	/// <summary>特定日志级别存储方式</summary>
	public sealed class LogLeveOption
	{
		/// <summary>
		/// 初始化 LogLeveOption 类实例。
		/// </summary>
		public LogLeveOption() : this(LogLevel.Information, LogSaveType.None, false) { }

		/// <summary>
		/// 使用指定参数初始化 LogLeveOption 类实例。 
		/// </summary>
		/// <param name="logLevel">日志级别</param>
		/// <param name="saveType">日志保存类型</param>
		/// <param name="enabled">是否有效</param>
		public LogLeveOption(LogLevel logLevel, LogSaveType saveType, bool enabled)
		{
			this.LogLevel = logLevel;
			this.SaveType = saveType;
			this.Enabled = enabled;
		}

		/// <summary>日志级别。</summary>
		public LogLevel LogLevel { get; set; }

		/// <summary>日志保存类型</summary>
		public LogSaveType SaveType { get; set; }

		/// <summary>
		/// 该级别日志配置信息是否有效。
		/// </summary>
		public bool Enabled { get; set; }

		///// <summary>
		///// 是否发送邮件。
		///// </summary>
		//[ConfigurationProperty("sendMail", DefaultValue = false, IsRequired = false)]
		//public bool SendMail { get; set; }

		///// <summary>默认邮箱</summary>
		//public string[] MailBoxs { get; set; }
	}
}
