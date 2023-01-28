using Basic.Designer;
using Basic.Enums;
using System;
using System.CodeDom;
using System.ComponentModel;
using System.Drawing.Design;

namespace Basic.DataEntities
{
	/// <summary>
	/// 提供一个比较模型的两个属性的特性。
	/// </summary>
	[PersistentDescription("DesignerValidation_CompareAttribute"), DefaultProperty("OtherProperty")]
    [TypeConverter(typeof(ValidationTypeConverter)), PersistentCategory("PersistentCategory_Attributes")]
	[System.Xml.Serialization.XmlRoot(XmlElementName), DisplayName("Compare"),]
    public sealed class CompareValidation : AbstractValidationAttribute, ICompareProperty
	{
		internal const string XmlElementName = "CompareAttribute";
		private const string DataTypeAttribute = "DataType";
		private const string OtherPropertyAttribute = "OtherProperty";
		private const string OperatorAttribute = "Operator";

		/// <summary>
		/// 初始化 CompareValidation 类的新实例。
		/// </summary>
		public CompareValidation(DataEntityPropertyElement property) : base(property) { }

		/// <summary>
		/// 异常关键字。
		/// </summary>
		public override string ErrorKey { get { return "Compare"; } }

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
			return XmlElementName;
		}

		private ValidationDataType _DataType = ValidationDataType.String;
		/// <summary>
		/// 表示需要比较的数据类型
		/// </summary>
		[PersistentDescription("DesignerValidation_ValidationDataType"), DefaultValue(typeof(ValidationDataType), "String")]
		public ValidationDataType DataType
		{
			get { return _DataType; }
			set
			{
				if (_DataType != value)
				{
					_DataType = value;
					base.RaisePropertyChanged("ValidationDataType");
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
		}

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
					base.RaisePropertyChanged("OtherProperty");
				}
			}
		}

		private ValidationCompareOperator _Operator = ValidationCompareOperator.Equal;
		/// <summary>
		/// 比较运算符
		/// </summary>
		[PersistentDescription("DesignerValidation_Operator"), DefaultValue(typeof(ValidationCompareOperator), "Equal")]
		public ValidationCompareOperator Operator
		{
			get { return _Operator; }
			set
			{
				if (_Operator != value)
				{
					_Operator = value;
					base.RaisePropertyChanged("Operator");
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
			if (name == OtherPropertyAttribute)
			{
				_OtherProperty = value; return true;
			}
			else if (name == DataTypeAttribute)
			{
				return Enum.TryParse<ValidationDataType>(value, out _DataType);
			}
			else if (name == OperatorAttribute)
			{
				return Enum.TryParse<ValidationCompareOperator>(value, out _Operator);
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
			if (DataType != ValidationDataType.String)
				writer.WriteAttributeString(DataTypeAttribute, DataType.ToString());
			if (Operator != ValidationCompareOperator.Equal)
				writer.WriteAttributeString(OperatorAttribute, Operator.ToString());
		}

		/// <summary>
		/// 将当前显示格式输出到属性的Attribute中。
		/// </summary>
		/// <param name="property">属性</param>
		protected internal override void WriteDesignerCodeAttribute(CodeMemberProperty property)
		{
			if (!string.IsNullOrWhiteSpace(OtherProperty))
			{
				CodeTypeReference compareTypeReference = new CodeTypeReference(typeof(Basic.Validations.CompareAttribute),
				CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration compareAttribute = new CodeAttributeDeclaration(compareTypeReference);

				CodePrimitiveExpression otherPropertyExpresion = new CodePrimitiveExpression(OtherProperty);
				compareAttribute.Arguments.Add(new CodeAttributeArgument(otherPropertyExpresion));

				if (DataType != ValidationDataType.String)
				{
					CodeFieldReferenceExpression dataTypeExpression = new CodeFieldReferenceExpression(
					new CodeTypeReferenceExpression("ValidationDataType"), DataType.ToString());
					compareAttribute.Arguments.Add(new CodeAttributeArgument(dataTypeExpression));
				}
				if (Operator != ValidationCompareOperator.Equal)
				{
					CodeFieldReferenceExpression operatorExpression = new CodeFieldReferenceExpression(
					new CodeTypeReferenceExpression("ValidationCompareOperator"), Operator.ToString());
					compareAttribute.Arguments.Add(new CodeAttributeArgument(operatorExpression));
				}
				property.CustomAttributes.Add(compareAttribute);
			}
		}
	}
}
