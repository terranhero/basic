using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Basic.Collections;
using Basic.EntityLayer;
using Basic.Interfaces;
using Basic.Tables;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示要对数据源执行的 SQL 语句或存储过程。为表示命令的、数据库特有的类提供一个基类。
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.ComponentModel.ToolboxItem(false)]
	public abstract class StaticCommand : DataCommand, IDbCommand, IXmlSerializable
	{
		#region Xml 节点名称常量
		/// <summary>
		/// 表示Xml元素名称
		/// </summary>
		protected internal const string XmlElementName = "StaticCommand";
		/// <summary>
		/// 表示 CommandText 元素。
		/// </summary>
		protected internal const string CommandTextElement = "CommandText";
		/// <summary>
		/// 
		/// </summary>
		protected internal const string NewValuesElement = "NewValues";
		#endregion

		/// <summary>
		/// 初始化 StaticCommand 类的新实例。 
		/// </summary>
		/// <param name="dbCommand"></param>
		protected StaticCommand(DbCommand dbCommand) : base(dbCommand) { }

		/// <summary>
		/// SQL 语句或存储过程的参数。
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DbParameterCollection Parameters { get { return dataDbCommand.Parameters; } }

		/// <summary>获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。</summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		public virtual string CommandText
		{
			get
			{
				return dataDbCommand.CommandText;
			}
			set
			{
				if (dataDbCommand.CommandText != value)
				{
					dataDbCommand.CommandText = value;
					OnPropertyChanged("CommandText");
				}
			}
		}

		/// <summary>
		/// 检测命令集合
		/// </summary>
		internal protected abstract CheckCommandCollection CheckCommands { get; }

		/// <summary>
		/// 新值命令集合
		/// </summary>
		internal protected abstract NewValueCommandCollection NewValues { get; }

		/// <summary>
		/// 初始化Transact-SQL命令执行参数值
		/// </summary>
		/// <param name="entities">数据实体类数组。</param>
		internal protected virtual bool ResetParameters(AbstractEntity[] entities) { return false; }

		/// <summary>
		/// 重置数据库连接
		/// </summary>
		/// <param name="connection">一个 DbConnection，它表示到关系数据库实例的连接。 </param>
		internal protected override void ResetConnection(DbConnection connection)
		{
			base.ResetConnection(connection);
			if (CheckCommands != null && CheckCommands.Count > 0)
			{
				foreach (CheckCommand checkCommand in CheckCommands)
				{
					checkCommand.ResetConnection(connection);
				}
			}
			if (NewValues != null && NewValues.Count > 0)
			{
				foreach (NewValueCommand newValueCommand in NewValues)
				{
					newValueCommand.ResetConnection(connection);
				}
			}
		}

		/// <summary>
		/// 释放数据库连接
		/// </summary>
		internal protected override void ReleaseConnection()
		{
			base.ReleaseConnection();
			if (CheckCommands != null && CheckCommands.Count > 0)
			{
				foreach (CheckCommand checkCommand in CheckCommands)
				{
					checkCommand.ReleaseConnection();
				}
			}
			if (NewValues != null && NewValues.Count > 0)
			{
				foreach (NewValueCommand newValueCommand in NewValues)
				{
					newValueCommand.ReleaseConnection();
				}
			}
		}

		#region 根据Lambda表达式创建Where条件
		/// <summary>解析一元运算符表达式</summary>
		/// <param name="ue">包含一元运算符的表达式</param>
		/// <returns>返回运算结果</returns>
		private object AnalyzeUnaryExpression(UnaryExpression ue)
		{
			return Expression.Lambda(ue.Operand).Compile().DynamicInvoke() ?? DBNull.Value;
		}

		/// <summary>获取成员表达式</summary>
		/// <param name="expression">包含成员表达式</param>
		/// <returns>返回运算结果</returns>
		private MemberExpression GetMemberExpression(Expression expression)
		{
			if (expression is MemberExpression member) { return member; }
			else if (expression is UnaryExpression ue) { return ue.Operand as MemberExpression; }
			return null;
		}

		/// <summary>
		/// 解析表达式的常量值
		/// </summary>
		/// <param name="expression">需要解析的表达式</param>
		/// <returns>返回解析结果</returns>
		private object CalculateResult(Expression expression)
		{
			object result = DBNull.Value;
			if (expression is MemberExpression)
				result = Expression.Lambda(expression).Compile().DynamicInvoke() ?? DBNull.Value;
			else if (expression is ConstantExpression)
				result = (expression as ConstantExpression).Value ?? DBNull.Value;
			else if (expression is UnaryExpression)
				result = AnalyzeUnaryExpression(expression as UnaryExpression);
			else if (expression is BinaryExpression)
				result = Expression.Lambda(expression).Compile().DynamicInvoke() ?? DBNull.Value;
			else if (expression is NewExpression)
				result = Expression.Lambda(expression as NewExpression).Compile().DynamicInvoke();
			else
				result = Expression.Lambda(expression).Compile().DynamicInvoke() ?? DBNull.Value;
			if (result != null && result != DBNull.Value && result is string[])
				return string.Concat("'", string.Join("','", result as string[]), "'");
			else if (result != null && result != DBNull.Value && result is System.Guid[])
				return string.Concat("'", string.Join("','", (result as Guid[]).Select(t => t.ToString()).ToArray()), "'");
			else if (result != null && result != DBNull.Value && result is byte[])
				return string.Join(",", result as byte[]);
			else if (result != null && result != DBNull.Value && result is int[])
				return string.Join(",", result as int[]);
			else if (result != null && result != DBNull.Value && result is long[])
				return string.Join(",", result as long[]);
			else if (result != null && result != DBNull.Value && result is short[])
				return string.Join(",", result as short[]);
			else if (result != null && result != DBNull.Value && result is Array)
				return string.Join(",", (result as Array));
			return result;
		}

		/// <summary>根据 ExpressionType 创建参数比较符号。</summary>
		/// <param name="builder">动态拼接 Transact-SQL 语句。</param>
		/// <param name="type">条件表达式。</param>
		protected void CreateExpressionType(StringBuilder builder, ExpressionType type)
		{
			if (type == ExpressionType.Equal) { builder.Append(" = "); }
			else if (type == ExpressionType.NotEqual) { builder.Append(" <> "); }
			else if (type == ExpressionType.GreaterThan) { builder.Append(" > "); }
			else if (type == ExpressionType.GreaterThanOrEqual) { builder.Append(" >= "); }
			else if (type == ExpressionType.LessThan) { builder.Append(" < "); }
			else if (type == ExpressionType.LessThanOrEqual) { builder.Append(" <= "); }
			//else if (type == ExpressionType.In) { builder.Append(" IN "); }
			//else if (type == ExpressionType.NotIn) { builder.Append(" NOT IN "); }
			//else if (type == ExpressionType.Like) { builder.Append(" LIKE "); }
			//else if (type == ExpressionType.NotLike) { builder.Append(" NOT LIKE "); }
			//else if (type == ExpressionType.Between) { builder.Append(" BETWEEN "); }
			//else if (type == ExpressionType.IsNull) { builder.Append(" IS NULL "); }
		}

		private void CreateMemberWhere(StringBuilder whereBuilder, MemberExpression mExp)
		{
			MemberInfo mi = mExp.Member;
			ColumnMappingAttribute cma = mi.GetCustomAttribute<ColumnMappingAttribute>();
			if (cma != null) { whereBuilder.Append(cma.ColumnName); return; }
			whereBuilder.Append(mi.Name);
		}

		private void CreateMemberParameter(StringBuilder whereBuilder, MemberExpression left, Expression right)
		{
			MemberInfo leftMember = left.Member;
			ColumnMappingAttribute cma = leftMember.GetCustomAttribute<ColumnMappingAttribute>();
			if (cma == null) { throw new ArgumentNullException("ColumnMappingAttribute", "成员属性中没有包含特性"); }
			if (right is MemberExpression rightMember && leftMember == rightMember.Member)
			{
				string paramName = this.CreateParameterName(cma.ColumnName);
				whereBuilder.Append(paramName);
				DbParameter parameter = this.CreateParameter(paramName, cma.SourceColumn, cma.DataType, cma.Nullable);
				parameter.Size = cma.Size;
				parameter.Precision = cma.Precision;
				parameter.Scale = cma.Scale;
				this.Parameters.Add(parameter); return;
			}
			else if (right is ConstantExpression constant)
			{
				string paramName = this.CreateParameterName(cma.ColumnName);
				whereBuilder.Append(paramName);
				DbParameter parameter = this.CreateParameter(paramName, cma.SourceColumn, cma.DataType, cma.Nullable);
				parameter.Size = cma.Size;
				parameter.Precision = cma.Precision;
				parameter.Scale = cma.Scale;
				parameter.Value = constant.Value ?? DBNull.Value;
				this.Parameters.Add(parameter); return;
			}
			else if (right is UnaryExpression)
			{
				string paramName = this.CreateParameterName(cma.ColumnName);
				whereBuilder.Append(paramName);
				DbParameter parameter = this.CreateParameter(paramName, cma.SourceColumn, cma.DataType, cma.Nullable);
				parameter.Size = cma.Size;
				parameter.Precision = cma.Precision;
				parameter.Scale = cma.Scale;
				parameter.Value = AnalyzeUnaryExpression(right as UnaryExpression);
				this.Parameters.Add(parameter); return;
			}
			else
			{
				string paramName = this.CreateParameterName(cma.ColumnName);
				whereBuilder.Append(paramName);
				DbParameter parameter = this.CreateParameter(paramName, cma.SourceColumn, cma.DataType, cma.Nullable);
				parameter.Size = cma.Size;
				parameter.Precision = cma.Precision;
				parameter.Scale = cma.Scale;
				parameter.Value = Expression.Lambda(right).Compile().DynamicInvoke() ?? DBNull.Value;
				this.Parameters.Add(parameter); return;
			}
			//whereBuilder.Append(rightMember.Name);
		}

		/// <summary>根据二元表达式创建WHERE条件</summary>
		/// <param name="whereBuilder"></param>
		/// <param name="exp"></param>
		private void CreateWhere(StringBuilder whereBuilder, BinaryExpression exp)
		{
			if (exp.NodeType == ExpressionType.AndAlso)
			{
				whereBuilder.Append("(");
				CreateWhere(whereBuilder, exp.Left);
				whereBuilder.Append(" AND ");
				CreateWhere(whereBuilder, exp.Right);
				whereBuilder.Append(")");
			}
			else if (exp.NodeType == ExpressionType.OrElse)
			{
				whereBuilder.Append("(");
				CreateWhere(whereBuilder, exp.Left);
				whereBuilder.Append(" OR ");
				CreateWhere(whereBuilder, exp.Right);
				whereBuilder.Append(")");
			}
			else if (exp.NodeType == ExpressionType.Equal || exp.NodeType == ExpressionType.NotEqual
				|| exp.NodeType == ExpressionType.GreaterThan || exp.NodeType == ExpressionType.GreaterThanOrEqual
				|| exp.NodeType == ExpressionType.LessThan || exp.NodeType == ExpressionType.LessThanOrEqual)
			{
				MemberExpression member = GetMemberExpression(exp.Left);
				if (member == null) { throw new MissingMemberException(GetType().ToString(), exp.Left.ToString()); }
				whereBuilder.Append("(");
				CreateMemberWhere(whereBuilder, member);
				CreateExpressionType(whereBuilder, exp.NodeType);
				CreateMemberParameter(whereBuilder, member, exp.Right);
				whereBuilder.Append(")");
			}
		}

		/// <summary>创建数据库比较语句</summary>
		/// <param name="whereBuilder"></param>
		/// <param name="expression">多个二元表达式集合</param>
		internal void CreateWhere(StringBuilder whereBuilder, Expression expression)
		{
			if (expression is BinaryExpression)
			{
				CreateWhere(whereBuilder, expression as BinaryExpression);
			}
		}

		/// <summary>创建数据库比较语句</summary>
		/// <param name="builder"></param>
		/// <param name="fields">多个二元表达式集合</param>
		internal void CreateUpdates(StringBuilder builder, List<MemberExpression> fields)
		{
			int length = builder.Length;
			foreach (MemberExpression member in fields)
			{
				ColumnMappingAttribute cma = member.Member.GetCustomAttribute<ColumnMappingAttribute>();
				if (cma == null) { continue; }
				string paramName = this.CreateParameterName(cma.ColumnName);
				if (length < builder.Length) { builder.Append(", "); }
				builder.Append(cma.ColumnName).Append("=").Append(paramName);
				DbParameter parameter = this.CreateParameter(paramName, cma.SourceColumn, cma.DataType, cma.Nullable);
				parameter.Size = cma.Size;
				parameter.Precision = cma.Precision;
				parameter.Scale = cma.Scale;
				this.Parameters.Add(parameter);
			}

		}
		#endregion

		#region 获取新值
		/// <summary>
		/// 执行查询，生成结果，根据新值命令返回结果的字段信息自动更新实体的值。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="table">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CreateNewValue<TR>(Result result, BaseTableType<TR> table) where TR : BaseTableRowType
		{
			if (NewValues != null && NewValues.Count > 0)
			{
				foreach (NewValueCommand newValueCommand in NewValues)
				{
					newValueCommand.CreateNewValue(result, table);
					if (result.Failure) { return result; }
				}
			}
			return result;
		}

		/// <summary>
		/// 执行查询，生成结果，根据新值命令返回结果的字段信息自动更新实体的值。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="entities">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual async Task<Result> CreateNewValueAsync(Result result, params AbstractEntity[] entities)
		{
			if (NewValues != null && NewValues.Count > 0)
			{
				foreach (NewValueCommand newValueCommand in NewValues)
				{
					await newValueCommand.CreateNewValueAsync(result, entities);
					if (result.Failure) { return result; }
				}
			}
			return result;
		}

		/// <summary>
		/// 执行查询，生成结果，根据新值命令返回结果的字段信息自动更新实体的值。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="entities">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CreateNewValue(Result result, params AbstractEntity[] entities)
		{
			if (NewValues != null && NewValues.Count > 0)
			{
				foreach (NewValueCommand newValueCommand in NewValues)
				{
					newValueCommand.CreateNewValue(result, entities);
					if (result.Failure) { return result; }
				}
			}
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="row">需要检测的键值对数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CreateNewValue(Result result, BaseTableRowType row)
		{
			if (NewValues != null && NewValues.Count > 0)
			{
				foreach (NewValueCommand newValueCommand in NewValues)
				{
					newValueCommand.CreateNewValue(result, row);
					row.RaiseCreatedValue();
					if (result.Failure) { return result; }
				}
			}
			return result;
		}
		#endregion

		#region 检查数据(CheckDataAsync)
		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="table">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual async Task<Result> CheckDataAsync<TR>(Result result, BaseTableType<TR> table) where TR : BaseTableRowType
		{
			if (CheckCommands != null && CheckCommands.Count > 0)
			{
				foreach (CheckCommand checkCommand in CheckCommands)
				{
					await checkCommand.CheckDataAsync(result, table);
					//if (!result.Successful) { return result; }
				}
			}
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="entities">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual async Task<Result> CheckDataAsync(Result result, params AbstractEntity[] entities)
		{
			if (CheckCommands != null && CheckCommands.Count > 0)
			{
				entities.ClearError();
				foreach (CheckCommand checkCommand in CheckCommands)
				{
					await checkCommand.CheckDataAsync(result, entities);
					//if (!result.Successful) { return result; }
				}
			}
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="anonObject">需要检测的键值对数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual async Task<Result> CheckDataAsync(Result result, object anonObject)
		{
			if (CheckCommands != null && CheckCommands.Count > 0)
			{
				foreach (CheckCommand checkCommand in CheckCommands)
				{
					await checkCommand.CheckDataAsync(result, anonObject);
					//if (!result.Successful) { return result; }
				}
			}
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="row">需要检测的键值对数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual async Task<Result> CheckDataAsync(Result result, BaseTableRowType row)
		{
			if (CheckCommands != null && CheckCommands.Count > 0)
			{
				foreach (CheckCommand checkCommand in CheckCommands)
				{
					await checkCommand.CheckDataAsync(result, row);
					//if (!result.Successful) { return result; }
				}
			}
			return result;
		}
		#endregion

		#region 检查数据(CheckData)
		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="table">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CheckData<TR>(Result result, BaseTableType<TR> table) where TR : BaseTableRowType
		{
			if (CheckCommands != null && CheckCommands.Count > 0)
			{
				foreach (CheckCommand checkCommand in CheckCommands)
				{
					checkCommand.CheckData(result, table);
					//if (!result.Successful) { return result; }
				}
			}
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="entities">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CheckData(Result result, params AbstractEntity[] entities)
		{
			if (CheckCommands != null && CheckCommands.Count > 0)
			{
				entities.ClearError();
				foreach (CheckCommand checkCommand in CheckCommands)
				{
					checkCommand.CheckData(result, entities);
					//if (!result.Successful) { return result; }
				}
			}
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="anonObject">需要检测的键值对数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CheckData(Result result, object anonObject)
		{
			if (CheckCommands != null && CheckCommands.Count > 0)
			{
				foreach (CheckCommand checkCommand in CheckCommands)
				{
					checkCommand.CheckData(result, anonObject);
					//if (!result.Successful) { return result; }
				}
			}
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="row">需要检测的键值对数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CheckData(Result result, BaseTableRowType row)
		{
			if (CheckCommands != null && CheckCommands.Count > 0)
			{
				foreach (CheckCommand checkCommand in CheckCommands)
				{
					checkCommand.CheckData(result, row);
					//if (!result.Successful) { return result; }
				}
			}
			return result;
		}
		#endregion

		#region 执行数据库命令
		/// <summary>
		/// 创建检查数据命令
		/// </summary>
		/// <returns>返回继承与 CheckDataCommand 的实例</returns>
		internal protected abstract CheckCommand CreateCheckCommand();

		/// <summary>
		/// 创建新值命令
		/// </summary>
		/// <returns>返回继承与 NewValueCommand 的实例</returns>
		internal protected abstract NewValueCommand CreateNewValueCommand();

		/// <summary>
		/// 针对 .NET Framework 数据提供程序的 Connection 对象执行 SQL 语句，并返回受影响的行数。
		/// </summary>
		/// <returns>受影响的行数。</returns>
		internal protected virtual int ExecuteNonQuery()
		{
			return dataDbCommand.ExecuteNonQuery();
		}

		/// <summary>将本对象属性复制到目标对象中</summary>
		/// <param name="staticCommand"></param>
		protected internal void CopyTo(StaticCommand staticCommand)
		{
			staticCommand.SourceColumn = SourceColumn;
			staticCommand.CommandName = CommandName;
		}

		/// <summary>
		/// 针对 .NET Framework 数据提供程序的 Connection 对象执行 SQL 语句，并返回受影响的行数。
		/// </summary>
		/// <returns>返回异步操作的结果, 受影响的行数。</returns>
		internal protected abstract System.Threading.Tasks.Task<int> ExecuteNonQueryAsync();

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="pagination"><![CDATA[需要填充的 Pagination<T> 类实例。]]></param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <exception cref="System.ArgumentNullException">参数 pagination 为 null。</exception>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		internal protected virtual IPagination<T> GetPagination<T>(Pagination<T> pagination, int pageSize, int pageIndex) where T : AbstractEntity, new()
		{
			if (pagination == null) { throw new ArgumentNullException("pagination"); }
			ResetPaginationParameter(pageSize, pageIndex);
			using (DbDataReader reader = ExecuteReader(CommandBehavior.Default))
			{
				if (!reader.IsClosed && reader.HasRows)
				{
					EntityPropertyMeta[] properties = AbstractEntity.GetProperties<T>();
					Dictionary<int, EntityPropertyMeta> fieldProperty = new Dictionary<int, EntityPropertyMeta>(reader.FieldCount);
					Type type = typeof(T);
					for (int index = 0; index < reader.FieldCount; index++)
					{
						string name = reader.GetName(index);
						foreach (EntityPropertyMeta info in properties)
						{
							if (info.Mapping != null && info.Mapping.ColumnName == name) { fieldProperty.Add(index, info); break; }
							else if (string.Compare(info.Name, name, true) == 0) { fieldProperty.Add(index, info); break; }
						}
					}
					while (reader.Read())
					{
						T entity = new T();
						foreach (KeyValuePair<int, EntityPropertyMeta> keyValue in fieldProperty)
						{
							keyValue.Value.SetValue(entity, reader.GetValue(keyValue.Key));
						}
						pagination.Add(entity);
					}
					reader.Close();
					ProcessOutputParamater(out int returnCount, out int returnValue);
					pagination.Capacity = returnCount;
				}
			}
			return pagination;
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		internal protected virtual IPagination<T> GetPagination<T>(int pageSize, int pageIndex) where T : AbstractEntity, new()
		{
			Pagination<T> pagination = new Pagination<T>();
			return GetPagination<T>(pagination, pageSize, pageIndex);
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		internal protected virtual IPagination<T> GetPagination<T>(AbstractCondition condition) where T : AbstractEntity, new()
		{
			return GetPagination<T>(condition.PageSize, condition.PageIndex);
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="pagination">需要填充的数据列表</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		internal protected virtual IPagination<T> GetPagination<T>(Pagination<T> pagination, AbstractCondition condition) where T : AbstractEntity, new()
		{
			return GetPagination<T>(pagination, condition.PageSize, condition.PageIndex);
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="pagination">需要填充的数据列表</param>
		/// <param name="anonObject">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		internal protected virtual IPagination<T> GetPagination<T>(Pagination<T> pagination, object anonObject) where T : AbstractEntity, new()
		{
			if (anonObject == null) { throw new ArgumentNullException("anonObject"); }
			int pageSize = 0, pageIndex = 0;
			Type anonType = anonObject.GetType();
			PropertyInfo pageSizeInfo = anonType.GetProperty("PageSize", BindingFlags.IgnoreCase);
			if (pageSizeInfo != null) { pageSize = Convert.ToInt32(pageSizeInfo.GetValue(anonObject, null)); }
			PropertyInfo pageIndexInfo = anonType.GetProperty("PageIndex", BindingFlags.IgnoreCase);
			if (pageSizeInfo != null) { pageIndex = Convert.ToInt32(pageIndexInfo.GetValue(anonObject, null)); }
			return GetPagination<T>(pagination, pageSize, pageIndex);
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="anonObject">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>执行Transact-SQL命令结果，包含错误代码，返回记录数，受影响的记录数。</returns>
		internal protected virtual IPagination<T> GetPagination<T>(object anonObject) where T : AbstractEntity, new()
		{
			if (anonObject == null) { throw new ArgumentNullException("anonObject"); }
			int pageSize = 0, pageIndex = 0;
			Type anonType = anonObject.GetType();
			PropertyInfo pageSizeInfo = anonType.GetProperty("PageSize", BindingFlags.IgnoreCase);
			if (pageSizeInfo != null) { pageSize = Convert.ToInt32(pageSizeInfo.GetValue(anonObject, null)); }
			PropertyInfo pageIndexInfo = anonType.GetProperty("PageIndex", BindingFlags.IgnoreCase);
			if (pageSizeInfo != null) { pageIndex = Convert.ToInt32(pageIndexInfo.GetValue(anonObject, null)); }
			return GetPagination<T>(pageSize, pageIndex);
		}

		/// <summary>
		/// 在数据源上创建该命令的准备好的（或已编译的）版本。
		/// </summary>
		internal protected virtual void Prepare()
		{
			dataDbCommand.Prepare();
		}
		#endregion

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		protected internal override void ReadXml(System.Xml.XmlReader reader)
		{
			base.ReadXml(reader);
			if (CheckCommands != null && CheckCommands.Count > 0)
			{
				foreach (CheckCommand checkCmd in CheckCommands)
				{
					checkCmd.ReadOwnerParameter();
				}
			}
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected internal override bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == CommandTextElement)//兼容5.0新版结构信息
			{
				string text = reader.ReadString();
				if (!string.IsNullOrEmpty(text))
				{
					StringBuilder builder = new StringBuilder(text);
					builder.Replace("\n", "", 0, 10).Replace("\t", "");
					builder.Replace("\n", "", builder.Length - 1, 1);
					builder.Replace("\n", "\r\n");
					CommandText = builder.ToString();
				}
			}
			else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == XmlElementName)
			{
				return true;
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DataCommand.CheckCommandsConfig)
			{
				#region 读取数据检测命令
				System.Xml.XmlReader reader2 = reader.ReadSubtree();
				while (reader2.Read())  //读取所有检测命令节点信息
				{
					if (reader2.NodeType == XmlNodeType.Element && reader2.LocalName == DataCommand.CheckCommandConfig)
					{
						CheckCommand checkCommand = CreateCheckCommand();
						checkCommand.ReadXml(reader.ReadSubtree());
						CheckCommands.Add(checkCommand);
					}
					else if (reader2.NodeType == XmlNodeType.EndElement && reader2.LocalName == DataCommand.CheckCommandsConfig)
					{
						break;
					}
				}
				#endregion
			}
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == NewValuesElement)
			{
				#region 读取数据新值命令
				System.Xml.XmlReader reader2 = reader.ReadSubtree();
				while (reader2.Read())  //读取所有检测命令节点信息
				{
					if (reader2.NodeType == XmlNodeType.Element && reader2.Name == NewValueCommand.XmlElementName)
					{
						NewValueCommand checkCommand = CreateNewValueCommand();
						checkCommand.ReadXml(reader.ReadSubtree());
						NewValues.Add(checkCommand);
					}
					else if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name == NewValuesElement)
					{
						break;
					}
				}
				#endregion
			}
			return base.ReadContent(reader);
		}
	}
}
