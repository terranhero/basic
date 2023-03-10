using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Enums
{
	/// <summary>
	/// 数据库命令结构类型
	/// </summary>
	[System.Flags()]
	public enum ConfigurationTypeEnum
	{
		/// <summary>
		/// 表示 Transact-SQL 插入数据命令。
		/// </summary>
		AddNew = 1,
		/// <summary>
		/// 表示 Transact-SQL 更新数据命令。
		/// </summary>
		Modify = 2,
		/// <summary>
		/// 表示 Transact-SQL 删除数据命令。
		/// </summary>
		Remove = 4,
		/// <summary>
		/// 表示 Transact-SQL 查询数据命令。
		/// </summary>
		SearchTable = 8,
		/// <summary>
		/// 表示 Transact-SQL 查询数据命令。
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		SelectByKey = 16,
		/// <summary>
		/// 表示 Transact-SQL 类型命令
		/// </summary>
		Other = 32,
		/// <summary>
		/// 表示 Transact-SQL 插入数据命令。
		/// </summary>
		Insert = 64,
		/// <summary>
		/// 表示 Transact-SQL 更新数据命令。
		/// </summary>
		Update = 128,
		/// <summary>
		/// 表示 Transact-SQL 删除数据命令。
		/// </summary>
		Delete = 256,
	}
}