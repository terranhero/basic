using System;
using System.Collections.Generic;
using System.Web;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示DataGrid页脚信息
	/// </summary>
	public sealed class DataGridFooterColumn<T>
	{
		internal readonly IBasicContext _Context;
		/// <summary>
		/// 初始化 DataGridColumn 类实例
		/// </summary>
		/// <param name="context">当前 HTTP 上下文信息。</param>
		internal DataGridFooterColumn(IBasicContext context)
		{
			_Context = context;
			AllowExport = true; AllowToHtml = true; AlignToCenter();
		}

		private Func<IEnumerable<T>, string> _TitleSelector;
		/// <summary>
		/// 设置Title属性值
		/// </summary>
		/// <param name="titleSelector">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		public DataGridFooterColumn<T> SetValue(Func<IEnumerable<T>, string> titleSelector)
		{
			_TitleSelector = titleSelector;
			return this;
		}

		/// <summary>获取页脚单元格的值</summary>
		/// <param name="source">数据源</param>
		/// <returns>返回当前列对象。</returns>
		public string GetValue(IEnumerable<T> source)
		{
			if (_TitleSelector != null) { return _TitleSelector.Invoke(source); }
			return string.Empty;
		}

		/// <summary>
		/// 设置Rowspan属性值
		/// </summary>
		/// <param name="rowspan">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		public DataGridFooterColumn<T> SetRowspan(int rowspan) { Rowspan = rowspan; return this; }

		/// <summary>
		/// 设置Colspan属性值
		/// </summary>
		/// <param name="colspan">要设置的属性新值</param>
		/// <returns>返回当前列对象。</returns>
		public DataGridFooterColumn<T> SetColspan(int colspan) { Colspan = colspan; return this; }

		/// <summary>
		/// 设置 align 属性值靠左对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public DataGridFooterColumn<T> AlignToLeft() { Align = "left"; return this; }

		/// <summary>
		/// 设置 align 属性值居中对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public DataGridFooterColumn<T> AlignToCenter() { Align = "center"; return this; }

		/// <summary>
		/// 设置 align 属性值靠右对齐
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public DataGridFooterColumn<T> AlignToRight() { Align = "right"; return this; }

		/// <summary>
		/// 设置Hidden属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public DataGridFooterColumn<T> Hide() { Hidden = true; return this; }

		/// <summary>设置页脚单元格样式</summary>
		/// <param name="style">页脚单元格样式名称</param>
		/// <returns>返回当前列对象。</returns>
		public DataGridFooterColumn<T> SetStyle(string style) { Style = style; return this; }

		#region 输出属性定义

		/// <summary>
		/// 设置 AllowToHtml 属性值为 false。
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public DataGridFooterColumn<T> NotToHtml() { AllowToHtml = false; return this; }

		/// <summary>
		/// 是否允许导出.
		/// </summary>
		internal bool AllowToHtml { get; set; }

		/// <summary>
		/// 设置 AllowExport 属性值
		/// </summary>
		/// <returns>返回当前列对象。</returns>
		public DataGridFooterColumn<T> NotExport() { AllowExport = false; return this; }

		/// <summary>
		/// 是否允许导出.
		/// </summary>
		public bool AllowExport { get; internal set; }

		#endregion

		#region 属性
		/// <summary>
		/// The column title text.
		/// </summary>
		public string Title { get; internal set; }

		/// <summary>
		/// Indicate how many rows a cell should take up. 
		/// </summary>
		/// <value>default value is "0"</value>
		public int Rowspan { get; internal set; }

		/// <summary>
		/// Indicate how many columns a cell should take up.
		/// </summary>
		/// <value>default value is "1"</value>
		public int Colspan { get; internal set; }

		/// <summary>
		/// Indicate how to align the column data. 'left','right','center' can be used.
		/// </summary>
		/// <value>default value is "TextAlign.Left"</value>
		public string Align { get; internal set; }

		/// <summary>
		/// True to hide the column.
		/// </summary>
		/// <value>default value is "false"</value>
		public bool Hidden { get; internal set; }

		/// <summary>页脚单元格样式</summary>
		internal string Style { get; set; }
		#endregion
	}
}
