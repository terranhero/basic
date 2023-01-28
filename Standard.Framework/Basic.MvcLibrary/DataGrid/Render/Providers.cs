using System.Collections.Generic;
using Basic.Enums;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// DataGrid Eexcl导出器提供程序
	/// </summary>
	public static class DataGridExportProviders
	{
		private static IDataGridExportBuilder _ExportBuilder;

		/// <summary>
		/// 获取 IDataGridExportBuilder 实例。
		/// </summary>
		public static IDataGridExportBuilder ExportBuilder { get { return _ExportBuilder; } set { _ExportBuilder = value; } }
	}

	/// <summary>
	/// 表示 Excel 导出程序构建器
	/// </summary>
	public interface IDataGridExportBuilder
	{
		/// <summary>
		/// <![CDATA[创建 IDataGridExportRender<T> 类型实例]]>
		/// </summary>
		/// <typeparam name="T">表示实体模型类型</typeparam>
		/// <param name="list">表示</param>
		/// <param name="mode"></param>
		/// <param name="grid"></param>
		/// <returns></returns>
		IDataGridExportRender<T> Create<T>(RenderMode mode, DataGrid<T> grid, DataGridRowCollection<T> list) where T : class;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IDataGridExportRender<T>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="dataSource"></param>
		void Write(System.IO.Stream stream, IEnumerable<T> dataSource);
	}
}
