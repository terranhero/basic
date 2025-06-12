using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Threading.Tasks;
using Basic.DataAccess;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.MySqlAccess;
using MySql.Data.MySqlClient;
using STT = System.Threading.Tasks;


namespace Basic.SqlServer
{
	/// <summary>
	/// 执行批处理的静态命令，一般执行INSERT 或 UPDATE 或 DELETE命令。
	/// </summary>
	public sealed class MySqlBatchCommand : BatchCommand
	{
		/// <summary>
		/// 初始化 MySqlBatchCommand 类的新实例。 
		/// </summary>
		/// <param name="cmd">表示 <see cref="StaticCommand"/> 实例。</param>
		public MySqlBatchCommand(StaticCommand cmd) : base(cmd) { }

		/// <summary>使用 XXXBulkCopy 类执行数据插入命令</summary>
		/// <typeparam name="TModel">表示 AbstractEntity 子类类型</typeparam>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <param name="entities">类型 BaseTableType&lt;BaseTableRowType&gt; 子类类实例，包含了需要执行参数的值。</param>
		/// <returns>执行Transact-SQL语句或存储过程后的返回结果。</returns>
		internal protected override async STT.Task<int> ExecuteAsync<TModel>(IEnumerable<TModel> entities, int timeout)
		{
			return await Task.FromResult(0);
		}
	}
}
