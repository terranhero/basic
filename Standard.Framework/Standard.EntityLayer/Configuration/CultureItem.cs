using System;
using System.Configuration;

namespace Basic.Configuration
{
    /// <summary>
    /// 配置文件中自定义数据库配置节
    /// </summary>
    public sealed class CultureItem : ConfigurationElement
    {
        /// <summary>
        /// 初始化 ConnectionItemElement 类实例
        /// </summary>
        public CultureItem() { }

        /// <summary>
        /// 获取或设置格式为“&lt;languagecode2&gt;-&lt;country/regioncode2&gt;”的区域性名称。
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// 获取或设置当前 CultureInfo 的区域性标识符。
        /// </summary>
        [ConfigurationProperty("lcid", IsRequired = true, IsKey = true)]
        public string LCID
        {
            get { return (string)this["lcid"]; }
            set { this["lcid"] = value; }
        }
    }

    /// <summary>
    /// 表示 CultureItem 配置节的集合
    /// </summary>
    public sealed class CultureItemCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 初始化 CultureCollection 类实例
        /// </summary>
        public CultureItemCollection() : base(StringComparer.CurrentCultureIgnoreCase) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return base.BaseGet(name) != null;
        }
        /// <summary>
        /// 获取 ConnectionItemCollection 的类型
        /// </summary>
        /// <value>此集合的 System.Configuration.ConfigurationElementCollectionType</value>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }
        /// <summary>
        /// 创建一个新的 ConnectionElementCollection类实例。
        /// </summary>
        /// <returns>新的 System.Configuration.ConfigurationElement子类实例。</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new CultureItem();
        }

        /// <summary>
        /// 获取指定配置元素的元素键。
        /// </summary>
        /// <param name="element">要为其返回键的 ConnectionElementCollection。</param>
        /// <returns>一个 System.Object，用作指定 ConnectionElementCollection.Name 的键。</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as CultureItem).Name;
        }
    }
}
