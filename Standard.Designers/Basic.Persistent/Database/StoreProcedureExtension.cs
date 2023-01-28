using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Configuration;
using Basic.Enums;
using System.Data;
using Basic.DataEntities;
using Basic.Collections;
using Basic.Designer;
using Basic.EntityLayer;

namespace Basic.Database
{
	internal static class StoreProcedureExtension
	{
		/// <summary>
		/// 创建存储过程命令命令
		/// </summary>
		/// <param name="staticCommand"></param>
		/// <returns></returns>
		public static StaticCommandElement CreateCommand(this StoreProcedure procedure, StaticCommandElement staticCommand)
		{
			staticCommand.Parameters.Clear();
			staticCommand.CheckCommands.Clear();
			staticCommand.NewCommands.Clear();
			staticCommand.Kind = ConfigurationTypeEnum.Other;
			staticCommand.Name = procedure.EntityName;
			staticCommand.CommandText = procedure.Name;
			staticCommand.ExecutableMethod = StaticMethodEnum.GetPagination;
			staticCommand.CommandType = CommandType.StoredProcedure;
			if (procedure.Parameters != null && procedure.Parameters.Length > 0)
			{
				DataConditionElement dataCondition = staticCommand.EntityElement.Condition;
				DataConditionPropertyCollection conditionProperties = dataCondition.Arguments;
				conditionProperties.Clear();
				foreach (ProcedureParameter column in procedure.Parameters)
				{
					string pn = StringHelper.GetPascalCase(column.Name);
					string columnName = column.Name.ToUpper();

					DataConditionPropertyElement property = new DataConditionPropertyElement(dataCondition);
					DbBuilderHelper.CreateAbstractProperty(property, column);
					conditionProperties.Add(property);

					CommandParameter parameter = new CommandParameter(staticCommand);
					parameter.Name = column.Name;
					parameter.SourceColumn = column.Name;
					parameter.ParameterType = column.DbType;
					if (column.DbType == DbTypeEnum.Decimal)
					{
						parameter.Precision = column.Precision;
						parameter.Scale = (byte)column.Scale;
					}
					else
					{
						parameter.Size = column.Size;
					}
					parameter.Direction = ParameterDirection.Input;
					parameter.Nullable = column.Nullable;
					staticCommand.Parameters.Add(parameter);
				}
			}
			return staticCommand;
		}

		/// <summary>
		/// 创建存储过程命令命令
		/// </summary>
		/// <param name="staticCommand"></param>
		/// <returns></returns>
		public static DataEntityElement CreateEntity(this StoreProcedure procedure, DataEntityElement dataEntity)
		{
			if (procedure.Columns == null || procedure.Columns.Length == 0)
				return null;
			dataEntity.Name = StringHelper.GetPascalCase(procedure.EntityName);
			DataEntityPropertyCollection dataProperties = dataEntity.Properties;

			foreach (DesignColumnInfo column in procedure.Columns)
			{
				//string pn = StringHelper.GetCsName(column.Name);
				if (!dataProperties.TryGetValue(column.Name, out DataEntityPropertyElement property))
				{
					property = new DataEntityPropertyElement(dataEntity);
					DbBuilderHelper.CreateAbstractProperty(property, column, false);
					dataProperties.Add(property);
				}
				else
				{
					DbBuilderHelper.CreateAbstractProperty(property, column, false);
				}
			}
			return dataEntity;
		}

	}
}
