using Basic.Designer;
using System;
using System.CodeDom;
using System.ComponentModel;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示数组或字符串允许的最大长度的特性
	/// </summary>
	[PersistentDescription("DesignerValidation_MaxLengthAttribute"), DefaultProperty("Length")]
    [TypeConverter(typeof(ValidationTypeConverter)), PersistentCategory("PersistentCategory_Attributes")]
	[System.Xml.Serialization.XmlRoot(XmlElementName)]
    public sealed class MaxLengthValidation : AbstractValidationAttribute
	{
		internal const string XmlElementName = "MaxLengthAttribute";
		private const string LengthAttribute = "Length";
		/// <summary>
		/// 初始化 DesignerMaxLength 类的新实例。
		/// </summary>
		/// <param name="property">当前验证器所属属性。</param>
		public MaxLengthValidation(DataEntityPropertyElement property) : base(property) { }

		/// <summary>异常关键字</summary>
		public override string ErrorKey { get { return "MaxLength"; } }

		private int _Length = 0;
		/// <summary>
		/// 获取或设置数组或字符串允许的最大长度。
		/// </summary>
		/// <value>数组或字符串允许的最大长度。</value>
		[PersistentDescription("DesignerValidation_Length"), DefaultValue(0)]
		public int Length
		{
			get { return _Length; }
			set
			{
				if (_Length != value && value >= 0)
				{
					_Length = value;
					OnPropertyChanged("Length");
				}
			}
		}

		/// <summary>
		/// 返回表示当前 DesingerStringLength 的 System.String。
		/// </summary>
		/// <returns>System.String，表示当前的 DesingerStringLength。</returns>
		public override string ToString()
		{
			if (Length > 0) { return string.Concat("Max Length: ", Length); }
			return XmlElementName;
		}

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == LengthAttribute) { _Length = Convert.ToInt32(value); return true; }
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteXml(System.Xml.XmlWriter writer)
		{
			if (_Length > 0) { base.WriteXml(writer); }
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			base.WriteAttribute(writer);
			writer.WriteStartAttribute(LengthAttribute);
			writer.WriteValue(_Length);
			writer.WriteEndAttribute();
		}

		/// <summary>
		/// 将当前显示格式输出到属性的Attribute中。
		/// </summary>
		/// <param name="property">属性</param>
		protected internal override void WriteDesignerCodeAttribute(CodeMemberProperty property)
		{
			if (_Length > 0)
			{
				CodeTypeReference maxLengthTypeReference = new CodeTypeReference(typeof(Basic.Validations.MaxLengthAttribute),
				CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration maxLengthAttribute = new CodeAttributeDeclaration(maxLengthTypeReference);

				CodePrimitiveExpression expresion = new CodePrimitiveExpression(_Length);
				maxLengthAttribute.Arguments.Add(new CodeAttributeArgument(expresion));

				if (string.IsNullOrWhiteSpace(ErrorMessage) == false)
				{
					maxLengthAttribute.Arguments.Add(new CodeAttributeArgument("ErrorMessage", new CodePrimitiveExpression(ErrorMessage)));
				}
				property.CustomAttributes.Add(maxLengthAttribute);
			}
		}
	}
}
