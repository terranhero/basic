using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Basic.Configuration
{
    /// <summary>
    /// 当前系统多语言转换器配置信息
    /// </summary>
    public sealed class MessagersSection : ConfigurationSection
    {
        /// <summary>
        /// Basic.eventLogs 配置节，多语言配置使用
        /// </summary>
        public const string ElementName = "basic.messagers";

        /// <summary>
        /// 初始化 MessagersSection 类实例
        /// </summary>
        public MessagersSection() { }

        /// <summary>
        /// 当前消息转换配置信息是否必须
        /// </summary>
        [ConfigurationProperty("defaultConverter", IsRequired = true)]
        public string DefaultConverter
        {
            get { return (string)this["defaultConverter"]; }
            set { this["defaultConverter"] = value; }
        }

        /// <summary>
        /// 默认转换的多语言信息
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(MessagerCollection), AddItemName = "add", ClearItemsName = "clear")]
        public MessagerCollection Values
        {
            get
            {
                return (MessagerCollection)base[""];
            }
        }
    }

}
