using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 自定义扩展WebHidden类
	/// </summary>
	public static class WebHiddenExtension
	{
		/// <summary>
		/// 使用指定的资源键生成本地化的资源隐藏控件。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="localCode">本地化资源的键。</param>
		/// <returns>一个 本地化的资源隐藏控件。</returns>
		public static MvcHtmlString WebHiddenLocalResource(this HtmlHelper html, string localCode)
		{
			string text = html.GetString(localCode);
			return html.Hidden(localCode, text);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“checkbox”。</returns>
		public static MvcHtmlString WebHiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
		{
			return htmlHelper.WebHiddenFor<TModel, TProperty>(expression, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的隐藏 input 元素。
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“checkbox”。</returns>
		/// <exception cref="System.ArgumentException">参数不能为空!;name</exception>
		public static MvcHtmlString WebHiddenFor<TM, TP>(this HtmlHelper<TM> html, Expression<Func<TM, TP>> expression, IHtmlAttrs htmlAttrs)
		{
			//return htmlHelper.HiddenFor<TModel, TProperty>(expression, htmlAttributes);
			string name = ExpressionHelper.GetExpressionText(expression);
			string fullHtmlFieldName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			if (string.IsNullOrEmpty(fullHtmlFieldName))
			{
				throw new ArgumentException("参数不能为空!", "name");
			}
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, TP>(expression, html.ViewData);
			TagBuilder tagBuilder = new TagBuilder("input");
			tagBuilder.MergeAttributes<string, object>(htmlAttrs);
			tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Hidden));
			tagBuilder.GenerateId(fullHtmlFieldName);
			tagBuilder.MergeAttribute("name", fullHtmlFieldName, true);
			string str2 = Convert.ToString(metadata.Model, CultureInfo.CurrentCulture);
			if (metadata.Model is int && int.Equals(metadata.Model, 0))
				str2 = string.Empty;
			ModelState state; string str4 = null;
			if (html.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out state) && (state.Value != null))
			{
				str4 = state.Value.AttemptedValue;
			}
			tagBuilder.MergeAttribute("value", str4 ?? str2, true);
			return tagBuilder.ToMvcHtmlString(TagRenderMode.SelfClosing);
		}

        /// <summary>
        /// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的复选框 input 元素。 
        /// </summary>
        /// <typeparam name="TModel">模型的类型。</typeparam>
        /// <typeparam name="TProperty">值的类型。</typeparam>
        /// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
        /// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
        /// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“checkbox”。</returns>
        public static MvcHtmlString WebKeyFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.WebKeyFor<TModel, TProperty>(expression, null);
        }

        /// <summary>
        /// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的隐藏 input 元素。
        /// </summary>
        /// <typeparam name="TM">模型的类型。</typeparam>
        /// <typeparam name="TP">值的类型。</typeparam>
        /// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
        /// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
        /// <param name="htmlAttrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
        /// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“checkbox”。</returns>
        /// <exception cref="System.ArgumentException">参数不能为空!;name</exception>
        public static MvcHtmlString WebKeyFor<TM, TP>(this HtmlHelper<TM> html, Expression<Func<TM, TP>> expression, IHtmlAttrs htmlAttrs)
        {
            //return htmlHelper.HiddenFor<TModel, TProperty>(expression, htmlAttributes);
            string name = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (string.IsNullOrEmpty(fullHtmlFieldName))
            {
                throw new ArgumentException("参数不能为空!", "name");
            }
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, TP>(expression, html.ViewData);
            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes<string, object>(htmlAttrs);
            tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Hidden));
            tagBuilder.GenerateId(fullHtmlFieldName);
            tagBuilder.MergeAttribute("name", fullHtmlFieldName, true);
            string str2 = Convert.ToString(metadata.Model, CultureInfo.CurrentCulture);
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
