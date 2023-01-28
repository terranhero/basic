using Basic.Configuration;
using Basic.Designer;
using Basic.Enums;
using System;
using System.Data;
using System.Xml;
using System.Xml.Serialization;

namespace Basic.Converters
{
    /// <summary>
    /// 表示命令参数
    /// </summary>
    internal sealed class ConverterParameter : AbstractCustomTypeDescriptor, IXmlSerializable
    {
        private readonly AbstractConverterCommand _Command;
        internal AbstractConverterCommand Command { get { return _Command; } }
        /// <summary>
        /// 初始化 ConverterParameter 类实例。
        /// </summary>
        /// <param name="command">拥有此参数的 AbstractConverterCommand 类命令实例。</param>
        internal ConverterParameter(AbstractConverterCommand command)
            : base() { _Command = command; }

        /// <summary>
        /// 获取当前节点元素命名空间
        /// </summary>
        protected internal override string ElementNamespace { get { return null; } }

        /// <summary>
        /// 返回此组件实例的类名。
        /// </summary>
        /// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
        public override string GetClassName() { return CommandParameter.XmlElementName; }

        /// <summary>
        /// 返回此组件实例的名称。
        /// </summary>
        /// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
        public override string GetComponentName() { return Name; }

        private string _Name = null;
        /// <summary>
        /// 获取或设置参数名称
        /// </summary>
        public string Name { get { return _Name; } set { _Name = value; } }

        private string _SourceColumn = string.Empty;
        /// <summary>
        /// 获取或设置源列的名称
        /// </summary>
        public string SourceColumn { get { return _SourceColumn; } set { _SourceColumn = value; } }

        private DbTypeEnum _ParameterType = DbTypeEnum.NVarChar;
        /// <summary>
        /// 获取或设置一个值，该值指示参数数据库参数的类型。
        /// </summary>
        public DbTypeEnum ParameterType { get { return _ParameterType; } set { _ParameterType = value; } }

        private bool _Nullable = false;
        /// <summary>
        /// 获取或设置一个值，该值指示参数是否接受空值
        /// </summary>
        public bool Nullable { get { return _Nullable; } set { _Nullable = value; } }

        private int _Size = 0;
        /// <summary>
        /// 获取或设置列中数据的最大大小（以字节为单位）。
        /// </summary>
        public int Size { get { return _Size; } set { _Size = value; } }

        private int _Precision = 0;
        /// <summary>
        /// 获取或设置列中数据的最大大小（以字节为单位）。
        /// </summary>
        public int Precision { get { return _Precision; } set { _Precision = value; } }

        private byte _Scale = 0;
        /// <summary>
        /// 获取或设置数据库参数值解析为的小数位数
        /// </summary>
        public byte Scale { get { return _Scale; } set { _Scale = value; } }

        private ParameterDirection _Direction = ParameterDirection.Input;
        /// <summary>
        /// 获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。
        /// </summary>
        public ParameterDirection Direction { get { return _Direction; } set { _Direction = value; } }

        /// <summary>
        /// 获取当前节点元素名称
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        protected internal override string ElementName { get { return CommandParameter.XmlElementName; } }

        /// <summary>
        /// 从对象的 XML 表示形式读取属性。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值</param>
        /// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
        protected internal override bool ReadAttribute(string name, string value)
        {
            if (name == CommandParameter.ColumnAttribute) { _SourceColumn = value; return true; }
            else if (name == "SourceColumn") { _SourceColumn = value; return true; }
            else if (name == CommandParameter.ParameterTypeAttribute) { return Enum.TryParse<DbTypeEnum>(value, true, out _ParameterType); }
            else if (name == CommandParameter.SizeAttribute) { _Size = Convert.ToInt32(value); return true; }
            else if (name == CommandParameter.PrecisionAttribute) { _Precision = Convert.ToInt32(value); return true; }
            else if (name == CommandParameter.ScaleAttribute) { _Scale = Convert.ToByte(value); return true; }
            else if (name == CommandParameter.NullableAttribute) { _Nullable = Convert.ToBoolean(value); return true; }
            else if (name == CommandParameter.DirectionAttribute) { return Enum.TryParse<ParameterDirection>(value, true, out _Direction); }
            return false;
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象扩展信息。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        protected internal override bool ReadContent(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Text && reader.Name == string.Empty)
            {
                Name = reader.ReadString();
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == ElementName)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式中属性部分。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString(CommandParameter.ColumnAttribute, SourceColumn);
            writer.WriteAttributeString(CommandParameter.ParameterTypeAttribute, ParameterType.ToString());
            if (_Size > 0)
                writer.WriteAttributeString(CommandParameter.SizeAttribute, Convert.ToString(_Size));
            if (_Precision > 0 && _ParameterType == DbTypeEnum.Decimal)
                writer.WriteAttributeString(CommandParameter.PrecisionAttribute, Convert.ToString(_Precision));
            if (_Scale > 0 && _ParameterType == DbTypeEnum.Decimal)
                writer.WriteAttributeString(CommandParameter.ScaleAttribute, Convert.ToString(_Scale));
            if (_Nullable)
                writer.WriteAttributeString(CommandParameter.NullableAttribute, "true");
            if (Direction != ParameterDirection.Input)
                writer.WriteAttributeString(CommandParameter.DirectionAttribute, Direction.ToString());
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        /// <param name="connectionType">表示数据库连接类型</param>
        protected internal override void WriteContent(System.Xml.XmlWriter writer)
        {
            writer.WriteString(Name);
        }
    }
}
