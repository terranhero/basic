using System.Globalization;

namespace Basic.Interfaces
{
	/// <summary>表示当前操作用户的上下文</summary>
	public interface IUserContext
	{
		/// <summary>当前用户使用的数据库连接</summary>
		string Connection { get; }

		/// <summary>当前用户上下文唯一键(全局唯一)</summary>
		string SessionID { get; }

		/// <summary>当前用户(全局唯一)</summary>
		string User { get; }

		/// <summary>当前用户特定区域</summary>
		System.Globalization.CultureInfo Culture { get; }
	}

	/// <summary>默认实现 IUserContext 接口类</summary>
	internal sealed class UserContext : IUserContext
	{
		/// <summary>初始化 UserContext 类实例</summary>
		/// <param name="con">数据库连接名称</param>
		/// <param name="ci"></param>
		internal UserContext(string con, CultureInfo ci) { Connection = con; Culture = ci; }

		/// <summary>初始化 UserContext 类实例</summary>
		/// <param name="con">数据库连接名称</param>
		internal UserContext(string con) { Connection = con; Culture = null; }

		/// <summary>当前用户使用的数据库连接</summary>
		public string Connection { get; }

		/// <summary>当前用户上下文唯一键(全局唯一)</summary>
		public string SessionID { get; }

		/// <summary>当前用户(全局唯一)</summary>
		public string User { get; }

		/// <summary>当前用户特定区域</summary>
		public CultureInfo Culture { get; }
	}
}
