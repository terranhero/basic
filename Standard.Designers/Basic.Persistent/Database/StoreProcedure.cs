using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Configuration;
using Basic.Enums;
using Basic.Designer;
using Basic.Collections;
using Basic.EntityLayer;
using System.Data;
using Basic.Database;

namespace Basic.Database
{
	/// <summary>
	/// 
	/// </summary>
	public class StoreProcedure
	{
		/// <summary>
		/// 初始化 TableInfoElement 类实例
		/// </summary>
		public StoreProcedure() { }

		/// <summary>
		/// 所有者
		/// </summary>
		public string Owner { get; set; }

		/// <summary>
		/// 数据库存储过程名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 数据库存储过程名称
		/// </summary>
		public string EntityName { get; set; }

		/// <summary>
		/// 存储过程参数信息
		/// </summary>
		public ProcedureParameter[] Parameters { get; set; }

		/// <summary>
		/// 存储过程结果表列信息
		/// </summary>
		public DesignColumnInfo[] Columns { get; set; }
	}
}
