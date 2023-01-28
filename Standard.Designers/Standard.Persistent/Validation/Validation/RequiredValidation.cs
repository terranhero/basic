using Basic.Designer;
using System;
using System.CodeDom;
using System.ComponentModel;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示数组或字符串允许的最大长度的特性
	/// </summary>
	[PersistentDescription("DesignerValidation_BoolRequiredAttribute"), PersistentCategory("PersistentCategory_Attributes")]
	[DisplayName("Reqiured"), TypeConverter(typeof(ValidationTypeConverter))]
	[System.Xml.Serialization.XmlRoot(XmlElementName)]
	public sealed class RequiredValidation : AbstractValidationAttribute
	{
		/// <summary>
		/// 当前类使用Xml序列化后生成元素名称。
		/// </summary>
		internal const string XmlElementName = "RequiredAttribute";

		/// <summary>
		/// 是否允许空字符串
		/// </summary>
		internal const string AllowEmptyAttribute = "AllowEmpty";

		/// <summary>
		/// 异常关键字。
		/// </summary>
		public override string ErrorKey { get { return "Required"; } }

		/// <summary>
		/// 初始化 RequiredValidation 类的新实例。
		/// </summary>
		/// <param name="property">当前验证器所属属性。</param>
		public RequiredValidation(DataEntityPropertyElement nofity) : base(nofity) { }

		/// <summary>
		/// 返回表示当前 DesignerBoolReqiured 的 System.String。
		/// </summary>
		/// <returns>System.String，表示当前的 DesignerBoolReqiured。</returns>
		public override string ToString()
		{
			return XmlElementName.Replace("Attribute", "");
		}

		private bool _AllowEmpty = false;
		/// <summary>
		/// 获取所允许的最小字段值。
		/// </summary>
		/// <value>所允许的数据字段最小值。</value>
		[PersistentDescription("RequiredValidation_AllowEmptyStrings"), DefaultValue(false)]
		public bool AllowEmpty
		{
			get { return _AllowEmpty; }
			set
			{
				if (_AllowEmpty != value)
				{
					_AllowEmpty = value;
					base.RaisePropertyChanged("AllowEmpty");
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
			if (name == AllowEmptyAttribute) { return Boolean.TryParse(value, out _AllowEmpty); }
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			base.WriteAttribute(writer);
			if (_AllowEmpty)
			{
				writer.WriteStartAttribute(AllowEmptyAttribute);
				writer.WriteValue(_AllowEmpty);
				writer.WriteEndAttribute();
			}
		}

		/// <summary>
		/// 将当前显示格式输出到属性的Attribute中。
		/// </summary>
		/// <param name="property">属性</param>
		protected internal override void WriteDesignerCodeAttribute(CodeMemberProperty property)
		{
			CodeTypeReference requiredTypeReference = new CodeTypeReference(typeof(Basic.Validations.RequiredAttribute),
			CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration requiredAttribute = new CodeAttributeDeclaration(requiredTypeReference);
			if (_AllowEmpty)
			{
				CodePrimitiveExpression allowEmptyExpresion = new CodePrimitiveExpression(_AllowEmpty);
				requiredAttribute.Arguments.Add(new CodeAttributeArgument(allowEmptyExpresion));
			}
			property.CustomAttributes.Add(requiredAttribute);
		}
	}
}
