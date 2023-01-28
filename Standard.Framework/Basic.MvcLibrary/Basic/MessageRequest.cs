using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Basic.Messages;

namespace Basic.MvcLibrary
{
	/// <summary>表示资源请求</summary>
	public sealed class MessageRequest : IMessageRequest
	{
		private readonly CultureInfo culture;
		/// <summary></summary>
		public MessageRequest(IDictionary<string, object> route) { culture = GetCultureInfo(route); }

		/// <summary></summary>
		public MessageRequest(HttpRequestBase request) { culture = GetCultureInfo(request); }

		private CultureInfo GetCultureInfo(IDictionary<string, object> route)
		{
			string csInfo = "zh-CN";
			if (route == null) { return CultureInfo.GetCultureInfo(csInfo); }
			if (route.TryGetValue("Culture", out object value))
			{
				return CultureInfo.GetCultureInfo((string)value);
			}
			return CultureInfo.GetCultureInfo(csInfo);
		}

		private CultureInfo GetCultureInfo(HttpRequestBase request)
		{
			string csInfo = "zh-CN";
			if (request == null) { return CultureInfo.GetCultureInfo(csInfo); }
			if (request.Cookies[BasicContext.CurrentLanguageCookieName] != null)
			{
				string cultureString = request.Cookies[BasicContext.CurrentLanguageCookieName].Value;
				return CultureInfo.GetCultureInfo(cultureString);
			}
			return CultureInfo.GetCultureInfo(csInfo);
		}

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string name) { return MessageContext.GetString(name, culture); }

		/// <summary>返回指定的 System.String 资源的值。</summary>
		/// <param name="name">要获取的资源名。</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string name, params object[] args) { return MessageContext.GetString(name, culture, args); }

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="converterName">消息转换器名称</param>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string converterName, string name) { return MessageContext.GetString(converterName, name, culture); }

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
		public string GetString(string converterName, string name, params object[] args) { return MessageContext.GetString(converterName, name, culture, args); }
	}
}
