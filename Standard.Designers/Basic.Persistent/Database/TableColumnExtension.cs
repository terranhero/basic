using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.EntityLayer;
using Basic.Designer;
using Basic.Configuration;
using System.Xml;
using Basic.Enums;
using Basic.DataEntities;
using System.Data;

namespace Basic.Database
{
	/// <summary>
	/// 数据库列信息扩展方法
	/// </summary>
	internal static class TableColumnExtension
	{
		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="parameter">需要创建的数据库参数。</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		internal static void CreateParameter(this DesignColumnInfo column, CommandParameter parameter)
		{
			string parameterName = parameter.Command.CreateParameterName(column.Name);
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
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		internal static void CreateProperty(this DesignColumnInfo column, DataConditionPropertyElement property)
		{
			column.CreateProperty(property, false);
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		internal static void CreateProperty(this DesignColumnInfo column, DataConditionPropertyElement property, bool updated)
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
			property.PrimaryKey = false;
			property.Comment = column.Comment;
			property.Column = column.Name;
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
		internal static void CreateProperty(this DesignColumnInfo column, DataEntityPropertyElement property)
		{
			column.CreateProperty(property, false);
		}

		/// <summary>
		/// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		/// </summary>
		/// <param name="column">数据库表结构信息</param>
		/// <param name="property">需要刷新的实体类信息</param>
		/// <param name="updated">当前是表示创建，还是更新，默认值为 false 表示需要创建当前字段的属性。</param>
		internal static void CreateProperty(this DesignColumnInfo column, DataEntityPropertyElement property, bool updated)
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
			property.DbType = column.DbType;
			property.Nullable = column.Nullable;
			property.Precision = column.Precision;
			property.Scale = column.Scale;
			property.Size = column.Size;
			property.DefaultValue = column.DefaultValue;
			property.Computed = column.Computed;
			if (property.DbType == DbTypeEnum.DateTime || property.DbType == DbTypeEnum.DateTimeOffset ||
				property.DbType == DbTypeEnum.Date || property.DbType == DbTypeEnum.Time ||
				property.DbType == DbTypeEnum.Timestamp || property.DbType == DbTypeEnum.DateTime2)
			{
				AbstractAttribute abstractDisplay = null;
				if (!property.Attributes.TryGetValue(DisplayFormat.XmlElementName, out abstractDisplay))
				{
					abstractDisplay = new DisplayFormat(property);
					property.Attributes.Add(abstractDisplay);
				}
				DisplayFormat displayFormat = abstractDisplay as DisplayFormat;
				switch (property.DbType)
				{
					case DbTypeEnum.DateTime:
					case DbTypeEnum.DateTimeOffset:
						displayFormat.DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}";
						break;
					case DbTypeEnum.Date:
						displayFormat.DataFormatString = "{0:yyyy-MM-dd}";
						break;
					case DbTypeEnum.Time:
						displayFormat.DataFormatString = @"{0:hh\:mm\:ss}";
						break;
					case DbTypeEnum.Timestamp:
					case DbTypeEnum.DateTime2:
						displayFormat.DataFormatString = "{0:yyyy-MM-dd HH:mm:ss.fff}";
						break;
				}
			}
		}
	}
}
