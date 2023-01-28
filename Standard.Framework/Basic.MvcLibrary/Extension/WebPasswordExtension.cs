using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 扩展 Password 控件输出
	/// </summary>
	public static class PasswordExtension
	{
		/// <summary>
		///  使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的密码 input 元素。
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <exception cref="System.ArgumentNullException">expression 参数为 null。</exception>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“password”。</returns>
		public static MvcHtmlString WebPasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
		{
			return htmlHelper.WebPasswordFor<TModel, TProperty>(expression, null);
		}

		/// <summary>
		///  使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的密码 input 元素。
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <exception cref="System.ArgumentNullException">expression 参数为 null。</exception>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“password”。</returns>
		public static MvcHtmlString WebPasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IHtmlAttrs htmlAttrs)
		{
			if (htmlAttrs == null) { htmlAttrs = new HtmlAttrs(5); }
			if (htmlAttrs.ContainsKey("class")) { htmlAttrs.AddCssClass("web-password"); }
			return htmlHelper.PasswordFor<TModel, TProperty>(expression, htmlAttrs);
		}
	}
}
