using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 自定义时间戳控件扩展方法
	/// </summary>
	public static class WebTimeStampExtension
	{
		/// <summary>
		/// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回复选框 input 元素列表。 
		/// </summary>
		/// <typeparam name="TM">模型的类型。</typeparam>
		/// <typeparam name="TP">值的类型。</typeparam>
		/// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>一组 input 元素，其 type 特性设置为“checkbox”。</returns>
		public static MvcHtmlString WebTimeStampFor<TM, TP>(this HtmlHelper<TM> helper, Expression<Func<TM, TP>> expression)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression<TM, TP>(expression, helper.ViewData);
			string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			string str2 = helper.FormatValue(modelMeta.Model, "{0:yyyy-MM-dd HH:mm:ss.fff}");

			TagBuilder tagBuilder = new TagBuilder("input");
			tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Hidden));
			tagBuilder.GenerateId(fullHtmlFieldName);
			tagBuilder.MergeAttribute("name", fullHtmlFieldName);
			tagBuilder.MergeAttribute("value", str2, true);
			tagBuilder.MergeAttribute("format-string", "yyyy-MM-dd HH:mm:ss.f");
			tagBuilder.MergeAttributes<string, object>(helper.GetUnobtrusiveValidationAttributes(name, modelMeta));
			return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
		}
	}
}
