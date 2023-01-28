using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Basic.Configuration;
using Basic.Messages;

namespace Basic.MvcLibrary
{
	/// <summary></summary>
	public abstract class BasicContext : IBasicContext
	{
		/// <summary>表示</summary>
		protected internal const string CurrentLanguageCookieName = "D18CEB4419204B9296E9BEFB759848AF";

		private readonly CultureInfo mCulture;
		private readonly HttpContextBase _Context;
		/// <summary>初始化 Basic 类实例。</summary>
		protected BasicContext(object model, HttpContextBase context, CultureInfo culture, IPrincipal principal)
		{
			Model = model; mCulture = culture; User = principal; _Context = context;
		}

		/// <summary>获取视图上下文的文本输出流</summary>
		public abstract System.IO.TextWriter Writer { get; }

		/// <summary>获取或设置与视图数据关联的模型。</summary>
		/// <value>对数据模型的引用。</value>
		public virtual object Model { get; }

		/// <summary>获取或设置与视图数据关联的模型。</summary>
		/// <value>对数据模型的引用。</value>
		public abstract IDictionary<string, object> ViewData { get; }

		/// <summary></summary>
		public IPrincipal User { get; private set; }

		/// <summary></summary>
		public string HttpMethod { get { return _Context.Request.HttpMethod; } }

		/// <summary></summary>
		public NameValueCollection Form { get { return _Context.Request.Form; } }

		/// <summary></summary>
		public NameValueCollection Headers { get { return _Context.Request.Headers; } }

		/// <summary></summary>
		public string RequestContentType { get { return _Context.Request.ContentType; } set { _Context.Request.ContentType = value; } }

		/// <summary></summary>
		public NameValueCollection QueryString { get { return _Context.Request.QueryString; } }

		/// <summary></summary>
		public string Browser { get { return _Context.Request.Browser.Browser; } }

		/// <summary></summary>
		public string ContentType { get { return _Context.Response.ContentType; } set { _Context.Response.ContentType = value; } }

		/// <summary></summary>
		public Encoding ContentEncoding { get { return _Context.Response.ContentEncoding; } set { _Context.Response.ContentEncoding = value; } }

		/// <summary>清空多语言信息</summary>
		/// <param name="response">Http响应</param>
		public static void ClearCulture(HttpResponseBase response)
		{
			response.Cookies.Remove(CurrentLanguageCookieName);
		}

		/// <summary>
		/// 使用指定的操作名称、控制器名称和路由值生成操作方法的完全限定 URL。
		/// </summary>
		/// <param name="actionName">操作方法的名称。</param>
		/// <param name="controllerName">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public abstract string Action(string actionName, string controllerName, IDictionary<string, object> routeValues);

		/// <summary>
		/// 使用指定的操作名称、控制器名称和路由值生成操作方法的完全限定 URL。
		/// </summary>
		/// <param name="actionName">操作方法的名称。</param>
		/// <param name="controllerName">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public abstract string Action(string actionName, string controllerName, object routeValues);

		/// <summary>
		/// 使用指定的操作名称、控制器名称和路由值生成操作方法的完全限定 URL。
		/// </summary>
		/// <param name="actionName">操作方法的名称。</param>
		/// <param name="controllerName">控制器的名称。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public abstract string Action(string actionName, string controllerName);

		/// <summary>设置HttpResponse</summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public abstract void SetCookieValue(string name, string value);

		/// <summary>设置HttpResponse</summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="expires"></param>
		public abstract void SetCookieValue(string name, string value, DateTime expires);

		/// <summary>设置HttpResponse</summary>
		/// <param name="name"></param>
		public abstract string GetCookieValue(string name);

		/// <summary>
		/// 将指定的特性字符串转换为 HTML 编码的字符串。
		/// </summary>
		/// <param name="value">要编码的字符串。</param>
		/// <returns>HTML 编码的字符串。如果值参数为 null 或为空，则此方法返回空字符串。</returns>
		public string AttributeEncode(string value) { return System.Web.HttpUtility.HtmlAttributeEncode(value); }

		/// <summary>
		/// 将指定的特性对象转换为 HTML 编码的字符串。
		/// </summary>
		/// <param name="value">要编码的对象。</param>
		/// <returns>HTML 编码的字符串。如果值参数为 null 或为空，则此方法返回空字符串。</returns>
		public string AttributeEncode(object value) { return System.Web.HttpUtility.HtmlEncode(value); }

		/// <summary>获取客户端语言</summary>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		public System.Globalization.CultureInfo GetCultureInfo() { return mCulture; }

		/// <summary>返回指定的 System.String 资源的值。</summary>
		/// <param name="name">要获取的资源名。</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string name, params object[] args) { return MessageContext.GetString(name, mCulture, args); }

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string name) { return MessageContext.GetString(name, mCulture); }

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="converterName">消息转换器名称</param>
		/// <param name="name">要获取的资源名。</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string converterName, string name, params object[] args)
		{
			return MessageContext.GetString(converterName, name, mCulture, args);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="converterName">消息转换器名称</param>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string converterName, string name)
		{
			return MessageContext.GetString(converterName, name, mCulture);
		}

		/// <summary>将字符串写入文本流</summary>
		/// <param name="value">要写入的字符串</param>
		public abstract void Write(string value);

		/// <summary>通过在对象上调用 ToString 方法将此对象的文本表示形式写入文本流</summary>
		/// <param name="value">要写入的对象</param>
		public abstract void Write(object value);

		/// <summary>将行终止符写入文本流</summary>
		public abstract void WriteLine();

		/// <summary>将字符串写入文本流，后跟行终止符</summary>
		/// <param name="value">要写入的字符串。 如果 value 为 null，则只写入行终止符</param>
		public abstract void WriteLine(string value);

		/// <summary>通过在对象上调用 ToString 方法将此对象的文本表示形式写入文本流，后跟行终止符。</summary>
		/// <param name="value">要写入的对象。 如果 value 为 null，则只写入行终止符</param>
		public abstract void WriteLine(object value);

		void IBasicContext.ClearContent()
		{
			_Context.Response.ClearContent();
		}

		void IBasicContext.ClearHeaders()
		{
			_Context.Response.ClearHeaders();
		}

		Task IBasicContext.WriteAsync(byte[] bytes)
		{
			_Context.Response.BinaryWrite(bytes);
			return Task.CompletedTask;
		}

		void IBasicContext.AddHeader(string header, string value)
		{
			_Context.Response.AddHeader(header, value);
		}

		void IBasicContext.Flush()
		{
			_Context.Response.Flush();
		}

		void IBasicContext.End()
		{
			_Context.Response.End();
		}
	}

	/// <summary>
	/// 金软科技基础开发框架扩展
	/// </summary>
	public class BasicContext<TModel> : BasicContext, IBasicContext<TModel>
	{
		private readonly TModel _Model;
		private readonly ViewContext mViewContext;
		private readonly HttpResponseBase mResponse;
		private readonly HttpRequestBase mRequest;
		/// <summary>
		/// 初始化 Basic 类实例。
		/// </summary>
		/// <param name="model">与视图数据关联的模型。</param>
		/// <param name="context">有关与所定义路由匹配的 HTTP 请求的信息</param>
		public BasicContext(TModel model, ViewContext context) : base(model, context.HttpContext, GetCulture(context), context.HttpContext.User)
		{
			mViewContext = context; mResponse = context.HttpContext.Response; mRequest = context.HttpContext.Request;
			_Model = model;
		}

		/// <summary>获取或设置与视图数据关联的模型。</summary>
		/// <value>对数据模型的引用。</value>
		public override IDictionary<string, object> ViewData { get { return mViewContext.ViewData; } }

		/// <summary>获取或设置与视图数据关联的模型。</summary>
		/// <value>对数据模型的引用。</value>
		public new TModel Model { get { return _Model; } }

		private static CultureInfo GetCulture(ViewContext context)
		{
			HttpRequestBase request = context.HttpContext.Request;
			string csInfo = "zh-CN";
			if (request == null)
			{
				return CultureInfo.GetCultureInfo(csInfo);
			}
			if (request.Cookies[CurrentLanguageCookieName] != null)
			{
				string cultureString = request.Cookies[CurrentLanguageCookieName].Value;
				return CultureInfo.GetCultureInfo(cultureString);
			}
			if (request.UserLanguages != null && request.UserLanguages.Length > 0)
			{
				string cultureString1 = request.UserLanguages[0];
				csInfo = ConfigurationContext.Cultures.DefaultName;
				foreach (CultureElement cs in ConfigurationContext.Cultures.Cultures)
				{
					if (string.Compare(cultureString1, cs.Name, true) == 0) { csInfo = cs.Name; }
					else if (cs.Values.Contains(cultureString1)) { csInfo = cs.Name; break; }
				}
				HttpResponseBase response = context.HttpContext.Response;
				HttpCookie languageCookie = new HttpCookie(CurrentLanguageCookieName, csInfo)
				{
					HttpOnly = false,
					Expires = new System.DateTime(9999, 12, 31)
				};
				response.Cookies.Add(languageCookie);
			}
			return CultureInfo.GetCultureInfo(csInfo);

		}

		/// <summary>获取或设置与视图数据关联的模型。</summary>
		/// <value>对数据模型的引用。</value>
		public ViewContext ViewContext { get { return mViewContext; } }

		/// <summary>获取视图上下文的文本输出流</summary>
		public override System.IO.TextWriter Writer { get { return mViewContext.Writer; } }

		/// <summary>设置HttpResponse</summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="expires"></param>
		public override void SetCookieValue(string name, string value, DateTime expires)
		{
			HttpCookie cookie = mResponse.Cookies.Get(name);
			cookie.Value = value; cookie.Expires = expires;
		}

		/// <summary>设置HttpResponse</summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SetCookieValue(string name, string value)
		{
			HttpCookie cookie = mResponse.Cookies.Get(name);
			cookie.Value = value;
		}

		/// <summary>设置HttpResponse</summary>
		/// <param name="name"></param>
		public override string GetCookieValue(string name)
		{
			HttpCookie cookie = mRequest.Cookies.Get(name);
			if (cookie != null) { return cookie.Value; }
			return null;
		}

		/// <summary>
		/// 使用指定的操作名称、控制器名称和路由值生成操作方法的完全限定 URL。
		/// </summary>
		/// <param name="actionName">操作方法的名称。</param>
		/// <param name="controllerName">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public override string Action(string actionName, string controllerName, IDictionary<string, object> routeValues)
		{
			RouteValueDictionary values = routeValues != null ? new RouteValueDictionary(routeValues) : new RouteValueDictionary();
			return UrlHelper.GenerateUrl(null, actionName, controllerName, values, RouteTable.Routes, mViewContext.RequestContext, true);
		}

		/// <summary>
		/// 使用指定的操作名称、控制器名称和路由值生成操作方法的完全限定 URL。
		/// </summary>
		/// <param name="actionName">操作方法的名称。</param>
		/// <param name="controllerName">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public override string Action(string actionName, string controllerName, object routeValues)
		{
			RouteValueDictionary routes = TypeHelper.ObjectToDictionary(routeValues);
			return UrlHelper.GenerateUrl(null, actionName, controllerName, routes, RouteTable.Routes, mViewContext.RequestContext, true);
		}

		/// <summary>
		/// 使用指定的操作名称、控制器名称和路由值生成操作方法的完全限定 URL。
		/// </summary>
		/// <param name="actionName">操作方法的名称。</param>
		/// <param name="controllerName">控制器的名称。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		public override string Action(string actionName, string controllerName)
		{
			return UrlHelper.GenerateUrl(null, actionName, controllerName, null, RouteTable.Routes, mViewContext.RequestContext, true);
		}

		/// <summary>
		/// 根据 Lambda 表达式获取模型的值。
		/// </summary>
		/// <typeparam name="TP"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		public IHtmlString RawFor<TP>(Expression<Func<TModel, TP>> expression)
		{
			if (_Model == null) { return new HtmlString(""); }
			Func<TModel, TP> func = expression.Compile();
			TP value = GetValue(func);
			return new HtmlString(Convert.ToString(value));
		}

		/// <summary>
		/// 根据 Lambda 表达式获取模型的值。
		/// </summary>
		/// <typeparam name="TP"></typeparam>
		/// <param name="func"></param>
		/// <returns></returns>
		public TP GetValue<TP>(Func<TModel, TP> func)
		{
			if (_Model == null) { return default; }
			return func(_Model);
		}

		/// <summary>
		/// 根据 Lambda 表达式获取模型的值。
		/// </summary>
		/// <typeparam name="TP"></typeparam>
		/// <param name="func"></param>
		/// <returns></returns>
		public object GetObject<TP>(Func<TModel, TP> func)
		{
			if (_Model == null) { return null; }
			return func(_Model);
		}

		/// <summary>将字符串写入文本流</summary>
		/// <param name="value">要写入的字符串</param>
		public override void Write(string value) { mViewContext.Writer.Write(value); }

		/// <summary>通过在对象上调用 ToString 方法将此对象的文本表示形式写入文本流</summary>
		/// <param name="value">要写入的对象</param>
		public override void Write(object value) { mViewContext.Writer.Write(value); }

		/// <summary>将行终止符写入文本流</summary>
		public override void WriteLine() { mViewContext.Writer.WriteLine(); }

		/// <summary>将字符串写入文本流，后跟行终止符</summary>
		/// <param name="value">要写入的字符串。 如果 value 为 null，则只写入行终止符</param>
		public override void WriteLine(string value) { mViewContext.Writer.WriteLine(value); }

		/// <summary>通过在对象上调用 ToString 方法将此对象的文本表示形式写入文本流，后跟行终止符。</summary>
		/// <param name="value">要写入的对象。 如果 value 为 null，则只写入行终止符</param>
		public override void WriteLine(object value) { mViewContext.Writer.WriteLine(value); }
	}
}
