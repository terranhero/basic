
using System.Web.Mvc;
using System.Linq.Expressions;
using System;
namespace Basic.MvcLibrary
{
	/// <summary>
	/// Represents support for the HTML span element in an ASP.NET MVC view.
	/// </summary>
	public static class WebSpanExtension
	{
		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的span元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML span 元素，使用指定的 HTML 特性将其指定表达式表示的对象中的每个属性。</returns>
		public static MvcHtmlString WebSpanFor<TM, TP>(this HtmlHelper<TM> html, Expression<Func<TM, TP>> expression)
		{
			return html.WebSpanFor<TM, TP>(expression, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的span元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="msgCode">如果系统支持多语言时，当前显示文本对应的多语言编码</param>
		/// <returns>一个 HTML span 元素，使用指定的 HTML 特性将其指定表达式表示的对象中的每个属性。</returns>
		public static MvcHtmlString WebSpanFor<TM, TP>(this HtmlHelper<TM> html, Expression<Func<TM, TP>> expression, string msgCode)
		{
			string text = html.GetString(msgCode);
			TagBuilder tagBuilder = new TagBuilder("span");
			if (string.IsNullOrWhiteSpace(text))
			{
				ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, TP>(expression, html.ViewData);
				tagBuilder.InnerHtml = metadata.DisplayName ?? metadata.PropertyName;
			}
			else
			{
				tagBuilder.InnerHtml = text;
			}
			return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
		}
	}
}
