using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Basic.Interfaces;
using Basic.MvcLibrary;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Basic.EasyLibrary
{
	/// <summary>输出EasyUI.DataGrid类型的表格</summary>
	/// <typeparam name="T"></typeparam>
	internal class HtmlRender<T> : IRender<T> where T : class
	{
		private const string _DefaultWidth = "80";
		private readonly DataGrid<T> mGrid;
		private readonly Func<T, string> mRenderRowTemplate;
		private readonly DataGridOptions mOptions;

		/// <summary>初始化 HtmlRender 类实例</summary>
		/// <param name="grid">IEasyGrid表格</param>
		internal HtmlRender(DataGrid<T> grid) { mGrid = grid; mOptions = grid.Options; mRenderRowTemplate = grid.RenderRowTemplate; }

		/// <summary>输出表格头列信息</summary>
		/// <param name="writer">接收EasyUI.DataGrid内容的 HtmlTextWriter 对象。</param>
		/// <param name="rows">表示需要输出的列头信息</param>
		private void RenderHeader(TextWriter writer, DataGridRowCollection<T> rows)
		{
			if (mOptions.ShowHeader == false) { return; }
			if (rows.HasNoneFrozenColumns() == false) { return; }
			using (TagHtmlWriter thead = new TagHtmlWriter("thead"))
			{
				thead.RenderBeginTag(writer);

				foreach (DataGridRow<T> row in rows)
				{
					using (TagHtmlWriter tr = new TagHtmlWriter("tr"))
					{
						if (row.HasNoneFrozenColumns() == false) { return; }
						if (row.HasCssClass == true) { tr.AddAttribute("class", row.CssClass); }
						if (row.Height.IsEmpty == false) { tr.AddStyleAttribute("height", row.Height.ToString()); }
						tr.RenderBeginTag(writer);
						foreach (DataGridColumn<T> col in row)
						{
							if (col.Frozen == true) { continue; }
							if (col.AllowToHtml == false) { continue; }
							if (col is DataGridArrayColumn<T>)
							{
								DataGridArrayColumn<T> arrayColumn = col as DataGridArrayColumn<T>;
								foreach (var item in arrayColumn.Fields)
								{
									TagHtmlWriter td = new TagHtmlWriter("th");
									td.AddAttribute("field", item.Key);

									if (col.Width.IsEmpty == false) { td.AddAttribute("width", col.Width.ToString()); }
									else { td.AddAttribute("width", _DefaultWidth); }

									if (!string.IsNullOrWhiteSpace(col.Align)) { td.AddAttribute("align", col.Align); }
									if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { td.AddAttribute("halign", col.HeaderAlign); }

									if (col.Sortable) { td.AddAttribute("sortable", col.Sortable.ToString().ToLower()); }
									if (col.Resizable) { td.AddAttribute("resizable", col.Resizable.ToString().ToLower()); }
									if (col.Hidden) { td.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }
									if (col.Checkbox) { td.AddAttribute("checkbox", col.Checkbox.ToString().ToLower()); }
									if (col.HasCssClass) { td.AddAttribute("class", col.CssClass); }
									if (string.IsNullOrWhiteSpace(col.Formatter) == false) { td.AddAttribute("formatter", col.Formatter); }

									td.RenderBeginTag(writer);
									td.Write(item.Value).RenderContent(writer);
									td.RenderEndTag(writer);
								}
							}
							else if (col is DataGridArraysColumn<T>)
							{
								DataGridArraysColumn<T> arrayColumn = col as DataGridArraysColumn<T>;
								foreach (var item in arrayColumn.Fields)
								{
									foreach (var keyValue in item.Value)
									{
										TagHtmlWriter td = new TagHtmlWriter("th");
										td.AddAttribute("field", keyValue.Key);

										if (col.Width.IsEmpty == false) { td.AddAttribute("width", col.Width.ToString()); }
										else { td.AddAttribute("width", _DefaultWidth); }

										if (!string.IsNullOrWhiteSpace(col.Align)) { td.AddAttribute("align", col.Align); }
										if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { td.AddAttribute("halign", col.HeaderAlign); }

										if (col.Sortable) { td.AddAttribute("sortable", col.Sortable.ToString().ToLower()); }
										if (col.Resizable) { td.AddAttribute("resizable", col.Resizable.ToString().ToLower()); }
										if (col.Hidden) { td.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }
										if (col.Checkbox) { td.AddAttribute("checkbox", col.Checkbox.ToString().ToLower()); }
										if (col.HasCssClass) { td.AddAttribute("class", col.CssClass); }
										if (string.IsNullOrWhiteSpace(col.Formatter) == false) { td.AddAttribute("formatter", col.Formatter); }

										td.RenderBeginTag(writer);
										td.Write(keyValue.Value).RenderContent(writer);
										td.RenderEndTag(writer);
									}
								}
							}
							else if (col is DataGridHeaderColumn<T>)
							{
								TagHtmlWriter th = new TagHtmlWriter("th");
								if (col.Width.IsEmpty == false) { th.AddAttribute("width", col.Width.ToString()); }
								else { th.AddAttribute("width", _DefaultWidth); }
								if (col.Rowspan > 1) { th.AddAttribute("rowspan", col.Rowspan.ToString()); }
								if (col.Colspan > 1) { th.AddAttribute("colspan", col.Colspan.ToString()); }

								if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { th.AddStyleAttribute("textalign", col.HeaderAlign); }
								if (col.Hidden) { th.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }

								if (col.HasCssClass) { th.AddAttribute("class", col.CssClass); }

								th.RenderBeginTag(writer);
								if (!string.IsNullOrEmpty(col.Title)) { th.Write(col.Title).RenderContent(writer); }
								else { th.Write(col.Field).RenderContent(writer); }
								th.RenderEndTag(writer);
							}
							else if (col is DataGridEnumColumn<T>)
							{
								TagHtmlWriter th = new TagHtmlWriter("th");
								if (!string.IsNullOrWhiteSpace(col.Field)) { th.AddAttribute("field", col.Field + "Text"); }

								if (col.Width.IsEmpty == false) { th.AddAttribute("width", col.Width.ToString()); }
								else { th.AddAttribute("width", _DefaultWidth); }

								if (col.Rowspan > 1) { th.AddAttribute("rowspan", col.Rowspan.ToString()); }
								if (col.Colspan > 1) { th.AddAttribute("colspan", col.Colspan.ToString()); }
								if (!string.IsNullOrWhiteSpace(col.Align)) { th.AddAttribute("align", col.Align); }
								if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { th.AddAttribute("halign", col.HeaderAlign); }

								if (col.Fixed == true) { th.AddAttribute("fixed", col.Fixed.ToString().ToLower()); }
								if (col.Sortable) { th.AddAttribute("sortable", col.Sortable.ToString().ToLower()); }
								if (col.Resizable) { th.AddAttribute("resizable", col.Resizable.ToString().ToLower()); }
								if (col.Hidden || col.JsonData) { th.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }
								if (col.Checkbox) { th.AddAttribute("checkbox", col.Checkbox.ToString().ToLower()); }
								if (col.HasCssClass) { th.AddAttribute("class", col.CssClass); }
								if (string.IsNullOrWhiteSpace(col.Formatter) == false) { th.AddAttribute("formatter", col.Formatter); }

								th.RenderBeginTag(writer);
								if (!string.IsNullOrEmpty(col.Title)) { th.Write(col.Title).RenderContent(writer); }
								else { th.Write(col.Field).RenderContent(writer); }
								th.RenderEndTag(writer);
							}
							else if (col is DataGridBooleanColumn<T> || col is DataGridNullableBooleanColumn<T>)
							{
								TagHtmlWriter th = new TagHtmlWriter("th");
								if (!string.IsNullOrWhiteSpace(col.Field)) { th.AddAttribute("field", col.Field + "Text"); }

								if (col.Width.IsEmpty == false) { th.AddAttribute("width", col.Width.ToString()); }
								else { th.AddAttribute("width", _DefaultWidth); }

								if (col.Rowspan > 1) { th.AddAttribute("rowspan", col.Rowspan.ToString()); }
								if (col.Colspan > 1) { th.AddAttribute("colspan", col.Colspan.ToString()); }
								if (!string.IsNullOrWhiteSpace(col.Align)) { th.AddAttribute("align", col.Align); }
								if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { th.AddAttribute("halign", col.HeaderAlign); }

								if (col.Fixed == true) { th.AddAttribute("fixed", col.Fixed.ToString().ToLower()); }
								if (col.Sortable) { th.AddAttribute("sortable", col.Sortable.ToString().ToLower()); }
								if (col.Resizable) { th.AddAttribute("resizable", col.Resizable.ToString().ToLower()); }
								if (col.Hidden || col.JsonData) { th.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }
								if (col.Checkbox) { th.AddAttribute("checkbox", col.Checkbox.ToString().ToLower()); }
								if (col.HasCssClass) { th.AddAttribute("class", col.CssClass); }
								if (string.IsNullOrWhiteSpace(col.Formatter) == false) { th.AddAttribute("formatter", col.Formatter); }

								th.RenderBeginTag(writer);
								if (!string.IsNullOrEmpty(col.Title)) { th.Write(col.Title).RenderContent(writer); }
								else { th.Write(col.Field).RenderContent(writer); }
								th.RenderEndTag(writer);
							}
							else
							{
								TagHtmlWriter th = new TagHtmlWriter("th");
								if (!string.IsNullOrWhiteSpace(col.Field)) { th.AddAttribute("field", col.Field); }

								if (col.Width.IsEmpty == false) { th.AddAttribute("width", col.Width.ToString()); }
								else { th.AddAttribute("width", _DefaultWidth); }

								if (col.Rowspan > 1) { th.AddAttribute("rowspan", col.Rowspan.ToString()); }
								if (col.Colspan > 1) { th.AddAttribute("colspan", col.Colspan.ToString()); }
								if (!string.IsNullOrWhiteSpace(col.Align)) { th.AddAttribute("align", col.Align); }
								if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { th.AddAttribute("halign", col.HeaderAlign); }

								if (col.Fixed == true) { th.AddAttribute("fixed", col.Fixed.ToString().ToLower()); }
								if (col.Sortable) { th.AddAttribute("sortable", col.Sortable.ToString().ToLower()); }
								if (col.Resizable) { th.AddAttribute("resizable", col.Resizable.ToString().ToLower()); }
								if (col.Hidden || col.JsonData) { th.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }
								if (col.Checkbox) { th.AddAttribute("checkbox", col.Checkbox.ToString().ToLower()); }
								if (col.HasCssClass) { th.AddAttribute("class", col.CssClass); }
								if (string.IsNullOrWhiteSpace(col.Formatter) == false) { th.AddAttribute("formatter", col.Formatter); }

								th.RenderBeginTag(writer);
								if (!string.IsNullOrEmpty(col.Title)) { th.Write(col.Title).RenderContent(writer); }
								else { th.Write(col.Field).RenderContent(writer); }
								th.RenderEndTag(writer);
							}
						}
						tr.RenderEndTag(writer);
					}
				}
				thead.RenderEndTag(writer);
			}
		}

		/// <summary>输出表格头列信息</summary>
		/// <param name="writer">接收EasyUI.DataGrid内容的 HtmlTextWriter 对象。</param>
		/// <param name="rows">表示需要输出的列头信息</param>
		private void RenderFrozenHeader(TextWriter writer, DataGridRowCollection<T> rows)
		{
			if (mOptions.ShowHeader == false) { return; }
			if (rows.HasFrozenColumns() == false) { return; }
			using (TagHtmlWriter thead = new TagHtmlWriter("thead"))
			{
				thead.AddAttribute("data-options", "frozen:true");
				thead.RenderBeginTag(writer);
				foreach (DataGridRow<T> row in rows)
				{
					using (TagHtmlWriter tr = new TagHtmlWriter("tr"))
					{
						if (row.HasFrozenColumns() == false) { continue; }
						if (row.HasCssClass == true) { tr.AddAttribute("class", row.CssClass); }
						if (row.Height.IsEmpty == false) { tr.AddStyleAttribute("height", row.Height.ToString()); }
						tr.RenderBeginTag(writer);
						foreach (DataGridColumn<T> col in row)
						{
							if (col.Frozen == false) { continue; }
							if (col.AllowToHtml == false) { continue; }
							if (col is DataGridArrayColumn<T>)
							{
								DataGridArrayColumn<T> arrayColumn = col as DataGridArrayColumn<T>;
								foreach (var item in arrayColumn.Fields)
								{
									TagHtmlWriter th = new TagHtmlWriter("th");
									th.AddAttribute("field", item.Key);

									if (col.Width.IsEmpty == false) { th.AddAttribute("width", col.Width.ToString()); }
									else { th.AddAttribute("width", _DefaultWidth); }

									if (!string.IsNullOrWhiteSpace(col.Align)) { th.AddAttribute("align", col.Align); }
									if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { th.AddAttribute("halign", col.HeaderAlign); }

									if (col.Sortable) { th.AddAttribute("sortable", col.Sortable.ToString().ToLower()); }
									if (col.Resizable) { th.AddAttribute("resizable", col.Resizable.ToString().ToLower()); }
									if (col.Hidden) { th.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }
									if (col.Checkbox) { th.AddAttribute("checkbox", col.Checkbox.ToString().ToLower()); }
									if (col.HasCssClass) { th.AddAttribute("class", col.CssClass); }
									if (string.IsNullOrWhiteSpace(col.Formatter) == false) { th.AddAttribute("formatter", col.Formatter); }

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
										th.AddAttribute("field", keyValue.Key);

										if (col.Width.IsEmpty == false) { th.AddAttribute("width", col.Width.ToString()); }
										else { th.AddAttribute("width", _DefaultWidth); }

										if (!string.IsNullOrWhiteSpace(col.Align)) { th.AddAttribute("align", col.Align); }
										if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { th.AddAttribute("halign", col.HeaderAlign); }

										if (col.Sortable) { th.AddAttribute("sortable", col.Sortable.ToString().ToLower()); }
										if (col.Resizable) { th.AddAttribute("resizable", col.Resizable.ToString().ToLower()); }
										if (col.Hidden) { th.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }
										if (col.Checkbox) { th.AddAttribute("checkbox", col.Checkbox.ToString().ToLower()); }
										if (col.HasCssClass) { th.AddAttribute("class", col.CssClass); }
										if (string.IsNullOrWhiteSpace(col.Formatter) == false) { th.AddAttribute("formatter", col.Formatter); }

										th.RenderBeginTag(writer);
										th.Write(keyValue.Value).RenderContent(writer);
										th.RenderEndTag(writer);
									}
								}
							}
							else if (col is DataGridHeaderColumn<T>)
							{
								TagHtmlWriter th = new TagHtmlWriter("th");
								if (col.Width.IsEmpty == false) { th.AddAttribute("width", col.Width.ToString()); }
								else { th.AddAttribute("width", _DefaultWidth); }
								if (col.Rowspan > 1) { th.AddAttribute("rowspan", col.Rowspan.ToString()); }
								if (col.Colspan > 1) { th.AddAttribute("colspan", col.Colspan.ToString()); }

								if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { th.AddStyleAttribute("textalign", col.HeaderAlign); }
								if (col.Hidden) { th.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }

								if (col.HasCssClass) { th.AddAttribute("class", col.CssClass); }

								th.RenderBeginTag(writer);
								if (!string.IsNullOrEmpty(col.Title)) { th.Write(col.Title).RenderContent(writer); }
								else { th.Write(col.Field).RenderContent(writer); }
								th.RenderEndTag(writer);
							}
							else
							{
								TagHtmlWriter th = new TagHtmlWriter("th");
								if (!string.IsNullOrWhiteSpace(col.Field)) { th.AddAttribute("field", col.Field); }

								if (col.Width.IsEmpty == false) { th.AddAttribute("width", col.Width.ToString()); }
								else { th.AddAttribute("width", _DefaultWidth); }

								if (col.Rowspan > 1) { th.AddAttribute("rowspan", col.Rowspan.ToString()); }
								if (col.Colspan > 1) { th.AddAttribute("colspan", col.Colspan.ToString()); }
								if (!string.IsNullOrWhiteSpace(col.Align)) { th.AddAttribute("align", col.Align); }
								if (!string.IsNullOrWhiteSpace(col.HeaderAlign)) { th.AddAttribute("halign", col.HeaderAlign); }

								if (col.Fixed == true) { th.AddAttribute("fixed", col.Fixed.ToString().ToLower()); }
								if (col.Sortable) { th.AddAttribute("sortable", col.Sortable.ToString().ToLower()); }
								if (col.Resizable) { th.AddAttribute("resizable", col.Resizable.ToString().ToLower()); }
								if (col.Hidden || col.JsonData) { th.AddAttribute("hidden", col.Hidden.ToString().ToLower()); }
								if (col.Checkbox) { th.AddAttribute("checkbox", col.Checkbox.ToString().ToLower()); }
								if (col.HasCssClass) { th.AddAttribute("class", col.CssClass); }
								if (string.IsNullOrWhiteSpace(col.Formatter) == false) { th.AddAttribute("formatter", col.Formatter); }

								th.RenderBeginTag(writer);
								if (!string.IsNullOrEmpty(col.Title)) { th.Write(col.Title).RenderContent(writer); }
								else { th.Write(col.Field).RenderContent(writer); }
								th.RenderEndTag(writer);
							}
						}
						tr.RenderEndTag(writer);
					}
				}
				thead.RenderEndTag(writer);
			}
		}

		/// <summary>输出表格主体详细Html代码</summary>
		/// <param name="writer">获取或设置用户输出 HTML 对象的 TextWriter 。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		private void RenderBody(TextWriter writer, IPagination<T> dataSource)
		{
			if (dataSource == null || dataSource.Count == 0) { return; }
			if (mGrid.Columns.Count == 0) { return; }
			IEnumerable<DataGridColumn<T>> columns = mGrid.Columns.GetColumns();
			NumberColumn<T>[] numbers = columns.Where(m => m is NumberColumn<T>).Cast<NumberColumn<T>>().ToArray();
			int beginNumber = (dataSource.PageIndex - 1) * dataSource.PageSize + 1;
			Array.ForEach(numbers, m => m.CurrentNumber = beginNumber);
			using (TagHtmlWriter tbody = new TagHtmlWriter("tbody"))
			{
				tbody.RenderBeginTag(writer);
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
									td.RenderBeginTag(writer);
									if (values.ContainsKey(item.Key))
									{
										td.Write(values[item.Key]).RenderContent(writer);
									}
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
									td.RenderBeginTag(writer);
									if (values.ContainsKey(item.Key))
									{
										td.Write(values[item.Key]).RenderContent(writer);
									}
									td.RenderEndTag(writer);
								}
							}
							else
							{
								TagHtmlWriter td = new TagHtmlWriter("td");
								td.AddAttribute("field", column.Field);
								if (column.Hidden || column.JsonData) { td.AddAttribute("hidden", column.Hidden.ToString().ToLower()); }
								td.RenderBeginTag(writer);
								td.Write(column.GetString(model)).RenderContent(writer);
								td.RenderEndTag(writer);
							}
						}
						tr.RenderEndTag(writer);
					}
				}
				tbody.RenderEndTag(writer);
			}
		}

		/// <summary>自定义行输出格式</summary>
		/// <param name="writer">获取或设置用户输出 HTML 对象的 TextWriter 。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		private void RenderRowTemplate(TextWriter writer, IPagination<T> dataSource)
		{
			if (dataSource == null || dataSource.Count == 0) { return; }
			if (this.mRenderRowTemplate == null) { return; }
			using (TagHtmlWriter tr = new TagHtmlWriter("tbody"))
			{
				tr.RenderBeginTag(writer);
				foreach (T model in dataSource)
				{
					string rowContent = mRenderRowTemplate.Invoke(model);
					tr.Write(rowContent).RenderContent(writer);
				}
				tr.RenderEndTag(writer);
			}
		}

		/// <summary>输出表格主体详细Html代码（Group 汇总）</summary>
		/// <param name="writer">获取或设置用户输出 HTML 对象的 TextWriter 。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		/// <param name="groupInfo">表格分组信息</param>
		private void RenderGroupBody(TextWriter writer, IPagination<T> dataSource, GroupInfo<T> groupInfo)
		{
			if (dataSource == null || dataSource.Count == 0) { return; }
			if (mGrid.Columns.Count == 0) { return; }

			IEnumerable<DataGridColumn<T>> columns = mGrid.Columns.GetColumns();
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
										using (TagHtmlWriter td = new TagHtmlWriter("td"))
										{
											td.MergeAttribute("field", item.Key);
											if (column.Hidden) { td.MergeAttribute("hidden", column.Hidden.ToString().ToLower()); }
											td.RenderBeginTag(writer);
											if (values.ContainsKey(item.Key))
											{
												td.SetInnerText(values[item.Key]).RenderContent(writer);
											}
											td.RenderEndTag(writer);
										}
									}
								}
								else if (column is DataGridArraysColumn<T>)
								{
									DataGridArraysColumn<T> arrayColumn = column as DataGridArraysColumn<T>;
									IDictionary<string, string> values = arrayColumn.GetStrings(model);
									foreach (var item in arrayColumn.Fields)
									{
										using (TagHtmlWriter td = new TagHtmlWriter("td"))
										{
											td.MergeAttribute("field", item.Key);
											if (column.Hidden) { td.MergeAttribute("hidden", column.Hidden.ToString().ToLower()); }
											td.RenderBeginTag(writer);
											if (values.ContainsKey(item.Key))
											{
												td.Write(values[item.Key]).RenderContent(writer);
											}
											td.RenderEndTag(writer);
										}
									}
								}
								else
								{
									using (TagHtmlWriter td = new TagHtmlWriter("td"))
									{
										td.MergeAttribute("field", column.Field);
										if (column.Hidden || column.JsonData) { td.MergeAttribute("hidden", column.Hidden.ToString().ToLower()); }
										td.RenderBeginTag(writer);
										td.Write(column.GetString(model)).RenderContent(writer);
										td.RenderEndTag(writer);
									}
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
							if (!string.IsNullOrWhiteSpace(groupInfo.RowStyle)) { tr.MergeAttribute("class", groupInfo.RowStyle); }
							tr.RenderBeginTag(writer);
							foreach (GroupCell<T> cell in row)
							{
								using (TagHtmlWriter td = new TagHtmlWriter("td"))
								{
									if (cell.Rowspan >= 2) { td.MergeAttribute("rowspan", Convert.ToString(cell.Rowspan)); }
									if (cell.Colspan >= 2) { td.MergeAttribute("colspan", Convert.ToString(cell.Colspan)); }
									if (string.IsNullOrWhiteSpace(cell.Align)) { td.MergeAttribute("align", cell.Align); }
									td.RenderBeginTag(writer);
									td.SetInnerText(cell.GetValue(grouping)).RenderContent(writer);
									td.RenderEndTag(writer);
								}
							}
							tr.RenderEndTag(writer);  //end Tr
						}
					}
				}
				tag.RenderEndTag(writer);  //end Tbody
			}

		}

		/// <summary>输出表格主体详细Html代码</summary>
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
					if (!string.IsNullOrWhiteSpace(footerInfo.RowStyle))
						tag.MergeAttribute("class", footerInfo.RowStyle);

					using (TagHtmlWriter trTag = new TagHtmlWriter("tr"))
					{
						trTag.RenderBeginTag(writer);
						foreach (FooterCell<T> cell in row)
						{
							using (TagHtmlWriter td = new TagHtmlWriter("td"))
							{
								if (cell.Rowspan >= 2) { td.MergeAttribute("rowspan", Convert.ToString(cell.Rowspan)); }
								if (cell.Colspan >= 2) { td.MergeAttribute("colspan", Convert.ToString(cell.Colspan)); }
								if (string.IsNullOrWhiteSpace(cell.Align)) { td.MergeAttribute("align", cell.Align); }
								if (cell.HasCssClass == true) { td.MergeAttribute("class", cell.CssClass); }
								td.RenderBeginTag(writer);
								td.SetInnerText(cell.GetValue(dataSource)).RenderContent(writer);
								td.RenderEndTag(writer);
							}
						}
						trTag.RenderEndTag(writer);  //end Tr
					}

				}
				tag.RenderEndTag(writer);
			}
		}

		/// <summary>下载文件名称。</summary>
		public string FileName { get; set; }

		/// <summary>输出 DataGrid 内容（Excel 2007 Or Later）</summary>
		/// <param name="writer">视图输出</param>
		/// <param name="response">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		public void Render(System.IO.TextWriter writer, ICustomResponse response, IPagination<T> dataSource)
		{
			using (TagHtmlWriter builder = new TagHtmlWriter("table"))
			{
				if (dataSource != null) { mOptions.Total = dataSource.Capacity; }
				builder.MergeAttributes(mOptions);
				builder.MergeStyles(mOptions.StyleOptions);
				builder.MergeAttribute("data-options", mOptions.GetOptions);
				builder.RenderBeginTag(writer);

				if (string.IsNullOrWhiteSpace(mGrid.Caption) == false)
				{
					using (TagHtmlWriter caption = new TagHtmlWriter("caption"))
					{
						caption.RenderBeginTag(writer);
						caption.SetInnerText(mGrid.Caption).RenderContent(writer);
						caption.RenderEndTag(writer);
					}
				}

				RenderFrozenHeader(writer, mGrid.Columns);
				RenderHeader(writer, mGrid.Columns);

				if (mRenderRowTemplate == null)
				{
					GroupInfo<T> groupInfo = mGrid.GetGroupInfo();
					if (groupInfo != null) { RenderGroupBody(writer, dataSource, groupInfo); }
					else { RenderBody(writer, dataSource); }
				}
				else { RenderRowTemplate(writer, dataSource); }

				FooterInfo<T> footerInfo = mGrid.GetFooterInfo();
				if (footerInfo != null) { RenderFoot(writer, dataSource, footerInfo); }

				builder.RenderEndTag(writer);
			}
		}

		/// <summary>输出 DataGrid 内容（Excel 2007 Or Later）</summary>
		/// <param name="writer">视图输出</param>
		/// <param name="response">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		public Task RenderAsync(System.IO.TextWriter writer, ICustomResponse response, IPagination<T> dataSource)
		{
			return Task.Run(() =>
			{
				//TagBuilder table = new TagBuilder("table");
				//table.MergeAttributes(mOptions);
				//table.MergeAttributes(mOptions.StyleOptions);
				//table.MergeAttribute("data-options", mOptions.GetOptions);

				//TagBuilder caption = new TagBuilder("caption");
				//caption.InnerHtml.Append(mGrid.Caption);
				//table.InnerHtml.AppendHtml(caption);

				//table.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
				using (TagHtmlWriter builder = new TagHtmlWriter("table"))
				{
					if (dataSource != null) { mOptions.Total = dataSource.Capacity; }
					builder.MergeAttributes(mOptions);
					builder.MergeStyles(mOptions.StyleOptions);
					builder.MergeAttribute("data-options", mOptions.GetOptions);
					builder.RenderBeginTag(writer);

					if (string.IsNullOrWhiteSpace(mGrid.Caption) == false)
					{
						using (TagHtmlWriter caption = new TagHtmlWriter("caption"))
						{
							caption.RenderBeginTag(writer);
							caption.SetInnerText(mGrid.Caption).RenderContent(writer);
							caption.RenderEndTag(writer);
						}
					}

					RenderFrozenHeader(writer, mGrid.Columns);
					RenderHeader(writer, mGrid.Columns);

					if (mRenderRowTemplate == null)
					{
						GroupInfo<T> groupInfo = mGrid.GetGroupInfo();
						if (groupInfo != null) { RenderGroupBody(writer, dataSource, groupInfo); }
						else { RenderBody(writer, dataSource); }
					}
					else { RenderRowTemplate(writer, dataSource); }

					FooterInfo<T> footerInfo = mGrid.GetFooterInfo();
					if (footerInfo != null) { RenderFoot(writer, dataSource, footerInfo); }

					builder.RenderEndTag(writer);
				}
			});
		}
	}
}
