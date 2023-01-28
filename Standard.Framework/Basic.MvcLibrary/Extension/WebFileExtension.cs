using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 扩展帮助器，扩展文件上传输出
	/// </summary>
	public static class WebFileExtension
	{
		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“file”。</returns>
		public static MvcHtmlString WebFile(this HtmlHelper html)
		{
			return html.WebFile(null, null, null, null, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="htmlAttrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“file”。</returns>
		public static MvcHtmlString WebFile(this HtmlHelper html, IHtmlAttrs htmlAttrs)
		{
			return html.WebFile(null, null, null, null, htmlAttrs);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">表示控件名称。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“file”。</returns>
		public static MvcHtmlString WebFile(this HtmlHelper html, string name)
		{
			return html.WebFile(name, null, null, null, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">表示控件名称。</param>
		/// <param name="htmlAttrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“file”。</returns>
		public static MvcHtmlString WebFile(this HtmlHelper html, string name, IHtmlAttrs htmlAttrs)
		{
			return html.WebFile(name, null, null, null, htmlAttrs);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">表示控件名称。</param>
		/// <param name="action">操作的名称。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“file”。</returns>
		public static MvcHtmlString WebFile(this HtmlHelper html, string name, string action)
		{
			return html.WebFile(name, action, null, null, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">表示控件名称。</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="htmlAttrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“file”。</returns>
		public static MvcHtmlString WebFile(this HtmlHelper html, string name, string action, IHtmlAttrs htmlAttrs)
		{
			return html.WebFile(name, action, null, null, htmlAttrs);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">表示控件名称。</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“file”。</returns>
		public static MvcHtmlString WebFile(this HtmlHelper html, string name, string action, string controller)
		{
			return html.WebFile(name, action, controller, null, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">表示控件名称。</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="htmlAttrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“file”。</returns>
		public static MvcHtmlString WebFile(this HtmlHelper html, string name, string action, string controller, IHtmlAttrs htmlAttrs)
		{
			return html.WebFile(name, action, controller, null, htmlAttrs);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">表示控件名称。</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“file”。</returns>
		public static MvcHtmlString WebFile(this HtmlHelper html, string name, string action, string controller, RouteValueDictionary routeValues)
		{
			return html.WebFile(name, action, controller, routeValues, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">表示控件名称。</param>
		/// <param name="action">操作的名称。</param>
		/// <param name="controller">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。</param>
		/// <param name="htmlAttrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“file”。</returns>
		public static MvcHtmlString WebFile(this HtmlHelper html, string name, string action, string controller,
			RouteValueDictionary routeValues, IHtmlAttrs htmlAttrs)
		{
			string fullHtmlFieldName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			TagBuilder tagBuilder = new TagBuilder("input");
			if (htmlAttrs != null)
				tagBuilder.MergeAttributes<string, object>(htmlAttrs);
			tagBuilder.MergeAttribute("type", "file");
			if (!string.IsNullOrWhiteSpace(name))
			{
				tagBuilder.MergeAttribute("name", fullHtmlFieldName, true);
				tagBuilder.GenerateId(name);
			}
			if (!string.IsNullOrWhiteSpace(action))
			{
				RequestContext request = html.ViewContext.RequestContext;
				RouteCollection routes = html.RouteCollection;
				string url = UrlHelper.GenerateUrl(null, action, controller, routeValues, routes, request, true);
				tagBuilder.MergeAttribute("action", url, true);
			}
			tagBuilder.AddCssClass("web-filebox");
			if (htmlAttrs != null && htmlAttrs.ContainsKey("class")) { tagBuilder.AddCssClass((string)htmlAttrs["class"]); }
			ModelState state;
			if (html.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out state) && (state.Errors.Count > 0))
				tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
			return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
		}
	}
}
