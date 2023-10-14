using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Basic.Collections;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Exceptions;
using Basic.Expressions;
using Basic.Properties;
using Basic.Tables;
using BD = Basic.DataAccess;
using System.Collections.Concurrent;
namespace Basic.DataAccess
{
	/// <summary>
	/// 表示要对数据源执行的 SQL 语句或存储过程。为表示命令的、数据库特有的类提供一个基类。
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.ComponentModel.ToolboxItem(false)]
	public abstract class DynamicCommand : DataCommand, IDbCommand, IXmlSerializable
	{
		#region Xml 节点名称常量
		/// <summary>
		/// 表示Xml元素名称
		/// </summary>
		protected internal const string XmlElementName = "DynamicCommand";

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

		/// <summary>缓存内连接命令</summary>
		private static readonly ConcurrentDictionary<string, DynamicJoinCommand> _innerJoins = new ConcurrentDictionary<string, DynamicJoinCommand>();
		/// <summary>表示需要连接的查询命令</summary>
		protected JoinCommand joinCommand;

		/// <summary>表示需要连接的查询命令</summary>
		protected internal DynamicJoinCommand _dynamicJoinCommand;

		/// <summary>
		/// 释放数据库连接
		/// </summary>
		protected internal override void ReleaseConnection()
		{
			mOrderList.Clear();
			TempWhereText = null;
			Parameters.Clear();
			base.ReleaseConnection();
		}

		/// <summary>
		/// 初始化 DynamicCommand 类的新实例。 
		/// </summary>
		/// <param name="dbCommand"></param>
		protected DynamicCommand(DbCommand dbCommand)
			: base(dbCommand)
		{
			OrderText = string.Empty; DistinctStatus = false;
			_WithClauses = new WithClauseCollection(this);
		}

		/// <summary>
		/// 
		/// </summary>
		protected readonly OrderFieldList mOrderList = new OrderFieldList(10);
		/// <summary>向排序集合末尾添加字段。</summary>
		/// <param name="field">排序字段名</param>
		/// <param name="isAscending">是否从小到大排序（true表示Ascending，false表示Descending）</param>
		private void AddToOrderList(string field, bool isAscending) { mOrderList.Add(field, isAscending); }

		/// <summary>设置需要串联的命令</summary>
		internal bool InitializeJoinCommand<T>()
		{
			Type type = typeof(T);
			if (_innerJoins.TryGetValue(type.FullName, out DynamicJoinCommand cmd))
			{
				_dynamicJoinCommand = cmd; return true;
			}
			List<InnerJoinAttribute> joins = new List<InnerJoinAttribute>(10);
			List<JoinParameterAttribute> parameters = new List<JoinParameterAttribute>(10);
			List<JoinOrderAttribute> orders = new List<JoinOrderAttribute>(10);
			for (Type et = type; et != null; et = et.BaseType)
			{
				foreach (var attribute in et.GetCustomAttributes(false))
				{
					if (attribute is InnerJoinAttribute) { joins.Add((InnerJoinAttribute)attribute); }
					else if (attribute is JoinParameterAttribute) { parameters.Add((JoinParameterAttribute)attribute); }
					else if (attribute is JoinOrderAttribute) { orders.Add((JoinOrderAttribute)attribute); }
				}
			}
			EntityPropertyProvidor.TryGetProperties(type, out EntityPropertyCollection properties);
			List<string> fields = new List<string>(50);
			foreach (EntityPropertyMeta meta in properties)
			{
				if (meta.JoinField == null) { continue; }
				fields.Add(meta.JoinField.Script);
			}
			List<string> whereClauses = new List<string>(10);
			List<DbParameter> dbParameters = new List<DbParameter>(10);
			foreach (JoinParameterAttribute param in parameters)
			{
				DbParameter parameter = CreateParameter(param);
				dbParameters.Add(parameter);
				whereClauses.Add(param.WhereClause.Replace("{%" + param.FieldName + "%}", parameter.ParameterName));
			}
			if (fields.Count == 0 || joins.Count == 0) { return false; }
			_dynamicJoinCommand = new DynamicJoinCommand(string.Join(", ", fields),
				string.Join("\r\n", joins.Select(m => m.JoinScript)),
				string.Join(" AND ", whereClauses),
				 string.Join(", ", orders.SelectMany(m => m.OrderClauses)),
				dbParameters.ToArray()
				);
			return _innerJoins.TryAdd(type.FullName, _dynamicJoinCommand);
		}

		/// <summary>设置需要串联的命令</summary>
		/// <param name="joinCmd"></param>
		internal void SetJoinCommand(JoinCommand joinCmd)
		{
			joinCommand = joinCmd;
		}

		/// <summary>
		/// 当前查询语句是否启用排序重复选项
		/// </summary>
		protected internal bool DistinctStatus { get; private set; }

		/// <summary>排除重复记录。</summary>
		internal void Distinct() { DistinctStatus = true; }

		/// <summary>
		/// 创建动态参数条件
		/// </summary>
		/// <param name="builder">构建SQL语句的StringBuilder类实例</param>
		/// <param name="parameter">动态参数</param>
		protected virtual void CreateDynamicParameter(StringBuilder builder, DynamicParameter parameter)
		{
			string fieldName = parameter.ParameterName;
			string parameterName = CreateParameterName(fieldName);
			if (parameter.CompareOperator == ParameterCompare.Equal)
				builder.AppendFormat("{0}={1}", fieldName, parameterName);
			else if (parameter.CompareOperator == ParameterCompare.NotEqual)
				builder.AppendFormat("{0}<>{1}", fieldName, parameterName);
			else if (parameter.CompareOperator == ParameterCompare.GreaterThan)
				builder.AppendFormat("{0}>{1}", fieldName, parameterName);
			else if (parameter.CompareOperator == ParameterCompare.GreaterThanEqual)
				builder.AppendFormat("{0}>={1}", fieldName, parameterName);
			else if (parameter.CompareOperator == ParameterCompare.LessThan)
				builder.AppendFormat("{0}<{1}", fieldName, parameterName);
			else if (parameter.CompareOperator == ParameterCompare.LessThanEqual)
				builder.AppendFormat("{0}<={1}", fieldName, parameterName);
			else if (parameter.CompareOperator == ParameterCompare.Like)
				builder.AppendFormat("{0} LIKE {1}", fieldName, parameterName);
			else if (parameter.CompareOperator == ParameterCompare.NotLike)
				builder.AppendFormat("{0} NOT LIKE {1}", fieldName, parameterName);
			else if (parameter.CompareOperator == ParameterCompare.In)
				builder.AppendFormat("{0} IN({1})", fieldName, parameterName);
			else if (parameter.CompareOperator == ParameterCompare.NotIn)
				builder.AppendFormat("{0} NOT IN({1})", fieldName, parameterName);
			else if (parameter.CompareOperator == ParameterCompare.BitAnd)
				builder.AppendFormat("(({0} & {1})>0)", fieldName, parameterName);
			else if (parameter.CompareOperator == ParameterCompare.IsNull)
				builder.AppendFormat("{0} IS NULL", fieldName);
			else if (parameter.CompareOperator == ParameterCompare.IsNotNull)
				builder.AppendFormat("{0} IS NOT NULL", fieldName);
		}

		/// <summary>
		/// 根据 ColumnAttribute 特性创建数据库参数。
		/// </summary>
		/// <param name="ca">包含数据库字段信息的特性信息。</param>
		internal DbParameter CreateParameter(ColumnAttribute ca)
		{
			DbParameter parameter = CreateParameter();
			parameter.ParameterName = CreateParameterName(ca.ColumnName);
			parameter.SourceColumn = ca.ColumnName;
			parameter.Size = ca.Size;
			parameter.Direction = ParameterDirection.Input;
			parameter.IsNullable = ca.Nullable;
			ConvertParameterType(parameter, ca.DataType, ca.Precision, ca.Scale);
			return parameter;
		}

		/// <summary>
		/// 根据 ColumnAttribute 特性创建数据库参数。
		/// </summary>
		/// <param name="ca">包含数据库字段信息的特性信息。</param>
		internal DbParameter CreateParameter(ColumnMappingAttribute ca)
		{
			DbParameter parameter = CreateParameter();
			parameter.ParameterName = CreateParameterName(ca.ColumnName);
			parameter.SourceColumn = ca.ColumnName;
			parameter.Size = ca.Size;
			parameter.Direction = ParameterDirection.Input;
			parameter.IsNullable = ca.Nullable;
			ConvertParameterType(parameter, ca.DataType, ca.Precision, ca.Scale);
			return parameter;
		}

		/// <summary>
		/// 根据 JoinParameterAttribute 特性创建数据库参数。
		/// </summary>
		/// <param name="jp">包含数据库字段信息的特性信息。</param>
		private DbParameter CreateParameter(JoinParameterAttribute jp)
		{
			DbParameter parameter = CreateParameter();
			parameter.ParameterName = CreateParameterName(jp.FieldName);
			parameter.SourceColumn = jp.FieldName;
			parameter.Size = jp.Size;
			parameter.Direction = ParameterDirection.Input;
			ConvertParameterType(parameter, jp.DataType, 0, 0);
			return parameter;
		}

		#region 根据Lambda 表达式集合创建Where条件和参数
		/// <summary>
		/// 创建数据库比较语句
		/// </summary>
		/// <param name="lambdaCollection">Lambda 表达式集合</param>
		internal void CreateWhere(LambdaExpressionCollection lambdaCollection)
		{
			if (lambdaCollection.Count > 0)
			{
				StringBuilder whereBuilder = new StringBuilder(1000);
				foreach (LambdaConditionExpression expression in lambdaCollection)
				{
					if (whereBuilder.Length != 0)
						whereBuilder.Append(" AND ");
					CreateLambdaConditionExpression(whereBuilder, expression);
				}
				TempWhereText = whereBuilder.ToString();
			}
		}

		private void CreateLambdaConditionExpression(StringBuilder builder, LambdaConditionExpression expression)
		{
			if (expression is BinaryConditionExpression)
			{
				CreateBinaryConditionExpression(builder, expression as BinaryConditionExpression);
			}
			else if (expression is CalculateExpression)
			{
				CreateCalculateExpression(builder, expression as CalculateExpression);
			}
			else if (expression is BetweenExpression)
			{
				CreateBetweenExpression(builder, expression as BetweenExpression);
			}
			else if (expression is ConditionExpression)
			{
				CreateConditionExpression(builder, expression as ConditionExpression);
			}
		}

		/// <summary>
		/// 根据Lambda条件表达式创建数据库参数。
		/// </summary>
		/// <param name="expression"></param>
		private DbParameter CreateExpressionParamter(ConditionExpression expression)
		{
			string parameterName = CreateParameterName(expression.ParameterName);
			if (dataDbCommand.Parameters.Contains(parameterName)) { return null; }
			DbParameter parameter = CreateParameter();
			parameter.ParameterName = CreateParameterName(expression.ParameterName);
			parameter.SourceColumn = expression.ColumnName;
			parameter.Size = expression.Size;
			parameter.Direction = ParameterDirection.Input;
			parameter.IsNullable = expression.Nullable;
			ConvertParameterType(parameter, expression.DataType, expression.Precision, expression.Scale);
			dataDbCommand.Parameters.Add(parameter);
			parameter.Value = expression.Value;
			return parameter;
		}

		/// <summary>
		/// 根据 ConditionExpression 创建参数。
		/// </summary>
		/// <param name="builder">动态拼接 Transact-SQL 语句。</param>
		/// <param name="expression">条件表达式。</param>
		protected virtual void CreateConditionExpression(StringBuilder builder, ConditionExpression expression)
		{
			if (!string.IsNullOrEmpty(expression.TableAlias))
			{
				builder.Append(expression.TableAlias).Append('.');
			}

			builder.Append(expression.ColumnName);
			if (expression.ExpressionType == ExpressionTypeEnum.In)
			{
				builder.Append(" IN (");
				builder.Append(expression.Value).Append(')');
			}
			else if (expression.ExpressionType == ExpressionTypeEnum.NotIn)
			{
				builder.Append(" NOT IN (");
				builder.Append(expression.Value).Append(')');
			}
			else if (expression.ExpressionType == ExpressionTypeEnum.IsNull)
			{
				builder.Append(" IS NULL");
			}
			else
			{
				AppendExpressionType(builder, expression.ExpressionType);
				AppendExpressionTypeLeftBracket(builder, expression.ExpressionType);
				builder.Append(CreateParameterName(expression.ParameterName));
				AppendExpressionTypeRightBracket(builder, expression.ExpressionType);
				CreateExpressionParamter(expression);
			}
		}

		/// <summary>
		/// 根据 ConditionExpression 创建参数。
		/// </summary>
		/// <param name="builder">动态拼接 Transact-SQL 语句。</param>
		/// <param name="expression">条件表达式。</param>
		protected virtual void CreateCalculateExpression(StringBuilder builder, CalculateExpression expression)
		{
			builder.Append("((");
			if (!string.IsNullOrEmpty(expression.TableAlias))
				builder.Append(expression.TableAlias).Append('.');
			builder.Append(expression.ColumnName);
			AppendCalculateType(builder, expression.CalculateType);
			builder.Append(expression.Constant).Append(")");
			AppendExpressionType(builder, expression.ExpressionType);
			builder.Append(CreateParameterName(expression.ParameterName)).Append(")");

			CreateExpressionParamter(expression);
		}

		/// <summary>
		/// 根据 CalculateTypeEnum 拼接SQL计算表达式
		/// </summary>
		/// <param name="builder">动态拼接 Transact-SQL 语句。</param>
		/// <param name="calculateType">CalculateTypeEnum枚举项。</param>
		protected virtual void AppendCalculateType(StringBuilder builder, CalculateTypeEnum calculateType)
		{
			if (calculateType == CalculateTypeEnum.Add) { builder.Append(" + "); }
			else if (calculateType == CalculateTypeEnum.Subtract) { builder.Append(" - "); }
			else if (calculateType == CalculateTypeEnum.Multiply) { builder.Append(" * "); }
			else if (calculateType == CalculateTypeEnum.Divide) { builder.Append(" / "); }
			else if (calculateType == CalculateTypeEnum.Modulo) { builder.Append(" % "); }
			else if (calculateType == CalculateTypeEnum.And) { builder.Append(" &"); }
			else if (calculateType == CalculateTypeEnum.Or) { builder.Append(" | "); }
			else if (calculateType == CalculateTypeEnum.ExclusiveOr) { builder.Append(" ^ "); }
			else if (calculateType == CalculateTypeEnum.LeftShift) { builder.Append(" << "); }
			else if (calculateType == CalculateTypeEnum.RightShift) { builder.Append(" >> "); }
		}

		/// <summary>
		/// 根据 ConditionExpression 创建参数。
		/// </summary>
		/// <param name="builder">动态拼接 Transact-SQL 语句。</param>
		/// <param name="expression">条件表达式。</param>
		protected virtual void CreateBetweenExpression(StringBuilder builder, BetweenExpression expression)
		{
			builder.Append("(");
			if (!string.IsNullOrEmpty(expression.TableAlias))
				builder.Append(expression.TableAlias).Append(".");
			builder.Append(expression.ColumnName);
			AppendExpressionType(builder, expression.ExpressionType);
			builder.Append(CreateParameterName(expression.ParameterName)).Append(" AND ");
			builder.Append(CreateParameterName(expression.ToParameterName)).Append(")");

			DbParameter parameter = CreateExpressionParamter(expression);
			if (parameter != null) { parameter.ParameterName = CreateParameterName(expression.ToParameterName); }
			DbParameter toParameter = CreateExpressionParamter(expression);
			if (toParameter != null) { toParameter.Value = expression.ToValue; }
		}

		/// <summary>
		/// 根据 BinaryConditionExpression 创建参数。
		/// </summary>
		/// <param name="builder">动态拼接 Transact-SQL 语句。</param>
		/// <param name="expression">条件表达式。</param>
		protected virtual void CreateBinaryConditionExpression(StringBuilder builder, BinaryConditionExpression expression)
		{
			builder.Append("(");
			CreateLambdaConditionExpression(builder, expression.LeftExpression);
			if (expression.ExpressionCompare == ExpressionCompareEnum.AndAlso) { builder.Append(" AND "); }
			else if (expression.ExpressionCompare == ExpressionCompareEnum.OrElse) { builder.Append(" OR "); }
			CreateLambdaConditionExpression(builder, expression.RightExpression);
			builder.Append(")");
		}

		/// <summary>
		/// 根据 ExpressionTypeEnum 创建参数比较符号。
		/// </summary>
		/// <param name="builder">动态拼接 Transact-SQL 语句。</param>
		/// <param name="expressionType">条件表达式。</param>
		protected virtual void AppendExpressionType(System.Text.StringBuilder builder, ExpressionTypeEnum expressionType)
		{
			if (expressionType == ExpressionTypeEnum.Equal) { builder.Append(" = "); }
			else if (expressionType == ExpressionTypeEnum.NotEqual) { builder.Append(" <> "); }
			else if (expressionType == ExpressionTypeEnum.GreaterThan) { builder.Append(" > "); }
			else if (expressionType == ExpressionTypeEnum.GreaterThanEqual) { builder.Append(" >= "); }
			else if (expressionType == ExpressionTypeEnum.LessThan) { builder.Append(" < "); }
			else if (expressionType == ExpressionTypeEnum.LessThanEqual) { builder.Append(" <= "); }
			else if (expressionType == ExpressionTypeEnum.In) { builder.Append(" IN "); }
			else if (expressionType == ExpressionTypeEnum.NotIn) { builder.Append(" NOT IN "); }
			else if (expressionType == ExpressionTypeEnum.Like) { builder.Append(" LIKE "); }
			else if (expressionType == ExpressionTypeEnum.NotLike) { builder.Append(" NOT LIKE "); }
			else if (expressionType == ExpressionTypeEnum.Between) { builder.Append(" BETWEEN "); }
			else if (expressionType == ExpressionTypeEnum.IsNull) { builder.Append(" IS NULL "); }
		}

		/// <summary>
		/// 根据 ExpressionTypeEnum 创建参数比较左括号。
		/// </summary>
		/// <param name="builder">动态拼接 Transact-SQL 语句。</param>
		/// <param name="expressionType">条件表达式。</param>
		protected virtual void AppendExpressionTypeLeftBracket(System.Text.StringBuilder builder, ExpressionTypeEnum expressionType)
		{
			if (expressionType == ExpressionTypeEnum.In || expressionType == ExpressionTypeEnum.NotIn
				 || expressionType == ExpressionTypeEnum.Like || expressionType == ExpressionTypeEnum.NotLike)
			{
				builder.Append("(");
			}
		}

		/// <summary>
		/// 根据 ExpressionTypeEnum 创建参数比较左括号。
		/// </summary>
		/// <param name="builder">动态拼接 Transact-SQL 语句。</param>
		/// <param name="expressionType">条件表达式。</param>
		protected virtual void AppendExpressionTypeRightBracket(System.Text.StringBuilder builder, ExpressionTypeEnum expressionType)
		{
			if (expressionType == ExpressionTypeEnum.In ||
				expressionType == ExpressionTypeEnum.NotIn ||
				expressionType == ExpressionTypeEnum.Like ||
				expressionType == ExpressionTypeEnum.NotLike)
			{
				builder.Append(")");
			}
		}
		#endregion

		/// <summary>
		/// 初始化Transact-SQL命令执行参数值
		/// </summary>
		/// <param name="dynamicParameters">键/值对数组，包含了需要执行参数的值。</param>
		internal void InitDynamicParameters(params DynamicParameter[] dynamicParameters)
		{
			if (dynamicParameters != null && dynamicParameters.Length > 0)
			{
				StringBuilder builder = new StringBuilder(500);
				foreach (DynamicParameter param in dynamicParameters)
				{
					if (builder.Length > 0) { builder.Append(" ").Append(param.LogicalOperator.ToString()).Append(" "); }
					this.CreateDynamicParameter(builder, param);
					if (param.CompareOperator != ParameterCompare.IsNull && param.CompareOperator != ParameterCompare.IsNotNull)
					{
						DbParameter dbParam = this.CreateParameter(param.ParameterName, param.ParameterName, param.DataType, false);
						if (param.Value != null && param.Value != DBNull.Value) { dbParam.Value = param.Value; }
						base.Parameters.Add(dbParam);
					}
				}
				TempWhereText = builder.ToString();
			}
		}

		/// <summary>根据键按升序对序列的元素排序。</summary>
		/// <param name="fieldName">排序字段。</param>
		internal void OrderBy<T>(string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName) == false) { AddToOrderList(fieldName, true); }
		}

		/// <summary>
		/// 根据键按升序对序列的元素排序。
		/// </summary>
		/// <param name="fieldName">排序字段。</param>
		internal void OrderByDescending<T>(string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName) == false) { AddToOrderList(fieldName, false); }
		}

		/// <summary>根据键按降序对序列的元素排序。</summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		internal void OrderByDescending<T>(Expression<Func<T, object>> keySelector) where T : class
		{
			if (keySelector.Body is MemberExpression)
			{
				MemberExpression member = keySelector.Body as MemberExpression;
				string fieldName = DynamicCommand.GetMemberFieldInfo(member.Member);
				AddToOrderList(fieldName, false);
			}
		}

		/// <summary>
		/// 根据键按升序对序列的元素排序。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		internal void OrderBy<T>(Expression<Func<T, object>> keySelector) where T : class
		{
			if (keySelector.Body is MemberExpression)
			{
				MemberExpression member = keySelector.Body as MemberExpression;
				string fieldName = DynamicCommand.GetMemberFieldInfo(member.Member);
				AddToOrderList(fieldName, true);
			}
		}

		/// <summary>
		/// 获取实体属性的列信息
		/// </summary>
		/// <param name="mi">属性信息</param>
		/// <returns>返回字段名称，如果存在表别名，则同时包含表别名。</returns>
		private static string GetMemberFieldInfo(MemberInfo mi)
		{
			ColumnMappingAttribute cma = (ColumnMappingAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnMappingAttribute));
			if (cma != null)
			{
				if (string.IsNullOrEmpty(cma.TableAlias))
					return cma.ColumnName;
				return string.Concat(cma.TableAlias, ".", cma.ColumnName);
			}
			ColumnAttribute ca = (ColumnAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnAttribute));
			if (ca == null) { throw new AttributeException("ColumnAttribute_NotExists", mi.DeclaringType, mi.Name); }
			if (string.IsNullOrEmpty(ca.TableName))
				return ca.ColumnName;
			return string.Concat(ca.TableName, ".", ca.ColumnName);
		}

		/// <summary>
		/// 动态命令结构中静态参数列表
		/// </summary>
		protected internal readonly List<DbParameter> DbParameters = new List<DbParameter>(20);

		/// <summary>添加动态查询的参数</summary>
		/// <param name="parameters">需要添加的参数</param>
		public void AddParameters(params DbParameter[] parameters)
		{
			if (parameters == null || parameters.Length == 0) { return; }
			DbParameters.AddRange(parameters);
		}

		/// <summary>
		/// 将当前命令的参数复制到指定的集合中
		/// </summary>
		/// <param name="parameters"></param>
		protected internal abstract void CopyParametersTo(ICollection<DbParameter> parameters);

		#region GetDataTable<T>
		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="table">表示需要填充的 DataTable 类实例。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		public virtual QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, int pageSize, int pageIndex) where T : BaseTableRowType
		{
			InitializeParameters();
			return new QueryDataTable<T>(table, this, pageSize, pageIndex, 0);
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="table">表示需要填充的 DataTable 类实例。</param>
		/// <param name="entity">实体类实例，其中包含了命令所需的参数。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		public virtual QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, AbstractEntity entity, int pageSize, int pageIndex) where T : BaseTableRowType
		{
			InitializeParameters();
			if (entity != null) { ResetParameters(entity); }
			return new QueryDataTable<T>(table, this, pageSize, pageIndex, 0);
		}

		/// <summary>
		/// 从文本排序信息中接续排序字段
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="orderText"></param>
		private void OrderByText<T>(string orderText) where T : IEntityInfo
		{
			if (string.IsNullOrWhiteSpace(orderText) == true) { return; }
			string[] sortFields = orderText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (sortFields == null || sortFields.Length == 0) { return; }
			foreach (string field in sortFields)
			{
				string sortProperty = field.ToUpper();
				if (sortProperty.IndexOf(" DESC") >= 0)
				{
					string[] props = field.Split(new char[] { ' ' });
					EntityPropertyProvidor.TryGetProperty<T>(props[0], out EntityPropertyMeta propInfo);
					if (propInfo.Mapping == null) { AddToOrderList(field, false); }
					else
					{
						if (string.IsNullOrWhiteSpace(propInfo.Mapping.TableAlias) == false)
						{ AddToOrderList(string.Concat(propInfo.Mapping.TableAlias, ".", propInfo.Mapping.ColumnName), false); }
						else { AddToOrderList(propInfo.Mapping.ColumnName, false); }
					}
				}
				else if (sortProperty.IndexOf(" ASC") >= 0)
				{
					string[] props = field.Split(new char[] { ' ' });
					EntityPropertyProvidor.TryGetProperty<T>(props[0], out EntityPropertyMeta propInfo);
					if (propInfo.Mapping == null) { AddToOrderList(field, true); }
					else
					{
						if (string.IsNullOrWhiteSpace(propInfo.Mapping.TableAlias) == false)
						{ AddToOrderList(string.Concat(propInfo.Mapping.TableAlias, ".", propInfo.Mapping.ColumnName), true); }
						else { AddToOrderList(propInfo.Mapping.ColumnName, true); }
					}
				}
				else
				{
					EntityPropertyProvidor.TryGetProperty<T>(field.Trim(), out EntityPropertyMeta propInfo);
					if (propInfo.Mapping == null) { AddToOrderList(field.Trim(), true); }
					else
					{
						if (string.IsNullOrWhiteSpace(propInfo.Mapping.TableAlias) == false)
						{ AddToOrderList(string.Concat(propInfo.Mapping.TableAlias, ".", propInfo.Mapping.ColumnName), true); }
						else { AddToOrderList(propInfo.Mapping.ColumnName, true); }
					}
				}
			}
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="table">表示需要填充的 DataTable 类实例。</param>
		/// <param name="condition">查询记录使用的查询条件类实例</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		public virtual QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, AbstractCondition condition) where T : BaseTableRowType
		{
			InitializeParameters();
			if (condition != null)
			{
				ResetParameters(condition);
				OrderByText<T>(condition.OrderText);
				//if (string.IsNullOrEmpty(condition.SortField) == false && condition.SortOrder == SortDirection.Ascending)
				//	OrderBy(condition.SortField);
				//else if (string.IsNullOrEmpty(condition.SortField) == false && condition.SortOrder == SortDirection.Descending)
				//	OrderByDescending(condition.SortField);
			}
			return new QueryDataTable<T>(table, this, condition.PageSize, condition.PageIndex, condition.TotalCount);
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="table">表示需要填充的 DataTable 类实例。</param>
		/// <param name="anonObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		public virtual QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, object anonObject) where T : BaseTableRowType
		{
			int pageSize = 0, pageIndex = 0, totalCount = 0;
			Type anonType = anonObject.GetType();
			PropertyInfo sizeInfo = anonType.GetProperty("PageSize");
			if (sizeInfo != null) { pageSize = Convert.ToInt32(sizeInfo.GetValue(anonObject, null)); }

			PropertyInfo indexInfo = anonType.GetProperty("PageIndex");
			if (indexInfo != null) { pageIndex = Convert.ToInt32(indexInfo.GetValue(anonObject, null)); }

			PropertyInfo totalInfo = anonType.GetProperty("TotalCount");
			if (totalInfo != null) { totalCount = Convert.ToInt32(totalInfo.GetValue(anonObject, null)); }

			InitializeParameters();
			if (anonObject != null) { ResetParameters(anonObject); }
			return new QueryDataTable<T>(table, this, pageSize, pageIndex, totalCount);
		}
		#endregion

		#region GetEntities<T>
		/// <summary>填充数据集</summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="entity">实体类实例，其中包含了命令所需的参数。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		public virtual QueryEntities<T> GetEntities<T>(AbstractEntity entity, int pageSize, int pageIndex) where T : AbstractEntity, new()
		{
			InitializeParameters();
			if (entity != null) { ResetParameters(entity); }
			return new QueryEntities<T>(this, pageSize, pageIndex, 0);
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="condition">查询记录使用的查询条件类实例</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		public virtual QueryEntities<T> GetEntities<T>(AbstractCondition condition) where T : AbstractEntity, new()
		{
			InitializeParameters();
			if (condition != null)
			{
				ResetParameters(condition);
				OrderByText<T>(condition.OrderText);
			}
			return new QueryEntities<T>(this, condition.PageSize, condition.PageIndex, condition.TotalCount);
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="anonObject">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		public virtual QueryEntities<T> GetEntities<T>(object anonObject) where T : AbstractEntity, new()
		{
			int pageSize = 0, pageIndex = 0, totalCount = 0;
			Type anonType = anonObject.GetType();
			PropertyInfo sizeInfo = anonType.GetProperty("PageSize");
			if (sizeInfo != null) { pageSize = Convert.ToInt32(sizeInfo.GetValue(anonObject, null)); }

			PropertyInfo indexInfo = anonType.GetProperty("PageIndex");
			if (indexInfo != null) { pageIndex = Convert.ToInt32(indexInfo.GetValue(anonObject, null)); }

			PropertyInfo totalInfo = anonType.GetProperty("TotalCount");
			if (totalInfo != null) { totalCount = Convert.ToInt32(totalInfo.GetValue(anonObject, null)); }

			InitializeParameters();
			if (anonObject != null) { ResetParameters(anonObject); }
			return new QueryEntities<T>(this, pageSize, pageIndex, totalCount);
		}
		#endregion

		#region GetJoinEntities<T>

		private void GetJoinCommand<T>() where T : AbstractEntity, new()
		{
			EntityPropertyProvidor.TryGetProperties(typeof(T), out EntityPropertyCollection properties);
			foreach (EntityPropertyMeta meta in properties)
			{
				if (meta.JoinField != null) { }
			}
		}

		/// <summary>填充数据集</summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="entity">实体类实例，其中包含了命令所需的参数。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		public virtual QueryEntities<T> GetJoinEntities<T>(AbstractEntity entity, int pageSize, int pageIndex) where T : AbstractEntity, new()
		{
			InitializeParameters();
			if (entity != null) { ResetParameters(entity); }
			return new QueryEntities<T>(this, pageSize, pageIndex, 0);
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="condition">查询记录使用的查询条件类实例</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		public virtual QueryEntities<T> GetJoinEntities<T>(AbstractCondition condition) where T : AbstractEntity, new()
		{
			InitializeParameters();
			if (condition != null)
			{
				ResetParameters(condition);
				OrderByText<T>(condition.OrderText);
			}
			return new QueryEntities<T>(this, condition.PageSize, condition.PageIndex, condition.TotalCount);
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="anonObject">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		public virtual QueryEntities<T> GetJoinEntities<T>(object anonObject) where T : AbstractEntity, new()
		{
			int pageSize = 0, pageIndex = 0, totalCount = 0;
			Type anonType = anonObject.GetType();
			PropertyInfo sizeInfo = anonType.GetProperty("PageSize");
			if (sizeInfo != null) { pageSize = Convert.ToInt32(sizeInfo.GetValue(anonObject, null)); }

			PropertyInfo indexInfo = anonType.GetProperty("PageIndex");
			if (indexInfo != null) { pageIndex = Convert.ToInt32(indexInfo.GetValue(anonObject, null)); }

			PropertyInfo totalInfo = anonType.GetProperty("TotalCount");
			if (totalInfo != null) { totalCount = Convert.ToInt32(totalInfo.GetValue(anonObject, null)); }

			InitializeParameters();
			if (anonObject != null) { ResetParameters(anonObject); }
			return new QueryEntities<T>(this, pageSize, pageIndex, totalCount);
		}
		#endregion

		/// <summary>
		/// 初始化查询固定参数。
		/// </summary>
		protected internal abstract void InitializeParameters();

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		protected internal override void ReadXml(System.Xml.XmlReader reader)
		{
			base.ReadXml(reader);
			if (Parameters.Count > 0)
			{
				DbParameters.AddRange(Parameters.Cast<DbParameter>());
				//DbParameter[] parameters = new DbParameter[Parameters.Count];
				//Parameters.CopyTo(parameters, 0);
				//DbParameters.AddRange(parameters);
			}
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected internal override bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == WithClauseCollection.XmlElementName)
			{
				_WithClauses.ReadXml(reader.ReadSubtree());
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == SelectTextElement)
			{
				SelectText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == FromTextElement)
			{
				FromText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == WhereTextElement)
			{
				WhereText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == GroupTextElement)
			{
				GroupText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == HavingTextElement)
			{
				HavingText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == OrderTextElement)
			{
				OrderText = reader.ReadString();
			}
			else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == XmlElementName)
			{
				return true;
			}
			return base.ReadContent(reader);
		}

		/// <summary>
		/// 初始化动态命令
		/// </summary>
		public DynamicCommand InitializeCommand()
		{
			joinCommand = null; _dynamicJoinCommand = null;
			TempWhereText = string.Empty;
			mOrderList.Clear();
			return this;
		}

		/// <summary>创建没有分页或获取第一页的 Transact-SQL 语句</summary>
		/// <param name="pageSize">分页时每页显示的记录数量。</param>
		/// <param name="builder">生成的 Transact-SQL语句结果。</param>
		protected internal virtual void InitializeNoPaginationText(StringBuilder builder, int pageSize)
		{
			if (_WithClauses.Count > 0)
			{
				builder.Append("WITH "); List<string> withList = new List<string>();
				foreach (WithClause with in _WithClauses) { withList.Add(with.ToSql()); }
				builder.Append(string.Join("," + Environment.NewLine, withList.ToArray())).AppendLine();
			}
			builder.Append(" SELECT ");
			if (DistinctStatus == true) { builder.Append("DISTINCT "); }
			if (pageSize > 0) { builder.Append("TOP ").Append(pageSize).Append(" "); }
			builder.AppendFormat("COUNT(1) OVER() AS {0}", ReturnCountName);

			int builderLength = builder.Length;
			if (!string.IsNullOrEmpty(SelectText)) { builder.Append(",").Append(SelectText); }
			if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.SelectText))
			{
				if (builderLength < builder.Length) { builder.Append(',').Append(_dynamicJoinCommand.SelectText); }
				else { builder.Append(',').Append(_dynamicJoinCommand.SelectText); }
			}
			else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.SelectText))
			{
				if (builderLength < builder.Length) { builder.Append(',').Append(joinCommand.SelectText); }
				else { builder.Append(',').Append(joinCommand.SelectText); }
			}
			builderLength = builder.Length;
			if (!string.IsNullOrEmpty(FromText)) { builder.AppendLine().Append(" FROM ").Append(FromText); }
			if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.FromText))
			{
				if (builderLength < builder.Length) { builder.AppendLine().Append(_dynamicJoinCommand.FromText); }
				else { builder.AppendLine().Append(" FROM ").Append(_dynamicJoinCommand.FromText); }
			}
			else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.FromText))
			{
				if (builderLength < builder.Length) { builder.AppendLine().Append(joinCommand.FromText); }
				else { builder.AppendLine().Append(" FROM ").Append(joinCommand.FromText); }
			}
			builderLength = builder.Length;
			if (!string.IsNullOrEmpty(WhereText)) { builder.AppendLine().Append(" WHERE ").Append(WhereText); }
			if (!string.IsNullOrEmpty(TempWhereText))
			{
				if (builderLength != builder.Length) { builder.Append(" AND ").Append(TempWhereText); }
				else { builder.AppendLine().Append(" WHERE ").Append(TempWhereText); }
			}
			if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.WhereText))
			{
				if (builderLength != builder.Length) { builder.Append(" AND ").Append(_dynamicJoinCommand.WhereText); }
				else { builder.AppendLine().Append(" WHERE ").Append(_dynamicJoinCommand.WhereText); }
			}
			else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.WhereText))
			{
				if (builderLength != builder.Length) { builder.Append(" AND ").Append(joinCommand.WhereText); }
				else { builder.AppendLine().Append(" WHERE ").Append(joinCommand.WhereText); }
			}
			builderLength = builder.Length;
			if (!string.IsNullOrEmpty(GroupText)) { builder.Append(" GROUP BY ").Append(GroupText); }
			if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.GroupText))
			{
				if (builderLength < builder.Length) { builder.Append(',').Append(_dynamicJoinCommand.GroupText); }
				else { builder.Append(',').Append(_dynamicJoinCommand.GroupText); }
			}
			else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.GroupText))
			{
				if (builderLength != builder.Length) { builder.Append(',').Append(joinCommand.GroupText); }
				else { builder.AppendLine().Append(" GROUP BY ").Append(joinCommand.GroupText); }
			}
			if (string.IsNullOrWhiteSpace(HavingText) == false) { builder.AppendLine().Append(" HAVING ").Append(HavingText); }
			List<string> orderList = new List<string>(mOrderList.Count + 5);
			if (mOrderList.Count > 0) { orderList.AddRange(mOrderList.ToArray()); }
			else
			{
				if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.OrderText))
				{
					orderList.Add(_dynamicJoinCommand.OrderText);
				}
				else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.OrderText))
				{
					orderList.Add(joinCommand.OrderText);
				}
				if (!string.IsNullOrEmpty(OrderText)) { orderList.Add(OrderText); }
			}

			if (orderList.Count > 0) { builder.Append(" ORDER BY ").Append(string.Join(",", orderList)); }
		}

		/// <summary>动态拼接 Transact-SQL 语句</summary>
		/// <param name="sorting">是否需要排序</param>
		protected internal virtual void InitializeCommandText(bool sorting)
		{
			StringBuilder builder = new StringBuilder(2000);

			if (_WithClauses.Count > 0)
			{
				builder.Append("WITH "); List<string> withList = new List<string>();
				foreach (WithClause with in _WithClauses) { withList.Add(with.ToSql()); }
				builder.Append(string.Join(',' + Environment.NewLine, withList.ToArray())).AppendLine();
			}
			builder.Append("SELECT ");
			if (DistinctStatus == true) { builder.Append("DISTINCT "); }
			if (!string.IsNullOrEmpty(SelectText)) { builder.Append(SelectText); }
			if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.SelectText))
			{
				builder.Append(',').Append(_dynamicJoinCommand.SelectText);
			}
			else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.SelectText))
			{ builder.Append(',').Append(joinCommand.SelectText); }

			int builderLength = builder.Length;

			if (!string.IsNullOrEmpty(FromText)) { builder.Append(" FROM ").Append(FromText); }
			if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.FromText))
			{
				if (builderLength != builder.Length) { builder.Append(" ").Append(_dynamicJoinCommand.FromText); }
				else { builder.Append(" FROM ").Append(_dynamicJoinCommand.FromText); }
			}
			else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.FromText))
			{
				if (builderLength != builder.Length) { builder.Append(" ").Append(joinCommand.FromText); }
				else { builder.Append(" FROM ").Append(joinCommand.FromText); }
			}

			List<string> whereList = new List<string>(3);
			if (string.IsNullOrEmpty(WhereText) == false) { whereList.Add(WhereText); }
			if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.WhereText))
			{
				whereList.Add(_dynamicJoinCommand.WhereText);
			}
			else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.WhereText))
			{
				whereList.Add(joinCommand.WhereText);
			}
			if (!string.IsNullOrEmpty(TempWhereText)) { whereList.Add(TempWhereText); }

			if (whereList.Count > 0) { builder.Append(" WHERE ").Append(string.Join(" AND ", whereList.ToArray())); }

			builderLength = builder.Length;
			if (!string.IsNullOrEmpty(GroupText)) { builder.Append(" GROUP BY ").Append(GroupText); }
			if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.WhereText))
			{
				if (builderLength != builder.Length) { builder.Append(',').Append(_dynamicJoinCommand.GroupText); }
				else { builder.AppendLine().Append(" GROUP BY ").Append(_dynamicJoinCommand.GroupText); }
			}
			else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.GroupText))
			{
				if (builderLength != builder.Length) { builder.Append(',').Append(joinCommand.GroupText); }
				else { builder.AppendLine().Append(" GROUP BY ").Append(joinCommand.GroupText); }
			}
			if (string.IsNullOrWhiteSpace(HavingText) == false) { builder.AppendLine().Append(" HAVING ").Append(HavingText); }

			List<string> orderList = new List<string>(3);
			string orderBy = null;

			if (mOrderList.Count > 0) { orderList.AddRange(mOrderList.ToArray()); }
			else
			{
				if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.WhereText))
				{
					orderList.Add(_dynamicJoinCommand.OrderText);
				}
				else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.OrderText))
					orderList.Add(joinCommand.OrderText);
				if (!string.IsNullOrEmpty(OrderText)) { orderList.Add(OrderText); }
			}
			if (orderList.Count > 0) { orderBy = string.Concat(" ORDER BY ", string.Join(",", orderList.ToArray())); }
			if (sorting) { builder.AppendFormat(" {0}", orderBy); }
			dataDbCommand.CommandText = builder.ToString();
		}

		/// <summary>
		/// 动态拼接 Transact-SQL 语句
		/// </summary>
		/// <param name="pageSize">分页时每页显示的记录数量。</param>
		/// <param name="pageIndex">分页时，当前页索引。</param>
		protected internal virtual void InitializeCommandText(int pageSize, int pageIndex)
		{
			StringBuilder builder = new StringBuilder(2000);
			if (pageSize == 0 && pageIndex == 0) { InitializeNoPaginationText(builder, 0); }
			else if (pageSize > 0 && pageIndex == 1) { InitializeNoPaginationText(builder, pageSize); }
			else if (pageSize > 0 && pageIndex >= 2)
			{
				List<string> orderList = new List<string>(3);
				string orderBy = null;

				if (mOrderList.Count > 0) { orderList.AddRange(mOrderList.ToArray()); }
				else
				{
					if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.OrderText))
					{
						orderList.Add(_dynamicJoinCommand.OrderText);
					}
					else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.OrderText))
					{ orderList.Add(joinCommand.OrderText); }
					if (!string.IsNullOrEmpty(OrderText)) { orderList.Add(OrderText); }
				}
				if (orderList.Count > 0) { orderBy = string.Concat(" ORDER BY ", string.Join(",", orderList.ToArray())); }
				else
				{
					string errorMsg = string.Format(Strings.Access_NotExist_OrderBy, CommandName);
					throw new ArgumentException(errorMsg);  //'{0}' 命令中 缺少 Order By子句，如果需要分页则必须需要此子句。
				}

				int nextPageRowNumber = pageSize * (pageIndex - 1) + 1;
				int nextPageMaxNumber = pageSize * pageIndex;
				if (_WithClauses.Count > 0)
				{
					builder.Append("WITH "); List<string> withList = new List<string>();
					foreach (WithClause with in _WithClauses) { withList.Add(with.ToSql()); }
					builder.Append(string.Join(',' + Environment.NewLine, withList.ToArray())).AppendLine();
				}
				builder.AppendFormat("SELECT TOP {0} * ", pageSize);
				builder.AppendLine();
				if (DistinctStatus == true) { builder.AppendFormat(" FROM (SELECT DISTINCT TOP {0} ", nextPageMaxNumber); }
				else { builder.AppendFormat(" FROM (SELECT TOP {0} ", nextPageMaxNumber); }
				builder.AppendFormat("ROW_NUMBER() OVER({0}) AS PAGEROWNUMBER", orderBy);
				builder.AppendFormat(",COUNT(1) OVER() AS {0}", AbstractDataCommand.ReturnCountName);

				if (!string.IsNullOrEmpty(SelectText)) { builder.Append(',').Append(SelectText); }
				if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.SelectText))
				{
					builder.Append(',').Append(_dynamicJoinCommand.SelectText);
				}
				else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.SelectText))
					builder.Append(',').Append(joinCommand.SelectText);

				int builderLength = builder.Length;

				if (!string.IsNullOrEmpty(FromText)) { builder.Append(" FROM ").Append(FromText); }
				if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.FromText))
				{
					if (builderLength != builder.Length) { builder.Append(" ").Append(_dynamicJoinCommand.FromText); }
					else { builder.Append(" FROM ").Append(_dynamicJoinCommand.FromText); }
				}
				else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.FromText))
				{
					if (builderLength != builder.Length) { builder.Append(" ").Append(joinCommand.FromText); }
					else { builder.Append(" FROM ").Append(joinCommand.FromText); }
				}

				List<string> whereList = new List<string>(3);
				if (string.IsNullOrEmpty(WhereText) == false) { whereList.Add(WhereText); }
				if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.WhereText))
				{
					whereList.Add(_dynamicJoinCommand.WhereText);
				}
				else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.WhereText))
					whereList.Add(joinCommand.WhereText);
				if (!string.IsNullOrEmpty(TempWhereText)) { whereList.Add(TempWhereText); }

				if (whereList.Count > 0) { builder.Append(" WHERE ").Append(string.Join(" AND ", whereList)); }

				builderLength = builder.Length;
				if (!string.IsNullOrEmpty(GroupText)) { builder.AppendLine().Append(" GROUP BY ").Append(GroupText); }
				if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.GroupText))
				{
					if (builderLength != builder.Length) { builder.Append(',').Append(_dynamicJoinCommand.GroupText); }
					else { builder.AppendLine().Append(" GROUP BY ").Append(_dynamicJoinCommand.GroupText); }
				}
				else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.GroupText))
				{
					if (builderLength != builder.Length) { builder.Append(',').Append(joinCommand.GroupText); }
					else { builder.AppendLine().Append(" GROUP BY ").Append(joinCommand.GroupText); }
				}
				if (string.IsNullOrWhiteSpace(HavingText) == false) { builder.AppendLine().Append(" HAVING ").Append(HavingText); }
				builder.AppendFormat(" {0}", orderBy);
				builder.AppendFormat(") T1 WHERE T1.PAGEROWNUMBER>={0}", nextPageRowNumber);
			}
			dataDbCommand.CommandText = builder.ToString();
		}

		/// <summary>// 初始化计算记录数 的 Transact-SQL 语句。</summary>
		protected internal virtual void InitCountText()
		{
			StringBuilder builder = new StringBuilder(1000);
			if (_WithClauses.Count > 0)
			{
				builder.Append("WITH "); List<string> withList = new List<string>();
				foreach (WithClause with in _WithClauses) { withList.Add(with.ToSql()); }
				builder.Append(string.Join("," + Environment.NewLine, withList.ToArray())).AppendLine();
			}
			builder.Append("SELECT COUNT(1)");
			List<string> fromList = new List<string>(3);
			int builderLength = builder.Length;
			if (!string.IsNullOrEmpty(FromText)) { builder.Append(" FROM ").Append(FromText); }

			if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.FromText))
			{
				if (builderLength != builder.Length) { builder.Append(" ").Append(_dynamicJoinCommand.FromText); }
				else { builder.Append(" FROM ").Append(_dynamicJoinCommand.FromText); }
			}
			else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.FromText))
			{
				if (builderLength != builder.Length) { builder.Append(" ").Append(joinCommand.FromText); }
				else { builder.Append(" FROM ").Append(joinCommand.FromText); }
			}

			List<string> whereList = new List<string>(3);
			if (!string.IsNullOrEmpty(WhereText)) { whereList.Add(WhereText); }

			if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.WhereText))
				whereList.Add(_dynamicJoinCommand.WhereText);
			else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.WhereText))
				whereList.Add(joinCommand.WhereText);
			if (!string.IsNullOrEmpty(TempWhereText))
			{
				whereList.Add(TempWhereText);
			}

			if (whereList.Count > 0) { builder.Append(" WHERE ").Append(string.Join(" AND ", whereList.ToArray())); }

			if (!string.IsNullOrWhiteSpace(GroupText))
			{
				builder.AppendLine().Append(" GROUP BY ").Append(GroupText);
			}
			if (string.IsNullOrWhiteSpace(HavingText) == false) { builder.AppendLine().Append(" HAVING ").Append(HavingText); }
			dataDbCommand.CommandText = builder.ToString();
		}

		/// <summary>将本对象属性复制到目标对象中</summary>
		protected internal void CopyTo(DynamicCommand command)
		{
			if (_WithClauses != null && _WithClauses.Count > 0)
			{
				_WithClauses.CloneCopy(command.WithClauses);
			}
			command.SourceColumn = SourceColumn;
			command.CommandName = CommandName;
			command.SelectText = SelectText;
			command.FromText = FromText;
			command.WhereText = WhereText;
			command.GroupText = GroupText;
			command.HavingText = HavingText;
			command.OrderText = OrderText;
		}
		#region 克隆 DynamicCommand 命令
		/// <summary>克隆当前动态查询命令</summary>
		/// <param name="selectText">要对数据源执行的 Transact-SQL 语句中 SELECT 数据库字段部分。</param>
		/// <param name="fromText">要对数据源执行的 Transact-SQL 语句中 FROM 数据库表部分。</param>
		/// <returns>返回创建成功的动态命令 DynamicCommand 子类实例(特定于某种数据库命令的实例)。</returns>
		public virtual DynamicCommand Clone(string selectText, string fromText)
		{
			DynamicCommand dynamicCommand = this.CloneCommand() as DynamicCommand;
			CopyTo(dynamicCommand);
			if (string.IsNullOrWhiteSpace(selectText) == false) { dynamicCommand.SelectText = selectText; }
			if (string.IsNullOrWhiteSpace(fromText) == false) { dynamicCommand.FromText = fromText; }
			return dynamicCommand;
		}

		/// <summary>克隆当前动态查询命令</summary>
		/// <param name="selectText">要对数据源执行的 Transact-SQL 语句中 SELECT 数据库字段部分。</param>
		/// <param name="fromText">要对数据源执行的 Transact-SQL 语句中 FROM 数据库表部分。</param>
		/// <param name="whereText">要对数据源执行的 Transact-SQL 语句中 WHERE 条件部分。</param>
		/// <returns>返回创建成功的动态命令 DynamicCommand 子类实例(特定于某种数据库命令的实例)。</returns>
		public virtual DynamicCommand Clone(string selectText, string fromText, string whereText)
		{
			DynamicCommand dynamicCommand = Clone(selectText, fromText);
			if (string.IsNullOrWhiteSpace(whereText) == false) { dynamicCommand.WhereText = whereText; }
			return dynamicCommand;
		}

		/// <summary>克隆当前动态查询命令</summary>
		/// <param name="selectText">要对数据源执行的 Transact-SQL 语句中 SELECT 数据库字段部分。</param>
		/// <param name="fromText">要对数据源执行的 Transact-SQL 语句中 FROM 数据库表部分。</param>
		/// <param name="whereText">要对数据源执行的 Transact-SQL 语句中 WHERE 条件部分。</param>
		/// <param name="orderText">要对数据源执行的 Transact-SQL 语句中 ORDER BY 条件部分。</param>
		/// <returns>返回创建成功的动态命令 DynamicCommand 子类实例(特定于某种数据库命令的实例)。</returns>
		public virtual DynamicCommand Clone(string selectText, string fromText, string whereText, string orderText)
		{
			DynamicCommand dynamicCommand = Clone(selectText, fromText);
			if (string.IsNullOrWhiteSpace(whereText) == false) { dynamicCommand.WhereText = whereText; }
			if (string.IsNullOrWhiteSpace(orderText) == false) { dynamicCommand.OrderText = orderText; }
			return dynamicCommand;
		}

		/// <summary>克隆当前动态查询命令</summary>
		/// <param name="selectText">要对数据源执行的 Transact-SQL 语句中 SELECT 数据库字段部分。</param>
		/// <param name="fromText">要对数据源执行的 Transact-SQL 语句中 FROM 数据库表部分。</param>
		/// <param name="whereText">要对数据源执行的 Transact-SQL 语句中 WHERE 条件部分。</param>
		/// <param name="orderText">要对数据源执行的 Transact-SQL 语句中 ORDER BY 条件部分。</param>
		/// <param name="groupText">要对数据源执行的 Transact-SQL 语句中 GROUP 部分</param>
		/// <param name="havingText">要对数据源执行的 Transact-SQL 语句中 HANVING 条件部分</param>
		/// <returns>返回创建成功的动态命令 DynamicCommand 子类实例(特定于某种数据库命令的实例)。</returns>
		public virtual DynamicCommand Clone(string selectText, string fromText, string whereText, string orderText, string groupText, string havingText)
		{
			DynamicCommand dynamicCommand = Clone(selectText, fromText);
			if (string.IsNullOrWhiteSpace(whereText) == false) { dynamicCommand.WhereText = whereText; }
			if (string.IsNullOrWhiteSpace(orderText) == false) { dynamicCommand.OrderText = orderText; }
			if (string.IsNullOrWhiteSpace(groupText) == false) { dynamicCommand.GroupText = groupText; }
			if (string.IsNullOrWhiteSpace(havingText) == false) { dynamicCommand.HavingText = havingText; }
			return dynamicCommand;
		}
		#endregion

		private readonly BD.WithClauseCollection _WithClauses;
		/// <summary>获取或设置要对数据源执行的 Transact-SQL 语句中 WITH 子句部分。</summary>
		/// <value>要执行的 Transact-SQL 语句的 WITH 子句部分。</value>
		public virtual BD.WithClauseCollection WithClauses { get { return _WithClauses; } }

		/// <summary>获取或设置要对数据源执行的 Transact-SQL 语句中 SELECT 数据库字段部分。</summary>
		/// <value>要执行的 Transact-SQL 语句的 SELECT 部分，默认值为空字符串。</value>
		public virtual string SelectText { get; protected internal set; }

		/// <summary>获取或设置要对数据源执行的 Transact-SQL 语句中 FROM 数据库表部分。</summary>
		/// <value>要执行的 Transact-SQL 语句的 FROM 部分，默认值为空字符串。</value>
		public virtual string FromText { get; protected internal set; }

		/// <summary>获取或设置要对数据源执行的 Transact-SQL 语句中 WHERE 条件部分。</summary>
		/// <value>要执行的 Transact-SQL 语句的 WHERE 部分，默认值为空字符串。</value>
		public virtual string WhereText { get; protected internal set; }

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 GROUP 部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 GROUP 部分，默认值为空字符串。</value>
		public virtual string GroupText { get; protected internal set; }

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 HANVING 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 HANVING 部分，默认值为空字符串。</value>
		public virtual string HavingText { get; protected internal set; }

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 ORDER BY 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 ORDER BY 部分，默认值为空字符串。</value>
		public virtual string OrderText { get; protected internal set; }

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中动态添加 WHERE 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的动态添加的 WHERE 部分，默认值为空字符串。</value>
		internal string TempWhereText { get; set; }

		/// <summary>表示排序字段列表</summary>
		protected sealed class OrderFieldList : IEnumerable<OrderFieldInfo>
		{
			private readonly List<OrderFieldInfo> mOrderFields;
			/// <summary>初始化 OrderFieldList 类实例。</summary>
			/// <param name="capacity">新列表最初可以存储的元素数。</param>
			public OrderFieldList(int capacity) { mOrderFields = new List<OrderFieldInfo>(capacity); }

			/// <summary></summary>
			/// <param name="fieldName"></param>
			/// <returns></returns>
			public bool Contains(string fieldName)
			{
				return mOrderFields.Exists(m => m.FieldName == fieldName);
			}

			/// <summary>向集合末尾添加 从小到大排序字段。</summary>
			/// <param name="field">排序字段名</param>
			public void Add(string field)
			{
				if (mOrderFields.Exists(m => m.FieldName == field)) { return; }
				mOrderFields.Add(new OrderFieldInfo(field, true));
			}

			/// <summary>向集合末尾添加 排序字段。</summary>
			/// <param name="field">排序字段名</param>
			/// <param name="isAscending">是否从小到大排序（true表示Ascending，false表示Descending）</param>
			public void Add(string field, bool isAscending)
			{
				if (mOrderFields.Exists(m => m.FieldName == field)) { return; }
				mOrderFields.Add(new OrderFieldInfo(field, isAscending));
			}

			/// <summary>向集合末尾添加 排序字段。</summary>
			/// <param name="field">排序字段名</param>
			/// <param name="pSortDirection">排序方向</param>
			public void Add(string field, SortDirection pSortDirection)
			{
				if (mOrderFields.Exists(m => m.FieldName == field)) { return; }
				mOrderFields.Add(new OrderFieldInfo(field, pSortDirection));
			}

			/// <summary>清空集合中所有元素</summary>
			public int Count { get { return mOrderFields.Count; } }

			/// <summary>清空集合中所有元素</summary>
			public void Clear() { mOrderFields.Clear(); }

			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			public string[] ToArray()
			{
				List<string> results = new List<string>(mOrderFields.Count);
				mOrderFields.ForEach(m => results.Add(m));
				return results.ToArray();
			}

			/// <summary></summary>
			/// <returns></returns>
			IEnumerator IEnumerable.GetEnumerator() { return mOrderFields.GetEnumerator(); }

			/// <summary></summary>
			/// <returns></returns>
			IEnumerator<OrderFieldInfo> IEnumerable<OrderFieldInfo>.GetEnumerator() { return mOrderFields.GetEnumerator(); }
		}


		/// <summary>表示排序字段类型</summary>
		private struct OrderFieldInfo
		{
			/// <summary>初始化 OrderFieldInfo 实例</summary>
			/// <param name="field">排序字段名</param>
			public OrderFieldInfo(string field) : this(field, true) { }

			/// <summary>初始化 OrderField 实例</summary>
			/// <param name="field">排序字段名</param>
			/// <param name="isAscending">是否从小到大排序（true表示Ascending，false表示Descending）</param>
			public OrderFieldInfo(string field, bool isAscending)
			{
				if (string.IsNullOrEmpty(field) == true) { throw new ArgumentNullException(nameof(field)); }
				FieldName = field;
				SortDirection = isAscending ? SortDirection.Ascending : SortDirection.Descending;
			}

			/// <summary>初始化 OrderField 实例</summary>
			/// <param name="field">排序字段名</param>
			/// <param name="pSortDirection">排序方向</param>
			public OrderFieldInfo(string field, SortDirection pSortDirection)
			{
				if (string.IsNullOrEmpty(field) == true) { throw new ArgumentNullException(nameof(field)); }
				FieldName = field;
				SortDirection = pSortDirection;
			}

			/// <summary>排序字段名（包含表别名）</summary>
			public string FieldName { get; private set; }
			/// <summary>排序方向（默认 Ascending ）</summary>
			public SortDirection SortDirection { get; private set; }

			/// <summary></summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Concat(FieldName, SortDirection == SortDirection.Ascending ? " ASC" : " DESC");
			}

			/// <summary></summary>
			/// <param name="d"></param>
			public static implicit operator string(OrderFieldInfo d)
			{
				return d.ToString();
			}

			/// <summary></summary>
			/// <param name="value"></param>
			public static explicit operator OrderFieldInfo(string value)
			{
				return new OrderFieldInfo(value);
			}



		}

	}

}
