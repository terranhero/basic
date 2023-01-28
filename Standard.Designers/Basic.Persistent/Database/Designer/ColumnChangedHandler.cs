using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Database
{
	/// <summary>
	/// 表示 ColumnDesignerInfo 类中属性已更改的委托参数。
	/// </summary>
	public sealed class ColumnChangedEventArgs : EventArgs
	{
		private readonly ColumnDesignerInfo columnInfo;
		/// <summary>
		/// 使用带 ColumnDesignerInfo类，初始化 ColumnCheckedEventArgs 类实例。
		/// </summary>
		/// <param name="column"></param>
		public ColumnChangedEventArgs(ColumnDesignerInfo column) : base() { columnInfo = column; }

		/// <summary>
		/// 表示当前更改的 ColumnDesignerInfo 类实例。
		/// </summary>
		public ColumnDesignerInfo Column { get { return columnInfo; } }
	}

	/// <summary>
	/// 表示数据库字段中 Checked 属性已更改的事件委托。
	/// </summary>
	/// <param name="sender">委托的发起者</param>
	/// <param name="e">带有 ColumnCheckedEventArgs 类的事件参数。</param>
	public delegate void ColumnChangedHandler(object sender, ColumnChangedEventArgs e);
}
