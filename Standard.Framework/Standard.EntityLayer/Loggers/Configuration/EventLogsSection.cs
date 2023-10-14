using System.Configuration;
using Basic.Enums;

namespace Basic.Configuration
{
	/// <summary>
	/// 自定义日志配置信息。
	/// </summary>
	public sealed class EventLogsSection : ConfigurationSection
	{
		static EventLogsSection()
		{
			string configName = ConfigurationGroup.ElementName;
			string secName = ElementName;
			object section = ConfigurationManager.GetSection(string.Concat(configName, "/", secName));
			if (section != null && section is EventLogsSection configurationSection)
			{
				_DefaultSection = configurationSection;
			}
		}

		private static EventLogsSection _DefaultSection;
		/// <summary>
		/// 日志配置信息
		/// </summary>
		internal static EventLogsSection DefaultSection
		{
			get { return _DefaultSection; }
		}

		/// <summary>
		/// Basic.eventLogs 配置节，多语言配置使用
		/// </summary>
		public const string ElementName = "basic.eventLogs";

		/// <summary>
		/// 初始化 EventLogsSection 类实例
		/// </summary>
		public EventLogsSection() { }

		/// <summary>
		/// 系统日志表
		/// </summary>
		[ConfigurationProperty("tableName", IsRequired = true, DefaultValue = "SYS_EVENTLOG")]
		public string TableName
		{
			get { return (string)this["tableName"]; }
			set { this["tableName"] = value; }
		}

		/// <summary>
		/// 文件型日志保存模式(按天/按周/按月)。
		/// </summary>
		[ConfigurationProperty("mode", IsRequired = true, DefaultValue = CycleMode.Daily)]
		public CycleMode Mode
		{
			get { return (CycleMode)this["mode"]; }
			set { this["mode"] = value; }
		}

		/// <summary>
		/// 数据库连接配置信息
		/// </summary>
		[ConfigurationProperty("", IsDefaultCollection = true), ConfigurationCollection(typeof(EventLogCollection))]
		public EventLogCollection Values
		{
			get
			{
				return (EventLogCollection)base[""];
			}
		}
	}

}
