using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Basic.Messages;

namespace Basic.MvcLibrary
{
	/// <summary>表示资源请求</summary>
	public sealed class MessageRequest : IMessageRequest
	{
		private static IMessageRequest UpdateValue(string key, IMessageRequest value) { return value; }

		private static readonly ConcurrentDictionary<string, IMessageRequest> _messages = new ConcurrentDictionary<string, IMessageRequest>();

		/// <summary>获取当前区域特性的消息读取器</summary>
		/// <param name="ci">表示当前显示区域特性</param>
		/// <returns>表示 IMessageRequest 实例，一个区域读取请求，返回结果永远不为 null。</returns>
		public static IMessageRequest GetRequest(CultureInfo ci)
		{
			if (_messages.TryGetValue(ci.Name, out IMessageRequest request)) { return request; }
			request = new MessageRequest(ci);
			return _messages.AddOrUpdate(ci.Name, request, UpdateValue);
		}

		/// <summary>获取缓存的日志写入器</summary>
		/// <param name="ci">数据库连接名称</param>
		/// <param name="request">日志写入器</param>
		/// <returns>true if the key was found in the <see cref="ConcurrentDictionary{TKey,TValue}"/>; otherwise, false</returns>
		private static IMessageRequest AddOrUpdate(CultureInfo ci, IMessageRequest request)
		{
			return _messages.AddOrUpdate(ci.Name, request, UpdateValue);
		}

		private readonly CultureInfo _culture;
		/// <summary></summary>
		private MessageRequest(CultureInfo ci) { _culture = ci; }

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string name) { return MessageContext.GetString(name, _culture); }

		/// <summary>返回指定的 System.String 资源的值。</summary>
		/// <param name="name">要获取的资源名。</param>
		/// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string name, params object[] args) { return MessageContext.GetString(name, _culture, args); }

		/// <summary>
		/// 返回指定的 System.String 资源的值。
		/// </summary>
		/// <param name="converterName">消息转换器名称</param>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
		/// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
		/// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
		/// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
		public string GetString(string converterName, string name) { return MessageContext.GetString(converterName, name, _culture); }

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
			return MessageContext.GetString(converterName, name, _culture, args);
		}
	}
}
