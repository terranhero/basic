using System;

namespace Basic.MvcLibrary
{
	/// <summary>表示模板视图</summary>
	public interface ITemplateView : IDisposable
	{
	}

	/// <summary>表示模板视图</summary>
	public interface ITemplateView<T> : ITemplateView, IDisposable where T : class
	{
	}
}
