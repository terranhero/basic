using System;
using System.Linq.Expressions;
using Basic.Interfaces;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示 With 子查询动态查询接口的实现类
	/// 提供基于表达式树的动态查询构建能力，支持条件筛选、排序和分页操作
	/// 
	/// 功能特性：
	/// - 支持链式方法调用，提供流畅的查询接口
	/// - 基于表达式树动态构建查询条件
	/// - 集成分页查询功能，支持大数据量分页处理
	/// - 与 DynamicCommand 协同工作，实现动态 SQL 生成
	/// 
	/// 典型使用场景：
	/// 1. 复杂的分页数据查询
	/// 2. 动态条件筛选
	/// 3. 多表关联查询的 With 子句封装
	/// 4. 大数据量的高效分页检索
	/// </summary>
	/// <typeparam name="T">查询的实体类型，必须是引用类型</typeparam>
	/// <example>
	/// // 典型使用示例：
	/// var query = new WithQuery&lt;User&gt;(command, 20, 1, 0)
	///     .Where(u => u.Age > 18)
	///     .Where(u => u.Status == UserStatus.Active)
	///     .OrderBy(u => u.CreateTime)
	///     .OrderByDescending(u => u.LastLoginTime);
	/// </example>
	public sealed class WithQuery<T> : IWithQuery<T> where T : class
	{
		/// <summary>
		/// 需要查询的当前页大小。
		/// 表示每页显示的记录数量，用于分页查询
		/// 示例：PageSize = 20 表示每页显示20条记录
		/// </summary>
		private readonly int PageSize;

		/// <summary>
		/// 需要查询的当前页索引，索引从1开始。
		/// 注意：与编程中常见的0基索引不同，这里使用1基索引以符合用户习惯
		/// 示例：PageIndex = 1 表示查询第一页数据
		/// </summary>
		private readonly int PageIndex;

		/// <summary>
		/// 获取上次查询结果的记录总数。
		/// 此参数用于优化分页性能，当已知总记录数时可避免重复计数查询
		/// 特殊值：0 表示需要重新计算总记录数
		/// </summary>
		private readonly int TotalItems;

		/// <summary>
		/// 执行此次查询的动态命令实例。
		/// 负责将表达式树转换为具体的数据库查询命令（如SQL语句）
		/// 封装了数据库访问底层细节，提供统一的查询执行接口
		/// </summary>
		private readonly DynamicCommand dynamicCommand;

		/// <summary>
		/// 条件表达式树。
		/// 存储所有通过 Where 方法添加的查询条件，使用 AndAlso 逻辑连接多个条件
		/// 初始为 null，在添加第一个条件时初始化
		/// </summary>
		private Expression expressions = null;

		/// <summary>
		/// 使用动态命令和分页信息初始化 WithQuery 类实例
		/// 此构造函数为内部访问级别，确保查询对象通过受控的方式创建
		/// </summary>
		/// <param name="dataCommand">
		/// 执行此次查询的动态命令 DynamicCommand 子类实例
		/// 负责查询的执行和结果映射，是查询功能的核心组件
		/// </param>
		/// <param name="pageSize">
		/// 需要查询的当前页大小。
		/// 取值范围：大于0的正整数，建议根据业务需求合理设置
		/// </param>
		/// <param name="pageIndex">
		/// 需要查询的当前页索引，索引从1开始。
		/// 注意：传入小于1的值可能导致查询异常
		/// </param>
		/// <param name="totalItems">
		/// 上次查询结果的记录数，如果传入此参数则不同不再重新检索当前查询结果的总数。
		/// 使用场景：在连续分页查询时，如果总记录数不变，可传入此值优化性能
		/// 特殊处理：传入0表示需要自动计算总记录数
		/// </param>
		/// <example>
		/// // 内部创建示例：
		/// var query = new WithQuery&lt;User&gt;(dynamicCommand, 20, 1, 0);
		/// // 使用已知总数优化性能：
		/// var query = new WithQuery&lt;User&gt;(dynamicCommand, 20, 2, 150);
		/// </example>
		internal WithQuery(DynamicCommand dataCommand, int pageSize, int pageIndex, int totalItems)
		{
			dynamicCommand = dataCommand;
			PageSize = pageSize;
			PageIndex = pageIndex;
			TotalItems = totalItems;
		}

		/// <summary>
		/// 基于谓词筛选值序列。
		/// 支持多次调用，多个条件之间使用 AND 逻辑连接
		/// 使用表达式树构建查询条件，提供编译时类型安全检查
		/// </summary>
		/// <param name="predicate">
		/// 用于测试每个元素是否满足条件的函数。
		/// 示例：u => u.Age > 18 || p => p.Price &lt; 100 &amp;&amp; p.Stock > 0
		/// 支持复杂的Lambda表达式，包括成员访问、方法调用、常量比较等
		/// </param>
		/// <returns>返回当前查询对象实例，支持链式调用。</returns>
		/// <example>
		/// // 单条件查询：
		/// query.Where(u => u.Name.Contains("张"));
		/// 
		/// // 多条件组合查询：
		/// query.Where(u => u.Age > 18)
		///      .Where(u => u.City == "北京")
		///      .Where(u => u.RegisterTime > DateTime.Now.AddMonths(-1));
		/// 
		/// // 复杂条件查询：
		/// query.Where(u => u.Status == UserStatus.Active &amp;&amp; (u.Role == Role.Admin || u.Role == Role.Manager));
		/// </example>
		public IWithQuery<T> Where(Expression<Func<T, bool>> predicate)
		{
			if (expressions == null)
			{
				expressions = predicate.Body;
				return this;
			}
			else
			{
				// 使用 AND 逻辑连接新条件和已有条件
				expressions = Expression.AndAlso(expressions, predicate.Body);
				return this;
			}
		}

		/// <summary>
		/// 根据键按升序对序列的元素排序。
		/// 注意：当前实现为预留接口，具体排序逻辑需要在 DynamicCommand 中实现
		/// </summary>
		/// <param name="keySelector">
		/// 用于从元素中提取排序键的函数。
		/// 示例：u => u.CreateTime 或 p => p.Price
		/// </param>
		/// <returns>返回当前查询对象实例，支持链式调用。</returns>
		/// <example>
		/// // 单字段排序：
		/// query.OrderBy(u => u.CreateTime);
		/// 
		/// // 多字段排序（结合OrderByDescending）：
		/// query.OrderBy(u => u.Department)
		///      .OrderByDescending(u => u.Salary);
		/// </example>
		public IWithQuery<T> OrderBy(Expression<Func<T, object>> keySelector)
		{
			// 排序逻辑应在 DynamicCommand 中具体实现
			// 此处返回当前实例以支持链式调用
			return this;
		}

		/// <summary>
		/// 根据键按降序对序列的元素排序。
		/// 注意：当前实现为预留接口，具体排序逻辑需要在 DynamicCommand 中实现
		/// </summary>
		/// <param name="keySelector">
		/// 用于从元素中提取排序键的函数。
		/// 示例：u => u.LastLoginTime 或 p => p.SalesCount
		/// </param>
		/// <returns>返回当前查询对象实例，支持链式调用。</returns>
		/// <example>
		/// // 按时间倒序排列：
		/// query.OrderByDescending(u => u.LastLoginTime);
		/// 
		/// // 组合排序：先按部门升序，再按薪资降序
		/// query.OrderBy(u => u.Department)
		///      .OrderByDescending(u => u.Salary);
		/// </example>
		public IWithQuery<T> OrderByDescending(Expression<Func<T, object>> keySelector)
		{
			// 排序逻辑应在 DynamicCommand 中具体实现
			// 此处返回当前实例以支持链式调用
			return this;
		}
	}
}
