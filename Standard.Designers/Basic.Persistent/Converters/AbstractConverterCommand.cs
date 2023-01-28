using System;
using System.Collections.Specialized;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using Basic.Collections;
using Basic.Designer;
using Basic.Enums;
using Basic.Properties;
using Basic.Configuration;
using System.Text;

namespace Basic.Converters
{
    /// <summary>
    /// 表示抽象配置命令
    /// </summary>
    internal abstract class AbstractConverterCommand : AbstractCustomTypeDescriptor
    {
        /// <summary>
        /// 返回此组件实例的类名。
        /// </summary>
        /// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
        public override string GetClassName() { return ElementName; }

        /// <summary>
        /// 返回此组件实例的名称。
        /// </summary>
        /// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
        public override string GetComponentName() { return Name; }

        /// <summary>
        /// 获取当前节点元素命名空间
        /// </summary>
        protected internal override string ElementNamespace { get { return null; } }

        private readonly ConverterParameterCollection _Parameters;
        /// <summary>
        /// 初始化 AbstractConverterCommand 类实例。
        /// </summary>
        protected AbstractConverterCommand(AbstractCustomTypeDescriptor nofity)
            : base(nofity)
        {
            _Parameters = new ConverterParameterCollection(this);
        }

        /// <summary>
        /// 当前命令是否需要写入Name属性。
        /// </summary>
        protected virtual bool WriteName { get { return true; } }

        private string _Name = null;
        /// <summary>
        /// 获取或设置命令名称
        /// </summary>
        public virtual string Name { get { return _Name; } set { _Name = value; } }

        private CommandType _CommandType = CommandType.Text;
        /// <summary>
        /// 获取或设置一个值，该值指示如何解释 CommandText 属性。
        /// </summary>
        /// <value>CommandType 值之一，默认值为 Text。</value>
        public CommandType CommandType { get { return _CommandType; } set { _CommandType = value; } }

        private int _CommandTimeout = 30;
        /// <summary>
        /// 获取或设置在终止执行命令的尝试并生成错误之前的等待时间。
        /// </summary>
        /// <value>等待命令执行的时间（以秒为单位）,默认为 30 秒。</value>
        public virtual int CommandTimeout { get { return _CommandTimeout; } set { _CommandTimeout = value; } }

        /// <summary>
        /// 获取命令参数集合
        /// </summary>
        public ConverterParameterCollection Parameters { get { return _Parameters; } }

        /// <summary>
        /// 从对象的 XML 表示形式读取属性。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值</param>
        /// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
        protected internal override bool ReadAttribute(string name, string value)
        {
            if (name == AbstractCommandElement.NameAttribute) { _Name = value; return true; }
            else if (name == AbstractCommandElement.CommandTypeAttribute)
            {
                return Enum.TryParse<CommandType>(value, true, out _CommandType);
            }
            else if (name == AbstractCommandElement.CommandTimeoutAttribute)
            {
                CommandTimeout = Convert.ToInt32(value); return true;
            }
            return false;
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式中属性部分。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
        {
            if (WriteName) { writer.WriteAttributeString(AbstractCommandElement.NameAttribute, _Name); }
            if (_CommandType != System.Data.CommandType.Text)
                writer.WriteAttributeString(AbstractCommandElement.CommandTypeAttribute, CommandType.ToString());
            if (_CommandTimeout != 30)
                writer.WriteAttributeString(AbstractCommandElement.CommandTimeoutAttribute, _CommandTimeout.ToString());
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象扩展信息。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        /// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
        internal protected override bool ReadContent(System.Xml.XmlReader reader)
        {
         if (reader.NodeType == XmlNodeType.Element && reader.LocalName == AbstractCommandElement.ParametersElement)
            {
                _Parameters.ReadXml(reader.ReadSubtree());
            }
            return false;
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal override void WriteContent(System.Xml.XmlWriter writer)
        {
            if (_Parameters.Count == 0) { return; }
            writer.WriteStartElement(AbstractCommandElement.ParametersElement);
            foreach (ConverterParameter param in _Parameters)
            {
                param.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
    }
}
