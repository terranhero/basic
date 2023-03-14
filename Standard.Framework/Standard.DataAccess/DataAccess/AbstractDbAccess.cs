using System;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Basic.Collections;
using Basic.Configuration;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Exceptions;
using Basic.Interfaces;
using Basic.Messages;
using Basic.Tables;
using ST = System.Transactions;

namespace Basic.DataAccess
{
	/// <summary>
	/// 数据操作类
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0019:使用模式匹配", Justification = "<挂起>")]
	public abstract class AbstractDbAccess : global::System.IDisposable
#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
, System.IAsyncDisposable
#endif
	{
		#region 常量
		/// <summary>
		/// 新增结构配置节名称
		/// </summary>
		internal protected const string NewKeyConfig = "NewKeyConfig";

		/// <summary>
		/// 新增结构配置节名称
		/// </summary>
		internal protected const string CreateName = "Create";

		/// <summary>
		/// 新增结构配置节名称
		/// </summary>
		internal protected const string CreateConfig = "CreateConfig";

		/// <summary>
		/// 更新结构配置节名称
		/// </summary>
		internal protected const string ModifyName = "Update";

		/// <summary>
		/// 更新结构配置节名称
		/// </summary>
		internal protected const string ModifyConfig = "UpdateConfig";

		/// <summary>
		/// 删除结构配置节名称
		/// </summary>
		internal protected const string RemoveName = "Delete";

		/// <summary>
		/// 删除结构配置节名称
		/// </summary>
		internal protected const string RemoveConfig = "DeleteConfig";

		/// <summary>
		/// 删除结构配置节名称
		/// </summary>
		internal protected const string DeleteByFKeyConfig = "DeleteByFKeyConfig";

		/// <summary>
		/// 使用关键字为条件查询记录结构配置节名称
		/// </summary>
		internal protected const string SelectByKeyConfig = "SelectByKeyConfig";

		/// <summary>
		/// 使用外关键字为条件查询记录结构配置节名称
		/// </summary>
		internal protected const string SelectByFKeyConfig = "SelectByFKeyConfig";

		/// <summary>
		/// 查询所有记录结构配置节名称
		/// </summary>
		internal protected const string SelectAllConfig = "SelectAllConfig";

		/// <summary>
		/// 查询所有记录结构配置节名称
		/// </summary>
		internal protected const string SearchTableName = "SearchTable";

		/// <summary>
		/// 查询所有记录结构配置节名称
		/// </summary>
		internal protected const string SearchTableConfig = "SearchTableConfig";
		#endregion

		#region 属性
		/// <summary>数据库命令执行结果。</summary>
		private readonly Result _Result;

		/// <summary>数据库连接工厂类</summary>
		private readonly ConnectionFactory _ConnectionFactory;

		/// <summary>数据库数据表配置类</summary>
		private readonly TableConfiguration _TableInfo;

		/// <summary>表示 ConfigurationAttribute 类型对象的实例化信息</summary>
		private readonly ConfigurationInfo _ConfigInfo;

		/// <summary>表示当前用户上下文信息</summary>
		private readonly IUserContext _User;

		/// <summary>表示当前连接名称</summary>
		public string ConnectionName { get { return _ConfigInfo.ConnectionName; } }

		/// <summary>事务是否已经完成</summary>
		private TransactionState CommitTranState = TransactionState.NotExistTransaction;

		/// <summary>
		/// 获取分布式事务实例
		/// </summary>
		private CommittableTransaction commitTransaction = null;
		/// <summary>
		/// 获取分布式事务实例
		/// </summary>
		public CommittableTransaction CommitTransaction { get { return commitTransaction; } }

		private DbConnection _dbConnection;
		/// <summary>// 数据库连接</summary>
		public DbConnection DbConnection { get { return _dbConnection; } }
		#endregion

		#region 构造函数
		/// <summary>创建数据处理类实例(默认关闭事务)</summary>
		protected AbstractDbAccess() : this(null, false, null, null) { }

		/// <summary>创建数据处理类实例</summary>
		/// <param name="startTransaction">是否启用事务</param>
		protected AbstractDbAccess(bool startTransaction) : this(null, startTransaction, null, null) { }

		/// <summary>使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。</summary>
		/// <param name="connection">基础框架配置的数据库连接名称</param>
		protected AbstractDbAccess(string connection) : this(new UserContext(connection), false, null, null) { }

		/// <summary>当前AbstractAccess子类是否需要控制事务</summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)。</param>
		protected AbstractDbAccess(IUserContext user) : this(user, false, null, null) { _User = user; }

		/// <summary>使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。</summary>
		/// <param name="connection">基础框架配置的数据库连接名称</param>
		/// <param name="startTransaction">是否启用事务</param>
		protected AbstractDbAccess(string connection, bool startTransaction) : this(new UserContext(connection), startTransaction, null, null) { }

		/// <summary>当前AbstractAccess子类是否需要控制事务</summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)。</param>
		/// <param name="startTransaction">是否启用事务</param>
		protected AbstractDbAccess(IUserContext user, bool startTransaction) : this(user, startTransaction, null, null) { _User = user; }

		/// <summary>使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。</summary>
		/// <param name="connection">数据库连接字符串</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractDbAccess(string connection, TimeSpan timeout) : this(new UserContext(connection), true, null, new CommittableTransaction(timeout)) { }

		/// <summary>当前AbstractAccess子类是否需要控制事务</summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)。</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractDbAccess(IUserContext user, TimeSpan timeout) : this(user, true, null, new CommittableTransaction(timeout)) { _User = user; }

		/// <summary>使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。</summary>
		/// <param name="connection">数据库连接字符串</param>
		/// <param name="isolationLevel">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractDbAccess(string connection, ST.IsolationLevel isolationLevel, TimeSpan timeout)
			: this(new UserContext(connection), true, null, new CommittableTransaction(new TransactionOptions() { IsolationLevel = isolationLevel, Timeout = timeout })) { }

		/// <summary>当前AbstractAccess子类是否需要控制事务</summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)。</param>
		/// <param name="isolationLevel">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractDbAccess(IUserContext user, ST.IsolationLevel isolationLevel, TimeSpan timeout)
			: this(user, true, null, new CommittableTransaction(new TransactionOptions() { IsolationLevel = isolationLevel, Timeout = timeout })) { _User = user; }

		/// <summary>使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。</summary>
		/// <param name="connection">数据库连接字符串</param>
		/// <param name="transaction">对用于登记的现有 Transaction 的引用。</param>
		protected AbstractDbAccess(string connection, CommittableTransaction transaction)
			: this(new UserContext(connection), true, null, transaction) { }

		/// <summary>使用数据库连接和连接类型，初始化 AbstractDbAccess 类实例。/// </summary>
		/// <param name="user">当前用户信息(包含但不限于数据库连接名称、区域、Session等)。</param>
		/// <param name="transaction">对用于登记的现有 Transaction 的引用。</param>
		protected AbstractDbAccess(IUserContext user, CommittableTransaction transaction)
			: this(user, true, null, transaction) { _User = user; }

		/// <summary>创建数据处理类实例</summary>
		/// <param name="access">数据库操作类。</param>
		protected AbstractDbAccess(AbstractDbAccess access) : this(null, false, access, null) { _User = access._User; }

		/// <summary>创建数据处理类实例</summary>
		/// <param name="transaction">对用于登记的现有 Transaction 的引用。</param>
		protected AbstractDbAccess(CommittableTransaction transaction) : this(null, true, null, transaction) { }

		/// <summary>使用指定的事务选项初始化 AbstractDbAccess 类的新实例，并启用事务。</summary>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractDbAccess(TimeSpan timeout)
			: this(null, true, null, new CommittableTransaction(timeout)) { }

		/// <summary>使用指定的事务选项初始化 AbstractDbAccess 类的新实例，并启用事务。</summary>
		/// <param name="isolationLevel">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		protected AbstractDbAccess(ST.IsolationLevel isolationLevel, TimeSpan timeout)
			: this(null, true, null, new CommittableTransaction(new TransactionOptions() { IsolationLevel = isolationLevel, Timeout = timeout })) { }

		/// <summary>当前AbstractAccess子类是否需要控制事务</summary>
		/// <param name="options">一个 System.Transactions.TransactionOptions 结构，描述用于新事务的事务选项。</param>
		protected AbstractDbAccess(TransactionOptions options) : this(null, true, null, new CommittableTransaction(options)) { }

		/// <summary>当前AbstractAccess子类是否需要控制事务</summary>
		/// <param name="user">用户的上下文信息(包含数据库连接和用户区域)。</param>
		/// <param name="startTransaction">是否启用事务</param>
		/// <param name="access">数据库操作类</param>
		/// <param name="transaction">对用于登记的现有 Transaction 的引用。</param>
		private AbstractDbAccess(IUserContext user, bool startTransaction, AbstractDbAccess access, CommittableTransaction transaction)
		{
			_User = user ?? new UserContext(ConnectionContext.DefaultName);
			_Result = Result.Empty; Type accessType = GetType();
			ConfigurationAttribute ca = Attribute.GetCustomAttribute(accessType, typeof(ConfigurationAttribute)) as ConfigurationAttribute;
			//类{0}不存在自定义属性{1}，无法获取配置文件信息。
			if (ca == null) { throw new ConfigurationException("Access_ConfigurationAttribute", accessType, typeof(ConfigurationAttribute)); }

			_ConfigInfo = ca.CreateInfo(accessType, access != null ? access.ConnectionName : _User.Connection);
			_ConnectionFactory = ConnectionFactoryBuilder.CreateConnectionFactory(_ConfigInfo, accessType);
			if (DataCommandCache.GetValue(_ConfigInfo.Key, out _TableInfo) == false)
				_TableInfo = DataCommandCache.SetValue(_ConfigInfo.Key, new TableConfiguration(_ConnectionFactory));
			CommitTranState = TransactionState.NotExistTransaction;
			if (access != null)
			{
				if (_ConfigInfo.CompareTo(access._ConfigInfo) != 0)
				{
					CreatePersistent(_ConfigInfo);
					if (access.CommitTransaction != null)
						_dbConnection.EnlistTransaction(access.CommitTransaction);
				}
				else
				{
					_dbConnection = access.DbConnection;
				}
			}
			else
			{
				CreatePersistent(_ConfigInfo);
				if (startTransaction) { BeginTransaction(transaction); }
			}
		}

		///// <summary>当前AbstractAccess子类是否需要控制事务</summary>
		///// <param name="connectionName">数据库连接名称，此连接配置与应用程序config文件中。</param>
		///// <param name="startTransaction">是否启用事务</param>
		///// <param name="access">数据库操作类</param>
		///// <param name="transaction">对用于登记的现有 Transaction 的引用。</param>
		//private AbstractDbAccess(string connectionName, bool startTransaction, AbstractDbAccess access, CommittableTransaction transaction)
		//{
		//	_Result = Result.Empty; Type accessType = GetType();
		//	ConfigurationAttribute ca = Attribute.GetCustomAttribute(accessType, typeof(ConfigurationAttribute)) as ConfigurationAttribute;
		//	//类{0}不存在自定义属性{1}，无法获取配置文件信息。
		//	if (ca == null) { throw new ConfigurationException("Access_ConfigurationAttribute", accessType, typeof(ConfigurationAttribute)); }

		//	_ConfigInfo = ca.CreateInfo(accessType, access != null ? access._ConfigInfo.ConnectionName : connectionName);
		//	_ConnectionFactory = ConnectionFactoryBuilder.CreateConnectionFactory(_ConfigInfo, accessType);
		//	if (DataCommandCache.GetValue(_ConfigInfo.TableName, out _TableInfo) == false)
		//		_TableInfo = DataCommandCache.SetValue(_ConfigInfo.TableName, new TableConfiguration(_ConnectionFactory));
		//	CommitTranState = TransactionState.NotExistTransaction;
		//	if (access != null)
		//	{
		//		if (_ConfigInfo.CompareTo(access._ConfigInfo) != 0)
		//		{
		//			CreatePersistent(_ConfigInfo);
		//			if (access.CommitTransaction != null)
		//				_dbConnection.EnlistTransaction(access.CommitTransaction);
		//		}
		//		else
		//		{
		//			_dbConnection = access.DbConnection;
		//		}
		//	}
		//	else
		//	{
		//		CreatePersistent(_ConfigInfo);
		//		if (startTransaction) { BeginTransaction(transaction); }
		//	}
		//}

		/// <summary>
		/// 启动可提交的事务
		/// </summary>
		/// <param name="transaction">可提交事务，默认值为 null。</param>
		internal void BeginTransaction(CommittableTransaction transaction)
		{
			if (transaction != null) { commitTransaction = transaction; }
			else { commitTransaction = new CommittableTransaction(); }
			_dbConnection.EnlistTransaction(commitTransaction);
			CommitTranState = TransactionState.ExistTransaction;
		}

		/// <summary>
		/// 使用指定的事务选项初始化 System.Transactions.CommittableTransaction 类的新实例，并启用事务。
		/// </summary>
		/// <param name="isolationLevel">一个 System.Transactions.IsolationLevel 枚举类型的值，该值表示事务 CommittableTransaction 的隔离级别。</param>
		/// <param name="timeout">一个TimeSpan 类型的值，该值表示事务 CommittableTransaction 的超时时间限制。</param>
		internal void BeginTransaction(ST.IsolationLevel isolationLevel, TimeSpan timeout)
		{
			TransactionOptions options = new TransactionOptions() { IsolationLevel = isolationLevel, Timeout = timeout };
			commitTransaction = new CommittableTransaction(options);
			_dbConnection.EnlistTransaction(commitTransaction);
			CommitTranState = TransactionState.ExistTransaction;
		}

		/// <summary>
		/// 使用指定的事务选项初始化 System.Transactions.CommittableTransaction 类的新实例，并启用事务。
		/// </summary>
		/// <param name="options">一个 System.Transactions.TransactionOptions 结构，描述用于新事务的事务选项。</param>
		internal void BeginTransaction(TransactionOptions options)
		{
			commitTransaction = new CommittableTransaction(options);
			_dbConnection.EnlistTransaction(commitTransaction);
			CommitTranState = TransactionState.ExistTransaction;
		}

		/// <summary>
		/// 使用指定的事务选项初始化 System.Transactions.CommittableTransaction 类的新实例，并启用事务。
		/// </summary>
		/// <param name="commandName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		protected void BeginTransaction(string commandName)
		{
			DataCommand command = this.CreateDataCommand(commandName);
			if (command != null)
			{
				TimeSpan timespan = TimeSpan.FromSeconds(command.CommandTimeout);
				commitTransaction = new CommittableTransaction(timespan);
				_dbConnection.EnlistTransaction(commitTransaction);
				CommitTranState = TransactionState.ExistTransaction;
			}
		}

		private void CreatePersistent(ConfigurationInfo ci)
		{
			if (ci.ConnectionName != null && ci.ConnectionName != string.Empty)
			{
				//类{0}中配置信息错误,名称为{1}的连接配置信息不存在，请检查应用程序配置文件(*.config)。
				if (ConnectionContext.Contains(ci.ConnectionName) == false)
				{
					throw new ConfigurationException("Access_NotExistsConnection", GetType(), ci.ConnectionName);
				}
				ConnectionInfo config = ConnectionContext.GetConnection(ci.ConnectionName);
				_dbConnection = _ConnectionFactory.CreateConnection(config);
			}
			else
			{
				_dbConnection = _ConnectionFactory.CreateConnection(ConnectionContext.DefaultConnection);
			}
			_dbConnection.Open();
		}
		#endregion

		#region 登记数据库连接
		private DynamicCommand BeginDynamicExecute(DynamicCommand dataCommand)
		{
			BeginExecute(dataCommand);
			return dataCommand;
		}

		private StaticCommand BeginStaticExecute(StaticCommand dataCommand)
		{
			BeginExecute(dataCommand);
			return dataCommand;
		}

		internal virtual DataCommand BeginExecute(DataCommand dataCommand)
		{
			_Result.Clear();
			if (DbConnection != null && DbConnection.State != ConnectionState.Open)
				DbConnection.Open();
			dataCommand.ResetConnection(DbConnection);
			return dataCommand;
		}
		#endregion

		#region 获取区域字符串资源
		/// <summary>使用 Lambda 表达式获取布尔类型属性值。</summary>
		/// <param name="expression">要获取的资源名。</param>
		/// <param name="item">判断参数</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		protected string GetString<TM>(Expression<Func<TM, bool>> expression, TM item)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			bool value = expression.Compile().Invoke(item);
			EntityPropertyProvidor.TryGetProperty<TM>(name, out EntityPropertyMeta propertyInfo);

			if (propertyInfo != null && propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converter = wda.ConverterName;
				string valueText = string.Concat(wda.DisplayName, "_", value ? "True" : "False", "Text");

				if (_User.Culture != null) { return MessageContext.GetString(converter, valueText, _User.Culture); }
				return MessageContext.GetString(converter, valueText);
			}

			if (_User.Culture != null) { return MessageContext.GetString(name, _User.Culture); }
			return MessageContext.GetString(name);
		}

		/// <summary>使用 Lambda 表达式获取布尔类型属性值。</summary>
		/// <param name="expression">要获取的资源名。</param>
		/// <param name="item">判断参数</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		protected string GetString<TM>(Expression<Func<TM, Enum>> expression, TM item)
		{
			Enum value = expression.Compile().Invoke(item); Type enumType = value.GetType();
			string converter = null;
			if (Attribute.IsDefined(enumType, typeof(WebDisplayConverterAttribute)))
			{
				WebDisplayConverterAttribute wdca = (WebDisplayConverterAttribute)Attribute.GetCustomAttribute(enumType, typeof(WebDisplayConverterAttribute));
				converter = wdca.ConverterName;
			}
			string enumName = enumType.Name;
			string name = Enum.GetName(enumType, value);
			string itemName = string.Concat(enumName, "_", name);

			if (_User.Culture != null) { return MessageContext.GetString(converter, itemName, _User.Culture); }
			return MessageContext.GetString(converter, itemName);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		protected string GetString(string name)
		{
			if (_User.Culture != null) { return MessageContext.GetString(name, _User.Culture); }
			return MessageContext.GetString(name);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="name">资源名称</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>为指定区域性本地化的资源的值。如果不可能有最佳匹配，则返回 null。</returns>
		protected string GetString(string name, params object[] args)
		{
			if (_User.Culture != null) { return MessageContext.GetString(name, _User.Culture, args); }
			return MessageContext.GetString(name, args);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">资源名称</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		protected string GetString(string converterName, string name)
		{
			if (_User.Culture != null) { return MessageContext.GetString(converterName, name, _User.Culture); }
			return MessageContext.GetString(converterName, name);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
		/// </summary>
		/// <param name="converterName">需要指定的转换器名称。</param>
		/// <param name="name">资源名称</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		protected string GetString(string converterName, string name, params object[] args)
		{
			if (_User.Culture != null) { return MessageContext.GetString(converterName, name, _User.Culture, args); }
			return MessageContext.GetString(converterName, name, args);
		}
		#endregion

		#region 执行数据库方法(ExecuteNonQueryAsync)
		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		protected System.Threading.Tasks.Task<Result> ExecuteNonQueryAsync(string cmdName)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				return this.ExecuteNonQueryAsync(dataCommand);
			}
		}

		/// <summary>
		/// 采用匿名类为参数，执行Transact-SQL命令，此命令不执行NewValues节内的所有新值命令。
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="anonObject">键/值对数组，包含了需要执行参数的值。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		protected Task<Result> ExecuteNonQueryAsync(string cmdName, object anonObject)
		{
			TaskCompletionSource<Result> source = new TaskCompletionSource<Result>();
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.CheckData(_Result, anonObject);
				if (_Result.Failure) { source.SetResult(_Result); return source.Task; }

				dataCommand.ResetParameters(anonObject);
				return this.ExecuteNonQueryAsync(dataCommand);
			}
		}

		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="entity">实体类数组，包含了需要执行参数的值。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		protected System.Threading.Tasks.Task<Result> ExecuteNonQueryAsync(string cmdName, AbstractEntity entity)
		{
			TaskCompletionSource<Result> source = new TaskCompletionSource<Result>();
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.CheckData(_Result, entity);
				if (_Result.Failure) { source.SetResult(_Result); return source.Task; }

				dataCommand.ResetParameters(entity);
				return this.ExecuteNonQueryAsync(dataCommand);
			}
		}

		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="entities">实体类数组，包含了需要执行参数的值。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		protected System.Threading.Tasks.Task<Result> ExecuteNonQueryAsync(string cmdName, AbstractEntity[] entities)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return this.ExecuteNonQueryAsync(dataCommand, entities);
		}

		/// <summary>
		/// 采用匿名类为参数，执行Transact-SQL命令，此命令不执行NewValues节内的所有新值命令。
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="anonObject">包含可执行参数的匿名类。</param>
		/// <exception cref="System.ArgumentNullException">参数 entities 为null或数组长度为0。</exception>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		internal protected System.Threading.Tasks.Task<Result> ExecuteNonQueryAsync(StaticCommand dataCommand, object anonObject)
		{
			TaskCompletionSource<Result> source = new TaskCompletionSource<Result>();
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.CheckData(_Result, anonObject);
				if (_Result.Failure) { source.SetResult(_Result); return source.Task; }

				dataCommand.ResetParameters(anonObject);
				return this.ExecuteNonQueryAsync(dataCommand);
			}
		}

		/// <summary>
		/// 采用匿名类为参数，执行Transact-SQL命令，此命令不执行NewValues节内的所有新值命令。
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="entity">包含可执行参数的类。</param>
		/// <exception cref="System.ArgumentNullException">参数 entities 为null或数组长度为0。</exception>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		internal protected async Task<Result> ExecuteNonQueryAsync(StaticCommand dataCommand, AbstractEntity entity)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				await dataCommand.CheckDataAsync(_Result, entity);
				if (_Result.Failure) { return _Result; }

				await dataCommand.CreateNewValueAsync(_Result, entity);
				if (_Result.Failure) { return _Result; }

				dataCommand.ResetParameters(entity);
				return await this.ExecuteNonQueryAsync(dataCommand);
			}
		}

		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="entities">实体类数组，包含了需要执行参数的值。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		internal protected async Task<Result> ExecuteNonQueryAsync(StaticCommand dataCommand, AbstractEntity[] entities)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				await dataCommand.CheckDataAsync(_Result, entities);
				if (_Result.Failure) { return _Result; }

				dataCommand.CreateNewValue(_Result, entities);
				if (!_Result.Successful) { return _Result; }

				if (dataCommand.ResetParameters(entities))
				{
					return await this.ExecuteNonQueryAsync(dataCommand);
				}
				foreach (AbstractEntity entity in entities)
				{
					dataCommand.ResetParameters(entity);
					_Result.AffectedRows += await dataCommand.ExecuteNonQueryAsync();
				}
			}
			return _Result;
		}

		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程命令结构。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		internal protected System.Threading.Tasks.Task<Result> ExecuteNonQueryAsync(StaticCommand dataCommand)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				return dataCommand.ExecuteNonQueryAsync().ContinueWith(tt =>
				{
					if (tt.IsFaulted) { _Result.AddError(tt.Exception.Message); }
					else { _Result.AffectedRows = tt.Result; }
					return _Result;
				});
			}
		}
		#endregion

		#region 执行数据库方法(ExecuteNonQuery)
		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		protected Result ExecuteNonQuery(string cmdName)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteNonQuery(dataCommand);
		}

		/// <summary>
		/// 采用匿名类为参数，执行Transact-SQL命令，此命令不执行NewValues节内的所有新值命令。
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="anonObject">键/值对数组，包含了需要执行参数的值。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		protected Result ExecuteNonQuery(string cmdName, object anonObject)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteNonQuery(dataCommand, anonObject);
		}

		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="entity">实体类数组，包含了需要执行参数的值。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		protected Result ExecuteNonQuery(string cmdName, AbstractEntity entity)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteNonQuery(dataCommand, entity);
		}

		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="entities">实体类数组，包含了需要执行参数的值。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		protected Result ExecuteNonQuery(string cmdName, AbstractEntity[] entities)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteNonQuery(dataCommand, entities);
		}

		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		internal protected Result ExecuteNonQuery(StaticCommand dataCommand)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				_Result.AffectedRows = dataCommand.ExecuteNonQuery();
				return _Result;
			}
		}

		/// <summary>
		/// 采用匿名类为参数，执行Transact-SQL命令，此命令不执行NewValues节内的所有新值命令。
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="anonObject">包含可执行参数的匿名类。</param>
		/// <exception cref="System.ArgumentNullException">参数 entities 为null或数组长度为0。</exception>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		internal protected Result ExecuteNonQuery(StaticCommand dataCommand, object anonObject)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.CheckData(_Result, anonObject);
				if (!_Result.Successful) { return _Result; }

				dataCommand.ResetParameters(anonObject);
				_Result.AffectedRows = dataCommand.ExecuteNonQuery();
				return _Result;
			}
		}

		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="entities">实体类，包含了需要执行参数的值。</param>
		/// <exception cref="System.ArgumentNullException">参数 entities 为null或数组长度为0。</exception>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		internal protected Result ExecuteNonQuery(StaticCommand dataCommand, AbstractEntity[] entities)
		{
			if (entities == null || entities.Length == 0)
				throw new ArgumentNullException("entities");
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.CheckData(_Result, entities);
				if (!_Result.Successful) { return _Result; }

				dataCommand.CreateNewValue(_Result, entities);
				if (!_Result.Successful) { return _Result; }

				if (dataCommand.ResetParameters(entities))
				{
					_Result.AffectedRows = dataCommand.ExecuteNonQuery();
					return _Result;
				}
				foreach (AbstractEntity entity in entities)
				{
					dataCommand.ResetParameters(entity);
					_Result.AffectedRows += dataCommand.ExecuteNonQuery();
				}
				entities.ClearError();
				return _Result;
			}
		}

		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="entity">实体类，包含了需要执行参数的值。</param>
		/// <exception cref="System.ArgumentNullException">参数 entities 为null或数组长度为0。</exception>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		internal protected Result ExecuteNonQuery(StaticCommand dataCommand, AbstractEntity entity)
		{
			if (entity == null) { throw new ArgumentNullException("entity"); }
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.CheckData(_Result, entity);
				if (!_Result.Successful) { return _Result; }

				dataCommand.CreateNewValue(_Result, entity);
				if (!_Result.Successful) { return _Result; }

				dataCommand.ResetParameters(entity);
				_Result.AffectedRows = dataCommand.ExecuteNonQuery();
				entity.ClearError();
				return _Result;
			}
		}
		#endregion

		#region 异步执行数据库方法(BatchExecuteAsync)
		/// <summary>
		/// 使用 XXXBulkCopy 类执行数据插入命令
		/// </summary>
		/// <param name="batchCommand">批处理命令</param>
		/// <param name="table">类型 BaseTableType&lt;BaseTableRowType&gt; 子类类实例，包含了需要执行参数的值。</param>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <returns>执行Transact-SQL语句或存储过程后的返回结果。</returns>
		protected Task<Result> BatchExecuteAsync<TR>(BatchCommand batchCommand, BaseTableType<TR> table, int timeout) where TR : BaseTableRowType
		{
			using (batchCommand)
			{
				return batchCommand.BatchExecuteAsync<TR>(table, timeout).ContinueWith(tt =>
				{
					if (tt.IsCompleted)
					{
						if (tt.Exception != null) { _Result.AddError(tt.Exception.Message); }
						else { _Result.AffectedRows = table.Count; }
					}
					return _Result;
				});
			}
		}

		/// <summary>
		/// 使用 XXXBulkCopy 类执行数据插入命令
		/// </summary>
		/// <param name="table">类型 BaseTableType&lt;BaseTableRowType&gt; 子类类实例，包含了需要执行参数的值。</param>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <returns>执行Transact-SQL语句或存储过程后的返回结果。</returns>
		protected Task<Result> BatchExecuteAsync<TR>(BaseTableType<TR> table, int timeout) where TR : BaseTableRowType
		{
			BatchCommand batchCommand = CreateBatchCommand();
			return BatchExecuteAsync(batchCommand, table, timeout);
		}

		/// <summary>
		/// 使用 XXXBulkCopy 类执行数据插入命令
		/// </summary>
		/// <param name="table">类型 BaseTableType&lt;BaseTableRowType&gt; 子类类实例，包含了需要执行参数的值。</param>
		/// <returns>执行Transact-SQL语句或存储过程后的返回结果。</returns>
		protected Task<Result> BatchExecuteAsync<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			BatchCommand batchCommand = CreateBatchCommand();
			return BatchExecuteAsync(batchCommand, table, 30);
		}
		#endregion

		#region 执行数据库方法(BatchExecute)
		/// <summary>
		/// 使用 XXXBulkCopy 类执行数据插入命令
		/// </summary>
		/// <param name="batchCommand">批处理命令</param>
		/// <param name="table">类型 BaseTableType&lt;BaseTableRowType&gt; 子类类实例，包含了需要执行参数的值。</param>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <returns>执行Transact-SQL语句或存储过程后的返回结果。</returns>
		protected Result BatchExecute<TR>(BatchCommand batchCommand, BaseTableType<TR> table, int timeout) where TR : BaseTableRowType
		{
			using (batchCommand)
			{
				batchCommand.BatchExecute<TR>(table, timeout);
				return _Result;
			}
		}


		/// <summary>
		/// 使用 XXXBulkCopy 类执行数据插入命令
		/// </summary>
		/// <param name="table">类型 BaseTableType&lt;BaseTableRowType&gt; 子类类实例，包含了需要执行参数的值。</param>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <returns>执行Transact-SQL语句或存储过程后的返回结果。</returns>
		protected Result BatchExecute<TR>(BaseTableType<TR> table, int timeout) where TR : BaseTableRowType
		{
			BatchCommand batchCommand = CreateBatchCommand();
			return BatchExecute(batchCommand, table, timeout);
		}

		/// <summary>
		/// 使用 XXXBulkCopy 类执行数据插入命令
		/// </summary>
		/// <param name="table">类型 BaseTableType&lt;BaseTableRowType&gt; 子类类实例，包含了需要执行参数的值。</param>
		/// <returns>执行Transact-SQL语句或存储过程后的返回结果。</returns>
		protected Result BatchExecute<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			BatchCommand batchCommand = CreateBatchCommand();
			return BatchExecute(batchCommand, table, 30);
		}
		#endregion

		#region 执行数据库方法(ExecuteCore)
		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程命令结构。</param>
		/// <param name="table">类型 BaseTableType&lt;BaseTableRowType&gt; 子类类实例，包含了需要执行参数的值。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		protected Task<Result> ExecuteCoreAsync<TR>(StaticCommand dataCommand, BaseTableType<TR> table) where TR : BaseTableRowType
		{
			TaskCompletionSource<Result> tsc = new TaskCompletionSource<Result>();
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.CheckData<TR>(_Result, table);
				if (_Result.Failure) { tsc.SetResult(_Result); return tsc.Task; }

				dataCommand.CreateNewValue(_Result, table);
				if (_Result.Failure) { tsc.SetResult(_Result); return tsc.Task; }

				foreach (BaseTableRowType row in table.Rows)
				{
					dataCommand.ResetParameters(row);
					_Result.AffectedRows += dataCommand.ExecuteNonQuery();
				}
				tsc.SetResult(_Result);
			}
			return tsc.Task;
		}

		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程命令结构。</param>
		/// <param name="row"><![CDATA[类型 BaseTableType&lt;BaseTableRowType&gt; 子类类实例，包含了需要执行参数的值。]]></param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		protected Task<Result> ExecuteCoreAsync(StaticCommand dataCommand, BaseTableRowType row)
		{
			TaskCompletionSource<Result> source = new TaskCompletionSource<Result>();
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.CheckData(_Result, row);
				if (_Result.Failure) { source.SetResult(_Result); return source.Task; }

				dataCommand.CreateNewValue(_Result, row);
				if (_Result.Failure) { source.SetResult(_Result); return source.Task; }

				dataCommand.ResetParameters(row);

				System.Threading.Tasks.Task<int> task = dataCommand.ExecuteNonQueryAsync();
				return task.ContinueWith<Result>(delegate (Task<int> tt)
				{
					_Result.AffectedRows = tt.Result;
					return _Result;
				});
			}
		}
		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="table">类型 BaseTableType&lt;BaseTableRowType&gt; 子类类实例，包含了需要执行参数的值。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		protected Result ExecuteCore<TR>(string cmdName, BaseTableType<TR> table) where TR : BaseTableRowType
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteCore<TR>(dataCommand, table);
		}

		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="table">实体类，包含了需要执行参数的值。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		internal protected Result ExecuteCore<TR>(StaticCommand dataCommand, BaseTableType<TR> table) where TR : BaseTableRowType
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.CheckData<TR>(_Result, table);
				if (_Result.Failure) { return _Result; }

				dataCommand.CreateNewValue(_Result, table);
				if (_Result.Failure) { return _Result; }
				foreach (BaseTableRowType row in table.Rows)
				{
					dataCommand.ResetParameters(row);
					_Result.AffectedRows += dataCommand.ExecuteNonQuery();
					row.ClearErrors();
				}
				return _Result;
			}
		}

		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="row">键/值对数组，包含了需要执行参数的值。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		protected Result ExecuteCore<TR>(string cmdName, TR row) where TR : BaseTableRowType
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteCore<TR>(dataCommand, row);
		}

		/// <summary>
		/// 执行Transact-SQL命令
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="row">键/值对数组，包含了需要执行参数的值。</param>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		internal protected Result ExecuteCore<TR>(StaticCommand dataCommand, TR row) where TR : BaseTableRowType
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.CheckData(_Result, row);
				if (!_Result.Successful) { return _Result; }

				dataCommand.CreateNewValue(_Result, row);
				if (!_Result.Successful) { return _Result; }

				dataCommand.ResetParameters(row);
				_Result.AffectedRows = dataCommand.ExecuteNonQuery();
				row.ClearErrors();
				return _Result;
			}
		}
		#endregion

		#region 执行数据库方法(ExecuteScalar)
		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行.
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程名称。</param>
		/// <returns>返回查询结果，如果为空则返回空引用</returns>
		protected object ExecuteScalar(string cmdName)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteScalar(dataCommand);
		}

		/// <summary>
		/// 采用匿名类为参数，执行Transact-SQL命令。
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="anonObject">包含可执行参数的匿名类。</param>
		/// <exception cref="System.ArgumentNullException">参数 anonObject 为null。</exception>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		protected object ExecuteScalar(string cmdName, object anonObject)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteScalar(dataCommand, anonObject);
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行.
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程名称。</param>
		/// <param name="objArray">包含执行命令的参数信息。</param>
		/// <returns>返回查询结果，如果为空则返回空引用</returns>
		protected object ExecuteScalar(string cmdName, BaseTableRowType objArray)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteScalar(dataCommand, objArray);
		}

		/// <summary>
		/// 获取数据库的只读流
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程名称。</param>
		/// <param name="entity">实体类，包含了需要执行参数的值。</param>
		/// <returns>返回查询结果，如果为空则返回空引用</returns>
		protected object ExecuteScalar(string cmdName, AbstractEntity entity)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteScalar(dataCommand, entity);
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行.
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <returns>返回查询结果，如果为空则返回空引用</returns>
		internal protected object ExecuteScalar(StaticCommand dataCommand)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				return dataCommand.ExecuteScalar();
			}
		}

		/// <summary>
		/// 采用匿名类为参数，执行Transact-SQL命令。
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="anonObject">包含可执行参数的匿名类。</param>
		/// <exception cref="System.ArgumentNullException">参数 entities 为null或数组长度为0。</exception>
		/// <returns>执行ransact-SQL语句的返回值</returns>
		internal protected object ExecuteScalar(StaticCommand dataCommand, object anonObject)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.ResetParameters(anonObject);
				return dataCommand.ExecuteScalar();
			}
		}


		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行.
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="entity">实体类，包含了需要执行参数的值。</param>
		/// <returns>返回查询结果，如果为空则返回空引用</returns>
		protected internal object ExecuteScalar(StaticCommand dataCommand, AbstractEntity entity)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.ResetParameters(entity);
				return dataCommand.ExecuteScalar();
			}
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行.
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="objArray">键/值对数组，包含了需要执行参数的值。</param>
		/// <returns>返回查询结果，如果为空则返回空引用</returns>
		internal protected object ExecuteScalar(StaticCommand dataCommand, BaseTableRowType objArray)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.ResetParameters(objArray);
				return dataCommand.ExecuteScalar();
			}
		}

		#endregion

		#region 执行数据库方法(ExecuteReader)

		/// <summary>
		/// 获取数据库的只读流
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <returns>返回 DbDataReader 对应数据库的实例。</returns>
		protected DbDataReader ExecuteReader(string cmdName)
		{
			DataCommand dataCommand = CreateDataCommand(cmdName);
			return ExecuteReader(dataCommand);
		}

		/// <summary>
		/// 采用匿名类为参数，执行Transact-SQL命令。
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="anonObject">包含可执行参数的匿名类。</param>
		/// <exception cref="System.ArgumentNullException">参数 anonObject 为null。</exception>
		/// <returns>返回 DbDataReader 对应数据库的实例。</returns>
		protected DbDataReader ExecuteReader(string cmdName, object anonObject)
		{
			DataCommand dataCommand = CreateDataCommand(cmdName);
			return ExecuteReader(dataCommand, anonObject);
		}

		/// <summary>
		/// 获取数据库的只读流
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="objArray">键/值对数组，包含了需要执行参数的值。</param>
		/// <returns>返回 DbDataReader 对应数据库的实例。</returns>
		protected DbDataReader ExecuteReader(string cmdName, BaseTableRowType objArray)
		{
			DataCommand dataCommand = CreateDataCommand(cmdName);
			return ExecuteReader(dataCommand, objArray);
		}

		/// <summary>
		/// 获取数据库的只读流
		/// </summary>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="entity">实体类，包含了需要执行参数的值。</param>
		/// <returns>返回 DbDataReader 对应数据库的实例。</returns>
		protected DbDataReader ExecuteReader(string cmdName, AbstractEntity entity)
		{
			DataCommand dataCommand = CreateDataCommand(cmdName);
			return ExecuteReader(dataCommand, entity);
		}

		/// <summary>
		/// 获取数据库的只读流
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <returns>返回 DbDataReader 对应数据库的实例。</returns>
		internal protected DbDataReader ExecuteReader(DataCommand dataCommand)
		{
			using (dataCommand = BeginExecute(dataCommand))
			{
				return dataCommand.ExecuteReader();
			}
		}

		/// <summary>
		/// 采用匿名类为参数，执行Transact-SQL命令。
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="dynamicObject">包含可执行参数的匿名类。</param>
		/// <exception cref="System.ArgumentNullException">参数 entities 为null或数组长度为0。</exception>
		/// <returns>返回 DbDataReader 对应数据库的实例。</returns>
		internal protected DbDataReader ExecuteReader(DataCommand dataCommand, object dynamicObject)
		{
			using (dataCommand = BeginExecute(dataCommand))
			{
				dataCommand.ResetParameters(dynamicObject);
				return dataCommand.ExecuteReader();
			}
		}

		/// <summary>
		/// 获取数据库的只读流
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="entity">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回 DbDataReader 对应数据库的实例。</returns>
		internal protected DbDataReader ExecuteReader(DataCommand dataCommand, AbstractEntity entity)
		{
			using (dataCommand = BeginExecute(dataCommand))
			{
				dataCommand.ResetParameters(entity);
				return dataCommand.ExecuteReader();
			}
		}

		/// <summary>
		/// 获取数据库的只读流
		/// </summary>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程。</param>
		/// <param name="objArray">键/值对数组，包含了需要执行参数的值。</param>
		/// <returns>返回 DbDataReader 对应数据库的实例。</returns>
		internal protected DbDataReader ExecuteReader(DataCommand dataCommand, BaseTableRowType objArray)
		{
			using (dataCommand = BeginExecute(dataCommand))
			{
				dataCommand.ResetParameters(objArray);
				return dataCommand.ExecuteReader();
			}
		}
		#endregion

		#region 执行数据库方法(SearchEntity-Basic.Entity.AbstractEntity)
		/// <summary>
		/// 按关键字查询记录
		/// </summary>
		/// <param name="entity">需要填充的Basic.Entity.AbstractEntity类实例</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <returns>如果查询结果存在则返回true，否则返回false。</returns>
		internal bool SearchEntity(DataCommand dataCommand, AbstractEntity entity)
		{
			using (DbDataReader reader = ExecuteReader(dataCommand, entity))
			{
				if (!reader.IsClosed && reader.HasRows)
				{
					int fieldCount = reader.FieldCount - 1;
					if (reader.Read())
					{
						EntityPropertyMeta propertyInfo = null;
						for (int index = 0; index <= fieldCount; index++)
						{
							string name = reader.GetName(index);
							if (entity.TryGetDbProperty(name, out propertyInfo))
							{
								object propertyValue = reader.GetValue(index);
								propertyInfo.SetValue(entity, propertyValue);
							}
						}
					}
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 按关键字查询记录
		/// </summary>
		/// <param name="entity">需要填充的Basic.Entity.AbstractEntity类实例</param>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <returns>如果查询结果存在则返回true，否则返回false。</returns>
		protected bool SearchEntity(string cmdName, AbstractEntity entity)
		{
			using (DataCommand dataCommand = CreateDataCommand(cmdName))
			{
				return this.SearchEntity(dataCommand, entity);
			}
		}
		#endregion

		#region 执行数据库方法(Fill-DataTable)
		///// <summary>
		///// 填充数据集
		///// </summary>
		///// <param name="table">待填充的实体类实例</param>
		///// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		///// <param name="anonObject">需要查询的当前页大小</param>
		///// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		//internal protected int Fill(DataTable table, DataCommand dataCommand, object anonObject)
		//{
		//	using (dataCommand = BeginExecute(dataCommand))
		//	{
		//		dataCommand.ResetParameters(anonObject);
		//		return dataCommand.Fill(table);
		//	}
		//}

		///// <summary>
		///// 填充数据集
		///// </summary>
		///// <param name="table">待填充的实体类实例</param>
		///// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		///// <param name="pageSize">需要查询的当前页大小</param>
		///// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		///// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		//internal protected int Fill(DataTable table, DataCommand dataCommand, int pageSize, int pageIndex)
		//{
		//	using (dataCommand = BeginExecute(dataCommand))
		//	{
		//		dataCommand.ResetPaginationParameter(pageSize, pageIndex);
		//		return dataCommand.Fill(table);
		//	}
		//}

		///// <summary>
		///// 填充数据集
		///// </summary>
		///// <param name="table">待填充的实体类实例</param>
		///// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		///// <param name="entity">数据实体类，包含了需要执行参数的值。</param>
		///// <param name="pageSize">需要查询的当前页大小</param>
		///// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		///// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		//internal protected int Fill(DataTable table, DataCommand dataCommand, AbstractEntity entity, int pageSize, int pageIndex)
		//{
		//	using (dataCommand = BeginExecute(dataCommand))
		//	{
		//		dataCommand.ResetPaginationParameter(pageSize, pageIndex);
		//		dataCommand.ResetParameters(entity);
		//		return dataCommand.Fill(table);
		//	}
		//}

		///// <summary>
		///// 填充数据集
		///// </summary>
		///// <param name="table">待填充的实体类实例</param>
		///// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		///// <param name="condition">查询条件类，包含了需要执行参数的值。</param>
		///// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		//internal protected int Fill(DataTable table, DataCommand dataCommand, AbstractCondition condition)
		//{
		//	using (dataCommand = BeginExecute(dataCommand))
		//	{
		//		dataCommand.ResetParameters(condition);
		//		return dataCommand.Fill(table);
		//	}
		//}

		///// <summary>
		///// 填充数据集
		///// </summary>
		///// <param name="table">待填充的实体类实例</param>
		///// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		///// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		//protected int Fill(DataTable table, string cmdName)
		//{
		//	StaticCommand dataCommand = CreateStaticCommand(cmdName);
		//	return this.Fill(table, dataCommand, 0, 0);
		//}

		///// <summary>
		///// 填充数据集
		///// </summary>
		///// <param name="table">待填充的实体类实例</param>
		///// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		///// <param name="entity">键/值对数组，包含了需要执行参数的值。</param>
		///// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		//protected int Fill(DataTable table, string cmdName, AbstractEntity entity)
		//{
		//	StaticCommand dataCommand = CreateStaticCommand(cmdName);
		//	return this.Fill(table, dataCommand, entity, 0, 0);
		//}

		///// <summary>
		///// 填充数据集
		///// </summary>
		///// <param name="table">待填充的实体类实例</param>
		///// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		///// <param name="entity">键/值对数组，包含了需要执行参数的值。</param>
		///// <param name="pageSize">需要查询的当前页大小</param>
		///// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		///// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		//protected int Fill(DataTable table, string cmdName, AbstractEntity entity, int pageSize, int pageIndex)
		//{
		//	StaticCommand dataCommand = CreateStaticCommand(cmdName);
		//	return this.Fill(table, dataCommand, entity, pageSize, pageIndex);
		//}

		///// <summary>
		///// 填充数据集
		///// </summary>
		///// <param name="table">待填充的实体类实例</param>
		///// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		///// <param name="condition">键/值对数组，包含了需要执行参数的值。</param>
		///// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		//protected int Fill(DataTable table, string cmdName, AbstractCondition condition)
		//{
		//	StaticCommand dataCommand = CreateStaticCommand(cmdName);
		//	return this.Fill(table, dataCommand, condition);
		//}

		///// <summary>
		///// 填充数据集
		///// </summary>
		///// <param name="table">待填充的实体类实例</param>
		///// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		///// <param name="anonObject">需要查询的当前页大小</param>
		///// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		//protected int Fill(DataTable table, string cmdName, object anonObject)
		//{
		//	StaticCommand dataCommand = CreateStaticCommand(cmdName);
		//	return this.Fill(table, dataCommand, anonObject);
		//}
		#endregion

		#region 执行数据库方法(GetDataTable<BaseTableRowType>)
		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Tables.BaseTableRowType的实体类。</typeparam>
		/// <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, DynamicCommand dataCommand)
			 where T : BaseTableRowType
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetDataTable<T>(table, null, 0, 0);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		///  <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, string cmdName, AbstractCondition condition)
			 where T : BaseTableRowType
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return GetDataTable<T>(table, dataCommand, condition);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Tables.BaseTableRowType的实体类。</typeparam>
		/// <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, DynamicCommand dataCommand, AbstractCondition condition)
			 where T : BaseTableRowType
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetDataTable<T>(table, condition);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		///  <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="dynamicObject">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, string cmdName, object dynamicObject)
			 where T : BaseTableRowType
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return GetDataTable<T>(table, dataCommand, dynamicObject);
		}


		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		///  <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, string cmdName, int pageSize, int pageIndex)
			 where T : BaseTableRowType
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return GetDataTable<T>(table, dataCommand, pageSize, pageIndex);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Tables.BaseTableRowType的实体类。</typeparam>
		/// <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, DynamicCommand dataCommand, int pageSize, int pageIndex)
			 where T : BaseTableRowType
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetDataTable<T>(table, pageSize, pageIndex);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Tables.BaseTableRowType的实体类。</typeparam>
		/// <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="dynamicObject">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, DynamicCommand dataCommand, object dynamicObject)
			 where T : BaseTableRowType
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetDataTable<T>(table, dynamicObject);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Tables.BaseTableRowType的实体类。</typeparam>
		/// <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="joinCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, DynamicCommand dataCommand, JoinCommand joinCommand, AbstractCondition condition)
			where T : BaseTableRowType
		{
			dataCommand.SetJoinCommand(joinCommand);
			return this.GetDataTable<T>(table, dataCommand, condition);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Tables.BaseTableRowType的实体类。</typeparam>
		/// <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="joinCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="dynamicObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, DynamicCommand dataCommand, JoinCommand joinCommand, object dynamicObject)
			where T : BaseTableRowType
		{
			dataCommand.SetJoinCommand(joinCommand);
			return this.GetDataTable<T>(table, dataCommand, dynamicObject);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Tables.BaseTableRowType的实体类。</typeparam>
		/// <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="commandName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="joinCommand">表示要对数据源执行的 SQL 语句或存储过程结构名称，此命令存在与 access 参数关联的类调用。</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, string commandName, JoinCommand joinCommand, AbstractCondition condition)
			where T : BaseTableRowType
		{
			DynamicCommand dataCommand = CreateDynamicCommand(commandName);
			return this.GetDataTable<T>(table, dataCommand, joinCommand, condition);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Tables.BaseTableRowType的实体类。</typeparam>
		/// <param name="table">需要填充数据的 DataTable 类实例。</param>
		/// <param name="commandName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="joinCommand">表示要对数据源执行的 SQL 语句或存储过程结构名称，此命令存在与 access 参数关联的类调用。</param>
		/// <param name="dynamicObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回可查询的实体列表。</returns>
		protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, string commandName, JoinCommand joinCommand, object dynamicObject)
			where T : BaseTableRowType
		{
			DynamicCommand dataCommand = CreateDynamicCommand(commandName);
			return this.GetDataTable<T>(table, dataCommand, joinCommand, dynamicObject);
		}
		#endregion

		#region 执行数据库方法(GetUpdateEntities<T>)
		/// <summary>执行UPDATE Transact-SQL 语句。</summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		public UpdateEntities<T> GetUpdateEntities<T>() where T : AbstractEntity
		{
			StaticCommand dataCommand = CreateStaticCommand();
			BeginStaticExecute(dataCommand);
			UpdateEntities<T> update = new UpdateEntities<T>(dataCommand);

			//UpdateEntities<EventLogEntity> update1 = new UpdateEntities<EventLogEntity>(dataCommand);
			//update1.Set(m => m.Controller);
			return update;
		}
		#endregion

		#region 执行数据库方法(GetDeleteEntities<T>)
		/// <summary>执行 DELETE Transact-SQL 语句</summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		public DeleteEntities<T> GetDeleteEntities<T>() where T : AbstractEntity
		{
			StaticCommand dataCommand = CreateStaticCommand();
			BeginStaticExecute(dataCommand);
			DeleteEntities<T> delete = new DeleteEntities<T>(dataCommand);
			return delete;
		}
		#endregion

		#region 执行数据库方法(GetEntities<Entity>)
		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="commandName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="joinCommand">表示要对数据源执行的 SQL 语句或存储过程结构名称，此命令存在与 access 参数关联的类调用。</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		protected QueryEntities<T> GetEntities<T>(string commandName, JoinCommand joinCommand, AbstractCondition condition) where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(commandName);
			return this.GetEntities<T>(dataCommand, joinCommand, condition);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="commandName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="joinCommand">表示要对数据源执行的 SQL 语句或存储过程结构名称，此命令存在与 access 参数关联的类调用。</param>
		/// <param name="dynamicObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回可查询的实体列表。</returns>
		protected QueryEntities<T> GetEntities<T>(string commandName, JoinCommand joinCommand, object dynamicObject) where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(commandName);
			return this.GetEntities<T>(dataCommand, joinCommand, dynamicObject);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="entity">实体类实例，其中包含了命令所需的参数。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		protected QueryEntities<T> GetEntities<T>(string cmdName, AbstractEntity entity, int pageSize, int pageIndex)
			 where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.GetEntities<T>(dataCommand, entity, pageSize, pageIndex);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		protected QueryEntities<T> GetEntities<T>(string cmdName, int pageSize, int pageIndex)
			 where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.GetEntities<T>(dataCommand, pageSize, pageIndex);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		protected QueryEntities<T> GetEntities<T>(string cmdName, AbstractCondition condition)
			 where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.GetEntities<T>(dataCommand, condition);
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="anonObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		protected QueryEntities<T> GetEntities<T>(string cmdName, object anonObject) where T : AbstractEntity, new()
		{
			if (anonObject == null) { throw new ArgumentNullException("anonObject"); }
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return GetEntities<T>(dataCommand, anonObject);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="joinCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryEntities<T> GetEntities<T>(DynamicCommand dataCommand, JoinCommand joinCommand, AbstractCondition condition) where T : AbstractEntity, new()
		{
			dataCommand.SetJoinCommand(joinCommand);
			return this.GetEntities<T>(dataCommand, condition);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="joinCommand">表示要对数据源执行的 SQL 语句或存储过程结构名称，此命令存在与 access 参数关联的类调用。</param>
		/// <param name="dynamicObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryEntities<T> GetEntities<T>(DynamicCommand dataCommand, JoinCommand joinCommand, object dynamicObject) where T : AbstractEntity, new()
		{
			dataCommand.SetJoinCommand(joinCommand);
			return this.GetEntities<T>(dataCommand, dynamicObject);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryEntities<T> GetEntities<T>(DynamicCommand dataCommand, int pageSize, int pageIndex)
			where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(null, pageSize, pageIndex);
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="dynamicObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		protected internal QueryEntities<T> GetEntities<T>(DynamicCommand dataCommand, object dynamicObject) where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(dynamicObject);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryEntities<T> GetEntities<T>(DynamicCommand dataCommand, AbstractCondition condition)
			where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(condition);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="entity">实体类实例，其中包含了命令所需的参数。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryEntities<T> GetEntities<T>(DynamicCommand dataCommand, AbstractEntity entity, int pageSize, int pageIndex)
			where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(entity, pageSize, pageIndex);
		}
		#endregion

		#region 执行数据库方法(GetJoinEntities<Entity>)
		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="entity">实体类实例，其中包含了命令所需的参数。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		protected QueryEntities<T> GetJoinEntities<T>(string cmdName, AbstractEntity entity, int pageSize, int pageIndex)
			 where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.GetJoinEntities<T>(dataCommand, entity, pageSize, pageIndex);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		protected QueryEntities<T> GetJoinEntities<T>(string cmdName, int pageSize, int pageIndex)
			 where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.GetJoinEntities<T>(dataCommand, pageSize, pageIndex);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		protected QueryEntities<T> GetJoinEntities<T>(string cmdName, AbstractCondition condition)
			 where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.GetJoinEntities<T>(dataCommand, condition);
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="anonObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		protected QueryEntities<T> GetJoinEntities<T>(string cmdName, object anonObject) where T : AbstractEntity, new()
		{
			if (anonObject == null) { throw new ArgumentNullException("anonObject"); }
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return GetJoinEntities<T>(dataCommand, anonObject);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryEntities<T> GetJoinEntities<T>(DynamicCommand dataCommand, int pageSize, int pageIndex)
			where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(null, pageSize, pageIndex);
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="dynamicObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		internal protected QueryEntities<T> GetJoinEntities<T>(DynamicCommand dataCommand, object dynamicObject) where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(dynamicObject);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="condition">查询数据库记录的条件，包含分页信息。</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryEntities<T> GetJoinEntities<T>(DynamicCommand dataCommand, AbstractCondition condition)
			where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(condition);
		}

		/// <summary>
		/// 获取可查询的实体列表类实例。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="entity">实体类实例，其中包含了命令所需的参数。</param>
		/// <param name="pageSize">需要查询的当前页大小</param>
		/// <param name="pageIndex">需要查询的当前页索引,索引从1开始</param>
		/// <returns>返回可查询的实体列表。</returns>
		internal protected QueryEntities<T> GetJoinEntities<T>(DynamicCommand dataCommand, AbstractEntity entity, int pageSize, int pageIndex)
			where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(entity, pageSize, pageIndex);
		}
		#endregion

		#region 执行数据库方法(GetPagination<Entity>)
		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		protected IPagination<T> GetPagination<T>(string cmdName) where T : AbstractEntity, new()
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return GetPagination<T>(dataCommand);
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="dynamicObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		protected IPagination<T> GetPagination<T>(string cmdName, object dynamicObject) where T : AbstractEntity, new()
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return GetPagination<T>(dataCommand, dynamicObject);
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="condition">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		protected IPagination<T> GetPagination<T>(string cmdName, AbstractCondition condition) where T : AbstractEntity, new()
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return GetPagination<T>(dataCommand, condition);
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		internal protected IPagination<T> GetPagination<T>(StaticCommand dataCommand) where T : AbstractEntity, new()
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				return dataCommand.GetPagination<T>(0, 0);
			}
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="dynamicObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		internal protected IPagination<T> GetPagination<T>(StaticCommand dataCommand, object dynamicObject) where T : AbstractEntity, new()
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				if (dynamicObject != null) { dataCommand.ResetParameters(dynamicObject); }
				return dataCommand.GetPagination<T>(dynamicObject);
			}
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="condition">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		internal protected IPagination<T> GetPagination<T>(StaticCommand dataCommand, AbstractCondition condition) where T : AbstractEntity, new()
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				if (condition != null) { dataCommand.ResetParameters(condition); }
				return dataCommand.GetPagination<T>(condition);
			}
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="pagination">需要填充的数据列表</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		protected IPagination<T> GetPagination<T>(string cmdName, Pagination<T> pagination) where T : AbstractEntity, new()
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return GetPagination<T>(dataCommand, pagination);
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="pagination">需要填充的数据列表</param>
		/// <param name="dynamicObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		protected IPagination<T> GetPagination<T>(string cmdName, Pagination<T> pagination, object dynamicObject) where T : AbstractEntity, new()
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return GetPagination<T>(dataCommand, pagination, dynamicObject);
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="pagination">需要填充的数据列表</param>
		/// <param name="condition">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		protected IPagination<T> GetPagination<T>(string cmdName, Pagination<T> pagination, AbstractCondition condition) where T : AbstractEntity, new()
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return GetPagination<T>(dataCommand, pagination, condition);
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="pagination">需要填充的数据列表</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		internal protected IPagination<T> GetPagination<T>(StaticCommand dataCommand, Pagination<T> pagination) where T : AbstractEntity, new()
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				return dataCommand.GetPagination<T>(pagination, 0, 0);
			}
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="pagination">需要填充的数据列表</param>
		/// <param name="dynamicObject">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		internal protected IPagination<T> GetPagination<T>(StaticCommand dataCommand, Pagination<T> pagination, object dynamicObject) where T : AbstractEntity, new()
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				if (dynamicObject != null) { dataCommand.ResetParameters(dynamicObject); }
				return dataCommand.GetPagination<T>(pagination, dynamicObject);
			}
		}

		/// <summary>
		/// 执行Transact-SQL 语句或存储过程，获取可分页的实体列表。
		/// </summary>
		/// <typeparam name="T">继承自Basic.Entity.AbstractEntity的实体类，需要默认构造函数。</typeparam>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="pagination">需要填充的数据列表</param>
		/// <param name="condition">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>返回执行Transact-SQL 语句或存储过程后，执行结果的可分页实体列表。</returns>
		internal protected IPagination<T> GetPagination<T>(StaticCommand dataCommand, Pagination<T> pagination, AbstractCondition condition) where T : AbstractEntity, new()
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				if (condition != null) { dataCommand.ResetParameters(condition); }
				return dataCommand.GetPagination<T>(pagination, condition);
			}
		}
		#endregion

		#region 执行数据库方法(Fill-System.Data.DataTable)

		/// <summary>填充数据集</summary>
		/// <param name="dataTable">需填充的数据集，一个 System.Data.DataTable 类实例。</param>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		protected int Fill(DataTable dataTable, string cmdName)
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.Fill(dataTable, dataCommand);
		}

		/// <summary>填充数据集</summary>
		/// <param name="table">需填充的数据集，一个 System.Data.DataTable 类实例。</param>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="anonObject">匿名类，包含了需要执行参数的值。</param>
		/// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		protected int Fill(System.Data.DataTable table, string cmdName, object anonObject)
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.Fill(table, dataCommand, anonObject);
		}

		/// <summary>填充数据集</summary>
		/// <param name="table">需填充的数据集，一个 System.Data.DataTable 类实例。</param>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="condition">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		protected int Fill(System.Data.DataTable table, string cmdName, AbstractCondition condition)
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.Fill(table, dataCommand, condition);
		}

		/// <summary>填充数据集</summary>
		/// <param name="table">需填充的数据集，一个 System.Data.DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <returns>执行Transact-SQL命令结果，已在 System.Data.DataTable 中成功添加或刷新的行数。</returns>
		internal protected int Fill(System.Data.DataTable table, StaticCommand dataCommand)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				return dataCommand.Fill(table);
			}
		}


		/// <summary>填充数据集</summary>
		/// <param name="table">需填充的数据集，一个 System.Data.DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="anonObject">匿名类，包含了需要执行参数的值。</param>
		/// <returns>执行Transact-SQL命令结果，已在 System.Data.DataTable 中成功添加或刷新的行数。</returns>
		internal protected int Fill(System.Data.DataTable table, StaticCommand dataCommand, object anonObject)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				if (anonObject != null) { dataCommand.ResetParameters(anonObject); }
				return dataCommand.Fill(table);
			}
		}

		/// <summary>填充数据集</summary>
		/// <param name="table">需填充的数据集，一个 System.Data.DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="condition">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>执行Transact-SQL命令结果，已在 System.Data.DataTable 中成功添加或刷新的行数。</returns>
		internal protected int Fill(System.Data.DataTable table, StaticCommand dataCommand, AbstractCondition condition)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				if (condition != null) { dataCommand.ResetParameters(condition); }
				return dataCommand.Fill(table);
			}
		}

		/// <summary>填充数据集</summary>
		/// <param name="table">需填充的数据集，一个 System.Data.DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <returns>执行Transact-SQL命令结果，已在 System.Data.DataTable 中成功添加或刷新的行数。</returns>
		internal protected int Fill(System.Data.DataTable table, DynamicCommand dataCommand)
		{
			using (dataCommand = BeginDynamicExecute(dataCommand))
			{
				dataCommand.InitializeParameters();
				dataCommand.InitializeCommandText(0, 0);
				return dataCommand.Fill(table);
			}
		}

		/// <summary>填充数据集</summary>
		/// <param name="table">需填充的数据集，一个 System.Data.DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="parameters">动态参数数组，包含了需要执行参数的值。</param>
		/// <returns>执行Transact-SQL命令结果，已在 System.Data.DataTable 中成功添加或刷新的行数。</returns>
		internal protected int Fill(System.Data.DataTable table, DynamicCommand dataCommand, DynamicParameter[] parameters)
		{
			using (dataCommand = BeginDynamicExecute(dataCommand))
			{
				dataCommand.InitializeParameters();
				if (parameters != null) { dataCommand.InitDynamicParameters(parameters); }
				dataCommand.InitializeCommandText(0, 0);
				return dataCommand.Fill(table);
			}
		}

		/// <summary>填充数据集</summary>
		/// <param name="table">需填充的数据集，一个 System.Data.DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="anonObject">匿名类，包含了需要执行参数的值。</param>
		/// <returns>执行Transact-SQL命令结果，已在 System.Data.DataTable 中成功添加或刷新的行数。</returns>
		internal protected int Fill(System.Data.DataTable table, DynamicCommand dataCommand, object anonObject)
		{
			using (dataCommand = BeginDynamicExecute(dataCommand))
			{
				dataCommand.InitializeParameters();
				int pageSize = 0, pageIndex = 0;
				if (anonObject != null)
				{
					dataCommand.ResetParameters(anonObject);
					Type anonType = anonObject.GetType();
					PropertyInfo sizeInfo = anonType.GetProperty("PageSize");
					if (sizeInfo != null) { pageSize = Convert.ToInt32(sizeInfo.GetValue(anonObject, null)); }

					PropertyInfo indexInfo = anonType.GetProperty("PageIndex");
					if (indexInfo != null) { pageIndex = Convert.ToInt32(indexInfo.GetValue(anonObject, null)); }
				}
				dataCommand.InitializeCommandText(pageSize, pageIndex);
				return dataCommand.Fill(table);
			}
		}

		/// <summary>填充数据集</summary>
		/// <param name="table">需填充的数据集，一个 System.Data.DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="condition">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>执行Transact-SQL命令结果，已在 System.Data.DataTable 中成功添加或刷新的行数。</returns>
		internal protected int Fill(System.Data.DataTable table, DynamicCommand dataCommand, AbstractCondition condition)
		{
			using (dataCommand = BeginDynamicExecute(dataCommand))
			{
				dataCommand.InitializeParameters();
				if (condition != null) { dataCommand.ResetParameters(condition); }
				dataCommand.InitializeCommandText(condition.PageSize, condition.PageIndex);
				return dataCommand.Fill(table);
			}
		}
		#endregion

		#region 执行数据库方法(Fill-System.Data.DataSet)
		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <param name="dataSet">需填充的数据集，一个 System.Data.DataSet 类实例。</param>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="anonObject">匿名类，包含了需要执行参数的值。</param>
		/// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		protected int Fill(System.Data.DataSet dataSet, string cmdName, object anonObject)
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.Fill(dataSet, dataCommand, anonObject);
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <param name="dataSet">需填充的数据集，一个 System.Data.DataSet 类实例。</param>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		protected int Fill(System.Data.DataSet dataSet, string cmdName)
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.Fill(dataSet, dataCommand);
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <param name="dataSet">需填充的数据集，一个 System.Data.DataSet 类实例。</param>
		/// <param name="cmdName">表示要对数据源执行的 SQL 语句或存储过程结构名称。</param>
		/// <param name="condition">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		protected int Fill(System.Data.DataSet dataSet, string cmdName, AbstractCondition condition)
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.Fill(dataSet, dataCommand, condition);
		}

		/// <summary>填充数据集</summary>
		/// <param name="dataSet">需填充的数据集，一个 System.Data.DataTable 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <returns>执行Transact-SQL命令结果，已在 System.Data.DataTable 中成功添加或刷新的行数。</returns>
		internal protected int Fill(System.Data.DataSet dataSet, DynamicCommand dataCommand)
		{
			using (dataCommand = BeginDynamicExecute(dataCommand))
			{
				dataCommand.InitializeParameters();
				dataCommand.InitializeCommandText(0, 0);
				return dataCommand.Fill(dataSet);
			}
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <param name="dataSet">需填充的数据集，一个 System.Data.DataSet 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="parameters">动态参数数组，包含了需要执行参数的值。</param>
		/// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		internal protected int Fill(System.Data.DataSet dataSet, DynamicCommand dataCommand, DynamicParameter[] parameters)
		{
			using (dataCommand = BeginDynamicExecute(dataCommand))
			{
				dataCommand.InitializeParameters();
				if (parameters != null) { dataCommand.InitDynamicParameters(parameters); }
				dataCommand.InitializeCommandText(0, 0);
				return dataCommand.Fill(dataSet);
			}
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <param name="dataSet">需填充的数据集，一个 System.Data.DataSet 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="anonObject">匿名类，包含了需要执行参数的值。</param>
		/// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		internal protected int Fill(System.Data.DataSet dataSet, DynamicCommand dataCommand, object anonObject)
		{
			using (dataCommand = BeginDynamicExecute(dataCommand))
			{
				dataCommand.InitializeParameters();
				int pageSize = 0, pageIndex = 0;
				if (anonObject != null)
				{
					dataCommand.ResetParameters(anonObject);
					Type anonType = anonObject.GetType();
					PropertyInfo sizeInfo = anonType.GetProperty("PageSize");
					if (sizeInfo != null) { pageSize = Convert.ToInt32(sizeInfo.GetValue(anonObject, null)); }

					PropertyInfo indexInfo = anonType.GetProperty("PageIndex");
					if (indexInfo != null) { pageIndex = Convert.ToInt32(indexInfo.GetValue(anonObject, null)); }
				}
				dataCommand.InitializeCommandText(pageSize, pageIndex);

				return dataCommand.Fill(dataSet);
			}
		}

		/// <summary>
		/// 填充数据集
		/// </summary>
		/// <param name="dataSet">需填充的数据集，一个 System.Data.DataSet 类实例。</param>
		/// <param name="dataCommand">表示要对数据源执行的 SQL 语句或存储过程结构。</param>
		/// <param name="condition">数据实体类，包含了需要执行参数的值。</param>
		/// <returns>如果执行成功则返回零，否则返回错误代码。</returns>
		internal protected int Fill(System.Data.DataSet dataSet, DynamicCommand dataCommand, AbstractCondition condition)
		{
			using (dataCommand = BeginDynamicExecute(dataCommand))
			{
				dataCommand.InitializeParameters();
				if (condition != null) { dataCommand.ResetParameters(condition); }
				dataCommand.InitializeCommandText(condition.PageSize, condition.PageIndex);
				return dataCommand.Fill(dataSet);
			}
		}
		#endregion

		#region 获取数据库命令
		/// <summary>创建 数据库表批处理命令</summary>
		/// <returns>返回 BatchCommand 类型对象</returns>
		internal protected BatchCommand CreateBatchCommand()
		{
			if (_TableInfo.Columns.HasRows == false)
			{
				using (CommandBuilder builder = _ConfigInfo.CreateCommandBuilder())
				{
					builder.CreateDataCommand(_TableInfo);
				}
			}
			return _ConnectionFactory.CreateBatchCommand(_dbConnection, _TableInfo);
		}

		/// <summary>
		/// 创建执行数据库的结构
		/// </summary>
		/// <param name="CommandName">命令名称</param>
		/// <returns>返回DataCommand结构信息</returns>
		internal virtual DataCommand CreateDataCommand(string CommandName)
		{
			if (_TableInfo.TryGetValue(CommandName, out DataCommand dataCommand))
			{
				return dataCommand.CloneCommand();
			}
			lock (_TableInfo)
			{
				using (CommandBuilder builder = _ConfigInfo.CreateCommandBuilder())
				{
					builder.CreateDataCommand(_TableInfo);
				}
			}
			if (_TableInfo.TryGetValue(CommandName, out dataCommand))
			{
				return dataCommand.CloneCommand();
			}
			throw new ConfigurationException("Access_NotExistsElement", CommandName);
		}

		/// <summary>
		/// 创建执行数据库的结构
		/// </summary>
		/// <returns>返回StaticCommand结构信息</returns>
		protected StaticCommand CreateStaticCommand()
		{
			return _ConnectionFactory.CreateStaticCommand();
		}

		/// <summary>
		/// 创建执行数据库的结构
		/// </summary>
		/// <param name="CommandName">命令名称</param>
		/// <returns>返回 StaticCommand 结构信息</returns>
		internal StaticCommand CreateStaticCommandOrNull(string CommandName)
		{
			return CreateDataCommand(CommandName) as StaticCommand;
		}

		/// <summary>
		/// 创建执行数据库的结构
		/// </summary>
		/// <param name="CommandName">命令名称</param>
		/// <returns>返回SqlStruct结构信息</returns>
		internal protected JoinCommand CreateJoinCommand(string CommandName)
		{
			DynamicCommand dynamicCommand = CreateDynamicCommand(CommandName);
			return new JoinCommand(dynamicCommand);
		}

		/// <summary>
		/// 创建执行数据库的结构
		/// </summary>
		/// <param name="CommandName">命令名称</param>
		/// <returns>返回StaticCommand结构信息</returns>
		protected internal StaticCommand CreateStaticCommand(string CommandName)
		{
			return CreateDataCommand(CommandName) as StaticCommand;
		}

		/// <summary>
		/// 创建执行数据库的结构
		/// </summary>
		/// <param name="CommandName">命令名称</param>
		/// <returns>返回SqlStruct结构信息</returns>
		protected internal DynamicCommand CreateDynamicCommand(string CommandName)
		{
			return CreateDataCommand(CommandName) as DynamicCommand;
		}

		/// <summary>
		/// 创建自定义动态命令
		/// </summary>
		/// <param name="selectText">要对数据源执行的 Transact-SQL 语句中 SELECT 数据库字段部分。</param>
		/// <param name="fromText">要对数据源执行的 Transact-SQL 语句中 FROM 数据库表部分。</param>
		/// <param name="whereText">要对数据源执行的 Transact-SQL 语句中 WHERE 条件部分。</param>
		/// <param name="orderText">要对数据源执行的 Transact-SQL 语句中 ORDER BY 条件部分。</param>
		/// <returns>返回创建成功的动态命令 DynamicCommand 子类实例(特定于某种数据库命令的实例)。</returns>
		internal protected DynamicCommand CreateDynamicCommand(string selectText, string fromText, string whereText, string orderText)
		{
			DynamicCommand dynamicCommand = _ConnectionFactory.CreateDynamicCommand();
			dynamicCommand.SelectText = selectText;
			dynamicCommand.FromText = fromText;
			dynamicCommand.WhereText = whereText;
			dynamicCommand.OrderText = orderText;
			return dynamicCommand;
		}

		/// <summary>
		/// 创建自定义动态命令
		/// </summary>
		/// <param name="selectText">要对数据源执行的 Transact-SQL 语句中 SELECT 数据库字段部分。</param>
		/// <param name="fromText">要对数据源执行的 Transact-SQL 语句中 FROM 数据库表部分。</param>
		/// <param name="whereText">要对数据源执行的 Transact-SQL 语句中 WHERE 条件部分。</param>
		/// <param name="orderText">要对数据源执行的 Transact-SQL 语句中 ORDER BY 条件部分。</param>
		/// <param name="groupText">要对数据源执行的 Transact-SQL 语句中 GROUP 部分</param>
		/// <param name="havingText">要对数据源执行的 Transact-SQL 语句中 HANVING 条件部分</param>
		/// <returns>返回创建成功的动态命令 DynamicCommand 子类实例(特定于某种数据库命令的实例)。</returns>
		internal protected DynamicCommand CreateDynamicCommand(string selectText, string fromText, string whereText, string orderText, string groupText, string havingText)
		{
			DynamicCommand dynamicCommand = CreateDynamicCommand(selectText, fromText, whereText, orderText);
			dynamicCommand.GroupText = groupText;
			dynamicCommand.HavingText = havingText;
			return dynamicCommand;
		}

		/// <summary>
		/// 创建命令执行参数名称
		/// </summary>
		/// <param name="noSymbolName">不带符号的参数名</param>
		/// <returns>返回创建成功的带符号的参数名称</returns>
		internal protected string CreateParameterName(string noSymbolName)
		{
			return _ConnectionFactory.CreateParameterName(noSymbolName);
		}
		#endregion

		#region 获取数据缓存
		/// <summary>
		/// 获取当前数据上下文管理类的缓存对象
		/// </summary>
		/// <returns>如果存在系统则返回缓存 ICacheClient 接口的实例，否则返回null。</returns>
		protected ICacheClient GetClient() { return CacheClientFactory.GetClient(_ConfigInfo.ConnectionName); }

		/// <summary>
		/// 获取当前数据上下文管理类的缓存对象
		/// </summary>
		/// <returns>如果存在系统则返回缓存 ICacheClient 接口的实例，否则返回null。</returns>
		protected ICacheClient GetClient(string name) { return CacheClientFactory.GetClient(name); }

		#endregion

		#region 手动控制事务
		/// <summary>
		/// 设置分布式事物为可提交状态
		/// </summary>
		public Result SetComplete()
		{
			if (commitTransaction != null && CommitTranState == TransactionState.ExistTransaction)
				CommitTranState = TransactionState.AllowCommitTransaction;
			return _Result;
		}
		/// <summary>
		/// 尝试手动提交事务。
		/// </summary>
		public Result Commit()
		{
			if (commitTransaction != null)
			{
				commitTransaction.Commit();
				CommitTranState = TransactionState.AlreadyCommitTransaction;
			}
			return _Result;
		}

		/// <summary>
		/// 回滚（中止）事务。
		/// </summary>
		public Result Rollback()
		{
			if (commitTransaction != null)
			{
				commitTransaction.Rollback();
				CommitTranState = TransactionState.AlreadyRollbackTransaction;
			}
			return _Result;
		}

		/// <summary>
		/// 回滚（中止）事务。
		/// </summary>
		/// <param name="e">有关发生回滚的原因的说明。</param>
		public Result Rollback(Exception e)
		{
			if (commitTransaction != null)
			{
				commitTransaction.Rollback(e);
				CommitTranState = TransactionState.AlreadyRollbackTransaction;
			}
			return _Result;
		}

		#endregion

		#region IDisposable 成员
		/// <summary>
		/// 释放资源，关闭数据库连接，自动提交(回滚)事务。
		/// </summary>
		public void Dispose()
		{
			try
			{
				if (commitTransaction != null)
				{
					if (CommitTranState == TransactionState.ExistTransaction)
					{
						commitTransaction.Rollback();
					}
					else if (CommitTranState == TransactionState.AllowCommitTransaction)
					{
						commitTransaction.Commit();
					}
				}
			}
			finally
			{
				if (_dbConnection != null) { _dbConnection.Close(); }
			}
		}

		/// <summary>
		/// 释放资源
		/// </summary>
		void IDisposable.Dispose()
		{
			Dispose();
		}
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
		ValueTask IAsyncDisposable.DisposeAsync()
		{
			try
			{
				if (commitTransaction != null)
				{
					if (CommitTranState == TransactionState.ExistTransaction)
					{
						commitTransaction.Rollback();
					}
					else if (CommitTranState == TransactionState.AllowCommitTransaction)
					{
						commitTransaction.Commit();
					}
				}
			}
			finally
			{
				if (_dbConnection != null) { _dbConnection.Close(); }
			}
#if NET5_0_OR_GREATER
			return ValueTask.CompletedTask;
#elif NETSTANDARD2_1_OR_GREATER
			return new ValueTask(Task.CompletedTask);
#endif
		}
#endif
		#endregion
	}
}
