using Microsoft.AspNetCore.Html;

namespace Basic.MvcLibrary
{
	/// <summary>Button 扩展方法</summary>
	public static class ButtonExtension
	{
		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <returns>返回当前按钮实例</returns>
		public static IHtmlContent ToHtml(this Button btn)
		{
			return new HtmlString(btn.ToString(true));
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <returns>返回当前按钮实例</returns>
		public static IHtmlContent ToHtml<T>(this Input<T> input) where T : Options
		{
			return new HtmlString(input.ToString());
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <returns>返回当前按钮实例</returns>
		public static IHtmlContent ToHtml(this Label lbl)
		{
			return new HtmlString(lbl.ToString());
		}

		/// <summary>将指定的标记特性和值添加到该按钮的属性列表中。</summary>
		/// <returns>返回当前按钮实例</returns>
		public static IHtmlContent ToHtml(this Span lbl)
		{
			return new HtmlString(lbl.ToString());
		}
	}
}
