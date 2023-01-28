using Basic.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Basic.MvcLibrary
{
	/// <summary>表示基础扩展</summary>
	public interface IBasicContext
	{
		/// <summary>获取视图上下文的文本输出流</summary>
		System.IO.TextWriter Writer { get; }

		/// <summary>获取视图上下文的文本输出流</summary>
		ICustomResponse Response { get; }

		/// <summary>获取或设置与视图数据关联的模型。</summary>
		/// <value>对数据模型的引用。</value>
		object Model { get; }

		/// <summary>获取或设置与视图数据关联的模型。</summary>
		/// <value>对数据模型的引用。</value>
		IDictionary<string, object> ViewData { get; }

		/// <summary></summary>
		System.Security.Principal.IPrincipal User { get; }

		/// <summary></summary>
		string HttpMethod { get; }

		/// <summary></summary>
		NameValueCollection Form { get; }

		/// <summary></summary>
		NameValueCollection Headers { get; }

		/// <summary></summary>
		string ContentType { get; set; }

		/// <summary></summary>
		NameValueCollection QueryString { get; }

		/// <summary></summary>
		string Browser { get; }

		/// <summary></summary>
		void ClearContent();

		/// <summary></summary>
		void ClearHeaders();

		/// <summary></summary>
		Task WriteAsync(byte[] bytes);

		/// <summary></summary>
		void AddHeader(string header, string value);

		/// <summary></summary>
		System.Text.Encoding ContentEncoding { get; set; }

		/// <summary></summary>
		string RequestContentType { get; set; }

		/// <summary></summary>
		Task FlushAsync();

		/// <summary></summary>
		void Flush();

		/// <summary>获取客户端语言</summary>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		System.Globalization.CultureInfo GetCultureInfo();

		/// <summary>返回指定的 System.String 资源的值。</summary>
		/// <param name="name">要获取的资源名。</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		string GetString(string name, params object[] args);

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		string GetString(string name);

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
		string GetString(string converterName, string name, params object[] args);

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="converterName">消息转换器名称</param>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		string GetString(string converterName, string name);

		/// <summary>设置HttpResponse</summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		void SetCookieValue(string name, string value);

		/// <summary>设置HttpResponse</summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="expires"></param>
		void SetCookieValue(string name, string value, DateTime expires);

		/// <summary>设置HttpResponse</summary>
		/// <param name="name"></param>
		string GetCookieValue(string name);

		/// <summary>
		/// 使用指定的操作名称、控制器名称和路由值生成操作方法的完全限定 URL。
		/// </summary>
		/// <param name="actionName">操作方法的名称。</param>
		/// <param name="controllerName">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		string Action(string actionName, string controllerName, IDictionary<string, object> routeValues);

		/// <summary>
		/// 使用指定的操作名称、控制器名称和路由值生成操作方法的完全限定 URL。
		/// </summary>
		/// <param name="actionName">操作方法的名称。</param>
		/// <param name="controllerName">控制器的名称。</param>
		/// <param name="routeValues">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		string Action(string actionName, string controllerName, object routeValues);

		/// <summary>
		/// 使用指定的操作名称、控制器名称和路由值生成操作方法的完全限定 URL。
		/// </summary>
		/// <param name="actionName">操作方法的名称。</param>
		/// <param name="controllerName">控制器的名称。</param>
		/// <returns>操作方法的完全限定 URL。</returns>
		string Action(string actionName, string controllerName);

		/// <summary>将字符串写入文本流</summary>
		/// <param name="value">要写入的字符串</param>
		void Write(string value);

		/// <summary>通过在对象上调用 ToString 方法将此对象的文本表示形式写入文本流</summary>
		/// <param name="value">要写入的对象</param>
		void Write(object value);

		/// <summary>将行终止符写入文本流</summary>
		void WriteLine();

		/// <summary>将字符串写入文本流，后跟行终止符</summary>
		/// <param name="value">要写入的字符串。 如果 value 为 null，则只写入行终止符</param>
		void WriteLine(string value);

		/// <summary>通过在对象上调用 ToString 方法将此对象的文本表示形式写入文本流，后跟行终止符。</summary>
		/// <param name="value">要写入的对象。 如果 value 为 null，则只写入行终止符</param>
		void WriteLine(object value);
	}

	/// <summary>表示基础扩展</summary>
	/// <typeparam name="TModel"></typeparam>
	public interface IBasicContext<TModel> : IBasicContext
	{
		/// <summary>获取或设置与视图数据关联的模型。</summary>
		/// <value>对数据模型的引用。</value>
		new TModel Model { get; }

		/// <summary>
		/// 根据 Lambda 表达式获取模型的值。
		/// </summary>
		/// <typeparam name="TP"></typeparam>
		/// <param name="func"></param>
		/// <returns></returns>
		TP GetValue<TP>(Func<TModel, TP> func);

		/// <summary>
		/// 根据 Lambda 表达式获取模型的值。
		/// </summary>
		/// <typeparam name="TP"></typeparam>
		/// <param name="func"></param>
		/// <returns></returns>
		object GetObject<TP>(Func<TModel, TP> func);
	}
}
