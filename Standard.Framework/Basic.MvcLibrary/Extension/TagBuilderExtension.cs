namespace Basic.MvcLibrary
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Web.Mvc;

	/// <summary>
	/// TagBuilder 类扩展
	/// </summary>
	internal static class TagBuilderExtensions
	{
		/// <summary>
		/// 返回 MvcHtmlString 类实例。
		/// </summary>
		/// <param name="tagBuilder">需要扩展的TagBuilder 类实例。</param>
		/// <param name="renderMode"></param>
		/// <returns></returns>
		internal static MvcHtmlString ToMvcHtmlString(this TagBuilder tagBuilder, TagRenderMode renderMode)
		{
			return new MvcHtmlString(tagBuilder.ToString(renderMode));
		}

		/// <summary>
		/// 将
		/// </summary>
		/// <param name="tagBuilder"></param>
		/// <returns></returns>
		internal static MvcHtmlString ToMvcHtmlString(this TagBuilder tagBuilder)
		{
			return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
		}
	}
}

