using System.Configuration;

namespace Basic.Configuration
{
	/// <summary>
	/// 表示ConnectionElement配置节的集合
	/// </summary>
	public sealed class ConnectionItemCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// 获取 ConnectionItemCollection 的类型
		/// </summary>
		/// <value>此集合的 System.Configuration.ConfigurationElementCollectionType</value>
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

        /// <summary>
        /// 向 ConnectionItemCollection 类集合中添加 ConnectionItem 类实例。
        /// </summary>
        /// <param name="element">需要添加的 ConnectionItem 类实例。</param>
        public void Add(ConnectionItem element)
        {
            base.BaseAdd(element, false);
        }

		/// <summary>
		/// 创建一个新的 ConnectionElementCollection类实例。
		/// </summary>
		/// <returns>新的 System.Configuration.ConfigurationElement子类实例。</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ConnectionItem();
		}

		/// <summary>
		/// 获取指定配置元素的元素键。
		/// </summary>
		/// <param name="element">要为其返回键的 ConnectionElementCollection。</param>
		/// <returns>一个 System.Object，用作指定 ConnectionElementCollection.Name 的键。</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return (element as ConnectionItem).Name;
		}
	}

}
