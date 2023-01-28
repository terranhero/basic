using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Basic.EntityLayer;
using Basic.Interfaces;

namespace Basic.MvcLibrary
{
	/// <summary>表示单选按钮组</summary>
	public sealed class RadioList : Input<RadioList>
	{
		private readonly IBasicContext basicContext;
		private readonly EntityPropertyMeta propertyMeta;
		/// <summary>初始化 RadioList 类实例</summary>
		/// <param name="basic">表示请求上下文信息</param>
		/// <param name="meta">表示属性的元数据</param>
		internal RadioList(IBasicContext basic, EntityPropertyMeta meta)
			: base(basic, meta, InputTags.RadioGroup) { basicContext = basic; propertyMeta = meta; }

		/// <summary>初始化空实例，此不输出。</summary>
		private RadioList(bool empty) : base(empty) { }

		/// <summary>表示空实例</summary>
		internal static RadioList Empty { get { return new RadioList(true); } }

		private string Option(string tag, string label, string text)
		{
			TagHtmlWriter writer = new TagHtmlWriter(tag);
			writer.MergeAttribute("value", label);
			writer.MergeAttribute("label", label);
			writer.SetInnerText(text);
			return (writer.ToString());
		}

		private string Option(string tag, string label, string text, string attrs)
		{
			TagHtmlWriter writer = new TagHtmlWriter(tag);
			writer.MergeAttribute("value", label);
			writer.MergeAttribute("label", label);
			writer.MergeAttribute("attrs", attrs);
			writer.SetInnerText(text);
			return (writer.ToString());
		}

		private RadioList Options(string tag)
		{
			IDictionary<string, object> datas = basicContext.ViewData;
			if (datas.TryGetValue(Key, out object items) == true)
			{
				StringBuilder builder = new StringBuilder(1000);
				if (items is IEnumerable<SelectListItem> list)
				{
					foreach (SelectListItem item in list)
					{
						builder.AppendLine(Option(tag, item.Value, item.Text));
					}
				}
				else if (items is IEnumerable<ISelectOption> options)
				{
					foreach (ISelectOption item in options)
					{
						builder.AppendLine(Option(tag, item.Value, item.Text, item.Attributes));
					}
				}
				InnerHtml = (builder.ToString());
			}
			else if (propertyMeta.PropertyType.IsEnum)
			{
				StringBuilder builder = new StringBuilder(1000);
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
					//TagHtmlWriter writer = new TagHtmlWriter(tag);
					//writer.MergeAttribute("value", Convert.ToString((int)value));
					//writer.MergeAttribute("label", Convert.ToString((int)value));
					string itemName = string.Concat(enumName, "_", name);
					string text = basicContext.GetString(converterName, itemName);

					if (string.IsNullOrWhiteSpace(text) == false) { builder.AppendLine(Option(tag, Convert.ToString((int)value), text)); }
					else { builder.AppendLine(Option(tag, Convert.ToString((int)value), name)); }
				}
				InnerHtml = (builder.ToString());
			}
			else if (propertyMeta.PropertyType == typeof(bool) || propertyMeta.PropertyType == typeof(bool?))
			{
				string trueText = "True", falseText = "False";

				StringBuilder builder = new StringBuilder(1000);
				string converterName = null;
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

				TagHtmlWriter tWriter = new TagHtmlWriter(tag);
				tWriter.MergeAttribute("value", "true");
				tWriter.MergeAttribute("label", "true");
				tWriter.SetInnerText(trueText ?? "True");
				builder.AppendLine(tWriter.ToString());

				TagHtmlWriter fWriter = new TagHtmlWriter(tag);
				fWriter.MergeAttribute("value", "false");
				fWriter.MergeAttribute("label", "false");
				fWriter.SetInnerText(falseText ?? "False");
				builder.AppendLine(fWriter.ToString());

				InnerHtml = (builder.ToString());
			}
			return this;
		}

		/// <summary>输出选择项信息</summary>
		/// <returns>返回当前对象。</returns>
		public RadioList Options() { return Options(InputTags.Radio); }

		/// <summary>输出选择项信息</summary>
		/// <returns>返回当前对象。</returns>
		public RadioList Buttons() { return Options(InputTags.RadioButton); }

		/// <summary>输出单个单选按钮</summary>
		private RadioList OptionFor<TE>(string tag, TE value)
		{
			Type enumType = typeof(TE);
			if (enumType.IsEnum == false) { throw new ArgumentException("参数类型错误，此参数必须是枚举", nameof(value)); }
			StringBuilder builder = new StringBuilder(1000);
			builder.AppendLine(InnerHtml); string converterName = null;
			WebDisplayConverterAttribute wdca = enumType.GetCustomAttribute<WebDisplayConverterAttribute>();
			if (wdca != null) { converterName = wdca.ConverterName; }

			string name = Enum.GetName(enumType, value);
			string itemName = string.Concat(enumType.Name, "_", name);
			string text = basicContext.GetString(converterName, itemName);
			string strValue = Convert.ToString(Convert.ToInt32(value));
			if (string.IsNullOrWhiteSpace(text) == false) { builder.AppendLine(Option(tag, strValue, text)); }
			else { builder.AppendLine(Option(tag, strValue, name)); }
			InnerHtml = builder.ToString();
			return this;
		}

		/// <summary>输出单个单选按钮</summary>
		public RadioList OptionFor<TE>(TE value)
		{
			return OptionFor<TE>(InputTags.Radio, value);
		}

		/// <summary>输出单个单选按钮</summary>
		public RadioList ButtonFor<TE>(TE value)
		{
			return OptionFor<TE>(InputTags.RadioButton, value);
		}

		/// <summary>作为 value 唯一标识的键名，绑定值为对象类型时必填 </summary>
		/// <returns>返回当前对象。</returns>
		public RadioList ValueKey(string value) { SetProp("value-key", value); return this; }

		/// <summary>多选模式下是否折叠Tag </summary>
		/// <returns>返回当前对象。</returns>
		public RadioList CollapseTags() { SetProp("collapse-tags", true); return this; }

		/// <summary>选项分隔符 </summary>
		/// <returns>返回当前对象。</returns>
		public RadioList SetMultiple() { SetProp("multiple", true); return this; }

		/// <summary>是否可清空</summary>
		/// <returns>返回当前对象。</returns>
		public RadioList Clearable() { SetProp("clearable", true); return this; }

		/// <summary>输入框中是否显示选中值的完整路径，</summary>
		/// <param name="value">输入框是否显示选中值的完整路径，默认值为 true</param>
		/// <returns>返回当前对象。</returns>
		public RadioList ShowAllLevels(bool value) { SetProp("show-all-levels", value); return this; }

		/// <summary>输入框中是否显示选中值的完整路径，默认值为 true</summary>
		/// <returns>返回当前对象。</returns>
		public RadioList HideAllLevels() { SetProp("show-all-levels", false); return this; }

		/// <summary>是否可搜索选项，</summary>
		/// <param name="value">是否可搜索选项，默认值为 true</param>
		/// <returns>返回当前对象。</returns>
		public RadioList Filterable(bool value) { SetProp("filterable", value); return this; }

		/// <summary>原生属性，自动补全(on, off),默认off</summary>
		/// <returns>返回当前对象。</returns>
		public RadioList AutoComplete() { SetAttr("autocomplete", "on"); return this; }

		/// <summary>输出开始标签</summary>
		/// <returns>返回当前对象</returns>
		public new RadioList Begin() { return base.Begin() as RadioList; }

		/// <summary>释放非托管资源</summary>
		protected override void Dispose()
		{
			basicContext.Writer.WriteLine(InnerHtml);
			tagBuilder.RenderEndTag(basicContext.Writer);
		}

		/// <summary>显示输入标签的字符串表示形式(HTML)</summary>
		/// <returns>返回 RadioList 的 Html 字符串</returns>
		public override string ToString()
		{
			base.AddClass("web-radiolist");
			return base.ToString();
		}
	}
}
