using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System;
using Basic.Database;
using Basic.Enums;
using Basic.Collections;
using Basic.Designer;
using UniqueConstraint = Basic.Database.UniqueConstraint;
using System.Data.Common;

namespace Basic.DataContexts
{
	/// <summary>
	/// SQL Server数据上下文接口
	/// </summary>
	public sealed class SqlServerDataContext : IDataContext
	{
		private readonly SqlCommand sqlCommand;
		private readonly SqlConnection sqlConnection;
		internal SqlServerDataContext(string connectionString)
		{
			sqlConnection = new SqlConnection(connectionString);
			sqlConnection.Open();
			sqlCommand = new SqlCommand(string.Empty, sqlConnection);
		}

		/// <summary>获取特定数据库参数带符号的名称</summary>
		/// <param name="parameterName">不带参数符号的参数名称</param>
		/// <returns>返回特定数据库参数带符号的名称。</returns>
		public string GetParameterName(string parameterName) { return string.Concat("@", parameterName); }

		/// <summary>
		/// 获取数据库系统中所有表类型的对象（含Table、View、 Table Function）
		/// </summary>
		/// <returns></returns>
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
			if ((ObjectTypeEnum.ClrTableFunction & objectType) == ObjectTypeEnum.ClrTableFunction)
			{
				GetClrTableFunction(tables);
			}
			if ((ObjectTypeEnum.SqlTableFunction & objectType) == ObjectTypeEnum.SqlTableFunction)
			{
				GetSqlTableFunction(tables);
			}
			if ((ObjectTypeEnum.InlineTableFunction & objectType) == ObjectTypeEnum.InlineTableFunction)
			{
				GetInlineTableFunction(tables);
			}
		}

		/// <summary>
		/// 获取表对象
		/// </summary>
		/// <param name="tableList"></param>
		private void GetTableObject(TableDesignerCollection tables)
		{
			sqlCommand.CommandText = @"SELECT t1.object_id as [OBJECT_ID],t2.name as [OWNER],t1.name as [NAME],t3.value as VALUE
FROM sys.all_objects t1 join sys.schemas t2 ON t1.schema_id=t2.schema_id 
LEFT JOIN sys.extended_properties t3 ON t1.object_id=t3.major_id and t3.minor_id=0
WHERE t1.type='U' AND t1.name<>'sysdiagrams'
ORDER BY name";
			using (SqlDataReader reader = sqlCommand.ExecuteReader())
			{
				int idIndex = reader.GetOrdinal("OBJECT_ID");
				int ownerIndex = reader.GetOrdinal("OWNER");
				int nameIndex = reader.GetOrdinal("NAME");
				int valueIndex = reader.GetOrdinal("VALUE");
				while (reader.Read())
				{
					TableDesignerInfo info = tables.CreateTable();
					info.ObjectId = reader.GetInt32(idIndex);
					info.Owner = reader.GetString(ownerIndex);
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
		private void GetViewObject(TableDesignerCollection tableList)
		{
			sqlCommand.CommandText = @"SELECT t1.object_id as [OBJECT_ID],t2.name as [OWNER],t1.name as [NAME]
FROM sys.all_objects t1 join sys.schemas t2 ON t1.schema_id=t2.schema_id 
WHERE t1.type='V' AND t1.object_id>0 ORDER BY name";
			using (SqlDataReader reader = sqlCommand.ExecuteReader())
			{
				int idIndex = reader.GetOrdinal("OBJECT_ID");
				int ownerIndex = reader.GetOrdinal("OWNER");
				int nameIndex = reader.GetOrdinal("NAME");
				while (reader.Read())
				{
					TableDesignerInfo info = tableList.CreateTable();
					info.ObjectId = reader.GetInt32(idIndex);
					info.Owner = reader.GetString(ownerIndex);
					info.Name = reader.GetString(nameIndex);
					info.ObjectType = ObjectTypeEnum.UserView;
					tableList.Add(info);
				}
			}
		}

		/// <summary>
		/// 获取表对象
		/// </summary>
		/// <param name="tableList"></param>
		private void GetClrTableFunction(TableDesignerCollection tableList)
		{
			sqlCommand.CommandText = @"SELECT t1.object_id as [OBJECT_ID],t2.name as [OWNER],t1.name as [NAME]
FROM sys.all_objects t1 join sys.schemas t2 ON t1.schema_id=t2.schema_id 
WHERE t1.type='FT' AND t1.object_id>0 ORDER BY name";
			using (SqlDataReader reader = sqlCommand.ExecuteReader())
			{
				int idIndex = reader.GetOrdinal("OBJECT_ID");
				int ownerIndex = reader.GetOrdinal("OWNER");
				int nameIndex = reader.GetOrdinal("NAME");
				while (reader.Read())
				{
					TableFunctionInfo info = tableList.CreateTableFunction();
					info.ObjectId = reader.GetInt32(idIndex);
					info.Owner = reader.GetString(ownerIndex);
					info.Name = reader.GetString(nameIndex);
					info.ObjectType = ObjectTypeEnum.ClrTableFunction;
					tableList.Add(info);
				}
			}
		}

		/// <summary>
		/// 获取表对象
		/// </summary>
		/// <param name="tableList"></param>
		private void GetSqlTableFunction(TableDesignerCollection tableList)
		{
			sqlCommand.CommandText = @"SELECT t1.object_id as [OBJECT_ID],t2.name as [OWNER],t1.name as [NAME]
FROM sys.all_objects t1 join sys.schemas t2 ON t1.schema_id=t2.schema_id 
WHERE t1.type='TF' AND t1.object_id>0 ORDER BY name";
			using (SqlDataReader reader = sqlCommand.ExecuteReader())
			{
				int idIndex = reader.GetOrdinal("OBJECT_ID");
				int ownerIndex = reader.GetOrdinal("OWNER");
				int nameIndex = reader.GetOrdinal("NAME");
				while (reader.Read())
				{
					TableFunctionInfo info = tableList.CreateTableFunction();
					info.ObjectId = reader.GetInt32(idIndex);
					info.Owner = reader.GetString(ownerIndex);
					info.Name = reader.GetString(nameIndex);
					info.ObjectType = ObjectTypeEnum.SqlTableFunction;
					tableList.Add(info);
				}
			}
		}

		/// <summary>
		/// 获取表对象
		/// </summary>
		/// <param name="tableList"></param>
		private void GetInlineTableFunction(TableDesignerCollection tableList)
		{
			sqlCommand.CommandText = @"SELECT t1.object_id as [OBJECT_ID],t2.name as [OWNER],t1.name as [NAME]
FROM sys.all_objects t1 join sys.schemas t2 ON t1.schema_id=t2.schema_id 
WHERE t1.type='IF' AND t1.object_id>0 ORDER BY name";
			using (SqlDataReader reader = sqlCommand.ExecuteReader())
			{
				int idIndex = reader.GetOrdinal("OBJECT_ID");
				int ownerIndex = reader.GetOrdinal("OWNER");
				int nameIndex = reader.GetOrdinal("NAME");
				while (reader.Read())
				{
					TableFunctionInfo info = tableList.CreateTableFunction();
					info.ObjectId = reader.GetInt32(idIndex);
					info.Owner = reader.GetString(ownerIndex);
					info.Name = reader.GetString(nameIndex);
					info.ObjectType = ObjectTypeEnum.InlineTableFunction;
					tableList.Add(info);
				}
			}
		}

		/// <summary>
		/// 获取表或视图的列信息
		/// </summary>
		/// <param name="tableInfo">表或视图名称。</param>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public void GetColumns(TableDesignerInfo tableInfo)
		{
			sqlCommand.CommandText = string.Format(@"SELECT t1.object_id,t1.name as COLUMN_NAME,t3.name as TYPE_NAME,
CASE WHEN t1.system_type_id iN(231,239,99) then t1.max_length/2 ELSE t1.PRECISION END as PRECISION,
t1.max_length as LENGTH,t1.SCALE,t1.is_nullable as NULLABLE,t2.definition as COLUMN_DEF,
t1.column_id as ORDINAL_POSITION,t4.value AS COLUMN_DES
FROM sys.all_columns t1 left join sys.default_constraints t2 on t1.default_object_id=t2.object_id
join sys.types t3 on t1.user_type_id=t3.user_type_id
left join sys.extended_properties t4 ON t1.object_id=t4.major_id AND t1.column_id=t4.minor_id
where t1.object_id=object_id('{0}')", tableInfo.Name);
			ColumnDesignerCollection listColumn = tableInfo.Columns;
			listColumn.Clear();
			#region 获取数据库表列信息
			using (SqlDataReader reader = sqlCommand.ExecuteReader())
			{
				int nameIndex = reader.GetOrdinal("COLUMN_NAME");
				int typeIndex = reader.GetOrdinal("TYPE_NAME");
				int precIndex = reader.GetOrdinal("PRECISION");
				int sizeIndex = reader.GetOrdinal("LENGTH");
				int scaleIndex = reader.GetOrdinal("SCALE");
				int nullIndex = reader.GetOrdinal("NULLABLE");
				int defaultIndex = reader.GetOrdinal("COLUMN_DEF");
				int oradinalIndex = reader.GetOrdinal("ORDINAL_POSITION");
				int desIndex = reader.GetOrdinal("COLUMN_DES");
				while (reader.Read())
				{
					ColumnDesignerInfo info = listColumn.CreateColumn();
					info.Name = reader.GetString(nameIndex);
					info.PropertyName = StringHelper.GetPascalCase(info.Name);
					string typeName = reader.GetString(typeIndex).ToUpper();
					info.DbType = GetUserDbType(typeName);
					string upperName = info.Name.ToUpper();
					if (info.DbType == DbTypeEnum.DateTime && upperName == TableDesignerInfo.ModifiedTimeColumn)
						info.DbType = DbTypeEnum.Timestamp;
					else if (info.DbType == DbTypeEnum.DateTime && upperName == TableDesignerInfo.ModifyTimeColumn)
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
								info.Scale = reader.GetByte(scaleIndex);
							break;
					}
					info.Nullable = reader.GetBoolean(nullIndex);
					if (!reader.IsDBNull(defaultIndex))
					{
						info.DefaultValue = reader.GetString(defaultIndex).ToUpper();
						if (!string.IsNullOrWhiteSpace(info.DefaultValue) && info.DefaultValue.StartsWith("("))
							info.DefaultValue = info.DefaultValue.TrimStart('(');
						if (!string.IsNullOrWhiteSpace(info.DefaultValue) && info.DefaultValue.EndsWith(")"))
							info.DefaultValue = info.DefaultValue.TrimEnd(')');
						if (!string.IsNullOrWhiteSpace(info.DefaultValue) && info.DefaultValue.EndsWith("("))
							info.DefaultValue = string.Concat(info.DefaultValue, ")");
					}
					if (!reader.IsDBNull(desIndex))
						info.Comment = reader.GetString(desIndex);
					listColumn.Add(info);
				}
			}
			#endregion
		}

		/// <summary>
		/// 获取函数的参数信息
		/// </summary>
		/// <param name="tableInfo">表或视图名称。</param>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public void GetParameters(TableFunctionInfo tableInfo)
		{
			sqlCommand.CommandText = string.Format(@"SELECT t1.object_id,t1.name as COLUMN_NAME,t3.name as TYPE_NAME,
CASE WHEN t1.system_type_id iN(231,239,99) then t1.max_length/2 ELSE t1.PRECISION END as PRECISION,
t1.max_length as LENGTH,t1.SCALE,t1.parameter_id as ORDINAL_POSITION,t1.is_output as IS_OUTPUT
FROM sys.parameters t1 join sys.types t3 on t1.user_type_id=t3.user_type_id
where t1.object_id=object_id('{0}')", tableInfo.Name);
			FunctionParameterCollection parameters = tableInfo.Parameters;
			parameters.Clear();
			using (SqlDataReader reader = sqlCommand.ExecuteReader())
			{
				int nameIndex = reader.GetOrdinal("COLUMN_NAME");
				int typeIndex = reader.GetOrdinal("TYPE_NAME");
				int precIndex = reader.GetOrdinal("PRECISION");
				int sizeIndex = reader.GetOrdinal("LENGTH");
				int scaleIndex = reader.GetOrdinal("SCALE");
				int oradinalIndex = reader.GetOrdinal("ORDINAL_POSITION");
				while (reader.Read())
				{
					FunctionParameterInfo info = tableInfo.CreateParameter();
					info.Name = reader.GetString(nameIndex).Remove(0, 1);
					string typeName = reader.GetString(typeIndex).ToUpper();
					info.ParameterType = GetUserDbType(typeName);
					string upperName = info.Name.ToUpper();
					if (info.ParameterType == DbTypeEnum.DateTime && upperName == TableDesignerInfo.ModifiedTimeColumn)
						info.ParameterType = DbTypeEnum.Timestamp;
					else if (info.ParameterType == DbTypeEnum.DateTime && upperName == TableDesignerInfo.ModifyTimeColumn)
						info.ParameterType = DbTypeEnum.Timestamp;
					info.Size = 0;
					switch (info.ParameterType)
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
								info.Scale = reader.GetByte(scaleIndex);
							break;
					}
					info.Nullable = true;
					parameters.Add(info);
				}
			}
		}

		/// <summary>
		/// 获取数据库中所有用户表
		/// </summary>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public DesignTableInfo[] GetTables(ObjectTypeEnum objectType)
		{
			Dictionary<string, DesignTableInfo> tableList = new Dictionary<string, DesignTableInfo>(200);
			if ((ObjectTypeEnum.UserTable & objectType) == ObjectTypeEnum.UserTable)
			{
				sqlCommand.CommandText = "exec sp_Tables @table_type='''TABLE'''";
				using (SqlDataReader reader = sqlCommand.ExecuteReader())
				{
					int ownerIndex = reader.GetOrdinal("TABLE_OWNER");
					int nameIndex = reader.GetOrdinal("TABLE_NAME");
					while (reader.Read())
					{
						DesignTableInfo info = new DesignTableInfo(null);
						info.Owner = reader.GetString(ownerIndex);
						info.TableName = reader.GetString(nameIndex);
						info.ViewName = info.TableName;
						info.ObjectType = ObjectTypeEnum.UserTable;
						if (info.TableName != null && info.TableName.IndexOf('_') >= 0)
							info.EntityName = StringHelper.GetPascalCase(info.TableName.Substring(info.TableName.IndexOf('_') + 1,
								info.TableName.Length - info.TableName.IndexOf('_') - 1));
						else
							info.EntityName = StringHelper.GetPascalCase(info.TableName);
						tableList.Add(info.TableName, info);
					}
				}
				sqlCommand.CommandText = "select objtype,objname,name,value from fn_listextendedproperty(NULL,'SCHEMA','dbo', 'table', default, NULL, NULL)";
				using (SqlDataReader reader1 = sqlCommand.ExecuteReader())
				{
					int valueIndex = reader1.GetOrdinal("value");
					int objnameIndex = reader1.GetOrdinal("objname");
					int nameIndex = reader1.GetOrdinal("name");
					while (reader1.Read())
					{
						if (reader1.GetString(nameIndex) == "MS_Description")
						{
							string tableName = reader1.GetString(objnameIndex);
							tableList[tableName].Description = reader1.GetString(valueIndex);
						}
					}
				}
			}
			if ((ObjectTypeEnum.UserView & objectType) == ObjectTypeEnum.UserView)
			{
				sqlCommand.CommandText = "exec sp_Tables @table_owner='dbo',@table_type='''VIEW'''";
				using (SqlDataReader reader = sqlCommand.ExecuteReader())
				{
					int ownerIndex = reader.GetOrdinal("TABLE_OWNER");
					int nameIndex = reader.GetOrdinal("TABLE_NAME");
					while (reader.Read())
					{
						DesignTableInfo info = new DesignTableInfo(null);
						info.Owner = reader.GetString(ownerIndex);
						info.TableName = reader.GetString(nameIndex);
						info.ViewName = info.TableName;
						info.ObjectType = ObjectTypeEnum.UserView;
						if (info.TableName != null && info.TableName.IndexOf('_') >= 0)
							info.EntityName = StringHelper.GetPascalCase(info.TableName.Substring(info.TableName.IndexOf('_') + 1,
								info.TableName.Length - info.TableName.IndexOf('_') - 1));
						else
							info.EntityName = StringHelper.GetPascalCase(info.TableName);
						tableList.Add(info.TableName, info);
					}
				}
				sqlCommand.CommandText = "select objtype,objname,name,value from fn_listextendedproperty(NULL,'SCHEMA','dbo', 'view', default, NULL, NULL)";
				using (SqlDataReader reader1 = sqlCommand.ExecuteReader())
				{
					int valueIndex = reader1.GetOrdinal("value");
					int objnameIndex = reader1.GetOrdinal("objname");
					int nameIndex = reader1.GetOrdinal("name");
					while (reader1.Read())
					{
						if (reader1.GetString(nameIndex) == "MS_Description")
						{
							string tableName = reader1.GetString(objnameIndex);
							tableList[tableName].Description = reader1.GetString(valueIndex);
						}
					}
				}
			}
			DesignTableInfo[] tableArray = new DesignTableInfo[tableList.Count];
			tableList.Values.CopyTo(tableArray, 0);
			return tableArray;
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
		public void GetColumns(DesignTableInfo tableInfo)
		{
			sqlCommand.CommandText = string.Format("exec sp_columns @table_name='{0}'", tableInfo.TableName);
			DesignColumnCollection listColumn = tableInfo.Columns;
			#region 获取数据库表列信息
			using (SqlDataReader reader = sqlCommand.ExecuteReader())
			{

				int nameIndex = reader.GetOrdinal("COLUMN_NAME");
				int typeIndex = reader.GetOrdinal("TYPE_NAME");
				int precIndex = reader.GetOrdinal("PRECISION");
				int sizeIndex = reader.GetOrdinal("LENGTH");
				int scaleIndex = reader.GetOrdinal("SCALE");
				int nullIndex = reader.GetOrdinal("NULLABLE");
				int defaultIndex = reader.GetOrdinal("COLUMN_DEF");
				int oradinalIndex = reader.GetOrdinal("ORDINAL_POSITION");
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
						if (!string.IsNullOrWhiteSpace(info.DefaultValue) && info.DefaultValue.StartsWith("("))
							info.DefaultValue = info.DefaultValue.TrimStart('(');
						if (!string.IsNullOrWhiteSpace(info.DefaultValue) && info.DefaultValue.EndsWith(")"))
							info.DefaultValue = info.DefaultValue.TrimEnd(')');
						if (!string.IsNullOrWhiteSpace(info.DefaultValue) && info.DefaultValue.EndsWith("("))
							info.DefaultValue = string.Concat(info.DefaultValue, ")");
					}
				}
			}
			#endregion

			#region 数据库列描述
			StringBuilder sqlBuilder = new StringBuilder(100);
			sqlBuilder.AppendLine("select objtype,objname,name,value");
			if (tableInfo.ObjectType == ObjectTypeEnum.UserTable)
				sqlBuilder.AppendFormat(" from fn_listextendedproperty(NULL,'SCHEMA','dbo', 'table', '{0}', 'column', NULL)", tableInfo.TableName);
			else
				sqlBuilder.AppendFormat(" from fn_listextendedproperty(NULL,'SCHEMA','dbo', 'view', '{0}', 'column', NULL)", tableInfo.TableName);
			sqlCommand.CommandText = sqlBuilder.ToString();
			using (SqlDataReader reader1 = sqlCommand.ExecuteReader())
			{
				int valueIndex = reader1.GetOrdinal("value");
				int objnameIndex = reader1.GetOrdinal("objname");
				int nameIndex = reader1.GetOrdinal("name");
				string columnName = null;
				while (reader1.Read())
				{
					if (reader1.GetString(nameIndex) == "MS_Description")
					{
						columnName = reader1.GetString(objnameIndex);
						listColumn[columnName].Comment = reader1.GetString(valueIndex);
					}
				}
			}
			#endregion

			#region 数据库主键信息描述
			UniqueConstraint pkeyConstraint = tableInfo.PrimaryKey;
			pkeyConstraint.Columns.Clear();
			sqlCommand.CommandText = string.Format("exec sp_pkeys @table_name='{0}'", tableInfo.TableName);
			using (SqlDataReader reader2 = sqlCommand.ExecuteReader())
			{
				if (reader2.HasRows)
				{
					int pkNameIndex = reader2.GetOrdinal("PK_NAME");
					int columnNameIndex = reader2.GetOrdinal("COLUMN_NAME");
					while (reader2.Read() && reader2.HasRows)
					{
						pkeyConstraint.Name = reader2.GetString(pkNameIndex);
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

			#region 数据库表唯一性约束信息
			sqlBuilder.Clear();
			sqlBuilder.AppendLine(" select ind.name, dsp.name as space_name, ind.index_id, ind.type, ");
			sqlBuilder.AppendLine("ind.is_unique, ind.is_primary_key, ind.is_unique_constraint,  ");
			sqlBuilder.AppendLine(" sta.no_recompute, ind_col.index_column_id, col.name as cname, ind_col.key_ordinal,  ");
			sqlBuilder.AppendLine(" ind_col.column_id , ind.data_space_id   ");
			sqlBuilder.AppendLine("from sys.indexes ind left outer join sys.stats sta on sta.object_id = ind.object_id and  ");
			sqlBuilder.AppendLine("sta.stats_id = ind.index_id left outer join  ");
			sqlBuilder.AppendLine("(sys.index_columns ind_col inner join sys.columns col on  ");
			sqlBuilder.AppendLine("col.object_id = ind_col.object_id and col.column_id = ind_col.column_id )   ");
			sqlBuilder.AppendLine("on ind_col.object_id = ind.object_id and ind_col.index_id = ind.index_id  ");
			sqlBuilder.AppendLine("left outer join sys.data_spaces dsp on dsp.data_space_id = ind.data_space_id   ");
			sqlBuilder.AppendFormat("where ind.object_id = object_id(N'{0}')  and ind.index_id >= 0  ", tableInfo.TableName);
			sqlBuilder.AppendLine("and ind.type <> 3 and ind.type <> 4 and ind.is_hypothetical = 0 and ind.is_unique=1 AND ind.is_primary_key=0 ");
			sqlBuilder.AppendLine("order by ind.index_id, ind_col.key_ordinal");
			UniqueConstraintCollection uniqueConstraints = tableInfo.UniqueConstraints;
			uniqueConstraints.Clear();
			sqlCommand.CommandText = sqlBuilder.ToString();
			using (SqlDataReader reader2 = sqlCommand.ExecuteReader())
			{
				if (reader2.HasRows)
				{
					string indexName = string.Empty;
					int indexNameIndex = reader2.GetOrdinal("name");
					int columnNameIndex = reader2.GetOrdinal("cname");
					List<DesignColumnInfo> uniqueColumn = new List<DesignColumnInfo>();
					while (reader2.Read())
					{
						string indexName1 = reader2.GetString(indexNameIndex);
						string columnName = reader2.GetString(columnNameIndex);
						if (indexName != string.Empty && indexName != indexName1)
						{
							uniqueConstraints.Add(uniqueConstraints.Create(indexName, uniqueColumn.ToArray(), false));
							uniqueColumn.Clear();
						}
						indexName = indexName1;
						if (listColumn.ContainsKey(columnName))
							uniqueColumn.Add(listColumn[columnName]);
					}
					uniqueConstraints.Add(uniqueConstraints.Create(indexName, uniqueColumn.ToArray(), false));
				}
			}
			#endregion
		}

		/// <summary>
		/// 获取数据库中所有存储过程
		/// </summary>
		/// <returns>获取获取的存储过程列表。</returns>
		public StoreProcedure[] GetProcedures()
		{
			sqlCommand.CommandText = "select t1.name,t2.name as owner from sys.procedures t1 join sys.schemas t2 on t1.schema_id=t2.schema_id" +
				" where t1.name not like 'sp_%' order by t1.name";
			List<StoreProcedure> tableList = new List<StoreProcedure>(200);
			using (SqlDataReader reader = sqlCommand.ExecuteReader())
			{
				int ownerIndex = reader.GetOrdinal("owner");
				int nameIndex = reader.GetOrdinal("name");
				while (reader.Read())
				{
					StoreProcedure info = new StoreProcedure();
					info.Name = reader.GetString(nameIndex);
					info.Owner = reader.GetString(ownerIndex);
					if (info.Name != null && info.Name.IndexOf('_') >= 0)
						info.EntityName = StringHelper.GetPascalCase(info.Name.Substring(info.Name.IndexOf('_') + 1,
							info.Name.Length - info.Name.IndexOf('_') - 1));
					else
						info.EntityName = StringHelper.GetPascalCase(info.Name);
					tableList.Add(info);
				}
			}
			return tableList.ToArray();
		}

		/// <summary>
		/// 获取存储过程参数列表
		/// </summary>
		/// <param name="procedure">存储过程名称</param>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public bool GetParameters(StoreProcedure procedure)
		{
			if (procedure.Parameters != null && procedure.Parameters.Length > 0) { return true; }
			sqlCommand.CommandText = string.Format("exec sp_procedure_params_rowset @procedure_name='{0}'", procedure.Name);
			List<ProcedureParameter> listColumn = new List<ProcedureParameter>(50);
			using (SqlDataReader reader = sqlCommand.ExecuteReader())
			{
				int nameIndex = reader.GetOrdinal("PARAMETER_NAME");
				int directionIndex = reader.GetOrdinal("PARAMETER_TYPE");
				int precIndex = reader.GetOrdinal("NUMERIC_PRECISION");
				int sizeIndex = reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH");
				int scaleIndex = reader.GetOrdinal("NUMERIC_SCALE");
				int nullIndex = reader.GetOrdinal("IS_NULLABLE");
				int typeIndex = reader.GetOrdinal("TYPE_NAME");
				while (reader.Read())
				{
					string name = reader.GetString(nameIndex).Remove(0, 1);
					if (name == "RETURN_VALUE") { continue; }
					ProcedureParameter info = new ProcedureParameter();
					info.Direction = ParameterDirection.Input;
					if (!reader.IsDBNull(directionIndex))
					{
						short derection = reader.GetInt16(directionIndex);
						switch (derection)
						{
							case 4:
								info.Direction = ParameterDirection.ReturnValue;
								break;
							case 2:
								info.Direction = ParameterDirection.Output;
								break;
							case 1:
								info.Direction = ParameterDirection.Input;
								break;
						}
					}
					info.Name = name;
					if (!reader.IsDBNull(sizeIndex))
						info.Size = reader.GetInt32(sizeIndex);
					if (!reader.IsDBNull(precIndex))
						info.Precision = (int)reader.GetInt16(precIndex);
					if (!reader.IsDBNull(scaleIndex))
						info.Scale = reader.GetInt16(scaleIndex);

					string typeName = reader.GetString(typeIndex).ToUpper();
					info.DbType = GetUserDbType(typeName);
					info.Nullable = reader.GetBoolean(nullIndex);
					listColumn.Add(info);
				}
				if (listColumn.Count > 0)
					procedure.Parameters = listColumn.ToArray();
			}
			return true;
		}

		/// <summary>
		/// 转换数据库参数类型
		/// </summary>
		/// <param name="sqlParameter">数据库命令执行的参数</param>
		/// <param name="dbType">SqlServer数据库列类型,SqlDbType枚举的值</param>
		/// <param name="precision">获取或设置列中数据的最大大小（以字节为单位）。</param>
		/// <param name="scale">获取或设置数据库参数值解析为的小数位数。</param>
		private void ConvertSqlParameterType(SqlParameter sqlParameter, DbTypeEnum dbType, byte precision, byte scale)
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
					sqlParameter.SqlDbType = SqlDbType.DateTime2;
					sqlParameter.Size = 3;
					return;
				case DbTypeEnum.VarBinary:
					sqlParameter.SqlDbType = SqlDbType.VarBinary;
					return;
				case DbTypeEnum.VarChar:
					sqlParameter.SqlDbType = SqlDbType.VarChar;
					return;
			}
		}

		/// <summary>
		/// 获取存储过程返回结果列信息
		/// </summary>
		/// <param name="procedure">存储过程名称</param>
		/// <returns>如果获取数据成功则返回 True，否则返回 False。</returns>
		public bool GetColumns(StoreProcedure procedure)
		{
			List<DesignColumnInfo> columns = new List<DesignColumnInfo>();
			sqlCommand.CommandText = procedure.Name;
			sqlCommand.CommandType = CommandType.StoredProcedure;
			if (procedure.Parameters != null && procedure.Parameters.Length > 0)
			{
				foreach (ProcedureParameter parameter in procedure.Parameters)
				{
					SqlParameter sqlParameter = sqlCommand.CreateParameter();
					sqlParameter.ParameterName = parameter.Name;
					sqlParameter.Precision = (byte)parameter.Precision;
					sqlParameter.Size = parameter.Size;
					sqlParameter.Scale = (byte)parameter.Scale;
					sqlParameter.Direction = parameter.Direction;
					ConvertSqlParameterType(sqlParameter, parameter.DbType, sqlParameter.Precision, sqlParameter.Scale);
					sqlParameter.Value = parameter.Value;
					sqlCommand.Parameters.Add(sqlParameter);
				}
			}
			using (SqlDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
			{
				DataTable table = reader.GetSchemaTable();
				if (table == null) { return false; }
				foreach (DataRow row in table.Rows)
				{
					DesignColumnInfo info = new DesignColumnInfo(null);
					info.Name = row["ColumnName"].ToString();
					string typeName = row["DataTypeName"].ToString().ToUpper();
					info.DbType = GetUserDbType(typeName);
					switch (info.DbType)
					{
						case DbTypeEnum.Char:
						case DbTypeEnum.VarChar:
						case DbTypeEnum.NChar:
						case DbTypeEnum.NVarChar:
						case DbTypeEnum.Binary:
						case DbTypeEnum.VarBinary:
							info.Size = Convert.ToInt32(row["ColumnSize"]);
							break;
						case DbTypeEnum.Decimal:
							info.Precision = Convert.ToByte(row["NumericPrecision"]);
							if (!row.IsNull("NumericScale"))
								info.Scale = Convert.ToByte(row["NumericScale"]);
							break;
						case DbTypeEnum.Timestamp:
							if (!row.IsNull("NumericScale"))
								info.Scale = Convert.ToByte(row["NumericScale"]);
							break;
					}
					info.Nullable = Convert.ToBoolean(row["AllowDBNull"]);
					columns.Add(info);
				}
				if (columns.Count > 0)
					procedure.Columns = columns.ToArray();
				return procedure.Columns != null && procedure.Columns.Length > 0;
			}
		}

		/// <summary>
		/// 获取命令文本中返回结果的列信息。
		/// </summary>
		/// <param name="columns">待填充 DataTable 实例。</param>
		/// <param name="trancateSql">需要查询的 Trancate-Sql 实例。</param>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public bool GetTransactSql(IList<DesignColumnInfo> columns, string trancateSql)
		{
			if (columns == null) { columns = new List<DesignColumnInfo>(50); }
			else { columns.Clear(); }
			sqlCommand.CommandText = trancateSql;
			using (SqlDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
			{
				DataTable table = reader.GetSchemaTable();
				foreach (DataRow row in table.Rows)
				{
					DesignColumnInfo info = new DesignColumnInfo(null)
					{
						Name = row["ColumnName"].ToString()
					};
					string typeName = row["DataTypeName"].ToString().ToUpper();
					info.DbType = GetUserDbType(typeName);
					switch (info.DbType)
					{
						case DbTypeEnum.Char:
						case DbTypeEnum.VarChar:
						case DbTypeEnum.NChar:
						case DbTypeEnum.NVarChar:
						case DbTypeEnum.Binary:
						case DbTypeEnum.VarBinary:
							info.Size = Convert.ToInt32(row["ColumnSize"]);
							break;
						case DbTypeEnum.Decimal:
							info.Precision = Convert.ToByte(row["NumericPrecision"]);
							if (!row.IsNull("NumericScale"))
								info.Scale = Convert.ToByte(row["NumericScale"]);
							break;
						case DbTypeEnum.Timestamp:
							if (!row.IsNull("NumericScale"))
								info.Scale = Convert.ToByte(row["NumericScale"]);
							break;
					}
					info.Nullable = Convert.ToBoolean(row["AllowDBNull"]);
					columns.Add(info);
				}
			}
			return true;
		}

		/// <summary>
		/// 根据自定义 Trancate-SQL 查询表结构信息。
		/// </summary>
		/// <param name="result">查询表结构定义</param>
		/// <param name="trancateSql">需要查询的 Trancate-Sql 实例。</param>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public bool GetTransactSql(TransactSqlResult result, string trancateSql)
		{
			sqlCommand.CommandText = trancateSql;
			foreach (TransactParameterInfo info in result.Parameters)
			{
				string name = string.Concat("@", info.Name);
				if (!sqlCommand.Parameters.Contains(name))
				{
					SqlParameter parameter = sqlCommand.CreateParameter();
					info.CreateSqlParameter(parameter, name);
					parameter.SqlValue = DBNull.Value;
					sqlCommand.Parameters.Add(parameter);
				}
			}
			#region 获取列定义
			using (SqlDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
			{
				DataTable table = reader.GetSchemaTable();
				//result.Columns.Clear();
				foreach (DataRow row in table.Rows)
				{
					if (!row.IsNull("IsHidden") && Convert.ToBoolean(row["IsHidden"])) { continue; }
					string columnName = Convert.ToString(row["ColumnName"]);
					if (result.Columns.TryGetValue(columnName, out TransactColumnInfo eColumn))
					{
						if (string.IsNullOrWhiteSpace(eColumn.PropertyName))
						{
							if (result.PropertyMapping.TryGetValue(columnName, out string propName))
							{
								eColumn.PropertyName = propName;
							}
							else
							{
								eColumn.PropertyName = StringHelper.GetPascalCase(columnName);
							}
						}

						eColumn.Source = Convert.ToString(row["BaseColumnName"]);
						string typeName = row["DataTypeName"].ToString().ToUpper();
						eColumn.DbType = GetUserDbType(typeName);
						eColumn.PrimaryKey = Convert.ToBoolean(row["IsKey"]);
						switch (eColumn.DbType)
						{
							case DbTypeEnum.Char:
							case DbTypeEnum.VarChar:
							case DbTypeEnum.NChar:
							case DbTypeEnum.NVarChar:
							case DbTypeEnum.Binary:
							case DbTypeEnum.VarBinary:
								eColumn.Size = Convert.ToInt32(row["ColumnSize"]);
								break;
							case DbTypeEnum.Decimal:
								eColumn.Precision = Convert.ToByte(row["NumericPrecision"]);
								if (!row.IsNull("NumericScale"))
									eColumn.Scale = Convert.ToByte(row["NumericScale"]);
								break;
							case DbTypeEnum.Timestamp:
								if (!row.IsNull("NumericScale"))
									eColumn.Scale = Convert.ToByte(row["NumericScale"]);
								break;
						}
						eColumn.Nullable = Convert.ToBoolean(row["AllowDBNull"]);
					}
					else
					{
						string tableName = Convert.ToString(row["BaseTableName"]);
						TransactColumnInfo nColumn = result.AddColumn(tableName, columnName);
						if (string.IsNullOrWhiteSpace(nColumn.PropertyName))
						{
							if (result.PropertyMapping.TryGetValue(columnName, out string propName))
							{
								nColumn.PropertyName = propName;
							}
							else
							{
								nColumn.PropertyName = StringHelper.GetPascalCase(columnName);
							}
						}
						nColumn.Source = Convert.ToString(row["BaseColumnName"]);
						string typeName = row["DataTypeName"].ToString().ToUpper();
						nColumn.DbType = GetUserDbType(typeName);
						nColumn.PrimaryKey = Convert.ToBoolean(row["IsKey"]);
						switch (nColumn.DbType)
						{
							case DbTypeEnum.Char:
							case DbTypeEnum.VarChar:
							case DbTypeEnum.NChar:
							case DbTypeEnum.NVarChar:
							case DbTypeEnum.Binary:
							case DbTypeEnum.VarBinary:
								nColumn.Size = Convert.ToInt32(row["ColumnSize"]);
								break;
							case DbTypeEnum.Decimal:
								nColumn.Precision = Convert.ToByte(row["NumericPrecision"]);
								if (!row.IsNull("NumericScale"))
									nColumn.Scale = Convert.ToByte(row["NumericScale"]);
								break;
							case DbTypeEnum.Timestamp:
								if (!row.IsNull("NumericScale"))
									nColumn.Scale = Convert.ToByte(row["NumericScale"]);
								break;
						}
						nColumn.Nullable = Convert.ToBoolean(row["AllowDBNull"]);
					}

				}
			}
			#endregion
			List<string> tableList = new List<string>(result.Count);
			foreach (TransactTableInfo tableInfo in result)
			{
				string objName = string.Concat("object_id(N'", tableInfo.ObjectName, "')");
				if (tableList.Contains(objName) == false) { tableList.Add(objName); }
			}
			sqlCommand.Parameters.Clear();
			sqlCommand.CommandText = string.Format(@"SELECT t1.object_id AS TABLE_ID,t2.name as TABLE_NAME,t1.name as COLUMN_NAME,
t1.column_id as ORDINAL_POSITION,t4.value AS COLUMN_DES
FROM sys.all_objects t2 join sys.all_columns t1 on t2.object_id=t1.object_id
left join sys.extended_properties t4 ON t1.object_id=t4.major_id AND t1.column_id=t4.minor_id
where t1.object_id IN({0}) order by t1.object_id,t1.column_id", string.Join(",", tableList.ToArray()));
			//string.Join("object_id('", tableInfo.TableAlias.Keys.Select(m=>return string.Concat("object_id('","","')")));
			using (SqlDataReader reader = sqlCommand.ExecuteReader())
			{
				int nameIndex = reader.GetOrdinal("TABLE_ID");
				int directionIndex = reader.GetOrdinal("TABLE_NAME");
				int columnIndex = reader.GetOrdinal("COLUMN_NAME");
				int sizeIndex = reader.GetOrdinal("ORDINAL_POSITION");
				int desIndex = reader.GetOrdinal("COLUMN_DES");
				while (reader.Read())
				{
					string columnName = reader.GetString(columnIndex);
					if (result.Columns.TryGetValue(columnName, out TransactColumnInfo column) && !reader.IsDBNull(desIndex))
					{
						column.Comment = reader.GetString(desIndex);
					}
				}
			}
			return true;
		}

		/// <summary>
		/// 获取函数的参数信息
		/// </summary>
		/// <param name="result">表或视图名称。</param>
		/// <returns>如果获取数据成功则返回True，否则返回False。</returns>
		public void GetParameters(TransactSqlResult result)
		{
			if (result.Count == 0) { return; }
			List<string> tableList = new List<string>(result.Count);
			result.Parameters.Clear();
			foreach (TransactTableInfo tableInfo in result)
			{
				string objName = string.Concat("object_id(N'", tableInfo.ObjectName, "')");
				if (tableList.Contains(objName) == false) { tableList.Add(objName); }
			}
			sqlCommand.CommandText = string.Format(@"SELECT t1.object_id,ts.name as OBJECT_NAME,t1.name as COLUMN_NAME,t3.name as TYPE_NAME,
CASE WHEN t1.system_type_id iN(231,239,99) then t1.max_length/2 ELSE t1.PRECISION END as PRECISION,
t1.max_length as LENGTH,t1.SCALE,t1.parameter_id as ORDINAL_POSITION,t1.is_output as IS_OUTPUT
FROM sys.objects ts join sys.parameters t1 on ts.object_id=t1.object_id
join sys.types t3 on t1.user_type_id=t3.user_type_id where t1.object_id IN({0})", string.Join(",", tableList.ToArray()));
			using (SqlDataReader reader = sqlCommand.ExecuteReader())
			{
				int objIndex = reader.GetOrdinal("OBJECT_NAME");
				int nameIndex = reader.GetOrdinal("COLUMN_NAME");
				int typeIndex = reader.GetOrdinal("TYPE_NAME");
				int precIndex = reader.GetOrdinal("PRECISION");
				int sizeIndex = reader.GetOrdinal("LENGTH");
				int scaleIndex = reader.GetOrdinal("SCALE");
				int oradinalIndex = reader.GetOrdinal("ORDINAL_POSITION");
				TransactTableInfo tableInfo = null;
				while (reader.Read())
				{
					string objName = reader.GetString(objIndex);
					if (result.TryGetValue(objName, out tableInfo))
					{
						TransactParameterInfo parameter = new TransactParameterInfo(result)
						{
							Name = reader.GetString(nameIndex).Remove(0, 1)
						};
						string typeName = reader.GetString(typeIndex).ToUpper();
						parameter.ParameterType = GetUserDbType(typeName);
						string upperName = parameter.Name.ToUpper();
						if (parameter.ParameterType == DbTypeEnum.DateTime && upperName == TableDesignerInfo.ModifiedTimeColumn)
							parameter.ParameterType = DbTypeEnum.Timestamp;
						else if (parameter.ParameterType == DbTypeEnum.DateTime2 && upperName == TableDesignerInfo.ModifyTimeColumn)
							parameter.ParameterType = DbTypeEnum.Timestamp;
						parameter.Size = 0;
						switch (parameter.ParameterType)
						{
							case DbTypeEnum.Char:
							case DbTypeEnum.VarChar:
							case DbTypeEnum.NChar:
							case DbTypeEnum.NVarChar:
							case DbTypeEnum.Binary:
							case DbTypeEnum.VarBinary:
								parameter.Size = reader.GetInt32(precIndex);
								break;
							case DbTypeEnum.Decimal:
								parameter.Precision = (byte)reader.GetInt32(precIndex);
								if (!reader.IsDBNull(scaleIndex))
									parameter.Scale = reader.GetByte(scaleIndex);
								break;
						}
						parameter.Nullable = true;
						result.Parameters.Add(parameter);
					}
				}
			}
		}

		/// <summary>
		/// sqlCommand
		/// </summary>
		public void Dispose()
		{
			sqlConnection.Close();
			sqlConnection.Dispose();
			sqlCommand.Dispose();
		}
	}
}
