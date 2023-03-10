using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Enums
{
	/// <summary>
	/// 实体类生成方式，包含生成实体类，生成强类型DataTable，生成实体类和强类型DataTable
	/// </summary>
	public enum GenerateModeEnum : byte
	{
		DataEntity = 1, DataTable = 2, TableEntity = 3
	}
}