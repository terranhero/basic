using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Database
{
	/// <summary>
	/// 表示 RelationDesignerInfo 类中属性已更改的委托参数。
	/// </summary>
	public sealed class RelationChangedEventArgs : EventArgs
	{
		private readonly RelationDesignerInfo relationInfo;
		/// <summary>
		/// 使用带 RelationDesignerInfo 类，初始化 RelationChangedEventArgs 类实例。
		/// </summary>
		/// <param name="relation"></param>
		public RelationChangedEventArgs(RelationDesignerInfo relation) : base() { relationInfo = relation; }

		/// <summary>
		/// 表示当前更改的 ColumnDesignerInfo 类实例。
		/// </summary>
		public RelationDesignerInfo Relation { get { return relationInfo; } }
	}

	/// <summary>
	/// 表示数据库字段中 Checked 属性已更改的事件委托。
	/// </summary>
	/// <param name="sender">委托的发起者</param>
	/// <param name="e">带有 ColumnCheckedEventArgs 类的事件参数。</param>
	public delegate void RelationChangedHandler(object sender, RelationChangedEventArgs e);
}
