using System.IO;
using Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary>表示表单域输入组件</summary>
	public abstract class Input<TR> : Options<TR>, System.IDisposable where TR : Options
	{
		private readonly IBasicContext basicContext;
		private readonly EntityPropertyMeta mPropertyMeta;
		/// <summary></summary>
		private readonly TagHtmlWriter tagBuilder;
		/// <summary>初始化 Input 类实例</summary>
		/// <param name="basic">表示请求上下文信息</param>
		/// <param name="meta">表示属性的元数据</param>
		/// <param name="tag">表示输出标签名称</param>
		protected Input(IBasicContext basic, EntityPropertyMeta meta, string tag)
		{
			mPropertyMeta = meta; Ref(meta.Name); basicContext = basic; tagBuilder = new TagHtmlWriter(tag);
			Key = meta.Name;
		}

		/// <summary></summary>
		protected readonly bool mEmpty = false;
		/// <summary>初始化 Input 类实例</summary>
		protected Input(bool empty) { mEmpty = empty; tagBuilder = new TagHtmlWriter("input"); }

		/// <summary>表格数据源时间戳列名。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public TR Model(string value) { return Attr("v-model", value); }

		/// <summary>表格数据源时间戳列名。</summary>
		/// <param name="model"></param>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public TR Model(string model, string value)
		{
			if (string.IsNullOrWhiteSpace(model) == true) { return Attr("v-model", value); }
			else { return Attr("v-model", string.Concat(model, ".", value)); }
		}

		/// <summary>Table 的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public TR SizeToMedium() { return Attr("size", "medium"); }

		/// <summary>Table 的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public TR SizeToSmall() { return Attr("size", "small"); }

		/// <summary>Table 的尺寸</summary>
		/// <returns>返回当前对象。</returns>
		public TR SizeToMini() { return Attr("size", "mini"); }

		/// <summary>输入框占位文本</summary>
		/// <returns>返回当前对象。</returns>
		public TR Placeholder()
		{
			WebDisplayAttribute wda = mPropertyMeta.Display;
			if (wda == null) { return Placeholder(mPropertyMeta.DisplayName); }
			return Placeholder(wda.ConverterName, wda.DisplayName);
		}

		/// <summary>输入框占位文本</summary>
		/// <returns>返回当前对象。</returns>
		public TR Placeholder(string value) { return Attr("placeholder", value); }

		/// <summary>输入框占位文本</summary>
		/// <param name="converter">文本消息转换器名称</param>
		/// <param name="name">文本消息资源名称</param>
		/// <returns>返回当前对象。</returns>
		public TR Placeholder(string converter, string name)
		{
			string value = basicContext.GetString(converter, name);
			return Attr("placeholder", value);
		}

		/// <summary>禁用</summary>
		/// <returns>返回当前对象。</returns>
		public TR Disabled() { return SetProp("disabled", true); }

		/// <summary>原生属性</summary>
		/// <returns>返回当前对象。</returns>
		public TR Name(string id) { return Attr("name", id); }

		/// <summary>原生属性</summary>
		/// <returns>返回当前对象。</returns>
		public TR ReadOnly() { return SetProp("readonly", true); }

		/// <summary>输入框的tabindex</summary>
		/// <returns>返回当前对象。</returns>
		public TR TabIndex(string value) { return Attr("tabindex", value); }
		//resize 控制是否能被用户缩放  string none, both, horizontal, vertical    —

		/// <summary>输出开始标签</summary>Class1.cs
		/// <returns>返回当前对象</returns>
		public virtual void Render()
		{
			if (mEmpty == true) { return; }
			tagBuilder.MergeOptions(this);
			tagBuilder.InnerHtml = InnerHtml;
			tagBuilder.Render(basicContext.Writer);
		}

		/// <summary>输出开始标签</summary>Class1.cs
		/// <returns>返回当前对象</returns>
		public virtual TR Begin()
		{
			if (mEmpty == true) { return this as TR; }
			tagBuilder.MergeOptions(this);
			tagBuilder.RenderBeginTag(basicContext.Writer);
			return this as TR;
		}

		/// <summary>释放非托管资源</summary>
		protected override void Dispose()
		{
			tagBuilder.RenderEndTag(basicContext.Writer);
		}

		/// <summary>初始化 Input 类实例</summary>
		protected string InnerHtml { get; set; }

		/// <summary>显示输入标签的字符串表示形式(HTML)</summary>
		/// <returns>返回按钮的 Html 字符串</returns>
		public override string ToString()
		{
			if (mEmpty == true) { return string.Empty; }
			tagBuilder.MergeOptions(this);
			tagBuilder.InnerHtml = InnerHtml;
			return tagBuilder.ToString();
		}

		/// <summary>输出开始标记，包含属性</summary>
		protected void RenderBeginTag(TextWriter writer)
		{
			tagBuilder.RenderBeginTag(writer);
		}

		/// <summary>输出标签结束标记</summary>
		protected void RenderEndTag(TextWriter writer)
		{
			tagBuilder.RenderEndTag(writer);
		}

		/// <summary></summary>
		protected void Render(TextWriter writer)
		{
			tagBuilder.Render(writer);
		}

		/// <summary></summary>
		protected void RenderContent(TextWriter writer)
		{
			tagBuilder.RenderContent(writer);
		}
	}
}
