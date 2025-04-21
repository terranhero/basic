#if NET8_0_OR_GREATER
using System.Data.Common;
using Microsoft.Data.SqlClient;
#else
using System.Data.Common;
using System.Data.SqlClient;
#endif
using Basic.EntityLayer;
using System.Data;
using Basic.Exceptions;
using Basic.Enums;

namespace Basic.SqlServer
{
	internal static class SqlParameterConverter
	{

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="sqlParameter">数据库命令执行的参数</param>
		/// <param name="dbType">SqlServer数据库列类型,SqlDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		public static void ConvertSqlParameterType(SqlParameter sqlParameter, DataTypeEnum dbType, byte precision, byte scale)
		{
			switch (dbType)
			{
				case DataTypeEnum.Boolean:
					sqlParameter.SqlDbType = SqlDbType.Bit;
					break;
				case DataTypeEnum.Binary:
					sqlParameter.SqlDbType = SqlDbType.Binary;
					break;
				case DataTypeEnum.Char:
					sqlParameter.SqlDbType = SqlDbType.Char;
					break;
				case DataTypeEnum.Date:
					sqlParameter.SqlDbType = SqlDbType.Date;
					break;
				case DataTypeEnum.DateTime:
					sqlParameter.SqlDbType = SqlDbType.DateTime;
					break;
				case DataTypeEnum.DateTime2:
					sqlParameter.SqlDbType = SqlDbType.DateTime2;
					break;
				case DataTypeEnum.DateTimeOffset:
					sqlParameter.SqlDbType = SqlDbType.DateTimeOffset;
					break;
				case DataTypeEnum.Decimal:
					sqlParameter.SqlDbType = SqlDbType.Decimal;
					sqlParameter.Precision = precision;
					sqlParameter.Scale = scale;
					break;
				case DataTypeEnum.Double:
					sqlParameter.SqlDbType = SqlDbType.Float;
					break;
				case DataTypeEnum.Guid:
					sqlParameter.SqlDbType = SqlDbType.UniqueIdentifier;
					break;
				case DataTypeEnum.Int16:
					sqlParameter.SqlDbType = SqlDbType.SmallInt;
					break;
				case DataTypeEnum.Int32:
					sqlParameter.SqlDbType = SqlDbType.Int;
					break;
				case DataTypeEnum.Int64:
					sqlParameter.SqlDbType = SqlDbType.BigInt;
					break;
				case DataTypeEnum.Image:
					sqlParameter.SqlDbType = SqlDbType.VarBinary;
					sqlParameter.Size = -1;
					break;
				case DataTypeEnum.NText:
					sqlParameter.SqlDbType = SqlDbType.NVarChar;
					sqlParameter.Size = -1;
					break;
				case DataTypeEnum.Text:
					sqlParameter.SqlDbType = SqlDbType.VarChar;
					sqlParameter.Size = -1;
					break;
				case DataTypeEnum.NChar:
					sqlParameter.SqlDbType = SqlDbType.NChar;
					break;
				case DataTypeEnum.NVarChar:
					sqlParameter.SqlDbType = SqlDbType.NVarChar;
					break;
				case DataTypeEnum.Single:
					sqlParameter.SqlDbType = SqlDbType.Real;
					break;
				case DataTypeEnum.Time:
					sqlParameter.SqlDbType = SqlDbType.Time;
					break;
				case DataTypeEnum.Timestamp:
                    sqlParameter.SqlDbType = SqlDbType.DateTime;
					//sqlParameter.Size = 3;
					break;
				case DataTypeEnum.VarBinary:
					sqlParameter.SqlDbType = SqlDbType.VarBinary;
					break;
				case DataTypeEnum.VarChar:
					sqlParameter.SqlDbType = SqlDbType.VarChar;
					break;
				default:
					throw new ParameterDbTypeException("Access_ParameterType_UnSupport", typeof(DataTypeEnum), dbType.ToString("D"));
			}
		}

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="sqlParameter">数据库命令执行的参数</param>
		/// <param name="dbType">SqlServer数据库列类型,SqlDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		public static void ConvertSqlParameterType(SqlParameter sqlParameter, DbTypeEnum dbType, byte precision, byte scale)
		{
			switch (dbType)
			{
				case DbTypeEnum.Boolean:
					sqlParameter.SqlDbType = SqlDbType.Bit;
					return;
				case DbTypeEnum.Binary:
					sqlParameter.SqlDbType = SqlDbType.Binary;
					return;
				case DbTypeEnum.Char:
					sqlParameter.SqlDbType = SqlDbType.Char;
					return;
				case DbTypeEnum.Date:
					sqlParameter.SqlDbType = SqlDbType.Date;
					return;
				case DbTypeEnum.DateTime:
					sqlParameter.SqlDbType = SqlDbType.DateTime;
					return;
				case DbTypeEnum.DateTime2:
					sqlParameter.SqlDbType = SqlDbType.DateTime2;
					return;
				case DbTypeEnum.DateTimeOffset:
					sqlParameter.SqlDbType = SqlDbType.DateTimeOffset;
					return;
				case DbTypeEnum.Decimal:
					sqlParameter.SqlDbType = SqlDbType.Decimal;
					sqlParameter.Precision = precision;
					sqlParameter.Scale = scale;
					return;
				case DbTypeEnum.Double:
					sqlParameter.SqlDbType = SqlDbType.Float;
					return;
				case DbTypeEnum.Guid:
					sqlParameter.SqlDbType = SqlDbType.UniqueIdentifier;
					return;
				case DbTypeEnum.Int16:
					sqlParameter.SqlDbType = SqlDbType.SmallInt;
					return;
				case DbTypeEnum.Int32:
					sqlParameter.SqlDbType = SqlDbType.Int;
					return;
				case DbTypeEnum.Int64:
					sqlParameter.SqlDbType = SqlDbType.BigInt;
					return;
				case DbTypeEnum.Image:
					sqlParameter.SqlDbType = SqlDbType.VarBinary;
					sqlParameter.Size = -1;
					return;
				case DbTypeEnum.NText:
					sqlParameter.SqlDbType = SqlDbType.NVarChar;
					sqlParameter.Size = -1;
					return;
				case DbTypeEnum.Text:
					sqlParameter.SqlDbType = SqlDbType.VarChar;
					sqlParameter.Size = -1;
					return;
				case DbTypeEnum.NChar:
					sqlParameter.SqlDbType = SqlDbType.NChar;
					return;
				case DbTypeEnum.NVarChar:
					sqlParameter.SqlDbType = SqlDbType.NVarChar;
					return;
				case DbTypeEnum.Single:
					sqlParameter.SqlDbType = SqlDbType.Real;
					return;
				case DbTypeEnum.Time:
					sqlParameter.SqlDbType = SqlDbType.Time;
					return;
				case DbTypeEnum.Timestamp:
					sqlParameter.SqlDbType = SqlDbType.DateTime;
					//sqlParameter.Size = 3;
					return;
				case DbTypeEnum.VarBinary:
					sqlParameter.SqlDbType = SqlDbType.VarBinary;
					return;
				case DbTypeEnum.VarChar:
					sqlParameter.SqlDbType = SqlDbType.VarChar;
					return;
			}
			throw new ParameterDbTypeException("Access_ParameterType_UnSupport", typeof(DbTypeEnum), dbType.ToString("D"));
		}

	}
}
