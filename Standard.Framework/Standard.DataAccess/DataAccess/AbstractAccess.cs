using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Reflection;
using System.Transactions;
using ST = System.Transactions;
using System.Threading.Tasks;
using Basic.Exceptions;
using Basic.Collections;
using Basic.EntityLayer;
using Basic.Interfaces;
using Basic.Tables;
using System.Text;
using System.Collections.Generic;

namespace Basic.DataAccess
{
	/// <summary>
	/// 数据操作类
	/// </summary>
	public abstract class AbstractAccess : AbstractDbAccess
	{
		#region 构造函数
		/// <summary>
		/// 创建数据处理类实例(默认关闭事务)
		/// </summary>
		protected AbstractAccess() : base() { }

		/// <summary>
		/// 创建数据处理类实例
		/// </summary>
		/// <param name="access">数据库操作类。</param>
		protected AbstractAccess(AbstractDbAccess access) : base(access) { }

		/// <summary>
		/// 创建数据处理类实例
		/// </summary>
		/// <param name="startTransaction">是否启用事务</param>
		protected AbstractAccess(bool startTransaction) : base(startTransaction) { }

		/// <summary>
		/// 创建数据处理类实例
		/// </summary>
		/// <param name="transaction">对用于登记的现有 Transaction 的引用。</param>
		protected AbstractAccess(CommittableTransaction transaction) : base(transaction) { }

		/// <summary>
		/// 使用指定的事务选项初始化 AbstractAccess 类的新实例，并启用事务。
		/// </summary>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractAccess(TimeSpan timeout) : base(timeout) { }

		/// <summary>
		/// 使用指定的事务选项初始化 AbstractAccess 类的新实例，并启用事务。
		/// </summary>
		/// <param name="isolationLevel">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractAccess(ST.IsolationLevel isolationLevel, TimeSpan timeout) : base(isolationLevel, timeout) { }

		/// <summary>
		/// 使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。
		/// </summary>
		/// <param name="connectionName">数据库连接名称</param>
		protected AbstractAccess(string connectionName) : base(connectionName) { }

		/// <summary>
		/// 使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。
		/// </summary>
		/// <param name="connectionName">数据库连接名称</param>
		/// <param name="startTransaction">是否启用事务</param>
		protected AbstractAccess(string connectionName, bool startTransaction) : base(connectionName, startTransaction) { }

		/// <summary>
		/// 使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。
		/// </summary>
		/// <param name="connectionName">数据库连接字符串</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractAccess(string connectionName, TimeSpan timeout) : base(connectionName, timeout) { }

		/// <summary>
		/// 使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。
		/// </summary>
		/// <param name="connectionName">数据库连接字符串</param>
		/// <param name="isolationLevel">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractAccess(string connectionName, ST.IsolationLevel isolationLevel, TimeSpan timeout) : base(connectionName, isolationLevel, timeout) { }

		/// <summary>
		/// 使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。
		/// </summary>
		/// <param name="connectionName">数据库连接字符串</param>
		/// <param name="transaction">对用于登记的现有 Transaction 的引用。</param>
		protected AbstractAccess(string connectionName, CommittableTransaction transaction) : base(connectionName, transaction) { }

		/// <summary>
		/// 初始化 AbstractAccess 类的实例。
		/// </summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>
		protected AbstractAccess(global::Basic.Interfaces.IUserContext user) :
				base(user)
		{
		}

		/// <summary>
		/// 初始化 AbstractAccess 类的实例。
		/// </summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>
		/// <param name="startTransaction">是否启用事务</param>
		protected AbstractAccess(global::Basic.Interfaces.IUserContext user, bool startTransaction) :
				base(user, startTransaction)
		{
		}

		/// <summary>
		/// 初始化 AbstractAccess 类的实例。
		/// </summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractAccess(global::Basic.Interfaces.IUserContext user, global::System.TimeSpan timeout) :
				base(user, timeout)
		{
		}

		/// <summary>
		/// 初始化 AbstractAccess 类的实例。
		/// </summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>
		/// <param name="transaction">对用于登记的现有 Transaction 的引用。</param>
		protected AbstractAccess(global::Basic.Interfaces.IUserContext user, global::System.Transactions.CommittableTransaction transaction) :
				base(user, transaction)
		{
		}

		/// <summary>
		/// 初始化 AbstractAccess 类的实例。
		/// </summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)</param>
		/// <param name="isolationLevel">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractAccess(global::Basic.Interfaces.IUserContext user, global::System.Transactions.IsolationLevel isolationLevel, global::System.TimeSpan timeout) :
				base(user, isolationLevel, timeout)
		{
		}
		#endregion

		#region 新增数据
		/// <summary>
		/// 新增数据实体
		/// </summary>
		/// <param name="table">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Result CreateCore<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			return base.ExecuteCore<TR>(CreateCommand, table);
		}

		/// <summary>
		/// 新增数据实体
		/// </summary>
		/// <param name="row">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Result CreateCore<TR>(TR row) where TR : BaseTableRowType
		{
			return base.ExecuteCore<TR>(CreateCommand, row);
		}

		/// <summary>
		/// 使用指定的命令新增数据实体
		/// </summary>
		/// <param name="entity">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Result Create<T>(T entity) where T : AbstractEntity
		{
			return base.ExecuteNonQuery(CreateCommand, entity);
		}

		/// <summary>
		/// 使用指定的命令新增数据实体
		/// </summary>
		/// <param name="entities">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Result Create<T>(T[] entities) where T : AbstractEntity
		{
			return base.ExecuteNonQuery(CreateCommand, entities);
		}

		private StaticCommand _CreateCommand = null;
		/// <summary>
		/// 新增命令Transact-SQL结构
		/// </summary>
		/// <returns>返回Transact-SQL结构</returns>
		internal protected virtual StaticCommand CreateCommand
		{
			get
			{
				if (_CreateCommand == null) { _CreateCommand = CreateStaticCommand(CreateConfig); }
				if (_CreateCommand == null) { throw new UninitializedCommandException("Access_UninitializedCommand"); }
				return _CreateCommand;
			}
		}

		/// <summary>
		/// 使用指定的命令新增数据实体
		/// </summary>
		/// <param name="row">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Task<Result> CreateCoreAsync(BaseTableRowType row)
		{
			return base.ExecuteCoreAsync(CreateCommand, row);
		}

		/// <summary>
		/// 使用指定的命令新增数据实体
		/// </summary>
		/// <param name="table">包含需要新增的数据表实例，此实例是 BaseTableType&lt;BaseTableRowType&gt; 类的子类。</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Task<Result> CreateCoreAsync<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			return base.ExecuteCoreAsync<TR>(CreateCommand, table);
		}

		/// <summary>
		/// 使用指定的命令新增数据实体
		/// </summary>
		/// <param name="entities">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Task<Result> CreateAsync<T>(params T[] entities) where T : AbstractEntity
		{
			return base.ExecuteNonQueryAsync(CreateCommand, entities);
		}

#if NET6_0_OR_GREATER
		/// <summary>使用 BatchCommand 类批处理 INSERT 命令执行插入数据命令</summary>
		/// <typeparam name="TModel">表示 <see cref="AbstractEntity"/> 类型实例</typeparam>
		/// <remarks>使用此命令执行时，不在执行<see cref="StaticCommand"/>中 
		/// <see  cref="StaticCommand.CheckCommands">CheckCommands</see> 和 
		/// <see cref="StaticCommand.NewValues">NewValues</see> 中包含的命令
		/// 所以在执行此命令前，需要将数据有效性验证和取值命令提前执行完成 </remarks>
		/// <param name="entities">实体 <typeparamref name="TModel"/> 类数组，包含了需要执行参数的值。</param>
		/// <returns>执行Transact-SQL语句或存储过程后的返回结果。</returns>
		public async Task<Result> BatchCreateAsync<TModel>(IEnumerable<TModel> entities) where TModel : AbstractEntity
		{
			return await base.BatchAsync(CreateCommand, entities);
		}
#endif
		#endregion

		#region 更新数据
		private StaticCommand _UpdateCommand = null;
		/// <summary>
		/// 创建更新命令Transact-SQL结构
		/// </summary>
		/// <returns>返回Transact-SQL结构</returns>
		protected virtual StaticCommand UpdateCommand
		{
			get
			{
				if (_UpdateCommand == null) { _UpdateCommand = CreateStaticCommand(ModifyConfig); }
				if (_UpdateCommand == null) { throw new UninitializedCommandException("Access_UninitializedCommand"); }
				return _UpdateCommand;
			}
		}
		/// <summary>
		/// 更新数据实体
		/// </summary>
		/// <param name="entity">数据实体</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Result Update<T>(T entity) where T : AbstractEntity
		{
			return base.ExecuteNonQuery(UpdateCommand, entity);
		}

		/// <summary>
		/// 更新数据实体
		/// </summary>
		/// <param name="entities">数据实体</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Result Update<T>(T[] entities) where T : AbstractEntity
		{
			return base.ExecuteNonQuery(UpdateCommand, entities);
		}

		/// <summary>使用指定的命令删除数据实体</summary>
		/// <param name="anonObject">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Result Update(object anonObject)
		{
			return base.ExecuteNonQuery(UpdateCommand, anonObject);
		}

		/// <summary>
		/// 更新数据实体
		/// </summary>
		/// <param name="row">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Result UpdateCore<TR>(TR row) where TR : BaseTableRowType
		{
			return base.ExecuteCore<TR>(UpdateCommand, row);
		}

		/// <summary>
		/// 更新数据实体
		/// </summary>
		/// <param name="table">包含需要新增的数据表实例，此实例是 BaseTableType&lt;BaseTableRowType&gt; 类的子类。</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Result UpdateCore<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			return base.ExecuteCore<TR>(UpdateCommand, table);
		}

		/// <summary>
		/// 使用指定的命令新增数据实体
		/// </summary>
		/// <param name="row">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Task<Result> UpdateAsync(BaseTableRowType row)
		{
			return base.ExecuteCoreAsync(UpdateCommand, row);
		}

		/// <summary>
		/// 使用指定的命令新增数据实体
		/// </summary>
		/// <param name="entities">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Task<Result> UpdateAsync<T>(params T[] entities) where T : AbstractEntity
		{
			return base.ExecuteNonQueryAsync(UpdateCommand, entities);
		}

		/// <summary>使用指定的命令删除数据实体</summary>
		/// <param name="anonObject">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Task<Result> UpdateAsync(object anonObject)
		{
			return base.ExecuteNonQueryAsync(UpdateCommand, anonObject);
		}
		#endregion

		#region 删除数据
		/// <summary>
		/// 使用指定的命令新增数据实体
		/// </summary>
		/// <param name="row">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Task<Result> DeleteAsync(BaseTableRowType row)
		{
			return base.ExecuteCoreAsync(DeleteCommand, row);
		}

		///// <summary>
		///// 使用指定的命令新增数据实体
		///// </summary>
		///// <param name="entities">包含数据的键值对数组</param>
		///// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		//public virtual Task<Result> DeleteAsync<T>(IEnumerable<T> entities) where T : AbstractEntity
		//{
		//	return base.ExecuteNonQueryAsync(DeleteCommand, entities);
		//}

		/// <summary>
		/// 使用指定的命令新增数据实体
		/// </summary>
		/// <param name="entities">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Task<Result> DeleteAsync<T>(params T[] entities) where T : AbstractEntity
		{
			return base.ExecuteNonQueryAsync(DeleteCommand, entities);
		}

		/// <summary>使用指定的命令删除数据实体</summary>
		/// <param name="anonObject">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Task<Result> DeleteAsync(object anonObject)
		{
			return base.ExecuteNonQueryAsync(DeleteCommand, anonObject);
		}

		private StaticCommand _DeleteCommand = null;
		/// <summary>
		/// 创建删除命令Transact-SQL结构
		/// </summary>
		/// <returns>返回Transact-SQL结构</returns>
		protected virtual StaticCommand DeleteCommand
		{
			get
			{
				if (_DeleteCommand == null) { _DeleteCommand = CreateStaticCommand(RemoveConfig); }
				if (_DeleteCommand == null) { throw new UninitializedCommandException("Access_UninitializedCommand"); }
				return _DeleteCommand;
			}
		}

		/// <summary>
		/// 根据参数化数组初始化SqlStruct实例默认值
		/// </summary>
		/// <param name="dataCommand">需执行的Transact-SQL语句结构</param>
		/// <param name="paramArray">参数数组</param>
		internal void InitDataCommandDefaultValue(StaticCommand dataCommand, object[] paramArray)
		{
			if (paramArray != null && paramArray.Length > 0 && paramArray.Length >= dataCommand.Parameters.Count)
			{
				int index = 0;
				foreach (DbParameter sqlParam in dataCommand.Parameters)
				{
					sqlParam.Value = paramArray[index++];
				}
			}
		}

		/// <summary>执行 DELETE 命令，删除数据实体</summary>
		/// <param name="anonObject">匿名数据实体</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		protected Result Delete(object anonObject)
		{
			return ExecuteNonQuery(DeleteCommand, anonObject);
		}

		/// <summary>执行 DELETE 命令，删除数据实体</summary>
		/// <param name="entity">数据实体</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Result Delete<T>(T entity) where T : AbstractEntity
		{
			return base.ExecuteNonQuery(DeleteCommand, entity);
		}

		/// <summary>执行 DELETE 命令，删除数据实体</summary>
		/// <param name="entities">数据实体</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Result Delete<T>(T[] entities) where T : AbstractEntity
		{
			return base.ExecuteNonQuery(DeleteCommand, entities);
		}

		/// <summary>执行 DELETE 命令，删除数据实体</summary>
		/// <param name="row">包含数据的键值对数组</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Result DeleteCore<TR>(TR row) where TR : BaseTableRowType
		{
			return base.ExecuteCore<TR>(DeleteCommand, row);
		}

		/// <summary>执行 DELETE 命令，删除数据实体</summary>
		/// <param name="table">包含需要新增的数据表实例，此实例是 BaseTableType&lt;BaseTableRowType&gt; 类的子类。</param>
		/// <returns>如果执行成功返回0，执行失败则返回错误代码。</returns>
		public virtual Result DeleteCore<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			return base.ExecuteCore<TR>(DeleteCommand, table);
		}
		#endregion

		#region 使用关键字查询数据库记录
		/// <summary>
		/// 按关键字查询记录的命令结构
		/// </summary>
		private StaticCommand _SelectByKeyCommand = null;
		/// <summary>
		/// 创建按关键字查询记录的命令结构
		/// </summary>
		/// <returns>返回Transact-SQL结构</returns>
		protected virtual StaticCommand SelectByKeyCommand
		{
			get
			{
				if (_SelectByKeyCommand == null)
					_SelectByKeyCommand = CreateStaticCommand(SelectByKeyConfig);
				return _SelectByKeyCommand;
			}
		}

		/// <summary>
		/// 按关键字查询记录
		/// </summary>
		/// <param name="entity">需要填充的Basic.Entity.AbstractEntity类实例</param>
		/// <returns>返回查询到的数据表</returns>
		public bool SearchByKey(AbstractEntity entity) { return this.SearchByKey(entity, (AbstractCondition)null); }

		/// <summary>
		/// 按关键字查询记录
		/// </summary>
		/// <param name="entity">需要填充的Basic.Entity.AbstractEntity类实例</param>
		/// <param name="condition">查询条件类，包含当前查询需要的参数。</param>
		/// <returns>返回查询到的数据表</returns>
		public bool SearchByKey(AbstractEntity entity, AbstractCondition condition)
		{
			using (DynamicCommand dynamicCommand = SelectAllCommand)
			{
				IReadOnlyCollection<EntityPropertyMeta> propertyKeys = entity.GetPrimaryKey();
				if (propertyKeys == null || propertyKeys.Count == 0) { return false; }
				StringBuilder builder = new StringBuilder(500);
				dynamicCommand.InitializeParameters();
				if (condition != null) { dynamicCommand.ResetParameters(condition); }
				if (entity != null) { dynamicCommand.ResetParameters(entity); }
				foreach (EntityPropertyMeta info in propertyKeys)
				{
					ColumnMappingAttribute cma = info.Mapping;
					ColumnAttribute column = info.Column;
					if (cma == null && column == null) { throw new AttributeException("ColumnMappingAttribute_NotExists", entity.GetType(), info.Name); }
					if (cma != null)
					{
						DbParameter parameter = dynamicCommand.CreateParameter(cma);
						if (builder.Length > 0) { builder.Append(" AND "); }
						if (!string.IsNullOrEmpty(cma.TableAlias))
							builder.Append(cma.TableAlias).Append(".");
						builder.Append(cma.SourceColumn).Append("=").Append(parameter.ParameterName);
						dynamicCommand.Parameters.Add(parameter);
					}
					else if (column != null)
					{
						DbParameter parameter = dynamicCommand.CreateParameter(column);
						if (builder.Length > 0) { builder.Append(" AND "); }
						if (!string.IsNullOrEmpty(column.TableName))
							builder.Append(column.TableName).Append(".");
						builder.Append(column.ColumnName).Append("=").Append(parameter.ParameterName);
						dynamicCommand.Parameters.Add(parameter);
					}
				}
				dynamicCommand.TempWhereText = builder.ToString();
				dynamicCommand.InitializeCommandText(0, 0);
				return base.SearchEntity(dynamicCommand, entity);
			}
		}

		/// <summary>
		/// 按关键字查询记录
		/// </summary>
		/// <param name="entity">需要填充的Basic.Entity.AbstractEntity类实例</param>
		/// <param name="joinCmd">需要做连接的 JoinCommand 类命令实例。</param>
		/// <returns>返回查询到的数据表</returns>
		public bool SearchByKey(AbstractEntity entity, JoinCommand joinCmd) { return this.SearchByKey(entity, null, joinCmd); }

		/// <summary>
		/// 按关键字查询记录
		/// </summary>
		/// <param name="entity">需要填充的Basic.Entity.AbstractEntity类实例</param>
		/// <param name="condition">查询条件类，包含当前查询需要的参数。</param>
		/// <param name="joinCmd">需要做连接的 JoinCommand 类命令实例。</param>
		/// <returns>返回查询到的数据表</returns>
		public bool SearchByKey(AbstractEntity entity, AbstractCondition condition, JoinCommand joinCmd)
		{
			using (DynamicCommand dynamicCommand = SelectAllCommand)
			{
				dynamicCommand.SetJoinCommand(joinCmd);
				IReadOnlyCollection<EntityPropertyMeta> propertyKeys = entity.GetPrimaryKey();
				if (propertyKeys == null || propertyKeys.Count == 0) { return false; }
				StringBuilder builder = new StringBuilder(500);
				dynamicCommand.InitializeParameters();
				if (condition != null) { dynamicCommand.ResetParameters(condition); }
				if (entity != null) { dynamicCommand.ResetParameters(entity); }
				foreach (EntityPropertyMeta info in propertyKeys)
				{
					ColumnMappingAttribute cma = info.Mapping;
					ColumnAttribute column = info.Column;
					if (cma == null && column == null) { throw new AttributeException("ColumnMappingAttribute_NotExists", entity.GetType(), info.Name); }
					if (cma != null)
					{
						DbParameter parameter = dynamicCommand.CreateParameter(cma);
						if (builder.Length > 0) { builder.Append(" AND "); }
						if (!string.IsNullOrEmpty(cma.TableAlias))
							builder.Append(cma.TableAlias).Append(".");
						builder.Append(cma.SourceColumn).Append("=").Append(parameter.ParameterName);
						dynamicCommand.Parameters.Add(parameter);
					}
					else if (column != null)
					{
						DbParameter parameter = dynamicCommand.CreateParameter(column);
						if (builder.Length > 0) { builder.Append(" AND "); }
						if (!string.IsNullOrEmpty(column.TableName))
							builder.Append(column.TableName).Append(".");
						builder.Append(column.ColumnName).Append("=").Append(parameter.ParameterName);
						dynamicCommand.Parameters.Add(parameter);
					}
				}
				dynamicCommand.TempWhereText = builder.ToString();
				dynamicCommand.InitializeCommandText(0, 0);
				return base.SearchEntity(dynamicCommand, entity);
			}
		}

		/// <summary>
		/// 按关键字查询符合条件的记录
		/// </summary>
		/// <typeparam name="TR">表示强类型的 Basic.Tables.BaseTableRowType 实例。</typeparam>
		/// <param name="table">需要填充的 Basic.Tables.BaseTableType&lt;TR&gt; 类实例</param>
		/// <param name="condition">包含数据的键值对数组</param>
		/// <returns>返回强类型的 Basic.Tables.BaseTableRowType 实例。</returns>
		public bool SearchByKey<TR>(BaseTableType<TR> table, AbstractCondition condition) where TR : BaseTableRowType
		{
			using (DynamicCommand dynamicCommand = SelectAllCommand)
			{
				Type type = typeof(TR);
				DataColumn[] promaryKey = table.PrimaryKey;
				if (promaryKey == null || promaryKey.Length == 0) { return false; }
				StringBuilder builder = new StringBuilder(500);
				dynamicCommand.InitializeParameters();
				foreach (PropertyInfo propertyInfo in type.GetProperties())
				{
					if (Attribute.IsDefined(propertyInfo, typeof(PrimaryKeyAttribute)))
					{
						ColumnMappingAttribute cma = (ColumnMappingAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(ColumnMappingAttribute));
						if (cma == null) { throw new AttributeException("ColumnMappingAttribute_NotExists", type, propertyInfo.Name); }
						DbParameter parameter = dynamicCommand.CreateParameter(cma);
						if (builder.Length > 0) { builder.Append(" AND "); }
						if (!string.IsNullOrEmpty(cma.TableAlias))
							builder.Append(cma.TableAlias).Append(".");
						builder.Append(cma.SourceColumn).Append("=").Append(parameter.ParameterName);
						dynamicCommand.Parameters.Add(parameter);
					}
				}
				dynamicCommand.TempWhereText = builder.ToString();
				if (condition != null) { dynamicCommand.ResetParameters(condition); }
				dynamicCommand.InitializeCommandText(0, 0);
				return base.Fill(table, dynamicCommand, condition) > 0;
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
		public bool SearchByKey<TR>(BaseTableType<TR> table, JoinCommand joinCommand, AbstractCondition condition) where TR : BaseTableRowType
		{
			using (DynamicCommand dynamicCommand = SelectAllCommand)
			{
				Type type = typeof(TR);
				dynamicCommand.SetJoinCommand(joinCommand);
				dynamicCommand.InitializeParameters();
				DataColumn[] promaryKey = table.PrimaryKey;
				if (promaryKey == null || promaryKey.Length == 0) { return false; }
				StringBuilder builder = new StringBuilder(500);
				foreach (PropertyInfo propertyInfo in type.GetProperties())
				{
					if (Attribute.IsDefined(propertyInfo, typeof(PrimaryKeyAttribute)))
					{
						ColumnMappingAttribute cma = (ColumnMappingAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(ColumnMappingAttribute));
						if (cma == null) { throw new AttributeException("ColumnMappingAttribute_NotExists", type, propertyInfo.Name); }
						DbParameter parameter = dynamicCommand.CreateParameter(cma);
						if (builder.Length > 0) { builder.Append(" AND "); }
						if (!string.IsNullOrEmpty(cma.TableAlias))
							builder.Append(cma.TableAlias).Append(".");
						builder.Append(cma.SourceColumn).Append("=").Append(parameter.ParameterName);
						dynamicCommand.Parameters.Add(parameter);
					}
				}
				dynamicCommand.TempWhereText = builder.ToString();
				if (condition != null) { dynamicCommand.ResetParameters(condition); }
				dynamicCommand.InitializeCommandText(0, 0);
				return base.Fill(table, dynamicCommand, condition) > 0;
			}
		}

		///// <summary>
		///// 按关键字查询记录
		///// </summary>
		///// <param name="entity">需要填充的Basic.Entity.AbstractEntity类实例</param>
		///// <returns>返回查询到的数据表</returns>
		//public bool FillByKey(AbstractEntity entity)
		//{
		//    return base.SearchEntity(SelectByKeyCommand, entity);
		//}

		///// <summary>
		///// 按关键字查询符合条件的记录
		///// </summary>
		///// <typeparam name="TR">表示强类型的 Basic.Tables.BaseTableRowType 实例。</typeparam>
		///// <param name="table">需要填充的 Basic.Tables.BaseTableType&lt;TR&gt; 类实例</param>
		///// <param name="objArray">包含数据的键值对数组</param>
		///// <returns>返回强类型的 Basic.Tables.BaseTableRowType 实例。</returns>
		//public TR FillByKey<TR>(BaseTableType<TR> table, params object[] objArray) where TR : BaseTableRowType
		//{
		//    StaticCommand dataCommand = SelectByKeyCommand;
		//    InitDataCommandDefaultValue(dataCommand, objArray);
		//    this.Fill(table, dataCommand, 0, 0);
		//    return table.Rows.Find(objArray) as TR;
		//}
		#endregion

		#region 查询所有数据库记录
		/// <summary>
		/// 查询所有数据库记录的命令结构
		/// </summary>
		private DynamicCommand _SelectAllCommand = null;
		/// <summary>
		/// 查询所有数据库记录的命令结构
		/// </summary>
		/// <returns>返回Transact-SQL结构</returns>
		protected virtual DynamicCommand SelectAllCommand
		{
			get
			{
				if (_SelectAllCommand == null)
					_SelectAllCommand = CreateDynamicCommand(SearchTableConfig);
				return _SelectAllCommand;
			}
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自 Basic.Tables.BaseTableRowType 的强类型 DataRow 实例。</typeparam>
		/// <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, AbstractCondition condition) where T : BaseTableRowType
		{
			return base.GetDataTable<T>(table, SelectAllCommand, condition);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自 Basic.Tables.BaseTableRowType 的强类型 DataRow 实例。</typeparam>
		/// <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="dynamicObject">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, object dynamicObject) where T : BaseTableRowType
		{
			return GetDataTable<T>(table, SelectAllCommand, dynamicObject);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自 Basic.Tables.BaseTableRowType 的强类型 DataRow 实例。</typeparam>
		/// <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table) where T : BaseTableRowType
		{
			return base.GetDataTable<T>(table, SelectAllCommand);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Tables.BaseTableRowType的实体类。</typeparam>
		/// <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="joinCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, JoinCommand joinCommand, AbstractCondition condition)
			where T : BaseTableRowType
		{
			return base.GetDataTable<T>(table, SelectAllCommand, joinCommand, condition);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Tables.BaseTableRowType的实体类。</typeparam>
		/// <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="joinCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="dynamicObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回可查询的实体列表。</returns>
		protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, JoinCommand joinCommand, object dynamicObject)
			where T : BaseTableRowType
		{
			return base.GetDataTable<T>(table, SelectAllCommand, joinCommand, dynamicObject);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自 Basic.Entity.AbstractEntity 的实例。</typeparam>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryEntities<T> GetEntities<T>(AbstractCondition condition) where T : AbstractEntity, new()
		{
			return base.GetEntities<T>(SelectAllCommand, condition);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自 Basic.Entity.AbstractEntity 的实例。</typeparam>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryEntities<T> GetEntities<T>(int pageSize, int pageIndex) where T : AbstractEntity, new()
		{
			return base.GetEntities<T>(SelectAllCommand, pageSize, pageIndex);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自 Basic.Entity.AbstractEntity 的实例。</typeparam>
		/// <param name="entity">实体类实例，其中包含了命令所需的参数。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryEntities<T> GetEntities<T>(AbstractEntity entity, int pageSize, int pageIndex) where T : AbstractEntity, new()
		{
			return base.GetEntities<T>(SelectAllCommand, entity, pageSize, pageIndex);
		}
		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自 Basic.Entity.AbstractEntity 的实例。</typeparam>
		/// <param name="dynamicObject">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryEntities<T> GetEntities<T>(object dynamicObject) where T : AbstractEntity, new()
		{
			return GetEntities<T>(SelectAllCommand, dynamicObject);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自 Basic.Entity.AbstractEntity 的实例。</typeparam>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryEntities<T> GetEntities<T>() where T : AbstractEntity, new()
		{
			return base.GetEntities<T>(SelectAllCommand, new { });
		}
		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Tables.BaseTableRowType的实体类。</typeparam>
		/// <param name="joinCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryEntities<T> GetEntities<T>(JoinCommand joinCommand, AbstractCondition condition)
			where T : AbstractEntity, new()
		{
			return base.GetEntities<T>(SelectAllCommand, joinCommand, condition);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Tables.BaseTableRowType的实体类。</typeparam>
		/// <param name="joinCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="dynamicObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryEntities<T> GetEntities<T>(JoinCommand joinCommand, object dynamicObject)
			where T : AbstractEntity, new()
		{
			return base.GetEntities<T>(SelectAllCommand, joinCommand, dynamicObject);
		}
		#endregion

		#region 查询所有数据库记录并通过Join连接通用表
		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自 Basic.Entity.AbstractEntity 的实例。</typeparam>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryEntities<T> GetJoinEntities<T>(AbstractCondition condition) where T : AbstractEntity, new()
		{
			return base.GetJoinEntities<T>(SelectAllCommand, condition);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自 Basic.Entity.AbstractEntity 的实例。</typeparam>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryEntities<T> GetJoinEntities<T>(int pageSize, int pageIndex) where T : AbstractEntity, new()
		{
			return base.GetJoinEntities<T>(SelectAllCommand, pageSize, pageIndex);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自 Basic.Entity.AbstractEntity 的实例。</typeparam>
		/// <param name="dynamicObject">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryEntities<T> GetJoinEntities<T>(object dynamicObject) where T : AbstractEntity, new()
		{
			return GetJoinEntities<T>(SelectAllCommand, dynamicObject);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自 Basic.Entity.AbstractEntity 的实例。</typeparam>
		/// <returns>返回可查询的实体列表。</returns>
		public QueryEntities<T> GetJoinEntities<T>() where T : AbstractEntity, new()
		{
			return base.GetJoinEntities<T>(SelectAllCommand, new { });
		}
		#endregion
	}
}
