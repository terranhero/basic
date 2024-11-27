using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Basic.Interfaces
{
	/// <summary>
	/// 可查询的实体列表信息
	/// </summary>
	/// <typeparam name="T">模型实体</typeparam>
	public interface IQueryEntities<T> where T : class
	{
		///// <summary>
		///// 获取或设置要对数据源执行的 Transact-SQL 语句中动态添加 WHERE 条件部分。
		///// </summary>
		///// <value>要执行的 Transact-SQL 语句的动态添加的 WHERE 部分，默认值为空字符串。</value>
		//string TempWhereText { get; set; }

		///// <summary>
		///// 获取或设置要对数据源执行的 Transact-SQL 语句中 ORDER BY 部分。
		///// </summary>
		///// <value>要执行的 Transact-SQL 语句的动态添加的 ORDER BY 部分，默认值为空字符串。</value>
		//string TempOrderText { get; set; }

		/// <summary>查询初始化属性事件</summary>
		event System.Action<T, DbDataReader, IReadOnlyDictionary<string, int>> InitializeProperty;

		/// <summary>将可查询的实体列表转换成实体列表</summary>
		/// <param name="sorting">是否需要排序</param>
		/// <returns>接口 IPagination 的实例</returns>
		IPagination<T> ToResult(bool sorting);

		/// <summary>将可查询的实体列表转换成实体列表</summary>
		/// <param name="pagination">需要填充的实例列表</param>
		/// <param name="sorting">是否需要排序</param>
		/// <returns>接口 IPagination 的实例</returns>
		IPagination<T> ToResult(IPagination<T> pagination, bool sorting);

		/// <summary>将可查询的实体列表转换成实体列表</summary>
		/// <param name="pagination">需要填充的实例列表</param>
		/// <param name="sorting">是否需要排序</param>
		/// <returns>接口 IPagination 的实例</returns>
		System.Threading.Tasks.Task<IPagination<T>> ToResultAsync(IPagination<T> pagination, bool sorting);

		/// <summary>将可查询的实体列表转换成实体列表</summary>
		/// <param name="sorting">是否需要排序</param>
		/// <returns>接口 IPagination 的实例</returns>
		System.Threading.Tasks.Task<IPagination<T>> ToResultAsync(bool sorting);

		/// <summary>
		/// 将可查询的实体列表转换成分页实体列表
		/// </summary>
		/// <returns>接口 IPagination 的实例</returns>
		IPagination<T> ToPagination();

		///// <summary>
		///// 将可查询的实体列表转换成分页实体列表
		///// </summary>
		///// <param name="pagination"></param>
		///// <returns>接口 IPagination 的实例</returns>
		//IPagination<T> ToPagination(Pagination<T> pagination);

		/// <summary>
		/// 将可查询的实体列表转换成分页实体列表
		/// </summary>
		/// <returns>接口 IPagination 的实例</returns>
		Task<IPagination<T>> ToPaginationAsync();

		///// <summary>
		///// 将可查询的实体列表转换成分页实体列表
		///// </summary>
		///// <param name="pagination"></param>
		///// <returns>接口 IPagination 的实例</returns>
		//Task<IPagination<T>> ToPaginationAsync(Pagination<T> pagination);

		/// <summary>返回当前查询结果的第一条记录并填充实例属性。</summary>
		/// <returns><![CDATA[如果查询成功则返回true，否则返回false]]></returns>
		System.Threading.Tasks.Task<bool> ToEntityAsync(T entity);

		/// <summary>返回当前查询结果的第一条记录。</summary>
		/// <returns><![CDATA[如果查询结果存在记录则返回 <T> 类型实体模型，否则返回null。]]></returns>
		T ToEntity();

		/// <summary>返回当前查询结果的第一条记录。</summary>
		/// <returns><![CDATA[如果查询结果存在记录则返回 <T> 类型实体模型，否则返回null。]]></returns>
		System.Threading.Tasks.Task<T> ToEntityAsync();

		/// <summary>基于谓词筛选值序列。</summary>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		IQueryEntities<T> Where(Expression<Func<T, bool>> predicate);

		/// <summary>基于谓词筛选值序列。</summary>
		/// <param name="conditioin">测试当前 predicate 参数是否允许使用。</param>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		IQueryEntities<T> Where(bool conditioin, Expression<Func<T, bool>> predicate);

		/// <summary>排除重复记录。</summary>
		IQueryEntities<T> Distinct();

		/// <summary>
		/// 根据键按序列的元素排序。
		/// </summary>
		/// <param name="orderText">字段排序信息。</param>
		IQueryEntities<T> OrderBy(string orderText);

		/// <summary>
		/// 根据键按序列的元素排序。
		/// </summary>
		/// <param name="orderText">字段排序信息。</param>
		IQueryEntities<T> OrderByDescending(string orderText);

		/// <summary>
		/// 根据键按升序对序列的元素排序。
		/// </summary>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		IQueryEntities<T> OrderByDescending(Expression<Func<T, object>> keySelector);

		/// <summary>
		/// 根据键按升序对序列的元素排序。
		/// </summary>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		IQueryEntities<T> OrderBy(Expression<Func<T, object>> keySelector);
	}
}
