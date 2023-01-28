using Basic.EntityLayer;
using Basic.Enums;

namespace Basic.MvcLibrary
{
	/// <summary>表示表单域输入组件</summary>
	public class InputText : Input<InputText>
	{
		private readonly IBasicContext basicContext;
		private readonly EntityPropertyMeta mPropertyMeta;
		/// <summary>初始化 InputText 类实例</summary>
		/// <param name="basic">表示请求上下文信息</param>
		/// <param name="meta">表示属性的元数据</param>
		internal protected InputText(IBasicContext basic, EntityPropertyMeta meta)
			: base(basic, meta, InputTags.Input) { basicContext = basic; mPropertyMeta = meta; }

		/// <summary>初始化空按钮实例，此按钮不输出。</summary>
		private InputText(bool empty) : base(empty) { }

		/// <summary>表示空按钮</summary>
		public static InputText Empty { get { return new InputText(true); } }

		/// <summary>输入控件类型</summary>
		/// <returns>返回当前对象。</returns>
		public InputText Type(string type) { SetAttr("type", type); return this; }

		/// <summary>输入控件类型</summary>
		/// <returns>返回当前对象。</returns>
		public InputText TypeToArea() { SetAttr("type", "textarea"); return this; }

		/// <summary>输入控件类型</summary>
		/// <returns>返回当前对象。</returns>
		public InputText TypeToText() { SetAttr("type", "text"); return this; }

		/// <summary>原生属性，设置最大值</summary>
		/// <returns>返回当前对象。</returns>
		public InputText Max(int value) { SetAttr("max", value); return this; }

		/// <summary>原生属性，设置最小值</summary>
		/// <returns>返回当前对象。</returns>
		public InputText Min(int value) { SetAttr("min", value); return this; }

		/// <summary>原生属性，设置输入字段的合法数字间隔</summary>
		/// <returns>返回当前对象。</returns>
		public InputText Step(int value) { SetAttr("step", value); return this; }

		/// <summary>输入框关联的label文字</summary>
		/// <returns>返回当前对象。</returns>
		public InputText Label(string label) { SetAttr("label", label); return this; }

		/// <summary>输入框占位文本</summary>
		/// <param name="converter">文本消息转换器名称</param>
		/// <param name="name">文本消息资源名称</param>
		/// <returns>返回当前对象。</returns>
		public InputText Label(string converter, string name)
		{
			string value = basicContext.GetString(converter, name);
			SetAttr("label", value); return this;
		}

		/// <summary>输入框关联的label文字</summary>
		/// <returns>返回当前对象。</returns>
		public InputText Label()
		{
			WebDisplayAttribute wda = mPropertyMeta.Display;
			if (wda == null) { return this; }
			return Label(wda.ConverterName, wda.DisplayName);
		}

		/// <summary>原生属性</summary>
		/// <returns>返回当前对象。</returns>
		public InputText Form(string name) { SetProp("form", name); return this; }

		/// <summary>是否显示输入字数统计，只在 type = "text" 或 type = "textarea" 时有效</summary>
		/// <returns>返回当前对象。</returns>
		public InputText ShowWordLimit()
		{
			DbTypeEnum type = DbTypeEnum.Int32;
			if (mPropertyMeta.Mapping != null) { type = mPropertyMeta.Mapping.DataType; }
			if (type == DbTypeEnum.Char || type == DbTypeEnum.NChar || type == DbTypeEnum.NVarChar
				|| type == DbTypeEnum.VarChar) { MaxLength(mPropertyMeta.Mapping.Size); }
			SetProp("show-word-limit", true); return this;
		}

		/// <summary>是否显示切换密码图标</summary>
		/// <returns>返回当前对象。</returns>
		public InputText ShowPassword() { SetProp("show-password", true); return this; }

		/// <summary>是否可清空</summary>
		/// <returns>返回当前对象。</returns>
		public InputText Clearable() { SetProp("clearable", true); return this; }

		/// <summary>原生属性，最大输入长度</summary>
		/// <returns>返回当前对象。</returns>
		public InputText MaxLength(int length) { SetAttr("maxlength", length); return this; }

		/// <summary>原生属性，最小输入长度</summary>
		/// <returns>返回当前对象。</returns>
		public InputText MinLength(int length) { SetAttr("minlength", length); return this; }

		/// <summary>输入框头部图标</summary>
		/// <returns>返回当前对象。</returns>
		public InputText PrefixIcon(string css) { SetAttr("prefix-icon", css); return this; }

		/// <summary>输入框尾部图标</summary>
		/// <returns>返回当前对象。</returns>
		public InputText SuffixIcon(string css) { SetAttr("suffix-icon", css); return this; }

		/// <summary>输入框行数，只对 type = "textarea" 有效</summary>
		/// <returns>返回当前对象。</returns>
		public InputText Rows(int rows) { SetAttr("rows", rows); return this; }

		/// <summary>自适应内容高度，只对 type = "textarea" 有效</summary>
		/// <returns>返回当前对象。</returns>
		public InputText AutoSize() { SetAttr("autosize", true); return this; }

		/// <summary>自适应内容高度，只对 type = "textarea" 有效，可传入对象，
		/// 如，{ minRows: 2, maxRows: 6 }</summary>
		/// <returns>返回当前对象。</returns>
		public InputText AutoSize(string json) { SetAttr("autosize", json); return this; }

		/// <summary>原生属性，自动补全(on, off),默认off</summary>
		/// <returns>返回当前对象。</returns>
		public InputText AutoComplete() { SetAttr("autocomplete", "on"); return this; }

		/// <summary>原生属性，设置输入字段的合法数字间隔</summary>
		/// <returns>返回当前对象。</returns>
		public InputText AutoFocus() { SetProp("autofocus", true); return this; }

		/// <summary>输入时是否触发表单的校验</summary>
		/// <returns>返回当前对象。</returns>
		public InputText ValidateEvent() { SetProp("validate-event", true); return this; }

	}
}
