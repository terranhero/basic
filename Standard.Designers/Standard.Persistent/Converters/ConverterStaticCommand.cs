using Basic.Collections;
using Basic.Configuration;
using Basic.DataEntities;
using Basic.Designer;
using Basic.Enums;
using System;
using System.Collections.Specialized;
using System.Drawing.Design;
using System.Xml;
using System.Xml.Serialization;
using GBS = Basic.Collections;

namespace Basic.Converters
{
    /// <summary>
    /// 表示静态配置命令
    /// </summary>
    internal sealed class ConverterStaticCommand : ConverterDataCommand, IXmlSerializable
    {
        private readonly ConverterCheckCommandCollection _CheckCommands;
        private readonly ConverterNewCommandCollection _NewCommands;
        private string _SourceColumn = null;
        /// <summary>
        /// 初始化 ConverterStaticCommand 类实例
        /// </summary>
        internal ConverterStaticCommand(ConverterConfiguration converter)
            : base(converter)
        {
            _CheckCommands = new ConverterCheckCommandCollection(this);
            _NewCommands = new ConverterNewCommandCollection(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public ConverterNewCommand CreateNewCommand(ConverterStaticCommand owner)
        {
            ConverterNewCommand command = new ConverterNewCommand(owner);
           // command.CommandText = _CommandText.ToUpper();
            command.CommandText = _CommandText.Replace(" FROM ", " AS " + _SourceColumn + " FROM ");
            return command;
        }
        /// <summary>
        /// 获取检测命令集合
        /// </summary>
        public ConverterCheckCommandCollection CheckCommands { get { return _CheckCommands; } }

        /// <summary>
        /// 获取新值命令集合
        /// </summary>
        public ConverterNewCommandCollection NewCommands { get { return _NewCommands; } }

        private string _CommandText = null;
        /// <summary>
        /// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
        /// </summary>
        /// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
        public string CommandText { get { return _CommandText; } set { _CommandText = value; } }

        /// <summary>
        /// 获取当前节点元素名称
        /// </summary>
        protected internal override string ElementName { get { return StaticCommandElement.XmlElementName; } }

        /// <summary>
        /// 从对象的 XML 表示形式读取属性。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值</param>
        /// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
        protected internal override bool ReadAttribute(string name, string value)
        {
            if (name == "SourceColumn") { _SourceColumn = value; return true; }
            else if (name == "ReturnColumn") { _SourceColumn = value; return true; }
            return base.ReadAttribute(name, value);
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象扩展信息。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        /// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
        protected internal override bool ReadContent(System.Xml.XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == CheckedCommandCollection.XmlElementName)
            {
                _CheckCommands.ReadXml(reader.ReadSubtree());
            }
            else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == NewCommandCollection.XmlElementName)
            {
                _NewCommands.ReadXml(reader.ReadSubtree());
            }
            else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == StaticCommandElement.CommandTextElement)//兼容5.0新版结构信息
            {
                _CommandText = reader.ReadString();
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == StaticCommandElement.XmlElementName)
            {
                return true;
            }
            return base.ReadContent(reader);
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        protected internal override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            foreach (ConverterCheckCommand checkCommand in _CheckCommands)
            {
                checkCommand.ResetParameters();
            }
        }
        /// <summary>
        /// 将对象转换为其 XML 表示形式中属性部分。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        /// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
        protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
        {
            base.WriteAttribute(writer);
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal override void WriteContent(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString(StaticCommandElement.CommandTextElement, CommandText);
            if (_CheckCommands != null && _CheckCommands.Count > 0)
                _CheckCommands.WriteXml(writer);
            if (_NewCommands != null && _NewCommands.Count > 0)
                _NewCommands.WriteXml(writer);
            base.WriteContent(writer);
        }
    }
}
