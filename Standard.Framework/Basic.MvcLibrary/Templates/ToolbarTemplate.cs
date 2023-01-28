using System;
using System.Web.Mvc;

namespace Basic.MvcLibrary
{
	/// <summary>表示工具条插槽模板</summary>
	public interface IToolbarTemplate : IToolbar, ITemplateView
	{
	}

	/// <summary>表示工具条插槽模板</summary>
	public interface IToolbarTemplate<T> : IToolbar, IToolbar<T>, ITemplateView
	{
		/// <summary>数据源。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		new IToolbarTemplate<T> Model(string value);
	}

	/// <summary>表示工具条插槽模板</summary>
	public sealed class ToolbarTemplate<T> : Toolbar<T>, IToolbar<T>, IToolbarTemplate<T>, IToolbarTemplate where T : class
	{
		private readonly TagHtmlWriter templateWriter;
		private readonly TagHtmlWriter toolbarWriter;
		private readonly IBasicContext mBasic;

		/// <summary>初始化 ToolbarTemplate 类实例</summary>
		/// <param name="bh">金软科技基础开发框架扩展</param>
		internal ToolbarTemplate(IBasicContext bh) : base(bh)
		{
			mBasic = bh; templateWriter = new TagHtmlWriter(ViewTags.Template);
			toolbarWriter = new TagHtmlWriter(ViewTags.ToolBar);
			templateWriter.MergeAttribute("v-slot:toolbar", "");
			templateWriter.RenderBeginTag(mBasic.Writer);
		}

		/// <summary>数据源。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public new IToolbarTemplate<T> Model(string value) { base.Model(value); return this; }

		/// <summary>初始化 ToolbarTemplate 类实例</summary>
		/// <param name="bh">金软科技基础开发框架扩展</param>
		/// <param name="opts">模板特性</param>
		internal ToolbarTemplate(IBasicContext bh, IOptions opts) : base(bh)
		{
			mBasic = bh; templateWriter = new TagHtmlWriter(ViewTags.Template);
			templateWriter.MergeOptions(opts);
			templateWriter.MergeAttribute("v-slot:toolbar", "");
			templateWriter.RenderBeginTag(mBasic.Writer);
		}

		/// <summary>释放托管资源</summary>
		void IDisposable.Dispose()
		{
			base.Render();
			templateWriter.RenderEndTag(mBasic.Writer);
		}
	}
}
