using Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary>表示表单域输入组件</summary>
	public class InputDate : Input<InputDate>
	{
		private readonly IBasicContext basicContext;
		private readonly EntityPropertyMeta mPropertyMeta;
		/// <summary>初始化 InputDate 类实例</summary>
		/// <param name="basic">表示请求上下文信息</param>
		/// <param name="meta">表示属性的元数据</param>
		internal protected InputDate(IBasicContext basic, EntityPropertyMeta meta)
			: base(basic, meta, InputTags.Date) { basicContext = basic; mPropertyMeta = meta; }

		/// <summary>初始化空按钮实例，此按钮不输出。</summary>
		private InputDate(bool empty) : base(empty) { }

		/// <summary>表示空按钮</summary>
		internal static InputDate Empty { get { return new InputDate(true); } }

		/// <summary>是否可清空</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate Unclearable() { Prop("clearable", false); return this; }

		/// <summary>用户确认选定的值时触发</summary>
		/// <param name="func">回调函数 function(组件绑定值。格式与绑定值一致，可受 value-format 控制){}</param>
		/// <returns>返回当前对象。</returns>
		public InputDate OnChange(string func) { Event("change", func); return this; }

		/// <summary>当 input 失去焦点时触发</summary>
		/// <param name="func">回调函数 function(组件实例){}</param>
		/// <returns>返回当前对象。</returns>
		public InputDate OnBlur(string func) { Event("blur", func); return this; }

		/// <summary>当 input 获得焦点时触发</summary>
		/// <param name="func">回调函数 function(组件实例){}</param>
		/// <returns>返回当前对象。</returns>
		public InputDate OnFocus(string func) { Event("focus", func); return this; }

		/// <summary>文本框可输入</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate Editable(bool edit = false) { SetProp("editable", edit); return this; }

		/// <summary>显示在输入框中的格式</summary>
		/// <param name="format">显示在输入框中的格式</param>
		/// <param name="variable">是否变量</param>
		/// <returns>返回当前对象。</returns>
		public InputDate Format(string format, bool variable = false)
		{
			if (variable) { RemoveAttribute("format"); Prop("format", format); }
			else { Attr("format", format); }
			return this;
		}

		/// <summary>可选，绑定值的格式。不指定则绑定值为 Date 对象</summary>
		/// <param name="format">可选，绑定值的格式。不指定则绑定值为 Date 对象</param>
		/// <param name="variable">是否变量</param>
		/// <returns>返回当前对象。</returns>
		public InputDate ValueFormat(string format, bool variable = false)
		{
			if (variable) { RemoveAttribute("value-format"); Prop("value-format", format); }
			else { Attr("value-format", format); }
			return this;
		}

		/// <summary>输入时是否触发表单的校验</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate ValidateEvent() { Prop("validate-event", true); return this; }

		/// <summary>范围选择时开始日期的占位内容</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate StartPlaceholder(string value) { Attr("start-placeholder", value); return this; }

		/// <summary>范围选择时结束日期的占位内容</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate EndPlaceholder(string value) { Attr("end-placeholder", value); return this; }

		/// <summary>显示年份选择(type=year)</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate Year() { Attr("type", "year"); return this; }

		/// <summary>显示月份选择(type=month)</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate Month() { Attr("type", "month"); return this; }

		/// <summary>显示日期选择(type=date)</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate Date() { Attr("type", "date"); return this; }

		/// <summary>显示多个日期选择(type=dates)</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate Dates() { Attr("type", "dates"); return this; }

		/// <summary>显示星期选择(type=week)</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate Week() { Attr("type", "week"); return this; }

		/// <summary>显示日期时间选择(type=datetime)</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate DateTime() { Attr("type", "datetime"); return this; }

		/// <summary>显示日期时间范围选择(type=datetimerange)</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate DateTimeRange() { Attr("type", "datetimerange"); return this; }

		/// <summary>显示日期范围选择(type=daterange)</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate DateRange() { Attr("type", "daterange"); return this; }

		/// <summary>显示月份范围选择(type=monthrange)</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate MonthRange() { Attr("type", "monthrange"); return this; }

		/// <summary>输入框关联的label文字</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate Label(string label) { Attr("label", label); return this; }

		/// <summary>日期选择扩展属性</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate PickerOptions(string options) { Prop("picker-options", options); return this; }

		/// <summary>输入框占位文本</summary>
		/// <param name="converter">文本消息转换器名称</param>
		/// <param name="name">文本消息资源名称</param>
		/// <returns>返回当前对象。</returns>
		public InputDate Label(string converter, string name)
		{
			string value = basicContext.GetString(converter, name);
			Attr("label", value); return this;
		}

		/// <summary>输入框关联的label文字</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate Label()
		{
			WebDisplayAttribute wda = mPropertyMeta.Display;
			if (wda == null) { return this; }
			return Label(wda.ConverterName, wda.DisplayName);
		}
		/// <summary>原生属性</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate Form(string name) { SetProp("form", name); return this; }

		/// <summary>输入框头部图标</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate PrefixIcon(string css) { Attr("prefix-icon", css); return this; }

		/// <summary>自定义清空图标的类名</summary>
		/// <returns>返回当前对象。</returns>
		public InputDate ClearIcon(string css) { Attr("clear-icon", css); return this; }


	}

}
