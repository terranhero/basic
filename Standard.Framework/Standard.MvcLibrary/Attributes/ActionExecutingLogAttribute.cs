using System;
using System.Threading.Tasks;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Loggers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

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

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (context == null) { throw new ArgumentNullException("context"); }

			if (next == null) { throw new ArgumentNullException("next"); }
			UrlHelper urlHelper = new UrlHelper(context);
			if (context.ModelState.IsValid == false) { return Task.CompletedTask; }
			string hostName = GetRequestAddress(context);
			string controllerName = (string)context.RouteData.Values["Controller"];
			string actionName = (string)context.RouteData.Values["Action"];
			string UserName = context.HttpContext.User.Identity.Name;
			string url = urlHelper.Action(actionName, controllerName, context.RouteData.Values);
			string msg = base.GetMessage(context);
			EventLogWriter.WriteLogging(GuidConverter.NewGuid, url, hostName, UserName, msg, LogLevel.Information, LogResult.Successful);
			return Task.CompletedTask;
		}
	}
}
