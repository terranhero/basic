using System;
using System.Collections.Generic;
using System.Text;

namespace Basic.Enums
{
	/// <summary>
	/// 数据库连接类型
	/// </summary>
	public enum DataBaseType
	{
		/// <summary>
		/// SQL SERVER类型数据库
		/// </summary>
		SqlServer = 1,

		/// <summary>
		/// Oracle类型数据库
		/// </summary>
		Oracle,

		/// <summary>
		/// DB2类型数据库
		/// </summary>
		IBMDB2,

		/// <summary>
		/// Access类型数据库
		/// </summary>
		Access,

		/// <summary>
		/// SQLite类型数据库
		/// </summary>
		SQLite,

        /// <summary>
        /// MySQL类型数据库
		/// </summary>
		MySQL,

		/// <summary>
		/// PostgreSQL
		/// </summary>
		PostgreSQL
	}
}
