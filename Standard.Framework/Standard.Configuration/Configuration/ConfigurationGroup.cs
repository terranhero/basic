using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Basic.Configuration
{
    /// <summary>
    /// 表示基础框架中的元素组(Basic.Configuration)
    /// </summary>
    public sealed class ConfigurationGroup : ConfigurationSectionGroup
    {
        /// <summary>
        /// 基础开发框架根配置组名称
        /// </summary>
        public const string ElementName = "basic.configuration";

        private ConnectionsSection connectionsSection = null;

        /// <summary>
        /// 初始化 ConfigurationGroup 类实例。
        /// </summary>
        public ConfigurationGroup() { }

        /// <summary>
        /// 数据库连接字符串配置信息
        /// </summary>
        internal ConnectionsSection ConnectionsSection
        {
            get
            {
                if (connectionsSection == null)
                    connectionsSection = base.Sections[ConnectionsSection.ElementName] as ConnectionsSection;
                return connectionsSection;
            }
        }
    }
}
