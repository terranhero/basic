using System.Data;
using Basic.Enums;
using Basic.Exceptions;
using Oracle.ManagedDataAccess.Client;

namespace Basic.OracleAccess
{
	internal static class ParameterConverter
	{
		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="parameter">数据库命令执行的参数</param>
		/// <param name="dbType">SqlServer数据库列类型,OracleDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		public static void ConvertParameterType(OracleParameter parameter, DataTypeEnum dbType, byte precision, byte scale)
		{
			switch (dbType)
			{
				case DataTypeEnum.Boolean:
					parameter.OracleDbType = OracleDbType.Boolean;
					break;
				case DataTypeEnum.Binary:
					parameter.OracleDbType = OracleDbType.Blob;
					break;
				case DataTypeEnum.Char:
					parameter.OracleDbType = OracleDbType.Char;
					break;
				case DataTypeEnum.Date:
					parameter.OracleDbType = OracleDbType.Date;
					break;
				case DataTypeEnum.DateTime:
					parameter.OracleDbType = OracleDbType.TimeStamp;
					break;
				case DataTypeEnum.DateTime2:
					parameter.OracleDbType = OracleDbType.TimeStamp;
					break;
				case DataTypeEnum.DateTimeOffset:
					parameter.OracleDbType = OracleDbType.TimeStampLTZ;
					break;
				case DataTypeEnum.Decimal:
					parameter.OracleDbType = OracleDbType.Decimal;
					parameter.Precision = precision;
					parameter.Scale = scale;
					break;
				case DataTypeEnum.Double:
					parameter.OracleDbType = OracleDbType.Double;
					break;
				case DataTypeEnum.Guid:
					parameter.DbType = DbType.Guid;
					break;
				case DataTypeEnum.Int16:
					parameter.OracleDbType = OracleDbType.Int16;
					break;
				case DataTypeEnum.Int32:
					parameter.OracleDbType = OracleDbType.Int32;
					break;
				case DataTypeEnum.Int64:
					parameter.OracleDbType = OracleDbType.Int64;
					break;
				case DataTypeEnum.Image:
					parameter.OracleDbType = OracleDbType.Blob;
					parameter.Size = -1;
					break;
				case DataTypeEnum.NText:
					parameter.OracleDbType = OracleDbType.NClob;
					parameter.Size = -1;
					break;
				case DataTypeEnum.Text:
					parameter.OracleDbType = OracleDbType.Clob;
					parameter.Size = -1;
					break;
				case DataTypeEnum.NChar:
					parameter.OracleDbType = OracleDbType.NChar;
					break;
				case DataTypeEnum.NVarChar:
					parameter.OracleDbType = OracleDbType.NVarchar2;
					break;
				case DataTypeEnum.Single:
					parameter.OracleDbType = OracleDbType.Single;
					break;
				case DataTypeEnum.Time:
					parameter.OracleDbType = OracleDbType.IntervalDS;
					break;
				case DataTypeEnum.Timestamp:
					parameter.OracleDbType = OracleDbType.TimeStamp;
					//sqlParameter.Size = 3;
					break;
				case DataTypeEnum.VarBinary:
					parameter.OracleDbType = OracleDbType.Blob;
					break;
				case DataTypeEnum.VarChar:
					parameter.OracleDbType = OracleDbType.Varchar2;
					break;
				default:
					throw new ParameterDbTypeException("Access_ParameterType_UnSupport", typeof(DataTypeEnum), dbType.ToString("D"));
			}
		}

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="sqlParameter">数据库命令执行的参数</param>
		/// <param name="dbType">SqlServer数据库列类型,OracleDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		public static void ConvertParameterType(OracleParameter sqlParameter, DbTypeEnum dbType, byte precision, byte scale)
		{
			switch (dbType)
			{
				case DbTypeEnum.Boolean:
					sqlParameter.OracleDbType = OracleDbType.Boolean;
					return;
				case DbTypeEnum.Binary:
					sqlParameter.OracleDbType = OracleDbType.Blob;
					return;
				case DbTypeEnum.Char:
					sqlParameter.OracleDbType = OracleDbType.Char;
					return;
				case DbTypeEnum.Date:
					sqlParameter.OracleDbType = OracleDbType.Date;
					return;
				case DbTypeEnum.DateTime:
					sqlParameter.OracleDbType = OracleDbType.TimeStamp;
					return;
				case DbTypeEnum.DateTime2:
					sqlParameter.OracleDbType = OracleDbType.TimeStamp;
					return;
				case DbTypeEnum.DateTimeOffset:
					sqlParameter.OracleDbType = OracleDbType.TimeStampLTZ;
					return;
				case DbTypeEnum.Decimal:
					sqlParameter.OracleDbType = OracleDbType.Decimal;
					sqlParameter.Precision = precision;
					sqlParameter.Scale = scale;
					return;
				case DbTypeEnum.Double:
					sqlParameter.OracleDbType = OracleDbType.Double;
					return;
				case DbTypeEnum.Guid:
					sqlParameter.DbType = DbType.Guid;
					//sqlParameter.OracleDbType = OracleDbType.Char;
					return;
				case DbTypeEnum.Int16:
					sqlParameter.OracleDbType = OracleDbType.Int16;
					return;
				case DbTypeEnum.Int32:
					sqlParameter.OracleDbType = OracleDbType.Int32;
					return;
				case DbTypeEnum.Int64:
					sqlParameter.OracleDbType = OracleDbType.Int64;
					return;
				case DbTypeEnum.Image:
					sqlParameter.OracleDbType = OracleDbType.Blob;
					sqlParameter.Size = -1;
					return;
				case DbTypeEnum.NText:
					sqlParameter.OracleDbType = OracleDbType.NClob;
					sqlParameter.Size = -1;
					return;
				case DbTypeEnum.Text:
					sqlParameter.OracleDbType = OracleDbType.Clob;
					sqlParameter.Size = -1;
					return;
				case DbTypeEnum.NChar:
					sqlParameter.OracleDbType = OracleDbType.NChar;
					return;
				case DbTypeEnum.NVarChar:
					sqlParameter.OracleDbType = OracleDbType.NVarchar2;
					return;
				case DbTypeEnum.Single:
					sqlParameter.OracleDbType = OracleDbType.Single;
					return;
				case DbTypeEnum.Time:
					sqlParameter.OracleDbType = OracleDbType.IntervalDS;
					return;
				case DbTypeEnum.Timestamp:
					sqlParameter.OracleDbType = OracleDbType.TimeStamp;
					//sqlParameter.Size = 3;
					return;
				case DbTypeEnum.VarBinary:
					sqlParameter.OracleDbType = OracleDbType.Blob;
					return;
				case DbTypeEnum.VarChar:
					sqlParameter.OracleDbType = OracleDbType.Varchar2;
					return;
			}
			throw new ParameterDbTypeException("Access_ParameterType_UnSupport", typeof(DbTypeEnum), dbType.ToString("D"));
		}

	}
}
