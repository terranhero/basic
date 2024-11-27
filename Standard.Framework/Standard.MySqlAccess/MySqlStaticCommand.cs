using Basic.DataAccess;
using Basic.Properties;
using Basic.Enums;
using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using STT = System.Threading.Tasks;

namespace Basic.MySqlAccess
{
	/// <summary>
	/// 表示要对 SQL Server 数据库执行的一个静态结构的 Transact-SQL 语句或存储过程。
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.Xml.Serialization.XmlRoot(DataCommand.StaticCommandConfig)]
	internal sealed class MySqlStaticCommand : StaticCommand, IDisposable, ICloneable
	{
		private readonly MySqlCommand _MySqlCommand;
		private readonly MySqlCheckCommandCollection _CheckCommands = null;
		private readonly MySqlNewValueCommandCollection _NewValues = null;
		/// <summary>
		/// 初始化 MySqlStaticCommand 类的新实例。 
		/// </summary>
		public MySqlStaticCommand() : this(new MySqlCommand()) { }

		/// <summary>
		/// 根据数据库命令，初始化 MySqlStaticCommand 类的新实例，主要克隆实例时使用。
		/// </summary>
		/// <param name="dbCommand">表示 MySqlCommand 类实例。</param>
		private MySqlStaticCommand(MySqlCommand dbCommand)
			: base(dbCommand)
		{
			_MySqlCommand = dbCommand;
			_CheckCommands = new MySqlCheckCommandCollection(this);
			_NewValues = new MySqlNewValueCommandCollection(this);
		}

		/// <summary>当前命令的数据库类型</summary>
		public override ConnectionType ConnectionType { get { return ConnectionType.MySqlConnection; } }

		/// <summary>
		/// 针对 .NET Framework 数据提供程序的 Connection 对象执行 SQL 语句，并返回受影响的行数。
		/// </summary>
		/// <returns>受影响的行数。</returns>
		internal protected override STT.Task<int> ExecuteNonQueryAsync()
		{
			return _MySqlCommand.ExecuteNonQueryAsync();
		}

		/// <summary>
		/// 针对 Connection 执行 CommandText，并生成 DbDataReader。 
		/// </summary>
		/// <returns>一个 DbDataReader 对象。 </returns>
		internal protected override STT.Task<DbDataReader> ExecuteReaderAsync()
		{
			return _MySqlCommand.ExecuteReaderAsync().ContinueWith<DbDataReader>((reader) => { return reader.Result; });
		}

		/// <summary>
		/// 针对 Connection 执行 CommandText，并使用 CommandBehavior 值之一返回 DbDataReader。 
		/// </summary>
		/// <param name="behavior">类型： System.Data.CommandBehavior，CommandBehavior 值之一。 </param>
		/// <returns>一个 DbDataReader 对象。 </returns>
		internal protected override STT.Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior)
		{
			return _MySqlCommand.ExecuteReaderAsync(behavior).ContinueWith<DbDataReader>((reader) => { return reader.Result; });
		}

		/// <summary>
		/// 检测命令集合
		/// </summary>
		internal protected override CheckCommandCollection CheckCommands { get { return _CheckCommands; } }

		/// <summary>
		/// 新值命令集合
		/// </summary>
		internal protected override NewValueCommandCollection NewValues { get { return _NewValues; } }

		/// <summary>
		/// 获取数据库连接
		/// </summary>
		internal MySqlConnection Connection { get { return _MySqlCommand.Connection; } }

		/// <summary>
		/// 获取 MySqlCommand 类实例。
		/// </summary>
		internal MySqlCommand MySqlCommand { get { return _MySqlCommand; } }

		/// <summary>
		/// 重置数据库连接
		/// </summary>
		/// <param name="connection">一个 DbConnection，它表示到关系数据库实例的连接。 </param>
		public void ResetConnection(MySqlConnection connection)
		{
			_MySqlCommand.Connection = connection;
		}

		/// <summary>
		/// 将 CommandText 发送到 Connection 并生成一个 SqlDataReader。
		/// </summary>
		/// <remarks>
		/// 当 CommandType 属性设置为 StoredProcedure 时， CommandText 属性应设置为存储过程的名称。 
		/// 当调用 ExecuteReader 时，该命令将执行此存储过程。 
		/// </remarks>
		/// <returns>一个 SqlDataReader 对象。</returns>
		public new MySqlDataReader ExecuteReader()
		{
			return _MySqlCommand.ExecuteReader();
		}

		/// <summary>
		/// 将 CommandText 发送到 Connection，并使用 CommandBehavior 值之一生成一个 SqlDataReader。 
		/// </summary>
		/// <param name="behavior">CommandBehavior 值之一。 </param>
		/// <remarks>
		/// 当 CommandType 属性设置为 StoredProcedure 时， CommandText 属性应设置为存储过程的名称。 
		/// 当调用 ExecuteReader 时，该命令将执行此存储过程。
		/// </remarks>
		/// <returns>一个 SqlDataReader 对象。</returns>
		public new MySqlDataReader ExecuteReader(CommandBehavior behavior)
		{
			return _MySqlCommand.ExecuteReader(behavior);
		}

		/// <summary>
		/// 初始化参数值
		/// </summary>
		/// <param name="dbParam">数据库参数</param>
		/// <param name="value">需要设置的数据库参数值</param>
		protected internal override void ResetParameterValue(DbParameter dbParam, object value)
		{
			MySqlParameter parameter = dbParam as MySqlParameter;
			if (parameter == null)
				throw new ArgumentException(string.Format(Strings.Access_InvalidArgument, "dbParam"), "dbParam");
			if (value == null)
			{
				parameter.Value = DBNull.Value;
				return;
			}
			if (parameter.MySqlDbType == MySqlDbType.String && value is int[])
			{
				parameter.Value = string.Join(",", (value as int[]));
				return;
			}
			parameter.Value = value;
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <param name="table">待填充的实体类实例</param>
		/// <returns>执行Transact-SQL命令结果，返回受影响的记录数。</returns>
		internal protected override int Fill(DataTable table)
		{
			using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_MySqlCommand))
			{
				return dataAdapter.Fill(table);
			}
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <param name="dataSet">待填充的数据集</param>
		/// <returns>执行Transact-SQL命令结果，返回受影响的记录数。</returns>
		internal protected override int Fill(System.Data.DataSet dataSet)
		{
			using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_MySqlCommand))
			{
				return dataAdapter.Fill(dataSet);
			}
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
			return string.Concat("@", parameterName);
		}

		/// <summary>
		/// 返回存储过程参数名称全名称
		/// </summary>
		/// <param name="parameterName">不带参数符号的参数名称</param>
		/// <returns>返回带存储过程符号的参数名称</returns>
		public string CreateSourceName(string parameterName)
		{
			if (parameterName.StartsWith("@")) { return parameterName.TrimStart('@'); }
			return parameterName;
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

		/// <summary>
		/// 创建检查数据命令
		/// </summary>
		/// <returns>返回继承与 ICheckCommand 的实例</returns>
		internal protected override CheckCommand CreateCheckCommand()
		{
			return new MySqlCheckCommand(this);
		}

		/// <summary>
		/// 创建检查数据命令
		/// </summary>
		/// <returns>返回 SqlNewValueCommand 的实例</returns>
		internal protected override NewValueCommand CreateNewValueCommand()
		{
			return new MySqlNewValueCommand(this);
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
		public MySqlStaticCommand Clone()
		{
			lock (this)
			{
				MySqlCommand command = (MySqlCommand)_MySqlCommand.Clone();
				MySqlStaticCommand staticCommand = new MySqlStaticCommand(command)
				{
					SourceColumn = SourceColumn,
					CommandName = CommandName
				};
				if (_CheckCommands != null && _CheckCommands.Count > 0)
				{
					foreach (MySqlCheckCommand checkCommand in _CheckCommands)
					{
						staticCommand.CheckCommands.Add(checkCommand.Clone(staticCommand));
					}
				}
				if (_NewValues != null && _NewValues.Count > 0)
				{
					foreach (MySqlNewValueCommand newCommand in _NewValues)
					{
						staticCommand.NewValues.Add(newCommand.Clone(staticCommand));
					}
				}
				return staticCommand;
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
