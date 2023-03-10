using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Enums
{
	/// <summary>
	/// 
	/// </summary>
	[System.Flags()]
	public enum ObjectTypeEnum
	{
		/// <summary>
		/// 表（用户定义类型）
		/// </summary>
		UserTable = 0x1,
		/// <summary>
		/// 视图（用户定义类型）
		/// </summary>
		UserView = 0x2,
		/// <summary>
		/// 程序集 (CLR) 表值函数
		/// </summary>
		ClrTableFunction = 0x4,
		/// <summary>
		/// SQL 表值函数
		/// </summary>
		SqlTableFunction = 0x8,
		/// <summary>
		/// SQL 内联表值函数
		/// </summary>
		InlineTableFunction = 0x10,
		/// <summary>
		/// SQL 存储过程
		/// </summary>
		StoredProcedure = 0x20
	}
}