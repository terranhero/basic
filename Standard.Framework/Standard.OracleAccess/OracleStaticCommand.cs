﻿using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using STT = System.Threading.Tasks;
using System.Linq;
using Basic.DataAccess;
using Basic.Enums;
using Oracle.ManagedDataAccess.Client;
using Basic.EntityLayer;

namespace Basic.OracleAccess
{
	/// <summary>
	/// 表示要对 SQL Server 数据库执行的一个静态结构的 Transact-SQL 语句或存储过程。
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.Xml.Serialization.XmlRoot(DataCommand.StaticCommandConfig)]
	internal sealed class OracleStaticCommand : StaticCommand, IDisposable, ICloneable
	{
		private readonly OracleCommand oracleCommand;
		/// <summary>
		/// 初始化 SqlDynamicCommand 类的新实例。 
		/// </summary>
		public OracleStaticCommand() : base(new OracleCommand()) { oracleCommand = dataDbCommand as OracleCommand; }

		/// <summary>
		/// 根据数据库命令，初始化 SqlDynamicCommand 类的新实例，主要克隆实例时使用。
		/// </summary>
		/// <param name="dbCommand">表示 OracleCommand 类实例。</param>
		private OracleStaticCommand(OracleCommand dbCommand) : base(dbCommand) { oracleCommand = dbCommand; }

		/// <summary>当前命令的数据库类型</summary>
		public override ConnectionType ConnectionType { get { return ConnectionType.OracleConnection; } }

		private OracleCheckCommandCollection _CheckCommands = null;
		/// <summary>
		/// 检测命令集合
		/// </summary>
		internal protected override CheckCommandCollection CheckCommands
		{
			get
			{
				if (_CheckCommands == null)
					_CheckCommands = new OracleCheckCommandCollection(this);
				return _CheckCommands;
			}
		}

		private OracleNewValueCommandCollection _NewValues = null;
		/// <summary>
		/// 新值命令集合
		/// </summary>
		internal protected override NewValueCommandCollection NewValues
		{
			get
			{
				if (_NewValues == null)
					_NewValues = new OracleNewValueCommandCollection(this);
				return _NewValues;
			}
		}

		/// <summary>
		/// 重置数据库连接
		/// </summary>
		internal OracleConnection Connection { get { return oracleCommand.Connection; } }

		/// <summary>创建批处理命令</summary>
		/// <returns>返回 SqlServerBatchCommand 的实例</returns>
		protected internal override BatchCommand CreateBatchCommand() { return new OracleBatchCommand(this); }

		/// <summary>
		/// 重置数据库连接
		/// </summary>
		/// <param name="connection">一个 DbConnection，它表示到关系数据库实例的连接。 </param>
		internal void ResetConnection(OracleConnection connection)
		{
			oracleCommand.Connection = connection;
		}

		/// <summary>
		/// 针对 .NET Framework 数据提供程序的 Connection 对象执行 SQL 语句，并返回受影响的行数。
		/// </summary>
		/// <returns>受影响的行数。</returns>
		internal protected override STT.Task<int> ExecuteNonQueryAsync()
		{
			return oracleCommand.ExecuteNonQueryAsync();
		}

		/// <summary>
		/// 针对 Connection 执行 CommandText，并生成 DbDataReader。 
		/// </summary>
		/// <returns>一个 DbDataReader 对象。 </returns>
		internal protected override STT.Task<DbDataReader> ExecuteReaderAsync()
		{
			return oracleCommand.ExecuteReaderAsync().ContinueWith<DbDataReader>((reader) => { return reader.Result; });
		}

		/// <summary>
		/// 针对 Connection 执行 CommandText，并使用 CommandBehavior 值之一返回 DbDataReader。 
		/// </summary>
		/// <param name="behavior">类型： System.Data.CommandBehavior，CommandBehavior 值之一。 </param>
		/// <returns>一个 DbDataReader 对象。 </returns>
		internal protected override STT.Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior)
		{
			return oracleCommand.ExecuteReaderAsync().ContinueWith<DbDataReader>((reader) => { return reader.Result; });
		}

		/// <summary>
		/// 将 CommandText 发送到 Connection 并生成一个 SqlDataReader。
		/// </summary>
		/// <remarks>
		/// 当 CommandType 属性设置为 StoredProcedure 时， CommandText 属性应设置为存储过程的名称。 
		/// 当调用 ExecuteReader 时，该命令将执行此存储过程。 
		/// </remarks>
		/// <returns>一个 SqlDataReader 对象。</returns>
		internal protected override DbDataReader ExecuteReader()
		{
			return oracleCommand.ExecuteReader();
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
		internal protected override DbDataReader ExecuteReader(CommandBehavior behavior)
		{
			return oracleCommand.ExecuteReader(behavior);
		}

		/// <summary></summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		protected internal override bool ResetParameters(AbstractEntity[] entities)
		{
			//string connectStr = "User Id=scott;Password=tiger;Data Source=";
			//OracleConnection conn = new OracleConnection(connectStr);
			//OracleCommand command = new OracleCommand();
			//command.Connection = conn;
			////到此为止，还都是我们熟悉的代码，下面就要开始喽
			////这个参数需要指定每次批插入的记录数
			//command.ArrayBindCount = recc;
			////在这个命令行中,用到了参数,参数我们很熟悉,但是这个参数在传值的时候
			////用到的是数组,而不是单个的值,这就是它独特的地方
			//command.CommandText = "insert into dept values(:deptno, :deptname, :loc)";
			//conn.Open();
			////下面定义几个数组,分别表示三个字段,数组的长度由参数直接给出
			//int[] deptNo = new int[recc];
			//string[] dname = new string[recc];
			//string[] loc = new string[recc];
			//// 为了传递参数,不可避免的要使用参数,下面会连续定义三个
			//// 从名称可以直接看出每个参数的含义,不在每个解释了
			//OracleParameter deptNoParam = new OracleParameter("deptno", OracleDbType.Int32);
			//deptNoParam.Direction = ParameterDirection.Input;
			//deptNoParam.Value = deptNo;
			//command.Parameters.Add(deptNoParam);
			//OracleParameter deptNameParam = new OracleParameter("deptname", OracleDbType.Varchar2);
			//deptNameParam.Direction = ParameterDirection.Input;
			//deptNameParam.Value = dname; command.Parameters.Add(deptNameParam);
			//OracleParameter deptLocParam = new OracleParameter("loc", OracleDbType.Varchar2);
			//deptLocParam.Direction = ParameterDirection.Input;
			//deptLocParam.Value = loc;
			//command.Parameters.Add(deptLocParam);
			////在下面的循环中,先把数组定义好,而不是像上面那样直接生成SQL
			//for (int i = 0; i < recc; i++)
			//{
			//	deptNo[i] = i;
			//	dname[i] = i.ToString();
			//	loc[i] = i.ToString();
			//}
			////这个调用将把参数数组传进SQL,同时写入数据库
			//command.ExecuteNonQuery();
			return base.ResetParameters(entities);
		}

		/// <summary>
		/// 初始化参数值
		/// </summary>
		/// <param name="dbParam">数据库参数</param>
		/// <param name="value">需要设置的数据库参数值</param>
		internal protected override void ResetParameterValue(DbParameter dbParam, object value)
		{
			if (dbParam is OracleParameter parameter)
			{
				if (value == null) { parameter.Value = DBNull.Value; return; }
				if (parameter.OracleDbType == OracleDbType.NVarchar2 && value is int[])
				{
					parameter.Value = string.Join(",", (value as int[]));
					return;
				}
				parameter.Value = value; return;
			}
			throw new ArgumentException("参数错误", "dbParam");
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <param name="table">待填充的实体类实例</param>
		/// <returns>执行Transact-SQL命令结果，已在 System.Data.DataTable 中成功添加或刷新的行数。</returns>
		internal protected override int Fill(DataTable table)
		{
			using (OracleDataAdapter dataAdapter = new OracleDataAdapter(oracleCommand))
			{
				return dataAdapter.Fill(table);
			}
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <param name="dataSet">待填充的数据集</param>
		/// <returns>执行Transact-SQL命令结果，已在 System.Data.DataSet 中成功添加或刷新的行数。</returns>
		internal protected override int Fill(System.Data.DataSet dataSet)
		{
			using (OracleDataAdapter dataAdapter = new OracleDataAdapter(oracleCommand))
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
			OracleParameter sqlParameter = parameter as OracleParameter;
			ParameterConverter.ConvertParameterType(sqlParameter, dbType, precision, scale);
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
			OracleParameter sqlParameter = parameter as OracleParameter;
			ParameterConverter.ConvertParameterType(sqlParameter, dbType, precision, scale);
		}

		/// <summary>
		/// 返回存储过程参数名称全名称
		/// </summary>
		/// <param name="parameterName">不带参数符号的参数名称</param>
		/// <returns>返回带存储过程符号的参数名称</returns>
		public override string CreateParameterName(string parameterName)
		{
			if (parameterName.StartsWith("@"))
				return parameterName;
			return string.Concat("@", parameterName);
		}

		/// <summary>
		/// 返回存储过程参数名称全名称
		/// </summary>
		/// <param name="parameterName">不带参数符号的参数名称</param>
		/// <returns>返回带存储过程符号的参数名称</returns>
		internal string CreateSourceName(string parameterName)
		{
			if (parameterName.StartsWith("@"))
				return parameterName.TrimStart('@');
			return parameterName;
		}

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <returns>一个 DbParameter 对象。</returns>
		public override DbParameter CreateParameter()
		{
			return oracleCommand.CreateParameter();
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
			OracleParameter parameter = oracleCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Size = size;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			ParameterConverter.ConvertParameterType(parameter, dbType, 0, 0);
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
			OracleParameter parameter = oracleCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Precision = precision;
			parameter.Scale = scale;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			ParameterConverter.ConvertParameterType(parameter, dbType, precision, scale);
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
			OracleParameter parameter = oracleCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Size = size;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			ParameterConverter.ConvertParameterType(parameter, dbType, 0, 0);
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
			OracleParameter parameter = oracleCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Precision = precision;
			parameter.Scale = scale;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			ParameterConverter.ConvertParameterType(parameter, dbType, precision, scale);
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
			OracleParameter parameter = oracleCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Size = size;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			ParameterConverter.ConvertParameterType(parameter, dbType, 0, 0);
			oracleCommand.Parameters.Add(parameter);
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
			OracleParameter parameter = oracleCommand.CreateParameter();
			parameter.ParameterName = CreateParameterName(parameterName);
			parameter.SourceColumn = sourceColumn;
			parameter.Precision = precision;
			parameter.Scale = scale;
			parameter.Direction = direction;
			parameter.IsNullable = isNullable;
			ParameterConverter.ConvertParameterType(parameter, dbType, precision, scale);
			oracleCommand.Parameters.Add(parameter);
			return parameter;
		}

		#endregion

		/// <summary>
		/// 创建检查数据命令
		/// </summary>
		/// <returns>返回继承与 ICheckCommand 的实例</returns>
		internal protected override CheckCommand CreateCheckCommand()
		{
			return new OracleCheckCommand(this);
		}

		/// <summary>
		/// 创建检查数据命令
		/// </summary>
		/// <returns>返回 SqlNewValueCommand 的实例</returns>
		internal protected override NewValueCommand CreateNewValueCommand()
		{
			return new OracleNewValueCommand(this);
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
		public OracleStaticCommand Clone()
		{
			lock (this)
			{
				OracleCommand command = oracleCommand.Clone() as OracleCommand;
				OracleStaticCommand staticCommand = new OracleStaticCommand(command);
				CopyTo(staticCommand);
				if (_CheckCommands != null && _CheckCommands.Count > 0)
				{
					foreach (OracleCheckCommand checkCommand in _CheckCommands)
					{
						staticCommand.CheckCommands.Add(checkCommand.Clone(staticCommand));
					}
				}
				if (_NewValues != null && _NewValues.Count > 0)
				{
					foreach (OracleNewValueCommand newValueCommand in _NewValues)
					{
						staticCommand.NewValues.Add(newValueCommand.Clone(staticCommand));
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
