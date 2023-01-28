
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 扩展帮助器，扩展日期类型的标签输出
	/// </summary>
	public static class WebDateExtension
	{
		#region 输出日期时间控件
		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="modelMeta">数据模型的公共元数据。</param>
		/// <param name="name">模型名称</param>
		/// <param name="minDate">显示日期的最小值。</param>
		/// <param name="maxDate">显示日期的最大值。</param>
		/// <param name="attrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		private static MvcHtmlString WebDateFor<TM>(this HtmlHelper<TM> helper, ModelMetadata modelMeta, string name,
			  string minDate, string maxDate, object attrs)
		{
			string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			TagBuilder tagBuilder = new TagBuilder("input");
			tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Text));
			tagBuilder.GenerateId(name);
			tagBuilder.AddCssClass("easyui-datebox");
			tagBuilder.MergeAttribute("name", fullHtmlFieldName);

			IDictionary<string, object> htmlAttrs = HtmlHelper.AnonymousObjectToHtmlAttributes(attrs);
			if (htmlAttrs.ContainsKey("options")) { htmlAttrs["data-options"] = htmlAttrs["options"]; htmlAttrs.Remove("options"); }
			if (htmlAttrs.ContainsKey("dataoptions")) { htmlAttrs["data-options"] = htmlAttrs["dataoptions"]; htmlAttrs.Remove("dataoptions"); }
			IDictionary<string, object> dicAttrs = helper.GetUnobtrusiveValidationAttributes(name, modelMeta);
			foreach (KeyValuePair<string, object> keyValue in dicAttrs) { htmlAttrs.Add(keyValue); }

			tagBuilder.MergeAttributes<string, object>(htmlAttrs);
			if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out ModelState state) && (state.Errors.Count > 0))
				tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);

			if (string.IsNullOrWhiteSpace(minDate) == false) { tagBuilder.MergeAttribute("min-date", minDate); }
			if (string.IsNullOrWhiteSpace(maxDate) == false) { tagBuilder.MergeAttribute("max-date", maxDate); }

			string value = null;
			string format = modelMeta.DisplayFormatString;
			if (modelMeta.Model != null)
			{
				DateTime dt = (DateTime)modelMeta.Model;
				if (dt != DateTime.MinValue)
				{
					if (string.IsNullOrWhiteSpace(format))
						value = string.Format("{0:yyyy-MM-dd}", dt.Date);
					else
						value = string.Format(format, dt.Date);
				}
			}
			if (!string.IsNullOrWhiteSpace(value))
			{
				tagBuilder.MergeAttribute("value", value);
			}
			return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="attrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebDateFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, DateTime?>> expression, object attrs)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
			return helper.WebDateFor(modelMeta, name, null, null, attrs);
		}
		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="minDate">显示日期的最小值。</param>
		/// <param name="maxDate">显示日期的最大值。</param>
		/// <param name="attrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebDateFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, DateTime?>> expression,
			  string minDate, string maxDate, object attrs)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
			return helper.WebDateFor<TM>(modelMeta, name, minDate, maxDate, attrs);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="attrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebDateFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, DateTime>> expression, object attrs)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
			return helper.WebDateFor(modelMeta, name, null, null, attrs);
		}
		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="minDate">显示日期的最小值。</param>
		/// <param name="maxDate">显示日期的最大值。</param>
		/// <param name="attrs">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebDateFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, DateTime>> expression,
			  string minDate, string maxDate, object attrs)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
			return helper.WebDateFor(modelMeta, name, minDate, maxDate, attrs);
		}
		#endregion

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttributes">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebDateFor<TM>(this HtmlHelper<TM> helper,
			 Expression<Func<TM, DateTime?>> expression, IHtmlAttrs htmlAttributes)
		{
			return helper.WebDateFor<TM>(expression, null, null, htmlAttributes);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebDateFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, DateTime?>> expression)
		{
			return helper.WebDateFor<TM>(expression, null, null, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="minDate">显示日期的最小值。</param>
		/// <param name="maxDate">显示日期的最大值。</param>
		/// <param name="htmlAttributes">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebDateFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, DateTime?>> expression,
			  string minDate, string maxDate, IHtmlAttrs htmlAttributes)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression<TM, DateTime?>(expression, helper.ViewData);
			return helper.WebDateFor<TM>(modelMeta, name, minDate, maxDate, htmlAttributes);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttributes">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebDateFor<TM>(this HtmlHelper<TM> helper,
			 Expression<Func<TM, DateTime>> expression, IHtmlAttrs htmlAttributes)
		{
			return helper.WebDateFor<TM>(expression, null, null, htmlAttributes);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebDateFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, DateTime>> expression)
		{
			return helper.WebDateFor<TM>(expression, null, null, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="minDate">显示日期的最小值。</param>
		/// <param name="maxDate">显示日期的最大值。</param>
		/// <param name="htmlAttributes">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebDateFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, DateTime>> expression,
			  string minDate, string maxDate, IHtmlAttrs htmlAttributes)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression<TM, DateTime>(expression, helper.ViewData);
			return helper.WebDateFor<TM>(modelMeta, name, minDate, maxDate, htmlAttributes);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="modelMeta">数据模型的公共元数据。</param>
		/// <param name="name">模型名称</param>
		/// <param name="minDate">显示日期的最小值。</param>
		/// <param name="maxDate">显示日期的最大值。</param>
		/// <param name="htmlAttributes">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		private static MvcHtmlString WebDateFor<TM>(this HtmlHelper<TM> helper, ModelMetadata modelMeta, string name,
			  string minDate, string maxDate, IHtmlAttrs htmlAttributes)
		{
			string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			TagBuilder tagBuilder = new TagBuilder("input");
			tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Text));
			tagBuilder.GenerateId(name);
			tagBuilder.AddCssClass("easyui-datebox");
			tagBuilder.MergeAttribute("name", fullHtmlFieldName);
			if (htmlAttributes == null) { htmlAttributes = new HtmlAttrs(10); }
			IDictionary<string, object> Validation = helper.GetUnobtrusiveValidationAttributes(name, modelMeta);
			foreach (KeyValuePair<string, object> keyValue in Validation)
				htmlAttributes.Add(keyValue);
			tagBuilder.MergeAttributes(htmlAttributes);

			if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out ModelState state) && (state.Errors.Count > 0))
				tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);

			if (!string.IsNullOrWhiteSpace(minDate))
				tagBuilder.MergeAttribute("min-date", minDate);
			if (!string.IsNullOrWhiteSpace(maxDate))
				tagBuilder.MergeAttribute("max-date", maxDate);
			string value = null;
			string format = modelMeta.DisplayFormatString;
			if (modelMeta.Model != null)
			{
				DateTime dt = (DateTime)modelMeta.Model;
				if (dt != DateTime.MinValue)
				{
					if (string.IsNullOrWhiteSpace(format))
						value = string.Format("{0:yyyy-MM-dd}", dt.Date);
					else
						value = string.Format(format, dt.Date);
				}
			}
			if (!string.IsNullOrWhiteSpace(value))
			{
				tagBuilder.MergeAttribute("value", value);
			}
			return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
		}
	}
}
