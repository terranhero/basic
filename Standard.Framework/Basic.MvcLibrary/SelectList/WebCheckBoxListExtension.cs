using System;
using System.Collections;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Globalization;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 自定义CheckBoxList控件扩展方法
	/// </summary>
	public static class WebCheckBoxListExtension
	{
		/// <summary>
		/// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回复选框 input 元素列表。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一组 input 元素，其 type 特性设置为“checkbox”。</returns>
		public static MvcHtmlString WebCheckBoxListFor<TM, TP>(this HtmlHelper<TM> helper, Expression<Func<TM, TP>> expression)
		{
			return helper.WebCheckBoxListFor(expression, null, null);
		}

		/// <summary>
		/// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回复选框 input 元素列表。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="selectList">一个用于填充下拉列表的 SelectListItem 对象的集合。</param>
		/// <returns>一组 input 元素，其 type 特性设置为“checkbox”。</returns>
		public static MvcHtmlString WebCheckBoxListFor<TM, TP>(this HtmlHelper<TM> helper,
			Expression<Func<TM, TP>> expression, CheckBoxList selectList)
		{
			return helper.WebCheckBoxListFor(expression, selectList, null);
		}

		/// <summary>
		/// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回复选框 input 元素列表。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <param name="selectList">一个用于填充下拉列表的 SelectListItem 对象的集合。</param>
		/// <param name="inputAttrs">一个 IHtmlAttrs 对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一组 input 元素，其 type 特性设置为“checkbox”。</returns>
		public static MvcHtmlString WebCheckBoxListFor<TM, TP>(this HtmlHelper<TM> helper,
			Expression<Func<TM, TP>> expression, CheckBoxList selectList, IHtmlAttrs inputAttrs)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			if (selectList == null)
			{
				if (helper.ViewData != null)
					selectList = helper.ViewData.Eval(name) as CheckBoxList;
				if (selectList == null)
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "{0}缺少{1}类型的数据源.", name, "CheckBoxList"));
			}
			string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			StringBuilder builder = new StringBuilder();
			if (selectList != null)
			{
				int repeatColumns = selectList.RepeatColumns;
				int index = 0;
				foreach (SelectListItem item in selectList)
				{
					if (repeatColumns > 0 && index > 0 && index % repeatColumns == 0)
						builder.AppendLine("<br/>");
					builder.Append(helper.ListItemToItem(fullHtmlFieldName, item, selectList.RepeatLayout, inputAttrs));
					index++;
				}
			}
			TagBuilder tagBuilder = new TagBuilder("ul") { InnerHtml = builder.ToString() };
			tagBuilder.AddCssClass("web-checkboxes");
			tagBuilder.GenerateId(name);
			ModelState state;
			if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out state) && (state.Errors.Count > 0))
				tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
			return MvcHtmlString.Create(tagBuilder.ToString());
		}

		/// <summary>
		/// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回复选框 input 元素列表。 
		/// </summary>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">一个字符串对象，表示当前输出元素的名称。</param>
		/// <returns>一组 input 元素，其 type 特性设置为“checkbox”。</returns>
		public static MvcHtmlString WebCheckBoxList(this HtmlHelper helper, string name)
		{
			return helper.WebCheckBoxList(name, null, null);
		}

		/// <summary>
		/// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回复选框 input 元素列表。 
		/// </summary>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">一个字符串对象，表示当前输出元素的名称。</param>
		/// <param name="checkes">一个用于填充下拉列表的 CheckBoxList 对象。</param>
		/// <returns>一组 input 元素，其 type 特性设置为“checkbox”。</returns>
		public static MvcHtmlString WebCheckBoxList(this HtmlHelper helper, string name, CheckBoxList checkes)
		{
			return helper.WebCheckBoxList(name, checkes, null);
		}

		/// <summary>
		/// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回复选框 input 元素列表。 
		/// </summary>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="name">一个字符串对象，表示当前输出元素的名称。</param>
		/// <param name="checkes">一个用于填充下拉列表的 CheckBoxList 对象。</param>
		/// <param name="inputAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一组 input 元素，其 type 特性设置为“checkbox”。</returns>
		public static MvcHtmlString WebCheckBoxList(this HtmlHelper helper, string name, CheckBoxList checkes, IHtmlAttrs inputAttrs)
		{
			if (checkes == null)
			{
				if (helper.ViewData != null)
					checkes = helper.ViewData.Eval(name) as CheckBoxList;
				if (checkes == null)
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "{0}缺少{1}类型的数据源.", name, "CheckBoxList"));
			}
			string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			StringBuilder builder = new StringBuilder();
			if (checkes != null)
			{
				int repeatColumns = checkes.RepeatColumns;
				int index = 0;
				foreach (SelectListItem item in checkes)
				{
					if (repeatColumns > 0 && index > 0 && index % repeatColumns == 0)
						builder.AppendLine("<br/>");
					builder.Append(helper.ListItemToItem(fullHtmlFieldName, item, checkes.RepeatLayout, inputAttrs));
					index++;
				}
			}
			TagBuilder tagBuilder = new TagBuilder("ul") { InnerHtml = builder.ToString() };
			tagBuilder.AddCssClass("web-checkboxes");
			tagBuilder.GenerateId(name);
			ModelState state;
			if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out state) && (state.Errors.Count > 0))
				tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
			return MvcHtmlString.Create(tagBuilder.ToString());
		}

		private static string ListItemToItem(this HtmlHelper helper, string name, SelectListItem item, RepeatLayout layout, IHtmlAttrs inputAttrs)
		{
			TagBuilder labelBuilder = new TagBuilder("label") { InnerHtml = helper.Encode(item.Text) };
			string checkId = string.Format("{0}_{1}", name, item.Value);
			labelBuilder.MergeAttribute("For", checkId);
			TagBuilder checkBuilder = new TagBuilder("input");
			if (inputAttrs != null) { checkBuilder.MergeAttributes<string, object>(inputAttrs); }
			checkBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.CheckBox));
			checkBuilder.GenerateId(checkId);
			if (item.Selected)
				checkBuilder.MergeAttribute("checked", "checked");
			checkBuilder.MergeAttribute("name", name);
			checkBuilder.MergeAttribute("value", item.Value);

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(checkBuilder.ToString(TagRenderMode.SelfClosing));
			stringBuilder.Append(labelBuilder.ToString(TagRenderMode.Normal));
			TagBuilder builder = new TagBuilder("li") { InnerHtml = stringBuilder.ToString() };
			return builder.ToString(TagRenderMode.Normal);
		}
	}
}
