using Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary>表示标签</summary>
	public class Label : Options, System.IDisposable
	{
		private readonly IBasicContext basicContext;
		private readonly EntityPropertyMeta mPropertyMeta;
		/// <summary></summary>
		protected readonly TagHtmlWriter tagBuilder;
		/// <summary>初始化 Label 类实例</summary>
		/// <param name="basic">表示请求上下文信息</param>
		/// <param name="meta">表示属性的元数据</param>
		internal protected Label(IBasicContext basic, EntityPropertyMeta meta)
		{
			mPropertyMeta = meta; basicContext = basic; tagBuilder = new TagHtmlWriter("label");
		}

		/// <summary>表示空按钮</summary>
		internal static Label Empty { get { return new Label(true); } }

		private readonly bool mEmpty = false;
		/// <summary>初始化 Label 类实例</summary>
		protected Label(bool empty) { mEmpty = empty; tagBuilder = new TagHtmlWriter("label"); }

		/// <summary>规定 label 绑定到哪个表单元素</summary>
		/// <returns>返回当前对象。</returns>
		public Label For(string id) { SetAttr("for", id); return this; }

		/// <summary>原生属性</summary>
		/// <returns>返回当前对象。</returns>
		public Label Text(string text) { tagBuilder.SetInnerText(text); return this; }

		/// <summary>输入框占位文本</summary>
		/// <param name="converter">文本消息转换器名称</param>
		/// <param name="name">文本消息资源名称</param>
		/// <returns>返回当前对象。</returns>
		public Label Text(string converter, string name)
		{
			string value = basicContext.GetString(converter, name);
			tagBuilder.SetInnerText(value); return this;
		}

		/// <summary>输入框关联的label文字</summary>
		/// <returns>返回当前对象。</returns>
		public Label Text()
		{
			WebDisplayAttribute wda = mPropertyMeta.Display;
			if (wda == null) { return this; }
			return Text(wda.ConverterName, wda.DisplayName);
		}

		/// <summary>原生属性</summary>
		/// <returns>返回当前对象。</returns>
		public Label Html(string html) { tagBuilder.InnerHtml = html; return this; }

		/// <summary>输出开始标签</summary>Class1.cs
		/// <returns>返回当前对象</returns>
		public virtual Label Begin()
		{
			if (mEmpty == true) { return this; }
			tagBuilder.MergeOptions(this);
			tagBuilder.RenderBeginTag(basicContext.Writer);
			return this;
		}

		/// <summary>释放非托管资源</summary>
		protected override void Dispose()
		{
			tagBuilder.RenderEndTag(basicContext.Writer);
		}

		/// <summary>显示输入标签的字符串表示形式(HTML)</summary>
		/// <returns>返回按钮的 Html 字符串</returns>
		public override string ToString()
		{
			if (mEmpty == true) { return string.Empty; }
			tagBuilder.MergeOptions(this);
			return tagBuilder.ToString();
		}
	}
}
