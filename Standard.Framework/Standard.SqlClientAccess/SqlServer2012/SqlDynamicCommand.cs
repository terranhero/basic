using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Basic.DataAccess;
using Basic.Enums;
using Basic.Properties;

namespace Basic.SqlServer2012
{
	/// <summary>
	/// 表示要对 SQL Server 数据库执行的一个动态结构的 Transact-SQL 语句或存储过程。
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.Xml.Serialization.XmlRoot(DataCommand.DynamicCommandConfig)]
	internal sealed class SqlDynamicCommand : DynamicCommand, IDisposable, ICloneable
	{
		private readonly SqlCommand sqlCommand;

		/// <summary>
		/// 初始化 SqlDynamicCommand 类的新实例。 
		/// </summary>
		public SqlDynamicCommand() : this(new SqlCommand()) { }

		/// <summary>
		/// 根据数据库命令，初始化 SqlDynamicCommand 类的新实例，主要克隆实例时使用。
		/// </summary>
		/// <param name="dbCommand">表示 SqlCommand 类实例。</param>
		private SqlDynamicCommand(SqlCommand dbCommand) : base(dbCommand) { sqlCommand = dbCommand; }

		/// <summary>当前命令的数据库类型</summary>
		public override ConnectionType ConnectionType { get { return ConnectionType.SqlConnection; } }

		/// <summary>
		/// 初始化参数值
		/// </summary>
		/// <param name="dbParam">数据库参数</param>
		/// <param name="value">需要设置的数据库参数值</param>
		internal protected override void ResetParameterValue(DbParameter dbParam, object value)
		{
			SqlParameter parameter = dbParam as SqlParameter;
			if (parameter == null)
				throw new ArgumentException(string.Format(Strings.Access_InvalidArgument, "dbParam"), "dbParam");

			if (value == null) { parameter.SqlValue = DBNull.Value; }
			if (parameter.SqlDbType == SqlDbType.NVarChar && value is int[])
			{
				parameter.SqlValue = string.Join(",", (value as int[]));
			}
			else { parameter.SqlValue = value; }
		}

		/// <summary>
		/// 动态拼接 Transact-SQL 语句
		/// </summary>
		/// <param name="pageSize">分页时每页显示的记录数量。</param>
		/// <param name="pageIndex">分页时，当前页索引。</param>
		protected internal override void InitializeCommandText(int pageSize, int pageIndex)
		{
			StringBuilder builder = new StringBuilder(2000);
			if (pageSize == 0 && pageIndex == 0) { InitializeNoPaginationText(builder, 0); }
			else if (pageSize > 0 && pageIndex == 1) { InitializeNoPaginationText(builder, pageSize); }
			else if (pageSize > 0 && pageIndex >= 2)
			{
				List<string> orderList = new List<string>(3);
				if (mOrderList.Count > 0) { orderList.AddRange(mOrderList.ToArray()); }
				else
				{
					if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.OrderText))
						orderList.Add(joinCommand.OrderText);
					if (!string.IsNullOrEmpty(OrderText)) { orderList.Add(OrderText); }
				}

				int skipRows = pageSize * (pageIndex - 1);
				if (skipRows <= 0) { skipRows = 0; }
				if (WithClauses.Count > 0)
				{
					builder.Append("WITH "); List<string> withList = new List<string>();
					foreach (WithClause with in WithClauses) { withList.Add(with.ToSql()); }
					builder.Append(string.Join(Environment.NewLine + ", ", withList.ToArray())).AppendLine();
				}
				if (DistinctStatus == false) { builder.AppendFormat("SELECT COUNT(1) OVER() AS {0}", ReturnCountName); }
				else { builder.AppendFormat("SELECT DISTINCT COUNT(1) OVER() AS {0}", ReturnCountName); }
				//builder.AppendFormat("SELECT TOP {0} * ", pageSize);
				//builder.AppendLine();
				//if (DistinctStatus == true) { builder.AppendFormat(" FROM (SELECT DISTINCT TOP {0} ", nextPageMaxNumber); }
				//else { builder.AppendFormat(" FROM (SELECT TOP {0} ", nextPageMaxNumber); }
				//builder.AppendFormat("ROW_NUMBER() OVER({0}) AS PAGEROWNUMBER", orderBy);
				//builder.AppendFormat(",COUNT(1) OVER() AS {0}", AbstractDataCommand.ReturnCountName);

				if (!string.IsNullOrEmpty(SelectText)) { builder.Append(",").Append(SelectText); }
				if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.SelectText))
				{
					builder.Append(',').Append(_dynamicJoinCommand.SelectText);
				}
				else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.SelectText))
					builder.Append(",").Append(joinCommand.SelectText);

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
				if (_dynamicJoinCommand != null && !string.IsNullOrEmpty(_dynamicJoinCommand.GroupText))
				{
					if (builderLength != builder.Length) { builder.Append(',').Append(_dynamicJoinCommand.GroupText); }
					else { builder.AppendLine().Append(" GROUP BY ").Append(_dynamicJoinCommand.GroupText); }
				}
				else if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.GroupText))
				{
					if (builderLength != builder.Length) { builder.Append(",").Append(joinCommand.GroupText); }
					else { builder.AppendLine().Append(" GROUP BY ").Append(joinCommand.GroupText); }
				}
				if (orderList.Count == 0)
				{
					string errorMsg = string.Format(Strings.Access_NotExist_OrderBy, CommandName);
					throw new ArgumentException(errorMsg);  //'{0}' 命令中 缺少 Order By子句，如果需要分页则必须需要此子句。
				}

				builder.AppendFormat(" ORDER BY {0} OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY",
					string.Join(",", orderList.ToArray()), skipRows, pageSize);
				//builder.AppendFormat(") T1 WHERE T1.PAGEROWNUMBER>={0}", nextPageRowNumber);
			}
			dataDbCommand.CommandText = builder.ToString();
		}

		/// <summary>
		/// 针对 Connection 执行 CommandText，并生成 DbDataReader。 
		/// </summary>
		/// <returns>一个 DbDataReader 对象。 </returns>
		internal protected override System.Threading.Tasks.Task<DbDataReader> ExecuteReaderAsync()
		{
			return sqlCommand.ExecuteReaderAsync().ContinueWith<DbDataReader>(task => task.Result);
		}

		/// <summary>
		/// 针对 Connection 执行 CommandText，并使用 CommandBehavior 值之一返回 DbDataReader。 
		/// </summary>
		/// <param name="behavior">类型： System.Data.CommandBehavior，CommandBehavior 值之一。 </param>
		/// <returns>一个 DbDataReader 对象。 </returns>
		internal protected override System.Threading.Tasks.Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior)
		{
			return sqlCommand.ExecuteReaderAsync(behavior).ContinueWith<DbDataReader>(task => task.Result);
		}

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="parameter">数据库命令执行的参数</param>
		/// <param name="dbType">SqlServer数据库列类型,SqlDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		internal protected override void ConvertParameterType(DbParameter parameter, DataTypeEnum dbType, byte precision, byte scale)
		{
			SqlParameter sqlParameter = parameter as SqlParameter;
			SqlParameterConverter.ConvertSqlParameterType(sqlParameter, dbType, precision, scale);
		}

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="parameter">数据库命令执行的参数</param>
		/// <param name="dbType">SqlServer数据库列类型,SqlDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		internal protected override void ConvertParameterType(DbParameter parameter, DbTypeEnum dbType, byte precision, byte scale)
		{
			SqlParameter sqlParameter = parameter as SqlParameter;
			SqlParameterConverter.ConvertSqlParameterType(sqlParameter, dbType, precision, scale);
		}

		/// <summary>
		/// 返回存储过程参数名称全名称
		/// </summary>
		/// <param name="parameterName">不带参数符号的参数名称</param>
		/// <returns>返回带存储过程符号的参数名称</returns>
		public override string CreateParameterName(string parameterName)
		{
			if (parameterName.StartsWith("@")) { return parameterName; }
			return string.Concat("@", parameterName);
		}

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <returns>一个 DbParameter 对象。</returns>
		public override DbParameter CreateParameter()
		{
			return sqlCommand.CreateParameter();
		}

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="size">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		internal protected override DbParameter CreateParameter(string parameterName, string sourceColumn, DataTypeEnum dbType,
			 int size, ParameterDirection direction, bool isNullable)
		{
			SqlParameter parameter = sqlCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Size = size;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			SqlParameterConverter.ConvertSqlParameterType(parameter, dbType, 0, 0);
			return parameter;
		}

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		/// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		internal protected override DbParameter CreateParameter(string parameterName, string sourceColumn, DataTypeEnum dbType,
			 byte precision, byte scale, ParameterDirection direction, bool isNullable)
		{
			SqlParameter parameter = sqlCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Precision = precision;
			parameter.Scale = scale;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			SqlParameterConverter.ConvertSqlParameterType(parameter, dbType, precision, scale);
			return parameter;
		}

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public override DbParameter CreateParameter(string parameterName, string sourceColumn, DbTypeEnum dbType, bool isNullable)
		{
			return CreateParameter(parameterName, sourceColumn, dbType, 0, ParameterDirection.Input, isNullable);
		}

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="size">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public override DbParameter CreateParameter(string parameterName, string sourceColumn, DbTypeEnum dbType, int size, bool isNullable)
		{
			return CreateParameter(parameterName, sourceColumn, dbType, size, ParameterDirection.Input, isNullable);
		}

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public override DbParameter CreateParameter(string parameterName, string sourceColumn, DbTypeEnum dbType,
			 byte precision, byte scale, bool isNullable)
		{
			return CreateParameter(parameterName, sourceColumn, dbType, precision, scale, ParameterDirection.Input, isNullable);
		}

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="size">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public override DbParameter CreateParameter(string parameterName, string sourceColumn, DbTypeEnum dbType,
			 int size, ParameterDirection direction, bool isNullable)
		{
			SqlParameter parameter = sqlCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Size = size;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			SqlParameterConverter.ConvertSqlParameterType(parameter, dbType, 0, 0);
			return parameter;
		}

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		/// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public override DbParameter CreateParameter(string parameterName, string sourceColumn, DbTypeEnum dbType,
			 byte precision, byte scale, ParameterDirection direction, bool isNullable)
		{
			SqlParameter parameter = sqlCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Precision = precision;
			parameter.Scale = scale;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			SqlParameterConverter.ConvertSqlParameterType(parameter, dbType, precision, scale);
			return parameter;
		}

		#region 创建数据库命令参数并添加到参数集合中

		/// <summary>创建数据库命令参数并添加到参数集合中</summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public override DbParameter CreateAddParameter(string parameterName, string sourceColumn, DbTypeEnum dbType, bool isNullable)
		{
			return CreateAddParameter(parameterName, sourceColumn, dbType, 0, ParameterDirection.Input, isNullable);
		}

		/// <summary>创建数据库命令参数并添加到参数集合中</summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="size">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public override DbParameter CreateAddParameter(string parameterName, string sourceColumn, DbTypeEnum dbType, int size, bool isNullable)
		{
			return CreateAddParameter(parameterName, sourceColumn, dbType, size, ParameterDirection.Input, isNullable);
		}

		/// <summary>创建数据库命令参数并添加到参数集合中</summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public override DbParameter CreateAddParameter(string parameterName, string sourceColumn, DbTypeEnum dbType,
			byte precision, byte scale, bool isNullable)
		{
			return CreateAddParameter(parameterName, sourceColumn, dbType, precision, scale, ParameterDirection.Input, isNullable);
		}

		/// <summary>创建数据库命令参数并添加到参数集合中</summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="size">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public override DbParameter CreateAddParameter(string parameterName, string sourceColumn, DbTypeEnum dbType, int size,
			ParameterDirection direction, bool isNullable)
		{
			SqlParameter parameter = sqlCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Size = size;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			SqlParameterConverter.ConvertSqlParameterType(parameter, dbType, 0, 0);
			//sqlCommand.Parameters.Add(parameter);
			DbParameters.Add(parameter);
			return parameter;
		}

		/// <summary>创建数据库命令参数并添加到参数集合中</summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		/// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public override DbParameter CreateAddParameter(string parameterName, string sourceColumn, DbTypeEnum dbType,
			byte precision, byte scale, ParameterDirection direction, bool isNullable)
		{
			SqlParameter parameter = sqlCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Precision = precision;
			parameter.Scale = scale;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			SqlParameterConverter.ConvertSqlParameterType(parameter, dbType, precision, scale);
			//sqlCommand.Parameters.Add(parameter);
			DbParameters.Add(parameter);
			return parameter;
		}

		#endregion

		/// <summary>
		/// 在 DataSet 的指定范围中添加或刷新行，以与使用 DataTable 名称的数据源中的行匹配。 
		/// </summary>
		/// <param name="table">待填充的实体类实例</param>
		/// <returns>执行Transact-SQL命令结果，返回受影响的记录数。</returns>
		internal protected override int Fill(DataTable table)
		{
			SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);
			return dataAdapter.Fill(table);
		}

		/// <summary>
		/// 在 DataSet 中添加或刷新行。
		/// </summary>
		/// <param name="dataSet">要用记录和架构（如果必要）填充的 DataSet。</param>
		/// <returns>已在 DataSet 中成功添加或刷新的行数。 这不包括受不返回行的语句影响的行。 </returns>
		internal protected override int Fill(DataSet dataSet)
		{
			SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);
			return dataAdapter.Fill(dataSet);
		}

		/// <summary>
		/// 将当前命令的参数复制到指定的集合中
		/// </summary>
		/// <param name="parameters"></param>
		internal protected override void CopyParametersTo(ICollection<DbParameter> parameters)
		{
			foreach (SqlParameter parameter in sqlCommand.Parameters)
			{
				parameters.Add((parameter as ICloneable).Clone() as SqlParameter);
			}
		}

		/// <summary>
		/// 初始化查询参数列表
		/// </summary>
		internal protected override void InitializeParameters()
		{
			lock (Parameters)
			{
				Parameters.Clear();
				if (DbParameters != null && DbParameters.Count > 0)
				{
					Parameters.AddRange(DbParameters.ToArray());
				}
				if (_dynamicJoinCommand != null && _dynamicJoinCommand.Parameters.Length > 0)
				{
					foreach (SqlParameter parameter in _dynamicJoinCommand.Parameters)
					{
						if (!Parameters.Contains(parameter.ParameterName))
						{
							Parameters.Add((parameter as ICloneable).Clone() as SqlParameter);
						}
					}
				}
				else if (joinCommand != null && joinCommand.Parameters.Length > 0)
				{
					foreach (SqlParameter parameter in joinCommand.Parameters)
					{
						if (!Parameters.Contains(parameter.ParameterName))
						{
							Parameters.Add((parameter as ICloneable).Clone() as SqlParameter);
						}
					}
				}
			}
		}

		/// <summary>
		/// 创建作为当前实例副本的新对象。
		/// </summary>
		/// <returns>作为此实例副本的新对象。</returns>
		internal protected override DataCommand CloneCommand() { return Clone(); }

		/// <summary>
		/// 创建作为当前实例副本的新对象。
		/// </summary>
		/// <returns>作为此实例副本的新对象。</returns>
		public SqlDynamicCommand Clone()
		{
			lock (this)
			{
				SqlDynamicCommand dynamicCommand = new SqlDynamicCommand();
				if (this.DbParameters != null && this.DbParameters.Count >= 0)
				{
					dynamicCommand.DbParameters.AddRange(DbParameters.Select(m =>
					{
						return (m as ICloneable).Clone() as SqlParameter;
					}));

					//List<SqlParameter> list = new List<SqlParameter>(this.DbParameters.Length);
					//foreach (SqlParameter parameter in this.DbParameters)
					//{
					//	list.Add((parameter as ICloneable).Clone() as SqlParameter);
					//}
					//dynamicCommand.DbParameters = list.ToArray();
				}
				CopyTo(dynamicCommand);
				return dynamicCommand;
			}
		}

		/// <summary>
		/// 创建作为当前实例副本的新对象。
		/// </summary>
		/// <returns>作为此实例副本的新对象。</returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}
	}
}
