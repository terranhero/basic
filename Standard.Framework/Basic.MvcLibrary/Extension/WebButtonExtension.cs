using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 扩展Html帮助类，显示按钮输出
	/// </summary>
	public static class WebButtonExtension
	{
		#region 使用指定路由名称生成链接 input[Submit] 元素
		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 按钮（input[Submit] 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebSubmitButton(this HtmlHelper html, string text)
		{
			return html.WebSubmitButton(text, null, null, null, null, null);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 按钮（input[Submit] 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebSubmitButton(this HtmlHelper html, string text, IHtmlAttrs htmlAttrs)
		{
			return html.WebSubmitButton(text, null, null, null, null, htmlAttrs);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 按钮（input[Submit] 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebSubmitButton(this HtmlHelper html, string text, string routeName, string action)
		{
			return html.WebSubmitButton(text, routeName, action, null, null, null);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 按钮（input[Submit] 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebSubmitButton(this HtmlHelper html, string text, string routeName, string action, IHtmlAttrs htmlAttrs)
		{
			return html.WebSubmitButton(text, routeName, action, null, null, htmlAttrs);
		}


		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 按钮（input[Submit] 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebSubmitButton(this HtmlHelper html, string text, string routeName, string action, string controller)
		{
			return html.WebSubmitButton(text, routeName, action, controller, null, null);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 按钮（input[Button] 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebSubmitButton(this HtmlHelper html, string text, string routeName, string action, string controller, IHtmlAttrs htmlAttributes)
		{
			return html.WebSubmitButton(text, routeName, action, controller, null, htmlAttributes);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（a 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="id">URL路由关键字</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebSubmitButton(this HtmlHelper html, string text, string routeName, string action, string controller, object id, IHtmlAttrs htmlAttributes)
		{
			return html.WebButtonPrivate("submit", text, routeName, action, controller, id, htmlAttributes);
		}
		#endregion

		#region 使用指定路由名称生成链接 input[Button] 元素
		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（input[Button] 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebButton(this HtmlHelper html, string text)
		{
			return html.WebButton(text, null, null, null, null, null);
		}


		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 按钮（input[Button] 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebButton(this HtmlHelper html, string text, IHtmlAttrs htmlAttrs)
		{
			return html.WebButton(text, null, null, null, null, htmlAttrs);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（input[Button] 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebButton(this HtmlHelper html, string text, string routeName, string action)
		{
			return html.WebButton(text, routeName, action, null, null, null);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（input[Button] 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebButton(this HtmlHelper html, string text, string routeName, string action, IHtmlAttrs htmlAttributes)
		{
			return html.WebButton(text, routeName, action, null, null, htmlAttributes);
		}


		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（input[Button] 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebButton(this HtmlHelper html, string text, string routeName, string action, string controller)
		{
			return html.WebButton(text, routeName, action, controller, null, null);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（input[Button] 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebButton(this HtmlHelper html, string text, string routeName, string action, string controller, IHtmlAttrs htmlAttributes)
		{
			return html.WebButton(text, routeName, action, controller, null, htmlAttributes);
		}

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（a 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="id">URL路由关键字</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		public static MvcHtmlString WebButton(this HtmlHelper html, string text, string routeName, string action, string controller, object id, IHtmlAttrs htmlAttributes)
		{
			return html.WebButtonPrivate("button", text, routeName, action, controller, id, htmlAttributes);
		}
		#endregion

		/// <summary>
		/// 生成链接到指定 URL 路由的 HTML 定位元素（a 元素）。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="type">Button类型：button,submit,reset</param>
		/// <param name="text">定位点元素的内部文本。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="id">URL路由关键字</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>链接到指定 URL 路由的 HTML 元素。</returns>
		private static MvcHtmlString WebButtonPrivate(this HtmlHelper html, string type, string text,
			string routeName, string action, string controller, object id, IHtmlAttrs htmlAttrs)
		{
			string msgText = html.GetString(text);
			if (!string.IsNullOrEmpty(msgText))
				text = msgText;
			TagBuilder builder = new TagBuilder("button") { InnerHtml = text };
			builder.MergeAttribute("type", type);
			//builder.MergeAttribute("value", text);
			if (!string.IsNullOrEmpty(action))
			{
				string href = html.WebAction(routeName, action, controller, id);
				builder.MergeAttribute("href", href);
			}
			if (htmlAttrs == null)
				htmlAttrs = new HtmlAttrs();
			htmlAttrs.className = "web-button";
			builder.MergeAttributes<string, object>(htmlAttrs);
			return builder.ToMvcHtmlString(TagRenderMode.Normal);
		}
	}
}
