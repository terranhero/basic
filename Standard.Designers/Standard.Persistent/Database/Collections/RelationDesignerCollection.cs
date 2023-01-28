using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Database;
using System.Collections.Specialized;

namespace Basic.Collections
{
	/// <summary>
	/// 表示 RelationDesignerInfo 类的集合。
	/// </summary>
	public sealed class RelationDesignerCollection : BaseCollection<RelationDesignerInfo>, INotifyCollectionChanged
	{
		private readonly TableDesignerCollection tableInfos;
		/// <summary>
		/// 
		/// </summary>
		public TableDesignerCollection Tables { get { return tableInfos; } }
		/// <summary>
		/// 初始化 RelationDesignerCollection 类的新实例。
		/// </summary>
		/// <param name="tables">需要通知 TableDesignerInfo 类实例当前类的属性已更改。</param>
		internal RelationDesignerCollection(TableDesignerCollection tables) : base() { tableInfos = tables; }

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal override string GetKey(RelationDesignerInfo item) { return item.Parent.Name; }

		/// <summary>
		/// 引发 RelationChanged 事件的受保护方法。
		/// </summary>
		/// <param name="relationInfo">引发事件的 RelationDesignerInfo 类实例。</param>
		internal void OnRelationChanged(RelationDesignerInfo relationInfo)
		{
			tableInfos.OnRelationChanged(relationInfo);
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			tableInfos.OnRelationChanged(null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void InsertItem(int index, RelationDesignerInfo item)
		{
			base.InsertItem(index, item);
			tableInfos.OnRelationChanged(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		protected override void RemoveItem(int index)
		{
			RelationDesignerInfo item = base.Items[index];
			base.RemoveItem(index);
			tableInfos.OnRelationChanged(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void SetItem(int index, RelationDesignerInfo item)
		{
			base.SetItem(index, item);
			tableInfos.OnRelationChanged(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oldIndex"></param>
		/// <param name="newIndex"></param>
		protected override void MoveItem(int oldIndex, int newIndex)
		{
			RelationDesignerInfo item = base.Items[oldIndex];
			base.MoveItem(oldIndex, newIndex);
			tableInfos.OnRelationChanged(item);
		}
	}
}
