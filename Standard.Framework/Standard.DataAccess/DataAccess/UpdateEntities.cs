using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Basic.EntityLayer;
using Basic.Interfaces;

namespace Basic.DataAccess
{
	/// <summary>表示With子查询动态查询接口</summary>
	/// <typeparam name="T"></typeparam>
	public sealed class UpdateEntities<T> : IUpdateEntities<T> where T : AbstractEntity
	{
		/// <summary>执行此次查询的动态命令实例</summary>
		private readonly StaticCommand _StaticCommand;

		/// <summary>当前实体模型关联的数据库表定义</summary>
		private readonly TableMappingAttribute _tableMapping;

		/// <summary>条件表达式树</summary>
		private Expression expressions = null;

		/// <summary>SET子句 Lambda 表达式数组</summary>
		private readonly List<MemberExpression> fields = new List<MemberExpression>(30);

		/// <summary>使用动态命令和分页信息初始化 WithQuery 类实例</summary>
		/// <param name="command">执行此次查询的动态命令 DynamicCommand 子类实例</param>
		internal UpdateEntities(StaticCommand command)
		{
			_StaticCommand = command;
			_tableMapping = typeof(T).GetCustomAttribute<TableMappingAttribute>();
			if (_tableMapping == null) { throw new MissingMemberException("TableMappingAttribute", "table"); }
		}

		private StaticCommand BuildCommandText()
		{
			StringBuilder builder = new StringBuilder(500);
			builder.Append("UPDATE ").Append(_tableMapping.TableName).Append(" SET ");
			_StaticCommand.CreateUpdates(builder, fields);
			builder.Append(" WHERE ");
			_StaticCommand.CreateWhere(builder, expressions);
			_StaticCommand.CommandText = builder.ToString();
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

		/// <summary>使用Lambda表示式，设置更新命令的字段</summary>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		public IUpdateEntities<T> Set<TP>(Expression<Func<T, TP>> predicate)
		{
			if (predicate.Body is MemberExpression me) { fields.Add(me); }
			//else if (predicate.Body is BinaryExpression be)
			//{
			//	if (be.Left is MemberExpression me1 && be.NodeType == ExpressionType.Assign)
			//	{
			//		fields.Add(me1);
			//	}
			//}
			else { throw new MemberAccessException("类型成员访问异常,此方法必须使用成员方法"); }
			return this;
		}

		/// <summary>使用Lambda表示式，设置更新命令的条件</summary>
		/// <param name="expression">用于测试每个元素是否满足条件的函数。</param>
		public IUpdateEntities<T> Where<TP>(Expression<Func<T, TP>> expression)
		{
			if (expression == null) { return this; }
			BinaryExpression be = Expression.Equal(expression.Body, expression.Body);
			if (expressions == null) { expressions = be; return this; }
			else { expressions = Expression.AndAlso(expressions, be); return this; }
		}

		/// <summary>使用Lambda表示式，设置更新命令的条件</summary>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		public IUpdateEntities<T> Where(Expression<Func<T, bool>> predicate)
		{
			if (expressions == null) { expressions = predicate.Body; return this; }
			else { expressions = Expression.AndAlso(expressions, predicate.Body); return this; }
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

		/// <summary>根据传入的匿名对象更新数据库</summary>
		/// <param name="anonymous">需要更新的匿名对象</param>
		/// <returns>执行Transact-SQL语句的返回结果</returns>
		public async System.Threading.Tasks.Task<Result> ExecuteAsync(object anonymous)
		{
			Result result = Result.Empty; BuildCommandText();
			_StaticCommand.ResetParameters(anonymous);
			result.AffectedRows = await _StaticCommand.ExecuteNonQueryAsync();
			return result;
		}

		/// <summary>释放程序中的托管资源</summary>
		void IDisposable.Dispose() { }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
		ValueTask IAsyncDisposable.DisposeAsync()
		{
#if NET5_0_OR_GREATER
			return ValueTask.CompletedTask;
#elif NETSTANDARD2_1_OR_GREATER
			return new ValueTask(Task.CompletedTask);
#endif
		}
#endif
	}
}
