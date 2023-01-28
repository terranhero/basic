using System.Web.Mvc;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 为验证授权码事件提供委托。
	/// </summary>
	/// <param name="sender">调用此方法的HtmlHelper实例。</param>
	/// <param name="authorizationCode">事件参数</param>
	/// <returns>如果有效则返回true，否则返回false。</returns>
	public delegate bool AuthorizeCodeEventHandler(object sender, int authorizationCode);

	/// <summary>
	/// 为验证区段授权码事件提供委托。
	/// </summary>
	/// <param name="sender">调用此方法的HtmlHelper实例。</param>
	/// <param name="authorizeBeginCode">按钮授权码开始编号</param>
	/// <param name="authorizeEndCode">按钮授权码结束编号</param>
	/// <returns>如果有效则返回true，否则返回false。</returns>
	public delegate bool AuthorizeRegionCodeEventHandler(object sender, int authorizeBeginCode, int authorizeEndCode);

	/// <summary>
	/// 按钮授权管理
	/// </summary>
	public static class AuthorizeContext
	{
		/// <summary>
		/// 静态初始化AuthorizeContext类实例
		/// </summary>
		static AuthorizeContext() { }

		/// <summary>
		/// 授权码校验事件
		/// </summary>
		public static event AuthorizeCodeEventHandler CheckAuthorizeCode;
		/// <summary>
		/// 区段授权码校验事件
		/// </summary>
		public static event AuthorizeRegionCodeEventHandler CheckAuthorizeResionCode;

		/// <summary>检查授权码是否有效</summary>
		/// <param name="bh">The HTML.</param>
		/// <param name="authorizationCode">The authorization code.</param>
		/// <returns>如果有效则返回true，否则返回false。</returns>
		internal static bool CheckAuthorizationCode(IBasicContext bh, int authorizationCode)
		{
			if (authorizationCode == 0) { return false; }
			if (CheckAuthorizeCode != null)
			{
				return CheckAuthorizeCode(bh, authorizationCode);
			}
			return true;
		}

		/// <summary>检查区段授权码是否有效(从开始授权码检查到结束授权码，只要有一个条件则授权成功)。</summary>
		/// <param name="bh">The HTML.</param>
		/// <param name="authorizeBeginCode">按钮授权码开始编号</param>
		/// <param name="authorizeEndCode">按钮授权码结束编号</param>
		/// <returns>如果有效则返回true，否则返回false。</returns>
		internal static bool CheckAuthorizationCode(IBasicContext bh, int authorizeBeginCode, int authorizeEndCode)
		{
			if (CheckAuthorizeResionCode != null)
			{
				if (authorizeBeginCode == 0 && authorizeEndCode == 0) { return false; }
				return CheckAuthorizeResionCode(bh, authorizeBeginCode, authorizeEndCode);
			}
			return true;
		}

		/// <summary>
		/// 检查授权码是否有效
		/// </summary>
		/// <param name="html">The HTML.</param>
		/// <param name="authorizationCode">The authorization code.</param>
		/// <returns>
		/// 如果有效则返回true，否则返回false。
		/// </returns>
		internal static bool CheckAuthorizationCode(HtmlHelper html, int authorizationCode)
		{
			if (authorizationCode == 0) { return false; }
			if (CheckAuthorizeCode != null)
			{
				return CheckAuthorizeCode(html, authorizationCode);
			}
			return true;
		}

		/// <summary>
		/// 检查区段授权码是否有效(从开始授权码检查到结束授权码，只要有一个条件则授权成功)。
		/// </summary>
		/// <param name="html">The HTML.</param>
		/// <param name="authorizeBeginCode">按钮授权码开始编号</param>
		/// <param name="authorizeEndCode">按钮授权码结束编号</param>
		/// <returns>
		/// 如果有效则返回true，否则返回false。
		/// </returns>
		internal static bool CheckAuthorizationCode(HtmlHelper html, int authorizeBeginCode, int authorizeEndCode)
		{
			if (CheckAuthorizeResionCode != null)
			{
				if (authorizeBeginCode == 0 && authorizeEndCode == 0) { return false; }
				return CheckAuthorizeResionCode(html, authorizeBeginCode, authorizeEndCode);
			}
			return true;
		}
	}
}
