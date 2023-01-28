using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Designer;
using System.ComponentModel;
using System.CodeDom;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示实体类属性导入配置特性信息。
	/// </summary>
	[PersistentDescription("DesignerValidation_ImportPorpertyAttribute"), PersistentCategory("PersistentCategory_Attributes")]
	[DefaultProperty("Index"), DisplayName("Import"), TypeConverter(typeof(ValidationTypeConverter))]
	[System.Xml.Serialization.XmlRoot(XmlElementName)]
	public sealed class ImportPorpertyAttribute : AbstractAttribute
	{
		/// <summary>
		/// 当前类使用Xml序列化后生成元素名称。
		/// </summary>
		internal const string XmlElementName = "ImportAttribute";
		private const string IndexAttribute = "Index";
		private const string RequiredAttribute = "Required";
		private const string CheckedAttribute = "Checked";

		/// <summary>
		/// 初始化 BoolReqiuredValidation 类的新实例。
		/// </summary>
		/// <param name="property">当前验证器所属属性。</param>
		public ImportPorpertyAttribute(DataEntityPropertyElement nofity) : base(nofity) { }

		/// <summary>
		/// 返回表示当前 DesignerBoolReqiured 的 System.String。
		/// </summary>
		/// <returns>System.String，表示当前的 DesignerBoolReqiured。</returns>
		public override string ToString()
		{
			if (m_Index >= 0)
			{
				return string.Concat("Index Of ", XmlElementName.Replace("Attribute", ":"), m_Index);
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
			if (name == IndexAttribute) { m_Index = Convert.ToInt32(value); return true; }
			else if (name == RequiredAttribute) { m_Required = Convert.ToBoolean(value); return true; }
			else if (name == CheckedAttribute) { m_Checked = Convert.ToBoolean(value); return true; }
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			base.WriteAttribute(writer);
			if (m_Index >= 0)
			{
				writer.WriteStartAttribute(IndexAttribute);
				writer.WriteValue(m_Index);
				writer.WriteEndAttribute();
			}

			if (m_Required)
			{
				writer.WriteStartAttribute(RequiredAttribute);
				writer.WriteValue(m_Required);
				writer.WriteEndAttribute();
			}

			if (!m_Checked)
			{
				writer.WriteStartAttribute(CheckedAttribute);
				writer.WriteValue(m_Checked);
				writer.WriteEndAttribute();
			}
		}

		/// <summary>
		/// 当前属性导入时指定导入数据源的索引。
		/// </summary>
		private int m_Index = -1;
		/// <summary>
		/// 获取或设置当前属性导入时指定导入数据源的索引。
		/// </summary>
		[PersistentDescription("DesignerValidation_Index"), DefaultValue(-1)]
		public int Index
		{
			get { return m_Index; }
			set
			{
				if (m_Index != value)
				{
					m_Index = value;
					base.RaisePropertyChanged("Index");
				}
			}
		}

		/// <summary>
		/// 确定当前属性是否必须导入。
		/// </summary>
		private bool m_Checked = true;
		/// <summary>
		/// 获取或设置当前属性是否需要导入。
		/// </summary>
		[PersistentDescription("DesignerValidation_Checked"), DefaultValue(true)]
		public bool Checked
		{
			get { return m_Checked; }
			set
			{
				if (m_Checked != value)
				{
					m_Checked = value;
					base.RaisePropertyChanged("Checked");
				}
			}
		}

		/// <summary>
		/// 确定当前属性是否必须导入。
		/// </summary>
		private bool m_Required = false;
		/// <summary>
		/// 获取或设置当前属性是否必须导入。
		/// </summary>
		[PersistentDescription("DesignerValidation_Required"), DefaultValue(false)]
		public bool Required
		{
			get { return m_Required; }
			set
			{
				if (m_Required != value)
				{
					m_Required = value;
					base.RaisePropertyChanged("Reqiured");
				}
			}
		}

		/// <summary>
		/// 将当前显示格式输出到属性的Attribute中。
		/// </summary>
		/// <param name="property">属性</param>
		protected internal override void WriteDesignerCodeAttribute(System.CodeDom.CodeMemberProperty property)
		{
			if (m_Index >= 0)
			{
				CodeTypeReference importAttributeReference = new CodeTypeReference(typeof(Basic.EntityLayer.ImportAttribute),
				CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration importAttribute = new CodeAttributeDeclaration(importAttributeReference);

				CodePrimitiveExpression indexExpresion = new CodePrimitiveExpression(m_Index);
				importAttribute.Arguments.Add(new CodeAttributeArgument(indexExpresion));
				if (m_Required)
				{
					importAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(m_Required)));
				}
				if (!m_Checked)
				{
					if (importAttribute.Arguments.Count == 1)
						importAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(false)));
					importAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(m_Checked)));
				}
				property.CustomAttributes.Add(importAttribute);
			}
		}
	}
}
