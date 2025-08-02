using System;
using System.Threading.Tasks;
using Basic.EntityLayer;
using Basic.Interfaces;
using Basic.Tables;
using ST = System.Transactions;

namespace Basic.DataAccess
{
	/// <summary>数据管理类</summary>
	public abstract class AbstractContext : AbstractDataContext
	{
		/// <summary>初始化 AbstractContext 类实例，此方法紧供子类继承调用</summary>
		protected AbstractContext() : base() { }

		/// <summary>初始化 AbstractContext 类实例，此方法紧供子类继承调用</summary>
		protected AbstractContext(IUserContext context) : base(context) { }

		/// <summary>
		/// 初始化 AbstractContext 类实例，AbstractContext 类为抽象类，所以此方法紧供子类继承调用
		/// </summary>
		/// <param name="connection">数据库连接名称</param>
		protected AbstractContext(string connection) : base(connection) { }

		/// <summary>
		/// 创建AbstractAccess子类实例
		/// </summary>
		/// <param name="connectionName">数据库连接名称</param>
		/// <returns>返回AbstractAccess子类的实例</returns>
		protected abstract AbstractAccess CreateAccess(string connectionName);

		/// <summary>
		/// 创建AbstractAccess子类实例
		/// </summary>
		/// <param name="startTransaction">是否启用事务</param>
		/// <param name="connectionName">数据库连接字符串</param>
		/// <returns>返回AbstractAccess子类的实例</returns>
		protected abstract AbstractAccess CreateAccess(string connectionName, bool startTransaction);

		/// <summary>使用指定的事物隔离级别，创建 AbstractAccess 子类实例</summary>
		/// <param name="connection">基础框架配置的数据库连接名称</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <returns>返回 AbstractAccess 子类的实例</returns>
		protected abstract AbstractAccess CreateAccess(string connection, ST.IsolationLevel isolationLevel);

		/// <summary>创建 AbstractAccess 子类实例</summary>
		/// <param name="connection">基础框架配置的数据库连接名称</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <param name="second">一个 int 类型的值，该值表示事务 <see cref="ST.CommittableTransaction"/> 的超时时间限制，单位秒。</param>
		/// <returns>返回 AbstractAccess 子类的实例</returns>
		protected abstract AbstractAccess CreateAccess(string connection, ST.IsolationLevel isolationLevel, int second);

		#region 新增数据
		/// <summary>执行 INSERT 命令，新增数据实体</summary>
		/// <param name="entity">需要新增的 <typeparamref name="T"/> 类实例</param>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public Result Create<T>(T entity) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.Create<T>(entity);
			}
		}

		/// <summary>执行 INSERT 命令，新增数据实体</summary>
		/// <param name="entities">需要新增的 <typeparamref name="T"/> 类实例数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public Result Create<T>(T[] entities) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, true))
			{
				Result result = ac.Create<T>(entities);
				ac.SetComplete();
				return result;
			}
		}

		/// <summary>使用指定事务隔离级别执行 INSERT 命令，新增数据实体</summary>
		/// <param name="entities">需要新增的 <typeparamref name="T"/> 类实例数组</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public Result Create<T>(T[] entities, ST.IsolationLevel isolationLevel) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, isolationLevel))
			{
				Result result = ac.Create<T>(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>使用指定事务隔离级别和超时时间执行 INSERT 命令，新增数据实体</summary>
		/// <param name="entities">需要新增的 <typeparamref name="T"/> 类实例数组</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <param name="second">一个 int 类型的值，该值表示事务 <see cref="ST.CommittableTransaction"/> 的超时时间限制，单位秒。</param>
		/// <returns>返回 AbstractAccess 子类的实例</returns>
		public Result Create<T>(T[] entities, ST.IsolationLevel isolationLevel, int second) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, isolationLevel, second))
			{
				Result result = ac.Create<T>(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>执行 INSERT 命令，新增数据实体</summary>
		/// <param name="row">包含数据的键值对数组</param>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public Result CreateCore(BaseTableRowType row)
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.CreateCore(row);
			}
		}
		#endregion

		#region 更新数据
		/// <summary>执行 UPDATE 命令，更新数据实体</summary>
		/// <param name="entity">需要更新的 <typeparamref name="T"/> 类实例</param>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public Result Update<T>(T entity) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.Update(entity);
			}
		}

		/// <summary>执行 UPDATE 命令，更新数据实体</summary>
		/// <param name="entities">需要更新的 <typeparamref name="T"/> 类实例数组</param>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public Result Update<T>(T[] entities) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, true))
			{
				Result result = ac.Update(entities);
				ac.SetComplete();
				return result;
			}
		}

		/// <summary>使用指定事务隔离级别执行 UPDATE 命令，更新数据实体</summary>
		/// <param name="entities">需要更新的 <typeparamref name="T"/> 类实例数组</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public Result Update<T>(T[] entities, ST.IsolationLevel isolationLevel) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, isolationLevel))
			{
				Result result = ac.Update<T>(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>使用指定事务隔离级别和超时时间执行 UPDATE 命令，更新数据实体</summary>
		/// <param name="entities">需要更新的 <typeparamref name="T"/> 类实例数组</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <param name="second">一个 int 类型的值，该值表示事务 <see cref="ST.CommittableTransaction"/> 的超时时间限制，单位秒。</param>
		/// <returns>返回 AbstractAccess 子类的实例</returns>
		public Result Update<T>(T[] entities, ST.IsolationLevel isolationLevel, int second) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, isolationLevel, second))
			{
				Result result = ac.Update<T>(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>执行 UPDATE 命令，更新数据实体</summary>
		/// <param name="objArray">包含数据的键值对数组</param>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public Result UpdateCore(BaseTableRowType objArray)
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.UpdateCore(objArray);
			}
		}
		#endregion

		#region 删除数据
		/// <summary>执行 DELETE 命令，删除数据实体</summary>
		/// <param name="entity">需要删除的 <typeparamref name="T"/> 类实例</param>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public Result Delete<T>(T entity) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.Delete(entity);
			}
		}

		/// <summary>执行 DELETE 命令，删除数据实体</summary>
		/// <param name="entities">需要删除的 <typeparamref name="T"/> 类实例数组</param>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public Result Delete<T>(T[] entities) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, true))
			{
				Result result = ac.Delete(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>执行 DELETE 命令，删除数据实体</summary>
		/// <param name="entities">需要删除的 <typeparamref name="T"/> 类实例数组</param>
		/// <param name="startTransaction">是否启用事务</param>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public Result Delete<T>(T[] entities, bool startTransaction) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, startTransaction))
			{
				Result result = ac.Delete(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>使用指定事务隔离级别执行 DELETE 命令，删除数据实体</summary>
		/// <param name="entities">需要删除的 <typeparamref name="T"/> 类实例数组</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public Result Delete<T>(T[] entities, ST.IsolationLevel isolationLevel) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, isolationLevel))
			{
				Result result = ac.Delete<T>(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>使用指定事务隔离级别和超时时间执行 DELETE 命令，更新数据实体</summary>
		/// <param name="entities">需要删除的 <typeparamref name="T"/> 类实例数组</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <param name="second">一个 int 类型的值，该值表示事务 <see cref="ST.CommittableTransaction"/> 的超时时间限制，单位秒。</param>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public Result Delete<T>(T[] entities, ST.IsolationLevel isolationLevel, int second) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, isolationLevel, second))
			{
				Result result = ac.Delete<T>(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>
		/// 删除数据
		/// </summary>
		/// <param name="row">包含数据的键值对数组</param>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public Result DeleteCore(BaseTableRowType row)
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.DeleteCore(row);
			}
		}
		#endregion

		#region 按关键字查询数据
		/// <summary>
		/// 按关键字查询记录
		/// </summary>
		/// <param name="entity">需要填充的Basic.Entity.AbstractEntity类实例</param>
		/// <returns>返回查询到的数据表</returns>
		public virtual bool SearchByKey(AbstractEntity entity)
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.SearchByKey(entity);
			}
		}

		/// <summary>
		/// 按关键字查询记录
		/// </summary>
		/// <param name="joinCommand">需要做连接的 JoinCommand 类命令实例。</param>
		/// <param name="entity">需要填充的Basic.Entity.AbstractEntity类实例</param>
		/// <returns>返回查询到的数据表</returns>
		public virtual bool SearchByKey(AbstractEntity entity, JoinCommand joinCommand)
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.SearchByKey(entity, joinCommand);
			}
		}

		/// <summary>
		/// 按关键字查询符合条件的记录
		/// </summary>
		/// <typeparam name="TR">表示强类型的 Basic.Tables.BaseTableRowType 实例。</typeparam>
		/// <param name="table">需要填充的 Basic.Tables.BaseTableType&lt;TR&gt; 类实例</param>
		/// <param name="condition">包含数据的键值对数组</param>
		/// <returns>返回强类型的 Basic.Tables.BaseTableRowType 实例。</returns>
		public virtual bool SearchByKey<TR>(BaseTableType<TR> table, AbstractCondition condition) where TR : BaseTableRowType
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.SearchByKey(table, condition);
			}
		}

		/// <summary>
		/// 按关键字查询符合条件的记录
		/// </summary>
		/// <typeparam name="TR">表示强类型的 Basic.Tables.BaseTableRowType 实例。</typeparam>
		/// <param name="table">需要填充的 Basic.Tables.BaseTableType&lt;TR&gt; 类实例</param>
		/// <param name="joinCommand">需要对表做连接的命令。</param>
		/// <param name="condition">包含数据的键值对数组</param>
		/// <returns>返回强类型的 Basic.Tables.BaseTableRowType 实例。</returns>
		public virtual bool SearchByKey<TR>(BaseTableType<TR> table, JoinCommand joinCommand, AbstractCondition condition) where TR : BaseTableRowType
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.SearchByKey(table, joinCommand, condition);
			}
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		public virtual IPagination<T> GetPagination<T>(AbstractCondition condition) where T : AbstractEntity, new()
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.GetEntities<T>(condition).ToPagination();
			}
		}

		/// <summary>
		/// 查询数据库表所有记录
		/// </summary>
		/// <returns>返回填充的数据集</returns>
		public virtual IPagination<T> GetPagination<T>() where T : AbstractEntity, new()
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.GetEntities<T>(0, 0).ToPagination();
			}
		}

		/// <summary>
		/// 查询数据库表所有记录
		/// </summary>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回填充的数据集</returns>
		public IPagination<T> GetPagination<T>(int pageSize, int pageIndex) where T : AbstractEntity, new()
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.GetEntities<T>(pageSize, pageIndex).ToPagination();
			}
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="entity">实体类实例，其中包含了命令所需的参数。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		public IPagination<T> GetPagination<T>(AbstractEntity entity, int pageSize, int pageIndex) where T : AbstractEntity, new()
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return ac.GetEntities<T>(entity, pageSize, pageIndex).ToPagination();
			}
		}
		#endregion

		#region 新增数据(异步方法)

		/// <summary>执行 INSERT 命令，新增数据实体</summary>
		/// <param name="entity">需要新增的实体类实例或继承于Basic.Entity.AbstractEntity类的实例</param>
		/// <example><![CDATA[Result result = await context.CreateAsync(entity);]]></example>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public async Task<Result> CreateAsync(AbstractEntity entity)
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return await ac.CreateAsync(entity);
			}
		}

		/// <summary>执行 INSERT 命令，新增数据实体</summary>
		/// <param name="row">包含数据的键值对数组</param>
		/// <example><![CDATA[Result result = await context.CreateAsync(entity);]]></example>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public async Task<Result> CreateCoreAsync(BaseTableRowType row)
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return await ac.CreateCoreAsync(row);
			}
		}

		///// <summary>
		///// 使用指定的命令新增数据实体
		///// </summary>
		///// <param name="entities">包含数据的键值对数组</param>
		///// <example><![CDATA[Result result = await context.CreateAsync(entities);]]></example>
		///// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		//public async Task<Result> BatchCreateAsync<T>(T[] entities) where T : AbstractEntity
		//{
		//	using (AbstractAccess ac = CreateAccess(Connection, true))
		//	{
		//		Result result = await ac.CreateAsync(entities);
		//		if (result.Failure) { return result; }
		//		return ac.SetComplete();

		//	}
		//}

		/// <summary>执行 INSERT 命令，新增数据实体</summary>
		/// <param name="entities">包含数据的键值对数组</param>
		/// <example><![CDATA[Result result = await context.CreateAsync(entities);]]></example>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public async Task<Result> CreateAsync<T>(T[] entities) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, true))
			{
				Result result = await ac.CreateAsync(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();

			}
		}

		/// <summary>使用指定事务隔离级别执行 INSERT 命令，新增数据实体</summary>
		/// <param name="entities">需要新增的 <typeparamref name="T"/> 类实例数组</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public async Task<Result> CreateAsync<T>(T[] entities, ST.IsolationLevel isolationLevel) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, isolationLevel))
			{
				Result result = await ac.CreateAsync<T>(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>使用指定事务隔离级别和超时时间执行 INSERT 命令，新增数据实体</summary>
		/// <param name="entities">需要新增的 <typeparamref name="T"/> 类实例数组</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <param name="second">一个 int 类型的值，该值表示事务 <see cref="ST.CommittableTransaction"/> 的超时时间限制，单位秒。</param>
		/// <returns>返回 AbstractAccess 子类的实例</returns>
		public async Task<Result> CreateAsync<T>(T[] entities, ST.IsolationLevel isolationLevel, int second) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, isolationLevel, second))
			{
				Result result = await ac.CreateAsync<T>(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}
		#endregion

		#region 更新数据(异步方法)
		/// <summary>执行 UPDATE 命令，更新数据实体</summary>
		/// <param name="entities">需要更新的Basic.Entity.AbstractEntity类实例数组</param>
		/// <example><![CDATA[Result result = await context.UpdateAsync(entitys);]]></example>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public async Task<Result> UpdateAsync(params AbstractEntity[] entities)
		{
			using (AbstractAccess ac = CreateAccess(Connection, true))
			{
				Result result = await ac.UpdateAsync(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>使用指定事务隔离级别执行 UPDATE 命令，更新数据实体</summary>
		/// <param name="entities">需要更新的 <typeparamref name="T"/> 类实例数组</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public async Task<Result> UpdateAsync<T>(T[] entities, ST.IsolationLevel isolationLevel) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, isolationLevel))
			{
				Result result = await ac.UpdateAsync<T>(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>使用指定事务隔离级别和超时时间执行 UPDATE 命令，更新数据实体</summary>
		/// <param name="entities">需要更新的 <typeparamref name="T"/> 类实例数组</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <param name="second">一个 int 类型的值，该值表示事务 <see cref="ST.CommittableTransaction"/> 的超时时间限制，单位秒。</param>
		/// <returns>返回 AbstractAccess 子类的实例</returns>
		public async Task<Result> UpdateAsync<T>(T[] entities, ST.IsolationLevel isolationLevel, int second) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, isolationLevel, second))
			{
				Result result = await ac.UpdateAsync<T>(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>执行 UPDATE 命令，更新数据实体</summary>
		/// <param name="objArray">包含数据的键值对数组</param>
		/// <example><![CDATA[Result result = await context.UpdateAsync(objArray);]]></example>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public async Task<Result> UpdateAsync(BaseTableRowType objArray)
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return await ac.UpdateAsync(objArray);
			}
		}
		#endregion

		#region 删除数据(异步方法)

		/// <summary>执行 DELETE 命令，删除数据实体</summary>
		/// <param name="entity">需要删除的GoldSoftEntity类实例或继承于GoldSoftEntity类的实例</param>
		/// <example><![CDATA[Result result = await context.DeleteAsync(entity);]]></example>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public async Task<Result> DeleteAsync(AbstractEntity entity)
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return await ac.DeleteAsync(entity);
			}
		}

		/// <summary>执行 DELETE 命令，删除数据实体</summary>
		/// <param name="entities">需要删除的Basic.Entity.AbstractEntity类实例数组</param>
		/// <example><![CDATA[Result result = await context.DeleteAsync(entity);]]></example>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public async Task<Result> DeleteAsync<T>(T[] entities) where T : AbstractEntity
		{
			TaskCompletionSource<Result> tcs = new TaskCompletionSource<Result>();
			using (AbstractAccess ac = CreateAccess(Connection, true))
			{
				Result result = await ac.DeleteAsync(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}


		/// <summary>执行 DELETE 命令，删除数据实体</summary>
		/// <param name="entities">需要删除的 <typeparamref name="T"/> 类实例数组</param>
		/// <param name="startTransaction">是否启用事务</param>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public async Task<Result> DeleteAsync<T>(T[] entities, bool startTransaction) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, startTransaction))
			{
				Result result = await ac.DeleteAsync(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>使用指定事务隔离级别执行 DELETE 命令，删除数据实体</summary>
		/// <param name="entities">包含执行命令所需的 <typeparamref name="T"/> 数据.</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public async Task<Result> DeleteAsync<T>(T[] entities, ST.IsolationLevel isolationLevel) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, isolationLevel))
			{
				Result result = await ac.DeleteAsync<T>(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>使用指定事务隔离级别和超时时间执行 DELETE 命令，删除数据实体</summary>
		/// <param name="entities">包含执行命令所需的 <typeparamref name="T"/> 数据.</param>
		/// <param name="isolationLevel">一个 <see cref="ST.IsolationLevel"/> 枚举类型的值，
		/// 该值表示事务 <see cref="ST.CommittableTransaction"/> 的隔离级别。</param>
		/// <param name="second">一个 int 类型的值，该值表示事务 <see cref="ST.CommittableTransaction"/> 的超时时间限制，单位秒。</param>
		/// <returns>返回 AbstractAccess 子类的实例</returns>
		public async Task<Result> DeleteAsync<T>(T[] entities, ST.IsolationLevel isolationLevel, int second) where T : AbstractEntity
		{
			using (AbstractAccess ac = CreateAccess(Connection, isolationLevel, second))
			{
				Result result = await ac.DeleteAsync<T>(entities);
				if (result.Failure) { return result; }
				return ac.SetComplete();
			}
		}

		/// <summary>
		/// 删除数据
		/// </summary>
		/// <param name="table">包含数据的键值对数组</param>
		/// <example><![CDATA[Result result = await context.DeleteAsync(entity);]]></example>
		/// <returns>返回执行信息包含受影响行数，错误代码，如果有分页则有总页数。</returns>
		public async Task<Result> DeleteAsync(BaseTableRowType table)
		{
			using (AbstractAccess ac = CreateAccess(Connection))
			{
				return await ac.DeleteAsync(table);
			}
		}
		#endregion
	}
}
