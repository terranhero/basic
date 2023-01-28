using System.Collections.Specialized;
using System.Xml;
using System.Xml.Serialization;
using Basic.Collections;
using GBS = Basic.Collections;

using Basic.Designer;
using Basic.Enums;
using Basic.DataEntities;
using System.CodeDom;
using System;
using System.Drawing.Design;
using Basic.EntityLayer;
using System.Threading.Tasks;

namespace Basic.Configuration
{
	/// <summary>
	/// 表示静态配置命令
	/// </summary>
	public class StaticCommandElement : DataCommandElement, IXmlSerializable, INotifyCollectionChanged
	{
		/// <summary>
		/// 初始化 StaticCommandElement 类实例
		/// </summary>
		internal StaticCommandElement(DataEntityElement entity)
			: base(entity)
		{
			_CheckCommands = new GBS.CheckedCommandCollection(this);
			_CheckCommands.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCheckCollectionChanged);
			_NewCommands = new NewCommandCollection(this);
			_NewCommands.CollectionChanged += new NotifyCollectionChangedEventHandler(OnNewCollectionChanged);
		}

		/// <summary>
		/// 引发 CollectionChanged 事件
		/// </summary>
		/// <param name="sender">引发事件的对象。</param>
		/// <param name="e">有关事件的信息。</param>
		private void OnCheckCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			base.RaisePropertyChanged("CheckCommandsVisibility");
			base.OnCollectionChanged(sender, e);
		}

		/// <summary>
		/// 引发 CollectionChanged 事件
		/// </summary>
		/// <param name="sender">引发事件的对象。</param>
		/// <param name="e">有关事件的信息。</param>
		private void OnNewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			base.RaisePropertyChanged("NewCommandsVisibility");
			base.OnCollectionChanged(sender, e);
		}

		#region Xml 节点名称常量
		/// <summary>
		/// 表示Xml元素名称
		/// </summary>
		protected internal const string XmlElementName = "StaticCommand";
		/// <summary>
		/// 表示 CommandText 元素。
		/// </summary>
		protected internal const string CommandTextElement = "CommandText";
		#endregion

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
				if (string.IsNullOrWhiteSpace(Name))
					return null;
				return Name;
			}
		}

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return GetType().Name.Replace("Element", ""); }

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName() { return Name; }

		private readonly GBS.CheckedCommandCollection _CheckCommands;
		private readonly NewCommandCollection _NewCommands;

		/// <summary>
		/// 获取检测命令集合
		/// </summary>
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCheckCommand)]
		[System.ComponentModel.Description("获取检测命令集合")]
		public GBS.CheckedCommandCollection CheckCommands { get { return _CheckCommands; } }

		/// <summary>
		/// 获取新值命令集合
		/// </summary>
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryNewCommand)]
		[System.ComponentModel.Description("获取新值命令集合")]
		public NewCommandCollection NewCommands { get { return _NewCommands; } }

		private string _CommandText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[System.ComponentModel.Editor(typeof(CommandTextEditor), typeof(UITypeEditor))]
		[Basic.Designer.PersistentDescription("PersistentDescription_CommandText")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDataCommand)]
		public string CommandText
		{
			get { return _CommandText; }
			set
			{
				if (_CommandText != value)
				{
					base.OnPropertyChanging("CommandText");
					_CommandText = value;
					base.RaisePropertyChanged("CommandText");
				}
			}
		}

		#region 生成 ExecuteNonQuery 方法
		/// <summary>
		/// 生成 ExecuteNonQuery 方法调用
		/// </summary>
		/// <param name="members">方法集合</param>
		/// <param name="method">当前方法主体</param>
		protected override void GenerateContextExecuteNonQuery(CodeTypeMemberCollection members, CodeMemberMethod method)
		{
			CodeVariableReferenceExpression accessReference = new CodeVariableReferenceExpression("access");
			CodeMethodReferenceExpression methodReference = new CodeMethodReferenceExpression(accessReference, MethodName);
			CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(methodReference);
			if (this.Parameters.Count > 0)
			{
				string paramComment = string.Format("<param name=\"entity\">表示 {0} 类的实例。</param>", EntityElement.EntityName);
				method.Comments.Add(new CodeCommentStatement(paramComment, true));
				method.Parameters.Add(new CodeParameterDeclarationExpression(EntityElement.EntityName, "entity"));
				CodeVariableReferenceExpression entityReference = new CodeVariableReferenceExpression("entity");
				methodInvoke.Parameters.Add(entityReference);
			}
			string returnComment = string.Format("<returns>执行ransact-SQL语句的返回值。</returns>");
			method.Comments.Add(new CodeCommentStatement(returnComment, true));
			CodeMethodReturnStatement codeResult = new CodeMethodReturnStatement(methodInvoke);
			method.ReturnType = new CodeTypeReference(typeof(Result).Name);
			method.Statements.Add(codeResult);
		}

		/// <summary>
		/// 生成 ExecuteNonQuery 方法调用
		/// </summary>
		/// <param name="members">方法集合</param>
		/// <param name="method">当前方法主体</param>
		/// <param name="targetFramework">生成代码的目标框架版本（例如：262144=v4.0）</param>
		protected override void GenerateDesignerExecuteNonQuery(CodeTypeMemberCollection members, int targetFramework)
		{
			string returnComment = string.Format("<returns>执行ransact-SQL语句的返回值。</returns>");

			#region 创建匿名实例方法
			CodeMemberMethod anonObjectMethod = new CodeMemberMethod();
			this.CreateDesignerMemberMethod(anonObjectMethod, false);
			CodeMethodReferenceExpression anonMethodReference = new CodeMethodReferenceExpression();
			anonMethodReference.MethodName = "ExecuteNonQuery";
			anonMethodReference.TargetObject = new CodeBaseReferenceExpression();
			CodeMethodInvokeExpression anonMethodInvoke = new CodeMethodInvokeExpression(anonMethodReference);
			anonMethodInvoke.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));
			if (this.Parameters.Count > 0)
			{
				string paramComment = string.Format("<param name=\"anonObject\">包含可执行参数的匿名类。</param>");
				anonObjectMethod.Comments.Add(new CodeCommentStatement(paramComment, true));
				anonObjectMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "anonObject"));
				anonMethodInvoke.Parameters.Add(new CodeVariableReferenceExpression("anonObject"));
			}
			anonObjectMethod.Comments.Add(new CodeCommentStatement(returnComment, true));
			anonObjectMethod.ReturnType = new CodeTypeReference(typeof(Result).Name);
			anonObjectMethod.Statements.Add(new CodeMethodReturnStatement(anonMethodInvoke));
			members.Add(anonObjectMethod);
			#endregion

			#region 创建单实例方法
			if (this.Parameters.Count > 0)
			{
				CodeMemberMethod singleMethod = new CodeMemberMethod();
				this.CreateDesignerMemberMethod(singleMethod, false);
				CodeMethodReferenceExpression methodReference = new CodeMethodReferenceExpression();
				methodReference.MethodName = "ExecuteNonQuery";
				methodReference.TargetObject = new CodeBaseReferenceExpression();
				CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(methodReference);
				methodInvoke.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));

				string paramComment = string.Format("<param name=\"entity\">表示 {0} 类的实例。</param>", EntityElement.EntityName);
				singleMethod.Comments.Add(new CodeCommentStatement(paramComment, true));
				singleMethod.Parameters.Add(new CodeParameterDeclarationExpression(EntityElement.EntityName, "entity"));
				methodInvoke.Parameters.Add(new CodeVariableReferenceExpression("entity"));

				singleMethod.Comments.Add(new CodeCommentStatement(returnComment, true));
				singleMethod.ReturnType = new CodeTypeReference(typeof(Result).Name);
				singleMethod.Statements.Add(new CodeMethodReturnStatement(methodInvoke));
				members.Add(singleMethod);
			}
			#endregion

			#region 创建实例数组方法
			if (this.Parameters.Count > 0)
			{
				CodeMemberMethod arrayMethod = new CodeMemberMethod();
				this.CreateDesignerMemberMethod(arrayMethod, false);
				CodeMethodReferenceExpression arrayMethodReference = new CodeMethodReferenceExpression();
				arrayMethodReference.MethodName = "ExecuteNonQuery";
				arrayMethodReference.TargetObject = new CodeBaseReferenceExpression();
				CodeMethodInvokeExpression arrayMethodInvoke = new CodeMethodInvokeExpression(arrayMethodReference);
				arrayMethodInvoke.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));
				string paramComment1 = string.Format("<param name=\"entities\">表示 {0} 类的实例。</param>", EntityElement.EntityName);
				arrayMethod.Comments.Add(new CodeCommentStatement(paramComment1, true));
				CodeTypeReference typeArrayReference = new CodeTypeReference(string.Concat(EntityElement.EntityName, "[]"));
				arrayMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeArrayReference, "entities"));
				arrayMethodInvoke.Parameters.Add(new CodeVariableReferenceExpression("entities"));
				arrayMethod.Comments.Add(new CodeCommentStatement(returnComment, true));
				arrayMethod.ReturnType = new CodeTypeReference(typeof(Result).Name);
				arrayMethod.Statements.Add(new CodeMethodReturnStatement(arrayMethodInvoke));
				members.Add(arrayMethod);
			}
			#endregion

			//if (targetFramework >= 262144)
			//{
			#region 创建匿名实例方法(异步)
			CodeMemberMethod anonObjectMethodAsync = new CodeMemberMethod();
			this.CreateDesignerMemberMethod(anonObjectMethodAsync, true);
			CodeMethodReferenceExpression anonMethodReferenceAsync = new CodeMethodReferenceExpression();
			anonMethodReferenceAsync.MethodName = "ExecuteNonQueryAsync";
			anonMethodReferenceAsync.TargetObject = new CodeBaseReferenceExpression();
			CodeMethodInvokeExpression anonMethodInvokeAsync = new CodeMethodInvokeExpression(anonMethodReferenceAsync);
			anonMethodInvokeAsync.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));
			if (this.Parameters.Count > 0)
			{
				string paramComment = string.Format("<param name=\"anonObject\">包含可执行参数的匿名类。</param>");
				anonObjectMethodAsync.Comments.Add(new CodeCommentStatement(paramComment, true));
				anonObjectMethodAsync.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "anonObject"));
				anonMethodInvokeAsync.Parameters.Add(new CodeVariableReferenceExpression("anonObject"));
			}
			anonObjectMethodAsync.Comments.Add(new CodeCommentStatement(returnComment, true));
			anonObjectMethodAsync.ReturnType = new CodeTypeReference(typeof(Task<>).MakeGenericType(typeof(Result)));
			anonObjectMethodAsync.Statements.Add(new CodeMethodReturnStatement(anonMethodInvokeAsync));
			members.Add(anonObjectMethodAsync);
			#endregion

			#region 创建单实例方法(异步)
			if (this.Parameters.Count > 0)
			{
				CodeMemberMethod singleMethodAsync = new CodeMemberMethod();
				this.CreateDesignerMemberMethod(singleMethodAsync, true);
				CodeMethodReferenceExpression methodRefAsync = new CodeMethodReferenceExpression();
				methodRefAsync.MethodName = "ExecuteNonQueryAsync";
				methodRefAsync.TargetObject = new CodeBaseReferenceExpression();
				CodeMethodInvokeExpression methodInvokeAsync = new CodeMethodInvokeExpression(methodRefAsync);
				methodInvokeAsync.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));

				string paramComment = string.Format("<param name=\"entity\">表示 {0} 类的实例。</param>", EntityElement.EntityName);
				singleMethodAsync.Comments.Add(new CodeCommentStatement(paramComment, true));
				singleMethodAsync.Parameters.Add(new CodeParameterDeclarationExpression(EntityElement.EntityName, "entity"));
				methodInvokeAsync.Parameters.Add(new CodeVariableReferenceExpression("entity"));
				singleMethodAsync.Comments.Add(new CodeCommentStatement(returnComment, true));
				singleMethodAsync.ReturnType = new CodeTypeReference(typeof(Task<>).MakeGenericType(typeof(Result)));
				singleMethodAsync.Statements.Add(new CodeMethodReturnStatement(methodInvokeAsync));
				members.Add(singleMethodAsync);
			}
			
			#endregion

			#region 创建实例数组方法(异步)
			if (this.Parameters.Count > 0)
			{
				CodeMemberMethod arrayMethodAsync = new CodeMemberMethod();
				this.CreateDesignerMemberMethod(arrayMethodAsync, true);
				CodeMethodReferenceExpression arrayMethodRefAsync = new CodeMethodReferenceExpression();
				arrayMethodRefAsync.MethodName = "ExecuteNonQueryAsync";
				arrayMethodRefAsync.TargetObject = new CodeBaseReferenceExpression();
				CodeMethodInvokeExpression arrayMethodInvokeAsync = new CodeMethodInvokeExpression(arrayMethodRefAsync);
				arrayMethodInvokeAsync.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));
				string paramComment1 = string.Format("<param name=\"entities\">表示 {0} 类的实例。</param>", EntityElement.EntityName);
				arrayMethodAsync.Comments.Add(new CodeCommentStatement(paramComment1, true));
				CodeTypeReference typeArrayReference = new CodeTypeReference(string.Concat(EntityElement.EntityName, "[]"));
				arrayMethodAsync.Parameters.Add(new CodeParameterDeclarationExpression(typeArrayReference, "entities"));
				arrayMethodInvokeAsync.Parameters.Add(new CodeVariableReferenceExpression("entities"));
				arrayMethodAsync.Comments.Add(new CodeCommentStatement(returnComment, true));
				arrayMethodAsync.ReturnType = new CodeTypeReference(typeof(Task<>).MakeGenericType(typeof(Result)));
				arrayMethodAsync.Statements.Add(new CodeMethodReturnStatement(arrayMethodInvokeAsync));
				members.Add(arrayMethodAsync);
			}
			#endregion
			//}
		}
		#endregion

		#region 生成 ExecuteScalar 方法
		/// <summary>
		/// 生成 ExecuteScalar 方法调用
		/// </summary>
		/// <param name="members">方法集合</param>
		/// <param name="method">当前方法主体</param>
		/// <param name="entity">实体模型定义</param>
		protected override void GenerateContextExecuteScalar(CodeTypeMemberCollection members, CodeMemberMethod method)
		{
			CodeVariableReferenceExpression accessReference = new CodeVariableReferenceExpression("access");
			CodeMethodReferenceExpression methodReference = new CodeMethodReferenceExpression(accessReference, MethodName);
			CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(methodReference);
			string paramComment = string.Format("<param name=\"entity\">表示 {0} 类的实例。</param>", EntityElement.EntityName);
			method.Comments.Add(new CodeCommentStatement(paramComment, true));
			method.Parameters.Add(new CodeParameterDeclarationExpression(EntityElement.EntityName, "entity"));
			CodeVariableReferenceExpression entityReference = new CodeVariableReferenceExpression("entity");
			methodInvoke.Parameters.Add(entityReference);
			string returnComment = string.Format("<returns>执行ransact-SQL语句的返回值。</returns>");
			method.Comments.Add(new CodeCommentStatement(returnComment, true));
			CodeMethodReturnStatement codeResult = new CodeMethodReturnStatement(methodInvoke);
			method.ReturnType = new CodeTypeReference(typeof(object), CodeTypeReferenceOptions.GenericTypeParameter);
			method.Statements.Add(codeResult);
		}

		/// <summary>
		/// 生成 ExecuteScalar 方法调用
		/// </summary>
		/// <param name="members">方法集合</param>
		protected override void GenerateDesignerExecuteScalar(CodeTypeMemberCollection members)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			this.CreateDesignerMemberMethod(method);
			CodeMethodReferenceExpression methodReference = new CodeMethodReferenceExpression();
			methodReference.MethodName = "ExecuteScalar";
			methodReference.TargetObject = new CodeBaseReferenceExpression();
			CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(methodReference);
			methodInvoke.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));
			string paramComment = string.Format("<param name=\"entity\">表示 {0} 类的实例。</param>", EntityElement.EntityName);
			method.Comments.Add(new CodeCommentStatement(paramComment, true));

			method.Parameters.Add(new CodeParameterDeclarationExpression(EntityElement.EntityName, "entity"));
			methodInvoke.Parameters.Add(new CodeVariableReferenceExpression("entity"));
			string returnComment = string.Format("<returns>执行ransact-SQL语句的返回值。</returns>");
			method.Comments.Add(new CodeCommentStatement(returnComment, true));
			method.ReturnType = new CodeTypeReference(typeof(object), CodeTypeReferenceOptions.GenericTypeParameter);
			method.Statements.Add(new CodeMethodReturnStatement(methodInvoke));
			members.Add(method);
		}
		#endregion

		#region 生成 ExecuteCore<DataRow> 方法
		/// <summary>
		/// 生成 ExecuteCore 代码段
		/// </summary>
		/// <param name="members">表示CodeDOM方法集合</param>
		/// <param name="method">表示CodeDOM方法</param>
		protected override void GenerateContextExecuteCore(CodeTypeMemberCollection members, CodeMemberMethod method)
		{
			CodeVariableReferenceExpression accessReference = new CodeVariableReferenceExpression("access");
			CodeMethodReferenceExpression methodReference = new CodeMethodReferenceExpression(accessReference, MethodName);
			CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(methodReference);
			CodeMethodReturnStatement codeResult = new CodeMethodReturnStatement(methodInvoke);
			if (this.Parameters.Count > 0)
			{
				string paramComment = string.Format("<param name=\"row\">表示强类型 {0} 类的实例。</param>", EntityElement.FullDataRowName);
				CodeTypeReference codeTypeReference = new CodeTypeReference(EntityElement.FullDataRowName);
				method.Comments.Add(new CodeCommentStatement(paramComment, true));
				method.Parameters.Add(new CodeParameterDeclarationExpression(codeTypeReference, "row"));
				methodInvoke.Parameters.Add(new CodeFieldReferenceExpression(null, "row"));
			}
			string returnComment = string.Format("<returns>执行ransact-SQL语句的返回值。</returns>");
			method.Comments.Add(new CodeCommentStatement(returnComment, true));
			method.ReturnType = new CodeTypeReference(typeof(Result).Name);
			method.Statements.Add(codeResult);
		}

		/// <summary>
		/// 生成 ExecuteCore 代码段
		/// </summary>
		/// <param name="members">表示CodeDOM方法集合</param>
		protected override void GenerateDesignerExecuteCore(CodeTypeMemberCollection members)
		{
			#region 单实例方法
			CodeMemberMethod singleMethod = new CodeMemberMethod();
			this.CreateDesignerMemberMethod(singleMethod);
			singleMethod.ReturnType = new CodeTypeReference(typeof(Result).Name);
			CodeMethodReferenceExpression singleMethodReference = new CodeMethodReferenceExpression();
			singleMethodReference.MethodName = "ExecuteCore";
			singleMethodReference.TargetObject = new CodeBaseReferenceExpression();
			CodeMethodInvokeExpression singleMethodInvoke = new CodeMethodInvokeExpression(singleMethodReference);
			singleMethodInvoke.Method.TypeArguments.Add(EntityElement.FullDataRowName);
			singleMethodInvoke.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));
			if (this.Parameters.Count > 0)
			{
				string paramComment = string.Format("<param name=\"row\">表示强类型 {0} 类的实例。</param>", EntityElement.FullDataRowName);
				CodeTypeReference codeTypeReference = new CodeTypeReference(EntityElement.FullDataRowName);
				singleMethod.Comments.Add(new CodeCommentStatement(paramComment, true));
				singleMethod.Parameters.Add(new CodeParameterDeclarationExpression(codeTypeReference, "row"));
				singleMethodInvoke.Parameters.Add(new CodeFieldReferenceExpression(null, "row"));
			}
			singleMethod.Comments.Add(new CodeCommentStatement("<returns>执行ransact-SQL语句的返回值。</returns>", true));
			singleMethod.Statements.Add(new CodeMethodReturnStatement(singleMethodInvoke));
			members.Add(singleMethod);
			#endregion

			#region 数组实例方法
			CodeMemberMethod arrayMethod = new CodeMemberMethod();
			this.CreateDesignerMemberMethod(arrayMethod);
			arrayMethod.ReturnType = new CodeTypeReference(typeof(Result).Name);
			CodeMethodReferenceExpression arrayMethodReference = new CodeMethodReferenceExpression();
			arrayMethodReference.MethodName = "ExecuteCore";
			arrayMethodReference.TargetObject = new CodeBaseReferenceExpression();
			CodeMethodInvokeExpression arrayMethodInvoke = new CodeMethodInvokeExpression(arrayMethodReference);
			arrayMethodInvoke.Method.TypeArguments.Add(EntityElement.FullDataRowName);
			arrayMethodInvoke.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));
			if (this.Parameters.Count > 0)
			{
				string paramComment = string.Format("<param name=\"table\">表示强类型 {0} 类的实例。</param>", EntityElement.DataTableName);
				CodeTypeReference codeTypeReference = new CodeTypeReference(EntityElement.DataTableName);
				arrayMethod.Comments.Add(new CodeCommentStatement(paramComment, true));
				arrayMethod.Parameters.Add(new CodeParameterDeclarationExpression(codeTypeReference, "table"));
				arrayMethodInvoke.Parameters.Add(new CodeFieldReferenceExpression(null, "table"));
			}
			arrayMethod.Comments.Add(new CodeCommentStatement("<returns>执行ransact-SQL语句的返回值。</returns>", true));
			arrayMethod.Statements.Add(new CodeMethodReturnStatement(arrayMethodInvoke));
			members.Add(arrayMethod);
			#endregion
		}
		#endregion

		#region 生成 BatchExecute<DataRow> 方法
		/// <summary>
		/// 生成 ExecuteCore 代码段
		/// </summary>
		/// <param name="members">表示CodeDOM方法集合</param>
		/// <param name="method">表示CodeDOM方法</param>
		protected internal void GenerateContextBatchExecute(CodeTypeMemberCollection members, CodeMemberMethod method)
		{
			CodeVariableReferenceExpression accessReference = new CodeVariableReferenceExpression("access");
			CodeMethodReferenceExpression methodReference = new CodeMethodReferenceExpression(accessReference, MethodName);
			CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(methodReference);
			CodeMethodReturnStatement codeResult = new CodeMethodReturnStatement(methodInvoke);

			string paramComment = string.Format("<param name=\"table\">表示强类型 {0} 类的实例。</param>", EntityElement.DataTableName);
			CodeTypeReference codeTypeReference = new CodeTypeReference(EntityElement.DataTableName);
			method.Comments.Add(new CodeCommentStatement(paramComment, true));
			method.Parameters.Add(new CodeParameterDeclarationExpression(codeTypeReference, "table"));
			methodInvoke.Parameters.Add(new CodeFieldReferenceExpression(null, "table"));

			string returnComment = string.Format("<returns>执行ransact-SQL语句的返回值。</returns>");
			method.Comments.Add(new CodeCommentStatement(returnComment, true));
			method.ReturnType = new CodeTypeReference(typeof(Result).Name);
			method.Statements.Add(codeResult);
		}

		/// <summary>
		/// 生成 ExecuteCore 代码段
		/// </summary>
		/// <param name="members">表示CodeDOM方法集合</param>
		protected internal void GenerateDesignerBatchExecute(CodeTypeMemberCollection members)
		{
			CodeMemberMethod method = new CodeMemberMethod(); members.Add(method);
			this.CreateDesignerMemberMethod(method);
			method.ReturnType = new CodeTypeReference(typeof(Result).Name);
			CodeMethodReferenceExpression methodReference = new CodeMethodReferenceExpression();
			methodReference.MethodName = "BatchExecute";
			methodReference.TargetObject = new CodeBaseReferenceExpression();
			CodeMethodInvokeExpression codeMethodInvoke = new CodeMethodInvokeExpression(methodReference);
			codeMethodInvoke.Method.TypeArguments.Add(EntityElement.FullDataRowName);
			codeMethodInvoke.Parameters.Add(new CodePrimitiveExpression(this.ConfigurationName));

			string paramComment = string.Format("<param name=\"table\">表示强类型 {0} 类的实例。</param>", EntityElement.DataTableName);
			CodeTypeReference codeTypeReference = new CodeTypeReference(EntityElement.DataTableName);
			method.Comments.Add(new CodeCommentStatement(paramComment, true));
			method.Parameters.Add(new CodeParameterDeclarationExpression(codeTypeReference, "table"));
			codeMethodInvoke.Parameters.Add(new CodeFieldReferenceExpression(null, "table"));

			method.Comments.Add(new CodeCommentStatement("<returns>执行ransact-SQL语句的返回值。</returns>", true));
			method.Statements.Add(new CodeMethodReturnStatement(codeMethodInvoke));
		}
		#endregion
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
				StaticMethodEnum staticEnum = StaticMethodEnum.ExecuteNonQuery;
				if (Enum.TryParse<StaticMethodEnum>(value, true, out staticEnum))
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
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == GBS.CheckedCommandCollection.XmlElementName)
			{
				System.Xml.XmlReader reader2 = reader.ReadSubtree();
				while (reader2.Read())  //读取所有静态命令节点信息
				{
					if (reader2.NodeType == XmlNodeType.Whitespace) { continue; }
					else if (reader2.NodeType == XmlNodeType.Element && reader2.LocalName == CheckedCommandElement.XmlElementName)
					{
						CheckedCommandElement checkCommand = new CheckedCommandElement(this);
						checkCommand.ReadXml(reader2.ReadSubtree());
						_CheckCommands.Add(checkCommand);
					}
					else if (reader2.NodeType == XmlNodeType.EndElement && reader2.LocalName == GBS.CheckedCommandCollection.XmlElementName)
						break;
				}
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == NewCommandCollection.XmlElementName)
			{
				System.Xml.XmlReader reader2 = reader.ReadSubtree();
				while (reader2.Read())  //读取所有静态命令节点信息
				{
					if (reader2.NodeType == XmlNodeType.Whitespace) { continue; }
					else if (reader2.NodeType == XmlNodeType.Element && reader2.LocalName == NewCommandElement.XmlElementName)
					{
						NewCommandElement newCommand = new NewCommandElement(this);
						newCommand.ReadXml(reader2.ReadSubtree());
						_NewCommands.Add(newCommand);
					}
					else if (reader2.NodeType == XmlNodeType.EndElement && reader2.LocalName == NewCommandCollection.XmlElementName)
						break;
				}
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == CommandTextElement)//兼容5.0新版结构信息
			{
				_CommandText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == XmlElementName)
			{
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
			if (_ExecutableMethod != StaticMethodEnum.ExecuteNonQuery)
				writer.WriteAttributeString(ExecutableAttribute, _ExecutableMethod.ToString());
			base.WriteAttribute(writer);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement(CommandTextElement);
			writer.WriteCData(CommandText);//写CData
			writer.WriteEndElement();
			if (_CheckCommands != null && _CheckCommands.Count > 0)
			{
				writer.WriteStartElement(GBS.CheckedCommandCollection.XmlElementName);
				foreach (CheckedCommandElement checkCommand in _CheckCommands)
				{
					checkCommand.WriteXml(writer);
				}
				writer.WriteEndElement();
			}
			if (_NewCommands != null && _NewCommands.Count > 0)
			{
				writer.WriteStartElement(NewCommandCollection.XmlElementName);
				foreach (NewCommandElement newCommand in _NewCommands)
				{
					newCommand.WriteXml(writer);
				}
				writer.WriteEndElement();
			}
			base.WriteContent(writer);
		}

		private StaticMethodEnum _ExecutableMethod = StaticMethodEnum.ExecuteNonQuery;
		/// <summary>
		/// 表示当前命令调用基类方法实例
		/// </summary>
		[System.ComponentModel.Description("获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程")]
		[System.ComponentModel.DefaultValue(typeof(StaticMethodEnum), "ExecuteNonQuery")]
		[Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryCodeGenerator)]
		public StaticMethodEnum ExecutableMethod
		{
			get { return _ExecutableMethod; }
			set
			{
				if (_ExecutableMethod != value)
				{
					_ExecutableMethod = value;
					base.RaisePropertyChanged("Name");
					base.RaisePropertyChanged("MethodName");
					base.RaisePropertyChanged("ExecutableMethod");
				}
			}
		}

		/// <summary>
		/// 扩展设计时代码
		/// </summary>
		/// <param name="members">命令集合</param>
		/// <param name="method">当前命令DomCode</param>
		protected override void WriteContextDesignerCode(CodeTypeMemberCollection members, CodeMemberMethod method)
		{
			if (ExecutableMethod == StaticMethodEnum.FillDataSet)
				GenerateContextDataSet(members, method);
			else if (ExecutableMethod == StaticMethodEnum.FillDataTable)
				GenerateContextDataTable(members, method);
			else if (ExecutableMethod == StaticMethodEnum.GetPagination)
				GenerateContextGetPagination(members, method);
			else if (ExecutableMethod == StaticMethodEnum.ExecuteCore)
				GenerateContextExecuteCore(members, method);
			else if (ExecutableMethod == StaticMethodEnum.ExecuteNonQuery)
				GenerateContextExecuteNonQuery(members, method);
			else if (ExecutableMethod == StaticMethodEnum.ExecuteReader)
				GenerateContextExecuteReader(members, method);
			else if (ExecutableMethod == StaticMethodEnum.ExecuteScalar)
				GenerateContextExecuteScalar(members, method);
			else if (ExecutableMethod == StaticMethodEnum.SearchEntity)
				GenerateContextSearchEntity(members, method);
			else if (_ExecutableMethod == StaticMethodEnum.BatchExecute)
				GenerateContextBatchExecute(members, method);
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
			if (_ExecutableMethod == StaticMethodEnum.FillDataSet)
				GenerateDesignerDataSet(classCode.Members);
			else if (_ExecutableMethod == StaticMethodEnum.FillDataTable)
				GenerateDesignerDataTable(classCode.Members);
			else if (_ExecutableMethod == StaticMethodEnum.GetPagination)
				GenerateDesignerGetPagination(classCode.Members);
			else if (_ExecutableMethod == StaticMethodEnum.ExecuteCore)
				GenerateDesignerExecuteCore(classCode.Members);
			else if (_ExecutableMethod == StaticMethodEnum.BatchExecute)
				GenerateDesignerBatchExecute(classCode.Members);
			else if (_ExecutableMethod == StaticMethodEnum.ExecuteNonQuery)
				GenerateDesignerExecuteNonQuery(classCode.Members, targetFramework);
			else if (_ExecutableMethod == StaticMethodEnum.ExecuteReader)
				GenerateDesignerExecuteReader(classCode.Members);
			else if (_ExecutableMethod == StaticMethodEnum.ExecuteScalar)
				GenerateDesignerExecuteScalar(classCode.Members);
			else if (_ExecutableMethod == StaticMethodEnum.SearchEntity)
				GenerateDesignerSearchEntity(classCode.Members);
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
			writer.WriteStartElement(CommandTextElement);
			writer.WriteCData(CreateCommandText(_CommandText, connectionType));//写CData
			writer.WriteEndElement();
			if (_CheckCommands != null && _CheckCommands.Count > 0)
			{
				writer.WriteStartElement(GBS.CheckedCommandCollection.XmlElementName);
				foreach (CheckedCommandElement checkCommand in _CheckCommands)
				{
					checkCommand.GenerateConfiguration(writer, connectionType);
				}
				writer.WriteEndElement();
			}
			if (_NewCommands != null && _NewCommands.Count > 0)
			{
				writer.WriteStartElement(NewCommandCollection.XmlElementName);
				foreach (NewCommandElement newCommand in _NewCommands)
				{
					newCommand.GenerateConfiguration(writer, connectionType);
				}
				writer.WriteEndElement();
			}
			base.GenerateConfiguration(writer, connectionType);
			writer.WriteEndElement();
		}
	}
}
