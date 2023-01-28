using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Basic.Authentication
{
	/// <summary>
	/// 表示一个特性，该特性用于限制调用方对操作方法的访问。
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class AuthorizationAttribute : System.Web.Mvc.AuthorizeAttribute
	{
		/// <summary>
		/// 在缓存模块请求授权时调用。
		/// </summary>
		/// <param name="httpContext"> HTTP 上下文，它封装有关单个 HTTP 请求的所有 HTTP 特定的信息。</param>
		/// <returns>对验证状态的引用。</returns>
		protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
		{
			return base.OnCacheAuthorization(httpContext);
		}

		/// <summary>
		/// 处理授权失败的 HTTP 请求。
		/// </summary>
		/// <param name="filterContext">封装用于 System.Web.Mvc.AuthorizeAttribute 的信息。
		/// filterContext 对象包括控制器、HTTP 上下文、请求上下文、操作结果和路由数据。</param>
		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			if (filterContext == null) { throw new ArgumentNullException("filterContext"); }
			filterContext.Result = new HttpUnauthorizedResult("登陆超时，请重新登陆");
			filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
			//base.HandleUnauthorizedRequest(filterContext);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="data"></param>
		/// <param name="validationStatus"></param>
		private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
		{
			validationStatus = this.OnCacheAuthorization(new HttpContextWrapper(context));
		}


		/// <summary>
		/// 在过程请求授权时调用。
		/// </summary>
		/// <param name="filterContext">筛选器上下文，它封装用于 System.Web.Mvc.AuthorizeAttribute 的信息。</param>
		/// <exception cref="System.ArgumentNullException">filterContext 参数为 null。</exception>
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null)
				throw new ArgumentNullException("filterContext");
			if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) &&
				!filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
			{
				if (this.AuthorizeCore(filterContext.HttpContext))
				{
					HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
					cache.SetProxyMaxAge(new TimeSpan(0));
					cache.AddValidationCallback(new HttpCacheValidateHandler(CacheValidateHandler), null);
				}
				else
				{
					this.HandleUnauthorizedRequest(filterContext);
				}
			}
		}

		/// <summary>
		/// 重写时，提供一个入口点用于进行自定义授权检查。
		/// </summary>
		/// <param name="httpContext">HTTP 上下文，它封装有关单个 HTTP 请求的所有 HTTP 特定的信息。</param>
		/// <exception cref="System.ArgumentNullException">filterContext 参数为 null。</exception>
		/// <returns> 如果用户已经过授权，则为 true；否则为 false。</returns>
		protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
		{
			return base.AuthorizeCore(httpContext);
		}
	}
}
