using System.Collections.Generic;
using System.Collections.Specialized;
using Basic.Database;

namespace Basic.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UniqueConstraintCollection : AbstractCollection<UniqueConstraint>,
        ICollection<UniqueConstraint>, IEnumerable<UniqueConstraint>, INotifyCollectionChanged
    {
        /// <summary>
        /// 数据库表中所有列配置节名称
        /// </summary>
        internal const string XmlElementName = "UniqueConstraints";
        private readonly DesignTableInfo tableInfo;

        /// <summary>
        /// 初始化 TableColumnCollection 类的新实例。
        /// </summary>
        internal UniqueConstraintCollection(DesignTableInfo tie) : base(tie) { tableInfo = tie; }

        /// <summary>
        /// 使用带参构造函数，初始化UniqueConstraintInfo类实例
        /// </summary>
        /// <param name="uniqueName">Unique 约束名称</param>
        /// <param name="columns">Unique 约束列</param>
        /// <param name="pkConstraint">是否为主键约束</param>
        public UniqueConstraint Create(string uniqueName, DesignColumnInfo[] columns, bool pkConstraint)
        {
            return new UniqueConstraint(tableInfo, uniqueName, columns, pkConstraint);
        }


        /// <summary>
        /// 获取集合的键属性
        /// </summary>
        /// <param name="item">需要获取键的集合子元素</param>
        /// <returns>返回元素的键</returns>
        protected internal override string GetKey(UniqueConstraint item) { return item.Name; }

        /// <summary>
        /// 获取当前节点元素名称
        /// </summary>
        protected internal override string ElementName { get { return XmlElementName; } }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象扩展信息。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        /// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
        protected internal override bool ReadChildContent(System.Xml.XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == System.Xml.XmlNodeType.Whitespace) { continue; }
                else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == UniqueConstraint.XmlElementName)
                {
                    UniqueConstraint element = new UniqueConstraint(tableInfo, tableInfo.Columns, false);
                    element.ReadXml(reader);
                    base.Add(element);
                }
            }
            return false;
        }
    }
}
