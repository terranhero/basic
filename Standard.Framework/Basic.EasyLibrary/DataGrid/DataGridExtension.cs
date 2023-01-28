using System.Collections.Generic;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 扩展Html
	/// </summary>
	public static class DataGridExtension
	{
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
