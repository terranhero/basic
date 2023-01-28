using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Enums
{
	/// <summary>属性命名规则</summary>
	public enum NamingRules
	{
		/// <summary>
		/// 按输入规则命名
		/// </summary>
		DefaultCase = 1,

		/// <summary>
		/// 帕斯卡命名规则
		/// </summary>
		PascalCase = 2,

		/// <summary>
		/// 驼峰命名规则
		/// </summary>
		CamelCase = 3,

		/// <summary>
		/// 大写命名规则
		/// </summary>
		UpperCase = 4,

		/// <summary>
		/// 小写命名规则
		/// </summary>
		LowerCase = 5
	}
}
