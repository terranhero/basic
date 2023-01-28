using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.ComponentModel;

using System.Data.Common;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.DataAccess;

namespace Basic.SqlServer
{
	/// <summary>
	/// 
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.ComponentModel.ToolboxItem(false)]
	public sealed class SqlNewValueCommand : NewValueCommand, ICloneable
	{
		private readonly SqlCommand sqlCommand;
		private readonly SqlStaticCommand dbStaticCommand;
		/// <summary>
		/// 执行SQL Server 类型数据库的数据检测。
		/// </summary>
		/// <param name="staticCommand">表示执行静态命令 StaticCommand 类实例。</param>
		internal SqlNewValueCommand(SqlStaticCommand staticCommand)
			: base(staticCommand, new SqlCommand())
		{
			sqlCommand = dataDbCommand as SqlCommand;
			dbStaticCommand = staticCommand;
		}

		/// <summary>
		/// 执行SQL Server 类型数据库的数据检测。
		/// </summary>
		/// <param name="staticCommand">表示执行静态命令 StaticCommand 类实例。</param>
		/// <param name="sqlCmd">表示要对 SQL Server 数据库执行的一个 Transact-SQL 语句或存储过程类实例。</param>
		private SqlNewValueCommand(SqlStaticCommand staticCommand, SqlCommand sqlCmd)
			: base(staticCommand, sqlCmd) { sqlCommand = sqlCmd; dbStaticCommand = staticCommand; }

		/// <summary>当前命令的数据库类型</summary>
		public override ConnectionType ConnectionType { get { return ConnectionType.SqlConnection; } }

		/// <summary>
		///  获取 System.Data.SqlClient.SqlParameterCollection。
		/// </summary>
		/// <value>Transact-SQL 语句或存储过程的参数。默认值为空集合。</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SqlParameterCollection Parameters
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
		/// 复制 SqlNewValueCommand 命令。
		/// </summary>
		public SqlNewValueCommand Clone()
		{
			lock (this)
			{
				SqlNewValueCommand checkCommand = new SqlNewValueCommand(dbStaticCommand, sqlCommand.Clone());
				checkCommand.NewType = this.NewType;
				return checkCommand;
			}
		}

		/// <summary>
		/// 复制 SqlNewValueCommand 命令。
		/// </summary>
		/// <param name="staticCommand">复制后 SqlNewValueCommand 所属的静态命令。</param>
		internal SqlNewValueCommand Clone(SqlStaticCommand staticCommand)
		{
			lock (this)
			{
				SqlNewValueCommand newCommand = new SqlNewValueCommand(staticCommand, sqlCommand.Clone());
				newCommand.NewType = this.NewType;
				return newCommand;
			}
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
			dbStaticCommand.ConvertParameterType(parameter, dbType, precision, scale);
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
