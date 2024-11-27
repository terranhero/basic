using Basic.Enums;
using Basic.Exceptions;
using Npgsql;
using NpgsqlTypes;

namespace Basic.PostgreSql
{
	internal static class NpgParameterConverter
	{

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="parameter">数据库命令执行的参数</param>
		/// <param name="dbType">Npg数据库列类型,NpgsqlDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		public static void ConvertSqlParameterType(NpgsqlParameter parameter, DataTypeEnum dbType, byte precision, byte scale)
		{
			switch (dbType)
			{
				case DataTypeEnum.Boolean:
					parameter.NpgsqlDbType = NpgsqlDbType.Bit;
					break;
				case DataTypeEnum.Binary:
					parameter.NpgsqlDbType = NpgsqlDbType.Bytea;
					break;
				case DataTypeEnum.Char:
					parameter.NpgsqlDbType = NpgsqlDbType.Char;
					break;
				case DataTypeEnum.Date:
					parameter.NpgsqlDbType = NpgsqlDbType.Date;
					break;
				case DataTypeEnum.DateTime:
					parameter.NpgsqlDbType = NpgsqlDbType.Timestamp;
					break;
				case DataTypeEnum.DateTime2:
					parameter.NpgsqlDbType = NpgsqlDbType.Timestamp;
					break;
				case DataTypeEnum.DateTimeOffset:
					parameter.NpgsqlDbType = NpgsqlDbType.TimestampTz;
					break;
				case DataTypeEnum.Decimal:
					parameter.NpgsqlDbType = NpgsqlDbType.Numeric;
					parameter.Precision = precision;
					parameter.Scale = scale;
					break;
				case DataTypeEnum.Double:
					parameter.NpgsqlDbType = NpgsqlDbType.Double;
					break;
				case DataTypeEnum.Guid:
					parameter.NpgsqlDbType = NpgsqlDbType.Uuid;
					break;
				case DataTypeEnum.Int16:
					parameter.NpgsqlDbType = NpgsqlDbType.Smallint;
					break;
				case DataTypeEnum.Int32:
					parameter.NpgsqlDbType = NpgsqlDbType.Integer;
					break;
				case DataTypeEnum.Int64:
					parameter.NpgsqlDbType = NpgsqlDbType.Bigint;
					break;
				case DataTypeEnum.Image:
					parameter.NpgsqlDbType = NpgsqlDbType.Bytea;
					parameter.Size = -1;
					break;
				case DataTypeEnum.NText:
					parameter.NpgsqlDbType = NpgsqlDbType.Citext;
					parameter.Size = -1;
					break;
				case DataTypeEnum.Text:
					parameter.NpgsqlDbType = NpgsqlDbType.Text;
					parameter.Size = -1;
					break;
				case DataTypeEnum.NChar:
					parameter.NpgsqlDbType = NpgsqlDbType.Char;
					break;
				case DataTypeEnum.NVarChar:
					parameter.NpgsqlDbType = NpgsqlDbType.Varchar;
					break;
				case DataTypeEnum.Single:
					parameter.NpgsqlDbType = NpgsqlDbType.Real;
					break;
				case DataTypeEnum.Time:
					parameter.NpgsqlDbType = NpgsqlDbType.Time;
					break;
				case DataTypeEnum.Timestamp:
					parameter.NpgsqlDbType = NpgsqlDbType.Timestamp;
					//sqlParameter.Size = 3;
					break;
				case DataTypeEnum.VarBinary:
					parameter.NpgsqlDbType = NpgsqlDbType.Bytea;
					break;
				case DataTypeEnum.VarChar:
					parameter.NpgsqlDbType = NpgsqlDbType.Varchar;
					break;
				default:
					throw new ParameterDbTypeException("Access_ParameterType_UnSupport", typeof(DataTypeEnum), dbType.ToString("D"));
			}
		}

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="parameter">数据库命令执行的参数</param>
		/// <param name="dbType">Npg数据库列类型,NpgsqlDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		public static void ConvertSqlParameterType(NpgsqlParameter parameter, DbTypeEnum dbType, byte precision, byte scale)
		{
			switch (dbType)
			{
				case DbTypeEnum.Boolean:
					parameter.NpgsqlDbType = NpgsqlDbType.Bit;
					return;
				case DbTypeEnum.Binary:
					parameter.NpgsqlDbType = NpgsqlDbType.Bytea;
					return;
				case DbTypeEnum.Char:
					parameter.NpgsqlDbType = NpgsqlDbType.Char;
					return;
				case DbTypeEnum.Date:
					parameter.NpgsqlDbType = NpgsqlDbType.Date;
					return;
				case DbTypeEnum.DateTime:
					parameter.NpgsqlDbType = NpgsqlDbType.Timestamp;
					return;
				case DbTypeEnum.DateTime2:
					parameter.NpgsqlDbType = NpgsqlDbType.Timestamp;
					return;
				case DbTypeEnum.DateTimeOffset:
					parameter.NpgsqlDbType = NpgsqlDbType.TimestampTz;
					return;
				case DbTypeEnum.Decimal:
					parameter.NpgsqlDbType = NpgsqlDbType.Numeric;
					parameter.Precision = precision;
					parameter.Scale = scale;
					return;
				case DbTypeEnum.Double:
					parameter.NpgsqlDbType = NpgsqlDbType.Double;
					return;
				case DbTypeEnum.Guid:
					parameter.NpgsqlDbType = NpgsqlDbType.Uuid;
					return;
				case DbTypeEnum.Int16:
					parameter.NpgsqlDbType = NpgsqlDbType.Smallint;
					return;
				case DbTypeEnum.Int32:
					parameter.NpgsqlDbType = NpgsqlDbType.Integer;
					return;
				case DbTypeEnum.Int64:
					parameter.NpgsqlDbType = NpgsqlDbType.Bigint;
					return;
				case DbTypeEnum.Image:
					parameter.NpgsqlDbType = NpgsqlDbType.Bytea;
					parameter.Size = -1;
					return;
				case DbTypeEnum.NText:
					parameter.NpgsqlDbType = NpgsqlDbType.Citext;
					parameter.Size = -1;
					return;
				case DbTypeEnum.Text:
					parameter.NpgsqlDbType = NpgsqlDbType.Text;
					parameter.Size = -1;
					return;
				case DbTypeEnum.NChar:
					parameter.NpgsqlDbType = NpgsqlDbType.Char;
					return;
				case DbTypeEnum.NVarChar:
					parameter.NpgsqlDbType = NpgsqlDbType.Varchar;
					return;
				case DbTypeEnum.Single:
					parameter.NpgsqlDbType = NpgsqlDbType.Real;
					return;
				case DbTypeEnum.Time:
					parameter.NpgsqlDbType = NpgsqlDbType.Time;
					return;
				case DbTypeEnum.Timestamp:
					parameter.NpgsqlDbType = NpgsqlDbType.Timestamp;
					//sqlParameter.Size = 3;
					return;
				case DbTypeEnum.VarBinary:
					parameter.NpgsqlDbType = NpgsqlDbType.Bytea;
					return;
				case DbTypeEnum.VarChar:
					parameter.NpgsqlDbType = NpgsqlDbType.Varchar;
					return;
			}
			throw new ParameterDbTypeException("Access_ParameterType_UnSupport", typeof(DbTypeEnum), dbType.ToString("D"));
		}

	}
}
