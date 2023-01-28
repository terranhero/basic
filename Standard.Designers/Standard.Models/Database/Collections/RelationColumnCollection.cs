using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Database;
using System.Collections.Specialized;

namespace Basic.Collections
{
	/// <summary>
	/// 表示 RelationColumnInfo 类的集合。
	/// </summary>
	public sealed class RelationColumnCollection : BaseCollection<RelationColumnInfo>, INotifyCollectionChanged
	{
		private readonly RelationDesignerInfo relationInfo;
		/// <summary>
		/// 初始化 RelationColumnCollection 类的新实例。
		/// </summary>
		/// <param name="relation">需要通知 TableDesignerInfo 类实例当前类的属性已更改。</param>
		internal RelationColumnCollection(RelationDesignerInfo relation) : base() { relationInfo = relation; }

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected override string GetKey(RelationColumnInfo item) { return item.Parent.Name; }
	}
}
