using System;
using Basic.Collections;

namespace Basic.MvcLibrary
{
	/// <summary>表示 el-table 列信息定义</summary>
	public sealed class GridViewHeaderColumn<T> : GridViewColumn<T>, IGridViewColumn where T : class
	{
		/// <summary>初始化 GridViewHeaderColumn 类实例</summary>
		/// <param name="bh"></param>
		/// <param name="props"></param>
		public GridViewHeaderColumn(IBasicContext bh, EntityPropertyCollection props) : base(bh, props) { }

		/// <summary>创建列</summary>
		/// <param name="expression"></param>
		public void ColumnsFor(Action<IColumnsProvider<T>> expression)
		{
			if (expression == null) { return; }
			expression.Invoke(Columns);
		}
	}
}
