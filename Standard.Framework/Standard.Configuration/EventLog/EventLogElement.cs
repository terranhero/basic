using System.Configuration;
using Basic.Enums;

namespace Basic.Configuration
{
	/// <summary>
	/// 自定义日志配置信息。
	/// </summary>
	public sealed class EventLogElement : ConfigurationElement
	{
		/// <summary>
		/// 初始化 EventLogElement 类实例。
		/// </summary>
		public EventLogElement()
			: this(LogLevel.Information, LogSaveType.None, false, false) { }

		/// <summary>
		/// 使用指定参数初始化 EventLogElement 类实例。 
		/// </summary>
		/// <param name="logLevel">日志级别</param>
		/// <param name="saveType">日志保存类型</param>
		/// <param name="sendMail">是否需要发送Mail</param>
		/// <param name="enabled">是否有效</param>
		public EventLogElement(LogLevel logLevel, LogSaveType saveType, bool sendMail, bool enabled)
		{
			this.LogLevel = logLevel;
			this.SaveType = saveType;
			this.SendMail = sendMail;
			this.Enabled = enabled;
		}

		/// <summary>日志级别。</summary>
		[ConfigurationProperty("logLevel", IsRequired = true, IsKey = true)]
		public LogLevel LogLevel
		{
			get { return (LogLevel)this["logLevel"]; }
			set { this["logLevel"] = value; }
		}

		/// <summary>
		/// 日志保存类型
		/// </summary>
		[ConfigurationProperty("saveType", IsRequired = false)]
		public LogSaveType SaveType
		{
			get
			{
				return (LogSaveType)base["saveType"];
			}
			set
			{
				base["saveType"] = value;
			}
		}

		/// <summary>
		/// 该级别日志配置信息是否有效。
		/// </summary>
		[ConfigurationProperty("enabled", DefaultValue = false, IsRequired = false)]
		public bool Enabled
		{
			get
			{
				return (bool)base["enabled"];
			}
			set
			{
				base["enabled"] = value;
			}
		}

		/// <summary>
		/// 是否发送邮件。
		/// </summary>
		[ConfigurationProperty("sendMail", DefaultValue = false, IsRequired = false)]
		public bool SendMail
		{
			get
			{
				return (bool)base["sendMail"];
			}
			set
			{
				base["sendMail"] = value;
			}
		}

		/// <summary>
		/// 默认转换的多语言信息
		/// </summary>
		[ConfigurationProperty("", IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(EventLogItemCollection), AddItemName = "mailTo")]
		public EventLogItemCollection Items
		{
			get
			{
				return (EventLogItemCollection)base[""];
			}
		}
	}

	/// <summary>
	/// 表示 CultureElement 配置节的集合
	/// </summary>
	public sealed class EventLogCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// 获取在派生的类中重写时用于标识配置文件中此元素集合的名称。
		/// </summary>
		/// <value>集合的名称；否则为空字符串。默认值为空字符串。</value>
		protected override string ElementName { get { return "basic.eventLog"; } }

		/// <summary>
		/// 获取 ConnectionCollection 的类型
		/// </summary>
		/// <value>此集合的 System.Configuration.ConfigurationElementCollectionType</value>
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}

		/// <summary>
		/// 创建一个新的 ConnectionElementCollection类实例。
		/// </summary>
		/// <returns>新的 System.Configuration.ConfigurationElement子类实例。</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new EventLogElement();
		}

		/// <summary>
		/// 获取指定配置元素的元素键。
		/// </summary>
		/// <param name="element">要为其返回键的 ConnectionElementCollection。</param>
		/// <returns>一个 System.Object，用作指定 ConnectionElementCollection.Name 的键。</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return (element as EventLogElement).LogLevel;
		}

		/// <summary>
		/// 根据配置元素关键字获取配置元素
		/// </summary>
		/// <param name="logLevel">日志级别信息。</param>
		/// <returns>返回具有指定键的连接信息。</returns>
		public EventLogElement GetEventLog(LogLevel logLevel)
		{
			return base.BaseGet(logLevel) as EventLogElement;
		}
	}

}
