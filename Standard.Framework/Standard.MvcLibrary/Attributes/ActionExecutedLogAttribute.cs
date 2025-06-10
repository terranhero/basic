using Basic.EntityLayer;
using Basic.Interfaces;
using Basic.Loggers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basic.MvcLibrary
{
	/// <summary>表示 OnActionExecuted 执行的日志记录。</summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public sealed class ActionExecutedLogAttribute : ActionFilterAttribute
	{
		internal readonly string _Format;
		internal readonly bool _UseArray;
		internal readonly string[] _Arguments;
		internal const string BatchNumber = "8E1103CCC862459B875FC7C4726F4289";
		/// <summary>
		/// 使用日志初始化 ActionExecutedLogAttribute 类实例
		/// </summary>
		/// <param name="format">复合格式字符串。</param>
		public ActionExecutedLogAttribute(string format) : this(format, false, null) { }

		/// <summary>
		/// 使用日志初始化 ActionExecutedLogAttribute 类实例，
		/// </summary>
		/// <param name="format">复合格式字符串。</param>
		/// <param name="args">
		/// 一个字符串对象数组，其中包含零个或多个要设置格式的对象。
		/// 单个字符串对象表示当前Action执行的参数名（argName），或参数对象的属性名（entity.PropertyName）
		/// </param>
		public ActionExecutedLogAttribute(string format, params string[] args) : this(format, false, args) { }

		/// <summary>
		/// 使用日志初始化 ActionExecutedLogAttribute 类实例，
		/// </summary>
		/// <param name="format">复合格式字符串。</param>
		/// <param name="useArray">是否使用数组参数格式取值。</param>
		/// <param name="args">
		/// 一个字符串对象数组，其中包含零个或多个要设置格式的对象。
		/// 单个字符串对象表示当前Action执行的参数名（argName），或参数对象的属性名（entity.PropertyName）
		/// </param>
		public ActionExecutedLogAttribute(string format, bool useArray, params string[] args)
		{
			_Format = format; _UseArray = useArray; _Arguments = args;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			ActionExecutedContext aeContext = await next();
			await OnActionExecutedAsync(aeContext);
		}

		/// <summary></summary>
		/// <param name="context"></param>
		public async Task OnActionExecutedAsync(ActionExecutedContext context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
			ILoggerWriter _writer = context.HttpContext.RequestServices.GetService<ILoggerWriter>();
			if (_writer == null) { throw new ArgumentNullException("ILoggerWriter"); }

			Controller controller = (Controller)context.Controller; Guid guid = GuidConverter.NewGuid;
			if (controller.ViewData.ContainsKey(BatchNumber)) { guid = (Guid)controller.ViewData[BatchNumber]; }

			UrlHelper urlHelper = new UrlHelper(context);
			if (context.ModelState.IsValid == false) { return; }
			if (context.ExceptionHandled == false && context.Exception == null)
			{
				string hostName = GetRequestAddress(context);
				string controllerName = (string)context.RouteData.Values["Controller"];
				string actionName = (string)context.RouteData.Values["Action"];
				string UserName = context.HttpContext.User.Identity.Name;
				string url = urlHelper.Action(actionName, controllerName, context.RouteData.Values);
				string msg = await GetMessageAsync(context);
				_writer.Information(guid, url, hostName, UserName, msg);
			}
			else if (context.ExceptionHandled == false && context.Exception != null)
			{
				string hostName = GetRequestAddress(context);
				string controllerName = (string)context.RouteData.Values["Controller"];
				string actionName = (string)context.RouteData.Values["Action"];

				string UserName = context.HttpContext.User.Identity.Name;
				string url = urlHelper.Action(actionName, controllerName, context.RouteData.Values);
				_writer.Error(guid, url, hostName, UserName, context.Exception);
				if (IsAjaxRequest(context.HttpContext.Request))
				{
					ModelStateDictionary modelState = new ModelStateDictionary();
					modelState.AddModelError("", context.Exception.Message);
					modelState.AddModelError("", context.Exception.StackTrace);
					if (context.Exception.InnerException != null)
					{
						modelState.AddModelError("", context.Exception.InnerException.Message);
						modelState.AddModelError("", context.Exception.InnerException.StackTrace);
					}
					context.Result = new Basic.MvcLibrary.JsonResult(modelState);
				}
				else
				{
					context.Result = new ViewResult()
					{
						ViewName = "Error",
						ViewData = new ViewDataDictionary<Exception>(controller.ViewData, context.Exception),
						TempData = controller.TempData
					};
				}
				context.ExceptionHandled = true;
			}
			return;
		}

		private static bool IsAjaxRequest(HttpRequest request)
		{
			return string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
				string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal);
		}

		/// <summary>
		/// 根据参数获取日志消息。
		/// </summary>
		/// <param name="context">当前控制器上下文信息</param>
		/// <returns>返回格式化后的日志消息。</returns>
		private async Task<string> GetMessageAsync(ActionExecutedContext context)
		{
			if (_Arguments == null || _Arguments.Length == 0) { return _Format; }
			List<object> list = new List<object>(_Arguments.Length);
			Controller controller = (Controller)context.Controller;
			ValueProviderFactoriesContext valueProviders = new ValueProviderFactoriesContext(context);
			await valueProviders.CreateAsync(controller.ControllerContext.ValueProviderFactories);

			if (_UseArray)
			{
				List<string> msgs = new List<string>(100);
				int index = 0;
				while (true)
				{
					list.Clear(); bool isContinue = true;
					foreach (string arg in _Arguments)
					{
						string arg1 = arg.Replace("[]", string.Concat("[", index, "]"));
						ValueProviderResult result = valueProviders.GetValue(arg1);
						if (result == ValueProviderResult.None) { isContinue = false; break; }
						else { list.Add(result.FirstValue); }
					}
					if (isContinue == false) { break; }
					index++;
					msgs.Add(string.Format(_Format, list.ToArray()));
				}
				return string.Join(",", msgs);
			}
			else
			{
				foreach (string arg in _Arguments)
				{
					ValueProviderResult result = valueProviders.GetValue(arg);
					if (result != ValueProviderResult.None) { list.Add(result.FirstValue); }
				}
			}
			return string.Format(_Format, list.ToArray());
		}

		/// <summary>获取 HttpRequest中请求的地址。</summary>
		/// <param name="context">当前控制器上下文信息。</param>
		/// <returns>返回地址信息。</returns>
		private string GetRequestAddress(FilterContext context)
		{
			HttpRequest request = context.HttpContext.Request;
			IHeaderDictionary headers = request.Headers;
			if (headers.TryGetValue("HTTP_X_FORWARDED_FOR", out StringValues value)) { return value; }
			else if (headers.TryGetValue("REMOTE_ADDR", out StringValues value1)) { return value1; }
			else { return request.Host.Host; }
		}

		private sealed class ValueProviderFactoriesContext : ValueProviderFactoryContext
		{
			/// <summary>Creates a new Microsoft.AspNetCore.Mvc.ModelBinding.ValueProviderFactoryContext.</summary>
			/// <param name="context">The Microsoft.AspNetCore.Mvc.ModelBinding.ValueProviderFactoryContext.ActionContext.</param>
			public ValueProviderFactoriesContext(ActionContext context) : base(context) { }

			/// <summary></summary>
			/// <param name="factories"></param>
			/// <returns></returns>
			public async Task CreateAsync(IList<IValueProviderFactory> factories)
			{
				foreach (IValueProviderFactory factory in factories)
				{
					await factory.CreateValueProviderAsync(this);
				}
			}

			/// <summary>Retrieves a value object using the specified key.</summary>
			/// <param name="key">The key of the value object to retrieve.</param>
			/// <returns>The value object for the specified key. If the exact key is not found, null.</returns>
			public ValueProviderResult GetValue(string key)
			{
				if (ValueProviders == null || ValueProviders.Count == 0) { return ValueProviderResult.None; }
				foreach (IValueProvider provider in ValueProviders)
				{
					if (provider.ContainsPrefix(key) == true) { return provider.GetValue(key); }
				}
				return ValueProviderResult.None;
			}
		}
	}
}
