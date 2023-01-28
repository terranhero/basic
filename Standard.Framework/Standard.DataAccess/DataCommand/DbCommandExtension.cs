using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Basic.DataAccess
{
	/// <summary>
	/// 数据库命令 DbCommand 类实例方法扩展
	/// </summary>
	internal static class DbCommandExtension
	{
		/// <summary>
		/// 清空 DbParameterCollection 参数集合中所有参数的值。
		/// </summary>
		/// <param name="parameters">参数集合。</param>
		/// <returns></returns>
		internal static bool ClearParametersValue(this DbParameterCollection parameters)
		{
			if (parameters == null || parameters.Count == 0) { return true; }
			foreach (DbParameter parameter in parameters)
			{ parameter.Value = DBNull.Value; }
			return true;
		}

		/// <summary>
		/// 清空命令参数
		/// </summary>
		/// <param name="dataCommand"></param>
		/// <returns></returns>
		internal static bool ClearParametersValue(this DbCommand dataCommand)
		{
			if (dataCommand == null) { return false; }
			return dataCommand.Parameters.ClearParametersValue();
		}
	}
}
