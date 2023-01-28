using Basic.Database;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Basic.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TransactColumnCollection : BaseCollection<TransactColumnInfo>, INotifyCollectionChanged
    {
        private readonly TransactTableCollection m_TransactTables;
        /// <summary>
        /// 使用是否成功解析作为参数，初始化 TransactTableCollection 类实例。
        /// </summary>
        /// <param name="collection">拥有此集合的 TransactTableCollection 类实例。</param>
        internal TransactColumnCollection(TransactTableCollection collection)
        {
            m_TransactTables = collection;
        }

        /// <summary>
        /// 获取集合的键属性
        /// </summary>
        /// <param name="item">需要获取键的集合子元素</param>
        /// <returns>返回元素的键</returns>
        protected internal override string GetKey(TransactColumnInfo item) { return item.Name; }
    }
}
