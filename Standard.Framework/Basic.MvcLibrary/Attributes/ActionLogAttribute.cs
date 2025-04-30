using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 表示 OnActionExecuted 执行的日志记录。
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public abstract class ActionLogAttribute : FilterAttribute, IActionFilter
	{
		internal readonly string _Format;
		internal readonly bool _UseArray;
		internal readonly string[] _Arguments;
		internal const string BatchNumber = "8E1103CCC862459B875FC7C4726F4289";
		internal const string ArgumentNumber = "6A28FCB733FB46B49ADFB56A26CE3D29";
		/// <summary>
		/// 使用日志初始化 ActionExecutedLogAttribute 类实例
		/// </summary>
		/// <param name="format">复合格式字符串。</param>
		protected ActionLogAttribute(string format) : this(format, false, null) { }

		/// <summary>
		/// 使用日志初始化 ActionExecutedLogAttribute 类实例，
		/// </summary>
		/// <param name="format">复合格式字符串。</param>
		/// <param name="args">
		/// 一个字符串对象数组，其中包含零个或多个要设置格式的对象。
		/// 单个字符串对象表示当前Action执行的参数名（argName），或参数对象的属性名（entity.PropertyName）
		/// </param>
		protected ActionLogAttribute(string format, string[] args) : this(format, false, args) { }

		/// <summary>
		/// 使用日志初始化 ActionExecutedLogAttribute 类实例，
		/// </summary>
		/// <param name="format">复合格式字符串。</param>
		/// <param name="useArray">是否使用数组参数格式取值。</param>
		/// <param name="args">
		/// 一个字符串对象数组，其中包含零个或多个要设置格式的对象。
		/// 单个字符串对象表示当前Action执行的参数名（argName），或参数对象的属性名（entity.PropertyName）
		/// </param>
		protected ActionLogAttribute(string format, bool useArray, string[] args)
		{
			_Format = format; _UseArray = useArray; _Arguments = args;
		}

		/// <summary>
		/// 根据参数获取日志消息。
		/// </summary>
		/// <param name="context">当前控制器上下文信息</param>
		/// <returns>返回格式化后的日志消息。</returns>
		protected string GetMessage(ControllerContext context)
		{
			if (_Arguments == null || _Arguments.Length == 0) { return _Format; }
			List<object> list = new List<object>(_Arguments.Length);
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
						ValueProviderResult result = context.Controller.ValueProvider.GetValue(arg1);
						if (result == null) { isContinue = false; break; }
						if (result != null) { list.Add(result.AttemptedValue); }
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
					ValueProviderResult result = context.Controller.ValueProvider.GetValue(arg);
					if (result != null) { list.Add(result.AttemptedValue); }
				}
			}
			return string.Format(_Format, list.ToArray());
		}

		/// <summary>
		/// 获取 HttpRequest中请求的地址。
		/// </summary>
		/// <param name="context">当前控制器上下文信息。</param>
		/// <returns>返回地址信息。</returns>
		protected string GetRequestAddress(ControllerContext context)
		{
			HttpRequestBase request = context.HttpContext.Request;
			string result = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if (string.IsNullOrEmpty(result)) { result = request.ServerVariables["REMOTE_ADDR"]; }
			if (string.IsNullOrEmpty(result)) { result = request.UserHostAddress; }
			if (string.IsNullOrEmpty(result)) { result = "0.0.0.0"; }
			return result;
		}

		/// <summary>
		/// 在执行操作方法后调用。
		/// </summary>
		/// <param name="filterContext"> 筛选器上下文。</param>
		protected virtual void OnActionExecuted(ActionExecutedContext filterContext) { }

		/// <summary>
		/// 在执行操作方法之前调用。
		/// </summary>
		/// <param name="filterContext">筛选器上下文。</param>
		protected virtual void OnActionExecuting(ActionExecutingContext filterContext) { }

		/// <summary>
		/// 在执行操作方法后调用。
		/// </summary>
		/// <param name="filterContext"> 筛选器上下文。</param>
		void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext) { OnActionExecuted(filterContext); }

		/// <summary>
		/// 在执行操作方法之前调用。
		/// </summary>
		/// <param name="filterContext">筛选器上下文。</param>
		void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext) { OnActionExecuting(filterContext); }
	}
}
