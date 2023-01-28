
namespace Basic.Enums
{
	/// <summary>
	/// Web控件输出模式
	/// </summary>
	public enum RenderMode : byte
	{
		/// <summary>
		/// 生成网页输出模式
		/// </summary>
		Normal = 0,
		/// <summary>
		/// 生成Json格式
		/// </summary>
		Json = 1,
		/// <summary>
		/// 生成Excel 2003及以前格式
		/// </summary>
		ExcelXls = 2,
		/// <summary>
		/// 生成Excel 2007及以后格式
		/// </summary>
		ExcelXlsx = 3,
		/// <summary>
		/// 表格内容，没有标题
		/// </summary>
		TableContent = 4,
	}
}
