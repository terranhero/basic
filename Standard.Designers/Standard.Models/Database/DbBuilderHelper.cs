using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.EntityLayer;
using Basic.Designer;
using Basic.Enums;

namespace Basic.Database
{
	/// <summary>
	/// 数据库实体构建类帮助器
	/// </summary>
	internal static class DbHelper
	{
		/// <summary>
		/// 表示从 DbTypeEnum 枚举类型转换成 .Net 类型辅助方法。
		/// </summary>
		/// <param name="dbType">DbTypeEnum 枚举类型，表示数据库字段类型。</param>
		/// <returns>返回 .Net 类型的字符串表示形式。</returns>
		public static string DbTypeToNetTypeString(DbTypeEnum dbType)
		{
			switch (dbType)
			{
				case DbTypeEnum.Guid:
					return "System.Guid";

				case DbTypeEnum.Boolean:
					return "bool";

				case DbTypeEnum.Int16:
					return "short";

				case DbTypeEnum.Int32:
					return "int";

				case DbTypeEnum.Int64:
					return "long";

				case DbTypeEnum.Decimal:
					return "decimal";

				case DbTypeEnum.Single:
					return "float";

				case DbTypeEnum.Double:
					return "double";

				case DbTypeEnum.Binary:
				case DbTypeEnum.VarBinary:
				case DbTypeEnum.Image:
					return "byte[]";

				case DbTypeEnum.Char:
				case DbTypeEnum.VarChar:
				case DbTypeEnum.Text:
				case DbTypeEnum.NChar:
				case DbTypeEnum.NVarChar:
				case DbTypeEnum.NText:
					return "string";

				case DbTypeEnum.Time:
					return "System.TimeSpan";

				case DbTypeEnum.Date:
				case DbTypeEnum.Timestamp:
				case DbTypeEnum.DateTime:
				case DbTypeEnum.DateTime2:
				case DbTypeEnum.DateTimeOffset:
					return "System.DateTime";
			}
			return "string";
		}

		/// <summary>
		/// 表示从 DbTypeEnum 枚举类型转换成 .Net 类型辅助方法。
		/// </summary>
		/// <param name="dbType">DbTypeEnum 枚举类型，表示数据库字段类型。</param>
		/// <returns>返回 .Net 类型。</returns>
		public static Type DbTypeToNetType(DbTypeEnum dbType)
		{
			switch (dbType)
			{
				case DbTypeEnum.Guid:
					return typeof(Guid);

				case DbTypeEnum.Boolean:
					return typeof(bool);

				case DbTypeEnum.Binary:
				case DbTypeEnum.VarBinary:
				case DbTypeEnum.Image:
					return typeof(byte[]);

				case DbTypeEnum.Int16:
					return typeof(short);

				case DbTypeEnum.Int32:
					return typeof(int);

				case DbTypeEnum.Int64:
					return typeof(long);

				case DbTypeEnum.Decimal:
					return typeof(decimal);

				case DbTypeEnum.Single:
					return typeof(float);

				case DbTypeEnum.Double:
					return typeof(double);

				case DbTypeEnum.Char:
				case DbTypeEnum.VarChar:
				case DbTypeEnum.Text:
				case DbTypeEnum.NChar:
				case DbTypeEnum.NVarChar:
				case DbTypeEnum.NText:
					return typeof(string);
				case DbTypeEnum.Time:
					return typeof(System.TimeSpan);

				case DbTypeEnum.Date:
				case DbTypeEnum.Timestamp:
				case DbTypeEnum.DateTime:
				case DbTypeEnum.DateTime2:
				case DbTypeEnum.DateTimeOffset:
					return typeof(System.DateTime);
			}
			return typeof(string);
		}
	}
}
