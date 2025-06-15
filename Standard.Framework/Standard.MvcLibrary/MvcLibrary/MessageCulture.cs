using System;
using System.Linq.Expressions;
using Basic.Configuration;
using Basic.EntityLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Basic.Messages
{
	/// <summary>
	/// 当前登录用户使用的而语言名称，如果存在则读取，不存在则从客户端使用语言中判断。
	/// </summary>
	public static class MessageCulture
	{
		private const string CurrentLanguageCookieName = "D18CEB4419204B9296E9BEFB759848AF";
		private static System.Globalization.CultureInfo _CultureInfo = System.Globalization.CultureInfo.CurrentUICulture;
		/// <summary>
		/// 获取当前系统使用的区域特性信息。
		/// </summary>
		public static System.Globalization.CultureInfo CultureInfo { get { return _CultureInfo; } }

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
		public static string GetCultureString(string converterName, string name, params object[] args)
		{
			if (string.IsNullOrEmpty(name)) { return null; }
			return MessageContext.GetString(converterName, name, _CultureInfo, args);
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
		public static string GetCultureString(string converterName, string name)
		{
			if (string.IsNullOrEmpty(name)) { return null; }
			return MessageContext.GetString(converterName, name, _CultureInfo);
		}

		/// <summary>
		/// 当前登录用户使用的语言名称，如果存在则读取，不存在则从客户端使用的语言中判断。
		/// </summary>
		/// <param name="request">当前客户端请求</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		private static System.Globalization.CultureInfo GetCultureInfo(this HttpRequest request)
		{
			string csInfo = "zh-CN";
			if (request == null)
			{
				return System.Globalization.CultureInfo.GetCultureInfo(csInfo);
			}
			if (request.Cookies.TryGetValue(CurrentLanguageCookieName, out string cultureString))
			{
				return System.Globalization.CultureInfo.GetCultureInfo(cultureString);
			}
			//StringValues valus = request.Headers[HeaderNames.AcceptLanguage];
			//var languages = valus.ToArray();
			//if (languages != null && languages.Length > 0)
			//{
			//	string cultureString1 = languages[0];
			//	csInfo = EventLogElementContext.Cultures.DefaultName;
			//	foreach (CultureElement cs in EventLogElementContext.Cultures.Cultures)
			//	{
			//		if (string.Compare(cultureString1, cs.Name, true) == 0) { csInfo = cs.Name; }
			//		else if (cs.Values.Contains(cultureString1)) { csInfo = cs.Name; break; }
			//	}
			//}
			return System.Globalization.CultureInfo.GetCultureInfo(csInfo);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="request">当前客户端请求。</param>
		/// <param name="value">要获取的资源枚举项。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public static string GetEnumText<T>(this HttpRequest request, T value) where T : struct
		{
			if (!typeof(T).IsEnum) { throw new System.ArgumentException("类型参数异常，此方法仅支持获取枚举资源。"); }
			System.Globalization.CultureInfo culture = request.GetCultureInfo();
			Type enumType = typeof(T);
			string enumName = enumType.Name, converterName = "";
			if (Attribute.IsDefined(enumType, typeof(WebDisplayConverterAttribute)))
			{
				WebDisplayConverterAttribute wdca = (WebDisplayConverterAttribute)Attribute.GetCustomAttribute(enumType, typeof(WebDisplayConverterAttribute));
				converterName = wdca.ConverterName;
			}
			string name = Enum.GetName(enumType, value);
			string itemName = string.Concat(enumName, "_", name);
			string text = MessageContext.GetString(converterName, itemName, culture);
			if (text == itemName) { return name; }
			return text;
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="request">当前客户端请求。</param>
		/// <param name="name">要获取的资源名。</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public static string GetString(this HttpRequest request, string name, params object[] args)
		{
			if (string.IsNullOrEmpty(name))
				return null;
			System.Globalization.CultureInfo culture = request.GetCultureInfo();
			return MessageContext.GetString(name, culture, args);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="request">当前客户端请求。</param>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public static string GetString(this HttpRequest request, string name)
		{
			if (string.IsNullOrEmpty(name))
				return null;
			System.Globalization.CultureInfo culture = request.GetCultureInfo();
			return MessageContext.GetString(name, culture);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="request">当前客户端请求。</param>
		/// <param name="converterName">消息转换器名称</param>
		/// <param name="name">要获取的资源名。</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public static string GetString(this HttpRequest request, string converterName, string name, params object[] args)
		{
			if (string.IsNullOrEmpty(name))
				return null;
			System.Globalization.CultureInfo culture = request.GetCultureInfo();
			return MessageContext.GetString(converterName, name, culture, args);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="request">当前客户端请求。</param>
		/// <param name="converterName">消息转换器名称</param>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public static string GetString(this HttpRequest request, string converterName, string name)
		{
			if (string.IsNullOrEmpty(name))
				return null;
			System.Globalization.CultureInfo culture = request.GetCultureInfo();
			return MessageContext.GetString(converterName, name, culture);
		}

		/// <summary>
		/// 获取实体模型属性显示文本
		/// </summary>
		/// <typeparam name="TM">模型类型</typeparam>
		/// <param name="request">当前客户端请求。</param>
		/// <param name="expression">模型属性的 Lambda 表达式。</param>
		/// <returns>返回执行模型属性的本地化资源。</returns>
		public static string GetPropertyText<TM>(this HttpRequest request, Expression<Func<TM, object>> expression)
		{
			EntityPropertyMeta propertyDescriptor = LambdaHelper.GetProperty(expression);
			System.Globalization.CultureInfo info = MessageCulture.GetCultureInfo(request);
			return propertyDescriptor.GetCultureDisplayName(info);
		}

		/// <summary>
		/// 当前登录用户使用的语言名称，如果存在则读取，不存在则从客户端使用的语言中判断。
		/// </summary>
		/// <param name="response">当前客户端请求</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		public static void ClearCultureInfo(this HttpResponse response)
		{
			response.Cookies.Delete(CurrentLanguageCookieName);
		}

		/// <summary>
		/// 当前登录用户使用的语言名称，如果存在则读取，不存在则从客户端使用的语言中判断。
		/// </summary>
		/// <param name="response">当前客户端请求</param>
		/// <param name="culture">
		/// System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。请注意，如果尚未为此区域性本地化该资源，则查找将使用当前线程的
		/// System.Globalization.CultureInfo.Parent 属性回退，并在非特定语言区域性中查找后停止。如果此值为 null，则使用当前线程的
		/// System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。
		/// </param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		public static void SetCultureInfo(this HttpResponse response, System.Globalization.CultureInfo culture)
		{
			if (response == null) { _CultureInfo = culture; return; }
			response.Cookies.Append(CurrentLanguageCookieName, culture.Name, new CookieOptions()
			{
				HttpOnly = false,
				Expires = new DateTime(9999, 12, 31)
			});
		}

		/// <summary>
		/// 当前登录用户使用的语言名称，如果存在则读取，不存在则从客户端使用的语言中判断。
		/// </summary>
		/// <param name="culture">
		/// System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。请注意，如果尚未为此区域性本地化该资源，则查找将使用当前线程的
		/// System.Globalization.CultureInfo.Parent 属性回退，并在非特定语言区域性中查找后停止。如果此值为 null，则使用当前线程的
		/// System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。
		/// </param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		public static void SetCultureInfo(System.Globalization.CultureInfo culture)
		{
			_CultureInfo = culture;
		}
	}
}
