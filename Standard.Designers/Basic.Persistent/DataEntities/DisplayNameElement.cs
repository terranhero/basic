using System;
using System.CodeDom;
using System.ComponentModel.DataAnnotations;
using Basic.Configuration;
using Basic.Designer;
using Basic.EntityLayer;
using Basic.Enums;

namespace Basic.DataEntities
{
	/// <summary>
	/// 属性的显示名称格式设置
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(DisplayNameConverter))]
	[PersistentCategory("PersistentCategory_Attributes"), PersistentDescription("DisplayName_Description")]
	public sealed class DisplayNameElement : AbstractCustomTypeDescriptor
	{
		internal const string XmlElementName = "DisplayName";
		internal const string NameAttribute = "Name";
		internal const string PromptAttribute = "Prompt";
		internal const string ConverterAttribute = "Converter";
		internal const string DisplayTypeAttribute = "DisplayType";
		private readonly AbstractPropertyElement propertyElement;
		/// <summary>
		/// 当亲属性
		/// </summary>
		internal AbstractPropertyElement Property { get { return propertyElement; } }

		/// <summary>
		/// 初始化 DisplayNameElement 类实例。
		/// </summary>
		/// <param name="property"></param>
		internal DisplayNameElement(AbstractPropertyElement property)
			: base(property) { propertyElement = property; }

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName() { return XmlElementName; }

		/// <summary>
		/// 获取当前节点元素命名空间
		/// </summary>
		protected internal override string ElementNamespace { get { return null; } }

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return typeof(DisplayNameElement).Name; }

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName { get { return XmlElementName; } }

		/// <summary>
		/// 返回表示当前 DisplayNameElement 的 System.String。
		/// </summary>
		/// <returns>System.String，表示当前的 DisplayNameElement。</returns>
		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(_DisplayName) && !string.IsNullOrWhiteSpace(_ConverterName))
			{
				return string.Concat(_ConverterName, ":", _DisplayName);
			}
			else if (!string.IsNullOrWhiteSpace(_DisplayName))
			{
				return _DisplayName;
			}
			return _DisplayType.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="property"></param>
		internal void WriteDisplayNameCode(CodeMemberProperty property)
		{
			if (!string.IsNullOrWhiteSpace(_DisplayName))
			{

				if (_DisplayType == DisplayTypeEnum.WebDisplayAttribute)
				{
					CodeTypeReference displayReference = new CodeTypeReference(typeof(WebDisplayAttribute), CodeTypeReferenceOptions.GlobalReference);
					CodeAttributeDeclaration columnAttribute = new CodeAttributeDeclaration(displayReference);
					columnAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_DisplayName)));
					if (!string.IsNullOrWhiteSpace(_ConverterName))
						columnAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_ConverterName)));
					if (!string.IsNullOrWhiteSpace(_Prompt))
						columnAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_Prompt)));
					property.CustomAttributes.Add(columnAttribute);
				}
				else if (_DisplayType == DisplayTypeEnum.DisplayAttribute)
				{
					CodeTypeReference displayReference = new CodeTypeReference(typeof(DisplayAttribute), CodeTypeReferenceOptions.GlobalReference);
					CodeAttributeDeclaration columnAttribute = new CodeAttributeDeclaration(displayReference);
					columnAttribute.Arguments.Add(new CodeAttributeArgument("Name", new CodePrimitiveExpression(_DisplayName)));
					if (!string.IsNullOrWhiteSpace(_Prompt))
						columnAttribute.Arguments.Add(new CodeAttributeArgument("Prompt", new CodePrimitiveExpression(_Prompt)));
					property.CustomAttributes.Add(columnAttribute);
				}
			}
		}

		private DisplayTypeEnum _DisplayType = DisplayTypeEnum.WebDisplayAttribute;
		/// <summary>
		/// 获取或设置显示特性的类型。
		/// </summary>
		[PersistentDescription("DisplayName_DisplayType")]
		[System.ComponentModel.DefaultValue(typeof(DisplayTypeEnum), "WebDisplayAttribute")]
		public DisplayTypeEnum DisplayType
		{
			get { return _DisplayType; }
			set
			{
				if (_DisplayType != value)
				{
					_DisplayType = value;
					base.RaisePropertyChanged("DisplayType");
				}
			}
		}

		private string _Prompt = null;
		/// <summary>
		/// 获取或设置一个值，该值将用于为用户界面中的提示设置水印。
		/// </summary>
		[System.ComponentModel.TypeConverter(typeof(DisplayPromptConveter))]
		[Basic.Designer.PersistentDescription("DisplayName_Prompt")]
		public string Prompt
		{
			get { return _Prompt; }
			set
			{
				if (_Prompt != value)
				{
					_Prompt = value;
					base.RaisePropertyChanged("Prompt");
				}
			}
		}

		private string _DisplayName = null;
		/// <summary>
		/// 获取或设置属性的显示名称。
		/// </summary>
		[System.ComponentModel.TypeConverter(typeof(DisplaySourceConveter))]
		[Basic.Designer.PersistentDescription("DisplayName_DisplayName")]
		public string DisplayName
		{
			get { return _DisplayName; }
			set
			{
				if (_DisplayName != value)
				{
					_DisplayName = value;
					base.RaisePropertyChanged("DisplayName");
				}
			}
		}

		private string _ConverterName = null;
		/// <summary>
		/// 获取或设置属性的本地显示名称的转换器名称。
		/// </summary>
		[Basic.Designer.PersistentDescription("DisplayName_ConverterName")]
		[System.ComponentModel.TypeConverter(typeof(MessageTypeConverter))]
		public string ConverterName
		{
			get { return _ConverterName; }
			set
			{
				if (_ConverterName != value)
				{
					_ConverterName = value;
					base.RaisePropertyChanged("ConverterName");
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
			if (name == NameAttribute) { _DisplayName = value; return true; }
			else if (name == ConverterAttribute) { _ConverterName = value; return true; }
			else if (name == PromptAttribute) { _Prompt = value; return true; }
			else if (name == DisplayTypeAttribute) { return Enum.TryParse<DisplayTypeEnum>(value, out _DisplayType); }
			return false;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		protected internal override bool ReadContent(System.Xml.XmlReader reader) { return true; }

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			if (_DisplayType != DisplayTypeEnum.WebDisplayAttribute)
				writer.WriteAttributeString(DisplayTypeAttribute, _DisplayType.ToString());

			if (!string.IsNullOrWhiteSpace(_DisplayName))
				writer.WriteAttributeString(NameAttribute, _DisplayName);
			if (!string.IsNullOrWhiteSpace(_ConverterName))
				writer.WriteAttributeString(ConverterAttribute, _ConverterName);
			if (!string.IsNullOrWhiteSpace(_Prompt))
				writer.WriteAttributeString(PromptAttribute, _Prompt);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer) { }

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteXml(System.Xml.XmlWriter writer)
		{
			if (!string.IsNullOrWhiteSpace(_DisplayName))
				base.WriteXml(writer);
		}
		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(System.Xml.XmlWriter writer, ConnectionTypeEnum connectionType) { }
	}
}
