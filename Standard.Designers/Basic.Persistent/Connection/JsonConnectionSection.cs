using System.Collections.Generic;
using System.Configuration;
using Basic.Enums;

namespace Basic.Configuration
{
    /// <summary>
    /// 配置文件中自定义数据库连接配置信息
    /// </summary>
    public sealed class JsonConnectionSection : Dictionary<string, string>
    {
        /// <summary>
        /// 初始化 JsonConnectionSection 类实例
        /// </summary>
        public JsonConnectionSection() { }

        /// <summary>自定义数据库信息名称，此名称集合中唯一。</summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据库连接类型
        /// </summary>
        public ConnectionType ConnectionType { get; set; }
    }

}
