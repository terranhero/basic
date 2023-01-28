using System;

namespace Basic.Interfaces
{
	/// <summary>表示用户权限的验证</summary>
	public interface IAuthorizeValidation
	{
		/// <summary>检查授权码是否有效</summary>
		/// <param name="code">The authorization code.</param>
		/// <returns>如果有效则返回true，否则返回false。</returns>
		bool CheckCode(int code);

		/// <summary>检查区段授权码是否有效(从开始授权码检查到结束授权码，只要有一个条件则授权成功)。</summary>
		/// <param name="bCode">按钮授权码开始编号</param>
		/// <param name="eCode">按钮授权码结束编号</param>
		/// <returns>如果有效则返回true，否则返回false。</returns>
		bool CheckCode(int bCode, int eCode);

		/// <summary>检查一组授权码是否成功。</summary>
		/// <param name="codes">表示一组授权码</param>
		/// <returns>这组授权码其中一个检测成功则返回true，所有授权码检测都不成功则返回false。</returns>
		bool CheckCode(params int[] codes);

		/// <summary>检查授权码是否有效</summary>
		/// <param name="code">授权码</param>
		/// <returns>如果有效则返回true，否则返回false。</returns>
		bool CheckCode(string code);

		/// <summary>检查一组授权码是否成功。</summary>
		/// <param name="codes">表示一组授权码</param>
		/// <returns>这组授权码其中一个检测成功则返回true，所有授权码检测都不成功则返回false。</returns>
		bool CheckCode(params string[] codes);

		/// <summary>检查授权码是否有效</summary>
		/// <param name="code">授权码</param>
		/// <returns>如果有效则返回true，否则返回false。</returns>
		bool CheckCode(Guid code);

		/// <summary>检查一组授权码是否成功。</summary>
		/// <param name="codes">表示一组授权码</param>
		/// <returns>这组授权码其中一个检测成功则返回true，所有授权码检测都不成功则返回false。</returns>
		bool CheckCode(params Guid[] codes);

	}
}
