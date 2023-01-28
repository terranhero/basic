using System;
using System.ComponentModel;
using System.Data.Common;
using Basic.DataAccess;
using Basic.Enums;
using IBM.Data.DB2.Core;

namespace Basic.DB2Access
{
	/// <summary>
	/// 
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.ComponentModel.ToolboxItem(false)]
	internal class DB2CheckCommand : CheckCommand, ICloneable
	{
		private readonly DB2Command dbCommand;
		private readonly DB2StaticCommand dbStaticCommand;
		/// <summary>
		/// 执行SQL Server 类型数据库的数据检测。
		/// </summary>
		/// <param name="staticCommand">表示执行静态命令 StaticCommand 类实例。</param>
		internal DB2CheckCommand(DB2StaticCommand staticCommand)
			: base(staticCommand, new DB2Command())
		{
			dbStaticCommand = staticCommand;
			dbCommand = dataDbCommand as DB2Command;
		}

		/// <summary>
		/// 执行SQL Server 类型数据库的数据检测。
		/// </summary>
		/// <param name="staticCommand">表示执行静态命令 StaticCommand 类实例。</param>
		/// <param name="sqlCmd">表示要对 SQL Server 数据库执行的一个 Transact-SQL 语句或存储过程类实例。</param>
		private DB2CheckCommand(DB2StaticCommand staticCommand, DB2Command sqlCmd)
			: base(staticCommand, sqlCmd) { dbCommand = sqlCmd; dbStaticCommand = staticCommand; }

		/// <summary>当前命令的数据库类型</summary>
		public override ConnectionType ConnectionType { get { return ConnectionType.Db2Connection; } }

		/// <summary>
		///  获取 System.Data.DB2Client.DB2ParameterCollection。
		/// </summary>
		/// <value>Transact-SQL 语句或存储过程的参数。默认值为空集合。</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new DB2ParameterCollection Parameters
		{
			get { return dbCommand.Parameters; }
		}

		/// <summary>
		/// 创建作为当前实例副本的新对象。
		/// </summary>
		/// <returns>作为此实例副本的新对象。</returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// 复制 CheckDataCommand 命令。
		/// </summary>
		public DB2CheckCommand Clone() { return Clone(dbStaticCommand); }

		/// <summary>
		/// 复制 CheckDataCommand 命令。
		/// </summary>
		/// <param name="staticCommand">复制后 DB2CheckCommand 所属的静态命令。</param>
		internal DB2CheckCommand Clone(DB2StaticCommand staticCommand)
		{
			lock (this)
			{
				DB2Command cmd = ((ICloneable)dbCommand).Clone() as DB2Command;
				DB2CheckCommand checkCommand = new DB2CheckCommand(staticCommand, cmd);
				checkCommand.ErrorCode = ErrorCode;
				checkCommand.Parameter = Parameter;
				checkCommand.ErrorMessage = ErrorMessage;
				checkCommand.Converter = Converter;
				checkCommand.CheckExist = CheckExist;
				checkCommand.PropertyName = PropertyName;
				return checkCommand;
			}
		}

		/// <summary>
		/// 根据父命令读取当前检测命令的参数信息
		/// </summary>
		/// <param name="strArray">此命令需要的参数名称数组</param>
		internal protected override void ReadOwnerCommandParameter(string[] strArray)
		{
			foreach (string parameterName in strArray)
			{
				string paramName = dataCommand.CreateParameterName(parameterName);
				if (dataCommand.Parameters.Contains(paramName))
				{
					DB2Parameter param = dataCommand.Parameters[paramName] as DB2Parameter;
					DB2Parameter newParameter = (param as ICloneable).Clone() as DB2Parameter;
					dbCommand.Parameters.Add(newParameter);
				}
			}
		}

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="parameter">数据库命令执行的参数</param>
		/// <param name="dbType">DB2Server数据库列类型,DB2DbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		internal protected override void ConvertParameterType(DbParameter parameter, DbTypeEnum dbType, byte precision, byte scale)
		{
			DB2Parameter dbParameter = parameter as DB2Parameter;
			DB2ParameterConverter.ConvertDB2ParameterType(dbParameter, dbType, precision, scale);
		}

		/// <summary>
		/// 返回存储过程参数名称全名称
		/// </summary>
		/// <param name="noParameterName">不带参数符号的参数名称</param>
		/// <returns>返回带存储过程符号的参数名称</returns>
		public override string CreateParameterName(string noParameterName)
		{
			return dbStaticCommand.CreateParameterName(noParameterName);
		}
	}
}
