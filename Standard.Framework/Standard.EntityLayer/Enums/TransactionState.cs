using Basic.EntityLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic.Enums
{
	/// <summary>
	/// 事务的提交状态
	/// </summary>
	internal enum TransactionState
	{
		/// <summary>
		/// 指示当前不存在可被提交的事务。
		/// </summary>
		NotExistTransaction = 1,

		/// <summary>
		/// 指示当前存在可被提交的事务。
		/// </summary>
		ExistTransaction = 2,

		/// <summary>
		/// 指示当前存在可被回滚的事务，且事务允许被回滚。
		/// </summary>
		AllowRollbackTransaction = 3,

		/// <summary>
		/// 指示当前存在可被提交的事务，且事务已经被回滚，
		/// 如果在提交则系统抛出TransactionInDoubtException异常。
		/// </summary>
		AlreadyRollbackTransaction = 4,

		/// <summary>
		/// 指示当前存在可被提交的事务，且事务允许被提交。
		/// </summary>
		AllowCommitTransaction = 5,

		/// <summary>
		/// 指示当前存在可被提交的事务，且事务已经被提交，
		/// 如果在提交则系统抛出TransactionInDoubtException异常。
		/// </summary>
		AlreadyCommitTransaction = 6
	}
}
