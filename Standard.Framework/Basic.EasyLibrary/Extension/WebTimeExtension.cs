using Basic.MvcLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using Basic.EntityLayer;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 扩展帮助器，扩展时间类型的标签输出
	/// </summary>
	public static class WebTimeExtension
	{
		#region Basic<T> 扩展
		private static TimeSpan? GetTimeValue<TM>(this BasicContext<TM> basic, Expression<Func<TM, TimeSpan>> expression)
		{
			EntityPropertyMeta metadata = LambdaHelper.GetProperty(expression);
			if (basic.Model == null) { return null; }
			return expression.Compile().Invoke(basic.Model);
		}

		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="basic">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		private static string PrivateTimeFor<TM>(this BasicContext<TM> basic, Expression<Func<TM, TimeSpan>> expression)
		{
			EntityPropertyMeta metadata = LambdaHelper.GetProperty(expression);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			string displayName = metadata.DisplayName;
			TimeSpan? value = basic.GetTimeValue(expression);
			if (value.HasValue)
			{
				if (metadata.DisplayFormat != null)
				{
					DisplayFormatAttribute wfa = metadata.DisplayFormat;
					if (!string.IsNullOrEmpty(wfa.DataFormatString))
						return string.Format(wfa.DataFormatString, value);
				}
				return string.Format(@"{0:hh\:mm\:ss}", value);
			}
			return string.Empty;
		}

		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		public static MvcHtmlString TimeFor<TM>(this BasicContext<TM> html, Expression<Func<TM, TimeSpan>> expression)
		{
			return MvcHtmlString.Create(html.AttributeEncode(html.PrivateTimeFor<TM>(expression)));
		}
		#endregion

		#region WebTimeFor<TimeSpan>
		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebTimeFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, TimeSpan?>> expression)
		{
			return helper.WebTimeFor<TM>(expression, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttributes">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebTimeFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, TimeSpan?>> expression, IHtmlAttrs htmlAttributes)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression<TM, TimeSpan?>(expression, helper.ViewData);
			return helper.WebTimeSpanFor<TM>(modelMeta, name, htmlAttributes);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebTimeFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, TimeSpan>> expression)
		{
			return helper.WebTimeFor<TM>(expression, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttributes">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebTimeFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, TimeSpan>> expression, IHtmlAttrs htmlAttributes)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression<TM, TimeSpan>(expression, helper.ViewData);
			return helper.WebTimeSpanFor<TM>(modelMeta, name, htmlAttributes);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="modelMeta">数据模型的公共元数据。</param>
		/// <param name="name">模型名称</param>
		/// <param name="htmlAtts">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		private static MvcHtmlString WebTimeSpanFor<TM>(this HtmlHelper<TM> helper, ModelMetadata modelMeta, string name, IHtmlAttrs htmlAtts)
		{
			if (htmlAtts == null) { htmlAtts = new HtmlAttrs(); }
			string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			TagBuilder tagBuilder = new TagBuilder("input");
			tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Text));
			tagBuilder.GenerateId(name);
			tagBuilder.AddCssClass("easyui-timespinner");
			tagBuilder.MergeAttribute("name", fullHtmlFieldName);
			IDictionary<string, object> Validation = helper.GetUnobtrusiveValidationAttributes(name, modelMeta);
			foreach (KeyValuePair<string, object> keyValue in Validation)
				htmlAtts.Add(keyValue);
			tagBuilder.MergeAttributes<string, object>(htmlAtts);
			ModelState state;
			if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out state) && (state.Errors.Count > 0))
				tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);

			string value = null;
			string format = modelMeta.DisplayFormatString;
			if (modelMeta.Model != null)
			{
				TimeSpan dt = (TimeSpan)modelMeta.Model;
				if (string.IsNullOrWhiteSpace(format))
					value = string.Format(@"{0:hh\:mm\:ss}", modelMeta.Model);
				else
					value = string.Format(format, modelMeta.Model);
			}
			if (!string.IsNullOrWhiteSpace(value))
			{
				tagBuilder.MergeAttribute("value", value);
			}
			return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
		}
		#endregion

		#region WebTimeFor<int>
		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttributes">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebTimeFor<TM>(this HtmlHelper<TM> helper,
			Expression<Func<TM, int?>> expression, IHtmlAttrs htmlAttributes)
		{
			return helper.WebTimeFor<TM>(expression, null, null, htmlAttributes);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebTimeFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, int?>> expression)
		{
			return helper.WebTimeFor<TM>(expression, null, null, null);
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
		public static MvcHtmlString WebTimeFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, int?>> expression,
			 string minDate, string maxDate, IHtmlAttrs htmlAttributes)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression<TM, int?>(expression, helper.ViewData);
			return helper.WebTimeFor<TM>(modelMeta, name, minDate, maxDate, htmlAttributes);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttributes">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebTimeFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, int>> expression, IHtmlAttrs htmlAttributes)
		{
			return helper.WebTimeFor<TM>(expression, null, null, htmlAttributes);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的日期选择元素。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebTimeFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, int>> expression)
		{
			return helper.WebTimeFor<TM>(expression, null, null, null);
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
		public static MvcHtmlString WebTimeFor<TM>(this HtmlHelper<TM> helper, Expression<Func<TM, int>> expression,
			 string minDate, string maxDate, IHtmlAttrs htmlAttributes)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression<TM, int>(expression, helper.ViewData);
			return helper.WebTimeFor<TM>(modelMeta, name, minDate, maxDate, htmlAttributes);
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
		/// <param name="htmlAtts">一个包含要为该元素设置的 HTML 特性的字典。</param>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“text”。</returns>
		private static MvcHtmlString WebTimeFor<TM>(this HtmlHelper<TM> helper, ModelMetadata modelMeta, string name,
			 string minDate, string maxDate, IHtmlAttrs htmlAtts)
		{
			if (htmlAtts == null) { htmlAtts = new HtmlAttrs(); }
			string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			TagBuilder tagBuilder = new TagBuilder("input");
			tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Text));
			tagBuilder.GenerateId(name);
			tagBuilder.AddCssClass("easyui-timebox");
			tagBuilder.MergeAttribute("name", fullHtmlFieldName);
			IDictionary<string, object> Validation = helper.GetUnobtrusiveValidationAttributes(name, modelMeta);
			foreach (KeyValuePair<string, object> keyValue in Validation)
				htmlAtts.Add(keyValue);
			tagBuilder.MergeAttributes<string, object>(htmlAtts);
			ModelState state;
			if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out state) && (state.Errors.Count > 0))
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
					value = string.Format(format, modelMeta.Model);
				}
			}
			if (!string.IsNullOrWhiteSpace(value))
			{
				tagBuilder.MergeAttribute("value", value);
			}
			return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
		}
		#endregion
	}
}
