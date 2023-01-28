using Basic.Enums;
using Basic.Exceptions;
using MySql.Data.MySqlClient;

namespace Basic.MySqlAccess
{
	internal static class MySqlParameterConverter
	{

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="sqlParameter">数据库命令执行的参数</param>
		/// <param name="dbType">SqlServer数据库列类型,MySqlDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		public static void ConvertSqlParameterType(MySqlParameter sqlParameter, DataTypeEnum dbType, byte precision, byte scale)
		{
			switch (dbType)
			{
				case DataTypeEnum.Boolean:
					sqlParameter.MySqlDbType = MySqlDbType.Bit;
					break;
				case DataTypeEnum.Binary:
					sqlParameter.MySqlDbType = MySqlDbType.Binary;
					break;
				case DataTypeEnum.Char:
					sqlParameter.MySqlDbType = MySqlDbType.VarChar;
					break;
				case DataTypeEnum.Date:
					sqlParameter.MySqlDbType = MySqlDbType.Date;
					break;
				case DataTypeEnum.DateTime:
					sqlParameter.MySqlDbType = MySqlDbType.DateTime;
					break;
				case DataTypeEnum.DateTime2:
					sqlParameter.MySqlDbType = MySqlDbType.Timestamp;
					break;
				case DataTypeEnum.DateTimeOffset:
					sqlParameter.MySqlDbType = MySqlDbType.DateTime;
					break;
				case DataTypeEnum.Decimal:
					sqlParameter.MySqlDbType = MySqlDbType.Decimal;
					sqlParameter.Precision = precision;
					sqlParameter.Scale = scale;
					break;
				case DataTypeEnum.Double:
					sqlParameter.MySqlDbType = MySqlDbType.Float;
					break;
				case DataTypeEnum.Guid:
					sqlParameter.MySqlDbType = MySqlDbType.Guid;
					break;
				case DataTypeEnum.Int16:
					sqlParameter.MySqlDbType = MySqlDbType.Int16;
					break;
				case DataTypeEnum.Int32:
					sqlParameter.MySqlDbType = MySqlDbType.Int32;
					break;
				case DataTypeEnum.Int64:
					sqlParameter.MySqlDbType = MySqlDbType.Int64;
					break;
				case DataTypeEnum.Image:
					sqlParameter.MySqlDbType = MySqlDbType.LongBlob;
					sqlParameter.Size = -1;
					break;
				case DataTypeEnum.NText:
					sqlParameter.MySqlDbType = MySqlDbType.LongText;
					sqlParameter.Size = -1;
					break;
				case DataTypeEnum.Text:
					sqlParameter.MySqlDbType = MySqlDbType.Text;
					sqlParameter.Size = -1;
					break;
				case DataTypeEnum.NChar:
					sqlParameter.MySqlDbType = MySqlDbType.VarChar;
					break;
				case DataTypeEnum.NVarChar:
					sqlParameter.MySqlDbType = MySqlDbType.VarChar;
					break;
				case DataTypeEnum.Single:
					sqlParameter.MySqlDbType = MySqlDbType.Float;
					break;
				case DataTypeEnum.Time:
					sqlParameter.MySqlDbType = MySqlDbType.Time;
					break;
				case DataTypeEnum.Timestamp:
					sqlParameter.MySqlDbType = MySqlDbType.Timestamp;
					sqlParameter.Size = 3;
					break;
				case DataTypeEnum.VarBinary:
					sqlParameter.MySqlDbType = MySqlDbType.VarBinary;
					break;
				case DataTypeEnum.VarChar:
					sqlParameter.MySqlDbType = MySqlDbType.VarChar;
					break;
				default:
					throw new ParameterDbTypeException("Access_ParameterType_UnSupport", typeof(DataTypeEnum), dbType.ToString("D"));
			}
		}

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="sqlParameter">数据库命令执行的参数</param>
		/// <param name="dbType">SqlServer数据库列类型,MySqlDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		public static void ConvertSqlParameterType(MySqlParameter sqlParameter, DbTypeEnum dbType, byte precision, byte scale)
		{
			switch (dbType)
			{
				case DbTypeEnum.Boolean:
					sqlParameter.MySqlDbType = MySqlDbType.Bit;
					return;
				case DbTypeEnum.Binary:
					sqlParameter.MySqlDbType = MySqlDbType.Binary;
					return;
				case DbTypeEnum.Char:
					sqlParameter.MySqlDbType = MySqlDbType.VarChar;
					return;
				case DbTypeEnum.Date:
					sqlParameter.MySqlDbType = MySqlDbType.Date;
					return;
				case DbTypeEnum.DateTime:
					sqlParameter.MySqlDbType = MySqlDbType.DateTime;
					sqlParameter.Size = 3;
					return;
				case DbTypeEnum.DateTime2:
					sqlParameter.MySqlDbType = MySqlDbType.DateTime;
					return;
				case DbTypeEnum.DateTimeOffset:
					sqlParameter.MySqlDbType = MySqlDbType.DateTime;
					return;
				case DbTypeEnum.Decimal:
					sqlParameter.MySqlDbType = MySqlDbType.Decimal;
					sqlParameter.Precision = precision;
					sqlParameter.Scale = scale;
					return;
				case DbTypeEnum.Double:
					sqlParameter.MySqlDbType = MySqlDbType.Float;
					return;
				case DbTypeEnum.Guid:
					sqlParameter.MySqlDbType = MySqlDbType.Guid;
					return;
				case DbTypeEnum.Int16:
					sqlParameter.MySqlDbType = MySqlDbType.Int16;
					return;
				case DbTypeEnum.Int32:
					sqlParameter.MySqlDbType = MySqlDbType.Int32;
					return;
				case DbTypeEnum.Int64:
					sqlParameter.MySqlDbType = MySqlDbType.Int64;
					return;
				case DbTypeEnum.Image:
					sqlParameter.MySqlDbType = MySqlDbType.LongBlob;
					return;
				case DbTypeEnum.NText:
					sqlParameter.MySqlDbType = MySqlDbType.LongText;
					sqlParameter.Size = -1;
					return;
				case DbTypeEnum.Text:
					sqlParameter.MySqlDbType = MySqlDbType.LongText;
					return;
				case DbTypeEnum.NChar:
					sqlParameter.MySqlDbType = MySqlDbType.VarChar;
					return;
				case DbTypeEnum.NVarChar:
					sqlParameter.MySqlDbType = MySqlDbType.VarChar;
					return;
				case DbTypeEnum.Single:
					sqlParameter.MySqlDbType = MySqlDbType.Float;
					return;
				case DbTypeEnum.Time:
					sqlParameter.MySqlDbType = MySqlDbType.Time;
					return;
				case DbTypeEnum.Timestamp:
					sqlParameter.MySqlDbType = MySqlDbType.Timestamp;
					sqlParameter.Size = 3;
					return;
				case DbTypeEnum.VarBinary:
					sqlParameter.MySqlDbType = MySqlDbType.VarBinary;
					return;
				case DbTypeEnum.VarChar:
					sqlParameter.MySqlDbType = MySqlDbType.VarChar;
					return;
			}
			throw new ParameterDbTypeException("Access_ParameterType_UnSupport", typeof(DbTypeEnum), dbType.ToString("D"));
		}

	}
}
