using Basic.Designer;
using System;
using System.CodeDom;
using System.ComponentModel;

namespace Basic.DataEntities
{
	/// <summary>
	///  指定数据字段值的数值范围约束。
	/// </summary>
	[PersistentDescription("DesignerValidation_MaxLengthAttribute"), DefaultProperty("Maximum")]
	[TypeConverter(typeof(ValidationTypeConverter)), PersistentCategory("PersistentCategory_Attributes")]
	[System.Xml.Serialization.XmlRoot(XmlElementName), DisplayName("Range"),]
	public sealed class RangeValidation : AbstractValidationAttribute
	{
		internal const string XmlElementName = "RangeAttribute";
		private const string MinimumAttribute = "Minimum";
		private const string MaximumAttribute = "Maximum";
		/// <summary>
		/// 初始化 RangeValidation 类的新实例。
		/// </summary>
		/// <param name="property">当前验证器所属属性。</param>
		public RangeValidation(DataEntityPropertyElement property) : base(property) { }

		/// <summary>
		/// 异常关键字。
		/// </summary>
		public override string ErrorKey { get { return "Range"; } }

		/// <summary>
		/// 返回表示当前 DesingerStringLength 的 System.String。
		/// </summary>
		/// <returns>System.String，表示当前的 DesingerStringLength。</returns>
		public override string ToString()
		{
			return string.Concat("Between ", _Minimum, " To ", _Maximum);
		}

		private string _Maximum = "0";
		/// <summary>
		/// 获取所允许的最大字段值。
		/// </summary>
		/// <value>所允许的数据字段最大值。</value>
		[PersistentDescription("DesignerValidation_Maximum"), DefaultValue("0")]
		public string Maximum
		{
			get { return _Maximum; }
			set
			{
				if (_Maximum != value)
				{
					_Maximum = value;
					base.RaisePropertyChanged("Maximum");
				}
			}
		}

		private string _Minimum = "0";
		/// <summary>
		/// 获取所允许的最小字段值。
		/// </summary>
		/// <value>所允许的数据字段最小值。</value>
		[PersistentDescription("DesignerValidation_Minimum"), DefaultValue("0")]
		public string Minimum
		{
			get { return _Minimum; }
			set
			{
				if (_Minimum != value)
				{
					_Minimum = value;
					base.RaisePropertyChanged("Minimum");
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
			if (name == MinimumAttribute) { _Minimum = value; return true; }
			else if (name == MaximumAttribute) { _Maximum = value; return true; }
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			base.WriteAttribute(writer);
			writer.WriteStartAttribute(MinimumAttribute);
			writer.WriteValue(_Minimum);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute(MaximumAttribute);
			writer.WriteValue(_Maximum);
			writer.WriteEndAttribute();
		}

		/// <summary>
		/// 将当前显示格式输出到属性的Attribute中。
		/// </summary>
		/// <param name="property">属性</param>
		protected internal override void WriteDesignerCodeAttribute(CodeMemberProperty property)
		{
			CodeTypeReference maxLengthTypeReference = new CodeTypeReference(typeof(Basic.Validations.RangeAttribute),
			CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration maxLengthAttribute = new CodeAttributeDeclaration(maxLengthTypeReference);

			if (Property.Type != null && (Property.Type == typeof(long) || Property.Type == typeof(int) || Property.Type == typeof(short) || Property.Type == typeof(byte)))
			{
				CodePrimitiveExpression minExpression = new CodePrimitiveExpression(Convert.ToInt32(_Minimum));
				maxLengthAttribute.Arguments.Add(new CodeAttributeArgument(minExpression));
				CodePrimitiveExpression maxExpression = new CodePrimitiveExpression(Convert.ToInt32(_Maximum));
				maxLengthAttribute.Arguments.Add(new CodeAttributeArgument(maxExpression));
			}
			else if (Property.Type != null && (Property.Type == typeof(double) || Property.Type == typeof(decimal) || Property.Type == typeof(Single)))
			{
				CodePrimitiveExpression minExpression = new CodePrimitiveExpression(Convert.ToDouble(_Minimum));
				maxLengthAttribute.Arguments.Add(new CodeAttributeArgument(minExpression));
				CodePrimitiveExpression maxExpression = new CodePrimitiveExpression(Convert.ToDouble(_Maximum));
				maxLengthAttribute.Arguments.Add(new CodeAttributeArgument(maxExpression));
				if (string.IsNullOrWhiteSpace(ErrorMessage) == false)
				{
					maxLengthAttribute.Arguments.Add(new CodeAttributeArgument("ErrorMessage", new CodePrimitiveExpression(ErrorMessage)));
				}
			}
			else
			{
				CodeTypeOfExpression typeExpression = new CodeTypeOfExpression(Property.TypeName);
				maxLengthAttribute.Arguments.Add(new CodeAttributeArgument(typeExpression));
				CodePrimitiveExpression minExpression = new CodePrimitiveExpression(_Minimum);
				maxLengthAttribute.Arguments.Add(new CodeAttributeArgument(minExpression));
				CodePrimitiveExpression maxExpression = new CodePrimitiveExpression(_Maximum);
				maxLengthAttribute.Arguments.Add(new CodeAttributeArgument(maxExpression));
			}
			if (string.IsNullOrWhiteSpace(ErrorMessage) == false)
			{
				maxLengthAttribute.Arguments.Add(new CodeAttributeArgument("ErrorMessage", new CodePrimitiveExpression(ErrorMessage)));
			}
			property.CustomAttributes.Add(maxLengthAttribute);
		}
	}
}
