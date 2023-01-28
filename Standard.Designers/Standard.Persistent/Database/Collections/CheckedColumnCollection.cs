using System.Collections.Specialized;
using Basic.Database;

namespace Basic.Collections
{
	/// <summary>
	/// 表示 ColumnDesignerInfo 类的集合。
	/// </summary>
	public sealed class CheckedColumnCollection : BaseCollection<ColumnDesignerInfo>, INotifyCollectionChanged
	{
		private readonly TableDesignerCollection tableDesignerInfos;
		/// <summary>
		/// 初始化 ColumnDesignerCollection 类的新实例。
		/// </summary>
		/// <param name="tables">需要通知 TableDesignerInfo 类实例当前类的属性已更改。</param>
		internal CheckedColumnCollection(TableDesignerCollection tables) : base() { tableDesignerInfos = tables; }

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal override string GetKey(ColumnDesignerInfo item) { return item.Name; }
	}
}
