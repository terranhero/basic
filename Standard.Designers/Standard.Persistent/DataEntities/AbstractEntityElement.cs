using System;
using System.CodeDom;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Xml;
using Basic.Designer;
using Basic.EntityLayer;
using Basic.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Basic.Configuration;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示数据实体抽象类
	/// </summary>
	public abstract class AbstractEntityElement : AbstractCustomTypeDescriptor
	{
		#region Xml节点常量
		internal const string NameAttribute = "Name";
		internal const string GuidAttribute = "Guid";
		internal const string IsAbstractAttribute = "Abstract";
		internal const string ExpandedAttribute = "Expanded";
		internal const string ModifiedAttribute = "Modified";
		internal const string TableNameAttribute = "TableName";
		internal const string CommentElement = "Comment";
		internal const string BaseClassElement = "BaseClass";
		#endregion
		private readonly PersistentConfiguration persistent;
		/// <summary>
		/// 初始化 AbstractEntityElement 类实例。
		/// </summary>
		/// <param name="nofity">当前配置文件，此配置文件为当前实体更改后需要通知的对象。</param>
		/// <param name="baseClass">当前实体类基类</param>
		protected AbstractEntityElement(PersistentConfiguration nofity, string baseClass)
			: base(nofity)
		{
			persistent = nofity;
			_BaseClass = baseClass;
		}

		private bool _Expanded = false;
		/// <summary>获取或设置命令描述</summary>
		[System.ComponentModel.Browsable(false)]
		public bool Expanded
		{
			get
			{
				return _Expanded;
			}
			set
			{
				_Expanded = value; RaisePropertyChanged("Expanded");
			}
		}

		/// <summary>
		/// 返回此组件实例的名称。
		public override string GetComponentName() { return Name; }

		/// <summary>
		/// 获取当前节点元素命名空间
		/// </summary>
		protected internal override string ElementNamespace { get { return null; } }

		/// <summary>
		/// 获取实体类的完全限定名，包括实体类的命名空间，但不包括程序集。
		/// </summary>
		internal string FullName { get { return string.Concat(persistent.EntityNamespace, ".", ClassName); } }
		private string _OldName = string.Empty;
		/// <summary>
		/// 获取或设置一个值，表示实体类名称
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		protected internal string OldName { get { return _OldName; } }

		private System.Guid _Guid = Guid.Empty;
		/// <summary>
		/// 获取或设置一个值，表示实体类型Guid。
		/// </summary>
		[System.ComponentModel.DefaultValue(""), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PropertyDescription_Guid")]
		[System.ComponentModel.Browsable(false)]
		public System.Guid Guid
		{
			get { return _Guid; }
			set { if (_Guid != value) { _Guid = value; base.RaisePropertyChanged("Guid"); } }
		}

		/// <summary>
		/// 获取或设置一个值，表示实体类名称
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		protected internal bool NameChanged { get { return _OldName != _Name; } }

		private string _Name = string.Empty;
		/// <summary>
		/// 获取或设置一个值，表示实体类名称
		/// </summary>
		[System.ComponentModel.DefaultValue(""), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDescription("PropertyDescription_Name")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_EntityName")]
		public virtual string Name
		{
			get { return _Name; }
			set
			{
				if (_Name != value)
				{
					_Name = value;
					base.RaisePropertyChanged("Name");
					base.RaisePropertyChanged("ClassName");
				}
			}
		}

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_ClassName")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[System.ComponentModel.Browsable(false)]
		public virtual string ClassName { get { return string.Concat(Name, "Entity"); } }

		private ClassModifierEnum _Modifier = ClassModifierEnum.Public;
		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.DefaultValue(typeof(ClassModifierEnum), "Public")]
		[System.ComponentModel.Bindable(true), System.ComponentModel.Browsable(false)]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		public ClassModifierEnum Modifier
		{
			get { return _Modifier; }
			set
			{
				if (_Modifier != value)
				{
					_Modifier = value;
					RaisePropertyChanged("Modifier");
				}
			}
		}

		private bool _IsAbstract = false;
		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_IsAbstract")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[System.ComponentModel.DefaultValue(false), System.ComponentModel.Browsable(false)]
		public bool IsAbstract
		{
			get { return _IsAbstract; }
			set
			{
				if (_IsAbstract != value)
				{
					_IsAbstract = value;
					RaisePropertyChanged("IsAbstract");
				}
			}
		}

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[System.ComponentModel.Browsable(false)]
		public virtual string EntityName { get { return string.Concat(Name, "Entity"); } }

		/// <summary>
		/// 获取实体类元数据定义类型名称
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public virtual string MetadataName { get { return string.Concat(EntityName, "Metadata"); } }

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[System.ComponentModel.Browsable(false)]
		internal protected virtual string OldEntityName { get { return string.Concat(OldName, "Entity"); } }

		private string _BaseClass = typeof(AbstractEntity).FullName;
		/// <summary>
		/// 获取或设置当前实体类的基类类型.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
		[System.ComponentModel.DefaultValue("Basic.EntityLayer.AbstractEntity")]
		[System.ComponentModel.Editor(typeof(BaseClassSelector), typeof(UITypeEditor))]
		public virtual string BaseClass
		{
			get { return _BaseClass; }
			set
			{
				if (_BaseClass != value)
				{
					_BaseClass = value;
					RaisePropertyChanged("BaseClass");
					RaisePropertyChanged("BaseClassName");
				}
			}
		}

		/// <summary>
		/// Gets or sets the EntityDefinition BaseType.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[System.ComponentModel.DefaultValue("AbstractEntity"), System.ComponentModel.Browsable(false)]
		public string BaseClassName
		{
			get
			{
				if (_BaseClass != null)
					return _BaseClass;
				return null;
			}
		}

		private string _TableName = null;
		/// <summary>
		/// Gets or sets the EntityDefinition TableName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_TableName")]
		[System.ComponentModel.DefaultValue("")]
		public virtual string TableName
		{
			get { return _TableName; }
			set
			{
				if (_TableName != value)
				{
					_TableName = value;
					RaisePropertyChanged("TableName");
				}
			}
		}

		private string _Comment = null;
		/// <summary>
		/// 备注，描述。
		/// </summary>
		[Basic.Designer.PersistentDescription("PersistentDescription_Comment")]
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		[Basic.Designer.PersistentDisplay("PersistentDisplay_EntityComment")]
		[System.ComponentModel.DefaultValue("")]
		public string Comment
		{
			get { return _Comment; }
			set
			{
				if (_Comment != value)
				{
					_Comment = value; RaisePropertyChanged("Comment");
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
			if (name == NameAttribute) { _Name = _OldName = value; return true; }
			else if (name == ExpandedAttribute) { return bool.TryParse(value, out _Expanded); }
			else if (name == GuidAttribute) { return Guid.TryParse(value, out _Guid); }
			else if (name == ModifiedAttribute) { return Enum.TryParse<ClassModifierEnum>(value, true, out _Modifier); }
			else if (name == TableNameAttribute) { _TableName = value; return true; }
			else if (name == IsAbstractAttribute) { return bool.TryParse(value, out _IsAbstract); }
			return false;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			writer.WriteElementString(BaseClassElement, _BaseClass);
			writer.WriteElementString(CommentElement, _Comment);
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		protected internal override bool ReadContent(XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == BaseClassElement)
			{
				_BaseClass = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == CommentElement)
			{
				_Comment = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == ElementName)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			if (_Guid != Guid.Empty) { writer.WriteAttributeString(GuidAttribute, _Guid.ToString("D")); }
			writer.WriteAttributeString(NameAttribute, Name);
			writer.WriteAttributeString(TableNameAttribute, _TableName);
			if (_IsAbstract) { writer.WriteAttributeString(IsAbstractAttribute, Convert.ToString(_IsAbstract).ToLower()); }
			if (_Modifier != ClassModifierEnum.Public) { writer.WriteAttributeString(ModifiedAttribute, _Modifier.ToString()); }
			if (Expanded) { writer.WriteAttributeString(ExpandedAttribute, Expanded.ToString().ToLower()); }
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType) { }

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteXml(XmlWriter writer)
		{
			base.WriteXml(writer);
			_OldName = _Name;
		}

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		/// <param name="codeNamespace">表示需要写入代码的命名空间</param>
		protected internal virtual CodeTypeDeclaration WriteEntityCode(CodeNamespace codeNamespace)
		{
			CodeTypeDeclaration entityClass = new CodeTypeDeclaration(EntityName);
			entityClass.Comments.Add(new CodeCommentStatement("<summary>", true));
			if (string.IsNullOrWhiteSpace(Comment))
				entityClass.Comments.Add(new CodeCommentStatement(EntityName, true));
			else
				entityClass.Comments.Add(new CodeCommentStatement(Comment, true));
			entityClass.Comments.Add(new CodeCommentStatement("</summary>", true));

			//CodeTypeReference metadataTypeReference = new CodeTypeReference(typeof(MetadataTypeAttribute));
			//CodeAttributeDeclaration metadataTypeAttribute = new CodeAttributeDeclaration(metadataTypeReference);
			//CodeTypeOfExpression typeofExpression = new CodeTypeOfExpression(string.Concat(EntityName, ".", MetadataName));
			//metadataTypeAttribute.Arguments.Add(new CodeAttributeArgument(typeofExpression));
			//entityClass.CustomAttributes.Add(metadataTypeAttribute);

			entityClass.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Declaration", EntityName)));
			entityClass.IsPartial = true;
			entityClass.IsClass = true;
			entityClass.TypeAttributes = TypeAttributes.Class;
			if (_IsAbstract)
			{
				entityClass.TypeAttributes = entityClass.TypeAttributes | TypeAttributes.Abstract;
				entityClass.Attributes = MemberAttributes.Abstract;
			}
			if (_Modifier == ClassModifierEnum.Internal)
			{
				entityClass.TypeAttributes = entityClass.TypeAttributes | TypeAttributes.NestedAssembly;
				entityClass.Attributes = entityClass.Attributes | MemberAttributes.Assembly;
			}
			else if (_Modifier == ClassModifierEnum.Public)
			{
				entityClass.TypeAttributes = entityClass.TypeAttributes | TypeAttributes.Public;
				entityClass.Attributes = entityClass.Attributes | MemberAttributes.Public;
			}
			entityClass.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));
			CodeTypeDeclaration metadataClass = new CodeTypeDeclaration(MetadataName);
			metadataClass.IsClass = true;
			metadataClass.TypeAttributes = TypeAttributes.Sealed | TypeAttributes.NotPublic;
			metadataClass.Attributes = MemberAttributes.FamilyOrAssembly;
			entityClass.Members.Add(metadataClass);
			codeNamespace.Types.Add(entityClass);
			return entityClass;
		}

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		/// <param name="codeNamespace">表示需要写入代码的命名空间</param>
		protected internal virtual CodeTypeDeclaration WriteEntityDesignerCode(CodeNamespace codeNamespace)
		{
			CodeTypeDeclaration entityClass = new CodeTypeDeclaration(EntityName);
			codeNamespace.Types.Add(entityClass);
			entityClass.Comments.Add(new CodeCommentStatement("<summary>", true));
			if (string.IsNullOrWhiteSpace(Comment))
				entityClass.Comments.Add(new CodeCommentStatement(EntityName, true));
			else
				entityClass.Comments.Add(new CodeCommentStatement(Comment, true));
			entityClass.Comments.Add(new CodeCommentStatement("</summary>", true));
			entityClass.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Declaration", EntityName)));
			entityClass.BaseTypes.Add(new CodeTypeReference(BaseClass, CodeTypeReferenceOptions.GlobalReference));
			entityClass.IsPartial = true;
			entityClass.IsClass = true;
			entityClass.TypeAttributes = TypeAttributes.Class;
			if (_IsAbstract)
			{
				entityClass.TypeAttributes = entityClass.TypeAttributes | TypeAttributes.Abstract;
				entityClass.Attributes = MemberAttributes.Abstract;
			}
			if (_Modifier == ClassModifierEnum.Internal)
			{
				entityClass.TypeAttributes = entityClass.TypeAttributes | TypeAttributes.NestedAssembly;
				entityClass.Attributes = entityClass.Attributes | MemberAttributes.Assembly;
			}
			else if (_Modifier == ClassModifierEnum.Public)
			{
				entityClass.TypeAttributes = entityClass.TypeAttributes | TypeAttributes.Public;
				entityClass.Attributes = entityClass.Attributes | MemberAttributes.Public;
			}

			CodeTypeReference serializableTypeReference = new CodeTypeReference(typeof(SerializableAttribute), CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration serializableAttribute = new CodeAttributeDeclaration(serializableTypeReference);
			entityClass.CustomAttributes.Add(serializableAttribute);
			CodeTypeReference toolboxItemTypeReference = new CodeTypeReference(typeof(ToolboxItemAttribute), CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration toolboxItemAttribute = new CodeAttributeDeclaration(toolboxItemTypeReference);
			toolboxItemAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(false)));
			entityClass.CustomAttributes.Add(toolboxItemAttribute);

			CodeTypeReference guidAttributeReference = new CodeTypeReference(typeof(GuidAttribute), CodeTypeReferenceOptions.GlobalReference);
			CodeAttributeDeclaration guidAttribute = new CodeAttributeDeclaration(guidAttributeReference);
			guidAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_Guid.ToString("D").ToUpper())));
			entityClass.CustomAttributes.Add(guidAttribute);

			if (!string.IsNullOrWhiteSpace(TableName))
			{
				CodeTypeReference tableTypeReference = new CodeTypeReference(typeof(TableMappingAttribute), CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration tableAttribute = new CodeAttributeDeclaration(tableTypeReference);
				tableAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(TableName)));
				entityClass.CustomAttributes.Add(tableAttribute);
			}
			return entityClass;
		}
	}
}
