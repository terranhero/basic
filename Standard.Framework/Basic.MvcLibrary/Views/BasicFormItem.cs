namespace Basic.MvcLibrary
{
	/// <summary>表单域</summary>
	public class BasicFormItem : Options<BasicFormItem>, System.IDisposable
	{
		private readonly IBasicContext basicContext;
		private readonly TagHtmlWriter writer;
		/// <summary>初始化 BasicFormItem 类实例</summary>
		/// <param name="bh">表示数据的上下文请求</param>
		internal BasicFormItem(IBasicContext bh)
		{
			basicContext = bh; AddClass("el-col");
			writer = new TagHtmlWriter(FormTags.FormItem);
		}

		/// <summary>开始输出 form 组件标记</summary>
		public BasicFormItem Begin()
		{
			writer.MergeOptions(this);
			writer.RenderBeginTag(basicContext.Writer);
			return this;
		}

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="span">表示24栅格系统所占列数</param>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem SetSpan(int span) { AddClass(string.Concat("el-col-", span)); return this; }

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="span">表示24栅格系统所占列数</param>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem Span(int span) { AddClass(string.Concat("el-col-", span)); return this; }

		/// <summary>释放非托管资源</summary>
		protected override void Dispose() { writer.RenderEndTag(basicContext.Writer); }

		/// <summary>释放非托管资源</summary>
		void System.IDisposable.Dispose() { this.Dispose(); }

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="prop">表单域 model 字段，在使用 validate、resetFields 方法的情况下，该属性是必填的</param>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem SetPropValue(string prop) { SetAttr("prop", prop); return this; }

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="label">标签文本</param>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem SetLabel(string label) { SetAttr("label", label); return this; }

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="error">自定义错误对象, object 类型数据。</param>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem SetError(string error) { SetProp("error", error); return this; }

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="rules">表单验证规则, object 类型数据。</param>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem SetRules(string rules) { SetProp("rules", rules); return this; }

		/// <summary>表单域标签的宽度，支持 auto。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem LabelWidth(string width) { SetAttr("label-width", width); return this; }

		/// <summary>表单域标签的宽度像素。</summary>
		/// <param name="width">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem LabelWidth(int width) { SetAttr("label-width", string.Concat(width, "px")); return this; }

		/// <summary>表单域标签的宽度像素。</summary>
		/// <param name="condition">需要在后续参数中设置宽的的逻辑表达式</param>
		/// <param name="tWidth">如果参数 condition 为真，则使用此值设置标签宽度</param>
		/// <param name="fWidth">如果参数 condition 为假，则使用此值设置标签宽度</param>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem LabelWidth(bool condition, int tWidth, int fWidth) { return condition ? LabelWidth(tWidth) : LabelWidth(fWidth); }

		/// <summary>用于控制该表单内组件的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem SizeToMedium() { SetAttr("size", "medium"); return this; }

		/// <summary>用于控制该表单内组件的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem SizeToSmall() { SetAttr("size", "small"); return this; }

		/// <summary>用于控制该表单内组件的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem SizeToMini() { SetAttr("size", "mini"); return this; }

		/// <summary>设置必填，如不设置，则会根据校验规则自动生成。</summary>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem Required() { Prop("required", true); return this; }

		/// <summary>隐藏校验错误信息。</summary>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem HideMessage() { SetProp("show-message", false); return this; }

		/// <summary>以行内形式展示校验信息。</summary>
		/// <returns>返回当前对象。</returns>
		public BasicFormItem InlineMessage() { SetProp("inline-message", true); return this; }
	}
}
