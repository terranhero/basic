using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Basic.Interfaces;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 输出EasyUI.DataGrid类型的表格
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class ContentRender<T> : IRender<T> where T : class
	{
		private readonly Func<T, string> _RenderRowTemplate;
		private readonly DataGrid<T> _DataGrid;
		private readonly DataGridOptions _Options;
		/// <summary>
		/// 初始化 HtmlRender 类实例
		/// </summary>
		/// <param name="grid">IEasyGrid表格</param>
		internal ContentRender(DataGrid<T> grid) { _DataGrid = grid; _Options = grid.Options; _RenderRowTemplate = grid.RenderRowTemplate; }

		/// <summary>
		/// 输出表格头列信息
		/// </summary>
		/// <param name="writer">接收EasyUI.DataGrid内容的 HtmlTextWriter 对象。</param>
		private void RenderHeader(TextWriter writer)
		{
			if (_Options.ShowHeader == false) { return; }
			using (TagHtmlWriter head = new TagHtmlWriter("thead"))
			{
				head.RenderBeginTag(writer);
				DataGridRowCollection<T> rows = _DataGrid.Columns;
				foreach (DataGridRow<T> row in rows)
				{
					using (TagHtmlWriter tr = new TagHtmlWriter("tr"))
					{
						if (row.HasCssClass == true) { tr.AddAttribute("class", row.CssClass); }
						if (row.Height.IsEmpty == false) { tr.AddStyleAttribute("height", row.Height.ToString()); }
						tr.RenderBeginTag(writer);
						foreach (DataGridColumn<T> col in row)
						{
							if (col.AllowToHtml == false) { continue; }
							if (col is DataGridArrayColumn<T>)
							{
								DataGridArrayColumn<T> arrayColumn = col as DataGridArrayColumn<T>;
								foreach (var item in arrayColumn.Fields)
								{
									TagHtmlWriter th = new TagHtmlWriter("th");
									if (col.HasCssClass) { th.AddAttribute("class", col.CssClass); }
									th.AddAttribute("field", item.Key);
									if (col.Width.IsEmpty == false) { th.AddStyleAttribute("width", col.Width.ToString()); }

									if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { th.AddStyleAttribute("textalign", col.HeaderAlign); }

									if (col.Hidden) { th.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }
									th.RenderBeginTag(writer);
									th.Write(item.Value).RenderContent(writer);
									th.RenderEndTag(writer);
								}
							}
							else if (col is DataGridArraysColumn<T>)
							{
								DataGridArraysColumn<T> arrayColumn = col as DataGridArraysColumn<T>;
								foreach (var item in arrayColumn.Fields)
								{
									foreach (var keyValue in item.Value)
									{
										TagHtmlWriter th = new TagHtmlWriter("th");
										if (col.HasCssClass) { th.AddAttribute("class", col.CssClass); }
										th.AddAttribute("field", keyValue.Key);

										if (col.Width.IsEmpty == false) { th.AddStyleAttribute("width", col.Width.ToString()); }

										if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { th.AddStyleAttribute("textalign", col.HeaderAlign); }

										if (col.Hidden) { th.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }

										th.RenderBeginTag(writer);
										th.Write(keyValue.Value).RenderContent(writer);
										th.RenderEndTag(writer);
									}
								}
							}
							else if (col is DataGridHeaderColumn<T>)
							{
								TagHtmlWriter th = new TagHtmlWriter("th");
								if (col.HasCssClass) { th.AddAttribute("class", col.CssClass); }

								if (col.Width.IsEmpty == false) { th.AddStyleAttribute("width", col.Width.ToString()); }

								if (col.Rowspan > 1) { th.AddAttribute("rowspan", col.Rowspan.ToString()); }
								if (col.Colspan > 1) { th.AddAttribute("colspan", col.Colspan.ToString()); }
								if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { th.AddStyleAttribute("textalign", col.HeaderAlign); }

								if (col.Hidden) { th.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }

								th.RenderBeginTag(writer);
								if (!string.IsNullOrEmpty(col.Title)) { th.Write(col.Title).RenderContent(writer); }
								else { th.Write(col.Field).RenderContent(writer); }
								th.RenderEndTag(writer);
							}
							else
							{
								TagHtmlWriter th = new TagHtmlWriter("th");
								if (col.HasCssClass) { th.AddAttribute("class", col.CssClass); }
								if (!string.IsNullOrWhiteSpace(col.Field)) { th.AddAttribute("field", col.Field); }

								if (col.Width.IsEmpty == false) { th.AddStyleAttribute("width", col.Width.ToString()); }

								if (col.Rowspan > 1) { th.AddAttribute("rowspan", col.Rowspan.ToString()); }
								if (col.Colspan > 1) { th.AddAttribute("colspan", col.Colspan.ToString()); }
								if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { th.AddStyleAttribute("textalign", col.HeaderAlign); }
								if (col.Fixed == true) { th.AddAttribute("fixed", col.Fixed.ToString().ToLower()); }
								if (col.Hidden || col.JsonData) { th.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }

								th.RenderBeginTag(writer);
								if (!string.IsNullOrEmpty(col.Title)) { th.Write(col.Title).RenderContent(writer); }
								else { th.Write(col.Field).RenderContent(writer); }
								th.RenderEndTag(writer);
							}
						}
						tr.RenderEndTag(writer);
					}
				}
				head.RenderEndTag(writer);
			}
		}

		/// <summary>
		/// 输出表格主体详细Html代码
		/// </summary>
		/// <param name="writer">获取或设置用户输出 HTML 对象的 TextWriter 。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		private void RenderBody(TextWriter writer, IPagination<T> dataSource)
		{
			if (dataSource == null || dataSource.Count == 0) { return; }
			if (_DataGrid.Columns.Count == 0) { return; }

			IEnumerable<DataGridColumn<T>> columns = _DataGrid.Columns.GetColumns();

			NumberColumn<T>[] numbers = columns.Where(m => m is NumberColumn<T>).Cast<NumberColumn<T>>().ToArray();
			int beginNumber = (dataSource.PageIndex - 1) * dataSource.PageSize + 1;
			Array.ForEach(numbers, m => m.CurrentNumber = beginNumber);
			using (TagHtmlWriter tag = new TagHtmlWriter("tbody"))
			{
				tag.RenderBeginTag(writer);
				foreach (T model in dataSource)
				{
					using (TagHtmlWriter tr = new TagHtmlWriter("tr"))
					{
						tr.RenderBeginTag(writer);
						foreach (DataGridColumn<T> column in columns)
						{
							if (column.AllowToHtml == false) { continue; }
							if (column is DataGridArrayColumn<T>)
							{
								DataGridArrayColumn<T> arrayColumn = column as DataGridArrayColumn<T>;
								IDictionary<string, string> values = arrayColumn.GetStrings(model);
								foreach (var item in arrayColumn.Fields)
								{
									TagHtmlWriter td = new TagHtmlWriter("td");
									td.AddAttribute("field", item.Key);
									if (column.Hidden) { td.AddAttribute("hidden", column.Hidden.ToString().ToLower()); }
									if (!string.IsNullOrWhiteSpace(column.Align)) { td.AddStyleAttribute("textalign", column.Align); }
									if (column.HasCssClass) { td.AddAttribute("class", column.CssClass); }
									td.RenderBeginTag(writer);
									if (values.TryGetValue(item.Key, out string value)) { td.Write(value).RenderContent(writer); }
									td.RenderEndTag(writer);
								}
							}
							else if (column is DataGridArraysColumn<T>)
							{
								DataGridArraysColumn<T> arrayColumn = column as DataGridArraysColumn<T>;
								IDictionary<string, string> values = arrayColumn.GetStrings(model);
								foreach (var item in arrayColumn.Fields)
								{
									TagHtmlWriter td = new TagHtmlWriter("td");
									td.AddAttribute("field", item.Key);
									if (column.Hidden) { td.AddAttribute("hidden", column.Hidden.ToString().ToLower()); }
									if (!string.IsNullOrWhiteSpace(column.Align)) { td.AddStyleAttribute("textalign", column.Align); }
									if (column.HasCssClass) { td.AddAttribute("class", column.CssClass); }
									td.RenderBeginTag(writer);
									if (values.TryGetValue(item.Key, out string value)) { td.Write(value).RenderContent(writer); }
									td.RenderEndTag(writer);
								}
							}
							else
							{
								TagHtmlWriter td = new TagHtmlWriter("td");
								td.AddAttribute("field", column.Field);
								if (column.Hidden || column.JsonData) { td.AddAttribute("hidden", column.Hidden.ToString().ToLower()); }
								if (!string.IsNullOrWhiteSpace(column.Align)) { td.AddStyleAttribute("textalign", column.Align); }
								if (column.HasCssClass) { td.AddAttribute("class", column.CssClass); }
								td.RenderBeginTag(writer);
								td.Write(column.GetString(model)).RenderContent(writer);
								td.RenderEndTag(writer);
							}
						}
						tr.RenderEndTag(writer);
					}
				}
				tag.RenderEndTag(writer);
			}
		}

		/// <summary>自定义行输出格式</summary>
		/// <param name="writer">获取或设置用户输出 HTML 对象的 TextWriter 。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		private void RenderRowTemplate(TextWriter writer, IPagination<T> dataSource)
		{
			if (dataSource == null || dataSource.Count == 0) { return; }
			if (this._RenderRowTemplate == null) { return; }
			using (TagHtmlWriter tag = new TagHtmlWriter("tbody"))
			{
				tag.RenderBeginTag(writer);
				foreach (T model in dataSource)
				{
					string rowContent = _RenderRowTemplate.Invoke(model);
					tag.Write(rowContent).RenderContent(writer);
				}
				tag.RenderEndTag(writer);
			}
		}

		/// <summary>
		/// 输出表格主体详细Html代码（Group 汇总）
		/// </summary>
		/// <param name="writer">获取或设置用户输出 HTML 对象的 TextWriter 。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		/// <param name="groupInfo">表格分组信息</param>
		private void RenderGroupBody(TextWriter writer, IPagination<T> dataSource, GroupInfo<T> groupInfo)
		{
			if (dataSource == null || dataSource.Count == 0) { return; }
			if (_DataGrid.Columns.Count == 0) { return; }

			IEnumerable<DataGridColumn<T>> columns = _DataGrid.Columns.GetColumns();
			IEnumerable<IGrouping<dynamic, T>> groupSource = groupInfo.GroupBy(dataSource);
			GroupRowCollection<T> rows = groupInfo.GetRows();
			using (TagHtmlWriter tag = new TagHtmlWriter("tbody"))
			{
				tag.RenderBeginTag(writer);
				foreach (IGrouping<dynamic, T> grouping in groupSource.ToArray())
				{
					#region 输出分组明细数据
					foreach (T model in grouping)
					{
						using (TagHtmlWriter tr = new TagHtmlWriter("tr"))
						{
							tr.RenderBeginTag(writer);
							foreach (DataGridColumn<T> column in columns)
							{
								if (column.AllowToHtml == false) { continue; }
								if (column is DataGridArrayColumn<T>)
								{
									DataGridArrayColumn<T> arrayColumn = column as DataGridArrayColumn<T>;
									IDictionary<string, string> values = arrayColumn.GetStrings(model);
									foreach (var item in arrayColumn.Fields)
									{
										TagHtmlWriter td = new TagHtmlWriter("td");
										td.AddAttribute("field", item.Key);
										if (column.Hidden) { td.AddAttribute("hidden", column.Hidden.ToString().ToLower()); }
										if (!string.IsNullOrWhiteSpace(column.Align)) { td.AddStyleAttribute("textalign", column.Align); }
										if (column.HasCssClass) { td.AddAttribute("class", column.CssClass); }
										td.RenderBeginTag(writer);
										if (values.TryGetValue(item.Key, out string value)) { td.Write(value).RenderContent(writer); }
										td.RenderEndTag(writer);
									}
								}
								else if (column is DataGridArraysColumn<T>)
								{
									DataGridArraysColumn<T> arrayColumn = column as DataGridArraysColumn<T>;
									IDictionary<string, string> values = arrayColumn.GetStrings(model);
									foreach (var item in arrayColumn.Fields)
									{
										TagHtmlWriter td = new TagHtmlWriter("td");
										td.AddAttribute("field", item.Key);
										if (column.Hidden) { td.AddAttribute("hidden", column.Hidden.ToString().ToLower()); }
										if (!string.IsNullOrWhiteSpace(column.Align)) { td.AddStyleAttribute("textalign", column.Align); }
										if (column.HasCssClass) { td.AddAttribute("class", column.CssClass); }
										td.RenderBeginTag(writer);
										if (values.TryGetValue(item.Key, out string value)) { td.Write(value).RenderContent(writer); }
										td.RenderEndTag(writer);
									}
								}
								else
								{
									TagHtmlWriter td = new TagHtmlWriter("td");
									td.AddAttribute("field", column.Field);
									if (column.Hidden || column.JsonData) { td.AddAttribute("hidden", column.Hidden.ToString().ToLower()); }
									if (!string.IsNullOrWhiteSpace(column.Align)) { td.AddStyleAttribute("textalign", column.Align); }
									if (column.HasCssClass) { td.AddAttribute("class", column.CssClass); }
									td.RenderBeginTag(writer);
									td.Write(column.GetString(model)).RenderContent(writer);
									td.RenderEndTag(writer);
								}
							}
							tr.RenderEndTag(writer);
						}
					}
					#endregion
					foreach (GroupRow<T> row in rows)
					{
						using (TagHtmlWriter tr = new TagHtmlWriter("tr"))
						{
							if (!string.IsNullOrWhiteSpace(groupInfo.RowStyle)) { tr.AddAttribute("class", groupInfo.RowStyle); }
							tr.RenderBeginTag(writer);
							foreach (GroupCell<T> cell in row)
							{
								TagHtmlWriter td = new TagHtmlWriter("td");
								if (cell.Rowspan >= 2) { td.AddAttribute("rowspan", Convert.ToString(cell.Rowspan)); }
								if (cell.Colspan >= 2) { td.AddAttribute("colspan", Convert.ToString(cell.Colspan)); }
								if (!string.IsNullOrWhiteSpace(cell.Align)) { td.AddStyleAttribute("textalign", cell.Align); }
								if (cell.HasCssClass == true) { td.AddAttribute("class", cell.CssClass); }
								td.RenderBeginTag(writer);
								td.Write(cell.GetValue(grouping)).RenderContent(writer);
								td.RenderEndTag(writer);
							}
							tr.RenderEndTag(writer);  //end Tr
						}
					}
				}
				tag.RenderEndTag(writer);  //end Tbody
			}
		}

		/// <summary>
		/// 输出表格主体详细Html代码
		/// </summary>
		/// <param name="writer">获取或设置用户输出 HTML 对象的 TextWriter 。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		/// <param name="footerInfo">表示表格页脚信息</param>
		private void RenderFoot(TextWriter writer, IPagination<T> dataSource, FooterInfo<T> footerInfo)
		{
			if (dataSource == null || dataSource.Count == 0) { return; }
			if (footerInfo == null) { return; }
			using (TagHtmlWriter tag = new TagHtmlWriter("tfoot"))
			{
				tag.RenderBeginTag(writer);
				foreach (FooterRow<T> row in footerInfo.GetRows())
				{
					using (TagHtmlWriter tr = new TagHtmlWriter("tr"))
					{
						if (!string.IsNullOrWhiteSpace(footerInfo.RowStyle)) { tr.AddAttribute("class", footerInfo.RowStyle); }
						tr.RenderBeginTag(writer);
						foreach (FooterCell<T> cell in row)
						{
							TagHtmlWriter td = new TagHtmlWriter("td");
							if (cell.HasCssClass == true) { td.AddAttribute("class", cell.CssClass); }
							if (cell.Rowspan >= 2) { td.AddAttribute("rowspan", Convert.ToString(cell.Rowspan)); }
							if (cell.Colspan >= 2) { td.AddAttribute("colspan", Convert.ToString(cell.Colspan)); }
							if (!string.IsNullOrWhiteSpace(cell.Align)) { td.AddStyleAttribute("textalign", cell.Align); }
							td.RenderBeginTag(writer);
							td.Write(cell.GetValue(dataSource)).RenderContent(writer);
							td.RenderEndTag(writer);
						}
						tr.RenderEndTag(writer);  //end Tr
					}
				}
				tag.RenderEndTag(writer);
			}
		}

		/// <summary>下载文件名称。</summary>
		public string FileName { get; set; }

		/// <summary>输出 DataGrid 内容（Html）</summary>
		/// <param name="writer">视图输出</param>
		/// <param name="response">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		public void Render(System.IO.TextWriter writer, ICustomResponse response, IPagination<T> dataSource)
		{
			using (TagHtmlWriter tag = new TagHtmlWriter("table"))
			{
				if (dataSource != null) { _Options.Total = dataSource.Capacity; }
				tag.MergeAttributes(_Options);
				tag.MergeStyles(_Options.StyleOptions);
				tag.RenderBeginTag(writer);
				if (string.IsNullOrWhiteSpace(_DataGrid.Caption) == false)
				{
					using (TagHtmlWriter caption = new TagHtmlWriter("caption"))
					{
						caption.RenderBeginTag(writer);
						caption.Write(_DataGrid.Caption).RenderContent(writer);
						caption.RenderEndTag(writer);
					}
				}
				RenderHeader(writer);

				if (_RenderRowTemplate == null)
				{
					GroupInfo<T> groupInfo = _DataGrid.GetGroupInfo();
					if (groupInfo != null) { RenderGroupBody(writer, dataSource, groupInfo); }
					else { RenderBody(writer, dataSource); }
				}
				else { RenderRowTemplate(writer, dataSource); }


				FooterInfo<T> footerInfo = _DataGrid.GetFooterInfo();
				if (footerInfo != null) { RenderFoot(writer, dataSource, footerInfo); }

				tag.RenderEndTag(writer);
			}
		}

		/// <summary>输出 DataGrid 内容（Html）</summary>
		/// <param name="writer">视图输出</param>
		/// <param name="response">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		public Task RenderAsync(System.IO.TextWriter writer, ICustomResponse response, IPagination<T> dataSource)
		{
			return Task.Run(() =>
			{
				using (TagHtmlWriter tag = new TagHtmlWriter("table"))
				{
					if (dataSource != null) { _Options.Total = dataSource.Capacity; }
					tag.MergeAttributes(_Options);
					tag.MergeStyles(_Options.StyleOptions);
					tag.RenderBeginTag(writer);
					if (string.IsNullOrWhiteSpace(_DataGrid.Caption) == false)
					{
						using (TagHtmlWriter caption = new TagHtmlWriter("caption"))
						{
							caption.RenderBeginTag(writer);
							caption.Write(_DataGrid.Caption).RenderContent(writer);
							caption.RenderEndTag(writer);
						}
					}
					RenderHeader(writer);

					if (_RenderRowTemplate == null)
					{
						GroupInfo<T> groupInfo = _DataGrid.GetGroupInfo();
						if (groupInfo != null) { RenderGroupBody(writer, dataSource, groupInfo); }
						else { RenderBody(writer, dataSource); }
					}
					else { RenderRowTemplate(writer, dataSource); }


					FooterInfo<T> footerInfo = _DataGrid.GetFooterInfo();
					if (footerInfo != null) { RenderFoot(writer, dataSource, footerInfo); }

					tag.RenderEndTag(writer);
				}
			});
		}
	}
}
