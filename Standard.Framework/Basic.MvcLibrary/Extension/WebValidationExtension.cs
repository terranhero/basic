using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 提供对验证 HTML 窗体中的输入的支持。
	/// </summary>
	public static class WebValidationExtensions
	{
		/// <summary>
		///  用于在发生验证错误时设置错误消息样式的 CSS 类的名称（图片提示）。
		/// </summary>
		public static readonly string ValidationMessageCssClassName = "field-validation-error";
		/// <summary>
		/// 为由指定表达式表示的每个数据字段的验证错误消息返回对应的 HTML 标记。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">属性的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>如果该属性或对象有效，则为一个空字符串；否则为一个包含错误消息的 span 元素。</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
		public static MvcHtmlString WebValidationFor<TM, TP>(this HtmlHelper<TM> htmlHelper, Expression<Func<TM, TP>> expression)
		{
			return WebValidationFor(htmlHelper, expression, null, new RouteValueDictionary());
		}
		/// <summary>
		/// 为由指定表达式表示的每个数据字段的验证错误消息返回对应的 HTML 标记。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">属性的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="validationMessage">要在指定字段包含错误时显示的消息。</param>
		/// <returns>如果该属性或对象有效，则为一个空字符串；否则为一个包含错误消息的 span 元素。</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
		public static MvcHtmlString WebValidationFor<TM, TP>(this HtmlHelper<TM> htmlHelper, Expression<Func<TM, TP>> expression, string validationMessage)
		{
			return WebValidationFor(htmlHelper, expression, validationMessage, new RouteValueDictionary());
		}
		/// <summary>
		/// 为由指定表达式表示的每个数据字段的验证错误消息返回对应的 HTML 标记。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">属性的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="validationMessage">要在指定字段包含错误时显示的消息。</param>
		/// <param name="htmlAttributes">包含元素 HTML 特性的对象。</param>
		/// <returns>如果该属性或对象有效，则为一个空字符串；否则为一个包含错误消息的 span 元素。</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
		public static MvcHtmlString WebValidationFor<TM, TP>(this HtmlHelper<TM> htmlHelper,
			Expression<Func<TM, TP>> expression, string validationMessage, object htmlAttributes)
		{
			return WebValidationFor(htmlHelper, expression, validationMessage, new RouteValueDictionary(htmlAttributes));
		}
		/// <summary>
		/// 为由指定表达式表示的每个数据字段的验证错误消息返回对应的 HTML 标记。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">属性的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="validationMessage">要在指定字段包含错误时显示的消息。</param>
		/// <param name="htmlAttributes">包含元素 HTML 特性的对象。</param>
		/// <returns>如果该属性或对象有效，则为一个空字符串；否则为一个包含错误消息的 span 元素。</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
		public static MvcHtmlString WebValidationFor<TM, TP>(this HtmlHelper<TM> htmlHelper,
			Expression<Func<TM, TP>> expression, string validationMessage, IDictionary<string, object> htmlAttributes)
		{
			return ValidationMessageHelper(htmlHelper,
													 ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData),
													 ExpressionHelper.GetExpressionText(expression),
													 validationMessage,
													 htmlAttributes);
		}
		/// <summary>
		/// 为由指定表达式表示的每个数据字段的验证错误消息返回对应的 HTML 标记。 
		/// </summary>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="modelMetadata"></param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="validationMessage">要在指定字段包含错误时显示的消息。</param>
		/// <param name="htmlAttributes">包含元素 HTML 特性的对象。</param>
		/// <returns>如果该属性或对象有效，则为一个空字符串；否则为一个包含错误消息的 span 元素。</returns>
		private static MvcHtmlString ValidationMessageHelper(this HtmlHelper htmlHelper, ModelMetadata modelMetadata,
			string expression, string validationMessage, IDictionary<string, object> htmlAttributes)
		{
			string modelName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expression);
			FormContext formContext = null;
			if (htmlHelper.ViewContext.ClientValidationEnabled)
			{
				formContext = htmlHelper.ViewContext.FormContext;
			}

			if (!htmlHelper.ViewData.ModelState.ContainsKey(modelName) && formContext == null)
			{
				return null;
			}

			ModelState modelState = htmlHelper.ViewData.ModelState[modelName];
			ModelErrorCollection modelErrors = (modelState == null) ? null : modelState.Errors;
			ModelError modelError = ((modelErrors == null) || (modelErrors.Count == 0)) ? null : modelErrors[0];

			if (modelError == null && formContext == null)
			{
				return null;
			}

			TagBuilder builder = new TagBuilder("span");
			//TagBuilder builder = new TagBuilder("img");
			builder.MergeAttributes(htmlAttributes);
			//string imgSrc = UrlHelper.GenerateContentUrl("~/Content/Image/Waring.jpg", htmlHelper.ViewContext.HttpContext);
			//builder.MergeAttribute("src", imgSrc);
			builder.AddCssClass((modelError != null) ? HtmlHelper.ValidationMessageCssClassName : HtmlHelper.ValidationMessageValidCssClassName);

			if (!String.IsNullOrEmpty(validationMessage))
			{
				builder.SetInnerText(validationMessage);
			}
			else if (modelError != null)
			{
				string errorMsg = GetUserErrorMessageOrDefault(htmlHelper.ViewContext.HttpContext, modelError, modelState);
				//builder.MergeAttribute("alt", errorMsg);
				builder.MergeAttribute("title", errorMsg);
				//builder.SetInnerText(errorMsg);
			}

			if (formContext != null)
			{
				bool replaceValidationMessageContents = string.IsNullOrEmpty(validationMessage);
				if (htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
				{
					builder.MergeAttribute("data-valmsg-for", modelName);
					builder.MergeAttribute("data-valmsg-replace", replaceValidationMessageContents.ToString().ToLowerInvariant());
				}
				else
				{
					FieldValidationMetadata fieldMetadata = ApplyFieldValidationMetadata(htmlHelper, modelMetadata, modelName);
					// rules will already have been written to the metadata object 
					// only replace contents if no explicit message was specified
					fieldMetadata.ReplaceValidationMessageContents = replaceValidationMessageContents;
					builder.GenerateId(modelName + "_validationMessage");// client validation always requires an ID
					fieldMetadata.ValidationMessageId = builder.Attributes["id"];
				}
			}
			return builder.ToMvcHtmlString(TagRenderMode.SelfClosing);
		}

		private static FieldValidationMetadata ApplyFieldValidationMetadata(HtmlHelper htmlHelper, ModelMetadata modelMetadata, string modelName)
		{
			FormContext formContext = htmlHelper.ViewContext.FormContext;
			FieldValidationMetadata fieldMetadata = formContext.GetValidationMetadataForField(modelName, true);

			// write rules to context object
			IEnumerable<ModelValidator> validators = ModelValidatorProviders.Providers.GetValidators(modelMetadata, htmlHelper.ViewContext);
			foreach (ModelClientValidationRule rule in validators.SelectMany(v => v.GetClientValidationRules()))
			{
				fieldMetadata.ValidationRules.Add(rule);
			}

			return fieldMetadata;
		}

		private static string GetUserErrorMessageOrDefault(HttpContextBase httpContext, ModelError error, ModelState modelState)
		{
			if (!String.IsNullOrEmpty(error.ErrorMessage))
			{
				return error.ErrorMessage;
			}
			if (modelState == null)
			{
				return null;
			}

			string attemptedValue = (modelState.Value != null) ? modelState.Value.AttemptedValue : null;
			return String.Format(CultureInfo.CurrentCulture, GetInvalidPropertyValueResource(httpContext), attemptedValue);
		}
		private static string _resourceClassKey;

		/// <summary>
		/// 
		/// </summary>
		public static string ResourceClassKey
		{
			get
			{
				return _resourceClassKey ?? String.Empty;
			}
			set
			{
				_resourceClassKey = value;
			}
		}
		private static string GetInvalidPropertyValueResource(HttpContextBase httpContext)
		{
			string resourceValue = null;
			if (!String.IsNullOrEmpty(ResourceClassKey) && (httpContext != null))
			{
				// If the user specified a ResourceClassKey try to load the resource they specified.
				// If the class key is invalid, an exception will be thrown.
				// If the class key is valid but the resource is not found, it returns null, in which
				// case it will fall back to the MVC default error message.
				resourceValue = httpContext.GetGlobalResourceObject(ResourceClassKey, "InvalidPropertyValue", CultureInfo.CurrentUICulture) as string;
			}
			return resourceValue ?? "值“{0}”无效";
		}
	}
}
