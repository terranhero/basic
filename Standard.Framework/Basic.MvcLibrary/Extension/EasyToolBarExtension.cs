using Basic.Messages;
using Basic.MvcLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 工具条按钮扩展
	/// </summary>
	public static class EasyToolBarExtension
	{
		private static readonly string buttonConverter = typeof(EasyStrings).Name;
		#region 生成下拉工具条按钮
		/// <summary>
		/// 生成下拉工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizeBeginCode">按钮授权码开始编号。</param>
		/// <param name="authorizeEndCode">按钮授权码结束编号</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasySplitButton(this HtmlHelper html, int authorizeBeginCode, int authorizeEndCode, string iconClass)
		{
			return html.EasySplitButton(authorizeBeginCode, authorizeEndCode, null, iconClass, null);
		}

		/// <summary>
		/// 生成下拉工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizeBeginCode">按钮授权码开始编号。</param>
		/// <param name="authorizeEndCode">按钮授权码结束编号</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasySplitButton(this HtmlHelper html, int authorizeBeginCode, int authorizeEndCode, string iconClass, IHtmlAttrs htmlAttrs)
		{
			ButtonOptions options = html.CreateOptions(htmlAttrs);
			return html.EasySplitButton(authorizeBeginCode, authorizeEndCode, null, iconClass, options);
		}

		/// <summary>
		/// 生成下拉工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizeBeginCode">按钮授权码开始编号。</param>
		/// <param name="authorizeEndCode">按钮授权码结束编号</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="label">工具条按钮文本,使用资源代码或文本名称</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasySplitButton(this HtmlHelper html, int authorizeBeginCode, int authorizeEndCode, string label, string iconClass)
		{
			return html.EasySplitButton(authorizeBeginCode, authorizeEndCode, label, iconClass, null);
		}

		/// <summary>
		/// 生成下拉工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizeBeginCode">按钮授权码开始编号。</param>
		/// <param name="authorizeEndCode">按钮授权码结束编号</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="label">工具条按钮文本,使用资源代码或文本名称</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasySplitButton(this HtmlHelper html, int authorizeBeginCode, int authorizeEndCode,
			  string label, string iconClass, IHtmlAttrs htmlAttrs)
		{
			bool allowShow = AuthorizeContext.CheckAuthorizationCode(html, authorizeBeginCode, authorizeEndCode);
			if (!allowShow) { return MvcHtmlString.Empty; }
			ButtonOptions options = html.CreateOptions(htmlAttrs);
			options.className = "easyui-splitbutton";
			return html.PrivateEasyButton(null, null, null, label, iconClass, null, options);
		}
		#endregion

		#region 新增按钮
		/// <summary>
		/// 生成新增工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyCreate(this HtmlHelper html, int authorizationCode, ButtonOptions options)
		{
			return html.EasyCreate("Create", authorizationCode, null, options);
		}

		/// <summary>
		/// 生成新增工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="label">按钮显示文本名称</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyCreate(this HtmlHelper html, int authorizationCode, string label, ButtonOptions options)
		{
			return html.EasyCreate("Create", authorizationCode, label, options);
		}

		/// <summary>
		/// 生成新增工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyCreate(this HtmlHelper html, string actionName, int authorizationCode, ButtonOptions options)
		{
			return html.EasyCreate(actionName, authorizationCode, null, options);

		}

		/// <summary>
		/// 生成新增工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="label">按钮显示文本名称</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyCreate(this HtmlHelper html, string actionName, int authorizationCode, string label, ButtonOptions options)
		{
			if (options == null) { options = new ButtonOptions(ButtonTypeEnum.Create); }
			options.id = "lbtCreate"; if (options.ButtonType == ButtonTypeEnum.None) { options.ButtonType = ButtonTypeEnum.Create; }
			if (string.IsNullOrWhiteSpace(label) == true) { label = "EasyStrings:Button_Create"; }
			return html.PrivateEasyButton(null, actionName, null, authorizationCode, label, "icon-create", null, options);
		}

		/// <summary>
		/// 生成新增工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyCreate(this HtmlHelper html, int authorizationCode, IHtmlAttrs htmlAttrs)
		{
			ButtonOptions options = html.CreateOptions(htmlAttrs);
			return html.EasyCreate(authorizationCode, options);
		}
		#endregion

		#region 修改按钮
		/// <summary>
		/// 生成修改工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyEdit(this HtmlHelper html, string actionName, int authorizationCode, ButtonOptions options)
		{
			return html.EasyEdit(actionName, authorizationCode, null, options);
		}

		/// <summary>
		/// 生成修改工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="label">按钮显示文本名称</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyEdit(this HtmlHelper html, string actionName, int authorizationCode, string label, ButtonOptions options)
		{
			if (options == null) { options = new ButtonOptions(ButtonTypeEnum.Update); }
			options.id = "lbtEdit"; if (options.ButtonType == ButtonTypeEnum.None) { options.ButtonType = ButtonTypeEnum.Update; }

			if (string.IsNullOrWhiteSpace(label) == true) { label = "EasyStrings:Button_Edit"; }
			return html.PrivateEasyButton(null, actionName, null, authorizationCode, label, "icon-edit", null, options);
		}

		/// <summary>
		/// 生成修改工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyEdit(this HtmlHelper html, int authorizationCode, ButtonOptions options)
		{
			return html.EasyEdit("Edit", authorizationCode, options);
		}

		/// <summary>
		/// 生成修改工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyEdit(this HtmlHelper html, int authorizationCode, IHtmlAttrs htmlAttrs)
		{
			ButtonOptions options = html.CreateOptions(htmlAttrs);
			return html.EasyEdit(authorizationCode, options);
		}

		/// <summary>
		/// 生成修改工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyEdit(this HtmlHelper html, string actionName, int authorizationCode)
		{
			return html.EasyEdit(actionName, authorizationCode, null);
		}

		/// <summary>
		/// 生成修改工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyEdit(this HtmlHelper html, int authorizationCode)
		{
			return html.EasyEdit(authorizationCode, null);
		}
		#endregion

		#region 删除按钮
		/// <summary>
		/// 生成新增工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="label">按钮显示文本名称</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyDelete(this HtmlHelper html, string actionName, int authorizationCode, string label, ButtonOptions options)
		{
			if (options == null) { options = new ButtonOptions(ButtonTypeEnum.Delete); }
			options.id = "lbtDelete"; if (options.ButtonType == ButtonTypeEnum.None) { options.ButtonType = ButtonTypeEnum.Delete; }
			if (string.IsNullOrWhiteSpace(label) == true) { label = "EasyStrings:Button_Delete"; }
			return html.PrivateEasyButton(null, actionName, null, authorizationCode, label, "icon-remove", null, options);
		}

		/// <summary>
		/// 生成新增工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyDelete(this HtmlHelper html, string actionName, int authorizationCode, ButtonOptions options)
		{
			return html.EasyDelete(actionName, authorizationCode, null, options);
		}

		/// <summary>
		/// 生成删除工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyDelete(this HtmlHelper html, int authorizationCode, ButtonOptions options)
		{
			return html.EasyDelete("Delete", authorizationCode, null, options);
		}

		/// <summary>
		/// 生成新增工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="label">按钮显示文本名称</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyDelete(this HtmlHelper html, int authorizationCode, string label, ButtonOptions options)
		{
			return html.EasyDelete("Delete", authorizationCode, label, options);
		}

		/// <summary>
		/// 生成删除工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyDelete(this HtmlHelper html, int authorizationCode, IHtmlAttrs htmlAttrs)
		{
			ButtonOptions options = html.CreateOptions(htmlAttrs);
			return html.EasyDelete(null, authorizationCode, null, options);
		}

		/// <summary>
		/// 生成删除工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyDelete(this HtmlHelper html, string actionName, int authorizationCode)
		{
			return html.EasyDelete(actionName, authorizationCode, null, null);
		}

		/// <summary>
		/// 生成删除工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyDelete(this HtmlHelper html, int authorizationCode)
		{
			return html.EasyDelete(null, authorizationCode, null, null);
		}
		#endregion

		#region 生成导入按钮
		/// <summary>
		/// 生成导入按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyImport(this HtmlHelper html, string actionName, int authorizationCode, ButtonOptions options)
		{
			return html.EasyImport(actionName, authorizationCode, null, options);
		}

		/// <summary>
		/// 生成导入按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="label">按钮显示文本名称</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyImport(this HtmlHelper html, string actionName, int authorizationCode, string label, ButtonOptions options)
		{
			if (options == null) { options = new ButtonOptions(ButtonTypeEnum.Import); }
			options.id = "lbtImport"; if (options.ButtonType == ButtonTypeEnum.None) { options.ButtonType = ButtonTypeEnum.Import; }

			if (string.IsNullOrWhiteSpace(label) == true) { label = "EasyStrings:Button_Import"; }
			return html.PrivateEasyButton(null, actionName, null, authorizationCode, label, "icon-import", null, options);
		}

		/// <summary>
		/// 生成导入按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyImport(this HtmlHelper html, int authorizationCode, ButtonOptions options)
		{
			return html.EasyImport("Import", authorizationCode, null, options);
		}

		/// <summary>
		/// 生成导入按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyImport(this HtmlHelper html, int authorizationCode, IHtmlAttrs htmlAttrs)
		{
			ButtonOptions options = html.CreateOptions(htmlAttrs);
			return html.EasyImport("Import", authorizationCode, null, options);
		}

		/// <summary>
		/// 生成导入按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="actionName">请求名称</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyImport(this HtmlHelper html, string actionName, int authorizationCode)
		{
			return html.EasyImport(actionName, authorizationCode, null, null);
		}

		/// <summary>
		/// 生成导入按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyImport(this HtmlHelper html, int authorizationCode)
		{
			return html.EasyImport(authorizationCode, null);
		}
		#endregion

		#region 生成导出按钮
		/// <summary>
		/// 生成导出按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyExport(this HtmlHelper html, string actionName, int authorizationCode, ButtonOptions options)
		{
			if (options == null) { options = new ButtonOptions(ButtonTypeEnum.Export); }
			options.id = "lbtExport"; if (options.ButtonType == ButtonTypeEnum.None) { options.ButtonType = ButtonTypeEnum.Export; }
			return html.PrivateEasyButton(null, actionName, null, authorizationCode, "EasyStrings:Button_Export", "icon-export", null, options);
		}

		/// <summary>
		/// 生成导出按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyExport(this HtmlHelper html, int authorizationCode, ButtonOptions options)
		{
			return html.EasyExport("Export", authorizationCode, options);
		}

		/// <summary>
		/// 生成导出按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="actionName">请求名称</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyExport(this HtmlHelper html, string actionName, int authorizationCode)
		{
			return html.EasyExport(actionName, authorizationCode, new ButtonOptions());
		}

		/// <summary>
		/// 生成导出按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyExport(this HtmlHelper html, int authorizationCode)
		{
			return html.EasyExport(null, authorizationCode, new ButtonOptions());
		}
		#endregion

		#region 生成高级查询按钮
		/// <summary>
		/// 生成高级查询按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyComplexSearch(this HtmlHelper html, int authorizationCode, ButtonOptions options)
		{
			return html.EasyComplexSearch("ComplexSearch", authorizationCode, options);
		}

		/// <summary>
		/// 生成高级查询按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyComplexSearch(this HtmlHelper html, string actionName, int authorizationCode, ButtonOptions options)
		{
			if (options == null) { options = new ButtonOptions(ButtonTypeEnum.ComplexSearch); }
			options.id = "lbtComplexSearch"; if (options.ButtonType == ButtonTypeEnum.None) { options.ButtonType = ButtonTypeEnum.ComplexSearch; }
			return html.EasyLinkButton(null, actionName, null, authorizationCode, "EasyStrings:Button_ComplexSearch", "icon-complexsearch", null, options);
		}

		/// <summary>
		/// 生成高级查询按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyComplexSearch(this HtmlHelper html, int authorizationCode, IHtmlAttrs htmlAttrs)
		{
			ButtonOptions options = html.CreateOptions(htmlAttrs);
			return html.EasyComplexSearch(authorizationCode, options);
		}

		/// <summary>
		/// 生成高级查询按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyComplexSearch(this HtmlHelper html, int authorizationCode)
		{
			return html.EasyComplexSearch(authorizationCode, null);
		}

		/// <summary>
		/// 生成高级查询按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyComplexSearch(this HtmlHelper html, string actionName, int authorizationCode)
		{
			return html.EasyComplexSearch(actionName, authorizationCode, null);
		}
		#endregion

		#region 查询按钮
		/// <summary>
		/// 生成查询按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasySearch(this HtmlHelper html, int authorizationCode, ButtonOptions options)
		{
			return html.EasySearch("Search", authorizationCode, null, options);
		}

		/// <summary>
		/// 生成删除工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasySearch(this HtmlHelper html, int authorizationCode, IHtmlAttrs htmlAttrs)
		{
			ButtonOptions options = html.CreateOptions(htmlAttrs);
			return html.EasySearch("Search", authorizationCode, null, options);
		}

		/// <summary>
		/// 生成删除工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasySearch(this HtmlHelper html, int authorizationCode)
		{
			return html.EasySearch("Search", authorizationCode, null, null);
		}

		/// <summary>
		/// 生成删除工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasySearch(this HtmlHelper html, string actionName, int authorizationCode)
		{
			return html.EasySearch(actionName, authorizationCode, null, null);
		}

		/// <summary>
		/// 使用自定义查询请求，生成查询按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">使用自定义查询请求</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasySearch(this HtmlHelper html, string actionName, int authorizationCode, ButtonOptions options)
		{
			return html.EasySearch(actionName, authorizationCode, null, options);
		}

		/// <summary>
		/// 使用自定义查询请求，生成查询按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">使用自定义查询请求</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="label">按钮显示文本名称</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasySearch(this HtmlHelper html, string actionName, int authorizationCode, string label, ButtonOptions options)
		{
			if (options == null) { options = new ButtonOptions(ButtonTypeEnum.Search); }
			options.id = "lbtSearch"; if (options.ButtonType == ButtonTypeEnum.None) { options.ButtonType = ButtonTypeEnum.Search; }
			if (string.IsNullOrWhiteSpace(label) == true) { label = "EasyStrings:Button_Search"; }
			return html.PrivateEasyButton(null, actionName, null, authorizationCode, label, "icon-search", null, options);
		}


		#endregion

		#region 打印按钮
		/// <summary>
		/// 生成打印按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyPrint(this HtmlHelper html, int authorizationCode)
		{
			return html.EasyPrint(authorizationCode, null);
		}
		/// <summary>
		/// 生成打印按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyPrint(this HtmlHelper html, int authorizationCode, ButtonOptions options)
		{
			return html.EasyPrint("Print", authorizationCode, options);
		}

		/// <summary>
		/// 生成打印按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">请求名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyPrint(this HtmlHelper html, string actionName, int authorizationCode)
		{
			return html.EasyPrint(actionName, authorizationCode, null);
		}

		/// <summary>
		/// 生成打印按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">使用自定义查询请求</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyPrint(this HtmlHelper html, string actionName, int authorizationCode, ButtonOptions options)
		{
			if (options == null) { options = new ButtonOptions(ButtonTypeEnum.Print); }
			options.id = "lbtPrint"; if (options.ButtonType == ButtonTypeEnum.None) { options.ButtonType = ButtonTypeEnum.Print; }
			return html.PrivateEasyButton(null, actionName, null, authorizationCode, "EasyStrings:Button_Print", "icon-print", null, options);
		}
		#endregion

		#region 保存按钮

		/// <summary>
		/// 生成保存工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasySave(this HtmlHelper html, ButtonOptions options)
		{
			if (options == null) { options = new ButtonOptions(ButtonTypeEnum.Save); }
			options.id = "lbtSave"; if (options.ButtonType == ButtonTypeEnum.None) { options.ButtonType = ButtonTypeEnum.Save; }
			return html.PrivateEasyButton(null, null, null, "EasyStrings:Button_Save", "icon-save", null, options);
		}

		/// <summary>
		/// 生成保存工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasySave(this HtmlHelper html)
		{
			return html.EasySave(null);
		}
		#endregion

		#region 取消按钮
		/// <summary>
		/// 生成保存工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyCancel(this HtmlHelper html, ButtonOptions options)
		{
			if (options == null) { options = new ButtonOptions(ButtonTypeEnum.Custom); }
			options.id = "lbtCancel"; if (options.ButtonType == ButtonTypeEnum.None) { options.ButtonType = ButtonTypeEnum.Custom; }

			return html.PrivateEasyButton(null, null, null, "EasyStrings:Button_Cancel", "icon-cancel", null, options);
		}

		/// <summary>
		/// 生成保存工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyCancel(this HtmlHelper html)
		{
			return html.EasyCancel(null);
		}
		#endregion

		#region 普通工具条按钮
		/// <summary>
		/// 生成工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">操作的名称。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyLinkButton(this HtmlHelper html, string actionName, int authorizationCode, string iconClass, IHtmlAttrs htmlAttrs)
		{
			return html.EasyLinkButton(null, actionName, null, authorizationCode, null, iconClass, null, htmlAttrs);
		}

		/// <summary>
		/// 生成工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">操作的名称。</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="label">工具条按钮文本,使用资源代码或文本名称</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyLinkButton(this HtmlHelper html, string actionName, int authorizationCode, string label, string iconClass, IHtmlAttrs htmlAttrs)
		{
			return html.EasyLinkButton(null, actionName, null, authorizationCode, label, iconClass, null, htmlAttrs);
		}

		/// <summary>
		/// 生成工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="actionName">操作的名称。</param>
		/// <param name="controllerName">控制器的名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="label">工具条按钮文本,使用资源代码或文本名称</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyLinkButton(this HtmlHelper html, string routeName, string actionName, string controllerName, int authorizationCode,
			  string label, string iconClass, IHtmlAttrs htmlAttrs)
		{
			return html.EasyLinkButton(routeName, actionName, controllerName, authorizationCode, label, iconClass, null, htmlAttrs);
		}

		/// <summary>
		/// 生成工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">操作的名称。</param>
		/// <param name="controllerName">控制器的名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="label">工具条按钮文本,使用资源代码或文本名称</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyLinkButton(this HtmlHelper html, string actionName, string controllerName, int authorizationCode,
			  string label, string iconClass, IHtmlAttrs htmlAttrs)
		{
			return html.EasyLinkButton(null, actionName, controllerName, authorizationCode, label, iconClass, null, htmlAttrs);
		}

		/// <summary>
		/// 生成工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">操作的名称。</param>
		/// <param name="controllerName">控制器的名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="label">工具条按钮文本,使用资源代码或文本名称</param>
		/// <param name="routeValues">一个包含路由参数的对象。</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyLinkButton(this HtmlHelper html, string actionName, string controllerName, int authorizationCode,
			  string label, string iconClass, RouteValueDictionary routeValues, IHtmlAttrs htmlAttrs)
		{
			return html.EasyLinkButton(null, actionName, controllerName, authorizationCode, label, iconClass, routeValues, htmlAttrs);
		}

		/// <summary>
		/// 生成工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="actionName">操作的名称。</param>
		/// <param name="controllerName">控制器的名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="label">工具条按钮文本,使用资源代码或文本名称</param>
		/// <param name="routeValues">一个包含路由参数的对象。</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyLinkButton(this HtmlHelper html, string routeName, string actionName, string controllerName, int authorizationCode,
			  string label, string iconClass, RouteValueDictionary routeValues, IHtmlAttrs htmlAttrs)
		{
			if (htmlAttrs == null)
				htmlAttrs = new HtmlAttrs();
			htmlAttrs.className = "easyui-linkbutton";
			ButtonOptions options = html.CreateOptions(htmlAttrs);
			return html.PrivateEasyButton(routeName, actionName, controllerName, authorizationCode, label, iconClass, routeValues, options);
		}

		/// <summary>
		/// 生成工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">操作的名称。</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="label">工具条按钮文本,使用资源代码或文本名称</param>
		/// <param name="attrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyButton(this HtmlHelper html, string actionName, string label, string iconClass, ButtonOptions attrs)
		{
			return html.PrivateEasyButton(null, actionName, null, label, iconClass, null, attrs);
		}

		/// <summary>
		/// 生成工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="actionName">操作的名称。</param>
		/// <param name="controllerName">控制器的名称</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="label">工具条按钮文本,使用资源代码或文本名称</param>
		/// <param name="attrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyButton(this HtmlHelper html, string actionName, string controllerName, string label, string iconClass, ButtonOptions attrs)
		{
			return html.PrivateEasyButton(null, actionName, controllerName, label, iconClass, null, attrs);
		}

		/// <summary>
		/// 生成工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="actionName">操作的名称。</param>
		/// <param name="controllerName">控制器的名称</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="label">工具条按钮文本,使用资源代码或文本名称</param>
		/// <param name="attrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		public static MvcHtmlString EasyButton(this HtmlHelper html, string routeName, string actionName, string controllerName,
			string label, string iconClass, ButtonOptions attrs)
		{
			if (attrs == null) { attrs = new ButtonOptions() { className = "easyui-linkbutton" }; }
			else { attrs.className = "easyui-linkbutton"; }

			return html.PrivateEasyButton(routeName, actionName, controllerName, label, iconClass, null, attrs);
		}
		#endregion

		#region 生成工具条按钮
		/// <summary>
		/// 生成参数
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一个对象，其中包含 LinkButton 特性。</returns>
		private static ButtonOptions CreateOptions(this HtmlHelper html, ButtonOptions options)
		{
			return options;
		}

		/// <summary>
		/// 生成参数
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一个对象，其中包含 LinkButton 特性。</returns>
		private static ButtonOptions CreateOptions(this HtmlHelper html, IHtmlAttrs htmlAttrs)
		{
			if (htmlAttrs is ButtonOptions) { return htmlAttrs as ButtonOptions; }
			ButtonOptions options = new ButtonOptions();
			return html.CreateOptions(options, htmlAttrs);
		}

		/// <summary>
		/// 生成参数
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <param name="htmlAttrs">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>一个对象，其中包含 LinkButton 特性。</returns>
		private static ButtonOptions CreateOptions(this HtmlHelper html, ButtonOptions options, IHtmlAttrs htmlAttrs)
		{
			if (htmlAttrs == null) { return options; }
			foreach (var keyValue in htmlAttrs)
			{
				if (keyValue.Key == "width") { options.width = (int)keyValue.Value; }
				else if (keyValue.Key == "height") { options.height = (int)keyValue.Value; }
				else if (keyValue.Key == "enabled-state") { options.enabledState = (PageStatus)(int)keyValue.Value; }
				else if (keyValue.Key == "visibled-state") { options.visibledState = (PageStatus)(int)keyValue.Value; }
				else { options[keyValue.Key] = keyValue.Value; }
			}
			return options;
		}
		/// <summary>
		/// 生成工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="actionName">操作的名称。</param>
		/// <param name="controllerName">控制器的名称</param>
		/// <param name="authorizationCode">授权代码</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="label">工具条按钮文本,使用资源代码或文本名称</param>
		/// <param name="routeValues">一个包含路由参数的对象。</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		private static MvcHtmlString PrivateEasyButton(this HtmlHelper html, string routeName, string actionName, string controllerName, int authorizationCode,
			  string label, string iconClass, RouteValueDictionary routeValues, ButtonOptions options)
		{
			bool allowShow = AuthorizeContext.CheckAuthorizationCode(html, authorizationCode);
			if (!allowShow) { return MvcHtmlString.Empty; }

			return html.PrivateEasyButton(routeName, actionName, controllerName, label, iconClass, routeValues, options);
		}

		/// <summary>
		/// 生成工具条按钮
		/// </summary>
		/// <param name="html">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="routeName">路由名称</param>
		/// <param name="actionName">操作的名称。</param>
		/// <param name="controllerName">控制器的名称</param>
		/// <param name="iconClass">工具条按钮样式</param>
		/// <param name="label">工具条按钮文本,使用资源代码或文本名称</param>
		/// <param name="routeValues">一个包含路由参数的对象。</param>
		/// <param name="options">一个对象，其中包含要为该元素设置的 HTML 特性。</param>
		/// <returns>返回一个A元素，同时包含图片和文本</returns>
		private static MvcHtmlString PrivateEasyButton(this HtmlHelper html, string routeName, string actionName, string controllerName,
			  string label, string iconClass, RouteValueDictionary routeValues, ButtonOptions options)
		{
			RouteCollection routeCollection = html.RouteCollection;
			RequestContext requestContext = html.ViewContext.RequestContext;
			HttpRequestBase request = html.ViewContext.HttpContext.Request;
			TagBuilder builder = new TagBuilder("a");
			if (!string.IsNullOrWhiteSpace(label))
			{
				string sourceName = label, converterName = null;
				if (label.IndexOf(":") >= 0)
				{
					string[] strArray = label.Split(':');
					converterName = strArray[0]; sourceName = strArray[1];
				}
				if (buttonConverter == converterName)
				{
					System.Globalization.CultureInfo culture = request.GetCultureInfo();
					string cultureText = string.Concat(sourceName, "_", culture.Name);
					string msgText = MessageContext.GetString(converterName, cultureText);
					if (string.IsNullOrWhiteSpace(msgText)) { msgText = MessageContext.GetString(converterName, sourceName); }
					else if (cultureText == msgText) { msgText = MessageContext.GetString(converterName, sourceName); }
					builder.InnerHtml = msgText ?? label;
				}
				else
				{
					string msgText = request.GetString(converterName, sourceName);
					if (string.IsNullOrWhiteSpace(msgText)) { msgText = request.GetString(converterName, sourceName); }
					else if (sourceName == msgText) { msgText = request.GetString(converterName, sourceName); }
					builder.InnerHtml = msgText ?? sourceName;
				}
			}
			builder.MergeAttribute("href", "javascript:void(0);", true);
			if (!string.IsNullOrWhiteSpace(actionName))
			{
				options.url = UrlHelper.GenerateUrl(routeName, actionName, controllerName, routeValues, routeCollection, requestContext, true);
			}
			options.plain = true;
			options.iconCls = iconClass;
			//if (htmlAttrs != null) { options.m }
			builder.MergeAttributes<string, object>(options, true);
			return builder.ToMvcHtmlString(TagRenderMode.Normal);
		}
		#endregion
	}
}
