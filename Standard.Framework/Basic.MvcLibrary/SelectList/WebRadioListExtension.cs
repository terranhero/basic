using System;
using System.Collections;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Globalization;
using System.Reflection;
using Basic.EntityLayer;
using Basic.Messages;

namespace Basic.MvcLibrary
{
    /// <summary>
    /// 自定义RadioButtonList控件扩展方法
    /// </summary>
    public static class WebRadioListExtension
    {
        #region 输出Yes Or No单选按钮列表。
        /// <summary>
        /// 通过使用指定的 HTML 帮助器、返回Yes Or No单选按钮列表。 
        /// </summary>
        /// <typeparam name="TM">模型的类型。</typeparam>
        /// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
        /// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
        /// <returns>两个 input 元素，其 type 特性设置为“radio”。</returns>
        public static MvcHtmlString WebYesOrNoFor<TM>(this HtmlHelper<TM> html, Expression<Func<TM, bool>> expression)
        {
            return html.WebYesOrNoFor<TM>(expression, new HtmlAttrs());
        }

        /// <summary>
        /// 通过使用指定的 HTML 帮助器、返回Yes Or No单选按钮列表。 
        /// </summary>
        /// <typeparam name="TM">模型的类型。</typeparam>
        /// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
        /// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
        /// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
        /// <returns>两个 input 元素，其 type 特性设置为“radio”。</returns>
        public static MvcHtmlString WebYesOrNoFor<TM>(this HtmlHelper<TM> html, Expression<Func<TM, bool>> expression, IHtmlAttrs htmlAttrs)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, bool>(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string displayName = metadata.DisplayName;
            PropertyInfo propertyInfo = metadata.ContainerType.GetProperty(metadata.PropertyName);
            string trueText = "True", falseText = "False";
            if (propertyInfo.IsDefined(typeof(WebDisplayAttribute), true))
            {
                System.Web.HttpRequestBase request = html.ViewContext.HttpContext.Request;
                object[] attributes = propertyInfo.GetCustomAttributes(typeof(WebDisplayAttribute), true);
                foreach (WebDisplayAttribute wda in attributes)
                {

                    string converterName = wda.ConverterName;
                    if (string.IsNullOrWhiteSpace(converterName))
                        trueText = MessageCulture.GetString(request, string.Concat(wda.DisplayName, "_TrueText"));
                    else
                        trueText = MessageCulture.GetString(request, converterName, string.Concat(wda.DisplayName, "_TrueText"));

                    if (string.IsNullOrWhiteSpace(converterName))
                        falseText = MessageCulture.GetString(request, string.Concat(wda.DisplayName, "_FalseText"));
                    else
                        falseText = MessageCulture.GetString(request, converterName, string.Concat(wda.DisplayName, "_FalseText"));
                }
            }
            bool check = false;
            if (metadata.Model != null && metadata.Model is char)
                check = Convert.ToChar(metadata.Model) == 'Y';
            else if (metadata.Model != null && metadata.Model is byte)
                check = Convert.ToByte(metadata.Model) > 0;
            else if (metadata.Model != null && metadata.Model is short)
                check = Convert.ToInt16(metadata.Model) > 0;
            else if (metadata.Model != null && metadata.Model is int)
                check = Convert.ToInt32(metadata.Model) > 0;
            else if (metadata.Model != null && metadata.Model is bool)
                check = (bool)metadata.Model;
            SelectListItem itemYes = new SelectListItem();
            itemYes.Text = trueText ?? "True";
            itemYes.Value = "1";
            SelectListItem itemNo = new SelectListItem();
            itemNo.Text = falseText ?? "False";
            itemNo.Value = "0";
            RadioButtonList list = new RadioButtonList(new SelectListItem[] { itemYes, itemNo }, "Value", "Text", check ? 1 : 0);
            return html.WebRadioListFor<TM, bool>(expression, list, htmlAttrs);
        }

        /// <summary>
        /// 通过使用指定的 HTML 帮助器、返回Yes Or No单选按钮列表。 
        /// </summary>
        /// <typeparam name="TM">模型的类型。</typeparam>
        /// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
        /// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
        /// <param name="yesCode">Yes按钮文本编码</param>
        /// <param name="noCode">No按钮文本编码</param>
        /// <returns>两个 input 元素，其 type 特性设置为“radio”。</returns>
        public static MvcHtmlString WebYesOrNoFor<TM>(this HtmlHelper<TM> html, Expression<Func<TM, bool>> expression, string yesCode, string noCode)
        {
            return html.WebYesOrNoFor<TM>(expression, yesCode, noCode, null);
        }

        /// <summary>
        /// 通过使用指定的 HTML 帮助器、返回Yes Or No单选按钮列表。 
        /// </summary>
        /// <typeparam name="TM">模型的类型。</typeparam>
        /// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
        /// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
        /// <param name="yesCode">Yes按钮文本编码</param>
        /// <param name="noCode">No按钮文本编码</param>
        /// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
        /// <returns>两个 input 元素，其 type 特性设置为“radio”。</returns>
        public static MvcHtmlString WebYesOrNoFor<TM>(this HtmlHelper<TM> html, Expression<Func<TM, bool>> expression,
            string yesCode, string noCode, IHtmlAttrs htmlAttrs)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TM, bool>(expression, html.ViewData);
            bool check = false;
            if (metadata.Model != null && metadata.Model is char)
                check = Convert.ToChar(metadata.Model) == 'Y';
            else if (metadata.Model != null && metadata.Model is byte)
                check = Convert.ToByte(metadata.Model) > 0;
            else if (metadata.Model != null && metadata.Model is short)
                check = Convert.ToInt16(metadata.Model) > 0;
            else if (metadata.Model != null && metadata.Model is int)
                check = Convert.ToInt32(metadata.Model) > 0;
            else if (metadata.Model != null && metadata.Model is bool)
                check = (bool)metadata.Model;
            SelectListItem itemYes = new SelectListItem();
            itemYes.Text = MessageCulture.GetString(html.ViewContext.HttpContext.Request, yesCode) ?? "True";
            itemYes.Value = "1";
            SelectListItem itemNo = new SelectListItem();
            itemNo.Text = MessageCulture.GetString(html.ViewContext.HttpContext.Request, noCode) ?? "False";
            itemNo.Value = "0";
            RadioButtonList list = new RadioButtonList(new SelectListItem[] { itemYes, itemNo }, "Value", "Text", check ? 1 : 0);
            return html.WebRadioListFor<TM, bool>(expression, list, htmlAttrs);
        }
        #endregion

        /// <summary>
        /// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回复选框 input 元素列表。 
        /// </summary>
        /// <typeparam name="TM">模型的类型。</typeparam>
        /// <typeparam name="TP">值的类型。</typeparam>
        /// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
        /// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
        /// <returns>一组 input 元素，其 type 特性设置为“checkbox”。</returns>
        public static MvcHtmlString WebRadioListFor<TM, TP>(this HtmlHelper<TM> helper, Expression<Func<TM, TP>> expression)
        {
            return helper.WebRadioListFor(expression, null, null);
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
        public static MvcHtmlString WebRadioListFor<TM, TP>(this HtmlHelper<TM> helper,
            Expression<Func<TM, TP>> expression, RadioButtonList selectList)
        {
            return helper.WebRadioListFor(expression, selectList, null);
        }

        /// <summary>
        /// 通过使用指定的 HTML 帮助器、窗体字段的名称、指定列表项、选项标签和指定的 HTML 特性，返回复选框 input 元素列表。 
        /// </summary>
        /// <typeparam name="TM">模型的类型。</typeparam>
        /// <typeparam name="TP">值的类型。</typeparam>
        /// <param name="helper">此方法扩展的 HTML 帮助器实例。</param>
        /// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
        /// <param name="selectList">一个用于填充下拉列表的 SelectListItem 对象的集合。</param>
        /// <param name="htmlAttributes">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
        /// <returns>一组 input 元素，其 type 特性设置为“checkbox”。</returns>
        public static MvcHtmlString WebRadioListFor<TM, TP>(this HtmlHelper<TM> helper,
            Expression<Func<TM, TP>> expression, RadioButtonList selectList, IHtmlAttrs htmlAttributes)
        {
            string name = ExpressionHelper.GetExpressionText(expression);
            if (selectList == null)
            {
                if (helper.ViewData != null)
                    selectList = helper.ViewData.Eval(name) as RadioButtonList;
                if (selectList == null)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "{0}缺少{1}类型的数据源.", name, "RadioButtonList"));
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
                    builder.Append(helper.ListItemToItem(fullHtmlFieldName, item, selectList.RepeatLayout, htmlAttributes));
                    index++;
                }
            }
            TagBuilder tagBuilder = new TagBuilder("span") { InnerHtml = builder.ToString() };
            tagBuilder.MergeAttribute("class", "web-radiolist", true);
            tagBuilder.GenerateId(name);
            ModelState state;
            if (helper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out state) && (state.Errors.Count > 0))
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        private static string ListItemToItem(this HtmlHelper helper, string name, SelectListItem item, RepeatLayout layout, IHtmlAttrs htmlAttributes)
        {
            TagBuilder labelBuilder = new TagBuilder("label") { InnerHtml = helper.Encode(item.Text) };
            string checkId = string.Format("{0}_{1}", name, item.Value);
            labelBuilder.MergeAttribute("For", checkId);
            TagBuilder checkBuilder = new TagBuilder("input");
            checkBuilder.MergeAttributes<string, object>(htmlAttributes);
            checkBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Radio));
            checkBuilder.GenerateId(checkId);
            if (item.Selected)
                checkBuilder.MergeAttribute("checked", "checked");
            checkBuilder.MergeAttribute("name", name);
            checkBuilder.MergeAttribute("value", item.Value);
            checkBuilder.AddCssClass("web-radiobutton");
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(checkBuilder.ToString(TagRenderMode.SelfClosing));
            stringBuilder.Append(labelBuilder.ToString(TagRenderMode.Normal));
            TagBuilder builder = new TagBuilder("span") { InnerHtml = stringBuilder.ToString() };
            return builder.ToString(TagRenderMode.Normal);
        }
    }
}
