
using Basic.Interfaces;
using System;
using System.Reflection;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Basic.Collections;
using Basic.EntityLayer;
using Basic.Configuration;
using System.Web;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 
	/// </summary>
	public static class BooleanExtension
	{
		#region Basic<T> 扩展
		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="func">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		private static bool BooleanValueFor<TM>(this BasicContext<TM> html, Func<TM, bool> func)
		{
			object model = html.GetValue<bool>(func);
			return model != null ? (bool)model : false;
		}

		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		private static string PrivateBooleanFor<TM>(this BasicContext<TM> html, Expression<Func<TM, bool>> expression)
		{
			EntityPropertyMeta metadata = LambdaHelper.GetProperty(expression);
			Func<TM, bool> func = expression.Compile();

			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			string displayName = metadata.DisplayName;

			string trueText = "True", falseText = "False";
			bool check = html.BooleanValueFor(func); string text = null;

			if (metadata.Display != null)
			{
				WebDisplayAttribute wda = metadata.Display;
				string converterName = wda.ConverterName;
				if (check)
				{
					if (string.IsNullOrWhiteSpace(converterName))
						text = html.GetString(string.Concat(wda.DisplayName, "_TrueText"));
					else
						text = html.GetString(converterName, string.Concat(wda.DisplayName, "_TrueText"));
				}
				else
				{
					if (string.IsNullOrWhiteSpace(converterName))
						text = html.GetString(string.Concat(wda.DisplayName, "_FalseText"));
					else
						text = html.GetString(converterName, string.Concat(wda.DisplayName, "_FalseText"));
				}
			}
			return text ?? (check ? trueText : falseText);
		}

		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		public static MvcHtmlString BooleanFor<TM>(this BasicContext<TM> html, Expression<Func<TM, bool>> expression)
		{
			return MvcHtmlString.Create(html.AttributeEncode(html.PrivateBooleanFor<TM>(expression)));
		}

		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <param name="trueExpression">如果Lambda表达式结果为 True 时显示的属性信息。</param>
		/// <param name="falseExpression">如果Lambda表达式结果为 False 时显示的属性信息。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		public static MvcHtmlString BooleanFor<TM>(this BasicContext<TM> html, Expression<Func<TM, bool>> expression,
			Expression<Func<TM, string>> trueExpression, Expression<Func<TM, string>> falseExpression)
		{
			Func<TM, bool> func = expression.Compile();
			bool modelValue = html.BooleanValueFor<TM>(func);
			string text = html.PrivateBooleanFor<TM>(expression);
			string resultText = text;
			if (modelValue)
			{
				string trueValue = html.GetValue(trueExpression.Compile());
				if (string.IsNullOrEmpty(trueValue)) { resultText = text; }
				else { resultText = string.Concat(text, "=>", trueValue); }
			}
			else
			{
				string falseValue = html.GetValue(falseExpression.Compile());
				if (string.IsNullOrEmpty(falseValue)) { resultText = text; }
				else { resultText = string.Concat(text, "=>", falseValue); }
			}
			return MvcHtmlString.Create(html.AttributeEncode(resultText));
		}
		#endregion

		#region HtmlHelper 扩展
		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		private static bool BooleanValueFor<TM>(this HtmlHelper<TM> html, Expression<Func<TM, bool>> expression)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, bool>(expression, html.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			string displayName = metadata.DisplayName;
			PropertyInfo propertyInfo = metadata.ContainerType.GetProperty(metadata.PropertyName);
			bool check = false;
			if (metadata.Model != null && metadata.Model is bool)
				check = (bool)metadata.Model;
			return check;
		}

		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		private static string PrivateBooleanFor<TM>(this HtmlHelper<TM> html, Expression<Func<TM, bool>> expression)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, bool>(expression, html.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			string displayName = metadata.DisplayName;
			PropertyInfo propertyInfo = metadata.ContainerType.GetProperty(metadata.PropertyName);
			string trueText = "True", falseText = "False";
			bool check = false; string text = null;
			if (metadata.Model != null && metadata.Model is bool)
				check = (bool)metadata.Model;

			if (propertyInfo.IsDefined(typeof(WebDisplayAttribute), true))
			{
				object[] attributes = propertyInfo.GetCustomAttributes(typeof(WebDisplayAttribute), true);
				foreach (WebDisplayAttribute wda in attributes)
				{
					string converterName = wda.ConverterName;
					if (check)
					{
						if (string.IsNullOrWhiteSpace(converterName))
							text = html.GetString(string.Concat(wda.DisplayName, "_TrueText"));
						else
							text = html.GetString(converterName, string.Concat(wda.DisplayName, "_TrueText"));
					}
					else
					{
						if (string.IsNullOrWhiteSpace(converterName))
							text = html.GetString(string.Concat(wda.DisplayName, "_FalseText"));
						else
							text = html.GetString(converterName, string.Concat(wda.DisplayName, "_FalseText"));
					}
				}
			}
			return text ?? (check ? trueText : falseText);
		}

		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		public static MvcHtmlString BooleanFor<TM>(this HtmlHelper<TM> html, Expression<Func<TM, bool>> expression)
		{
			return MvcHtmlString.Create(html.AttributeEncode(html.PrivateBooleanFor<TM>(expression)));
		}

		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <param name="trueExpression">如果Lambda表达式结果为 True 时显示的属性信息。</param>
		/// <param name="falseExpression">如果Lambda表达式结果为 False 时显示的属性信息。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		public static MvcHtmlString BooleanFor<TM>(this HtmlHelper<TM> html, Expression<Func<TM, bool>> expression,
			Expression<Func<TM, object>> trueExpression, Expression<Func<TM, object>> falseExpression)
		{
			bool modelValue = html.BooleanValueFor<TM>(expression);
			string text = html.PrivateBooleanFor<TM>(expression);
			string resultText = text;
			if (modelValue)
			{
				ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, object>(trueExpression, html.ViewData);
				if (metadata.Model == null) { resultText = text; }
				else { resultText = string.Concat(text, "=>", metadata.Model); }
			}
			else
			{
				ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, object>(falseExpression, html.ViewData);
				if (metadata.Model == null) { resultText = text; }
				else { resultText = string.Concat(text, "=>", metadata.Model); }
			}
			return MvcHtmlString.Create(html.AttributeEncode(resultText));
		}
		#endregion

		#region Controller扩展
		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="controller">此方法扩展的 ControllerBase 实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		private static bool BooleanValueFor<TM>(this ControllerBase controller, Expression<Func<TM, bool>> expression)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, bool>(expression, new ViewDataDictionary<TM>(controller.ViewData));
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			string displayName = metadata.DisplayName;
			PropertyInfo propertyInfo = metadata.ContainerType.GetProperty(metadata.PropertyName);
			bool check = false;
			if (metadata.Model != null && metadata.Model is bool)
				check = (bool)metadata.Model;
			return check;
		}

		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="controller">此方法扩展的 ControllerBase 实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		public static string BooleanFor<TM>(this ControllerBase controller, Expression<Func<TM, bool>> expression)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, bool>(expression, new ViewDataDictionary<TM>(controller.ViewData));
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			string displayName = metadata.DisplayName;
			PropertyInfo propertyInfo = metadata.ContainerType.GetProperty(metadata.PropertyName);
			string trueText = "True", falseText = "False";
			bool check = false; string text = null;
			if (metadata.Model != null && metadata.Model is bool)
				check = (bool)metadata.Model;

			if (propertyInfo.IsDefined(typeof(WebDisplayAttribute), true))
			{
				object[] attributes = propertyInfo.GetCustomAttributes(typeof(WebDisplayAttribute), true);
				foreach (WebDisplayAttribute wda in attributes)
				{
					string converterName = wda.ConverterName;
					if (check)
					{
						if (string.IsNullOrWhiteSpace(converterName))
							text = controller.GetString(string.Concat(wda.DisplayName, "_TrueText"));
						else
							text = controller.GetString(converterName, string.Concat(wda.DisplayName, "_TrueText"));
					}
					else
					{
						if (string.IsNullOrWhiteSpace(converterName))
							text = controller.GetString(string.Concat(wda.DisplayName, "_FalseText"));
						else
							text = controller.GetString(converterName, string.Concat(wda.DisplayName, "_FalseText"));
					}
				}
			}
			return text ?? (check ? trueText : falseText);
		}


		/// <summary>提供一种机制，以创建与 ASP.NET MVC 模型联编程序和模板兼容的自定义 HTML 标记。</summary>
		/// <param name="controller">此方法扩展的 ControllerBase 实例。</param>
		/// <param name="model">模型实例。</param>
		/// <param name="expression">一个表达式，用于标识包含要公开的属性的对象。</param>
		/// <typeparam name="TM">模型。</typeparam>
		/// <returns>值的 HTML 标记。</returns>
		public static string BooleanFor<TM>(this ControllerBase controller, TM model, Expression<Func<TM, bool>> expression)
		{
			bool check = expression.Compile().Invoke(model);
			EntityPropertyMeta propertyInfo = LambdaHelper.GetProperty(expression);
			string trueText = "True", falseText = "False";

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (check)
				{
					if (string.IsNullOrWhiteSpace(converterName))
						trueText = controller.GetString(string.Concat(wda.DisplayName, "_TrueText"));
					else
						trueText = controller.GetString(converterName, string.Concat(wda.DisplayName, "_TrueText"));
				}
				else
				{
					if (string.IsNullOrWhiteSpace(converterName))
						falseText = controller.GetString(string.Concat(wda.DisplayName, "_FalseText"));
					else
						falseText = controller.GetString(converterName, string.Concat(wda.DisplayName, "_FalseText"));
				}
			}
			return check ? trueText : falseText;
		}
		#endregion
	}
}
