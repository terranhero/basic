using System;

namespace Basic.MvcLibrary
{
	/// <summary>表示树节点插槽模板</summary>
	public sealed class ListItemTemplate<T> : ITemplateView<T> where T : class
	{
		private readonly IBasicContext mBasic;
		private readonly TagHtmlWriter writer;

		/// <summary>初始化树形视图</summary>
		/// <param name="bh">金软科技基础开发框架扩展</param>
		internal ListItemTemplate(IBasicContext bh)
		{
			mBasic = bh; writer = new TagHtmlWriter(ViewTags.Template);
			writer.MergeAttribute("v-slot:item", "{row, index, rows}");
			writer.RenderBeginTag(mBasic.Writer);
		}

		/// <summary>初始化树形视图</summary>
		/// <param name="bh">金软科技基础开发框架扩展</param>
		/// <param name="opts">模板特性</param>
		internal ListItemTemplate(IBasicContext bh, IOptions opts)
		{
			mBasic = bh; writer = new TagHtmlWriter(ViewTags.Template);
			writer.MergeOptions(opts);
			writer.MergeAttribute("v-slot:item", "{row, index, rows}");
			writer.RenderBeginTag(mBasic.Writer);
		}

		/// <summary>释放托管资源</summary>
		void IDisposable.Dispose() { writer.RenderEndTag(mBasic.Writer); }
	}
}
