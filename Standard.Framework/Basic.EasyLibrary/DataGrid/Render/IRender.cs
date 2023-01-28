using System.Web;
using Basic.Interfaces;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 输出EasyUI.DataGrid类型的表格
	/// </summary>
	public interface IRender<T> where T : class
	{
		/// <summary>
		/// 下载文件名称。
		/// </summary>
		string FileName { get; set; }

		/// <summary>
		/// 输出控件类型
		/// </summary>
		/// <param name="context">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		void Render(IBasicContext context, IPagination<T> dataSource);
	}
}
