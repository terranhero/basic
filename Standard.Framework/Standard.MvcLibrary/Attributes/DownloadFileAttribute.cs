using System;
using System.Collections.Specialized;
using Basic.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Basic.MvcLibrary
{
	/// <summary>下载文件自定义名称特性</summary>
	public sealed class DownloadFileAttribute : Attribute, IActionFilter
	{
		/// <summary>
		/// 下载文件在视图字典的键值。
		/// </summary>
		private const string ViewDataKey = "7B5DE7739B4F447697AA4AB1AC960798";
		private readonly string _DownloadFileName;
		private readonly string _ConverterName = null;
		private readonly bool _IsLocaltion = false;

		/// <summary>
		/// 初始化 DownloadFileAttribute 类实例
		/// </summary>
		/// <param name="fileName">下载文件名称，不含扩展名。</param>
		public DownloadFileAttribute(string fileName) { _DownloadFileName = fileName; }

		/// <summary>
		/// 初始化 DownloadFileAttribute 类实例
		/// </summary>
		/// <param name="keyName">下载文件名称，不含扩展名（资源键值）。</param>
		/// <param name="islocaltion">是否需要对文件名做本地化资源转换。</param>
		public DownloadFileAttribute(string keyName, bool islocaltion) { _DownloadFileName = keyName; _IsLocaltion = islocaltion; }

		/// <summary>
		/// 初始化 DownloadFileAttribute 类实例
		/// </summary>
		/// <param name="converterName">资源转换器名称。</param>
		/// <param name="keyName">资源键值。</param>
		public DownloadFileAttribute(string converterName, string keyName)
		{
			_IsLocaltion = true;
			_ConverterName = converterName;
			_DownloadFileName = keyName;
		}

		/// <summary>获取下载文件名称</summary>
		/// <param name="headers">当前Http请求</param>
		/// <returns>返回下载文件名称</returns>
		public static string GetFileName(NameValueCollection headers)
		{
			if (headers[ViewDataKey] != null)
				return headers[ViewDataKey];
			return "Data";
		}

		/// <summary>
		/// Called after the action method executes.
		/// </summary>
		/// <param name="filterContext">The filter context.</param>
		public void OnActionExecuted(ActionExecutedContext filterContext) { }

		/// <summary>
		/// Called before an action method executes.
		/// </summary>
		/// <param name="filterContext">The filter context.</param>
		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (string.IsNullOrWhiteSpace(_DownloadFileName)) { return; }
			HttpRequest request = filterContext.HttpContext.Request;
			string resourceName = _DownloadFileName;
			if (_IsLocaltion)
			{
				//Messages.MessageContext.GetString(_ConverterName, resourceName);
				string fileName = request.GetString(_ConverterName, resourceName);
				filterContext.HttpContext.Request.Headers.Add(ViewDataKey, fileName);
			}
			else { filterContext.HttpContext.Request.Headers.Add(ViewDataKey, _DownloadFileName); }
		}

	}
}
