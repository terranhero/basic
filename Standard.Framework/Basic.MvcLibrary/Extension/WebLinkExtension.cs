using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Linq.Expressions;
using System;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 自定义输出HTML连接（&lt;a&gt;）。
	/// </summary>
	public static class WebHyperLinkExtensioncs
	{
		#region 生成链接地址
		/// <summary>
		/// 生成链接地址。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static string WebAction(this HtmlHelper html, string routeName, string action)
		{
			return html.WebAction(routeName, action, null, null);
		}

		/// <summary>
		/// 生成链接地址。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static string WebAction(this HtmlHelper html, string routeName, string action, string controller)
		{
			return html.WebAction(routeName, action, controller, null);
		}

		/// <summary>
		/// 生成链接地址。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="action">操作的名称。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static string WebAction(this HtmlHelper html, string action)
		{
			return html.WebAction(null, action, null, null);
		}

		/// <summary>
		/// 生成链接地址。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="id">URL路由关键字</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static string WebAction(this HtmlHelper html, string action, string controller, object id)
		{
			return html.WebAction(null, action, controller, id);
		}

		/// <summary>
		/// 生成链接地址。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="id">URL路由关键字</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static string WebAction(this HtmlHelper html, string routeName, string action, string controller, object id)
		{
			RequestContext request = html.ViewContext.RequestContext;
			RouteCollection routes = html.RouteCollection;
			RouteValueDictionary routeValues = new RouteValueDictionary();
			if (id != null)
				routeValues["id"] = id;
			routeValues["action"] = action;
			if (controller != null)
				routeValues["controller"] = controller;
			return UrlHelper.GenerateUrl(routeName, action, controller, routeValues, routes, request, true);
		}
		#endregion

		#region 使用指定路由名称生成链接 a 元素
		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（a 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="linkText">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebRoute(this HtmlHelper html, string linkText, string routeName, string action)
		{
			return html.WebRoute(linkText, routeName, action, null, null, null);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（a 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="linkText">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebRoute(this HtmlHelper html, string linkText, string routeName, string action, IHtmlAttrs htmlAttributes)
		{
			return html.WebRoute(linkText, routeName, action, null, null, htmlAttributes);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（a 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="linkText">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebRoute(this HtmlHelper html, string linkText, string routeName, string action, string controller)
		{
			return html.WebRoute(linkText, routeName, action, controller, null, null);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（a 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="linkText">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebRoute(this HtmlHelper html, string linkText, string routeName, string action, string controller, IHtmlAttrs htmlAttributes)
		{
			return html.WebRoute(linkText, routeName, action, controller, null, htmlAttributes);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（a 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="linkText">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="id">URL路由关键字</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebRoute(this HtmlHelper html, string linkText, string routeName, string action, string controller, object id, IHtmlAttrs htmlAttributes)
		{
			string href = html.WebAction(routeName, action, controller, id);
			TagBuilder builder = new TagBuilder("a") { InnerHtml = linkText };
			builder.MergeAttribute("href", href);
			if (htmlAttributes != null)
				builder.MergeAttributes<string, object>(htmlAttributes);
			return builder.ToMvcHtmlString(TagRenderMode.Normal);
		}
		#endregion

		#region 使用指定路由名称生成链接 a 元素(多语言版本)
		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（a 元素），多语言版本。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="msgCode">如果系统支持多语言时，当前显示文本对应的多语言编码，否则为null。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="id">URL路由关键字</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebLink(this HtmlHelper html, string msgCode, string routeName, string action, string controller, object id, IHtmlAttrs htmlAttributes)
		{
			string linkText = html.GetString(msgCode);
			return html.WebRoute(linkText, routeName, action, controller, id, htmlAttributes);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（a 元素），多语言版本。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="msgCode">如果系统支持多语言时，当前显示文本对应的多语言编码，否则为null。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebLink(this HtmlHelper html, string msgCode, string routeName, string action, string controller, IHtmlAttrs htmlAttributes)
		{
			return html.WebLink(msgCode, routeName, action, controller, null, htmlAttributes);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（a 元素），多语言版本。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="msgCode">如果系统支持多语言时，当前显示文本对应的多语言编码，否则为null。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebLink(this HtmlHelper html, string msgCode, string routeName, string action, string controller)
		{
			return html.WebLink(msgCode, routeName, action, controller, null, null);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（a 元素），多语言版本。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="msgCode">如果系统支持多语言时，当前显示文本对应的多语言编码，否则为null。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebLink(this HtmlHelper html, string msgCode, string routeName, string action, IHtmlAttrs htmlAttributes)
		{
			return html.WebLink(msgCode, routeName, action, null, null, htmlAttributes);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（a 元素），多语言版本。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="msgCode">如果系统支持多语言时，当前显示文本对应的多语言编码，否则为null。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebLink(this HtmlHelper html, string msgCode, string routeName, string action)
		{
			return html.WebLink(msgCode, routeName, action, null, null, null);
		}
		#endregion
	}
}
