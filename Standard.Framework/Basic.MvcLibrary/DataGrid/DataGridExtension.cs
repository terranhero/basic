using System;
using System.Collections.Generic;
using System.Linq;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 配置文件中自定义数据库配置节
	/// </summary>
	internal sealed class DataGridSetting
	{
		/// <summary>初始化 DataGridSetting 类实例</summary>
		public DataGridSetting() { PageList = new int[] { 20, 50, 80, 150 }; }

		/// <summary>
		/// 获取或设置EasyGrid分页列表，每页允许的记录数。
		/// </summary>
		public int[] PageList { get; set; }

		/// <summary>获取或设置EasyGrid分页列表，每页允许的记录数。</summary>
		public int PageSize { get; set; } = 20;

		/// <summary>获取或设置 EasyGrid 是否需要填充满屏。</summary>
		public bool Fit { get; set; } = true;

		/// <summary>
		/// 如果为true，当用户点击行的时候该复选框就会被选中或取消选中。
		/// 如果为false，当用户仅在点击该复选框的时候才会呗选中或取消。
		/// </summary>
		public bool CheckOnSelect { get; set; } = true;

		/// <summary>
		/// 如果为true，单击复选框将永远选择行。如果为false，选择行将不选中复选框。
		/// </summary>
		public bool SelectOnCheck { get; set; } = true;
	}

	/// <summary>
	/// 扩展Html
	/// </summary>
	public static class DataGridExtension
	{
		private static DataGridSetting settings = new DataGridSetting();
		internal static void ToOptions(DataGridOptions opts)
		{
			opts.PageList = settings.PageList;
			opts.CheckOnSelect = settings.CheckOnSelect;
			opts.SelectOnCheck = settings.SelectOnCheck;
			opts.Fit = settings.Fit; opts.PageSize = settings.PageSize;
		}

		/// <summary>设置DataGrid默认配置信息</summary>
		/// <param name="list">DataGrid分页列表</param>
		/// <param name="size">默认分页显示的记录数</param>
		/// <param name="fit">是否填充满屏</param>
		/// <param name="cos">如果为true，当用户点击行的时候该复选框就会被选中或取消选中。
		/// 如果为false，当用户仅在点击该复选框的时候才会呗选中或取消</param>
		/// <param name="soc">如果为true，单击复选框将永远选择行。如果为false，选择行将不选中复选框</param>
		public static void SetSettings(string list, int size, bool fit, bool cos, bool soc)
		{
			settings.PageList = list.Split(',').Select(m => Convert.ToInt32(m)).ToArray();
			settings.PageSize = size; settings.Fit = fit;
			settings.CheckOnSelect = cos; settings.SelectOnCheck = soc;
		}

		/// <summary>设置DataGrid默认配置信息</summary>
		/// <param name="list">DataGrid分页列表</param>
		/// <param name="size">默认分页显示的记录数</param>
		/// <param name="fit">是否填充满屏</param>
		public static void SetSettings(string list, int size, bool fit)
		{
			settings.PageList = list.Split(',').Select(m => Convert.ToInt32(m)).ToArray();
			settings.PageSize = size; settings.Fit = fit;
		}

		/// <summary>
		/// 创建JQuery.EasyUI.DataGrid的MVC对象实例(Version:1.3.1)。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
		/// <param name="options">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>返回JQuery.EasyUI.DataGrid的MVC对象实例。</returns>
		public static DataGrid<T> DataGrid<T>(this IBasicContext html, DataGridOptions options) where T : class
		{
			return html.DataGrid<T>(null, null, null, null, options);
		}

		/// <summary>
		/// 创建JQuery.EasyUI.DataGrid的MVC对象实例(Version:1.3.1)。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
		/// <param name="action">操作的名称。</param>
		/// <returns>返回JQuery.EasyUI.DataGrid的MVC对象实例。</returns>
		public static DataGrid<T> DataGrid<T>(this IBasicContext html, string action) where T : class
		{
			return html.DataGrid<T>(null, action, null, null, new DataGridOptions());
		}

		/// <summary>
		/// 创建JQuery.EasyUI.DataGrid的MVC对象实例(Version:1.3.1)。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
		/// <param name="id">Html元素的name</param>
		/// <param name="options">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>返回JQuery.EasyUI.DataGrid的MVC对象实例。</returns>
		public static DataGrid<T> DataGrid<T>(this IBasicContext html, string id, DataGridOptions options) where T : class
		{
			return html.DataGrid<T>(id, null, null, null, options);
		}

		/// <summary>
		/// 创建JQuery.EasyUI.DataGrid的MVC对象实例(Version:1.3.1)。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
		/// <param name="id">Html元素的name</param>
		/// <param name="action">操作的名称。</param>
		/// <returns>返回JQuery.EasyUI.DataGrid的MVC对象实例。</returns>
		public static DataGrid<T> DataGrid<T>(this IBasicContext html, string id, string action) where T : class
		{
			return html.DataGrid<T>(id, action, null, null, new DataGridOptions());
		}

		/// <summary>
		/// 创建JQuery.EasyUI.DataGrid的MVC对象实例(Version:1.3.1)。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
		/// <param name="id">Html元素的name</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="options">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>返回JQuery.EasyUI.DataGrid的MVC对象实例。</returns>
		public static DataGrid<T> DataGrid<T>(this IBasicContext html, string id, string action, DataGridOptions options) where T : class
		{
			return html.DataGrid<T>(id, action, null, null, options);
		}

		/// <summary>
		/// 创建JQuery.EasyUI.DataGrid的MVC对象实例(Version:1.3.1)。
		/// </summary>
		/// <typeparam name="T">模型实体类型</typeparam>
		/// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
		/// <param name="id">Html元素的name</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">路由值。</param>
		/// <param name="options">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>返回JQuery.EasyUI.DataGrid的MVC对象实例。</returns>
		public static DataGrid<T> DataGrid<T>(this IBasicContext html, string id, string action, string controller,
			IDictionary<string, object> routeValues, DataGridOptions options) where T : class
		{
			if (options == null) { options = new DataGridOptions(); }
			if (id != null) { options.id = id; }
			if (!string.IsNullOrWhiteSpace(action))
			{
				options.Url = html.Action(action, controller, routeValues);
			}
			return new DataGrid<T>(html, options);
		}
	}
}
