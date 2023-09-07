using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Basic.Collections;
using Basic.EntityLayer;
using Basic.Expressions;
using Basic.Interfaces;
using STT = System.Threading.Tasks;

namespace Basic.DataAccess
{
	/// <summary>
	/// 可查询实体类列表
	/// </summary>
	/// <typeparam name="T">返回的实体类型</typeparam>
	public sealed class QueryEntities<T> : IQueryEntities<T> where T : AbstractEntity, new()
	{
		#region QueryEntities 只读属性列表
		/// <summary>
		/// 需要查询的当前页大小。
		/// </summary>
		private readonly int PageSize;
		/// <summary>
		/// 需要查询的当前页索引,索引从1开始。
		/// </summary>
		private readonly int PageIndex;
		/// <summary>
		/// 获取上次查询结果的记录总数。
		/// </summary>
		private readonly int TotalItems;
		/// <summary>
		/// 执行此次查询的动态命令实例
		/// </summary>
		private readonly DynamicCommand dynamicCommand;
		/// <summary>
		/// Where条件 Lambda 表达式数组。
		/// </summary>
		private readonly LambdaExpressionCollection lambdaCollection;
		#endregion

		/// <summary>
		/// 使用动态命令和分页信息初始化 QueryEntities 类实例
		/// </summary>
		/// <param name="dataCommand">执行此次查询的动态命令 DynamicCommand 子类实例</param>
		/// <param name="pageSize">需要查询的当前页大小。</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始。</param>
		/// <param name="totalItems">上次查询结果的记录数，如果传入此参数则不同不再重新检索当前查询结果的总数。</param>
		internal QueryEntities(DynamicCommand dataCommand, int pageSize, int pageIndex, int totalItems)
		{
			lambdaCollection = new LambdaExpressionCollection(dataCommand);
			dynamicCommand = dataCommand;
			PageSize = pageSize;
			PageIndex = pageIndex;
			TotalItems = totalItems;
		}

		/// <summary>查询初始化属性事件</summary>
		public event System.Action<T, DbDataReader, IReadOnlyDictionary<string, int>> InitializeProperty;

		/// <summary>引发 InitializeProperty 事件</summary>
		/// <param name="entity">需要初始化的实体模型实例</param>
		/// <param name="reader">表示数据库单向只读的数据流</param>
		/// <param name="positions">表示 DbDataReader 中字段位置键值对数组</param>
		/// <returns>如果自定义代码触发此事件，则系统跳过默认初始化属性值</returns>
		private void OnInitializeProperty(T entity, DbDataReader reader, IReadOnlyDictionary<string, int> positions)
		{
			if (InitializeProperty != null) { InitializeProperty(entity, reader, positions); }
		}

		/// <summary>返回当前查询结果的第一条记录并填充实例属性。</summary>
		/// <returns><![CDATA[如果查询成功则返回true，否则返回false]]></returns>
		public async STT.Task<bool> ToEntityAsync(T entity)
		{
			try
			{
				dynamicCommand.CreateWhere(lambdaCollection);
				dynamicCommand.InitializeCommandText(1, 1);
				using (DbDataReader reader = await dynamicCommand.ExecuteReaderAsync(CommandBehavior.SingleRow))
				{
					if (reader.IsClosed == false && reader.Read())
					{
						Dictionary<string, int> fieldPositions = new Dictionary<string, int>(reader.FieldCount);
						for (int index = 0; index <= reader.FieldCount - 1; index++)
						{
							string name = reader.GetName(index); fieldPositions[name] = index;
							if (entity.TryGetDbProperty(name, out EntityPropertyMeta propertyInfo))
							{
								object propertyValue = reader.GetValue(index);
								propertyInfo.SetValue(entity, propertyValue);
							}
						}

						OnInitializeProperty(entity, reader, fieldPositions);
						return true;
					}
				}
				return false;
			}
			finally
			{
				dynamicCommand.ReleaseConnection();
			}
		}

		/// <summary>
		/// 返回当前查询结果的第一条记录。
		/// </summary>
		/// <returns><![CDATA[如果查询结果存在记录则返回 <T> 类型实体模型，否则返回null。]]></returns>
		public T ToEntity()
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
						T entity = new T(); Dictionary<string, int> fieldPositions = new Dictionary<string, int>(reader.FieldCount);
						for (int index = 0; index <= reader.FieldCount - 1; index++)
						{
							string name = reader.GetName(index); fieldPositions[name] = index;
							if (entity.TryGetDbProperty(name, out EntityPropertyMeta propertyInfo))
							{
								object propertyValue = reader.GetValue(index);
								propertyInfo.SetValue(entity, propertyValue);
							}
						}
						OnInitializeProperty(entity, reader, fieldPositions);
						return entity;
					}
				}
			}
			return null;
		}

		/// <summary>返回当前查询结果的第一条记录并填充实例属性。</summary>
		/// <returns><![CDATA[如果查询结果存在记录则返回 <T> 类型实体模型，否则返回null。]]></returns>
		public async STT.Task<T> ToEntityAsync()
		{
			try
			{
				dynamicCommand.CreateWhere(lambdaCollection);
				dynamicCommand.InitializeCommandText(1, 1);
				using (DbDataReader reader = await dynamicCommand.ExecuteReaderAsync(CommandBehavior.SingleRow))
				{
					if (reader.IsClosed == false && reader.Read())
					{
						T entity = new T(); Dictionary<string, int> fieldPositions = new Dictionary<string, int>(reader.FieldCount);
						for (int index = 0; index <= reader.FieldCount - 1; index++)
						{
							string name = reader.GetName(index); fieldPositions[name] = index;
							if (entity.TryGetDbProperty(name, out EntityPropertyMeta propertyInfo))
							{
								object propertyValue = reader.GetValue(index);
								propertyInfo.SetValue(entity, propertyValue);
							}
						}

						OnInitializeProperty(entity, reader, fieldPositions);
						return entity;
					}
				}
				return null;
			}
			finally
			{
				dynamicCommand.ReleaseConnection();
			}
		}

		/// <summary>
		/// 将可查询的实体列表转换成分页实体列表
		/// </summary>
		/// <param name="entity">需要接收查询结果的 AbstractEntity 子类实例。</param>
		/// <returns>如果查询结果存在则返回 True，否则返回 False。</returns>
		public bool SearchEntity(T entity)
		{
			using (dynamicCommand)
			{
				dynamicCommand.CreateWhere(lambdaCollection);
				dynamicCommand.InitializeCommandText(1, 1);
				using (DbDataReader reader = dynamicCommand.ExecuteReader(CommandBehavior.SingleRow))
				{
					if (!reader.IsClosed && reader.HasRows)
					{
						if (reader.Read())
						{
							Dictionary<string, int> fieldPositions = new Dictionary<string, int>(reader.FieldCount);
							for (int index = 0; index <= reader.FieldCount - 1; index++)
							{
								string name = reader.GetName(index); fieldPositions[name] = index;
								if (entity.TryGetDbProperty(name, out EntityPropertyMeta propertyInfo))
								{
									object propertyValue = reader.GetValue(index);
									propertyInfo.SetValue(entity, propertyValue);
								}
							}
							OnInitializeProperty(entity, reader, fieldPositions);
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
		/// <returns>接口 IPagination 的实例</returns>
		public IPagination<T> ToPagination()
		{
			return ToPagination(TotalItems);
		}

		/// <summary>
		/// 将可查询的实体列表转换成分页实体列表
		/// </summary>
		/// <param name="totalItems">上次查询结果的记录总数。</param>
		/// <returns>接口 IPagination 的实例</returns>
		public IPagination<T> ToPagination(int totalItems)
		{
			return ToPagination(new Pagination<T>(totalItems));
		}

		/// <summary>将可查询的实体列表转换成分页实体列表</summary>
		/// <param name="pagination"></param>
		/// <returns>接口 IPagination 的实例</returns>
		public IPagination<T> ToPagination(Pagination<T> pagination)
		{
			dynamicCommand.CreateWhere(lambdaCollection);
			try
			{
				pagination.PageIndex = PageIndex;
				pagination.PageSize = PageSize;
				dynamicCommand.InitializeCommandText(PageSize, PageIndex);
				using (DbDataReader reader = dynamicCommand.ExecuteReader(CommandBehavior.Default))
				{
					ReaderToPagination(reader, pagination);
					if (reader.NextResult()) { ReadToRecordRows(reader, pagination); }
				}
			}
			finally
			{
				dynamicCommand.ReleaseConnection();
			}
			if (PageSize == 0 && PageIndex == 0 && pagination.Capacity == 0) { pagination.Capacity = pagination.Count; }
			return pagination;
		}

		private void ReaderToPagination(DbDataReader reader, IPagination<T> pagination)
		{
			if (reader.IsClosed == false && reader.HasRows == true)
			{
				EntityPropertyMeta[] properties = AbstractEntity.GetProperties<T>();
				Dictionary<int, EntityPropertyMeta> fieldProperty = new Dictionary<int, EntityPropertyMeta>(reader.FieldCount);
				Dictionary<string, int> fieldPositions = new Dictionary<string, int>(reader.FieldCount);
				int returnCountIndex = -1;
				for (int index = 0; index < reader.FieldCount; index++)
				{
					string name = reader.GetName(index); fieldPositions[name] = index;
					if (name == AbstractDataCommand.ReturnCountName) { returnCountIndex = index; continue; }
					foreach (EntityPropertyMeta info in properties)
					{
						if (info.Mapping != null && info.Mapping.ColumnName == name) { fieldProperty.Add(index, info); break; }
						else if (string.Compare(info.Name, name, true) == 0) { fieldProperty.Add(index, info); break; }
					}
				}
				while (reader.Read())
				{
					if (returnCountIndex >= 0) { pagination.Capacity = reader.GetInt32(returnCountIndex); returnCountIndex = -1; }
					T entity = new T(); foreach (KeyValuePair<int, EntityPropertyMeta> keyValue in fieldProperty)
					{
						keyValue.Value.SetValue(entity, reader.GetValue(keyValue.Key));
					}
					OnInitializeProperty(entity, reader, fieldPositions); pagination.Add(entity);
				}
			}
			if (PageSize == 0 && PageIndex == 0 && pagination.Capacity == 0) { pagination.Capacity = pagination.Count; }
		}

		/// <summary>读取记录总数</summary>
		/// <param name="reader">从数据源读取仅向前的行流</param>
		/// <param name="pagination"></param>
		private void ReadToRecordRows(DbDataReader reader, IPagination<T> pagination)
		{
			while (reader.Read())
			{
				int returnCountIndex = reader.GetOrdinal(AbstractDataCommand.ReturnCountName);
				pagination.Capacity = reader.GetInt32(returnCountIndex);
			}
		}

		/// <summary>将可查询的实体列表转换成实体列表</summary>
		/// <param name="pagination">需要填充的实例列表</param>
		/// <param name="sorting">是否需要排序</param>
		/// <returns>接口 IPagination 的实例</returns>
		public IPagination<T> ToResult(IPagination<T> pagination, bool sorting)
		{
			dynamicCommand.CreateWhere(lambdaCollection);
			try
			{
				dynamicCommand.InitializeCommandText(sorting);
				using (DbDataReader reader = dynamicCommand.ExecuteReader(CommandBehavior.Default))
				{
					ReaderToPagination(reader, pagination);
					if (reader.NextResult()) { ReadToRecordRows(reader, pagination); }
				}
			}
			finally
			{
				dynamicCommand.ReleaseConnection();
			}

			return pagination;
		}

		/// <summary>将可查询的实体列表转换成实体列表</summary>
		/// <param name="pagination">需要填充的实例列表</param>
		/// <param name="sorting">是否需要排序</param>
		/// <returns>接口 IPagination 的实例</returns>
		public async Task<IPagination<T>> ToResultAsync(IPagination<T> pagination, bool sorting)
		{
			dynamicCommand.CreateWhere(lambdaCollection);
			dynamicCommand.InitializeCommandText(sorting);
			using (DbDataReader reader = await dynamicCommand.ExecuteReaderAsync())
			{
				try
				{
					ReaderToPagination(reader, pagination);
					if (reader.NextResult()) { ReadToRecordRows(reader, pagination); }
				}
				finally
				{
					dynamicCommand.ReleaseConnection();
				}
			}
			return pagination;
		}

		/// <summary>将可查询的实体列表转换成实体列表</summary>
		/// <param name="sorting">是否需要排序</param>
		/// <returns>接口 IPagination 的实例</returns>
		public IPagination<T> ToResult(bool sorting)
		{
			Pagination<T> pagination = new Pagination<T>();
			dynamicCommand.CreateWhere(lambdaCollection);
			try
			{
				dynamicCommand.InitializeCommandText(sorting);
				using (DbDataReader reader = dynamicCommand.ExecuteReader(CommandBehavior.Default))
				{
					ReaderToPagination(reader, pagination);
					if (reader.NextResult()) { ReadToRecordRows(reader, pagination); }
				}
			}
			finally
			{
				dynamicCommand.ReleaseConnection();
			}
			return pagination;
		}

		/// <summary>将可查询的实体列表转换成实体列表</summary>
		/// <param name="sorting">是否需要排序</param>
		/// <returns>接口 IPagination 的实例</returns>
		public async Task<IPagination<T>> ToResultAsync(bool sorting)
		{
			Pagination<T> pagination = new Pagination<T>();
			dynamicCommand.CreateWhere(lambdaCollection);

			pagination.PageIndex = PageIndex;
			pagination.PageSize = PageSize;
			dynamicCommand.InitializeCommandText(sorting);
			using (DbDataReader reader = await dynamicCommand.ExecuteReaderAsync())
			{
				try
				{
					ReaderToPagination(reader, pagination);
					if (reader.NextResult()) { ReadToRecordRows(reader, pagination); }
				}
				finally
				{
					dynamicCommand.ReleaseConnection();
				}
			}
			return pagination;
		}

		/// <summary>
		/// 将可查询的实体列表转换成分页实体列表
		/// </summary>
		/// <returns>接口 IPagination 的实例</returns>
		public Task<IPagination<T>> ToPaginationAsync()
		{
			return ToPaginationAsync(new Pagination<T>());
		}

		/// <summary>
		/// 将可查询的实体列表转换成分页实体列表
		/// </summary>
		/// <param name="pagination"></param>
		/// <returns>接口 IPagination 的实例</returns>
		public async Task<IPagination<T>> ToPaginationAsync(Pagination<T> pagination)
		{
			dynamicCommand.CreateWhere(lambdaCollection);

			pagination.PageIndex = PageIndex;
			pagination.PageSize = PageSize;
			dynamicCommand.InitializeCommandText(PageSize, PageIndex);
			using (DbDataReader reader = await dynamicCommand.ExecuteReaderAsync())
			{
				try
				{
					ReaderToPagination(reader, pagination);
					if (reader.NextResult()) { ReadToRecordRows(reader, pagination); }
				}
				finally
				{
					dynamicCommand.ReleaseConnection();
				}
			}
			return pagination;
		}

		/// <summary>排除重复记录。</summary>
		public IQueryEntities<T> Distinct() { dynamicCommand.Distinct(); return this; }

		/// <summary>基于谓词筛选值序列。</summary>
		/// <param name="conditioin">测试当前 predicate 参数是否允许使用。</param>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		public IQueryEntities<T> Where(bool conditioin, Expression<Func<T, bool>> predicate)
		{
			if (conditioin == false) { return this; }
			LambdaResolver.AnalyzeExpression(lambdaCollection, predicate.Body);
			return this;
		}

		/// <summary>基于谓词筛选值序列。</summary>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		public IQueryEntities<T> Where(Expression<Func<T, bool>> predicate)
		{
			LambdaResolver.AnalyzeExpression(lambdaCollection, predicate.Body);
			return this;
		}

		/// <summary>
		/// 根据键按序列的元素排序。
		/// </summary>
		/// <param name="orderText">字段排序信息。</param>
		public IQueryEntities<T> OrderBy(string orderText)
		{
			if (!string.IsNullOrEmpty(orderText))
				dynamicCommand.OrderBy<T>(orderText.Trim().ToUpper());
			return this;
		}

		/// <summary>
		/// 根据键按序列的元素排序。
		/// </summary>
		/// <param name="orderText">字段排序信息。</param>
		public IQueryEntities<T> OrderByDescending(string orderText)
		{
			if (!string.IsNullOrEmpty(orderText))
				dynamicCommand.OrderByDescending<T>(orderText.Trim().ToUpper());
			return this;
		}

		/// <summary>
		/// 根据键按升序对序列的元素排序。
		/// </summary>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		public IQueryEntities<T> OrderByDescending(Expression<Func<T, object>> keySelector)
		{
			dynamicCommand.OrderByDescending<T>(keySelector);
			return this;
		}

		/// <summary>
		/// 根据键按升序对序列的元素排序。
		/// </summary>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		public IQueryEntities<T> OrderBy(Expression<Func<T, object>> keySelector)
		{
			dynamicCommand.OrderBy<T>(keySelector);
			return this;
		}
	}
}
