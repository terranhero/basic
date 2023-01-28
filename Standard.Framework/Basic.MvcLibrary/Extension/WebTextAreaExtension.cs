using BV = Basic.Validations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 扩展文本域控件
	/// </summary>
	public static class WebTextAreaExtension
	{
		private static void InitConfiguration(this HtmlHelper htmlHelper, IHtmlAttrs htmlAttributes)
		{
			if (htmlAttributes != null) { htmlAttributes.className = "web-textarea"; }
		}

		/// <summary>
		/// 使用指定 HTML 特性以及行数和列数，为由指定表达式表示的对象中的每个属性返回对应的 HTML textarea 元素。
		/// </summary>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <exception cref="ArgumentNullException">expression 参数为 null 或为空。</exception>
		/// <returns>由表达式表示的对象中的每个属性对应的 HTML textarea 元素。</returns>
		public static MvcHtmlString WebTextAreaFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, string>> expression)
		{
			return htmlHelper.WebTextAreaFor<TModel>(expression, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性以及行数和列数，为由指定表达式表示的对象中的每个属性返回对应的 HTML textarea 元素。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <exception cref="ArgumentNullException">expression 参数为 null 或为空。</exception>
		/// <returns>由表达式表示的对象中的每个属性对应的 HTML textarea 元素。</returns>
		public static MvcHtmlString WebTextAreaFor<TM>(this HtmlHelper<TM> html, Expression<Func<TM, string>> expression, IHtmlAttrs htmlAttrs)
		{
			if (htmlAttrs == null) { htmlAttrs = new HtmlAttrs(); }
			html.InitConfiguration(htmlAttrs);
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression<TM, string>(expression, html.ViewData);
			PropertyInfo propertyInfo = modelMeta.ContainerType.GetProperty(modelMeta.PropertyName);
			if (propertyInfo != null && propertyInfo.IsDefined(typeof(BV.StringLengthAttribute), true))
			{
				BV.StringLengthAttribute wsla = (BV.StringLengthAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(BV.StringLengthAttribute), true);
				if (wsla.MaximumLength > 0) { htmlAttrs["maxlength"] = wsla.MaximumLength; }
			}
			else if (propertyInfo != null && propertyInfo.IsDefined(typeof(StringLengthAttribute), true))
			{
				StringLengthAttribute sla = (StringLengthAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(StringLengthAttribute), true);
				if (sla.MaximumLength > 0) { htmlAttrs["maxlength"] = sla.MaximumLength; }
			}
			else if (propertyInfo != null && propertyInfo.IsDefined(typeof(BV.MaxLengthAttribute), true))
			{
				BV.MaxLengthAttribute sla = (BV.MaxLengthAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(BV.MaxLengthAttribute), true);
				if (sla.Length > 0) { htmlAttrs["maxlength"] = sla.Length; }
			}
			return html.TextAreaFor<TM, string>(expression, htmlAttrs);
		}

		/// <summary>
		/// 使用指定 HTML 特性以及行数和列数，为由指定表达式表示的对象中的每个属性返回对应的 HTML textarea 元素。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="rows">行数。</param>
		/// <param name="columns">列数。</param>
		/// <param name="htmlAttrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <exception cref="ArgumentNullException">expression 参数为 null 或为空。</exception>
		/// <returns>由表达式表示的对象中的每个属性对应的 HTML textarea 元素。</returns>
		public static MvcHtmlString WebTextAreaFor<TM>(this HtmlHelper<TM> html, Expression<Func<TM, string>> expression, int rows, int columns, IHtmlAttrs htmlAttrs)
		{
			html.InitConfiguration(htmlAttrs);
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression<TM, string>(expression, html.ViewData);
			PropertyInfo propertyInfo = modelMeta.ContainerType.GetProperty(modelMeta.PropertyName);
			if (propertyInfo != null && propertyInfo.IsDefined(typeof(BV.StringLengthAttribute), true))
			{
				BV.StringLengthAttribute wsla = (BV.StringLengthAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(BV.StringLengthAttribute), true);
				if (wsla.MaximumLength > 0) { htmlAttrs["maxlength"] = wsla.MaximumLength; }
			}
			else if (propertyInfo != null && propertyInfo.IsDefined(typeof(StringLengthAttribute), true))
			{
				StringLengthAttribute sla = (StringLengthAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(StringLengthAttribute), true);
				if (sla.MaximumLength > 0) { htmlAttrs["maxlength"] = sla.MaximumLength; }
			}
			else if (propertyInfo != null && propertyInfo.IsDefined(typeof(BV.MaxLengthAttribute), true))
			{
				BV.MaxLengthAttribute sla = (BV.MaxLengthAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(BV.MaxLengthAttribute), true);
				if (sla.Length > 0) { htmlAttrs["maxlength"] = sla.Length; }
			}
			return html.TextAreaFor<TM, string>(expression, rows, columns, htmlAttrs);
		}
	}
}
