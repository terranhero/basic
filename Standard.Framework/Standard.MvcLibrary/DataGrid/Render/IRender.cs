using Basic.Interfaces;
using System.Threading.Tasks;

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

		/// <summary>输出内容</summary>
		/// <param name="writer">视图输出</param>
		/// <param name="response">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		void Render(System.IO.TextWriter writer, ICustomResponse response, IPagination<T> dataSource);

		/// <summary>使用异步方法输出内容</summary>
		/// <param name="writer">上下文视图输出</param>
		/// <param name="response">自定义内容输出</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		Task RenderAsync(System.IO.TextWriter writer, ICustomResponse response, IPagination<T> dataSource);
	}
}
