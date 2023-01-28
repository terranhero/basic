using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System;
using System.Web.Routing;
using System.Text;
using System.Globalization;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 自定义CheckBox控件扩展方法.
	/// </summary>
	public static class WebCheckBoxExtensions
	{
		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="id">记录关键字</param>
		/// <param name="timestamp">记录时间戳</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“checkbox”。</returns>
		public static MvcHtmlString WebSelectColumn(this HtmlHelper html, object id, DateTime timestamp)
		{
			return html.WebSelectColumn(id, timestamp, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="id">记录关键字</param>
		/// <param name="timestamp">记录时间戳</param>
		/// <param name="htmlAttrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“checkbox”。</returns>
		public static MvcHtmlString WebSelectColumn(this HtmlHelper html, object id, DateTime timestamp, IHtmlAttrs htmlAttrs)
		{
			if (htmlAttrs == null)
				htmlAttrs = new HtmlAttrs();
			TagBuilder tagBuilder = new TagBuilder("input");
			tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.CheckBox));
			tagBuilder.MergeAttributes<string, object>(htmlAttrs);
			tagBuilder.MergeAttribute("name", "chk");
            tagBuilder.AddCssClass("web-checkbox");
			tagBuilder.GenerateId(string.Format("chk{0}", id));
			tagBuilder.MergeAttribute("ModifiedTime", string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", timestamp), true);
			tagBuilder.MergeAttribute("value", Convert.ToString(id), true);
			return tagBuilder.ToMvcHtmlString(TagRenderMode.SelfClosing);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“checkbox”。</returns>
		public static MvcHtmlString WebCheckBoxFor<TM, TP>(this HtmlHelper<TM> htmlHelper, Expression<Func<TM, TP>> expression)
		{
			return htmlHelper.WebCheckBoxFor<TM, TP>(expression, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“checkbox”。</returns>
		public static MvcHtmlString WebCheckBoxFor<TM, TP>(this HtmlHelper<TM> htmlHelper, Expression<Func<TM, TP>> expression, IHtmlAttrs htmlAttrs)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, TP>(expression, htmlHelper.ViewData);
			bool check = false;
			if (metadata.Model != null && metadata.Model is char)
				check = Convert.ToChar(metadata.Model) == 'Y';
			else if (metadata.Model != null && metadata.Model is byte)
				check = Convert.ToByte(metadata.Model) > 0;
			else if (metadata.Model != null && metadata.Model is short)
				check = Convert.ToInt16(metadata.Model) > 0;
			else if (metadata.Model != null && metadata.Model is int)
				check = Convert.ToInt32(metadata.Model) > 0;
			else if (metadata.Model != null && metadata.Model is bool)
				check = (bool)metadata.Model;

			TagBuilder tagBuilder = new TagBuilder("input");
			tagBuilder.MergeAttributes<string, object>(htmlAttrs);
			tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.CheckBox));
			tagBuilder.MergeAttribute("name", fullHtmlFieldName, true);
            tagBuilder.AddCssClass("web-checkbox");
			tagBuilder.GenerateId(name);
			if (check)
				tagBuilder.MergeAttribute("checked", "checked");
			ModelState state;
			if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out state) && (state.Errors.Count > 0))
				tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
			return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
		}
	}
}
