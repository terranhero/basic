using Basic.Enums;
using Basic.Exceptions;
using IBM.Data.DB2.Core;

namespace Basic.DB2Access
{
	internal static class DB2ParameterConverter
	{

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="parameter">数据库命令执行的参数</param>
		/// <param name="dbType">DB2Server数据库列类型,DB2Type枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		public static void ConvertDB2ParameterType(DB2Parameter parameter, DataTypeEnum dbType, byte precision, byte scale)
		{
			switch (dbType)
			{
				case DataTypeEnum.Boolean:
					parameter.DB2Type = DB2Type.Byte;
					break;
				case DataTypeEnum.Binary:
					parameter.DB2Type = DB2Type.Binary;
					break;
				case DataTypeEnum.Char:
					parameter.DB2Type = DB2Type.Char;
					break;
				case DataTypeEnum.Date:
					parameter.DB2Type = DB2Type.Date;
					break;
				case DataTypeEnum.DateTime:
					parameter.DB2Type = DB2Type.DateTime;
					break;
				case DataTypeEnum.DateTime2:
					parameter.DB2Type = DB2Type.Timestamp;
					break;
				case DataTypeEnum.DateTimeOffset:
					parameter.DB2Type = DB2Type.TimeStampWithTimeZone;
					break;
				case DataTypeEnum.Decimal:
					parameter.DB2Type = DB2Type.Decimal;
					parameter.Precision = precision;
					parameter.Scale = scale;
					break;
				case DataTypeEnum.Double:
					parameter.DB2Type = DB2Type.Float;
					break;
				case DataTypeEnum.Guid:
					parameter.DB2Type = DB2Type.Char;
					parameter.Size = 36;
					break;
				case DataTypeEnum.Int16:
					parameter.DB2Type = DB2Type.SmallInt;
					break;
				case DataTypeEnum.Int32:
					parameter.DB2Type = DB2Type.Integer;
					break;
				case DataTypeEnum.Int64:
					parameter.DB2Type = DB2Type.BigInt;
					break;
				case DataTypeEnum.Image:
					parameter.DB2Type = DB2Type.VarBinary;
					parameter.Size = -1;
					break;
				case DataTypeEnum.NText:
					parameter.DB2Type = DB2Type.NVarChar;
					parameter.Size = -1;
					break;
				case DataTypeEnum.Text:
					parameter.DB2Type = DB2Type.VarChar;
					parameter.Size = -1;
					break;
				case DataTypeEnum.NChar:
					parameter.DB2Type = DB2Type.NChar;
					break;
				case DataTypeEnum.NVarChar:
					parameter.DB2Type = DB2Type.NVarChar;
					break;
				case DataTypeEnum.Single:
					parameter.DB2Type = DB2Type.Real;
					break;
				case DataTypeEnum.Time:
					parameter.DB2Type = DB2Type.Time;
					break;
				case DataTypeEnum.Timestamp:
					parameter.DB2Type = DB2Type.DateTime;
					//parameter.Size = 3;
					break;
				case DataTypeEnum.VarBinary:
					parameter.DB2Type = DB2Type.VarBinary;
					break;
				case DataTypeEnum.VarChar:
					parameter.DB2Type = DB2Type.VarChar;
					break;
				default:
					throw new ParameterDbTypeException("Access_ParameterType_UnSupport", typeof(DataTypeEnum), dbType.ToString("D"));
			}
		}

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="parameter">数据库命令执行的参数</param>
		/// <param name="dbType">DB2Server数据库列类型,DB2Type枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		public static void ConvertDB2ParameterType(DB2Parameter parameter, DbTypeEnum dbType, byte precision, byte scale)
		{
			switch (dbType)
			{
				case DbTypeEnum.Boolean:
					parameter.DB2Type = DB2Type.Byte;
					return;
				case DbTypeEnum.Binary:
					parameter.DB2Type = DB2Type.Binary;
					return;
				case DbTypeEnum.Char:
					parameter.DB2Type = DB2Type.Char;
					return;
				case DbTypeEnum.Date:
					parameter.DB2Type = DB2Type.Date;
					return;
				case DbTypeEnum.DateTime:
					parameter.DB2Type = DB2Type.DateTime;
					return;
				case DbTypeEnum.DateTime2:
					parameter.DB2Type = DB2Type.Timestamp;
					return;
				case DbTypeEnum.DateTimeOffset:
					parameter.DB2Type = DB2Type.TimeStampWithTimeZone;
					return;
				case DbTypeEnum.Decimal:
					parameter.DB2Type = DB2Type.Decimal;
					parameter.Precision = precision;
					parameter.Scale = scale;
					return;
				case DbTypeEnum.Double:
					parameter.DB2Type = DB2Type.Float;
					return;
				case DbTypeEnum.Guid:
					parameter.DB2Type = DB2Type.Char;
					parameter.Size = 36;
					return;
				case DbTypeEnum.Int16:
					parameter.DB2Type = DB2Type.SmallInt;
					return;
				case DbTypeEnum.Int32:
					parameter.DB2Type = DB2Type.Integer;
					return;
				case DbTypeEnum.Int64:
					parameter.DB2Type = DB2Type.BigInt;
					return;
				case DbTypeEnum.Image:
					parameter.DB2Type = DB2Type.VarBinary;
					parameter.Size = -1;
					return;
				case DbTypeEnum.NText:
					parameter.DB2Type = DB2Type.NVarChar;
					parameter.Size = -1;
					return;
				case DbTypeEnum.Text:
					parameter.DB2Type = DB2Type.VarChar;
					parameter.Size = -1;
					return;
				case DbTypeEnum.NChar:
					parameter.DB2Type = DB2Type.NChar;
					return;
				case DbTypeEnum.NVarChar:
					parameter.DB2Type = DB2Type.NVarChar;
					return;
				case DbTypeEnum.Single:
					parameter.DB2Type = DB2Type.Real;
					return;
				case DbTypeEnum.Time:
					parameter.DB2Type = DB2Type.Time;
					return;
				case DbTypeEnum.Timestamp:
					parameter.DB2Type = DB2Type.DateTime;
					//parameter.Size = 3;
					return;
				case DbTypeEnum.VarBinary:
					parameter.DB2Type = DB2Type.VarBinary;
					return;
				case DbTypeEnum.VarChar:
					parameter.DB2Type = DB2Type.VarChar;
					return;
			}
			throw new ParameterDbTypeException("Access_ParameterType_UnSupport", typeof(DbTypeEnum), dbType.ToString("D"));
		}

	}
}
