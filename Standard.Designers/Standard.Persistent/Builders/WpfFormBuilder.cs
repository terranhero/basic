using Basic.Configuration;
using Basic.DataEntities;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Builders
{
	/// <summary>
	/// 表示控制器生成类方法
	/// </summary>
	internal sealed class WpfFormBuilder : AbstractViewBuilder
	{
		private readonly EnvDTE.Project _Project;
		private readonly EnvDTE.ProjectItem _ProjectItem;
		/// <summary>
		/// 初始化 WpfFormBuilder 类实例
		/// </summary>
		internal WpfFormBuilder(PersistentService service, EnvDTE.Project project, EnvDTE.ProjectItem item)
			: base(service, project) { _Project = project; _ProjectItem = item; }

		/// <summary>
		/// 模版名称
		/// </summary>
		protected override string TemplateName { get { return "WpfGrid"; } }

		/// <summary>
		/// 表示控制器名称。
		/// </summary>
		public string ControllerClass { get { return string.Concat(base.Controller, "Controller"); } }

		private bool _IndexEnabled = true;
		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool IndexEnabled
		{
			get { return _IndexEnabled; }
			set
			{
				if (_IndexEnabled != value)
				{
					_IndexEnabled = value;
					OnPropertyChanged("IndexEnabled");
				}
			}
		}

		private bool _CreateEnabled = true;
		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool CreateEnabled
		{
			get { return _CreateEnabled; }
			set
			{
				if (_CreateEnabled != value)
				{
					_CreateEnabled = value;
					OnPropertyChanged("CreateEnabled");
				}
			}
		}

		private bool _EditEnabled = true;
		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool EditEnabled
		{
			get { return _EditEnabled; }
			set
			{
				if (_EditEnabled != value)
				{
					_EditEnabled = value;
					OnPropertyChanged("EditEnabled");
				}
			}
		}

		private bool _DeleteEnabled = true;
		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool DeleteEnabled
		{
			get { return _DeleteEnabled; }
			set
			{
				if (_DeleteEnabled != value)
				{
					_DeleteEnabled = value;
					OnPropertyChanged("DeleteEnabled");
				}
			}
		}

		private bool _SearchEnabled = true;
		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool SearchEnabled
		{
			get { return _SearchEnabled; }
			set
			{
				if (_SearchEnabled != value)
				{
					_SearchEnabled = value;
					OnPropertyChanged("SearchEnabled");
				}
			}
		}

		private bool _AdSearchEnabled = true;
		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool AdSearchEnabled
		{
			get { return _AdSearchEnabled; }
			set
			{
				if (_AdSearchEnabled != value)
				{
					_AdSearchEnabled = value;
					OnPropertyChanged("AdSearchEnabled");
				}
			}
		}

		/// <summary>
		/// 生成 CodeDOM 可编译代码单元
		/// </summary>
		/// <param name="codeComplieUnit">需要生成的 CodeCompileUnit 类实例。</param>
		/// <param name="defaultNamespace">生成类的默认命名空间。</param>
		public void GenerateCode(CodeCompileUnit codeComplieUnit, string defaultNamespace)
		{
			// Just for VB.NET:
			// Option Strict On (controls whether implicit type conversions are allowed)
			codeComplieUnit.UserData.Add("AllowLateBound", false);
			// Option Explicit On (controls whether variable declarations are required)
			codeComplieUnit.UserData.Add("RequireVariableDeclaration", true);

			CodeNamespace codeNamespace = new CodeNamespace(defaultNamespace);
			codeComplieUnit.Namespaces.Add(codeNamespace);

			CodeTypeDeclaration codeClass = new CodeTypeDeclaration(ControllerClass);
			codeClass.Comments.Add(new CodeCommentStatement("<summary>", true));
			codeClass.Comments.Add(new CodeCommentStatement(_Persistent.TableInfo.Description, true));
			codeClass.Comments.Add(new CodeCommentStatement("</summary>", true));
			codeNamespace.Types.Add(codeClass);
			codeClass.BaseTypes.Add(new CodeTypeReference("Controller"));

			codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.Collections"));
			codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.EntityLayer"));
			codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.Enums"));
			codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.Interfaces"));
			codeNamespace.Imports.Add(new CodeNamespaceImport("Basic.MvcLibrary"));
			if (!string.IsNullOrEmpty(_Persistent.Namespace))
				codeNamespace.Imports.Add(new CodeNamespaceImport(_Persistent.Namespace));
			if (_Persistent.Namespace != _Persistent.EntityNamespace)
				codeNamespace.Imports.Add(new CodeNamespaceImport(_Persistent.EntityNamespace));
			codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
			codeNamespace.Imports.Add(new CodeNamespaceImport("System.Web.Mvc"));
			codeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
			codeNamespace.Imports.Add(new CodeNamespaceImport("System.Text.RegularExpressions"));
			CodeThisReferenceExpression codeThisReference = new CodeThisReferenceExpression();
			CodeMemberField codeContext = new CodeMemberField(_Persistent.ContextName, "context");
			codeContext.InitExpression = new CodeObjectCreateExpression(_Persistent.ContextName);
			codeClass.Members.Add(codeContext);
			if (_IndexEnabled) { AddIndexAction(codeClass); }
			if (_CreateEnabled) { AddCreateAction(codeClass); }
			if (_EditEnabled) { AddEditAction(codeClass); }
			if (_DeleteEnabled) { AddDeleteAction(codeClass); }
			if (_SearchEnabled) { AddSearchAction(codeClass); }
			if (_AdSearchEnabled) { AddComplexSearchAction(codeClass); }
		}

		private void AddIndexAction(CodeTypeDeclaration codeClass)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			method.Comments.Add(new CodeCommentStatement("<summary>", true));
			method.Comments.Add(new CodeCommentStatement("Index Get Action ", true));
			method.Comments.Add(new CodeCommentStatement("</summary>", true));
			method.Name = "Index";
			method.ReturnType = new CodeTypeReference("ActionResult");
			CodeAttributeDeclaration attributeAcceptVerbs = new CodeAttributeDeclaration("AcceptVerbs");
			CodeTypeReferenceExpression httpVerbsType = new CodeTypeReferenceExpression("HttpVerbs");
			CodeFieldReferenceExpression httpVerbsGet = new CodeFieldReferenceExpression(httpVerbsType, "Get");
			attributeAcceptVerbs.Arguments.Add(new CodeAttributeArgument(httpVerbsGet));
			method.CustomAttributes.Add(attributeAcceptVerbs);
			CodeMethodReturnStatement codeReturn = new CodeMethodReturnStatement();
			CodeMethodReferenceExpression viewMethod = new CodeMethodReferenceExpression(null, "View");
			codeReturn.Expression = new CodeMethodInvokeExpression(viewMethod);
			method.Statements.Add(codeReturn);
			codeClass.Members.Add(method);
		}

		private void AddCreateAction(CodeTypeDeclaration codeClass)
		{
			DataEntityElement newEntity = _Persistent.NewEntity;
			DataEntityElement searchEntity = _Persistent.SearchEntity;
			if (newEntity == null) { return; }
			CodeMemberMethod getMethod = new CodeMemberMethod();
			getMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			getMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			getMethod.Comments.Add(new CodeCommentStatement("Create Get Action ", true));
			getMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			getMethod.Name = "Create";
			getMethod.ReturnType = new CodeTypeReference("ActionResult");
			CodeAttributeDeclaration getAcceptVerbs = new CodeAttributeDeclaration("AcceptVerbs");
			CodeTypeReferenceExpression httpVerbsType = new CodeTypeReferenceExpression("HttpVerbs");
			CodeFieldReferenceExpression httpVerbsGet = new CodeFieldReferenceExpression(httpVerbsType, "Get");
			getAcceptVerbs.Arguments.Add(new CodeAttributeArgument(httpVerbsGet));
			getMethod.CustomAttributes.Add(getAcceptVerbs);
			CodeVariableDeclarationStatement vdNewEntity = new CodeVariableDeclarationStatement(newEntity.ClassName, "entity");
			vdNewEntity.InitExpression = new CodeObjectCreateExpression(newEntity.ClassName);
			getMethod.Statements.Add(vdNewEntity);
			CodeMethodReturnStatement codeReturn = new CodeMethodReturnStatement();
			CodeMethodReferenceExpression viewMethod = new CodeMethodReferenceExpression(null, "View");
			CodeMethodInvokeExpression viewMethodInvoke = new CodeMethodInvokeExpression(viewMethod);
			viewMethodInvoke.Parameters.Add(new CodeFieldReferenceExpression(null, "entity"));
			codeReturn.Expression = viewMethodInvoke;
			getMethod.Statements.Add(codeReturn);
			codeClass.Members.Add(getMethod);

			CodeThisReferenceExpression thisReference = new CodeThisReferenceExpression();
			CodeMemberMethod postMethod = new CodeMemberMethod();
			postMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			CodeAttributeDeclaration postAcceptVerbs = new CodeAttributeDeclaration("AcceptVerbs");
			CodeFieldReferenceExpression httpVerbsPost = new CodeFieldReferenceExpression(httpVerbsType, "Post");
			postAcceptVerbs.Arguments.Add(new CodeAttributeArgument(httpVerbsPost));
			postMethod.CustomAttributes.Add(postAcceptVerbs);

			postMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			postMethod.Comments.Add(new CodeCommentStatement("Create Post Action ", true));
			postMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			postMethod.Name = "Create";
			postMethod.ReturnType = new CodeTypeReference("ActionResult");
			CodeParameterDeclarationExpression pdNewEntity = new CodeParameterDeclarationExpression(newEntity.ClassName, "entity");
			postMethod.Parameters.Add(pdNewEntity);

			CodeConditionStatement ifStatement = new CodeConditionStatement();
			CodeBinaryOperatorExpression ifCondition = new CodeBinaryOperatorExpression();
			CodeFieldReferenceExpression modelStateReference = new CodeFieldReferenceExpression(null, "ModelState");
			ifCondition.Left = new CodeFieldReferenceExpression(modelStateReference, "IsValid");
			ifCondition.Operator = CodeBinaryOperatorType.ValueEquality;
			ifCondition.Right = new CodePrimitiveExpression(true);
			ifStatement.Condition = ifCondition;

			postMethod.Statements.Add(ifStatement);
			CodeFieldReferenceExpression contextReference = new CodeFieldReferenceExpression(null, "context");
			CodeMethodReferenceExpression addNewReference = new CodeMethodReferenceExpression(contextReference, "AddNew");
			CodeMethodInvokeExpression addNewInvoke = new CodeMethodInvokeExpression(addNewReference);
			addNewInvoke.Parameters.Add(new CodeVariableReferenceExpression("entity"));
			CodeVariableDeclarationStatement resultVariable = new CodeVariableDeclarationStatement(typeof(Result).Name, "result");
			resultVariable.InitExpression = addNewInvoke;
			ifStatement.TrueStatements.Add(resultVariable);

			#region if (result.Successful == false)
			CodeMethodReferenceExpression jsonMvcMethod = new CodeMethodReferenceExpression(thisReference, "JsonMvc");
			CodeConditionStatement resultCondition = new CodeConditionStatement();
			CodeBinaryOperatorExpression resultConditionExpression = new CodeBinaryOperatorExpression();
			CodeVariableReferenceExpression resultReference = new CodeVariableReferenceExpression("result");
			resultConditionExpression.Left = new CodeFieldReferenceExpression(resultReference, "Successful");
			resultConditionExpression.Operator = CodeBinaryOperatorType.ValueEquality;
			resultConditionExpression.Right = new CodePrimitiveExpression(false);
			resultCondition.Condition = resultConditionExpression;

			CodeMethodInvokeExpression jsonMvcInvoke0 = new CodeMethodInvokeExpression(jsonMvcMethod);
			jsonMvcInvoke0.Parameters.Add(new CodeArgumentReferenceExpression("entity"));
			jsonMvcInvoke0.Parameters.Add(resultReference);
			resultCondition.TrueStatements.Add(new CodeMethodReturnStatement(jsonMvcInvoke0));
			ifStatement.TrueStatements.Add(resultCondition);
			#endregion

			#region ??Entity entityGrid = new ??Entity(PrimaryKey);
			string className = newEntity.ClassName;
			if (searchEntity != null) { className = searchEntity.ClassName; }
			CodeVariableDeclarationStatement vdEntity = new CodeVariableDeclarationStatement(className, "entityGrid");
			CodeObjectCreateExpression initExpression = new CodeObjectCreateExpression(className);
			CodeVariableReferenceExpression entityReference = new CodeVariableReferenceExpression("entity");
			foreach (DataEntityPropertyElement property in newEntity.Properties)
			{
				if (property.PrimaryKey)
					initExpression.Parameters.Add(new CodeFieldReferenceExpression(entityReference, property.Name));
			}
			vdEntity.InitExpression = initExpression;
			ifStatement.TrueStatements.Add(vdEntity);

			CodeMethodReferenceExpression searchByKeyReference = new CodeMethodReferenceExpression(contextReference, "SearchByKey");
			CodeMethodInvokeExpression searchByKeyInvoke = new CodeMethodInvokeExpression(searchByKeyReference);
			searchByKeyInvoke.Parameters.Add(new CodeVariableReferenceExpression("entityGrid"));
			ifStatement.TrueStatements.Add(searchByKeyInvoke);
			#endregion

			#region  return this.JsonView<??Entity>("Grid", entityGrid);
			CodeMethodReferenceExpression jsonViewReference = new CodeMethodReferenceExpression(thisReference, "JsonView");
			jsonViewReference.TypeArguments.Add(className);
			CodeMethodInvokeExpression jsonViewInvoke = new CodeMethodInvokeExpression(jsonViewReference);
			jsonViewInvoke.Parameters.Add(new CodePrimitiveExpression("Grid"));
			jsonViewInvoke.Parameters.Add(new CodeVariableReferenceExpression("entityGrid"));
			ifStatement.TrueStatements.Add(new CodeMethodReturnStatement(jsonViewInvoke));
			#endregion

			CodeMethodInvokeExpression jsonMvcInvoke = new CodeMethodInvokeExpression(jsonMvcMethod);
			jsonMvcInvoke.Parameters.Add(new CodeArgumentReferenceExpression("entity"));
			jsonMvcInvoke.Parameters.Add(new CodeArgumentReferenceExpression("ModelState"));
			postMethod.Statements.Add(new CodeMethodReturnStatement(jsonMvcInvoke));

			codeClass.Members.Add(postMethod);
		}

		private void AddEditAction(CodeTypeDeclaration codeClass)
		{
			DataEntityElement editEntity = _Persistent.NewEntity;
			DataEntityElement searchEntity = _Persistent.SearchEntity;

			if (editEntity == null || searchEntity == null) { return; }

			#region  [AcceptVerbs(HttpVerbs.Get)]ActionResult Edit
			CodeMemberMethod getMethod = new CodeMemberMethod();
			getMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			getMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			getMethod.Comments.Add(new CodeCommentStatement("Edit Get Action ", true));
			getMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			getMethod.Name = "Edit";
			getMethod.ReturnType = new CodeTypeReference("ActionResult");
			CodeAttributeDeclaration attributeAcceptVerbs = new CodeAttributeDeclaration("AcceptVerbs");
			CodeTypeReferenceExpression httpVerbsType = new CodeTypeReferenceExpression("HttpVerbs");
			CodeFieldReferenceExpression httpVerbsGet = new CodeFieldReferenceExpression(httpVerbsType, "Get");
			attributeAcceptVerbs.Arguments.Add(new CodeAttributeArgument(httpVerbsGet));
			getMethod.CustomAttributes.Add(attributeAcceptVerbs);

			#region ??Entity entity = new ??Entity(PrimaryKey);
			CodeVariableDeclarationStatement cvdEntity = new CodeVariableDeclarationStatement(searchEntity.ClassName, "entity");
			CodeObjectCreateExpression newMethod = new CodeObjectCreateExpression(searchEntity.ClassName);
			foreach (DataEntityPropertyElement property in editEntity.Properties)
			{
				if (property.PrimaryKey && property.Type != null)
				{
					getMethod.Parameters.Add(new CodeParameterDeclarationExpression(property.Type, property.Name));
					newMethod.Parameters.Add(new CodeVariableReferenceExpression(property.Name));
				}
				else if (property.PrimaryKey && property.Type == null)
				{
					getMethod.Parameters.Add(new CodeParameterDeclarationExpression(property.TypeName, property.Name));
					newMethod.Parameters.Add(new CodeVariableReferenceExpression(property.Name));
				}
			}
			cvdEntity.InitExpression = newMethod;
			getMethod.Statements.Add(cvdEntity);
			#endregion

			#region context.SearchByKey(entity);
			CodeFieldReferenceExpression contextReference = new CodeFieldReferenceExpression(null, "context");
			CodeMethodReferenceExpression searchByKeyReference = new CodeMethodReferenceExpression(contextReference, "SearchByKey");
			CodeMethodInvokeExpression searchByKeyInvoke = new CodeMethodInvokeExpression(searchByKeyReference);
			searchByKeyInvoke.Parameters.Add(new CodeVariableReferenceExpression("entity"));
			getMethod.Statements.Add(searchByKeyInvoke);
			#endregion

			CodeMethodReferenceExpression viewReference = new CodeMethodReferenceExpression(null, "View");
			CodeMethodInvokeExpression getViewInvoke = new CodeMethodInvokeExpression(viewReference);
			getViewInvoke.Parameters.Add(new CodeVariableReferenceExpression("entity"));
			getMethod.Statements.Add(new CodeMethodReturnStatement(getViewInvoke));
			codeClass.Members.Add(getMethod);
			#endregion

			#region  [AcceptVerbs(HttpVerbs.Post)]ActionResult Edit
			CodeThisReferenceExpression codeThisReference = new CodeThisReferenceExpression();
			CodeMemberMethod postMethod = new CodeMemberMethod();
			postMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			CodeAttributeDeclaration postAcceptVerbs = new CodeAttributeDeclaration("AcceptVerbs");
			CodeFieldReferenceExpression httpVerbsPost = new CodeFieldReferenceExpression(httpVerbsType, "Post");
			postAcceptVerbs.Arguments.Add(new CodeAttributeArgument(httpVerbsPost));
			postMethod.CustomAttributes.Add(postAcceptVerbs);

			postMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			postMethod.Comments.Add(new CodeCommentStatement("Edit Post Action ", true));
			postMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			postMethod.Name = "Edit";
			postMethod.ReturnType = new CodeTypeReference("ActionResult");
			CodeParameterDeclarationExpression pdNewEntity = new CodeParameterDeclarationExpression(editEntity.ClassName, "entity");
			postMethod.Parameters.Add(pdNewEntity);

			CodeConditionStatement ifStatement = new CodeConditionStatement();
			CodeBinaryOperatorExpression ifCondition = new CodeBinaryOperatorExpression();
			CodeFieldReferenceExpression modelStateReference = new CodeFieldReferenceExpression(null, "ModelState");
			ifCondition.Left = new CodeFieldReferenceExpression(modelStateReference, "IsValid");
			ifCondition.Operator = CodeBinaryOperatorType.ValueEquality;
			ifCondition.Right = new CodePrimitiveExpression(true);
			ifStatement.Condition = ifCondition;

			postMethod.Statements.Add(ifStatement);
			CodeMethodReferenceExpression addNewReference = new CodeMethodReferenceExpression(contextReference, "Update");
			CodeMethodInvokeExpression addNewInvoke = new CodeMethodInvokeExpression(addNewReference);
			addNewInvoke.Parameters.Add(new CodeVariableReferenceExpression("entity"));
			CodeVariableDeclarationStatement resultVariable = new CodeVariableDeclarationStatement(typeof(Result).Name, "result");
			resultVariable.InitExpression = addNewInvoke;
			ifStatement.TrueStatements.Add(resultVariable);

			#region if (result.Successful == false)
			CodeMethodReferenceExpression jsonMvcMethod = new CodeMethodReferenceExpression(codeThisReference, "JsonMvc");
			CodeConditionStatement resultCondition = new CodeConditionStatement();
			CodeBinaryOperatorExpression resultConditionExpression = new CodeBinaryOperatorExpression();
			CodeVariableReferenceExpression resultReference = new CodeVariableReferenceExpression("result");
			resultConditionExpression.Left = new CodeFieldReferenceExpression(resultReference, "Successful");
			resultConditionExpression.Operator = CodeBinaryOperatorType.ValueEquality;
			resultConditionExpression.Right = new CodePrimitiveExpression(false);
			resultCondition.Condition = resultConditionExpression;

			CodeMethodInvokeExpression jsonMvcInvoke0 = new CodeMethodInvokeExpression(jsonMvcMethod);
			jsonMvcInvoke0.Parameters.Add(new CodeArgumentReferenceExpression("entity"));
			jsonMvcInvoke0.Parameters.Add(resultReference);
			resultCondition.TrueStatements.Add(new CodeMethodReturnStatement(jsonMvcInvoke0));
			ifStatement.TrueStatements.Add(resultCondition);
			#endregion

			#region  return this.JsonMvc(true);
			CodeMethodReferenceExpression jsonMvcReference = new CodeMethodReferenceExpression(codeThisReference, "JsonMvc");
			//jsonViewReference.TypeArguments.Add(className);
			CodeMethodInvokeExpression jsonMvcInvoke = new CodeMethodInvokeExpression(jsonMvcReference);
			jsonMvcInvoke.Parameters.Add(new CodePrimitiveExpression(true));
			ifStatement.TrueStatements.Add(new CodeMethodReturnStatement(jsonMvcInvoke));
			#endregion

			CodeMethodInvokeExpression endReturnJsonMvcInvoke = new CodeMethodInvokeExpression(jsonMvcMethod);
			endReturnJsonMvcInvoke.Parameters.Add(new CodeArgumentReferenceExpression("entity"));
			endReturnJsonMvcInvoke.Parameters.Add(new CodeArgumentReferenceExpression("ModelState"));
			postMethod.Statements.Add(new CodeMethodReturnStatement(endReturnJsonMvcInvoke));

			codeClass.Members.Add(postMethod);
			#endregion
		}

		private void AddDeleteAction(CodeTypeDeclaration codeClass)
		{
			DataEntityElement deleteEntity = _Persistent.DeleteEntity;
			DataEntityElement searchEntity = _Persistent.SearchEntity;

			if (deleteEntity == null) { return; }
			CodeThisReferenceExpression thisReference = new CodeThisReferenceExpression();
			CodeMethodReferenceExpression jsonMvcReference = new CodeMethodReferenceExpression(thisReference, "JsonMvc");
			CodeMemberMethod postMethod = new CodeMemberMethod();
			postMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			postMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			postMethod.Comments.Add(new CodeCommentStatement("Delete Post Action ", true));
			postMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			postMethod.Name = "Delete";
			postMethod.ReturnType = new CodeTypeReference("ActionResult");
			CodeTypeReference typeArray = new CodeTypeReference(string.Concat(deleteEntity.ClassName, "[]"));
			postMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeArray, "entityArray"));
			CodeAttributeDeclaration postAcceptVerbs = new CodeAttributeDeclaration("AcceptVerbs");
			CodeTypeReferenceExpression httpVerbsType = new CodeTypeReferenceExpression("HttpVerbs");
			CodeFieldReferenceExpression httpVerbsPost = new CodeFieldReferenceExpression(httpVerbsType, "Post");
			postAcceptVerbs.Arguments.Add(new CodeAttributeArgument(httpVerbsPost));
			postMethod.CustomAttributes.Add(postAcceptVerbs);

			#region if (entityArray == null || entityArray.Length == 0)
			CodeConditionStatement entitiesCondition = new CodeConditionStatement();
			CodeBinaryOperatorExpression bocLeft = new CodeBinaryOperatorExpression();
			bocLeft.Left = new CodeVariableReferenceExpression("entityArray");
			bocLeft.Operator = CodeBinaryOperatorType.ValueEquality;
			bocLeft.Right = new CodePrimitiveExpression();
			CodeBinaryOperatorExpression bocRight = new CodeBinaryOperatorExpression();
			bocRight.Left = new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("entityArray"), "Length");
			bocRight.Operator = CodeBinaryOperatorType.ValueEquality;
			bocRight.Right = new CodePrimitiveExpression(0);
			CodeBinaryOperatorExpression boCondition = new CodeBinaryOperatorExpression();
			entitiesCondition.Condition = new CodeBinaryOperatorExpression(bocLeft, CodeBinaryOperatorType.BooleanOr, bocRight);

			CodeMethodInvokeExpression jsonMvcInvoke1 = new CodeMethodInvokeExpression(jsonMvcReference);
			jsonMvcInvoke1.Parameters.Add(new CodePrimitiveExpression());
			jsonMvcInvoke1.Parameters.Add(new CodeVariableReferenceExpression("ModelState"));
			entitiesCondition.TrueStatements.Add(new CodeMethodReturnStatement(jsonMvcInvoke1));

			postMethod.Statements.Add(entitiesCondition);
			#endregion

			#region Result result = context.Delete(entityArray);
			CodeFieldReferenceExpression contextReference = new CodeFieldReferenceExpression(null, "context");
			CodeMethodReferenceExpression deleteReference = new CodeMethodReferenceExpression(contextReference, "Delete");
			CodeMethodInvokeExpression deleteInvoke = new CodeMethodInvokeExpression(deleteReference);
			deleteInvoke.Parameters.Add(new CodeVariableReferenceExpression("entityArray"));
			CodeVariableDeclarationStatement resultVariable = new CodeVariableDeclarationStatement(typeof(Result).Name, "result");
			resultVariable.InitExpression = deleteInvoke;
			postMethod.Statements.Add(resultVariable);
			#endregion

			#region if (result.Successful == false)
			CodeMethodReferenceExpression jsonMvcMethod = new CodeMethodReferenceExpression(thisReference, "JsonMvc");
			CodeConditionStatement resultCondition = new CodeConditionStatement();
			CodeBinaryOperatorExpression resultConditionExpression = new CodeBinaryOperatorExpression();
			CodeVariableReferenceExpression resultReference = new CodeVariableReferenceExpression("result");
			resultConditionExpression.Left = new CodeFieldReferenceExpression(resultReference, "Successful");
			resultConditionExpression.Operator = CodeBinaryOperatorType.ValueEquality;
			resultConditionExpression.Right = new CodePrimitiveExpression(false);
			resultCondition.Condition = resultConditionExpression;

			CodeMethodInvokeExpression jsonMvcInvoke0 = new CodeMethodInvokeExpression(jsonMvcMethod);
			jsonMvcInvoke0.Parameters.Add(new CodePrimitiveExpression());
			jsonMvcInvoke0.Parameters.Add(resultReference);
			resultCondition.TrueStatements.Add(new CodeMethodReturnStatement(jsonMvcInvoke0));
			postMethod.Statements.Add(resultCondition);
			#endregion

			CodeMethodInvokeExpression viewInvoke = new CodeMethodInvokeExpression(jsonMvcReference);
			viewInvoke.Parameters.Add(new CodePrimitiveExpression(true));
			postMethod.Statements.Add(new CodeMethodReturnStatement(viewInvoke));
			codeClass.Members.Add(postMethod);
		}

		private void AddSearchAction(CodeTypeDeclaration codeClass)
		{
			DataEntityElement searchEntity = _Persistent.SearchEntity;
			if (searchEntity == null) { return; }
			DataConditionElement dataCondition = searchEntity.Condition;

			CodeThisReferenceExpression thisReference = new CodeThisReferenceExpression();
			CodeMemberMethod postMethod = new CodeMemberMethod();
			postMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			postMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			postMethod.Comments.Add(new CodeCommentStatement("Search Post Action ", true));
			postMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			postMethod.Name = "Search";
			postMethod.ReturnType = new CodeTypeReference("ActionResult");
			postMethod.Parameters.Add(new CodeParameterDeclarationExpression(dataCondition.ClassName, "condition"));

			CodeAttributeDeclaration attributeAcceptVerbs = new CodeAttributeDeclaration("AcceptVerbs");
			CodeTypeReferenceExpression httpVerbsType = new CodeTypeReferenceExpression("HttpVerbs");
			CodeFieldReferenceExpression httpVerbsGet = new CodeFieldReferenceExpression(httpVerbsType, "Post");
			attributeAcceptVerbs.Arguments.Add(new CodeAttributeArgument(httpVerbsGet));
			postMethod.CustomAttributes.Add(attributeAcceptVerbs);

			#region IPagination<??Entity> result = context.GetEntities(condition);
			CodeTypeReference trIPagination = new CodeTypeReference(typeof(IPagination).Name);
			trIPagination.TypeArguments.Add(new CodeTypeReference(searchEntity.ClassName));
			CodeVariableDeclarationStatement vdIPagination = new CodeVariableDeclarationStatement(trIPagination, "result");
			CodeFieldReferenceExpression contextReference = new CodeFieldReferenceExpression(null, "context");
			CodeMethodReferenceExpression getEntitiesReference = new CodeMethodReferenceExpression(contextReference, "GetEntities");
			CodeMethodInvokeExpression getEntitiesInvoke = new CodeMethodInvokeExpression(getEntitiesReference);
			getEntitiesInvoke.Parameters.Add(new CodeVariableReferenceExpression("condition"));
			vdIPagination.InitExpression = getEntitiesInvoke;
			postMethod.Statements.Add(vdIPagination);
			#endregion

			CodeMethodReferenceExpression jsonViewReference = new CodeMethodReferenceExpression(thisReference, "JsonView");
			jsonViewReference.TypeArguments.Add(searchEntity.ClassName);
			CodeMethodInvokeExpression jsonViewInvoke = new CodeMethodInvokeExpression(jsonViewReference);
			jsonViewInvoke.Parameters.Add(new CodePrimitiveExpression("Grid"));
			jsonViewInvoke.Parameters.Add(new CodeVariableReferenceExpression("result"));
			postMethod.Statements.Add(new CodeMethodReturnStatement(jsonViewInvoke));

			codeClass.Members.Add(postMethod);
		}

		private void AddComplexSearchAction(CodeTypeDeclaration codeClass)
		{
			DataEntityElement searchEntity = _Persistent.SearchEntity;
			if (searchEntity == null) { return; }
			DataConditionElement dataCondition = searchEntity.Condition;

			CodeThisReferenceExpression thisReference = new CodeThisReferenceExpression();
			#region [AcceptVerbs(HttpVerbs.Get)]ActionResult AdSearch
			CodeMemberMethod getMethod = new CodeMemberMethod();
			getMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			getMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			getMethod.Comments.Add(new CodeCommentStatement("AdSearch Get Action ", true));
			getMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			getMethod.Name = "AdSearch";
			getMethod.ReturnType = new CodeTypeReference("ActionResult");
			CodeAttributeDeclaration attributeAcceptVerbs = new CodeAttributeDeclaration("AcceptVerbs");
			CodeTypeReferenceExpression httpVerbsType = new CodeTypeReferenceExpression("HttpVerbs");
			CodeFieldReferenceExpression httpVerbsGet = new CodeFieldReferenceExpression(httpVerbsType, "Get");
			attributeAcceptVerbs.Arguments.Add(new CodeAttributeArgument(httpVerbsGet));
			getMethod.CustomAttributes.Add(attributeAcceptVerbs);
			CodeMethodReferenceExpression viewReference = new CodeMethodReferenceExpression(null, "PartialView");
			CodeMethodInvokeExpression getViewInvoke = new CodeMethodInvokeExpression(viewReference);
			getMethod.Statements.Add(new CodeMethodReturnStatement(getViewInvoke));
			codeClass.Members.Add(getMethod);
			#endregion

			#region  [AcceptVerbs(HttpVerbs.Post)]ActionResult AdSearch
			CodeMemberMethod postMethod = new CodeMemberMethod();
			postMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			postMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			postMethod.Comments.Add(new CodeCommentStatement("AdSearch Post Action ", true));
			postMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			postMethod.Name = "AdSearch";
			postMethod.Parameters.Add(new CodeParameterDeclarationExpression(dataCondition.ClassName, "condition"));

			postMethod.ReturnType = new CodeTypeReference("ActionResult");
			CodeAttributeDeclaration postAcceptVerbs = new CodeAttributeDeclaration("AcceptVerbs");
			CodeFieldReferenceExpression httpVerbsPost = new CodeFieldReferenceExpression(httpVerbsType, "Post");
			postAcceptVerbs.Arguments.Add(new CodeAttributeArgument(httpVerbsPost));
			postMethod.CustomAttributes.Add(postAcceptVerbs);

			#region IPagination<??Entity> result = context.GetEntities(condition);
			CodeTypeReference trIPagination = new CodeTypeReference(typeof(IPagination).Name);
			trIPagination.TypeArguments.Add(new CodeTypeReference(searchEntity.ClassName));
			CodeVariableDeclarationStatement vdIPagination = new CodeVariableDeclarationStatement(trIPagination, "result");
			CodeFieldReferenceExpression contextReference = new CodeFieldReferenceExpression(null, "context");
			CodeMethodReferenceExpression getEntitiesReference = new CodeMethodReferenceExpression(contextReference, "GetEntities");
			CodeMethodInvokeExpression getEntitiesInvoke = new CodeMethodInvokeExpression(getEntitiesReference);
			getEntitiesInvoke.Parameters.Add(new CodeVariableReferenceExpression("condition"));
			vdIPagination.InitExpression = getEntitiesInvoke;
			postMethod.Statements.Add(vdIPagination);
			#endregion

			CodeMethodReferenceExpression jsonViewReference = new CodeMethodReferenceExpression(thisReference, "JsonView");
			jsonViewReference.TypeArguments.Add(searchEntity.ClassName);
			CodeMethodInvokeExpression jsonViewInvoke = new CodeMethodInvokeExpression(jsonViewReference);
			jsonViewInvoke.Parameters.Add(new CodePrimitiveExpression("Grid"));
			jsonViewInvoke.Parameters.Add(new CodeVariableReferenceExpression("result"));
			postMethod.Statements.Add(new CodeMethodReturnStatement(jsonViewInvoke));

			codeClass.Members.Add(postMethod);
			#endregion
		}
	}
}
