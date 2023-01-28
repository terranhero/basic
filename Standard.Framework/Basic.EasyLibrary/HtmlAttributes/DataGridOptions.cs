using System;
using System.Collections.Generic;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 EasyGrid 属性定义
	/// </summary>
	public class DataGridOptions : EasyOptions, IHtmlAttrs
	{
		/// <summary>
		///  初始化 EasyGridOptions 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		/// <param name="useDefault">
		/// 是否使用默认值初始化 DataGridOptions 类实例。
		/// 如果为true 则使用默认值，如果为 false 则不实用默认值。默认使用默认值。
		/// </param>
		public DataGridOptions(bool useDefault = true) : this("easyui-gridpage", useDefault) { }

		/// <summary>
		///  使用样式名称初始化 EasyGridOptions 类实例。
		/// </summary>
		/// <param name="cssClass">表示当前EasyGrid的样式名称。</param>
		/// <param name="useDefault">
		/// 是否使用默认值初始化 DataGridOptions 类实例。
		/// 如果为true 则使用默认值，如果为 false 则不实用默认值。默认使用默认值。
		/// </param>
		public DataGridOptions(string cssClass, bool useDefault = true)
			: base()
		{
			if (useDefault) { Pagination = true; Fit = true; Rownumbers = true; Border = false; }
			className = cssClass;
		}

		/// <summary>
		/// 初始化 EasyGridOptions 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;string,object&gt;
		/// 中复制的元素并为键类型使用默认的相等比较器。
		/// </summary>
		/// <param name="dictionary">System.Collections.Generic.IDictionary&lt;string,object&gt;，它的元素被复制到新的 HtmlAttributes中。</param>
		/// <param name="useDefault">
		/// 是否使用默认值初始化 DataGridOptions 类实例。
		/// 如果为true 则使用默认值，如果为 false 则不实用默认值。默认使用默认值。
		/// </param>
		/// <exception cref="System.ArgumentNullException">dictionary 为 null。</exception>
		/// <exception cref="System.ArgumentException">dictionary 包含一个或多个重复键。</exception>
		public DataGridOptions(IDictionary<string, object> dictionary, bool useDefault = true)
			: base(dictionary)
		{
			if (useDefault) { Pagination = true; Fit = true; Rownumbers = true; Border = false; }
			className = "easyui-gridpage";
		}

		/// <summary>
		/// 获取或设置 IEasyGrid 在客户端是否允许导出。
		/// </summary>
		/// <value>default value is false</value>
		public bool AllowExport { set { base.DataOptions["allowExport"] = value ? "true" : "false"; } }

		/// <summary>
		/// 获取或设置DataGrid控件标题
		/// </summary>
		public string Title { set { base.DataOptions["title"] = string.Concat("'", value, "'"); } }

		/// <summary>
		/// 记录总数
		/// </summary>
		public int Total { set { base["total"] = value; } }

		/// <summary>
		/// 控件宽度
		/// </summary>
		public int Width { set { base.StyleOptions["width"] = string.Concat("'", value, "'px"); } }

		/// <summary>
		/// 控件高度
		/// </summary>
		public int Height { set { base.StyleOptions["height"] = string.Concat("'", value, "'px"); } }

		/// <summary>
		/// 是否需要填充满屏
		/// </summary>
		public bool Fit { set { base.DataOptions["fit"] = value ? "true" : "false"; } }

		/// <summary>
		/// 是否显示边框
		/// </summary>
		public bool Border { set { base.DataOptions["border"] = value ? "true" : "false"; } }

		/// <summary>
		/// 真正的自动展开/收缩列的大小，以适应网格的宽度，防止水平滚动。
		/// </summary>
		public bool FitColumns { set { base.DataOptions["fitColumns"] = value ? "true" : "false"; } }

		private bool _ShowHeader = true, _ShowFooter = false;
		///// <summary>是否以报表模式显示表格数据。</summary>
		///// <value>default value is POST</value>
		//public bool IsReport { set { base.DataOptions["Report"] = value ? "true" : "false"; _Report = value; } get { return _Report; } }

		/// <summary>定义是否显示列头。</summary>
		/// <value>默认值为 true</value>
		public bool ShowHeader { set { base.DataOptions["showHeader"] = value ? "true" : "false"; _ShowHeader = value; } get { return _ShowHeader; } }

		/// <summary>定义是否显示行脚。</summary>
		/// <value>默认值为 false。</value>
		public bool ShowFooter { set { base.DataOptions["showFooter"] = value ? "true" : "false"; _ShowFooter = value; } get { return _ShowFooter; } }

		/// <summary>
		/// 该方法类型请求远程数据。
		/// </summary>
		/// <value>default value is POST</value>
		public string Method { set { base.DataOptions["method"] = string.Concat("'", value, "'"); } }

		/// <summary>
		/// 如果为true，则在同一行中显示数据。设置为true可以提高加载性能。
		/// </summary>
		/// <value>default value is true</value>
		public bool Nowrap { set { base.DataOptions["nowrap"] = value ? "true" : "false"; } }

		/// <summary>
		/// 指明哪一个字段是标识字段。
		/// </summary>
		/// <value>default value is null</value>
		internal string IdField { set { base.DataOptions["idField"] = string.Concat("'", value, "'"); } }

		/// <summary>
		/// 获取或设置数据库表的时间戳字段(TimeStamp)。
		/// </summary>v
		/// <value>默认值为null</value>
		internal string tsField { set { base.DataOptions["tsField"] = string.Concat("'", value, "'"); } }

		/// <summary>
		/// 获取或设置数据库表额外关键字字段。
		/// </summary>v
		/// <value>默认值为null</value>
		public string[] opkField { set { base.DataOptions["opkField"] = string.Concat("['", string.Join("','", value), "']"); } }

		/// <summary>
		/// 一个URL从远程站点请求数据。
		/// </summary>
		/// <value>default value is null</value>
		internal string Url { set { base.DataOptions["url"] = string.Concat("'", value, "'"); } }

		/// <summary>
		/// 如果为true，则在DataGrid控件底部显示分页工具栏。
		/// </summary>
		/// <value>default value is false</value>
		public bool Pagination { set { base.DataOptions["pagination"] = value ? "true" : "false"; } }

		private bool _RowNumbers = false, _Striped = false, _AutoRowHeight = true;
		/// <summary>如果为true，则显示一个行号列。</summary>
		/// <value>default value is false</value>
		public bool Rownumbers { set { base.DataOptions["rownumbers"] = value ? "true" : "false"; _RowNumbers = value; } get { return _RowNumbers; } }

		/// <summary>是否显示斑马线效果。</summary>
		/// <value>default value is false</value>
		public bool Striped { set { base.DataOptions["striped"] = value ? "true" : "false"; _Striped = value; } get { return _Striped; } }

		/// <summary>定义设置行的高度，根据该行的内容。设置为false可以提高负载性能。</summary>
		/// <value>默认值 true</value>
		public bool AutoRowHeight { set { base.DataOptions["autoRowHeight"] = value ? "true" : "false"; _AutoRowHeight = value; } get { return _AutoRowHeight; } }

		/// <summary>
		/// 如果为true，则只允许选择一行。
		/// </summary>
		/// <value>default value is false</value>
		public bool SingleSelect { set { base.DataOptions["singleSelect"] = value ? "true" : "false"; } }

		/// <summary>
		/// 如果为true，当用户点击行的时候该复选框就会被选中或取消选中。
		/// 如果为false，当用户仅在点击该复选框的时候才会呗选中或取消。
		/// </summary>
		/// <value>default value is true</value>
		public bool CheckOnSelect { set { base.DataOptions["checkOnSelect"] = value ? "true" : "false"; } }

		/// <summary>
		/// 如果为true，单击复选框将永远选择行。
		/// 如果为false，选择行将不选中复选框。
		/// </summary>
		/// <value>default value is true</value>
		public bool SelectOnCheck { set { base.DataOptions["selectOnCheck"] = value ? "true" : "false"; } }

		/// <summary>
		/// 在设置分页属性的时候初始化页码。
		/// </summary>
		/// <value>default value is "1"</value>
		public int PageNumber { set { base.DataOptions["pageNumber"] = Convert.ToString(value); } }

		/// <summary>
		/// 在设置分页属性的时候初始化页面大小。
		/// </summary>
		/// <value>default value is "10"</value>
		public int PageSize { set { base.DataOptions["pageSize"] = Convert.ToString(value); } }

		/// <summary>
		/// 在设置分页属性的时候 初始化页面大小选择列表。
		/// </summary>
		/// <value>default value is "[10,20,30,40,50]"</value>
		public int[] PageList { set { base.DataOptions["pageList"] = string.Concat("[", string.Join(",", value), "]"); } }
	}
}
