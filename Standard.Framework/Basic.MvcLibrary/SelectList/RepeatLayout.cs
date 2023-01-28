
namespace Basic.MvcLibrary
{
	/// <summary>
	/// 指定列表控件项的布局。
	/// </summary>
	public enum RepeatLayout
	{
		/// <summary>
		/// 在表中显示项。
		/// </summary>
		Table = 0,
		/// <summary>
		/// 不以表结构显示项。呈现的标记由 span 元素组成，并且各项由 br 元素分隔。
		/// </summary>
		Flow = 1,
	}
}
