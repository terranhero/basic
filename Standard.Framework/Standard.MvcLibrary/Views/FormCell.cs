namespace Basic.MvcLibrary
{
	/// <summary>表单域</summary>
	public class FormCell : Options<FormCell>, System.IDisposable
	{
		private readonly IBasicContext basicContext;
		private readonly TagHtmlWriter writer;
		/// <summary>初始化 BasicFormItem 类实例</summary>
		/// <param name="bh">表示数据的上下文请求</param>
		internal FormCell(IBasicContext bh)
		{
			basicContext = bh; AddClass("el-col");
			writer = new TagHtmlWriter(FormTags.FormCell);
		}

		/// <summary>开始输出 form 组件标记</summary>
		public FormCell Begin()
		{
			writer.MergeOptions(this);
			writer.RenderBeginTag(basicContext.Writer);
			return this;
		}

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="span">表示24栅格系统所占列数</param>
		/// <returns>返回当前对象。</returns>
		public FormCell Span(int span) { AddClass(string.Concat("el-col-", span)); return this; }

		/// <summary>释放非托管资源</summary>
		protected override void Dispose() { writer.RenderEndTag(basicContext.Writer); }

		/// <summary>释放非托管资源</summary>
		void System.IDisposable.Dispose() { this.Dispose(); }

		/// <summary>设置默认的错误消息。</summary>
		/// <param name="prop">表单域 model 字段，在使用 validate、resetFields 方法的情况下，该属性是必填的</param>
		/// <returns>返回当前对象。</returns>
		public FormCell SetPropValue(string prop) { SetAttr("prop", prop); return this; }

		/// <summary>标题。</summary>
		/// <param name="title">标签文本</param>
		/// <returns>返回当前对象。</returns>
		public FormCell Title(string title) { SetAttr("title", title); return this; }

		/// <summary>备注信息，显示在标题下方。</summary>
		/// <param name="label">标签文本</param>
		/// <returns>返回当前对象。</returns>
		public FormCell Label(string label) { SetAttr("label", label); return this; }

		/// <summary>跳转链接。</summary>
		/// <param name="link">跳转的连接</param>
		/// <returns>返回当前对象。</returns>
		public FormCell To(string link) { return Prop("to", link); }

		/// <summary>设置必填，如不设置，则会根据校验规则自动生成。</summary>
		/// <returns>返回当前对象。</returns>
		public FormCell Required() { Prop("required", true); return this; }

	}
}
