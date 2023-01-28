
namespace Basic.Enums
{
	/// <summary>
	/// 指定数据控件（例如 EasyGrid 或 EasyTreeGrid 控件）中行的功能。
	/// </summary>
	public enum RowType
	{
		/// <summary>
		///  数据控件的标题行。
		///  标题行不能绑定数据。 
		/// </summary>
		Header,
		/// <summary>
		///  数据控件的脚注行。
		///  脚注行不能绑定数据。 
		/// </summary>
		Footer,
		/// <summary>
		///  数据控件的数据行。
		///  只有 DataRow 行能绑定数据。  
		/// </summary>
		DataRow,

		/// <summary>
		///  数据控件的数据行。
		///  只有 DataRow 行能绑定数据。  
		/// </summary>
		JsonRow
	}
}
