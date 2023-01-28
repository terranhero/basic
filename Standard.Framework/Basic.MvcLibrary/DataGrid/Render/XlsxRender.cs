using System.IO;
// ***********************************************************************
// Assembly         : Basic.MvcLibrary
// Author           : JACKY
// Created          : 09-19-2012
using System.Web;
using Basic.Enums;
using Basic.Interfaces;

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

		/// <summary>
		/// 下载文件名称。
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// 输出控件类型
		/// </summary>
		/// <param name="context">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		public void Render(Basic.MvcLibrary.IBasicContext context, IPagination<T> dataSource)
		{
			IDataGridExportBuilder builder = DataGridExportProviders.ExportBuilder;
			if (builder == null) { return; }
			context.ClearContent();
			context.ClearHeaders();
			string browser = context.Browser.ToUpper();
			if (string.IsNullOrEmpty(FileName))
				context.AddHeader("Content-Disposition", "attachment; filename=DownloadData.xlsx");
			else if (browser.Contains("FIREFOX") || browser.Contains("FF"))
				context.AddHeader("Content-Disposition", string.Concat("attachment; filename=", FileName, ".xlsx"));
			else/*IE CHROME*/
				context.AddHeader("Content-Disposition", string.Concat("attachment; filename=", HttpUtility.UrlEncode(FileName), ".xlsx"));
			context.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			context.ContentEncoding = System.Text.Encoding.Unicode;
			IDataGridExportRender<T> exporter = builder.Create<T>(RenderMode.ExcelXlsx, _DataGrid, _DataGrid.Columns);
			using (MemoryStream stream = new MemoryStream())
			{
				exporter.Write(stream, dataSource);
				stream.Flush();
				stream.Position = 0;
				context.WriteAsync(stream.ToArray());
			}
			context.Flush();
			context.End();
		}

		/// <summary>
		/// 输出控件类型
		/// </summary>
		/// <param name="context">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="writer">获取或设置用户输出 HTML 对象的 TextWriter 。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		public void Render(HttpContext context, System.IO.TextWriter writer, IPagination<T> dataSource)
		{
			IDataGridExportBuilder builder = DataGridExportProviders.ExportBuilder;
			if (builder == null) { return; }
			context.Response.ClearContent();
			context.Response.ClearHeaders();
			string browser = context.Request.Browser.Browser.ToUpper();
			if (string.IsNullOrEmpty(FileName))
				context.Response.AddHeader("Content-Disposition", "attachment; filename=DownloadData.xlsx");
			else if (browser.Contains("FIREFOX") || browser.Contains("FF"))
				context.Response.AddHeader("Content-Disposition", string.Concat("attachment; filename=", FileName, ".xlsx"));
			else/*IE CHROME*/
				context.Response.AddHeader("Content-Disposition", string.Concat("attachment; filename=", HttpUtility.UrlEncode(FileName), ".xlsx"));
			context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			context.Response.ContentEncoding = System.Text.Encoding.Unicode;
			IDataGridExportRender<T> exporter = builder.Create<T>(RenderMode.ExcelXlsx, _DataGrid, _DataGrid.Columns);
			using (MemoryStream stream = new MemoryStream())
			{
				exporter.Write(stream, dataSource);
				stream.Flush();
				stream.Position = 0;
				context.Response.BinaryWrite(stream.ToArray());
			}
			context.Response.Flush();
			context.Response.End();
		}
	}
}
