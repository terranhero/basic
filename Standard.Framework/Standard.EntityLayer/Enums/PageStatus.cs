
namespace Basic.MvcLibrary
{
	/// <summary>
	/// 页面状态枚举
	/// </summary>
	[System.Flags()]
	public enum PageStatus
	{
		/// <summary>
		/// 页面状态为默认状态，初始化时的状态
		/// </summary>
		None = 0,
		/// <summary>
		/// 页面状态为默认状态，初始化时的状态
		/// </summary>
		Normal = 1,
		/// <summary>
		/// 当选择DataGrid一行单元格后，才能有效的枚举值。
		/// </summary>
		SelectedRow = 2,
		/// <summary>
		/// 当前页面处于已经选择DataGrid的单元格，DataGrid的单元格被选择后的状态。
		/// </summary>
		SelectedRows = 4,
		/// <summary>
		/// 当选择DataGrid一行单元格勾选后，才能有效的枚举值。
		/// </summary>
		CheckedRow = 8,
		/// <summary>
		/// 当前页面处于已经勾选多行DataGrid的单元格，DataGrid的单元格被选择后的状态。
		/// </summary>
		CheckedRows = 16,
		/// <summary>
		/// 当前页面处于TreeNode选择状态，TreeNode被选择后的状态。
		/// </summary>
		SelectedNode = 32,
		/// <summary>
		/// 当前页面处于新增状态，新增按钮单击后的状态。
		/// </summary>
		AddNew = 64,
		/// <summary>
		/// 当前页面处于修改状态，修改按钮单击后的状态。
		/// </summary>
		Update = 128,
		/// <summary>
		/// 当前页面处于修改状态，修改按钮单击后的状态。
		/// </summary>
		Delete = 256,
		/// <summary>
		/// 当前页面已经查询出数据状态。
		/// </summary>
		Search = 512,
	}
}
