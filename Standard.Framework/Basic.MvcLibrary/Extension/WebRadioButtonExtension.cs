using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 自定义RadioButton扩展方法
	/// </summary>
	public static class WebRadioButtonExtensions
	{
		private static void InitConfiguration(this HtmlHelper htmlHelper, IHtmlAttrs htmlAttributes)
		{
            if (htmlAttributes != null) { htmlAttributes.className = "web-radiobutton"; }
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的单选按钮 input 元素。 
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="value">如果选择此单选按钮，则为在发送窗体时提交的此单选按钮的值。 如果 ViewDataDictionary 或 ModelStateDictionary 对象中选定的单选按钮的值与此值匹配，则选择此单选按钮</param>
		/// <exception cref="System.ArgumentNullException">value 参数为 null。</exception>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“radio”。</returns>
		public static MvcHtmlString WebRadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value)
		{
			return htmlHelper.WebRadioButtonFor<TModel, TProperty>(expression, value, null);
		}

		/// <summary>
		/// 使用指定 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的单选按钮 input 元素。 
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="value">如果选择此单选按钮，则为在发送窗体时提交的此单选按钮的值。 如果 ViewDataDictionary 或 ModelStateDictionary 对象中选定的单选按钮的值与此值匹配，则选择此单选按钮</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <exception cref="System.ArgumentNullException">value 参数为 null。</exception>
		/// <returns>一个 HTML input 元素，使用指定的 HTML 特性将其 type 特性针对指定表达式表示的对象中的每个属性均设置为“radio”。</returns>
		public static MvcHtmlString WebRadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, IHtmlAttrs htmlAttributes)
		{
			htmlHelper.InitConfiguration(htmlAttributes);
			return htmlHelper.RadioButtonFor<TModel, TProperty>(expression, value, htmlAttributes);
		}
	}
}
