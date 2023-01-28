namespace Basic.MvcLibrary
{
	/// <summary>表示栅格系统一行</summary>
	public sealed class GridRow : Options, System.IDisposable
	{
		private readonly IBasicContext basicContext;
		private readonly TagHtmlWriter writer;
		/// <summary>初始化 BasicForm 类实例</summary>
		/// <param name="bh">表示数据的上下文请求</param>
		internal GridRow(IBasicContext bh)
		{
			basicContext = bh;
			writer = new TagHtmlWriter(FormTags.Row);
			writer.RenderBeginTag(basicContext.Writer);
		}

		/// <summary>释放非托管资源</summary>
		void System.IDisposable.Dispose() { writer.RenderEndTag(basicContext.Writer); }

	}
}
