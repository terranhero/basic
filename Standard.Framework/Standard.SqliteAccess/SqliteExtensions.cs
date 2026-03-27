using System;
using Microsoft.Data.Sqlite;

namespace Basic.SqliteAccess
{

    internal static class SqliteExtensions
	{
		public static SqliteCommand Clone(this SqliteCommand source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			// 创建一个新的命令，并关联到同一个连接（注意：连接可能处于不同状态）
			SqliteCommand clone = new SqliteCommand
			{
				Connection = source.Connection,
				CommandText = source.CommandText,
				CommandType = source.CommandType,
				CommandTimeout = source.CommandTimeout,
				Transaction = source.Transaction  // 如果存在事务，需要关联到同一个事务
			};

			// 复制参数（深拷贝参数值）
			foreach (SqliteParameter param in source.Parameters)
			{
				// 创建一个新的参数，复制所有属性
				SqliteParameter newParam = new SqliteParameter(param.ParameterName, param.SqliteType)
				{
					Value = param.Value,
					Direction = param.Direction,
					Size = param.Size,
					Precision = param.Precision,
					Scale = param.Scale,
					SourceColumn = param.SourceColumn,
					SourceColumnNullMapping = param.SourceColumnNullMapping,
					SourceVersion = param.SourceVersion
				};
				clone.Parameters.Add(newParam);
			}

			return clone;
		}
	}
}
