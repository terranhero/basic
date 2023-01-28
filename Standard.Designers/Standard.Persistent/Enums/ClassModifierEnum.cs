using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Enums
{
	/// <summary>
	/// 实体类的可访问属性
	/// </summary>
	public enum ClassModifierEnum
	{
		/// <summary>
		/// 实体类型是公共成员。
		/// </summary>
		Public,
		/// <summary>
		/// 实体类型在程序集内可见
		/// </summary>
		Internal
	}

	/// <summary>
	/// 实体类的可访问属性
	/// </summary>
	public enum PropertyModifierEnum
	{
		/// <summary>
		/// 属性是公共成员。
		/// </summary>
		Public,
		/// <summary>
		/// 属性在程序集内可访问
		/// </summary>
		Internal,
		/// <summary>
		/// 属性在类外不可访问
		/// </summary>
		Private,
		/// <summary>
		/// 属性仅在家族成员内可访问
		/// </summary>
		Protected,
		/// <summary>
		/// 属性仅在家族成员和程序集内可访问
		/// </summary>
		ProtectedInternal
	}
}
