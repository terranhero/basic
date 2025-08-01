using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 表示 OnActionExecuted 执行的日志记录。
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public abstract class ActionLogAttribute : ActionFilterAttribute, IActionFilter
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
		protected async Task<string> GetMessageAsync(ActionExecutingContext context)
		{
			if (_Arguments == null || _Arguments.Length == 0) { return _Format; }
			List<object> list = new List<object>(_Arguments.Length);
			Controller controller = (Controller)context.Controller;
			IList<IValueProviderFactory> factories = controller.ControllerContext.ValueProviderFactories;
			ValueProviderFactoriesContext valueProviders = new ValueProviderFactoriesContext(context);
			foreach (IValueProviderFactory factory in factories)
			{
				await factory.CreateValueProviderAsync(valueProviders);
			}
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
					if (result != ValueProviderResult.None) { list.Add(result.FirstValue); break; }
				}
			}
			return string.Format(_Format, list.ToArray());
		}

		/// <summary>
		/// 获取 HttpRequest中请求的地址。
		/// </summary>
		/// <param name="context">当前控制器上下文信息。</param>
		/// <returns>返回地址信息。</returns>
		protected string GetRequestAddress(FilterContext context)
		{
			HttpRequest request = context.HttpContext.Request;
			string result = request.Headers["HTTP_X_FORWARDED_FOR"];
			if (string.IsNullOrEmpty(result)) { result = request.Headers["REMOTE_ADDR"]; }
			if (string.IsNullOrEmpty(result)) { result = request.Host.Host; }
			if (string.IsNullOrEmpty(result)) { result = "0.0.0.0"; }
			return result;
		}

		private sealed class ValueProviderFactoriesContext : ValueProviderFactoryContext
		{
			/// <summary>Creates a new Microsoft.AspNetCore.Mvc.ModelBinding.ValueProviderFactoryContext.</summary>
			/// <param name="context">The Microsoft.AspNetCore.Mvc.ModelBinding.ValueProviderFactoryContext.ActionContext.</param>
			public ValueProviderFactoriesContext(ActionContext context) : base(context) { }

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
