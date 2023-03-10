using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Enums
{
	/// <summary>
	/// 日志管理类型 
	/// </summary>
	public enum LogSaveType : byte
	{
		/// <summary>
		/// 采用服务模式管理日志，使用远程通讯。
		/// </summary>
		DataBase = 4,
		/// <summary>
		/// 采用本地模式管理日志 
		/// </summary>
		LocalFile = 2,
		/// <summary>
		/// 忽略日志信息
		/// </summary>
		None = 0,
		/// <summary>
		/// 采用Windows EventLog模式管理日志
		/// </summary>
		Windows = 1
	}
}