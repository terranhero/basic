
namespace Basic.Interfaces
{
	/// <summary>
	/// 权限授权点
	/// </summary>
	public interface IAuthorizePoint
	{
		/// <summary>
		/// 权限编码（全局唯一）
		/// </summary>
		int PointKey { get; }

		/// <summary>
		/// 按钮图标
		/// </summary>
		string IconCls { get; }

		/// <summary>
		/// 权限文本
		/// </summary>
		string Text { get; }

		/// <summary>
		/// 权限请求URL
		/// </summary>
		string RequestPath { get; }

		/// <summary>
		/// 权限描述
		/// </summary>
		string Description { get; }
	}
}
