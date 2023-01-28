using Basic.Designer;
using System.CodeDom;
using System.ComponentModel;

namespace Basic.DataEntities
{
	/// <summary>
	/// 指定用于验证关联成员的正则表达式。
	/// </summary>
    [PersistentDescription("DesignerValidation_BoolRequiredAttribute"), PersistentCategory("PersistentCategory_Attributes")]
	[DefaultProperty("Pattern"), DisplayName("RegularExpression"), TypeConverter(typeof(ValidationTypeConverter))]
	[System.Xml.Serialization.XmlRoot(XmlElementName)]
    public sealed class RegularExpressionValidation : AbstractValidationAttribute
	{
		internal const string XmlElementName = "RegularExpressionAttribute";
		private const string PatternAttribute = "Pattern";

		/// <summary>
		/// 初始化 RegularExpressionValidation 类的新实例。
		/// </summary>
		/// <param name="property">当前验证器所属属性。</param>
		public RegularExpressionValidation(DataEntityPropertyElement property) : base(property) { }

		/// <summary>
		/// 异常关键字。
		/// </summary>
		public override string ErrorKey { get { return "RegularExpression"; } }

		private string _Pattern = string.Empty;
		/// <summary>
		/// 获取用于验证关联成员的正则表达式。
		/// </summary>
		/// <value>用于验证关联成员的正则表达式。</value>
		[PersistentDescription("DesignerValidation_Pattern"), DefaultValue("")]
		public string Pattern
		{
			get { return _Pattern; }
			set
			{
				if (_Pattern != value)
				{
					_Pattern = value;
					base.RaisePropertyChanged("Pattern");
				}
			}
		}

		/// <summary>
		/// 返回表示当前 RegularExpressionAttribute 的 System.String。
		/// </summary>
		/// <returns>System.String，表示当前的 RegularExpressionAttribute。</returns>
		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(_Pattern)) { return _Pattern; }
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
			if (name == PatternAttribute) { _Pattern = value; return true; }
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteXml(System.Xml.XmlWriter writer)
		{
			if (!string.IsNullOrWhiteSpace(_Pattern)) { base.WriteXml(writer); }
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			base.WriteAttribute(writer);
			writer.WriteAttributeString(PatternAttribute, _Pattern);
		}

		/// <summary>
		/// 将当前显示格式输出到属性的Attribute中。
		/// </summary>
		/// <param name="property">属性</param>
		protected internal override void WriteDesignerCodeAttribute(CodeMemberProperty property)
		{
			if (!string.IsNullOrWhiteSpace(_Pattern))
			{
				CodeTypeReference regularExpressionTypeReference = new CodeTypeReference(typeof(Basic.Validations.RegularExpressionAttribute),
				CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration regularExpressionAttribute = new CodeAttributeDeclaration(regularExpressionTypeReference);

				CodePrimitiveExpression patternExpresion = new CodePrimitiveExpression(_Pattern);
				regularExpressionAttribute.Arguments.Add(new CodeAttributeArgument(patternExpresion));

				if (string.IsNullOrWhiteSpace(ErrorMessage) == false)
				{
					regularExpressionAttribute.Arguments.Add(new CodeAttributeArgument("ErrorMessage", new CodePrimitiveExpression(ErrorMessage)));
				}

				property.CustomAttributes.Add(regularExpressionAttribute);
			}
		}
	}
}
