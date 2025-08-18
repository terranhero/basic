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
	/// ���ݲ�����
	/// </summary>
	public abstract class AbstractAccess : AbstractDbAccess
	{
		#region ���캯��
		/// <summary>
		/// �������ݴ�����ʵ��(Ĭ�Ϲر�����)
		/// </summary>
		protected AbstractAccess() : base() { }

		/// <summary>
		/// �������ݴ�����ʵ��
		/// </summary>
		/// <param name="access">���ݿ�����ࡣ</param>
		protected AbstractAccess(AbstractDbAccess access) : base(access) { }

		/// <summary>
		/// �������ݴ�����ʵ��
		/// </summary>
		/// <param name="startTransaction">�Ƿ���������</param>
		protected AbstractAccess(bool startTransaction) : base(startTransaction) { }

		/// <summary>
		/// �������ݴ�����ʵ��
		/// </summary>
		/// <param name="transaction">�����ڵǼǵ����� Transaction �����á�</param>
		protected AbstractAccess(CommittableTransaction transaction) : base(transaction) { }

		/// <summary>
		/// ʹ��ָ��������ѡ���ʼ�� AbstractAccess �����ʵ��������������
		/// </summary>
		/// <param name="timeout">һ��TimeSpan ���͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĳ�ʱʱ�����ơ�</param>
		protected AbstractAccess(TimeSpan timeout) : base(timeout) { }

		/// <summary>
		/// ʹ��ָ��������ѡ���ʼ�� AbstractAccess �����ʵ��������������
		/// </summary>
		/// <param name="isolationLevel">һ�� System.Transactions.IsolationLevel ö�����͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĸ��뼶��</param>
		/// <param name="timeout">һ��TimeSpan ���͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĳ�ʱʱ�����ơ�</param>
		protected AbstractAccess(ST.IsolationLevel isolationLevel, TimeSpan timeout) : base(isolationLevel, timeout) { }

		/// <summary>
		/// ʹ�����ݿ����Ӻ��������ͣ���ʼ�� AbstractDbAccess ��ʵ����
		/// </summary>
		/// <param name="connectionName">���ݿ���������</param>
		protected AbstractAccess(string connectionName) : base(connectionName) { }

		/// <summary>
		/// ʹ�����ݿ����Ӻ��������ͣ���ʼ�� AbstractDbAccess ��ʵ����
		/// </summary>
		/// <param name="connectionName">���ݿ���������</param>
		/// <param name="startTransaction">�Ƿ���������</param>
		protected AbstractAccess(string connectionName, bool startTransaction) : base(connectionName, startTransaction) { }

		/// <summary>
		/// ʹ�����ݿ����Ӻ��������ͣ���ʼ�� AbstractDbAccess ��ʵ����
		/// </summary>
		/// <param name="connectionName">���ݿ������ַ���</param>
		/// <param name="timeout">һ��TimeSpan ���͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĳ�ʱʱ�����ơ�</param>
		protected AbstractAccess(string connectionName, TimeSpan timeout) : base(connectionName, timeout) { }

		/// <summary>
		/// ʹ�����ݿ����Ӻ��������ͣ���ʼ�� AbstractDbAccess ��ʵ����
		/// </summary>
		/// <param name="connectionName">���ݿ������ַ���</param>
		/// <param name="isolationLevel">һ�� System.Transactions.IsolationLevel ö�����͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĸ��뼶��</param>
		/// <param name="timeout">һ��TimeSpan ���͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĳ�ʱʱ�����ơ�</param>
		protected AbstractAccess(string connectionName, ST.IsolationLevel isolationLevel, TimeSpan timeout) : base(connectionName, isolationLevel, timeout) { }

		/// <summary>
		/// ʹ�����ݿ����Ӻ��������ͣ���ʼ�� AbstractDbAccess ��ʵ����
		/// </summary>
		/// <param name="connectionName">���ݿ������ַ���</param>
		/// <param name="transaction">�����ڵǼǵ����� Transaction �����á�</param>
		protected AbstractAccess(string connectionName, CommittableTransaction transaction) : base(connectionName, transaction) { }

		/// <summary>
		/// ��ʼ�� AbstractAccess ���ʵ����
		/// </summary>
		/// <param name="user">��ǰ�û���Ϣ(���������������ݿ��������ơ�����Session��)</param>
		protected AbstractAccess(global::Basic.Interfaces.IUserContext user) :
				base(user)
		{
		}

		/// <summary>
		/// ��ʼ�� AbstractAccess ���ʵ����
		/// </summary>
		/// <param name="user">��ǰ�û���Ϣ(���������������ݿ��������ơ�����Session��)</param>
		/// <param name="startTransaction">�Ƿ���������</param>
		protected AbstractAccess(global::Basic.Interfaces.IUserContext user, bool startTransaction) :
				base(user, startTransaction)
		{
		}

		/// <summary>
		/// ��ʼ�� AbstractAccess ���ʵ����
		/// </summary>
		/// <param name="user">��ǰ�û���Ϣ(���������������ݿ��������ơ�����Session��)</param>
		/// <param name="timeout">һ��TimeSpan ���͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĳ�ʱʱ�����ơ�</param>
		protected AbstractAccess(global::Basic.Interfaces.IUserContext user, global::System.TimeSpan timeout) :
				base(user, timeout)
		{
		}

		/// <summary>
		/// ��ʼ�� AbstractAccess ���ʵ����
		/// </summary>
		/// <param name="user">��ǰ�û���Ϣ(���������������ݿ��������ơ�����Session��)</param>
		/// <param name="transaction">�����ڵǼǵ����� Transaction �����á�</param>
		protected AbstractAccess(global::Basic.Interfaces.IUserContext user, global::System.Transactions.CommittableTransaction transaction) :
				base(user, transaction)
		{
		}

		/// <summary>
		/// ��ʼ�� AbstractAccess ���ʵ����
		/// </summary>
		/// <param name="user">��ǰ�û���Ϣ(���������������ݿ��������ơ�����Session��)</param>
		/// <param name="isolationLevel">һ�� System.Transactions.IsolationLevel ö�����͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĸ��뼶��</param>
		/// <param name="timeout">һ��TimeSpan ���͵�ֵ����ֵ��ʾ���� CommittableTransaction �ĳ�ʱʱ�����ơ�</param>
		protected AbstractAccess(global::Basic.Interfaces.IUserContext user, global::System.Transactions.IsolationLevel isolationLevel, global::System.TimeSpan timeout) :
				base(user, isolationLevel, timeout)
		{
		}
		#endregion

		#region ��������
		/// <summary>
		/// ��������ʵ��
		/// </summary>
		/// <param name="table">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Result CreateCore<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			return base.ExecuteCore<TR>(CreateCommand, table);
		}

		/// <summary>
		/// ��������ʵ��
		/// </summary>
		/// <param name="row">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Result CreateCore<TR>(TR row) where TR : BaseTableRowType
		{
			return base.ExecuteCore<TR>(CreateCommand, row);
		}

		/// <summary>
		/// ʹ��ָ����������������ʵ��
		/// </summary>
		/// <param name="entity">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Result Create<T>(T entity) where T : AbstractEntity
		{
			return base.ExecuteNonQuery(CreateCommand, entity);
		}

		/// <summary>
		/// ʹ��ָ����������������ʵ��
		/// </summary>
		/// <param name="entities">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Result Create<T>(T[] entities) where T : AbstractEntity
		{
			return base.ExecuteNonQuery(CreateCommand, entities);
		}

		private StaticCommand _CreateCommand = null;
		/// <summary>
		/// ��������Transact-SQL�ṹ
		/// </summary>
		/// <returns>����Transact-SQL�ṹ</returns>
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
		/// ʹ��ָ����������������ʵ��
		/// </summary>
		/// <param name="row">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Task<Result> CreateCoreAsync(BaseTableRowType row)
		{
			return base.ExecuteCoreAsync(CreateCommand, row);
		}

		/// <summary>
		/// ʹ��ָ����������������ʵ��
		/// </summary>
		/// <param name="table">������Ҫ���������ݱ�ʵ������ʵ���� BaseTableType&lt;BaseTableRowType&gt; ������ࡣ</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Task<Result> CreateCoreAsync<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			return base.ExecuteCoreAsync<TR>(CreateCommand, table);
		}

		/// <summary>
		/// ʹ��ָ����������������ʵ��
		/// </summary>
		/// <param name="entities">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Task<Result> CreateAsync<T>(params T[] entities) where T : AbstractEntity
		{
			return base.ExecuteNonQueryAsync(CreateCommand, entities);
		}

#if NET6_0_OR_GREATER
		/// <summary>ʹ�� BatchCommand �������� INSERT ����ִ�в�����������</summary>
		/// <typeparam name="TModel">��ʾ <see cref="AbstractEntity"/> ����ʵ��</typeparam>
		/// <remarks>ʹ�ô�����ִ��ʱ������ִ��<see cref="StaticCommand"/>�� 
		/// <see  cref="StaticCommand.CheckCommands">CheckCommands</see> �� 
		/// <see cref="StaticCommand.NewValues">NewValues</see> �а���������
		/// ������ִ�д�����ǰ����Ҫ��������Ч����֤��ȡֵ������ǰִ����� </remarks>
		/// <param name="entities">ʵ�� <typeparamref name="TModel"/> �����飬��������Ҫִ�в�����ֵ��</param>
		/// <returns>ִ��Transact-SQL����洢���̺�ķ��ؽ����</returns>
		public async Task<Result> BatchCreateAsync<TModel>(IEnumerable<TModel> entities) where TModel : AbstractEntity
		{
			return await base.BatchAsync(CreateCommand, entities);
		}
#endif
		#endregion

		#region ��������
		private StaticCommand _UpdateCommand = null;
		/// <summary>
		/// ������������Transact-SQL�ṹ
		/// </summary>
		/// <returns>����Transact-SQL�ṹ</returns>
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
		/// ��������ʵ��
		/// </summary>
		/// <param name="entity">����ʵ��</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Result Update<T>(T entity) where T : AbstractEntity
		{
			return base.ExecuteNonQuery(UpdateCommand, entity);
		}

		/// <summary>
		/// ��������ʵ��
		/// </summary>
		/// <param name="entities">����ʵ��</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Result Update<T>(T[] entities) where T : AbstractEntity
		{
			return base.ExecuteNonQuery(UpdateCommand, entities);
		}

		/// <summary>ʹ��ָ��������ɾ������ʵ��</summary>
		/// <param name="anonObject">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Result Update(object anonObject)
		{
			return base.ExecuteNonQuery(UpdateCommand, anonObject);
		}

		/// <summary>
		/// ��������ʵ��
		/// </summary>
		/// <param name="row">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Result UpdateCore<TR>(TR row) where TR : BaseTableRowType
		{
			return base.ExecuteCore<TR>(UpdateCommand, row);
		}

		/// <summary>
		/// ��������ʵ��
		/// </summary>
		/// <param name="table">������Ҫ���������ݱ�ʵ������ʵ���� BaseTableType&lt;BaseTableRowType&gt; ������ࡣ</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Result UpdateCore<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			return base.ExecuteCore<TR>(UpdateCommand, table);
		}

		/// <summary>
		/// ʹ��ָ����������������ʵ��
		/// </summary>
		/// <param name="row">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Task<Result> UpdateAsync(BaseTableRowType row)
		{
			return base.ExecuteCoreAsync(UpdateCommand, row);
		}

		/// <summary>
		/// ʹ��ָ����������������ʵ��
		/// </summary>
		/// <param name="entities">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Task<Result> UpdateAsync<T>(params T[] entities) where T : AbstractEntity
		{
			return base.ExecuteNonQueryAsync(UpdateCommand, entities);
		}

		/// <summary>ʹ��ָ��������ɾ������ʵ��</summary>
		/// <param name="anonObject">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Task<Result> UpdateAsync(object anonObject)
		{
			return base.ExecuteNonQueryAsync(UpdateCommand, anonObject);
		}
		#endregion

		#region ɾ������
		/// <summary>
		/// ʹ��ָ����������������ʵ��
		/// </summary>
		/// <param name="row">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Task<Result> DeleteAsync(BaseTableRowType row)
		{
			return base.ExecuteCoreAsync(DeleteCommand, row);
		}

		///// <summary>
		///// ʹ��ָ����������������ʵ��
		///// </summary>
		///// <param name="entities">�������ݵļ�ֵ������</param>
		///// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		//public virtual Task<Result> DeleteAsync<T>(IEnumerable<T> entities) where T : AbstractEntity
		//{
		//	return base.ExecuteNonQueryAsync(DeleteCommand, entities);
		//}

		/// <summary>
		/// ʹ��ָ����������������ʵ��
		/// </summary>
		/// <param name="entities">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Task<Result> DeleteAsync<T>(params T[] entities) where T : AbstractEntity
		{
			return base.ExecuteNonQueryAsync(DeleteCommand, entities);
		}

		/// <summary>ʹ��ָ��������ɾ������ʵ��</summary>
		/// <param name="anonObject">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Task<Result> DeleteAsync(object anonObject)
		{
			return base.ExecuteNonQueryAsync(DeleteCommand, anonObject);
		}

		private StaticCommand _DeleteCommand = null;
		/// <summary>
		/// ����ɾ������Transact-SQL�ṹ
		/// </summary>
		/// <returns>����Transact-SQL�ṹ</returns>
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
		/// ���ݲ����������ʼ��SqlStructʵ��Ĭ��ֵ
		/// </summary>
		/// <param name="dataCommand">��ִ�е�Transact-SQL���ṹ</param>
		/// <param name="paramArray">��������</param>
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

		/// <summary>ִ�� DELETE ���ɾ������ʵ��</summary>
		/// <param name="anonObject">��������ʵ��</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		protected Result Delete(object anonObject)
		{
			return ExecuteNonQuery(DeleteCommand, anonObject);
		}

		/// <summary>ִ�� DELETE ���ɾ������ʵ��</summary>
		/// <param name="entity">����ʵ��</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Result Delete<T>(T entity) where T : AbstractEntity
		{
			return base.ExecuteNonQuery(DeleteCommand, entity);
		}

		/// <summary>ִ�� DELETE ���ɾ������ʵ��</summary>
		/// <param name="entities">����ʵ��</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Result Delete<T>(T[] entities) where T : AbstractEntity
		{
			return base.ExecuteNonQuery(DeleteCommand, entities);
		}

		/// <summary>ִ�� DELETE ���ɾ������ʵ��</summary>
		/// <param name="row">�������ݵļ�ֵ������</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Result DeleteCore<TR>(TR row) where TR : BaseTableRowType
		{
			return base.ExecuteCore<TR>(DeleteCommand, row);
		}

		/// <summary>ִ�� DELETE ���ɾ������ʵ��</summary>
		/// <param name="table">������Ҫ���������ݱ�ʵ������ʵ���� BaseTableType&lt;BaseTableRowType&gt; ������ࡣ</param>
		/// <returns>���ִ�гɹ�����0��ִ��ʧ���򷵻ش�����롣</returns>
		public virtual Result DeleteCore<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			return base.ExecuteCore<TR>(DeleteCommand, table);
		}
		#endregion

		#region ʹ�ùؼ��ֲ�ѯ���ݿ��¼
		/// <summary>
		/// ���ؼ��ֲ�ѯ��¼������ṹ
		/// </summary>
		private StaticCommand _SelectByKeyCommand = null;
		/// <summary>
		/// �������ؼ��ֲ�ѯ��¼������ṹ
		/// </summary>
		/// <returns>����Transact-SQL�ṹ</returns>
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
		/// ���ؼ��ֲ�ѯ��¼
		/// </summary>
		/// <param name="entity">��Ҫ����Basic.Entity.AbstractEntity��ʵ��</param>
		/// <returns>���ز�ѯ�������ݱ�</returns>
		public bool SearchByKey(AbstractEntity entity) { return this.SearchByKey(entity, (AbstractCondition)null); }

		/// <summary>
		/// ���ؼ��ֲ�ѯ��¼
		/// </summary>
		/// <param name="entity">��Ҫ����Basic.Entity.AbstractEntity��ʵ��</param>
		/// <param name="condition">��ѯ�����࣬������ǰ��ѯ��Ҫ�Ĳ�����</param>
		/// <returns>���ز�ѯ�������ݱ�</returns>
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
		/// ���ؼ��ֲ�ѯ��¼
		/// </summary>
		/// <param name="entity">��Ҫ����Basic.Entity.AbstractEntity��ʵ��</param>
		/// <param name="joinCmd">��Ҫ�����ӵ� JoinCommand ������ʵ����</param>
		/// <returns>���ز�ѯ�������ݱ�</returns>
		public bool SearchByKey(AbstractEntity entity, JoinCommand joinCmd) { return this.SearchByKey(entity, null, joinCmd); }

		/// <summary>
		/// ���ؼ��ֲ�ѯ��¼
		/// </summary>
		/// <param name="entity">��Ҫ����Basic.Entity.AbstractEntity��ʵ��</param>
		/// <param name="condition">��ѯ�����࣬������ǰ��ѯ��Ҫ�Ĳ�����</param>
		/// <param name="joinCmd">��Ҫ�����ӵ� JoinCommand ������ʵ����</param>
		/// <returns>���ز�ѯ�������ݱ�</returns>
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
		/// ���ؼ��ֲ�ѯ���������ļ�¼
		/// </summary>
		/// <typeparam name="TR">��ʾǿ���͵� Basic.Tables.BaseTableRowType ʵ����</typeparam>
		/// <param name="table">��Ҫ���� Basic.Tables.BaseTableType&lt;TR&gt; ��ʵ��</param>
		/// <param name="condition">�������ݵļ�ֵ������</param>
		/// <returns>����ǿ���͵� Basic.Tables.BaseTableRowType ʵ����</returns>
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
		/// ���ؼ��ֲ�ѯ���������ļ�¼
		/// </summary>
		/// <typeparam name="TR">��ʾǿ���͵� Basic.Tables.BaseTableRowType ʵ����</typeparam>
		/// <param name="table">��Ҫ���� Basic.Tables.BaseTableType&lt;TR&gt; ��ʵ��</param>
		/// <param name="joinCommand">��Ҫ�Ա������ӵ����</param>
		/// <param name="condition">�������ݵļ�ֵ������</param>
		/// <returns>����ǿ���͵� Basic.Tables.BaseTableRowType ʵ����</returns>
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
		///// ���ؼ��ֲ�ѯ��¼
		///// </summary>
		///// <param name="entity">��Ҫ����Basic.Entity.AbstractEntity��ʵ��</param>
		///// <returns>���ز�ѯ�������ݱ�</returns>
		//public bool FillByKey(AbstractEntity entity)
		//{
		//    return base.SearchEntity(SelectByKeyCommand, entity);
		//}

		///// <summary>
		///// ���ؼ��ֲ�ѯ���������ļ�¼
		///// </summary>
		///// <typeparam name="TR">��ʾǿ���͵� Basic.Tables.BaseTableRowType ʵ����</typeparam>
		///// <param name="table">��Ҫ���� Basic.Tables.BaseTableType&lt;TR&gt; ��ʵ��</param>
		///// <param name="objArray">�������ݵļ�ֵ������</param>
		///// <returns>����ǿ���͵� Basic.Tables.BaseTableRowType ʵ����</returns>
		//public TR FillByKey<TR>(BaseTableType<TR> table, params object[] objArray) where TR : BaseTableRowType
		//{
		//    StaticCommand dataCommand = SelectByKeyCommand;
		//    InitDataCommandDefaultValue(dataCommand, objArray);
		//    this.Fill(table, dataCommand, 0, 0);
		//    return table.Rows.Find(objArray) as TR;
		//}
		#endregion

		#region ��ѯ�������ݿ��¼
		/// <summary>
		/// ��ѯ�������ݿ��¼������ṹ
		/// </summary>
		private DynamicCommand _SelectAllCommand = null;
		/// <summary>
		/// ��ѯ�������ݿ��¼������ṹ
		/// </summary>
		/// <returns>����Transact-SQL�ṹ</returns>
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
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳��� Basic.Tables.BaseTableRowType ��ǿ���� DataRow ʵ����</typeparam>
		/// <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, AbstractCondition condition) where T : BaseTableRowType
		{
			return base.GetDataTable<T>(table, SelectAllCommand, condition);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳��� Basic.Tables.BaseTableRowType ��ǿ���� DataRow ʵ����</typeparam>
		/// <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="dynamicObject">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, object dynamicObject) where T : BaseTableRowType
		{
			return GetDataTable<T>(table, SelectAllCommand, dynamicObject);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳��� Basic.Tables.BaseTableRowType ��ǿ���� DataRow ʵ����</typeparam>
		/// <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table) where T : BaseTableRowType
		{
			return base.GetDataTable<T>(table, SelectAllCommand);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Tables.BaseTableRowType��ʵ���ࡣ</typeparam>
		/// <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="joinCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, JoinCommand joinCommand, AbstractCondition condition)
			where T : BaseTableRowType
		{
			return base.GetDataTable<T>(table, SelectAllCommand, joinCommand, condition);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Tables.BaseTableRowType��ʵ���ࡣ</typeparam>
		/// <param name="table">��Ҫ������ݵ� DataTable ��ʵ����</param>
		/// <param name="joinCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="dynamicObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		protected QueryDataTable<T> GetDataTable<T>(BaseTableType<T> table, JoinCommand joinCommand, object dynamicObject)
			where T : BaseTableRowType
		{
			return base.GetDataTable<T>(table, SelectAllCommand, joinCommand, dynamicObject);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳��� Basic.Entity.AbstractEntity ��ʵ����</typeparam>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryEntities<T> GetEntities<T>(AbstractCondition condition) where T : AbstractEntity, new()
		{
			return base.GetEntities<T>(SelectAllCommand, condition);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳��� Basic.Entity.AbstractEntity ��ʵ����</typeparam>
		/// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		/// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryEntities<T> GetEntities<T>(int pageSize, int pageIndex) where T : AbstractEntity, new()
		{
			return base.GetEntities<T>(SelectAllCommand, pageSize, pageIndex);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳��� Basic.Entity.AbstractEntity ��ʵ����</typeparam>
		/// <param name="entity">ʵ����ʵ�������а�������������Ĳ�����</param>
		/// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		/// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryEntities<T> GetEntities<T>(AbstractEntity entity, int pageSize, int pageIndex) where T : AbstractEntity, new()
		{
			return base.GetEntities<T>(SelectAllCommand, entity, pageSize, pageIndex);
		}
		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳��� Basic.Entity.AbstractEntity ��ʵ����</typeparam>
		/// <param name="dynamicObject">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryEntities<T> GetEntities<T>(object dynamicObject) where T : AbstractEntity, new()
		{
			return GetEntities<T>(SelectAllCommand, dynamicObject);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳��� Basic.Entity.AbstractEntity ��ʵ����</typeparam>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryEntities<T> GetEntities<T>() where T : AbstractEntity, new()
		{
			return base.GetEntities<T>(SelectAllCommand, new { });
		}
		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Tables.BaseTableRowType��ʵ���ࡣ</typeparam>
		/// <param name="joinCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryEntities<T> GetEntities<T>(JoinCommand joinCommand, AbstractCondition condition)
			where T : AbstractEntity, new()
		{
			return base.GetEntities<T>(SelectAllCommand, joinCommand, condition);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳���Basic.Tables.BaseTableRowType��ʵ���ࡣ</typeparam>
		/// <param name="joinCommand">��ʾҪ������Դִ�е� SQL ����洢���̽ṹ��</param>
		/// <param name="dynamicObject">����ʵ���࣬��������Ҫִ�в�����ֵ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryEntities<T> GetEntities<T>(JoinCommand joinCommand, object dynamicObject)
			where T : AbstractEntity, new()
		{
			return base.GetEntities<T>(SelectAllCommand, joinCommand, dynamicObject);
		}
		#endregion

		#region ��ѯ�������ݿ��¼��ͨ��Join����ͨ�ñ�
		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳��� Basic.Entity.AbstractEntity ��ʵ����</typeparam>
		/// <param name="condition">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryEntities<T> GetJoinEntities<T>(AbstractCondition condition) where T : AbstractEntity, new()
		{
			return base.GetJoinEntities<T>(SelectAllCommand, condition);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳��� Basic.Entity.AbstractEntity ��ʵ����</typeparam>
		/// <param name="pageSize">��Ҫ��ѯ�ĵ�ǰҳ��С</param>
		/// <param name="pageIndex">��Ҫ��ѯ�ĵ�ǰҳ����,������1��ʼ</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryEntities<T> GetJoinEntities<T>(int pageSize, int pageIndex) where T : AbstractEntity, new()
		{
			return base.GetJoinEntities<T>(SelectAllCommand, pageSize, pageIndex);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳��� Basic.Entity.AbstractEntity ��ʵ����</typeparam>
		/// <param name="dynamicObject">��ѯ���ݿ��¼��������������ҳ��Ϣ��</param>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryEntities<T> GetJoinEntities<T>(object dynamicObject) where T : AbstractEntity, new()
		{
			return GetJoinEntities<T>(SelectAllCommand, dynamicObject);
		}

		/// <summary>
		/// ��ȡ�ɲ�ѯ��ʵ���б���ʵ����
		/// </summary>
		/// <typeparam name="T">�̳��� Basic.Entity.AbstractEntity ��ʵ����</typeparam>
		/// <returns>���ؿɲ�ѯ��ʵ���б�</returns>
		public QueryEntities<T> GetJoinEntities<T>() where T : AbstractEntity, new()
		{
			return base.GetJoinEntities<T>(SelectAllCommand, new { });
		}
		#endregion
	}
}
