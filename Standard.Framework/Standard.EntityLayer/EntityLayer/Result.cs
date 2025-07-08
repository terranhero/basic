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
	/// 执行Transact-SQL语句或存储过程后的返回结果。
	/// </summary>
	[Serializable]
	public class Result : MarshalByRefObject, ISerializable
	{
		/// <summary>
		/// 初始化Result实例
		/// </summary>
		public Result() : this(false, null, null, 0, 0) { }

		/// <summary>
		/// 初始化Result实例
		/// </summary>
		/// <param name="pAffectedRows">执行Transact-SQL语句后受影响行数。</param>
		protected Result(int pAffectedRows) : this(false, null, null, pAffectedRows, 0) { }

		/// <summary>
		/// 初始化Result实例
		/// </summary>
		/// <param name="pAffectedRows">执行Transact-SQL语句后受影响行数。</param>
		/// <param name="pResultCount">执行Transact-SQL语句后,结果集中记录数。</param>
		protected Result(int pAffectedRows, int pResultCount) : this(false, null, null, pAffectedRows, pResultCount) { }

		/// <summary>初始化Result实例</summary>
		/// <param name="status">表示状态</param>
		/// <param name="msg">执行Transact-SQL语句后异常代码。</param>
		public Result(bool status, string msg) : this(status, null, status ? "" : msg, 0, 0) { if (status == true) { _Message = msg; } }

		/// <summary>
		/// 初始化Result实例
		/// </summary>
		/// <param name="errorCode">执行Transact-SQL语句后异常代码。</param>
		public Result(string errorCode) : this(false, null, errorCode, 0, 0) { }

		/// <summary>
		/// 初始化Result实例
		/// </summary>
		/// <param name="errorCode">执行Transact-SQL语句后异常代码。</param>
		/// <param name="propertyName">当前错误信息相关的属性名称。</param>
		public Result(string propertyName, string errorCode) : this(false, propertyName, errorCode, 0, 0) { }

		/// <summary>
		/// 初始化Result实例
		/// </summary>
		/// <param name="errors">执行Transact-SQL语句后异常代码。</param>
		protected Result(ResultErrorCollection errors)
		{
			_Errors = new ResultErrorCollection(this);
			if (errors != null) { _Errors.AddErrors(errors); }
			TotalCount = 0;
			AffectedRows = 0;
		}

		/// <summary>初始化Result实例</summary>
		/// <param name="status">表示状态</param>
		/// <param name="errorCode">执行Transact-SQL语句后异常代码。</param>
		/// <param name="propertyName">当前错误信息相关的属性名称。</param>
		/// <param name="pAffectedRows">执行Transact-SQL语句后受影响行数。</param>
		/// <param name="pResultCount">执行Transact-SQL语句后,结果集中记录数。</param>
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
		/// 获取表示错误信息的集合。
		/// </summary>
		public ResultErrorCollection Errors { get { return _Errors; } }

		/// <summary>将错误集合属性重命名，重命名完成后保留集合</summary>
		/// <param name="source">原属性名称</param>
		/// <param name="target">修改后属性名称</param>
		/// <returns>返回修改后的错误集合，表示 ResultError 类型的集合</returns>
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

		/// <summary>获取所有错误信息，并合并成单个字符串</summary>
		/// <returns>由 \r\n 字符串分隔的 Errors 中的元素组成的字符串</returns>
		public string GetErrorText() { return string.Join("\r\n", _Errors.Select(m => m.Message)); }

		/// <summary>获取所有错误信息，并合并成单个字符串</summary>
		/// <param name="separator">要用作分隔符的字符串</param>
		/// <returns>由 separator 字符串分隔的 Errors 中的元素组成的字符串</returns>
		public string GetErrorText(string separator) { return string.Join(separator, _Errors.Select(m => m.Message)); }

		/// <summary>执行成功的消息</summary>
		public string Message { get { return _Message; } }

		/// <summary>
		/// 获取Result类空实例
		/// </summary>
		public static Result Empty
		{
			get { return new Result(); }
		}

		/// <summary>
		/// 获取Result类空实例
		/// </summary>
		public static Result Success
		{
			get { return new Result(); }
		}

		/// <summary>
		/// 清空结果集
		/// </summary>
		public void Clear()
		{
			_Errors.Clear();
			//ErrorMessage = null;
			//PropertyName = null;
			AffectedRows = 0;
			TotalCount = 0;
		}


		/// <summary>将指定 ResultError 集合的元素添加到 ResultErrorCollection 的末尾。</summary>
		/// <param name="errors">表示 ResultError 类型的集合。</param>
		public void AddErrors(IEnumerable<ResultError> errors) { _Errors.AddErrors(errors); }

		/// <summary>
		/// 根据索引号、属性名称、错误信息初始化 ResultError 类实例，并添加到集合末尾。
		/// </summary>
		/// <param name="index">表示模型数组的位置索引。如果不是数组则为 -1。</param>
		/// <param name="propertyName">表示当前属性名称。</param>
		/// <param name="errorMessage">表示错误信息。</param>
		public ResultError AddError(int index, string propertyName, string errorMessage)
		{
			return _Errors.AddError(index, propertyName, errorMessage);
		}

		/// <summary>
		/// 根据属性名称、错误信息初始化 ResultError 类实例，并添加到集合末尾。
		/// </summary>
		/// <param name="errorMessage">表示错误信息。</param>
		public ResultError AddError(string errorMessage)
		{
			return _Errors.AddError("", errorMessage);
		}

		/// <summary>
		/// 根据属性名称、错误信息初始化 ResultError 类实例，并添加到集合末尾。
		/// </summary>
		/// <param name="propertyName">表示当前属性名称。</param>
		/// <param name="errorMessage">表示错误信息。</param>
		public ResultError AddError(string propertyName, string errorMessage)
		{
			return _Errors.AddError(propertyName, errorMessage);
		}

		/// <summary>
		/// 执行Transact-SQL语句后受影响行数。
		/// </summary>
		public int AffectedRows { get; internal set; }

		///// <summary>
		///// 获取Transact-SQL 语句、表名或存储过程的错误编码,或者是存储过程的返回值
		///// 如果为零则执行成功，否则执行出错。
		///// </summary>
		//public string ErrorMessage { get; private set; }

		///// <summary>
		///// 获取Transact-SQL 语句、表名或存储过程的执行异常时关联的属性名称。
		///// </summary>
		//public string PropertyName { get; internal set; }

		/// <summary>
		/// 判断当前存储过程或Transact-SQL 语句执行是否成功。
		/// </summary>
		/// <value>如果不存在错误信息则返回 true，否则返回 false。</value>
		public bool Successful { get { if (_Status == true) { return true; } return _Errors.Count == 0; } }

		/// <summary>
		/// 判断当前存储过程或Transact-SQL 语句执行是否失败。
		/// 属性 Failure 正好与属性 Successful 结果相反。
		/// </summary>
		/// <value>如果存在错误信息则返回 true，否则返回 false。</value>
		public bool Failure { get { if (_Status == true) { return false; } return _Errors.Count > 0; } }

		/// <summary>
		/// 获取存储过程查询的记录数
		/// </summary>
		public int TotalCount { get; private set; }

		/// <summary>
		/// 设置存储过程查询的记录数
		/// </summary>
		/// <param name="count">记录数</param>
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public void SetResultCount(int count)
		{
			TotalCount = count;
		}

		/// <summary>
		/// 表示当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式</returns>
		public override string ToString() { return typeof(Result).Name; }

		#region ISerializable接口方法
		/// <summary>
		/// 用序列化数据初始化 Result 类的新实例。 
		/// </summary>
		/// <param name="info">它存有有关所引发异常的序列化的对象数据。</param>
		/// <param name="context">它包含有关源或目标的上下文信息。 </param>
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
		/// 使用将 Result 对象序列化所需的数据填充 SerializationInfo。
		/// </summary>
		/// <param name="info">要填充数据的 SerializationInfo。</param>
		/// <param name="context">此序列化的目标（请参见 StreamingContext）。</param>
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
		/// 使用将 GoldSoftException 对象序列化所需的数据填充 SerializationInfo。
		/// </summary>
		/// <param name="info">要填充数据的 SerializationInfo。</param>
		/// <param name="context">此序列化的目标（请参见 StreamingContext）。</param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			GetObjectData(info, context);
		}
		#endregion
	}

	/// <summary>
	/// 执行ExecuteReader方法后的返回结果。
	/// </summary>
	[Serializable]
	public sealed class ResultReader : Result
	{
		/// <summary>
		/// 数据库的结果集的只读对象。
		/// </summary>
		public IDataReader Reader { get; internal set; }
		/// <summary>
		/// 数据库命令类型。
		/// </summary>
		public CommandType CommandType { get; internal set; }
		/// <summary>
		/// 创建ResultReader实例
		/// </summary>
		/// <param name="reader">数据库的结果集的只读对象。</param>
		/// <param name="type">数据库命令类型</param>
		public ResultReader(DbDataReader reader, CommandType type) : base() { Reader = reader; CommandType = type; }
	}

	/// <summary>
	/// 执行ExecuteScalar方法后的返回结果。
	/// </summary>
	[Serializable]
	public sealed class ResultScalar : Result
	{
		/// <summary>
		/// 返回查询到的值，如果不存在则返回空引用。
		/// </summary>
		public object ScalarResult { get; internal set; }

		/// <summary>
		/// 创建ResultScalar实例
		/// </summary>
		/// <param name="scalarResult">返回查询到的值，如果不存在则返回空引用。</param>
		public ResultScalar(object scalarResult) : base() { ScalarResult = scalarResult; }
	}

	/// <summary>
	/// 执行Fill&lt;Tl&gt;方法后的返回结果。
	/// </summary>
	/// <typeparam name="T">实体模型类型</typeparam>
	[Serializable]
	public sealed class EntityResult<T> : Result where T : AbstractEntity
	{
		/// <summary>
		/// 返回查询到的值，如果不存在则返回空引用。
		/// </summary>
		public IPagination<T> Entities { get; internal set; }

		/// <summary>
		/// 初始化EntityResult实例
		/// </summary>
		public EntityResult(IPagination<T> entities) : this(entities, 0, 0) { }

		/// <summary>
		/// 初始化EntityResult实例
		/// </summary>
		/// <param name="entities">实体类列表</param>
		/// <param name="pAffectedRows">执行Transact-SQL语句后受影响行数。</param>
		public EntityResult(IPagination<T> entities, int pAffectedRows) : this(entities, pAffectedRows, 0) { }

		/// <summary>
		/// 初始化EntityResult实例
		/// </summary>
		/// <param name="entities">实体类列表</param>
		/// <param name="pAffectedRows">执行Transact-SQL语句后受影响行数。</param>
		/// <param name="pResultCount">执行Transact-SQL语句后,结果集中记录数。</param>
		public EntityResult(IPagination<T> entities, int pAffectedRows = 0, int pResultCount = 0)
			: base(pAffectedRows, pResultCount) { Entities = entities; }
	}
}
