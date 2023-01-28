using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Basic.DataAccess;
using Basic.Collections;
using Basic.EntityLayer;
using Basic.Expressions;
using Basic.Interfaces;
using Basic.Tables;
using Basic.Enums;
using System.Threading.Tasks;

namespace Basic.DataAccess
{
	/// <summary>
	/// 可查询的 DataTable 
	/// </summary>
	/// <typeparam name="TR">返回的实体类型</typeparam>
	public sealed class QueryDataTable<TR> : IQueryEntities<TR> where TR : BaseTableRowType
	{
		#region QueryEntities 只读属性列表
		private readonly BaseTableType<TR> dataTable;
		/// <summary>
		/// 需要查询的当前页大小。
		/// </summary>
		private readonly int PageSize;
		/// <summary>
		/// 需要查询的当前页索引,索引从1开始。
		/// </summary>
		private readonly int PageIndex;
		/// <summary>
		/// 执行此次查询的动态命令实例
		/// </summary>
		private readonly DynamicCommand dynamicCommand;
		/// <summary>
		/// Where条件 Lambda 表达式数组。
		/// </summary>
		private readonly LambdaExpressionCollection lambdaCollection;
		#endregion

		///// <summary>
		///// 获取或设置要对数据源执行的 Transact-SQL 语句中动态添加 WHERE 条件部分。
		///// </summary>
		///// <value>要执行的 Transact-SQL 语句的动态添加的 WHERE 部分，默认值为空字符串。</value>
		//public string TempWhereText { get { return dynamicCommand.TempWhereText; } set { dynamicCommand.TempWhereText = value; } }

		///// <summary>
		///// 获取或设置要对数据源执行的 Transact-SQL 语句中 ORDER BY 部分。
		///// </summary>
		///// <value>要执行的 Transact-SQL 语句的动态添加的 ORDER BY 部分，默认值为空字符串。</value>
		//public string TempOrderText { get { return dynamicCommand.TempOrderText; } set { dynamicCommand.TempOrderText = value; } }

		/// <summary>
		/// 使用动态命令和分页信息初始化 QueryEntities 类实例
		/// </summary>
		/// <param name="table">表示需要填充的 DataTable 类实例。</param>
		/// <param name="dataCommand">执行此次查询的动态命令 DynamicCommand 子类实例</param>
		/// <param name="pageSize">需要查询的当前页大小。</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始。</param>
		/// <param name="totalItems">上次查询结果的记录数，如果传入此参数则不同不再重新检索当前查询结果的总数。</param>
		internal QueryDataTable(BaseTableType<TR> table, DynamicCommand dataCommand, int pageSize, int pageIndex, int totalItems)
		{
			dataTable = table;
			lambdaCollection = new LambdaExpressionCollection(dataCommand);
			dynamicCommand = dataCommand;
			PageSize = pageSize;
			PageIndex = pageIndex;
			dataTable.Capacity = totalItems;
		}

		/// <summary>查询初始化属性事件</summary>
		public event System.Action<TR, DbDataReader, IReadOnlyDictionary<string, int>> InitializeProperty;

		/// <summary>引发 InitializeProperty 事件</summary>
		/// <param name="row">需要初始化的实体模型实例</param>
		/// <param name="reader">表示数据库单向只读的数据流</param>
		/// <param name="positions">表示 DbDataReader 中字段位置键值对数组</param>
		/// <returns>如果自定义代码触发此事件，则系统跳过默认初始化属性值</returns>
		private bool RaiseInitializeProperty(TR row, DbDataReader reader, IReadOnlyDictionary<string, int> positions)
		{
			if (InitializeProperty != null) { InitializeProperty(row, reader, positions); return false; }
			return true;
		}

		/// <summary>将可查询的实体列表转换成实体列表</summary>
		/// <param name="sorting">是否需要排序</param>
		/// <returns>接口 IPagination 的实例</returns>
		IPagination<TR> IQueryEntities<TR>.ToResult(bool sorting)
		{
			dynamicCommand.CreateWhere(lambdaCollection);
			try
			{
				dynamicCommand.InitializeCommandText(sorting);
				dynamicCommand.Fill(dataTable);
			}
			finally
			{
				dynamicCommand.ReleaseConnection();
			}
			return dataTable;
		}

		/// <summary>将可查询的实体列表转换成实体列表</summary>
		/// <param name="sorting">是否需要排序</param>
		/// <returns>接口 IPagination 的实例</returns>
		public BaseTableType<TR> ToResult(bool sorting)
		{
			dynamicCommand.CreateWhere(lambdaCollection);
			try
			{
				dynamicCommand.InitializeCommandText(sorting);
				dynamicCommand.Fill(dataTable);
			}
			finally
			{
				dynamicCommand.ReleaseConnection();
			}
			return dataTable;
		}

		/// <summary>
		/// 将可查询的实体列表转换成分页实体列表
		/// </summary>
		/// <returns>接口 IPagination 的实例</returns>
		IPagination<TR> IQueryEntities<TR>.ToPagination()
		{
			return GetDataTable();
		}


		/// <summary>
		/// 返回当前查询结果的第一条记录。
		/// </summary>
		/// <returns><![CDATA[如果查询结果存在记录则返回 <T> 类型实体模型，否则返回null。]]></returns>
		public TR ToEntity()
		{
			using (dynamicCommand)
			{
				dynamicCommand.CreateWhere(lambdaCollection);
				dynamicCommand.InitializeCommandText(1, 1);
				using (DbDataReader reader = dynamicCommand.ExecuteReader(CommandBehavior.SingleRow))
				{
					if (reader.IsClosed && !reader.HasRows) { return null; }
					if (reader.Read())
					{
						int fieldCount = reader.FieldCount - 1;
						TR row = dataTable.NewDataRow();
						for (int index = 0; index <= fieldCount; index++)
						{
							string name = reader.GetName(index);
							if (row.Table.Columns.Contains(name))
							{
								row[name] = reader.GetValue(index);
							}
						}
						return row;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// 将可查询的实体列表转换成分页实体列表
		/// </summary>
		/// <param name="row">需要接收查询结果的 AbstractEntity 子类实例。</param>
		/// <returns>如果查询结果存在则返回 True，否则返回 False。</returns>
		public bool SearchEntity(TR row)
		{
			using (dynamicCommand)
			{
				dynamicCommand.CreateWhere(lambdaCollection);
				dynamicCommand.InitializeCommandText(1, 1);
				using (DbDataReader reader = dynamicCommand.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleRow))
				{
					int fieldCount = reader.FieldCount - 1;
					if (reader.Read())
					{
						for (int index = 0; index <= fieldCount; index++)
						{
							string name = reader.GetName(index);
							if (row.Table.Columns.Contains(name))
							{
								row[name] = reader.GetValue(index);
							}
						}
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// 将可查询的实体列表转换成分页实体列表
		/// </summary>
		/// <param name="totalItems">上次查询结果的记录总数。</param>
		/// <returns>接口 IPagination 的实例</returns>
		public IPagination<TR> GetDataTable(int totalItems)
		{
			dataTable.Capacity = totalItems;
			return GetDataTable();
		}

		/// <summary>
		/// 将可查询的实体列表转换成分页实体列表
		/// </summary>
		/// <returns>接口 IPagination 的实例</returns>
		public BaseTableType<TR> GetDataTable()
		{
			try
			{
				dataTable.PageIndex = PageIndex;
				dataTable.PageSize = PageSize;
				dynamicCommand.CreateWhere(lambdaCollection);
				if (PageSize > 0 && PageIndex > 0 && dataTable.Capacity == 0)
				{
					dynamicCommand.InitCountText();
					dataTable.Capacity = (int)dynamicCommand.ExecuteScalar();
				}
				if (dataTable.Capacity == 0 && PageSize > 0 && PageIndex > 0) { return dataTable; }
				dynamicCommand.InitializeCommandText(PageSize, PageIndex);
				dynamicCommand.Fill(dataTable);
			}
			finally
			{
				dynamicCommand.ReleaseConnection();
			}
			if (PageSize == 0 && PageIndex == 0 && dataTable.Capacity == 0)
			{
				dataTable.Capacity = dataTable.Count;
			}
			return dataTable;
		}

		/// <summary>基于谓词筛选值序列。</summary>
		/// <param name="conditioin">测试当前 predicate 参数是否允许使用。</param>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		public IQueryEntities<TR> Where(bool conditioin, Expression<Func<TR, bool>> predicate)
		{
			if (conditioin == false) { return this; }
			LambdaResolver.AnalyzeExpression(lambdaCollection, predicate.Body);
			return this;
		}

		/// <summary>基于谓词筛选值序列。</summary>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		public IQueryEntities<TR> Where(Expression<Func<TR, bool>> predicate)
		{
			LambdaResolver.AnalyzeExpression(lambdaCollection, predicate.Body);
			return this;
		}

		/// <summary>排除重复记录。</summary>
		public IQueryEntities<TR> Distinct() { dynamicCommand.Distinct(); return this; }

		/// <summary>
		/// 根据键按序列的元素排序。
		/// </summary>
		/// <param name="orderText">字段排序信息。</param>
		public IQueryEntities<TR> OrderBy(string orderText)
		{
			if (!string.IsNullOrEmpty(orderText))
				dynamicCommand.OrderBy<TR>(orderText.Trim().ToUpper());
			return this;
		}

		/// <summary>
		/// 根据键按序列的元素排序。
		/// </summary>
		/// <param name="orderText">字段排序信息。</param>
		public IQueryEntities<TR> OrderByDescending(string orderText)
		{
			if (!string.IsNullOrEmpty(orderText))
				dynamicCommand.OrderByDescending<TR>(orderText.Trim().ToUpper());
			return this;
		}


		/// <summary>
		/// 根据键按升序对序列的元素排序。
		/// </summary>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		public IQueryEntities<TR> OrderByDescending(Expression<Func<TR, object>> keySelector)
		{
			dynamicCommand.OrderByDescending<TR>(keySelector);
			return this;
		}

		/// <summary>
		/// 根据键按升序对序列的元素排序。
		/// </summary>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		public IQueryEntities<TR> OrderBy(Expression<Func<TR, object>> keySelector)
		{
			dynamicCommand.OrderBy<TR>(keySelector);
			return this;
		}

		Task<IPagination<TR>> IQueryEntities<TR>.ToResultAsync(IPagination<TR> pagination, bool sorting)
		{
			dynamicCommand.CreateWhere(lambdaCollection);
			TaskCompletionSource<IPagination<TR>> tcs = new TaskCompletionSource<IPagination<TR>>();
			try
			{
				dynamicCommand.InitializeCommandText(sorting);
				dynamicCommand.Fill(dataTable);
				tcs.SetResult(dataTable);
			}
			finally
			{
				dynamicCommand.ReleaseConnection();
			}
			return tcs.Task;
		}

		Task<IPagination<TR>> IQueryEntities<TR>.ToResultAsync(bool sorting)
		{
			dynamicCommand.CreateWhere(lambdaCollection);
			TaskCompletionSource<IPagination<TR>> tcs = new TaskCompletionSource<IPagination<TR>>();
			try
			{
				dynamicCommand.InitializeCommandText(sorting);
				dynamicCommand.Fill(dataTable);
				tcs.SetResult(dataTable);
			}
			finally
			{
				dynamicCommand.ReleaseConnection();
			}
			return tcs.Task;
		}

		Task<TR> IQueryEntities<TR>.ToEntityAsync()
		{
			TaskCompletionSource<TR> tcs = new TaskCompletionSource<TR>();
			dynamicCommand.CreateWhere(lambdaCollection);
			dynamicCommand.InitializeCommandText(1, 1);
			using (DbDataReader reader = dynamicCommand.ExecuteReader(CommandBehavior.SingleRow))
			{
				if (reader.IsClosed && !reader.HasRows) { return null; }
				if (reader.Read())
				{
					int fieldCount = reader.FieldCount - 1;
					TR row = dataTable.NewDataRow();
					for (int index = 0; index <= fieldCount; index++)
					{
						string name = reader.GetName(index);
						if (row.Table.Columns.Contains(name))
						{
							row[name] = reader.GetValue(index);
						}
					}
					tcs.SetResult(row);
				}
			}
			return tcs.Task;
		}

		/// <summary>将可查询的实体列表转换成实体列表</summary>
		/// <param name="pagination">需要填充的实例列表</param>
		/// <param name="sorting">是否需要排序</param>
		/// <returns>接口 IPagination 的实例</returns>
		public IPagination<TR> ToResult(IPagination<TR> pagination, bool sorting)
		{
			BaseTableType<TR> table = pagination as BaseTableType<TR>;
			if (table == null)
			{
				throw new ArgumentException($"参数\"pagination\"类型错误，需要传入 BaseTableType<TR> 类型额参数，实际传入了 {pagination.GetType()}", nameof(pagination));
			}
			dynamicCommand.CreateWhere(lambdaCollection);
			try
			{
				dynamicCommand.InitializeCommandText(sorting);
				dynamicCommand.Fill(table);
			}
			finally
			{
				dynamicCommand.ReleaseConnection();
			}
			return table;
		}
	}
}
