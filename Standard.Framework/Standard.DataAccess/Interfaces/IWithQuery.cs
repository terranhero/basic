using System;
using System.Linq.Expressions;

namespace Basic.Interfaces
{
	/// <summary>表示With子查询动态查询接口</summary>
	/// <typeparam name="T"></typeparam>
	public interface IWithQuery<T> where T : class
	{
		/// <summary>基于谓词筛选值序列。</summary>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		/// <returns>返回当前查询对象实例。</returns>
		IWithQuery<T> Where(Expression<Func<T, bool>> predicate);

		/// <summary>根据键按升序对序列的元素排序。</summary>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		/// <returns>返回当前查询对象实例。</returns>
		IWithQuery<T> OrderBy(Expression<Func<T, object>> keySelector);

		/// <summary>根据键按降序对序列的元素排序。</summary>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		/// <returns>返回当前查询对象实例。</returns>
		IWithQuery<T> OrderByDescending(Expression<Func<T, object>> keySelector);
	}
}
