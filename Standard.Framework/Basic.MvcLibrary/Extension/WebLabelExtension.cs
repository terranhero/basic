
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Basic.EntityLayer;
using System.Web;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// Represents support for the HTML label element in an ASP.NET MVC view.
	/// </summary>
	public static class WebLabelExtension
	{
		/// <summary>
		/// 返回一个 HTML label 元素和由指定表达式表示的属性的属性名称。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="metadata"></param>
		/// <param name="htmlFieldName"></param>
		/// <param name="labelText">需要显示标签的文本</param>
		/// <param name="htmlAttrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML label 元素和由表达式表示的属性的属性名称。</returns>
		internal static MvcHtmlString LabelHelper(this HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string labelText, IHtmlAttrs htmlAttrs)
		{
			string str = labelText ?? (metadata.DisplayName ?? (metadata.PropertyName ?? htmlFieldName.Split(new char[] { '.' }).Last<string>()));
			if (string.IsNullOrEmpty(str)) { return MvcHtmlString.Empty; }
			TagBuilder tagBuilder = new TagBuilder("label");
			if (htmlAttrs == null) { htmlAttrs = new HtmlAttrs(); }
			if (htmlAttrs.ContainsKey("id")) { tagBuilder.GenerateId((string)htmlAttrs["id"]); }
			tagBuilder.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
			tagBuilder.InnerHtml = str;
			return tagBuilder.ToMvcHtmlString(TagRenderMode.Normal);
		}

		/// <summary>
		/// 返回一个 HTML label 元素和由指定表达式表示的属性的属性名称。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="labelText">需要显示标签的文本</param>
		/// <param name="htmlAttrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML label 元素和由表达式表示的属性的属性名称。</returns>
		public static MvcHtmlString WebLabel(this HtmlHelper html, string expression, string labelText, IHtmlAttrs htmlAttrs)
		{
			return html.LabelHelper(ModelMetadata.FromStringExpression(expression, html.ViewData), expression, labelText, htmlAttrs);
		}

		/// <summary>
		///  返回一个 HTML label 元素和由指定表达式表示的属性的属性名称。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TV">值的类型。</typeparam>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML label 元素和由表达式表示的属性的属性名称。</returns>
		public static MvcHtmlString WebLabelFor<TM, TV>(this HtmlHelper<TM> html, Expression<Func<TM, TV>> expression)
		{
			ModelMetadata metaData = ModelMetadata.FromLambdaExpression<TM, TV>(expression, html.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			string displayName = metaData.DisplayName; string displayText = metaData.PropertyName;
			PropertyInfo propertyInfo = metaData.ContainerType.GetProperty(metaData.PropertyName);
			if (propertyInfo.IsDefined(typeof(WebDisplayAttribute), true))
			{
				object[] attributes = propertyInfo.GetCustomAttributes(typeof(WebDisplayAttribute), true);
				if (attributes == null || attributes.Length == 0)
					return html.LabelHelper(metaData, htmlFieldName, null, null);
				foreach (WebDisplayAttribute wda in attributes)
				{
					string converterName = wda.ConverterName;
					if (string.IsNullOrWhiteSpace(converterName))
						displayText = html.GetString(displayName);
					else
						displayText = html.GetString(converterName, displayName);
				}
			}
			return html.LabelHelper(metaData, htmlFieldName, displayText, null);
		}

		/// <summary>
		/// 返回一个 HTML label 元素和由指定表达式表示的属性的属性名称。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TV">值的类型。</typeparam>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="msgCode">如果系统支持多语言时，当前显示文本对应的多语言编码</param>
		/// <returns>一个 HTML label 元素和由表达式表示的属性的属性名称。</returns>
		public static MvcHtmlString WebLabelFor<TM, TV>(this HtmlHelper<TM> html, Expression<Func<TM, TV>> expression, string msgCode)
		{
			string text = html.GetString(msgCode);
			ModelMetadata metaData = ModelMetadata.FromLambdaExpression<TM, TV>(expression, html.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			if (string.IsNullOrWhiteSpace(text))
				return html.LabelHelper(metaData, htmlFieldName, null, null);
			else
				return html.LabelHelper(metaData, htmlFieldName, text, null);
		}
	}
}
