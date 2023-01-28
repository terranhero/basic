using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Basic.EntityLayer;
using Basic.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Basic.MvcLibrary
{
	/// <summary>表示 select 选择器(默认使用原生控件)</summary>
	public class Select : Input<Select>
	{
		private readonly IBasicContext basicContext;
		private readonly EntityPropertyMeta propertyMeta;
		private readonly string OptionTagName = InputTags.Option;
		private readonly string GroupTagName = InputTags.OptionGroup;
		/// <summary>初始化 Select 类实例</summary>
		/// <param name="basic">表示请求上下文信息</param>
		/// <param name="meta">表示属性的元数据</param>
		internal protected Select(IBasicContext basic, EntityPropertyMeta meta)
			: base(basic, meta, InputTags.Select) { basicContext = basic; propertyMeta = meta; base.AddClass("web-select"); }

		/// <summary>初始化 Select 类实例</summary>
		/// <param name="basic">表示请求上下文信息</param>
		/// <param name="meta">表示属性的元数据</param>
		/// <param name="useElement">使用 element-ui 控件下拉选择</param>
		internal protected Select(IBasicContext basic, EntityPropertyMeta meta, bool useElement)
			: base(basic, meta, useElement ? InputTags.ElSelect : InputTags.Select)
		{
			basicContext = basic; propertyMeta = meta; base.AddClass("web-select");
			if (useElement) { OptionTagName = InputTags.ElOption; GroupTagName = InputTags.ElOptionGroup; }
		}

		/// <summary>初始化空实例，此不输出。</summary>
		private Select(bool empty) : base(empty) { }

		/// <summary>表示空实例</summary>
		public static Select Empty { get { return new Select(true); } }

		/// <summary>输出选择项信息</summary>
		/// <param name="nullOption">是否添加空白行</param>
		/// <returns>返回当前对象。</returns>
		public Select Options(bool nullOption)
		{
			return Options(nullOption ? string.Empty : null);
		}

		/// <summary>输出选择项信息</summary>
		/// <returns>返回当前对象。</returns>
		public Select Options(string label = null)
		{
			if (mEmpty == true) { return this; }
			IDictionary<string, object> datas = basicContext.ViewData;
			if (datas.TryGetValue(Key, out object items) == true)
			{
				StringBuilder builder = new StringBuilder(1000);
				if (label != null)
				{
					TagHtmlWriter writer = new TagHtmlWriter(OptionTagName);
					writer.MergeAttribute("value", "");
					writer.SetInnerText(label);
					builder.AppendLine(writer.ToString());
				}
				if (items is IEnumerable<SelectListItem> list)
				{
					StringBuilder builder1 = Options(list);
					builder.AppendLine(builder1.ToString());
				}
				else if (items is IEnumerable<ISelectOption> options)
				{
					foreach (ISelectOption item in options)
					{
						builder.AppendLine(ItemToOption(OptionTagName, item));
					}
				}
				InnerHtml = (builder.ToString());
			}
			else if (propertyMeta.PropertyType.IsEnum)
			{
				StringBuilder builder = new StringBuilder(1000);
				if (label != null)
				{
					TagHtmlWriter writer = new TagHtmlWriter(OptionTagName);
					writer.MergeAttribute("value", "");
					writer.SetInnerText(label);
					builder.AppendLine(writer.ToString());
				}
				string converterName = null; Type enumType = propertyMeta.PropertyType;
				WebDisplayConverterAttribute wdca = enumType.GetCustomAttribute<WebDisplayConverterAttribute>();
				if (wdca != null) { converterName = wdca.ConverterName; }

				Dictionary<string, MemberInfo> members = enumType.GetMembers(BindingFlags.Public | BindingFlags.Static).ToDictionary(m => m.Name);
				string enumName = enumType.Name;
				Array valueArray = Enum.GetValues(enumType);
				foreach (object value in valueArray)
				{
					string name = Enum.GetName(enumType, value);
					if (members.ContainsKey(name))
					{
						MemberInfo mi = members[name];
						BrowsableAttribute ba = mi.GetCustomAttribute<BrowsableAttribute>();
						if (ba != null && ba.Browsable == false) { continue; }
					}
					TagHtmlWriter writer = new TagHtmlWriter(OptionTagName);
					writer.MergeAttribute("value", Convert.ToString((int)value));
					string itemName = string.Concat(enumName, "_", name);
					string text = basicContext.GetString(converterName, itemName);

					if (string.IsNullOrWhiteSpace(text) == false) { writer.SetInnerText(text); }
					else { writer.SetInnerText(name); }
					builder.AppendLine(writer.ToString());
				}
				InnerHtml = (builder.ToString());
			}
			else if (propertyMeta.PropertyType == typeof(bool) || propertyMeta.PropertyType == typeof(bool?))
			{
				string trueText = "True", falseText = "False";

				StringBuilder builder = new StringBuilder(1000);
				if (label != null)
				{
					TagHtmlWriter writer2 = new TagHtmlWriter(OptionTagName);
					writer2.MergeAttribute("value", "");
					writer2.SetInnerText(label);
					builder.AppendLine(writer2.ToString());
				}
				string converterName = null; Type enumType = propertyMeta.PropertyType;
				WebDisplayAttribute wda = propertyMeta.Display;
				if (wda != null)
				{
					converterName = wda.ConverterName;
					if (string.IsNullOrWhiteSpace(converterName))
						trueText = basicContext.GetString(string.Concat(wda.DisplayName, "_TrueText"));
					else
						trueText = basicContext.GetString(converterName, string.Concat(wda.DisplayName, "_TrueText"));

					if (string.IsNullOrWhiteSpace(converterName))
						falseText = basicContext.GetString(string.Concat(wda.DisplayName, "_FalseText"));
					else
						falseText = basicContext.GetString(converterName, string.Concat(wda.DisplayName, "_FalseText"));
				}

				TagHtmlWriter writer = new TagHtmlWriter(OptionTagName);
				writer.MergeAttribute("value", "true");
				writer.SetInnerText(trueText ?? "True");
				builder.AppendLine(writer.ToString());

				TagHtmlWriter writer1 = new TagHtmlWriter(OptionTagName);
				writer1.MergeAttribute("value", "false");
				writer1.SetInnerText(falseText ?? "False");
				builder.AppendLine(writer1.ToString());

				InnerHtml = (builder.ToString());
			}
			return this;
		}

		/// <summary></summary>
		/// <param name="items"></param>
		/// <returns></returns>
		private StringBuilder Options(IEnumerable<SelectListItem> items)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (IGrouping<int, SelectListItem> item in from i in items
															group i by (i.Group != null) ? i.Group.GetHashCode() : i.GetHashCode())
			{
				SelectListGroup group = item.First().Group;
				TagHtmlWriter tagBuilder = null;
				if (group != null)
				{
					tagBuilder = new TagHtmlWriter(GroupTagName);
					if (group.Name != null) { tagBuilder.MergeAttribute("label", group.Name); }
					if (group.Disabled) { tagBuilder.MergeAttribute("disabled", "disabled"); }
					stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.StartTag));
				}

				foreach (SelectListItem item2 in item)
				{
					stringBuilder.AppendLine(ItemToOption(OptionTagName, item2));
				}

				if (group != null)
				{
					stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.EndTag));
				}
			}

			return stringBuilder;
		}

		/// <summary></summary>
		/// <param name="tag"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		private string ItemToOption(string tag, SelectListItem item)
		{
			TagHtmlWriter tagBuilder = new TagHtmlWriter(tag) { InnerHtml = HttpUtility.HtmlEncode(item.Text) };
			if (item.Value != null) { tagBuilder.Attributes["value"] = item.Value; }
			if (item.Selected) { tagBuilder.Attributes["selected"] = "selected"; }
			if (item.Disabled) { tagBuilder.Attributes["disabled"] = "disabled"; }
			return tagBuilder.ToString();
		}

		/// <summary></summary>
		/// <param name="tag"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		private string ItemToOption(string tag, ISelectOption item)
		{
			TagHtmlWriter tagBuilder = new TagHtmlWriter(tag) { InnerHtml = HttpUtility.HtmlEncode(item.Text) };
			if (item.Value != null) { tagBuilder.Attributes["value"] = item.Value; }
			if (item.Attributes != null && item.Attributes != "") { tagBuilder.Attributes["data-attrs"] = item.Value; }
			if (item.Selected) { tagBuilder.Attributes["selected"] = "selected"; }
			if (item.Disabled) { tagBuilder.Attributes["disabled"] = "disabled"; }
			return tagBuilder.ToString();
		}

		/// <summary>输出选择项信息</summary>
		public Select Option(string value)
		{
			return Option(value, value, null);
		}

		/// <summary>输出选择项信息</summary>
		/// <param name="text"></param>
		/// <param name="value"></param>
		public Select Option(string value, string text)
		{
			return Option(value, text, null);
		}

		/// <summary>输出选择项信息</summary>
		public Select Option(string value, string text, string @class)
		{
			if (mEmpty == true) { return this; }
			TagHtmlWriter writer = new TagHtmlWriter(OptionTagName);
			if (string.IsNullOrWhiteSpace(@class) == false) { writer.MergeAttribute("class", @class); }
			writer.MergeAttribute("value", value);
			writer.InnerHtml = text;
			if (string.IsNullOrWhiteSpace(InnerHtml)) { InnerHtml = writer.ToString(); }
			else
			{
				InnerHtml = string.Concat(InnerHtml, writer.ToString());
			}
			return this;
		}

		/// <summary>输出选择项信息</summary>
		public Select Options<T>(IEnumerable<T> collection) where T : ISelectOption
		{
			if (mEmpty == true) { return this; }
			if (collection == null) { return this; }
			StringBuilder stringBuilder = new StringBuilder();
			foreach (IGrouping<int, ISelectOption> item in from i in collection
														   group i by (i.Group != null) ? i.Group.GetHashCode() : i.GetHashCode())
			{
				SelectGroup group = item.First().Group;
				TagHtmlWriter tagBuilder = null;
				if (group != null)
				{
					tagBuilder = new TagHtmlWriter(GroupTagName);
					if (group.Name != null) { tagBuilder.MergeAttribute("label", group.Name); }
					if (group.Disabled) { tagBuilder.MergeAttribute("disabled", "disabled"); }
					stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.StartTag));
				}

				foreach (ISelectOption item2 in item)
				{
					stringBuilder.AppendLine(ItemToOption(OptionTagName, item2));
				}

				if (group != null)
				{
					stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.EndTag));
				}
			}
			if (string.IsNullOrWhiteSpace(InnerHtml)) { InnerHtml = stringBuilder.ToString(); }
			else { InnerHtml = string.Concat(InnerHtml, stringBuilder.ToString()); }
			return this;
		}

		/// <summary>作为 value 唯一标识的键名，绑定值为对象类型时必填 </summary>
		/// <returns>返回当前对象。</returns>
		public Select ValueKey(string value) { Prop("value-key", value); return this; }

		/// <summary>多选模式下是否折叠Tag </summary>
		/// <returns>返回当前对象。</returns>
		public Select CollapseTags() { Prop("collapse-tags", true); return this; }

		/// <summary>多选时用户最多可以选择的项目数，为 0 则不限制</summary>
		/// <returns>返回当前对象。</returns>
		public Select MultipleLimit(int count) { Attr("multiple-limit", count); return this; }

		/// <summary>是否多选 </summary>
		/// <returns>返回当前对象。</returns>
		public Select Multiple() { Prop("multiple", true); return this; }

		/// <summary>是否可清空</summary>
		/// <returns>返回当前对象。</returns>
		public Select Clearable() { Prop("clearable", true); return this; }

		/// <summary>输入框中是否显示选中值的完整路径，</summary>
		/// <param name="value">输入框是否显示选中值的完整路径，默认值为 true</param>
		/// <returns>返回当前对象。</returns>
		public Select ShowAllLevels(bool value) { Prop("show-all-levels", value); return this; }

		/// <summary>输入框中是否显示选中值的完整路径，默认值为 true</summary>
		/// <returns>返回当前对象。</returns>
		public Select HideAllLevels() { Prop("show-all-levels", false); return this; }

		/// <summary>是否可搜索选项，</summary>
		/// <param name="value">是否可搜索选项，默认值为 true</param>
		/// <returns>返回当前对象。</returns>
		public Select Filterable(bool value) { Prop("filterable", value); return this; }

		/// <summary>原生属性，自动补全(on, off),默认off</summary>
		/// <returns>返回当前对象。</returns>
		public Select AutoComplete() { Attr("autocomplete", "on"); return this; }

		/// <summary>输出开始标签</summary>
		/// <returns>返回当前对象</returns>
		public new Select Begin() { return base.Begin() as Select; }

		/// <summary>表示当前选择更改事件</summary>
		/// <returns>返回当前对象</returns>
		public Select Change(string callback) { Event("change", callback); return this; }

		/// <summary>释放非托管资源</summary>
		protected override void Dispose()
		{
			if (mEmpty == true) { return; }
			RenderContent(basicContext.Writer);
			RenderEndTag(basicContext.Writer);
		}
		/// <summary>显示输入标签的字符串表示形式(HTML)</summary>
		/// <returns>返回 Select 的 Html 字符串</returns>
		public override string ToString()
		{
			return base.ToString();
		}
	}
}
