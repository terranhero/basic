
namespace Basic.Enums
{
	/// <summary>
	/// 表示在选定 TreeView 控件中的节点时将引发的事件。
	/// </summary>
	public enum SelectAction
	{
		/// <summary>
		/// 当节点选择是引发 SelectedNodeChanged 事件
		/// </summary>
		Select,
		/// <summary>
		/// 在选定节点时引发 TreeNodeExpanded 事件。
		/// </summary>
		Expand,
		/// <summary>
		/// 在选定节点时引发 SelectedNodeChanged 和 TreeNodeExpanded 两个事件。
		/// </summary>
		SelectExpand,
		/// <summary>
		/// 在选定节点时不引发任何事件。
		/// </summary>
		None
	}
}
