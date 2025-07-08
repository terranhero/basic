using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using Basic.EntityLayer;
using Basic.Interfaces;

namespace Basic.EntityLayer
{
	/// <summary>
	/// ִ��Transact-SQL����洢���̺�ķ��ؽ����
	/// </summary>
	[Serializable]
	public class Result : MarshalByRefObject, ISerializable
	{
		/// <summary>
		/// ��ʼ��Resultʵ��
		/// </summary>
		public Result() : this(false, null, null, 0, 0) { }

		/// <summary>
		/// ��ʼ��Resultʵ��
		/// </summary>
		/// <param name="pAffectedRows">ִ��Transact-SQL������Ӱ��������</param>
		protected Result(int pAffectedRows) : this(false, null, null, pAffectedRows, 0) { }

		/// <summary>
		/// ��ʼ��Resultʵ��
		/// </summary>
		/// <param name="pAffectedRows">ִ��Transact-SQL������Ӱ��������</param>
		/// <param name="pResultCount">ִ��Transact-SQL����,������м�¼����</param>
		protected Result(int pAffectedRows, int pResultCount) : this(false, null, null, pAffectedRows, pResultCount) { }

		/// <summary>��ʼ��Resultʵ��</summary>
		/// <param name="status">��ʾ״̬</param>
		/// <param name="msg">ִ��Transact-SQL�����쳣���롣</param>
		public Result(bool status, string msg) : this(status, null, status ? "" : msg, 0, 0) { if (status == true) { _Message = msg; } }

		/// <summary>
		/// ��ʼ��Resultʵ��
		/// </summary>
		/// <param name="errorCode">ִ��Transact-SQL�����쳣���롣</param>
		public Result(string errorCode) : this(false, null, errorCode, 0, 0) { }

		/// <summary>
		/// ��ʼ��Resultʵ��
		/// </summary>
		/// <param name="errorCode">ִ��Transact-SQL�����쳣���롣</param>
		/// <param name="propertyName">��ǰ������Ϣ��ص��������ơ�</param>
		public Result(string propertyName, string errorCode) : this(false, propertyName, errorCode, 0, 0) { }

		/// <summary>
		/// ��ʼ��Resultʵ��
		/// </summary>
		/// <param name="errors">ִ��Transact-SQL�����쳣���롣</param>
		protected Result(ResultErrorCollection errors)
		{
			_Errors = new ResultErrorCollection(this);
			if (errors != null) { _Errors.AddErrors(errors); }
			TotalCount = 0;
			AffectedRows = 0;
		}

		/// <summary>��ʼ��Resultʵ��</summary>
		/// <param name="status">��ʾ״̬</param>
		/// <param name="errorCode">ִ��Transact-SQL�����쳣���롣</param>
		/// <param name="propertyName">��ǰ������Ϣ��ص��������ơ�</param>
		/// <param name="pAffectedRows">ִ��Transact-SQL������Ӱ��������</param>
		/// <param name="pResultCount">ִ��Transact-SQL����,������м�¼����</param>
		private Result(bool status, string propertyName = null, string errorCode = null, int pAffectedRows = 0, int pResultCount = 0)
		{
			_Status = status; _Errors = new ResultErrorCollection(this);
			if (errorCode != null) { _Errors.AddError(propertyName, errorCode); }
			TotalCount = pResultCount;
			AffectedRows = pAffectedRows;
		}
		private readonly bool _Status = false;
		private readonly string _Message = "";

		private readonly ResultErrorCollection _Errors = null;

		/// <summary>
		/// ��ȡ��ʾ������Ϣ�ļ��ϡ�
		/// </summary>
		public ResultErrorCollection Errors { get { return _Errors; } }

		/// <summary>�����󼯺���������������������ɺ�������</summary>
		/// <param name="source">ԭ��������</param>
		/// <param name="target">�޸ĺ���������</param>
		/// <returns>�����޸ĺ�Ĵ��󼯺ϣ���ʾ ResultError ���͵ļ���</returns>
		public ResultErrorCollection Rename(string source, string target)
		{
			IEnumerable<ResultError> errors = _Errors.Where(m => m.Name == source);
			if (errors == null) { return _Errors; }
			foreach (ResultError error in errors)
			{
				error.Rename(target);
			}
			return _Errors;
		}

		/// <summary>��ȡ���д�����Ϣ�����ϲ��ɵ����ַ���</summary>
		/// <returns>�� \r\n �ַ����ָ��� Errors �е�Ԫ����ɵ��ַ���</returns>
		public string GetErrorText() { return string.Join("\r\n", _Errors.Select(m => m.Message)); }

		/// <summary>��ȡ���д�����Ϣ�����ϲ��ɵ����ַ���</summary>
		/// <param name="separator">Ҫ�����ָ������ַ���</param>
		/// <returns>�� separator �ַ����ָ��� Errors �е�Ԫ����ɵ��ַ���</returns>
		public string GetErrorText(string separator) { return string.Join(separator, _Errors.Select(m => m.Message)); }

		/// <summary>ִ�гɹ�����Ϣ</summary>
		public string Message { get { return _Message; } }

		/// <summary>
		/// ��ȡResult���ʵ��
		/// </summary>
		public static Result Empty
		{
			get { return new Result(); }
		}

		/// <summary>
		/// ��ȡResult���ʵ��
		/// </summary>
		public static Result Success
		{
			get { return new Result(); }
		}

		/// <summary>
		/// ��ս����
		/// </summary>
		public void Clear()
		{
			_Errors.Clear();
			//ErrorMessage = null;
			//PropertyName = null;
			AffectedRows = 0;
			TotalCount = 0;
		}


		/// <summary>��ָ�� ResultError ���ϵ�Ԫ����ӵ� ResultErrorCollection ��ĩβ��</summary>
		/// <param name="errors">��ʾ ResultError ���͵ļ��ϡ�</param>
		public void AddErrors(IEnumerable<ResultError> errors) { _Errors.AddErrors(errors); }

		/// <summary>
		/// ���������š��������ơ�������Ϣ��ʼ�� ResultError ��ʵ��������ӵ�����ĩβ��
		/// </summary>
		/// <param name="index">��ʾģ�������λ���������������������Ϊ -1��</param>
		/// <param name="propertyName">��ʾ��ǰ�������ơ�</param>
		/// <param name="errorMessage">��ʾ������Ϣ��</param>
		public ResultError AddError(int index, string propertyName, string errorMessage)
		{
			return _Errors.AddError(index, propertyName, errorMessage);
		}

		/// <summary>
		/// �����������ơ�������Ϣ��ʼ�� ResultError ��ʵ��������ӵ�����ĩβ��
		/// </summary>
		/// <param name="errorMessage">��ʾ������Ϣ��</param>
		public ResultError AddError(string errorMessage)
		{
			return _Errors.AddError("", errorMessage);
		}

		/// <summary>
		/// �����������ơ�������Ϣ��ʼ�� ResultError ��ʵ��������ӵ�����ĩβ��
		/// </summary>
		/// <param name="propertyName">��ʾ��ǰ�������ơ�</param>
		/// <param name="errorMessage">��ʾ������Ϣ��</param>
		public ResultError AddError(string propertyName, string errorMessage)
		{
			return _Errors.AddError(propertyName, errorMessage);
		}

		/// <summary>
		/// ִ��Transact-SQL������Ӱ��������
		/// </summary>
		public int AffectedRows { get; internal set; }

		///// <summary>
		///// ��ȡTransact-SQL ��䡢������洢���̵Ĵ������,�����Ǵ洢���̵ķ���ֵ
		///// ���Ϊ����ִ�гɹ�������ִ�г���
		///// </summary>
		//public string ErrorMessage { get; private set; }

		///// <summary>
		///// ��ȡTransact-SQL ��䡢������洢���̵�ִ���쳣ʱ�������������ơ�
		///// </summary>
		//public string PropertyName { get; internal set; }

		/// <summary>
		/// �жϵ�ǰ�洢���̻�Transact-SQL ���ִ���Ƿ�ɹ���
		/// </summary>
		/// <value>��������ڴ�����Ϣ�򷵻� true�����򷵻� false��</value>
		public bool Successful { get { if (_Status == true) { return true; } return _Errors.Count == 0; } }

		/// <summary>
		/// �жϵ�ǰ�洢���̻�Transact-SQL ���ִ���Ƿ�ʧ�ܡ�
		/// ���� Failure ���������� Successful ����෴��
		/// </summary>
		/// <value>������ڴ�����Ϣ�򷵻� true�����򷵻� false��</value>
		public bool Failure { get { if (_Status == true) { return false; } return _Errors.Count > 0; } }

		/// <summary>
		/// ��ȡ�洢���̲�ѯ�ļ�¼��
		/// </summary>
		public int TotalCount { get; private set; }

		/// <summary>
		/// ���ô洢���̲�ѯ�ļ�¼��
		/// </summary>
		/// <param name="count">��¼��</param>
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public void SetResultCount(int count)
		{
			TotalCount = count;
		}

		/// <summary>
		/// ��ʾ��ǰ������ַ�����ʾ��ʽ��
		/// </summary>
		/// <returns>��ǰ������ַ�����ʾ��ʽ</returns>
		public override string ToString() { return typeof(Result).Name; }

		#region ISerializable�ӿڷ���
		/// <summary>
		/// �����л����ݳ�ʼ�� Result �����ʵ���� 
		/// </summary>
		/// <param name="info">�������й��������쳣�����л��Ķ������ݡ�</param>
		/// <param name="context">�������й�Դ��Ŀ�����������Ϣ�� </param>
		protected Result(SerializationInfo info, StreamingContext context)
		{
			_Errors = new ResultErrorCollection(this);
			TotalCount = info.GetInt32("ResultCount");
			AffectedRows = info.GetInt32("AffectedRows");
			_Status = info.GetBoolean("Status");
			_Message = info.GetString("Message");
			int count = info.GetInt32("Errors_Count");
			for (int index = 0; index < count; index++)
			{
				int errorIndex = info.GetInt32(string.Concat("ResultError_", index, "_Index"));
				string errorName = info.GetString(string.Concat("ResultError_", index, "_Name"));
				string errorMessage = info.GetString(string.Concat("ResultError_", index, "_Message"));
				_Errors.AddError(errorIndex, errorName, errorMessage);
			}
		}

		/// <summary>
		/// ʹ�ý� Result �������л������������� SerializationInfo��
		/// </summary>
		/// <param name="info">Ҫ������ݵ� SerializationInfo��</param>
		/// <param name="context">�����л���Ŀ�꣨��μ� StreamingContext����</param>
		protected void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//info.AddValue("ErrorMessage", ErrorMessage);
			info.AddValue("ResultCount", TotalCount);
			info.AddValue("AffectedRows", AffectedRows);
			info.AddValue("Status", _Status);
			info.AddValue("Message", _Message);
			info.AddValue("Errors_Count", _Errors.Count);
			for (int index = 0; index < _Errors.Count; index++)
			{
				ResultError error = _Errors[index];
				info.AddValue(string.Concat("ResultError_", index, "_Index"), error.Index);
				info.AddValue(string.Concat("ResultError_", index, "_Name"), error.Name);
				info.AddValue(string.Concat("ResultError_", index, "_Message"), error.Message);
			}
		}
		/// <summary>
		/// ʹ�ý� GoldSoftException �������л������������� SerializationInfo��
		/// </summary>
		/// <param name="info">Ҫ������ݵ� SerializationInfo��</param>
		/// <param name="context">�����л���Ŀ�꣨��μ� StreamingContext����</param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			GetObjectData(info, context);
		}
		#endregion
	}

	/// <summary>
	/// ִ��ExecuteReader������ķ��ؽ����
	/// </summary>
	[Serializable]
	public sealed class ResultReader : Result
	{
		/// <summary>
		/// ���ݿ�Ľ������ֻ������
		/// </summary>
		public IDataReader Reader { get; internal set; }
		/// <summary>
		/// ���ݿ��������͡�
		/// </summary>
		public CommandType CommandType { get; internal set; }
		/// <summary>
		/// ����ResultReaderʵ��
		/// </summary>
		/// <param name="reader">���ݿ�Ľ������ֻ������</param>
		/// <param name="type">���ݿ���������</param>
		public ResultReader(DbDataReader reader, CommandType type) : base() { Reader = reader; CommandType = type; }
	}

	/// <summary>
	/// ִ��ExecuteScalar������ķ��ؽ����
	/// </summary>
	[Serializable]
	public sealed class ResultScalar : Result
	{
		/// <summary>
		/// ���ز�ѯ����ֵ������������򷵻ؿ����á�
		/// </summary>
		public object ScalarResult { get; internal set; }

		/// <summary>
		/// ����ResultScalarʵ��
		/// </summary>
		/// <param name="scalarResult">���ز�ѯ����ֵ������������򷵻ؿ����á�</param>
		public ResultScalar(object scalarResult) : base() { ScalarResult = scalarResult; }
	}

	/// <summary>
	/// ִ��Fill&lt;Tl&gt;������ķ��ؽ����
	/// </summary>
	/// <typeparam name="T">ʵ��ģ������</typeparam>
	[Serializable]
	public sealed class EntityResult<T> : Result where T : AbstractEntity
	{
		/// <summary>
		/// ���ز�ѯ����ֵ������������򷵻ؿ����á�
		/// </summary>
		public IPagination<T> Entities { get; internal set; }

		/// <summary>
		/// ��ʼ��EntityResultʵ��
		/// </summary>
		public EntityResult(IPagination<T> entities) : this(entities, 0, 0) { }

		/// <summary>
		/// ��ʼ��EntityResultʵ��
		/// </summary>
		/// <param name="entities">ʵ�����б�</param>
		/// <param name="pAffectedRows">ִ��Transact-SQL������Ӱ��������</param>
		public EntityResult(IPagination<T> entities, int pAffectedRows) : this(entities, pAffectedRows, 0) { }

		/// <summary>
		/// ��ʼ��EntityResultʵ��
		/// </summary>
		/// <param name="entities">ʵ�����б�</param>
		/// <param name="pAffectedRows">ִ��Transact-SQL������Ӱ��������</param>
		/// <param name="pResultCount">ִ��Transact-SQL����,������м�¼����</param>
		public EntityResult(IPagination<T> entities, int pAffectedRows = 0, int pResultCount = 0)
			: base(pAffectedRows, pResultCount) { Entities = entities; }
	}
}
