using System.Configuration;

namespace Basic.Configuration
{
    /// <summary>
    /// 自定义多语言配置信息
    /// </summary>
    public sealed class CulturesSection : ConfigurationSection
    {
        /// <summary>
        /// Basic.cultures 配置节，多语言配置使用
        /// </summary>
        public const string ElementName = "basic.cultures";

        /// <summary>
        /// 初始化 CulturesSection 类实例
        /// </summary>
        public CulturesSection() { }

        /// <summary>
        /// 默认数据库连接配置名称
        /// </summary>
        [ConfigurationProperty("defaultName", IsRequired = true)]
        public string DefaultName
        {
            get { return (string)this["defaultName"]; }
            set { this["defaultName"] = value; }
        }

        /// <summary>
        /// 数据库连接配置信息
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true), ConfigurationCollection(typeof(CultureCollection))]
        public CultureCollection Cultures
        {
            get
            {
                return (CultureCollection)base[""];
            }
        }
    }

}
