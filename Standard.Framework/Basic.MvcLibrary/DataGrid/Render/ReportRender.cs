﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;
using Basic.Enums;
using Basic.Interfaces;
using System.Linq;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>输出EasyUI.DataGrid类型的表格</summary>
	/// <typeparam name="T"></typeparam>
	internal class ReportRender<T> : IRender<T> where T : class
	{
		private const string _DefaultWidth = "80";
		private readonly DataGrid<T> _DataGrid;
		private readonly Func<T, string> _RenderRowTemplate;
		private readonly DataGridOptions _Options;

		/// <summary>初始化 HtmlRender 类实例</summary>
		/// <param name="grid">IEasyGrid表格</param>
		internal ReportRender(DataGrid<T> grid) { _DataGrid = grid; _Options = grid.Options; _RenderRowTemplate = grid.RenderRowTemplate; }

		/// <summary>
		/// 输出表格头列信息
		/// </summary>
		/// <param name="writer">接收EasyUI.DataGrid内容的 HtmlTextWriter 对象。</param>
		private void RenderReportHeader(HtmlTextWriter writer)
		{
			if (_Options.ShowHeader == false) { return; }
			writer.RenderBeginTag(HtmlTextWriterTag.Thead);

			DataGridRowCollection<T> rows = _DataGrid.Columns;
			foreach (DataGridRow<T> row in rows)
			{
				if (row.HasCssClass == true) { writer.AddAttribute(HtmlTextWriterAttribute.Class, row.CssClass); }
				if (row.Height.IsEmpty == false) { writer.AddStyleAttribute(HtmlTextWriterStyle.Height, row.Height.ToString()); }
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				foreach (DataGridColumn<T> col in row)
				{
					if (col.AllowToHtml == false) { continue; }
					if (col is DataGridArrayColumn<T>)
					{
						DataGridArrayColumn<T> arrayColumn = col as DataGridArrayColumn<T>;
						foreach (var item in arrayColumn.Fields)
						{
							writer.AddAttribute("field", item.Key);

							if (col.Width.IsEmpty == false) { writer.AddStyleAttribute(HtmlTextWriterStyle.Width, col.Width.ToString()); }

							if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, col.HeaderAlign); }
							if (col.Hidden) { writer.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }
							if (col.HasCssClass) { writer.AddAttribute(HtmlTextWriterAttribute.Class, col.CssClass); }

							writer.RenderBeginTag(HtmlTextWriterTag.Th);
							writer.Write(item.Value);
							writer.RenderEndTag();
						}
					}
					else if (col is DataGridArraysColumn<T>)
					{
						DataGridArraysColumn<T> arrayColumn = col as DataGridArraysColumn<T>;
						foreach (var item in arrayColumn.Fields)
						{
							foreach (var keyValue in item.Value)
							{
								writer.AddAttribute("field", keyValue.Key);

								if (col.Width.IsEmpty == false) { writer.AddStyleAttribute(HtmlTextWriterStyle.Width, col.Width.ToString()); }

								if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, col.HeaderAlign); }
								if (col.Hidden) { writer.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }
								if (col.HasCssClass) { writer.AddAttribute(HtmlTextWriterAttribute.Class, col.CssClass); }

								writer.RenderBeginTag(HtmlTextWriterTag.Th);
								writer.Write(keyValue.Value);
								writer.RenderEndTag();
							}
						}
					}
					else if (col is DataGridHeaderColumn<T>)
					{
						if (col.Width.IsEmpty == false) { writer.AddStyleAttribute(HtmlTextWriterStyle.Width, col.Width.ToString()); }

						if (col.Rowspan > 1) { writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, col.Rowspan.ToString()); }
						if (col.Colspan > 1) { writer.AddAttribute(HtmlTextWriterAttribute.Colspan, col.Colspan.ToString()); }

						if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, col.HeaderAlign); }
						if (col.Hidden) { writer.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }

						if (col.HasCssClass) { writer.AddAttribute(HtmlTextWriterAttribute.Class, col.CssClass); }

						writer.RenderBeginTag(HtmlTextWriterTag.Th);
						if (!string.IsNullOrEmpty(col.Title)) { writer.Write(col.Title); }
						else { writer.Write(col.Field); }
						writer.RenderEndTag();
					}
					else
					{
						if (!string.IsNullOrWhiteSpace(col.Field)) { writer.AddAttribute("field", col.Field); }

						if (col.Width.IsEmpty == false) { writer.AddStyleAttribute(HtmlTextWriterStyle.Width, col.Width.ToString()); }

						if (col.Rowspan > 1) { writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, col.Rowspan.ToString()); }
						if (col.Colspan > 1) { writer.AddAttribute(HtmlTextWriterAttribute.Colspan, col.Colspan.ToString()); }

						if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, col.HeaderAlign); }
						if (col.Hidden || col.JsonData) { writer.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }
						if (col.Fixed == true) { writer.AddAttribute("fixed", col.Fixed.ToString().ToLower()); }
						if (col.HasCssClass) { writer.AddAttribute(HtmlTextWriterAttribute.Class, col.CssClass); }

						writer.RenderBeginTag(HtmlTextWriterTag.Th);
						if (!string.IsNullOrEmpty(col.Title)) { writer.Write(col.Title); }
						else { writer.Write(col.Field); }
						writer.RenderEndTag();
					}
				}
				writer.RenderEndTag();
			}
			writer.RenderEndTag();
		}

		/// <summary>
		/// 输出表格主体详细Html代码
		/// </summary>
		/// <param name="writer">获取或设置用户输出 HTML 对象的 TextWriter 。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		private void RenderBody(HtmlTextWriter writer, IPagination<T> dataSource)
		{
			if (dataSource == null || dataSource.Count == 0) { return; }
			if (_DataGrid.Columns.Count == 0) { return; }
			IEnumerable<DataGridColumn<T>> columns = _DataGrid.Columns.GetColumns();
			NumberColumn<T>[] numbers = columns.Where(m => m is NumberColumn<T>).Cast<NumberColumn<T>>().ToArray();
			int beginNumber = (dataSource.PageIndex - 1) * dataSource.PageSize + 1;
			Array.ForEach(numbers, m => m.CurrentNumber = beginNumber);
			writer.RenderBeginTag(HtmlTextWriterTag.Tbody);
			foreach (T model in dataSource)
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				foreach (DataGridColumn<T> column in columns)
				{
					if (column.AllowToHtml == false) { continue; }
					if (column is DataGridArrayColumn<T>)
					{
						DataGridArrayColumn<T> arrayColumn = column as DataGridArrayColumn<T>;
						IDictionary<string, string> values = arrayColumn.GetStrings(model);
						foreach (var item in arrayColumn.Fields)
						{
							writer.AddAttribute("field", item.Key);
							if (column.Hidden) { writer.AddAttribute("hidden", column.Hidden.ToString().ToLower()); }
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							if (values.ContainsKey(item.Key))
							{
								writer.Write(values[item.Key]);
							}
							writer.RenderEndTag();
						}
					}
					else if (column is DataGridArraysColumn<T>)
					{
						DataGridArraysColumn<T> arrayColumn = column as DataGridArraysColumn<T>;
						IDictionary<string, string> values = arrayColumn.GetStrings(model);
						foreach (var item in arrayColumn.Fields)
						{
							writer.AddAttribute("field", item.Key);
							if (column.Hidden) { writer.AddAttribute("hidden", column.Hidden.ToString().ToLower()); }
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							if (values.ContainsKey(item.Key))
							{
								writer.Write(values[item.Key]);
							}
							writer.RenderEndTag();
						}
					}
					else
					{
						writer.AddAttribute("field", column.Field);
						if (column.Hidden || column.JsonData) { writer.AddAttribute("hidden", column.Hidden.ToString().ToLower()); }
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						writer.Write(column.GetString(model));
						writer.RenderEndTag();
					}
				}
				writer.RenderEndTag();
			}
			writer.RenderEndTag();
		}

		/// <summary>自定义行输出格式</summary>
		/// <param name="writer">获取或设置用户输出 HTML 对象的 TextWriter 。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		private void RenderRowTemplate(HtmlTextWriter writer, IPagination<T> dataSource)
		{
			if (dataSource == null || dataSource.Count == 0) { return; }
			if (this._RenderRowTemplate == null) { return; }
			writer.RenderBeginTag(HtmlTextWriterTag.Tbody);
			foreach (T model in dataSource)
			{
				string rowContent = _RenderRowTemplate.Invoke(model);
				writer.Write(rowContent);
			}
			writer.RenderEndTag();
		}

		/// <summary>
		/// 输出表格主体详细Html代码（Group 汇总）
		/// </summary>
		/// <param name="writer">获取或设置用户输出 HTML 对象的 TextWriter 。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		/// <param name="groupInfo">表格分组信息</param>
		private void RenderGroupBody(HtmlTextWriter writer, IPagination<T> dataSource, GroupInfo<T> groupInfo)
		{
			if (dataSource == null || dataSource.Count == 0) { return; }
			if (_DataGrid.Columns.Count == 0) { return; }

			IEnumerable<DataGridColumn<T>> columns = _DataGrid.Columns.GetColumns();
			IEnumerable<IGrouping<dynamic, T>> groupSource = groupInfo.GroupBy(dataSource);
			GroupRowCollection<T> rows = groupInfo.GetRows();
			writer.RenderBeginTag(HtmlTextWriterTag.Tbody);
			foreach (IGrouping<dynamic, T> grouping in groupSource.ToArray())
			{
				#region 输出分组明细数据
				foreach (T model in grouping)
				{
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);
					foreach (DataGridColumn<T> column in columns)
					{
						if (column.AllowToHtml == false) { continue; }
						if (column is DataGridArrayColumn<T>)
						{
							DataGridArrayColumn<T> arrayColumn = column as DataGridArrayColumn<T>;
							IDictionary<string, string> values = arrayColumn.GetStrings(model);
							foreach (var item in arrayColumn.Fields)
							{
								writer.AddAttribute("field", item.Key);
								if (column.Hidden) { writer.AddAttribute("hidden", column.Hidden.ToString().ToLower()); }
								writer.RenderBeginTag(HtmlTextWriterTag.Td);
								if (values.ContainsKey(item.Key))
								{
									writer.Write(values[item.Key]);
								}
								writer.RenderEndTag();
							}
						}
						else if (column is DataGridArraysColumn<T>)
						{
							DataGridArraysColumn<T> arrayColumn = column as DataGridArraysColumn<T>;
							IDictionary<string, string> values = arrayColumn.GetStrings(model);
							foreach (var item in arrayColumn.Fields)
							{
								writer.AddAttribute("field", item.Key);
								if (column.Hidden) { writer.AddAttribute("hidden", column.Hidden.ToString().ToLower()); }
								writer.RenderBeginTag(HtmlTextWriterTag.Td);
								if (values.ContainsKey(item.Key))
								{
									writer.Write(values[item.Key]);
								}
								writer.RenderEndTag();
							}
						}
						else
						{
							writer.AddAttribute("field", column.Field);
							if (column.Hidden || column.JsonData) { writer.AddAttribute("hidden", column.Hidden.ToString().ToLower()); }
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							writer.Write(column.GetString(model));
							writer.RenderEndTag();
						}
					}
					writer.RenderEndTag();
				}
				#endregion
				foreach (GroupRow<T> row in rows)
				{
					if (!string.IsNullOrWhiteSpace(groupInfo.RowStyle))
						writer.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Class, groupInfo.RowStyle);
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);
					foreach (GroupCell<T> cell in row)
					{
						if (cell.Rowspan >= 2) { writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, Convert.ToString(cell.Rowspan)); }
						if (cell.Colspan >= 2) { writer.AddAttribute(HtmlTextWriterAttribute.Colspan, Convert.ToString(cell.Colspan)); }
						if (string.IsNullOrWhiteSpace(cell.Align)) { writer.AddAttribute(HtmlTextWriterAttribute.Align, cell.Align); }
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						writer.Write(cell.GetValue(grouping));
						writer.RenderEndTag();
					}
					writer.RenderEndTag();  //end Tr
				}
			}
			writer.RenderEndTag();  //end Tbody
		}

		/// <summary>
		/// 输出表格主体详细Html代码
		/// </summary>
		/// <param name="writer">获取或设置用户输出 HTML 对象的 TextWriter 。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		/// <param name="footerInfo">表示表格页脚信息</param>
		private void RenderFoot(HtmlTextWriter writer, IPagination<T> dataSource, FooterInfo<T> footerInfo)
		{
			if (dataSource == null || dataSource.Count == 0) { return; }
			if (footerInfo == null) { return; }

			writer.RenderBeginTag(HtmlTextWriterTag.Tfoot);
			foreach (FooterRow<T> row in footerInfo.GetRows())
			{
				if (!string.IsNullOrWhiteSpace(footerInfo.RowStyle))
					writer.AddAttribute(System.Web.UI.HtmlTextWriterAttribute.Class, footerInfo.RowStyle);
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				foreach (FooterCell<T> cell in row)
				{
					if (cell.Rowspan >= 2) { writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, Convert.ToString(cell.Rowspan)); }
					if (cell.Colspan >= 2) { writer.AddAttribute(HtmlTextWriterAttribute.Colspan, Convert.ToString(cell.Colspan)); }
					if (string.IsNullOrWhiteSpace(cell.Align)) { writer.AddAttribute(HtmlTextWriterAttribute.Align, cell.Align); }
					if (cell.HasCssClass == true) { writer.AddAttribute(HtmlTextWriterAttribute.Class, cell.CssClass); }
					writer.RenderBeginTag(HtmlTextWriterTag.Td);
					writer.Write(cell.GetValue(dataSource));
					writer.RenderEndTag();
				}
				writer.RenderEndTag();  //end Tr
			}
			writer.RenderEndTag();
		}

		/// <summary>
		/// 下载文件名称。
		/// </summary>
		public string FileName { get; set; }

		/// <summary>输出控件类型
		/// </summary>
		/// <param name="context">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		public void Render(IBasicContext context, IPagination<T> dataSource)
		{
			using (HtmlTextWriter htmlWriter = new HtmlTextWriter(context.Writer))
			{
				if (dataSource != null) { _Options.Total = dataSource.Capacity; }
				htmlWriter.AddAttribute(_Options);
				htmlWriter.AddStyleAttribute(_Options.StyleOptions);
				htmlWriter.AddAttribute("data-options", _Options.GetOptions);
				htmlWriter.RenderBeginTag(HtmlTextWriterTag.Table);

				if (string.IsNullOrWhiteSpace(_DataGrid.Caption) == false)
				{
					htmlWriter.RenderBeginTag(HtmlTextWriterTag.Caption);
					htmlWriter.Write(_DataGrid.Caption);
					htmlWriter.RenderEndTag();
				}
				RenderReportHeader(htmlWriter);

				if (_RenderRowTemplate == null)
				{
					GroupInfo<T> groupInfo = _DataGrid.GetGroupInfo();
					if (groupInfo != null) { RenderGroupBody(htmlWriter, dataSource, groupInfo); }
					else { RenderBody(htmlWriter, dataSource); }
				}
				else { RenderRowTemplate(htmlWriter, dataSource); }

				FooterInfo<T> footerInfo = _DataGrid.GetFooterInfo();
				if (footerInfo != null) { RenderFoot(htmlWriter, dataSource, footerInfo); }
				htmlWriter.RenderEndTag();
			}
		}

		/// <summary>输出控件类型</summary>
		/// <param name="context">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="writer">获取或设置用户输出 HTML 对象的 TextWriter 。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		public void Render(HttpContext context, System.IO.TextWriter writer, IPagination<T> dataSource)
		{
			using (HtmlTextWriter htmlWriter = new HtmlTextWriter(writer))
			{
				htmlWriter.AddAttribute(_Options);
				htmlWriter.AddStyleAttribute(_Options.StyleOptions);
				htmlWriter.AddAttribute("data-options", _Options.GetOptions);
				htmlWriter.RenderBeginTag(HtmlTextWriterTag.Table);
				if (string.IsNullOrWhiteSpace(_DataGrid.Caption) == false)
				{
					htmlWriter.RenderBeginTag(HtmlTextWriterTag.Caption);
					htmlWriter.Write(_DataGrid.Caption);
					htmlWriter.RenderEndTag();
				}
				RenderReportHeader(htmlWriter);

				if (_RenderRowTemplate == null)
				{
					GroupInfo<T> groupInfo = _DataGrid.GetGroupInfo();
					if (groupInfo != null) { RenderGroupBody(htmlWriter, dataSource, groupInfo); }
					else { RenderBody(htmlWriter, dataSource); }
				}
				else { RenderRowTemplate(htmlWriter, dataSource); }

				FooterInfo<T> footerInfo = _DataGrid.GetFooterInfo();
				if (footerInfo != null) { RenderFoot(htmlWriter, dataSource, footerInfo); }

				htmlWriter.RenderEndTag();
			}
		}
	}
}
