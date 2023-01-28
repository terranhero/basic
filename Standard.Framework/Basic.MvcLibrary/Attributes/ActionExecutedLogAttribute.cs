using Basic.EntityLayer;
using Basic.Enums;
using Basic.LogInfo;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 表示 OnActionExecuted 执行的日志记录。
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public sealed class ActionExecutedLogAttribute : ActionLogAttribute
	{
		/// <summary>
		/// 使用日志初始化 ActionExecutedLogAttribute 类实例，
		/// </summary>
		/// <param name="format">复合格式字符串。</param>
		public ActionExecutedLogAttribute(string format) : base(format, null) { }

		/// <summary>
		/// 使用日志初始化 ActionExecutedLogAttribute 类实例，
		/// </summary>
		/// <param name="format">复合格式字符串。</param>
		/// <param name="args">
		/// 一个字符串对象数组，其中包含零个或多个要设置格式的对象。
		/// 单个字符串对象表示当前Action执行的参数名（argName），或参数对象的属性名（entity.PropertyName）
		/// </param>
		public ActionExecutedLogAttribute(string format, params string[] args) : base(format, args) { }

		/// <summary>
		/// 使用日志初始化 ActionExecutedLogAttribute 类实例，
		/// </summary>
		/// <param name="format">复合格式字符串。</param>
		/// <param name="useArray">是否使用数组参数格式取值。</param>
		/// <param name="args">
		/// 一个字符串对象数组，其中包含零个或多个要设置格式的对象。
		/// 单个字符串对象表示当前Action执行的参数名（argName），或参数对象的属性名（entity.PropertyName）
		/// </param>
		public ActionExecutedLogAttribute(string format, bool useArray, params string[] args) : base(format, useArray, args) { }

		/// <summary>
		/// 在执行操作方法后调用。
		/// </summary>
		/// <param name="filterContext"> 筛选器上下文。</param>
		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			if (filterContext == null) { throw new ArgumentNullException("filterContext"); }
			if (filterContext.IsChildAction) { return; }
			ActionDescriptor action = filterContext.ActionDescriptor; System.Guid guid = GuidConverter.NewGuid;
			ControllerDescriptor controller = filterContext.ActionDescriptor.ControllerDescriptor;
			if (filterContext.Controller.ViewData.ContainsKey(BatchNumber))
				guid = (Guid)filterContext.Controller.ViewData[BatchNumber];

			bool allowLogging = action.IsDefined(typeof(NoneLoggingAttribute), true);
			if (!allowLogging) { allowLogging = controller.IsDefined(typeof(NoneLoggingAttribute), true); }

			if (!allowLogging && !filterContext.ExceptionHandled && filterContext.Exception == null)
			{
				if (filterContext.Controller.ViewData.ModelState.IsValid == false) { return; }
				string hostName = GetRequestAddress(filterContext);
				string controllerName = (string)filterContext.RouteData.Values["Controller"];
				string actionName = (string)filterContext.RouteData.Values["Action"];
				string UserName = filterContext.HttpContext.User.Identity.Name;
				string url = UrlHelper.GenerateUrl(null, actionName, controllerName, filterContext.RouteData.Values,
						RouteTable.Routes, filterContext.RequestContext, true);
				string msg = base.GetMessage(filterContext);
				EventLogWriter.WriteLogging(guid, url, hostName, UserName, msg, LogLevel.Information, LogResult.Successful);
			}
			else if (!filterContext.ExceptionHandled && filterContext.Exception != null)
			{
				string hostName = GetRequestAddress(filterContext);
				string controllerName = (string)filterContext.RouteData.Values["Controller"];
				string actionName = (string)filterContext.RouteData.Values["Action"];
				System.Web.HttpException httpException = new System.Web.HttpException(null, filterContext.Exception);
				if (httpException.GetHttpCode() == 500)
				{
					string UserName = filterContext.HttpContext.User.Identity.Name;
					string url = UrlHelper.GenerateUrl(null, actionName, controllerName,
						filterContext.RouteData.Values, RouteTable.Routes, filterContext.RequestContext, true);
					EventLogWriter.WriteLogging(guid, url, hostName, UserName, filterContext.Exception);
					if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
					{
						ModelStateDictionary modelState = new ModelStateDictionary();
						modelState.AddModelError("", filterContext.Exception.Message);
						modelState.AddModelError("", filterContext.Exception.StackTrace);
						if (filterContext.Exception.InnerException != null)
						{
							modelState.AddModelError("", filterContext.Exception.InnerException.Message);
							modelState.AddModelError("", filterContext.Exception.InnerException.StackTrace);
						}
						filterContext.Result = new JsonMvcResult(modelState);
					}
					else
					{
						HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
						filterContext.Result = new System.Web.Mvc.ViewResult()
						{
							ViewName = "Error",
							ViewData = new System.Web.Mvc.ViewDataDictionary<System.Web.Mvc.HandleErrorInfo>(model),
							TempData = filterContext.Controller.TempData
						};
					}
					filterContext.ExceptionHandled = true;
				}
			}
		}
	}
}
