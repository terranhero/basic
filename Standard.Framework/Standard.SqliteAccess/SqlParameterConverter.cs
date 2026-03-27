using System.Data;
using Basic.Enums;
using Basic.Exceptions;
using Microsoft.Data.Sqlite;

namespace Basic.SqliteAccess
{
	internal static class SqlParameterConverter
	{

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="sqlParameter">数据库命令执行的参数</param>
		/// <param name="dbType">SqlServer数据库列类型,SqliteType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		public static void ConvertSqlParameterType(SqliteParameter sqlParameter, DataTypeEnum dbType, byte precision, byte scale)
		{
			switch (dbType)
			{
				case DataTypeEnum.Boolean:
					sqlParameter.SqliteType = SqliteType.Integer;
					break;
				case DataTypeEnum.Binary:
					sqlParameter.SqliteType = SqliteType.Blob;
					break;
				case DataTypeEnum.Char:
					sqlParameter.SqliteType = SqliteType.Text;
					break;
				case DataTypeEnum.Date:
					sqlParameter.SqliteType = SqliteType.Text;
					break;
				case DataTypeEnum.DateTime:
					sqlParameter.SqliteType = SqliteType.Text;
					break;
				case DataTypeEnum.DateTime2:
					sqlParameter.SqliteType = SqliteType.Text;
					break;
				case DataTypeEnum.DateTimeOffset:
					sqlParameter.SqliteType = SqliteType.Text;
					break;
				case DataTypeEnum.Decimal:
					sqlParameter.SqliteType = SqliteType.Real;
					sqlParameter.Precision = precision;
					sqlParameter.Scale = scale;
					break;
				case DataTypeEnum.Double:
					sqlParameter.SqliteType = SqliteType.Real;
					break;
				case DataTypeEnum.Guid:
					sqlParameter.SqliteType = SqliteType.Text;
					break;
				case DataTypeEnum.Int16:
					sqlParameter.SqliteType = SqliteType.Integer;
					break;
				case DataTypeEnum.Int32:
					sqlParameter.SqliteType = SqliteType.Integer;
					break;
				case DataTypeEnum.Int64:
					sqlParameter.SqliteType = SqliteType.Integer;
					break;
				case DataTypeEnum.Image:
					sqlParameter.SqliteType = SqliteType.Blob;
					sqlParameter.Size = -1;
					break;
				case DataTypeEnum.NText:
					sqlParameter.SqliteType = SqliteType.Text;
					sqlParameter.Size = -1;
					break;
				case DataTypeEnum.Text:
					sqlParameter.SqliteType = SqliteType.Text;
					sqlParameter.Size = -1;
					break;
				case DataTypeEnum.NChar:
					sqlParameter.SqliteType = SqliteType.Text;
					break;
				case DataTypeEnum.NVarChar:
					sqlParameter.SqliteType = SqliteType.Text;
					break;
				case DataTypeEnum.Single:
					sqlParameter.SqliteType = SqliteType.Real;
					break;
				case DataTypeEnum.Time:
					sqlParameter.SqliteType = SqliteType.Text;
					break;
				case DataTypeEnum.Timestamp:
					sqlParameter.SqliteType = SqliteType.Text;
					//sqlParameter.Size = 3;
					break;
				case DataTypeEnum.VarBinary:
					sqlParameter.SqliteType = SqliteType.Blob;
					break;
				case DataTypeEnum.VarChar:
					sqlParameter.SqliteType = SqliteType.Text;
					break;
				default:
					throw new ParameterDbTypeException("Access_ParameterType_UnSupport", typeof(DataTypeEnum), dbType.ToString("D"));
			}
		}

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="sqlParameter">数据库命令执行的参数</param>
		/// <param name="dbType">SqlServer数据库列类型,SqliteType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		public static void ConvertSqlParameterType(SqliteParameter sqlParameter, DbTypeEnum dbType, byte precision, byte scale)
		{
			switch (dbType)
			{
				case DbTypeEnum.Boolean:
					sqlParameter.SqliteType = SqliteType.Integer;
					return;
				case DbTypeEnum.Binary:
					sqlParameter.SqliteType = SqliteType.Blob;
					return;
				case DbTypeEnum.Char:
					sqlParameter.SqliteType = SqliteType.Text;
					return;
				case DbTypeEnum.Date:
					sqlParameter.SqliteType = SqliteType.Text;
					return;
				case DbTypeEnum.DateTime:
					sqlParameter.SqliteType = SqliteType.Text;
					return;
				case DbTypeEnum.DateTime2:
					sqlParameter.SqliteType = SqliteType.Text;
					return;
				case DbTypeEnum.DateTimeOffset:
					sqlParameter.SqliteType = SqliteType.Text;
					return;
				case DbTypeEnum.Decimal:
					sqlParameter.SqliteType = SqliteType.Real;
					sqlParameter.Precision = precision;
					sqlParameter.Scale = scale;
					return;
				case DbTypeEnum.Double:
					sqlParameter.SqliteType = SqliteType.Real;
					return;
				case DbTypeEnum.Guid:
					sqlParameter.SqliteType = SqliteType.Text;
					return;
				case DbTypeEnum.Int16:
					sqlParameter.SqliteType = SqliteType.Integer;
					return;
				case DbTypeEnum.Int32:
					sqlParameter.SqliteType = SqliteType.Integer;
					return;
				case DbTypeEnum.Int64:
					sqlParameter.SqliteType = SqliteType.Integer;
					return;
				case DbTypeEnum.Image:
					sqlParameter.SqliteType = SqliteType.Text;
					sqlParameter.Size = -1;
					return;
				case DbTypeEnum.NText:
					sqlParameter.SqliteType = SqliteType.Text;
					sqlParameter.Size = -1;
					return;
				case DbTypeEnum.Text:
					sqlParameter.SqliteType = SqliteType.Text;
					sqlParameter.Size = -1;
					return;
				case DbTypeEnum.NChar:
					sqlParameter.SqliteType = SqliteType.Text;
					return;
				case DbTypeEnum.NVarChar:
					sqlParameter.SqliteType = SqliteType.Text;
					return;
				case DbTypeEnum.Single:
					sqlParameter.SqliteType = SqliteType.Real;
					return;
				case DbTypeEnum.Time:
					sqlParameter.SqliteType = SqliteType.Text;
					return;
				case DbTypeEnum.Timestamp:
					sqlParameter.SqliteType = SqliteType.Text;
					//sqlParameter.Size = 3;
					return;
				case DbTypeEnum.VarBinary:
					sqlParameter.SqliteType = SqliteType.Blob;
					return;
				case DbTypeEnum.VarChar:
					sqlParameter.SqliteType = SqliteType.Text;
					return;
			}
			throw new ParameterDbTypeException("Access_ParameterType_UnSupport", typeof(DbTypeEnum), dbType.ToString("D"));
		}

	}
}
