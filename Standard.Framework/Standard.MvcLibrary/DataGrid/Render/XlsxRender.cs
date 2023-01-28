using System.IO;
using System.Threading.Tasks;
using System.Web;
using Basic.Enums;
using Basic.Interfaces;
using Basic.MvcLibrary;
using Microsoft.AspNetCore.Http;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 导出表格控件到Excel2007/2010格式
	/// </summary>
	/// <typeparam name="T">实体模型类型</typeparam>
	internal class XlsxRender<T> : IRender<T> where T : class
	{
		private readonly DataGrid<T> _DataGrid;
		/// <summary>
		/// 初始化XlsxRender类实例
		/// </summary>
		/// <param name="grid">IEasyGrid表格</param>
		internal XlsxRender(DataGrid<T> grid) { _DataGrid = grid; }

		/// <summary>下载文件名称</summary>
		public string FileName { get; set; }

		/// <summary>输出 DataGrid 内容（Excel 2007 Or Later）</summary>
		/// <param name="writer">视图输出</param>
		/// <param name="response">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		public void Render(System.IO.TextWriter writer, ICustomResponse response, IPagination<T> dataSource)
		{
			IDataGridExportBuilder builder = DataGridExportProviders.ExportBuilder;
			if (builder == null) { return; }
			response.Clear();
			response.Headers.Clear();
			string contentDisposition;
			if (string.IsNullOrEmpty(FileName)) { contentDisposition = "attachment; filename=DownloadData.xlsx"; }
			else { contentDisposition = string.Concat("attachment; filename=", HttpUtility.UrlEncode(FileName), ".xlsx"); }
			response.AddHeader("Content-Disposition", contentDisposition);
			response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			IDataGridExportRender<T> exporter = builder.Create<T>(RenderMode.ExcelXlsx, _DataGrid, _DataGrid.Columns);
			using (MemoryStream stream = new MemoryStream())
			{
				exporter.Write(stream, dataSource);
				stream.Flush();
				stream.Position = 0;
				response.WriteAsync(stream.ToArray());
			}
			response.Body.Flush();
		}

		/// <summary>输出 DataGrid 内容（Excel 2007 Or Later）</summary>
		/// <param name="writer">视图输出</param>
		/// <param name="response">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		public async Task RenderAsync(System.IO.TextWriter writer, ICustomResponse response, IPagination<T> dataSource)
		{
			IDataGridExportBuilder builder = DataGridExportProviders.ExportBuilder;
			if (builder == null) { return; }
			response.Clear();
			response.Headers.Clear();
			string contentDisposition;
			if (string.IsNullOrEmpty(FileName)) { contentDisposition = "attachment; filename=DownloadData.xlsx"; }
			else { contentDisposition = string.Concat("attachment; filename=", HttpUtility.UrlEncode(FileName), ".xlsx"); }
			response.AddHeader("Content-Disposition", contentDisposition);
			response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			IDataGridExportRender<T> exporter = builder.Create<T>(RenderMode.ExcelXlsx, _DataGrid, _DataGrid.Columns);
			using (MemoryStream stream = new MemoryStream())
			{
				await exporter.WriteAsync(stream, dataSource);
				await stream.FlushAsync();
				stream.Position = 0;
				await response.WriteAsync(stream.ToArray());
			}
			await response.FlushAsync();
		}
	}
}
