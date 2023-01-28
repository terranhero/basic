using System.Collections.Generic;
using Basic.Configuration;
using System.Collections.Specialized;
using Basic.Collections;

namespace Basic.Converters
{
    /// <summary>
    /// 表示执行数据库命令的集合
    /// </summary>
    internal sealed class ConverterDataCommandCollection : Basic.Collections.AbstractCollection<ConverterDataCommand>
    {
        private readonly ConverterConfiguration _Configuration;
        /// <summary>
        /// 初始化 ConverterDataCommandCollection 类的新实例。
        /// </summary>
        /// <param name="persistent">包含此类实例的 PersistentConfiguration 类实例。</param>
        /// <param name="prefix">Xml文档元素前缀。</param>
        /// <param name="elementns">Xml文档元素命名空间。</param>
        internal ConverterDataCommandCollection(ConverterConfiguration persistent, string prefix, string elementns)
            : base(persistent, prefix, elementns) { _Configuration = persistent; }

        /// <summary>
        /// 获取集合的键属性
        /// </summary>
        /// <param name="item">需要获取键的集合子元素</param>
        /// <returns>返回元素的键</returns>
        protected internal override string GetKey(ConverterDataCommand item) { return item.Name; }

        /// <summary>
        /// 获取当前节点元素名称
        /// </summary>
        protected internal override string ElementName
        {
            get { return DataCommandCollection.XmlElementName; }
        }

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
                else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == StaticCommandElement.XmlElementName)
                {
                    ConverterStaticCommand element = new ConverterStaticCommand(_Configuration);
                    element.ReadXml(reader.ReadSubtree());
                    this.Add(element);
                }
                else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == DynamicCommandElement.XmlElementName)
                {
                    ConverterDynamicCommand element = new ConverterDynamicCommand(_Configuration);
                    element.ReadXml(reader.ReadSubtree());
                    this.Add(element);
                }
                else if (reader.NodeType == System.Xml.XmlNodeType.EndElement && reader.LocalName == ElementName)
                    break;
            }
            return false;
        }
    }
}
