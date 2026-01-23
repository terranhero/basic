using Basic.EntityLayer;
using Basic.Enums;

namespace Basic.Loggers
{
	/// <summary>
	/// 自定义日志配置信息。
	/// </summary>
	[global::Basic.EntityLayer.GroupNameAttribute("Logger")]
	public sealed class LoggerOptions
	{
		private static LoggerOptions _default = new LoggerOptions();

		/// <summary>日志配置信息</summary>
		public static LoggerOptions Default { get { return _default; } }

		/// <summary>
		/// 初始化 LoggerOptions 类实例
		/// </summary>
		private LoggerOptions() { }

		/// <summary>系统日志表</summary>
		[WebDisplayAttribute("Logger_TableName")]
		public string TableName { get; set; } = "SYS_EVENTLOGGER";

		/// <summary>文件型日志保存模式(按天/按周/按月)。</summary>
		[WebDisplayAttribute("Logger_Mode")]
		public CycleMode Mode { get; set; } = CycleMode.Weekly;

		/// <summary>消息日志保存方式</summary>
		[WebDisplayAttribute("Logger_LogLevel")]
		public LogLevelOption Information { get { return _information; } }
		private LogLevelOption _information = new LogLevelOption(LogLevel.Information, LogSaveType.DataBase, true);

		/// <summary>警告日志保存方式</summary>
		[WebDisplayAttribute("Logger_LogLevel")]
		public LogLevelOption Warning { get { return _warning; } }
		private LogLevelOption _warning = new LogLevelOption(LogLevel.Warning, LogSaveType.DataBase, true);

		/// <summary>错误日志保存方式</summary>
		[WebDisplayAttribute("Logger_LogLevel")]
		public LogLevelOption Error { get { return _error; } }
		private LogLevelOption _error = new LogLevelOption(LogLevel.Error, LogSaveType.DataBase, true);

		/// <summary>调试日志保存方式</summary>
		[WebDisplayAttribute("Logger_LogLevel")]
		public LogLevelOption Debug { get { return _debug; } }
		private LogLevelOption _debug = new LogLevelOption(LogLevel.Debug, LogSaveType.LocalFile, false);

		/// <summary>日志批处理大小，默认值200</summary>
		public int BatchSize { get; set; } = 200;

	}

	/// <summary>特定日志级别存储方式</summary>
	[System.Diagnostics.DebuggerDisplay("SaveType = {SaveType}, Enabled = {Enabled}")]
    [global::Basic.EntityLayer.GroupNameAttribute("Logger")]
    public sealed class LogLevelOption
	{
		/// <summary>
		/// 初始化 LogLevelOption 类实例。
		/// </summary>
		public LogLevelOption(LogLevel logLevel) : this(logLevel, LogSaveType.None, false) { }

		/// <summary>
		/// 使用指定参数初始化 LogLevelOption 类实例。 
		/// </summary>
		/// <param name="logLevel">日志级别</param>
		/// <param name="saveType">日志保存类型</param>
		/// <param name="enabled">是否有效</param>
		public LogLevelOption(LogLevel logLevel, LogSaveType saveType, bool enabled)
		{
			this.LogLevel = logLevel;
			this.SaveType = saveType;
			this.Enabled = enabled;
		}

		/// <summary>日志级别。</summary>
		[WebDisplayAttribute("Logger_LogLevel")]
		public LogLevel LogLevel { get; private set; }

		/// <summary>日志保存类型</summary>
		[WebDisplayAttribute("Logger_SaveType")]
		public LogSaveType SaveType { get; set; }

		/// <summary>
		/// 该级别日志配置信息是否有效。
		/// </summary>
		[WebDisplayAttribute("Logger_Enabled")]
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
