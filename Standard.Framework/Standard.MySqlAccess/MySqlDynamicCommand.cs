using Basic.DataAccess;
using Basic.Enums;
using Basic.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Basic.MySqlAccess
{
	/// <summary>
	/// 表示要对 MySql 数据库执行的一个动态结构的 Transact-SQL 语句或存储过程。
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.Xml.Serialization.XmlRoot(DataCommand.DynamicCommandConfig)]
	internal sealed class MySqlDynamicCommand : DynamicCommand, IDisposable, ICloneable
	{
		private readonly MySqlCommand _MySqlCommand;

		/// <summary>
		/// 初始化 MySqlDynamicCommand 类的新实例。 
		/// </summary>
		public MySqlDynamicCommand() : this(new MySqlCommand()) { }

		/// <summary>
		/// 根据数据库命令，初始化 MySqlDynamicCommand 类的新实例，主要克隆实例时使用。
		/// </summary>
		/// <param name="dbCommand">表示 SqlCommand 类实例。</param>
		private MySqlDynamicCommand(MySqlCommand dbCommand) : base(dbCommand) { _MySqlCommand = dbCommand; }

		/// <summary>当前命令的数据库类型</summary>
		public override ConnectionType ConnectionType { get { return ConnectionType.MySqlConnection; } }

		/// <summary>
		/// 针对 Connection 执行 CommandText，并生成 DbDataReader。 
		/// </summary>
		/// <returns>一个 DbDataReader 对象。 </returns>
		internal protected override async System.Threading.Tasks.Task<DbDataReader> ExecuteReaderAsync()
		{
			return await _MySqlCommand.ExecuteReaderAsync();
		}

		/// <summary>
		/// 针对 Connection 执行 CommandText，并使用 CommandBehavior 值之一返回 DbDataReader。 
		/// </summary>
		/// <param name="behavior">类型： System.Data.CommandBehavior，CommandBehavior 值之一。 </param>
		/// <returns>一个 DbDataReader 对象。 </returns>
		internal protected override async System.Threading.Tasks.Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior)
		{
			return await _MySqlCommand.ExecuteReaderAsync(behavior);
		}

		/// <summary>初始化计算记录数 的 Transact-SQL 语句。</summary>
		protected internal override void InitCountText()
		{
			StringBuilder builder = new StringBuilder(1000);
			if (WithClauses.Count > 0)
			{
				builder.Append("WITH "); List<string> withList = new List<string>();
				foreach (WithClause with in WithClauses)
				{
					withList.Add(string.Concat(with.TableName, "(", with.TableDefinition, ") AS (", with.TableQuery, ")"));
				}
				builder.Append(string.Join("," + Environment.NewLine, withList.ToArray())).AppendLine();
			}
			builder.Append("SELECT COUNT(1)");
			List<string> fromList = new List<string>(3);
			int builderLength = builder.Length;
			if (!string.IsNullOrEmpty(FromText))
				builder.Append(" FROM ").Append(FromText);
			if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.FromText))
			{
				if (builderLength != builder.Length) { builder.Append(" ").Append(joinCommand.FromText); }
				else { builder.Append(" FROM ").Append(joinCommand.FromText); }
			}

			List<string> whereList = new List<string>(3);
			if (!string.IsNullOrEmpty(WhereText))
				whereList.Add(WhereText);
			if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.WhereText))
				whereList.Add(joinCommand.WhereText);
			if (!string.IsNullOrEmpty(TempWhereText))
				whereList.Add(TempWhereText);

			if (whereList.Count > 0) { builder.Append(" WHERE ").Append(string.Join(" AND ", whereList.ToArray())); }

			if (!string.IsNullOrWhiteSpace(GroupText))
			{
				builder.AppendLine().Append(" GROUP BY ").Append(GroupText);
			}
			if (string.IsNullOrWhiteSpace(HavingText) == false) { builder.AppendLine().Append(" HAVING ").Append(HavingText); }
			dataDbCommand.CommandText = builder.ToString();
		}

		/// <summary>动态拼接 Transact-SQL 语句</summary>
		/// <param name="sorting">是否需要排序</param>
		protected internal override void InitializeCommandText(bool sorting)
		{
			StringBuilder builder = new StringBuilder(2000);
			if (WithClauses.Count > 0)
			{
				builder.Append("WITH "); List<string> withList = new List<string>();
				foreach (WithClause with in WithClauses)
				{
					withList.Add(string.Concat(with.TableName, "(", with.TableDefinition, ") AS (", with.TableQuery, ")"));
				}
				builder.Append(string.Join("," + Environment.NewLine, withList.ToArray())).AppendLine();
			}
			builder.Append("SELECT ");
			if (!string.IsNullOrEmpty(SelectText))
				builder.Append(SelectText);
			if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.SelectText))
				builder.Append(",").Append(joinCommand.SelectText);

			int builderLength = builder.Length;

			if (!string.IsNullOrEmpty(FromText))
				builder.Append(" FROM ").Append(FromText);
			if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.FromText))
			{
				if (builderLength != builder.Length) { builder.Append(" ").Append(joinCommand.FromText); }
				else { builder.Append(" FROM ").Append(joinCommand.FromText); }
			}

			List<string> whereList = new List<string>(3);
			if (string.IsNullOrEmpty(WhereText) == false) { whereList.Add(WhereText); }
			if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.WhereText))
				whereList.Add(joinCommand.WhereText);
			if (!string.IsNullOrEmpty(TempWhereText)) { whereList.Add(TempWhereText); }

			if (whereList.Count > 0) { builder.Append(" WHERE ").Append(string.Join(" AND ", whereList.ToArray())); }
			builderLength = builder.Length;
			if (!string.IsNullOrEmpty(GroupText))
				builder.Append(" GROUP BY ").Append(GroupText);
			if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.GroupText))
			{
				if (builderLength != builder.Length) { builder.Append(",").Append(joinCommand.GroupText); }
				else { builder.AppendLine().Append(" GROUP BY ").Append(joinCommand.GroupText); }
			}
			if (string.IsNullOrWhiteSpace(HavingText) == false) { builder.AppendLine().Append(" HAVING ").Append(HavingText); }

			List<string> orderList = new List<string>(3);
			string orderBy = null;

			if (mOrderList.Count > 0) { orderList.AddRange(mOrderList.ToArray()); }
			else
			{
				if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.OrderText))
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
					foreach (WithClause with in WithClauses)
					{
						withList.Add(string.Concat(with.TableName, "(", with.TableDefinition, ") AS (", with.TableQuery, ")"));
					}
					builder.Append(string.Join("," + Environment.NewLine, withList.ToArray())).AppendLine();
				}
				if (DistinctStatus == false) { builder.AppendFormat("SELECT COUNT(1) OVER() AS {0}", ReturnCountName); }
				else { builder.AppendFormat("SELECT DISTINCT COUNT(1) OVER() AS {0}", ReturnCountName); }

				if (!string.IsNullOrEmpty(SelectText)) { builder.Append(",").Append(SelectText); }
				if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.SelectText))
					builder.Append(",").Append(joinCommand.SelectText);

				int builderLength = builder.Length;

				if (!string.IsNullOrEmpty(FromText))
					builder.Append(" FROM ").Append(FromText);
				if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.FromText))
				{
					if (builderLength != builder.Length) { builder.Append(" ").Append(joinCommand.FromText); }
					else { builder.Append(" FROM ").Append(joinCommand.FromText); }
				}

				List<string> whereList = new List<string>(3);
				if (string.IsNullOrEmpty(WhereText) == false) { whereList.Add(WhereText); }
				if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.WhereText))
					whereList.Add(joinCommand.WhereText);
				if (!string.IsNullOrEmpty(TempWhereText)) { whereList.Add(TempWhereText); }

				if (whereList.Count > 0) { builder.Append(" WHERE ").Append(string.Join(" AND ", whereList.ToArray())); }
				builderLength = builder.Length;
				if (!string.IsNullOrEmpty(GroupText))
					builder.Append(" GROUP BY ").Append(GroupText);
				if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.GroupText))
				{
					if (builderLength != builder.Length) { builder.Append(",").Append(joinCommand.GroupText); }
					else { builder.AppendLine().Append(" GROUP BY ").Append(joinCommand.GroupText); }
				}
				if (orderList.Count == 0)
				{
					string errorMsg = string.Format(Strings.Access_NotExist_OrderBy, CommandName);
					throw new ArgumentException(errorMsg);  //'{0}' 命令中 缺少 Order By子句，如果需要分页则必须需要此子句。
				}

				builder.AppendFormat(" ORDER BY {0} LIMIT {1} , {2}", string.Join(",", orderList.ToArray()), skipRows, pageSize);
				//builder.AppendFormat(") T1 WHERE T1.PAGEROWNUMBER>={0}", nextPageRowNumber);
			}
			dataDbCommand.CommandText = builder.ToString();
		}

		/// <summary>
		/// 创建没有分页或获取第一页的 Transact-SQL 语句
		/// </summary>
		/// <param name="pageSize">分页时每页显示的记录数量。</param>
		/// <param name="builder">生成的 Transact-SQL语句结果。</param>
		protected internal override void InitializeNoPaginationText(StringBuilder builder, int pageSize)
		{
			if (WithClauses.Count > 0)
			{
				builder.Append("WITH "); List<string> withList = new List<string>();
				foreach (WithClause with in WithClauses)
				{
					withList.Add(string.Concat(with.TableName, "(", with.TableDefinition, ") AS (", with.TableQuery, ")"));
				}
				builder.Append(string.Join("," + Environment.NewLine, withList.ToArray())).AppendLine();
			}
			if (DistinctStatus == false) { builder.AppendFormat("SELECT COUNT(1) OVER() AS {0}", ReturnCountName); }
			else { builder.AppendFormat("SELECT DISTINCT COUNT(1) OVER() AS {0}", ReturnCountName); }

			int builderLength = builder.Length;
			if (!string.IsNullOrEmpty(SelectText)) { builder.Append(", ").Append(SelectText); }
			if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.SelectText))
			{
				if (builderLength < builder.Length) { builder.Append(",").Append(joinCommand.SelectText); }
				else { builder.Append(",").Append(joinCommand.SelectText); }
			}
			builderLength = builder.Length;
			if (!string.IsNullOrEmpty(FromText)) { builder.AppendLine().Append(" FROM ").Append(FromText); }
			if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.FromText))
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
			if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.WhereText))
			{
				if (builderLength != builder.Length) { builder.Append(" AND ").Append(joinCommand.WhereText); }
				else { builder.AppendLine().Append(" WHERE ").Append(joinCommand.WhereText); }
			}
			builderLength = builder.Length;
			if (string.IsNullOrEmpty(GroupText) == false) { builder.Append(" GROUP BY ").Append(GroupText); }
			if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.GroupText))
			{
				if (builderLength != builder.Length) { builder.Append(",").Append(joinCommand.GroupText); }
				else { builder.AppendLine().Append(" GROUP BY ").Append(joinCommand.GroupText); }
			}
			if (string.IsNullOrWhiteSpace(HavingText) == false) { builder.AppendLine().Append(" HAVING ").Append(HavingText); }
			List<string> orderList = new List<string>(mOrderList.Count + 5);
			if (mOrderList.Count > 0) { orderList.AddRange(mOrderList.ToArray()); }
			else
			{
				if (joinCommand != null && !string.IsNullOrEmpty(joinCommand.OrderText))
					orderList.Add(joinCommand.OrderText);
				if (!string.IsNullOrEmpty(OrderText)) { orderList.Add(OrderText); }
			}

			if (orderList.Count > 0) { builder.Append(" ORDER BY ").Append(string.Join(",", orderList.ToArray())); }
			if (pageSize > 0) { builder.Append(" LIMIT 0,").Append(pageSize); }
			//builder.AppendFormat(";SELECT FOUND_ROWS() AS {0}", ReturnCountName);
		}

		/// <summary>初始化参数值</summary>
		/// <param name="dbParam">数据库参数</param>
		/// <param name="value">需要设置的数据库参数值</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0019:使用模式匹配", Justification = "<挂起>")]
		protected internal override void ResetParameterValue(DbParameter dbParam, object value)
		{
			MySqlParameter parameter = dbParam as MySqlParameter;
			if (parameter == null)
				throw new ArgumentException(string.Format(Strings.Access_InvalidArgument, "dbParam"), "dbParam");
			if (value == null) { parameter.Value = DBNull.Value; return; }
			if (parameter.MySqlDbType == MySqlDbType.String && value is int[])
			{
				parameter.Value = string.Join(",", (value as int[]).Cast<string>().ToArray());
				return;
			}
			parameter.Value = value;
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
			MySqlParameter sqlParameter = parameter as MySqlParameter;
			MySqlParameterConverter.ConvertSqlParameterType(sqlParameter, dbType, precision, scale);
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
			MySqlParameter sqlParameter = parameter as MySqlParameter;
			MySqlParameterConverter.ConvertSqlParameterType(sqlParameter, dbType, precision, scale);
		}

		/// <summary>
		/// 返回存储过程参数名称全名称
		/// </summary>
		/// <param name="parameterName">不带参数符号的参数名称</param>
		/// <returns>返回带存储过程符号的参数名称</returns>
		public override string CreateParameterName(string parameterName)
		{
			if (parameterName.StartsWith("@")) { return parameterName; }
			return string.Format("@{0}", parameterName);
		}

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <returns>一个 DbParameter 对象。</returns>
		public override DbParameter CreateParameter()
		{
			return _MySqlCommand.CreateParameter();
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
			MySqlParameter parameter = _MySqlCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Size = size;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			MySqlParameterConverter.ConvertSqlParameterType(parameter, dbType, 0, 0);
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
			MySqlParameter parameter = _MySqlCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Precision = precision;
			parameter.Scale = scale;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			MySqlParameterConverter.ConvertSqlParameterType(parameter, dbType, precision, scale);
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
			MySqlParameter parameter = _MySqlCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Size = size;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			MySqlParameterConverter.ConvertSqlParameterType(parameter, dbType, 0, 0);
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
			MySqlParameter parameter = _MySqlCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Precision = precision;
			parameter.Scale = scale;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			MySqlParameterConverter.ConvertSqlParameterType(parameter, dbType, precision, scale);
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
			MySqlParameter parameter = _MySqlCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Size = size;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			MySqlParameterConverter.ConvertSqlParameterType(parameter, dbType, 0, 0);
			_MySqlCommand.Parameters.Add(parameter);
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
			MySqlParameter parameter = _MySqlCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Precision = precision;
			parameter.Scale = scale;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			MySqlParameterConverter.ConvertSqlParameterType(parameter, dbType, precision, scale);
			_MySqlCommand.Parameters.Add(parameter);
			return parameter;
		}
		#endregion
		/// <summary>填充数据集</summary>
		/// <param name="table">待填充的实体类实例</param>
		/// <returns>执行Transact-SQL命令结果，返回受影响的记录数。</returns>
		internal protected override int Fill(DataTable table)
		{
			MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_MySqlCommand);
			return dataAdapter.Fill(table);
		}

		/// <summary>在 DataSet 中添加或刷新行。</summary>
		/// <param name="dataSet">要用记录和架构（如果必要）填充的 DataSet。</param>
		/// <returns>已在 DataSet 中成功添加或刷新的行数。 这不包括受不返回行的语句影响的行。 </returns>
		internal protected override int Fill(DataSet dataSet)
		{
			MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_MySqlCommand);
			return dataAdapter.Fill(dataSet);
		}

		/// <summary>
		/// 将当前命令的参数复制到指定的集合中
		/// </summary>
		/// <param name="parameters"></param>
		internal protected override void CopyParametersTo(ICollection<DbParameter> parameters)
		{
			foreach (MySqlParameter parameter in _MySqlCommand.Parameters)
			{
				parameters.Add((parameter as ICloneable).Clone() as MySqlParameter);
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
				if (joinCommand != null && joinCommand.Parameters.Length > 0)
				{
					foreach (MySqlParameter parameter in joinCommand.Parameters)
					{
						if (!Parameters.Contains(parameter.ParameterName))
						{
							Parameters.Add((parameter as ICloneable).Clone() as MySqlParameter);
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
		public MySqlDynamicCommand Clone()
		{
			lock (this)
			{
				MySqlDynamicCommand dynamicCommand = new MySqlDynamicCommand((MySqlCommand)_MySqlCommand.Clone());
				if (this.DbParameters != null && this.DbParameters.Count >= 0)
				{
					dynamicCommand.DbParameters.AddRange(DbParameters.Select(m =>
					{
						return (m as ICloneable).Clone() as MySqlParameter;
					}));
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
