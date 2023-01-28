using System.Collections.Specialized;
using Basic.Database;

namespace Basic.Collections
{
	/// <summary>
	/// 表示 ColumnDesignerInfo 类的集合。
	/// </summary>
	public sealed class ColumnDesignerCollection : BaseCollection<ColumnDesignerInfo>, INotifyCollectionChanged
	{
		private readonly TableDesignerInfo tableDesignerInfo;
		/// <summary>
		/// 初始化 ColumnDesignerCollection 类的新实例。
		/// </summary>
		/// <param name="table">需要通知 TableDesignerInfo 类实例当前类的属性已更改。</param>
		internal ColumnDesignerCollection(TableDesignerInfo table)
			: base()
		{
			tableDesignerInfo = table;
		}

		/// <summary>
		/// 添加项到集合中。
		/// </summary>
		public ColumnDesignerInfo CreateColumn()
		{
			return new ColumnDesignerInfo(tableDesignerInfo);
		}

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal override string GetKey(ColumnDesignerInfo item) { return item.Name; }
	}
}
