using System;
using System.ComponentModel;
using System.Data.Common;
using Basic.DataAccess;
using Basic.Enums;
using Npgsql;

namespace Basic.PostgreSql
{
	/// <summary></summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.ComponentModel.ToolboxItem(false)]
	public sealed class NpgNewValueCommand : NewValueCommand, ICloneable
	{
		private readonly NpgsqlCommand sqlCommand;
		private readonly NpgStaticCommand dbStaticCommand;
		/// <summary>
		/// 执行SQL Server 类型数据库的数据检测。
		/// </summary>
		/// <param name="staticCommand">表示执行静态命令 StaticCommand 类实例。</param>
		internal NpgNewValueCommand(NpgStaticCommand staticCommand)
			: base(staticCommand, new NpgsqlCommand())
		{
			sqlCommand = dataDbCommand as NpgsqlCommand;
			dbStaticCommand = staticCommand;
		}

		/// <summary>
		/// 执行SQL Server 类型数据库的数据检测。
		/// </summary>
		/// <param name="staticCommand">表示执行静态命令 StaticCommand 类实例。</param>
		/// <param name="sqlCmd">表示要对 SQL Server 数据库执行的一个 Transact-SQL 语句或存储过程类实例。</param>
		private NpgNewValueCommand(NpgStaticCommand staticCommand, NpgsqlCommand sqlCmd)
			: base(staticCommand, sqlCmd) { sqlCommand = sqlCmd; dbStaticCommand = staticCommand; }

		/// <summary>当前命令的数据库类型</summary>
		public override ConnectionType ConnectionType { get { return ConnectionType.NpgSqlConnection; } }

		/// <summary>
		///  获取 System.Data.NpgClient.NpgParameterCollection。
		/// </summary>
		/// <value>Transact-SQL 语句或存储过程的参数。默认值为空集合。</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new NpgsqlParameterCollection Parameters
		{
			get { return sqlCommand.Parameters; }
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
		/// 复制 NpgNewValueCommand 命令。
		/// </summary>
		public NpgNewValueCommand Clone()
		{
			lock (this)
			{
				NpgNewValueCommand checkCommand = new NpgNewValueCommand(dbStaticCommand, sqlCommand.Clone());
				checkCommand.NewType = this.NewType;
				return checkCommand;
			}
		}

		/// <summary>
		/// 复制 NpgNewValueCommand 命令。
		/// </summary>
		/// <param name="staticCommand">复制后 NpgNewValueCommand 所属的静态命令。</param>
		internal NpgNewValueCommand Clone(NpgStaticCommand staticCommand)
		{
			lock (this)
			{
				NpgNewValueCommand newCommand = new NpgNewValueCommand(staticCommand, sqlCommand.Clone());
				newCommand.NewType = this.NewType;
				return newCommand;
			}
		}

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="parameter">数据库命令执行的参数</param>
		/// <param name="dbType">NpgNpg数据库列类型,NpgDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		internal protected override void ConvertParameterType(DbParameter parameter, DbTypeEnum dbType, byte precision, byte scale)
		{
			dbStaticCommand.ConvertParameter(parameter, dbType, precision, scale);
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
