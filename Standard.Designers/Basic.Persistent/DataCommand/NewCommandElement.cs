using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Basic.Enums;
using Basic.Designer;
using System.Drawing.Design;
using Basic.Properties;
using System.Data;
using System.ComponentModel;

namespace Basic.Configuration
{
    /// <summary>
    /// 表示抽象配置命令
    /// </summary>
    [System.ComponentModel.DisplayNameAttribute(XmlElementName)]
    [Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryNewCommand)]
    [Basic.Designer.PersistentDescription("PersistentDescription_NewCommand")]
    public sealed class NewCommandElement : AbstractCommandElement, IXmlSerializable
    {
        #region 节点元素名称
        /// <summary>
        /// 表示Xml元素名称
        /// </summary>
        internal const string XmlElementName = "NewValue";
        /// <summary>
        /// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。（元素名）
        /// </summary>
        internal const string CommandTextElement = "CommandText";
        /// <summary>
        /// 表示Xml节点的元素名称 “NewType”。
        /// </summary>
        internal const string NewTypeAttribute = "NewType";
        #endregion

        /// <summary>
        /// 返回此组件实例的类名。
        /// </summary>
        /// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
        public override string GetClassName()
        {
            return GetType().Name.Replace("Element", "");
        }

        /// <summary>
        /// 返回此组件实例的名称。
        /// </summary>
        /// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
        public override string GetComponentName()
        {
            return XmlElementName;
        }

        private readonly StaticCommandElement StaticCommand;
        /// <summary>
        /// 初始化 NewCommandElement 类实例。
        /// </summary>
        /// <param name="staticCommand">拥有此检测命令的静态命令结构</param>
        internal NewCommandElement(StaticCommandElement staticCommand)
            : base(staticCommand)
        {
            StaticCommand = staticCommand;
        }

        private ExecutedStatusEnum _NewType = ExecutedStatusEnum.EveryTime;
        /// <summary>
        /// 获取或设置要当前新值命令执行的方式。
        /// </summary>
        [System.ComponentModel.Description("获取或设置要当前新值命令执行的方式")]
        [System.ComponentModel.DefaultValue(typeof(ExecutedStatusEnum), "EveryTime")]
        [Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDataCommand)]
        public ExecutedStatusEnum NewType
        {
            get { return _NewType; }
            set
            {
                if (_NewType != value)
                {
                    _NewType = value;
                    base.RaisePropertyChanged("NewType");
                }
            }
        }

        private string _CommandText = null;
        /// <summary>
        /// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
        /// </summary>
        /// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
        [System.ComponentModel.Description("获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程")]
        [System.ComponentModel.DefaultValue(""), System.ComponentModel.Editor(typeof(CommandTextEditor), typeof(UITypeEditor))]
        [Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDataCommand)]
        public string CommandText
        {
            get { return _CommandText; }
            set
            {
                if (_CommandText != value)
                {
                    _CommandText = value;
                    base.RaisePropertyChanged("CommandText");
                }
            }
        }

        /// <summary>
        /// 返回表示当前 Basic.Configuration.NewCommandElement 的 System.String。
        /// </summary>
        /// <returns>System.String，表示当前的 Basic.Configuration.NewCommandElement。</returns>
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return typeof(NewCommandElement).Name;
            return Name;
        }

        /// <summary>
        /// 获取当前节点元素名称
        /// </summary>
        protected internal override string ElementName { get { return XmlElementName; } }

        /// <summary>
        /// 从对象的 XML 表示形式读取属性。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值</param>
        /// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
        protected internal override bool ReadAttribute(string name, string value)
        {
            if (name == NewTypeAttribute) { return Enum.TryParse<ExecutedStatusEnum>(value, true, out _NewType); }
            return base.ReadAttribute(name, value);
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        /// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
        protected internal override bool ReadContent(System.Xml.XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == CommandTextElement)//兼容5.0新版结构信息
            {
                _CommandText = reader.ReadString();
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == ElementName)
            {
                return true;
            }
            return base.ReadContent(reader);
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式中属性部分。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal override void WriteAttribute(XmlWriter writer)
        {
            base.WriteAttribute(writer);
            if (_NewType != ExecutedStatusEnum.EveryTime)
                writer.WriteAttributeString(NewTypeAttribute, _NewType.ToString());
        }
        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        /// <param name="connectionType">表示数据库连接类型</param>
        protected internal override void WriteContent(System.Xml.XmlWriter writer)
        {
			writer.WriteStartElement(CommandTextElement);
			writer.WriteCData(CommandText);//写CData
			writer.WriteEndElement();
            base.WriteContent(writer);
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        /// <param name="connectionType">表示数据库连接类型</param>
        protected internal override void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType)
        {
            writer.WriteStartElement(XmlElementName);
            if (CommandType != System.Data.CommandType.Text)
                writer.WriteAttributeString(CommandTypeAttribute, CommandType.ToString());
            if (CommandTimeout != 30)
                writer.WriteAttributeString(CommandTimeoutAttribute, CommandTimeout.ToString());
            if (_NewType != ExecutedStatusEnum.EveryTime)
                writer.WriteAttributeString(NewTypeAttribute, _NewType.ToString());

			writer.WriteStartElement(CommandTextElement);
			writer.WriteCData(CreateCommandText(_CommandText, connectionType));//写CData
			writer.WriteEndElement();
            base.GenerateConfiguration(writer, connectionType);
            writer.WriteEndElement();
        }
    }
}
