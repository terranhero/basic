using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Enums
{
	/// <summary>
	/// 表示基类可执行方法的枚举
	/// </summary>
	public enum StaticMethodEnum
	{
		/// <summary>
		/// 对连接执行 Transact-SQL 语句并返回受影响的行数，此方法采用批处理方式执行。
		/// </summary>
		BatchExecute,
		/// <summary>
		/// 对连接执行 Transact-SQL 语句并返回受影响的行数，此方法采用强类型实体类执行。
		/// </summary>
		ExecuteCore,
		/// <summary>
		/// 对连接执行 Transact-SQL 语句并返回受影响的行数，此方法采用强类型实体类执行。
		/// </summary>
		ExecuteNonQuery,
		/// <summary>
		/// 将 CommandText 发送到 Connection 并生成一个 DbDataReader。 
		/// </summary>
		ExecuteReader,
		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
		/// </summary>
		ExecuteScalar,
		/// <summary>
		/// 执行查询，并返回查询结果集中第一行的信息。忽略其他行。
		/// </summary>
		SearchEntity,
		/// <summary>
		/// 根据条件填充 DataSet 类实例，此方法主要用于报表数据显示。
		/// </summary>
		FillDataSet,
		/// <summary>
		/// 根据条件填充 DataTable 类实例。
		/// </summary>
		FillDataTable,
		/// <summary>
		/// 根据条件查询可分页的实体列表，此方法主要用于存储过程中使用。
		/// </summary>
		GetPagination
	}

	/// <summary>
	/// 表示基类可执行方法的枚举
	/// </summary>
	public enum DynamicMethodEnum
	{
		/// <summary>
		/// 根据条件查询可分页的实体列表。
		/// </summary>
		GetEntities,

		/// <summary>
		/// 根据条件查询可分页的实体列表。
		/// </summary>
		GetJoinEntities,

		/// <summary>
		/// 根据条件查询可分页的实体列表。
		/// </summary>
		GetDataTable
	}
}
