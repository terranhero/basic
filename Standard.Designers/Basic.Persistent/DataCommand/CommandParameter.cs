using System;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using Basic.Designer;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Database;
using System.ComponentModel;

namespace Basic.Configuration
{
    /// <summary>
    /// 表示命令参数
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [System.ComponentModel.DisplayNameAttribute(XmlElementName)]
    [System.ComponentModel.TypeConverter(typeof(CommandParameterConverter))]
    [Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryParameter)]
    [Basic.Designer.PersistentDescription("PersistentDescription_Parameter")]
    public sealed class CommandParameter : AbstractCustomTypeDescriptor, IXmlSerializable
    {
        private readonly AbstractCommandElement _Command;
        internal const string XmlElementName = "Parameter";
        internal const string ColumnAttribute = "Column";
        internal const string ParameterTypeAttribute = "DbType";
        internal const string SizeAttribute = "Size";
        internal const string PrecisionAttribute = "Precision";
        internal const string ScaleAttribute = "Scale";
        internal const string NullableAttribute = "Nullable";
        internal const string DirectionAttribute = "Direction";
        internal AbstractCommandElement Command { get { return _Command; } }
        /// <summary>
        /// 初始化 CommandParameter 类实例。
        /// </summary>
        /// <param name="command">拥有此参数的 AbstractCommandElement 类命令实例。</param>
        internal CommandParameter(AbstractCommandElement command)
            : base() { _Command = command; }

        /// <summary>
        /// 获取当前节点元素命名空间
        /// </summary>
        protected internal override string ElementNamespace { get { return null; } }

        /// <summary>
        /// 返回此组件实例的类名。
        /// </summary>
        /// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
        public override string GetClassName() { return XmlElementName; }

        /// <summary>
        /// 返回此组件实例的名称。
        /// </summary>
        /// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
        public override string GetComponentName() { return Name; }

        private string _Name = string.Empty;
        /// <summary>
        /// 获取或设置数据库参数的名称
        /// </summary>
        [System.ComponentModel.DefaultValue(""), TypeConverter(typeof(ColumnNameConverter))]
        [System.ComponentModel.Bindable(true), PersistentCategoryAttribute("PersistentCategory_Content")]
        [Basic.Designer.PersistentDescription("PropertyDescription_ParameterName")]
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    base.OnPropertyChanging("Name");
                    _Name = value;
                    base.RaisePropertyChanged("Name");
                }
            }
        }

        private string _SourceColumn = string.Empty;
        /// <summary>
        /// 获取或设置源列的名称
        /// </summary>
        [System.ComponentModel.DefaultValue(""), TypeConverter(typeof(ColumnNameConverter))]
        [System.ComponentModel.Bindable(true), PersistentCategoryAttribute("PersistentCategory_Content")]
        [Basic.Designer.PersistentDescription("PropertyDescription_SourceColumn")]
        public string SourceColumn
        {
            get { return _SourceColumn; }
            set
            {
                if (_SourceColumn != value)
                {
                    base.OnPropertyChanging("SourceColumn");
                    _SourceColumn = value;
                    base.RaisePropertyChanged("SourceColumn");
                }
            }
        }

        private DbTypeEnum _ParameterType = DbTypeEnum.NVarChar;
        /// <summary>
        /// 获取或设置一个值，该值指示参数数据库参数的类型。
        /// </summary>
        [System.ComponentModel.DefaultValue(typeof(DbTypeEnum), "NVarChar")]
        [System.ComponentModel.Bindable(true), PersistentCategoryAttribute("PersistentCategory_Content")]
        [Basic.Designer.PersistentDescription("PropertyDescription_ParameterType")]
        public DbTypeEnum ParameterType
        {
            get { return _ParameterType; }
            set
            {
                if (_ParameterType != value)
                {
                    base.OnPropertyChanging("ParameterType");
                    _ParameterType = value;
                    base.RaisePropertyChanged("ParameterType");
                }
            }
        }

        /// <summary>
        /// 获取 ParameterType 类型对应的.Net基元类型
        /// </summary>
        internal Type Type { get { return DbBuilderHelper.DbTypeToNetType(_ParameterType); } }

        private bool _Nullable = false;
        /// <summary>
        /// 获取或设置一个值，该值指示参数是否接受空值
        /// </summary>
        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.Bindable(true), PersistentCategoryAttribute("PersistentCategory_Content")]
        [Basic.Designer.PersistentDescription("PropertyDescription_Nullable")]
        public bool Nullable
        {
            get { return _Nullable; }
            set
            {
                if (_Nullable != value)
                {
                    base.OnPropertyChanging("Nullable");
                    _Nullable = value;
                    base.RaisePropertyChanged("Nullable");
                }
            }
        }

        /// <summary>
        /// 判断当前参数是否需要Size属性。
        /// </summary>
        private bool NeedSizeProperty
        {
            get
            {
                return _ParameterType == DbTypeEnum.Binary ||
                    _ParameterType == DbTypeEnum.VarBinary ||
                    _ParameterType == DbTypeEnum.Char ||
                    _ParameterType == DbTypeEnum.VarChar ||
                    _ParameterType == DbTypeEnum.NChar ||
                    _ParameterType == DbTypeEnum.NVarChar;
            }
        }
        private int _Size = 0;
        /// <summary>
        /// 获取或设置列中数据的最大大小（以字节为单位）。
        /// </summary>
        [System.ComponentModel.DefaultValue(0)]
        [System.ComponentModel.Bindable(true), PersistentCategoryAttribute("PersistentCategory_Content")]
        [Basic.Designer.PersistentDescription("PropertyDescription_Size")]
        public int Size
        {
            get { return _Size; }
            set
            {
                if (_Size != value)
                {
                    base.OnPropertyChanging("Size");
                    _Size = value;
                    base.RaisePropertyChanged("Size");
                }
            }
        }

        private int _Precision = 0;
        /// <summary>
        /// 获取或设置列中数据的最大大小（以字节为单位）。
        /// </summary>
        [System.ComponentModel.DefaultValue(0)]
        [System.ComponentModel.Bindable(true), PersistentCategoryAttribute("PersistentCategory_Content")]
        [Basic.Designer.PersistentDescription("PropertyDescription_Precision")]
        public int Precision
        {
            get { return _Precision; }
            set
            {
                if (_Precision != value)
                {
                    base.OnPropertyChanging("Precision");
                    _Precision = value;
                    base.RaisePropertyChanged("Precision");
                }
            }
        }

        private byte _Scale = 0;
        /// <summary>
        /// 获取或设置数据库参数值解析为的小数位数
        /// </summary>
        [System.ComponentModel.DefaultValue(typeof(byte), "0")]
        [System.ComponentModel.Bindable(true), PersistentCategoryAttribute("PersistentCategory_Content")]
        [Basic.Designer.PersistentDescription("PropertyDescription_Scale")]
        public byte Scale
        {
            get { return _Scale; }
            set
            {
                if (_Scale != value)
                {
                    base.OnPropertyChanging("Scale");
                    _Scale = value;
                    base.RaisePropertyChanged("Scale");
                }
            }
        }

        /// <summary>
        /// 参数显示的图标资源
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public string Source
        {
            get
            {
                if (Direction == ParameterDirection.Output)
                    return "/Basic.Persistent;component/Images/Database_OutputParameter.ico";
                else if (Direction == ParameterDirection.ReturnValue)
                    return "/Basic.Persistent;component/Images/Database_ReturnValue.ico";
                return "/Basic.Persistent;component/Images/Database_InputParameter.ico";
            }
        }

        private ParameterDirection _Direction = ParameterDirection.Input;
        /// <summary>
        /// 获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。
        /// </summary>
        [System.ComponentModel.DefaultValue(typeof(ParameterDirection), "Input")]
        [System.ComponentModel.Bindable(true), PersistentCategoryAttribute("PersistentCategory_Content")]
        [Basic.Designer.PersistentDescription("PropertyDescription_Direction")]
        public ParameterDirection Direction
        {
            get { return _Direction; }
            set
            {
                if (_Direction != value)
                {
                    base.OnPropertyChanging("Direction");
                    _Direction = value;
                    base.RaisePropertyChanged("Direction");
                    base.RaisePropertyChanged("Source");
                }
            }
        }

        /// <summary>
        /// 返回表示当前 Basic.Configuration.CommandParameter 的 System.String。
        /// </summary>
        /// <returns>System.String，表示当前的 Basic.Configuration.CommandParameter。</returns>
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return typeof(CommandParameter).Name;
            return string.Concat(Name, " : ", ParameterType);
        }

        /// <summary>
        /// 获取当前节点元素名称
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        protected internal override string ElementName { get { return XmlElementName; } }

        /// <summary>
        /// 从对象的 XML 表示形式读取属性。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值</param>
        /// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
        protected internal override bool ReadAttribute(string name, string value)
        {
            if (name == ColumnAttribute) { _SourceColumn = value; return true; }
            else if (name == ParameterTypeAttribute) { return Enum.TryParse<DbTypeEnum>(value, true, out _ParameterType); }
            else if (name == SizeAttribute) { _Size = Convert.ToInt32(value); return true; }
            else if (name == PrecisionAttribute) { _Precision = Convert.ToInt32(value); return true; }
            else if (name == ScaleAttribute) { _Scale = Convert.ToByte(value); return true; }
            else if (name == NullableAttribute) { _Nullable = Convert.ToBoolean(value); return true; }
            else if (name == DirectionAttribute) { return Enum.TryParse<ParameterDirection>(value, true, out _Direction); }
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
            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == ElementName)
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
            writer.WriteAttributeString(ColumnAttribute, SourceColumn);
            writer.WriteAttributeString(ParameterTypeAttribute, ParameterType.ToString());
            if (_Size > 0 && NeedSizeProperty)
                writer.WriteAttributeString(SizeAttribute, Convert.ToString(_Size));
            if (_Precision > 0 && _ParameterType == DbTypeEnum.Decimal)
                writer.WriteAttributeString(PrecisionAttribute, Convert.ToString(_Precision));
            if (_Scale > 0 && _ParameterType == DbTypeEnum.Decimal)
                writer.WriteAttributeString(ScaleAttribute, Convert.ToString(_Scale));
            if (_Nullable)
                writer.WriteAttributeString(NullableAttribute, "true");
            if (Direction != ParameterDirection.Input)
                writer.WriteAttributeString(DirectionAttribute, Direction.ToString());
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

        /// <summary>
        /// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        /// <param name="connectionType">表示数据库连接类型</param>
        protected internal override void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType)
        {
            writer.WriteStartElement(XmlElementName);
            writer.WriteAttributeString(ColumnAttribute, SourceColumn);
            writer.WriteAttributeString(ParameterTypeAttribute, ParameterType.ToString());
            if (_Size > 0)
                writer.WriteAttributeString(SizeAttribute, Convert.ToString(_Size));
            if (_Precision > 0)
                writer.WriteAttributeString(PrecisionAttribute, Convert.ToString(_Precision));
            if (_Scale > 0)
                writer.WriteAttributeString(ScaleAttribute, Convert.ToString(_Scale));
            if (_Nullable)
                writer.WriteAttributeString(NullableAttribute, Convert.ToString(_Nullable).ToLower());
            if (Direction != ParameterDirection.Input)
                writer.WriteAttributeString(DirectionAttribute, Direction.ToString());
            writer.WriteString(_Command.CreateParameterName(Name, connectionType));
            writer.WriteEndElement();
        }
    }
}
