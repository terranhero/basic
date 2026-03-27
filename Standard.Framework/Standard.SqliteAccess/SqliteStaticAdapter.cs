using Basic.DataAccess;
using Basic.Enums;

namespace Basic.SqliteAccess
{
	/// <summary>
	/// 表示用于填充 DataSet 和更新 SQL SERVER 数据库的一组数据命令和一个数据库连接。 
	/// </summary>
	[System.Serializable(), System.ComponentModel.ToolboxItem(false)]
	internal sealed class SqliteStaticAdapter : StaticDataAdapter
	{
		/// <summary>
		/// 初始化 SqliteStaticAdapter 类实例
		/// </summary>
		private SqliteStaticAdapter() : base(null) { }

		/// <summary>当前命令的数据库类型</summary>
		public override ConnectionType ConnectionType { get { return ConnectionType.SqlConnection; } }
	}
}
