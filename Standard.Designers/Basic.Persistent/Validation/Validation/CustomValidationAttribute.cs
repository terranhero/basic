using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using Basic.Designer;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示数组或字符串允许的最大长度的特性
	/// </summary>
	[PersistentDescription("DesignerValidation_MaxLengthAttribute"), DefaultProperty("Maximum")]
	[TypeConverter(typeof(ValidationTypeConverter)), PersistentCategory("PersistentCategory_Attributes")]
	[System.Xml.Serialization.XmlRoot(XmlElementName), DisplayName("Range"),]
	internal class CustomValidationAttribute : AbstractValidationAttribute
	{
		/// <summary>当前类使用Xml序列化后生成元素名称</summary>
		internal const string XmlElementName = "CustomValidationAttribute";
		private readonly List<string> _CustomValidations = new List<string>();
		private const string OtherPropertyAttribute = "OtherProperty";
		private const string RequiredValueAttribute = "RequiredValue";
		private const string PropertyTypeAttribute = "PropertyType";

		/// <summary>初始化 CustomValidationCollection 类的新实例</summary>
		/// <param name="nofity">当前验证器所属属性。</param>
		public CustomValidationAttribute(DataEntityPropertyElement nofity) : base(nofity) { }

		/// <summary>异常关键字</summary>
		public override string ErrorKey { get { return "CustomValidation"; } }

		/// <summary>将当前显示格式输出到属性的Attribute中</summary>
		/// <param name="property">属性</param>
		protected internal override void WriteDesignerCodeAttribute(CodeMemberProperty property)
		{
			foreach (string validation in _CustomValidations)
			{
				property.CustomAttributes.Add(new CodeAttributeDeclaration(validation));
			}
		}
	}
}
