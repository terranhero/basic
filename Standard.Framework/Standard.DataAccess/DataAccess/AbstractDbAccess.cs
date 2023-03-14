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
	/// ���ݲ�����
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0019:ʹ��ģʽƥ��", Justification = "<����>")]
	public abstract class AbstractDbAccess : global::System.IDisposable
#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
, System.IAsyncDisposable
#endif
	{
		#region ����
		/// <summary>
		/// �����ṹ���ý�����
		/// </summary>
		internal protected const string NewKeyConfig = "NewKeyConfig";

		/// <summary>
		/// �����ṹ���ý�����
		/// </summary>
		internal protected const string CreateName = "Create";

		/// <summary>
		/// �����ṹ���ý�����
		/// </summary>
		internal protected const string CreateConfig = "CreateConfig";

		/// <summary>
		/// ���½ṹ���ý�����
		/// </summary>
		internal protected const string ModifyName = "Update";

		/// <summary>
		/// ���½ṹ���ý�����
		/// </summary>
		internal protected const string ModifyConfig = "UpdateConfig";

		/// <summary>
		/// ɾ���ṹ���ý�����
		/// </summary>
		internal protected const string RemoveName = "Delete";

		/// <summary>
		/// ɾ���ṹ���ý�����
		/// </summary>
		internal protected const string RemoveConfig = "DeleteConfig";

		/// <summary>
		/// ɾ���ṹ���ý�����
		/// </summary>
		internal protected const string DeleteByFKeyConfig = "DeleteByFKeyConfig";

		/// <summary>
		/// ʹ�ùؼ���Ϊ������ѯ��¼�ṹ���ý�����
		/// </summary>
		internal protected const string SelectByKeyConfig = "SelectByKeyConfig";

		/// <summary>
		/// ʹ����ؼ���Ϊ������ѯ��¼�ṹ���ý�����
		/// </summary>
		internal protected const string SelectByFKeyConfig = "SelectByFKeyConfig";

		/// <summary>
		/// ��ѯ���м�¼�ṹ���ý�����
		/// </summary>
		internal protected const string SelectAllConfig = "SelectAllConfig";

		/// <summary>
		/// ��ѯ���м�¼�ṹ���ý�����
		/// </summary>
		internal protected const string SearchTableName = "SearchTable";

		/// <summary>
		/// ��ѯ���м�¼�ṹ���ý�����
		/// </summary>
		internal protected const string SearchTableConfig = "SearchTableConfig";
		#endregion

		#region ����
		/// <summary>���ݿ�����ִ�н����</summary>
		private readonly Result _Result;

		/// <summary>���ݿ����ӹ�����</summary>
		private readonly ConnectionFactory _ConnectionFactory;

		/// <summary>���ݿ����ݱ�������</summary>
		private readonly TableConfiguration _TableInfo;

		/// <summary>��ʾ ConfigurationAttribute ���Ͷ����ʵ������Ϣ</summary>
		private readonly ConfigurationInfo _ConfigInfo;

		/// <summary>��ʾ��ǰ�û���������Ϣ</summary>
		private readonly IUserContext _User;

		/// <summary>��ʾ��ǰ��������</summary>
		public string ConnectionName { get { return _ConfigInfo.ConnectionName; } }

		/// <summary>�����Ƿ��Ѿ����</summary>
		private TransactionState CommitTranState = TransactionState.NotExistTransaction;

		/// <summary>
		/// ��ȡ�ֲ�ʽ����ʵ��
		/// </summary>
		private CommittableTransaction commitTransaction = null;
		/// <summary>
		/// ��ȡ�ֲ�ʽ����ʵ��
		/// </summary>
		public CommittableTransaction CommitTransaction { get { return commitTransaction; } }

		private DbConnection _dbConnection;
		/// <summary>// ���ݿ�����</summary>
		public DbConnection DbConnection { get { return _dbConnection; } }
		#endregion

		#region ���캯��
		/// <summary>�������ݴ�����ʵ��(Ĭ�Ϲر�����)</summary>
		protected AbstractDbAccess() : this(null, false, null, null) { }

		/// <summary>�������ݴ�����ʵ��</summary>
		/// <param name="startTransaction">�Ƿ���������</param>
		protected AbstractDbAccess(bool startTransaction) : this(null, startTransaction, null, null) { }

		/// <summary>ʹ�����ݿ����Ӻ��������ͣ���ʼ�� AbstractDbAccess ��ʵ����</summary>
		/// <param name="connection">����������õ����ݿ���������</param>
		protected AbstractDbAccess(string connection) : this(new UserContext(connection), false, null, null) { }

		/// <summary>��ǰAbstractAccess�����Ƿ���Ҫ��������</summary>
		/// <param name="user">��ǰ�û���Ϣ(���������������ݿ��������ơ�����Session��)��</param>
		protected AbstractDbAccess(IUserContext user) : this(user, false, null, null) { _User = user; }

		/// <summary>ʹ�����ݿ����Ӻ��������ͣ���ʼ�� AbstractDbAccess ��ʵ����</summary>
		/// <param name="connection">����������õ����ݿ���������</param>
		/// <param name="startTransaction">�Ƿ���������</param>
		protected AbstractDbAccess(string connection, bool startTransaction) : this(new UserContext(connection), startTransaction, null, null) { }

		/// <summary>��ǰAbstractAccess�����Ƿ���Ҫ��������</summary>
		/// <param name="user">��ǰ�û���Ϣ(���������������ݿ��������ơ�����Session��)��</param>
		/// <param name="startTransaction">�Ƿ���������</param>
		protected AbstractDbAccess(IUserContext user, bool startTransaction) : this(user, startTransaction, null, null) { _User = user; }

		/// <summary>ʹ�����ݿ����Ӻ��������ͣ���ʼ�� AbstractDbAccess ��ʵ����</summary>
		/// <param name="connection">���ݿ������ַ���</param>
		/// <param name="timeout">һ��TimeSpan ���͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĳ�ʱʱ�����ơ�</param>
		protected AbstractDbAccess(string connection, TimeSpan timeout) : this(new UserContext(connection), true, null, new CommittableTransaction(timeout)) { }

		/// <summary>��ǰAbstractAccess�����Ƿ���Ҫ��������</summary>
		/// <param name="user">��ǰ�û���Ϣ(���������������ݿ��������ơ�����Session��)��</param>
		/// <param name="timeout">һ��TimeSpan ���͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĳ�ʱʱ�����ơ�</param>
		protected AbstractDbAccess(IUserContext user, TimeSpan timeout) : this(user, true, null, new CommittableTransaction(timeout)) { _User = user; }

		/// <summary>ʹ�����ݿ����Ӻ��������ͣ���ʼ�� AbstractDbAccess ��ʵ����</summary>
		/// <param name="connection">���ݿ������ַ���</param>
		/// <param name="isolationLevel">һ�� System.Transactions.IsolationLevel ö�����͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĸ��뼶��</param>
		/// <param name="timeout">һ��TimeSpan ���͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĳ�ʱʱ�����ơ�</param>
		protected AbstractDbAccess(string connection, ST.IsolationLevel isolationLevel, TimeSpan timeout)
			: this(new UserContext(connection), true, null, new CommittableTransaction(new TransactionOptions() { IsolationLevel = isolationLevel, Timeout = timeout })) { }

		/// <summary>��ǰAbstractAccess�����Ƿ���Ҫ��������</summary>
		/// <param name="user">��ǰ�û���Ϣ(���������������ݿ��������ơ�����Session��)��</param>
		/// <param name="isolationLevel">һ�� System.Transactions.IsolationLevel ö�����͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĸ��뼶��</param>
		/// <param name="timeout">һ��TimeSpan ���͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĳ�ʱʱ�����ơ�</param>
		protected AbstractDbAccess(IUserContext user, ST.IsolationLevel isolationLevel, TimeSpan timeout)
			: this(user, true, null, new CommittableTransaction(new TransactionOptions() { IsolationLevel = isolationLevel, Timeout = timeout })) { _User = user; }

		/// <summary>ʹ�����ݿ����Ӻ��������ͣ���ʼ�� AbstractDbAccess ��ʵ����</summary>
		/// <param name="connection">���ݿ������ַ���</param>
		/// <param name="transaction">�����ڵǼǵ����� Transaction �����á�</param>
		protected AbstractDbAccess(string connection, CommittableTransaction transaction)
			: this(new UserContext(connection), true, null, transaction) { }

		/// <summary>ʹ�����ݿ����Ӻ��������ͣ���ʼ�� AbstractDbAccess ��ʵ����/// </summary>
		/// <param name="user">��ǰ�û���Ϣ(���������������ݿ��������ơ�����Session��)��</param>
		/// <param name="transaction">�����ڵǼǵ����� Transaction �����á�</param>
		protected AbstractDbAccess(IUserContext user, CommittableTransaction transaction)
			: this(user, true, null, transaction) { _User = user; }

		/// <summary>�������ݴ�����ʵ��</summary>
		/// <param name="access">���ݿ�����ࡣ</param>
		protected AbstractDbAccess(AbstractDbAccess access) : this(null, false, access, null) { _User = access._User; }

		/// <summary>�������ݴ�����ʵ��</summary>
		/// <param name="transaction">�����ڵǼǵ����� Transaction �����á�</param>
		protected AbstractDbAccess(CommittableTransaction transaction) : this(null, true, null, transaction) { }

		/// <summary>ʹ��ָ��������ѡ���ʼ�� AbstractDbAccess �����ʵ��������������</summary>
		/// <param name="timeout">һ��TimeSpan ���͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĳ�ʱʱ�����ơ�</param>
		protected AbstractDbAccess(TimeSpan timeout)
			: this(null, true, null, new CommittableTransaction(timeout)) { }

		/// <summary>ʹ��ָ��������ѡ���ʼ�� AbstractDbAccess �����ʵ��������������</summary>
		/// <param name="isolationLevel">һ�� System.Transactions.IsolationLevel ö�����͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĸ��뼶��</param>
		/// <param name="timeout">һ��TimeSpan ���͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĳ�ʱʱ�����ơ�</param>
		protected AbstractDbAccess(ST.IsolationLevel isolationLevel, TimeSpan timeout)
			: this(null, true, null, new CommittableTransaction(new TransactionOptions() { IsolationLevel = isolationLevel, Timeout = timeout })) { }

		/// <summary>��ǰAbstractAccess�����Ƿ���Ҫ��������</summary>
		/// <param name="options">һ�� System.Transactions.TransactionOptions �ṹ���������������������ѡ�</param>
		protected AbstractDbAccess(TransactionOptions options) : this(null, true, null, new CommittableTransaction(options)) { }

		/// <summary>��ǰAbstractAccess�����Ƿ���Ҫ��������</summary>
		/// <param name="user">�û�����������Ϣ(�������ݿ����Ӻ��û�����)��</param>
		/// <param name="startTransaction">�Ƿ���������</param>
		/// <param name="access">���ݿ������</param>
		/// <param name="transaction">�����ڵǼǵ����� Transaction �����á�</param>
		private AbstractDbAccess(IUserContext user, bool startTransaction, AbstractDbAccess access, CommittableTransaction transaction)
		{
			_User = user ?? new UserContext(ConnectionContext.DefaultName);
			_Result = Result.Empty; Type accessType = GetType();
			ConfigurationAttribute ca = Attribute.GetCustomAttribute(accessType, typeof(ConfigurationAttribute)) as ConfigurationAttribute;
			//��{0}�������Զ�������{1}���޷���ȡ�����ļ���Ϣ��
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

		///// <summary>��ǰAbstractAccess�����Ƿ���Ҫ��������</summary>
		///// <param name="connectionName">���ݿ��������ƣ�������������Ӧ�ó���config�ļ��С�</param>
		///// <param name="startTransaction">�Ƿ���������</param>
		///// <param name="access">���ݿ������</param>
		///// <param name="transaction">�����ڵǼǵ����� Transaction �����á�</param>
		//private AbstractDbAccess(string connectionName, bool startTransaction, AbstractDbAccess access, CommittableTransaction transaction)
		//{
		//	_Result = Result.Empty; Type accessType = GetType();
		//	ConfigurationAttribute ca = Attribute.GetCustomAttribute(accessType, typeof(ConfigurationAttribute)) as ConfigurationAttribute;
		//	//��{0}�������Զ�������{1}���޷���ȡ�����ļ���Ϣ��
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
		/// �������ύ������
		/// </summary>
		/// <param name="transaction">���ύ����Ĭ��ֵΪ null��</param>
		internal void BeginTransaction(CommittableTransaction transaction)
		{
			if (transaction != null) { commitTransaction = transaction; }
			else { commitTransaction = new CommittableTransaction(); }
			_dbConnection.EnlistTransaction(commitTransaction);
			CommitTranState = TransactionState.ExistTransaction;
		}

		/// <summary>
		/// ʹ��ָ��������ѡ���ʼ�� System.Transactions.CommittableTransaction �����ʵ��������������
		/// </summary>
		/// <param name="isolationLevel">һ�� System.Transactions.IsolationLevel ö�����͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĸ��뼶��</param>
		/// <param name="timeout">һ��TimeSpan ���͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĳ�ʱʱ�����ơ�</param>
		internal void BeginTransaction(ST.IsolationLevel isolationLevel, TimeSpan timeout)
		{
			TransactionOptions options = new TransactionOptions() { IsolationLevel = isolationLevel, Timeout = timeout };
			commitTransaction = new CommittableTransaction(options);
			_dbConnection.EnlistTransaction(commitTransaction);
			CommitTranState = TransactionState.ExistTransaction;
		}

		/// <summary>
		/// ʹ��ָ��������ѡ���ʼ�� System.Transactions.CommittableTransaction �����ʵ��������������
		/// </summary>
		/// <param name="options">һ�� System.Transactions.TransactionOptions �ṹ���������������������ѡ�</param>
		internal void BeginTransaction(TransactionOptions options)
		{
			commitTransaction = new CommittableTransaction(options);
			_dbConnection.EnlistTransaction(commitTransaction);
			CommitTranState = TransactionState.ExistTransaction;
		}

		/// <summary>
		/// ʹ��ָ��������ѡ���ʼ�� System.Transactions.CommittableTransaction �����ʵ��������������
		/// </summary>
		/// <param name="commandName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
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
				//��{0}��������Ϣ����,����Ϊ{1}������������Ϣ�����ڣ�����Ӧ�ó��������ļ�(*.config)��
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

		#region �Ǽ����ݿ�����
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

		#region ��ȡ�����ַ�����Դ
		/// <summary>ʹ�� Lambda ���ʽ��ȡ������������ֵ��</summary>
		/// <param name="expression">Ҫ��ȡ����Դ����</param>
		/// <param name="item">�жϲ���</param>
		/// <returns> ��Ե��÷��ĵ�ǰ���������ö����ػ�����Դ��ֵ�������������ƥ����򷵻� null��</returns>
		/// <exception cref="System.ArgumentNullException">name ����Ϊ null��</exception>
		/// <exception cref="System.InvalidOperationException">ָ����Դ��ֵ�����ַ�����</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">δ�ҵ����õ���Դ��������û�з��ض������Ե���Դ��</exception>
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

		/// <summary>ʹ�� Lambda ���ʽ��ȡ������������ֵ��</summary>
		/// <param name="expression">Ҫ��ȡ����Դ����</param>
		/// <param name="item">�жϲ���</param>
		/// <returns> ��Ե��÷��ĵ�ǰ���������ö����ػ�����Դ��ֵ�������������ƥ����򷵻� null��</returns>
		/// <exception cref="System.ArgumentNullException">name ����Ϊ null��</exception>
		/// <exception cref="System.InvalidOperationException">ָ����Դ��ֵ�����ַ�����</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">δ�ҵ����õ���Դ��������û�з��ض������Ե���Դ��</exception>
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
		/// ����ָ���� System.String ��Դ��ֵ��
		/// </summary>
		/// <param name="name">Ҫ��ȡ����Դ����</param>
		/// <returns> ��Ե��÷��ĵ�ǰ���������ö����ػ�����Դ��ֵ�������������ƥ����򷵻� null��</returns>
		/// <exception cref="System.ArgumentNullException">name ����Ϊ null��</exception>
		/// <exception cref="System.InvalidOperationException">ָ����Դ��ֵ�����ַ�����</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">δ�ҵ����õ���Դ��������û�з��ض������Ե���Դ��</exception>
		protected string GetString(string name)
		{
			if (_User.Culture != null) { return MessageContext.GetString(name, _User.Culture); }
			return MessageContext.GetString(name);
		}

		/// <summary>
		/// ����ָ���� System.String ��Դ��ֵ��ʹ��ָ���Ĳ����滻��Դ�п�ȱ��ֵ
		/// </summary>
		/// <param name="name">��Դ����</param>
		/// <param name="args">һ���������飬���а����������Ҫ���ø�ʽ�Ķ���</param>
		/// <returns>Ϊָ�������Ա��ػ�����Դ��ֵ����������������ƥ�䣬�򷵻� null��</returns>
		protected string GetString(string name, params object[] args)
		{
			if (_User.Culture != null) { return MessageContext.GetString(name, _User.Culture, args); }
			return MessageContext.GetString(name, args);
		}

		/// <summary>
		/// ����ָ���� System.String ��Դ��ֵ��ʹ��ָ���Ĳ����滻��Դ�п�ȱ��ֵ
		/// </summary>
		/// <param name="converterName">��Ҫָ����ת�������ơ�</param>
		/// <param name="name">��Դ����</param>
		/// <returns> ��Ե��÷��ĵ�ǰ���������ö����ػ�����Դ��ֵ�������������ƥ����򷵻� null��</returns>
		protected string GetString(string converterName, string name)
		{
			if (_User.Culture != null) { return MessageContext.GetString(converterName, name, _User.Culture); }
			return MessageContext.GetString(converterName, name);
		}

		/// <summary>
		/// ����ָ���� System.String ��Դ��ֵ��ʹ��ָ���Ĳ����滻��Դ�п�ȱ��ֵ
		/// </summary>
		/// <param name="converterName">��Ҫָ����ת�������ơ�</param>
		/// <param name="name">��Դ����</param>
		/// <param name="args">һ���������飬���а����������Ҫ���ø�ʽ�Ķ���</param>
		/// <returns> ��Ե��÷��ĵ�ǰ���������ö����ػ�����Դ��ֵ�������������ƥ����򷵻� null��</returns>
		protected string GetString(string converterName, string name, params object[] args)
		{
			if (_User.Culture != null) { return MessageContext.GetString(converterName, name, _User.Culture, args); }
			return MessageContext.GetString(converterName, name, args);
		}
		#endregion

		#region ִ�����ݿⷽ��(ExecuteNonQueryAsync)
		/// <summary>
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
		protected System.Threading.Tasks.Task<Result> ExecuteNonQueryAsync(string cmdName)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				return this.ExecuteNonQueryAsync(dataCommand);
			}
		}

		/// <summary>
		/// ����������Ϊ������ִ��Transact-SQL��������ִ��NewValues���ڵ�������ֵ���
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="anonObject">��/ֵ�����飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
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
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="entity">ʵ�������飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
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
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="entities">ʵ�������飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
		protected System.Threading.Tasks.Task<Result> ExecuteNonQueryAsync(string cmdName, AbstractEntity[] entities)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return this.ExecuteNonQueryAsync(dataCommand, entities);
		}

		/// <summary>
		/// ����������Ϊ������ִ��Transact-SQL��������ִ��NewValues���ڵ�������ֵ���
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="anonObject">������ִ�в����������ࡣ</param>
		/// <exception cref="System.ArgumentNullException">���� entities Ϊnull�����鳤��Ϊ0��</exception>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
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
		/// ����������Ϊ������ִ��Transact-SQL��������ִ��NewValues���ڵ�������ֵ���
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="entity">������ִ�в������ࡣ</param>
		/// <exception cref="System.ArgumentNullException">���� entities Ϊnull�����鳤��Ϊ0��</exception>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
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
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="entities">ʵ�������飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
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
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢��������ṹ��</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
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

		#region ִ�����ݿⷽ��(ExecuteNonQuery)
		/// <summary>
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
		protected Result ExecuteNonQuery(string cmdName)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteNonQuery(dataCommand);
		}

		/// <summary>
		/// ����������Ϊ������ִ��Transact-SQL��������ִ��NewValues���ڵ�������ֵ���
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="anonObject">��/ֵ�����飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
		protected Result ExecuteNonQuery(string cmdName, object anonObject)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteNonQuery(dataCommand, anonObject);
		}

		/// <summary>
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="entity">ʵ�������飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
		protected Result ExecuteNonQuery(string cmdName, AbstractEntity entity)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteNonQuery(dataCommand, entity);
		}

		/// <summary>
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="entities">ʵ�������飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
		protected Result ExecuteNonQuery(string cmdName, AbstractEntity[] entities)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteNonQuery(dataCommand, entities);
		}

		/// <summary>
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
		internal protected Result ExecuteNonQuery(StaticCommand dataCommand)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				_Result.AffectedRows = dataCommand.ExecuteNonQuery();
				return _Result;
			}
		}

		/// <summary>
		/// ����������Ϊ������ִ��Transact-SQL��������ִ��NewValues���ڵ�������ֵ���
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="anonObject">������ִ�в����������ࡣ</param>
		/// <exception cref="System.ArgumentNullException">���� entities Ϊnull�����鳤��Ϊ0��</exception>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
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
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="entities">ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <exception cref="System.ArgumentNullException">���� entities Ϊnull�����鳤��Ϊ0��</exception>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
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
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="entity">ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <exception cref="System.ArgumentNullException">���� entities Ϊnull�����鳤��Ϊ0��</exception>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
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

		#region �첽ִ�����ݿⷽ��(BatchExecuteAsync)
		/// <summary>
		/// ʹ�� XXXBulkCopy ��ִ�����ݲ�������
		/// </summary>
		/// <param name="batchCommand">����������</param>
		/// <param name="table">���� BaseTableType&lt;BaseTableRowType&gt; ������ʵ������������Ҫִ�в�����ֵ��</param>
		/// <param name="timeout">��ʱ֮ǰ��������������������</param>
		/// <returns>ִ��Transact-SQL����洢���̺�ķ��ؽ����</returns>
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
		/// ʹ�� XXXBulkCopy ��ִ�����ݲ�������
		/// </summary>
		/// <param name="table">���� BaseTableType&lt;BaseTableRowType&gt; ������ʵ������������Ҫִ�в�����ֵ��</param>
		/// <param name="timeout">��ʱ֮ǰ��������������������</param>
		/// <returns>ִ��Transact-SQL����洢���̺�ķ��ؽ����</returns>
		protected Task<Result> BatchExecuteAsync<TR>(BaseTableType<TR> table, int timeout) where TR : BaseTableRowType
		{
			BatchCommand batchCommand = CreateBatchCommand();
			return BatchExecuteAsync(batchCommand, table, timeout);
		}

		/// <summary>
		/// ʹ�� XXXBulkCopy ��ִ�����ݲ�������
		/// </summary>
		/// <param name="table">���� BaseTableType&lt;BaseTableRowType&gt; ������ʵ������������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��Transact-SQL����洢���̺�ķ��ؽ����</returns>
		protected Task<Result> BatchExecuteAsync<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			BatchCommand batchCommand = CreateBatchCommand();
			return BatchExecuteAsync(batchCommand, table, 30);
		}
		#endregion

		#region ִ�����ݿⷽ��(BatchExecute)
		/// <summary>
		/// ʹ�� XXXBulkCopy ��ִ�����ݲ�������
		/// </summary>
		/// <param name="batchCommand">����������</param>
		/// <param name="table">���� BaseTableType&lt;BaseTableRowType&gt; ������ʵ������������Ҫִ�в�����ֵ��</param>
		/// <param name="timeout">��ʱ֮ǰ��������������������</param>
		/// <returns>ִ��Transact-SQL����洢���̺�ķ��ؽ����</returns>
		protected Result BatchExecute<TR>(BatchCommand batchCommand, BaseTableType<TR> table, int timeout) where TR : BaseTableRowType
		{
			using (batchCommand)
			{
				batchCommand.BatchExecute<TR>(table, timeout);
				return _Result;
			}
		}


		/// <summary>
		/// ʹ�� XXXBulkCopy ��ִ�����ݲ�������
		/// </summary>
		/// <param name="table">���� BaseTableType&lt;BaseTableRowType&gt; ������ʵ������������Ҫִ�в�����ֵ��</param>
		/// <param name="timeout">��ʱ֮ǰ��������������������</param>
		/// <returns>ִ��Transact-SQL����洢���̺�ķ��ؽ����</returns>
		protected Result BatchExecute<TR>(BaseTableType<TR> table, int timeout) where TR : BaseTableRowType
		{
			BatchCommand batchCommand = CreateBatchCommand();
			return BatchExecute(batchCommand, table, timeout);
		}

		/// <summary>
		/// ʹ�� XXXBulkCopy ��ִ�����ݲ�������
		/// </summary>
		/// <param name="table">���� BaseTableType&lt;BaseTableRowType&gt; ������ʵ������������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��Transact-SQL����洢���̺�ķ��ؽ����</returns>
		protected Result BatchExecute<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			BatchCommand batchCommand = CreateBatchCommand();
			return BatchExecute(batchCommand, table, 30);
		}
		#endregion

		#region ִ�����ݿⷽ��(ExecuteCore)
		/// <summary>
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢��������ṹ��</param>
		/// <param name="table">���� BaseTableType&lt;BaseTableRowType&gt; ������ʵ������������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
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
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢��������ṹ��</param>
		/// <param name="row"><![CDATA[���� BaseTableType&lt;BaseTableRowType&gt; ������ʵ������������Ҫִ�в�����ֵ��]]></param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
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
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="table">���� BaseTableType&lt;BaseTableRowType&gt; ������ʵ������������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
		protected Result ExecuteCore<TR>(string cmdName, BaseTableType<TR> table) where TR : BaseTableRowType
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteCore<TR>(dataCommand, table);
		}

		/// <summary>
		/// ִ��Transact-SQL����
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="table">ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
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
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="row">��/ֵ�����飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
		protected Result ExecuteCore<TR>(string cmdName, TR row) where TR : BaseTableRowType
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteCore<TR>(dataCommand, row);
		}

		/// <summary>
		/// ִ��Transact-SQL����
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="row">��/ֵ�����飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
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

		#region ִ�����ݿⷽ��(ExecuteScalar)
		/// <summary>
		/// ִ�в�ѯ�������ز�ѯ�����صĽ�����е�һ�еĵ�һ�С����Զ�����л���.
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢�������ơ�</param>
		/// <returns>���ز�ѯ��������Ϊ���򷵻ؿ�����</returns>
		protected object ExecuteScalar(string cmdName)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteScalar(dataCommand);
		}

		/// <summary>
		/// ����������Ϊ������ִ��Transact-SQL���
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="anonObject">������ִ�в����������ࡣ</param>
		/// <exception cref="System.ArgumentNullException">���� anonObject Ϊnull��</exception>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
		protected object ExecuteScalar(string cmdName, object anonObject)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteScalar(dataCommand, anonObject);
		}

		/// <summary>
		/// ִ�в�ѯ�������ز�ѯ�����صĽ�����е�һ�еĵ�һ�С����Զ�����л���.
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢�������ơ�</param>
		/// <param name="objArray">����ִ������Ĳ�����Ϣ��</param>
		/// <returns>���ز�ѯ��������Ϊ���򷵻ؿ�����</returns>
		protected object ExecuteScalar(string cmdName, BaseTableRowType objArray)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteScalar(dataCommand, objArray);
		}

		/// <summary>
		/// ��ȡ���ݿ��ֻ����
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢�������ơ�</param>
		/// <param name="entity">ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ز�ѯ��������Ϊ���򷵻ؿ�����</returns>
		protected object ExecuteScalar(string cmdName, AbstractEntity entity)
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return ExecuteScalar(dataCommand, entity);
		}

		/// <summary>
		/// ִ�в�ѯ�������ز�ѯ�����صĽ�����е�һ�еĵ�һ�С����Զ�����л���.
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <returns>���ز�ѯ��������Ϊ���򷵻ؿ�����</returns>
		internal protected object ExecuteScalar(StaticCommand dataCommand)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				return dataCommand.ExecuteScalar();
			}
		}

		/// <summary>
		/// ����������Ϊ������ִ��Transact-SQL���
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="anonObject">������ִ�в����������ࡣ</param>
		/// <exception cref="System.ArgumentNullException">���� entities Ϊnull�����鳤��Ϊ0��</exception>
		/// <returns>ִ��ransact-SQL���ķ���ֵ</returns>
		internal protected object ExecuteScalar(StaticCommand dataCommand, object anonObject)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.ResetParameters(anonObject);
				return dataCommand.ExecuteScalar();
			}
		}


		/// <summary>
		/// ִ�в�ѯ�������ز�ѯ�����صĽ�����е�һ�еĵ�һ�С����Զ�����л���.
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="entity">ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ز�ѯ��������Ϊ���򷵻ؿ�����</returns>
		protected internal object ExecuteScalar(StaticCommand dataCommand, AbstractEntity entity)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.ResetParameters(entity);
				return dataCommand.ExecuteScalar();
			}
		}

		/// <summary>
		/// ִ�в�ѯ�������ز�ѯ�����صĽ�����е�һ�еĵ�һ�С����Զ�����л���.
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="objArray">��/ֵ�����飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ز�ѯ��������Ϊ���򷵻ؿ�����</returns>
		internal protected object ExecuteScalar(StaticCommand dataCommand, BaseTableRowType objArray)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				dataCommand.ResetParameters(objArray);
				return dataCommand.ExecuteScalar();
			}
		}

		#endregion

		#region ִ�����ݿⷽ��(ExecuteReader)

		/// <summary>
		/// ��ȡ���ݿ��ֻ����
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <returns>���� DbDataReader ��Ӧ���ݿ��ʵ����</returns>
		protected DbDataReader ExecuteReader(string cmdName)
		{
			DataCommand dataCommand = CreateDataCommand(cmdName);
			return ExecuteReader(dataCommand);
		}

		/// <summary>
		/// ����������Ϊ������ִ��Transact-SQL���
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="anonObject">������ִ�в����������ࡣ</param>
		/// <exception cref="System.ArgumentNullException">���� anonObject Ϊnull��</exception>
		/// <returns>���� DbDataReader ��Ӧ���ݿ��ʵ����</returns>
		protected DbDataReader ExecuteReader(string cmdName, object anonObject)
		{
			DataCommand dataCommand = CreateDataCommand(cmdName);
			return ExecuteReader(dataCommand, anonObject);
		}

		/// <summary>
		/// ��ȡ���ݿ��ֻ����
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="objArray">��/ֵ�����飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���� DbDataReader ��Ӧ���ݿ��ʵ����</returns>
		protected DbDataReader ExecuteReader(string cmdName, BaseTableRowType objArray)
		{
			DataCommand dataCommand = CreateDataCommand(cmdName);
			return ExecuteReader(dataCommand, objArray);
		}

		/// <summary>
		/// ��ȡ���ݿ��ֻ����
		/// </summary>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="entity">ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���� DbDataReader ��Ӧ���ݿ��ʵ����</returns>
		protected DbDataReader ExecuteReader(string cmdName, AbstractEntity entity)
		{
			DataCommand dataCommand = CreateDataCommand(cmdName);
			return ExecuteReader(dataCommand, entity);
		}

		/// <summary>
		/// ��ȡ���ݿ��ֻ����
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <returns>���� DbDataReader ��Ӧ���ݿ��ʵ����</returns>
		internal protected DbDataReader ExecuteReader(DataCommand dataCommand)
		{
			using (dataCommand = BeginExecute(dataCommand))
			{
				return dataCommand.ExecuteReader();
			}
		}

		/// <summary>
		/// ����������Ϊ������ִ��Transact-SQL���
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="dynamicObject">������ִ�в����������ࡣ</param>
		/// <exception cref="System.ArgumentNullException">���� entities Ϊnull�����鳤��Ϊ0��</exception>
		/// <returns>���� DbDataReader ��Ӧ���ݿ��ʵ����</returns>
		internal protected DbDataReader ExecuteReader(DataCommand dataCommand, object dynamicObject)
		{
			using (dataCommand = BeginExecute(dataCommand))
			{
				dataCommand.ResetParameters(dynamicObject);
				return dataCommand.ExecuteReader();
			}
		}

		/// <summary>
		/// ��ȡ���ݿ��ֻ����
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="entity">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���� DbDataReader ��Ӧ���ݿ��ʵ����</returns>
		internal protected DbDataReader ExecuteReader(DataCommand dataCommand, AbstractEntity entity)
		{
			using (dataCommand = BeginExecute(dataCommand))
			{
				dataCommand.ResetParameters(entity);
				return dataCommand.ExecuteReader();
			}
		}

		/// <summary>
		/// ��ȡ���ݿ��ֻ����
		/// </summary>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̡�</param>
		/// <param name="objArray">��/ֵ�����飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���� DbDataReader ��Ӧ���ݿ��ʵ����</returns>
		internal protected DbDataReader ExecuteReader(DataCommand dataCommand, BaseTableRowType objArray)
		{
			using (dataCommand = BeginExecute(dataCommand))
			{
				dataCommand.ResetParameters(objArray);
				return dataCommand.ExecuteReader();
			}
		}
		#endregion

		#region ִ�����ݿⷽ��(SearchEntity-Basic.Entity.AbstractEntity)
		/// <summary>
		/// ���ؼ��ֲ�ѯ��¼
		/// </summary>
		/// <param name="entity">��Ҫ����Basic.Entity.AbstractEntity��ʵ��</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <returns>�����ѯ��������򷵻�true�����򷵻�false��</returns>
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
		/// ���ؼ��ֲ�ѯ��¼
		/// </summary>
		/// <param name="entity">��Ҫ����Basic.Entity.AbstractEntity��ʵ��</param>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <returns>�����ѯ��������򷵻�true�����򷵻�false��</returns>
		protected bool SearchEntity(string cmdName, AbstractEntity entity)
		{
			using (DataCommand dataCommand = CreateDataCommand(cmdName))
			{
				return this.SearchEntity(dataCommand, entity);
			}
		}
		#endregion

		#region ִ�����ݿⷽ��(Fill-DataTable)
		///// <summary>
		///// ������ݼ�
		///// </summary>
		///// <param name="table">������ʵ����ʵ��</param>
		///// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		///// <param name="anonObject">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		///// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		//internal protected int Fill(DataTable table, DataCommand dataCommand, object anonObject)
		//{
		//	using (dataCommand = BeginExecute(dataCommand))
		//	{
		//		dataCommand.ResetParameters(anonObject);
		//		return dataCommand.Fill(table);
		//	}
		//}

		///// <summary>
		///// ������ݼ�
		///// </summary>
		///// <param name="table">������ʵ����ʵ��</param>
		///// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		///// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		///// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		///// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		//internal protected int Fill(DataTable table, DataCommand dataCommand, int pageSize, int pageIndex)
		//{
		//	using (dataCommand = BeginExecute(dataCommand))
		//	{
		//		dataCommand.ResetPaginationParameter(pageSize, pageIndex);
		//		return dataCommand.Fill(table);
		//	}
		//}

		///// <summary>
		///// ������ݼ�
		///// </summary>
		///// <param name="table">������ʵ����ʵ��</param>
		///// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		///// <param name="entity">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		///// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		///// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		///// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
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
		///// ������ݼ�
		///// </summary>
		///// <param name="table">������ʵ����ʵ��</param>
		///// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		///// <param name="condition">��ѯ�����࣬��������Ҫִ�в�����ֵ��</param>
		///// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		//internal protected int Fill(DataTable table, DataCommand dataCommand, AbstractCondition condition)
		//{
		//	using (dataCommand = BeginExecute(dataCommand))
		//	{
		//		dataCommand.ResetParameters(condition);
		//		return dataCommand.Fill(table);
		//	}
		//}

		///// <summary>
		///// ������ݼ�
		///// </summary>
		///// <param name="table">������ʵ����ʵ��</param>
		///// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		///// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		//protected int Fill(DataTable table, string cmdName)
		//{
		//	StaticCommand dataCommand = CreateStaticCommand(cmdName);
		//	return this.Fill(table, dataCommand, 0, 0);
		//}

		///// <summary>
		///// ������ݼ�
		///// </summary>
		///// <param name="table">������ʵ����ʵ��</param>
		///// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		///// <param name="entity">��/ֵ�����飬��������Ҫִ�в�����ֵ��</param>
		///// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		//protected int Fill(DataTable table, string cmdName, AbstractEntity entity)
		//{
		//	StaticCommand dataCommand = CreateStaticCommand(cmdName);
		//	return this.Fill(table, dataCommand, entity, 0, 0);
		//}

		///// <summary>
		///// ������ݼ�
		///// </summary>
		///// <param name="table">������ʵ����ʵ��</param>
		///// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		///// <param name="entity">��/ֵ�����飬��������Ҫִ�в�����ֵ��</param>
		///// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		///// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		///// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		//protected int Fill(DataTable table, string cmdName, AbstractEntity entity, int pageSize, int pageIndex)
		//{
		//	StaticCommand dataCommand = CreateStaticCommand(cmdName);
		//	return this.Fill(table, dataCommand, entity, pageSize, pageIndex);
		//}

		///// <summary>
		///// ������ݼ�
		///// </summary>
		///// <param name="table">������ʵ����ʵ��</param>
		///// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		///// <param name="condition">��/ֵ�����飬��������Ҫִ�в�����ֵ��</param>
		///// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		//protected int Fill(DataTable table, string cmdName, AbstractCondition condition)
		//{
		//	StaticCommand dataCommand = CreateStaticCommand(cmdName);
		//	return this.Fill(table, dataCommand, condition);
		//}

		///// <summary>
		///// ������ݼ�
		///// </summary>
		///// <param name="table">������ʵ����ʵ��</param>
		///// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		///// <param name="anonObject">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		///// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		//protected int Fill(DataTable table, string cmdName, object anonObject)
		//{
		//	StaticCommand dataCommand = CreateStaticCommand(cmdName);
		//	return this.Fill(table, dataCommand, anonObject);
		//}
		#endregion

		#region ִ�����ݿⷽ��(GetDataTable<BaseTableRowType>)
		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Tables.BaseTableRowType��ʵ���ࡣ</typeparam>
		/// <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, DynamicCommand dataCommand)
			 where T : BaseTableRowType
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetDataTable<T>(table, null, 0, 0);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		///  <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, string cmdName, AbstractCondition condition)
			 where T : BaseTableRowType
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return GetDataTable<T>(table, dataCommand, condition);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Tables.BaseTableRowType��ʵ���ࡣ</typeparam>
		/// <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, DynamicCommand dataCommand, AbstractCondition condition)
			 where T : BaseTableRowType
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetDataTable<T>(table, condition);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		///  <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="dynamicObject">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, string cmdName, object dynamicObject)
			 where T : BaseTableRowType
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return GetDataTable<T>(table, dataCommand, dynamicObject);
		}


		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		///  <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		/// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, string cmdName, int pageSize, int pageIndex)
			 where T : BaseTableRowType
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return GetDataTable<T>(table, dataCommand, pageSize, pageIndex);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Tables.BaseTableRowType��ʵ���ࡣ</typeparam>
		/// <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		/// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, DynamicCommand dataCommand, int pageSize, int pageIndex)
			 where T : BaseTableRowType
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetDataTable<T>(table, pageSize, pageIndex);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Tables.BaseTableRowType��ʵ���ࡣ</typeparam>
		/// <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="dynamicObject">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, DynamicCommand dataCommand, object dynamicObject)
			 where T : BaseTableRowType
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetDataTable<T>(table, dynamicObject);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Tables.BaseTableRowType��ʵ���ࡣ</typeparam>
		/// <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="joinCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, DynamicCommand dataCommand, JoinCommand joinCommand, AbstractCondition condition)
			where T : BaseTableRowType
		{
			dataCommand.SetJoinCommand(joinCommand);
			return this.GetDataTable<T>(table, dataCommand, condition);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Tables.BaseTableRowType��ʵ���ࡣ</typeparam>
		/// <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="joinCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="dynamicObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, DynamicCommand dataCommand, JoinCommand joinCommand, object dynamicObject)
			where T : BaseTableRowType
		{
			dataCommand.SetJoinCommand(joinCommand);
			return this.GetDataTable<T>(table, dataCommand, dynamicObject);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Tables.BaseTableRowType��ʵ���ࡣ</typeparam>
		/// <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="commandName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="joinCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ƣ������������ access ��������������á�</param>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, string commandName, JoinCommand joinCommand, AbstractCondition condition)
			where T : BaseTableRowType
		{
			DynamicCommand dataCommand = CreateDynamicCommand(commandName);
			return this.GetDataTable<T>(table, dataCommand, joinCommand, condition);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Tables.BaseTableRowType��ʵ���ࡣ</typeparam>
		/// <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="commandName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="joinCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ƣ������������ access ��������������á�</param>
		/// <param name="dynamicObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, string commandName, JoinCommand joinCommand, object dynamicObject)
			where T : BaseTableRowType
		{
			DynamicCommand dataCommand = CreateDynamicCommand(commandName);
			return this.GetDataTable<T>(table, dataCommand, joinCommand, dynamicObject);
		}
		#endregion

		#region ִ�����ݿⷽ��(GetUpdateEntities<T>)
		/// <summary>ִ��UPDATE Transact-SQL ��䡣</summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
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

		#region ִ�����ݿⷽ��(GetDeleteEntities<T>)
		/// <summary>ִ�� DELETE Transact-SQL ���</summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		public DeleteEntities<T> GetDeleteEntities<T>() where T : AbstractEntity
		{
			StaticCommand dataCommand = CreateStaticCommand();
			BeginStaticExecute(dataCommand);
			DeleteEntities<T> delete = new DeleteEntities<T>(dataCommand);
			return delete;
		}
		#endregion

		#region ִ�����ݿⷽ��(GetEntities<Entity>)
		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="commandName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="joinCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ƣ������������ access ��������������á�</param>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		protected QueryEntities<T> GetEntities<T>(string commandName, JoinCommand joinCommand, AbstractCondition condition) where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(commandName);
			return this.GetEntities<T>(dataCommand, joinCommand, condition);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="commandName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="joinCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ƣ������������ access ��������������á�</param>
		/// <param name="dynamicObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		protected QueryEntities<T> GetEntities<T>(string commandName, JoinCommand joinCommand, object dynamicObject) where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(commandName);
			return this.GetEntities<T>(dataCommand, joinCommand, dynamicObject);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="entity">ʵ����ʵ�������а�������������Ĳ�����</param>
		/// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		/// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		protected QueryEntities<T> GetEntities<T>(string cmdName, AbstractEntity entity, int pageSize, int pageIndex)
			 where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.GetEntities<T>(dataCommand, entity, pageSize, pageIndex);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		/// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		protected QueryEntities<T> GetEntities<T>(string cmdName, int pageSize, int pageIndex)
			 where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.GetEntities<T>(dataCommand, pageSize, pageIndex);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		protected QueryEntities<T> GetEntities<T>(string cmdName, AbstractCondition condition)
			 where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.GetEntities<T>(dataCommand, condition);
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="anonObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		protected QueryEntities<T> GetEntities<T>(string cmdName, object anonObject) where T : AbstractEntity, new()
		{
			if (anonObject == null) { throw new ArgumentNullException("anonObject"); }
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return GetEntities<T>(dataCommand, anonObject);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="joinCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryEntities<T> GetEntities<T>(DynamicCommand dataCommand, JoinCommand joinCommand, AbstractCondition condition) where T : AbstractEntity, new()
		{
			dataCommand.SetJoinCommand(joinCommand);
			return this.GetEntities<T>(dataCommand, condition);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="joinCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ƣ������������ access ��������������á�</param>
		/// <param name="dynamicObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryEntities<T> GetEntities<T>(DynamicCommand dataCommand, JoinCommand joinCommand, object dynamicObject) where T : AbstractEntity, new()
		{
			dataCommand.SetJoinCommand(joinCommand);
			return this.GetEntities<T>(dataCommand, dynamicObject);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		/// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryEntities<T> GetEntities<T>(DynamicCommand dataCommand, int pageSize, int pageIndex)
			where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(null, pageSize, pageIndex);
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="dynamicObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		protected internal QueryEntities<T> GetEntities<T>(DynamicCommand dataCommand, object dynamicObject) where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(dynamicObject);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryEntities<T> GetEntities<T>(DynamicCommand dataCommand, AbstractCondition condition)
			where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(condition);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="entity">ʵ����ʵ�������а�������������Ĳ�����</param>
		/// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		/// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryEntities<T> GetEntities<T>(DynamicCommand dataCommand, AbstractEntity entity, int pageSize, int pageIndex)
			where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(entity, pageSize, pageIndex);
		}
		#endregion

		#region ִ�����ݿⷽ��(GetJoinEntities<Entity>)
		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="entity">ʵ����ʵ�������а�������������Ĳ�����</param>
		/// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		/// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		protected QueryEntities<T> GetJoinEntities<T>(string cmdName, AbstractEntity entity, int pageSize, int pageIndex)
			 where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.GetJoinEntities<T>(dataCommand, entity, pageSize, pageIndex);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		/// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		protected QueryEntities<T> GetJoinEntities<T>(string cmdName, int pageSize, int pageIndex)
			 where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.GetJoinEntities<T>(dataCommand, pageSize, pageIndex);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		protected QueryEntities<T> GetJoinEntities<T>(string cmdName, AbstractCondition condition)
			 where T : AbstractEntity, new()
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.GetJoinEntities<T>(dataCommand, condition);
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="anonObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		protected QueryEntities<T> GetJoinEntities<T>(string cmdName, object anonObject) where T : AbstractEntity, new()
		{
			if (anonObject == null) { throw new ArgumentNullException("anonObject"); }
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return GetJoinEntities<T>(dataCommand, anonObject);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		/// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryEntities<T> GetJoinEntities<T>(DynamicCommand dataCommand, int pageSize, int pageIndex)
			where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(null, pageSize, pageIndex);
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="dynamicObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		internal protected QueryEntities<T> GetJoinEntities<T>(DynamicCommand dataCommand, object dynamicObject) where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(dynamicObject);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryEntities<T> GetJoinEntities<T>(DynamicCommand dataCommand, AbstractCondition condition)
			where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(condition);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���ࡣ</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="entity">ʵ����ʵ�������а�������������Ĳ�����</param>
		/// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		/// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		internal protected QueryEntities<T> GetJoinEntities<T>(DynamicCommand dataCommand, AbstractEntity entity, int pageSize, int pageIndex)
			where T : AbstractEntity, new()
		{
			BeginDynamicExecute(dataCommand);
			return dataCommand.GetEntities<T>(entity, pageSize, pageIndex);
		}
		#endregion

		#region ִ�����ݿⷽ��(GetPagination<Entity>)
		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		protected IPagination<T> GetPagination<T>(string cmdName) where T : AbstractEntity, new()
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return GetPagination<T>(dataCommand);
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="dynamicObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		protected IPagination<T> GetPagination<T>(string cmdName, object dynamicObject) where T : AbstractEntity, new()
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return GetPagination<T>(dataCommand, dynamicObject);
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="condition">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		protected IPagination<T> GetPagination<T>(string cmdName, AbstractCondition condition) where T : AbstractEntity, new()
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return GetPagination<T>(dataCommand, condition);
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		internal protected IPagination<T> GetPagination<T>(StaticCommand dataCommand) where T : AbstractEntity, new()
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				return dataCommand.GetPagination<T>(0, 0);
			}
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="dynamicObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		internal protected IPagination<T> GetPagination<T>(StaticCommand dataCommand, object dynamicObject) where T : AbstractEntity, new()
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				if (dynamicObject != null) { dataCommand.ResetParameters(dynamicObject); }
				return dataCommand.GetPagination<T>(dynamicObject);
			}
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="condition">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		internal protected IPagination<T> GetPagination<T>(StaticCommand dataCommand, AbstractCondition condition) where T : AbstractEntity, new()
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				if (condition != null) { dataCommand.ResetParameters(condition); }
				return dataCommand.GetPagination<T>(condition);
			}
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="pagination">��Ҫ���������б�</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		protected IPagination<T> GetPagination<T>(string cmdName, Pagination<T> pagination) where T : AbstractEntity, new()
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return GetPagination<T>(dataCommand, pagination);
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="pagination">��Ҫ���������б�</param>
		/// <param name="dynamicObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		protected IPagination<T> GetPagination<T>(string cmdName, Pagination<T> pagination, object dynamicObject) where T : AbstractEntity, new()
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return GetPagination<T>(dataCommand, pagination, dynamicObject);
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="pagination">��Ҫ���������б�</param>
		/// <param name="condition">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		protected IPagination<T> GetPagination<T>(string cmdName, Pagination<T> pagination, AbstractCondition condition) where T : AbstractEntity, new()
		{
			StaticCommand dataCommand = CreateStaticCommand(cmdName);
			return GetPagination<T>(dataCommand, pagination, condition);
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="pagination">��Ҫ���������б�</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		internal protected IPagination<T> GetPagination<T>(StaticCommand dataCommand, Pagination<T> pagination) where T : AbstractEntity, new()
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				return dataCommand.GetPagination<T>(pagination, 0, 0);
			}
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="pagination">��Ҫ���������б�</param>
		/// <param name="dynamicObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		internal protected IPagination<T> GetPagination<T>(StaticCommand dataCommand, Pagination<T> pagination, object dynamicObject) where T : AbstractEntity, new()
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				if (dynamicObject != null) { dataCommand.ResetParameters(dynamicObject); }
				return dataCommand.GetPagination<T>(pagination, dynamicObject);
			}
		}

		/// <summary>
		/// ִ��Transact-SQL ����洢���̣���ȡ�ɷ�ҳ��ʵ���б�
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Entity.AbstractEntity��ʵ���࣬��ҪĬ�Ϲ��캯����</typeparam>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="pagination">��Ҫ���������б�</param>
		/// <param name="condition">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>����ִ��Transact-SQL ����洢���̺�ִ�н���Ŀɷ�ҳʵ���б�</returns>
		internal protected IPagination<T> GetPagination<T>(StaticCommand dataCommand, Pagination<T> pagination, AbstractCondition condition) where T : AbstractEntity, new()
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				if (condition != null) { dataCommand.ResetParameters(condition); }
				return dataCommand.GetPagination<T>(pagination, condition);
			}
		}
		#endregion

		#region ִ�����ݿⷽ��(Fill-System.Data.DataTable)

		/// <summary>������ݼ�</summary>
		/// <param name="dataTable">���������ݼ���һ�� System.Data.DataTable ��ʵ����</param>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		protected int Fill(DataTable dataTable, string cmdName)
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.Fill(dataTable, dataCommand);
		}

		/// <summary>������ݼ�</summary>
		/// <param name="table">���������ݼ���һ�� System.Data.DataTable ��ʵ����</param>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="anonObject">�����࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		protected int Fill(System.Data.DataTable table, string cmdName, object anonObject)
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.Fill(table, dataCommand, anonObject);
		}

		/// <summary>������ݼ�</summary>
		/// <param name="table">���������ݼ���һ�� System.Data.DataTable ��ʵ����</param>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="condition">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		protected int Fill(System.Data.DataTable table, string cmdName, AbstractCondition condition)
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.Fill(table, dataCommand, condition);
		}

		/// <summary>������ݼ�</summary>
		/// <param name="table">���������ݼ���һ�� System.Data.DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <returns>ִ��Transact-SQL������������ System.Data.DataTable �гɹ���ӻ�ˢ�µ�������</returns>
		internal protected int Fill(System.Data.DataTable table, StaticCommand dataCommand)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				return dataCommand.Fill(table);
			}
		}


		/// <summary>������ݼ�</summary>
		/// <param name="table">���������ݼ���һ�� System.Data.DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="anonObject">�����࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��Transact-SQL������������ System.Data.DataTable �гɹ���ӻ�ˢ�µ�������</returns>
		internal protected int Fill(System.Data.DataTable table, StaticCommand dataCommand, object anonObject)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				if (anonObject != null) { dataCommand.ResetParameters(anonObject); }
				return dataCommand.Fill(table);
			}
		}

		/// <summary>������ݼ�</summary>
		/// <param name="table">���������ݼ���һ�� System.Data.DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="condition">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��Transact-SQL������������ System.Data.DataTable �гɹ���ӻ�ˢ�µ�������</returns>
		internal protected int Fill(System.Data.DataTable table, StaticCommand dataCommand, AbstractCondition condition)
		{
			using (dataCommand = BeginStaticExecute(dataCommand))
			{
				if (condition != null) { dataCommand.ResetParameters(condition); }
				return dataCommand.Fill(table);
			}
		}

		/// <summary>������ݼ�</summary>
		/// <param name="table">���������ݼ���һ�� System.Data.DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <returns>ִ��Transact-SQL������������ System.Data.DataTable �гɹ���ӻ�ˢ�µ�������</returns>
		internal protected int Fill(System.Data.DataTable table, DynamicCommand dataCommand)
		{
			using (dataCommand = BeginDynamicExecute(dataCommand))
			{
				dataCommand.InitializeParameters();
				dataCommand.InitializeCommandText(0, 0);
				return dataCommand.Fill(table);
			}
		}

		/// <summary>������ݼ�</summary>
		/// <param name="table">���������ݼ���һ�� System.Data.DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="parameters">��̬�������飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��Transact-SQL������������ System.Data.DataTable �гɹ���ӻ�ˢ�µ�������</returns>
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

		/// <summary>������ݼ�</summary>
		/// <param name="table">���������ݼ���һ�� System.Data.DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="anonObject">�����࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��Transact-SQL������������ System.Data.DataTable �гɹ���ӻ�ˢ�µ�������</returns>
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

		/// <summary>������ݼ�</summary>
		/// <param name="table">���������ݼ���һ�� System.Data.DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="condition">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��Transact-SQL������������ System.Data.DataTable �гɹ���ӻ�ˢ�µ�������</returns>
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

		#region ִ�����ݿⷽ��(Fill-System.Data.DataSet)
		/// <summary>
		/// ������ݼ�
		/// </summary>
		/// <param name="dataSet">���������ݼ���һ�� System.Data.DataSet ��ʵ����</param>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="anonObject">�����࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		protected int Fill(System.Data.DataSet dataSet, string cmdName, object anonObject)
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.Fill(dataSet, dataCommand, anonObject);
		}

		/// <summary>
		/// ������ݼ�
		/// </summary>
		/// <param name="dataSet">���������ݼ���һ�� System.Data.DataSet ��ʵ����</param>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		protected int Fill(System.Data.DataSet dataSet, string cmdName)
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.Fill(dataSet, dataCommand);
		}

		/// <summary>
		/// ������ݼ�
		/// </summary>
		/// <param name="dataSet">���������ݼ���һ�� System.Data.DataSet ��ʵ����</param>
		/// <param name="cmdName">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ���ơ�</param>
		/// <param name="condition">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
		protected int Fill(System.Data.DataSet dataSet, string cmdName, AbstractCondition condition)
		{
			DynamicCommand dataCommand = CreateDynamicCommand(cmdName);
			return this.Fill(dataSet, dataCommand, condition);
		}

		/// <summary>������ݼ�</summary>
		/// <param name="dataSet">���������ݼ���һ�� System.Data.DataTable ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <returns>ִ��Transact-SQL������������ System.Data.DataTable �гɹ���ӻ�ˢ�µ�������</returns>
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
		/// ������ݼ�
		/// </summary>
		/// <param name="dataSet">���������ݼ���һ�� System.Data.DataSet ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="parameters">��̬�������飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
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
		/// ������ݼ�
		/// </summary>
		/// <param name="dataSet">���������ݼ���һ�� System.Data.DataSet ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="anonObject">�����࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
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
		/// ������ݼ�
		/// </summary>
		/// <param name="dataSet">���������ݼ���һ�� System.Data.DataSet ��ʵ����</param>
		/// <param name="dataCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="condition">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ִ�гɹ��򷵻��㣬���򷵻ش�����롣</returns>
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

		#region ��ȡ���ݿ�����
		/// <summary>���� ���ݿ������������</summary>
		/// <returns>���� BatchCommand ���Ͷ���</returns>
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
		/// ����ִ�����ݿ�Ľṹ
		/// </summary>
		/// <param name="CommandName">��������</param>
		/// <returns>����DataCommand�ṹ��Ϣ</returns>
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
		/// ����ִ�����ݿ�Ľṹ
		/// </summary>
		/// <returns>����StaticCommand�ṹ��Ϣ</returns>
		protected StaticCommand CreateStaticCommand()
		{
			return _ConnectionFactory.CreateStaticCommand();
		}

		/// <summary>
		/// ����ִ�����ݿ�Ľṹ
		/// </summary>
		/// <param name="CommandName">��������</param>
		/// <returns>���� StaticCommand �ṹ��Ϣ</returns>
		internal StaticCommand CreateStaticCommandOrNull(string CommandName)
		{
			return CreateDataCommand(CommandName) as StaticCommand;
		}

		/// <summary>
		/// ����ִ�����ݿ�Ľṹ
		/// </summary>
		/// <param name="CommandName">��������</param>
		/// <returns>����SqlStruct�ṹ��Ϣ</returns>
		internal protected JoinCommand CreateJoinCommand(string CommandName)
		{
			DynamicCommand dynamicCommand = CreateDynamicCommand(CommandName);
			return new JoinCommand(dynamicCommand);
		}

		/// <summary>
		/// ����ִ�����ݿ�Ľṹ
		/// </summary>
		/// <param name="CommandName">��������</param>
		/// <returns>����StaticCommand�ṹ��Ϣ</returns>
		protected internal StaticCommand CreateStaticCommand(string CommandName)
		{
			return CreateDataCommand(CommandName) as StaticCommand;
		}

		/// <summary>
		/// ����ִ�����ݿ�Ľṹ
		/// </summary>
		/// <param name="CommandName">��������</param>
		/// <returns>����SqlStruct�ṹ��Ϣ</returns>
		protected internal DynamicCommand CreateDynamicCommand(string CommandName)
		{
			return CreateDataCommand(CommandName) as DynamicCommand;
		}

		/// <summary>
		/// �����Զ��嶯̬����
		/// </summary>
		/// <param name="selectText">Ҫ������Դִ�е� Transact-SQL ����� SELECT ���ݿ��ֶβ��֡�</param>
		/// <param name="fromText">Ҫ������Դִ�е� Transact-SQL ����� FROM ���ݿ���֡�</param>
		/// <param name="whereText">Ҫ������Դִ�е� Transact-SQL ����� WHERE �������֡�</param>
		/// <param name="orderText">Ҫ������Դִ�е� Transact-SQL ����� ORDER BY �������֡�</param>
		/// <returns>���ش����ɹ��Ķ�̬���� DynamicCommand ����ʵ��(�ض���ĳ�����ݿ������ʵ��)��</returns>
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
		/// �����Զ��嶯̬����
		/// </summary>
		/// <param name="selectText">Ҫ������Դִ�е� Transact-SQL ����� SELECT ���ݿ��ֶβ��֡�</param>
		/// <param name="fromText">Ҫ������Դִ�е� Transact-SQL ����� FROM ���ݿ���֡�</param>
		/// <param name="whereText">Ҫ������Դִ�е� Transact-SQL ����� WHERE �������֡�</param>
		/// <param name="orderText">Ҫ������Դִ�е� Transact-SQL ����� ORDER BY �������֡�</param>
		/// <param name="groupText">Ҫ������Դִ�е� Transact-SQL ����� GROUP ����</param>
		/// <param name="havingText">Ҫ������Դִ�е� Transact-SQL ����� HANVING ��������</param>
		/// <returns>���ش����ɹ��Ķ�̬���� DynamicCommand ����ʵ��(�ض���ĳ�����ݿ������ʵ��)��</returns>
		internal protected DynamicCommand CreateDynamicCommand(string selectText, string fromText, string whereText, string orderText, string groupText, string havingText)
		{
			DynamicCommand dynamicCommand = CreateDynamicCommand(selectText, fromText, whereText, orderText);
			dynamicCommand.GroupText = groupText;
			dynamicCommand.HavingText = havingText;
			return dynamicCommand;
		}

		/// <summary>
		/// ��������ִ�в�������
		/// </summary>
		/// <param name="noSymbolName">�������ŵĲ�����</param>
		/// <returns>���ش����ɹ��Ĵ����ŵĲ�������</returns>
		internal protected string CreateParameterName(string noSymbolName)
		{
			return _ConnectionFactory.CreateParameterName(noSymbolName);
		}
		#endregion

		#region ��ȡ���ݻ���
		/// <summary>
		/// ��ȡ��ǰ���������Ĺ�����Ļ������
		/// </summary>
		/// <returns>�������ϵͳ�򷵻ػ��� ICacheClient �ӿڵ�ʵ�������򷵻�null��</returns>
		protected ICacheClient GetClient() { return CacheClientFactory.GetClient(_ConfigInfo.ConnectionName); }

		/// <summary>
		/// ��ȡ��ǰ���������Ĺ�����Ļ������
		/// </summary>
		/// <returns>�������ϵͳ�򷵻ػ��� ICacheClient �ӿڵ�ʵ�������򷵻�null��</returns>
		protected ICacheClient GetClient(string name) { return CacheClientFactory.GetClient(name); }

		#endregion

		#region �ֶ���������
		/// <summary>
		/// ���÷ֲ�ʽ����Ϊ���ύ״̬
		/// </summary>
		public Result SetComplete()
		{
			if (commitTransaction != null && CommitTranState == TransactionState.ExistTransaction)
				CommitTranState = TransactionState.AllowCommitTransaction;
			return _Result;
		}
		/// <summary>
		/// �����ֶ��ύ����
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
		/// �ع�����ֹ������
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
		/// �ع�����ֹ������
		/// </summary>
		/// <param name="e">�йط����ع���ԭ���˵����</param>
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

		#region IDisposable ��Ա
		/// <summary>
		/// �ͷ���Դ���ر����ݿ����ӣ��Զ��ύ(�ع�)����
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
		/// �ͷ���Դ
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
