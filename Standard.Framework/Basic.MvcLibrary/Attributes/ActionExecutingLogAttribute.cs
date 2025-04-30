using System;
using System.Web.Mvc;
using System.Web.Routing;
using Basic.Enums;
using Basic.Loggers;
using BE = Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 表示 ActionExecuting 执行的日志记录。
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public sealed class ActionExecutingLogAttribute : ActionLogAttribute
	{
		/// <summary>
		/// 使用日志初始化 ActionExecutingLogAttribute 类实例，
		/// </summary>
		/// <param name="format">复合格式字符串。</param>
		public ActionExecutingLogAttribute(string format) : base(format, null) { }

		/// <summary>
		/// 使用日志初始化 ActionExecutingLogAttribute 类实例，
		/// </summary>
		/// <param name="format">复合格式字符串。</param>
		/// <param name="args">
		/// 一个字符串对象数组，其中包含零个或多个要设置格式的对象。
		/// 单个字符串对象表示当前Action执行的参数名（argName），或参数对象的属性名（entity.PropertyName）
		/// </param>
		public ActionExecutingLogAttribute(string format, params string[] args) : base(format, args) { }

		/// <summary>
		/// 使用日志初始化 ActionExecutedLogAttribute 类实例，
		/// </summary>
		/// <param name="format">复合格式字符串。</param>
		/// <param name="useArray">是否使用数组参数格式取值。</param>
		/// <param name="args">
		/// 一个字符串对象数组，其中包含零个或多个要设置格式的对象。
		/// 单个字符串对象表示当前Action执行的参数名（argName），或参数对象的属性名（entity.PropertyName）
		/// </param>
		public ActionExecutingLogAttribute(string format, bool useArray, params string[] args) : base(format, useArray, args) { }

		/// <summary>
		/// 在执行操作方法之前调用。
		/// </summary>
		/// <param name="filterContext">筛选器上下文。</param>
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (filterContext == null) { throw new ArgumentNullException("filterContext"); }
			if (filterContext.IsChildAction) { return; }
			string controllerName = (string)filterContext.RouteData.Values["Controller"];
			string actionName = (string)filterContext.RouteData.Values["Action"];
			string UserName = filterContext.HttpContext.User.Identity.Name;
			string hostName = GetRequestAddress(filterContext);
			string url = UrlHelper.GenerateUrl(null, actionName, controllerName, filterContext.RouteData.Values,
						RouteTable.Routes, filterContext.RequestContext, true);
			string msg = base.GetMessage(filterContext);
			EventLogWriter.WriteLogging(BE.GuidConverter.NewGuid, url, hostName, UserName, msg, LogLevel.Information, LogResult.Successful);
		}
	}
}
