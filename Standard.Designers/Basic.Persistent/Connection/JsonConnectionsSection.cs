using System.Collections.Generic;

namespace Basic.Configuration
{
    /// <summary>Json配置文件类</summary>
    internal sealed class JsonConnectionsSection
    {
        private readonly Dictionary<string, JsonConnectionSection> connections = new Dictionary<string, JsonConnectionSection>();
        /// <summary>默认数据库连接配置名称</summary>
        public string DefaultName { get; set; }

        /// <summary>
        /// 数据库连接配置信息
        /// </summary>
        public Dictionary<string, JsonConnectionSection> Connections { get { return connections; } }
    }
}
