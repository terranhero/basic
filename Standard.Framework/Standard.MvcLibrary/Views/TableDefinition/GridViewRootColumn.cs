using System.IO;
using Basic.Collections;

namespace Basic.MvcLibrary
{
	/// <summary>表示 el-table 默认列集合定义</summary>
	internal sealed class GridViewRootColumn<T> : GridViewColumn<T> where T : class
	{
		private readonly GridView<T> mGridView;
		/// <summary>初始化 GridViewColumn 类实例</summary>
		public GridViewRootColumn(GridView<T> view, EntityPropertyCollection props) : base(view.Basic, props) { mGridView = view; }

		/// <summary>输出表格列信息</summary>
		/// <param name="writer"></param>
		protected internal override void Render(TextWriter writer)
		{
			foreach (GridViewColumn<T> column in Columns)
			{
				column.Render(writer);
			}
		}
	}

}
