using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Basic.EntityLayer;
using Basic.Expressions;
using Basic.Interfaces;

namespace Basic.DataAccess
{
	/// <summary>表示With子查询动态查询接口</summary>
	/// <typeparam name="T"></typeparam>
	public sealed class DeleteEntities<T> : IDeleteEntities<T> where T : AbstractEntity
	{
		/// <summary>执行此次查询的动态命令实例</summary>
		private readonly StaticCommand _StaticCommand;
		/// <summary>当前实体模型关联的数据库表定义</summary>
		private readonly TableMappingAttribute _tableMapping;

		/// <summary>条件表达式树</summary>
		private Expression expressions = null;

		/// <summary>使用动态命令和分页信息初始化 WithQuery 类实例</summary>
		/// <param name="command">执行此次查询的动态命令 DynamicCommand 子类实例</param>
		internal DeleteEntities(StaticCommand command)
		{
			_StaticCommand = command;
			_tableMapping = typeof(T).GetCustomAttribute<TableMappingAttribute>();
			if (_tableMapping == null) { throw new MissingMemberException("TableMappingAttribute", "table"); }
		}

		/// <summary>使用Lambda表示式，设置更新命令的条件</summary>
		/// <param name="expression">用于测试每个元素是否满足条件的函数。</param>
		public IDeleteEntities<T> Where<TP>(Expression<Func<T, TP>> expression)
		{
			if (expression == null) { return this; }
			BinaryExpression be = Expression.Equal(expression.Body, expression.Body);
			if (expressions == null) { expressions = be; }
			else { expressions = Expression.AndAlso(expressions, be); }
			return this;
		}

		/// <summary>使用Lambda表示式，设置更新命令的条件</summary>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		public IDeleteEntities<T> Where(Expression<Func<T, bool>> predicate)
		{
			if (expressions == null) { expressions = predicate.Body; }
			else { expressions = Expression.AndAlso(expressions, predicate.Body); }
			return this;
		}

		/// <summary>使用Lambda表示式，设置更新命令的条件</summary>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		IDeleteEntities<T> IDeleteEntities<T>.Where(Expression<Func<T, bool>> predicate) { return this.Where(predicate); }

		private StaticCommand BuildCommandText()
		{
			TableMappingAttribute table = typeof(T).GetCustomAttribute<TableMappingAttribute>();
			if (table == null) { throw new MissingMemberException("TableMappingAttribute", "table"); }
			StringBuilder whereBuilder = new StringBuilder(500);
			whereBuilder.Append("DELETE FROM ").Append(_tableMapping.TableName).Append(" WHERE ");
			_StaticCommand.CreateWhere(whereBuilder, expressions);
			_StaticCommand.CommandText = whereBuilder.ToString();
			return _StaticCommand;
		}

		/// <summary>根据传入的实体模型更新数据库</summary>
		/// <param name="entities">需要更新的实体模型</param>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		public Result Execute(T[] entities)
		{
			Result result = Result.Empty; BuildCommandText();
			if (_StaticCommand.ResetParameters(entities))
			{
				result.AffectedRows = _StaticCommand.ExecuteNonQuery();
				return result;
			}
			foreach (AbstractEntity entity in entities)
			{
				_StaticCommand.ResetParameters(entity);
				result.AffectedRows += _StaticCommand.ExecuteNonQuery();
			}
			return result;
		}

		/// <summary>根据传入的实体模型更新数据库</summary>
		/// <param name="entity">需要更新的实体模型</param>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		public Result Execute(T entity)
		{
			Result result = Result.Empty; BuildCommandText();
			_StaticCommand.ResetParameters(entity);
			result.AffectedRows = _StaticCommand.ExecuteNonQuery();
			return result;
		}

		/// <summary>根据传入的匿名对象更新数据库</summary>
		/// <param name="anonymous">需要更新的匿名对象</param>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		public Result Execute(object anonymous)
		{
			Result result = Result.Empty; BuildCommandText();
			_StaticCommand.ResetParameters(anonymous);
			result.AffectedRows = _StaticCommand.ExecuteNonQuery();
			return result;
		}

		/// <summary>根据传入的实体模型更新数据库</summary>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		public Result Execute()
		{
			Result result = Result.Empty; BuildCommandText();
			result.AffectedRows = _StaticCommand.ExecuteNonQuery();
			return result;
		}

		/// <summary>通过传入实体模型，执行当前删除命令</summary>
		/// <param name="entities">需要更新的实体模型</param>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		public async System.Threading.Tasks.Task<Result> ExecuteAsync(T[] entities)
		{
			Result result = Result.Empty; BuildCommandText();
			if (_StaticCommand.ResetParameters(entities))
			{
				result.AffectedRows = await _StaticCommand.ExecuteNonQueryAsync();
				return result;
			}
			foreach (AbstractEntity entity in entities)
			{
				_StaticCommand.ResetParameters(entity);
				result.AffectedRows += await _StaticCommand.ExecuteNonQueryAsync();
			}
			return result;
		}

		/// <summary>通过传入实体模型，执行当前删除命令</summary>
		/// <param name="entity">需要更新的实体模型</param>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		public async System.Threading.Tasks.Task<Result> ExecuteAsync(T entity)
		{
			Result result = Result.Empty; BuildCommandText();
			_StaticCommand.ResetParameters(entity);
			result.AffectedRows = await _StaticCommand.ExecuteNonQueryAsync();
			return result;
		}

		/// <summary>通过传入实体模型，执行当前删除命令</summary>
		/// <param name="anonymous">需要更新的匿名对象</param>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		public async System.Threading.Tasks.Task<Result> ExecuteAsync(object anonymous)
		{
			Result result = Result.Empty; BuildCommandText();
			_StaticCommand.ResetParameters(anonymous);
			result.AffectedRows = await _StaticCommand.ExecuteNonQueryAsync();
			return result;
		}

		/// <summary>根据传入的实体模型更新数据库</summary>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		public async System.Threading.Tasks.Task<Result> ExecuteAsync()
		{
			Result result = Result.Empty; BuildCommandText();
			result.AffectedRows = await _StaticCommand.ExecuteNonQueryAsync();
			return result;
		}

		/// <summary>释放程序中的托管资源</summary>
		void IDisposable.Dispose() { }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
		System.Threading.Tasks.ValueTask IAsyncDisposable.DisposeAsync()
		{
#if NET5_0_OR_GREATER
			return System.Threading.Tasks.ValueTask.CompletedTask;
#elif NETSTANDARD2_1_OR_GREATER
			return new System.Threading.Tasks.ValueTask(System.Threading.Tasks.Task.CompletedTask);
#endif
		}
#endif
	}
}
