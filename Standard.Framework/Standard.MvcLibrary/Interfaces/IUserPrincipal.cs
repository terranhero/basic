

namespace Basic.Interfaces
{
	/// <summary>表示用户登录信息</summary>
	public interface IUserPrincipal : IUserContext
	{
		/// <summary>登录用户关键字</summary>
		System.Guid UserKey { get; }

		/// <summary>登录员工关键字</summary>
		System.Int32 EmpKey { get; }

		/// <summary>用户角色</summary>
		System.Guid RoleKey { get; }

		/// <summary>用户群组</summary>
		System.Guid GroupKey { get; }
	}
}
