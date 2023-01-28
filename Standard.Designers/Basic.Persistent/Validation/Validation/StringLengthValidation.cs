using Basic.Designer;
using System;
using System.CodeDom;
using System.ComponentModel;

namespace Basic.DataEntities
{
	/// <summary>
	/// 指定数据字段中允许的最小和最大字符长度。
	/// </summary>
	[PersistentDescription("DesignerValidation_StringLengthAttribute"), DefaultProperty("Maximum")]
	[TypeConverter(typeof(ValidationTypeConverter)), PersistentCategory("PersistentCategory_Attributes")]
	[System.Xml.Serialization.XmlRoot(XmlElementName), DisplayName("StringLength"),]
    public sealed class StringLengthValidation : AbstractValidationAttribute
	{
		internal const string XmlElementName = "StringLengthAttribute";
		private const string MaximumLengthAttribute = "MaximumLength";
		private const string MinimumLengthAttribute = "MinimumLength";
		/// <summary>
		/// 初始化 DesingerStringLength 类的新实例。
		/// </summary>
		/// <param name="property">当前验证器所属属性。</param>
		public StringLengthValidation(DataEntityPropertyElement property) : base(property) { }

		/// <summary>
		/// 异常关键字。
		/// </summary>
		public override string ErrorKey { get { return "StringLength"; } }

		private int _MaximumLength = 0;
		/// <summary>
		/// 获取或设置字符串的最大长度。
		/// </summary>
		/// <value>字符串的最大长度。</value>
		[PersistentDescription("DesignerValidation_MaximumLength"), DefaultValue(0)]
		public int MaximumLength
		{
			get { return _MaximumLength; }
			set
			{
				if (_MaximumLength != value && value >= 0)
				{
					_MaximumLength = value;
					if (_MinimumLength > value)
						MinimumLength = value;
					base.RaisePropertyChanged("MaximumLength");
				}
			}
		}

		private int _MinimumLength = 0;
		/// <summary>
		/// 获取或设置字符串的最小长度。
		/// </summary>
		/// <value>字符串的最小长度。</value>
		[PersistentDescription("DesignerValidation_MinimumLength"), DefaultValue(0)]
		public int MinimumLength
		{
			get { return _MinimumLength; }
			set
			{
				if (_MinimumLength != value && value >= 0)
				{
					_MinimumLength = value;
					if (_MaximumLength < value)
						MaximumLength = value;
					base.RaisePropertyChanged("MinimumLength");
				}
			}
		}

		/// <summary>
		/// 返回表示当前 DesingerStringLength 的 System.String。
		/// </summary>
		/// <returns>System.String，表示当前的 DesingerStringLength。</returns>
		public override string ToString()
		{
			if (MinimumLength > 0)
				return string.Concat("String Length ", MinimumLength, " To ", MaximumLength);
			return string.Concat("String Length <= ", MaximumLength);
		}

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == MaximumLengthAttribute) { _MaximumLength = Convert.ToInt32(value); return true; }
			else if (name == MinimumLengthAttribute) { _MinimumLength = Convert.ToInt32(value); return true; }
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			base.WriteAttribute(writer);
			if (MinimumLength > 0)
			{
				writer.WriteStartAttribute(MinimumLengthAttribute);
				writer.WriteValue(MinimumLength);
				writer.WriteEndAttribute();
			}
			if (MaximumLength > 0)
			{
				writer.WriteStartAttribute(MaximumLengthAttribute);
				writer.WriteValue(MaximumLength);
				writer.WriteEndAttribute();
			}
		}

		/// <summary>
		/// 将当前显示格式输出到属性的Attribute中。
		/// </summary>
		/// <param name="property">属性</param>
		protected internal override void WriteDesignerCodeAttribute(CodeMemberProperty property)
		{
			CodeTypeReference maxLengthTypeReference = new CodeTypeReference(typeof(Basic.Validations.StringLengthAttribute),
			CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration maxLengthAttribute = new CodeAttributeDeclaration(maxLengthTypeReference);

			CodePrimitiveExpression maxexpression = new CodePrimitiveExpression(_MaximumLength);
			maxLengthAttribute.Arguments.Add(new CodeAttributeArgument(maxexpression));
			if (_MinimumLength > 0)
			{
				CodePrimitiveExpression minexpresion = new CodePrimitiveExpression(_MinimumLength);
				maxLengthAttribute.Arguments.Add(new CodeAttributeArgument(minexpresion));
			}

			if (string.IsNullOrWhiteSpace(ErrorMessage) == false)
			{
				maxLengthAttribute.Arguments.Add(new CodeAttributeArgument("ErrorMessage", new CodePrimitiveExpression(ErrorMessage)));
			}
			property.CustomAttributes.Add(maxLengthAttribute);
		}
	}
}
