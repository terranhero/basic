using System.ComponentModel;
using System;
using Basic.Designer;
using Basic.Enums;
using System.CodeDom;
using System.ComponentModel.DataAnnotations;

namespace Basic.DataEntities
{
	/// <summary>
	/// 指定如何显示数据字段以及如何设置数据字段的格式。
	/// </summary>
	[Serializable, TypeConverter(typeof(DiaplayFormatConverter))]
	[PersistentDescription("PropertyDescription_DisplayFormat"), PersistentCategory("PersistentCategory_Attributes")]
	public sealed class DisplayFormat : AbstractValidationAttribute
	{
		internal const string XmlElementName = "DisplayFormatAttribute";
		internal const string ApplyFormatInEditModeAttribute = "ApplyFormatInEditMode";
		internal const string ConvertEmptyStringToNullAttribute = "ConvertEmptyStringToNull";
		internal const string DataFormatStringAttribute = "DataFormatString";
		internal const string HtmlEncodeAttribute = "HtmlEncode";
		internal const string NullDisplayTextAttribute = "NullDisplayText";
		/// <summary>
		/// 初始化 DisplayFormat 类实例。
		/// </summary>
		/// <param name="nofity">需要通知 DataEntityPropertyElement 类实例当前 DisplayFormat 类的属性已更改。</param>
		public DisplayFormat(DataEntityPropertyElement nofity) : base(nofity) { }

		/// <summary>异常关键字</summary>
		public override string ErrorKey { get { return "DisplayFormat"; } }

		private bool _ApplyFormatInEditMode = false;
		/// <summary>
		/// 获取或设置一个值，该值指示数据字段处于编辑模式时，
		/// 是否将 System.ComponentModel.DataAnnotations.DisplayFormatAttribute.DataFormatString
		/// 属性指定的格式设置字符串应用于字段值。
		/// </summary>
		/// <value>如果在编辑模式中将格式设置字符串应用于字段值，则为 true；否则为 false。默认值为 false。</value>
		[PersistentDescription("DisplayFormat_ApplyFormatInEditMode"), DefaultValue(false)]
		public bool ApplyFormatInEditMode
		{
			get { return _ApplyFormatInEditMode; }
			set
			{
				if (_ApplyFormatInEditMode != value)
				{
					_ApplyFormatInEditMode = value;
					base.RaisePropertyChanged("ApplyFormatInEditMode");
				}
			}
		}

		private bool _ConvertEmptyStringToNull = true;
		/// <summary>
		/// 获取或设置一个值，该值指示在数据源中更新数据字段时是否将空字符串值 ("") 自动转换为 null。
		/// </summary>
		/// <value>如果将空字符串值自动转换为 null，则为 true；否则为 false。默认值为 true。</value>
		[PersistentDescription("DisplayFormat_ConvertEmptyStringToNull"), DefaultValue(true)]
		public bool ConvertEmptyStringToNull
		{
			get { return _ConvertEmptyStringToNull; }
			set
			{
				if (_ConvertEmptyStringToNull != value)
				{
					_ConvertEmptyStringToNull = value;
					base.RaisePropertyChanged("ConvertEmptyStringToNull");
				}
			}
		}

		/// <summary>获取或设置一条错误消息，以便在验证失败时与验证控件关联</summary>
		[PersistentDescription("DisplayName_ErrorMessage"), Browsable(false)]
		public override string ErrorMessage { get; set; }

		private string _DataFormatString = string.Empty;
		/// <summary>
		/// 获取或设置字段值的显示格式。
		/// </summary>
		/// <value>一个指定数据字段值的显示格式的格式设置字符串。默认值为空字符串 ("")，表示尚无特殊格式设置应用于该字段值。</value>
		[PersistentDescription("DisplayFormat_DataFormatString"), DefaultValue("")]
		public string DataFormatString
		{
			get { return _DataFormatString; }
			set
			{
				if (_DataFormatString != value)
				{
					_DataFormatString = value;
					base.RaisePropertyChanged("DataFormatString");
				}
			}
		}

		private bool _HtmlEncode = false;
		/// <summary>
		/// 获取或设置一个值，该值指示字段是否应经过 HTML 编码。
		/// </summary>
		/// <value>如果字段应经过 HTML 编码，则为 true；否则为 false。</value>
		[PersistentDescription("DisplayFormat_HtmlEncode"), DefaultValue(false)]
		public bool HtmlEncode
		{
			get { return _HtmlEncode; }
			set
			{
				if (_HtmlEncode != value)
				{
					_HtmlEncode = value;
					base.RaisePropertyChanged("HtmlEncode");
				}
			}
		}

		private string _NullDisplayText = string.Empty;
		/// <summary>
		/// 获取或设置字段值为 null 时为字段显示的文本。
		/// </summary>
		/// <value>字段值为 null 时为字段显示的文本。默认值为空字符串 ("")，表示尚未设置此属性。</value>
		[PersistentDescription("DisplayFormat_NullDisplayText"), DefaultValue("")]
		public string NullDisplayText
		{
			get { return _NullDisplayText; }
			set
			{
				if (_NullDisplayText != value)
				{
					_NullDisplayText = value;
					base.RaisePropertyChanged("NullDisplayText");
				}
			}
		}

		/// <summary>
		/// 判断当前格式是否为空。
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public bool IsEmpty
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(DataFormatString))
				{
					return false;
				}
				if (!string.IsNullOrWhiteSpace(NullDisplayText))
				{
					return false;
				}
				if (ApplyFormatInEditMode)
				{
					return false;
				}
				if (!ConvertEmptyStringToNull)
				{
					return false;
				}
				if (HtmlEncode)
				{
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// 返回表示当前 DisplayFormat 的 System.String。
		/// </summary>
		/// <returns>System.String，表示当前的 DisplayFormat。</returns>
		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(_DataFormatString))
			{
				return _DataFormatString;
			}
			return XmlElementName.Replace("Attribute", "");
		}

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == ApplyFormatInEditModeAttribute) { return bool.TryParse(value, out _ApplyFormatInEditMode); }
			else if (name == DisplayFormat.ConvertEmptyStringToNullAttribute) { return bool.TryParse(value, out _ConvertEmptyStringToNull); }
			else if (name == DisplayFormat.HtmlEncodeAttribute) { return bool.TryParse(value, out _HtmlEncode); }
			else if (name == NullDisplayTextAttribute) { NullDisplayText = value; return true; }
			else if (name == DataFormatStringAttribute) { DataFormatString = value; return true; }
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteXml(System.Xml.XmlWriter writer)
		{
			if (!IsEmpty) { base.WriteXml(writer); }
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			base.WriteAttribute(writer);
			if (!string.IsNullOrWhiteSpace(DataFormatString))
				writer.WriteAttributeString(DataFormatStringAttribute, DataFormatString);
			if (!string.IsNullOrWhiteSpace(NullDisplayText))
				writer.WriteAttributeString(NullDisplayTextAttribute, NullDisplayText);
			if (ApplyFormatInEditMode)
			{
				writer.WriteStartAttribute(ApplyFormatInEditModeAttribute);
				writer.WriteValue(ApplyFormatInEditMode);
				writer.WriteEndAttribute();
			}
			if (!ConvertEmptyStringToNull)
			{
				writer.WriteStartAttribute(ConvertEmptyStringToNullAttribute);
				writer.WriteValue(ConvertEmptyStringToNull);
				writer.WriteEndAttribute();
			}
			if (HtmlEncode)
			{
				writer.WriteStartAttribute(HtmlEncodeAttribute);
				writer.WriteValue(HtmlEncode);
				writer.WriteEndAttribute();
			}
		}

		protected internal override string ElementName
		{
			get { return XmlElementName; }
		}

		/// <summary>
		/// 将当前显示格式输出到属性的Attribute中。
		/// </summary>
		/// <param name="property">属性</param>
		protected internal override void WriteDesignerCodeAttribute(CodeMemberProperty property)
		{
			if (!IsEmpty)
			{
				CodeTypeReference dateFormatTypeReference = new CodeTypeReference(typeof(DisplayFormatAttribute),
				CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration dateFormatAttribute = new CodeAttributeDeclaration(dateFormatTypeReference);
				if (!string.IsNullOrWhiteSpace(DataFormatString))
				{
					CodePrimitiveExpression formatExpresion = new CodePrimitiveExpression(DataFormatString);
					dateFormatAttribute.Arguments.Add(new CodeAttributeArgument("DataFormatString", formatExpresion));
				}
				if (!string.IsNullOrWhiteSpace(NullDisplayText))
				{
					CodePrimitiveExpression formatExpresion = new CodePrimitiveExpression(NullDisplayText);
					dateFormatAttribute.Arguments.Add(new CodeAttributeArgument("NullDisplayText", formatExpresion));
				}
				if (ApplyFormatInEditMode)
				{
					CodePrimitiveExpression formatExpresion = new CodePrimitiveExpression(ApplyFormatInEditMode);
					dateFormatAttribute.Arguments.Add(new CodeAttributeArgument("ApplyFormatInEditMode", formatExpresion));
				}
				if (!ConvertEmptyStringToNull)
				{
					CodePrimitiveExpression formatExpresion = new CodePrimitiveExpression(ConvertEmptyStringToNull);
					dateFormatAttribute.Arguments.Add(new CodeAttributeArgument("ConvertEmptyStringToNull", formatExpresion));
				}
				if (HtmlEncode)
				{
					CodePrimitiveExpression formatExpresion = new CodePrimitiveExpression(HtmlEncode);
					dateFormatAttribute.Arguments.Add(new CodeAttributeArgument("HtmlEncode", formatExpresion));
				}
				property.CustomAttributes.Add(dateFormatAttribute);
			}
		}
	}
}
