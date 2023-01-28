using System;
using System.CodeDom;
using System.Drawing.Design;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Basic.Collections;
using Basic.DataAccess;
using Basic.DataEntities;
using Basic.Designer;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;
using BD = Basic.Designer;

namespace Basic.Configuration
{
	/// <summary>
	/// 表示动态配置命令
	/// </summary>
	internal class DynamicCommandElement : DataCommandElement, IXmlSerializable
	{
		#region Xml 节点名称常量
		/// <summary>
		/// 表示Xml元素名称
		/// </summary>
		protected internal const string XmlElementName = "DynamicCommand";

		/// <summary>
		/// Condition 配置节名称，表示当前命令执行需要的参数信息。
		/// </summary>
		protected internal const string ConditionElement = "Condition";

		/// <summary>
		/// SelectText 配置节名称
		/// </summary>
		protected internal const string SelectTextElement = "SelectText";

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中From 数据库表部分。
		/// </summary>
		protected internal const string FromTextElement = "FromText";

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中Where 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		protected internal const string WhereTextElement = "WhereText";

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中Group 部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		protected internal const string GroupTextElement = "GroupText";

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中Hanving条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		protected internal const string HavingTextElement = "HavingText";

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中Order By条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		protected internal const string OrderTextElement = "OrderText";
		#endregion

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName()
		{
			return GetType().Name.Replace("Element", "");
		}

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName()
		{
			return Name;
		}

		/// <summary>
		/// 初始化 DynamicCommandElement 类实例
		/// </summary>
		internal protected DynamicCommandElement(DataEntityElement entity)
			: base(entity) { _WithClauses = new BD.WithClauseCollection(this); }

		private string _Condition = string.Empty;
		/// <summary>
		/// 获取或设置要一个值，该值指示当前查询命令使用的参数类型。
		/// </summary>
		/// <value>当前查询命令使用的参数类型名称。</value>
		[System.ComponentModel.Bindable(true)]
		[System.ComponentModel.Description("获取或设置要一个值，该值指示当前查询命令使用的参数类型。")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDynamicCommand)]
		[System.ComponentModel.Editor(typeof(ConditionSelector), typeof(UITypeEditor))]
		public string Condition
		{
			get { return _Condition; }
			set
			{
				if (_Condition != value)
				{
					base.OnPropertyChanging("Condition");
					_Condition = value;
					base.RaisePropertyChanged("Condition");
				}
			}
		}

		private BD.WithClauseCollection _WithClauses;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 WITH 子句部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 WITH 子句部分。</value>
		[System.ComponentModel.Bindable(true)]
		[System.ComponentModel.Description("获取或设置要对数据源执行的 Transact-SQL 语句中 WITH 子句集合")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDynamicCommand)]
		public virtual BD.WithClauseCollection WithClauses
		{
			get { return _WithClauses; }
			//set
			//{
			//	//if (_WithClauses != value)
			//	//{
			//	//	base.OnPropertyChanging("WithClauses");
			//	//	_WithClauses = value;
			//	//	base.RaisePropertyChanged("WithClauses");
			//	//}
			//}
		}

		private string _SelectText = string.Empty;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 SELECT 数据库字段部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 SELECT 部分，默认值为空字符串。</value>
		[System.ComponentModel.Bindable(true)]
		[System.ComponentModel.Description("获取或设置要对数据源执行的 Transact-SQL 语句中 SELECT 数据库字段部分")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDynamicCommand)]
		[System.ComponentModel.Editor(typeof(DynamicCommandEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string SelectText
		{
			get { return _SelectText; }
			set
			{
				if (_SelectText != value)
				{
					_SelectText = value;
					base.RaisePropertyChanged("SelectText");
				}
			}
		}


		private string _FromText = string.Empty;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 FROM 数据库表部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 FROM 部分，默认值为空字符串。</value>
		[System.ComponentModel.Bindable(true)]
		[System.ComponentModel.Description("获取或设置要对数据源执行的 Transact-SQL 语句中 FROM 数据库表部分")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDynamicCommand)]
		[System.ComponentModel.Editor(typeof(DynamicCommandEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string FromText
		{
			get { return _FromText; }
			set
			{
				if (_FromText != value)
				{
					_FromText = value;
					base.RaisePropertyChanged("FromText");
				}
			}
		}

		private string _WhereText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 WHERE 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 WHERE 部分，默认值为空字符串。</value>
		[System.ComponentModel.Bindable(true)]
		[System.ComponentModel.Description("获取或设置要对数据源执行的 Transact-SQL 语句中 WHERE 条件部分")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDynamicCommand)]
		[System.ComponentModel.Editor(typeof(DynamicCommandEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string WhereText
		{
			get { return _WhereText; }
			set
			{
				if (_WhereText != value)
				{
					_WhereText = value;
					base.RaisePropertyChanged("WhereText");
					base.RaisePropertyChanged("HasWhere");
				}
			}
		}
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 WHERE 条件部分是否为空。
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public bool HasWhere { get { return !string.IsNullOrWhiteSpace(_WhereText); } }

		private string _GroupText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 GROUP BY 部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 GROUP BY 部分，默认值为空字符串。</value>
		[System.ComponentModel.Bindable(true)]
		[System.ComponentModel.Description("获取或设置要对数据源执行的 Transact-SQL 语句中 GROUP BY 部分")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDynamicCommand)]
		[System.ComponentModel.Editor(typeof(DynamicCommandEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string GroupText
		{
			get { return _GroupText; }
			set
			{
				if (_GroupText != value)
				{
					_GroupText = value;
					base.RaisePropertyChanged("GroupText");
					base.RaisePropertyChanged("HasGroup");
				}
			}
		}
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 GROUP BY 条件部分是否为空。
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public bool HasGroup { get { return !string.IsNullOrWhiteSpace(_GroupText); } }

		private string _HavingText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 HANVING 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 HANVING 部分，默认值为空字符串。</value>
		[System.ComponentModel.Bindable(true)]
		[System.ComponentModel.Description("获取或设置要对数据源执行的 Transact-SQL 语句中 HANVING 条件部分")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDynamicCommand)]
		[System.ComponentModel.Editor(typeof(DynamicCommandEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string HavingText
		{
			get { return _HavingText; }
			set
			{
				if (_HavingText != value)
				{
					_HavingText = value;
					base.RaisePropertyChanged("HavingText");
					base.RaisePropertyChanged("HasHaving");
				}
			}
		}
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 HANVING 条件部分是否为空。
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public bool HasHaving { get { return !string.IsNullOrWhiteSpace(_HavingText); } }

		private string _OrderText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 ORDER BY 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 ORDER BY 部分，默认值为空字符串。</value>
		[System.ComponentModel.Bindable(true)]
		[System.ComponentModel.Description("获取或设置要对数据源执行的 Transact-SQL 语句中 ORDER BY 条件部分")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDynamicCommand)]
		[System.ComponentModel.Editor(typeof(DynamicCommandEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string OrderText
		{
			get { return _OrderText; }
			set
			{
				if (_OrderText != value)
				{
					_OrderText = value;
					base.RaisePropertyChanged("OrderText"); base.RaisePropertyChanged("HasOrder");
				}
			}
		}
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 ORDER BY 条件部分是否为空。
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public bool HasOrder { get { return !string.IsNullOrWhiteSpace(_OrderText); } }

		/// <summary>
		/// 获取命令所关联的方法名称
		/// </summary>
		[System.ComponentModel.Description("获取或设置命令名称"), System.ComponentModel.DefaultValue("")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		[System.ComponentModel.Browsable(false)]
		public override string MethodName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Name)) { return null; }
				else if (Kind == ConfigurationTypeEnum.SearchTable && ExecutableMethod == DynamicMethodEnum.GetEntities && AsyncGenerated == false)
					return "GetEntities";
				else if (Kind == ConfigurationTypeEnum.SearchTable && ExecutableMethod == DynamicMethodEnum.GetEntities && AsyncGenerated == true)
					return "GetEntitiesAsync";
				else if (Kind == ConfigurationTypeEnum.SearchTable && ExecutableMethod == DynamicMethodEnum.GetDataTable && AsyncGenerated == false)
					return "GetDataTable";
				else if (Kind == ConfigurationTypeEnum.SearchTable && ExecutableMethod == DynamicMethodEnum.GetDataTable && AsyncGenerated == true)
					return "GetDataTableAsync";
				return AsyncGenerated == true ? AsyncName : Name;
			}
		}

		private DynamicMethodEnum _ExecutableMethod = DynamicMethodEnum.GetEntities;
		/// <summary>
		/// 表示当前命令调用基类方法实例
		/// </summary>
		[System.ComponentModel.Description("获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程")]
		[System.ComponentModel.DefaultValue(typeof(DynamicMethodEnum), "GetEntities")]
		[Basic.Designer.PersistentDisplay("DisplayName_ExecutableMethod")]
		[PersistentCategoryAttribute(PersistentCategoryAttribute.CategoryCodeGenerator)]
		public DynamicMethodEnum ExecutableMethod
		{
			get { return _ExecutableMethod; }
			set
			{
				if (_ExecutableMethod != value)
				{
					_ExecutableMethod = value;
					base.RaisePropertyChanged("ExecutableMethod");
				}
			}
		}

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName { get { return XmlElementName; } }

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == ExecutableAttribute)
			{
				DynamicMethodEnum staticEnum = DynamicMethodEnum.GetEntities;
				if (Enum.TryParse<DynamicMethodEnum>(value, true, out staticEnum))
				{
					this.ExecutableMethod = staticEnum;
					return true;
				}
				return false;
			}
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected internal override bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == BD.WithClauseCollection.XmlElementName)
			{
				_WithClauses.ReadXml(reader.ReadSubtree());
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == SelectTextElement)
			{
				_SelectText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == FromTextElement)
			{
				_FromText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == WhereTextElement)
			{
				_WhereText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == GroupTextElement)
			{
				_GroupText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == HavingTextElement)
			{
				_HavingText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == OrderTextElement)
			{
				_OrderText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == ConditionElement)
			{
				_Condition = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == ElementName)
			{
				//if (string.IsNullOrWhiteSpace(_Condition)) { _Condition = EntityElement.Condition.ClassName; }
				return true;
			}
			return base.ReadContent(reader);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			if (_ExecutableMethod != DynamicMethodEnum.GetEntities)
				writer.WriteAttributeString(ExecutableAttribute, _ExecutableMethod.ToString());
			base.WriteAttribute(writer);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			base.WriteContent(writer);
			if (!string.IsNullOrEmpty(_Condition))
				writer.WriteElementString(ConditionElement, _Condition);

			_WithClauses.WriteXml(writer);

			writer.WriteStartElement(SelectTextElement);
			writer.WriteCData(_SelectText);//写CData
			writer.WriteEndElement();

			writer.WriteStartElement(FromTextElement);
			writer.WriteCData(_FromText);//写CData
			writer.WriteEndElement();

			if (!string.IsNullOrEmpty(_WhereText))
			{
				writer.WriteStartElement(WhereTextElement);
				writer.WriteCData(_WhereText);//写CData
				writer.WriteEndElement();
			}
			if (!string.IsNullOrEmpty(_GroupText))
				writer.WriteElementString(GroupTextElement, _GroupText);
			if (!string.IsNullOrEmpty(_HavingText))
			{
				writer.WriteStartElement(HavingTextElement);
				writer.WriteCData(_HavingText);//写CData
				writer.WriteEndElement();
			}
			writer.WriteElementString(OrderTextElement, _OrderText);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType)
		{
			writer.WriteStartElement(XmlElementName);
			writer.WriteAttributeString(NameAttribute, ConfigurationName);
			if (CommandType != System.Data.CommandType.Text)
				writer.WriteAttributeString(CommandTypeAttribute, CommandType.ToString());
			if (CommandTimeout != 30)
				writer.WriteAttributeString(CommandTimeoutAttribute, CommandTimeout.ToString());
			if (Kind != ConfigurationTypeEnum.Other)
				writer.WriteAttributeString(KindAttribute, Kind.ToString());

			if (_WithClauses.Count > 0) { _WithClauses.GenerateConfiguration(writer, connectionType); }

			writer.WriteStartElement(SelectTextElement);
			writer.WriteCData(CreateCommandText(_SelectText, connectionType));//写CData
			writer.WriteEndElement();

			writer.WriteStartElement(FromTextElement);
			writer.WriteCData(CreateCommandText(_FromText, connectionType));//写CData
			writer.WriteEndElement();

			if (string.IsNullOrEmpty(_WhereText) == false)
			{
				writer.WriteStartElement(WhereTextElement);
				writer.WriteCData(CreateCommandText(_WhereText, connectionType));//写CData
				writer.WriteEndElement();
			}
			if (!string.IsNullOrEmpty(_GroupText))
				writer.WriteElementString(GroupTextElement, CreateCommandText(_GroupText, connectionType));
			if (!string.IsNullOrEmpty(_HavingText))
			{
				writer.WriteStartElement(HavingTextElement);
				writer.WriteCData(CreateCommandText(_HavingText, connectionType));//写CData
				writer.WriteEndElement();
			}
			writer.WriteElementString(OrderTextElement, CreateCommandText(_OrderText, connectionType));
			base.GenerateConfiguration(writer, connectionType);
			writer.WriteEndElement();
		}

		/// <summary>
		/// 实现设计时代码
		/// </summary>
		/// <param name="entity">当前命令使用的实体类信息</param>
		/// <param name="classCode">表示需要写入代码的类空间</param>
		/// <param name="targetFramework">生成代码的目标框架版本（例如：262144=v4.0）</param>
		protected internal override void WriteDesignerCode(CodeTypeDeclaration classCode, int targetFramework)
		{
			if (!AutoGenerated) { return; }
			if (_ExecutableMethod == DynamicMethodEnum.GetEntities)
				GenerateDesignerGetEntities(classCode.Members);
			else if (_ExecutableMethod == DynamicMethodEnum.GetDataTable)
				GenerateDesignerGetDataTable(classCode.Members);
		}

		/// <summary>
		/// 表示执行 GetDataTable 方法的命令。
		/// </summary>
		/// <param name="members">当前类型申明的 CodeTypeMemberCollection 。</param>
		/// <param name="method">需要定义的当前方法</param>
		/// <param name="entity">当前实体</param>
		protected internal void GenerateContextGetDataTable(CodeTypeMemberCollection members, CodeMemberMethod method)
		{
			DataEntityElement entity = EntityElement;
			string paramTable = string.Format("<param name=\"table\">表示 {0} 类的实例。</param>", entity.DataTableName);
			string paramComment = string.Format("<param name=\"condition\">表示 {0} 类的实例。</param>", entity.Condition.EntityName);
			string returnComment = string.Format("<returns>返回查询结果，此结果表示 {0} 类的集合。</returns>", entity.EntityName);
			method.Comments.Add(new CodeCommentStatement(paramTable, true));
			method.Comments.Add(new CodeCommentStatement(paramComment, true));
			method.Comments.Add(new CodeCommentStatement(returnComment, true));
			method.Parameters.Add(new CodeParameterDeclarationExpression(EntityElement.DataTableName, "table"));

			if (!string.IsNullOrWhiteSpace(_Condition))
				method.Parameters.Add(new CodeParameterDeclarationExpression(_Condition, "condition"));
			else if (entity.Condition.Arguments.Count > 0)
				method.Parameters.Add(new CodeParameterDeclarationExpression(entity.Condition.EntityName, "condition"));
			else if (entity.Condition.BaseClass != typeof(AbstractCondition).FullName)
				method.Parameters.Add(new CodeParameterDeclarationExpression(entity.Condition.EntityName, "condition"));

			CodeVariableReferenceExpression tableReference = new CodeVariableReferenceExpression("table");
			CodeVariableReferenceExpression conditionReference = new CodeVariableReferenceExpression("condition");
			CodeVariableReferenceExpression accessReference = new CodeVariableReferenceExpression("access");
			CodeMethodReferenceExpression methodReference = new CodeMethodReferenceExpression(accessReference, MethodName);
			//methodReference.TypeArguments.Add(entity.FullDataRowName);
			CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(methodReference);
			methodInvoke.Parameters.Add(tableReference);
			if (!string.IsNullOrWhiteSpace(_Condition))
				methodInvoke.Parameters.Add(conditionReference);
			else if (EntityElement.Condition.Arguments.Count > 0)
				methodInvoke.Parameters.Add(conditionReference);
			else if (EntityElement.Condition.BaseClass != typeof(AbstractCondition).FullName)
				methodInvoke.Parameters.Add(conditionReference);
			else { methodInvoke.Parameters.Add(new CodePrimitiveExpression(0)); methodInvoke.Parameters.Add(new CodePrimitiveExpression(0)); }


			CodeTypeReference queryEntitiesType = new CodeTypeReference("QueryDataTable");
			queryEntitiesType.TypeArguments.Add(EntityElement.FullDataRowName);
			CodeVariableDeclarationStatement iQueryEntities = new CodeVariableDeclarationStatement(queryEntitiesType, "queries");
			iQueryEntities.InitExpression = methodInvoke;
			method.Statements.Add(iQueryEntities);
			CodeVariableReferenceExpression queryEntitiesReference = new CodeVariableReferenceExpression("queries");
			foreach (DataConditionPropertyElement conditionProperty in EntityElement.Condition.Arguments)
			{
				if (!EntityElement.Properties.ContainsKey(conditionProperty.Name)) { continue; }
				CodePropertyReferenceExpression hasProperty = new CodePropertyReferenceExpression(conditionReference, conditionProperty.HasName);
				CodeConditionStatement conditionStatement = new CodeConditionStatement();
				conditionStatement.Condition = new CodeBinaryOperatorExpression(hasProperty,
					CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true));
				CodeMethodReferenceExpression whereMethod = new CodeMethodReferenceExpression(queryEntitiesReference, "Where");
				CodeMethodInvokeExpression whereInvoke = new CodeMethodInvokeExpression(whereMethod);
				string lambda = string.Concat("m => m.", conditionProperty.Name, " == condition.", conditionProperty.Name);

				if (conditionProperty.Type != null && conditionProperty.Type == typeof(string))
					lambda = string.Concat("m => m.", conditionProperty.Name, ".Like(condition.", conditionProperty.Name, ")");
				else if (conditionProperty.Type != null && conditionProperty.Type.IsArray)
					lambda = string.Concat("m => m.", conditionProperty.Name, ".In(condition.", conditionProperty.Name, ")");
				else if (conditionProperty.Type != null && conditionProperty.Type == typeof(DateTime) && conditionProperty.Name.EndsWith("1"))
					lambda = string.Concat("m => m.", conditionProperty.Name.TrimEnd('1'), " >= condition.", conditionProperty.Name);
				else if (conditionProperty.Type != null && conditionProperty.Type == typeof(DateTime) && conditionProperty.Name.EndsWith("2"))
					lambda = string.Concat("m => m.", conditionProperty.Name.TrimEnd('2'), " <= condition.", conditionProperty.Name);
				/*****添加自定义数组类型的Lambda表达式*****/
				else if (conditionProperty.Type == null && conditionProperty.TypeName.EndsWith("[]"))
					lambda = string.Concat("m => m.", conditionProperty.Name, ".In(condition.", conditionProperty.Name, ")");

				whereInvoke.Parameters.Add(new CodeSnippetExpression(lambda));
				conditionStatement.TrueStatements.Add(whereInvoke);
				method.Statements.Add(conditionStatement);
			}
			CodeMethodReferenceExpression paginationMethod = new CodeMethodReferenceExpression(queryEntitiesReference, "GetDataTable");
			CodeMethodInvokeExpression paginationInvoke = new CodeMethodInvokeExpression(paginationMethod);
			method.Statements.Add(paginationInvoke);
		}

		/// <summary>
		/// 表示执行 GetDataTable 方法的命令。
		/// </summary>
		/// <param name="members">当前类型申明的 CodeTypeMemberCollection 。</param>
		protected internal void GenerateDesignerGetDataTable(CodeTypeMemberCollection members)
		{
			DataEntityElement entity = EntityElement;
			CodeMemberMethod method = new CodeMemberMethod(); members.Add(method);
			this.CreateDesignerMemberMethod(method);
			string paramTable = string.Format("<param name=\"table\">表示 {0} 类的实例。</param>", entity.DataTableName);
			string paramComment = string.Format("<param name=\"condition\">表示 {0} 类的实例。</param>", entity.Condition.EntityName);
			string returnComment = string.Format("<returns>返回查询结果，此结果表示 {0} 类的集合。</returns>", entity.EntityName);
			method.Comments.Add(new CodeCommentStatement(paramTable, true));
			method.Comments.Add(new CodeCommentStatement(paramComment, true));
			method.Comments.Add(new CodeCommentStatement(returnComment, true));
			method.Parameters.Add(new CodeParameterDeclarationExpression(EntityElement.DataTableName, "table"));
			if (!string.IsNullOrWhiteSpace(_Condition))
				method.Parameters.Add(new CodeParameterDeclarationExpression(_Condition, "condition"));
			else if (entity.Condition.Arguments.Count > 0 || entity.BaseCondition != typeof(AbstractCondition).FullName)
				method.Parameters.Add(new CodeParameterDeclarationExpression(EntityElement.Condition.EntityName, "condition"));
			else    /*表示没有条件类是采用默认参数*/
			{
				method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "pageSize"));
				method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "pageIndex"));
			}

			CodeVariableReferenceExpression tableReference = new CodeVariableReferenceExpression("table");
			CodeMethodReferenceExpression methodReference = new CodeMethodReferenceExpression();
			methodReference.MethodName = "GetDataTable";
			methodReference.TargetObject = new CodeBaseReferenceExpression();
			methodReference.TypeArguments.Add(EntityElement.FullDataRowName);
			CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(methodReference);

			methodInvoke.Parameters.Add(tableReference);
			methodInvoke.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));

			if (!string.IsNullOrWhiteSpace(_Condition))
				methodInvoke.Parameters.Add(new CodeVariableReferenceExpression("condition"));
			else if (entity.Condition.Arguments.Count > 0 || entity.BaseCondition != typeof(AbstractCondition).FullName)
				methodInvoke.Parameters.Add(new CodeVariableReferenceExpression("condition"));
			else    /*表示没有条件类是采用默认参数*/
			{
				methodInvoke.Parameters.Add(new CodeVariableReferenceExpression("pageSize"));
				methodInvoke.Parameters.Add(new CodeVariableReferenceExpression("pageIndex"));
			}
			method.ReturnType = new CodeTypeReference("QueryDataTable");
			method.ReturnType.TypeArguments.Add(EntityElement.FullDataRowName);
			method.Statements.Add(new CodeMethodReturnStatement(methodInvoke));
		}

		/// <summary>
		/// 表示执行 GetEntities 方法的命令。
		/// </summary>
		/// <param name="members">当前类型申明的 CodeTypeMemberCollection 。</param>
		/// <param name="method">需要定义的当前方法</param>
		/// <param name="entity">当前实体</param>
		protected internal void GenerateContextGetEntities(CodeTypeMemberCollection members, CodeMemberMethod method)
		{
			DataEntityElement entity = EntityElement;
			string paramComment = string.Format("<param name=\"condition\">表示 {0} 类的实例。</param>", EntityElement.Condition.EntityName);
			string returnComment = string.Format("<returns>返回查询结果，此结果表示 {0} 类的集合。</returns>", EntityElement.EntityName);
			if (!string.IsNullOrWhiteSpace(_Condition))
			{
				method.Comments.Add(new CodeCommentStatement(paramComment, true));
				method.Parameters.Add(new CodeParameterDeclarationExpression(_Condition, "condition"));
			}
			else if (EntityElement.Condition.Arguments.Count > 0)
			{
				method.Comments.Add(new CodeCommentStatement(paramComment, true));
				method.Parameters.Add(new CodeParameterDeclarationExpression(EntityElement.Condition.EntityName, "condition"));
			}
			else if (EntityElement.Condition.BaseClass != typeof(AbstractCondition).FullName)
			{
				method.Comments.Add(new CodeCommentStatement(paramComment, true));
				method.Parameters.Add(new CodeParameterDeclarationExpression(EntityElement.Condition.EntityName, "condition"));
			}
			method.Comments.Add(new CodeCommentStatement(returnComment, true));

			CodeVariableReferenceExpression accessReference = new CodeVariableReferenceExpression("access");
			CodeVariableReferenceExpression conditionReference = new CodeVariableReferenceExpression("condition");

			string methodName = Kind == ConfigurationTypeEnum.SearchTable ? "GetEntities" : MethodName;
			CodeMethodReferenceExpression methodReference = new CodeMethodReferenceExpression(accessReference, methodName);

			if (Kind == ConfigurationTypeEnum.SearchTable && Persistent.BaseAccess == typeof(AbstractAccess).Name)
				methodReference.TypeArguments.Add(EntityElement.EntityName);
			CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(methodReference);
			if (!string.IsNullOrWhiteSpace(_Condition))
				methodInvoke.Parameters.Add(conditionReference);
			else if (EntityElement.Condition.Arguments.Count > 0)
				methodInvoke.Parameters.Add(conditionReference);
			else if (EntityElement.Condition.BaseClass != typeof(AbstractCondition).FullName)
				methodInvoke.Parameters.Add(conditionReference);
			else { methodInvoke.Parameters.Add(new CodePrimitiveExpression(0)); methodInvoke.Parameters.Add(new CodePrimitiveExpression(0)); }

			CodeTypeReference queryEntitiesType = new CodeTypeReference("QueryEntities");
			queryEntitiesType.TypeArguments.Add(EntityElement.EntityName);
			CodeVariableDeclarationStatement iQueryEntities = new CodeVariableDeclarationStatement(queryEntitiesType, "queries");
			iQueryEntities.InitExpression = methodInvoke;
			method.Statements.Add(iQueryEntities);
			CodeVariableReferenceExpression queriesReference = new CodeVariableReferenceExpression("queries");
			DataEntityPropertyCollection entityProperties = EntityElement.Properties;
			foreach (DataConditionPropertyElement conditionProperty in EntityElement.Condition.Arguments)
			{
				//如果是日期类型的查询条件，则移除末尾数字在判断是否存在属性
				string name = conditionProperty.Name;
				if (name.EndsWith("1") || name.EndsWith("2")) { name = name.TrimEnd(new char[] { '1', '2' }); }
				if (!entityProperties.ContainsKey(name)) { continue; }
				CodePropertyReferenceExpression hasProperty = new CodePropertyReferenceExpression(conditionReference, conditionProperty.HasName);
				CodeConditionStatement conditionStatement = new CodeConditionStatement();
				conditionStatement.Condition = new CodeBinaryOperatorExpression(hasProperty,
					CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true));
				CodeMethodReferenceExpression whereMethod = new CodeMethodReferenceExpression(queriesReference, "Where");
				CodeMethodInvokeExpression whereInvoke = new CodeMethodInvokeExpression(whereMethod);
				string lambda = string.Concat("m => m.", conditionProperty.Name, " == condition.", conditionProperty.Name);
				if (conditionProperty.Type != null && conditionProperty.Type == typeof(string))
					lambda = string.Concat("m => m.", conditionProperty.Name, ".Like(condition.", conditionProperty.Name, ")");
				else if (conditionProperty.Type != null && conditionProperty.Type.IsArray)
					lambda = string.Concat("m => m.", conditionProperty.Name, ".In(condition.", conditionProperty.Name, ")");
				else if (conditionProperty.Type != null && conditionProperty.Type == typeof(DateTime) && conditionProperty.Name.EndsWith("1"))
					lambda = string.Concat("m => m.", conditionProperty.Name.TrimEnd('1'), " >= condition.", conditionProperty.Name);
				else if (conditionProperty.Type != null && conditionProperty.Type == typeof(DateTime) && conditionProperty.Name.EndsWith("2"))
					lambda = string.Concat("m => m.", conditionProperty.Name.TrimEnd('2'), " <= condition.", conditionProperty.Name);

				/*****添加自定义数组类型的Lambda表达式*****/
				else if (conditionProperty.Type == null && conditionProperty.TypeName.EndsWith("[]"))
					lambda = string.Concat("m => m.", conditionProperty.Name, ".In(condition.", conditionProperty.Name, ")");
				whereInvoke.Parameters.Add(new CodeSnippetExpression(lambda));
				conditionStatement.TrueStatements.Add(whereInvoke);
				method.Statements.Add(conditionStatement);
			}
			string baseMethodName = AsyncGenerated == true ? "ToPaginationAsync" : "ToPagination";
			CodeMethodReferenceExpression paginationMethod = new CodeMethodReferenceExpression(queriesReference, baseMethodName);
			CodeMethodInvokeExpression paginationInvoke = new CodeMethodInvokeExpression(paginationMethod);
			CodeMethodReturnStatement codeResult = new CodeMethodReturnStatement(paginationInvoke);
			if (AsyncGenerated == true)
			{
				CodeTypeReference iPaginationReference = new CodeTypeReference("IPagination");
				iPaginationReference.TypeArguments.Add(EntityElement.EntityName);
				method.ReturnType = new CodeTypeReference("Task");
				method.ReturnType.TypeArguments.Add(iPaginationReference);
			}
			else
			{
				method.ReturnType = new CodeTypeReference("IPagination");
				method.ReturnType.TypeArguments.Add(EntityElement.EntityName);
			}

			method.Statements.Add(codeResult);
		}

		/// <summary>
		/// 表示执行 GetEntities 方法的命令。
		/// </summary>
		/// <param name="members">当前类型申明的 CodeTypeMemberCollection 。</param>
		protected internal void GenerateDesignerGetEntities(CodeTypeMemberCollection members)
		{
			DataEntityElement entity = EntityElement;
			string returnComment = string.Format("<returns>返回查询结果，此结果表示 {0} 类的集合。</returns>", entity.EntityName);
			CodeMethodReferenceExpression methodReference = new CodeMethodReferenceExpression();
			methodReference.MethodName = "GetEntities";
			methodReference.TargetObject = new CodeBaseReferenceExpression();
			methodReference.TypeArguments.Add(EntityElement.EntityName);

			#region 生成无参数重载方法 GetEntities<T>(int pageSize, int pageIndex)
			CodeMemberMethod method0 = new CodeMemberMethod();
			this.CreateDesignerMemberMethod(method0);
			string sizeParam0 = "<param name=\"pageSize\">需要查询的当前页大小</param>";
			string indexParam0 = "<param name=\"pageIndex\">需要查询的当前页索引,索引从1开始</param>";
			method0.Comments.Add(new CodeCommentStatement(sizeParam0, true));
			method0.Comments.Add(new CodeCommentStatement(indexParam0, true));
			method0.Comments.Add(new CodeCommentStatement(returnComment, true));
			method0.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "pageSize"));
			method0.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "pageIndex"));

			CodeMethodInvokeExpression methodInvoke0 = new CodeMethodInvokeExpression(methodReference);
			methodInvoke0.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));
			methodInvoke0.Parameters.Add(new CodeVariableReferenceExpression("pageSize"));
			methodInvoke0.Parameters.Add(new CodeVariableReferenceExpression("pageIndex"));

			method0.ReturnType = new CodeTypeReference(typeof(QueryEntities<>).Name);
			method0.ReturnType.TypeArguments.Add(EntityElement.EntityName);
			method0.Statements.Add(new CodeMethodReturnStatement(methodInvoke0));
			members.Add(method0);
			#endregion

			#region 生成匿名重载方法 GetEntities<T>(string cmdName, object anonObject)
			CodeMemberMethod method1 = new CodeMemberMethod();
			this.CreateDesignerMemberMethod(method1);
			string paramComment1 = "<param name=\"anonObject\">数据实体类，包含了需要执行参数的值</param>";
			method1.Comments.Add(new CodeCommentStatement(paramComment1, true));
			method1.Comments.Add(new CodeCommentStatement(returnComment, true));
			method1.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "anonObject"));

			CodeMethodInvokeExpression methodInvoke1 = new CodeMethodInvokeExpression(methodReference);
			methodInvoke1.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));
			methodInvoke1.Parameters.Add(new CodeVariableReferenceExpression("anonObject"));

			method1.ReturnType = new CodeTypeReference(typeof(QueryEntities<>).Name);
			method1.ReturnType.TypeArguments.Add(EntityElement.EntityName);
			method1.Statements.Add(new CodeMethodReturnStatement(methodInvoke1));
			members.Add(method1);
			#endregion

			if (string.IsNullOrWhiteSpace(_Condition) == false)
			{
				CodeMemberMethod method2 = new CodeMemberMethod(); members.Add(method2);
				this.CreateDesignerMemberMethod(method2);
				string paramComment = string.Format("<param name=\"condition\">表示 {0} 类的实例。</param>", _Condition);

				method2.Comments.Add(new CodeCommentStatement(paramComment, true));
				method2.Comments.Add(new CodeCommentStatement(returnComment, true));
				method2.Parameters.Add(new CodeParameterDeclarationExpression(_Condition, "condition"));

				CodeMethodInvokeExpression methodInvoke2 = new CodeMethodInvokeExpression(methodReference);
				methodInvoke2.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));
				methodInvoke2.Parameters.Add(new CodeVariableReferenceExpression("condition"));

				method2.ReturnType = new CodeTypeReference(typeof(QueryEntities<>).Name);
				method2.ReturnType.TypeArguments.Add(EntityElement.EntityName);
				method2.Statements.Add(new CodeMethodReturnStatement(methodInvoke2));
			}
			else if (entity.Condition.Arguments.Count > 0 || entity.BaseCondition != typeof(AbstractCondition).FullName)
			{
				CodeMemberMethod method2 = new CodeMemberMethod(); members.Add(method2);
				this.CreateDesignerMemberMethod(method2);
				string paramComment = string.Format("<param name=\"condition\">表示 {0} 类的实例。</param>", entity.Condition.EntityName);

				method2.Comments.Add(new CodeCommentStatement(paramComment, true));
				method2.Comments.Add(new CodeCommentStatement(returnComment, true));
				method2.Parameters.Add(new CodeParameterDeclarationExpression(EntityElement.Condition.EntityName, "condition"));

				CodeMethodInvokeExpression methodInvoke2 = new CodeMethodInvokeExpression(methodReference);
				methodInvoke2.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));
				methodInvoke2.Parameters.Add(new CodeVariableReferenceExpression("condition"));

				method2.ReturnType = new CodeTypeReference(typeof(QueryEntities<>).Name);
				method2.ReturnType.TypeArguments.Add(EntityElement.EntityName);
				method2.Statements.Add(new CodeMethodReturnStatement(methodInvoke2));
			}
		}

		/// <summary>
		/// 扩展设计时代码
		/// </summary>
		/// <param name="members">命令集合</param>
		/// <param name="method">当前命令DomCode</param>
		protected override void WriteContextDesignerCode(CodeTypeMemberCollection members, CodeMemberMethod method)
		{
			if (ExecutableMethod == DynamicMethodEnum.GetEntities)
				GenerateContextGetEntities(members, method);
			else if (ExecutableMethod == DynamicMethodEnum.GetDataTable)
				GenerateContextGetDataTable(members, method);
		}
	}
}
