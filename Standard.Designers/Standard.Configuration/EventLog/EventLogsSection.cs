using System.Configuration;

namespace Basic.Configuration
{
    /// <summary>
    /// 自定义日志配置信息。
    /// </summary>
    public sealed class EventLogsSection : ConfigurationSection
    {
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
