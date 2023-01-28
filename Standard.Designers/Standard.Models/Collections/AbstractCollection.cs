using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using Basic.Designer;

namespace Basic.Collections
{
    /// <summary>
    /// 表示数据持久类文件集合类公共抽象基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractCollection<T> : Basic.Collections.BaseCollection<T>,
        IEnumerable<T>, INotifyCollectionChanged, IXmlSerializable where T : AbstractCustomTypeDescriptor
    {
        #region 实体定义字段
        private readonly string XmlElementNamespace;
        private readonly string XmlElementPrefix;
        #endregion

        private readonly AbstractCustomTypeDescriptor notifyObject;
        /// <summary>
        /// 初始化 AbstractCustomTypeDescriptor 类实例。
        /// </summary>
        protected AbstractCollection(AbstractCustomTypeDescriptor nofity) : this(nofity, null, null) { }

        /// <summary>
        /// 初始化 TableInfoElement 类实例
        /// </summary>
        /// <param name="nofity">包含此类实例的 PersistentConfiguration 类实例。</param>
        /// <param name="prefix">Xml文档元素前缀。</param>
        /// <param name="elementns">Xml文档元素命名空间。</param>
        protected AbstractCollection(AbstractCustomTypeDescriptor nofity, string prefix, string elementns)
            : base()
        {
            notifyObject = nofity;
            XmlElementNamespace = elementns;
            XmlElementPrefix = prefix;
        }

        /// <summary>
        /// 引发带有提供的参数的 BaseDictionary&lt;T&gt;.CollectionChanged 事件。
        /// </summary>
        /// <param name="e">要引发的事件的参数。</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            notifyObject.OnFileContentChanged(EventArgs.Empty);
            base.OnCollectionChanged(e);
        }

        #region 接口 IXmlSerializable 默认实现
        /// <summary>
        /// 获取当前节点元素名称
        /// </summary>
        protected internal abstract string ElementName { get; }

        /// <summary>
        /// 从对象的 XML 表示形式读取属性。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值</param>
        /// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
        protected internal virtual bool ReadAttribute(string name, string value) { return false; }

        /// <summary>
        /// 将对象转换为其 XML 表示形式中属性部分。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal virtual void WriteAttribute(System.Xml.XmlWriter writer) { }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象扩展信息。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        /// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
        protected internal abstract bool ReadChildContent(System.Xml.XmlReader reader);

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        protected internal virtual void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            if (reader.HasAttributes)
            {
                for (int index = 0; index < reader.AttributeCount; index++)
                {
                    reader.MoveToAttribute(index);
                    ReadAttribute(reader.LocalName, reader.GetAttribute(index));
                }
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
                else if (reader.NodeType == XmlNodeType.Element && ReadChildContent(reader.ReadSubtree())) { break; }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == ElementName) { break; }
            }
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(XmlElementPrefix) && !string.IsNullOrWhiteSpace(XmlElementNamespace))
                writer.WriteStartElement(XmlElementPrefix, ElementName, XmlElementNamespace);
            else
                writer.WriteStartElement(ElementName);
            WriteAttribute(writer);
            foreach (AbstractCustomTypeDescriptor child in this)
            {
                child.WriteXml(writer);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// 此方法是保留方法，请不要使用。在实现 IXmlSerializable 接口时，
        /// 应从此方法返回 null（在 Visual Basic 中为 Nothing），
        /// 如果需要指定自定义架构，应向该类应用 XmlSchemaProviderAttribute。
        /// </summary>
        /// <returns>XmlSchema，描述由 WriteXml 方法产生并由 ReadXml 方法使用的对象的 XML 表示形式。</returns>
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() { return null; }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) { ReadXml(reader); }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) { WriteXml(writer); }
        #endregion
    }
}
