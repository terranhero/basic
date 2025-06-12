using Basic.EntityLayer;
using Basic.Enums;
using Basic.Tables;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Xml.Serialization;
using STT = System.Threading.Tasks;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示要对数据源执行的 SQL 语句或存储过程。为表示命令的、数据库特有的类提供一个基类。
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.ComponentModel.ToolboxItem(false)]
	public abstract class BulkCopyCommand : AbstractDataCommand, IDbCommand, IXmlSerializable
	{
		private readonly TableConfiguration _TableInfo;

		/// <summary>
		/// 初始化 BulkCopyCommand 类的新实例。 
		/// </summary>
		/// <param name="dbCommand"></param>
		///<param name="configInfo">表示当前数据库表配置信息</param>
		protected BulkCopyCommand(DbCommand dbCommand, TableConfiguration configInfo) : base(dbCommand) { _TableInfo = configInfo; }

		/// <summary>
		/// 创建新的 XXXBulkCopyColumnMapping 并将其添加到集合中，
		/// 使用列名指定源列和目标列。
		/// </summary>
		/// <param name="sourceColumn">数据源中源列的名称</param>
		/// <param name="destinationColumn">数据源中目标列的名称</param>
		/// <returns>受影响的行数。</returns>
		public abstract bool TryAddOrUpdateMapping(string sourceColumn, string destinationColumn);

		/// <summary>
		/// 针对 .NET Framework 数据提供程序的 Connection 对象执行 SQL 语句，并返回受影响的行数。
		/// </summary>
		/// <param name="table">实体类，包含了需要执行参数的值。</param>
		/// <returns>受影响的行数。</returns>
		internal protected virtual void BatchExecute<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			this.BatchExecute<TR>(table, 30);
		}

		/// <summary>
		/// 针对 .NET Framework 数据提供程序的 Connection 对象执行 SQL 语句，并返回受影响的行数。
		/// </summary>
		/// <typeparam name="TR">表示 BaseTableRowType 子类类型</typeparam>
		/// <param name="table">实体类，包含了需要执行参数的值。</param>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <returns>受影响的行数。</returns>
		internal protected abstract void BatchExecute<TR>(BaseTableType<TR> table, int timeout) where TR : BaseTableRowType;

		/// <summary>
		/// 针对 .NET Framework 数据提供程序的 Connection 对象执行 SQL 语句，并返回受影响的行数。
		/// </summary>
		/// <param name="table">实体类，包含了需要执行参数的值。</param>
		/// <returns>受影响的行数。</returns>
		internal protected virtual STT.Task BatchExecuteAsync<TR>(BaseTableType<TR> table) where TR : BaseTableRowType
		{
			return this.BatchExecuteAsync<TR>(table, 30);
		}

		/// <summary>
		/// 针对 .NET Framework 数据提供程序的 Connection 对象执行 SQL 语句，并返回受影响的行数。
		/// </summary>
		/// <typeparam name="TR">表示 BaseTableRowType 子类类型</typeparam>
		/// <param name="table">实体类，包含了需要执行参数的值。</param>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <returns>受影响的行数。</returns>
		internal protected abstract STT.Task BatchExecuteAsync<TR>(BaseTableType<TR> table, int timeout) where TR : BaseTableRowType;

#if NET8_0_OR_GREATER
		/// <summary>使用 XXXBulkCopy 类执行数据插入命令</summary>
		/// <param name="entities">类型 BaseTableType&lt;BaseTableRowType&gt; 子类类实例，包含了需要执行参数的值。</param>
		/// <returns>执行Transact-SQL语句或存储过程后的返回结果。</returns>
		internal protected abstract STT.Task<Result> BatchAsync<TModel>(params TModel[] entities) where TModel : AbstractEntity;
#endif
	}
}
