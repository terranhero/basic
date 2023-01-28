using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Configuration;
using Basic.Collections;
using System.Xml;
using System.CodeDom;
using System.ComponentModel;
using Basic.Designer;
using Basic.EntityLayer;
using Basic.Enums;
using System.Windows;
using System.Drawing.Design;
using System.Runtime.Serialization;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示条件模型的定义。
	/// </summary>
	[TypeConverter(typeof(ConditionTypeConverter))]
	[PersistentCategoryAttribute(PersistentCategoryAttribute.CategoryCondition)]
	public class DataConditionElement : AbstractEntityElement
	{
		internal const string XmlElementName = "DataConditionElement";
		internal const string BaseClassAttribute = "BaseClass";
		private readonly DataConditionPropertyCollection propertyCollection;
		private readonly DataEntityElement dataEntityElement;
		/// <summary>
		/// 初始化 DataConditionElement 类的新实例。 
		/// </summary>
		public DataConditionElement(DataEntityElement entityElement)
			: base(entityElement.Persistent, typeof(AbstractCondition).FullName)
		{
			dataEntityElement = entityElement;
			propertyCollection = new DataConditionPropertyCollection(this);
		}

		internal PersistentConfiguration Persistent { get { return dataEntityElement.Persistent; } }
		/// <summary>
		/// 结果类实例
		/// </summary>
		[System.ComponentModel.Browsable(false), System.ComponentModel.Bindable(true)]
		public DataConditionPropertyCollection Arguments { get { return propertyCollection; } }

		/// <summary>
		/// 当前条件元素是否显示
		/// </summary>
		public Visibility Visibility
		{
			get
			{
				foreach (DataCommandElement dataCommand in dataEntityElement.DataCommands)
				{
					if (dataCommand is DynamicCommandElement) { return System.Windows.Visibility.Visible; }
					StaticCommandElement staticCommand = dataCommand as StaticCommandElement;
					if (staticCommand != null && staticCommand.ExecutableMethod == StaticMethodEnum.FillDataSet)
						return Visibility.Visible;
					else if (staticCommand != null && staticCommand.ExecutableMethod == StaticMethodEnum.FillDataTable)
						return Visibility.Visible;
					else if (staticCommand != null && staticCommand.ExecutableMethod == StaticMethodEnum.GetPagination)
						return Visibility.Visible;
				}
				return Visibility.Collapsed;
			}
		}

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return XmlElementName.Replace("Element", ""); }

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName { get { return XmlElementName; } }

		/// <summary>
		/// 获取或设置一个值，表示实体类名称
		/// </summary>
		[System.ComponentModel.DefaultValue(""), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDescription("PropertyDescription_Name")]
		[System.ComponentModel.Browsable(false)]
		public new string Name
		{
			get
			{
				return dataEntityElement.Name;
			}
		}

		/// <summary>
		/// Gets or sets the EntityDefinition TableName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		public override string TableName
		{
			get
			{
				return dataEntityElement.TableName;
			}
			set
			{
				dataEntityElement.TableName = value;
			}
		}

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[System.ComponentModel.Browsable(false)]
		public override string ClassName { get { return EntityName; } }

		/// <summary>
		/// 获取或设置当前实体类的基类类型.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_BaseClass")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[System.ComponentModel.DefaultValue("Basic.EntityLayer.AbstractCondition")]
		[System.ComponentModel.Editor(typeof(BaseConditionSelector), typeof(UITypeEditor))]
		public override string BaseClass
		{
			get { return base.BaseClass; }
			set { base.BaseClass = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[System.ComponentModel.DefaultValue(false)]
		public override string EntityName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Name) == false)
				{
					if (Name.EndsWith("Info", StringComparison.OrdinalIgnoreCase))
					{
						return string.Concat(Name.TrimEnd(new char[] { 'I', 'i', 'N', 'n', 'F', 'f', 'O', 'o' }), "Condition");
					}
					return string.Concat(Name, "Condition");
				}
				return base.BaseClassName;
			}
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		protected internal override bool ReadContent(XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DataConditionPropertyCollection.XmlElementName)
			{
				XmlReader reader2 = reader.ReadSubtree();
				while (reader2.Read())
				{
					if (reader2.NodeType == XmlNodeType.Whitespace) { continue; }
					else if (reader2.NodeType == XmlNodeType.Element && reader2.LocalName == DataConditionPropertyElement.XmlElementName)
					{
						DataConditionPropertyElement element = new DataConditionPropertyElement(this);
						element.ReadXml(reader2.ReadSubtree());
						propertyCollection.Add(element);
					}
					else if (reader2.NodeType == XmlNodeType.EndElement && reader2.LocalName == DataEntityPropertyCollection.XmlElementName)
					{
						break;
					}
				}
			}
			return base.ReadContent(reader);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			base.WriteContent(writer);
			writer.WriteStartElement(DataConditionPropertyCollection.XmlElementName);
			foreach (DataConditionPropertyElement property in propertyCollection)
				property.WriteXml(writer);
			writer.WriteEndElement();
		}

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		/// <param name="codeNamespace">表示需要写入代码的命名空间</param>
		protected internal override CodeTypeDeclaration WriteEntityDesignerCode(CodeNamespace codeNamespace)
		{
			CodeTypeDeclaration entityClass = base.WriteEntityDesignerCode(codeNamespace);
			//IEnumerable<DataConditionPropertyElement> list = propertyCollection.Where(m => m.DataMember == true);
			//if (list.Count() > 0)
			//{
			//    CodeTypeReference dataContractTypeReference = new CodeTypeReference(typeof(DataContractAttribute), CodeTypeReferenceOptions.GlobalReference);
			//    CodeAttributeDeclaration dataContractAttribute = new CodeAttributeDeclaration(dataContractTypeReference);
			//    entityClass.CustomAttributes.Add(dataContractAttribute);
			//}

			CodeTypeReference groupNameTypeReference = new CodeTypeReference(typeof(GroupNameAttribute), CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration groupNameAttribute = new CodeAttributeDeclaration(groupNameTypeReference);
			groupNameAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(Persistent.EntityName)));
			if (!string.IsNullOrWhiteSpace(Persistent.MessageConverter.ConverterName))
			{
				groupNameAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(Persistent.MessageConverter.ConverterName)));
			}
			entityClass.CustomAttributes.Add(groupNameAttribute);

			CodeConstructor constructor = new CodeConstructor();
			constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor.Comments.Add(new CodeCommentStatement(string.Format("初始化 {0} 类的实例。", EntityName), true));
			constructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;

			constructor.BaseConstructorArgs.Add(new CodePrimitiveExpression(propertyCollection.Count));
			entityClass.Members.Add(constructor);

			foreach (DataConditionPropertyElement property in propertyCollection)
			{
				property.WriteDesignerCode(entityClass, null);
			}
			entityClass.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));
			return entityClass;
		}

		/// <summary>
		/// 返回表示当前 DataConditionElement 的 System.String。
		/// </summary>
		/// <returns>System.String，表示当前的 DataConditionElement。</returns>
		public override string ToString()
		{
			return EntityName;
		}
	}
}
