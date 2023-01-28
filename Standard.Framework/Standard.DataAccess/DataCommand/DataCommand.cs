using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Xml.Serialization;
using Basic.Enums;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示要对数据源执行的 SQL 语句或存储过程。为表示命令的、数据库特有的类提供一个基类。
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.ComponentModel.ToolboxItem(false)]
	public abstract class DataCommand : AbstractDataCommand, IDbCommand, IXmlSerializable//, IDataCommand
	{
		/// <summary>
		/// 结构配置节名称
		/// </summary>
		internal protected const string CommandConfig = "CommandConfig";

		/// <summary>
		/// 静态命令结构配置节名称
		/// </summary>
		internal protected const string StaticCommandConfig = "StaticCommandConfig";

		/// <summary>
		/// 动态命令结构配置节名称
		/// </summary>
		internal protected const string DynamicCommandConfig = "DynamicCommandConfig";

		/// <summary>
		/// 时间戳列编程名称
		/// </summary>
		internal protected const string TimeStampColumn = "DateTime.Now";

		/// <summary>
		/// 数据库表中所有列配置节名称
		/// </summary>
		internal protected const string ParametersConfig = "Parameters";

		/// <summary>
		/// 数据库表中所有列配置节名称
		/// </summary>
		internal protected const string ParameterConfig = "Parameter";

		/// <summary>
		/// 数据库表中列的源列的配置节名称
		/// </summary>
		internal protected const string Parameter_SourceColumn = "SourceColumn";

		/// <summary>
		/// 数据库表中列的源列的配置节名称
		/// </summary>
		internal protected const string Parameter_Column = "Column";

		/// <summary>
		/// 数据库表中列类型配置节名称
		/// </summary>
		internal protected const string Parameter_DbType = "DbType";

		/// <summary>
		/// 数据库表中列类型长度配置节名称
		/// </summary>
		internal protected const string Parameter_Length = "Length";

		/// <summary>
		/// 数据库表中列类型长度配置节名称
		/// </summary>
		internal protected const string Parameter_Size = "Size";

		/// <summary>
		/// 数据库表中列类型长度配置节名称
		/// </summary>
		internal protected const string Parameter_Precision = "Precision";

		/// <summary>
		/// 数据库表中列类型精度配置节名称
		/// </summary>
		internal protected const string Parameter_Scale = "Scale";

		/// <summary>
		/// 数据库表中列是否允许为空配置节名称
		/// </summary>
		internal protected const string Parameter_Nullable = "Nullable";

		/// <summary>
		/// 数据库表中列是否允许为空配置节名称
		/// </summary>
		internal protected const string Parameter_Direction = "Direction";

		/// <summary>
		/// 命名名称属性关键字
		/// </summary>
		internal protected const string CommandNameAttribute = "Name";

		///// <summary>
		///// 当前命令是否正在使用
		///// </summary>
		//internal protected bool Executing { get { return dataDbCommand != null && dataDbCommand.Connection != null; } }

		/// <summary>
		/// 初始化 DataCommand 类的新实例。 
		/// </summary>
		protected DataCommand(DbCommand command)
			: base(command)
		{
			ConfigurationType = ConfigurationTypeEnum.Other;
			AllowWriteLog = true;
		}

		#region 命令属性
		/// <summary>
		/// 当前命令的副本名称
		/// </summary>
		protected internal string CloneCommandName { get; set; }

		/// <summary>
		/// 获取或设置要当前命令结构的类型。
		/// </summary>
		/// <value>枚举 ConfigurationTypeEnum 的值之一。</value>
		protected internal ConfigurationTypeEnum ConfigurationType { get; internal set; }

		/// <summary>
		/// 获取或设置当前命令是否允许记录日志。
		/// </summary>
		/// <value>一个布尔值，该值指示当前命令是否允许进行日志记录，默认值为True。</value>
		protected internal bool AllowWriteLog { get; internal set; }

		/// <summary>
		/// 创建作为当前实例副本的新对象。
		/// </summary>
		/// <returns>作为此实例副本的新对象。</returns>
		protected internal abstract DataCommand CloneCommand();

		/// <summary>
		/// 获取或设置要对数据源执行的 DataCommand 名称。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程在配置文件中的唯一名称。</value>
		protected internal virtual string CommandName { get; set; }

		/// <summary>
		/// 获取或设置源列的名称，该源列映射到 DataSet 并用于加载或返回 Value
		/// </summary>
		/// <value>映射到 DataSet /DataTable/Entity的源列(属性)的名称。默认值为空字符串。</value>
		protected internal virtual string SourceColumn { get; set; }
		#endregion

		#region 执行数据库命令
		/// <summary>
		/// 重置数据库连接
		/// </summary>
		/// <param name="connection">一个 DbConnection，它表示到关系数据库实例的连接。 </param>
		internal protected virtual void ResetConnection(DbConnection connection)
		{
			dataDbCommand.Connection = connection;
		}

		/// <summary>
		/// 释放数据库连接
		/// </summary>
		internal protected virtual void ReleaseConnection()
		{
			//if (dataDbCommand.Connection != null && dataDbCommand.Connection.State != ConnectionState.Closed)
			//    dataDbCommand.Connection.Close();
			dataDbCommand.Connection = null;
		}

		/// <summary>
		/// 尝试取消 SqlCommand 的执行。 
		/// </summary>
		internal protected virtual void Cancel()
		{
			dataDbCommand.Cancel();
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <returns>结果集中第一行的第一列。</returns>
		internal protected virtual object ExecuteScalar()
		{
			return dataDbCommand.ExecuteScalar();
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <returns>结果集中第一行的第一列。</returns>
		internal protected virtual System.Threading.Tasks.Task<object> ExecuteScalarAsync()
		{
			return dataDbCommand.ExecuteScalarAsync();
		}

		/// <summary>
		/// 针对 Connection 执行 CommandText，并生成 DbDataReader。 
		/// </summary>
		/// <returns>一个 DbDataReader 对象。 </returns>
		internal protected abstract System.Threading.Tasks.Task<DbDataReader> ExecuteReaderAsync();

		/// <summary>
		/// 针对 Connection 执行 CommandText，并使用 CommandBehavior 值之一返回 DbDataReader。 
		/// </summary>
		/// <param name="behavior">类型： System.Data.CommandBehavior，CommandBehavior 值之一。 </param>
		/// <returns>一个 DbDataReader 对象。 </returns>
		internal protected abstract System.Threading.Tasks.Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior);

		/// <summary>
		/// 针对 Connection 执行 CommandText，并生成 DbDataReader。 
		/// </summary>
		/// <returns>一个 DbDataReader 对象。 </returns>
		internal protected virtual DbDataReader ExecuteReader()
		{
			return dataDbCommand.ExecuteReader();
		}

		/// <summary>
		/// 针对 Connection 执行 CommandText，并使用 CommandBehavior 值之一返回 DbDataReader。 
		/// </summary>
		/// <param name="behavior">类型： System.Data.CommandBehavior，CommandBehavior 值之一。 </param>
		/// <returns>一个 DbDataReader 对象。 </returns>
		internal protected virtual DbDataReader ExecuteReader(CommandBehavior behavior)
		{
			return dataDbCommand.ExecuteReader(behavior);
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <param name="table">待填充的实体类实例</param>
		/// <returns>执行Transact-SQL命令结果，返回受影响的记录数。</returns>
		internal protected abstract int Fill(DataTable table);

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <param name="dataSet">待填充的实体类实例</param>
		/// <returns>执行Transact-SQL命令结果，返回受影响的记录数。</returns>
		internal protected abstract int Fill(System.Data.DataSet dataSet);

		/// <summary>
		/// 释放由 此成员 占用的资源。 
		/// </summary>
		internal protected new void Dispose()
		{
			ReleaseConnection();
		}
		#endregion

		#region 创建数据库参数
		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="parameter">数据库命令执行的参数</param>
		/// <param name="dbType">SqlServer数据库列类型,SqlDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		internal protected abstract void ConvertParameterType(DbParameter parameter, DataTypeEnum dbType, byte precision, byte scale);

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		public abstract DbParameter CreateParameter();

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="size">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		internal protected abstract DbParameter CreateParameter(string parameterName, string sourceColumn, DataTypeEnum dbType, int size,
			ParameterDirection direction, bool isNullable);

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
		internal protected abstract DbParameter CreateParameter(string parameterName, string sourceColumn, DataTypeEnum dbType,
			byte precision, byte scale, ParameterDirection direction, bool isNullable);

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public abstract DbParameter CreateParameter(string parameterName, string sourceColumn, DbTypeEnum dbType, bool isNullable);

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="size">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public abstract DbParameter CreateParameter(string parameterName, string sourceColumn, DbTypeEnum dbType, int size, bool isNullable);

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="size">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public abstract DbParameter CreateParameter(string parameterName, string sourceColumn, DbTypeEnum dbType, int size,
			ParameterDirection direction, bool isNullable);

		/// <summary>
		/// 创建数据库命令参数
		/// </summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public abstract DbParameter CreateParameter(string parameterName, string sourceColumn, DbTypeEnum dbType,
			byte precision, byte scale, bool isNullable);

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
		public abstract DbParameter CreateParameter(string parameterName, string sourceColumn, DbTypeEnum dbType,
			byte precision, byte scale, ParameterDirection direction, bool isNullable);

		/// <summary>创建数据库命令参数并添加到参数集合中</summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public abstract DbParameter CreateAddParameter(string parameterName, string sourceColumn, DbTypeEnum dbType, bool isNullable);

		/// <summary>创建数据库命令参数并添加到参数集合中</summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="size">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public abstract DbParameter CreateAddParameter(string parameterName, string sourceColumn, DbTypeEnum dbType, int size, bool isNullable);

		/// <summary>创建数据库命令参数并添加到参数集合中</summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="size">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public abstract DbParameter CreateAddParameter(string parameterName, string sourceColumn, DbTypeEnum dbType, int size,
			ParameterDirection direction, bool isNullable);

		/// <summary>创建数据库命令参数并添加到参数集合中</summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public abstract DbParameter CreateAddParameter(string parameterName, string sourceColumn, DbTypeEnum dbType,
			byte precision, byte scale, bool isNullable);

		/// <summary>创建数据库命令参数并添加到参数集合中</summary>
		/// <param name="parameterName">获取或设置数据库参数的名称。</param>
		/// <param name="sourceColumn">获取或设置源列的名称。</param>
		/// <param name="dbType">参数的数据库类型,DataTypeEnum枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		/// <param name="direction">获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
		/// <param name="isNullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public abstract DbParameter CreateAddParameter(string parameterName, string sourceColumn, DbTypeEnum dbType,
			byte precision, byte scale, ParameterDirection direction, bool isNullable);
		#endregion

		#region 处理数据库参数返回值
		/// <summary>
		/// 处理命令输出参数结果
		/// </summary>
		/// <param name="returnCount">存储过程中返回记录总数</param>
		/// <param name="returnValue">存储过程返回值</param>
		internal protected virtual void ProcessOutputParamater(out int returnCount, out int returnValue)
		{
			string returnCountName = CreateParameterName(ReturnCountName);
			returnCount = 0; returnValue = 0;
			foreach (DbParameter parameter in dataDbCommand.Parameters)
			{
				//是否输出参数
				bool outputParameter = parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput;
				if (!outputParameter && parameter.Direction != ParameterDirection.ReturnValue)
					continue;
				if (parameter.Direction == ParameterDirection.ReturnValue)
				{
					if (parameter.Value != DBNull.Value && parameter.Value != null)
						returnValue = (int)(parameter.Value);
				}
				else if (outputParameter && parameter.ParameterName == returnCountName)
				{
					if (parameter.Value != DBNull.Value && parameter.Value != null)
						returnCount = (int)(parameter.Value);
				}
			}
		}
		#endregion

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		internal protected override bool ReadAttribute(string name, string value)
		{
			if (name == DataCommand.ReturnColumnConfig || name == DataCommand.SourceColumnConfig)
			{
				SourceColumn = value; return true;
			}
			else if (name == DataCommand.CommandTimeoutConfig)
			{
				CommandTimeout = Convert.ToInt32(value); return true;
			}
			else if (name == DataCommand.KindConfig)
			{
#if (NET35)
                if (Enum.IsDefined(typeof(ConfigurationTypeEnum), value))
                {
                    ConfigurationType = (ConfigurationTypeEnum)Enum.Parse(typeof(ConfigurationTypeEnum), value, true);
                }
#else
				ConfigurationTypeEnum configType = ConfigurationTypeEnum.Other;
				if (Enum.TryParse<ConfigurationTypeEnum>(value, out configType))
					ConfigurationType = configType;
#endif
				return true;
			}
			else if (name == CommandNameAttribute)
			{
				CommandName = value; return true;
			}
			return base.ReadAttribute(name, value);
		}
	}
}
