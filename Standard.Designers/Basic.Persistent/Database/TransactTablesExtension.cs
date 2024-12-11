using Basic.Collections;
using Basic.Configuration;
using Basic.DataContexts;
using Basic.DataEntities;
using Basic.Designer;
using Basic.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Basic.Database
{
	/// <summary>
	/// 扩展 TransactTableCollection 类方法
	/// </summary>
	internal static class TransactTablesExtension
	{
		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="tableCollection">数据库表结构信息</param>
		/// <param name="dataEntity">需要刷新的实体类信息</param>
		/// <param name="kind">当前实体类用途</param>
		internal static void CreateDataEntityElement(this TransactSqlResult tableCollection, DataEntityElement dataEntity)
		{
			if (string.IsNullOrWhiteSpace(dataEntity.Name))
				dataEntity.Name = tableCollection.EntityName;
			dataEntity.TableName = tableCollection.TableName;
			dataEntity.Comment = tableCollection.Description;
			DataEntityPropertyCollection dataProperties = dataEntity.Properties;
			foreach (TransactColumnInfo column in tableCollection.Columns)
			{
				string pn = column.PropertyName;
				if (dataEntity.NamingRule == NamingRules.PascalCase) { pn = StringHelper.GetPascalCase(column.Name); }
				else if (dataEntity.NamingRule == NamingRules.CamelCase) { pn = StringHelper.GetCamelCase(column.Name); }
				else if (dataEntity.NamingRule == NamingRules.UpperCase) { pn = StringHelper.GetUpperCase(column.Name); }
				else if (dataEntity.NamingRule == NamingRules.LowerCase) { pn = StringHelper.GetLowerCase(column.Name); }

				if (dataProperties.TryGetValue(pn, out DataEntityPropertyElement property)) { column.CreateProperty(property, false); }
				else
				{
					property = new DataEntityPropertyElement(dataEntity);
					column.CreateProperty(property, false);
					dataProperties.Add(property);
				}
			}
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		internal static void CreateProperty(this TransactColumnInfo column, DataEntityPropertyElement property)
		{
			column.CreateProperty(property, false);
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		internal static void CreateProperty(this TransactColumnInfo column, DataEntityPropertyElement property, bool updated)
		{
			NamingRules nr = property.Owner.NamingRule;
			if (updated == false && nr == NamingRules.DefaultCase && string.IsNullOrWhiteSpace(column.PropertyName) == false)
			{ property.Name = column.PropertyName; }
			else if (updated == false && nr == NamingRules.PascalCase)
			{ property.Name = StringHelper.GetPascalCase(column.Name); }
			else if (updated == false && nr == NamingRules.CamelCase)
			{ property.Name = StringHelper.GetCamelCase(column.Name); }
			else if (updated == false && nr == NamingRules.UpperCase)
			{ property.Name = StringHelper.GetUpperCase(column.Name); }
			else if (updated == false && nr == NamingRules.LowerCase)
			{ property.Name = StringHelper.GetLowerCase(column.Name); }

			if (property.Type != null)
				property.Type = StringHelper.DbTypeToNetType(column.DbType);
			property.PrimaryKey = column.PrimaryKey;
			property.Comment = column.Comment;

			property.Column = column.Name;
			property.Source = column.Source;
			property.DbType = column.DbType;
			property.Nullable = column.Nullable;
			property.Precision = column.Precision;
			property.Scale = column.Scale;
			property.Size = column.Size;
			property.Profix = column.Alias;
			property.DefaultValue = column.DefaultValue;
			property.Computed = column.Computed;
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="tableInfo">数据库表结构信息</param>
		/// <param name="dataCondition">需要刷新的条件类信息</param>
		internal static void CreateDataConditionElement(this TransactSqlResult tableCollection, DataConditionElement dataCondition)
		{
			dataCondition.TableName = tableCollection.TableName;
			dataCondition.Comment = tableCollection.Description;
			DataConditionPropertyCollection conditionProperties = dataCondition.Arguments;
			foreach (TransactColumnInfo column in tableCollection.Columns)
			{
				string columnName = column.Name.ToUpper();
				DataConditionPropertyElement conditionProperty = null;
				if (column.DbType != DbTypeEnum.Date && column.DbType != DbTypeEnum.DateTime &&
					 column.DbType != DbTypeEnum.DateTime2 && column.DbType != DbTypeEnum.DateTimeOffset)
				{
					if (conditionProperties.TryGetValue(columnName, out conditionProperty))
					{
						column.CreateProperty(conditionProperty, false);
					}
					else
					{
						conditionProperty = new DataConditionPropertyElement(dataCondition);
						column.CreateProperty(conditionProperty, false);
						conditionProperties.Add(conditionProperty);
					}
				}
				else
				{
					string propertyName = column.PropertyName;
					string dtColumn1 = string.Concat(columnName, "1");
					string dtColumn2 = string.Concat(columnName, "2");
					if (conditionProperties.TryGetValue(dtColumn1, out conditionProperty))
					{
						column.Name = dtColumn1;
						column.PropertyName = string.Concat(column.PropertyName, "1");
						column.CreateProperty(conditionProperty, false);
						column.Name = columnName;
						column.PropertyName = propertyName;
					}
					else
					{
						conditionProperty = new DataConditionPropertyElement(dataCondition);
						column.Name = dtColumn1;
						column.PropertyName = string.Concat(column.PropertyName, "1");
						column.CreateProperty(conditionProperty, false);
						column.Name = columnName;
						column.PropertyName = propertyName;
						conditionProperties.Add(conditionProperty);
					}

					if (conditionProperties.TryGetValue(dtColumn2, out conditionProperty))
					{
						column.Name = dtColumn2;
						column.PropertyName = string.Concat(column.PropertyName, "2");
						column.CreateProperty(conditionProperty, false);
						column.Name = columnName;
						column.PropertyName = propertyName;
					}
					else
					{
						conditionProperty = new DataConditionPropertyElement(dataCondition);
						column.Name = dtColumn2;
						column.PropertyName = string.Concat(column.PropertyName, "2");
						column.CreateProperty(conditionProperty, false);
						column.Name = columnName;
						column.PropertyName = propertyName;
						conditionProperties.Add(conditionProperty);
					}
				}
			}
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		internal static void CreateProperty(this TransactColumnInfo column, DataConditionPropertyElement property)
		{
			column.CreateProperty(property, false);
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		internal static void CreateProperty(this TransactColumnInfo column, DataConditionPropertyElement property, bool updated)
		{
			if (!updated && !string.IsNullOrWhiteSpace(column.PropertyName))
				property.Name = column.PropertyName;
			else if (!updated && string.IsNullOrWhiteSpace(column.PropertyName))
				property.Name = StringHelper.GetPascalCase(column.Name);
			if (property.Type != null)
				property.Type = StringHelper.DbTypeToNetType(column.DbType);
			property.PrimaryKey = column.PrimaryKey;
			property.Comment = column.Comment;

			property.Column = column.Name;
			property.Source = column.Source;
			property.DbType = column.DbType;
			property.Nullable = column.Nullable;
			property.Precision = column.Precision;
			property.Scale = column.Scale;
			property.Size = column.Size;
			property.DefaultValue = column.DefaultValue;
			property.Computed = column.Computed;
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		public static void CreateParameter(this TransactParameterInfo column, CommandParameter parameter)
		{
			parameter.Name = column.Name;
			parameter.SourceColumn = column.Name;
			parameter.ParameterType = column.ParameterType;
			switch (column.ParameterType)
			{
				case DbTypeEnum.Decimal:
					parameter.Precision = column.Precision;
					parameter.Scale = (byte)column.Scale;
					parameter.Size = 0;
					break;
				case DbTypeEnum.NChar:
				case DbTypeEnum.NVarChar:
				case DbTypeEnum.Char:
				case DbTypeEnum.VarChar:
				case DbTypeEnum.Binary:
				case DbTypeEnum.VarBinary:
					parameter.Precision = 0;
					parameter.Scale = 0;
					parameter.Size = column.Size;
					break;
				default:
					parameter.Precision = 0;
					parameter.Scale = 0;
					parameter.Size = 0;
					break;
			}
			parameter.Direction = ParameterDirection.Input;
			parameter.Nullable = column.Nullable;
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="tableCollection">数据库表结构信息</param>
		/// <param name="dataCommand">需要刷新的条件类信息</param>
		internal static void CreateParameters(this TransactSqlResult tableCollection, DataCommandElement dataCommand)
		{
			foreach (TransactParameterInfo info in tableCollection.Parameters)
			{
				if (!dataCommand.Parameters.ContainsKey(info.Name))
				{
					CommandParameter parameter = new CommandParameter(dataCommand);
					info.CreateParameter(parameter);
					dataCommand.Parameters.Add(parameter);
				}
			}
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		private static void GetParameter(this TransactParameterInfo parameter, CommandParameter column)
		{
			parameter.Name = column.Name;
			parameter.SourceColumn = column.Name;
			parameter.ParameterType = column.ParameterType;
			switch (column.ParameterType)
			{
				case DbTypeEnum.Decimal:
					parameter.Precision = column.Precision;
					parameter.Scale = (byte)column.Scale;
					parameter.Size = 0;
					break;
				case DbTypeEnum.NChar:
				case DbTypeEnum.NVarChar:
				case DbTypeEnum.Char:
				case DbTypeEnum.VarChar:
				case DbTypeEnum.Binary:
				case DbTypeEnum.VarBinary:
					parameter.Precision = 0;
					parameter.Scale = 0;
					parameter.Size = column.Size;
					break;
				default:
					parameter.Precision = 0;
					parameter.Scale = 0;
					parameter.Size = 0;
					break;
			}
			parameter.Direction = ParameterDirection.Input;
			parameter.Nullable = column.Nullable;
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="result">数据库表结构信息</param>
		/// <param name="dataCommand">需要刷新的条件类信息</param>
		internal static void GetParameters(this TransactSqlResult result, IDataContext context, DataCommandElement dataCommand, string transactSql)
		{
			foreach (CommandParameter parameter in dataCommand.Parameters)
			{
				string parameterName = context.GetParameterName(parameter.Name);
				if (transactSql.Contains(parameterName))
				{
					TransactParameterInfo tpinfo = new TransactParameterInfo(result);
					tpinfo.GetParameter(parameter);
					result.Parameters.Add(tpinfo);
				}
			}
		}
	}
}
