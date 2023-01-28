using System.Collections.Generic;
using System.Text;
using Basic.Collections;
using Basic.Database;
using Basic.Designer;
using Basic.Enums;
using MySql.Data.MySqlClient;

namespace Basic.DataContexts
{
	/// <summary>
	/// MySql 数据上下文接口
	/// </summary>
	public sealed class MySqlDataContext : IDataContext
	{
		private readonly MySqlCommand myCommand;
		private readonly MySqlConnection myConnection;
		internal MySqlDataContext(string connectionString)
		{
			myConnection = new MySqlConnection(connectionString);
			myConnection.Open();
			myCommand = new MySqlCommand(string.Empty, myConnection);
		}

		/// <summary>获取数据库系统中所有表类型的对象（含Table、View、 Table Function）</summary>
		/// <param name="tables">需要填充的数据表集合，</param>
		/// <param name="objectType">需要填充的表对象类型</param>
		public void GetTableObjects(TableDesignerCollection tables, ObjectTypeEnum objectType)
		{
			if ((ObjectTypeEnum.UserTable & objectType) == ObjectTypeEnum.UserTable)
			{
				GetTableObject(tables);
			}
			if ((ObjectTypeEnum.UserView & objectType) == ObjectTypeEnum.UserView)
			{
				GetViewObject(tables);
			}
		}

		/// <summary>
		/// 获取表对象
		/// </summary>
		/// <param name="tableList"></param>
		private void GetTableObject(TableDesignerCollection tables)
		{
			myCommand.CommandText = string.Format(@"SELECT TABLE_NAME AS NAME,TABLE_COMMENT AS VALUE 
FROM information_schema.TABLES 
WHERE TABLE_SCHEMA='{0}' AND TABLE_TYPE='BASE TABLE'", myConnection.Database);
			using (MySqlDataReader reader = myCommand.ExecuteReader())
			{
				int nameIndex = reader.GetOrdinal("NAME");
				int valueIndex = reader.GetOrdinal("VALUE");
				while (reader.Read())
				{
					TableDesignerInfo info = tables.CreateTable();
					info.Name = reader.GetString(nameIndex);
					if (!reader.IsDBNull(valueIndex))
						info.Common = reader.GetString(valueIndex);
					info.ObjectType = ObjectTypeEnum.UserTable;
					tables.Add(info);
				}
			}
		}

		/// <summary>
		/// 获取表对象
		/// </summary>
		/// <param name="tableList"></param>
		private void GetViewObject(TableDesignerCollection tables)
		{
			myCommand.CommandText = string.Format(@"SELECT TABLE_NAME AS NAME,TABLE_COMMENT AS VALUE 
FROM information_schema.TABLES 
WHERE TABLE_SCHEMA='{0}' AND TABLE_TYPE='VIEW'", myConnection.Database);
			using (MySqlDataReader reader = myCommand.ExecuteReader())
			{
				int nameIndex = reader.GetOrdinal("NAME");
				int valueIndex = reader.GetOrdinal("VALUE");
				while (reader.Read())
				{
					TableDesignerInfo info = tables.CreateTable();
					info.Name = reader.GetString(nameIndex);
					if (!reader.IsDBNull(valueIndex))
						info.Common = reader.GetString(valueIndex);
					info.ObjectType = ObjectTypeEnum.UserTable;
					tables.Add(info);
				}
			}
		}

		/// <summary>
		/// 获取数据库中所有用户表
		/// </summary>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public DesignTableInfo[] GetTables(ObjectTypeEnum objectType)
		{
			List<DesignTableInfo> tables = new List<DesignTableInfo>();
			myCommand.CommandText = string.Format(@"SELECT TABLE_NAME AS NAME,TABLE_COMMENT AS VALUE 
FROM information_schema.TABLES 
WHERE TABLE_TYPE='BASE TABLE' AND TABLE_SCHEMA='{0}'", myConnection.Database);
			using (MySqlDataReader reader = myCommand.ExecuteReader())
			{
				int nameIndex = reader.GetOrdinal("NAME");
				int valueIndex = reader.GetOrdinal("VALUE");
				while (reader.Read())
				{
					DesignTableInfo info = new DesignTableInfo(null);
					info.TableName = reader.GetString(nameIndex);
					if (!reader.IsDBNull(valueIndex))
						info.Description = reader.GetString(valueIndex);
					info.ObjectType = ObjectTypeEnum.UserTable;
					if (info.TableName != null && info.TableName.IndexOf('_') >= 0)
						info.EntityName = StringHelper.GetPascalCase(info.TableName.Substring(info.TableName.IndexOf('_') + 1,
							info.TableName.Length - info.TableName.IndexOf('_') - 1));
					else
						info.EntityName = StringHelper.GetPascalCase(info.TableName);
					tables.Add(info);
				}
			}
			return tables.ToArray();
		}

		/// <summary>
		/// 获取表或视图的列信息
		/// </summary>
		/// <param name="tableInfo">表或视图名称。</param>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public void GetColumns(DesignTableInfo tableInfo)
		{
			myCommand.CommandText = string.Format(@"SELECT COLUMN_NAME,DATA_TYPE AS TYPE_NAME,IFNULL(NUMERIC_PRECISION, 0) AS `PRECISION`, 
 IFNULL(CHARACTER_MAXIMUM_LENGTH,0) AS LENGTH,IFNULL(NUMERIC_SCALE, 0) AS SCALE,CASE IS_NULLABLE WHEN 'YES' THEN 1 ELSE 0 END AS NULLABLE,
COLUMN_DEFAULT, ORDINAL_POSITION,COLUMN_COMMENT FROM information_schema.COLUMNS WHERE TABLE_NAME = '{0}'", tableInfo.TableName);
			DesignColumnCollection listColumn = tableInfo.Columns;
			#region 获取数据库表列信息
			using (MySqlDataReader reader = myCommand.ExecuteReader())
			{

				int nameIndex = reader.GetOrdinal("COLUMN_NAME");
				int typeIndex = reader.GetOrdinal("TYPE_NAME");
				int precIndex = reader.GetOrdinal("PRECISION");
				int sizeIndex = reader.GetOrdinal("LENGTH");
				int scaleIndex = reader.GetOrdinal("SCALE");
				int nullIndex = reader.GetOrdinal("NULLABLE");
				int defaultIndex = reader.GetOrdinal("COLUMN_DEFAULT");
				int oradinalIndex = reader.GetOrdinal("ORDINAL_POSITION");
				int commentIndex = reader.GetOrdinal("COLUMN_COMMENT");
				while (reader.Read())
				{
					string name = reader.GetString(nameIndex);
					if (!listColumn.TryGetValue(name, out DesignColumnInfo info))
					{
						info = listColumn.CreateColumn();
						info.Name = reader.GetString(nameIndex);
						info.PropertyName = StringHelper.GetPascalCase(info.Name);
						listColumn.Add(info);
					}
					string typeName = reader.GetString(typeIndex).ToUpper();
					info.DbType = GetUserDbType(typeName);
					string upperName = info.Name.ToUpper();
					if (info.DbType == DbTypeEnum.DateTime && upperName == DesignTableInfo.ModifiedTimeColumn)
						info.DbType = DbTypeEnum.Timestamp;
					else if (info.DbType == DbTypeEnum.DateTime && upperName == DesignTableInfo.ModifyTimeColumn)
						info.DbType = DbTypeEnum.Timestamp;
					info.Size = 0;
					switch (info.DbType)
					{
						case DbTypeEnum.Char:
						case DbTypeEnum.VarChar:
						case DbTypeEnum.NChar:
						case DbTypeEnum.NVarChar:
						case DbTypeEnum.Binary:
						case DbTypeEnum.VarBinary:
							info.Size = reader.GetInt32(precIndex);
							break;
						case DbTypeEnum.Decimal:
							info.Precision = (byte)reader.GetInt32(precIndex);
							if (!reader.IsDBNull(scaleIndex))
								info.Scale = (byte)reader.GetInt16(scaleIndex);
							break;
					}
					info.Nullable = reader.GetInt16(nullIndex) > 0;
					if (!reader.IsDBNull(defaultIndex))
					{
						info.DefaultValue = reader.GetString(defaultIndex).ToUpper();
						info.DefaultValue = info.DefaultValue.Trim(new char[] { '(', ')' });
						if (!string.IsNullOrWhiteSpace(info.DefaultValue) && info.DefaultValue.StartsWith("("))
							info.DefaultValue = info.DefaultValue.TrimStart('(');
						if (!string.IsNullOrWhiteSpace(info.DefaultValue) && info.DefaultValue.EndsWith(")"))
							info.DefaultValue = info.DefaultValue.TrimEnd(')');
						if (!string.IsNullOrWhiteSpace(info.DefaultValue) && info.DefaultValue.EndsWith("("))
							info.DefaultValue = string.Concat(info.DefaultValue, ")");
					}
					if (reader.IsDBNull(commentIndex) == false)
					{
						info.Comment = reader.GetString(commentIndex);
					}
				}
			}
			#endregion

			#region 数据库主键信息描述
			UniqueConstraint pkeyConstraint = tableInfo.PrimaryKey;
			pkeyConstraint.Columns.Clear();
			myCommand.CommandText = string.Format(@"SELECT CONSTRAINT_SCHEMA,CONSTRAINT_NAME,TABLE_SCHEMA,TABLE_NAME,COLUMN_NAME,ORDINAL_POSITION
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = '{0}'", tableInfo.TableName);
			using (MySqlDataReader reader2 = myCommand.ExecuteReader())
			{
				if (reader2.HasRows)
				{
					int pkNameIndex = reader2.GetOrdinal("TABLE_NAME");
					int columnNameIndex = reader2.GetOrdinal("COLUMN_NAME");
					while (reader2.Read() && reader2.HasRows)
					{
						pkeyConstraint.Name = string.Concat("PK_", reader2.GetString(pkNameIndex));
						string columnName = reader2.GetString(columnNameIndex);
						if (listColumn.ContainsKey(columnName))
						{
							DesignColumnInfo columnInfo = listColumn[columnName];
							columnInfo.PrimaryKey = true;
							pkeyConstraint.Columns.Add(columnInfo);
						}
					}
				}
			}
			#endregion
		}

		/// <summary>
		///  将数据库类型转换成SqlDbType枚举类型
		/// </summary>
		/// <param name="type">数据库类型</param>
		/// <returns>返回转换成功的SqlDbType枚举类型</returns>
		private DbTypeEnum GetUserDbType(string type)
		{
			if (type == "BINARY")
				return DbTypeEnum.Binary;
			else if (type == "INT" || type == "INTEGER")
				return DbTypeEnum.Int32;
			else if (type == "BIGINT")
				return DbTypeEnum.Int64;
			else if (type == "NVARCHAR" || type == "SYSNAME")
				return DbTypeEnum.NVarChar;
			else if (type == "NCHAR")
				return DbTypeEnum.NChar;
			else if (type == "VARCHAR")
				return DbTypeEnum.VarChar;
			else if (type == "CHAR")
				return DbTypeEnum.Char;
			else if (type == "TIME")
				return DbTypeEnum.Time;
			else if (type == "DATE")
				return DbTypeEnum.Date;
			else if (type == "DATETIME")
				return DbTypeEnum.DateTime;
			else if (type == "DATETIME2" || type == "TIMESTAMP")
				return DbTypeEnum.Timestamp;
			else if (type == "DATETIMEOFFSET")
				return DbTypeEnum.DateTimeOffset;
			else if (type == "SMALLINT")
				return DbTypeEnum.Int16;
			else if (type == "DECIMAL")
				return DbTypeEnum.Decimal;
			else if (type == "REAL")
				return DbTypeEnum.Single;
			else if (type == "UNIQUEIDENTIFIER")
				return DbTypeEnum.Guid;
			else if (type == "VARBINARY")
				return DbTypeEnum.VarBinary;
			else if (type == "IMAGE")
				return DbTypeEnum.Image;
			else if (type == "NTEXT")
				return DbTypeEnum.NText;
			else if (type == "TEXT")
				return DbTypeEnum.Text;
			else if (type == "FLOAT")
				return DbTypeEnum.Double;
			else if (type == "BIT")
				return DbTypeEnum.Boolean;
			return DbTypeEnum.Int32;
		}

		/// <summary>
		/// 获取表或视图的列信息
		/// </summary>
		/// <param name="tableInfo">表或视图名称。</param>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public void GetParameters(TableFunctionInfo tableInfo) { }

		/// <summary>
		/// 获取数据库中所有存储过程
		/// </summary>
		/// <returns>获取获取的存储过程列表。</returns>
		public StoreProcedure[] GetProcedures() { return null; }

		/// <summary>
		/// 获取存储过程参数列表
		/// </summary>
		/// <param name="procedure">存储过程信息</param>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public bool GetParameters(StoreProcedure procedure) { return true; }

		/// <summary>
		/// 获取存储过程返回结果列信息
		/// </summary>
		/// <param name="procedure">存储过程信息</param>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public bool GetColumns(StoreProcedure procedure)
		{
			return true;
		}

		/// <summary>根据自定义 Trancate-SQL 查询表结构信息。</summary>
		/// <param name="tableCollection">查询表结构定义</param>
		/// <param name="trancateSql">需要查询的 Trancate-Sql 实例。</param>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public bool GetTransactSql(TransactTableCollection tableCollection, string trancateSql)
		{
			return true;
		}

		/// <summary>获取函数的参数信息</summary>
		/// <param name="tableCollection">查询表结构定义。</param>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public void GetParameters(TransactTableCollection tableCollection)
		{

		}

		/// <summary>
		/// 释放数据库关键字
		/// </summary>
		public void Dispose()
		{
			myConnection.Close();
			myConnection.Dispose();
			myCommand.Dispose();
		}
	}
}
