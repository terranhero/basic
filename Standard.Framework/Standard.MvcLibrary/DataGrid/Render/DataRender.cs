using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Basic.Interfaces;
using System.Linq;
using Basic.MvcLibrary;
using System.Threading.Tasks;
using Basic.EntityLayer;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 输出EasyUI.DataGrid类型的表格
	/// </summary>
	/// <typeparam name="T">模型类型</typeparam>
	public sealed class DataRender<T> : IRender<T> where T : class
	{
		private readonly DataGrid<T> _DataGrid;
		private readonly DataGridOptions _Options;
		/// <summary>
		/// 初始化 JsonRender 类实例
		/// </summary>
		/// <param name="grid">IEasyGrid表格</param>
		internal DataRender(DataGrid<T> grid) { _DataGrid = grid; _Options = grid.Options; }

		/// <summary>
		/// 下载文件名称。
		/// </summary>
		public string FileName { get; set; }

		/// <summary>输出 DataGrid 内容（Excel 2007 Or Later）</summary>
		/// <param name="writer">视图输出</param>
		/// <param name="response">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		public void Render(System.IO.TextWriter writer, ICustomResponse response, IPagination<T> dataSource)
		{
			response.Clear();
			response.Headers.Clear();
			response.ContentType = "application/json";
			WriteData(response, dataSource);
			response.Flush();
		}

		private void WriteData(ICustomResponse response, IPagination<T> dataSource)
		{
			if (dataSource == null || dataSource.Capacity == 0)
			{
				response.Write("{\"Success\":true,\"total\":0,\"rows\":[] }"); return;
			}
			IEnumerable<DataGridColumn<T>> columns = _DataGrid.Columns.GetColumns();

			response.Write(string.Format("{{ \"Success\":true,\"total\":{0},", dataSource.Capacity));
			response.Write("\"rows\": [");
			if (columns.Any())
			{
				int index1 = 0; List<string> list = new List<string>(columns.Count() + 5);
				foreach (T model in dataSource)
				{
					response.Write("{"); index1++; list.Clear();
					#region 输出列
					foreach (DataGridColumn<T> column in columns)
					{
						if (column is DataGridArrayColumn<T>)
						{
							DataGridArrayColumn<T> arrayColumn = column as DataGridArrayColumn<T>;
							IDictionary<string, string> values = arrayColumn.GetStrings(model);
							foreach (var item in values)
							{
								list.Add(string.Concat("\"", item.Key, "\":\"", HttpUtility.JavaScriptStringEncode(item.Value), "\""));
							}
						}
						else if (column is DataGridArraysColumn<T>)
						{
							DataGridArraysColumn<T> arrayColumn = column as DataGridArraysColumn<T>;
							IDictionary<string, string> values = arrayColumn.GetStrings(model);
							foreach (var item in values)
							{
								list.Add(string.Concat("\"", item.Key, "\":\"", HttpUtility.JavaScriptStringEncode(item.Value), "\""));
							}
						}
						else if (column is DataGridEnumColumn<T>)
						{
							object obj = column.GetValue(model);
							if (obj == null) { list.Add(string.Concat("\"", column.Field, "Value\":0")); }
							else { list.Add(string.Concat("\"", column.Field, "Value\":", Convert.ToInt32(obj))); }

							if (obj == null) { list.Add(string.Concat("\"", column.Field, "\":0")); }
							else { list.Add(string.Concat("\"", column.Field, "\":", Convert.ToInt32(obj))); }

							string resultValue = column.GetString(model);
							if (resultValue == null) { list.Add(string.Concat("\"", column.Field, "Text\":null")); }
							else { list.Add(string.Concat("\"", column.Field, "Text\":\"", HttpUtility.JavaScriptStringEncode(resultValue), "\"")); }
						}
						else if (column is DataGridBoolColumn<T>)
						{
							object obj = column.GetValue(model);
							if (obj == null) { list.Add(string.Concat("\"", column.Field, "Value\":0")); }
							else { list.Add(string.Concat("\"", column.Field, "Value\":", Convert.ToInt32(obj))); }

							if (obj == null) { list.Add(string.Concat("\"", column.Field, "\":0")); }
							else { list.Add(string.Concat("\"", column.Field, "\":\"", Convert.ToInt32(obj), "\"")); }

							string bValue = column.GetString(model);
							if (bValue == null) { list.Add(string.Concat("\"", column.Field, "Text\":null")); }
							else { list.Add(string.Concat("\"", column.Field, "Text\":\"", HttpUtility.JavaScriptStringEncode(bValue), "\"")); }
						}
						else
						{
							object obj = column.GetValue(model);
							if (obj == null) { list.Add(string.Concat("\"", column.Field, "\":null")); }
							else if (obj.GetType().IsClass)
							{
								string resultValue = JsonSerializer.SerializeObject(obj, true);
								list.Add(string.Concat("\"", column.Field, "\":", resultValue));
							}
							else
							{
								string resultValue = column.GetString(model);
								if (resultValue == null) { list.Add(string.Concat("\"", column.Field, "\":null")); }
								else { list.Add(string.Concat("\"", column.Field, "\":\"", HttpUtility.JavaScriptStringEncode(resultValue), "\"")); }
							}
						}
					}
					#endregion
					response.Write(string.Join(",", list.ToArray()));
					if (dataSource.Count > index1)
						response.Write("},");
					else
						response.Write("}");
				}
			}
			response.Write("]");
			FooterInfo<T> footer = _DataGrid.GetFooterInfo();
			if (_Options.ShowFooter && footer != null)
			{
				List<string> list = new List<string>(columns.Count() + 5);
				response.Write(",\"footer\": ["); int index = 0;
				foreach (FooterRow<T> row in footer.GetRows())
				{
					list.Clear(); index++;
					foreach (FooterCell<T> cell in row)
					{
						if (cell.HasField == false) { continue; }
						string value = Convert.ToString(cell.GetValue(dataSource));
						list.Add(string.Concat("\"", cell.Field, "\":\"", HttpUtility.JavaScriptStringEncode(value), "\""));
					}
					if (index >= 2) { response.Write(","); }
					response.Write("{");
					response.Write(string.Join(",", list.ToArray()));
					response.Write("}");
				}
				response.Write("]");
			}
			response.Write("}");
		}

		/// <summary>输出 DataGrid 内容（Excel 2007 Or Later）</summary>
		/// <param name="writer">视图输出</param>
		/// <param name="response">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		public async Task RenderAsync(System.IO.TextWriter writer, ICustomResponse response, IPagination<T> dataSource)
		{
			response.Clear();
			response.Headers.Clear();
			response.ContentType = "application/json";
			await WriteDataAsync(response, dataSource);
			await response.FlushAsync();
		}

		private async Task WriteDataAsync(ICustomResponse response, IPagination<T> dataSource)
		{
			if (dataSource == null || dataSource.Capacity == 0)
			{
				await response.WriteAsync("{\"Success\":true,\"total\":0,\"rows\":[] }"); return;
			}
			IEnumerable<DataGridColumn<T>> columns = _DataGrid.Columns.GetColumns();

			await response.WriteAsync(string.Format("{{ \"Success\":true,\"total\":{0},", dataSource.Capacity));
			await response.WriteAsync("\"rows\": [");
			if (columns.Any())
			{
				int index1 = 0; List<string> list = new List<string>(columns.Count() + 5);
				foreach (T model in dataSource)
				{
					await response.WriteAsync("{"); index1++; list.Clear();
					#region 输出列
					foreach (DataGridColumn<T> column in columns)
					{
						if (column is DataGridArrayColumn<T>)
						{
							DataGridArrayColumn<T> arrayColumn = column as DataGridArrayColumn<T>;
							IDictionary<string, string> values = arrayColumn.GetStrings(model);
							foreach (var item in values)
							{
								list.Add(string.Concat("\"", item.Key, "\":\"", HttpUtility.JavaScriptStringEncode(item.Value), "\""));
							}
						}
						else if (column is DataGridArraysColumn<T>)
						{
							DataGridArraysColumn<T> arrayColumn = column as DataGridArraysColumn<T>;
							IDictionary<string, string> values = arrayColumn.GetStrings(model);
							foreach (var item in values)
							{
								list.Add(string.Concat("\"", item.Key, "\":\"", HttpUtility.JavaScriptStringEncode(item.Value), "\""));
							}
						}
						else if (column is DataGridEnumColumn<T>)
						{
							object obj = column.GetValue(model);
							if (obj == null) { list.Add(string.Concat("\"", column.Field, "Value\":0")); }
							else { list.Add(string.Concat("\"", column.Field, "Value\":", Convert.ToInt32(obj))); }

							if (obj == null) { list.Add(string.Concat("\"", column.Field, "\":0")); }
							else { list.Add(string.Concat("\"", column.Field, "\":", Convert.ToInt32(obj))); }

							string resultValue = column.GetString(model);
							if (resultValue == null) { list.Add(string.Concat("\"", column.Field, "Text\":null")); }
							else { list.Add(string.Concat("\"", column.Field, "Text\":\"", HttpUtility.JavaScriptStringEncode(resultValue), "\"")); }
						}
						else if (column is DataGridBoolColumn<T>)
						{
							object obj = column.GetValue(model);
							if (obj == null) { list.Add(string.Concat("\"", column.Field, "Value\":0")); }
							else { list.Add(string.Concat("\"", column.Field, "Value\":", Convert.ToInt32(obj))); }

							if (obj == null) { list.Add(string.Concat("\"", column.Field, "\":0")); }
							else { list.Add(string.Concat("\"", column.Field, "\":\"", Convert.ToInt32(obj), "\"")); }

							string bValue = column.GetString(model);
							if (bValue == null) { list.Add(string.Concat("\"", column.Field, "Text\":null")); }
							else { list.Add(string.Concat("\"", column.Field, "Text\":\"", HttpUtility.JavaScriptStringEncode(bValue), "\"")); }
						}
						else if (column is DataGridJsonColumn<T, bool> jBoolCol)
						{
							bool value = jBoolCol.GetModelValue(model);
							list.Add(string.Concat("\"", column.Field, "Value\":", value ? "true" : "false"));
							list.Add(string.Concat("\"", column.Field, "\":\"", value ? "True" : "False", "\""));
						}
						else
						{
							object obj = column.GetValue(model);
							if (obj.GetType().IsClass)
							{
								string resultValue = JsonSerializer.SerializeObject(obj, true); ;
								if (resultValue == null) { list.Add(string.Concat("\"", column.Field, "\":null")); }
								else { list.Add(string.Concat("\"", column.Field, "\":", HttpUtility.JavaScriptStringEncode(resultValue))); }
							}
							else
							{
								string resultValue = column.GetString(model);
								if (resultValue == null) { list.Add(string.Concat("\"", column.Field, "\":null")); }
								else { list.Add(string.Concat("\"", column.Field, "\":\"", HttpUtility.JavaScriptStringEncode(resultValue), "\"")); }
							}

							//else if (resultValue is string)
							//list.Add(string.Concat("\"", column.Field, "\":\"", HttpUtility.JavaScriptStringEncode((string)resultValue), "\""));
							//else if (resultValue is byte || resultValue is int || resultValue is short || resultValue is long || resultValue is decimal)
							//	list.Add(string.Concat("\"", column.Field, "\":", resultValue));
							//else
							//	list.Add(string.Concat("\"", column.Field, "\":\"", HttpUtility.JavaScriptStringEncode(Convert.ToString(resultValue, CultureInfo.CurrentCulture)), "\""));
						}
					}
					#endregion
					await response.WriteAsync(string.Join(",", list.ToArray()));
					if (dataSource.Count > index1)
						await response.WriteAsync("},");
					else
						await response.WriteAsync("}");
				}
			}
			await response.WriteAsync("]");
			FooterInfo<T> footer = _DataGrid.GetFooterInfo();
			if (_Options.ShowFooter && footer != null)
			{
				List<string> list = new List<string>(columns.Count() + 5);
				await response.WriteAsync(",\"footer\": ["); int index = 0;
				foreach (FooterRow<T> row in footer.GetRows())
				{
					list.Clear(); index++;
					foreach (FooterCell<T> cell in row)
					{
						if (cell.HasField == false) { continue; }
						string value = Convert.ToString(cell.GetValue(dataSource));
						list.Add(string.Concat("\"", cell.Field, "\":\"", HttpUtility.JavaScriptStringEncode(value), "\""));
					}
					if (index >= 2) { await response.WriteAsync(","); }
					await response.WriteAsync("{");
					await response.WriteAsync(string.Join(",", list.ToArray()));
					await response.WriteAsync("}");
				}
				await response.WriteAsync("]");
			}
			await response.WriteAsync("}");
		}
	}
}
