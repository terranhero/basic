using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Basic.Configuration
{
    /// <summary>
    /// 表示ConnectionElement配置节的集合
    /// </summary>
    public sealed class ConnectionCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 获取在派生的类中重写时用于标识配置文件中此元素集合的名称。
        /// </summary>
        /// <value>集合的名称；否则为空字符串。默认值为空字符串。</value>
        protected override string ElementName { get { return "basic.connection"; } }

        /// <summary>
        /// 向 ConnectionCollection 类集合中添加 ConnectionElement 类实例。
        /// </summary>
        /// <param name="element">需要添加的 ConnectionElement 类实例。</param>
        public void Add(ConnectionElement element)
        {
            base.BaseAdd(element, false);
        }

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
            return new ConnectionElement();
        }

        /// <summary>
        /// 获取指定配置元素的元素键。
        /// </summary>
        /// <param name="element">要为其返回键的 ConnectionElementCollection。</param>
        /// <returns>一个 System.Object，用作指定 ConnectionElementCollection.Name 的键。</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ConnectionElement).Name;
        }

        /// <summary>
        /// 根据配置元素关键字获取配置元素
        /// </summary>
        /// <param name="name">配置信息名称</param>
        /// <returns>返回具有指定键的连接信息。</returns>
        public ConnectionElement GetConnection(string name)
        {
            return base.BaseGet(name) as ConnectionElement;
        }
    }
}
