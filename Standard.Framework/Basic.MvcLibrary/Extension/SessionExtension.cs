using System;
using System.Web.Mvc;
using System.Web;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 提供对Session的输入的支持。
	/// </summary>
	public static class SessionExtension
	{
		internal const string Session_PageSize = "Session_PageSize";
		internal const string Session_UserKey = "Session_UserKey";
		/// <summary>
		/// 为由指定表达式表示的每个数据字段的验证错误消息返回对应的 HTML 标记。 
		/// </summary>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="keyName">Session键名称。</param>
		/// <returns>如果该属性或对象有效，则为一个空字符串；否则为一个包含错误消息的 span 元素。</returns>
		private static object Session(this HtmlHelper htmlHelper, string keyName)
		{
			return htmlHelper.ViewContext.HttpContext.Session[keyName];
		}

		/// <summary>
		/// 为由指定表达式表示的每个数据字段的验证错误消息返回对应的 HTML 标记。 
		/// </summary>
		/// <param name="htmlHelper">此方法扩展的 HTML 帮助器实例。</param>
		/// <param name="keyName">Session键名称。</param>
		/// <param name="value">Session键值</param>
		private static void Session(this HtmlHelper htmlHelper, string keyName, object value)
		{
			htmlHelper.ViewContext.HttpContext.Session[keyName] = value;
		}

		/// <summary>
		/// 获取或设置当前用户关键字的到Session中。 
		/// </summary>
		/// <param name="session">此方法扩展的 Controller 实例。</param>
		/// <param name="userKey">分页控件每页大小。</param>
		public static int SessionUserKey(this HttpSessionStateBase session, int? userKey = null)
		{
			object obj = session[SessionExtension.Session_UserKey];
			if (obj != null && obj != DBNull.Value)
			{
				int key = (int)obj;
				if (userKey.HasValue && userKey != key)
				{
					session[SessionExtension.Session_UserKey] = userKey;
					return userKey.Value;
				}
				return key;
			}
			if (userKey.HasValue)
			{
				session[SessionExtension.Session_UserKey] = userKey.Value;
				return userKey.Value;
			}
			return 0;
		}

		/// <summary>
		/// 获取或设置当前用户关键字的到Session中。 
		/// </summary>
		/// <param name="session">此方法扩展的 Controller 实例。</param>
		/// <param name="userKey">分页控件每页大小。</param>
		public static Guid SessionUserKey(this HttpSessionStateBase session, Guid? userKey = null)
		{
			object obj = session[SessionExtension.Session_UserKey];
			if (obj != null && obj != DBNull.Value)
			{
				Guid key = (Guid)obj;
				if (userKey.HasValue && userKey != key)
				{
					session[SessionExtension.Session_UserKey] = userKey;
					return userKey.Value;
				}
				return key;
			}
			if (userKey.HasValue)
			{
				session[SessionExtension.Session_UserKey] = userKey.Value;
				return userKey.Value;
			}
			return Guid.Empty;
		}

		/// <summary>
		/// 设置当前用户分页的大小到Session中。 
		/// </summary>
		/// <param name="session">此方法扩展的 Controller 实例。</param>
		/// <param name="pageSize">分页控件每页大小。</param>
		public static int SessionPageSize(this HttpSessionStateBase session, int? pageSize = null)
		{
			object obj = session[SessionExtension.Session_PageSize];
			if (obj != null && obj != DBNull.Value)
			{
				int page = (int)obj;
				if (pageSize.HasValue && pageSize != page)
				{
					session[SessionExtension.Session_PageSize] = pageSize;
					return pageSize.Value;
				}
				return page;
			}
			if (pageSize.HasValue && pageSize.Value > 0)
			{
				session[SessionExtension.Session_PageSize] = pageSize.Value;
				return pageSize.Value;
			}
			return 0;
		}
	}
}
