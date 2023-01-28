using System;
using System.Linq.Expressions;
using Basic.Expressions;
using Basic.Interfaces;

namespace Basic.DataAccess
{
	/// <summary>表示With子查询动态查询接口</summary>
	/// <typeparam name="T"></typeparam>
	public sealed class WithQuery<T> : IWithQuery<T> where T : class
	{
		/// <summary>需要查询的当前页大小。</summary>
		private readonly int PageSize;

		/// <summary>需要查询的当前页索引,索引从1开始。</summary>
		private readonly int PageIndex;

		/// <summary>获取上次查询结果的记录总数。</summary>
		private readonly int TotalItems;

		/// <summary>执行此次查询的动态命令实例</summary>
		private readonly DynamicCommand dynamicCommand;

		/// <summary>Where条件 Lambda 表达式数组。</summary>
		private readonly LambdaExpressionCollection lambdaCollection;

		/// <summary>使用动态命令和分页信息初始化 WithQuery 类实例</summary>
		/// <param name="dataCommand">执行此次查询的动态命令 DynamicCommand 子类实例</param>
		/// <param name="pageSize">需要查询的当前页大小。</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始。</param>
		/// <param name="totalItems">上次查询结果的记录数，如果传入此参数则不同不再重新检索当前查询结果的总数。</param>
		internal WithQuery(DynamicCommand dataCommand, int pageSize, int pageIndex, int totalItems)
		{
			lambdaCollection = new LambdaExpressionCollection(dataCommand);
			dynamicCommand = dataCommand;
			PageSize = pageSize;
			PageIndex = pageIndex;
			TotalItems = totalItems;
		}

		/// <summary>基于谓词筛选值序列。</summary>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		/// <returns>返回当前查询对象实例。</returns>
		public IWithQuery<T> Where(Expression<Func<T, bool>> predicate) { LambdaResolver.AnalyzeExpression(lambdaCollection, predicate.Body); return this; }

		/// <summary>根据键按升序对序列的元素排序。</summary>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		/// <returns>返回当前查询对象实例。</returns>
		public IWithQuery<T> OrderBy(Expression<Func<T, object>> keySelector) { return this; }

		/// <summary>根据键按降序对序列的元素排序。</summary>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		/// <returns>返回当前查询对象实例。</returns>
		public IWithQuery<T> OrderByDescending(Expression<Func<T, object>> keySelector) { return this; }
	}
}
