using Basic.Designer;
using System;
using System.CodeDom;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示数组或字符串允许的最大长度的特性
	/// </summary>
	[PersistentDescription("DesignerValidation_BoolRequiredAttribute"), PersistentCategory("PersistentCategory_Attributes")]
	[DefaultProperty("OtherProperty"), DisplayName("BoolReqiured"), TypeConverter(typeof(ValidationTypeConverter))]
	[System.Xml.Serialization.XmlRoot(XmlElementName)]
    public sealed class BoolRequiredValidation : AbstractValidationAttribute, ICompareProperty
	{
		/// <summary>
		/// 当前类使用Xml序列化后生成元素名称。
		/// </summary>
		internal const string XmlElementName = "BoolRequiredAttribute";
		private const string OtherPropertyAttribute = "OtherProperty";
		private const string RequiredValueAttribute = "RequiredValue";
		private const string PropertyTypeAttribute = "PropertyType";

		/// <summary>异常关键字</summary>
		public override string ErrorKey { get { return "Required"; } }

		/// <summary>
		/// 初始化 BoolReqiuredValidation 类的新实例。
		/// </summary>
		/// <param name="property">当前验证器所属属性。</param>
		public BoolRequiredValidation(DataEntityPropertyElement nofity) : base(nofity) { }

		private string _OtherProperty = string.Empty;
		/// <summary>
		/// 需要与当前属性比较的属性
		/// </summary>
		[PersistentDescription("DesignerValidation_OtherProperty"), DefaultValue("")]
		[Editor(typeof(OtherPropertyEditor), typeof(UITypeEditor))]
		public string OtherProperty
		{
			get { return _OtherProperty; }
			set
			{
				if (_OtherProperty != value)
				{
					_OtherProperty = value;
					if (value == null)
					{
						PropertyType = null;
					}
					RequiredValue = null;
					base.RaisePropertyChanged("OtherProperty");
				}
			}
		}
		private System.Type _PropertyType = null;

		/// <summary>
		/// OtherProperty 属性值对应的属性值类型
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public System.Type PropertyType
		{
			get { return _PropertyType; }
			set
			{
				if (_PropertyType != value)
				{
					_PropertyType = value;
					base.RaisePropertyChanged("PropertyType");
				}
			}
		}

		/// <summary>
		/// 设置 OtherProperty 属性值。
		/// </summary>
		/// <param name="property"></param>
		void ICompareProperty.SetOtherProperty(DataEntityPropertyElement property)
		{
			OtherProperty = property.Name;
			PropertyType = property.Type;
		}

		private object _RequiredValue;
		/// <summary>
		/// 目标属性为何值时，当前属性必输
		/// </summary>
		[PersistentDescription("DesignerValidation_RequiredValue"), TypeConverter(typeof(CompareValueConverter))]
		public object RequiredValue
		{
			get { return _RequiredValue; }
			set
			{
				if (_RequiredValue != value)
				{
					_RequiredValue = value;
					base.RaisePropertyChanged("RequiredValue");
				}
			}
		}

		/// <summary>
		/// 返回表示当前 DesignerBoolReqiured 的 System.String。
		/// </summary>
		/// <returns>System.String，表示当前的 DesignerBoolReqiured。</returns>
		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(_OtherProperty))
			{
				return _OtherProperty;
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
			if (name == OtherPropertyAttribute) { OtherProperty = value; return true; }
			else if (name == PropertyTypeAttribute) { PropertyType = Type.GetType(value); return true; }
			else if (name == RequiredValueAttribute && PropertyType != null)
			{
				RequiredValue = ConvertToValue(PropertyType, CultureInfo.CurrentUICulture, value);
				return true;
			}
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteXml(System.Xml.XmlWriter writer)
		{
			if (!string.IsNullOrWhiteSpace(_OtherProperty)) { base.WriteXml(writer); }
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			base.WriteAttribute(writer);
			writer.WriteAttributeString(OtherPropertyAttribute, OtherProperty);
			writer.WriteAttributeString(PropertyTypeAttribute, PropertyType.FullName);
			if (RequiredValue != null)
			{
				writer.WriteStartAttribute(RequiredValueAttribute);
				writer.WriteValue(RequiredValue);
				writer.WriteEndAttribute();
			}
		}

		/// <summary>
		/// 将当前显示格式输出到属性的Attribute中。
		/// </summary>
		/// <param name="property">属性</param>
		protected internal override void WriteDesignerCodeAttribute(CodeMemberProperty property)
		{
			if (!string.IsNullOrWhiteSpace(OtherProperty) && RequiredValue != null)
			{
				CodeTypeReference boolRequiredTypeReference = new CodeTypeReference(typeof(Basic.Validations.BoolRequiredAttribute),
				CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration boolRequiredAttribute = new CodeAttributeDeclaration(boolRequiredTypeReference);

				CodePrimitiveExpression otherPropertyExpresion = new CodePrimitiveExpression(OtherProperty);
				boolRequiredAttribute.Arguments.Add(new CodeAttributeArgument(otherPropertyExpresion));

				if (PropertyType == typeof(System.DateTime))
				{
					DateTime dt = (DateTime)RequiredValue;
					CodeObjectCreateExpression propertyValueExpresion =
						new CodeObjectCreateExpression(PropertyType, new CodePrimitiveExpression(dt.Ticks));
					boolRequiredAttribute.Arguments.Add(new CodeAttributeArgument(propertyValueExpresion));
				}
				else if (PropertyType == typeof(System.DateTimeOffset))
				{
					DateTimeOffset dt = (DateTimeOffset)RequiredValue;
					CodeObjectCreateExpression propertyValueExpresion = new CodeObjectCreateExpression(PropertyType);
					propertyValueExpresion.Parameters.Add(new CodePrimitiveExpression(dt.Ticks));
					boolRequiredAttribute.Arguments.Add(new CodeAttributeArgument(propertyValueExpresion));
				}
				else if (PropertyType == typeof(System.Guid))
				{
					CodeObjectCreateExpression propertyValueExpresion = new CodeObjectCreateExpression(PropertyType);
					propertyValueExpresion.Parameters.Add(new CodePrimitiveExpression(RequiredValue.ToString()));
					boolRequiredAttribute.Arguments.Add(new CodeAttributeArgument(propertyValueExpresion));
				}
				else if (PropertyType == typeof(System.TimeSpan))
				{
					TimeSpan time = (TimeSpan)RequiredValue;
					CodeObjectCreateExpression propertyValueExpresion = new CodeObjectCreateExpression(PropertyType);
					propertyValueExpresion.Parameters.Add(new CodePrimitiveExpression(time.Ticks));
					boolRequiredAttribute.Arguments.Add(new CodeAttributeArgument(propertyValueExpresion));
				}
				else
				{
					CodePrimitiveExpression propertyValueExpresion = new CodePrimitiveExpression(RequiredValue);
					boolRequiredAttribute.Arguments.Add(new CodeAttributeArgument(propertyValueExpresion));
				}
				if (string.IsNullOrWhiteSpace(ErrorMessage) == false)
				{
					boolRequiredAttribute.Arguments.Add(new CodeAttributeArgument("ErrorMessage", new CodePrimitiveExpression(ErrorMessage)));
				}
				property.CustomAttributes.Add(boolRequiredAttribute);
			}
		}
	}
}
