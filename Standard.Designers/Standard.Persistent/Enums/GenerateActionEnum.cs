using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Enums
{
	/// <summary>
	/// 代码生成模式
	/// </summary>
	public enum GenerateActionEnum : byte
	{
		/// <summary>
		/// 系统自动初始化时采用单个实体类
		/// </summary>
		Single = 1,

		/// <summary>
		/// 系统自动初始化时采用多个实体类。
		/// </summary>
		Multiple = 2
	}
}
