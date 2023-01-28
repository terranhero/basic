using System;
using System.CodeDom;
using System.ComponentModel;
using System.Xml;
using Basic.Collections;
using Basic.Designer;

namespace Basic.DataEntities
{
	/// <summary>
	/// 实体类属性信息
	/// </summary>
	public sealed partial class DataEntityPropertyElement : AbstractPropertyElement
	{
		private readonly DataEntityElement dataEntityElement;
		private readonly AbstractValidationCollection abstractAttributes;

		#region 构造函数
		/// <summary>
		/// Initializes a new instance of a DataEntityPropertyElement object.
		/// </summary>
		/// <param name="owner">拥有此属性的实体定义文件</param>
		internal DataEntityPropertyElement(DataEntityElement owner) : this(owner, null, typeof(string), false) { }

		/// <summary>
		/// Initializes a new instance of a DataEntityPropertyElement object.
		/// </summary>
		/// <param name="owner">拥有此属性的实体定义文件</param>
		/// <param name="name">连接字符串的名称。</param>
		internal DataEntityPropertyElement(DataEntityElement owner, string name) : this(owner, name, typeof(string), false) { }

		/// <summary>
		/// Initializes a new instance of a DataEntityPropertyElement object.
		/// </summary>
		/// <param name="owner">拥有此属性的实体定义文件</param>
		/// <param name="name">连接字符串的名称。</param>
		/// <param name="type">属性类型。</param>
		internal DataEntityPropertyElement(DataEntityElement owner, string name, Type type) : this(owner, name, type, false) { }

		/// <summary>
		/// Initializes a new instance of a DataEntityPropertyElement object.
		/// </summary>
		/// <param name="owner">拥有此属性的实体定义文件</param>
		/// <param name="name">属性名称。</param>
		/// <param name="type">属性类型。</param>
		/// <param name="nullable">属性是否不能为空。</param>
		internal DataEntityPropertyElement(DataEntityElement owner, string name, Type type, bool nullable)
			: base(owner, name, type, nullable)
		{
			this.dataEntityElement = owner;
			abstractAttributes = new AbstractValidationCollection(this);
		}
		#endregion

		/// <summary>
		/// 字段前缀，主要用于查询
		/// </summary>
		[PersistentDescription("PropertyDescription_Attributes"), PersistentCategory("PersistentCategory_Attributes")]
		//[Editor(typeof(ValidationAttributesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[Editor(typeof(ValidationAttributesListEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public AbstractValidationCollection Attributes { get { return abstractAttributes; } set { } }

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		/// <param name="entityClass">表示需要写入属性代码的类型定义</param>
		/// <param name="pkConstructor">如果实体存在主键则</param>
		/// <returns></returns>
		protected internal override CodeMemberProperty WriteDesignerCode(CodeTypeDeclaration entityClass, CodeConstructor pkConstructor)
		{
			CodeMemberProperty property = base.WriteDesignerCode(entityClass, pkConstructor);
			foreach (AbstractAttribute aa in abstractAttributes)
			{
				aa.WriteDesignerCodeAttribute(property);
			}
			return property;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteContent(XmlWriter writer)
		{
			base.WriteContent(writer);
			foreach (AbstractAttribute aa in abstractAttributes)
			{
				aa.WriteXml(writer);
			}
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected internal override bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DisplayFormat.XmlElementName)
			{
				DisplayFormat displayFormat = new DisplayFormat(this);
				displayFormat.ReadXml(reader.ReadSubtree());
				abstractAttributes.Add(displayFormat);
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == ImportPorpertyAttribute.XmlElementName)
			{
				ImportPorpertyAttribute validation = new ImportPorpertyAttribute(this);
				validation.ReadXml(reader.ReadSubtree());
				abstractAttributes.Add(validation);
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == RequiredValidation.XmlElementName)
			{
				RequiredValidation validation = new RequiredValidation(this);
				validation.ReadXml(reader.ReadSubtree());
				abstractAttributes.Add(validation);
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == BoolRequiredValidation.XmlElementName)
			{
				BoolRequiredValidation validation = new BoolRequiredValidation(this);
				validation.ReadXml(reader.ReadSubtree());
				abstractAttributes.Add(validation);
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == CompareValidation.XmlElementName)
			{
				CompareValidation validation = new CompareValidation(this);
				validation.ReadXml(reader.ReadSubtree());
				abstractAttributes.Add(validation);
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == RangeValidation.XmlElementName)
			{
				RangeValidation validation = new RangeValidation(this);
				validation.ReadXml(reader.ReadSubtree());
				abstractAttributes.Add(validation);
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == RegularExpressionValidation.XmlElementName)
			{
				RegularExpressionValidation validation = new RegularExpressionValidation(this);
				validation.ReadXml(reader.ReadSubtree());
				abstractAttributes.Add(validation);
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == MaxLengthValidation.XmlElementName)
			{
				MaxLengthValidation validation = new MaxLengthValidation(this);
				validation.ReadXml(reader.ReadSubtree());
				abstractAttributes.Add(validation);
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == StringLengthValidation.XmlElementName)
			{
				StringLengthValidation validation = new StringLengthValidation(this);
				validation.ReadXml(reader.ReadSubtree());
				abstractAttributes.Add(validation);
			}
			return base.ReadContent(reader);
		}
	}
}
