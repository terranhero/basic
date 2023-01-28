using System;
using System.Transactions;
using Basic.EntityLayer;
using SCC = System.Collections.Concurrent;
using ST = System.Transactions;

namespace Basic.DataAccess
{
	/// <summary>使用后台队列实现数据库CUD操作</summary>
	/// <typeparam name="TM"></typeparam>
	public abstract class AbstractQueueAccess<TM> : AbstractAccess where TM : AbstractEntity
	{
		private static SCC.ConcurrentQueue<TM> _entityQueue = new SCC.ConcurrentQueue<TM>();

		#region 构造函数
		/// <summary>
		/// 创建数据处理类实例(默认关闭事务)
		/// </summary>
		protected AbstractQueueAccess() : base() { }

		/// <summary>
		/// 创建数据处理类实例
		/// </summary>
		/// <param name="access">数据库操作类。</param>
		protected AbstractQueueAccess(AbstractDbAccess access) : base(access) { }

		/// <summary>
		/// 创建数据处理类实例
		/// </summary>
		/// <param name="startTransaction">是否启用事务</param>
		protected AbstractQueueAccess(bool startTransaction) : base(startTransaction) { }

		/// <summary>
		/// 创建数据处理类实例
		/// </summary>
		/// <param name="transaction">对用于登记的现有 Transaction 的引用。</param>
		protected AbstractQueueAccess(CommittableTransaction transaction) : base(transaction) { }

		/// <summary>
		/// 使用指定的事务选项初始化 AbstractQueueAccess 类的新实例，并启用事务。
		/// </summary>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractQueueAccess(TimeSpan timeout) : base(timeout) { }

		/// <summary>
		/// 使用指定的事务选项初始化 AbstractQueueAccess 类的新实例，并启用事务。
		/// </summary>
		/// <param name="isolationLevel">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractQueueAccess(ST.IsolationLevel isolationLevel, TimeSpan timeout) : base(isolationLevel, timeout) { }

		/// <summary>
		/// 使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。
		/// </summary>
		/// <param name="connectionName">数据库连接名称</param>
		protected AbstractQueueAccess(string connectionName) : base(connectionName) { }

		/// <summary>
		/// 使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。
		/// </summary>
		/// <param name="connectionName">数据库连接名称</param>
		/// <param name="startTransaction">是否启用事务</param>
		protected AbstractQueueAccess(string connectionName, bool startTransaction) : base(connectionName, startTransaction) { }

		/// <summary>
		/// 使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。
		/// </summary>
		/// <param name="connectionName">数据库连接字符串</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractQueueAccess(string connectionName, TimeSpan timeout) : base(connectionName, timeout) { }

		/// <summary>
		/// 使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。
		/// </summary>
		/// <param name="connectionName">数据库连接字符串</param>
		/// <param name="isolationLevel">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractQueueAccess(string connectionName, ST.IsolationLevel isolationLevel, TimeSpan timeout) : base(connectionName, isolationLevel, timeout) { }

		/// <summary>
		/// 使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。
		/// </summary>
		/// <param name="connectionName">数据库连接字符串</param>
		/// <param name="transaction">对用于登记的现有 Transaction 的引用。</param>
		protected AbstractQueueAccess(string connectionName, CommittableTransaction transaction) : base(connectionName, transaction) { }

		/// <summary>
		/// 初始化 AbstractQueueAccess 类的实例。
		/// </summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>
		protected AbstractQueueAccess(global::Basic.Interfaces.IUserContext user) :
				base(user)
		{
		}

		/// <summary>
		/// 初始化 AbstractQueueAccess 类的实例。
		/// </summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>
		/// <param name="startTransaction">是否启用事务</param>
		protected AbstractQueueAccess(global::Basic.Interfaces.IUserContext user, bool startTransaction) :
				base(user, startTransaction)
		{
		}

		/// <summary>
		/// 初始化 AbstractQueueAccess 类的实例。
		/// </summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractQueueAccess(global::Basic.Interfaces.IUserContext user, global::System.TimeSpan timeout) :
				base(user, timeout)
		{
		}

		/// <summary>
		/// 初始化 AbstractQueueAccess 类的实例。
		/// </summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>
		/// <param name="transaction">对用于登记的现有 Transaction 的引用。</param>
		protected AbstractQueueAccess(global::Basic.Interfaces.IUserContext user, global::System.Transactions.CommittableTransaction transaction) :
				base(user, transaction)
		{
		}

		/// <summary>
		/// 初始化 AbstractQueueAccess 类的实例。
		/// </summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>
		/// <param name="isolationLevel">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractQueueAccess(global::Basic.Interfaces.IUserContext user, global::System.Transactions.IsolationLevel isolationLevel, global::System.TimeSpan timeout) :
				base(user, isolationLevel, timeout)
		{
		}
		#endregion

		#region 新增数据
		/// <summary>使用指定的命令新增数据实体</summary>
		/// <param name="entities">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public void EnqueueInsert(params TM[] entities)
		{
			if (entities == null && entities.Length == 0) { return; }
			foreach (TM entity in entities) { entity.SetAdded(); _entityQueue.Enqueue(entity); }
		}

		/// <summary>使用指定的命令更新数据实体</summary>
		/// <param name="entities">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public void EnqueueUpdate(params TM[] entities)
		{
			if (entities == null && entities.Length == 0) { return; }
			foreach (TM entity in entities) { entity.SetModified(); _entityQueue.Enqueue(entity); }
		}

		/// <summary>使用指定的命令删除数据实体</summary>
		/// <param name="entities">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public void EnqueueDelete(params TM[] entities)
		{
			if (entities == null && entities.Length == 0) { return; }
			foreach (TM entity in entities) { entity.SetDeleted(); _entityQueue.Enqueue(entity); }
		}
		#endregion
	}
}
