using System.ComponentModel;
using System.Data.Common;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示要对数据源执行的 SQL 语句或存储过程。为表示命令的、数据库特有的类提供一个基类。
	/// </summary>
	[System.ComponentModel.ToolboxItem(false), System.Serializable()]
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class DynamicJoinCommand : Component
	{
		/// <summary>
		/// 动态命令结构中静态参数列表
		/// </summary>
		private readonly DbParameter[] m_Parameters;
		/// <summary>
		/// 初始化 DynamicJoinCommand 类的新实例。 
		/// </summary>
		/// <param name="select"></param>
		/// <param name="from"></param>
		/// <param name="where"></param>
		/// <param name="order"></param>
		/// <param name="parameters"></param>
		internal DynamicJoinCommand(string select, string from, string where, string order, DbParameter[] parameters)
			: this(select, from, where, "", "", order, parameters) { }

		/// <summary>
		/// 初始化 DynamicJoinCommand 类的新实例。 
		/// </summary>
		/// <param name="select"></param>
		/// <param name="from"></param>
		/// <param name="where"></param>
		/// <param name="group"></param>
		/// <param name="having"></param>
		/// <param name="order"></param>
		/// <param name="parameters"></param>
		internal DynamicJoinCommand(string select, string from, string where, string group, string having, string order, DbParameter[] parameters)
		{
			_SelectText = select;
			_FromText = from;
			_WhereText = where;
			_GroupText = group;
			_HavingText = having;
			_OrderText = order;
			m_Parameters = parameters;
		}

		/// <summary>
		/// 需要执行的参数信息。
		/// </summary>
		public DbParameter[] Parameters { get { return m_Parameters; } }

		private string _SelectText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 SELECT 数据库字段部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 SELECT 部分，默认值为空字符串。</value>
		public string SelectText { get { return _SelectText; } }

		private string _FromText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 FROM 数据库表部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 FROM 部分，默认值为空字符串。</value>
		public string FromText { get { return _FromText; } }

		private string _WhereText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 WHERE 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 WHERE 部分，默认值为空字符串。</value>
		public string WhereText { get { return _WhereText; } }

		private string _GroupText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 GROUP 部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 GROUP 部分，默认值为空字符串。</value>
		public string GroupText { get { return _GroupText; } }

		private string _HavingText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 HANVING 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 HANVING 部分，默认值为空字符串。</value>
		public string HavingText { get { return _HavingText; } }

		private string _OrderText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 ORDER BY 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 ORDER BY 部分，默认值为空字符串。</value>
		public string OrderText { get { return _OrderText; } }
	}
}
