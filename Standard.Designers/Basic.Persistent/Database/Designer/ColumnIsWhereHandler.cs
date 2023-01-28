using System;

namespace Basic.Database
{
	/// <summary>
	/// 表示 ColumnDesignerInfo 类中 IsWhere 属性已更改的委托参数。
	/// </summary>
	public sealed class ColumnIsWhereEventArgs : EventArgs
	{	
		private readonly ColumnDesignerInfo columnInfo;
		/// <summary>
		/// 使用带 ColumnDesignerInfo类，初始化 ColumnCheckedEventArgs 类实例。
		/// </summary>
		/// <param name="column"></param>
		public ColumnIsWhereEventArgs(ColumnDesignerInfo column) : base() { columnInfo = column; }

		/// <summary>
		/// 表示当前更改的 ColumnDesignerInfo 类实例。
		/// </summary>
		public ColumnDesignerInfo Column { get { return columnInfo; } }
	}

	/// <summary>
	/// 表示数据库字段中 IsWhere 属性已更改的事件委托。
	/// </summary>
	/// <param name="sender">委托的发起者</param>
	/// <param name="e">带有 ColumnIsWhereEventArgs 类的事件参数。</param>
	public delegate void ColumnIsWhereChangedHandler(object sender, ColumnIsWhereEventArgs e);
}
