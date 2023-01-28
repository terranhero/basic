using System.Data;
using System.Text;
using Basic.Collections;
using Basic.Configuration;
using Basic.DataEntities;
using Basic.Designer;
using Basic.Enums;

namespace Basic.Database
{
	/// <summary>
	/// 数据库表列信息
	/// </summary>
	internal static class TableInfoExtension
	{
		/// <summary>
		/// 根据数据库列信息，创建新增数据库命令及其参数
		/// </summary>
		/// <param name="staticCommand">从配置文件总读取的命令结构信息</param>
		/// <param name="fileClass">配置文件信息</param>
		/// <param name="conType">需要生成命令结构的类型</param>
		internal static StaticCommandElement CreateInsertSqlStruct(this DesignTableInfo tableInfo, DataEntityElement dataEntity, StaticCommandElement staticCommand)
		{
			if (tableInfo.Columns == null || tableInfo.Columns.Count == 0) { return null; }
			bool newCreated = staticCommand == null;
			dataEntity.Comment = tableInfo.Description;
			if (string.IsNullOrWhiteSpace(dataEntity.Name))
				dataEntity.Name = string.Concat(tableInfo.EntityName, "New");
			dataEntity.TableName = tableInfo.TableName;
			DataEntityPropertyCollection dataProperties = dataEntity.Properties;
			#region 构建 INSERT,实体类
			foreach (DesignColumnInfo column in tableInfo.Columns)
			{
				if (column.Computed) { continue; }
				string pn = StringHelper.GetCsName(column.Name);
				string columnName = column.Name.ToUpper();
				if (column.IsCreatedTimeColumn)
				{
					if (dataProperties.ContainsKey(pn))
						dataProperties.Remove(dataProperties[pn]);
					continue;
				}
				else if (column.IsModifiedTimeColumn)
				{
					if (dataProperties.ContainsKey(pn))
						dataProperties.Remove(dataProperties[pn]);
					continue;
				}
				DataEntityPropertyElement property;
				if (dataProperties.TryGetValue(column.Name, out property))
					DbBuilderHelper.CreateAbstractProperty(property, column, true);
				else
				{
					property = new DataEntityPropertyElement(dataEntity);
					DbBuilderHelper.CreateAbstractProperty(property, column, false);
					dataProperties.Add(property);
				}
			}
			#endregion
			return tableInfo.CreateInsertSqlStruct(staticCommand);
		}

		/// <summary>
		/// 根据数据库列信息，创建新增数据库命令及其参数
		/// </summary>
		/// <param name="staticCommand">从配置文件总读取的命令结构信息</param>
		/// <param name="fileClass">配置文件信息</param>
		/// <param name="conType">需要生成命令结构的类型</param>
		internal static StaticCommandElement CreateInsertSqlStruct(this DesignTableInfo tableInfo, StaticCommandElement staticCommand)
		{
			if (tableInfo.Columns == null || tableInfo.Columns.Count == 0) { return null; }
			bool newCreated = staticCommand == null;
			if (staticCommand == null) { return staticCommand; }
			staticCommand.Parameters.Clear();
			staticCommand.Kind = ConfigurationTypeEnum.AddNew;
			staticCommand.Comment = string.Concat("新增", tableInfo.Description);
			staticCommand.Name = "Create";
			StringBuilder sqlBuilder = new StringBuilder("INSERT INTO ", 1000);
			sqlBuilder.AppendFormat("{0}(", tableInfo.TableName);
			int insertLength = sqlBuilder.Length;
			StringBuilder valueBuilder = new StringBuilder("VALUES(", 500);
			int valueLength = valueBuilder.Length;

			#region 构建 INSERT,实体类
			foreach (DesignColumnInfo column in tableInfo.Columns)
			{
				if (column.Computed) { continue; }

				if (insertLength == sqlBuilder.Length)
					sqlBuilder.AppendFormat("{0}", column.Name);
				else
					sqlBuilder.AppendFormat(",{0}", column.Name);

				if (column.IsCreatedTimeColumn)
				{
					if (valueLength == valueBuilder.Length)
						valueBuilder.Append("{%NOW%}");
					else
						valueBuilder.Append(",{%NOW%}");
					continue;
				}
				else if (column.IsModifiedTimeColumn)
				{
					if (valueLength == valueBuilder.Length)
						valueBuilder.Append("{%NOW%}");
					else
						valueBuilder.Append(",{%NOW%}");
					continue;
				}

				CommandParameter parameter = new CommandParameter(staticCommand);
				column.CreateParameter(parameter);
				staticCommand.Parameters.Add(parameter);
				if (valueLength == valueBuilder.Length)
					valueBuilder.Append(staticCommand.CreateParameterName(column.Name));
				else
					valueBuilder.AppendFormat(",{0}", staticCommand.CreateParameterName(column.Name));
			}
			sqlBuilder.AppendLine(")");
			sqlBuilder.Append(valueBuilder.ToString());
			sqlBuilder.Append(")");
			staticCommand.CommandText = sqlBuilder.ToString();
			#endregion

			#region 创建主键新值命令
			if (tableInfo.PrimaryKey.Columns.Count > 0)
			{
				if (!staticCommand.NewCommands.ContainsKey(tableInfo.PrimaryKey.Name))
				{
					if (tableInfo.PrimaryKey.Columns.Count != tableInfo.Columns.Count)
					{
						NewCommandElement newCommand = new NewCommandElement(staticCommand);
						newCommand.Name = tableInfo.PrimaryKey.Name;
						sqlBuilder.Clear();
						sqlBuilder.AppendFormat("SELECT "); int length = sqlBuilder.Length;
						foreach (DesignColumnInfo column in tableInfo.PrimaryKey.Columns)
						{
							if (column.DbType == DbTypeEnum.Int16 || column.DbType == DbTypeEnum.Int32 ||
								column.DbType == DbTypeEnum.Int64 || column.DbType == DbTypeEnum.Decimal)
							{
								if (length == sqlBuilder.Length)
									sqlBuilder.AppendFormat("ISNULL(MAX({0}),0)+1 AS {0}", column.Name);
								else
									sqlBuilder.AppendFormat(",ISNULL(MAX({0}),0)+1 AS {0}", column.Name);
							}
						}
						if (sqlBuilder.Length > length)
						{
							sqlBuilder.AppendFormat(" FROM {0}", tableInfo.TableName);
							newCommand.CommandText = sqlBuilder.ToString();
							staticCommand.NewCommands.Add(newCommand);
						}
					}
				}
			}
			#endregion

			#region 创建约束检查
			if (tableInfo.UniqueConstraints.Count > 0)
			{
				staticCommand.CheckCommands.Clear();
				foreach (UniqueConstraint unique in tableInfo.UniqueConstraints)
				{
					sqlBuilder.Clear();
					CheckedCommandElement checkCommand = new CheckedCommandElement(staticCommand);
					checkCommand.Name = unique.Name;
					sqlBuilder.AppendFormat("SELECT 1 FROM {0} WHERE ", tableInfo.TableName); int length = sqlBuilder.Length;
					foreach (DesignColumnInfo column in unique.Columns)
					{
						string parameterName = checkCommand.CreateParameterName(column.Name);
						if (length == sqlBuilder.Length)
							sqlBuilder.AppendFormat("{0}={1}", column.Name, parameterName);
						else
							sqlBuilder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
						CommandParameter parameter = new CommandParameter(checkCommand);
						column.CreateParameter(parameter);
						checkCommand.Parameters.Add(parameter);
					}
					checkCommand.CommandText = sqlBuilder.ToString();
					staticCommand.CheckCommands.Add(checkCommand);
				}
			}
			#endregion
			return staticCommand;
		}

		/// <summary>
		/// 根据数据库列信息，创建新增数据库命令及其参数
		/// </summary>
		/// <param name="staticCommand">从配置文件总读取的命令结构信息</param>
		/// <param name="fileClass">配置文件信息</param>
		/// <param name="conType">需要生成命令结构的类型</param>
		internal static StaticCommandElement CreateUpdateSqlStruct(this DesignTableInfo tableInfo, DataEntityElement dataEntity, StaticCommandElement staticCommand)
		{
			if (tableInfo.Columns == null || tableInfo.Columns.Count == 0) { return null; }
			if (staticCommand == null) { staticCommand = new StaticCommandElement(dataEntity); }
			staticCommand.Kind = ConfigurationTypeEnum.Modify;
			staticCommand.Name = "Update";
			staticCommand.Comment = string.Concat("更新", tableInfo.Description);
			staticCommand.Parameters.Clear();
			staticCommand.CheckCommands.Clear();
			staticCommand.NewCommands.Clear();
			dataEntity.Comment = tableInfo.Description;
			if (string.IsNullOrWhiteSpace(dataEntity.Name))
				dataEntity.Name = string.Concat(tableInfo.EntityName, "Edit");
			dataEntity.TableName = tableInfo.TableName;
			DataEntityPropertyCollection dataProperties = dataEntity.Properties;

			#region 构建 UPDATE SQL
			StringBuilder updateBuilder = new StringBuilder("UPDATE ", 1000);
			updateBuilder.AppendFormat("{0} SET ", tableInfo.TableName);
			int updateLength = updateBuilder.Length;
			StringBuilder whereBuilder = new StringBuilder(" WHERE ", 500);
			int whereLength = whereBuilder.Length;
			DesignColumnInfo timeStampColumn = null;
			foreach (DesignColumnInfo column in tableInfo.Columns)
			{
				if (column.Computed) { continue; }
				if (column.IsCreatedTimeColumn) { continue; }
				string pn = StringHelper.GetCsName(column.Name);
				DataEntityPropertyElement property;
				if (dataProperties.TryGetValue(column.Name, out property))
					DbBuilderHelper.CreateAbstractProperty(property, column, true);
				else
				{
					property = new DataEntityPropertyElement(dataEntity);
					DbBuilderHelper.CreateAbstractProperty(property, column, false);
					dataProperties.Add(property);
				}

				if (column.IsModifiedTimeColumn) { timeStampColumn = column; }
				CommandParameter parameter = new CommandParameter(staticCommand);
				string parameterName = staticCommand.CreateParameterName(column.Name);
				parameter.Name = column.Name;
				parameter.SourceColumn = column.Name;

				parameter.ParameterType = column.DbType;
				if (column.DbType == DbTypeEnum.Decimal)
				{
					parameter.Precision = column.Precision;
					parameter.Scale = (byte)column.Scale;
				}
				else { parameter.Size = column.Size; }
				parameter.Direction = ParameterDirection.Input;
				parameter.Nullable = column.Nullable;
				staticCommand.Parameters.Add(parameter);
				if (column.IsModifiedTimeColumn)
				{
					if (updateLength == updateBuilder.Length)
						updateBuilder.AppendFormat("{0}={{%NOW%}}", column.Name);
					else
						updateBuilder.AppendFormat(", {0}={{%NOW%}}", column.Name);
				}
				else if (!column.PrimaryKey)
				{
					if (updateLength == updateBuilder.Length)
						updateBuilder.AppendFormat("{0}={1}", column.Name, parameterName);
					else
						updateBuilder.AppendFormat(", {0}={1}", column.Name, parameterName);
				}
				if (column.PrimaryKey || column.IsModifiedTimeColumn)
				{
					if (whereLength == whereBuilder.Length)
						whereBuilder.AppendFormat("{0}={1}", column.Name, parameterName);
					else
						whereBuilder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
				}
			}
			updateBuilder.AppendLine();
			updateBuilder.Append(whereBuilder.ToString());
			staticCommand.CommandText = updateBuilder.ToString();
			#endregion

			if (timeStampColumn != null)
			{
				#region 检测更新时间戳
				CheckedCommandElement checkCommand = new CheckedCommandElement(staticCommand);
				checkCommand.Name = string.Concat("PK_", tableInfo.TableName);
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("SELECT 1 FROM {0} WHERE ", tableInfo.TableName);
				int length = builder.Length;
				if (tableInfo.PrimaryKey != null && tableInfo.PrimaryKey.Columns != null && tableInfo.PrimaryKey.Columns.Count > 0)
				{
					foreach (DesignColumnInfo column in tableInfo.PrimaryKey.Columns)
					{
						CommandParameter parameter = new CommandParameter(checkCommand);
						string parameterName = staticCommand.CreateParameterName(column.Name);
						column.CreateParameter(parameter);
						checkCommand.Parameters.Add(parameter);

						if (length == builder.Length)
							builder.AppendFormat("{0}={1}", column.Name, parameterName);
						else
							builder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
					}
					CommandParameter timestampParameter = new CommandParameter(checkCommand);
					timeStampColumn.CreateParameter(timestampParameter);
					checkCommand.Parameters.Add(timestampParameter);
					builder.AppendFormat(" AND {0}<>{1}", timeStampColumn.Name, checkCommand.CreateParameterName(timeStampColumn.Name));
				}
				else
				{
					CommandParameter timestampParameter = new CommandParameter(checkCommand);
					timeStampColumn.CreateParameter(timestampParameter);
					checkCommand.Parameters.Add(timestampParameter);
					builder.AppendFormat("{0}<>{1}", timeStampColumn.Name, checkCommand.CreateParameterName(timeStampColumn.Name));
				}
				checkCommand.ErrorCode = string.Join("_", new string[] { tableInfo.EntityName, timeStampColumn.PropertyName, "Exist" });
				checkCommand.PropertyName = timeStampColumn.PropertyName;
				checkCommand.CommandText = builder.ToString();
				staticCommand.CheckCommands.Add(checkCommand);
				#endregion
			}

			if (tableInfo.UniqueConstraints.Count > 0)
			{
				#region 约束检测
				foreach (UniqueConstraint unique in tableInfo.UniqueConstraints)
				{
					StringBuilder builder = new StringBuilder();
					builder.AppendFormat("SELECT 1 FROM {0} WHERE ", tableInfo.TableName);
					int length = builder.Length;
					CheckedCommandElement checkCommand = new CheckedCommandElement(staticCommand);
					checkCommand.Name = unique.Name;
					if (tableInfo.PrimaryKey != null && tableInfo.PrimaryKey.Columns != null && tableInfo.PrimaryKey.Columns.Count > 0)
					{
						foreach (DesignColumnInfo column in tableInfo.PrimaryKey.Columns)
						{
							CommandParameter parameter = new CommandParameter(checkCommand);
							string parameterName = staticCommand.CreateParameterName(column.Name);
							parameter.Name = column.Name;
							parameter.SourceColumn = column.Name;
							parameter.ParameterType = column.DbType;
							if (column.DbType == DbTypeEnum.Decimal)
							{
								parameter.Precision = column.Precision;
								parameter.Scale = (byte)column.Scale;
							}
							else { parameter.Size = column.Size; }
							parameter.Direction = ParameterDirection.Input;
							parameter.Nullable = column.Nullable;
							checkCommand.Parameters.Add(parameter);
							if (length == builder.Length)
								builder.AppendFormat("{0}<>{1}", column.Name, parameterName);
							else
								builder.AppendFormat(" AND {0}<>{1}", column.Name, parameterName);
						}
					}

					foreach (DesignColumnInfo column in unique.Columns)
					{
						CommandParameter parameter = new CommandParameter(checkCommand);
						string parameterName = staticCommand.CreateParameterName(column.Name);
						parameter.Name = column.Name;
						parameter.SourceColumn = column.Name;
						parameter.ParameterType = column.DbType;
						if (column.DbType == DbTypeEnum.Decimal)
						{
							parameter.Precision = column.Precision;
							parameter.Scale = (byte)column.Scale;
						}
						else { parameter.Size = column.Size; }
						parameter.Direction = ParameterDirection.Input;
						parameter.Nullable = column.Nullable;
						checkCommand.Parameters.Add(parameter);
						if (length == builder.Length)
							builder.AppendFormat("{0}={1}", column.Name, parameterName);
						else
							builder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
						checkCommand.ErrorCode = string.Join("_", new string[] { tableInfo.EntityName, column.PropertyName, "Exist" });
						checkCommand.PropertyName = column.Name;
					}
					checkCommand.CommandText = builder.ToString();
					staticCommand.CheckCommands.Add(checkCommand);
				}
				#endregion
			}
			return staticCommand;
		}

		/// <summary>
		/// 根据数据库列信息，创建新增数据库命令及其参数
		/// </summary>
		/// <param name="staticCommand">从配置文件总读取的命令结构信息</param>
		/// <param name="conType">需要生成命令结构的类型</param>
		internal static StaticCommandElement CreateUpdateSqlStruct(this DesignTableInfo tableInfo, StaticCommandElement staticCommand)
		{
			if (tableInfo.Columns == null || tableInfo.Columns.Count == 0) { return null; }
			if (staticCommand == null) { return staticCommand; }
			staticCommand.Kind = ConfigurationTypeEnum.Modify;
			staticCommand.Name = "Update";
			staticCommand.Comment = string.Concat("更新", tableInfo.Description);
			staticCommand.Parameters.Clear();
			staticCommand.CheckCommands.Clear();
			staticCommand.NewCommands.Clear();
			#region 构建 UPDATE SQL
			StringBuilder updateBuilder = new StringBuilder("UPDATE ", 1000);
			updateBuilder.AppendFormat("{0} SET ", tableInfo.TableName);
			int updateLength = updateBuilder.Length;
			StringBuilder whereBuilder = new StringBuilder(" WHERE ", 500);
			int whereLength = whereBuilder.Length;
			DesignColumnInfo timeStampColumn = null;
			foreach (DesignColumnInfo column in tableInfo.Columns)
			{
				if (column.Computed) { continue; }
				string pn = StringHelper.GetCsName(column.Name);

				if (column.IsCreatedTimeColumn)
					continue;
				if (column.IsModifiedTimeColumn)
					timeStampColumn = column;
				CommandParameter parameter = new CommandParameter(staticCommand);
				string parameterName = staticCommand.CreateParameterName(column.Name);
				parameter.Name = column.Name;
				parameter.SourceColumn = column.Name;

				parameter.ParameterType = column.DbType;
				if (column.DbType == DbTypeEnum.Decimal)
				{
					parameter.Precision = column.Precision;
					parameter.Scale = (byte)column.Scale;
				}
				else { parameter.Size = column.Size; }
				parameter.Direction = ParameterDirection.Input;
				parameter.Nullable = column.Nullable;
				staticCommand.Parameters.Add(parameter);
				if (column.IsModifiedTimeColumn)
				{
					if (updateLength == updateBuilder.Length)
						updateBuilder.AppendFormat("{0}={{%NOW%}}", column.Name);
					else
						updateBuilder.AppendFormat(", {0}={{%NOW%}}", column.Name);
				}
				else if (!column.PrimaryKey)
				{
					if (updateLength == updateBuilder.Length)
						updateBuilder.AppendFormat("{0}={1}", column.Name, parameterName);
					else
						updateBuilder.AppendFormat(", {0}={1}", column.Name, parameterName);
				}
				if (column.PrimaryKey || timeStampColumn == column)
				{
					if (whereLength == whereBuilder.Length)
						whereBuilder.AppendFormat("{0}={1}", column.Name, parameterName);
					else
						whereBuilder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
				}
			}
			updateBuilder.AppendLine();
			updateBuilder.Append(whereBuilder.ToString());
			staticCommand.CommandText = updateBuilder.ToString();
			#endregion

			if (timeStampColumn != null)
			{
				#region 检测更新时间戳
				CheckedCommandElement checkCommand = new CheckedCommandElement(staticCommand);
				checkCommand.Name = string.Concat("PK_", tableInfo.TableName);
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("SELECT 1 FROM {0} WHERE ", tableInfo.TableName);
				int length = builder.Length;
				if (tableInfo.PrimaryKey != null && tableInfo.PrimaryKey.Columns != null && tableInfo.PrimaryKey.Columns.Count > 0)
				{
					foreach (DesignColumnInfo column in tableInfo.PrimaryKey.Columns)
					{
						CommandParameter parameter = new CommandParameter(checkCommand);
						string parameterName = staticCommand.CreateParameterName(column.Name);
						column.CreateParameter(parameter);
						checkCommand.Parameters.Add(parameter);

						if (length == builder.Length)
							builder.AppendFormat("{0}={1}", column.Name, parameterName);
						else
							builder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
					}
					CommandParameter timestampParameter = new CommandParameter(checkCommand);
					timeStampColumn.CreateParameter(timestampParameter);
					checkCommand.Parameters.Add(timestampParameter);
					builder.AppendFormat(" AND {0}<>{1}", timeStampColumn.Name, checkCommand.CreateParameterName(timeStampColumn.Name));
				}
				else
				{
					CommandParameter timestampParameter = new CommandParameter(checkCommand);
					timeStampColumn.CreateParameter(timestampParameter);
					checkCommand.Parameters.Add(timestampParameter);
					builder.AppendFormat("{0}<>{1}", timeStampColumn.Name, checkCommand.CreateParameterName(timeStampColumn.Name));
				}
				checkCommand.ErrorCode = string.Join("_", new string[] { tableInfo.EntityName, timeStampColumn.PropertyName, "Exist" });
				checkCommand.PropertyName = timeStampColumn.PropertyName;
				checkCommand.CommandText = builder.ToString();
				staticCommand.CheckCommands.Add(checkCommand);
				#endregion
			}

			if (tableInfo.UniqueConstraints.Count > 0)
			{
				#region 约束检测
				foreach (UniqueConstraint unique in tableInfo.UniqueConstraints)
				{
					StringBuilder builder = new StringBuilder();
					builder.AppendFormat("SELECT 1 FROM {0} WHERE ", tableInfo.TableName);
					int length = builder.Length;
					CheckedCommandElement checkCommand = new CheckedCommandElement(staticCommand);
					checkCommand.Name = unique.Name;
					if (tableInfo.PrimaryKey != null && tableInfo.PrimaryKey.Columns != null && tableInfo.PrimaryKey.Columns.Count > 0)
					{
						foreach (DesignColumnInfo column in tableInfo.PrimaryKey.Columns)
						{
							CommandParameter parameter = new CommandParameter(checkCommand);
							string parameterName = staticCommand.CreateParameterName(column.Name);
							parameter.Name = column.Name;
							parameter.SourceColumn = column.Name;
							parameter.ParameterType = column.DbType;
							if (column.DbType == DbTypeEnum.Decimal)
							{
								parameter.Precision = column.Precision;
								parameter.Scale = (byte)column.Scale;
							}
							else { parameter.Size = column.Size; }
							parameter.Direction = ParameterDirection.Input;
							parameter.Nullable = column.Nullable;
							checkCommand.Parameters.Add(parameter);
							if (length == builder.Length)
								builder.AppendFormat("{0}<>{1}", column.Name, parameterName);
							else
								builder.AppendFormat(" AND {0}<>{1}", column.Name, parameterName);
						}
					}

					foreach (DesignColumnInfo column in unique.Columns)
					{
						CommandParameter parameter = new CommandParameter(checkCommand);
						string parameterName = staticCommand.CreateParameterName(column.Name);
						parameter.Name = column.Name;
						parameter.SourceColumn = column.Name;
						parameter.ParameterType = column.DbType;
						if (column.DbType == DbTypeEnum.Decimal)
						{
							parameter.Precision = column.Precision;
							parameter.Scale = (byte)column.Scale;
						}
						else { parameter.Size = column.Size; }
						parameter.Direction = ParameterDirection.Input;
						parameter.Nullable = column.Nullable;
						checkCommand.Parameters.Add(parameter);
						if (length == builder.Length)
							builder.AppendFormat("{0}={1}", column.Name, parameterName);
						else
							builder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
					}
					checkCommand.CommandText = builder.ToString();
					staticCommand.CheckCommands.Add(checkCommand);
				}
				#endregion
			}
			return staticCommand;
		}

		/// <summary>
		/// 根据数据库列信息，创建删除数据库命令及其参数
		/// </summary>
		/// <param name="sqlStruct">从配置文件总读取的命令结构信息</param>
		/// <param name="fileClass">配置文件信息</param>
		/// <param name="conType">需要生成命令结构的类型</param>
		internal static StaticCommandElement CreateDeleteSqlStruct(this DesignTableInfo tableInfo, DataEntityElement dataEntity, StaticCommandElement staticCommand)
		{
			if (tableInfo.Columns == null || tableInfo.Columns.Count == 0) { return null; }
			if (staticCommand == null) { staticCommand = new StaticCommandElement(dataEntity); }
			staticCommand.Kind = ConfigurationTypeEnum.Remove;
			staticCommand.Name = "Delete";
			staticCommand.Comment = string.Concat("删除", tableInfo.Description);
			staticCommand.Parameters.Clear();
			staticCommand.CheckCommands.Clear();
			staticCommand.NewCommands.Clear();
			dataEntity.Comment = tableInfo.Description;
			if (string.IsNullOrWhiteSpace(dataEntity.Name))
				dataEntity.Name = string.Concat(tableInfo.EntityName, "Del");
			dataEntity.TableName = tableInfo.TableName;
			DataEntityPropertyCollection dataProperties = dataEntity.Properties;

			#region 构建 DELETE SQL
			StringBuilder deleteBuilder = new StringBuilder(500);
			deleteBuilder.Append("DELETE FROM ").Append(tableInfo.TableName).Append(" WHERE ");
			int deleteLength = deleteBuilder.Length;
			DesignColumnInfo timeStampColumn = null;
			foreach (DesignColumnInfo column in tableInfo.Columns)
			{
				if (column.PrimaryKey || column.IsModifiedTimeColumn)
				{
					if (dataProperties.TryGetValue(column.Name, out DataEntityPropertyElement property))
						DbBuilderHelper.CreateAbstractProperty(property, column, true);
					else
					{
						property = new DataEntityPropertyElement(dataEntity);
						DbBuilderHelper.CreateAbstractProperty(property, column, false);
						dataProperties.Add(property);
					}

					CommandParameter parameter = new CommandParameter(staticCommand);
					string parameterName = staticCommand.CreateParameterName(column.Name);
					parameter.Name = column.Name;
					parameter.SourceColumn = column.Name;

					parameter.ParameterType = column.DbType;
					if (column.DbType == DbTypeEnum.Decimal)
					{
						parameter.Precision = column.Precision;
						parameter.Scale = (byte)column.Scale;
					}
					else { parameter.Size = column.Size; }
					parameter.Direction = ParameterDirection.Input;
					parameter.Nullable = column.Nullable;
					staticCommand.Parameters.Add(parameter);
					if (deleteLength == deleteBuilder.Length)
						deleteBuilder.AppendFormat("{0}={1}", column.Name, parameterName);
					else
						deleteBuilder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
				}
			}
			staticCommand.CommandText = deleteBuilder.ToString();
			#endregion

			if (timeStampColumn != null)
			{
				#region 检测更新时间戳
				CheckedCommandElement checkCommand = new CheckedCommandElement(staticCommand);
				checkCommand.Name = string.Concat("PK_", tableInfo.TableName);
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("SELECT 1 FROM {0} WHERE ", tableInfo.TableName);
				int length = builder.Length;
				if (tableInfo.PrimaryKey != null && tableInfo.PrimaryKey.Columns != null && tableInfo.PrimaryKey.Columns.Count > 0)
				{
					foreach (DesignColumnInfo column in tableInfo.PrimaryKey.Columns)
					{
						CommandParameter parameter = new CommandParameter(checkCommand);
						string parameterName = staticCommand.CreateParameterName(column.Name);
						column.CreateParameter(parameter);
						checkCommand.Parameters.Add(parameter);

						if (length == builder.Length)
							builder.AppendFormat("{0}={1}", column.Name, parameterName);
						else
							builder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
					}
					CommandParameter timestampParameter = new CommandParameter(checkCommand);
					timeStampColumn.CreateParameter(timestampParameter);
					checkCommand.Parameters.Add(timestampParameter);
					builder.AppendFormat(" AND {0}<>{1}", timeStampColumn.Name, checkCommand.CreateParameterName(timeStampColumn.Name));
				}
				else
				{
					CommandParameter timestampParameter = new CommandParameter(checkCommand);
					timeStampColumn.CreateParameter(timestampParameter);
					checkCommand.Parameters.Add(timestampParameter);
					builder.AppendFormat("{0}<>{1}", timeStampColumn.Name, checkCommand.CreateParameterName(timeStampColumn.Name));
				}
				checkCommand.ErrorCode = string.Join("_", new string[] { tableInfo.EntityName, timeStampColumn.PropertyName, "Exist" });
				checkCommand.PropertyName = timeStampColumn.PropertyName;
				checkCommand.CommandText = builder.ToString();
				staticCommand.CheckCommands.Add(checkCommand);
				#endregion
			}
			return staticCommand;
		}

		/// <summary>
		/// 根据数据库列信息，创建删除数据库命令及其参数
		/// </summary>
		/// <param name="sqlStruct">从配置文件总读取的命令结构信息</param>
		/// <param name="fileClass">配置文件信息</param>
		/// <param name="conType">需要生成命令结构的类型</param>
		internal static StaticCommandElement CreateDeleteSqlStruct(this DesignTableInfo tableInfo, StaticCommandElement staticCommand)
		{
			if (tableInfo.Columns == null || tableInfo.Columns.Count == 0) { return null; }
			if (staticCommand == null) { return staticCommand; }
			staticCommand.Kind = ConfigurationTypeEnum.Remove;
			staticCommand.Name = "Delete";
			staticCommand.Comment = string.Concat("删除", tableInfo.Description);
			staticCommand.Parameters.Clear();
			staticCommand.CheckCommands.Clear();
			staticCommand.NewCommands.Clear();

			#region 构建 DELETE SQL
			StringBuilder deleteBuilder = new StringBuilder(500);
			deleteBuilder.Append("DELETE FROM ").Append(tableInfo.TableName).Append(" WHERE ");
			int deleteLength = deleteBuilder.Length;
			DesignColumnInfo timeStampColumn = null;
			foreach (DesignColumnInfo column in tableInfo.Columns)
			{
				if (column.PrimaryKey || column.IsModifiedTimeColumn)
				{
					CommandParameter parameter = new CommandParameter(staticCommand);
					string parameterName = staticCommand.CreateParameterName(column.Name);
					parameter.Name = column.Name;
					parameter.SourceColumn = column.Name;

					parameter.ParameterType = column.DbType;
					if (column.DbType == DbTypeEnum.Decimal)
					{
						parameter.Precision = column.Precision;
						parameter.Scale = (byte)column.Scale;
					}
					else { parameter.Size = column.Size; }
					parameter.Direction = ParameterDirection.Input;
					parameter.Nullable = column.Nullable;
					staticCommand.Parameters.Add(parameter);
					if (deleteLength == deleteBuilder.Length)
						deleteBuilder.AppendFormat("{0}={1}", column.Name, parameterName);
					else
						deleteBuilder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
				}
			}
			staticCommand.CommandText = deleteBuilder.ToString();
			#endregion

			if (timeStampColumn != null)
			{
				#region 检测更新时间戳
				CheckedCommandElement checkCommand = new CheckedCommandElement(staticCommand)
				{
					Name = string.Concat("PK_", tableInfo.TableName)
				};
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("SELECT 1 FROM {0} WHERE ", tableInfo.TableName);
				int length = builder.Length;
				if (tableInfo.PrimaryKey != null && tableInfo.PrimaryKey.Columns != null && tableInfo.PrimaryKey.Columns.Count > 0)
				{
					foreach (DesignColumnInfo column in tableInfo.PrimaryKey.Columns)
					{
						CommandParameter parameter = new CommandParameter(checkCommand);
						string parameterName = staticCommand.CreateParameterName(column.Name);
						column.CreateParameter(parameter);
						checkCommand.Parameters.Add(parameter);

						if (length == builder.Length)
							builder.AppendFormat("{0}={1}", column.Name, parameterName);
						else
							builder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
					}
					CommandParameter timestampParameter = new CommandParameter(checkCommand);
					timeStampColumn.CreateParameter(timestampParameter);
					checkCommand.Parameters.Add(timestampParameter);
					builder.AppendFormat(" AND {0}<>{1}", timeStampColumn.Name, checkCommand.CreateParameterName(timeStampColumn.Name));
				}
				else
				{
					CommandParameter timestampParameter = new CommandParameter(checkCommand);
					timeStampColumn.CreateParameter(timestampParameter);
					checkCommand.Parameters.Add(timestampParameter);
					builder.AppendFormat("{0}<>{1}", timeStampColumn.Name, checkCommand.CreateParameterName(timeStampColumn.Name));
				}
				checkCommand.ErrorCode = string.Join("_", new string[] { tableInfo.EntityName, timeStampColumn.PropertyName, "Exist" });
				checkCommand.PropertyName = timeStampColumn.PropertyName;
				checkCommand.CommandText = builder.ToString();
				staticCommand.CheckCommands.Add(checkCommand);
				#endregion
			}
			return staticCommand;
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="sqlStruct">从配置文件总读取的命令结构信息</param>
		/// <param name="tableColumns">数据库列信息</param>
		/// <param name="viewName">表对应的视图名称</param>
		/// <param name="conType">需要生成命令结构的类型</param>
		internal static DynamicCommandElement CreateSearchSqlStruct(this DesignTableInfo tableInfo, DataEntityElement dataEntity, DynamicCommandElement dynamicCommand)
		{
			if (tableInfo.Columns == null || tableInfo.Columns.Count == 0) { return null; }
			if (dynamicCommand == null) { dynamicCommand = new DynamicCommandElement(dataEntity); }
			dynamicCommand.Kind = ConfigurationTypeEnum.SearchTable;
			dynamicCommand.Name = dynamicCommand.Kind.ToString();
			dynamicCommand.Comment = string.Concat("查询", tableInfo.Description);
			dynamicCommand.Parameters.Clear();

			dataEntity.Comment = tableInfo.Description;
			if (string.IsNullOrWhiteSpace(dataEntity.Name))
				dataEntity.Name = tableInfo.EntityName;
			dataEntity.TableName = tableInfo.TableName;
			DataEntityPropertyCollection dataProperties = dataEntity.Properties;

			DataConditionElement dataCondition = dataEntity.Condition;
			dataCondition.Comment = tableInfo.Description;
			dataCondition.TableName = tableInfo.TableName;
			DataConditionPropertyCollection conditionProperties = dataCondition.Arguments;
			bool conditionPropertyIsExists = conditionProperties.Count == 0;
			StringBuilder selectBuilder = new StringBuilder(600);
			StringBuilder orderBuilder = new StringBuilder(100);
			int indexColumn = 0; 
			foreach (DesignColumnInfo column in tableInfo.Columns)
			{
				string pn = StringHelper.GetCsName(column.Name);
				if (dataProperties.TryGetValue(column.Name, out DataEntityPropertyElement property))
				{
					column.CreateProperty(property, true);
				}
				else
				{
					property = new DataEntityPropertyElement(dataEntity);
					column.CreateProperty(property, false);
					property.Profix = "T1";
					dataProperties.Add(property);
				}

				if (conditionPropertyIsExists)
				{
					if (conditionProperties.TryGetValue(column.Name, out DataConditionPropertyElement conditionProperty))
						column.CreateProperty(conditionProperty, true);
					else
					{
						conditionProperty = new DataConditionPropertyElement(dataCondition);
						column.CreateProperty(conditionProperty, false);
						conditionProperties.Add(conditionProperty);
					}
				}
				if (column.PrimaryKey && orderBuilder.Length == 0)
					orderBuilder.Append("T1.").Append(column.Name);
				else if (column.PrimaryKey)
					orderBuilder.Append(",T1.").Append(column.Name);

				if (indexColumn == 0)
					selectBuilder.AppendFormat("T1.{0}", column.Name);
				else
					selectBuilder.AppendFormat(",T1.{0}", column.Name);
				indexColumn++;
			}
			dynamicCommand.SelectText = selectBuilder.ToString();
			dynamicCommand.FromText = string.Concat(tableInfo.TableName, " T1");
			dynamicCommand.OrderText = orderBuilder.ToString();
			return dynamicCommand;
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="tableInfo">从配置文件总读取的命令结构信息</param>
		/// <param name="dynamicCommand">数据库列信息</param>
		internal static DynamicCommandElement CreateSelectAllSqlStruct(this DesignTableInfo tableInfo, DynamicCommandElement dynamicCommand)
		{
			if (tableInfo.Columns == null || tableInfo.Columns.Count == 0) { return null; }
			if (dynamicCommand == null) { return dynamicCommand; }
			dynamicCommand.Kind = ConfigurationTypeEnum.SearchTable;
			dynamicCommand.Name = dynamicCommand.Kind.ToString();
			dynamicCommand.Comment = string.Concat("查询", tableInfo.Description);
			dynamicCommand.Parameters.Clear();

			StringBuilder selectBuilder = new StringBuilder(600);
			StringBuilder orderBuilder = new StringBuilder(100);
			int indexColumn = 0;
			foreach (DesignColumnInfo column in tableInfo.Columns)
			{
				if (column.PrimaryKey && orderBuilder.Length == 0)
					orderBuilder.Append("T1.").Append(column.Name);
				else if (column.PrimaryKey)
					orderBuilder.Append(",T1.").Append(column.Name);
				if (indexColumn == 0)
					selectBuilder.AppendFormat("T1.{0}", column.Name);
				else
					selectBuilder.AppendFormat(",T1.{0}", column.Name);
				indexColumn++;
			}
			dynamicCommand.SelectText = selectBuilder.ToString();
			dynamicCommand.FromText = string.Concat(tableInfo.TableName, " T1");
			dynamicCommand.OrderText = orderBuilder.ToString();
			return dynamicCommand;
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="sqlStruct">从配置文件总读取的命令结构信息</param>
		/// <param name="tableColumns">数据库列信息</param>
		/// <param name="viewName">表对应的视图名称</param>
		/// <param name="conType">需要生成命令结构的类型</param>
		internal static StaticCommandElement CreateSelectByPKeySqlStruct(this DesignTableInfo tableInfo, StaticCommandElement staticCommand)
		{
			if (staticCommand == null) { return staticCommand; }
			staticCommand.Kind = ConfigurationTypeEnum.SelectByKey;
			staticCommand.Name = staticCommand.Kind.ToString();
			staticCommand.Parameters.Clear();
			staticCommand.CheckCommands.Clear();
			staticCommand.NewCommands.Clear();
			StringBuilder selectBuilder = new StringBuilder("SELECT ", 1000);
			int indexColumn = 0;
			StringBuilder whereBuilder = new StringBuilder("WHERE ", 500);
			int whereLength = whereBuilder.Length;
			foreach (DesignColumnInfo column in tableInfo.Columns)
			{
				if (indexColumn == 0)
					selectBuilder.AppendFormat("{0}", column.Name);
				else
					selectBuilder.AppendFormat(",{0}", column.Name);
				if (column.PrimaryKey)
				{
					CommandParameter parameter = new CommandParameter(staticCommand);
					string parameterName = staticCommand.CreateParameterName(column.Name);
					parameter.Name = column.Name;
					parameter.SourceColumn = column.Name;
					parameter.ParameterType = column.DbType;
					if (column.DbType == DbTypeEnum.Decimal)
					{
						parameter.Precision = column.Precision;
						parameter.Scale = (byte)column.Scale;
					}
					else { parameter.Size = column.Size; }
					parameter.Direction = ParameterDirection.Input;
					parameter.Nullable = column.Nullable;
					staticCommand.Parameters.Add(parameter);
					if (whereLength == whereBuilder.Length)
						whereBuilder.AppendFormat("{0}={1}", column.Name, parameterName);
					else
						whereBuilder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
				}
				indexColumn++;
			}
			selectBuilder.AppendLine();
			selectBuilder.AppendFormat("FROM {0}", tableInfo.TableName);
			selectBuilder.AppendLine();
			selectBuilder.Append(whereBuilder.ToString());
			staticCommand.CommandText = selectBuilder.ToString();
			return staticCommand;
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="tableInfo">数据库表结构信息</param>
		/// <param name="dataEntity">需要刷新的实体类信息</param>
		/// <param name="kind">当前实体类用途</param>
		internal static void CreateDataEntityElement(this DesignTableInfo tableInfo, DataEntityElement dataEntity, ConfigurationTypeEnum kind)
		{
			if (string.IsNullOrWhiteSpace(dataEntity.Name))
				dataEntity.Name = tableInfo.EntityName;
			dataEntity.TableName = tableInfo.TableName;
			dataEntity.Comment = tableInfo.Description;
			DataEntityPropertyCollection dataProperties = dataEntity.Properties;
			foreach (DesignColumnInfo column in tableInfo.Columns)
			{
				string pn = StringHelper.GetCsName(column.Name);
				DataEntityPropertyElement property;
				if (kind == ConfigurationTypeEnum.AddNew && column.IsCreatedTimeColumn)
				{
					if (dataProperties.ContainsKey(pn))
						dataProperties.Remove(dataProperties[pn]);
					continue;
				}
				else if (kind == ConfigurationTypeEnum.AddNew && column.IsCreatedTimeColumn)
				{
					if (dataProperties.ContainsKey(pn))
						dataProperties.Remove(dataProperties[pn]);
					continue;
				}
				else if (kind == ConfigurationTypeEnum.AddNew && column.DbType == DbTypeEnum.Timestamp)
				{
					if (dataProperties.ContainsKey(pn))
						dataProperties.Remove(dataProperties[pn]);
					continue;
				}
				else if (kind == ConfigurationTypeEnum.Modify && column.IsCreatedTimeColumn)
				{
					if (dataProperties.ContainsKey(pn))
						dataProperties.Remove(dataProperties[pn]);
					continue;
				}
				else if (kind == ConfigurationTypeEnum.Modify && column.IsCreatedTimeColumn)
				{
					if (dataProperties.ContainsKey(pn))
						dataProperties.Remove(dataProperties[pn]);
					continue;
				}
				else if (kind == ConfigurationTypeEnum.Remove && column.PrimaryKey)
				{
					if (dataProperties.TryGetValue(column.Name, out property))
						column.CreateProperty(property, false);
					else
					{
						property = new DataEntityPropertyElement(dataEntity);
						column.CreateProperty(property, false);
						dataProperties.Add(property);
					}
					continue;
				}
				else if (kind == ConfigurationTypeEnum.Remove && column.DbType == DbTypeEnum.Timestamp)
				{
					if (dataProperties.TryGetValue(column.Name, out property))
						column.CreateProperty(property, false);
					else
					{
						property = new DataEntityPropertyElement(dataEntity);
						column.CreateProperty(property, false);
						dataProperties.Add(property);
					}
					continue;
				}
				else if (kind == ConfigurationTypeEnum.Remove)
				{
					if (dataProperties.ContainsKey(pn))
						dataProperties.Remove(dataProperties[pn]);
					continue;
				}
				else
				{
					if (dataProperties.TryGetValue(column.Name, out property))
						column.CreateProperty(property, false);
					else
					{
						property = new DataEntityPropertyElement(dataEntity);
						column.CreateProperty(property, false);
						dataProperties.Add(property);
					}
				}
			}
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="tableInfo">数据库表结构信息</param>
		/// <param name="dataCondition">需要刷新的条件类信息</param>
		internal static void CreateDataConditionElement(this DesignTableInfo tableInfo, DataConditionElement dataCondition)
		{
			dataCondition.Comment = tableInfo.Description;
			dataCondition.TableName = tableInfo.TableName;
			DataConditionPropertyCollection conditionProperties = dataCondition.Arguments;
			DataConditionPropertyElement conditionProperty;
			foreach (DesignColumnInfo column in tableInfo.Columns)
			{
				if (column.DbType != DbTypeEnum.Date && column.DbType != DbTypeEnum.DateTime &&
					column.DbType != DbTypeEnum.DateTime2 && column.DbType != DbTypeEnum.DateTimeOffset)
				{
					if (conditionProperties.TryGetValue(column.Name, out conditionProperty))
					{
						column.CreateProperty(conditionProperty);
					}
					else
					{
						conditionProperty = new DataConditionPropertyElement(dataCondition);
						column.CreateProperty(conditionProperty);
						conditionProperties.Add(conditionProperty);
					}
				}
				else
				{
					string columnName = column.Name;
					string propertyName = column.PropertyName;
					string dtColumn1 = string.Concat(column.Name, "1");
					string dtColumn2 = string.Concat(column.Name, "2");
					if (conditionProperties.TryGetValue(dtColumn1, out conditionProperty))
					{
						column.Name = dtColumn1;
						column.PropertyName = string.Concat(column.PropertyName, "1");
						column.CreateProperty(conditionProperty);
						column.Name = columnName;
						column.PropertyName = propertyName;
					}
					else
					{
						conditionProperty = new DataConditionPropertyElement(dataCondition);
						column.Name = dtColumn1;
						column.PropertyName = string.Concat(column.PropertyName, "1");
						column.CreateProperty(conditionProperty);
						column.Name = columnName;
						column.PropertyName = propertyName;
						conditionProperties.Add(conditionProperty);
					}

					if (conditionProperties.TryGetValue(dtColumn2, out conditionProperty))
					{
						column.Name = dtColumn2;
						column.PropertyName = string.Concat(column.PropertyName, "2");
						column.CreateProperty(conditionProperty);
						column.Name = columnName;
						column.PropertyName = propertyName;
					}
					else
					{
						conditionProperty = new DataConditionPropertyElement(dataCondition);
						column.Name = dtColumn2;
						column.PropertyName = string.Concat(column.PropertyName, "2");
						column.CreateProperty(conditionProperty);
						column.Name = columnName;
						column.PropertyName = propertyName;
						conditionProperties.Add(conditionProperty);
					}
				}
			}
		}
	}
}
