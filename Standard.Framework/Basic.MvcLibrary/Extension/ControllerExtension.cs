using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Basic.Collections;
using Basic.Configuration;
using Basic.EntityLayer;
using Basic.Interfaces;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 控制器类扩展实例
	/// </summary>
	public static partial class ControllerExtension
	{
		private const string CurrentLanguageCookieName = "D18CEB4419204B9296E9BEFB759848AF";

		/// <summary>
		/// 当前登录用户使用的语言名称，如果存在则读取，不存在则从客户端使用的语言中判断。
		/// </summary>
		/// <param name="controller">当前客户端请求</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		public static System.Globalization.CultureInfo GetCultureInfo(this ControllerBase controller)
		{
			HttpContextBase httpContext = controller.ControllerContext.HttpContext;
			string csInfo = "zh-CN";
			if (httpContext.Request == null)
			{
				return System.Globalization.CultureInfo.GetCultureInfo(csInfo);
			}
			if (httpContext.Request.Cookies[CurrentLanguageCookieName] != null)
			{
				string cultureString = httpContext.Request.Cookies[CurrentLanguageCookieName].Value;
				return System.Globalization.CultureInfo.GetCultureInfo(cultureString);
			}
			if (httpContext.Request.UserLanguages != null && httpContext.Request.UserLanguages.Length > 0)
			{
				string cultureString1 = httpContext.Request.UserLanguages[0];
				foreach (CultureElement cs in ConfigurationContext.Cultures.Cultures)
				{
					if (string.Compare(cultureString1, cs.Name, true) == 0) { csInfo = cs.Name; }
					else
					{
						foreach (CultureElement ce in cs.Values)
						{
							if (string.Compare(cultureString1, ce.Name, true) == 0)
							{
								csInfo = cs.Name;
								break;
							}
						}
					}
					if (csInfo == cs.Name) { break; }
				}
				HttpCookie languageCookie = new HttpCookie(CurrentLanguageCookieName, csInfo);
				languageCookie.HttpOnly = false;
				languageCookie.Expires = new System.DateTime(9999, 12, 31);
				httpContext.Request.Cookies.Add(languageCookie);
			}
			return System.Globalization.CultureInfo.GetCultureInfo(csInfo);
		}

		/// <summary>
		/// 当前登录用户使用的语言名称，如果存在则读取，不存在则从客户端使用的语言中判断。
		/// </summary>
		/// <param name="controller">当前客户端请求</param>
		/// <param name="culture">
		/// System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。请注意，如果尚未为此区域性本地化该资源，则查找将使用当前线程的
		/// System.Globalization.CultureInfo.Parent 属性回退，并在非特定语言区域性中查找后停止。如果此值为 null，则使用当前线程的
		/// System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。
		/// </param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		public static void SetCultureInfo(this ControllerBase controller, System.Globalization.CultureInfo culture)
		{
			HttpContextBase httpContext = controller.ControllerContext.HttpContext;
			HttpCookie languageCookie = new HttpCookie(CurrentLanguageCookieName, culture.Name);
			languageCookie.HttpOnly = false;
			languageCookie.Expires = new System.DateTime(9999, 12, 31);
			httpContext.Request.Cookies.Add(languageCookie);
		}

		/// <summary>
		/// 返回执行成功的PartialViewResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		///<param name="viewName">返回的视图名称</param>
		/// <returns>返回PartialViewResult类实例</returns>
		public static ViewResult JsonView<T>(this ControllerBase controller, string viewName) where T : class
		{
			return new ViewResult { ViewName = viewName, ViewData = controller.ViewData, TempData = controller.TempData };
		}

		/// <summary>
		/// 返回执行成功的PartialViewResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		///<param name="viewName">返回的视图名称</param>
		/// <param name="model">生成视图需要的模型信息</param>
		/// <returns>返回PartialViewResult类实例</returns>
		public static ViewResult JsonView<T>(this ControllerBase controller, string viewName, IPagination<T> model) where T : class
		{
			if (model != null) { controller.ViewData.Model = model; }

			return new ViewResult { ViewName = viewName, ViewData = controller.ViewData, TempData = controller.TempData };
		}

		/// <summary>
		/// 返回执行成功的PartialViewResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="viewName">返回的视图名称</param>
		/// <param name="model">生成视图需要的模型信息</param>
		/// <returns>返回PartialViewResult类实例</returns>
		public static ViewResult JsonView<T>(this ControllerBase controller, string viewName, T model) where T : class
		{
			IPagination<T> customes = new Pagination<T>(new T[] { model });
			return controller.JsonView<T>(viewName, customes);
		}

		#region 返回客户端Json数据
		/// <summary>
		/// 返回执行成功的JsonMvcResult
		/// </summary>
		/// <param name="controller">当前需要扩展的控制器实例。</param>
		/// <param name="success">当前Action是否执行成功</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, bool success)
		{
			return new JsonMvcResult(success);
		}

		/// <summary>
		/// 返回执行成功的JsonMvcResult
		/// </summary>
		/// <param name="controller">当前需要扩展的控制器实例。</param>
		/// <param name="success">当前Action是否执行成功</param>
		/// <param name="msg">操作执行成功，返回客户端的消息</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, bool success, string msg)
		{
			return new JsonMvcResult(success, msg);
		}

		/// <summary>
		/// 返回执行成功的JsonMvcResult
		/// </summary>
		/// <param name="controller">当前需要扩展的控制器实例。</param>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity entity) { return new JsonMvcResult(entity); }

		/// <summary>
		/// 返回执行成功的JsonMvcResult
		/// </summary>
		/// <param name="controller">当前需要扩展的控制器实例。</param>
		/// <param name="entity">当前Action执行的模型实体类信息。</param>
		/// <param name="sendClientData">
		/// 当前 ActionResult 是否需要向客户端发送实体数据，默认值 true 。
		/// 如果模型验证结果有错误，错误数据始终发送到客户端。
		/// </param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity entity, bool sendClientData)
		{
			return new JsonMvcResult(entity, sendClientData);
		}

		/// <summary>
		/// 返回执行成功的JsonMvcResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <param name="contentType">内容的类型</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity entity, string contentType)
		{
			return new JsonMvcResult(entity) { ContentType = contentType };
		}

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="modelState">当前Action执行的模型错误信息</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, ModelStateDictionary modelState)
		{
			return new JsonMvcResult(modelState);
		}

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="modelState">当前Action执行的模型错误信息</param>
		/// <param name="contentType">内容的类型</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, ModelStateDictionary modelState, string contentType)
		{
			//ErrorResult errors = controller.CreateErrorResult(modelState);
			return new JsonMvcResult(modelState) { ContentType = contentType };
		}

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="dbResult">当前Action执行的数据库方法的错误信息</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, Result dbResult)
		{
			return new JsonMvcResult(dbResult);
		}

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="dbResult">当前Action执行的数据库方法的错误信息</param>
		/// <param name="contentType">内容的类型</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, Result dbResult, string contentType)
		{
			return new JsonMvcResult(dbResult) { ContentType = contentType };
		}

		/// <summary>
		/// 返回执行成功的JsonMvcResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity[] entities)
		{
			return new JsonMvcResult(entities);
		}


		/// <summary>
		/// 返回执行成功的JsonMvcResult
		/// </summary>
		/// <param name="controller">当前需要扩展的控制器实例。</param>
		/// <param name="entities">当前Action执行的模型实体类信息。</param>
		/// <param name="sendClientData">
		/// 当前 ActionResult 是否需要向客户端发送实体数据，默认值 true 。
		/// 如果模型验证结果有错误，错误数据始终发送到客户端。
		/// </param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity[] entities, bool sendClientData)
		{
			return new JsonMvcResult(entities, sendClientData);
		}

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <param name="dbResult">当前Action执行的数据库方法的错误信息</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity[] entities, Result dbResult)
		{
			return new JsonMvcResult(entities, dbResult);
		}

		/// <summary>
		/// 返回执行成功的JsonMvcResult
		/// </summary>
		/// <param name="controller">当前需要扩展的控制器实例。</param>
		/// <param name="entities">当前Action执行的模型实体类信息。</param>
		/// <param name="dbResult">当前Action执行的数据库方法的错误信息</param>
		/// <param name="sendClientData">
		/// 当前 ActionResult 是否需要向客户端发送实体数据，默认值 true 。
		/// 如果模型验证结果有错误，错误数据始终发送到客户端。
		/// </param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity[] entities, Result dbResult, bool sendClientData)
		{
			return new JsonMvcResult(entities, dbResult, sendClientData);
		}

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entities">当前Action执行的模型实体类信息</param>
		/// <param name="modelState">当前Action执行的模型错误信息</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity[] entities, ModelStateDictionary modelState)
		{
			return new JsonMvcResult(entities, modelState);
		}

		/// <summary>
		/// 返回执行成功的JsonMvcResult
		/// </summary>
		/// <param name="controller">当前需要扩展的控制器实例。</param>
		/// <param name="entities">当前Action执行的模型实体类信息。</param>
		/// <param name="modelState">当前Action执行的模型错误信息</param>
		/// <param name="sendClientData">
		/// 当前 ActionResult 是否需要向客户端发送实体数据，默认值 true 。
		/// 如果模型验证结果有错误，错误数据始终发送到客户端。
		/// </param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity[] entities, ModelStateDictionary modelState, bool sendClientData)
		{
			return new JsonMvcResult(entities, modelState, sendClientData);
		}

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <param name="modelState">当前Action执行的模型错误信息</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity entity, ModelStateDictionary modelState)
		{
			return new JsonMvcResult(entity, modelState);
		}

		/// <summary>
		/// 返回执行成功的JsonMvcResult
		/// </summary>
		/// <param name="controller">当前需要扩展的控制器实例。</param>
		/// <param name="entity">当前Action执行的模型实体类信息。</param>
		/// <param name="dbResult">当前Action执行的数据库方法的错误信息</param>
		/// <param name="sendClientData">
		/// 当前 ActionResult 是否需要向客户端发送实体数据，默认值 true 。
		/// 如果模型验证结果有错误，错误数据始终发送到客户端。
		/// </param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity entity, Result dbResult, bool sendClientData)
		{
			return new JsonMvcResult(entity, dbResult, sendClientData);
		}

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <param name="modelState">当前Action执行的模型错误信息</param>
		/// <param name="sendClientData">
		/// 当前 ActionResult 是否需要向客户端发送实体数据，默认值 true 。
		/// 如果模型验证结果有错误，错误数据始终发送到客户端。
		/// </param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity entity, ModelStateDictionary modelState, bool sendClientData)
		{
			return new JsonMvcResult(entity, modelState, sendClientData);
		}

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <param name="modelState">当前Action执行的模型错误信息</param>
		/// <param name="contentType">内容的类型</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity entity, ModelStateDictionary modelState, string contentType)
		{
			//ErrorResult errors = controller.CreateErrorResult(modelState);
			return new JsonMvcResult(entity, modelState) { ContentType = contentType };
		}

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <param name="dbResult">当前Action执行的数据库方法的错误信息</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity entity, Result dbResult)
		{
			//ErrorResult errors = controller.CreateErrorResult(dbResult);
			return new JsonMvcResult(entity, dbResult);
		}

		/// <summary>
		/// 返回执行失败的JsonMvcResult
		/// </summary>
		/// <param name="controller">需要扩展的Controller类实例</param>
		/// <param name="entity">当前Action执行的模型实体类信息</param>
		/// <param name="dbResult">当前Action执行的数据库方法的错误信息</param>
		/// <param name="contentType">内容的类型</param>
		/// <returns>返回JsonMvcResult类实例</returns>
		public static JsonMvcResult JsonMvc(this ControllerBase controller, AbstractEntity entity, Result dbResult, string contentType)
		{
			//ErrorResult errors = controller.CreateErrorResult(dbResult);
			return new JsonMvcResult(entity, dbResult) { ContentType = contentType };
		}
		#endregion

		#region ModelStateDictionary.AddModelError 扩展
		/// <summary>
		/// 将指定的错误消息添加到与指定键关联的模型状态字典的错误集合中。
		/// </summary>
		/// <param name="controller">需要扩展的 ControllerBase 子类实例。</param>
		/// <param name="expression">表示需要添加当前模型属性的Lambda表达式，此表达式表示成员属性。</param>
		/// <param name="sourceName">文本消息资源健。</param>
		public static void AddModelError<T>(this ControllerBase controller, Expression<Func<T, object>> expression, string sourceName) where T : class
		{
			EntityPropertyMeta property = LambdaHelper.GetProperty(expression);
			string converterName = property.Display != null ? property.Display.ConverterName : null;
			controller.AddModelError(property.Name, converterName, sourceName);
		}

		/// <summary>
		/// 将指定的错误消息添加到与指定键关联的模型状态字典的错误集合中。
		/// </summary>
		/// <param name="controller">需要扩展的 ControllerBase 子类实例。</param>
		/// <param name="expression">表示需要添加当前模型属性的Lambda表达式，此表达式表示成员属性。</param>
		/// <param name="ex">异常。</param>
		public static void AddModelError<T>(this ControllerBase controller, Expression<Func<T, object>> expression, Exception ex) where T : class
		{
			MemberExpression member = LambdaHelper.GetMember(expression);
			controller.AddModelError(member.Member.Name, ex);
		}

		/// <summary>
		/// 将指定的错误消息添加到与指定键关联的模型状态字典的错误集合中。
		/// </summary>
		/// <param name="controller">需要扩展的 ControllerBase 子类实例。</param>
		/// <param name="key">键。</param>
		/// <param name="converterName">消息转换器名称</param>
		/// <param name="sourceName">文本消息资源健。</param>
		public static void AddModelError(this ControllerBase controller, string key, string converterName, string sourceName)
		{
			if (string.IsNullOrWhiteSpace(sourceName)) { return; }
			string message = controller.GetString(converterName, sourceName);
			controller.ViewData.ModelState.AddModelError(key, message);
		}

		/// <summary>
		/// 将指定的错误消息添加到与指定键关联的模型状态字典的错误集合中。
		/// </summary>
		/// <param name="controller">需要扩展的 ControllerBase 子类实例。</param>
		/// <param name="key">键。</param>
		/// <param name="errorMessage">错误消息。</param>
		public static void AddModelError(this ControllerBase controller, string key, string errorMessage)
		{
			if (string.IsNullOrWhiteSpace(errorMessage)) { return; }
			if (Regex.IsMatch(errorMessage, "^[A-Za-z0-9_]+$"))
				errorMessage = controller.GetString(errorMessage);
			controller.ViewData.ModelState.AddModelError(key, errorMessage);
		}

		/// <summary>
		/// 将指定的错误消息添加到与指定键关联的模型状态字典的错误集合中。
		/// </summary>
		/// <param name="controller">需要扩展的 ControllerBase 子类实例。</param>
		/// <param name="errorMessage">错误消息。</param>
		public static void AddModelError(this ControllerBase controller, string errorMessage)
		{
			if (string.IsNullOrWhiteSpace(errorMessage)) { return; }
			errorMessage = errorMessage.Trim();
			if (Regex.IsMatch(errorMessage, "^[A-Za-z0-9_]+$"))
				errorMessage = controller.GetString(errorMessage);
			controller.ViewData.ModelState.AddModelError(string.Empty, errorMessage);
		}

		/// <summary>
		/// 将指定的错误消息添加到与指定键关联的模型状态字典的错误集合中。
		/// </summary>
		/// <param name="controller">需要扩展的 ControllerBase 子类实例。</param>
		/// <param name="key">键。</param>
		/// <param name="ex">异常。</param>
		public static void AddModelError(this ControllerBase controller, string key, Exception ex)
		{
			controller.ViewData.ModelState.AddModelError(key, ex);
		}

		/// <summary>
		/// 将指定的错误消息添加到与指定键关联的模型状态字典的错误集合中。
		/// </summary>
		/// <param name="controller">需要扩展的 ControllerBase 子类实例。</param>
		/// <param name="ex">异常。</param>
		public static void AddModelError(this ControllerBase controller, Exception ex)
		{
			controller.ViewData.ModelState.AddModelError(string.Empty, ex);
		}
		#endregion
	}
}
