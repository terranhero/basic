using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 自定义Select控件扩展方法
	/// </summary>
	public static class WebSelectExtension
	{
		/// <summary>
		/// 使用指定列表项、选项标签和 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的 HTML select 元素。 
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <exception cref="ArgumentNullException">expression 参数为 null 或为空。</exception>
		/// <returns>由表达式表示的对象中的每个属性对应的 HTML select 元素。</returns>
		public static MvcHtmlString WebSelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression)
		{
			return htmlHelper.WebSelectFor<TModel, TProperty>(expression, null, null, null);
		}

		/// <summary>
		/// 使用指定列表项、选项标签和 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的 HTML select 元素。 
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="selectList">一个用于填充下拉列表的 SelectListItem 对象的集合。</param>
		/// <exception cref="ArgumentNullException">expression 参数为 null 或为空。</exception>
		/// <returns>由表达式表示的对象中的每个属性对应的 HTML select 元素。</returns>
		public static MvcHtmlString WebSelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression, SelectList selectList)
		{
			return htmlHelper.WebSelectFor<TModel, TProperty>(expression, selectList, null, null);
		}

		/// <summary>
		/// 使用指定列表项、选项标签和 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的 HTML select 元素。 
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <exception cref="ArgumentNullException">expression 参数为 null 或为空。</exception>
		/// <returns>由表达式表示的对象中的每个属性对应的 HTML select 元素。</returns>
		public static MvcHtmlString WebSelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression, IHtmlAttrs htmlAttributes)
		{
			return htmlHelper.WebSelectFor<TModel, TProperty>(expression, null, null, htmlAttributes);
		}

		/// <summary>
		/// 使用指定列表项、选项标签和 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的 HTML select 元素。 
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="selectList">一个用于填充下拉列表的 SelectListItem 对象的集合。</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <exception cref="ArgumentNullException">expression 参数为 null 或为空。</exception>
		/// <returns>由表达式表示的对象中的每个属性对应的 HTML select 元素。</returns>
		public static MvcHtmlString WebSelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression, SelectList selectList, IHtmlAttrs htmlAttributes)
		{
			return htmlHelper.WebSelectFor<TModel, TProperty>(expression, selectList, null, htmlAttributes);
		}

		/// <summary>
		/// 使用指定列表项、选项标签和 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的 HTML select 元素。 
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="selectList">一个用于填充下拉列表的 SelectListItem 对象的集合。</param>
		/// <param name="nulloption">是否添加空行。</param>
		/// <exception cref="ArgumentNullException">expression 参数为 null 或为空。</exception>
		/// <returns>由表达式表示的对象中的每个属性对应的 HTML select 元素。</returns>
		public static MvcHtmlString WebSelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression, SelectList selectList, bool nulloption)
		{
			return htmlHelper.WebSelectFor<TModel, TProperty>(expression, selectList, nulloption, null);
		}


		/// <summary>
		/// 使用指定列表项、选项标签和 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的 HTML select 元素。 
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="selectList">一个用于填充下拉列表的 SelectListItem 对象的集合。</param>
		/// <param name="nulloption">是否添加空行。</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <exception cref="ArgumentNullException">expression 参数为 null 或为空。</exception>
		/// <returns>由表达式表示的对象中的每个属性对应的 HTML select 元素。</returns>
		public static MvcHtmlString WebSelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression, SelectList selectList, bool nulloption, IHtmlAttrs htmlAttributes)
		{
			//MvcConfiguration.InitDefaultConfiguration("web-select", ref htmlAttributes);
			if (htmlAttributes == null) { htmlAttributes = new HtmlAttrs(); }
			htmlAttributes.className = "web-select";
			if (nulloption)
				return htmlHelper.DropDownListFor<TModel, TProperty>(expression, selectList, "", htmlAttributes);
			return htmlHelper.DropDownListFor<TModel, TProperty>(expression, selectList, htmlAttributes);
		}

		/// <summary>
		/// 使用指定列表项、选项标签和 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的 HTML select 元素。 
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="selectList">一个用于填充下拉列表的 SelectListItem 对象的集合。</param>
		/// <param name="optionLabel">默认空项的文本。 此参数可以为 null。</param>
		/// <exception cref="ArgumentNullException">expression 参数为 null 或为空。</exception>
		/// <returns>由表达式表示的对象中的每个属性对应的 HTML select 元素。</returns>
		public static MvcHtmlString WebSelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression, SelectList selectList, string optionLabel)
		{
			return htmlHelper.WebSelectFor<TModel, TProperty>(expression, selectList, optionLabel, null);
		}


		/// <summary>
		/// 使用指定列表项、选项标签和 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的 HTML select 元素。 
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="selectList">一个用于填充下拉列表的 SelectListItem 对象的集合。</param>
		/// <param name="optionLabel">默认空项的文本。 此参数可以为 null。</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <exception cref="ArgumentNullException">expression 参数为 null 或为空。</exception>
		/// <returns>由表达式表示的对象中的每个属性对应的 HTML select 元素。</returns>
		public static MvcHtmlString WebSelectFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression, SelectList selectList, string optionLabel, IHtmlAttrs htmlAttributes)
		{
			//MvcConfiguration.InitDefaultConfiguration("web-select", ref htmlAttributes);
			if (htmlAttributes == null) { htmlAttributes = new HtmlAttrs(); }
			htmlAttributes.className = "web-select";
			return htmlHelper.DropDownListFor<TModel, TProperty>(expression, selectList, optionLabel, htmlAttributes);
		}

		/// <summary>
		/// 使用指定列表项、选项标签和 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的 HTML select 元素。 
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="selectList">一个用于填充下拉列表的 SelectListItem 对象的集合。</param>
		/// <exception cref="ArgumentNullException">expression 参数为 null 或为空。</exception>
		/// <returns>由表达式表示的对象中的每个属性对应的 HTML select 元素。</returns>
		public static MvcHtmlString WebListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, SelectList selectList)
		{
			return htmlHelper.WebListBoxFor<TModel, TProperty>(expression, selectList, null);
		}

		/// <summary>
		/// 使用指定列表项、选项标签和 HTML 特性，为由指定表达式表示的对象中的每个属性返回对应的 HTML select 元素。 
		/// </summary>
		/// <typeparam name="TModel">模型的类型。</typeparam>
		/// <typeparam name="TProperty">值的类型。</typeparam>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="selectList">一个用于填充下拉列表的 SelectListItem 对象的集合。</param>
		/// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <exception cref="ArgumentNullException">expression 参数为 null 或为空。</exception>
		/// <returns>由表达式表示的对象中的每个属性对应的 HTML select 元素。</returns>
		public static MvcHtmlString WebListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, SelectList selectList, IHtmlAttrs htmlAttributes)
		{
			if (htmlAttributes == null) { htmlAttributes = new HtmlAttrs(); }
			htmlAttributes.className = "web-select";
			return htmlHelper.ListBoxFor<TModel, TProperty>(expression, selectList, htmlAttributes);
		}
	}
}
