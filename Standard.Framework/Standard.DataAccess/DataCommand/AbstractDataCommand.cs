using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Tables;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示数据库命令抽象类，此类仅有基础反序列化功能。
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.ComponentModel.ToolboxItem(false)]
	public abstract class AbstractDataCommand : Component, IDbCommand, IXmlSerializable, INotifyPropertyChanged
	{
		#region Xml 节点名称常量
		/// <summary>
		/// 常量：存储过程结果记录数输出参数名称
		/// </summary>
		internal protected const string ReturnCountName = "RETURNCOUNT";

		/// <summary>
		/// 常量：分页使用的页面大小字段名称
		/// </summary>
		internal protected const string GoldSoftPageSize = "PAGESIZE";

		/// <summary>
		/// 常量：分页使用的页面索引字段名称
		/// </summary>
		internal protected const string GoldSoftPageIndex = "PAGEINDEX";

		/// <summary>
		/// 数据库表中所有列配置节名称
		/// </summary>
		internal protected const string CheckCommandsConfig = "CheckCommands";

		/// <summary>
		/// 数据库表中所有列配置节名称
		/// </summary>
		internal protected const string CheckCommandConfig = "CheckCommand";

		/// <summary>
		/// 命令类型属性名
		/// </summary>
		internal protected const string CommandTypeConfig = "CommandType";

		/// <summary>
		/// 是否自动生成命令属性名
		/// </summary>
		internal protected const string AutoGenerationConfig = "AutoGeneration";

		/// <summary>
		/// 是否自动生成命令属性名
		/// </summary>
		internal protected const string DynamicParameterConfig = "DynamicParameter";

		/// <summary>
		/// 获取的新值填入的字段名称的属性名(一般用于NewKeyConfig中)
		/// </summary>
		internal protected const string ReturnColumnConfig = "ReturnColumn";

		/// <summary>
		/// 常量：存储过程返回值参数名称
		/// </summary>
		internal protected const string ReturnValueName = "ReturnValue";

		/// <summary>
		/// 获取的新值填入的字段名称的属性名(一般用于CheckCommand中)
		/// </summary>
		internal protected const string ErrorCodeConfig = "ErrorCode";

		/// <summary>
		/// 获取的新值填入的字段名称的属性名(一般用于NewKeyConfig中)
		/// </summary>
		internal protected const string SourceColumnConfig = "SourceColumn";

		/// <summary>
		/// 获取或设置在终止执行命令的尝试并生成错误之前的等待时间（属性名）。
		/// </summary>
		internal protected const string CommandTimeoutConfig = "CommandTimeout";

		/// <summary>
		/// 获取或设置命令类型（属性名）。
		/// </summary>
		internal protected const string KindConfig = "Kind";

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程（元素名）。
		/// </summary>
		internal protected const string CommandTextConfig = "CommandText";

		/// <summary>
		/// 表示 CommandType 属性
		/// </summary>
		internal protected const string CommandTypeAttribute = "CommandType";

		/// <summary>
		/// 表示 CommandTimeout 属性
		/// </summary>
		internal protected const string CommandTimeoutAttribute = "CommandTimeout";

		/// <summary>
		/// 表示 Parameters 元素。
		/// </summary>
		internal protected const string ParametersElement = "Parameters";
		#endregion

		/// <summary>当前命令的数据库类型</summary>
		public abstract ConnectionType ConnectionType { get; }

		/// <summary>
		/// 表示要对数据源执行的 SQL 语句或存储过程。为表示命令的、数据库特有的类提供一个基类。
		/// </summary>
		internal readonly DbCommand dataDbCommand;

		/// <summary>
		/// 表示要对数据源执行的 SQL 语句或存储过程。为表示命令的、数据库特有的类提供一个基类。
		/// </summary>
		internal protected DbCommand DbCommand { get { return dataDbCommand; } }

		/// <summary>
		/// 初始化 AbstractDataCommand 类实例
		/// </summary>
		/// <param name="command">包含此命令的 DbCommand 子类实例。</param>
		protected AbstractDataCommand(DbCommand command) { dataDbCommand = command; }

		#region 初始化数据库参数
		/// <summary>
		/// 设置分页参数
		/// </summary>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		internal protected virtual void ResetPaginationParameter(int pageSize, int pageIndex)
		{
			if (dataDbCommand.CommandType == CommandType.StoredProcedure)
			{
				string pageSizeParameterName = CreateParameterName(GoldSoftPageSize);
				string pageIndexParameterName = CreateParameterName(GoldSoftPageIndex);
				if (dataDbCommand.Parameters.Contains(pageSizeParameterName))
					dataDbCommand.Parameters[pageSizeParameterName].Value = pageSize;
				if (dataDbCommand.Parameters.Contains(pageIndexParameterName))
					dataDbCommand.Parameters[pageIndexParameterName].Value = pageIndex;
			}
		}

		/// <summary>
		/// 初始化参数值
		/// </summary>
		/// <param name="dbParam">数据库参数</param>
		/// <param name="value">需要设置的数据库参数值</param>
		internal protected virtual void ResetParameterValue(DbParameter dbParam, object value)
		{
			if (value == null) { dbParam.Value = DBNull.Value; }
			else { dbParam.Value = value; }
		}

		/// <summary>
		/// 初始化Transact-SQL命令执行参数值
		/// </summary>
		internal protected virtual void ResetParameters()
		{
			foreach (DbParameter dbParam in dataDbCommand.Parameters)
			{
				dbParam.Value = DBNull.Value;
			}
		}

		/// <summary>
		/// 初始化Transact-SQL命令执行参数值
		/// </summary>
		/// <param name="row">键/值对数组，包含了需要执行参数的值。</param>
		internal protected virtual void ResetParameters(BaseTableRowType row)
		{
			if (row == null) { throw new ArgumentNullException("row"); }
			foreach (DbParameter dbParam in Parameters)
			{
				if (dbParam.Direction == ParameterDirection.Output) { continue; }
				if (row.ContainsKey(dbParam.SourceColumn) && row[dbParam.SourceColumn] != null)
				{
					ResetParameterValue(dbParam, row[dbParam.SourceColumn]);
				}
				else
				{
					ResetParameterValue(dbParam, DBNull.Value);
				}
			}
		}

		/// <summary>
		/// 初始化Transact-SQL命令执行参数值
		/// </summary>
		/// <param name="entity">数据实体类。</param>
		internal protected virtual void ResetParameters(AbstractEntity entity)
		{
			if (entity == null) { throw new ArgumentNullException("entity"); }
			lock (Parameters)
			{
				foreach (DbParameter dbParam in Parameters)
				{
					if (dbParam.Direction == ParameterDirection.Output) { continue; }
					if (entity.TryGetDbProperty(dbParam.SourceColumn, out EntityPropertyMeta propertyInfo))
					{
						object value = propertyInfo.GetValue(entity);
						ResetParameterValue(dbParam, value);
					}
				}
			}
		}

		/// <summary>
		/// 初始化Transact-SQL命令执行参数值
		/// </summary>
		/// <param name="anonObject">包含可执行参数的匿名类。</param>
		internal protected virtual void ResetParameters(object anonObject)
		{
			if (anonObject == null) { throw new ArgumentNullException("anonObject"); }
			Type type = anonObject.GetType();
			lock (Parameters)
			{
				foreach (DbParameter dbParam in Parameters)
				{
					if (dbParam.Direction == ParameterDirection.Output) { continue; }
					PropertyInfo propertyInfo = type.GetProperty(dbParam.SourceColumn);
					if (propertyInfo != null) { ResetParameterValue(dbParam, propertyInfo.GetValue(anonObject, null)); }
					else { ResetParameterValue(dbParam, DBNull.Value); }
				}
			}
		}
		#endregion

		#region 接口 IXmlSerializable 默认实现
		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		internal protected virtual bool ReadAttribute(string name, string value)
		{
			if (name == CommandTypeAttribute)
			{
				CommandType commandType = CommandType.Text;
				if (Enum.TryParse<CommandType>(value, true, out commandType))
					dataDbCommand.CommandType = commandType;
				return true;
			}
			else if (name == CommandTimeoutAttribute)
			{
				dataDbCommand.CommandTimeout = Convert.ToInt32(value);
				return true;
			}
			return false;
		}

		#region 创建数据库参数
		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="parameter">数据库命令执行的参数</param>
		/// <param name="dbType">SqlServer数据库列类型,SqlDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		internal protected abstract void ConvertParameterType(DbParameter parameter, DbTypeEnum dbType, byte precision, byte scale);

		/// <summary>创建SQL命令需要执行的参数名称</summary>
		/// <param name="parameterName">不带参数符号的参数名称</param>
		/// <returns>返回带存储过程符号的参数名称</returns>
		public abstract string CreateParameterName(string parameterName);
		#endregion

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="parameter">数据库参数实例。</param>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		internal protected virtual void CreateParameterReader(DbParameter parameter, XmlReader reader)
		{
			if (reader.HasAttributes)
			{
				int attributeCount = reader.AttributeCount;
				byte precision = 0, scale = 0;
				DbTypeEnum tempDbType = DbTypeEnum.Int32;
				for (int index = 0; index < attributeCount; index++)
				{
					reader.MoveToAttribute(index);
					string name = reader.LocalName;
					string value = reader.GetAttribute(index);
					if (name == DataCommand.Parameter_DbType)
					{
						if (!Regex.IsMatch(value, @"^\d+$"))
							tempDbType = (DbTypeEnum)Enum.Parse(typeof(DbTypeEnum), value);
						else
							tempDbType = (DbTypeEnum)reader.ReadContentAsInt();
					}
					else if (name == DataCommand.Parameter_SourceColumn || name == DataCommand.Parameter_Column)
					{
						parameter.SourceColumn = value;
					}
					else if (name == DataCommand.Parameter_Size)
					{
						parameter.Size = Convert.ToInt32(value);
					}
					else if (name == DataCommand.Parameter_Precision)
					{
						precision = Convert.ToByte(value);
					}
					else if (name == DataCommand.Parameter_Length)
					{
						int length = Convert.ToInt32(value);
						if (tempDbType == DbTypeEnum.Decimal)
							precision = (byte)length;
						else
							parameter.Size = length;
					}
					else if (name == DataCommand.Parameter_Scale)
					{
						scale = Convert.ToByte(value);
					}
					else if (name == DataCommand.Parameter_Nullable)
					{
						parameter.IsNullable = Convert.ToBoolean(value);
					}
					else if (name == DataCommand.Parameter_Direction)
					{
						string directionString = reader.ReadContentAsString();
						ParameterDirection tempDirection = ParameterDirection.Input;
						if (Regex.IsMatch(directionString, @"^\d+$"))
							parameter.Direction = (ParameterDirection)Convert.ToInt32(value);
						else if (Enum.TryParse<ParameterDirection>(value, out tempDirection))
							parameter.Direction = tempDirection;
					}
				}
				ConvertParameterType(parameter, tempDbType, precision, scale);
			}
			parameter.ParameterName = CreateParameterName(reader.ReadElementString());
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		internal protected virtual bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.Name == ParametersElement)
			{
				System.Xml.XmlReader reader2 = reader.ReadSubtree();
				while (reader2.Read())  //读取所有静态命令节点信息
				{
					if (reader2.NodeType == XmlNodeType.Element && reader2.Name == DataCommand.ParameterConfig)
					{
						DbParameter parameter = dataDbCommand.CreateParameter();
						CreateParameterReader(parameter, reader2);
						dataDbCommand.Parameters.Add(parameter);
					}
					else if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name == ParametersElement)
						break;
				}
			}
			return false;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		internal protected virtual void ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToContent();
			if (reader.HasAttributes)
			{
				for (int index = 0; index < reader.AttributeCount; index++)
				{
					reader.MoveToAttribute(index);
					ReadAttribute(reader.LocalName, reader.GetAttribute(index));
				}
			}
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
				if (ReadContent(reader)) { break; }
			}
		}

		/// <summary>
		/// 此方法是保留方法，请不要使用。在实现 IXmlSerializable 接口时，
		/// 应从此方法返回 null（在 Visual Basic 中为 Nothing），
		/// 如果需要指定自定义架构，应向该类应用 XmlSchemaProviderAttribute。
		/// </summary>
		/// <returns>XmlSchema，描述由 WriteXml 方法产生并由 ReadXml 方法使用的对象的 XML 表示形式。</returns>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) { ReadXml(reader); }

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) { }
		#endregion

		#region 接口 INotifyPropertyChanged 的默认实现
		/// <summary>
		/// 在更改属性值时发生。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;


		/// <summary>
		/// 引发 PropertyChanged 事件
		/// </summary>
		/// <param name="propertyName">已更改的属性名。</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1005:可简化委托调用。", Justification = "<挂起>")]
		internal protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		#region 接口 IDbCommand 默认实现
		/// <summary>
		/// 尝试取消 System.Data.Common.DbCommand 的执行。
		/// </summary>
		void IDbCommand.Cancel()
		{
			dataDbCommand.Cancel();
		}

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		string IDbCommand.CommandText
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
		/// 获取或设置在终止执行命令的尝试并生成错误之前的等待时间。
		/// </summary>
		/// <value>等待命令执行的时间（以秒为单位）,默认为 30 秒。</value>
		int IDbCommand.CommandTimeout
		{
			get { return dataDbCommand.CommandTimeout; }
			set { if (dataDbCommand.CommandTimeout != value) { dataDbCommand.CommandTimeout = value; OnPropertyChanged("CommandTimeout"); } }
		}

		/// <summary>
		/// 获取或设置在终止执行命令的尝试并生成错误之前的等待时间。
		/// </summary>
		/// <value>等待命令执行的时间（以秒为单位）,默认为 30 秒。</value>
		internal protected int CommandTimeout
		{
			get
			{
				return dataDbCommand.CommandTimeout;
			}
			set
			{
				if (dataDbCommand.CommandTimeout != value)
				{
					dataDbCommand.CommandTimeout = value;
					OnPropertyChanged("CommandTimeout");
				}
			}
		}

		/// <summary>
		/// 获取或设置一个值，该值指示如何解释 CommandText 属性。
		/// </summary>
		/// <value>CommandType 值之一，默认值为 Text。</value>
		CommandType IDbCommand.CommandType
		{
			get
			{
				return dataDbCommand.CommandType;
			}
			set
			{
				if (dataDbCommand.CommandType != value)
				{
					dataDbCommand.CommandType = value;
					OnPropertyChanged("CommandType");
				}
			}
		}

		/// <summary>
		/// 获取或设置一个值，该值指示如何解释 CommandText 属性。
		/// </summary>
		/// <value>CommandType 值之一，默认值为 Text。</value>
		internal protected CommandType CommandType
		{
			get
			{
				return dataDbCommand.CommandType;
			}
			set
			{
				if (dataDbCommand.CommandType != value)
				{
					dataDbCommand.CommandType = value;
					OnPropertyChanged("CommandType");
				}
			}
		}

		/// <summary>
		///  获取或设置此 System.Data.Common.DbCommand 使用的 System.Data.Common.DbConnection。
		/// </summary>
		/// <value>与数据源的连接。</value>
		IDbConnection IDbCommand.Connection
		{
			get
			{
				return dataDbCommand.Connection;
			}
			set
			{
				if (dataDbCommand.Connection != value)
				{
					dataDbCommand.Connection = value as DbConnection;
					OnPropertyChanged("Connection");
				}
			}
		}

		/// <summary>
		/// 创建 System.Data.Common.DbParameter 对象的新实例。
		/// </summary>
		/// <returns>一个 System.Data.Common.DbParameter 对象。</returns>
		IDbDataParameter IDbCommand.CreateParameter()
		{
			return dataDbCommand.CreateParameter();
		}

		/// <summary>
		/// 对连接对象执行 SQL 语句。
		/// </summary>
		/// <returns> 受影响的行数。</returns>
		int IDbCommand.ExecuteNonQuery()
		{
			return dataDbCommand.ExecuteNonQuery();
		}

		/// <summary>
		/// 针对 System.Data.Common.DbCommand.Connection 执行 System.Data.Common.DbCommand.CommandText，并使用
		/// System.Data.CommandBehavior 值之一返回 System.Data.Common.DbDataReader。
		/// </summary>
		/// <param name="behavior">System.Data.CommandBehavior 值之一。</param>
		/// <returns>一个 System.Data.Common.DbDataReader 对象。</returns>
		IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
		{
			return dataDbCommand.ExecuteReader(behavior);
		}

		/// <summary>
		/// 针对 System.Data.Common.DbCommand.Connection 执行 System.Data.Common.DbCommand.CommandText，
		/// 并返回 System.Data.Common.DbDataReader。
		/// </summary>
		/// <returns>一个 System.Data.Common.DbDataReader 对象。</returns>
		IDataReader IDbCommand.ExecuteReader()
		{
			return dataDbCommand.ExecuteReader();
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
		/// </summary>
		/// <returns>结果集中第一行的第一列。</returns>
		object IDbCommand.ExecuteScalar()
		{
			return dataDbCommand.ExecuteReader();
		}

		/// <summary>
		/// SQL 语句或存储过程的参数。
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal DbParameterCollection Parameters { get { return dataDbCommand.Parameters; } }

		/// <summary>
		/// 获取 System.Data.Common.DbParameter 对象的集合。
		/// </summary>
		/// <value>SQL 语句或存储过程的参数。</value>
		IDataParameterCollection IDbCommand.Parameters
		{
			get { return dataDbCommand.Parameters; }
		}

		/// <summary>
		/// 在数据源上创建该命令的准备好的（或已编译的）版本。
		/// </summary>
		void IDbCommand.Prepare()
		{
			dataDbCommand.Prepare();
		}

		/// <summary>
		///  获取或设置将在其中执行此 System.Data.Common.DbCommand 对象的 System.Data.Common.DbTransaction。
		/// </summary>
		/// <value>要在其中执行 .NET Framework 数据提供程序的 Command 对象的事务。默认值为 null 引用。</value>
		IDbTransaction IDbCommand.Transaction
		{
			get { return dataDbCommand.Transaction; }
			set { dataDbCommand.Transaction = value as DbTransaction; }
		}

		/// <summary>
		/// 获取或设置命令结果在由 DbDataAdapter 的 Update 方法使用时如何应用于 DataRow。 
		/// </summary>
		/// <value> System.Data.UpdateRowSource 值之一。如果该命令不是自动生成的，则默认值为 Both。否则，默认值为 None。</value>
		UpdateRowSource IDbCommand.UpdatedRowSource
		{
			get { return dataDbCommand.UpdatedRowSource; }
			set { dataDbCommand.UpdatedRowSource = value; }
		}

		void System.IDisposable.Dispose()
		{
			dataDbCommand.Dispose();
		}
		#endregion
	}
}
