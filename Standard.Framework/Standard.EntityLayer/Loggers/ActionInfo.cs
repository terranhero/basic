using System;

namespace Basic.Loggers
{
	/// <summary>
	/// 表示系统日志请求菜单名称和操作名称定义
	/// </summary>
	public struct ActionInfo : IComparable<ActionInfo>
	{
		private readonly string _Url;
		private readonly string _Controller;
		private readonly string _Action;

		/// <summary>
		/// 使用参数初始化 ActionInfo 实例。
		/// </summary>
		/// <param name="url">表示请求的路径。</param>
		/// <param name="controller">表示当前请求所属控制器、窗体名称</param>
		/// <param name="action">表示当前请求名称</param>
		public ActionInfo(string url, string controller, string action)
		{
			_Url = url.ToLower(); _Controller = controller; _Action = action;
		}

		/// <summary>
		/// 表示请求的路径
		/// </summary>
		public string Url { get { return _Url; } }

		/// <summary>
		/// 表示当前请求所属控制器、窗体名称
		/// </summary>
		public string Controller { get { return _Controller; } }

		/// <summary>
		/// 表示当前请求名称
		/// </summary>
		public string Action { get { return _Action; } }

		/// <summary>
		/// 比较当前对象和同一类型的另一对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>一个值，指示要比较的对象的相对顺序。返回值的含义如下：
		/// 值含义小于零此对象小于 other 参数。
		/// 零此对象等于 other。
		/// 大于零此对象大于 other。</returns>
		int IComparable<ActionInfo>.CompareTo(ActionInfo other)
		{
			return string.Compare(_Url, other.Url, true);
		}
	}
}
