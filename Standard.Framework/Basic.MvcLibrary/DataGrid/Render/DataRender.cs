using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Basic.Interfaces;
using System.Linq;
using Basic.MvcLibrary;
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

		/// <summary>
		/// 输出控件类型
		/// </summary>
		/// <param name="context">提供来自 ASP.NET 操作的 HTTP 上下文信息。</param>
		/// <param name="dataSource">绑定表格的数据源。</param>
		public void Render(IBasicContext context, IPagination<T> dataSource)
		{
			context.ClearContent();
			context.ClearHeaders();
			context.ContentType = "application/json";
			WriteData(context.Writer, dataSource);
		}

		private void WriteData(System.IO.TextWriter writer, IPagination<T> dataSource)
		{
			if (dataSource == null || dataSource.Capacity == 0)
			{
				writer.Write("{\"Success\":true,\"total\":0,\"rows\":[] }");
				return;
			}
			IEnumerable<DataGridColumn<T>> columns = _DataGrid.Columns.GetColumns();

			writer.Write("{{ \"Success\":true,\"total\":{0},", dataSource.Capacity);
			writer.Write("\"rows\": [");
			if (columns.Any())
			{
				int index1 = 0; List<string> list = new List<string>(columns.Count() + 5);
				foreach (T model in dataSource)
				{
					writer.Write("{"); index1++; list.Clear();
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

							string resultValue = column.GetString(model);
							if (resultValue == null) { list.Add(string.Concat("\"", column.Field, "\":null")); }
							else { list.Add(string.Concat("\"", column.Field, "\":\"", HttpUtility.JavaScriptStringEncode(resultValue), "\"")); }
						}
						else if (column is DataGridBoolColumn<T>)
						{
							string resultValue = column.GetString(model);
							if (resultValue == null) { list.Add(string.Concat("\"", column.Field, "\":null")); }
							else { list.Add(string.Concat("\"", column.Field, "\":\"", HttpUtility.JavaScriptStringEncode(resultValue), "\"")); }
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
							else if (obj is bool val2)
							{
								list.Add(string.Concat("\"", column.Field, "\":", val2 ? "true" : "false"));
							}
							else if (obj is bool?)
							{
								bool? val3 = obj is bool?;
								if (val3 == null) { list.Add(string.Concat("\"", column.Field, "\":null")); }
								else { list.Add(string.Concat("\"", column.Field, "\":", val3.Value ? "true" : "false")); }
							}
							else
							{
								string resultValue = column.GetString(model);
								if (resultValue == null) { list.Add(string.Concat("\"", column.Field, "\":null")); }
								else { list.Add(string.Concat("\"", column.Field, "\":\"", HttpUtility.JavaScriptStringEncode(resultValue), "\"")); }
							}

							//string resultValue = column.GetString(model);
							//if (resultValue == null) { list.Add(string.Concat("\"", column.Field, "\":null")); }
							//else { list.Add(string.Concat("\"", column.Field, "\":\"", HttpUtility.JavaScriptStringEncode(resultValue), "\"")); }
							//else if (resultValue is string)
							//list.Add(string.Concat("\"", column.Field, "\":\"", HttpUtility.JavaScriptStringEncode((string)resultValue), "\""));
							//else if (resultValue is byte || resultValue is int || resultValue is short || resultValue is long || resultValue is decimal)
							//	list.Add(string.Concat("\"", column.Field, "\":", resultValue));
							//else
							//	list.Add(string.Concat("\"", column.Field, "\":\"", HttpUtility.JavaScriptStringEncode(Convert.ToString(resultValue, CultureInfo.CurrentCulture)), "\""));
						}
					}
					#endregion
					writer.Write(string.Join(",", list.ToArray()));
					if (dataSource.Count > index1)
						writer.WriteLine("},");
					else
						writer.WriteLine("}");
				}
			}
			writer.Write("]");
			FooterInfo<T> footer = _DataGrid.GetFooterInfo();
			if (_Options.ShowFooter && footer != null)
			{
				List<string> list = new List<string>(columns.Count() + 5);
				writer.Write(",\"footer\": ["); int index = 0;
				foreach (FooterRow<T> row in footer.GetRows())
				{
					list.Clear(); index++;
					foreach (FooterCell<T> cell in row)
					{
						if (cell.HasField == false) { continue; }
						string value = Convert.ToString(cell.GetValue(dataSource));
						list.Add(string.Concat("\"", cell.Field, "\":\"", HttpUtility.JavaScriptStringEncode(value), "\""));
					}
					if (index >= 2) { writer.Write(","); }
					writer.Write("{"); writer.Write(string.Join(",", list.ToArray())); writer.Write("}");
				}
				writer.Write("]");
			}
			writer.Write("}");
		}
	}
}
