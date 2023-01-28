using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Globalization;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 开发通用扩展类
	/// </summary>
    public static class CssExtension
	{
		/// <summary>
		/// 生成Css文件引用。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="cssFileName">Css文件名称（不包含扩展名,相对于Content目录的路径）。</param>
		/// <returns>返回样式文件的引用。</returns>
		public static MvcHtmlString CssFile(this HtmlHelper html, string cssFileName)
		{
			return html.CssFile(null, cssFileName, false);
		}

		/// <summary>
		/// 生成Css文件引用。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="cssFileName">Css文件名称（不包含扩展名,相对于Content目录的路径）。</param>
		/// <param name="enableDynamic">是否启用动态样式(动态样式指保存于客户端的样式目录)。</param>
		/// <returns>返回样式文件的引用。</returns>
		public static MvcHtmlString CssFile(this HtmlHelper html, string cssFileName, bool enableDynamic = false)
		{
			return html.CssFile(null, cssFileName, enableDynamic);
		}

		/// <summary>
		/// 生成Css文件引用。 
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="folderName">样式文件所在文件夹名称</param>
		/// <param name="cssFileName">Css文件名称（不包含扩展名,相对于Content目录的路径）。</param>
		/// <param name="enableDynamic">是否启用动态样式(动态样式指保存于客户端的样式目录)。</param>
		/// <returns>返回样式文件的引用。</returns>
		public static MvcHtmlString CssFile(this HtmlHelper html, string folderName, string cssFileName, bool enableDynamic = false)
		{
			if (enableDynamic && string.IsNullOrWhiteSpace(folderName))
				folderName = "default";
			if (!string.IsNullOrWhiteSpace(folderName))
			{
				string filePath = string.Format("~/Content/{0}/{1}.css", folderName, cssFileName);
				string path1 = UrlHelper.GenerateContentUrl(filePath, html.ViewContext.HttpContext);
				return MvcHtmlString.Create(string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", path1));
			}
			string cssFilePath = string.Format("~/Content/{0}.css", cssFileName);
			string path = UrlHelper.GenerateContentUrl(cssFilePath, html.ViewContext.HttpContext);
			return MvcHtmlString.Create(string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", path));
		}
	}
}
