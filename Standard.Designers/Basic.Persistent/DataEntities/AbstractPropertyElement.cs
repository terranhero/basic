using Basic.Designer;
using Basic.EntityLayer;
using Basic.Enums;
using EnvDTE;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.IO;
using System.Runtime.Serialization;

namespace Basic.DataEntities
{
	/// <summary>
	/// 表示实体或条件属性信息
	/// </summary>
	public abstract partial class AbstractPropertyElement : AbstractCustomTypeDescriptor
	{
		#region 实体定义字段
		private readonly AbstractEntityElement abstractEntityElement = null;
		protected internal readonly DisplayNameElement displayName;
		private readonly PropertyGeneratorElement generatorElement;
		internal const string XmlElementName = "Property";
		internal const string NameAttribute = "Name";
		internal const string TypeAttribute = "Type";
		internal const string NullableAttribute = "Nullable";
		internal const string InheritanceAttribute = "Inherit";
		internal const string CommentElement = "Comment";
		#endregion

		#region 构造函数

		/// <summary>
		/// 属性所属实体类实例
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public AbstractEntityElement Owner { get { return abstractEntityElement; } }

		/// <summary>
		/// Initializes a new instance of a EntityDefinitionProperty object.
		/// </summary>
		/// <param name="owner">拥有此属性的实体定义文件</param>
		protected AbstractPropertyElement(AbstractEntityElement owner) : this(owner, null, typeof(string), false) { }

		/// <summary>
		/// Initializes a new instance of a EntityDefinitionProperty object.
		/// </summary>
		/// <param name="owner">拥有此属性的实体定义文件</param>
		/// <param name="name">连接字符串的名称。</param>
		protected AbstractPropertyElement(AbstractEntityElement owner, string name) : this(owner, name, typeof(string), false) { }

		/// <summary>
		/// Initializes a new instance of a EntityDefinitionProperty object.
		/// </summary>
		/// <param name="owner">拥有此属性的实体定义文件</param>
		/// <param name="name">连接字符串的名称。</param>
		/// <param name="type">属性类型。</param>
		protected AbstractPropertyElement(AbstractEntityElement owner, string name, Type type) : this(owner, name, type, false) { }

		/// <summary>
		/// Initializes a new instance of a EntityDefinitionProperty object.
		/// </summary>
		/// <param name="owner">拥有此属性的实体定义文件</param>
		/// <param name="name">属性名称。</param>
		/// <param name="type">属性类型。</param>
		/// <param name="nullable">属性是否不能为空。</param>
		protected AbstractPropertyElement(AbstractEntityElement owner, string name, Type type, bool nullable)
		{
			generatorElement = new PropertyGeneratorElement(this);
			displayName = new DisplayNameElement(this);
			abstractEntityElement = owner;
			Name = name;
			Type = type;
			Nullable = nullable;
		}
		#endregion

		#region 方法重载
		/// <summary>
		/// 将当前实例的属性复制到 AbstractPropertyElement 类实例的属性上。
		/// </summary>
		/// <param name="property">需要复制当前实例属性的 AbstractPropertyElement 类实例</param>
		public virtual void CopyTo(AbstractPropertyElement property)
		{
			property._Column = this._Column;
			property._Comment = this._Comment;
			property._DbType = this._DbType;
			property.Inheritance = this.Inheritance;
			property.Modifier = this.Modifier;
			property.Override = this.Override;
			property._Name = this._Name;
			property._Nullable = this._Nullable;
			property._Precision = this._Precision;
			property._PrimaryKey = this._PrimaryKey;
			property._Profix = this._Profix;
			property._Scale = this._Scale;
			property._Size = this._Size;
			property._Type = this._Type;
		}
		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return XmlElementName; }

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName() { return Name; }

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName { get { return XmlElementName; } }

		/// <summary>
		/// 获取当前节点元素命名空间
		/// </summary>
		protected internal override string ElementNamespace { get { return null; } }

		/// <summary>
		/// 引发 FileContentChanged 事件
		/// </summary>
		/// <param name="e">引发事件的 EventArgs 类实例参数</param>
		protected internal override void OnFileContentChanged(EventArgs e)
		{
			base.OnFileContentChanged(e);
			abstractEntityElement.OnFileContentChanged(e);
		}

		#endregion

		#region 实体类属性定义信息
		///// <summary>
		///// 参数显示的图标资源
		///// </summary>
		//[System.ComponentModel.Browsable(false)]
		//public string ImageSource
		//{
		//    get
		//    {
		//        if (PrimaryKey)
		//            return ImageSourceList.Database_PrimaryKey;
		//        return ImageSourceList.Properties;
		//    }
		//}

		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.DefaultValue(typeof(PropertyModifierEnum), "Public"), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		public PropertyModifierEnum Modifier
		{
			get { return generatorElement.Modifier; }
			set
			{
				if (generatorElement.Modifier != value)
				{
					generatorElement.Modifier = value;
					RaisePropertyChanged("Modifier");
				}
			}
		}

		/// <summary>是否为属性添加 IgnorePropertyAttribute 特性标记</summary>
		/// <value>是否 IgnoreProperty 标记属性，true 表示添加特性标记。默认值为 false。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_PropertyIgnore")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[System.ComponentModel.DefaultValue(false)]
		public bool Ignore
		{
			get { return generatorElement.Ignore; }
			set
			{
				if (generatorElement.Ignore != value)
				{
					generatorElement.Ignore = value;
					base.RaisePropertyChanged("Ignore");
				}
			}
		}

		/// <summary>
		/// 获取或设置一个值，该值指示当前属性是否需要添加标记 System.Runtime.Serialization.DataMemberAttribute。
		/// </summary>
		/// <value>如果需要添加则为 true，否则为 false。默认值为 false。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_PropertyDataMember")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[System.ComponentModel.DefaultValue(false)]
		public bool DataMember
		{
			get { return generatorElement.DataMember; }
			set
			{
				if (generatorElement.DataMember != value)
				{
					generatorElement.DataMember = value;
					base.RaisePropertyChanged("DataMember");
				}
			}
		}

		/// <summary>
		/// 是否为继承属性，如果是继承属性，则在当前类实例中不写入属性信息。
		/// </summary>
		/// <value>是否为继承属性，true 表示是继承属性。默认值为 false。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_PropertyInheritance")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[System.ComponentModel.DefaultValue(false)]
		public bool Inheritance
		{
			get { return generatorElement.Inheritance; }
			set
			{
				if (generatorElement.Inheritance != value && !_PrimaryKey)
				{
					generatorElement.Inheritance = value;
					base.RaisePropertyChanged("Inheritance");
				}
			}
		}

		/// <summary>
		/// 是否为继承属性，如果是继承属性，则在当前类实例中不写入属性信息。
		/// </summary>
		/// <value>是否为继承属性，true 表示是继承属性。默认值为 false。</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_PropertyOverride")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[System.ComponentModel.DefaultValue(false)]
		public bool Override
		{
			get { return generatorElement.Override; }
			set
			{
				if (generatorElement.Override != value)
				{
					generatorElement.Override = value;
					base.RaisePropertyChanged("Override");
				}
			}
		}

		/// <summary>
		/// 获取或设置属性的本地显示名称的转换器名称。
		/// </summary>
		[PersistentDescription("PersistentDescription_PropertyVirtual")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[System.ComponentModel.DefaultValue(false)]
		public bool Virtual
		{
			get { return generatorElement.Virtual; }
			set
			{
				if (generatorElement.Virtual != value)
				{
					generatorElement.Virtual = value;
					base.RaisePropertyChanged("Virtual");
				}
			}
		}

		private string _Name = string.Empty;
		/// <summary>
		/// 实体属性名称
		/// </summary>
		[Basic.Designer.PersistentDescription("PersistentDescription_PropertyName")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryProperty)]
		[System.ComponentModel.DefaultValue(""), Basic.Designer.PropertyOrder(1)]
		public string Name
		{
			get { return _Name; }
			set
			{
				if (_Name != value)
				{
					_Name = value;
					RaisePropertyChanged("Name");
				}
			}
		}

		private Type _Type = null;
		/// <summary>
		/// 属性类型名称
		/// </summary>
		[Basic.Designer.PersistentDescription("PersistentDescription_PropertyType")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryProperty)]
		[System.ComponentModel.DefaultValue(typeof(Type), "System.String"), Basic.Designer.PropertyOrder(2)]
		[System.ComponentModel.Editor(typeof(PropertyTypeEditor), typeof(UITypeEditor))]
		public Type Type
		{
			get { return _Type; }
			set
			{
				if (_Type != value)
				{
					_Type = value;
					_TypeName = null;
					RaisePropertyChanged("Type");
					RaisePropertyChanged("TypeName");
				}
			}
		}

		private string _TypeName = null;
		/// <summary>
		/// 属性类型名称
		/// </summary>
		[Basic.Designer.PersistentDescription("PersistentDescription_PropertyType")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryProperty)]
		[System.ComponentModel.DefaultValue(""), Basic.Designer.PropertyOrder(3), DisplayName("CustomType")]
		[System.ComponentModel.Editor(typeof(ReflectedTypeEditor), typeof(UITypeEditor))]
		public string TypeName
		{
			get
			{
				if (_Type != null)
					return _Type.Name;
				return _TypeName;
			}
			set
			{
				if (!string.IsNullOrWhiteSpace(value) && _TypeName != value)
				{
					_Type = null;
					_TypeName = value;
					RaisePropertyChanged("Type");
					RaisePropertyChanged("TypeName");
				}
			}
		}

		private bool _PrimaryKey = false;
		/// <summary>
		/// 当前字段是否是主键
		/// </summary>
		[Basic.Designer.PersistentDescription("PersistentDescription_ColumnPrimaryKey")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryProperty)]
		[System.ComponentModel.DefaultValue(false), Basic.Designer.PropertyOrder(4)]
		public bool PrimaryKey
		{
			get { return _PrimaryKey; }
			set
			{
				if (_PrimaryKey != value)
				{
					_PrimaryKey = value;
					base.RaisePropertyChanged("PrimaryKey");
					base.RaisePropertyChanged("Source");
					base.RaisePropertyChanged("Icon");
					if (_PrimaryKey)
					{
						generatorElement.Inheritance = false;
						base.RaisePropertyChanged("Inheritance");
					}
				}
			}
		}

		private bool _Nullable = false;
		/// <summary>
		/// 属性是否允许为空
		/// </summary>
		[Basic.Designer.PersistentDescription("PersistentDescription_PropertyNullable")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryProperty)]
		[System.ComponentModel.DefaultValue(typeof(bool), "false"), Basic.Designer.PropertyOrder(5)]
		public bool Nullable
		{
			get { return _Nullable; }
			set
			{
				if (_Nullable != value)
				{
					_Nullable = value;
					RaisePropertyChanged("Nullable");
					RaisePropertyChanged("NullableText");
				}
			}
		}

		/// <summary>
		/// 属性是否允许为空
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public string NullableText
		{
			get { return (_Nullable && _Type != typeof(string)) ? "?" : ""; }
		}

		private string _Comment;
		/// <summary>
		/// 属性对应数据库字段名称
		/// </summary>
		[Basic.Designer.PersistentDescription("PersistentDescription_PropertyComment")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryProperty)]
		[System.ComponentModel.DefaultValue(""), Basic.Designer.PropertyOrder(6)]
		public string Comment
		{
			get { return _Comment; }
			set
			{
				if (_Comment != value)
				{
					_Comment = value;
					RaisePropertyChanged("Comment");
				}
			}
		}
		#endregion

		#region 接口 IXmlSerializable 默认实现
		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == NameAttribute) { _Name = value; return true; }
			else if (name == PrimaryKeyAttribute) { _PrimaryKey = Convert.ToBoolean(value); return true; }
			else if (name == NullableAttribute) { _Nullable = Convert.ToBoolean(value); return true; }
			else if (name == PropertyGeneratorElement.InheritAttribute) { generatorElement.Inheritance = Convert.ToBoolean(value); return true; }
			else if (name == PropertyGeneratorElement.ModifierAttribute)
			{
				PropertyModifierEnum _Modifier = PropertyModifierEnum.Public;
				if (Enum.TryParse<PropertyModifierEnum>(value, out _Modifier))
					generatorElement.Modifier = _Modifier;
				return true;
			}
			else if (name == TypeAttribute)
			{
				try { _Type = Type.GetType(value); if (_Type == null) { _TypeName = value; } }
				catch { _TypeName = value; }
				return true;
			}
			return false;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected internal override bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == CommentElement)
			{
				_Comment = reader.ReadString(); return false;
			}
			else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == ColumnElement)
			{
				return ReadColumn(reader.ReadSubtree());
			}
			else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == PropertyGeneratorElement.XmlElementName)
			{
				generatorElement.ReadXml(reader.ReadSubtree()); return false;
			}
			else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == DisplayNameElement.XmlElementName)
			{
				displayName.ReadXml(reader.ReadSubtree()); return false;
			}
			else if (reader.NodeType == System.Xml.XmlNodeType.EndElement && reader.LocalName == XmlElementName)
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
			writer.WriteAttributeString(NameAttribute, _Name);
			if (_Type != null)
				writer.WriteAttributeString(TypeAttribute, _Type.FullName);
			else
				writer.WriteAttributeString(TypeAttribute, _TypeName);
			if (_PrimaryKey)
				writer.WriteAttributeString(PrimaryKeyAttribute, Convert.ToString(_PrimaryKey).ToLower());
			if (_Nullable)
				writer.WriteAttributeString(NullableAttribute, Convert.ToString(_Nullable).ToLower());
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			if (!generatorElement.IsEmpty)
				generatorElement.WriteXml(writer);
			displayName.WriteXml(writer);
			if (!string.IsNullOrWhiteSpace(_Comment))
				writer.WriteElementString(CommentElement, _Comment);
			WriteColumn(writer);
		}
		#endregion

		/// <summary>
		/// 属性的显示名称格式设置。
		/// </summary>
		public DisplayNameElement DisplayName { get { return displayName; } }

		/// <summary>
		/// 属性字段名称
		/// </summary>
		protected internal string FieldName { get { return string.Concat("m_", Name); } }
		/// <summary>
		/// 当前字段在强类型DataTable中的列属性名称。
		/// </summary>
		protected internal string TableColumn { get { return string.Concat("C", Name); } }
		/// <summary>
		/// 当前字段在强类型DataTable中的列变量名称。
		/// </summary>
		protected internal string ColumnField { get { return string.Concat("column", Name); } }

		/// <summary>
		/// 在文件代码模型中查找当前属性的定义
		/// </summary>
		/// <param name="codeElements">文件代码模型元素集合</param>
		/// <returns>如果找到属性则返回次属性的 CodeProperty 对象引用，否则返回null。</returns>
		private CodeProperty FindCodeProperty(CodeElements codeElements)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			foreach (CodeElement element in codeElements)
			{
				if (element.Kind == vsCMElement.vsCMElementClass)
				{
					CodeClass codeClass = element as CodeClass;
					CodeProperty codeProperty = FindCodeProperty(codeClass.Members);
					if (codeProperty != null) { return codeProperty; }
				}
				else if (element.Kind == vsCMElement.vsCMElementProperty && element.Name == Name)
				{
					return element as CodeProperty;
				}
			}
			return null;
		}

		private vsCMTypeRef GetPropertyType()
		{
			switch (_DbType)
			{
				case DbTypeEnum.Binary:
					return vsCMTypeRef.vsCMTypeRefArray;
				default:
					return vsCMTypeRef.vsCMTypeRefString;
			}
		}

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD010:在主线程上调用单线程类型", Justification = "<挂起>")]
		protected internal virtual CodeProperty WritePropertyCode(CodeClass codeClass, CodeDomProvider provider)
		{
			CodeProperty codeProperty = FindCodeProperty(codeClass.Members);
			if (codeProperty == null)
			{
				vsCMTypeRef typeRef = GetPropertyType();
				CodeMemberProperty property = new CodeMemberProperty();
				property.Attributes = MemberAttributes.Final;
				property.Comments.Add(new CodeCommentStatement("<summary>", true));
				if (string.IsNullOrWhiteSpace(Comment))
					property.Comments.Add(new CodeCommentStatement(string.Format("Property: {0}", Name), true));
				else
					property.Comments.Add(new CodeCommentStatement(Comment, true));
				property.Comments.Add(new CodeCommentStatement("</summary>", true));
				if (string.IsNullOrWhiteSpace(_Comment))
					property.Comments.Add(new CodeCommentStatement(string.Concat("<value>属性 ", _Name, " 的值</value>"), true));
				else
					property.Comments.Add(new CodeCommentStatement(string.Concat("<value>", _Comment, "</value>"), true));
				if (Modifier == PropertyModifierEnum.Public)
					property.Attributes = property.Attributes | MemberAttributes.Public;
				else if (Modifier == PropertyModifierEnum.Internal)
					property.Attributes = property.Attributes | MemberAttributes.Assembly;
				else if (Modifier == PropertyModifierEnum.Private)
					property.Attributes = property.Attributes | MemberAttributes.Private;
				else if (Modifier == PropertyModifierEnum.Protected)
					property.Attributes = property.Attributes | MemberAttributes.Family;
				else if (Modifier == PropertyModifierEnum.ProtectedInternal)
					property.Attributes = property.Attributes | MemberAttributes.FamilyOrAssembly;
				if (_Type != null)
					property.Type = new CodeTypeReference(_Type);
				else
					property.Type = new CodeTypeReference(_TypeName);
				property.Name = Name;
				property.HasGet = false;
				property.HasSet = true;
				CodeGeneratorOptions options = new CodeGeneratorOptions();
				options.BlankLinesBetweenMembers = true;
				options.BracingStyle = "C";
				System.Text.StringBuilder textBuilder = new System.Text.StringBuilder(1500);
				using (StringWriter writer = new StringWriter(textBuilder))
				{
					provider.GenerateCodeFromMember(property, writer, options);
				}
				vsCMAccess access = vsCMAccess.vsCMAccessPublic;
				codeProperty = codeClass.AddProperty(Name, Name, typeRef, -1, access);//
				EditPoint editPoint = codeProperty.GetStartPoint(vsCMPart.vsCMPartWholeWithAttributes).CreateEditPoint();
				EditPoint movePoint = codeProperty.GetEndPoint(vsCMPart.vsCMPartWholeWithAttributes).CreateEditPoint();
				editPoint.StartOfLine(); movePoint.EndOfLine();
				editPoint.ReplaceText(movePoint, textBuilder.ToString(), (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
			}
			return codeProperty;
		}

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		/// <param name="entityClass">表示需要写入属性代码的类型定义</param>
		/// <param name="pkConstructor">如果实体存在主键则</param>
		protected internal virtual CodeMemberProperty WriteDesignerCode(CodeTypeDeclaration entityClass, CodeConstructor pkConstructor)
		{
			if (Inheritance)
				return null;
			CodeMemberProperty property = new CodeMemberProperty();
			CodeTypeReference propertyType = null;
			if (_Nullable && _Type != null && !_Type.IsClass)
				propertyType = new CodeTypeReference(typeof(Nullable<>).MakeGenericType(_Type));
			else if (_Nullable && _Type != null && _Type.IsClass)
				propertyType = new CodeTypeReference(_Type);
			else if (_Nullable && _Type == null)
				propertyType = new CodeTypeReference(string.Concat(_TypeName, "?"));
			else if (!_Nullable && _Type != null)
				propertyType = new CodeTypeReference(_Type);
			else
				propertyType = new CodeTypeReference(_TypeName);
			CodeMemberField entityField = new CodeMemberField(propertyType, FieldName);
			property.Comments.Add(new CodeCommentStatement("<summary>", true));
			if (string.IsNullOrWhiteSpace(Comment))
				property.Comments.Add(new CodeCommentStatement(string.Format("Property: {0}", Name), true));
			else
				property.Comments.Add(new CodeCommentStatement(Comment, true));
			property.Comments.Add(new CodeCommentStatement("</summary>", true));
			if (string.IsNullOrWhiteSpace(_Comment))
				property.Comments.Add(new CodeCommentStatement(string.Concat("<value>属性 ", _Name, " 的值</value>"), true));
			else
				property.Comments.Add(new CodeCommentStatement(string.Concat("<value>", _Comment, "</value>"), true));

			property.Type = propertyType;
			property.HasSet = true;
			property.HasGet = true;
			property.Name = Name;
			if (Modifier == PropertyModifierEnum.Public)
				property.Attributes = MemberAttributes.Public;
			else if (Modifier == PropertyModifierEnum.Internal)
				property.Attributes = MemberAttributes.Assembly;
			else if (Modifier == PropertyModifierEnum.Private)
				property.Attributes = MemberAttributes.Private;
			else if (Modifier == PropertyModifierEnum.Protected)
				property.Attributes = MemberAttributes.Family;
			else if (Modifier == PropertyModifierEnum.ProtectedInternal)
				property.Attributes = MemberAttributes.FamilyOrAssembly;
			if (Override) { property.Attributes |= MemberAttributes.Override; }
			else if (!Virtual) { property.Attributes |= MemberAttributes.Final; }

			if (DataMember)
			{
				CodeTypeReference dataMemberTypeReference = new CodeTypeReference(typeof(DataMemberAttribute), CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration dataMemberAttribute = new CodeAttributeDeclaration(dataMemberTypeReference);
				property.CustomAttributes.Add(dataMemberAttribute);
			}

			if (Ignore)
			{
				CodeTypeReference ignoreTypeReference = new CodeTypeReference(typeof(IgnorePropertyAttribute), CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration ignoreAttribute = new CodeAttributeDeclaration(ignoreTypeReference);
				property.CustomAttributes.Add(ignoreAttribute);
			}

			#region 主键信息
			if (_PrimaryKey)
			{
				CodeTypeReference primaryKeyReference = new CodeTypeReference(typeof(Basic.EntityLayer.PrimaryKeyAttribute),
				CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration primaryKeyAttribute = new CodeAttributeDeclaration(primaryKeyReference);
				property.CustomAttributes.Add(primaryKeyAttribute);
			}
			#endregion

			if (_DbType == DbTypeEnum.Timestamp)
			{
				CodeTypeReference timestampReference = new CodeTypeReference(typeof(System.ComponentModel.DataAnnotations.TimestampAttribute),
				CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration timestampAttribute = new CodeAttributeDeclaration(timestampReference);
				property.CustomAttributes.Add(timestampAttribute);
			}
			//写入字段信息
			WriteColumnCode(property);
			displayName.WriteDisplayNameCode(property);
			CodeVariableReferenceExpression field = new CodeVariableReferenceExpression(FieldName);
			// 产生 return m_property
			CodeMethodReturnStatement propertyReturn = new CodeMethodReturnStatement(field);
			property.GetStatements.Add(propertyReturn);
			// 产生 m_property = value;
			CodeConditionStatement conditionStatement = new CodeConditionStatement();
			conditionStatement.Condition = new CodeBinaryOperatorExpression(field,
				 CodeBinaryOperatorType.IdentityInequality, new CodePropertySetValueReferenceExpression());
			CodeBaseReferenceExpression baseTypeReference = new CodeBaseReferenceExpression();
			CodeMethodInvokeExpression propertyChanging = new CodeMethodInvokeExpression();
			propertyChanging.Method = new CodeMethodReferenceExpression(baseTypeReference, "OnPropertyChanging");
			propertyChanging.Parameters.Add(new CodePrimitiveExpression(Name));
			conditionStatement.TrueStatements.Add(propertyChanging);

			CodeAssignStatement propertyAssignment = new CodeAssignStatement(field, new CodePropertySetValueReferenceExpression());
			conditionStatement.TrueStatements.Add(propertyAssignment);

			CodeMethodInvokeExpression propertyChanged = new CodeMethodInvokeExpression();
			propertyChanged.Method = new CodeMethodReferenceExpression(baseTypeReference, "OnPropertyChanged");
			propertyChanged.Parameters.Add(new CodePrimitiveExpression(Name));
			conditionStatement.TrueStatements.Add(propertyChanged);
			property.SetStatements.Add(conditionStatement);
			entityClass.Members.Add(entityField);
			entityClass.Members.Add(property);
			return property;
		}

		/// <summary>
		/// 当前列是可为空时，使用的判断方法名称
		/// </summary>
		private string IsColumnNull { get { return string.Concat(Name, "IsNull"); } }

		/// <summary>
		/// 当前列是可为空时，使用的字段设置为DbNull方法名称
		/// </summary>
		private string SetColumnNull { get { return string.Concat(Name, "SetNull"); } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableCode"></param>
		protected internal void WriteTableCode(CodeTypeDeclaration tableCode)
		{
			if (string.IsNullOrWhiteSpace(Column)) { return; }
			CodeTypeReference columnType = new CodeTypeReference(typeof(DataColumn), CodeTypeReferenceOptions.GlobalReference);
			CodeMemberField memberField = new CodeMemberField(columnType, ColumnField);
			CodeMemberProperty property = new CodeMemberProperty();
			property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			property.Comments.Add(new CodeCommentStatement("<summary>", true));
			if (string.IsNullOrWhiteSpace(Comment))
				property.Comments.Add(new CodeCommentStatement(string.Concat("Column ", Name), true));
			else
				property.Comments.Add(new CodeCommentStatement(Comment, true));
			property.Comments.Add(new CodeCommentStatement("</summary>", true));
			property.Type = new CodeTypeReference(typeof(DataColumn), CodeTypeReferenceOptions.GlobalReference);
			property.HasGet = true;
			property.Name = TableColumn;

			CodeVariableReferenceExpression field = new CodeVariableReferenceExpression(ColumnField);
			// 产生 return m_property
			CodeMethodReturnStatement propertyReturn = new CodeMethodReturnStatement(field);
			property.GetStatements.Add(propertyReturn);
			tableCode.Members.Add(memberField);
			tableCode.Members.Add(property);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="initColumnsMethod"></param>
		internal void WriteIniColumnCode(CodeMemberMethod initColumnsMethod)
		{
			if (string.IsNullOrWhiteSpace(Column)) { return; }
			CodeAssignStatement codeAssign = new CodeAssignStatement();
			CodeThisReferenceExpression thisReference = new CodeThisReferenceExpression();
			codeAssign.Left = new CodeFieldReferenceExpression(thisReference, ColumnField);
			CodeBaseReferenceExpression baseReference = new CodeBaseReferenceExpression();
			CodePropertyReferenceExpression columnsProperty = new CodePropertyReferenceExpression(baseReference, "Columns");
			CodeIndexerExpression columnsIndexer = new CodeIndexerExpression(columnsProperty);
			columnsIndexer.Indices.Add(new CodePrimitiveExpression(Column));
			codeAssign.Right = columnsIndexer;
			initColumnsMethod.Statements.Add(codeAssign);
		}

		internal void WritePrimaryKeyCode(CodeMemberMethod declareMethod, CodeMethodInvokeExpression findKey)
		{
			if (PrimaryKey)
			{
				string varName = string.Concat(Char.ToLower(Name[0]), Name.Remove(0, 1));
				declareMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">{1}</param>", varName, Comment), true));
				if (_Type != null)
					declareMethod.Parameters.Add(new CodeParameterDeclarationExpression(_Type, varName));
				else
					declareMethod.Parameters.Add(new CodeParameterDeclarationExpression(_TypeName, varName));

				findKey.Parameters.Add(new CodeVariableReferenceExpression(varName));
			}
		}

		internal void WritePrimaryKeyCode(CodeArrayCreateExpression newColumnArray)
		{
			if (PrimaryKey)
			{
				newColumnArray.Initializers.Add(new CodeVariableReferenceExpression(ColumnField));
			}
		}

		internal void WritePrimaryKeyCode(CodeMemberMethod declareMethod)
		{
			if (PrimaryKey)
			{
				string varName = string.Concat(Char.ToLower(Name[0]), Name.Remove(0, 1));
				declareMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">{1}</param>", varName, Comment), true));
				if (_Type != null)
					declareMethod.Parameters.Add(new CodeParameterDeclarationExpression(_Type, varName));
				else
					declareMethod.Parameters.Add(new CodeParameterDeclarationExpression(_TypeName, varName));
				CodeAssignStatement assign = new CodeAssignStatement();
				assign.Left = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), FieldName);
				assign.Right = new CodeVariableReferenceExpression(varName);
				declareMethod.Statements.Add(assign);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="initClassMethod"></param>
		internal void WriteIniClassCode(CodeMemberMethod initClassMethod)
		{
			if (string.IsNullOrWhiteSpace(Column)) { return; }
			CodeAssignStatement createAssegn = new CodeAssignStatement();
			CodeBaseReferenceExpression baseReference = new CodeBaseReferenceExpression();
			CodeThisReferenceExpression thisReference = new CodeThisReferenceExpression();
			CodeFieldReferenceExpression columnReference = new CodeFieldReferenceExpression(thisReference, ColumnField);
			createAssegn.Left = columnReference;
			CodeTypeReference dataColumnType = new CodeTypeReference(typeof(DataColumn), CodeTypeReferenceOptions.GlobalReference);
			CodeObjectCreateExpression dataColumnConstructor = new CodeObjectCreateExpression(dataColumnType);
			dataColumnConstructor.Parameters.Add(new CodePrimitiveExpression(Column));
			if (_Type != null)
				dataColumnConstructor.Parameters.Add(new CodeTypeOfExpression(_Type));
			else
				dataColumnConstructor.Parameters.Add(new CodeTypeOfExpression(_TypeName));
			dataColumnConstructor.Parameters.Add(new CodePrimitiveExpression(null));
			CodeFieldReferenceExpression fieldTypeExpress = new CodeFieldReferenceExpression(
					  new CodeTypeReferenceExpression(typeof(MappingType)), "Element");
			dataColumnConstructor.Parameters.Add(fieldTypeExpress);
			createAssegn.Right = dataColumnConstructor;
			initClassMethod.Statements.Add(createAssegn);
			CodePropertyReferenceExpression columnsProperty = new CodePropertyReferenceExpression(baseReference, "Columns");
			CodeMethodInvokeExpression columnsAddMethod = new CodeMethodInvokeExpression(columnsProperty, "Add");
			columnsAddMethod.Parameters.Add(columnReference);
			initClassMethod.Statements.Add(columnsAddMethod);
			if (Nullable)
			{
				CodePropertyReferenceExpression allowDbNull = new CodePropertyReferenceExpression(columnReference, "AllowDBNull");
				CodeAssignStatement allowDbNullAssegn = new CodeAssignStatement(allowDbNull, new CodePrimitiveExpression(Nullable));
				initClassMethod.Statements.Add(allowDbNullAssegn);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rowCode"></param>
		/// <param name="entityName"></param>
		internal void WriteRowCode(CodeTypeDeclaration rowCode, string entityName)
		{
			if (string.IsNullOrWhiteSpace(Column)) { return; }
			CodeTypeReference propertyType = null;
			if (_Nullable && _Type != null && !_Type.IsClass)
				propertyType = new CodeTypeReference(typeof(Nullable<>).MakeGenericType(_Type));
			else if (_Nullable && _Type != null && _Type.IsClass)
				propertyType = new CodeTypeReference(_Type);
			else if (_Nullable && _Type == null)
				propertyType = new CodeTypeReference(string.Concat(_TypeName, "?"));
			else if (!_Nullable && _Type != null)
				propertyType = new CodeTypeReference(_Type);
			else
				propertyType = new CodeTypeReference(_TypeName);
			CodeMemberProperty property = new CodeMemberProperty();
			property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			property.Comments.Add(new CodeCommentStatement("<summary>", true));
			if (string.IsNullOrWhiteSpace(Comment))
				property.Comments.Add(new CodeCommentStatement(string.Format("Property: {0}", Name), true));
			else
				property.Comments.Add(new CodeCommentStatement(Comment, true));
			property.Comments.Add(new CodeCommentStatement("</summary>", true));
			property.Type = propertyType;
			property.HasSet = true;
			property.HasGet = true;
			property.Name = Name;
			#region 主键信息
			if (_PrimaryKey)
			{
				CodeTypeReference primaryKeyReference = new CodeTypeReference(typeof(Basic.EntityLayer.PrimaryKeyAttribute),
				CodeTypeReferenceOptions.GlobalReference);
				CodeAttributeDeclaration primaryKeyAttribute = new CodeAttributeDeclaration(primaryKeyReference);
				property.CustomAttributes.Add(primaryKeyAttribute);
			}
			#endregion

			//写入字段信息
			WriteColumnCode(property);

			string tableMemberName = string.Concat("table", entityName);
			CodeVariableReferenceExpression tableVariable = new CodeVariableReferenceExpression(tableMemberName);
			CodeThisReferenceExpression thisReference = new CodeThisReferenceExpression();
			CodePropertyReferenceExpression tableColumn = new CodePropertyReferenceExpression(tableVariable, TableColumn);
			// 产生 return m_property
			if (Nullable)
			{
				CodeMemberMethod isNullPropertyMethod = new CodeMemberMethod();
				isNullPropertyMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
				isNullPropertyMethod.Comments.Add(new CodeCommentStatement(string.Format("Field: {0} is Null", Name), true));
				isNullPropertyMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
				isNullPropertyMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
				isNullPropertyMethod.Name = IsColumnNull;
				isNullPropertyMethod.ReturnType = new CodeTypeReference(typeof(bool), CodeTypeReferenceOptions.GlobalReference);
				CodeMethodReturnStatement isNullReturn = new CodeMethodReturnStatement();
				CodeMethodInvokeExpression isNullMethod = new CodeMethodInvokeExpression();
				isNullMethod.Method.MethodName = "IsNull";
				isNullMethod.Method.TargetObject = thisReference;
				isNullMethod.Parameters.Add(tableColumn);
				isNullReturn.Expression = isNullMethod;
				isNullPropertyMethod.Statements.Add(isNullReturn);
				rowCode.Members.Add(isNullPropertyMethod);

				CodeMemberMethod setNullMethod = new CodeMemberMethod();
				setNullMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
				setNullMethod.Comments.Add(new CodeCommentStatement(string.Format("Field: Set {0}'s value Null", Name), true));
				setNullMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
				setNullMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
				setNullMethod.Name = SetColumnNull;
				CodeIndexerExpression columnIndexer = new CodeIndexerExpression(thisReference);
				columnIndexer.Indices.Add(tableColumn);
				CodeTypeReferenceExpression typeReference = new CodeTypeReferenceExpression(typeof(DBNull));
				CodeFieldReferenceExpression fieldReference = new CodeFieldReferenceExpression(typeReference, "Value");
				CodeAssignStatement columnAssignment = new CodeAssignStatement(columnIndexer, fieldReference);
				setNullMethod.Statements.Add(columnAssignment);
				rowCode.Members.Add(setNullMethod);

				CodeConditionStatement getCodition = new CodeConditionStatement();
				CodeMethodReferenceExpression methodReference = new CodeMethodReferenceExpression(thisReference, IsColumnNull);
				CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(methodReference);
				getCodition.Condition = methodInvoke;
				getCodition.TrueStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(null)));
				property.GetStatements.Add(getCodition);
			}
			CodeIndexerExpression propertyIndexer = new CodeIndexerExpression(thisReference);
			propertyIndexer.Indices.Add(tableColumn);
			//propertyReturn.Expression = propertyIndexer;//;new CodeCastExpression(Type, propertyIndexer);
			if (_Type != null)
				property.GetStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(_Type, propertyIndexer)));
			else
				property.GetStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(_TypeName, propertyIndexer)));
			// 产生 m_property = value;
			CodeAssignStatement propertyAssignment = new CodeAssignStatement(propertyIndexer, new CodePropertySetValueReferenceExpression());
			property.SetStatements.Add(propertyAssignment);
			rowCode.Members.Add(property);
		}
	}
}
