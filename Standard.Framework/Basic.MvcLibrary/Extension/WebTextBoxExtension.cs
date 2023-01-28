using Basic.EntityLayer;
using BV = Basic.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 字符型文本框帮助类
	/// </summary>
	public static class WebTextBoxExtensions
	{
		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="inputAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebWaterMarkFor<TM, TP>(this HtmlHelper<TM> html, int authorizationCode, Expression<Func<TM, TP>> expression, IHtmlAttrs inputAttrs)
		{
			bool allowShow = AuthorizeContext.CheckAuthorizationCode(html, authorizationCode);
			if (!allowShow) { return MvcHtmlString.Empty; }
			if (inputAttrs == null) { inputAttrs = new HtmlAttrs(5); }
			ModelMetadata metaData = ModelMetadata.FromLambdaExpression<TM, TP>(expression, html.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			string displayName = metaData.DisplayName; string displayText = metaData.PropertyName;
			PropertyInfo propertyInfo = metaData.ContainerType.GetProperty(metaData.PropertyName);
			if (propertyInfo.IsDefined(typeof(WebDisplayAttribute), true))
			{
				object[] attributes = propertyInfo.GetCustomAttributes(typeof(WebDisplayAttribute), true);
				if (attributes != null && attributes.Length > 0)
				{
					WebDisplayAttribute wda = attributes[0] as WebDisplayAttribute;
					string converterName = wda.ConverterName;
					if (!string.IsNullOrWhiteSpace(wda.Prompt))
						displayText = html.GetString(converterName, wda.Prompt);
					else if (!string.IsNullOrWhiteSpace(displayName))
						displayText = html.GetString(converterName, displayName);
					else
						displayText = displayName;
				}
			}
			TagBuilder inputBuilder = new TagBuilder("input");
			inputBuilder.MergeAttributes<string, object>(inputAttrs);
			inputBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Text));
			inputBuilder.MergeAttribute("name", htmlFieldName, true);
			inputBuilder.MergeAttribute("placeholder", displayText, true);
			inputBuilder.AddCssClass("web-textbox");
			if (inputAttrs != null && inputAttrs.ContainsKey("class")) { inputBuilder.AddCssClass((string)inputAttrs["class"]); }
			inputBuilder.GenerateId(htmlFieldName);
			return inputBuilder.ToMvcHtmlString();
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebWaterMarkFor<TM, TP>(this HtmlHelper<TM> html, int authorizationCode, Expression<Func<TM, TP>> expression)
		{
			return html.WebWaterMarkFor(authorizationCode, expression, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="name">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="localizationCode">标签文本，水印文本。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebWaterMark(this HtmlHelper html, int authorizationCode, string name, string localizationCode)
		{
			return html.WebWaterMark(authorizationCode, name, localizationCode, null, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="name">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="localizationCode">标签文本，水印文本。</param>
		/// <param name="inputAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebWaterMark(this HtmlHelper html, int authorizationCode, string name, string localizationCode, IHtmlAttrs inputAttrs)
		{
			return html.WebWaterMark(authorizationCode, name, localizationCode, null, inputAttrs);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="name">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="localizationCode">标签文本，水印文本。</param>
		/// <param name="labelAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <param name="inputAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebWaterMark(this HtmlHelper html, int authorizationCode, string name, string localizationCode, IHtmlAttrs labelAttrs, IHtmlAttrs inputAttrs)
		{
			bool allowShow = AuthorizeContext.CheckAuthorizationCode(html, authorizationCode);
			if (!allowShow) { return MvcHtmlString.Empty; }
			if (inputAttrs == null)
				inputAttrs = new HtmlAttrs(5);
			string htmlName = ExpressionHelper.GetExpressionText(name);
			string labelName = string.Concat("label_", htmlName);
			string localText = html.GetString(localizationCode);
			if (string.IsNullOrWhiteSpace(localText))
				localText = localizationCode;
			TagBuilder inputBuilder = new TagBuilder("input");
			inputBuilder.MergeAttributes<string, object>(inputAttrs);
			inputBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Text));
			inputBuilder.MergeAttribute("name", htmlName, true);
			inputBuilder.MergeAttribute("placeholder", localText, true);
			inputBuilder.AddCssClass("web-textbox");
			inputBuilder.AddCssClass("easyui-watermark");
			if (inputAttrs != null && inputAttrs.ContainsKey("class")) { inputBuilder.AddCssClass((string)inputAttrs["class"]); }
			inputBuilder.GenerateId(htmlName);
			return inputBuilder.ToMvcHtmlString(); //spanBuilder.ToMvcHtmlString();
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
		public static MvcHtmlString WebTextBoxFor<TM, TP>(this HtmlHelper<TM> html, Expression<Func<TM, TP>> expression, IHtmlAttrs htmlAttrs)
		{
			if (htmlAttrs == null) { htmlAttrs = new HtmlAttrs(); }
			htmlAttrs.AddCssClass("web-textbox");
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression<TM, TP>(expression, html.ViewData);
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
			if (modelMeta.ModelType == typeof(DateTime) || modelMeta.ModelType == typeof(Nullable<DateTime>))
			{
				return html.TextBoxFor<TM, TP>(expression, modelMeta.DisplayFormatString, htmlAttrs);
			}
			else if (modelMeta.ModelType == typeof(TimeSpan) || modelMeta.ModelType == typeof(Nullable<TimeSpan>))
			{
				return html.TextBoxFor<TM, TP>(expression, modelMeta.DisplayFormatString, htmlAttrs);
			}
			return html.TextBoxFor<TM, TP>(expression, htmlAttrs);
		}

		/// <summary>
		/// 为由指定表达式表示的对象中的每个属性返回对应的文本 input 元素。
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一个 HTML input 元素，其 type 特性针对表达式表示的对象中的每个属性均设置为“text”。</returns>
		public static MvcHtmlString WebTextBoxFor<TM, TP>(this HtmlHelper<TM> htmlHelper, Expression<Func<TM, TP>> expression)
		{
			return htmlHelper.WebTextBoxFor<TM, TP>(expression, null);
		}
	}
}
