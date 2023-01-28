using System;
using Basic.Designer;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示数据契约定义
	/// </summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [System.ComponentModel.TypeConverter(typeof(DataContractConverter))]
    public sealed class DataContractElement : AbstractCustomTypeDescriptor
    {
        internal const string XmlElementName = "DataContract";
        internal const string GenerateAttribute = "Generate";
        internal const string IsReferenceAttribute = "IsReference";
        internal const string NameAttribute = "Name";
        internal const string NamespaceAttribute = "Namespace";
        private readonly DataEntityElement dataEntityElement;
        /// <summary>
        /// 初始化 DataContractElement 类实例。
        /// </summary>
        /// <param name="baseClass"></param>
        internal DataContractElement(DataEntityElement nofity) : base(nofity) { dataEntityElement = nofity; }

        /// <summary>
        /// 返回此组件实例的名称。
        /// </summary>
        public override string GetComponentName() { return typeof(DataContractElement).Name; }

        /// <summary>
        /// 获取当前节点元素命名空间
        /// </summary>
        protected internal override string ElementNamespace { get { return null; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetClassName() { return typeof(DataContractElement).Name; }

        /// <summary>
        /// 
        /// </summary>
        protected internal override string ElementName { get { return XmlElementName; } }

        /// <summary>
        /// 将此实例的值转换为其等效字符串表示形式（“True”或“False”）。
        /// </summary>
        /// <returns>如果此实例的值为 true，则为 System.Boolean.TrueString；如果此实例的值为 false，则为 System.Boolean.FalseString。</returns>
        public override string ToString()
        {
            return _Generate.ToString();
        }

        private string _Name = null;
        /// <summary>
        /// 获取或设置类型的数据协定的名称。
        /// </summary>
        /// <value>数据协定的本地名称。 默认值是应用了该属性的类的名称。</value>
        [Basic.Designer.PersistentDescription("PersistentDescription_DataContract_Name")]
        [Basic.Designer.PersistentCategory("PersistentCategory_DataContract")]
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    base.RaisePropertyChanged("Name");
                }
            }
        }

        private string _Namespace = null;
        /// <summary>
        /// 获取或设置类型的数据协定的命名空间。
        /// </summary>
        /// <value>协定的命名空间。</value>
        [Basic.Designer.PersistentDescription("PersistentDescription_DataContract_Namespace")]
        [Basic.Designer.PersistentCategory("PersistentCategory_DataContract")]
        public string Namespace
        {
            get { return _Namespace; }
            set
            {
                if (_Namespace != value)
                {
                    _Namespace = value;
                    base.RaisePropertyChanged("Namespace");
                }
            }
        }

        private bool _Generate = false;
        /// <summary>
        /// 获取或设置类型是否使用数据协定。
        /// </summary>
        /// <value>协定的命名空间。</value>
        [Basic.Designer.PersistentDescription("PersistentDescription_DataContract_Generate")]
        [Basic.Designer.PersistentCategory("PersistentCategory_DataContract")]
        [System.ComponentModel.DefaultValue(false)]
        public bool Generate
        {
            get { return _Generate; }
            set
            {
                if (_Generate != value)
                {
                    _Generate = value;
                    foreach (DataEntityPropertyElement property in dataEntityElement.Properties)
                    {
                        property.DataMember = _Generate;
                    }
                    foreach (DataConditionPropertyElement property in dataEntityElement.Condition.Arguments)
                    {
                        property.DataMember = _Generate;
                    }
                    base.RaisePropertyChanged("Generate");
                }
            }
        }

        private bool _IsReference = false;
        /// <summary>
        /// 获取或设置类型是否使用数据协定。
        /// </summary>
        /// <value>协定的命名空间。</value>
        [Basic.Designer.PersistentDescription("PersistentDescription_DataContract_IsReference")]
        [Basic.Designer.PersistentCategory("PersistentCategory_DataContract")]
        [System.ComponentModel.DefaultValue(false)]
        public bool IsReference
        {
            get { return _IsReference; }
            set
            {
                if (_IsReference != value)
                {
                    _IsReference = value;
                    base.RaisePropertyChanged("IsReference");
                }
            }
        }

        /// <summary>
        /// 从对象的 XML 表示形式读取属性。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值</param>
        /// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
        protected internal override bool ReadAttribute(string name, string value)
        {
            if (name == GenerateAttribute) { _Generate = Convert.ToBoolean(value); return true; }
            else if (name == IsReferenceAttribute) { _IsReference = Convert.ToBoolean(value); return true; }
            else if (name == NameAttribute) { _Name = value; return true; }
            else if (name == NamespaceAttribute) { _Namespace = value; return true; }
            return false;
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象扩展信息。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        protected internal override bool ReadContent(System.Xml.XmlReader reader)
        {
            return true;
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式中属性部分。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
        {
            if (_Generate)
            {
                writer.WriteAttributeString(GenerateAttribute, Convert.ToString(_Generate).ToLower());
                if (_IsReference)
                    writer.WriteAttributeString(IsReferenceAttribute, Convert.ToString(_IsReference).ToLower());
                if (!string.IsNullOrWhiteSpace(_Name))
                    writer.WriteAttributeString(NameAttribute, _Name);
                if (!string.IsNullOrWhiteSpace(_Namespace))
                    writer.WriteAttributeString(NamespaceAttribute, _Namespace);
            }
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        /// <param name="connectionType">表示数据库连接类型</param>
        protected internal override void WriteContent(System.Xml.XmlWriter writer) { }
    }
}
