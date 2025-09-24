using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Threading.Tasks;
using Basic.DataAccess;
using Basic.Enums;
using Basic.Tables;
using Npgsql;

namespace Basic.PostgreSql
{
	/// <summary>
	/// 表示要对 SQL Server 数据库执行的一个静态结构的 Transact-SQL 语句或存储过程。
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	internal sealed class NpgBulkCopyCommand : BulkCopyCommand, IDisposable
	{
		//private readonly npgsql _BulkCopy;
		//private readonly List<string> _DestinationColumns = new List<string>(100);
		//private readonly TableConfiguration _TableInfo;
		/// <summary>初始化 NpgBulkCopyCommand 类的新实例。 </summary>
		/// <param name="connection">将用于执行批量复制操作的已经打开的 System.Data.SqlClient.SqlConnection 实例。</param>
		///<param name="configInfo">表示当前数据库表配置信息</param>
		public NpgBulkCopyCommand(NpgsqlConnection connection, TableConfiguration configInfo)
			: base(new NpgsqlCommand(), configInfo)
		{
			//_BulkCopy = new NpgBulkCopyCommand(connection);
			//_TableInfo = configInfo;

			//foreach (ColumnInfo column in _TableInfo.Columns)
			//{
			//	_BulkCopy.ColumnMappings.Add(column.Name, column.Name);
			//	_DestinationColumns.Add(column.Name);
			//}
			//_BulkCopy.DestinationTableName = _TableInfo.TableName;
		}

		/// <summary>
		/// 创建新的 XXXBulkCopyColumnMapping 并将其添加到集合中，
		/// 使用列名指定源列和目标列。
		/// </summary>
		/// <param name="sourceColumn">数据源中源列的名称</param>
		/// <param name="destinationColumn">数据源中目标列的名称</param>
		/// <returns>受影响的行数。</returns>
		public override bool TryAddOrUpdateMapping(string sourceColumn, string destinationColumn)
		{
			//if (_DestinationColumns.Contains(destinationColumn)) { return false; }
			//_SqlBulkCopy.ColumnMappings.Add(sourceColumn, destinationColumn);
			//_DestinationColumns.Add(destinationColumn);
			return true;
		}

		/// <summary>
		/// 针对 .NET Framework 数据提供程序的 Connection 对象执行 SQL 语句，并返回受影响的行数。
		/// </summary>
		/// <typeparam name="TR">表示 BaseTableRowType 子类类型</typeparam>
		/// <param name="table">实体类，包含了需要执行参数的值。</param>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <returns>受影响的行数。</returns>
		internal protected override System.Threading.Tasks.Task ExecuteAsync<TR>(BaseTableType<TR> table, int timeout)
		{
			return Task.CompletedTask;
		}

		/// <summary>当前命令的数据库类型</summary>
		public override ConnectionType ConnectionType { get { return ConnectionType.SqlConnection; } }

		/// <summary>
		/// 针对 .NET Framework 数据提供程序的 Connection 对象执行 SQL 语句，并返回受影响的行数。
		/// </summary>
		/// <typeparam name="TR">表示 BaseTableRowType 子类类型</typeparam>
		/// <param name="table">实体类，包含了需要执行参数的值。</param>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <returns>受影响的行数。</returns>
		internal protected override void Execute<TR>(BaseTableType<TR> table, int timeout)
		{
			//_DataAdapter.BatchSize = table.Count > 1000 ? 1000 : table.Count;
			//_DataAdapter.BulkCopyTimeout = timeout;
			//foreach (ColumnInfo column in _TableInfo.Columns)
			//{
			//	if (table.Columns.Contains(column.Name))
			//	{
			//		_DataAdapter.ColumnMappings.Add(column.Name, column.Name);
			//	}
			//}
			//_DataAdapter.DestinationTableName = _TableInfo.TableName;
			//_DataAdapter.WriteToServer(table);
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
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="parameter">数据库命令执行的参数</param>
		/// <param name="dbType">Npg数据库列类型,SqlDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		internal protected override void ConvertParameterType(DbParameter parameter, DbTypeEnum dbType, byte precision, byte scale)
		{
			NpgsqlParameter sqlParameter = parameter as NpgsqlParameter;
			NpgParameterConverter.ConvertSqlParameterType(sqlParameter, dbType, precision, scale);
		}
	}
}
