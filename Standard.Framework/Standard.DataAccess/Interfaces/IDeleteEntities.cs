using System;
using System.Linq.Expressions;
using Basic.EntityLayer;

namespace Basic.Interfaces
{
	/// <summary>可删除的动态命令信息</summary>
	/// <typeparam name="T">模型实体</typeparam>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
	public interface IDeleteEntities<T> : IDisposable, System.IAsyncDisposable where T : class
#else
	public interface IDeleteEntities<T> : IDisposable where T : class
#endif
	{
		/// <summary>通过传入实体模型，执行当前删除命令</summary>
		/// <param name="entities">需要更新的实体模型</param>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		Result Execute(T[] entities);

		/// <summary>通过传入实体模型，执行当前删除命令</summary>
		/// <param name="entity">需要更新的实体模型</param>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		Result Execute(T entity);

		/// <summary>根据传入的匿名对象删除数据</summary>
		/// <param name="anonymous">需要执行命令的匿名对象</param>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		Result Execute(object anonymous);

		/// <summary>根据传入的实体模型更新数据库</summary>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		Result Execute();

		/// <summary>通过传入实体模型，执行当前删除命令</summary>
		/// <param name="entities">需要更新的实体模型</param>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		System.Threading.Tasks.Task<Result> ExecuteAsync(T[] entities);

		/// <summary>通过传入实体模型，执行当前删除命令</summary>
		/// <param name="entity">需要更新的实体模型</param>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		System.Threading.Tasks.Task<Result> ExecuteAsync(T entity);

		/// <summary>通过传入匿名对象，执行当前删除命令</summary>
		/// <param name="anonymous">需要执行命令的匿名对象</param>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		System.Threading.Tasks.Task<Result> ExecuteAsync(object anonymous);

		/// <summary>根据传入的实体模型更新数据库</summary>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		System.Threading.Tasks.Task<Result> ExecuteAsync();

		/// <summary>使用Lambda表示式，设置更新命令的条件</summary>
		/// <param name="expression">用于测试每个元素是否满足条件的函数。</param>
		IDeleteEntities<T> Where<TP>(Expression<Func<T, TP>> expression);

		/// <summary>使用Lambda表示式，设置删除命令的条件</summary>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		IDeleteEntities<T> Where(Expression<Func<T, bool>> predicate);

	}
}
