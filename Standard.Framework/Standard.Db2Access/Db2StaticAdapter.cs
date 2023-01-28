
using Basic.DataAccess;
using Basic.Enums;
using IBM.Data.DB2.Core;

namespace Basic.DB2Access
{
	/// <summary>
	/// 表示用于填充 DataSet 和更新 SQL SERVER 数据库的一组数据命令和一个数据库连接。 
	/// </summary>
	[System.Serializable(), System.ComponentModel.ToolboxItem(false)]
	internal sealed class DB2StaticAdapter : StaticDataAdapter
	{
		/// <summary>
		/// 初始化 DB2StaticAdapter 类实例
		/// </summary>
		public DB2StaticAdapter() : base(new DB2DataAdapter()) { }

		/// <summary>
		/// 初始化 DB2StaticAdapter 类实例
		/// </summary>
		/// <param name="dataAdapter">用于创建新 DB2StaticAdapter 的 DB2DataAdapter 对象。 </param>
		internal DB2StaticAdapter(DB2DataAdapter dataAdapter) : base(dataAdapter) { }

		/// <summary>当前命令的数据库类型</summary>
		public override ConnectionType ConnectionType { get { return ConnectionType.Db2Connection; } }
	}
}
