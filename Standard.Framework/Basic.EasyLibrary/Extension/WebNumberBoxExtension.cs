using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Globalization;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 数字输入框扩展
	/// </summary>
	public static class WebNumberBoxExtension
	{
		#region 输出数字控件
		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="attrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebNumberBoxFor<TM, TP>(this HtmlHelper<TM> htmlHelper, Expression<Func<TM, TP>> expression, object attrs)
		{
			IDictionary<string, object> htmlAttrs = HtmlHelper.AnonymousObjectToHtmlAttributes(attrs);
			if (htmlAttrs.ContainsKey("class") == false) { htmlAttrs["class"] = "easyui-numberbox web-textbox"; }
			else { htmlAttrs["class"] = string.Concat("easyui-numberbox web-textbox ", htmlAttrs["class"]); }
			if (htmlAttrs.ContainsKey("options")) { htmlAttrs["data-options"] = htmlAttrs["options"]; htmlAttrs.Remove("options"); }
			if (htmlAttrs.ContainsKey("dataoptions")) { htmlAttrs["data-options"] = htmlAttrs["dataoptions"]; htmlAttrs.Remove("dataoptions"); }
			return htmlHelper.TextBoxFor(expression, htmlAttrs);
		}
		#endregion

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebNumberSpinnerFor<TM, TP>(this HtmlHelper<TM> htmlHelper, Expression<Func<TM, TP>> expression, IHtmlAttrs htmlAttrs)
		{
			if (htmlAttrs == null)
				htmlAttrs = new HtmlAttrs();
			htmlAttrs.className = "easyui-numberspinner";
			return htmlHelper.TextBoxFor<TM, TP>(expression, htmlAttrs);
		}

		/// <summary>
		/// 为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebNumberSpinnerFor<TM, TP>(this HtmlHelper<TM> htmlHelper, Expression<Func<TM, TP>> expression)
		{
			return htmlHelper.WebNumberSpinnerFor<TM, TP>(expression, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebNumberBoxFor<TM, TP>(this HtmlHelper<TM> htmlHelper, Expression<Func<TM, TP>> expression, IHtmlAttrs htmlAttrs)
		{
			if (htmlAttrs == null)
				htmlAttrs = new HtmlAttrs();
			htmlAttrs.className = "easyui-numberbox web-textbox";
			return htmlHelper.TextBoxFor(expression, htmlAttrs);
		}

		/// <summary>
		/// 为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebNumberBoxFor<TM, TP>(this HtmlHelper<TM> htmlHelper, Expression<Func<TM, TP>> expression)
		{
			return htmlHelper.WebNumberBoxFor<TM, TP>(expression, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebValidateBoxFor<TM, TP>(this HtmlHelper<TM> html, Expression<Func<TM, TP>> expression, IHtmlAttrs htmlAttrs)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			string fullHtmlFieldName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			if (string.IsNullOrEmpty(fullHtmlFieldName)) { throw new ArgumentException("参数不能为空!", "name"); }
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, TP>(expression, html.ViewData);
			TagBuilder tagBuilder = new TagBuilder("input");
			tagBuilder.MergeAttributes<string, object>(htmlAttrs);
			tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Text));
			tagBuilder.GenerateId(fullHtmlFieldName);
			tagBuilder.MergeAttribute("name", fullHtmlFieldName, true);
			tagBuilder.AddCssClass("web-textbox");
			string str2 = Convert.ToString(metadata.Model, CultureInfo.CurrentCulture);
			if (metadata.Model is decimal && decimal.Equals(metadata.Model, 0.00M))
				str2 = string.Empty;
			ModelState state; string str4 = null;
			if (html.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out state) && (state.Value != null))
			{
				str4 = state.Value.AttemptedValue;
			}
			tagBuilder.MergeAttribute("value", str4 ?? str2, true);
			return tagBuilder.ToMvcHtmlString(TagRenderMode.SelfClosing);
		}
	}
}
