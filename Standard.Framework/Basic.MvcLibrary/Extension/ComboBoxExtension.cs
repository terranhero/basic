using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using Basic.MvcLibrary;
using Basic.Properties;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 下拉型控件扩展方法
	/// </summary>
	public static class ComboBoxExtension
	{
		/// <summary>
		/// 为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的值对象。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebComboBoxFor<TM, TP>(this HtmlHelper<TM> html, Expression<Func<TM, TP>> expression)
		{
			return html.WebComboBoxFor<TM, TP>(expression, null);
		}

		/// <summary>
		/// 为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的值对象。</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebComboBoxFor<TM, TP>(this HtmlHelper<TM> html, Expression<Func<TM, TP>> expression, IHtmlAttrs htmlAttrs)
		{
			if (htmlAttrs == null)
				htmlAttrs = new HtmlAttrs();
			htmlAttrs.className = "easyui-combobox";
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, TP>(expression, html.ViewData);
			object value = metadata.Model;
			string name = ExpressionHelper.GetExpressionText(expression);
			string fullHtmlFieldName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			if (string.IsNullOrEmpty(fullHtmlFieldName))
			{
				throw new ArgumentException(EasyUIStrings.Common_NullOrEmpty, "name");
			}
			TagBuilder tagBuilder = new TagBuilder("input");
			tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Text));
			tagBuilder.GenerateId(fullHtmlFieldName);
			tagBuilder.MergeAttribute("name", fullHtmlFieldName, true);
			tagBuilder.MergeAttributes(htmlAttrs);
			if (value != null && value != DBNull.Value)
			{
				if (metadata.ModelType == typeof(DateTime) || metadata.ModelType == typeof(DateTime?))
					tagBuilder.MergeAttribute("value", html.FormatValue(value, metadata.DisplayFormatString), true);
				else if (metadata.ModelType == typeof(TimeSpan) || metadata.ModelType == typeof(TimeSpan?))
					tagBuilder.MergeAttribute("value", html.FormatValue(value, metadata.DisplayFormatString), true);
				else if (metadata.ModelType == typeof(int))
					tagBuilder.MergeAttribute("value", int.Equals(0, value) ? "" : Convert.ToString(value), true);
				else if (metadata.ModelType == typeof(Guid))
					tagBuilder.MergeAttribute("value", Guid.Equals(Guid.Empty, value) ? "" : Convert.ToString(value), true);
				else { tagBuilder.MergeAttribute("value", Convert.ToString(value), true); }
			}
			if (html.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out ModelState state) && (state.Errors.Count > 0))
			{
				tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
			}
			tagBuilder.MergeAttributes<string, object>(html.GetUnobtrusiveValidationAttributes(name, metadata));
			return tagBuilder.ToMvcHtmlString(TagRenderMode.SelfClosing);
		}


		/// <summary>
		/// 为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的值对象。</param>
		/// <param name="attrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebComboBoxFor<TM, TP>(this HtmlHelper<TM> html, Expression<Func<TM, TP>> expression, object attrs)
		{
			IDictionary<string, object> htmlAttrs = HtmlHelper.AnonymousObjectToHtmlAttributes(attrs);
			if (htmlAttrs.ContainsKey("class") == false) { htmlAttrs["class"] = "easyui-combobox"; }
			else { htmlAttrs["class"] = string.Concat("easyui-combobox ", htmlAttrs["class"]); }
			if (htmlAttrs.ContainsKey("options")) { htmlAttrs["data-options"] = htmlAttrs["options"]; htmlAttrs.Remove("options"); }
			if (htmlAttrs.ContainsKey("dataoptions")) { htmlAttrs["data-options"] = htmlAttrs["dataoptions"]; htmlAttrs.Remove("dataoptions"); }

			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
			object value = metadata.Model;
			string name = ExpressionHelper.GetExpressionText(expression);
			string fullHtmlFieldName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			if (string.IsNullOrEmpty(fullHtmlFieldName))
			{
				throw new ArgumentException(EasyUIStrings.Common_NullOrEmpty, "name");
			}
			TagBuilder tagBuilder = new TagBuilder("input");
			tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Text));
			tagBuilder.GenerateId(fullHtmlFieldName);
			tagBuilder.MergeAttribute("name", fullHtmlFieldName, true);
			tagBuilder.MergeAttributes<string, object>(htmlAttrs);
			if (value != null && value != DBNull.Value)
			{
				if (metadata.ModelType == typeof(DateTime) || metadata.ModelType == typeof(Nullable<DateTime>))
					tagBuilder.MergeAttribute("value", html.FormatValue(value, metadata.DisplayFormatString), true);
				else if (metadata.ModelType == typeof(TimeSpan) || metadata.ModelType == typeof(Nullable<TimeSpan>))
					tagBuilder.MergeAttribute("value", html.FormatValue(value, metadata.DisplayFormatString), true);
				else if (metadata.ModelType == typeof(int))
					tagBuilder.MergeAttribute("value", int.Equals(0, value) ? "" : Convert.ToString(value), true);
				else if (metadata.ModelType == typeof(Guid))
					tagBuilder.MergeAttribute("value", Guid.Equals(Guid.Empty, value) ? "" : Convert.ToString(value), true);
				else { tagBuilder.MergeAttribute("value", Convert.ToString(value), true); }
			}
			if (html.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out ModelState state) && (state.Errors.Count > 0))
			{
				tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
			}
			tagBuilder.MergeAttributes<string, object>(html.GetUnobtrusiveValidationAttributes(name, metadata));
			return tagBuilder.ToMvcHtmlString(TagRenderMode.SelfClosing);
		}
	}
}
