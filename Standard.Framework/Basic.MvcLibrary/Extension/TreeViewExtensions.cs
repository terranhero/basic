using Basic.MvcLibrary;
using System.Web.Mvc;
using System.Web.Routing;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 自定义TreeView控件扩展方法.
	/// </summary>
	public static class TreeViewExtensions
	{
		/// <summary>
		/// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回树型控件，客户端需要配合JQuery框架。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">树控件的ID。</param>
		/// <param name="action">树加载节点的操作方法</param>
		/// <exception cref="System.ArgumentException">name 参数为 null 或为空。</exception>
		/// <returns>树型控件，客户端需要配合JQuery框架。</returns>
		public static MvcHtmlString WebTree(this HtmlHelper html, string name, string action)
		{
			return html.WebTree(name, action, null, null, null, null);
		}

		/// <summary>
		/// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回树型控件，客户端需要配合JQuery框架。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">树控件的ID。</param>
		/// <param name="action">树加载节点的操作方法</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <exception cref="System.ArgumentException">name 参数为 null 或为空。</exception>
		/// <returns>树型控件，客户端需要配合JQuery框架。</returns>
		public static MvcHtmlString WebTree(this HtmlHelper html, string name, string action, IHtmlAttrs htmlAttrs)
		{
			return html.WebTree(name, action, null, null, null, htmlAttrs);
		}

		/// <summary>
		/// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回树型控件，客户端需要配合JQuery框架。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">树控件的ID。</param>
		/// <param name="action">树加载节点的操作方法</param>
		/// <param name="controller">树加载节点的控制器名称</param>
		/// <exception cref="System.ArgumentException">name 参数为 null 或为空。</exception>
		/// <returns>树型控件，客户端需要配合JQuery框架。</returns>
		public static MvcHtmlString WebTree(this HtmlHelper html, string name, string action, string controller)
		{
			return html.WebTree(name, action, controller, null, null, null);
		}

		/// <summary>
		/// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回树型控件，客户端需要配合JQuery框架。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">树控件的ID。</param>
		/// <param name="action">树加载节点的操作方法</param>
		/// <param name="controller">树加载节点的控制器名称</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <exception cref="System.ArgumentException">name 参数为 null 或为空。</exception>
		/// <returns>树型控件，客户端需要配合JQuery框架。</returns>
		public static MvcHtmlString WebTree(this HtmlHelper html, string name, string action, string controller, IHtmlAttrs htmlAttrs)
		{
			return html.WebTree(name, action, controller, null, null, htmlAttrs);
		}


		/// <summary>
		/// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回树型控件，客户端需要配合JQuery框架。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">树控件的ID。</param>
		/// <param name="action">树加载节点的操作方法</param>
		/// <param name="controller">树加载节点的控制器名称</param>
		/// <param name="route">树加载节点的路由名称</param>
		/// <param name="routeValues">路由参数</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <exception cref="System.ArgumentException">name 参数为 null 或为空。</exception>
		/// <returns>树型控件，客户端需要配合JQuery框架。</returns>
		public static MvcHtmlString WebTree(this HtmlHelper html, string name, string action, string controller, string route,
			RouteValueDictionary routeValues, IHtmlAttrs htmlAttrs)
		{
			TagBuilder tagBuilder = new TagBuilder("ul");
			if (action != null)
			{
				RequestContext request = html.ViewContext.RequestContext;
				RouteCollection routes = html.RouteCollection;
				if (routeValues == null)
					routeValues = new RouteValueDictionary();
				routeValues["action"] = action;
				if (controller != null)
					routeValues["controller"] = controller;
				string url = UrlHelper.GenerateUrl(route, action, controller, routeValues, routes, request, true);
				tagBuilder.MergeAttribute("url", url);
			}
			//tagBuilder.MergeAttribute("class", "easyui-tree");
			tagBuilder.GenerateId(name);
			if (htmlAttrs != null)
				tagBuilder.MergeAttributes<string, object>(htmlAttrs, true);
			return tagBuilder.ToMvcHtmlString();
		}
	}
}
